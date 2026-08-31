namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoverySettlementCycleCompletionTests
{
    private readonly record struct RequestPayload(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;

    private readonly record struct CompletionPayload(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion;

    private sealed class CountingCompletion :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion
    {
        public int InvocationCount { get; private set; }

        public void Invoke() => InvocationCount++;
    }

    private sealed record RecoveryWorldState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    private sealed class Capability :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCapability
    {
    }

    [Xunit.Fact]
    public void IdsTicksRevisionsAndArgumentsAreValidated()
    {
        var acknowledgement = RecoveryAcknowledgement();
        var completion = CompletionFor(
            acknowledgement,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Completed,
            new CompletionPayload("done"));

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementFlow.Settle<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>(
                        default,
                        UnderlyingSettlementId(),
                        acknowledgement,
                        acknowledgement.Revision,
                        acknowledgement.Reconstruction.Request,
                        acknowledgement.Reconstruction.Lease,
                        acknowledgement.Reconstruction.Request.Revision,
                        acknowledgement.Reconstruction.Lease.Revision,
                        acknowledgement.WorkerId,
                        acknowledgement.Reconstruction.Lease.ClockId,
                        settledTick: 80,
                        completion));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementFlow.Settle<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>(
                        RecoveryResumedSettlementId(),
                        default,
                        acknowledgement,
                        acknowledgement.Revision,
                        acknowledgement.Reconstruction.Request,
                        acknowledgement.Reconstruction.Lease,
                        acknowledgement.Reconstruction.Request.Revision,
                        acknowledgement.Reconstruction.Lease.Revision,
                        acknowledgement.WorkerId,
                        acknowledgement.Reconstruction.Lease.ClockId,
                        settledTick: 80,
                        completion));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementFlow.Settle<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>(
                        RecoveryResumedSettlementId(),
                        UnderlyingSettlementId(),
                        null!,
                        expectedAcknowledgementRevision: 0,
                        acknowledgement.Reconstruction.Request,
                        acknowledgement.Reconstruction.Lease,
                        acknowledgement.Reconstruction.Request.Revision,
                        acknowledgement.Reconstruction.Lease.Revision,
                        acknowledgement.WorkerId,
                        acknowledgement.Reconstruction.Lease.ClockId,
                        settledTick: 80,
                        completion));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Settle(
                acknowledgement,
                completion,
                expectedAcknowledgementRevision: -1));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementFlow.Settle<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>(
                        RecoveryResumedSettlementId(),
                        UnderlyingSettlementId(),
                        acknowledgement,
                        acknowledgement.Revision,
                        null!,
                        acknowledgement.Reconstruction.Lease,
                        expectedRequestRevision: 0,
                        acknowledgement.Reconstruction.Lease.Revision,
                        acknowledgement.WorkerId,
                        acknowledgement.Reconstruction.Lease.ClockId,
                        settledTick: 80,
                        completion));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementFlow.Settle<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>(
                        RecoveryResumedSettlementId(),
                        UnderlyingSettlementId(),
                        acknowledgement,
                        acknowledgement.Revision,
                        acknowledgement.Reconstruction.Request,
                        null!,
                        acknowledgement.Reconstruction.Request.Revision,
                        expectedLeaseRevision: 0,
                        acknowledgement.WorkerId,
                        acknowledgement.Reconstruction.Lease.ClockId,
                        settledTick: 80,
                        completion));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Settle(
                acknowledgement,
                completion,
                expectedRequestRevision: -1));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Settle(
                acknowledgement,
                completion,
                expectedLeaseRevision: -1));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementFlow.Settle<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>(
                        RecoveryResumedSettlementId(),
                        UnderlyingSettlementId(),
                        acknowledgement,
                        acknowledgement.Revision,
                        acknowledgement.Reconstruction.Request,
                        acknowledgement.Reconstruction.Lease,
                        acknowledgement.Reconstruction.Request.Revision,
                        acknowledgement.Reconstruction.Lease.Revision,
                        default,
                        acknowledgement.Reconstruction.Lease.ClockId,
                        settledTick: 80,
                        completion));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementFlow.Settle<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>(
                        RecoveryResumedSettlementId(),
                        UnderlyingSettlementId(),
                        acknowledgement,
                        acknowledgement.Revision,
                        acknowledgement.Reconstruction.Request,
                        acknowledgement.Reconstruction.Lease,
                        acknowledgement.Reconstruction.Request.Revision,
                        acknowledgement.Reconstruction.Lease.Revision,
                        acknowledgement.WorkerId,
                        default,
                        settledTick: 80,
                        completion));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Settle(
                acknowledgement,
                completion,
                settledTick: -1));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementFlow.Settle<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>(
                        RecoveryResumedSettlementId(),
                        UnderlyingSettlementId(),
                        acknowledgement,
                        acknowledgement.Revision,
                        acknowledgement.Reconstruction.Request,
                        acknowledgement.Reconstruction.Lease,
                        acknowledgement.Reconstruction.Request.Revision,
                        acknowledgement.Reconstruction.Lease.Revision,
                        acknowledgement.WorkerId,
                        acknowledgement.Reconstruction.Lease.ClockId,
                        settledTick: 80,
                        null!));

        var settlement = RecoverySettlement();

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementFlow.Complete<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>(
                        default,
                        settlement,
                        settlement.Revision,
                        completedTick: 81));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementFlow.Complete<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>(
                        CycleCompletionId(),
                        null!,
                        expectedSettlementRevision: 0,
                        completedTick: 81));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Complete(
                settlement,
                expectedSettlementRevision: -1));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Complete(settlement, completedTick: -1));
    }

    [Xunit.Fact]
    public void SettlementCreatesTerminalAuthorityAndAdvancesRevision()
    {
        var acknowledgement = RecoveryAcknowledgement();
        var completion = CompletionFor(
            acknowledgement,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Completed,
            new CompletionPayload("ok"));

        var result = Settle(acknowledgement, completion);
        var settlement = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumedAttemptSettlement<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>>(result.Settlement);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementStatus.AttemptSettled,
            result.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementStatus.Settled,
            result.AttemptSettlementStatus);
        Xunit.Assert.Same(acknowledgement, settlement.Acknowledgement);
        Xunit.Assert.Same(acknowledgement.Attempt, settlement.Attempt);
        Xunit.Assert.Same(completion, settlement.Completion);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Completed,
            settlement.Request.State);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseState.Released,
            settlement.Lease.State);
        Xunit.Assert.Equal(
            acknowledgement.Revision + 1,
            settlement.Revision);
        Xunit.Assert.Equal(80, settlement.SettledTick);
        Xunit.Assert.Equal(
            RecoveryResumedSettlementId(),
            settlement.RecoverySettlementId);
        Xunit.Assert.Equal(
            UnderlyingSettlementId(),
            settlement.SettlementId);
    }

    [Xunit.Fact]
    public void SettlementRevisionAndTimeAreExplicit()
    {
        var acknowledgement = RecoveryAcknowledgement();
        var completion = CompletionFor(
            acknowledgement,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Completed,
            new CompletionPayload("ok"));

        AssertSettlementStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementStatus
                .StaleAcknowledgementRevision,
            Settle(
                acknowledgement,
                completion,
                expectedAcknowledgementRevision:
                    acknowledgement.Revision + 1));
        AssertSettlementStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementStatus.SettlementTickRegressed,
            Settle(
                acknowledgement,
                completion,
                settledTick: acknowledgement.AcknowledgedTick - 1));
    }

    [Xunit.Fact]
    public void SettlementPreservesUnderlyingAttemptSettlementOutcomes()
    {
        var acknowledgement = RecoveryAcknowledgement();
        var completion = CompletionFor(
            acknowledgement,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Completed,
            new CompletionPayload("ok"));

        var stale = Settle(
            acknowledgement,
            completion,
            expectedRequestRevision:
                acknowledgement.Reconstruction.Request.Revision + 1);
        AssertSettlementStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementStatus
                .AttemptSettlementRejected,
            stale);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementStatus.StaleRequestRevision,
            stale.AttemptSettlementStatus);

        var mismatchedCompletion = CompletionFor(
            acknowledgement,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Completed,
            new CompletionPayload("wrong-dispatch"),
            dispatchId: acknowledgement.Reconstruction.PriorDispatchId);
        var mismatch = Settle(acknowledgement, mismatchedCompletion);
        AssertSettlementStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementStatus
                .AttemptSettlementRejected,
            mismatch);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementStatus.CompletionMismatch,
            mismatch.AttemptSettlementStatus);

        var other = CreateActiveContext(
            seed: 2,
            queueId: RecoveryQueueId());
        var requestMismatch = Settle(
            acknowledgement,
            completion,
            request: other.Request);
        AssertSettlementStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementStatus
                .AttemptSettlementRejected,
            requestMismatch);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementStatus.AttemptRequestMismatch,
            requestMismatch.AttemptSettlementStatus);

        var otherLease =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Acquire(
                    OtherRecoveryLeaseId(),
                    acknowledgement.WorkerId,
                    acknowledgement.Reconstruction.Reacquisition.Admission,
                    acknowledgement.Reconstruction.Lease.ClockId,
                    acquiredTick: 62,
                    durationTicks: 100);
        var leaseMismatch = Settle(
            acknowledgement,
            completion,
            lease: otherLease);
        AssertSettlementStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementStatus
                .AttemptSettlementRejected,
            leaseMismatch);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementStatus.AttemptLeaseMismatch,
            leaseMismatch.AttemptSettlementStatus);

        var expired = Settle(
            acknowledgement,
            completion,
            settledTick: acknowledgement.Reconstruction.Lease.ExpiresTick);
        AssertSettlementStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementStatus
                .AttemptSettlementRejected,
            expired);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementStatus.LeaseExpired,
            expired.AttemptSettlementStatus);
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
    public void EveryTerminalCompletionKindCanCloseTheResumedAttempt(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompletionKind completionKind,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestState expectedState)
    {
        var acknowledgement = RecoveryAcknowledgement();
        var completion = CompletionFor(
            acknowledgement,
            completionKind,
            new CompletionPayload(completionKind.ToString()));

        var settlement = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumedAttemptSettlement<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>>(
                        Settle(acknowledgement, completion).Settlement);

        Xunit.Assert.Equal(completionKind, settlement.OutcomeKind);
        Xunit.Assert.Equal(expectedState, settlement.Request.State);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseState.Released,
            settlement.Lease.State);
    }

    [Xunit.Fact]
    public void CycleCompletionClosesExactSettlementAuthority()
    {
        var settlement = RecoverySettlement();
        var result = Complete(settlement, completedTick: 81);
        var cycle = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCycleCompletion<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>>(result.CycleCompletion);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementStatus.CycleCompleted,
            result.Status);
        Xunit.Assert.Same(settlement, cycle.ResumedSettlement);
        Xunit.Assert.Same(settlement.Acknowledgement, cycle.Acknowledgement);
        Xunit.Assert.Same(settlement.Checkpoint, cycle.Checkpoint);
        Xunit.Assert.Same(settlement.Request, cycle.Request);
        Xunit.Assert.Same(settlement.Lease, cycle.Lease);
        Xunit.Assert.Same(settlement.Completion, cycle.ReportedCompletion);
        Xunit.Assert.Equal(settlement.Revision + 1, cycle.Revision);
        Xunit.Assert.Equal(81, cycle.CompletedTick);
        Xunit.Assert.Equal(CycleCompletionId(), cycle.CycleCompletionId);
        Xunit.Assert.Equal(settlement.OutcomeKind, cycle.OutcomeKind);
    }

    [Xunit.Fact]
    public void CycleCompletionRevisionAndTimeAreExplicit()
    {
        var settlement = RecoverySettlement();

        AssertCycleStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementStatus.StaleSettlementRevision,
            Complete(
                settlement,
                expectedSettlementRevision: settlement.Revision + 1));
        AssertCycleStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoverySettlementStatus.CompletionTickRegressed,
            Complete(
                settlement,
                completedTick: settlement.SettledTick - 1));
    }

    [Xunit.Fact]
    public void RecoveryLineageAndRevisionsAdvanceExactlyOnce()
    {
        var acknowledgement = RecoveryAcknowledgement();
        var settlement = RecoverySettlement(acknowledgement);
        var cycle = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCycleCompletion<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>>(
                        Complete(settlement).CycleCompletion);

        Xunit.Assert.Equal(
            acknowledgement.Revision + 1,
            settlement.Revision);
        Xunit.Assert.Equal(
            settlement.Revision + 1,
            cycle.Revision);
        Xunit.Assert.Same(
            acknowledgement.Reconstruction,
            settlement.Reconstruction);
        Xunit.Assert.Same(
            acknowledgement.Reacquisition,
            settlement.Reacquisition);
        Xunit.Assert.Same(
            acknowledgement.Reacquisition.Readmission,
            settlement.Readmission);
        Xunit.Assert.Same(
            acknowledgement.Reacquisition.Selection,
            settlement.Selection);
        Xunit.Assert.Same(
            acknowledgement.Reacquisition.Selection.Plan,
            settlement.Plan);
        Xunit.Assert.Same(
            acknowledgement.Reacquisition.Selection.Plan.Continuation,
            settlement.Continuation);
        Xunit.Assert.Same(
            acknowledgement.Reacquisition.Selection.Plan.Continuation.Checkpoint,
            settlement.Checkpoint);
        Xunit.Assert.Equal(acknowledgement.AttemptId, cycle.AttemptId);
        Xunit.Assert.Equal(acknowledgement.RequestId, cycle.RequestId);
        Xunit.Assert.Equal(acknowledgement.LeaseId, cycle.LeaseId);
    }

    [Xunit.Fact]
    public void ContractsRemainBoundedWithoutRetryTransportOrExecution()
    {
        var acknowledgement = RecoveryAcknowledgement();
        var originalRequest = acknowledgement.Reconstruction.Request;
        var originalLease = acknowledgement.Reconstruction.Lease;
        var priorAttempt = acknowledgement.PriorAttempt;
        var payload = new CountingCompletion();
        var completion = CompletionFor(
            acknowledgement,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Failed,
            payload);

        var settlementResult = Settle(acknowledgement, completion);
        var settlement = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumedAttemptSettlement<
                    RequestPayload,
                    RecoveryWorldState,
                    CountingCompletion>>(settlementResult.Settlement);
        var cycle = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCycleCompletion<
                    RequestPayload,
                    RecoveryWorldState,
                    CountingCompletion>>(
                        Complete(settlement).CycleCompletion);

        Xunit.Assert.Equal(0, payload.InvocationCount);
        Xunit.Assert.Same(priorAttempt, settlement.PriorAttempt);
        Xunit.Assert.Same(acknowledgement.Attempt, settlement.Attempt);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Pending,
            originalRequest.State);
        Xunit.Assert.True(originalLease.IsActive);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Failed,
            settlement.Request.State);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseState.Released,
            settlement.Lease.State);
        Xunit.Assert.Same(settlement, cycle.ResumedSettlement);
    }

    private static void AssertSettlementStatus<TCompletion>(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoverySettlementStatus expected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryResumedAttemptSettlementResult<
                RequestPayload,
                RecoveryWorldState,
                TCompletion> result)
        where TCompletion :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion
    {
        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(expected, result.Status);
        Xunit.Assert.Null(result.Settlement);
    }

    private static void AssertCycleStatus<TCompletion>(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoverySettlementStatus expected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCycleCompletionResult<
                RequestPayload,
                RecoveryWorldState,
                TCompletion> result)
        where TCompletion :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion
    {
        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(expected, result.Status);
        Xunit.Assert.Null(result.CycleCompletion);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryResumedAttemptAcknowledgement<
            RequestPayload,
            RecoveryWorldState>
        RecoveryAcknowledgement()
    {
        var reconstruction = RecoveryReconstruction();

        return Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumedAttemptAcknowledgement<
                    RequestPayload,
                    RecoveryWorldState>>(
                        Acknowledge(reconstruction).Acknowledgement);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryResumedAttemptSettlementResult<
            RequestPayload,
            RecoveryWorldState,
            TCompletion>
        Settle<TCompletion>(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumedAttemptAcknowledgement<
                    RequestPayload,
                    RecoveryWorldState> acknowledgement,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionEnvelope<TCompletion> completion,
            long? expectedAcknowledgementRevision = null,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestEnvelope<RequestPayload>? request = null,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLease<RequestPayload>? lease = null,
            long? expectedRequestRevision = null,
            long? expectedLeaseRevision = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeWorkerIdKind>? settlingWorkerId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeClockIdKind>? clockId = null,
            long settledTick = 80)
        where TCompletion :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion
    {
        var currentRequest = request ?? acknowledgement.Reconstruction.Request;
        var currentLease = lease ?? acknowledgement.Reconstruction.Lease;

        return global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoverySettlementFlow.Settle<
                RequestPayload,
                RecoveryWorldState,
                TCompletion>(
                    RecoveryResumedSettlementId(),
                    UnderlyingSettlementId(),
                    acknowledgement,
                    expectedAcknowledgementRevision ?? acknowledgement.Revision,
                    currentRequest,
                    currentLease,
                    expectedRequestRevision ?? currentRequest.Revision,
                    expectedLeaseRevision ?? currentLease.Revision,
                    settlingWorkerId ?? acknowledgement.WorkerId,
                    clockId ?? currentLease.ClockId,
                    settledTick,
                    completion);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryResumedAttemptSettlement<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>
        RecoverySettlement(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumedAttemptAcknowledgement<
                    RequestPayload,
                    RecoveryWorldState>? acknowledgement = null)
    {
        var currentAcknowledgement =
            acknowledgement ?? RecoveryAcknowledgement();
        var completion = CompletionFor(
            currentAcknowledgement,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind.Completed,
            new CompletionPayload("done"));

        return Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumedAttemptSettlement<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>>(
                        Settle(currentAcknowledgement, completion).Settlement);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCycleCompletionResult<
            RequestPayload,
            RecoveryWorldState,
            TCompletion>
        Complete<TCompletion>(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumedAttemptSettlement<
                    RequestPayload,
                    RecoveryWorldState,
                    TCompletion> settlement,
            long? expectedSettlementRevision = null,
            long completedTick = 81)
        where TCompletion :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoverySettlementFlow.Complete<
                RequestPayload,
                RecoveryWorldState,
                TCompletion>(
                    CycleCompletionId(),
                    settlement,
                    expectedSettlementRevision ?? settlement.Revision,
                    completedTick);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeCompletionEnvelope<TCompletion>
        CompletionFor<TCompletion>(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumedAttemptAcknowledgement<
                    RequestPayload,
                    RecoveryWorldState> acknowledgement,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionKind kind,
            TCompletion payload,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeDispatchIdKind>? dispatchId = null)
        where TCompletion :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion
    {
        var dispatch = acknowledgement.Attempt.Dispatch;

        return global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompletionFlow.Create(
                dispatchId ?? dispatch.DispatchId,
                dispatch.RequestId,
                dispatch.RuntimeInstanceId,
                dispatch.OperationId,
                dispatch.CorrelationId,
                dispatch.RouteId,
                dispatch.EndpointId,
                dispatch.AttemptNumber,
                kind,
                payload);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryReadmissionResult<
            RequestPayload,
            RecoveryWorldState>
        Readmit(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeResumedWorkSelection<
                    RequestPayload,
                    RecoveryWorldState> selection,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueSnapshot snapshot,
            long? expectedSelectionRevision = null,
            long? expectedQueueRevision = null,
            long readmittedTick = 60,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeAdmissionIdKind>? admissionId = null) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryReadmissionFlow.Readmit<
                RequestPayload,
                RecoveryWorldState>(
                    ReadmissionId(),
                    admissionId ?? RecoveryAdmissionId(),
                    selection,
                    expectedSelectionRevision ?? selection.Revision,
                    snapshot,
                    expectedQueueRevision ?? snapshot.Revision,
                    readmittedTick);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryReadmission<
            RequestPayload,
            RecoveryWorldState>
        RecoveryReadmission(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeResumedWorkSelection<
                    RequestPayload,
                    RecoveryWorldState> selection,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueSnapshot snapshot) =>
        Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmission<
                    RequestPayload,
                    RecoveryWorldState>>(
                        Readmit(selection, snapshot).Readmission);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryLeaseReacquisitionResult<
            RequestPayload,
            RecoveryWorldState>
        Reacquire(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmission<
                    RequestPayload,
                    RecoveryWorldState> readmission,
            long? expectedReadmissionRevision = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeLeaseIdKind>? leaseId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeClockIdKind>? clockId = null,
            long reacquiredTick = 61,
            long durationTicks = 100) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryReadmissionFlow.Reacquire<
                RequestPayload,
                RecoveryWorldState>(
                    ReacquisitionId(),
                    leaseId ?? RecoveryLeaseId(),
                    RecoveryWorkerId(),
                    readmission,
                    expectedReadmissionRevision ?? readmission.Revision,
                    clockId ?? ClockId(),
                    reacquiredTick,
                    durationTicks);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryLeaseReacquisition<
            RequestPayload,
            RecoveryWorldState>
        RecoveryReacquisition()
    {
        var context = CreateSelectionContext();
        var readmission = RecoveryReadmission(
            context.Selection,
            CurrentQueueSnapshot());

        return Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryLeaseReacquisition<
                    RequestPayload,
                    RecoveryWorldState>>(
                        Reacquire(readmission).Reacquisition);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryDispatchReconstructionResult<
            RequestPayload,
            RecoveryWorldState>
        Reconstruct(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryLeaseReacquisition<
                    RequestPayload,
                    RecoveryWorldState> reacquisition,
            long? expectedReacquisitionRevision = null,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueSnapshot? snapshot = null,
            long? expectedQueueRevision = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeClockIdKind>? clockId = null,
            long reconstructedTick = 70,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeDispatchSelectionIdKind>? selectionId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeDispatchIdKind>? dispatchId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeRouteIdKind>? routeId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeEndpointIdKind>? endpointId = null,
            int? attemptNumber = null)
    {
        var currentSnapshot = snapshot ?? reacquisition.Admission.Snapshot;

        return global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryDispatchFlow.Reconstruct<
                RequestPayload,
                RecoveryWorldState>(
                    ReconstructionId(),
                    selectionId ?? RecoveryDispatchSelectionId(),
                    reacquisition,
                    expectedReacquisitionRevision ?? reacquisition.Revision,
                    currentSnapshot,
                    expectedQueueRevision ?? currentSnapshot.Revision,
                    clockId ?? ClockId(),
                    reconstructedTick,
                    dispatchId ?? RecoveryDispatchId(),
                    routeId ?? RecoveryRouteId(),
                    endpointId ?? RecoveryEndpointId(),
                    attemptNumber ??
                        checked(
                            reacquisition.Selection.Candidate.AttemptNumber + 1));
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryDispatchReconstruction<
            RequestPayload,
            RecoveryWorldState>
        RecoveryReconstruction(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryLeaseReacquisition<
                    RequestPayload,
                    RecoveryWorldState>? reacquisition = null) =>
        Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchReconstruction<
                    RequestPayload,
                    RecoveryWorldState>>(
                        Reconstruct(
                            reacquisition ?? RecoveryReacquisition())
                                .Reconstruction);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryResumedAttemptAcknowledgementResult<
            RequestPayload,
            RecoveryWorldState>
        Acknowledge(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchReconstruction<
                    RequestPayload,
                    RecoveryWorldState> reconstruction,
            long? expectedReconstructionRevision = null,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestEnvelope<RequestPayload>? request = null,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLease<RequestPayload>? lease = null,
            long? expectedRequestRevision = null,
            long? expectedLeaseRevision = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeLeaseIdKind>? acknowledgedLeaseId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeWorkerIdKind>? acknowledgedWorkerId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeDispatchIdKind>? acknowledgedDispatchId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeRequestIdKind>? acknowledgedRequestId = null,
            int? acknowledgedAttemptNumber = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeClockIdKind>? clockId = null,
            long acknowledgedTick = 71,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeAttemptIdKind>? attemptId = null)
    {
        var currentRequest = request ?? reconstruction.Request;
        var currentLease = lease ?? reconstruction.Lease;

        return global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryDispatchFlow.Acknowledge<
                RequestPayload,
                RecoveryWorldState>(
                    RecoveryAcknowledgementId(),
                    attemptId ?? RecoveryAttemptId(),
                    reconstruction,
                    expectedReconstructionRevision ?? reconstruction.Revision,
                    currentRequest,
                    currentLease,
                    expectedRequestRevision ?? currentRequest.Revision,
                    expectedLeaseRevision ?? currentLease.Revision,
                    acknowledgedLeaseId ?? currentLease.LeaseId,
                    acknowledgedWorkerId ?? currentLease.WorkerId,
                    acknowledgedDispatchId ?? reconstruction.DispatchId,
                    acknowledgedRequestId ?? currentRequest.RequestId,
                    acknowledgedAttemptNumber ?? reconstruction.AttemptNumber,
                    clockId ?? currentLease.ClockId,
                    acknowledgedTick);
    }

    private static SelectionContext CreateSelectionContext(
        bool mismatchedPriorQueue = false)
    {
        var active = CreateActiveContext(
            seed: 1,
            queueId: mismatchedPriorQueue
                ? OtherQueueId()
                : RecoveryQueueId());
        var recovery = CreateRecoveryContext(new[] { Item(active) });
        var plan = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionPlan<
                    RequestPayload,
                    RecoveryWorldState>>(
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeRecoveryResumptionFlow.Plan<
                                RequestPayload,
                                RecoveryWorldState>(
                                    PlanId(),
                                    recovery.Continuation,
                                    recovery.Continuation.Revision,
                                    plannedTick: 50,
                                    revision: 4).Plan);
        var selection = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeResumedWorkSelection<
                    RequestPayload,
                    RecoveryWorldState>>(
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeRecoveryResumptionFlow.Select<
                                RequestPayload,
                                RecoveryWorldState>(
                                    SelectionId(),
                                    plan,
                                    plan.Revision,
                                    active.Attempt.AttemptId,
                                    selectedTick: 55).Selection);

        return new SelectionContext(active, recovery, plan, selection);
    }

    private static RecoveryContext CreateRecoveryContext(
        IEnumerable<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkItem<RequestPayload>> items)
    {
        var composition = Composition();
        var lifecycle =
            global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLifecycle
                .Create(RuntimeId(), composition.CompositionId);
        var queue = CurrentQueueSnapshot(revision: 2);
        var activeResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkFlow.Capture(
                    ActiveWorkSnapshotId(),
                    RuntimeId(),
                    ClockId(),
                    observedTick: 30,
                    revision: 6,
                    items);
        var active = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkSnapshot<RequestPayload>>(
                    activeResult.Snapshot);
        var persistence =
            new global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateSnapshotPersistence<RecoveryWorldState>(
                    new RecoveryCodec());
        var manager =
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<RecoveryWorldState>.Create(
                    WorldId(),
                    new RecoveryWorldState(7),
                    initialSimulationTick: 5);
        var document = persistence.Capture(manager.Read());
        var checkpoint = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpoint<RequestPayload>>(
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryFlow.CaptureCheckpoint(
                            CheckpointId(),
                            lifecycle,
                            composition,
                            queue,
                            active,
                            document,
                            capturedTick: 40,
                            revision: 4).Checkpoint);
        var continuation = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuation<
                    RequestPayload,
                    RecoveryWorldState>>(
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeRecoveryFlow.Continue<
                                RequestPayload,
                                RecoveryWorldState>(
                                    ContinuationId(),
                                    checkpoint,
                                    checkpoint.Revision,
                                    persistence.Restore(document),
                                    continuedTick: 45).Continuation);

        return new RecoveryContext(continuation);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeActiveWorkItem<RequestPayload>
        Item(ActiveWorkContext context) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkItem<RequestPayload>.Create(
                context.Attempt,
                context.Request,
                context.Lease);

    private static ActiveWorkContext CreateActiveContext(
        int seed,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueIdKind> queueId)
    {
        var pending =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.Create(
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeRequestIdKind>(Suffix(seed, 1)),
                    RuntimeId(),
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeOperationIdKind>(Suffix(seed, 2)),
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeCorrelationIdKind>(Suffix(seed, 3)),
                    default,
                    new RequestPayload($"payload-{seed}"));
        var queueSnapshot = CurrentQueueSnapshot(
            queueId: queueId,
            revision: 10);
        var priority =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimePriority.Create(
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimePriorityClass.Urgent,
                    sequence: seed);
        var admissionResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmissionFlow.Decide(
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeAdmissionIdKind>(Suffix(seed, 5)),
                    queueSnapshot,
                    queueSnapshot.Revision,
                    pending,
                    priority);
        var admission = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmission<RequestPayload>>(
                    admissionResult.Admission);
        var lease =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Acquire(
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeLeaseIdKind>(Suffix(seed, 6)),
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeWorkerIdKind>(Suffix(seed, 7)),
                    admission,
                    ClockId(),
                    acquiredTick: 10,
                    durationTicks: 100);
        var dispatchResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionFlow.Select(
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeDispatchSelectionIdKind>(
                                Suffix(seed, 8)),
                    admissionResult.Snapshot,
                    admissionResult.Snapshot.Revision,
                    lease,
                    ClockId(),
                    observedTick: 20,
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeDispatchIdKind>(Suffix(seed, 9)),
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeRouteIdKind>(Suffix(seed, 10)),
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeEndpointIdKind>(Suffix(seed, 11)),
                    attemptNumber: 1);
        var dispatch = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelection<RequestPayload>>(
                    dispatchResult.Selection);
        var acknowledgement =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementFlow.Acknowledge(
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeAttemptIdKind>(Suffix(seed, 12)),
                    dispatch,
                    pending,
                    lease,
                    pending.Revision,
                    lease.Revision,
                    lease.LeaseId,
                    lease.WorkerId,
                    dispatch.Dispatch.DispatchId,
                    pending.RequestId,
                    dispatch.Dispatch.AttemptNumber,
                    lease.ClockId,
                    acknowledgedTick: 20);
        var attempt = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInFlightAttempt<RequestPayload>>(
                    acknowledgement.Attempt);

        return new ActiveWorkContext(admission, pending, lease, attempt);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeQueueSnapshot CurrentQueueSnapshot(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeQueueIdKind>? queueId = null,
            int queuedCount = 0,
            long revision = 20) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeQueueSnapshot.Create(
                queueId ?? RecoveryQueueId(),
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeQueueCapacity.Create(8),
                queuedCount,
                revision);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeComposition Composition()
    {
        var descriptor =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCapabilityDescriptor.Create(
                    CapabilityId(),
                    new Capability(),
                    Array.Empty<
                        global::AI.Sandbox.Engine.Core.Identifiers.Id<
                            global::AI.Sandbox.Engine.Core.HostRuntime
                                .HostRuntimeCapabilityIdKind>>());
        var result =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompositionBuilder.Compose(
                    CompositionId(),
                    new[] { descriptor });

        return Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeComposition>(
                result.Composition);
    }

    private static int Suffix(int seed, int offset) =>
        460000 + (seed * 100) + offset;

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>
        Id<TKind>(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(
            $"019f0000-0000-7000-8000-{suffix:D12}");


private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryResumedAttemptSettlementIdKind>
    RecoveryResumedSettlementId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryResumedAttemptSettlementIdKind>(479001);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeSettlementIdKind>
    UnderlyingSettlementId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeSettlementIdKind>(479002);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCycleCompletionIdKind>
    CycleCompletionId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCycleCompletionIdKind>(479003);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseIdKind>
    OtherRecoveryLeaseId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeLeaseIdKind>(479004);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryReadmissionIdKind>
        ReadmissionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionIdKind>(469001);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryLeaseReacquisitionIdKind>
        ReacquisitionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryLeaseReacquisitionIdKind>(469002);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeAdmissionIdKind>
        RecoveryAdmissionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAdmissionIdKind>(469003);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseIdKind>
        RecoveryLeaseId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseIdKind>(469004);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkerIdKind>
        RecoveryWorkerId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkerIdKind>(469005);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryDispatchReconstructionIdKind>
        ReconstructionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchReconstructionIdKind>(469019);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryResumedAttemptAcknowledgementIdKind>
        RecoveryAcknowledgementId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumedAttemptAcknowledgementIdKind>(469020);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDispatchSelectionIdKind>
        RecoveryDispatchSelectionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionIdKind>(469021);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeDispatchIdKind>
        RecoveryDispatchId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchIdKind>(469022);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRouteIdKind>
        RecoveryRouteId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRouteIdKind>(469023);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeEndpointIdKind>
        RecoveryEndpointId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeEndpointIdKind>(469024);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeAttemptIdKind>
        RecoveryAttemptId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptIdKind>(469025);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkerIdKind>
        OtherWorkerId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkerIdKind>(469026);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryResumptionPlanIdKind>
        PlanId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionPlanIdKind>(469006);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeResumedWorkSelectionIdKind>
        SelectionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeResumedWorkSelectionIdKind>(469007);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointIdKind>
        CheckpointId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointIdKind>(469008);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuationIdKind>
        ContinuationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuationIdKind>(469009);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeInstanceIdKind>
        RuntimeId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInstanceIdKind>(469010);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompositionIdKind>
        CompositionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompositionIdKind>(469011);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCapabilityIdKind>
        CapabilityId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCapabilityIdKind>(469012);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueIdKind>
        RecoveryQueueId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueIdKind>(469013);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueIdKind>
        OtherQueueId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueIdKind>(469014);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkSnapshotIdKind>
        ActiveWorkSnapshotId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkSnapshotIdKind>(469015);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        ClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(469016);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        OtherClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(469017);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>
        WorldId() => Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>(469018);

    private sealed class RecoveryCodec :
        global::AI.Sandbox.Engine.Core.Persistence
            .IWorldStateSnapshotCodec<RecoveryWorldState>
    {
        public global::AI.Sandbox.Engine.Core.Persistence.PersistenceSchemaId
            SchemaId { get; } =
            global::AI.Sandbox.Engine.Core.Persistence.PersistenceSchemaId
                .Parse("host.recovery-dispatch");

        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaVersion CurrentSchemaVersion { get; } =
            global::AI.Sandbox.Engine.Core.Persistence
                .PersistenceSchemaVersion.From(1);

        public bool CanDecode(
            global::AI.Sandbox.Engine.Core.Persistence
                .PersistenceSchemaVersion version) =>
            version == CurrentSchemaVersion;

        public global::AI.Sandbox.Engine.Core.Persistence.SnapshotPayload
            Encode(RecoveryWorldState state) =>
            global::AI.Sandbox.Engine.Core.Persistence.SnapshotPayload.From(
                System.Text.Encoding.UTF8.GetBytes(
                    state.Value.ToString(
                        System.Globalization.CultureInfo.InvariantCulture)));

        public global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<RecoveryWorldState> Decode(
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion version,
                global::AI.Sandbox.Engine.Core.Persistence.SnapshotPayload
                    payload)
        {
            var text = System.Text.Encoding.UTF8.GetString(payload.ToArray());
            return int.TryParse(
                text,
                System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture,
                out var value)
                ? global::AI.Sandbox.Engine.Core.Persistence
                    .WorldStateDecodeDecision<RecoveryWorldState>.Accept(
                        new RecoveryWorldState(value))
                : global::AI.Sandbox.Engine.Core.Persistence
                    .WorldStateDecodeDecision<RecoveryWorldState>.Reject(
                        "Invalid recovery dispatch state.");
        }
    }

    private sealed record ActiveWorkContext(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeQueueAdmission<RequestPayload> Admission,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestEnvelope<RequestPayload> Request,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeWorkLease<RequestPayload> Lease,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeInFlightAttempt<RequestPayload> Attempt);

    private sealed record RecoveryContext(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuation<
                RequestPayload,
                RecoveryWorldState> Continuation);

    private sealed record SelectionContext(
        ActiveWorkContext Active,
        RecoveryContext Recovery,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryResumptionPlan<
                RequestPayload,
                RecoveryWorldState> Plan,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeResumedWorkSelection<
                RequestPayload,
                RecoveryWorldState> Selection);
}
