namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoveryReadmissionLeaseReacquisitionTests
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
    public void IdsTicksRevisionsDurationsAndArgumentsAreValidated()
    {
        var context = CreateSelectionContext();
        var snapshot = CurrentQueueSnapshot();

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionFlow.Readmit<
                    RequestPayload,
                    RecoveryWorldState>(
                        default,
                        RecoveryAdmissionId(),
                        context.Selection,
                        context.Selection.Revision,
                        snapshot,
                        snapshot.Revision,
                        readmittedTick: 60));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionFlow.Readmit<
                    RequestPayload,
                    RecoveryWorldState>(
                        ReadmissionId(),
                        default,
                        context.Selection,
                        context.Selection.Revision,
                        snapshot,
                        snapshot.Revision,
                        readmittedTick: 60));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionFlow.Readmit<
                    RequestPayload,
                    RecoveryWorldState>(
                        ReadmissionId(),
                        RecoveryAdmissionId(),
                        null!,
                        expectedSelectionRevision: 0,
                        snapshot,
                        snapshot.Revision,
                        readmittedTick: 60));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionFlow.Readmit<
                    RequestPayload,
                    RecoveryWorldState>(
                        ReadmissionId(),
                        RecoveryAdmissionId(),
                        context.Selection,
                        context.Selection.Revision,
                        null!,
                        expectedQueueRevision: 0,
                        readmittedTick: 60));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Readmit(
                context.Selection,
                snapshot,
                expectedSelectionRevision: -1));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Readmit(
                context.Selection,
                snapshot,
                expectedQueueRevision: -1));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Readmit(
                context.Selection,
                snapshot,
                readmittedTick: -1));

        var readmission = RecoveryReadmission(context.Selection, snapshot);

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionFlow.Reacquire<
                    RequestPayload,
                    RecoveryWorldState>(
                        default,
                        RecoveryLeaseId(),
                        RecoveryWorkerId(),
                        readmission,
                        readmission.Revision,
                        ClockId(),
                        reacquiredTick: 61,
                        durationTicks: 100));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionFlow.Reacquire<
                    RequestPayload,
                    RecoveryWorldState>(
                        ReacquisitionId(),
                        default,
                        RecoveryWorkerId(),
                        readmission,
                        readmission.Revision,
                        ClockId(),
                        reacquiredTick: 61,
                        durationTicks: 100));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionFlow.Reacquire<
                    RequestPayload,
                    RecoveryWorldState>(
                        ReacquisitionId(),
                        RecoveryLeaseId(),
                        default,
                        readmission,
                        readmission.Revision,
                        ClockId(),
                        reacquiredTick: 61,
                        durationTicks: 100));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionFlow.Reacquire<
                    RequestPayload,
                    RecoveryWorldState>(
                        ReacquisitionId(),
                        RecoveryLeaseId(),
                        RecoveryWorkerId(),
                        null!,
                        expectedReadmissionRevision: 0,
                        ClockId(),
                        reacquiredTick: 61,
                        durationTicks: 100));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Reacquire(
                readmission,
                expectedReadmissionRevision: -1));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionFlow.Reacquire<
                    RequestPayload,
                    RecoveryWorldState>(
                        ReacquisitionId(),
                        RecoveryLeaseId(),
                        RecoveryWorkerId(),
                        readmission,
                        readmission.Revision,
                        default,
                        reacquiredTick: 61,
                        durationTicks: 100));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Reacquire(readmission, reacquiredTick: -1));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Reacquire(readmission, durationTicks: 0));
    }

    [Xunit.Fact]
    public void ReadmissionPreservesSelectionAndCreatesAdmission()
    {
        var context = CreateSelectionContext();
        var snapshot = CurrentQueueSnapshot(queuedCount: 2, revision: 20);

        var result = Readmit(
            context.Selection,
            snapshot,
            readmittedTick: 60);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionStatus.Readmitted,
            result.Status);
        Xunit.Assert.Same(context.Selection, result.Selection);
        Xunit.Assert.Equal(3, result.Snapshot.QueuedCount);
        Xunit.Assert.Equal(21, result.Snapshot.Revision);

        var readmission = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmission<
                    RequestPayload,
                    RecoveryWorldState>>(result.Readmission);
        Xunit.Assert.Equal(ReadmissionId(), readmission.ReadmissionId);
        Xunit.Assert.Same(context.Selection, readmission.Selection);
        Xunit.Assert.Same(
            context.Selection.Candidate.Request,
            readmission.Request);
        Xunit.Assert.Equal(RecoveryAdmissionId(), readmission.AdmissionId);
        Xunit.Assert.Equal(
            context.Active.Admission.AdmissionId,
            readmission.PriorAdmissionId);
        Xunit.Assert.Equal(RecoveryQueueId(), readmission.QueueId);
        Xunit.Assert.Equal(context.Active.Attempt.AttemptId, readmission.AttemptId);
        Xunit.Assert.Same(
            context.Active.Admission.Priority,
            readmission.Admission.Priority);
        Xunit.Assert.Equal(60, readmission.ReadmittedTick);
        Xunit.Assert.Equal(
            context.Selection.Revision + 1,
            readmission.Revision);
    }

    [Xunit.Fact]
    public void ReadmissionStaleSelectionRevisionAndTickRegressionAreExplicit()
    {
        var context = CreateSelectionContext();
        var snapshot = CurrentQueueSnapshot();

        AssertReadmissionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionStatus.StaleSelectionRevision,
            Readmit(
                context.Selection,
                snapshot,
                expectedSelectionRevision:
                    context.Selection.Revision + 1));

        AssertReadmissionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionStatus.ReadmissionTickRegressed,
            Readmit(
                context.Selection,
                snapshot,
                readmittedTick: context.Selection.SelectedTick - 1));
    }

    [Xunit.Fact]
    public void ReadmissionQueueLineageAndPriorAdmissionReuseAreExplicit()
    {
        var mismatched = CreateSelectionContext(mismatchedPriorQueue: true);
        var snapshot = CurrentQueueSnapshot();

        AssertReadmissionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionStatus.QueueMismatch,
            Readmit(mismatched.Selection, snapshot));

        var context = CreateSelectionContext();
        var otherQueue = CurrentQueueSnapshot(queueId: OtherQueueId());

        AssertReadmissionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionStatus.QueueMismatch,
            Readmit(context.Selection, otherQueue));

        AssertReadmissionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionStatus.PriorAdmissionIdReused,
            Readmit(
                context.Selection,
                snapshot,
                admissionId: context.Active.Admission.AdmissionId));
    }

    [Xunit.Fact]
    public void ReadmissionAdmissionFailuresPreserveQueueAuthority()
    {
        var context = CreateSelectionContext();
        var snapshot = CurrentQueueSnapshot(revision: 20);

        var stale = Readmit(
            context.Selection,
            snapshot,
            expectedQueueRevision: 19);
        AssertReadmissionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionStatus.StaleQueueRevision,
            stale);
        Xunit.Assert.Same(snapshot, stale.Snapshot);

        var full = CurrentQueueSnapshot(
            queuedCount: 8,
            revision: 21);
        var fullResult = Readmit(context.Selection, full);
        AssertReadmissionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionStatus.QueueFull,
            fullResult);
        Xunit.Assert.Same(full, fullResult.Snapshot);
    }

    [Xunit.Fact]
    public void ReacquisitionCreatesNewLeaseAndAdvancesRevision()
    {
        var context = CreateSelectionContext();
        var readmission = RecoveryReadmission(
            context.Selection,
            CurrentQueueSnapshot());

        var result = Reacquire(
            readmission,
            reacquiredTick: 65,
            durationTicks: 120);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionStatus.LeaseReacquired,
            result.Status);
        Xunit.Assert.Same(readmission, result.Readmission);

        var reacquisition = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryLeaseReacquisition<
                    RequestPayload,
                    RecoveryWorldState>>(result.Reacquisition);
        Xunit.Assert.Equal(
            ReacquisitionId(),
            reacquisition.ReacquisitionId);
        Xunit.Assert.Same(readmission, reacquisition.Readmission);
        Xunit.Assert.Same(
            readmission.Admission,
            reacquisition.Lease.Admission);
        Xunit.Assert.Equal(RecoveryLeaseId(), reacquisition.LeaseId);
        Xunit.Assert.Equal(RecoveryWorkerId(), reacquisition.WorkerId);
        Xunit.Assert.Equal(ClockId(), reacquisition.ClockId);
        Xunit.Assert.Equal(65, reacquisition.Lease.AcquiredTick);
        Xunit.Assert.Equal(185, reacquisition.Lease.ExpiresTick);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseState.Active,
            reacquisition.Lease.State);
        Xunit.Assert.Equal(0, reacquisition.Lease.Revision);
        Xunit.Assert.Equal(65, reacquisition.ReacquiredTick);
        Xunit.Assert.Equal(
            readmission.Revision + 1,
            reacquisition.Revision);
    }

    [Xunit.Fact]
    public void ReacquisitionStaleRevisionTickClockAndPriorLeaseReuseAreExplicit()
    {
        var context = CreateSelectionContext();
        var readmission = RecoveryReadmission(
            context.Selection,
            CurrentQueueSnapshot());

        AssertReacquisitionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionStatus.StaleReadmissionRevision,
            Reacquire(
                readmission,
                expectedReadmissionRevision:
                    readmission.Revision + 1));

        AssertReacquisitionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionStatus
                .ReacquisitionTickRegressed,
            Reacquire(
                readmission,
                reacquiredTick: readmission.ReadmittedTick - 1));

        AssertReacquisitionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionStatus.ClockMismatch,
            Reacquire(readmission, clockId: OtherClockId()));

        AssertReacquisitionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionStatus.PriorLeaseIdReused,
            Reacquire(
                readmission,
                leaseId: context.Active.Lease.LeaseId));
    }

    [Xunit.Fact]
    public void ReacquisitionPreservesRequestQueueAttemptAndWorkerLineage()
    {
        var context = CreateSelectionContext();
        var readmission = RecoveryReadmission(
            context.Selection,
            CurrentQueueSnapshot());
        var result = Reacquire(readmission);
        var reacquisition = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryLeaseReacquisition<
                    RequestPayload,
                    RecoveryWorldState>>(result.Reacquisition);

        Xunit.Assert.Same(context.Active.Request, reacquisition.Request);
        Xunit.Assert.Equal(
            context.Active.Request.RequestId,
            reacquisition.RequestId);
        Xunit.Assert.Equal(
            context.Active.Attempt.AttemptId,
            reacquisition.AttemptId);
        Xunit.Assert.Equal(RecoveryQueueId(), reacquisition.QueueId);
        Xunit.Assert.Equal(
            context.Active.Lease.LeaseId,
            reacquisition.PriorLeaseId);
        Xunit.Assert.NotEqual(
            reacquisition.PriorLeaseId,
            reacquisition.LeaseId);
        Xunit.Assert.Equal(
            context.Active.Lease.WorkerId,
            context.Selection.WorkerId);
        Xunit.Assert.Equal(RecoveryWorkerId(), reacquisition.WorkerId);
        Xunit.Assert.Equal(
            context.Active.Attempt.DispatchId,
            reacquisition.Selection.DispatchId);
    }

    [Xunit.Fact]
    public void ContractsRemainBoundedWithoutDispatchSchedulingOrExecution()
    {
        var context = CreateSelectionContext();
        var originalAttempt = context.Active.Attempt;
        var originalLease = context.Active.Lease;
        var snapshot = CurrentQueueSnapshot();
        var readmission = RecoveryReadmission(context.Selection, snapshot);
        var reacquisition = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryLeaseReacquisition<
                    RequestPayload,
                    RecoveryWorldState>>(
                        Reacquire(readmission).Reacquisition);

        Xunit.Assert.Same(originalAttempt, context.Selection.Candidate.Attempt);
        Xunit.Assert.Same(originalLease, context.Selection.Candidate.Lease);
        Xunit.Assert.True(originalLease.IsActive);
        Xunit.Assert.True(reacquisition.Lease.IsActive);
        Xunit.Assert.NotSame(originalLease, reacquisition.Lease);
        Xunit.Assert.Same(
            context.Selection.Candidate.Request,
            reacquisition.Request);
        Xunit.Assert.Equal(1, readmission.Admission.Snapshot.QueuedCount);
        Xunit.Assert.Equal(
            context.Selection.Candidate.AttemptNumber,
            originalAttempt.AttemptNumber);
        Xunit.Assert.Equal(
            context.Selection.DispatchId,
            originalAttempt.DispatchId);
    }

    private static void AssertReadmissionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryReadmissionStatus expected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryReadmissionResult<
                RequestPayload,
                RecoveryWorldState> result)
    {
        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(expected, result.Status);
        Xunit.Assert.Null(result.Readmission);
    }

    private static void AssertReacquisitionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryReadmissionStatus expected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryLeaseReacquisitionResult<
                RequestPayload,
                RecoveryWorldState> result)
    {
        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(expected, result.Status);
        Xunit.Assert.Null(result.Reacquisition);
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
        450000 + (seed * 100) + offset;

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>
        Id<TKind>(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(
            $"019f0000-0000-7000-8000-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryReadmissionIdKind>
        ReadmissionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryReadmissionIdKind>(459001);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryLeaseReacquisitionIdKind>
        ReacquisitionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryLeaseReacquisitionIdKind>(459002);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeAdmissionIdKind>
        RecoveryAdmissionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAdmissionIdKind>(459003);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseIdKind>
        RecoveryLeaseId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseIdKind>(459004);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkerIdKind>
        RecoveryWorkerId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkerIdKind>(459005);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryResumptionPlanIdKind>
        PlanId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionPlanIdKind>(459006);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeResumedWorkSelectionIdKind>
        SelectionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeResumedWorkSelectionIdKind>(459007);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointIdKind>
        CheckpointId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointIdKind>(459008);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuationIdKind>
        ContinuationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuationIdKind>(459009);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeInstanceIdKind>
        RuntimeId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInstanceIdKind>(459010);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompositionIdKind>
        CompositionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompositionIdKind>(459011);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCapabilityIdKind>
        CapabilityId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCapabilityIdKind>(459012);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueIdKind>
        RecoveryQueueId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueIdKind>(459013);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueIdKind>
        OtherQueueId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueIdKind>(459014);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkSnapshotIdKind>
        ActiveWorkSnapshotId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkSnapshotIdKind>(459015);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        ClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(459016);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        OtherClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(459017);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>
        WorldId() => Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>(459018);

    private sealed class RecoveryCodec :
        global::AI.Sandbox.Engine.Core.Persistence
            .IWorldStateSnapshotCodec<RecoveryWorldState>
    {
        public global::AI.Sandbox.Engine.Core.Persistence.PersistenceSchemaId
            SchemaId { get; } =
            global::AI.Sandbox.Engine.Core.Persistence.PersistenceSchemaId
                .Parse("host.recovery-readmission");

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
                        "Invalid recovery readmission state.");
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
