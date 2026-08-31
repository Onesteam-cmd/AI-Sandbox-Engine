namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoveryCheckpointSupersessionSummaryTests
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
    var cycle = RecoveryCycleCompletion();
    var successor = SuccessorCheckpoint(cycle);

    Xunit.Assert.Throws<ArgumentException>(
        () => global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionFlow.Supersede<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>(
                    default,
                    cycle,
                    cycle.Revision,
                    successor,
                    successor.Revision,
                    supersededTick: 91));
    Xunit.Assert.Throws<ArgumentNullException>(
        () => global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionFlow.Supersede<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>(
                    CheckpointSupersessionId(),
                    null!,
                    expectedCycleCompletionRevision: 0,
                    successor,
                    successor.Revision,
                    supersededTick: 91));
    Xunit.Assert.Throws<ArgumentOutOfRangeException>(
        () => Supersede(
            cycle,
            successor,
            expectedCycleCompletionRevision: -1));
    Xunit.Assert.Throws<ArgumentNullException>(
        () => global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionFlow.Supersede<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>(
                    CheckpointSupersessionId(),
                    cycle,
                    cycle.Revision,
                    null!,
                    expectedSuccessorCheckpointRevision: 0,
                    supersededTick: 91));
    Xunit.Assert.Throws<ArgumentOutOfRangeException>(
        () => Supersede(
            cycle,
            successor,
            expectedSuccessorCheckpointRevision: -1));
    Xunit.Assert.Throws<ArgumentOutOfRangeException>(
        () => Supersede(cycle, successor, supersededTick: -1));

    var supersession = CheckpointSupersession(cycle, successor);

    Xunit.Assert.Throws<ArgumentException>(
        () => global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionFlow.Summarize<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>(
                    default,
                    supersession,
                    supersession.Revision,
                    summarizedTick: 92));
    Xunit.Assert.Throws<ArgumentNullException>(
        () => global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionFlow.Summarize<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>(
                    CompletedCycleSummaryId(),
                    null!,
                    expectedSupersessionRevision: 0,
                    summarizedTick: 92));
    Xunit.Assert.Throws<ArgumentOutOfRangeException>(
        () => Summarize(
            supersession,
            expectedSupersessionRevision: -1));
    Xunit.Assert.Throws<ArgumentOutOfRangeException>(
        () => Summarize(supersession, summarizedTick: -1));
}

[Xunit.Fact]
public void OptimisticRevisionsAreExplicit()
{
    var cycle = RecoveryCycleCompletion();
    var successor = SuccessorCheckpoint(cycle);

    AssertSupersessionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionStatus
            .StaleCycleCompletionRevision,
        Supersede(
            cycle,
            successor,
            expectedCycleCompletionRevision: cycle.Revision + 1));
    AssertSupersessionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionStatus
            .StaleSuccessorCheckpointRevision,
        Supersede(
            cycle,
            successor,
            expectedSuccessorCheckpointRevision:
                successor.Revision + 1));

    var supersession = CheckpointSupersession(cycle, successor);
    AssertSummaryStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionStatus
            .StaleSupersessionRevision,
        Summarize(
            supersession,
            expectedSupersessionRevision: supersession.Revision + 1));
}

[Xunit.Fact]
public void CheckpointIdentityRevisionAndTimeMustAdvance()
{
    var cycle = RecoveryCycleCompletion();

    AssertSupersessionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionStatus
            .CheckpointIdReused,
        Supersede(
            cycle,
            SuccessorCheckpoint(
                cycle,
                checkpointId: cycle.Checkpoint.CheckpointId)));
    AssertSupersessionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionStatus
            .SuccessorCheckpointTickRegressed,
        Supersede(
            cycle,
            SuccessorCheckpoint(
                cycle,
                capturedTick: cycle.CompletedTick - 1)));
    AssertSupersessionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionStatus
            .SuccessorCheckpointRevisionNotAdvanced,
        Supersede(
            cycle,
            SuccessorCheckpoint(
                cycle,
                revision: cycle.Checkpoint.Revision)));
    var successor = SuccessorCheckpoint(cycle);
    AssertSupersessionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionStatus
            .SupersessionTickRegressed,
        Supersede(
            cycle,
            successor,
            supersededTick: successor.CapturedTick - 1));
}

[Xunit.Fact]
public void RuntimeCompositionQueueClockAndWorldLineageMustMatch()
{
    var cycle = RecoveryCycleCompletion();

    AssertSupersessionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionStatus.RuntimeMismatch,
        Supersede(
            cycle,
            SuccessorCheckpoint(cycle, runtimeId: OtherRuntimeId())));
    AssertSupersessionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionStatus
            .CompositionMismatch,
        Supersede(
            cycle,
            SuccessorCheckpoint(
                cycle,
                compositionId: OtherCompositionId())));
    AssertSupersessionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionStatus.QueueMismatch,
        Supersede(
            cycle,
            SuccessorCheckpoint(cycle, queueId: OtherQueueId())));
    AssertSupersessionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionStatus.ClockMismatch,
        Supersede(
            cycle,
            SuccessorCheckpoint(cycle, clockId: OtherClockId())));
    AssertSupersessionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionStatus.WorldMismatch,
        Supersede(
            cycle,
            SuccessorCheckpoint(cycle, worldId: OtherWorldId())));
}

[Xunit.Fact]
public void WorldSnapshotAuthorityCannotRegress()
{
    var cycle = RecoveryCycleCompletion();
    var priorDocument = cycle.Checkpoint.WorldSnapshotDocument;

    AssertSupersessionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionStatus
            .SimulationTickRegressed,
        Supersede(
            cycle,
            SuccessorCheckpoint(
                cycle,
                simulationTick: priorDocument.SimulationTick - 1)));
}

[Xunit.Fact]
public void SupersessionPreservesBothCheckpointAuthorities()
{
    var cycle = RecoveryCycleCompletion();
    var successor = SuccessorCheckpoint(cycle);
    var result = Supersede(
        cycle,
        successor,
        supersededTick: successor.CapturedTick + 1);
    var supersession = Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersession<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(result.Supersession);

    Xunit.Assert.True(result.Succeeded);
    Xunit.Assert.Equal(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionStatus
            .CheckpointSuperseded,
        result.Status);
    Xunit.Assert.Same(cycle, result.CycleCompletion);
    Xunit.Assert.Same(successor, result.SuccessorCheckpoint);
    Xunit.Assert.Same(cycle, supersession.CycleCompletion);
    Xunit.Assert.Same(cycle.Checkpoint, supersession.PriorCheckpoint);
    Xunit.Assert.Same(successor, supersession.SuccessorCheckpoint);
    Xunit.Assert.Equal(
        CheckpointSupersessionId(),
        supersession.SupersessionId);
    Xunit.Assert.Equal(
        global::System.Math.Max(cycle.Revision, successor.Revision) + 1,
        supersession.Revision);
    Xunit.Assert.Equal(
        successor.CapturedTick + 1,
        supersession.SupersededTick);
    Xunit.Assert.Equal(cycle.OutcomeKind, supersession.OutcomeKind);
    Xunit.Assert.Equal(
        cycle.Checkpoint.CheckpointId,
        supersession.PriorCheckpointId);
    Xunit.Assert.Equal(
        successor.CheckpointId,
        supersession.SuccessorCheckpointId);
}

[Xunit.Fact]
public void SummaryRevisionAndTimeAreExplicit()
{
    var supersession = CheckpointSupersession();

    AssertSummaryStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionStatus
            .StaleSupersessionRevision,
        Summarize(
            supersession,
            expectedSupersessionRevision: supersession.Revision + 1));
    AssertSummaryStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionStatus
            .SummaryTickRegressed,
        Summarize(
            supersession,
            summarizedTick: supersession.SupersededTick - 1));
}

[Xunit.Fact]
public void SummaryIsACompactImmutableProjection()
{
    var supersession = CheckpointSupersession();
    var result = Summarize(
        supersession,
        summarizedTick: supersession.SupersededTick + 1);
    var summary = Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCompletedCycleSummary<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(result.Summary);

    Xunit.Assert.True(result.Succeeded);
    Xunit.Assert.Equal(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionStatus.SummaryCreated,
        result.Status);
    Xunit.Assert.Same(supersession, result.Supersession);
    Xunit.Assert.Same(supersession, summary.Supersession);
    Xunit.Assert.Equal(CompletedCycleSummaryId(), summary.SummaryId);
    Xunit.Assert.Equal(supersession.Revision + 1, summary.Revision);
    Xunit.Assert.Equal(
        supersession.SupersededTick + 1,
        summary.SummarizedTick);
    Xunit.Assert.Equal(
        supersession.PriorCheckpointId,
        summary.PriorCheckpointId);
    Xunit.Assert.Equal(
        supersession.SuccessorCheckpointId,
        summary.SuccessorCheckpointId);
    Xunit.Assert.Equal(
        supersession.CycleCompletion.RequestId,
        summary.RequestId);
    Xunit.Assert.Equal(
        supersession.CycleCompletion.AttemptId,
        summary.AttemptId);
    Xunit.Assert.Equal(supersession.OutcomeKind, summary.OutcomeKind);
    Xunit.Assert.Equal(
        supersession.PriorCheckpoint.WorldSnapshotDocument
            .WorldStateVersion,
        summary.PriorWorldStateVersion);
    Xunit.Assert.Equal(
        supersession.SuccessorCheckpoint.WorldSnapshotDocument
            .WorldStateVersion,
        summary.SuccessorWorldStateVersion);
}

[Xunit.Theory]
[Xunit.InlineData(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeCompletionKind.Completed)]
[Xunit.InlineData(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeCompletionKind.Rejected)]
[Xunit.InlineData(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeCompletionKind.Failed)]
[Xunit.InlineData(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeCompletionKind.Cancelled)]
public void EveryTerminalOutcomeCanBeSummarized(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeCompletionKind completionKind)
{
    var cycle = RecoveryCycleCompletion(completionKind);
    var supersession = CheckpointSupersession(
        cycle,
        SuccessorCheckpoint(cycle));
    var summary = Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCompletedCycleSummary<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    Summarize(supersession).Summary);

    Xunit.Assert.Equal(completionKind, summary.OutcomeKind);
    Xunit.Assert.Same(cycle, summary.Supersession.CycleCompletion);
    Xunit.Assert.Same(
        cycle.Checkpoint,
        summary.Supersession.PriorCheckpoint);
}

private static void AssertSupersessionStatus(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointSupersessionStatus expected,
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointSupersessionResult<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload> result)
{
    Xunit.Assert.False(result.Succeeded);
    Xunit.Assert.Equal(expected, result.Status);
    Xunit.Assert.Null(result.Supersession);
}

private static void AssertSummaryStatus(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointSupersessionStatus expected,
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCompletedCycleSummaryResult<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload> result)
{
    Xunit.Assert.False(result.Succeeded);
    Xunit.Assert.Equal(expected, result.Status);
    Xunit.Assert.Null(result.Summary);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryCycleCompletion<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    RecoveryCycleCompletion(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompletionKind completionKind =
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeCompletionKind.Completed)
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

    return Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCycleCompletion<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    Complete(settlement).CycleCompletion);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryCheckpointSupersessionResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    Supersede(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCycleCompletion<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> cycle,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpoint<RequestPayload> successor,
        long? expectedCycleCompletionRevision = null,
        long? expectedSuccessorCheckpointRevision = null,
        long? supersededTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointSupersessionFlow.Supersede<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                CheckpointSupersessionId(),
                cycle,
                expectedCycleCompletionRevision ?? cycle.Revision,
                successor,
                expectedSuccessorCheckpointRevision ?? successor.Revision,
                supersededTick ?? checked(successor.CapturedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryCheckpointSupersession<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    CheckpointSupersession(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCycleCompletion<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>? cycle = null,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpoint<RequestPayload>? successor = null)
{
    var currentCycle = cycle ?? RecoveryCycleCompletion();
    var currentSuccessor =
        successor ?? SuccessorCheckpoint(currentCycle);

    return Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersession<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    Supersede(
                        currentCycle,
                        currentSuccessor).Supersession);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryCompletedCycleSummaryResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    Summarize(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersession<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> supersession,
        long? expectedSupersessionRevision = null,
        long? summarizedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointSupersessionFlow.Summarize<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                CompletedCycleSummaryId(),
                supersession,
                expectedSupersessionRevision ?? supersession.Revision,
                summarizedTick ?? checked(supersession.SupersededTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryCheckpoint<RequestPayload>
    SuccessorCheckpoint(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCycleCompletion<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> cycle,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointIdKind>? checkpointId = null,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInstanceIdKind>? runtimeId = null,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompositionIdKind>? compositionId = null,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueIdKind>? queueId = null,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>? clockId = null,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldIdKind>? worldId = null,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion?
            worldStateVersion = null,
        ulong? simulationTick = null,
        long capturedTick = 90,
        long? revision = null)
{
    var prior = cycle.Checkpoint;
    var currentRuntimeId = runtimeId ?? prior.RuntimeInstanceId;
    var currentCompositionId =
        compositionId ?? prior.Composition.CompositionId;
    var currentClockId = clockId ?? prior.ClockId;
    var composition =
        currentCompositionId == prior.Composition.CompositionId
            ? prior.Composition
            : AlternateComposition(currentCompositionId);
    var lifecycle =
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLifecycle
            .Create(currentRuntimeId, currentCompositionId);
    var queue =
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeQueueSnapshot.Create(
                queueId ?? prior.QueueSnapshot.QueueId,
                prior.QueueSnapshot.Capacity,
                queuedCount: 0,
                revision: checked(prior.QueueSnapshot.Revision + 1));
    var activeResult =
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkFlow.Capture<RequestPayload>(
                SuccessorActiveWorkSnapshotId(),
                currentRuntimeId,
                currentClockId,
                observedTick:
                    global::System.Math.Min(
                        capturedTick,
                        cycle.CompletedTick),
                revision:
                    checked(prior.ActiveWorkSnapshot.Revision + 1),
                items:
                    Array.Empty<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeActiveWorkItem<RequestPayload>>());
    var active = Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkSnapshot<RequestPayload>>(
                activeResult.Snapshot);
    var priorDocument = prior.WorldSnapshotDocument;
    var document =
        global::AI.Sandbox.Engine.Core.Persistence.WorldSnapshotDocument
            .Create(
                priorDocument.FormatVersion,
                priorDocument.SchemaId,
                priorDocument.SchemaVersion,
                worldId ?? priorDocument.WorldId,
                worldStateVersion ?? priorDocument.WorldStateVersion,
                simulationTick ?? priorDocument.SimulationTick,
                priorDocument.Payload,
                priorDocument.Checksum);
    var result =
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryFlow.CaptureCheckpoint<RequestPayload>(
                checkpointId ?? SuccessorCheckpointId(),
                lifecycle,
                composition,
                queue,
                active,
                document,
                capturedTick,
                revision ?? checked(prior.Revision + 1));

    return Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpoint<RequestPayload>>(
                result.Checkpoint);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeComposition AlternateComposition(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompositionIdKind> compositionId)
{
    var descriptor =
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCapabilityDescriptor.Create(
                OtherCapabilityId(),
                new Capability(),
                Array.Empty<
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeCapabilityIdKind>>());
    var result =
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompositionBuilder.Compose(
                compositionId,
                new[] { descriptor });

    return Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeComposition>(
            result.Composition);
}

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointSupersessionIdKind>
    CheckpointSupersessionId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionIdKind>(489001);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCompletedCycleSummaryIdKind>
    CompletedCycleSummaryId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCompletedCycleSummaryIdKind>(489002);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointIdKind>
    SuccessorCheckpointId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointIdKind>(489003);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeActiveWorkSnapshotIdKind>
    SuccessorActiveWorkSnapshotId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkSnapshotIdKind>(489004);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeInstanceIdKind>
    OtherRuntimeId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeInstanceIdKind>(489005);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeCompositionIdKind>
    OtherCompositionId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompositionIdKind>(489006);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeCapabilityIdKind>
    OtherCapabilityId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCapabilityIdKind>(489007);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>
    OtherWorldId() => Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>(489008);

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
