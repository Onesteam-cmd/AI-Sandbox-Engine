namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeDispatchCompletionRoutingTests
{
    private readonly record struct RequestPayload(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;

    private readonly record struct CompletionPayload(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion;

    private record OpenCompletion(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion;

    private readonly record struct CancellationReason(string Code) :
        global::AI.Sandbox.Engine.Core.HostRuntime
            .IHostRuntimeCancellationReason;

    private sealed class CountingCompletion :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion
    {
        public int InvocationCount { get; private set; }

        public void Invoke() => InvocationCount++;
    }

    [Xunit.Fact]
    public void DispatchIdsAttemptAndStateAreValidated()
    {
        var pending = Pending();

        Xunit.Assert.Throws<ArgumentException>(
            () => Dispatch(default, RouteId(), EndpointId(), pending, 1));
        Xunit.Assert.Throws<ArgumentException>(
            () => Dispatch(DispatchId(), default, EndpointId(), pending, 1));
        Xunit.Assert.Throws<ArgumentException>(
            () => Dispatch(DispatchId(), RouteId(), default, pending, 1));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Dispatch(DispatchId(), RouteId(), EndpointId(), pending, 0));

        var failed = Finalize(
            pending,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Failed);
        Xunit.Assert.Throws<ArgumentException>(
            () => Dispatch(
                DispatchId(),
                RouteId(),
                EndpointId(),
                failed,
                1));
    }

    [Xunit.Fact]
    public void DispatchPreservesRoutingAndRequestAuthority()
    {
        var pending = Pending();
        var dispatch = Dispatch(
            DispatchId(),
            RouteId(),
            EndpointId(),
            pending,
            attemptNumber: 2);

        Xunit.Assert.Equal(DispatchId(), dispatch.DispatchId);
        Xunit.Assert.Equal(RouteId(), dispatch.RouteId);
        Xunit.Assert.Equal(EndpointId(), dispatch.EndpointId);
        Xunit.Assert.Same(pending, dispatch.Request);
        Xunit.Assert.Equal(2, dispatch.AttemptNumber);
        Xunit.Assert.Equal(pending.Revision, dispatch.ObservedRequestRevision);
        Xunit.Assert.Equal(pending.RequestId, dispatch.RequestId);
        Xunit.Assert.Equal(pending.CorrelationId, dispatch.CorrelationId);
    }

    [Xunit.Fact]
    public void CompletionIdsKindPayloadAndAttemptAreValidated()
    {
        var payload = new CompletionPayload("done");

        Xunit.Assert.Throws<ArgumentException>(
            () => Completion(
                default,
                RequestId(),
                RuntimeId(),
                OperationId(),
                CorrelationId(),
                RouteId(),
                EndpointId(),
                1,
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeCompletionKind.Completed,
                payload));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Completion(
                DispatchId(),
                RequestId(),
                RuntimeId(),
                OperationId(),
                CorrelationId(),
                RouteId(),
                EndpointId(),
                0,
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeCompletionKind.Completed,
                payload));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Completion(
                DispatchId(),
                RequestId(),
                RuntimeId(),
                OperationId(),
                CorrelationId(),
                RouteId(),
                EndpointId(),
                1,
                (global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeCompletionKind)999,
                payload));
        Xunit.Assert.Throws<ArgumentException>(
            () => Completion(
                DispatchId(),
                RequestId(),
                RuntimeId(),
                OperationId(),
                CorrelationId(),
                RouteId(),
                EndpointId(),
                1,
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeCompletionKind.Completed,
                new OpenCompletion("invalid")));
    }

    [Xunit.Fact]
    public void SuccessfulCompletionRoutesAndFinalizesRequest()
    {
        var pending = Pending();
        var dispatch = DispatchFor(pending);
        var completion = CompletionFor(
            dispatch,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Completed,
            new CompletionPayload("ok"));

        var result = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompletionRouter.Route(
                dispatch,
                pending,
                pending.Revision,
                completion);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionRoutingStatus.Routed,
            result.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Completed,
            result.Request.State);
        Xunit.Assert.Equal(1, result.Request.Revision);
        Xunit.Assert.Same(completion, result.Completion);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Pending,
            pending.State);
    }

    [Xunit.Fact]
    public void CancellationRequestedRequestAcceptsCancelledCompletion()
    {
        var pending = Pending();
        var dispatch = DispatchFor(pending);
        var cancellation = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.RequestCancellation(
                pending,
                pending.Revision,
                new CancellationReason("user"));
        var completion = CompletionFor(
            dispatch,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Cancelled,
            new CompletionPayload("cancelled"));

        var result = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompletionRouter.Route(
                dispatch,
                cancellation.Envelope,
                cancellation.Envelope.Revision,
                completion);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Cancelled,
            result.Request.State);
        Xunit.Assert.NotNull(result.Request.CancellationReason);
        Xunit.Assert.Equal(2, result.Request.Revision);
    }

    [Xunit.Fact]
    public void CompletionIdentityMismatchIsExplicitAndUnchanged()
    {
        var pending = Pending();
        var dispatch = DispatchFor(pending);
        var completion = Completion(
            OtherDispatchId(),
            pending.RequestId,
            pending.RuntimeInstanceId,
            pending.OperationId,
            pending.CorrelationId,
            dispatch.RouteId,
            dispatch.EndpointId,
            dispatch.AttemptNumber,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Completed,
            new CompletionPayload("mismatch"));

        var result = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompletionRouter.Route(
                dispatch,
                pending,
                pending.Revision,
                completion);

        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionRoutingStatus.CompletionMismatch,
            result.Status);
        Xunit.Assert.Same(pending, result.Request);
    }

    [Xunit.Fact]
    public void StaleTerminalAndDispatchMismatchAreExplicit()
    {
        var pending = Pending();
        var dispatch = DispatchFor(pending);
        var completion = CompletionFor(
            dispatch,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Failed,
            new CompletionPayload("failed"));

        var stale = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompletionRouter.Route(
                dispatch,
                pending,
                pending.Revision + 1,
                completion);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionRoutingStatus.StaleRevision,
            stale.Status);

        var terminal = Finalize(
            pending,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Completed);
        var terminalResult = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompletionRouter.Route(
                dispatch,
                terminal,
                terminal.Revision,
                completion);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionRoutingStatus.InvalidRequestState,
            terminalResult.Status);

        var otherPending = Pending(3309);
        var mismatchedDispatch = DispatchFor(otherPending);
        var mismatch = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompletionRouter.Route(
                mismatchedDispatch,
                pending,
                pending.Revision,
                CompletionFor(
                    mismatchedDispatch,
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeCompletionKind.Completed,
                    new CompletionPayload("other")));
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionRoutingStatus.DispatchMismatch,
            mismatch.Status);
        Xunit.Assert.Same(pending, mismatch.Request);
    }

    [Xunit.Fact]
    public void ContractsDoNotDispatchTransportOrExecuteCompletion()
    {
        var pending = Pending();
        var dispatch = DispatchFor(pending);
        var payload = new CountingCompletion();
        var completion = CompletionFor(
            dispatch,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Completed,
            payload);

        var result = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompletionRouter.Route(
                dispatch,
                pending,
                pending.Revision,
                completion);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(0, payload.InvocationCount);
        Xunit.Assert.Equal(pending.Payload, dispatch.Request.Payload);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeDispatchEnvelope<RequestPayload> DispatchFor(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestEnvelope<RequestPayload> request) =>
        Dispatch(
            DispatchId(),
            RouteId(),
            EndpointId(),
            request,
            attemptNumber: 1);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeDispatchEnvelope<RequestPayload> Dispatch(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeDispatchIdKind> dispatchId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeRouteIdKind> routeId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeEndpointIdKind> endpointId,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestEnvelope<RequestPayload> request,
            int attemptNumber) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDispatchFlow.Create(
                dispatchId,
                routeId,
                endpointId,
                request,
                attemptNumber);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeCompletionEnvelope<TCompletion>
        CompletionFor<TCompletion>(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchEnvelope<RequestPayload> dispatch,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind kind,
            TCompletion payload)
        where TCompletion :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion =>
        Completion(
            dispatch.DispatchId,
            dispatch.RequestId,
            dispatch.RuntimeInstanceId,
            dispatch.OperationId,
            dispatch.CorrelationId,
            dispatch.RouteId,
            dispatch.EndpointId,
            dispatch.AttemptNumber,
            kind,
            payload);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeCompletionEnvelope<TCompletion>
        Completion<TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeDispatchIdKind> dispatchId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeRequestIdKind> requestId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeInstanceIdKind> runtimeId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeOperationIdKind> operationId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeCorrelationIdKind> correlationId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeRouteIdKind> routeId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeEndpointIdKind> endpointId,
            int attemptNumber,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind kind,
            TCompletion payload)
        where TCompletion :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompletionFlow.Create(
                dispatchId,
                requestId,
                runtimeId,
                operationId,
                correlationId,
                routeId,
                endpointId,
                attemptNumber,
                kind,
                payload);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRequestEnvelope<RequestPayload> Finalize(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestEnvelope<RequestPayload> request,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState state) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.Finalize(
                request,
                request.Revision,
                state).Envelope;

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRequestEnvelope<RequestPayload> Pending(
            int requestSuffix = 3301) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.Create(
                Id<
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRequestIdKind>(requestSuffix),
                RuntimeId(),
                OperationId(),
                CorrelationId(),
                default,
                new RequestPayload("payload"));

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>
        Id<TKind>(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(
            $"019d0000-0000-7000-8000-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestIdKind>
        RequestId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestIdKind>(3301);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeInstanceIdKind>
        RuntimeId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInstanceIdKind>(3302);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeOperationIdKind>
        OperationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeOperationIdKind>(3303);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeCorrelationIdKind>
        CorrelationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCorrelationIdKind>(3304);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeDispatchIdKind>
        DispatchId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchIdKind>(3305);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeDispatchIdKind>
        OtherDispatchId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchIdKind>(3306);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRouteIdKind>
        RouteId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRouteIdKind>(3307);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeEndpointIdKind>
        EndpointId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeEndpointIdKind>(3308);
}
