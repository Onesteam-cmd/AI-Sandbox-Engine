namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeAttemptSettlementTests
{
    private readonly record struct RequestPayload(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;

    private readonly record struct CompletionPayload(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion;

    private sealed class CountingRequest :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
    {
        public int InvocationCount { get; private set; }

        public void Invoke() => InvocationCount++;
    }

    private sealed class CountingCompletion :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion
    {
        public int InvocationCount { get; private set; }

        public void Invoke() => InvocationCount++;
    }

    [Xunit.Fact]
    public void IdsRevisionsTickAndArgumentsAreValidated()
    {
        var context = CreateContext();
        var completion = Completion(context);

        Xunit.Assert.Throws<ArgumentException>(
            () => Settle(
                context,
                completion,
                settlementId: default(
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeSettlementIdKind>)));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Settle(
                context,
                completion,
                expectedRequestRevision: -1));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Settle(
                context,
                completion,
                expectedLeaseRevision: -1));
        Xunit.Assert.Throws<ArgumentException>(
            () => Settle(
                context,
                completion,
                settlingWorkerId: default(
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeWorkerIdKind>)));
        Xunit.Assert.Throws<ArgumentException>(
            () => Settle(
                context,
                completion,
                clockId: default(
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeClockIdKind>)));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Settle(
                context,
                completion,
                settledTick: -1));
    }

    [Xunit.Fact]
    public void SettlementProducesTerminalRequestAndReleasedLease()
    {
        var context = CreateContext();
        var completion = Completion(context);

        var result = Settle(context, completion);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementStatus.Settled,
            result.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Completed,
            result.Request.State);
        Xunit.Assert.Equal(context.Request.Revision + 1, result.Request.Revision);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseState.Released,
            result.Lease.State);
        Xunit.Assert.Equal(context.Lease.Revision + 1, result.Lease.Revision);

        var settlement = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlement<
                    RequestPayload,
                    CompletionPayload>>(result.Settlement);
        Xunit.Assert.Equal(SettlementId(), settlement.SettlementId);
        Xunit.Assert.Same(context.Attempt, settlement.Attempt);
        Xunit.Assert.Same(completion, settlement.Completion);
        Xunit.Assert.Same(result.Request, settlement.Request);
        Xunit.Assert.Same(result.Lease, settlement.Lease);
        Xunit.Assert.Equal(30, settlement.SettledTick);
        Xunit.Assert.Equal(
            result.Request.Revision,
            settlement.ObservedRequestRevision);
        Xunit.Assert.Equal(
            result.Lease.Revision,
            settlement.ObservedLeaseRevision);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Completed,
            settlement.OutcomeKind);
        Xunit.Assert.Equal(context.Attempt.AttemptId, settlement.AttemptId);
        Xunit.Assert.Equal(context.Attempt.RequestId, settlement.RequestId);
        Xunit.Assert.Equal(context.Attempt.LeaseId, settlement.LeaseId);
        Xunit.Assert.Equal(context.Attempt.WorkerId, settlement.WorkerId);
        Xunit.Assert.Equal(context.Attempt.DispatchId, settlement.DispatchId);
        Xunit.Assert.Equal(context.Attempt.AttemptNumber, settlement.AttemptNumber);
        Xunit.Assert.Equal(context.Attempt.ClockId, settlement.ClockId);
    }

    [Xunit.Theory]
    [Xunit.InlineData(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompletionKind.Completed,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestState.Completed)]
    [Xunit.InlineData(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompletionKind.Rejected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestState.Rejected)]
    [Xunit.InlineData(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompletionKind.Failed,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestState.Failed)]
    [Xunit.InlineData(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompletionKind.Cancelled,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestState.Cancelled)]
    public void CompletionKindsProduceExactTerminalStates(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompletionKind completionKind,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestState expectedState)
    {
        var context = CreateContext();
        var completion = Completion(context, completionKind);

        var result = Settle(context, completion);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(expectedState, result.Request.State);
        Xunit.Assert.Equal(completionKind, result.Settlement!.OutcomeKind);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseState.Released,
            result.Lease.State);
    }

    [Xunit.Fact]
    public void StaleRevisionsAndAttemptAuthorityMismatchesAreExplicit()
    {
        var context = CreateContext();
        var completion = Completion(context);

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementStatus.StaleRequestRevision,
            Settle(
                context,
                completion,
                expectedRequestRevision: context.Request.Revision + 1));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementStatus.StaleLeaseRevision,
            Settle(
                context,
                completion,
                expectedLeaseRevision: context.Lease.Revision + 1));

        var otherRequest =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.Create(
                    OtherRequestId(),
                    RuntimeId(),
                    OperationId(),
                    CorrelationId(),
                    default,
                    new RequestPayload("other"));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementStatus.AttemptRequestMismatch,
            Settle(
                context,
                completion,
                request: otherRequest));

        var otherLease =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Acquire(
                    OtherLeaseId(),
                    context.Lease.WorkerId,
                    context.Admission,
                    context.Lease.ClockId,
                    acquiredTick: 10,
                    durationTicks: 100);
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementStatus.AttemptLeaseMismatch,
            Settle(
                context,
                completion,
                lease: otherLease));
    }

    [Xunit.Fact]
    public void RequestLeaseWorkerClockAndTimeBoundariesAreExplicit()
    {
        var context = CreateContext();
        var completion = Completion(context);
        var terminalRequest =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.Finalize(
                    context.Request,
                    context.Request.Revision,
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRequestState.Failed).Envelope;
        var releasedLease =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Release(
                    context.Lease,
                    context.Lease.Revision,
                    context.Lease.WorkerId).Lease;

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementStatus.InvalidRequestState,
            Settle(
                context,
                completion,
                request: terminalRequest));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementStatus.InvalidLeaseState,
            Settle(
                context,
                completion,
                lease: releasedLease));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementStatus.WorkerMismatch,
            Settle(
                context,
                completion,
                settlingWorkerId: OtherWorkerId()));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementStatus.ClockMismatch,
            Settle(
                context,
                completion,
                clockId: OtherClockId()));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementStatus.BeforeAcknowledgement,
            Settle(
                context,
                completion,
                settledTick: context.Attempt.AcknowledgedTick - 1));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementStatus.LeaseExpired,
            Settle(
                context,
                completion,
                settledTick: context.Lease.ExpiresTick));
    }

    [Xunit.Fact]
    public void CompletionIdentityMismatchIsExplicit()
    {
        var context = CreateContext();
        var completion = Completion(
            context,
            dispatchId: OtherDispatchId());

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementStatus.CompletionMismatch,
            Settle(context, completion));
    }

    [Xunit.Fact]
    public void RenewedActiveLeaseCanBeSettled()
    {
        var context = CreateContext();
        var renewed =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Renew(
                    context.Lease,
                    context.Lease.Revision,
                    context.Lease.ClockId,
                    observedTick: 25,
                    durationTicks: 100).Lease;
        var completion = Completion(context);

        var result = Settle(
            context,
            completion,
            lease: renewed,
            settledTick: 30);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(renewed.Revision + 1, result.Lease.Revision);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseState.Released,
            result.Lease.State);
        Xunit.Assert.Equal(
            renewed.Revision + 1,
            result.Settlement!.ObservedLeaseRevision);
    }

    [Xunit.Fact]
    public void ContractsDoNotExecuteOrMutateAuthority()
    {
        var requestPayload = new CountingRequest();
        var completionPayload = new CountingCompletion();
        var context = CreateContext(requestPayload);
        var completion = Completion(context, completionPayload);
        var originalRequest = context.Request;
        var originalLease = context.Lease;

        var result = Settle(context, completion);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(0, requestPayload.InvocationCount);
        Xunit.Assert.Equal(0, completionPayload.InvocationCount);
        Xunit.Assert.Same(originalRequest, context.Request);
        Xunit.Assert.Same(originalLease, context.Lease);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Pending,
            originalRequest.State);
        Xunit.Assert.True(originalLease.IsActive);
    }

    private static void AssertStatus<TRequest, TCompletion>(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeAttemptSettlementStatus expected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeAttemptSettlementResult<TRequest, TCompletion> result)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
        where TCompletion :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion
    {
        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(expected, result.Status);
        Xunit.Assert.Null(result.Settlement);
    }

    private static SettlementContext<RequestPayload> CreateContext() =>
        CreateContext(new RequestPayload("payload"));

    private static SettlementContext<TRequest> CreateContext<TRequest>(
        TRequest payload)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
    {
        var pending =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.Create(
                    RequestId(),
                    RuntimeId(),
                    OperationId(),
                    CorrelationId(),
                    default,
                    payload);
        var snapshot =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueSnapshot.Create(
                    QueueId(),
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeQueueCapacity.Create(8),
                    queuedCount: 0,
                    revision: 10);
        var priority =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimePriority.Create(
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimePriorityClass.Urgent,
                    sequence: 8);
        var admissionResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmissionFlow.Decide(
                    AdmissionId(),
                    snapshot,
                    snapshot.Revision,
                    pending,
                    priority);
        var admission = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmission<TRequest>>(
                    admissionResult.Admission);
        var lease =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Acquire(
                    LeaseId(),
                    WorkerId(),
                    admission,
                    ClockId(),
                    acquiredTick: 10,
                    durationTicks: 100);
        var selectionResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionFlow.Select(
                    SelectionId(),
                    admissionResult.Snapshot,
                    admissionResult.Snapshot.Revision,
                    lease,
                    ClockId(),
                    observedTick: 20,
                    DispatchId(),
                    RouteId(),
                    EndpointId(),
                    attemptNumber: 1);
        var selection = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelection<TRequest>>(
                    selectionResult.Selection);
        var acknowledgement =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementFlow.Acknowledge(
                    AttemptId(),
                    selection,
                    pending,
                    lease,
                    pending.Revision,
                    lease.Revision,
                    lease.LeaseId,
                    lease.WorkerId,
                    selection.Dispatch.DispatchId,
                    pending.RequestId,
                    selection.Dispatch.AttemptNumber,
                    lease.ClockId,
                    acknowledgedTick: 20);
        var attempt = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInFlightAttempt<TRequest>>(
                    acknowledgement.Attempt);

        return new SettlementContext<TRequest>(
            admission,
            pending,
            lease,
            attempt);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeCompletionEnvelope<CompletionPayload> Completion<TRequest>(
            SettlementContext<TRequest> context,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind kind =
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeCompletionKind.Completed,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeDispatchIdKind>? dispatchId = null)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest =>
        Completion(
            context,
            new CompletionPayload("done"),
            kind,
            dispatchId);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeCompletionEnvelope<TCompletion>
        Completion<TRequest, TCompletion>(
            SettlementContext<TRequest> context,
            TCompletion payload,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind kind =
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeCompletionKind.Completed,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeDispatchIdKind>? dispatchId = null)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
        where TCompletion :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompletionFlow.Create(
                dispatchId ?? context.Attempt.DispatchId,
                context.Attempt.RequestId,
                context.Attempt.Dispatch.RuntimeInstanceId,
                context.Attempt.Dispatch.OperationId,
                context.Attempt.Dispatch.CorrelationId,
                context.Attempt.Dispatch.RouteId,
                context.Attempt.Dispatch.EndpointId,
                context.Attempt.AttemptNumber,
                kind,
                payload);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeAttemptSettlementResult<TRequest, TCompletion>
        Settle<TRequest, TCompletion>(
            SettlementContext<TRequest> context,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionEnvelope<TCompletion> completion,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestEnvelope<TRequest>? request = null,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLease<TRequest>? lease = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeSettlementIdKind>? settlementId = null,
            long? expectedRequestRevision = null,
            long? expectedLeaseRevision = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeWorkerIdKind>? settlingWorkerId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeClockIdKind>? clockId = null,
            long? settledTick = null)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
        where TCompletion :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion
    {
        var currentRequest = request ?? context.Request;
        var currentLease = lease ?? context.Lease;

        return global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeAttemptSettlementFlow.Settle(
                settlementId ?? SettlementId(),
                context.Attempt,
                currentRequest,
                currentLease,
                expectedRequestRevision ?? currentRequest.Revision,
                expectedLeaseRevision ?? currentLease.Revision,
                settlingWorkerId ?? currentLease.WorkerId,
                clockId ?? currentLease.ClockId,
                settledTick ?? 30,
                completion);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>
        Id<TKind>(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(
            $"019d0000-0000-7000-8000-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeSettlementIdKind>
        SettlementId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeSettlementIdKind>(3801);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeAttemptIdKind>
        AttemptId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptIdKind>(3802);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDispatchSelectionIdKind>
        SelectionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionIdKind>(3803);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueIdKind>
        QueueId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueIdKind>(3804);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeAdmissionIdKind>
        AdmissionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAdmissionIdKind>(3805);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseIdKind>
        LeaseId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseIdKind>(3806);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseIdKind>
        OtherLeaseId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseIdKind>(3807);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkerIdKind>
        WorkerId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkerIdKind>(3808);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkerIdKind>
        OtherWorkerId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkerIdKind>(3809);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        ClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(3810);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        OtherClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(3811);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeDispatchIdKind>
        DispatchId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchIdKind>(3812);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeDispatchIdKind>
        OtherDispatchId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchIdKind>(3813);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRouteIdKind>
        RouteId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRouteIdKind>(3814);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeEndpointIdKind>
        EndpointId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeEndpointIdKind>(3815);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestIdKind>
        RequestId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestIdKind>(3816);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestIdKind>
        OtherRequestId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestIdKind>(3817);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeInstanceIdKind>
        RuntimeId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInstanceIdKind>(3818);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeOperationIdKind>
        OperationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeOperationIdKind>(3819);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCorrelationIdKind>
        CorrelationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCorrelationIdKind>(3820);

    private sealed record SettlementContext<TRequest>(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeQueueAdmission<TRequest> Admission,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestEnvelope<TRequest> Request,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeWorkLease<TRequest> Lease,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeInFlightAttempt<TRequest> Attempt)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;
}
