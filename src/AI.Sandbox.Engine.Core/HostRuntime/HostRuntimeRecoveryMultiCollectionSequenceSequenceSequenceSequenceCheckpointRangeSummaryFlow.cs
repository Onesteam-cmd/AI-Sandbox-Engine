namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects compact multi-collection-sequence-sequence-sequence-sequence checkpoint-range summaries and
/// selects exact bounded adjacent multi-collection-sequence-sequence-sequences without discovery, reordering,
/// storage, indexing, history mutation, archival, compaction, pagination, diagnostics,
/// scheduling, supervision, waiting, restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryFlow
{
    /// <summary>Maximum summary authorities represented by one adjacent selection.</summary>
    public const int MaximumAdjacentMultiCollectionSequenceSequenceSequenceCount =
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceFlow.MaximumSummaryCount;

    /// <summary>Projects one compact immutable summary from an exact range.</summary>
    public static HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectSummary<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjectionIdKind> summaryId,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQuery<
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
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus
                    .StaleRangeRevision,
                range);
        }
        if (projectedTick < range.QueriedTick)
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus
                    .RangeSummaryProjectionTickRegressed,
                range);
        }

        var summary =
            new HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion>(
                    summaryId,
                    range,
                    projectedTick,
                    checked(range.Revision + 1));

        return new HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus
                    .MultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjected,
                range,
                summary);
    }

    /// <summary>Selects exact multi-collection-sequence-sequence-sequence summaries immediately before the range.</summary>
    public static HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectPreviousMultiCollectionSequenceSequenceSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int multiCollectionSequenceSequenceSequenceCount,
            long expectedSummaryRevision,
            long selectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        SelectAdjacentMultiCollectionSequenceSequenceSequence(
            selectionId,
            summary,
            multiCollectionSequenceSequenceSequenceCount,
            expectedSummaryRevision,
            selectedTick,
            selectsPreviousMultiCollectionSequenceSequenceSequence: true);

    /// <summary>Selects exact multi-collection-sequence-sequence-sequence summaries immediately after the range.</summary>
    public static HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectNextMultiCollectionSequenceSequenceSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int multiCollectionSequenceSequenceSequenceCount,
            long expectedSummaryRevision,
            long selectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        SelectAdjacentMultiCollectionSequenceSequenceSequence(
            selectionId,
            summary,
            multiCollectionSequenceSequenceSequenceCount,
            expectedSummaryRevision,
            selectedTick,
            selectsPreviousMultiCollectionSequenceSequenceSequence: false);

    private static HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectAdjacentMultiCollectionSequenceSequenceSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int multiCollectionSequenceSequenceSequenceCount,
            long expectedSummaryRevision,
            long selectedTick,
            bool selectsPreviousMultiCollectionSequenceSequenceSequence)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(selectionId.IsEmpty, nameof(selectionId));
        global::System.ArgumentNullException.ThrowIfNull(summary);
        EnsureMultiCollectionSequenceSequenceSequenceCount(
            multiCollectionSequenceSequenceSequenceCount,
            nameof(multiCollectionSequenceSequenceSequenceCount));
        EnsureRevision(expectedSummaryRevision, nameof(expectedSummaryRevision));
        EnsureTick(selectedTick, nameof(selectedTick));

        if (summary.Revision != expectedSummaryRevision)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus
                    .StaleSummaryRevision,
                summary);
        }
        if (selectedTick < summary.ProjectedTick)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus
                    .AdjacentMultiCollectionSequenceSequenceSequenceSelectionTickRegressed,
                summary);
        }
        if (multiCollectionSequenceSequenceSequenceCount > MaximumAdjacentMultiCollectionSequenceSequenceSequenceCount)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus
                    .TooManyAdjacentMultiCollectionSequenceSequenceSequences,
                summary);
        }

        var sourceSequence = summary.Sequence;
        int startSequenceSequenceSequenceSummaryIndex;
        int endSequenceSequenceSequenceSummaryIndex;
        int adjacentBoundaryIndex;

        if (selectsPreviousMultiCollectionSequenceSequenceSequence)
        {
            if (summary.StartSummaryIndex == 0)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus
                        .NoPreviousAdjacentMultiCollectionSequenceSequenceSequence,
                    summary);
            }

            endSequenceSequenceSequenceSummaryIndex = summary.StartSummaryIndex - 1;
            startSequenceSequenceSequenceSummaryIndex =
                checked(endSequenceSequenceSequenceSummaryIndex - multiCollectionSequenceSequenceSequenceCount + 1);
            if (startSequenceSequenceSequenceSummaryIndex < 0)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus
                        .PreviousAdjacentMultiCollectionSequenceSequenceSequenceTooShort,
                    summary);
            }

            adjacentBoundaryIndex = checked((endSequenceSequenceSequenceSummaryIndex * 2) + 1);
        }
        else
        {
            startSequenceSequenceSequenceSummaryIndex = checked(summary.EndSummaryIndex + 1);
            if (startSequenceSequenceSequenceSummaryIndex >= sourceSequence.MultiCollectionSequenceSequenceSequenceSummaryCount)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus
                        .NoNextAdjacentMultiCollectionSequenceSequenceSequence,
                    summary);
            }

            endSequenceSequenceSequenceSummaryIndex =
                checked(startSequenceSequenceSequenceSummaryIndex + multiCollectionSequenceSequenceSequenceCount - 1);
            if (endSequenceSequenceSequenceSummaryIndex >= sourceSequence.MultiCollectionSequenceSequenceSequenceSummaryCount)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus
                        .NextAdjacentMultiCollectionSequenceSequenceSequenceTooShort,
                    summary);
            }

            adjacentBoundaryIndex = checked((summary.EndSummaryIndex * 2) + 1);
        }

        var multiCollectionSequenceSequenceSequenceSummaries =
            new HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion>[multiCollectionSequenceSequenceSequenceCount];
        for (var index = 0; index < multiCollectionSequenceSequenceSequenceSummaries.Length; index++)
        {
            multiCollectionSequenceSequenceSequenceSummaries[index] =
                sourceSequence.MultiCollectionSequenceSequenceSequenceSummaries[
                    checked(startSequenceSequenceSequenceSummaryIndex + index)];
        }

        var boundaryCount = checked((multiCollectionSequenceSequenceSequenceCount * 2) - 1);
        var boundaryStartIndex = checked(startSequenceSequenceSequenceSummaryIndex * 2);
        if (boundaryStartIndex < 0 ||
            boundaryStartIndex + boundaryCount > sourceSequence.BoundarySupersessions.Count ||
            adjacentBoundaryIndex < 0 ||
            adjacentBoundaryIndex >= sourceSequence.BoundarySupersessions.Count)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus
                    .AdjacentMultiCollectionSequenceSequenceSequenceBoundaryMismatch,
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

        var firstSummary = multiCollectionSequenceSequenceSequenceSummaries[0];
        var lastSummary = multiCollectionSequenceSequenceSequenceSummaries[^1];
        var chain = summary.Chain;
        var incomingSupersession =
            firstSummary.StartCheckpointIndex > 0
                ? chain.Supersessions[firstSummary.StartCheckpointIndex - 1]
                : null;
        var outgoingSupersession =
            lastSummary.EndCheckpointIndex < chain.SupersessionCount
                ? chain.Supersessions[lastSummary.EndCheckpointIndex]
                : null;
        var adjacentBoundarySupersession = selectsPreviousMultiCollectionSequenceSequenceSequence
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
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus
                    .AdjacentMultiCollectionSequenceSequenceSequenceBoundaryMismatch,
                summary);
        }

        var selection =
            new HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelection<
                TRequest,
                TState,
                TCompletion>(
                    selectionId,
                    summary,
                    selectsPreviousMultiCollectionSequenceSequenceSequence,
                    startSequenceSequenceSequenceSummaryIndex,
                    multiCollectionSequenceSequenceSequenceSummaries,
                    boundarySupersessions,
                    adjacentBoundarySupersession,
                    incomingSupersession,
                    outgoingSupersession,
                    selectedTick,
                    checked(summary.Revision + 1));

        var status = selectsPreviousMultiCollectionSequenceSequenceSequence
            ? HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus
                .PreviousAdjacentMultiCollectionSequenceSequenceSequenceSelected
            : HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus
                .NextAdjacentMultiCollectionSequenceSequenceSequenceSelected;

        return new HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionResult<
            TRequest,
            TState,
            TCompletion>(status, summary, selection);
    }

    private static HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> SummaryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus status,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion> range)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, range, summary: null);

    private static HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus status,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection<
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

    private static void EnsureMultiCollectionSequenceSequenceSequenceCount(int count, string parameterName)
    {
        if (count <= 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                count,
                "Recovery adjacent multi-collection-sequence-sequence-sequence counts must be positive.");
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
