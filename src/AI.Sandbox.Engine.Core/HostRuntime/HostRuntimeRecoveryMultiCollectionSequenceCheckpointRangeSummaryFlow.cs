namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects compact multi-collection-sequence checkpoint-range summaries and selects exact
/// bounded adjacent multi-collections without discovery, reordering, storage,
/// indexing, history mutation, archival, compaction, pagination, diagnostics,
/// scheduling, supervision, waiting, restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryFlow
{
    /// <summary>Maximum multi-collection summaries represented by one adjacent selection.</summary>
    public const int MaximumAdjacentMultiCollectionCount =
        HostRuntimeRecoveryContinuousMultiCollectionSequenceFlow.MaximumSummaryCount;

    /// <summary>Projects one compact immutable summary from an exact range.</summary>
    public static HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectSummary<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjectionIdKind> summaryId,
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQuery<
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
                HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus
                    .StaleRangeRevision,
                range);
        }
        if (projectedTick < range.QueriedTick)
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus
                    .RangeSummaryProjectionTickRegressed,
                range);
        }

        var summary =
            new HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion>(
                    summaryId,
                    range,
                    projectedTick,
                    checked(range.Revision + 1));

        return new HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus
                    .MultiCollectionSequenceCheckpointRangeSummaryProjected,
                range,
                summary);
    }

    /// <summary>Selects exact multi-collection summaries immediately before the range.</summary>
    public static HostRuntimeRecoveryAdjacentMultiCollectionSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectPreviousMultiCollection<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentMultiCollectionSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int multiCollectionCount,
            long expectedSummaryRevision,
            long selectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        SelectAdjacentMultiCollection(
            selectionId,
            summary,
            multiCollectionCount,
            expectedSummaryRevision,
            selectedTick,
            selectsPreviousMultiCollection: true);

    /// <summary>Selects exact multi-collection summaries immediately after the range.</summary>
    public static HostRuntimeRecoveryAdjacentMultiCollectionSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectNextMultiCollection<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentMultiCollectionSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int multiCollectionCount,
            long expectedSummaryRevision,
            long selectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        SelectAdjacentMultiCollection(
            selectionId,
            summary,
            multiCollectionCount,
            expectedSummaryRevision,
            selectedTick,
            selectsPreviousMultiCollection: false);

    private static HostRuntimeRecoveryAdjacentMultiCollectionSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectAdjacentMultiCollection<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentMultiCollectionSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int multiCollectionCount,
            long expectedSummaryRevision,
            long selectedTick,
            bool selectsPreviousMultiCollection)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(selectionId.IsEmpty, nameof(selectionId));
        global::System.ArgumentNullException.ThrowIfNull(summary);
        EnsureMultiCollectionCount(multiCollectionCount, nameof(multiCollectionCount));
        EnsureRevision(expectedSummaryRevision, nameof(expectedSummaryRevision));
        EnsureTick(selectedTick, nameof(selectedTick));

        if (summary.Revision != expectedSummaryRevision)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus
                    .StaleSummaryRevision,
                summary);
        }
        if (selectedTick < summary.ProjectedTick)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus
                    .AdjacentMultiCollectionSelectionTickRegressed,
                summary);
        }
        if (multiCollectionCount > MaximumAdjacentMultiCollectionCount)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus
                    .TooManyAdjacentMultiCollections,
                summary);
        }

        var sourceSequence = summary.Sequence;
        int startSummaryIndex;
        int endSummaryIndex;
        int adjacentBoundaryIndex;

        if (selectsPreviousMultiCollection)
        {
            if (summary.StartSummaryIndex == 0)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus
                        .NoPreviousAdjacentMultiCollection,
                    summary);
            }

            endSummaryIndex = summary.StartSummaryIndex - 1;
            startSummaryIndex = checked(endSummaryIndex - multiCollectionCount + 1);
            if (startSummaryIndex < 0)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus
                        .PreviousAdjacentMultiCollectionTooShort,
                    summary);
            }

            adjacentBoundaryIndex = checked((endSummaryIndex * 2) + 1);
        }
        else
        {
            startSummaryIndex = checked(summary.EndSummaryIndex + 1);
            if (startSummaryIndex >= sourceSequence.MultiCollectionCount)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus
                        .NoNextAdjacentMultiCollection,
                    summary);
            }

            endSummaryIndex = checked(startSummaryIndex + multiCollectionCount - 1);
            if (endSummaryIndex >= sourceSequence.MultiCollectionCount)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus
                        .NextAdjacentMultiCollectionTooShort,
                    summary);
            }

            adjacentBoundaryIndex = checked((summary.EndSummaryIndex * 2) + 1);
        }

        var multiCollectionSummaries =
            new HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
                TRequest,
                TState,
                TCompletion>[multiCollectionCount];
        for (var index = 0; index < multiCollectionSummaries.Length; index++)
        {
            multiCollectionSummaries[index] = sourceSequence.MultiCollectionSummaries[
                checked(startSummaryIndex + index)];
        }

        var boundaryCount = checked((multiCollectionCount * 2) - 1);
        var boundaryStartIndex = checked(startSummaryIndex * 2);
        if (boundaryStartIndex < 0 ||
            boundaryStartIndex + boundaryCount >
                sourceSequence.BoundarySupersessions.Count ||
            adjacentBoundaryIndex < 0 ||
            adjacentBoundaryIndex >= sourceSequence.BoundarySupersessions.Count)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus
                    .AdjacentMultiCollectionBoundaryMismatch,
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

        var firstSummary = multiCollectionSummaries[0];
        var lastSummary = multiCollectionSummaries[^1];
        var chain = summary.Chain;
        var incomingSupersession =
            firstSummary.StartCheckpointIndex > 0
                ? chain.Supersessions[firstSummary.StartCheckpointIndex - 1]
                : null;
        var outgoingSupersession =
            lastSummary.EndCheckpointIndex < chain.SupersessionCount
                ? chain.Supersessions[lastSummary.EndCheckpointIndex]
                : null;
        var adjacentBoundarySupersession = selectsPreviousMultiCollection
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
                HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus
                    .AdjacentMultiCollectionBoundaryMismatch,
                summary);
        }

        var selection =
            new HostRuntimeRecoveryAdjacentMultiCollectionSelection<
                TRequest,
                TState,
                TCompletion>(
                    selectionId,
                    summary,
                    selectsPreviousMultiCollection,
                    startSummaryIndex,
                    multiCollectionSummaries,
                    boundarySupersessions,
                    adjacentBoundarySupersession,
                    incomingSupersession,
                    outgoingSupersession,
                    selectedTick,
                    checked(summary.Revision + 1));

        var status = selectsPreviousMultiCollection
            ? HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus
                .PreviousAdjacentMultiCollectionSelected
            : HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus
                .NextAdjacentMultiCollectionSelected;

        return new HostRuntimeRecoveryAdjacentMultiCollectionSelectionResult<
            TRequest,
            TState,
            TCompletion>(status, summary, selection);
    }

    private static HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> SummaryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus status,
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion> range)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, range, summary: null);

    private static HostRuntimeRecoveryAdjacentMultiCollectionSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus status,
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<
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
                "Recovery multi-collection-sequence summary identifiers cannot be empty.",
                parameterName);
        }
    }

    private static void EnsureMultiCollectionCount(int count, string parameterName)
    {
        if (count <= 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                count,
                "Recovery adjacent multi-collection counts must be positive.");
        }
    }

    private static void EnsureRevision(long revision, string parameterName)
    {
        if (revision < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                revision,
                "Recovery multi-collection-sequence summary revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery multi-collection-sequence summary ticks cannot be negative.");
        }
    }
}
