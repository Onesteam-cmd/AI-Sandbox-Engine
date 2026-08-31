namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects compact multi-sequence checkpoint-range summaries and selects exact
/// bounded adjacent summary collections without discovery, reordering, storage,
/// indexing, history mutation, archival, compaction, pagination, diagnostics,
/// scheduling, supervision, waiting, restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryFlow
{
    /// <summary>Maximum summaries represented by one adjacent selection.</summary>
    public const int MaximumAdjacentCollectionSummaryCount =
        HostRuntimeRecoveryContinuousMultiSequenceCollectionFlow.MaximumSummaryCount;

    /// <summary>Projects one compact immutable summary from an exact range.</summary>
    public static HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectSummary<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjectionIdKind> summaryId,
            HostRuntimeRecoveryMultiSequenceCheckpointRangeQuery<
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
                HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus
                    .StaleRangeRevision,
                range);
        }
        if (projectedTick < range.QueriedTick)
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus
                    .RangeSummaryProjectionTickRegressed,
                range);
        }

        var summary =
            new HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion>(
                    summaryId,
                    range,
                    projectedTick,
                    checked(range.Revision + 1));

        return new HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus
                    .MultiSequenceCheckpointRangeSummaryProjected,
                range,
                summary);
    }

    /// <summary>Selects exact summaries immediately before the range.</summary>
    public static HostRuntimeRecoveryAdjacentCollectionSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectPreviousCollection<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentCollectionSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int summaryCount,
            long expectedSummaryRevision,
            long selectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        SelectAdjacentCollection(
            selectionId,
            summary,
            summaryCount,
            expectedSummaryRevision,
            selectedTick,
            selectsPreviousCollection: true);

    /// <summary>Selects exact summaries immediately after the range.</summary>
    public static HostRuntimeRecoveryAdjacentCollectionSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectNextCollection<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentCollectionSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int summaryCount,
            long expectedSummaryRevision,
            long selectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        SelectAdjacentCollection(
            selectionId,
            summary,
            summaryCount,
            expectedSummaryRevision,
            selectedTick,
            selectsPreviousCollection: false);

    private static HostRuntimeRecoveryAdjacentCollectionSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectAdjacentCollection<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentCollectionSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int summaryCount,
            long expectedSummaryRevision,
            long selectedTick,
            bool selectsPreviousCollection)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(selectionId.IsEmpty, nameof(selectionId));
        global::System.ArgumentNullException.ThrowIfNull(summary);
        EnsureSummaryCount(summaryCount, nameof(summaryCount));
        EnsureRevision(expectedSummaryRevision, nameof(expectedSummaryRevision));
        EnsureTick(selectedTick, nameof(selectedTick));

        if (summary.Revision != expectedSummaryRevision)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus
                    .StaleSummaryRevision,
                summary);
        }
        if (selectedTick < summary.ProjectedTick)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus
                    .AdjacentCollectionSelectionTickRegressed,
                summary);
        }
        if (summaryCount > MaximumAdjacentCollectionSummaryCount)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus
                    .TooManyAdjacentCollectionSummaries,
                summary);
        }

        var sourceCollection = summary.Collection;
        int startSummaryIndex;
        int endSummaryIndex;
        int adjacentBoundaryIndex;

        if (selectsPreviousCollection)
        {
            if (summary.StartSummaryIndex == 0)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus
                        .NoPreviousAdjacentCollection,
                    summary);
            }

            endSummaryIndex = summary.StartSummaryIndex - 1;
            startSummaryIndex = checked(endSummaryIndex - summaryCount + 1);
            if (startSummaryIndex < 0)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus
                        .PreviousAdjacentCollectionTooShort,
                    summary);
            }

            adjacentBoundaryIndex = checked((endSummaryIndex * 2) + 1);
        }
        else
        {
            startSummaryIndex = checked(summary.EndSummaryIndex + 1);
            if (startSummaryIndex >= sourceCollection.SummaryCount)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus
                        .NoNextAdjacentCollection,
                    summary);
            }

            endSummaryIndex = checked(startSummaryIndex + summaryCount - 1);
            if (endSummaryIndex >= sourceCollection.SummaryCount)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus
                        .NextAdjacentCollectionTooShort,
                    summary);
            }

            adjacentBoundaryIndex = checked((summary.EndSummaryIndex * 2) + 1);
        }

        var multiSequenceSummaries =
            new HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion>[summaryCount];
        for (var index = 0; index < multiSequenceSummaries.Length; index++)
        {
            multiSequenceSummaries[index] = sourceCollection.MultiSequenceSummaries[
                checked(startSummaryIndex + index)];
        }

        var boundaryCount = checked((summaryCount * 2) - 1);
        var boundaryStartIndex = checked(startSummaryIndex * 2);
        if (boundaryStartIndex < 0 ||
            boundaryStartIndex + boundaryCount >
                sourceCollection.BoundarySupersessions.Count ||
            adjacentBoundaryIndex < 0 ||
            adjacentBoundaryIndex >= sourceCollection.BoundarySupersessions.Count)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus
                    .AdjacentCollectionBoundaryMismatch,
                summary);
        }

        var boundarySupersessions =
            new HostRuntimeRecoveryCheckpointSupersession<
                TRequest,
                TState,
                TCompletion>[boundaryCount];
        for (var index = 0; index < boundarySupersessions.Length; index++)
        {
            boundarySupersessions[index] = sourceCollection.BoundarySupersessions[
                checked(boundaryStartIndex + index)];
        }

        var firstSummary = multiSequenceSummaries[0];
        var lastSummary = multiSequenceSummaries[^1];
        var chain = summary.Chain;
        var incomingSupersession =
            firstSummary.StartCheckpointIndex > 0
                ? chain.Supersessions[firstSummary.StartCheckpointIndex - 1]
                : null;
        var outgoingSupersession =
            lastSummary.EndCheckpointIndex < chain.SupersessionCount
                ? chain.Supersessions[lastSummary.EndCheckpointIndex]
                : null;
        var adjacentBoundarySupersession = selectsPreviousCollection
            ? outgoingSupersession
            : incomingSupersession;
        var expectedBoundary = sourceCollection.BoundarySupersessions[
            adjacentBoundaryIndex];

        if (adjacentBoundarySupersession is null ||
            !global::System.Object.ReferenceEquals(
                adjacentBoundarySupersession,
                expectedBoundary))
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus
                    .AdjacentCollectionBoundaryMismatch,
                summary);
        }

        var selection =
            new HostRuntimeRecoveryAdjacentCollectionSelection<
                TRequest,
                TState,
                TCompletion>(
                    selectionId,
                    summary,
                    selectsPreviousCollection,
                    startSummaryIndex,
                    multiSequenceSummaries,
                    boundarySupersessions,
                    adjacentBoundarySupersession,
                    incomingSupersession,
                    outgoingSupersession,
                    selectedTick,
                    checked(summary.Revision + 1));

        var status = selectsPreviousCollection
            ? HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus
                .PreviousAdjacentCollectionSelected
            : HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus
                .NextAdjacentCollectionSelected;

        return new HostRuntimeRecoveryAdjacentCollectionSelectionResult<
            TRequest,
            TState,
            TCompletion>(status, summary, selection);
    }

    private static HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> SummaryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus status,
            HostRuntimeRecoveryMultiSequenceCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion> range)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, range, summary: null);

    private static HostRuntimeRecoveryAdjacentCollectionSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus status,
            HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection<
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
                "Recovery multi-sequence summary identifiers cannot be empty.",
                parameterName);
        }
    }

    private static void EnsureSummaryCount(int count, string parameterName)
    {
        if (count <= 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                count,
                "Recovery adjacent-collection summary counts must be positive.");
        }
    }

    private static void EnsureRevision(long revision, string parameterName)
    {
        if (revision < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                revision,
                "Recovery multi-sequence summary revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery multi-sequence summary ticks cannot be negative.");
        }
    }
}
