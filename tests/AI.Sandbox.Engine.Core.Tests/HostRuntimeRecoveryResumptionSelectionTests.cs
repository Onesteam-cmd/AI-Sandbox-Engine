namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoveryResumptionSelectionTests
{
    private readonly record struct RequestPayload(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;

    private sealed record RecoveryWorldState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    private sealed record CancellationReason(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime
            .IHostRuntimeCancellationReason;

    private sealed class Capability :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCapability
    {
    }

    [Xunit.Fact]
    public void IdsTicksRevisionsAndArgumentsAreValidated()
    {
        var active = CreateActiveContext(1);
        var context = CreateRecoveryContext(new[] { Item(active) });

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionFlow.Plan<
                    RequestPayload,
                    RecoveryWorldState>(
                        default,
                        context.Continuation,
                        context.Continuation.Revision,
                        plannedTick: 50,
                        revision: 0));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionFlow.Plan<
                    RequestPayload,
                    RecoveryWorldState>(
                        PlanId(),
                        null!,
                        expectedContinuationRevision: 0,
                        plannedTick: 50,
                        revision: 0));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Plan(
                context.Continuation,
                expectedContinuationRevision: -1));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Plan(context.Continuation, plannedTick: -1));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Plan(context.Continuation, revision: -1));

        var plan = ResumptionPlan(context.Continuation);

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionFlow.Select<
                    RequestPayload,
                    RecoveryWorldState>(
                        default,
                        plan,
                        plan.Revision,
                        active.Attempt.AttemptId,
                        selectedTick: 51));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionFlow.Select<
                    RequestPayload,
                    RecoveryWorldState>(
                        SelectionId(),
                        null!,
                        expectedPlanRevision: 0,
                        active.Attempt.AttemptId,
                        selectedTick: 51));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Select(
                plan,
                active.Attempt.AttemptId,
                expectedPlanRevision: -1));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionFlow.Select<
                    RequestPayload,
                    RecoveryWorldState>(
                        SelectionId(),
                        plan,
                        plan.Revision,
                        default,
                        selectedTick: 51));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Select(
                plan,
                active.Attempt.AttemptId,
                selectedTick: -1));
    }

    [Xunit.Fact]
    public void PlanBuildsDeterministicPendingCandidatesAndSuppressesCancellation()
    {
        var second = CreateActiveContext(2);
        var first = CreateActiveContext(1);
        var cancelled = CreateActiveContext(3);
        var cancellation = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.RequestCancellation(
                cancelled.Request,
                cancelled.Request.Revision,
                new CancellationReason("cancel")).Envelope;
        var cancelledItem = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkItem<RequestPayload>.Create(
                cancelled.Attempt,
                cancellation,
                cancelled.Lease);
        var context = CreateRecoveryContext(
            new[] { Item(second), cancelledItem, Item(first) });

        var result = Plan(
            context.Continuation,
            plannedTick: 50,
            revision: 7);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionStatus.PlanCreated,
            result.Status);
        Xunit.Assert.Same(context.Continuation, result.Continuation);

        var plan = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionPlan<
                    RequestPayload,
                    RecoveryWorldState>>(result.Plan);
        Xunit.Assert.Equal(PlanId(), plan.PlanId);
        Xunit.Assert.Equal(RuntimeId(), plan.RuntimeInstanceId);
        Xunit.Assert.Equal(ClockId(), plan.ClockId);
        Xunit.Assert.Equal(50, plan.PlannedTick);
        Xunit.Assert.Equal(7, plan.Revision);
        Xunit.Assert.Equal(2, plan.CandidateCount);
        Xunit.Assert.Equal(1, plan.SuppressedCount);
        Xunit.Assert.Same(first.Attempt, plan.Candidates[0].Attempt);
        Xunit.Assert.Same(second.Attempt, plan.Candidates[1].Attempt);
        Xunit.Assert.Single(plan.SuppressedAttemptIds);
        Xunit.Assert.Equal(
            cancelled.Attempt.AttemptId,
            plan.SuppressedAttemptIds[0]);
    }

    [Xunit.Fact]
    public void PlanStaleContinuationRevisionAndTickRegressionAreExplicit()
    {
        var active = CreateActiveContext(1);
        var context = CreateRecoveryContext(new[] { Item(active) });

        AssertPlanStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionStatus
                .StaleContinuationRevision,
            Plan(
                context.Continuation,
                expectedContinuationRevision:
                    context.Continuation.Revision + 1));

        AssertPlanStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionStatus
                .PlanningTickRegressed,
            Plan(
                context.Continuation,
                plannedTick: context.Continuation.ContinuedTick - 1));
    }

    [Xunit.Fact]
    public void PlanWithOnlyCancellationRequestedWorkIsExplicit()
    {
        var active = CreateActiveContext(1);
        var cancellation = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.RequestCancellation(
                active.Request,
                active.Request.Revision,
                new CancellationReason("cancel")).Envelope;
        var cancelledItem = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkItem<RequestPayload>.Create(
                active.Attempt,
                cancellation,
                active.Lease);
        var context = CreateRecoveryContext(new[] { cancelledItem });

        var result = Plan(context.Continuation);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionStatus.NoResumableWork,
            result.Status);
        Xunit.Assert.Same(context.Continuation, result.Continuation);
        var plan = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionPlan<
                    RequestPayload,
                    RecoveryWorldState>>(result.Plan);
        Xunit.Assert.Empty(plan.Candidates);
        Xunit.Assert.Single(plan.SuppressedAttemptIds);
        Xunit.Assert.Equal(
            active.Attempt.AttemptId,
            plan.SuppressedAttemptIds[0]);
    }

    [Xunit.Fact]
    public void SelectionPreservesExactCandidateAuthorityAndAdvancesRevision()
    {
        var first = CreateActiveContext(1);
        var second = CreateActiveContext(2);
        var context = CreateRecoveryContext(
            new[] { Item(first), Item(second) });
        var plan = ResumptionPlan(
            context.Continuation,
            plannedTick: 50,
            revision: 9);

        var result = Select(
            plan,
            second.Attempt.AttemptId,
            selectedTick: 52);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionStatus.SelectionCreated,
            result.Status);
        Xunit.Assert.Same(plan, result.Plan);
        Xunit.Assert.Equal(second.Attempt.AttemptId, result.RelatedAttemptId);

        var selection = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeResumedWorkSelection<
                    RequestPayload,
                    RecoveryWorldState>>(result.Selection);
        Xunit.Assert.Equal(SelectionId(), selection.SelectionId);
        Xunit.Assert.Same(plan, selection.Plan);
        Xunit.Assert.Same(second.Attempt, selection.Candidate.Attempt);
        Xunit.Assert.Same(second.Request, selection.Candidate.Request);
        Xunit.Assert.Same(second.Lease, selection.Candidate.Lease);
        Xunit.Assert.Equal(second.Attempt.AttemptId, selection.AttemptId);
        Xunit.Assert.Equal(second.Request.RequestId, selection.RequestId);
        Xunit.Assert.Equal(second.Lease.LeaseId, selection.LeaseId);
        Xunit.Assert.Equal(second.Lease.WorkerId, selection.WorkerId);
        Xunit.Assert.Equal(second.Attempt.DispatchId, selection.DispatchId);
        Xunit.Assert.Equal(52, selection.SelectedTick);
        Xunit.Assert.Equal(10, selection.Revision);
    }

    [Xunit.Fact]
    public void SelectionStaleRevisionAndTickRegressionAreExplicit()
    {
        var active = CreateActiveContext(1);
        var context = CreateRecoveryContext(new[] { Item(active) });
        var plan = ResumptionPlan(
            context.Continuation,
            plannedTick: 50,
            revision: 4);

        AssertSelectionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionStatus.StalePlanRevision,
            Select(
                plan,
                active.Attempt.AttemptId,
                expectedPlanRevision: 5));

        AssertSelectionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionStatus.SelectionTickRegressed,
            Select(
                plan,
                active.Attempt.AttemptId,
                selectedTick: 49));
    }

    [Xunit.Fact]
    public void SelectionMissingCandidateIsExplicit()
    {
        var active = CreateActiveContext(1);
        var context = CreateRecoveryContext(new[] { Item(active) });
        var plan = ResumptionPlan(context.Continuation);
        var missingAttemptId = Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptIdKind>(449999);

        var result = Select(plan, missingAttemptId);

        AssertSelectionStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionStatus.AttemptNotPlanned,
            result);
        Xunit.Assert.Equal(missingAttemptId, result.RelatedAttemptId);
    }

    [Xunit.Fact]
    public void PlanCollectionsAreDefensivelyOwnedAndDeterministicallyBounded()
    {
        var pending = CreateActiveContext(1);
        var cancelled = CreateActiveContext(2);
        var cancellation = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.RequestCancellation(
                cancelled.Request,
                cancelled.Request.Revision,
                new CancellationReason("cancel")).Envelope;
        var cancelledItem = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkItem<RequestPayload>.Create(
                cancelled.Attempt,
                cancellation,
                cancelled.Lease);
        var mutable = new List<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkItem<RequestPayload>>
        {
            Item(pending),
            cancelledItem,
        };
        var context = CreateRecoveryContext(mutable);
        mutable.Add(Item(CreateActiveContext(3)));

        var plan = ResumptionPlan(context.Continuation);

        Xunit.Assert.Equal(1, plan.CandidateCount);
        Xunit.Assert.Equal(1, plan.SuppressedCount);
        Xunit.Assert.Equal(
            context.Continuation.Checkpoint.ActiveWorkSnapshot.Count,
            plan.CandidateCount + plan.SuppressedCount);

        var candidateList = Xunit.Assert.IsAssignableFrom<
            IList<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeActiveWorkItem<RequestPayload>>>(
                        plan.Candidates);
        Xunit.Assert.Throws<NotSupportedException>(
            () => candidateList.Add(Item(CreateActiveContext(4))));

        var suppressedList = Xunit.Assert.IsAssignableFrom<
            IList<
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeAttemptIdKind>>>(
                            plan.SuppressedAttemptIds);
        Xunit.Assert.Throws<NotSupportedException>(
            () => suppressedList.Add(
                Id<
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeAttemptIdKind>(449998)));
    }

    [Xunit.Fact]
    public void ContractsRemainAdvisoryWithoutRestartSchedulingOrExecution()
    {
        var active = CreateActiveContext(1);
        var context = CreateRecoveryContext(new[] { Item(active) });
        var checkpoint = context.Continuation.Checkpoint;
        var plan = ResumptionPlan(context.Continuation);
        var selection = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeResumedWorkSelection<
                    RequestPayload,
                    RecoveryWorldState>>(
                        Select(plan, active.Attempt.AttemptId).Selection);

        Xunit.Assert.Same(context.Continuation, plan.Continuation);
        Xunit.Assert.Same(
            checkpoint.ActiveWorkSnapshot.Items[0],
            selection.Candidate);
        Xunit.Assert.Same(active.Attempt, selection.Candidate.Attempt);
        Xunit.Assert.Same(active.Request, selection.Candidate.Request);
        Xunit.Assert.Same(active.Lease, selection.Candidate.Lease);
        Xunit.Assert.True(selection.Candidate.Lease.IsActive);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Pending,
            selection.Candidate.Request.State);
        Xunit.Assert.Same(
            checkpoint.QueueSnapshot,
            selection.Plan.Continuation.Checkpoint.QueueSnapshot);
        Xunit.Assert.Equal(0, checkpoint.QueueSnapshot.QueuedCount);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleState.Created,
            checkpoint.LifecycleSnapshot.State);
    }

    private static void AssertPlanStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryResumptionStatus expected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryResumptionPlanResult<
                RequestPayload,
                RecoveryWorldState> result)
    {
        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(expected, result.Status);
        Xunit.Assert.Null(result.Plan);
    }

    private static void AssertSelectionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryResumptionStatus expected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeResumedWorkSelectionResult<
                RequestPayload,
                RecoveryWorldState> result)
    {
        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(expected, result.Status);
        Xunit.Assert.Null(result.Selection);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryResumptionPlanResult<
            RequestPayload,
            RecoveryWorldState>
        Plan(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuation<
                    RequestPayload,
                    RecoveryWorldState> continuation,
            long? expectedContinuationRevision = null,
            long plannedTick = 50,
            long revision = 0) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryResumptionFlow.Plan<
                RequestPayload,
                RecoveryWorldState>(
                    PlanId(),
                    continuation,
                    expectedContinuationRevision ?? continuation.Revision,
                    plannedTick,
                    revision);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryResumptionPlan<
            RequestPayload,
            RecoveryWorldState>
        ResumptionPlan(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuation<
                    RequestPayload,
                    RecoveryWorldState> continuation,
            long plannedTick = 50,
            long revision = 0) =>
        Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionPlan<
                    RequestPayload,
                    RecoveryWorldState>>(
                        Plan(
                            continuation,
                            plannedTick: plannedTick,
                            revision: revision).Plan);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeResumedWorkSelectionResult<
            RequestPayload,
            RecoveryWorldState>
        Select(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionPlan<
                    RequestPayload,
                    RecoveryWorldState> plan,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeAttemptIdKind> attemptId,
            long? expectedPlanRevision = null,
            long selectedTick = 51) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryResumptionFlow.Select<
                RequestPayload,
                RecoveryWorldState>(
                    SelectionId(),
                    plan,
                    expectedPlanRevision ?? plan.Revision,
                    attemptId,
                    selectedTick);

    private static RecoveryContext CreateRecoveryContext(
        IEnumerable<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkItem<RequestPayload>> items)
    {
        var composition = Composition();
        var lifecycle =
            global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLifecycle
                .Create(RuntimeId(), composition.CompositionId);
        var queue =
            global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueSnapshot
                .Create(
                    RecoveryQueueId(),
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeQueueCapacity.Create(8),
                    queuedCount: 0,
                    revision: 2);
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

        return new RecoveryContext(
            continuation,
            persistence,
            document);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeActiveWorkItem<RequestPayload>
        Item(ActiveWorkContext context) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkItem<RequestPayload>.Create(
                context.Attempt,
                context.Request,
                context.Lease);

    private static ActiveWorkContext CreateActiveContext(int seed)
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
        var queueSnapshot =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueSnapshot.Create(
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeQueueIdKind>(Suffix(seed, 4)),
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeQueueCapacity.Create(8),
                    queuedCount: 0,
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
        var selectionResult =
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
        var selection = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelection<RequestPayload>>(
                    selectionResult.Selection);
        var acknowledgement =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementFlow.Acknowledge(
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeAttemptIdKind>(Suffix(seed, 12)),
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
                .HostRuntimeInFlightAttempt<RequestPayload>>(
                    acknowledgement.Attempt);

        return new ActiveWorkContext(
            admission,
            pending,
            lease,
            attempt);
    }

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
        440000 + (seed * 100) + offset;

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>
        Id<TKind>(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(
            $"019f0000-0000-7000-8000-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryResumptionPlanIdKind>
        PlanId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryResumptionPlanIdKind>(449001);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeResumedWorkSelectionIdKind>
        SelectionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeResumedWorkSelectionIdKind>(449002);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointIdKind>
        CheckpointId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointIdKind>(449003);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuationIdKind>
        ContinuationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuationIdKind>(449004);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeInstanceIdKind>
        RuntimeId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInstanceIdKind>(449005);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompositionIdKind>
        CompositionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompositionIdKind>(449006);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCapabilityIdKind>
        CapabilityId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCapabilityIdKind>(449007);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueIdKind>
        RecoveryQueueId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueIdKind>(449008);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkSnapshotIdKind>
        ActiveWorkSnapshotId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkSnapshotIdKind>(449009);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        ClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(449010);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>
        WorldId() => Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>(449011);

    private sealed class RecoveryCodec :
        global::AI.Sandbox.Engine.Core.Persistence
            .IWorldStateSnapshotCodec<RecoveryWorldState>
    {
        public global::AI.Sandbox.Engine.Core.Persistence.PersistenceSchemaId
            SchemaId { get; } =
            global::AI.Sandbox.Engine.Core.Persistence.PersistenceSchemaId
                .Parse("host.recovery-resumption");

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
                        "Invalid recovery resumption state.");
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
                RecoveryWorldState> Continuation,
        global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateSnapshotPersistence<RecoveryWorldState> Persistence,
        global::AI.Sandbox.Engine.Core.Persistence
            .WorldSnapshotDocument WorldDocument);
}
