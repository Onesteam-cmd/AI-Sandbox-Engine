namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects compact multi-collection checkpoint-range summaries and selects exact
/// bounded adjacent collection-sequences of exact collection-pair summaries without discovery, reordering,
/// storage, indexing, history mutation, archival, compaction, pagination,
/// diagnostics, scheduling, supervision, waiting, restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryFlow
{
    /// <summary>Maximum collection-pair summaries represented by one adjacent selection.</summary>
    public const int MaximumAdjacentCollectionSequencePairCount =
        HostRuntimeRecoveryContinuousCollectionSequenceFlow.MaximumCollectionPairCount;

    /// <summary>Projects one compact immutable summary from an exact range.</summary>
    public static HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectSummary<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjectionIdKind> summaryId,
            HostRuntimeRecoveryMultiCollectionCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion> range,
            long expectedRangeRevision,
            long projectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(summaryId.IsEmpty, nameof(summaryId));
        global::System.ArgumentNullException.ThrowIfNull(range);
        EnsureRevision(expectedRangeRevision, nameof(expectedRangeRevision));
        EnsureTick(projectedTick, nameof(projectedTick));

        if (range.Revision != expectedRangeRevision)
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus
                    .StaleRangeRevision,
                range);
        }
        if (projectedTick < range.QueriedTick)
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus
                    .RangeSummaryProjectionTickRegressed,
                range);
        }

        var summary =
            new HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion>(
                    summaryId,
                    range,
                    projectedTick,
                    checked(range.Revision + 1));

        return new HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus
                    .MultiCollectionCheckpointRangeSummaryProjected,
                range,
                summary);
    }

    /// <summary>Selects exact collection-pair summaries immediately before the range.</summary>
    public static HostRuntimeRecoveryAdjacentCollectionSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectPreviousCollectionSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentCollectionSequenceSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int collectionPairCount,
            long expectedSummaryRevision,
            long selectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        SelectAdjacentCollectionSequence(
            selectionId,
            summary,
            collectionPairCount,
            expectedSummaryRevision,
            selectedTick,
            selectsPreviousCollectionSequence: true);

    /// <summary>Selects exact collection-pair summaries immediately after the range.</summary>
    public static HostRuntimeRecoveryAdjacentCollectionSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectNextCollectionSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentCollectionSequenceSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int collectionPairCount,
            long expectedSummaryRevision,
            long selectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        SelectAdjacentCollectionSequence(
            selectionId,
            summary,
            collectionPairCount,
            expectedSummaryRevision,
            selectedTick,
            selectsPreviousCollectionSequence: false);

    private static HostRuntimeRecoveryAdjacentCollectionSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectAdjacentCollectionSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentCollectionSequenceSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int collectionPairCount,
            long expectedSummaryRevision,
            long selectedTick,
            bool selectsPreviousCollectionSequence)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(selectionId.IsEmpty, nameof(selectionId));
        global::System.ArgumentNullException.ThrowIfNull(summary);
        EnsureCollectionPairCount(collectionPairCount, nameof(collectionPairCount));
        EnsureRevision(expectedSummaryRevision, nameof(expectedSummaryRevision));
        EnsureTick(selectedTick, nameof(selectedTick));

        if (summary.Revision != expectedSummaryRevision)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus
                    .StaleSummaryRevision,
                summary);
        }
        if (selectedTick < summary.ProjectedTick)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus
                    .AdjacentCollectionSequenceSelectionTickRegressed,
                summary);
        }
        if (collectionPairCount > MaximumAdjacentCollectionSequencePairCount)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus
                    .TooManyAdjacentCollectionSequencePairs,
                summary);
        }

        var sourceSequence = summary.Sequence;
        int startCollectionPairIndex;
        int endCollectionPairIndex;
        int adjacentBoundaryIndex;

        if (selectsPreviousCollectionSequence)
        {
            if (summary.StartCollectionPairIndex == 0)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus
                        .NoPreviousAdjacentCollectionSequence,
                    summary);
            }

            endCollectionPairIndex = summary.StartCollectionPairIndex - 1;
            startCollectionPairIndex = checked(endCollectionPairIndex - collectionPairCount + 1);
            if (startCollectionPairIndex < 0)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus
                        .PreviousAdjacentCollectionSequenceTooShort,
                    summary);
            }

            adjacentBoundaryIndex = checked((endCollectionPairIndex * 2) + 1);
        }
        else
        {
            startCollectionPairIndex = checked(summary.EndCollectionPairIndex + 1);
            if (startCollectionPairIndex >= sourceSequence.CollectionPairCount)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus
                        .NoNextAdjacentCollectionSequence,
                    summary);
            }

            endCollectionPairIndex = checked(startCollectionPairIndex + collectionPairCount - 1);
            if (endCollectionPairIndex >= sourceSequence.CollectionPairCount)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus
                        .NextAdjacentCollectionSequenceTooShort,
                    summary);
            }

            adjacentBoundaryIndex = checked((summary.EndCollectionPairIndex * 2) + 1);
        }

        var collectionPairSummaries =
            new HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
                TRequest,
                TState,
                TCompletion>[collectionPairCount];
        for (var index = 0; index < collectionPairSummaries.Length; index++)
        {
            collectionPairSummaries[index] = sourceSequence.CollectionPairSummaries[
                checked(startCollectionPairIndex + index)];
        }

        var boundaryCount = checked((collectionPairCount * 2) - 1);
        var boundaryStartIndex = checked(startCollectionPairIndex * 2);
        if (boundaryStartIndex < 0 ||
            boundaryStartIndex + boundaryCount > sourceSequence.BoundarySupersessions.Count ||
            adjacentBoundaryIndex < 0 ||
            adjacentBoundaryIndex >= sourceSequence.BoundarySupersessions.Count)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus
                    .AdjacentCollectionSequenceBoundaryMismatch,
                summary);
        }

        var boundarySupersessions =
            new HostRuntimeRecoveryCheckpointSupersession<
                TRequest,
                TState,
                TCompletion>[boundaryCount];
        for (var index = 0; index < boundarySupersessions.Length; index++)
        {
            boundarySupersessions[index] = sourceSequence.BoundarySupersessions[
                checked(boundaryStartIndex + index)];
        }

        var firstCollectionPair = collectionPairSummaries[0];
        var lastCollectionPair = collectionPairSummaries[^1];
        var chain = summary.Chain;
        var incomingSupersession =
            firstCollectionPair.StartCheckpointIndex > 0
                ? chain.Supersessions[firstCollectionPair.StartCheckpointIndex - 1]
                : null;
        var outgoingSupersession =
            lastCollectionPair.EndCheckpointIndex < chain.SupersessionCount
                ? chain.Supersessions[lastCollectionPair.EndCheckpointIndex]
                : null;
        var adjacentBoundarySupersession = selectsPreviousCollectionSequence
            ? outgoingSupersession
            : incomingSupersession;
        var expectedBoundary = sourceSequence.BoundarySupersessions[
            adjacentBoundaryIndex];

        if (adjacentBoundarySupersession is null ||
            !global::System.Object.ReferenceEquals(
                adjacentBoundarySupersession,
                expectedBoundary))
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus
                    .AdjacentCollectionSequenceBoundaryMismatch,
                summary);
        }

        var selection =
            new HostRuntimeRecoveryAdjacentCollectionSequenceSelection<
                TRequest,
                TState,
                TCompletion>(
                    selectionId,
                    summary,
                    selectsPreviousCollectionSequence,
                    startCollectionPairIndex,
                    collectionPairSummaries,
                    boundarySupersessions,
                    adjacentBoundarySupersession,
                    incomingSupersession,
                    outgoingSupersession,
                    selectedTick,
                    checked(summary.Revision + 1));

        var status = selectsPreviousCollectionSequence
            ? HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus
                .PreviousAdjacentCollectionSequenceSelected
            : HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus
                .NextAdjacentCollectionSequenceSelected;

        return new HostRuntimeRecoveryAdjacentCollectionSequenceSelectionResult<
            TRequest,
            TState,
            TCompletion>(status, summary, selection);
    }

    private static HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> SummaryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus status,
            HostRuntimeRecoveryMultiCollectionCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion> range)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, range, summary: null);

    private static HostRuntimeRecoveryAdjacentCollectionSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus status,
            HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, summary, selection: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery multi-collection summary identifiers cannot be empty.",
                parameterName);
        }
    }

    private static void EnsureCollectionPairCount(int count, string parameterName)
    {
        if (count <= 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                count,
                "Recovery adjacent collection-sequence pair counts must be positive.");
        }
    }

    private static void EnsureRevision(long revision, string parameterName)
    {
        if (revision < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                revision,
                "Recovery multi-collection summary revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery multi-collection summary ticks cannot be negative.");
        }
    }
}
