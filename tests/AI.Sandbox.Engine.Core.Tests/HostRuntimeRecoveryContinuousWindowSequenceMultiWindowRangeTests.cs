namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoveryContinuousWindowSequenceMultiWindowRangeTests
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
public void IdsRevisionsTicksAndArgumentsAreValidated()
{
    var pair = PairSummary(PreviousContinuity());

    Xunit.Assert.Throws<ArgumentException>(
        () => global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceFlow.ValidateSequence<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>(
                    validationId: default,
                    pairSummaries: new[] { pair },
                    expectedPairSummaryRevisions: new[] { pair.Revision },
                    validatedTick: pair.ProjectedTick));
    Xunit.Assert.Throws<ArgumentNullException>(
        () => global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceFlow.ValidateSequence<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>(
                    ContinuousWindowSequenceValidationId(),
                    null!,
                    new long[] { 0 },
                    validatedTick: 0));
    Xunit.Assert.Throws<ArgumentNullException>(
        () => global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceFlow.ValidateSequence<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>(
                    ContinuousWindowSequenceValidationId(),
                    new[] { pair },
                    null!,
                    validatedTick: 0));
    Xunit.Assert.Throws<ArgumentOutOfRangeException>(
        () => ValidateSequence(
            new[] { pair },
            expectedRevisions: new long[] { -1 }));
    Xunit.Assert.Throws<ArgumentOutOfRangeException>(
        () => ValidateSequence(new[] { pair }, validatedTick: -1));

    var sequence = Sequence(pair);
    Xunit.Assert.Throws<ArgumentException>(
        () => global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceFlow.QueryRange<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>(
                    queryId: default,
                    sequence: sequence,
                    startCheckpointId: sequence.StartCheckpointId,
                    endCheckpointId: sequence.EndCheckpointId,
                    expectedSequenceRevision: sequence.Revision,
                    queriedTick: sequence.ValidatedTick));
    Xunit.Assert.Throws<ArgumentNullException>(
        () => global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceFlow.QueryRange<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>(
                    MultiWindowCheckpointRangeQueryId(),
                    null!,
                    sequence.StartCheckpointId,
                    sequence.EndCheckpointId,
                    expectedSequenceRevision: 0,
                    queriedTick: 0));
    Xunit.Assert.Throws<ArgumentException>(
        () => QueryMultiWindow(
            sequence,
            startCheckpointId: default(
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryCheckpointIdKind>)));
    Xunit.Assert.Throws<ArgumentException>(
        () => QueryMultiWindow(
            sequence,
            endCheckpointId: default(
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryCheckpointIdKind>)));
    Xunit.Assert.Throws<ArgumentOutOfRangeException>(
        () => QueryMultiWindow(sequence, expectedSequenceRevision: -1));
    Xunit.Assert.Throws<ArgumentOutOfRangeException>(
        () => QueryMultiWindow(sequence, queriedTick: -1));
}

[Xunit.Fact]
public void EmptyOversizedAndRevisionCountSequencesAreRejected()
{
    var pair = PairSummary(PreviousContinuity());
    var emptyPairs = global::System.Array.Empty<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>();

    AssertSequenceStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceStatus
            .PairCollectionEmpty,
        ValidateSequence(emptyPairs, expectedRevisions: global::System.Array.Empty<long>(), validatedTick: 0));

    var oversized = global::System.Linq.Enumerable.ToArray(
        global::System.Linq.Enumerable.Repeat(
            pair,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousWindowSequenceFlow.MaximumPairCount + 1));
    AssertSequenceStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceStatus
            .PairCollectionTooLarge,
        ValidateSequence(
            oversized,
            expectedRevisions: global::System.Linq.Enumerable.ToArray(
                global::System.Linq.Enumerable.Repeat(pair.Revision, oversized.Length)),
            validatedTick: pair.ProjectedTick + 1));

    AssertSequenceStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceStatus
            .PairRevisionCountMismatch,
        ValidateSequence(
            new[] { pair },
            expectedRevisions: global::System.Array.Empty<long>()));
}

[Xunit.Fact]
public void SequenceValidationRejectsStaleRevisionAndRegressedTick()
{
    var pair = PairSummary(PreviousContinuity());

    AssertSequenceStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceStatus
            .StalePairSummaryRevision,
        ValidateSequence(
            new[] { pair },
            expectedRevisions: new[] { pair.Revision + 1 }));
    AssertSequenceStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceStatus
            .SequenceValidationTickRegressed,
        ValidateSequence(
            new[] { pair },
            validatedTick: pair.ProjectedTick - 1));
}

[Xunit.Fact]
public void SequenceValidationRejectsDuplicateSourceAndContinuityFailures()
{
    var first = PairSummaryWithId(PreviousContinuity(), 550101);

    AssertSequenceStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceStatus
            .DuplicatePairSummaryId,
        ValidateSequence(
            new[] { first, first },
            expectedRevisions: new[] { first.Revision, first.Revision }));

    var other = PairSummaryWithId(NextContinuity(), 550102);
    AssertSequenceStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceStatus
            .PairSourceMismatch,
        ValidateSequence(
            new[] { first, other },
            expectedRevisions: new[] { first.Revision, other.Revision },
            validatedTick: global::System.Math.Max(
                first.ProjectedTick,
                other.ProjectedTick) + 1));

    var sameSourceOverlap = PairSummaryWithId(first.Continuity, 550103);
    AssertSequenceStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceStatus
            .SequenceNotContinuous,
        ValidateSequence(
            new[] { first, sameSourceOverlap },
            expectedRevisions: new[] { first.Revision, sameSourceOverlap.Revision },
            validatedTick: global::System.Math.Max(
                first.ProjectedTick,
                sameSourceOverlap.ProjectedTick) + 1));
}

[Xunit.Fact]
public void SinglePairSequencePreservesExactBoundedAuthority()
{
    var pair = PairSummary(PreviousContinuity());
    var result = ValidateSequence(
        new[] { pair },
        validatedTick: pair.ProjectedTick + 2);
    var sequence = Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(result.Validation);

    Xunit.Assert.True(result.Succeeded);
    Xunit.Assert.Equal(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceStatus
            .ContinuousWindowSequenceValidated,
        result.Status);
    Xunit.Assert.Single(result.PairSummaries);
    Xunit.Assert.Same(pair, result.PairSummaries[0]);
    Xunit.Assert.Single(sequence.PairSummaries);
    Xunit.Assert.Same(pair, sequence.FirstPair);
    Xunit.Assert.Same(pair, sequence.LastPair);
    Xunit.Assert.Same(pair.SourceProjection, sequence.SourceProjection);
    Xunit.Assert.Same(pair.Chain, sequence.Chain);
    Xunit.Assert.Single(sequence.BoundarySupersessions);
    Xunit.Assert.Same(pair.ConnectingSupersession, sequence.BoundarySupersessions[0]);
    Xunit.Assert.Equal(1, sequence.PairCount);
    Xunit.Assert.Equal(2, sequence.WindowCount);
    Xunit.Assert.Equal(pair.CheckpointCount, sequence.CheckpointCount);
    Xunit.Assert.Equal(pair.SupersessionCount, sequence.SupersessionCount);
    Xunit.Assert.Equal(pair.StartCheckpointIndex, sequence.StartCheckpointIndex);
    Xunit.Assert.Equal(pair.EndCheckpointIndex, sequence.EndCheckpointIndex);
    Xunit.Assert.Same(pair.StartCheckpoint, sequence.StartCheckpoint);
    Xunit.Assert.Same(pair.EndCheckpoint, sequence.EndCheckpoint);
    Xunit.Assert.Equal(pair.Revision + 1, sequence.Revision);
    Xunit.Assert.Equal(pair.ProjectedTick + 2, sequence.ValidatedTick);
}

[Xunit.Fact]
public void MultiWindowQueryRejectsStaleRegressedMissingAndInvalidRanges()
{
    var sequence = Sequence(PairSummary(PreviousContinuity()));
    var missingId = Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointIdKind>(550099);

    AssertMultiWindowQueryStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceStatus
            .StaleSequenceRevision,
        QueryMultiWindow(
            sequence,
            expectedSequenceRevision: sequence.Revision + 1));
    AssertMultiWindowQueryStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceStatus
            .MultiWindowRangeQueryTickRegressed,
        QueryMultiWindow(sequence, queriedTick: sequence.ValidatedTick - 1));
    AssertMultiWindowQueryStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceStatus
            .RangeStartNotFound,
        QueryMultiWindow(sequence, startCheckpointId: missingId));
    AssertMultiWindowQueryStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceStatus
            .RangeEndNotFound,
        QueryMultiWindow(sequence, endCheckpointId: missingId));
    AssertMultiWindowQueryStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceStatus
            .RangeOrderInvalid,
        QueryMultiWindow(
            sequence,
            startCheckpointId: sequence.EndCheckpointId,
            endCheckpointId: sequence.StartCheckpointId));
    AssertMultiWindowQueryStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceStatus
            .RangeDoesNotCrossWindowBoundary,
        QueryMultiWindow(
            sequence,
            startCheckpointId: sequence.StartCheckpointId,
            endCheckpointId: sequence.StartCheckpointId));
}

[Xunit.Fact]
public void MultiWindowQueryPreservesExactCrossBoundaryEvidence()
{
    var pair = PairSummary(PreviousContinuity());
    var sequence = Sequence(pair);
    var result = QueryMultiWindow(
        sequence,
        queriedTick: sequence.ValidatedTick + 2);
    var query = Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeQuery<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(result.Query);

    Xunit.Assert.True(result.Succeeded);
    Xunit.Assert.Equal(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceStatus
            .MultiWindowCheckpointRangeQueried,
        result.Status);
    Xunit.Assert.Same(sequence, result.Sequence);
    Xunit.Assert.Same(sequence, query.Sequence);
    Xunit.Assert.Equal(sequence.StartCheckpointIndex, query.StartCheckpointIndex);
    Xunit.Assert.Equal(sequence.EndCheckpointIndex, query.EndCheckpointIndex);
    Xunit.Assert.Equal(sequence.CheckpointCount, query.CheckpointCount);
    Xunit.Assert.Equal(sequence.SupersessionCount, query.SupersessionCount);
    Xunit.Assert.Equal(0, query.StartPairIndex);
    Xunit.Assert.Equal(0, query.EndPairIndex);
    Xunit.Assert.Equal(1, query.PairCount);
    Xunit.Assert.Single(query.CrossedBoundarySupersessions);
    Xunit.Assert.Same(pair.ConnectingSupersession, query.CrossedBoundarySupersessions[0]);
    Xunit.Assert.Equal(1, query.CrossedBoundaryCount);
    Xunit.Assert.Equal(2, query.CrossedWindowCount);
    Xunit.Assert.Same(sequence.StartCheckpoint, query.StartCheckpoint);
    Xunit.Assert.Same(sequence.EndCheckpoint, query.EndCheckpoint);
    Xunit.Assert.Same(pair.ConnectingSupersession, query.Supersessions[0]);
    Xunit.Assert.Equal(sequence.Revision + 1, query.Revision);
    Xunit.Assert.Equal(sequence.ValidatedTick + 2, query.QueriedTick);
}

[Xunit.Fact]
public void SequenceAndQueryCollectionsRemainImmutable()
{
    var pair = PairSummary(PreviousContinuity());
    var sequence = Sequence(pair);
    var query = Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeQuery<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(QueryMultiWindow(sequence).Query);

    var pairList = Xunit.Assert.IsAssignableFrom<
        global::System.Collections.Generic.IList<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>>>(sequence.PairSummaries);
    Xunit.Assert.Throws<NotSupportedException>(() => pairList.Add(pair));

    var checkpointList = Xunit.Assert.IsAssignableFrom<
        global::System.Collections.Generic.IList<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpoint<RequestPayload>>>(query.Checkpoints);
    Xunit.Assert.Throws<NotSupportedException>(
        () => checkpointList.Add(query.StartCheckpoint));

    var boundaryList = Xunit.Assert.IsAssignableFrom<
        global::System.Collections.Generic.IList<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointSupersession<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>>>(query.CrossedBoundarySupersessions);
    Xunit.Assert.Throws<NotSupportedException>(
        () => boundaryList.Add(pair.ConnectingSupersession));
}

[Xunit.Fact]
public void ContractsRemainBoundedAndSideEffectFree()
{
    var sequence = Sequence(PairSummary(PreviousContinuity()));
    var query = Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeQuery<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(QueryMultiWindow(sequence).Query);

    Xunit.Assert.Equal(
        8,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceFlow.MaximumPairCount);
    Xunit.Assert.Equal(
        64,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceFlow.MaximumCheckpointCount);
    Xunit.Assert.Same(sequence, query.Sequence);
    Xunit.Assert.DoesNotContain(
        typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>).GetProperties(),
        property => property.SetMethod is not null);
    Xunit.Assert.DoesNotContain(
        typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeQuery<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>).GetProperties(),
        property => property.SetMethod is not null);
}

private static void AssertSequenceStatus(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousWindowSequenceStatus expected,
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousWindowSequenceValidationResult<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload> result)
{
    Xunit.Assert.False(result.Succeeded);
    Xunit.Assert.Equal(expected, result.Status);
    Xunit.Assert.Null(result.Validation);
}

private static void AssertMultiWindowQueryStatus(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousWindowSequenceStatus expected,
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryMultiWindowCheckpointRangeQueryResult<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload> result)
{
    Xunit.Assert.False(result.Succeeded);
    Xunit.Assert.Equal(expected, result.Status);
    Xunit.Assert.Null(result.Query);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryContinuousWindowSequenceValidationResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    ValidateSequence(
        global::System.Collections.Generic.IReadOnlyList<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>> pairSummaries,
        global::System.Collections.Generic.IReadOnlyList<long>?
            expectedRevisions = null,
        long? validatedTick = null)
{
    var revisions = expectedRevisions ??
        global::System.Linq.Enumerable.ToArray(
            global::System.Linq.Enumerable.Select(
                pairSummaries,
                pair => pair.Revision));
    var maximumTick = pairSummaries.Count == 0
        ? 0
        : global::System.Linq.Enumerable.Max(
            global::System.Linq.Enumerable.Select(
                pairSummaries,
                pair => pair.ProjectedTick));

    return global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousWindowSequenceFlow.ValidateSequence<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                ContinuousWindowSequenceValidationId(),
                pairSummaries,
                revisions,
                validatedTick ?? checked(maximumTick + 1));
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryContinuousWindowSequenceValidation<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    Sequence(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> pair) =>
    Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    ValidateSequence(new[] { pair }).Validation);

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryMultiWindowCheckpointRangeQueryResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    QueryMultiWindow(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> sequence,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointIdKind>?
                    startCheckpointId = null,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointIdKind>?
                    endCheckpointId = null,
        long? expectedSequenceRevision = null,
        long? queriedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousWindowSequenceFlow.QueryRange<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                MultiWindowCheckpointRangeQueryId(),
                sequence,
                startCheckpointId ?? sequence.StartCheckpointId,
                endCheckpointId ?? sequence.EndCheckpointId,
                expectedSequenceRevision ?? sequence.Revision,
                queriedTick ?? checked(sequence.ValidatedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    PairSummaryWithId(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeContinuityValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> continuity,
        int suffix) =>
    Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryContinuousWindowPairFlow.ProjectPair<
                            RequestPayload,
                            RecoveryWorldState,
                            CompletionPayload>(
                                Id<
                                    global::AI.Sandbox.Engine.Core.HostRuntime
                                        .HostRuntimeRecoveryContinuousWindowPairSummaryProjectionIdKind>(
                                            suffix),
                                continuity,
                                continuity.Revision,
                                checked(continuity.ValidatedTick + 1)).Summary);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousWindowSequenceValidationIdKind>
    ContinuousWindowSequenceValidationId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceValidationIdKind>(550001);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryMultiWindowCheckpointRangeQueryIdKind>
    MultiWindowCheckpointRangeQueryId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeQueryIdKind>(550002);

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryContinuousWindowPairSummaryProjectionResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    ProjectPair(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeContinuityValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> continuity,
        long? expectedContinuityRevision = null,
        long? projectedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousWindowPairFlow.ProjectPair<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                ContinuousWindowPairSummaryProjectionId(),
                continuity,
                expectedContinuityRevision ?? continuity.Revision,
                projectedTick ?? checked(continuity.ValidatedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    PairSummary(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeContinuityValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> continuity) =>
    Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(ProjectPair(continuity).Summary);

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryCrossWindowCheckpointRangeQueryResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    QueryCrossWindow(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> pair,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointIdKind>?
                    startCheckpointId = null,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointIdKind>?
                    endCheckpointId = null,
        long? expectedPairSummaryRevision = null,
        long? queriedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousWindowPairFlow.QueryRange<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                CrossWindowCheckpointRangeQueryId(),
                pair,
                startCheckpointId ?? pair.StartCheckpointId,
                endCheckpointId ?? pair.EndCheckpointId,
                expectedPairSummaryRevision ?? pair.Revision,
                queriedTick ?? checked(pair.ProjectedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryCheckpointRangeContinuityValidation<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    PreviousContinuity()
{
    var selection = PreviousSelection();
    var projection = AdjacentProjection(selection);
    return Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeContinuityValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    ValidateContinuity(selection.Summary, projection).Validation);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryCheckpointRangeContinuityValidation<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    NextContinuity()
{
    var selection = NextSelection();
    var projection = AdjacentProjection(selection);
    return Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeContinuityValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    ValidateContinuity(selection.Summary, projection).Validation);
}

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousWindowPairSummaryProjectionIdKind>
    ContinuousWindowPairSummaryProjectionId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowPairSummaryProjectionIdKind>(540001);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCrossWindowCheckpointRangeQueryIdKind>
    CrossWindowCheckpointRangeQueryId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCrossWindowCheckpointRangeQueryIdKind>(540002);

private static void AssertProjectionStatus(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryAdjacentWindowProjectionStatus expected,
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryAdjacentWindowProjectionResult<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload> result)
{
    Xunit.Assert.False(result.Succeeded);
    Xunit.Assert.Equal(expected, result.Status);
    Xunit.Assert.Null(result.Projection);
}

private static void AssertContinuityStatus(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryAdjacentWindowProjectionStatus expected,
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointRangeContinuityValidationResult<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload> result)
{
    Xunit.Assert.False(result.Succeeded);
    Xunit.Assert.Equal(expected, result.Status);
    Xunit.Assert.Null(result.Validation);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryAdjacentWindowProjectionResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    ProjectAdjacentWindow(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentWindowSelection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> selection,
        long? expectedSelectionRevision = null,
        long? projectedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryAdjacentWindowFlow.ProjectWindow<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                AdjacentWindowProjectionId(),
                selection,
                expectedSelectionRevision ?? selection.Revision,
                projectedTick ?? checked(selection.SelectedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryAdjacentWindowProjection<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    AdjacentProjection(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentWindowSelection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> selection) =>
    Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentWindowProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(ProjectAdjacentWindow(selection).Projection);

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryCheckpointRangeContinuityValidationResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    ValidateContinuity(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> summary,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentWindowProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> projection,
        long? expectedSummaryRevision = null,
        long? expectedAdjacentWindowRevision = null,
        long? validatedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryAdjacentWindowFlow.ValidateContinuity<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                CheckpointRangeContinuityValidationId(),
                summary,
                projection,
                expectedSummaryRevision ?? summary.Revision,
                expectedAdjacentWindowRevision ?? projection.Revision,
                validatedTick ?? checked(projection.ProjectedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryAdjacentWindowSelection<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    PreviousSelection()
{
    var summary = RangeSummary(LatestRange());
    return Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentWindowSelection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(SelectPrevious(summary).Selection);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryAdjacentWindowSelection<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    NextSelection()
{
    var summary = RangeSummary(RootRange());
    return Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentWindowSelection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(SelectNext(summary).Selection);
}

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryAdjacentWindowProjectionIdKind>
    AdjacentWindowProjectionId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentWindowProjectionIdKind>(530001);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointRangeContinuityValidationIdKind>
    CheckpointRangeContinuityValidationId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeContinuityValidationIdKind>(530002);

private static void AssertSummaryStatus(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointRangeSummaryStatus expected,
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointRangeSummaryProjectionResult<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload> result)
{
    Xunit.Assert.False(result.Succeeded);
    Xunit.Assert.Equal(expected, result.Status);
    Xunit.Assert.Null(result.Summary);
}

private static void AssertSelectionStatus(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointRangeSummaryStatus expected,
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryAdjacentWindowSelectionResult<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload> result)
{
    Xunit.Assert.False(result.Succeeded);
    Xunit.Assert.Equal(expected, result.Status);
    Xunit.Assert.Null(result.Selection);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryCheckpointRangeSummaryProjectionResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    ProjectRangeSummary(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeQuery<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> range,
        long? expectedRangeRevision = null,
        long? projectedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointRangeSummaryFlow.ProjectSummary<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                CheckpointRangeSummaryProjectionId(),
                range,
                expectedRangeRevision ?? range.Revision,
                projectedTick ?? checked(range.QueriedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryCheckpointRangeSummaryProjection<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    RangeSummary(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeQuery<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> range) =>
    Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(ProjectRangeSummary(range).Summary);

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryAdjacentWindowSelectionResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    SelectPrevious(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> summary,
        int checkpointCount = 1,
        long? expectedSummaryRevision = null,
        long? selectedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointRangeSummaryFlow
        .SelectPreviousWindow<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                AdjacentWindowSelectionId(),
                summary,
                checkpointCount,
                expectedSummaryRevision ?? summary.Revision,
                selectedTick ?? checked(summary.ProjectedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryAdjacentWindowSelectionResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    SelectNext(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> summary,
        int checkpointCount = 1,
        long? expectedSummaryRevision = null,
        long? selectedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointRangeSummaryFlow
        .SelectNextWindow<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                AdjacentWindowSelectionId(),
                summary,
                checkpointCount,
                expectedSummaryRevision ?? summary.Revision,
                selectedTick ?? checked(summary.ProjectedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryCheckpointRangeQuery<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    RootRange()
{
    var window = LineageWindow();
    return Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeQuery<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    QueryRange(
                        window,
                        startCheckpointId: window.StartCheckpointId,
                        endCheckpointId: window.StartCheckpointId).Query);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryCheckpointRangeQuery<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    LatestRange()
{
    var window = LineageWindow();
    return Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeQuery<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    QueryRange(
                        window,
                        startCheckpointId: window.EndCheckpointId,
                        endCheckpointId: window.EndCheckpointId).Query);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryCheckpointRangeQuery<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    FullRange()
{
    var window = LineageWindow();
    return Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeQuery<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(QueryRange(window).Query);
}

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointRangeSummaryProjectionIdKind>
    CheckpointRangeSummaryProjectionId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeSummaryProjectionIdKind>(520001);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryAdjacentWindowSelectionIdKind>
    AdjacentWindowSelectionId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentWindowSelectionIdKind>(520002);

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryLineageWindowProjectionResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    ProjectWindow(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryChainSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> projection,
        int startCheckpointIndex = 0,
        int? checkpointCount = null,
        long? expectedProjectionRevision = null,
        long? projectedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryLineageWindowFlow.ProjectWindow<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                LineageWindowProjectionId(),
                projection,
                startCheckpointIndex,
                checkpointCount ?? projection.CheckpointCount,
                expectedProjectionRevision ?? projection.Revision,
                projectedTick ?? checked(projection.ProjectedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryLineageWindowProjection<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    LineageWindow(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryChainSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>? projection = null)
{
    var currentProjection = projection ?? ChainSummaryProjection();
    return Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryLineageWindowProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    ProjectWindow(currentProjection).Window);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryCheckpointRangeQueryResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    QueryRange(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryLineageWindowProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> window,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointIdKind>?
                    startCheckpointId = null,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointIdKind>?
                    endCheckpointId = null,
        long? expectedWindowRevision = null,
        long? queriedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryLineageWindowFlow.QueryRange<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                CheckpointRangeQueryId(),
                window,
                startCheckpointId ?? window.StartCheckpointId,
                endCheckpointId ?? window.EndCheckpointId,
                expectedWindowRevision ?? window.Revision,
                queriedTick ?? checked(window.ProjectedTick + 1));

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryLineageWindowProjectionIdKind>
    LineageWindowProjectionId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryLineageWindowProjectionIdKind>(510001);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointRangeQueryIdKind>
    CheckpointRangeQueryId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeQueryIdKind>(510002);

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryChainSummaryProjectionResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    ProjectSummary(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoverySupersessionChain<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> chain,
        long? expectedChainRevision = null,
        long? projectedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryChainQueryFlow.ProjectSummary<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                ChainSummaryProjectionId(),
                chain,
                expectedChainRevision ?? chain.Revision,
                projectedTick ?? checked(chain.ValidatedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryChainSummaryProjection<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    ChainSummaryProjection(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoverySupersessionChain<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>? chain = null)
{
    var currentChain = chain ?? SupersessionChain();
    return Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryChainSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    ProjectSummary(currentChain).Projection);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryCheckpointLineageQueryResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    QueryLineage(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryChainSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> projection,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointIdKind> checkpointId,
        long? expectedProjectionRevision = null,
        long? queriedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryChainQueryFlow.QueryLineage<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                CheckpointLineageQueryId(),
                projection,
                checkpointId,
                expectedProjectionRevision ?? projection.Revision,
                queriedTick ?? checked(projection.ProjectedTick + 1));

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryChainSummaryProjectionIdKind>
    ChainSummaryProjectionId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryChainSummaryProjectionIdKind>(500001);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointLineageQueryIdKind>
    CheckpointLineageQueryId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointLineageQueryIdKind>(500002);

private static void AssertChainStatus(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoverySupersessionChainStatus expected,
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoverySupersessionChainResult<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload> result)
{
    Xunit.Assert.False(result.Succeeded);
    Xunit.Assert.Equal(expected, result.Status);
    Xunit.Assert.Null(result.Chain);
}

private static void AssertSelectionStatus(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoverySupersessionChainStatus expected,
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryLatestCheckpointSelectionResult<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload> result)
{
    Xunit.Assert.False(result.Succeeded);
    Xunit.Assert.Equal(expected, result.Status);
    Xunit.Assert.Null(result.Selection);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoverySupersessionChainResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    Validate(
        global::System.Collections.Generic.IReadOnlyList<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointSupersession<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>> supersessions,
        global::System.Collections.Generic.IReadOnlyList<long>
            expectedRevisions,
        long validatedTick) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoverySupersessionChainFlow.Validate<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                SupersessionChainId(),
                supersessions,
                expectedRevisions,
                validatedTick);

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoverySupersessionChain<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    SupersessionChain()
{
    var supersession = CreateSupersession();
    return Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoverySupersessionChain<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    Validate(
                        new[] { supersession },
                        new[] { supersession.Revision },
                        supersession.SupersededTick + 1).Chain);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryLatestCheckpointSelectionResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    SelectLatest(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoverySupersessionChain<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> chain,
        long? expectedChainRevision = null,
        long? selectedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoverySupersessionChainFlow.SelectLatest<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                LatestCheckpointSelectionId(),
                chain,
                expectedChainRevision ?? chain.Revision,
                selectedTick ?? checked(chain.ValidatedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryCheckpointSupersession<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    CreateSupersession(
        int supersessionSuffix = 499001,
        int checkpointSuffix = 499002,
        long capturedTick = 90)
{
    var cycle = RecoveryCycleCompletion();
    var successor = SuccessorCheckpoint(
        cycle,
        checkpointId: Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointIdKind>(checkpointSuffix),
        capturedTick: capturedTick,
        revision: checked(cycle.Checkpoint.Revision + 1));
    var result =
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersessionFlow.Supersede<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>(
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeRecoveryCheckpointSupersessionIdKind>(
                                supersessionSuffix),
                    cycle,
                    cycle.Revision,
                    successor,
                    successor.Revision,
                    checked(successor.CapturedTick + 1));

    return Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersession<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(result.Supersession);
}

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoverySupersessionChainIdKind>
    SupersessionChainId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoverySupersessionChainIdKind>(499003);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryLatestCheckpointSelectionIdKind>
    LatestCheckpointSelectionId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryLatestCheckpointSelectionIdKind>(499004);

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
