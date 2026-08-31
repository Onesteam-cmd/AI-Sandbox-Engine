namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoveryDispatchAcknowledgementTests
{
    private readonly record struct RequestPayload(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;

    private sealed record RecoveryWorldState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    private sealed class Capability :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCapability
    {
    }

    [Xunit.Fact]
    public void IdsTicksRevisionsAttemptsAndArgumentsAreValidated()
    {
        var reacquisition = RecoveryReacquisition();
        var snapshot = reacquisition.Admission.Snapshot;

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchFlow.Reconstruct<
                    RequestPayload,
                    RecoveryWorldState>(
                        default,
                        RecoveryDispatchSelectionId(),
                        reacquisition,
                        reacquisition.Revision,
                        snapshot,
                        snapshot.Revision,
                        ClockId(),
                        reconstructedTick: 70,
                        RecoveryDispatchId(),
                        RecoveryRouteId(),
                        RecoveryEndpointId(),
                        attemptNumber: 2));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchFlow.Reconstruct<
                    RequestPayload,
                    RecoveryWorldState>(
                        ReconstructionId(),
                        default,
                        reacquisition,
                        reacquisition.Revision,
                        snapshot,
                        snapshot.Revision,
                        ClockId(),
                        reconstructedTick: 70,
                        RecoveryDispatchId(),
                        RecoveryRouteId(),
                        RecoveryEndpointId(),
                        attemptNumber: 2));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchFlow.Reconstruct<
                    RequestPayload,
                    RecoveryWorldState>(
                        ReconstructionId(),
                        RecoveryDispatchSelectionId(),
                        null!,
                        expectedReacquisitionRevision: 0,
                        snapshot,
                        snapshot.Revision,
                        ClockId(),
                        reconstructedTick: 70,
                        RecoveryDispatchId(),
                        RecoveryRouteId(),
                        RecoveryEndpointId(),
                        attemptNumber: 2));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Reconstruct(
                reacquisition,
                expectedReacquisitionRevision: -1));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchFlow.Reconstruct<
                    RequestPayload,
                    RecoveryWorldState>(
                        ReconstructionId(),
                        RecoveryDispatchSelectionId(),
                        reacquisition,
                        reacquisition.Revision,
                        null!,
                        expectedQueueRevision: 0,
                        ClockId(),
                        reconstructedTick: 70,
                        RecoveryDispatchId(),
                        RecoveryRouteId(),
                        RecoveryEndpointId(),
                        attemptNumber: 2));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Reconstruct(
                reacquisition,
                expectedQueueRevision: -1));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchFlow.Reconstruct<
                    RequestPayload,
                    RecoveryWorldState>(
                        ReconstructionId(),
                        RecoveryDispatchSelectionId(),
                        reacquisition,
                        reacquisition.Revision,
                        snapshot,
                        snapshot.Revision,
                        default,
                        reconstructedTick: 70,
                        RecoveryDispatchId(),
                        RecoveryRouteId(),
                        RecoveryEndpointId(),
                        attemptNumber: 2));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Reconstruct(reacquisition, reconstructedTick: -1));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchFlow.Reconstruct<
                    RequestPayload,
                    RecoveryWorldState>(
                        ReconstructionId(),
                        RecoveryDispatchSelectionId(),
                        reacquisition,
                        reacquisition.Revision,
                        snapshot,
                        snapshot.Revision,
                        ClockId(),
                        reconstructedTick: 70,
                        default,
                        RecoveryRouteId(),
                        RecoveryEndpointId(),
                        attemptNumber: 2));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchFlow.Reconstruct<
                    RequestPayload,
                    RecoveryWorldState>(
                        ReconstructionId(),
                        RecoveryDispatchSelectionId(),
                        reacquisition,
                        reacquisition.Revision,
                        snapshot,
                        snapshot.Revision,
                        ClockId(),
                        reconstructedTick: 70,
                        RecoveryDispatchId(),
                        default,
                        RecoveryEndpointId(),
                        attemptNumber: 2));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchFlow.Reconstruct<
                    RequestPayload,
                    RecoveryWorldState>(
                        ReconstructionId(),
                        RecoveryDispatchSelectionId(),
                        reacquisition,
                        reacquisition.Revision,
                        snapshot,
                        snapshot.Revision,
                        ClockId(),
                        reconstructedTick: 70,
                        RecoveryDispatchId(),
                        RecoveryRouteId(),
                        default,
                        attemptNumber: 2));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Reconstruct(reacquisition, attemptNumber: 0));

        var reconstruction = RecoveryReconstruction(reacquisition);

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchFlow.Acknowledge<
                    RequestPayload,
                    RecoveryWorldState>(
                        default,
                        RecoveryAttemptId(),
                        reconstruction,
                        reconstruction.Revision,
                        reconstruction.Request,
                        reconstruction.Lease,
                        reconstruction.Request.Revision,
                        reconstruction.Lease.Revision,
                        reconstruction.LeaseId,
                        reconstruction.WorkerId,
                        reconstruction.DispatchId,
                        reconstruction.RequestId,
                        reconstruction.AttemptNumber,
                        reconstruction.ClockId,
                        acknowledgedTick: 71));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchFlow.Acknowledge<
                    RequestPayload,
                    RecoveryWorldState>(
                        RecoveryAcknowledgementId(),
                        default,
                        reconstruction,
                        reconstruction.Revision,
                        reconstruction.Request,
                        reconstruction.Lease,
                        reconstruction.Request.Revision,
                        reconstruction.Lease.Revision,
                        reconstruction.LeaseId,
                        reconstruction.WorkerId,
                        reconstruction.DispatchId,
                        reconstruction.RequestId,
                        reconstruction.AttemptNumber,
                        reconstruction.ClockId,
                        acknowledgedTick: 71));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchFlow.Acknowledge<
                    RequestPayload,
                    RecoveryWorldState>(
                        RecoveryAcknowledgementId(),
                        RecoveryAttemptId(),
                        null!,
                        expectedReconstructionRevision: 0,
                        reconstruction.Request,
                        reconstruction.Lease,
                        reconstruction.Request.Revision,
                        reconstruction.Lease.Revision,
                        reconstruction.LeaseId,
                        reconstruction.WorkerId,
                        reconstruction.DispatchId,
                        reconstruction.RequestId,
                        reconstruction.AttemptNumber,
                        reconstruction.ClockId,
                        acknowledgedTick: 71));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Acknowledge(
                reconstruction,
                expectedReconstructionRevision: -1));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchFlow.Acknowledge<
                    RequestPayload,
                    RecoveryWorldState>(
                        RecoveryAcknowledgementId(),
                        RecoveryAttemptId(),
                        reconstruction,
                        reconstruction.Revision,
                        null!,
                        reconstruction.Lease,
                        expectedRequestRevision: 0,
                        reconstruction.Lease.Revision,
                        reconstruction.LeaseId,
                        reconstruction.WorkerId,
                        reconstruction.DispatchId,
                        reconstruction.RequestId,
                        reconstruction.AttemptNumber,
                        reconstruction.ClockId,
                        acknowledgedTick: 71));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchFlow.Acknowledge<
                    RequestPayload,
                    RecoveryWorldState>(
                        RecoveryAcknowledgementId(),
                        RecoveryAttemptId(),
                        reconstruction,
                        reconstruction.Revision,
                        reconstruction.Request,
                        null!,
                        reconstruction.Request.Revision,
                        expectedLeaseRevision: 0,
                        reconstruction.LeaseId,
                        reconstruction.WorkerId,
                        reconstruction.DispatchId,
                        reconstruction.RequestId,
                        reconstruction.AttemptNumber,
                        reconstruction.ClockId,
                        acknowledgedTick: 71));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Acknowledge(
                reconstruction,
                expectedRequestRevision: -1));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Acknowledge(
                reconstruction,
                expectedLeaseRevision: -1));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Acknowledge(reconstruction, acknowledgedTick: -1));
    }

    [Xunit.Fact]
    public void ReconstructionCreatesNewDispatchAndAdvancesAuthority()
    {
        var reacquisition = RecoveryReacquisition();
        var before = reacquisition.Admission.Snapshot;

        var result = Reconstruct(reacquisition);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchStatus.DispatchReconstructed,
            result.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionStatus.Selected,
            result.SelectionStatus);
        Xunit.Assert.Equal(before.QueuedCount - 1, result.Snapshot.QueuedCount);
        Xunit.Assert.Equal(before.Revision + 1, result.Snapshot.Revision);

        var reconstruction = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchReconstruction<
                    RequestPayload,
                    RecoveryWorldState>>(result.Reconstruction);
        Xunit.Assert.Equal(ReconstructionId(), reconstruction.ReconstructionId);
        Xunit.Assert.Same(reacquisition, reconstruction.Reacquisition);
        Xunit.Assert.Same(reacquisition.Request, reconstruction.Request);
        Xunit.Assert.Same(reacquisition.Lease, reconstruction.Lease);
        Xunit.Assert.Equal(
            RecoveryDispatchSelectionId(),
            reconstruction.SelectionId);
        Xunit.Assert.Equal(RecoveryDispatchId(), reconstruction.DispatchId);
        Xunit.Assert.Equal(
            reacquisition.Selection.Candidate.Attempt.Selection.SelectionId,
            reconstruction.PriorSelectionId);
        Xunit.Assert.Equal(
            reacquisition.Selection.Candidate.Attempt.DispatchId,
            reconstruction.PriorDispatchId);
        Xunit.Assert.Equal(
            reacquisition.Selection.Candidate.AttemptNumber + 1,
            reconstruction.AttemptNumber);
        Xunit.Assert.Equal(70, reconstruction.ReconstructedTick);
        Xunit.Assert.Equal(
            reacquisition.Revision + 1,
            reconstruction.Revision);
    }

    [Xunit.Fact]
    public void ReconstructionRevisionTimeQueueIdentityAndAttemptAreExplicit()
    {
        var reacquisition = RecoveryReacquisition();

        AssertReconstructionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchStatus.StaleReacquisitionRevision,
            Reconstruct(
                reacquisition,
                expectedReacquisitionRevision: reacquisition.Revision + 1));

        AssertReconstructionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchStatus.ReconstructionTickRegressed,
            Reconstruct(
                reacquisition,
                reconstructedTick: reacquisition.ReacquiredTick - 1));

        AssertReconstructionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchStatus.QueueMismatch,
            Reconstruct(
                reacquisition,
                snapshot: CurrentQueueSnapshot(
                    queueId: OtherQueueId(),
                    queuedCount: 1,
                    revision: 30)));

        AssertReconstructionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchStatus.PriorSelectionIdReused,
            Reconstruct(
                reacquisition,
                selectionId:
                    reacquisition.Selection.Candidate.Attempt.Selection
                        .SelectionId));

        AssertReconstructionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchStatus.PriorDispatchIdReused,
            Reconstruct(
                reacquisition,
                dispatchId:
                    reacquisition.Selection.Candidate.Attempt.DispatchId));

        AssertReconstructionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchStatus.AttemptNumberMismatch,
            Reconstruct(
                reacquisition,
                attemptNumber:
                    reacquisition.Selection.Candidate.AttemptNumber));
    }

    [Xunit.Fact]
    public void ReconstructionPreservesUnderlyingDispatchSelectionOutcomes()
    {
        var reacquisition = RecoveryReacquisition();
        var snapshot = reacquisition.Admission.Snapshot;

        var stale = Reconstruct(
            reacquisition,
            snapshot: snapshot,
            expectedQueueRevision: snapshot.Revision + 1);
        AssertReconstructionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchStatus.DispatchSelectionRejected,
            stale);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionStatus.StaleQueueRevision,
            stale.SelectionStatus);
        Xunit.Assert.Same(snapshot, stale.Snapshot);

        var expired = Reconstruct(
            reacquisition,
            reconstructedTick: reacquisition.Lease.ExpiresTick);
        AssertReconstructionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchStatus.DispatchSelectionRejected,
            expired);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionStatus.LeaseExpired,
            expired.SelectionStatus);
    }

    [Xunit.Fact]
    public void AcknowledgementCreatesNewResumedAttemptAndAdvancesAuthority()
    {
        var reconstruction = RecoveryReconstruction();

        var result = Acknowledge(reconstruction);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchStatus.AttemptAcknowledged,
            result.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus.Acknowledged,
            result.AcknowledgementStatus);

        var acknowledgement = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumedAttemptAcknowledgement<
                    RequestPayload,
                    RecoveryWorldState>>(result.Acknowledgement);
        Xunit.Assert.Equal(
            RecoveryAcknowledgementId(),
            acknowledgement.AcknowledgementId);
        Xunit.Assert.Same(reconstruction, acknowledgement.Reconstruction);
        Xunit.Assert.Same(reconstruction.Selection, acknowledgement.Attempt.Selection);
        Xunit.Assert.Same(reconstruction.Request, acknowledgement.Attempt.Request);
        Xunit.Assert.Same(reconstruction.Lease, acknowledgement.Attempt.Lease);
        Xunit.Assert.Equal(RecoveryAttemptId(), acknowledgement.AttemptId);
        Xunit.Assert.Equal(
            reconstruction.PriorAttemptId,
            acknowledgement.PriorAttemptId);
        Xunit.Assert.NotEqual(
            acknowledgement.PriorAttemptId,
            acknowledgement.AttemptId);
        Xunit.Assert.Equal(reconstruction.DispatchId, acknowledgement.DispatchId);
        Xunit.Assert.Equal(reconstruction.AttemptNumber, acknowledgement.AttemptNumber);
        Xunit.Assert.Equal(71, acknowledgement.AcknowledgedTick);
        Xunit.Assert.Equal(
            reconstruction.Revision + 1,
            acknowledgement.Revision);
    }

    [Xunit.Fact]
    public void AcknowledgementRevisionTimeAndPriorAttemptReuseAreExplicit()
    {
        var reconstruction = RecoveryReconstruction();

        AssertAcknowledgementStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchStatus.StaleReconstructionRevision,
            Acknowledge(
                reconstruction,
                expectedReconstructionRevision:
                    reconstruction.Revision + 1));

        AssertAcknowledgementStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchStatus.AcknowledgementTickRegressed,
            Acknowledge(
                reconstruction,
                acknowledgedTick: reconstruction.ReconstructedTick - 1));

        AssertAcknowledgementStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchStatus.PriorAttemptIdReused,
            Acknowledge(
                reconstruction,
                attemptId: reconstruction.PriorAttemptId));
    }

    [Xunit.Fact]
    public void AcknowledgementPreservesUnderlyingValidationOutcomes()
    {
        var reconstruction = RecoveryReconstruction();

        var staleRequest = Acknowledge(
            reconstruction,
            expectedRequestRevision: reconstruction.Request.Revision + 1);
        AssertAcknowledgementStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchStatus
                .DispatchAcknowledgementRejected,
            staleRequest);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus.StaleRequestRevision,
            staleRequest.AcknowledgementStatus);

        var workerMismatch = Acknowledge(
            reconstruction,
            acknowledgedWorkerId: OtherWorkerId());
        AssertAcknowledgementStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchStatus
                .DispatchAcknowledgementRejected,
            workerMismatch);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus.WorkerMismatch,
            workerMismatch.AcknowledgementStatus);

        var dispatchMismatch = Acknowledge(
            reconstruction,
            acknowledgedDispatchId: reconstruction.PriorDispatchId);
        AssertAcknowledgementStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryDispatchStatus
                .DispatchAcknowledgementRejected,
            dispatchMismatch);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus.DispatchMismatch,
            dispatchMismatch.AcknowledgementStatus);
    }

    [Xunit.Fact]
    public void RecoveryLineageAndRevisionsAdvanceExactlyOnce()
    {
        var reacquisition = RecoveryReacquisition();
        var reconstruction = RecoveryReconstruction(reacquisition);
        var acknowledgement = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumedAttemptAcknowledgement<
                    RequestPayload,
                    RecoveryWorldState>>(
                        Acknowledge(reconstruction).Acknowledgement);

        Xunit.Assert.Equal(
            reacquisition.Revision + 1,
            reconstruction.Revision);
        Xunit.Assert.Equal(
            reconstruction.Revision + 1,
            acknowledgement.Revision);
        Xunit.Assert.Same(
            reacquisition.Readmission,
            reconstruction.Reacquisition.Readmission);
        Xunit.Assert.Equal(reacquisition.RequestId, reconstruction.RequestId);
        Xunit.Assert.Equal(reacquisition.LeaseId, reconstruction.LeaseId);
        Xunit.Assert.Equal(reacquisition.WorkerId, reconstruction.WorkerId);
        Xunit.Assert.Equal(
            reacquisition.Selection.Candidate.AttemptId,
            reconstruction.PriorAttemptId);
        Xunit.Assert.Equal(
            reconstruction.Selection.SelectionId,
            acknowledgement.Attempt.Selection.SelectionId);
        Xunit.Assert.Equal(
            reconstruction.DispatchId,
            acknowledgement.Attempt.DispatchId);
    }

    [Xunit.Fact]
    public void ContractsRemainBoundedWithoutTransportSchedulingOrExecution()
    {
        var reacquisition = RecoveryReacquisition();
        var priorAttempt = reacquisition.Selection.Candidate.Attempt;
        var reconstruction = RecoveryReconstruction(reacquisition);
        var acknowledgement = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumedAttemptAcknowledgement<
                    RequestPayload,
                    RecoveryWorldState>>(
                        Acknowledge(reconstruction).Acknowledgement);

        Xunit.Assert.Same(
            priorAttempt,
            reconstruction.Reacquisition.Selection.Candidate.Attempt);
        Xunit.Assert.Same(reacquisition.Lease, reconstruction.Lease);
        Xunit.Assert.True(reconstruction.Lease.IsActive);
        Xunit.Assert.NotSame(priorAttempt, acknowledgement.Attempt);
        Xunit.Assert.Same(reconstruction.Request, acknowledgement.Attempt.Request);
        Xunit.Assert.Same(reconstruction.Lease, acknowledgement.Attempt.Lease);
        Xunit.Assert.Equal(
            reconstruction.Selection.Snapshot.QueuedCount,
            reacquisition.Admission.Snapshot.QueuedCount - 1);
        Xunit.Assert.Equal(
            priorAttempt.AttemptNumber + 1,
            acknowledgement.AttemptNumber);
    }

    private static void AssertReconstructionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryDispatchStatus expected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryDispatchReconstructionResult<
                RequestPayload,
                RecoveryWorldState> result)
    {
        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(expected, result.Status);
        Xunit.Assert.Null(result.Reconstruction);
    }

    private static void AssertAcknowledgementStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryDispatchStatus expected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryResumedAttemptAcknowledgementResult<
                RequestPayload,
                RecoveryWorldState> result)
    {
        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(expected, result.Status);
        Xunit.Assert.Null(result.Acknowledgement);
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
