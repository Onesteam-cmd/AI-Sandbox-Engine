namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects compact multi-window checkpoint-range summaries and selects exact
/// bounded adjacent pair-summary sequences without discovery, reordering,
/// storage, indexing, history mutation, archival, compaction, pagination,
/// diagnostics, scheduling, supervision, waiting, restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryFlow
{
    /// <summary>Maximum pair summaries represented by one adjacent selection.</summary>
    public const int MaximumAdjacentSequencePairCount =
        HostRuntimeRecoveryContinuousWindowSequenceFlow.MaximumPairCount;

    /// <summary>Projects one compact immutable summary from an exact range.</summary>
    public static HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectSummary<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjectionIdKind> summaryId,
            HostRuntimeRecoveryMultiWindowCheckpointRangeQuery<
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
                HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus
                    .StaleRangeRevision,
                range);
        }
        if (projectedTick < range.QueriedTick)
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus
                    .RangeSummaryProjectionTickRegressed,
                range);
        }

        var summary =
            new HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion>(
                    summaryId,
                    range,
                    projectedTick,
                    checked(range.Revision + 1));

        return new HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus
                    .MultiWindowCheckpointRangeSummaryProjected,
                range,
                summary);
    }

    /// <summary>Selects exact pair summaries immediately before the range.</summary>
    public static HostRuntimeRecoveryAdjacentSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectPreviousSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentSequenceSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int pairCount,
            long expectedSummaryRevision,
            long selectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        SelectAdjacentSequence(
            selectionId,
            summary,
            pairCount,
            expectedSummaryRevision,
            selectedTick,
            selectsPreviousSequence: true);

    /// <summary>Selects exact pair summaries immediately after the range.</summary>
    public static HostRuntimeRecoveryAdjacentSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectNextSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentSequenceSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int pairCount,
            long expectedSummaryRevision,
            long selectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        SelectAdjacentSequence(
            selectionId,
            summary,
            pairCount,
            expectedSummaryRevision,
            selectedTick,
            selectsPreviousSequence: false);

    private static HostRuntimeRecoveryAdjacentSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectAdjacentSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentSequenceSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int pairCount,
            long expectedSummaryRevision,
            long selectedTick,
            bool selectsPreviousSequence)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(selectionId.IsEmpty, nameof(selectionId));
        global::System.ArgumentNullException.ThrowIfNull(summary);
        EnsurePairCount(pairCount, nameof(pairCount));
        EnsureRevision(expectedSummaryRevision, nameof(expectedSummaryRevision));
        EnsureTick(selectedTick, nameof(selectedTick));

        if (summary.Revision != expectedSummaryRevision)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus
                    .StaleSummaryRevision,
                summary);
        }
        if (selectedTick < summary.ProjectedTick)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus
                    .AdjacentSequenceSelectionTickRegressed,
                summary);
        }
        if (pairCount > MaximumAdjacentSequencePairCount)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus
                    .TooManyAdjacentSequencePairs,
                summary);
        }

        var sourceSequence = summary.Sequence;
        int startPairIndex;
        int endPairIndex;
        int adjacentBoundaryIndex;

        if (selectsPreviousSequence)
        {
            if (summary.StartPairIndex == 0)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus
                        .NoPreviousAdjacentSequence,
                    summary);
            }

            endPairIndex = summary.StartPairIndex - 1;
            startPairIndex = checked(endPairIndex - pairCount + 1);
            if (startPairIndex < 0)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus
                        .PreviousAdjacentSequenceTooShort,
                    summary);
            }

            adjacentBoundaryIndex = checked((endPairIndex * 2) + 1);
        }
        else
        {
            startPairIndex = checked(summary.EndPairIndex + 1);
            if (startPairIndex >= sourceSequence.PairCount)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus
                        .NoNextAdjacentSequence,
                    summary);
            }

            endPairIndex = checked(startPairIndex + pairCount - 1);
            if (endPairIndex >= sourceSequence.PairCount)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus
                        .NextAdjacentSequenceTooShort,
                    summary);
            }

            adjacentBoundaryIndex = checked((summary.EndPairIndex * 2) + 1);
        }

        var pairSummaries =
            new HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                TRequest,
                TState,
                TCompletion>[pairCount];
        for (var index = 0; index < pairSummaries.Length; index++)
        {
            pairSummaries[index] = sourceSequence.PairSummaries[
                checked(startPairIndex + index)];
        }

        var boundaryCount = checked((pairCount * 2) - 1);
        var boundaryStartIndex = checked(startPairIndex * 2);
        if (boundaryStartIndex < 0 ||
            boundaryStartIndex + boundaryCount > sourceSequence.BoundarySupersessions.Count ||
            adjacentBoundaryIndex < 0 ||
            adjacentBoundaryIndex >= sourceSequence.BoundarySupersessions.Count)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus
                    .AdjacentSequenceBoundaryMismatch,
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

        var firstPair = pairSummaries[0];
        var lastPair = pairSummaries[^1];
        var chain = summary.Chain;
        var incomingSupersession =
            firstPair.StartCheckpointIndex > 0
                ? chain.Supersessions[firstPair.StartCheckpointIndex - 1]
                : null;
        var outgoingSupersession =
            lastPair.EndCheckpointIndex < chain.SupersessionCount
                ? chain.Supersessions[lastPair.EndCheckpointIndex]
                : null;
        var adjacentBoundarySupersession = selectsPreviousSequence
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
                HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus
                    .AdjacentSequenceBoundaryMismatch,
                summary);
        }

        var selection =
            new HostRuntimeRecoveryAdjacentSequenceSelection<
                TRequest,
                TState,
                TCompletion>(
                    selectionId,
                    summary,
                    selectsPreviousSequence,
                    startPairIndex,
                    pairSummaries,
                    boundarySupersessions,
                    adjacentBoundarySupersession,
                    incomingSupersession,
                    outgoingSupersession,
                    selectedTick,
                    checked(summary.Revision + 1));

        var status = selectsPreviousSequence
            ? HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus
                .PreviousAdjacentSequenceSelected
            : HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus
                .NextAdjacentSequenceSelected;

        return new HostRuntimeRecoveryAdjacentSequenceSelectionResult<
            TRequest,
            TState,
            TCompletion>(status, summary, selection);
    }

    private static HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> SummaryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus status,
            HostRuntimeRecoveryMultiWindowCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion> range)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, range, summary: null);

    private static HostRuntimeRecoveryAdjacentSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus status,
            HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
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
                "Recovery multi-window summary identifiers cannot be empty.",
                parameterName);
        }
    }

    private static void EnsurePairCount(int count, string parameterName)
    {
        if (count <= 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                count,
                "Recovery adjacent-sequence pair counts must be positive.");
        }
    }

    private static void EnsureRevision(long revision, string parameterName)
    {
        if (revision < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                revision,
                "Recovery multi-window summary revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery multi-window summary ticks cannot be negative.");
        }
    }
}
