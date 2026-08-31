namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeDeadLetterDispositionTests
{
    private readonly record struct RequestPayload(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;

    private readonly record struct CompletionPayload(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion;

    private sealed record RetryReason(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRetryReason;

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

    private sealed class CountingRetryReason :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRetryReason
    {
        public int InvocationCount { get; private set; }

        public void Invoke() => InvocationCount++;
    }

    [Xunit.Fact]
    public void IdsTickAndArgumentsAreValidated()
    {
        var settlement = FailedSettlement();
        var decision = Decision(settlement);

        Xunit.Assert.Throws<ArgumentException>(
            () => Dispose(
                settlement,
                decision,
                dispositionId: default(
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeDeadLetterDispositionIdKind>)));
        Xunit.Assert.Throws<ArgumentException>(
            () => Dispose(
                settlement,
                decision,
                clockId: default(
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeClockIdKind>)));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Dispose(
                settlement,
                decision,
                disposedTick: -1));
    }

    [Xunit.Fact]
    public void AttemptLimitCreatesDeadLetterDisposition()
    {
        var settlement = FailedSettlement();
        var decision = Decision(
            settlement,
            maximumAttempts: 1,
            observedTick: 30);

        var result = Dispose(
            settlement,
            decision,
            disposedTick: 30);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDeadLetterDispositionStatus.Disposed,
            result.Status);

        var disposition = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDeadLetterDisposition<
                    RequestPayload,
                    CompletionPayload>>(
                        result.Disposition);
        Xunit.Assert.Equal(DispositionId(), disposition.DispositionId);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDeadLetterDispositionKind.AttemptLimitReached,
            disposition.Kind);
        Xunit.Assert.Same(settlement, disposition.Settlement);
        Xunit.Assert.Same(decision, disposition.RetryDecision);
        Xunit.Assert.Same(settlement.Request, disposition.Request);
        Xunit.Assert.Same(settlement.Completion, disposition.Completion);
        Xunit.Assert.Equal(30, disposition.DisposedTick);
        Xunit.Assert.Equal(
            settlement.ObservedRequestRevision,
            disposition.ObservedTerminalRequestRevision);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryDecisionStatus.AttemptLimitReached,
            disposition.RetryDecisionStatus);
    }

    [Xunit.Fact]
    public void DeadlineExceededCreatesDeadLetterDisposition()
    {
        var settlement = FailedSettlement();
        var deadline =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDeadline.Create(
                    ClockId(),
                    dueTick: 35);
        var decision = Decision(
            settlement,
            maximumAttempts: 3,
            observedTick: 35,
            deadline: deadline);

        var result = Dispose(
            settlement,
            decision,
            disposedTick: 35);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDeadLetterDispositionKind.DeadlineExceeded,
            result.Disposition!.Kind);
        Xunit.Assert.Same(deadline, result.Disposition.Deadline);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryDecisionStatus.DeadlineExceeded,
            result.Disposition.RetryDecisionStatus);
    }

    [Xunit.Fact]
    public void CompletedAndCancelledSettlementsAreRejected()
    {
        foreach (var kind in new[]
        {
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Completed,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Cancelled,
        })
        {
            var settlement = Settlement(kind);
            var decision = Decision(
                settlement,
                maximumAttempts: 1,
                observedTick: 30);

            AssertStatus(
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeDeadLetterDispositionStatus
                    .InvalidSettlementOutcome,
                settlement,
                Dispose(
                    settlement,
                    decision,
                    disposedTick: 30));
        }
    }

    [Xunit.Fact]
    public void AllowedRetryCannotBeDeadLettered()
    {
        var settlement = FailedSettlement();
        var decision = Decision(
            settlement,
            maximumAttempts: 3,
            observedTick: 30,
            retryDelayTicks: 10);

        Xunit.Assert.True(decision.ShouldRetry);

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDeadLetterDispositionStatus.RetryStillAllowed,
            settlement,
            Dispose(
                settlement,
                decision,
                disposedTick: 30));
    }

    [Xunit.Fact]
    public void SettlementRequestAttemptAndClockMismatchesAreExplicit()
    {
        var settlement = FailedSettlement();

        var otherPending =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.Create(
                    OtherRequestId(),
                    RuntimeId(),
                    OperationId(),
                    CorrelationId(),
                    default,
                    new RequestPayload("other"));
        var otherFailed =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.Finalize(
                    otherPending,
                    otherPending.Revision,
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRequestState.Failed).Envelope;
        var otherDecision =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryDecisionFlow.Decide(
                    otherFailed,
                    otherFailed.Revision,
                    ClockId(),
                    observedTick: 30,
                    completedAttemptNumber: 1,
                    Policy(1, 0),
                    deadline: null,
                    new RetryReason("other"));

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDeadLetterDispositionStatus
                .SettlementRequestMismatch,
            settlement,
            Dispose(
                settlement,
                otherDecision,
                disposedTick: 30));

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDeadLetterDispositionStatus
                .AttemptNumberMismatch,
            settlement,
            Dispose(
                settlement,
                Decision(
                    settlement,
                    maximumAttempts: 2,
                    completedAttemptNumber: 2,
                    observedTick: 30),
                disposedTick: 30));

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDeadLetterDispositionStatus.ClockMismatch,
            settlement,
            Dispose(
                settlement,
                Decision(
                    settlement,
                    maximumAttempts: 1,
                    clockId: OtherClockId(),
                    observedTick: 30),
                disposedTick: 30));

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDeadLetterDispositionStatus.ClockMismatch,
            settlement,
            Dispose(
                settlement,
                Decision(
                    settlement,
                    maximumAttempts: 1,
                    observedTick: 30),
                clockId: OtherClockId(),
                disposedTick: 30));
    }

    [Xunit.Fact]
    public void DispositionTickBeforeSettlementIsExplicit()
    {
        var settlement = FailedSettlement();

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDeadLetterDispositionStatus.BeforeSettlement,
            settlement,
            Dispose(
                settlement,
                Decision(settlement),
                disposedTick: settlement.SettledTick - 1));
    }

    [Xunit.Fact]
    public void UnsupportedRetryDenialsRemainExplicit()
    {
        var settlement = FailedSettlement();
        var staleDecision = Decision(
            settlement,
            expectedRevision:
                settlement.Request.Revision - 1,
            maximumAttempts: 1,
            observedTick: 30);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryDecisionStatus.StaleRevision,
            staleDecision.Status);

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDeadLetterDispositionStatus
                .UnsupportedRetryDenial,
            settlement,
            Dispose(
                settlement,
                staleDecision,
                disposedTick: 30));
    }

    [Xunit.Fact]
    public void DispositionPreservesTerminalAuthorityAndLineage()
    {
        var settlement = Settlement(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Rejected);
        var reason = new RetryReason("terminal");
        var decision = Decision(
            settlement,
            maximumAttempts: 1,
            observedTick: 40,
            reason: reason);

        var result = Dispose(
            settlement,
            decision,
            disposedTick: 45);
        var disposition = result.Disposition!;

        Xunit.Assert.Same(settlement, result.Settlement);
        Xunit.Assert.Same(settlement.Request, result.Request);
        Xunit.Assert.Equal(SettlementId(), disposition.SettlementId);
        Xunit.Assert.Equal(AttemptId(), disposition.AttemptId);
        Xunit.Assert.Equal(RequestId(), disposition.RequestId);
        Xunit.Assert.Equal(WorkerId(), disposition.WorkerId);
        Xunit.Assert.Equal(DispatchId(), disposition.DispatchId);
        Xunit.Assert.Equal(1, disposition.AttemptNumber);
        Xunit.Assert.Equal(ClockId(), disposition.ClockId);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Rejected,
            disposition.OutcomeKind);
        Xunit.Assert.Same(decision.Policy, disposition.Policy);
        Xunit.Assert.Same(reason, disposition.Reason);
    }

    [Xunit.Fact]
    public void ContractsDoNotStoreScheduleOrExecutePayloads()
    {
        var request = new CountingRequest();
        var completion = new CountingCompletion();
        var reason = new CountingRetryReason();
        var settlement = CreateSettlement(
            request,
            completion,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Failed);
        var decision = Decision(
            settlement,
            maximumAttempts: 1,
            observedTick: 30,
            reason: reason);

        var result = Dispose(
            settlement,
            decision,
            disposedTick: 30);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(0, request.InvocationCount);
        Xunit.Assert.Equal(0, completion.InvocationCount);
        Xunit.Assert.Equal(0, reason.InvocationCount);
        Xunit.Assert.Same(request, result.Request.Payload);
        Xunit.Assert.Same(
            completion,
            result.Disposition!.Completion.Payload);
        Xunit.Assert.Same(reason, result.Disposition.Reason);
    }

    private static void AssertStatus<TRequest, TCompletion>(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDeadLetterDispositionStatus expected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeAttemptSettlement<TRequest, TCompletion>
                settlement,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDeadLetterDispositionResult<
                TRequest,
                TCompletion> result)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
        where TCompletion :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion
    {
        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(expected, result.Status);
        Xunit.Assert.Null(result.Disposition);
        Xunit.Assert.Same(settlement, result.Settlement);
        Xunit.Assert.Same(settlement.Request, result.Request);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeDeadLetterDispositionResult<TRequest, TCompletion>
        Dispose<TRequest, TCompletion>(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlement<TRequest, TCompletion>
                    settlement,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryDecision<TRequest> decision,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeDeadLetterDispositionIdKind>?
                dispositionId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeClockIdKind>? clockId = null,
            long? disposedTick = null)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
        where TCompletion :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDeadLetterDispositionFlow.Dispose(
                dispositionId ?? DispositionId(),
                settlement,
                decision,
                clockId ?? ClockId(),
                disposedTick ?? 30);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRetryDecision<TRequest>
        Decision<TRequest, TCompletion>(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlement<TRequest, TCompletion>
                    settlement,
            int maximumAttempts = 1,
            int? completedAttemptNumber = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeClockIdKind>? clockId = null,
            long observedTick = 30,
            long retryDelayTicks = 0,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDeadline? deadline = null,
            long? expectedRevision = null,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .IHostRuntimeRetryReason? reason = null)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
        where TCompletion :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRetryDecisionFlow.Decide(
                settlement.Request,
                expectedRevision ?? settlement.Request.Revision,
                clockId ?? ClockId(),
                observedTick,
                completedAttemptNumber ?? settlement.AttemptNumber,
                Policy(maximumAttempts, retryDelayTicks),
                deadline,
                reason ?? new RetryReason("retry"));

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeAttemptSettlement<RequestPayload, CompletionPayload>
        FailedSettlement() =>
        Settlement(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Failed);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeAttemptSettlement<RequestPayload, CompletionPayload>
        Settlement(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind kind) =>
        CreateSettlement(
            new RequestPayload("payload"),
            new CompletionPayload("completion"),
            kind);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeAttemptSettlement<TRequest, TCompletion>
        CreateSettlement<TRequest, TCompletion>(
            TRequest requestPayload,
            TCompletion completionPayload,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind kind,
            int attemptNumber = 1,
            long settledTick = 30)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
        where TCompletion :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion
    {
        var pending =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.Create(
                    RequestId(),
                    RuntimeId(),
                    OperationId(),
                    CorrelationId(),
                    default,
                    requestPayload);
        var initialSnapshot = Snapshot(8, 0, 10);
        var initialPriority = Priority(8);
        var admissionResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmissionFlow.Decide(
                    AdmissionId(),
                    initialSnapshot,
                    initialSnapshot.Revision,
                    pending,
                    initialPriority);
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
                    attemptNumber);
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
        var completion =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionFlow.Create(
                    attempt.DispatchId,
                    attempt.RequestId,
                    attempt.Dispatch.RuntimeInstanceId,
                    attempt.Dispatch.OperationId,
                    attempt.Dispatch.CorrelationId,
                    attempt.Dispatch.RouteId,
                    attempt.Dispatch.EndpointId,
                    attempt.AttemptNumber,
                    kind,
                    completionPayload);
        var settlementResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementFlow.Settle(
                    SettlementId(),
                    attempt,
                    pending,
                    lease,
                    pending.Revision,
                    lease.Revision,
                    lease.WorkerId,
                    lease.ClockId,
                    settledTick,
                    completion);

        return Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlement<TRequest, TCompletion>>(
                    settlementResult.Settlement);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRetryPolicy Policy(
            int maximumAttempts,
            long retryDelayTicks) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRetryPolicy.Create(
                PolicyId(),
                maximumAttempts,
                retryDelayTicks);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeQueueSnapshot Snapshot(
            int capacity,
            int queuedCount,
            long revision) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeQueueSnapshot.Create(
                QueueId(),
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeQueueCapacity.Create(capacity),
                queuedCount,
                revision);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimePriority Priority(long sequence) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimePriority.Create(
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimePriorityClass.Urgent,
                sequence);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>
        Id<TKind>(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(
            $"019d0000-0000-7000-8000-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDeadLetterDispositionIdKind>
        DispositionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDeadLetterDispositionIdKind>(4001);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeSettlementIdKind>
        SettlementId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeSettlementIdKind>(4002);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeAttemptIdKind>
        AttemptId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptIdKind>(4003);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDispatchSelectionIdKind>
        SelectionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionIdKind>(4004);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueIdKind>
        QueueId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueIdKind>(4005);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeAdmissionIdKind>
        AdmissionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAdmissionIdKind>(4006);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseIdKind>
        LeaseId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseIdKind>(4007);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkerIdKind>
        WorkerId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkerIdKind>(4008);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        ClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(4009);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        OtherClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(4010);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeDispatchIdKind>
        DispatchId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchIdKind>(4011);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRouteIdKind>
        RouteId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRouteIdKind>(4012);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeEndpointIdKind>
        EndpointId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeEndpointIdKind>(4013);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestIdKind>
        RequestId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestIdKind>(4014);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestIdKind>
        OtherRequestId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestIdKind>(4015);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeInstanceIdKind>
        RuntimeId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInstanceIdKind>(4016);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeOperationIdKind>
        OperationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeOperationIdKind>(4017);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCorrelationIdKind>
        CorrelationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCorrelationIdKind>(4018);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRetryPolicyIdKind>
        PolicyId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryPolicyIdKind>(4019);
}
