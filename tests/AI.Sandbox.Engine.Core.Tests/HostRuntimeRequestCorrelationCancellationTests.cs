namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRequestCorrelationCancellationTests
{
    private readonly record struct ValueRequest(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;

    private sealed record SealedRequest(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;

    private record OpenRequest(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;

    private readonly record struct ValueReason(string Code) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCancellationReason;

    private record OpenReason(string Code) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCancellationReason;

    private sealed class CountingRequest :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
    {
        public int InvocationCount { get; private set; }

        public void Invoke() => InvocationCount++;
    }

    private sealed class CountingReason :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCancellationReason
    {
        public int InvocationCount { get; private set; }

        public void Invoke() => InvocationCount++;
    }

    [Xunit.Fact]
    public void IdsPayloadAndParentAreValidated()
    {
        Xunit.Assert.Throws<ArgumentException>(
            () => Create(default, RuntimeId(), OperationId(), CorrelationId(), default, new ValueRequest("x")));
        Xunit.Assert.Throws<ArgumentException>(
            () => Create(RequestId(), default, OperationId(), CorrelationId(), default, new ValueRequest("x")));
        Xunit.Assert.Throws<ArgumentException>(
            () => Create(RequestId(), RuntimeId(), default, CorrelationId(), default, new ValueRequest("x")));
        Xunit.Assert.Throws<ArgumentException>(
            () => Create(RequestId(), RuntimeId(), OperationId(), default, default, new ValueRequest("x")));
        Xunit.Assert.Throws<ArgumentException>(
            () => Create(RequestId(), RuntimeId(), OperationId(), CorrelationId(), RequestId(), new ValueRequest("x")));
        Xunit.Assert.Throws<ArgumentException>(
            () => Create(RequestId(), RuntimeId(), OperationId(), CorrelationId(), default, new OpenRequest("x")));
    }

    [Xunit.Fact]
    public void CreationPreservesCorrelationAndOptionalParent()
    {
        var payload = new SealedRequest("payload");
        var envelope = Create(
            RequestId(),
            RuntimeId(),
            OperationId(),
            CorrelationId(),
            ParentRequestId(),
            payload);

        Xunit.Assert.Equal(RequestId(), envelope.RequestId);
        Xunit.Assert.Equal(RuntimeId(), envelope.RuntimeInstanceId);
        Xunit.Assert.Equal(OperationId(), envelope.OperationId);
        Xunit.Assert.Equal(CorrelationId(), envelope.CorrelationId);
        Xunit.Assert.Equal(ParentRequestId(), envelope.ParentRequestId);
        Xunit.Assert.Same(payload, envelope.Payload);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestState.Pending,
            envelope.State);
        Xunit.Assert.Equal(0, envelope.Revision);
        Xunit.Assert.Null(envelope.CancellationReason);
        Xunit.Assert.False(envelope.IsTerminal);
    }

    [Xunit.Fact]
    public void CancellationRequestIsExplicitImmutableAndRevisioned()
    {
        var pending = Pending();
        var reason = new ValueReason("user_request");

        var result = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.RequestCancellation(
                pending,
                pending.Revision,
                reason);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.CancellationRequested,
            result.Envelope.State);
        Xunit.Assert.Equal(1, result.Envelope.Revision);
        Xunit.Assert.Equal(reason, result.Envelope.CancellationReason);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Pending,
            pending.State);
        Xunit.Assert.Equal(0, pending.Revision);
    }

    [Xunit.Fact]
    public void StaleAndRepeatedCancellationAreRejected()
    {
        var pending = Pending();
        var reason = new ValueReason("cancel");

        var stale = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.RequestCancellation(
                pending,
                expectedRevision: 1,
                reason);
        var first = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.RequestCancellation(
                pending,
                pending.Revision,
                reason);
        var repeated = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.RequestCancellation(
                first.Envelope,
                first.Envelope.Revision,
                reason);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestTransitionStatus.StaleRevision,
            stale.Status);
        Xunit.Assert.Same(pending, stale.Envelope);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestTransitionStatus.InvalidState,
            repeated.Status);
        Xunit.Assert.Same(first.Envelope, repeated.Envelope);
    }

    [Xunit.Fact]
    public void TerminalizationIsExplicitAndPreservesCancellationReason()
    {
        var pending = Pending();
        var cancellation = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.RequestCancellation(
                pending,
                pending.Revision,
                new ValueReason("cancel"));

        var finalized = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.Finalize(
                cancellation.Envelope,
                cancellation.Envelope.Revision,
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeRequestState.Cancelled);

        Xunit.Assert.True(finalized.Succeeded);
        Xunit.Assert.True(finalized.Envelope.IsTerminal);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Cancelled,
            finalized.Envelope.State);
        Xunit.Assert.NotNull(finalized.Envelope.CancellationReason);
        Xunit.Assert.Equal(2, finalized.Envelope.Revision);
    }

    [Xunit.Fact]
    public void InvalidTerminalTargetsAndTerminalRetransitionAreRejected()
    {
        var pending = Pending();

        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.Finalize(
                    pending,
                    pending.Revision,
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRequestState.Pending));

        var completed = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.Finalize(
                pending,
                pending.Revision,
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeRequestState.Completed);
        var repeated = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.Finalize(
                completed.Envelope,
                completed.Envelope.Revision,
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeRequestState.Failed);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestTransitionStatus.InvalidState,
            repeated.Status);
        Xunit.Assert.Same(completed.Envelope, repeated.Envelope);
    }

    [Xunit.Fact]
    public void CancellationReasonMustBeExact()
    {
        var pending = Pending();

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.RequestCancellation(
                    pending,
                    pending.Revision,
                    new OpenReason("invalid")));
    }

    [Xunit.Fact]
    public void ContractsDoNotExecuteRequestsOrCancellation()
    {
        var request = new CountingRequest();
        var reason = new CountingReason();
        var pending = Create(
            RequestId(),
            RuntimeId(),
            OperationId(),
            CorrelationId(),
            default,
            request);

        var cancellation = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.RequestCancellation(
                pending,
                pending.Revision,
                reason);
        var finalized = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.Finalize(
                cancellation.Envelope,
                cancellation.Envelope.Revision,
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeRequestState.Cancelled);

        Xunit.Assert.True(finalized.Succeeded);
        Xunit.Assert.Equal(0, request.InvocationCount);
        Xunit.Assert.Equal(0, reason.InvocationCount);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRequestEnvelope<TRequest> Create<TRequest>(
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
                    .HostRuntimeRequestIdKind> parentRequestId,
            TRequest payload)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.Create(
                requestId,
                runtimeId,
                operationId,
                correlationId,
                parentRequestId,
                payload);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRequestEnvelope<ValueRequest> Pending() =>
        Create(
            RequestId(),
            RuntimeId(),
            OperationId(),
            CorrelationId(),
            default,
            new ValueRequest("payload"));

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>
        Id<TKind>(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(
            $"019d0000-0000-7000-8000-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestIdKind>
        RequestId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestIdKind>(3101);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestIdKind>
        ParentRequestId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestIdKind>(3102);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeInstanceIdKind>
        RuntimeId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInstanceIdKind>(3103);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeOperationIdKind>
        OperationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeOperationIdKind>(3104);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeCorrelationIdKind>
        CorrelationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCorrelationIdKind>(3105);
}
