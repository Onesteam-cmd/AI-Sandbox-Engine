namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRetryRequeueTests
{
    private readonly record struct RequestPayload(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;

    private readonly record struct CompletionPayload(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion;

    private readonly record struct RetryReason(string Text) :
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
    public void IdsRevisionTickAndArgumentsAreValidated()
    {
        var settlement = FailedSettlement();
        var decision = Decision(settlement);
        var snapshot = Snapshot(4, 0, 7);
        var priority = Priority(10);

        Xunit.Assert.Throws<ArgumentException>(
            () => Requeue(
                settlement,
                decision,
                snapshot,
                priority,
                requeueId: default(
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeRetryRequeueIdKind>)));
        Xunit.Assert.Throws<ArgumentException>(
            () => Requeue(
                settlement,
                decision,
                snapshot,
                priority,
                admissionId: default(
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeAdmissionIdKind>)));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Requeue(
                settlement,
                decision,
                snapshot,
                priority,
                expectedQueueRevision: -1));
        Xunit.Assert.Throws<ArgumentException>(
            () => Requeue(
                settlement,
                decision,
                snapshot,
                priority,
                clockId: default(
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeClockIdKind>)));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Requeue(
                settlement,
                decision,
                snapshot,
                priority,
                observedTick: -1));
    }

    [Xunit.Fact]
    public void FailedSettlementIsRequeuedImmutably()
    {
        var settlement = FailedSettlement();
        var decision = Decision(
            settlement,
            observedTick: 30,
            retryDelayTicks: 10);
        var snapshot = Snapshot(4, 1, 7);
        var priority = Priority(25);

        var result = Requeue(
            settlement,
            decision,
            snapshot,
            priority,
            observedTick: 40);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryRequeueStatus.Requeued,
            result.Status);

        var requeue = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryRequeue<RequestPayload, CompletionPayload>>(
                    result.Requeue);
        Xunit.Assert.Equal(RequeueId(), requeue.RequeueId);
        Xunit.Assert.Same(settlement, requeue.Settlement);
        Xunit.Assert.Same(decision, requeue.RetryDecision);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Pending,
            requeue.Request.State);
        Xunit.Assert.Equal(
            settlement.Request.Revision + 1,
            requeue.Request.Revision);
        Xunit.Assert.Equal(
            settlement.Request.RequestId,
            requeue.Request.RequestId);
        Xunit.Assert.Equal(
            settlement.Request.RuntimeInstanceId,
            requeue.Request.RuntimeInstanceId);
        Xunit.Assert.Equal(
            settlement.Request.OperationId,
            requeue.Request.OperationId);
        Xunit.Assert.Equal(
            settlement.Request.CorrelationId,
            requeue.Request.CorrelationId);
        Xunit.Assert.Equal(
            settlement.Request.ParentRequestId,
            requeue.Request.ParentRequestId);
        Xunit.Assert.Equal(
            settlement.Request.Payload,
            requeue.Request.Payload);
        Xunit.Assert.Null(requeue.Request.CancellationReason);
        Xunit.Assert.Equal(2, result.Snapshot.QueuedCount);
        Xunit.Assert.Equal(8, result.Snapshot.Revision);
        Xunit.Assert.Equal(1, snapshot.QueuedCount);
        Xunit.Assert.Equal(7, snapshot.Revision);
        Xunit.Assert.Same(requeue.Request, requeue.Admission.Request);
        Xunit.Assert.Same(priority, requeue.Priority);
        Xunit.Assert.Same(result.Snapshot, requeue.Snapshot);
    }

    [Xunit.Fact]
    public void RejectedSettlementIsRequeuedImmutably()
    {
        var settlement = Settlement(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Rejected);
        var decision = Decision(
            settlement,
            observedTick: 30,
            retryDelayTicks: 0);

        var result = Requeue(
            settlement,
            decision,
            Snapshot(2, 0, 3),
            Priority(1),
            observedTick: 30);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Pending,
            result.Request.State);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Rejected,
            settlement.Request.State);
    }

    [Xunit.Fact]
    public void CompletedAndCancelledSettlementsAreNotRetryable()
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
                maximumAttempts: 3,
                completedAttemptNumber: 1,
                observedTick: 30,
                retryDelayTicks: 0);

            var result = Requeue(
                settlement,
                decision,
                Snapshot(2, 0, 0),
                Priority(1),
                observedTick: 30);

            AssertUnchanged(
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeRetryRequeueStatus
                    .InvalidSettlementOutcome,
                settlement,
                null,
                result);
        }
    }

    [Xunit.Fact]
    public void DeniedRetryDecisionIsExplicitAndUnchanged()
    {
        var settlement = FailedSettlement();
        var decision = Decision(
            settlement,
            maximumAttempts: 1,
            completedAttemptNumber: 1,
            observedTick: 30,
            retryDelayTicks: 0);
        var snapshot = Snapshot(2, 0, 4);

        Xunit.Assert.False(decision.ShouldRetry);

        var result = Requeue(
            settlement,
            decision,
            snapshot,
            Priority(1),
            observedTick: 30);

        AssertUnchanged(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryRequeueStatus.RetryDenied,
            settlement,
            snapshot,
            result);
    }

    [Xunit.Fact]
    public void SettlementRequestAttemptAndClockMismatchesAreExplicit()
    {
        var settlement = FailedSettlement();
        var snapshot = Snapshot(3, 0, 5);
        var priority = Priority(1);

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
                    Policy(3, 0),
                    deadline: null,
                    new RetryReason("other"));

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryRequeueStatus.SettlementRequestMismatch,
            Requeue(
                settlement,
                otherDecision,
                snapshot,
                priority,
                observedTick: 30));

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryRequeueStatus.AttemptNumberMismatch,
            Requeue(
                settlement,
                Decision(
                    settlement,
                    completedAttemptNumber: 2,
                    observedTick: 30,
                    retryDelayTicks: 0),
                snapshot,
                priority,
                observedTick: 30));

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryRequeueStatus.ClockMismatch,
            Requeue(
                settlement,
                Decision(
                    settlement,
                    clockId: OtherClockId(),
                    observedTick: 30,
                    retryDelayTicks: 0),
                snapshot,
                priority,
                observedTick: 30));

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryRequeueStatus.ClockMismatch,
            Requeue(
                settlement,
                Decision(
                    settlement,
                    observedTick: 30,
                    retryDelayTicks: 0),
                snapshot,
                priority,
                clockId: OtherClockId(),
                observedTick: 30));
    }

    [Xunit.Fact]
    public void SettlementAndRetryTickBoundariesAreExplicit()
    {
        var settlement = FailedSettlement();
        var snapshot = Snapshot(3, 0, 5);
        var priority = Priority(1);

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryRequeueStatus
                .RetryDecisionBeforeSettlement,
            Requeue(
                settlement,
                Decision(
                    settlement,
                    observedTick: 20,
                    retryDelayTicks: 10),
                snapshot,
                priority,
                observedTick: 30));

        var normal = Decision(
            settlement,
            observedTick: 30,
            retryDelayTicks: 10);

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryRequeueStatus.BeforeSettlement,
            Requeue(
                settlement,
                normal,
                snapshot,
                priority,
                observedTick: 29));

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryRequeueStatus.BeforeRetryTick,
            Requeue(
                settlement,
                normal,
                snapshot,
                priority,
                observedTick: 39));
    }

    [Xunit.Fact]
    public void StaleAndFullQueueRemainUnchanged()
    {
        var settlement = FailedSettlement();
        var decision = Decision(
            settlement,
            observedTick: 30,
            retryDelayTicks: 0);
        var priority = Priority(1);
        var snapshot = Snapshot(2, 0, 5);

        var stale = Requeue(
            settlement,
            decision,
            snapshot,
            priority,
            expectedQueueRevision: 4,
            observedTick: 30);
        AssertUnchanged(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryRequeueStatus.StaleQueueRevision,
            settlement,
            snapshot,
            stale);

        var fullSnapshot = Snapshot(1, 1, 8);
        var full = Requeue(
            settlement,
            decision,
            fullSnapshot,
            priority,
            observedTick: 30);
        AssertUnchanged(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryRequeueStatus.QueueFull,
            settlement,
            fullSnapshot,
            full);
    }

    [Xunit.Fact]
    public void RequeuePreservesLineagePriorityAndAttemptNumbers()
    {
        var settlement = FailedSettlement();
        var decision = Decision(
            settlement,
            observedTick: 35,
            retryDelayTicks: 5);
        var priority = Priority(99);

        var result = Requeue(
            settlement,
            decision,
            Snapshot(5, 2, 10),
            priority,
            observedTick: 40);
        var requeue = result.Requeue!;

        Xunit.Assert.Equal(SettlementId(), requeue.SettlementId);
        Xunit.Assert.Equal(AttemptId(), requeue.AttemptId);
        Xunit.Assert.Equal(RetryAdmissionId(), requeue.AdmissionId);
        Xunit.Assert.Equal(QueueId(), requeue.QueueId);
        Xunit.Assert.Equal(RequestId(), requeue.RequestId);
        Xunit.Assert.Equal(1, requeue.CompletedAttemptNumber);
        Xunit.Assert.Equal(2, requeue.NextAttemptNumber);
        Xunit.Assert.Equal(40, requeue.RetryAtTick);
        Xunit.Assert.Equal(40, requeue.RequeuedTick);
        Xunit.Assert.Equal(ClockId(), requeue.ClockId);
        Xunit.Assert.Equal(
            settlement.ObservedRequestRevision,
            requeue.ObservedTerminalRequestRevision);
        Xunit.Assert.Equal(10, requeue.ObservedQueueRevision);
        Xunit.Assert.Same(priority, requeue.Admission.Priority);
    }

    [Xunit.Fact]
    public void ContractsDoNotScheduleStoreOrExecutePayloads()
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
            observedTick: 30,
            retryDelayTicks: 0,
            reason: reason);

        var result = Requeue(
            settlement,
            decision,
            Snapshot(2, 0, 0),
            Priority(1),
            observedTick: 30);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(0, request.InvocationCount);
        Xunit.Assert.Equal(0, completion.InvocationCount);
        Xunit.Assert.Equal(0, reason.InvocationCount);
        Xunit.Assert.Same(request, result.Request.Payload);
        Xunit.Assert.Same(
            completion,
            settlement.Completion.Payload);
        Xunit.Assert.Same(reason, decision.Reason);
    }

    private static void AssertStatus<TRequest, TCompletion>(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRetryRequeueStatus expected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRetryRequeueResult<TRequest, TCompletion> result)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
        where TCompletion :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion
    {
        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(expected, result.Status);
        Xunit.Assert.Null(result.Requeue);
        Xunit.Assert.Null(result.Admission);
    }

    private static void AssertUnchanged<TRequest, TCompletion>(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRetryRequeueStatus expected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeAttemptSettlement<TRequest, TCompletion> settlement,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeQueueSnapshot? SnapshotReference,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRetryRequeueResult<TRequest, TCompletion> result)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
        where TCompletion :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion
    {
        AssertStatus(expected, result);
        Xunit.Assert.Same(settlement.Request, result.Request);
        if (SnapshotReference is not null)
        {
            Xunit.Assert.Same(SnapshotReference, result.Snapshot);
        }
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRetryRequeueResult<TRequest, TCompletion>
        Requeue<TRequest, TCompletion>(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlement<TRequest, TCompletion>
                    settlement,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryDecision<TRequest> decision,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueSnapshot snapshot,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimePriority priority,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeRetryRequeueIdKind>? requeueId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeAdmissionIdKind>? admissionId = null,
            long? expectedQueueRevision = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeClockIdKind>? clockId = null,
            long? observedTick = null)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
        where TCompletion :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRetryRequeueFlow.Requeue(
                requeueId ?? RequeueId(),
                admissionId ?? RetryAdmissionId(),
                settlement,
                decision,
                snapshot,
                expectedQueueRevision ?? snapshot.Revision,
                priority,
                clockId ?? ClockId(),
                observedTick ?? 40);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRetryDecision<TRequest>
        Decision<TRequest, TCompletion>(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlement<TRequest, TCompletion>
                    settlement,
            int maximumAttempts = 3,
            int? completedAttemptNumber = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeClockIdKind>? clockId = null,
            long observedTick = 30,
            long retryDelayTicks = 10,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .IHostRuntimeRetryReason? reason = null)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
        where TCompletion :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRetryDecisionFlow.Decide(
                settlement.Request,
                settlement.Request.Revision,
                clockId ?? ClockId(),
                observedTick,
                completedAttemptNumber ?? settlement.AttemptNumber,
                Policy(maximumAttempts, retryDelayTicks),
                deadline: null,
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
            .HostRuntimeRetryRequeueIdKind>
        RequeueId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryRequeueIdKind>(3901);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeSettlementIdKind>
        SettlementId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeSettlementIdKind>(3902);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeAttemptIdKind>
        AttemptId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptIdKind>(3903);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDispatchSelectionIdKind>
        SelectionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionIdKind>(3904);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueIdKind>
        QueueId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueIdKind>(3905);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeAdmissionIdKind>
        AdmissionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAdmissionIdKind>(3906);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeAdmissionIdKind>
        RetryAdmissionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAdmissionIdKind>(3907);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseIdKind>
        LeaseId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseIdKind>(3908);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkerIdKind>
        WorkerId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkerIdKind>(3909);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        ClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(3910);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        OtherClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(3911);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeDispatchIdKind>
        DispatchId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchIdKind>(3912);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRouteIdKind>
        RouteId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRouteIdKind>(3913);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeEndpointIdKind>
        EndpointId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeEndpointIdKind>(3914);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestIdKind>
        RequestId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestIdKind>(3915);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestIdKind>
        OtherRequestId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestIdKind>(3916);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeInstanceIdKind>
        RuntimeId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInstanceIdKind>(3917);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeOperationIdKind>
        OperationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeOperationIdKind>(3918);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCorrelationIdKind>
        CorrelationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCorrelationIdKind>(3919);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRetryPolicyIdKind>
        PolicyId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryPolicyIdKind>(3920);
}
