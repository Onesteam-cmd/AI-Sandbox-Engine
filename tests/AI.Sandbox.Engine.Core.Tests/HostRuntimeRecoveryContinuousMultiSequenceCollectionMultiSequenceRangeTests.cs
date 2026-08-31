namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoveryContinuousMultiSequenceCollectionMultiSequenceRangeTests
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

    private sealed record SyntheticAdjacentSequenceContext(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryChainSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> SourceProjection,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> SourceSequence,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> Summary,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentSequenceSelection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> Selection,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> PreviousPair,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> RangePair,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersession<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> Boundary,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpoint<RequestPayload> PriorCheckpoint,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpoint<RequestPayload> SuccessorCheckpoint);

[Xunit.Fact]
public void IdsRevisionsTicksAndArgumentsAreValidated()
{
    var summary = MultiSequenceSummary(MultiWindowContinuity(SyntheticContext()));

    Xunit.Assert.Throws<ArgumentException>(
        () => global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionFlow
            .ValidateCollection<RequestPayload, RecoveryWorldState, CompletionPayload>(
                validationId: default,
                multiSequenceSummaries: new[] { summary },
                expectedSummaryRevisions: new[] { summary.Revision },
                validatedTick: summary.ProjectedTick));
    Xunit.Assert.Throws<ArgumentNullException>(
        () => global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionFlow
            .ValidateCollection<RequestPayload, RecoveryWorldState, CompletionPayload>(
                ContinuousMultiSequenceCollectionValidationId(),
                null!,
                new long[] { 0 },
                validatedTick: 0));
    Xunit.Assert.Throws<ArgumentNullException>(
        () => global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionFlow
            .ValidateCollection<RequestPayload, RecoveryWorldState, CompletionPayload>(
                ContinuousMultiSequenceCollectionValidationId(),
                new[] { summary },
                null!,
                validatedTick: 0));
    Xunit.Assert.Throws<ArgumentOutOfRangeException>(
        () => ValidateCollection(
            new[] { summary },
            expectedRevisions: new long[] { -1 }));
    Xunit.Assert.Throws<ArgumentOutOfRangeException>(
        () => ValidateCollection(new[] { summary }, validatedTick: -1));

    var collection = Collection(summary);
    Xunit.Assert.Throws<ArgumentException>(
        () => global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionFlow
            .QueryRange<RequestPayload, RecoveryWorldState, CompletionPayload>(
                queryId: default,
                collection: collection,
                startCheckpointId: collection.StartCheckpointId,
                endCheckpointId: collection.EndCheckpointId,
                expectedCollectionRevision: collection.Revision,
                queriedTick: collection.ValidatedTick));
    Xunit.Assert.Throws<ArgumentNullException>(
        () => global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionFlow
            .QueryRange<RequestPayload, RecoveryWorldState, CompletionPayload>(
                MultiSequenceCheckpointRangeQueryId(),
                null!,
                collection.StartCheckpointId,
                collection.EndCheckpointId,
                expectedCollectionRevision: 0,
                queriedTick: 0));
    Xunit.Assert.Throws<ArgumentException>(
        () => QueryMultiSequence(
            collection,
            startCheckpointId: default(
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryCheckpointIdKind>)));
    Xunit.Assert.Throws<ArgumentException>(
        () => QueryMultiSequence(
            collection,
            endCheckpointId: default(
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryCheckpointIdKind>)));
    Xunit.Assert.Throws<ArgumentOutOfRangeException>(
        () => QueryMultiSequence(collection, expectedCollectionRevision: -1));
    Xunit.Assert.Throws<ArgumentOutOfRangeException>(
        () => QueryMultiSequence(collection, queriedTick: -1));
}

[Xunit.Fact]
public void EmptyOversizedAndRevisionCountCollectionsAreRejected()
{
    var summary = MultiSequenceSummary(MultiWindowContinuity(SyntheticContext()));
    var empty = global::System.Array.Empty<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>();

    AssertCollectionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .SummaryCollectionEmpty,
        ValidateCollection(
            empty,
            expectedRevisions: global::System.Array.Empty<long>(),
            validatedTick: 0));

    var oversized = global::System.Linq.Enumerable.ToArray(
        global::System.Linq.Enumerable.Repeat(
            summary,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiSequenceCollectionFlow
                .MaximumSummaryCount + 1));
    AssertCollectionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .SummaryCollectionTooLarge,
        ValidateCollection(
            oversized,
            expectedRevisions: global::System.Linq.Enumerable.ToArray(
                global::System.Linq.Enumerable.Repeat(
                    summary.Revision,
                    oversized.Length)),
            validatedTick: summary.ProjectedTick + 1));

    AssertCollectionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .SummaryRevisionCountMismatch,
        ValidateCollection(
            new[] { summary },
            expectedRevisions: global::System.Array.Empty<long>()));
}

[Xunit.Fact]
public void CollectionValidationRejectsStaleRevisionAndRegressedTick()
{
    var summary = MultiSequenceSummary(MultiWindowContinuity(SyntheticContext()));

    AssertCollectionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .StaleMultiSequenceSummaryRevision,
        ValidateCollection(
            new[] { summary },
            expectedRevisions: new[] { summary.Revision + 1 }));
    AssertCollectionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .CollectionValidationTickRegressed,
        ValidateCollection(
            new[] { summary },
            validatedTick: summary.ProjectedTick - 1));
}

[Xunit.Fact]
public void CollectionValidationRejectsDuplicateSourceAndContinuityFailures()
{
    var context = SyntheticContext();
    var first = MultiSequenceSummaryWithId(
        MultiWindowContinuity(context),
        590101);

    AssertCollectionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .DuplicateMultiSequenceSummaryId,
        ValidateCollection(
            new[] { first, first },
            expectedRevisions: new[] { first.Revision, first.Revision }));

    var other = MultiSequenceSummaryWithId(
        MultiWindowContinuity(SyntheticContext(seed: 590500)),
        590102);
    AssertCollectionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .SummarySourceMismatch,
        ValidateCollection(
            new[] { first, other },
            expectedRevisions: new[] { first.Revision, other.Revision },
            validatedTick: global::System.Math.Max(
                first.ProjectedTick,
                other.ProjectedTick) + 1));

    var overlapping = MultiSequenceSummaryWithId(first.Continuity, 590103);
    AssertCollectionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .SummaryPairRangeNotContinuous,
        ValidateCollection(
            new[] { first, overlapping },
            expectedRevisions: new[] { first.Revision, overlapping.Revision },
            validatedTick: global::System.Math.Max(
                first.ProjectedTick,
                overlapping.ProjectedTick) + 1));

    var otherContext = SyntheticContext(seed: 590800);
    var wrongContinuity = ConstructNonPublic<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    MultiWindowCheckpointRangeContinuityValidationId(),
                    context.Summary,
                    AdjacentSequenceProjection(context.Selection),
                    otherContext.Boundary,
                    230,
                    5);
    var wrongSummary = ConstructNonPublic<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionIdKind>(
                            590104),
                    wrongContinuity,
                    231,
                    6);
    AssertCollectionStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .SummarySupersessionMismatch,
        ValidateCollection(
            new[] { wrongSummary },
            expectedRevisions: new[] { wrongSummary.Revision },
            validatedTick: wrongSummary.ProjectedTick + 1));
}

[Xunit.Fact]
public void SingleSummaryCollectionPreservesExactBoundedAuthority()
{
    var summary = MultiSequenceSummary(MultiWindowContinuity(SyntheticContext()));
    var result = ValidateCollection(
        new[] { summary },
        validatedTick: summary.ProjectedTick + 2);
    var collection = Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(result.Validation);

    Xunit.Assert.True(result.Succeeded);
    Xunit.Assert.Equal(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .ContinuousMultiSequenceCollectionValidated,
        result.Status);
    Xunit.Assert.Single(result.MultiSequenceSummaries);
    Xunit.Assert.Same(summary, result.MultiSequenceSummaries[0]);
    Xunit.Assert.Single(collection.MultiSequenceSummaries);
    Xunit.Assert.Same(summary, collection.FirstSummary);
    Xunit.Assert.Same(summary, collection.LastSummary);
    Xunit.Assert.Same(summary.SourceProjection, collection.SourceProjection);
    Xunit.Assert.Same(summary.Chain, collection.Chain);
    Xunit.Assert.Single(collection.BoundarySupersessions);
    Xunit.Assert.Same(
        summary.ConnectingSupersession,
        collection.BoundarySupersessions[0]);
    Xunit.Assert.Equal(1, collection.SummaryCount);
    Xunit.Assert.Equal(2, collection.SequenceCount);
    Xunit.Assert.Equal(summary.PairCount, collection.PairCount);
    Xunit.Assert.Equal(summary.WindowCount, collection.WindowCount);
    Xunit.Assert.Equal(summary.CheckpointCount, collection.CheckpointCount);
    Xunit.Assert.Equal(summary.SupersessionCount, collection.SupersessionCount);
    Xunit.Assert.Equal(summary.StartPairIndex, collection.StartPairIndex);
    Xunit.Assert.Equal(summary.EndPairIndex, collection.EndPairIndex);
    Xunit.Assert.Equal(
        summary.StartCheckpointIndex,
        collection.StartCheckpointIndex);
    Xunit.Assert.Equal(summary.EndCheckpointIndex, collection.EndCheckpointIndex);
    Xunit.Assert.Same(summary.StartCheckpoint, collection.StartCheckpoint);
    Xunit.Assert.Same(summary.EndCheckpoint, collection.EndCheckpoint);
    Xunit.Assert.Equal(summary.Revision + 1, collection.Revision);
    Xunit.Assert.Equal(summary.ProjectedTick + 2, collection.ValidatedTick);
}

[Xunit.Fact]
public void MultiSequenceQueryRejectsStaleRevisionAndRegressedTick()
{
    var collection = Collection(
        MultiSequenceSummary(MultiWindowContinuity(SyntheticContext())));

    AssertMultiSequenceQueryStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .StaleCollectionRevision,
        QueryMultiSequence(
            collection,
            expectedCollectionRevision: collection.Revision + 1));
    AssertMultiSequenceQueryStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .MultiSequenceRangeQueryTickRegressed,
        QueryMultiSequence(
            collection,
            queriedTick: collection.ValidatedTick - 1));
}

[Xunit.Fact]
public void QueryRejectsMissingReversedAndSingleCheckpointRanges()
{
    var collection = Collection(
        MultiSequenceSummary(MultiWindowContinuity(SyntheticContext())));
    var missingId = Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointIdKind>(590099);

    AssertMultiSequenceQueryStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .RangeStartNotFound,
        QueryMultiSequence(collection, startCheckpointId: missingId));
    AssertMultiSequenceQueryStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .RangeEndNotFound,
        QueryMultiSequence(collection, endCheckpointId: missingId));
    AssertMultiSequenceQueryStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .RangeOrderInvalid,
        QueryMultiSequence(
            collection,
            startCheckpointId: collection.EndCheckpointId,
            endCheckpointId: collection.StartCheckpointId));
    AssertMultiSequenceQueryStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .RangeDoesNotCrossSequenceBoundary,
        QueryMultiSequence(
            collection,
            startCheckpointId: collection.StartCheckpointId,
            endCheckpointId: collection.StartCheckpointId));
}

[Xunit.Fact]
public void QueryPreservesExactMultiSequenceEvidence()
{
    var context = SyntheticContext();
    var summary = MultiSequenceSummary(MultiWindowContinuity(context));
    var collection = Collection(summary);
    var result = QueryMultiSequence(
        collection,
        queriedTick: collection.ValidatedTick + 2);
    var query = Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiSequenceCheckpointRangeQuery<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(result.Query);

    Xunit.Assert.True(result.Succeeded);
    Xunit.Assert.Equal(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .MultiSequenceCheckpointRangeQueried,
        result.Status);
    Xunit.Assert.Same(collection, result.Collection);
    Xunit.Assert.Same(collection, query.Collection);
    Xunit.Assert.Same(summary.SourceProjection, query.SourceProjection);
    Xunit.Assert.Same(summary.Chain, query.Chain);
    Xunit.Assert.Equal(2, query.CheckpointCount);
    Xunit.Assert.Equal(1, query.SupersessionCount);
    Xunit.Assert.Equal(1, query.SummaryCount);
    Xunit.Assert.Equal(2, query.SequenceCount);
    Xunit.Assert.Equal(1, query.CrossedBoundaryCount);
    Xunit.Assert.Same(context.PriorCheckpoint, query.StartCheckpoint);
    Xunit.Assert.Same(context.SuccessorCheckpoint, query.EndCheckpoint);
    Xunit.Assert.Same(context.Boundary, query.Supersessions[0]);
    Xunit.Assert.Same(
        context.Boundary,
        query.CrossedBoundarySupersessions[0]);
    Xunit.Assert.Null(query.IncomingSupersession);
    Xunit.Assert.Null(query.OutgoingSupersession);
    Xunit.Assert.True(query.StartsAtCollectionStart);
    Xunit.Assert.True(query.EndsAtCollectionEnd);
    Xunit.Assert.Equal(collection.Revision + 1, query.Revision);
    Xunit.Assert.Equal(collection.ValidatedTick + 2, query.QueriedTick);
}

[Xunit.Fact]
public void CollectionsRemainImmutableAndContractsStayBounded()
{
    var summary = MultiSequenceSummary(MultiWindowContinuity(SyntheticContext()));
    var collection = Collection(summary);
    var query = Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiSequenceCheckpointRangeQuery<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(QueryMultiSequence(collection).Query);

    Xunit.Assert.IsAssignableFrom<
        global::System.Collections.ObjectModel.ReadOnlyCollection<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>>>(collection.MultiSequenceSummaries);
    Xunit.Assert.IsAssignableFrom<
        global::System.Collections.ObjectModel.ReadOnlyCollection<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointSupersession<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>>>(collection.BoundarySupersessions);
    Xunit.Assert.IsAssignableFrom<
        global::System.Collections.ObjectModel.ReadOnlyCollection<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpoint<RequestPayload>>>(query.Checkpoints);
    Xunit.Assert.IsAssignableFrom<
        global::System.Collections.ObjectModel.ReadOnlyCollection<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointSupersession<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>>>(query.CrossedBoundarySupersessions);
    Xunit.Assert.DoesNotContain(
        typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>).GetProperties(),
        property => property.SetMethod is not null);
    Xunit.Assert.DoesNotContain(
        typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiSequenceCheckpointRangeQuery<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>).GetProperties(),
        property => property.SetMethod is not null);
    Xunit.Assert.Equal(
        8,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionFlow
            .MaximumSummaryCount);
    Xunit.Assert.Equal(
        64,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionFlow
            .MaximumCheckpointCount);
    Xunit.Assert.Same(summary.Chain, collection.Chain);
    Xunit.Assert.Same(collection, query.Collection);
}

private static void AssertCollectionStatus(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus expected,
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousMultiSequenceCollectionValidationResult<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload> result)
{
    Xunit.Assert.False(result.Succeeded);
    Xunit.Assert.Equal(expected, result.Status);
    Xunit.Assert.Null(result.Validation);
}

private static void AssertMultiSequenceQueryStatus(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus expected,
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryMultiSequenceCheckpointRangeQueryResult<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload> result)
{
    Xunit.Assert.False(result.Succeeded);
    Xunit.Assert.Equal(expected, result.Status);
    Xunit.Assert.Null(result.Query);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryContinuousMultiSequenceCollectionValidationResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    ValidateCollection(
        global::System.Collections.Generic.IReadOnlyList<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>> summaries,
        global::System.Collections.Generic.IReadOnlyList<long>?
            expectedRevisions = null,
        long? validatedTick = null)
{
    var revisions = expectedRevisions ??
        global::System.Linq.Enumerable.ToArray(
            global::System.Linq.Enumerable.Select(
                summaries,
                summary => summary.Revision));
    var maximumTick = summaries.Count == 0
        ? 0
        : global::System.Linq.Enumerable.Max(
            global::System.Linq.Enumerable.Select(
                summaries,
                summary => summary.ProjectedTick));

    return global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousMultiSequenceCollectionFlow
        .ValidateCollection<RequestPayload, RecoveryWorldState, CompletionPayload>(
            ContinuousMultiSequenceCollectionValidationId(),
            summaries,
            revisions,
            validatedTick ?? checked(maximumTick + 1));
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    Collection(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> summary) =>
    Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    ValidateCollection(new[] { summary }).Validation);

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryMultiSequenceCheckpointRangeQueryResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    QueryMultiSequence(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> collection,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointIdKind>? startCheckpointId = null,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointIdKind>? endCheckpointId = null,
        long? expectedCollectionRevision = null,
        long? queriedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousMultiSequenceCollectionFlow
        .QueryRange<RequestPayload, RecoveryWorldState, CompletionPayload>(
            MultiSequenceCheckpointRangeQueryId(),
            collection,
            startCheckpointId ?? collection.StartCheckpointId,
            endCheckpointId ?? collection.EndCheckpointId,
            expectedCollectionRevision ?? collection.Revision,
            queriedTick ?? checked(collection.ValidatedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    MultiSequenceSummaryWithId(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> continuity,
        int suffix) =>
    Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryContinuousMultiSequenceFlow
                        .ProjectSummary<
                            RequestPayload,
                            RecoveryWorldState,
                            CompletionPayload>(
                                Id<
                                    global::AI.Sandbox.Engine.Core.HostRuntime
                                        .HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionIdKind>(
                                            suffix),
                                continuity,
                                continuity.Revision,
                                checked(continuity.ValidatedTick + 1)).Summary);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousMultiSequenceCollectionValidationIdKind>
    ContinuousMultiSequenceCollectionValidationId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceCollectionValidationIdKind>(
                590001);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryMultiSequenceCheckpointRangeQueryIdKind>
    MultiSequenceCheckpointRangeQueryId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiSequenceCheckpointRangeQueryIdKind>(
                590002);

private static void AssertMultiSequenceSummaryStatus(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousMultiSequenceStatus expected,
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionResult<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload> result)
{
    Xunit.Assert.False(result.Succeeded);
    Xunit.Assert.Equal(expected, result.Status);
    Xunit.Assert.Null(result.Summary);
}

private static void AssertCrossSequenceQueryStatus(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousMultiSequenceStatus expected,
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCrossSequenceCheckpointRangeQueryResult<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload> result)
{
    Xunit.Assert.False(result.Succeeded);
    Xunit.Assert.Equal(expected, result.Status);
    Xunit.Assert.Null(result.Query);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    ProjectMultiSequence(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> continuity,
        long? expectedContinuityRevision = null,
        long? projectedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousMultiSequenceFlow.ProjectSummary<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                ContinuousMultiSequenceSummaryProjectionId(),
                continuity,
                expectedContinuityRevision ?? continuity.Revision,
                projectedTick ?? checked(continuity.ValidatedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    MultiSequenceSummary(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> continuity) =>
    Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(ProjectMultiSequence(continuity).Summary);

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryCrossSequenceCheckpointRangeQueryResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    QueryCrossSequence(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> summary,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointIdKind>? startCheckpointId = null,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointIdKind>? endCheckpointId = null,
        long? expectedSummaryRevision = null,
        long? queriedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousMultiSequenceFlow.QueryRange<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                CrossSequenceCheckpointRangeQueryId(),
                summary,
                startCheckpointId ?? summary.StartCheckpointId,
                endCheckpointId ?? summary.EndCheckpointId,
                expectedSummaryRevision ?? summary.Revision,
                queriedTick ?? checked(summary.ProjectedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    MultiWindowContinuity(SyntheticAdjacentSequenceContext context)
{
    var projection = AdjacentSequenceProjection(context.Selection);
    return Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    ValidateSequenceContinuity(
                        context.Summary,
                        projection).Validation);
}

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionIdKind>
    ContinuousMultiSequenceSummaryProjectionId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionIdKind>(580001);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCrossSequenceCheckpointRangeQueryIdKind>
    CrossSequenceCheckpointRangeQueryId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCrossSequenceCheckpointRangeQueryIdKind>(580002);

private static void AssertAdjacentSequenceProjectionStatus(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryAdjacentSequenceProjectionStatus expected,
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryAdjacentSequenceProjectionResult<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload> result)
{
    Xunit.Assert.False(result.Succeeded);
    Xunit.Assert.Equal(expected, result.Status);
    Xunit.Assert.Null(result.Projection);
}

private static void AssertMultiWindowContinuityStatus(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryAdjacentSequenceProjectionStatus expected,
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidationResult<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload> result)
{
    Xunit.Assert.False(result.Succeeded);
    Xunit.Assert.Equal(expected, result.Status);
    Xunit.Assert.Null(result.Validation);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryAdjacentSequenceProjectionResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    ProjectAdjacentSequence(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentSequenceSelection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> selection,
        long? expectedSelectionRevision = null,
        long? projectedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryAdjacentSequenceFlow.ProjectSequence<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                AdjacentSequenceProjectionId(),
                selection,
                expectedSelectionRevision ?? selection.Revision,
                projectedTick ?? checked(selection.SelectedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryAdjacentSequenceProjection<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    AdjacentSequenceProjection(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentSequenceSelection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> selection) =>
    Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentSequenceProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(ProjectAdjacentSequence(selection).Projection);

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidationResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    ValidateSequenceContinuity(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> summary,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentSequenceProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> projection,
        long? expectedSummaryRevision = null,
        long? expectedAdjacentSequenceRevision = null,
        long? validatedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryAdjacentSequenceFlow.ValidateContinuity<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                MultiWindowCheckpointRangeContinuityValidationId(),
                summary,
                projection,
                expectedSummaryRevision ?? summary.Revision,
                expectedAdjacentSequenceRevision ?? projection.Revision,
                validatedTick ?? checked(global::System.Math.Max(
                    summary.ProjectedTick,
                    projection.ProjectedTick) + 1));

private static SyntheticAdjacentSequenceContext SyntheticContext(int seed = 570000)
{
    var sourceProjection = ChainSummaryProjection();
    var chain = sourceProjection.Chain;
    var boundary = chain.Supersessions[0];
    var priorCheckpoint = chain.RootCheckpoint;
    var successorCheckpoint = boundary.SuccessorCheckpoint;
    var previousPair = SyntheticPair(
        sourceProjection,
        priorCheckpoint,
        checkpointIndex: 0,
        boundary,
        seed + 100);
    var rangePair = SyntheticPair(
        sourceProjection,
        successorCheckpoint,
        checkpointIndex: 1,
        boundary,
        seed + 200);
    var sourceSequence = ConstructNonPublic<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryContinuousWindowSequenceValidationIdKind>(
                            seed + 1),
                    new[] { previousPair, rangePair },
                    new[] { boundary, boundary, boundary },
                    210,
                    2);
    var summary = SyntheticSummary(
        sourceProjection,
        sourceSequence,
        startPairIndex: 1,
        startCheckpointIndex: 1,
        checkpoint: successorCheckpoint,
        incomingSupersession: boundary,
        seed + 300);
    var selection = ConstructNonPublic<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentSequenceSelection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryAdjacentSequenceSelectionIdKind>(seed + 2),
                    summary,
                    true,
                    0,
                    new[] { previousPair },
                    new[] { boundary },
                    boundary,
                    null,
                    boundary,
                    213,
                    4);

    return new(
        sourceProjection,
        sourceSequence,
        summary,
        selection,
        previousPair,
        rangePair,
        boundary,
        priorCheckpoint,
        successorCheckpoint);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    SyntheticPair(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryChainSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> sourceProjection,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpoint<RequestPayload> checkpoint,
        int checkpointIndex,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersession<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> boundary,
        int suffix)
{
    var incoming = checkpointIndex == 0 ? null : boundary;
    var outgoing = checkpointIndex == 0 ? boundary : null;
    var window = ConstructNonPublic<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryLineageWindowProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryLineageWindowProjectionIdKind>(suffix + 1),
                    sourceProjection,
                    new[] { checkpoint },
                    global::System.Array.Empty<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeRecoveryCheckpointSupersession<
                                RequestPayload,
                                RecoveryWorldState,
                                CompletionPayload>>(),
                    incoming,
                    outgoing,
                    checkpointIndex,
                    100 + checkpointIndex,
                    1);
    var range = ConstructNonPublic<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeQuery<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryCheckpointRangeQueryIdKind>(suffix + 2),
                    window,
                    new[] { checkpoint },
                    global::System.Array.Empty<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeRecoveryCheckpointSupersession<
                                RequestPayload,
                                RecoveryWorldState,
                                CompletionPayload>>(),
                    incoming,
                    outgoing,
                    0,
                    0,
                    102 + checkpointIndex,
                    2);
    var summary = ConstructNonPublic<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryCheckpointRangeSummaryProjectionIdKind>(
                            suffix + 3),
                    range,
                    103 + checkpointIndex,
                    3);
    var windowSelection = ConstructNonPublic<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentWindowSelection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryAdjacentWindowSelectionIdKind>(suffix + 4),
                    summary,
                    false,
                    checkpointIndex,
                    1,
                    checkpoint,
                    checkpoint,
                    incoming,
                    outgoing,
                    104 + checkpointIndex,
                    4);
    var adjacentWindow = ConstructNonPublic<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentWindowProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryAdjacentWindowProjectionIdKind>(suffix + 5),
                    windowSelection,
                    new[] { checkpoint },
                    global::System.Array.Empty<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeRecoveryCheckpointSupersession<
                                RequestPayload,
                                RecoveryWorldState,
                                CompletionPayload>>(),
                    105 + checkpointIndex,
                    5);
    var continuity = ConstructNonPublic<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointRangeContinuityValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryCheckpointRangeContinuityValidationIdKind>(
                            suffix + 6),
                    summary,
                    adjacentWindow,
                    boundary,
                    106 + checkpointIndex,
                    6);
    return ConstructNonPublic<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryContinuousWindowPairSummaryProjectionIdKind>(
                            suffix + 7),
                    continuity,
                    107 + checkpointIndex,
                    7);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    SyntheticSummary(
        SyntheticAdjacentSequenceContext context,
        int startPairIndex,
        int startCheckpointIndex,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpoint<RequestPayload> checkpoint,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersession<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>? incomingSupersession,
        int suffix) =>
    SyntheticSummary(
        context.SourceProjection,
        context.SourceSequence,
        startPairIndex,
        startCheckpointIndex,
        checkpoint,
        incomingSupersession,
        suffix);

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    SyntheticSummary(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryChainSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> sourceProjection,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowSequenceValidation<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> sourceSequence,
        int startPairIndex,
        int startCheckpointIndex,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpoint<RequestPayload> checkpoint,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersession<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>? incomingSupersession,
        int suffix)
{
    var crossedBoundary = incomingSupersession is null
        ? global::System.Array.Empty<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointSupersession<
                    RequestPayload,
                    RecoveryWorldState,
                    CompletionPayload>>()
        : new[] { incomingSupersession! };
    var range = ConstructNonPublic<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeQuery<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryMultiWindowCheckpointRangeQueryIdKind>(
                            suffix + 1),
                    sourceSequence,
                    new[] { checkpoint },
                    global::System.Array.Empty<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeRecoveryCheckpointSupersession<
                                RequestPayload,
                                RecoveryWorldState,
                                CompletionPayload>>(),
                    crossedBoundary,
                    incomingSupersession,
                    null,
                    startCheckpointIndex,
                    startCheckpointIndex,
                    startPairIndex,
                    startPairIndex,
                    211,
                    2);
    return ConstructNonPublic<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjectionIdKind>(
                            suffix + 2),
                    range,
                    212,
                    3);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryAdjacentSequenceSelection<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    SyntheticSelection(
        SyntheticAdjacentSequenceContext context,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> summary,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>[]? pairSummaries = null,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersession<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>[]? boundarySupersessions = null,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointSupersession<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>? outgoingSupersession = null,
        bool omitOutgoingSupersession = false,
        int suffix = 575000) =>
    ConstructNonPublic<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentSequenceSelection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRecoveryAdjacentSequenceSelectionIdKind>(suffix + 1),
                    summary,
                    true,
                    0,
                    pairSummaries ?? new[] { context.PreviousPair },
                    boundarySupersessions ?? new[] { context.Boundary },
                    context.Boundary,
                    null,
                    omitOutgoingSupersession
                        ? null
                        : outgoingSupersession ?? context.Boundary,
                    213,
                    4);

private static T ConstructNonPublic<T>(params object?[] arguments)
{
    var constructor = global::System.Linq.Enumerable.Single(
        typeof(T).GetConstructors(
            global::System.Reflection.BindingFlags.Instance |
            global::System.Reflection.BindingFlags.NonPublic),
        candidate => candidate.GetParameters().Length == arguments.Length);
    return (T)constructor.Invoke(arguments);
}

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryAdjacentSequenceProjectionIdKind>
    AdjacentSequenceProjectionId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentSequenceProjectionIdKind>(570001);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidationIdKind>
    MultiWindowCheckpointRangeContinuityValidationId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidationIdKind>(570002);

private static void AssertMultiWindowSummaryStatus(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus expected,
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjectionResult<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload> result)
{
    Xunit.Assert.False(result.Succeeded);
    Xunit.Assert.Equal(expected, result.Status);
    Xunit.Assert.Null(result.Summary);
}

private static void AssertAdjacentSequenceStatus(
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus expected,
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryAdjacentSequenceSelectionResult<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload> result)
{
    Xunit.Assert.False(result.Succeeded);
    Xunit.Assert.Equal(expected, result.Status);
    Xunit.Assert.Null(result.Selection);
}

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjectionResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    ProjectMultiWindowSummary(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeQuery<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> range,
        long? expectedRangeRevision = null,
        long? projectedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryFlow.ProjectSummary<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                MultiWindowCheckpointRangeSummaryProjectionId(),
                range,
                expectedRangeRevision ?? range.Revision,
                projectedTick ?? checked(range.QueriedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    MultiWindowSummary(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeQuery<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> range) =>
    Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(ProjectMultiWindowSummary(range).Summary);

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryAdjacentSequenceSelectionResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    SelectPreviousSequence(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> summary,
        int pairCount = 1,
        long? expectedSummaryRevision = null,
        long? selectedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryFlow
        .SelectPreviousSequence<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                AdjacentSequenceSelectionId(),
                summary,
                pairCount,
                expectedSummaryRevision ?? summary.Revision,
                selectedTick ?? checked(summary.ProjectedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryAdjacentSequenceSelectionResult<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    SelectNextSequence(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload> summary,
        int pairCount = 1,
        long? expectedSummaryRevision = null,
        long? selectedTick = null) =>
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryFlow
        .SelectNextSequence<
            RequestPayload,
            RecoveryWorldState,
            CompletionPayload>(
                AdjacentSequenceSelectionId(),
                summary,
                pairCount,
                expectedSummaryRevision ?? summary.Revision,
                selectedTick ?? checked(summary.ProjectedTick + 1));

private static global::AI.Sandbox.Engine.Core.HostRuntime
    .HostRuntimeRecoveryMultiWindowCheckpointRangeQuery<
        RequestPayload,
        RecoveryWorldState,
        CompletionPayload>
    MultiWindowRange()
{
    var sequence = Sequence(PairSummary(PreviousContinuity()));
    return Xunit.Assert.IsType<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeQuery<
                RequestPayload,
                RecoveryWorldState,
                CompletionPayload>>(QueryMultiWindow(sequence).Query);
}

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjectionIdKind>
    MultiWindowCheckpointRangeSummaryProjectionId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjectionIdKind>(560001);

private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
    global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryAdjacentSequenceSelectionIdKind>
    AdjacentSequenceSelectionId() => Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryAdjacentSequenceSelectionIdKind>(560002);

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
