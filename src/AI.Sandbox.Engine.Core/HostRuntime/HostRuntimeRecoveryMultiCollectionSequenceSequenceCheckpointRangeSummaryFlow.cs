namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects compact multi-collection-sequence-sequence checkpoint-range summaries and
/// selects exact bounded adjacent multi-collection-sequences without discovery, reordering,
/// storage, indexing, history mutation, archival, compaction, pagination, diagnostics,
/// scheduling, supervision, waiting, restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryFlow
{
    /// <summary>Maximum summary authorities represented by one adjacent selection.</summary>
    public const int MaximumAdjacentMultiCollectionSequenceCount =
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceFlow.MaximumSummaryCount;

    /// <summary>Projects one compact immutable summary from an exact range.</summary>
    public static HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectSummary<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjectionIdKind> summaryId,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeQuery<
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
                HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus
                    .StaleRangeRevision,
                range);
        }
        if (projectedTick < range.QueriedTick)
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus
                    .RangeSummaryProjectionTickRegressed,
                range);
        }

        var summary =
            new HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion>(
                    summaryId,
                    range,
                    projectedTick,
                    checked(range.Revision + 1));

        return new HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus
                    .MultiCollectionSequenceSequenceCheckpointRangeSummaryProjected,
                range,
                summary);
    }

    /// <summary>Selects exact multi-collection-sequence summaries immediately before the range.</summary>
    public static HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectPreviousMultiCollectionSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int multiCollectionSequenceCount,
            long expectedSummaryRevision,
            long selectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        SelectAdjacentMultiCollectionSequence(
            selectionId,
            summary,
            multiCollectionSequenceCount,
            expectedSummaryRevision,
            selectedTick,
            selectsPreviousMultiCollectionSequence: true);

    /// <summary>Selects exact multi-collection-sequence summaries immediately after the range.</summary>
    public static HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectNextMultiCollectionSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int multiCollectionSequenceCount,
            long expectedSummaryRevision,
            long selectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        SelectAdjacentMultiCollectionSequence(
            selectionId,
            summary,
            multiCollectionSequenceCount,
            expectedSummaryRevision,
            selectedTick,
            selectsPreviousMultiCollectionSequence: false);

    private static HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectAdjacentMultiCollectionSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int multiCollectionSequenceCount,
            long expectedSummaryRevision,
            long selectedTick,
            bool selectsPreviousMultiCollectionSequence)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(selectionId.IsEmpty, nameof(selectionId));
        global::System.ArgumentNullException.ThrowIfNull(summary);
        EnsureMultiCollectionSequenceCount(
            multiCollectionSequenceCount,
            nameof(multiCollectionSequenceCount));
        EnsureRevision(expectedSummaryRevision, nameof(expectedSummaryRevision));
        EnsureTick(selectedTick, nameof(selectedTick));

        if (summary.Revision != expectedSummaryRevision)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus
                    .StaleSummaryRevision,
                summary);
        }
        if (selectedTick < summary.ProjectedTick)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus
                    .AdjacentMultiCollectionSequenceSelectionTickRegressed,
                summary);
        }
        if (multiCollectionSequenceCount > MaximumAdjacentMultiCollectionSequenceCount)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus
                    .TooManyAdjacentMultiCollectionSequences,
                summary);
        }

        var sourceSequence = summary.Sequence;
        int startSequenceSummaryIndex;
        int endSequenceSummaryIndex;
        int adjacentBoundaryIndex;

        if (selectsPreviousMultiCollectionSequence)
        {
            if (summary.StartSummaryIndex == 0)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus
                        .NoPreviousAdjacentMultiCollectionSequence,
                    summary);
            }

            endSequenceSummaryIndex = summary.StartSummaryIndex - 1;
            startSequenceSummaryIndex =
                checked(endSequenceSummaryIndex - multiCollectionSequenceCount + 1);
            if (startSequenceSummaryIndex < 0)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus
                        .PreviousAdjacentMultiCollectionSequenceTooShort,
                    summary);
            }

            adjacentBoundaryIndex = checked((endSequenceSummaryIndex * 2) + 1);
        }
        else
        {
            startSequenceSummaryIndex = checked(summary.EndSummaryIndex + 1);
            if (startSequenceSummaryIndex >= sourceSequence.MultiCollectionSequenceSummaryCount)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus
                        .NoNextAdjacentMultiCollectionSequence,
                    summary);
            }

            endSequenceSummaryIndex =
                checked(startSequenceSummaryIndex + multiCollectionSequenceCount - 1);
            if (endSequenceSummaryIndex >= sourceSequence.MultiCollectionSequenceSummaryCount)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus
                        .NextAdjacentMultiCollectionSequenceTooShort,
                    summary);
            }

            adjacentBoundaryIndex = checked((summary.EndSummaryIndex * 2) + 1);
        }

        var multiCollectionSequenceSummaries =
            new HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion>[multiCollectionSequenceCount];
        for (var index = 0; index < multiCollectionSequenceSummaries.Length; index++)
        {
            multiCollectionSequenceSummaries[index] =
                sourceSequence.MultiCollectionSequenceSummaries[
                    checked(startSequenceSummaryIndex + index)];
        }

        var boundaryCount = checked((multiCollectionSequenceCount * 2) - 1);
        var boundaryStartIndex = checked(startSequenceSummaryIndex * 2);
        if (boundaryStartIndex < 0 ||
            boundaryStartIndex + boundaryCount > sourceSequence.BoundarySupersessions.Count ||
            adjacentBoundaryIndex < 0 ||
            adjacentBoundaryIndex >= sourceSequence.BoundarySupersessions.Count)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus
                    .AdjacentMultiCollectionSequenceBoundaryMismatch,
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

        var firstSummary = multiCollectionSequenceSummaries[0];
        var lastSummary = multiCollectionSequenceSummaries[^1];
        var chain = summary.Chain;
        var incomingSupersession =
            firstSummary.StartCheckpointIndex > 0
                ? chain.Supersessions[firstSummary.StartCheckpointIndex - 1]
                : null;
        var outgoingSupersession =
            lastSummary.EndCheckpointIndex < chain.SupersessionCount
                ? chain.Supersessions[lastSummary.EndCheckpointIndex]
                : null;
        var adjacentBoundarySupersession = selectsPreviousMultiCollectionSequence
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
                HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus
                    .AdjacentMultiCollectionSequenceBoundaryMismatch,
                summary);
        }

        var selection =
            new HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection<
                TRequest,
                TState,
                TCompletion>(
                    selectionId,
                    summary,
                    selectsPreviousMultiCollectionSequence,
                    startSequenceSummaryIndex,
                    multiCollectionSequenceSummaries,
                    boundarySupersessions,
                    adjacentBoundarySupersession,
                    incomingSupersession,
                    outgoingSupersession,
                    selectedTick,
                    checked(summary.Revision + 1));

        var status = selectsPreviousMultiCollectionSequence
            ? HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus
                .PreviousAdjacentMultiCollectionSequenceSelected
            : HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus
                .NextAdjacentMultiCollectionSequenceSelected;

        return new HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionResult<
            TRequest,
            TState,
            TCompletion>(status, summary, selection);
    }

    private static HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> SummaryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus status,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion> range)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, range, summary: null);

    private static HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus status,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection<
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
                "Recovery range-summary identifiers cannot be empty.",
                parameterName);
        }
    }

    private static void EnsureMultiCollectionSequenceCount(int count, string parameterName)
    {
        if (count <= 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                count,
                "Recovery adjacent multi-collection-sequence counts must be positive.");
        }
    }

    private static void EnsureRevision(long revision, string parameterName)
    {
        if (revision < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                revision,
                "Recovery range-summary revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery range-summary ticks cannot be negative.");
        }
    }
}
