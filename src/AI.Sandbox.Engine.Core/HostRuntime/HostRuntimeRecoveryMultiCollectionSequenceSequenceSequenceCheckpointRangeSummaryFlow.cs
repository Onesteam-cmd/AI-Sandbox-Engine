namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects compact multi-collection-sequence-sequence-sequence checkpoint-range summaries and
/// selects exact bounded adjacent multi-collection-sequence-sequences without discovery, reordering,
/// storage, indexing, history mutation, archival, compaction, pagination, diagnostics,
/// scheduling, supervision, waiting, restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryFlow
{
    /// <summary>Maximum summary authorities represented by one adjacent selection.</summary>
    public const int MaximumAdjacentMultiCollectionSequenceSequenceCount =
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceFlow.MaximumSummaryCount;

    /// <summary>Projects one compact immutable summary from an exact range.</summary>
    public static HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectSummary<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjectionIdKind> summaryId,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQuery<
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
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus
                    .StaleRangeRevision,
                range);
        }
        if (projectedTick < range.QueriedTick)
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus
                    .RangeSummaryProjectionTickRegressed,
                range);
        }

        var summary =
            new HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion>(
                    summaryId,
                    range,
                    projectedTick,
                    checked(range.Revision + 1));

        return new HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus
                    .MultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjected,
                range,
                summary);
    }

    /// <summary>Selects exact multi-collection-sequence-sequence summaries immediately before the range.</summary>
    public static HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectPreviousMultiCollectionSequenceSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int multiCollectionSequenceSequenceCount,
            long expectedSummaryRevision,
            long selectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        SelectAdjacentMultiCollectionSequenceSequence(
            selectionId,
            summary,
            multiCollectionSequenceSequenceCount,
            expectedSummaryRevision,
            selectedTick,
            selectsPreviousMultiCollectionSequenceSequence: true);

    /// <summary>Selects exact multi-collection-sequence-sequence summaries immediately after the range.</summary>
    public static HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectNextMultiCollectionSequenceSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int multiCollectionSequenceSequenceCount,
            long expectedSummaryRevision,
            long selectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        SelectAdjacentMultiCollectionSequenceSequence(
            selectionId,
            summary,
            multiCollectionSequenceSequenceCount,
            expectedSummaryRevision,
            selectedTick,
            selectsPreviousMultiCollectionSequenceSequence: false);

    private static HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectAdjacentMultiCollectionSequenceSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelectionIdKind> selectionId,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int multiCollectionSequenceSequenceCount,
            long expectedSummaryRevision,
            long selectedTick,
            bool selectsPreviousMultiCollectionSequenceSequence)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(selectionId.IsEmpty, nameof(selectionId));
        global::System.ArgumentNullException.ThrowIfNull(summary);
        EnsureMultiCollectionSequenceSequenceCount(
            multiCollectionSequenceSequenceCount,
            nameof(multiCollectionSequenceSequenceCount));
        EnsureRevision(expectedSummaryRevision, nameof(expectedSummaryRevision));
        EnsureTick(selectedTick, nameof(selectedTick));

        if (summary.Revision != expectedSummaryRevision)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus
                    .StaleSummaryRevision,
                summary);
        }
        if (selectedTick < summary.ProjectedTick)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus
                    .AdjacentMultiCollectionSequenceSequenceSelectionTickRegressed,
                summary);
        }
        if (multiCollectionSequenceSequenceCount > MaximumAdjacentMultiCollectionSequenceSequenceCount)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus
                    .TooManyAdjacentMultiCollectionSequenceSequences,
                summary);
        }

        var sourceSequence = summary.Sequence;
        int startSequenceSequenceSummaryIndex;
        int endSequenceSequenceSummaryIndex;
        int adjacentBoundaryIndex;

        if (selectsPreviousMultiCollectionSequenceSequence)
        {
            if (summary.StartSummaryIndex == 0)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus
                        .NoPreviousAdjacentMultiCollectionSequenceSequence,
                    summary);
            }

            endSequenceSequenceSummaryIndex = summary.StartSummaryIndex - 1;
            startSequenceSequenceSummaryIndex =
                checked(endSequenceSequenceSummaryIndex - multiCollectionSequenceSequenceCount + 1);
            if (startSequenceSequenceSummaryIndex < 0)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus
                        .PreviousAdjacentMultiCollectionSequenceSequenceTooShort,
                    summary);
            }

            adjacentBoundaryIndex = checked((endSequenceSequenceSummaryIndex * 2) + 1);
        }
        else
        {
            startSequenceSequenceSummaryIndex = checked(summary.EndSummaryIndex + 1);
            if (startSequenceSequenceSummaryIndex >= sourceSequence.MultiCollectionSequenceSequenceSummaryCount)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus
                        .NoNextAdjacentMultiCollectionSequenceSequence,
                    summary);
            }

            endSequenceSequenceSummaryIndex =
                checked(startSequenceSequenceSummaryIndex + multiCollectionSequenceSequenceCount - 1);
            if (endSequenceSequenceSummaryIndex >= sourceSequence.MultiCollectionSequenceSequenceSummaryCount)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus
                        .NextAdjacentMultiCollectionSequenceSequenceTooShort,
                    summary);
            }

            adjacentBoundaryIndex = checked((summary.EndSummaryIndex * 2) + 1);
        }

        var multiCollectionSequenceSequenceSummaries =
            new HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion>[multiCollectionSequenceSequenceCount];
        for (var index = 0; index < multiCollectionSequenceSequenceSummaries.Length; index++)
        {
            multiCollectionSequenceSequenceSummaries[index] =
                sourceSequence.MultiCollectionSequenceSequenceSummaries[
                    checked(startSequenceSequenceSummaryIndex + index)];
        }

        var boundaryCount = checked((multiCollectionSequenceSequenceCount * 2) - 1);
        var boundaryStartIndex = checked(startSequenceSequenceSummaryIndex * 2);
        if (boundaryStartIndex < 0 ||
            boundaryStartIndex + boundaryCount > sourceSequence.BoundarySupersessions.Count ||
            adjacentBoundaryIndex < 0 ||
            adjacentBoundaryIndex >= sourceSequence.BoundarySupersessions.Count)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus
                    .AdjacentMultiCollectionSequenceSequenceBoundaryMismatch,
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

        var firstSummary = multiCollectionSequenceSequenceSummaries[0];
        var lastSummary = multiCollectionSequenceSequenceSummaries[^1];
        var chain = summary.Chain;
        var incomingSupersession =
            firstSummary.StartCheckpointIndex > 0
                ? chain.Supersessions[firstSummary.StartCheckpointIndex - 1]
                : null;
        var outgoingSupersession =
            lastSummary.EndCheckpointIndex < chain.SupersessionCount
                ? chain.Supersessions[lastSummary.EndCheckpointIndex]
                : null;
        var adjacentBoundarySupersession = selectsPreviousMultiCollectionSequenceSequence
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
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus
                    .AdjacentMultiCollectionSequenceSequenceBoundaryMismatch,
                summary);
        }

        var selection =
            new HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelection<
                TRequest,
                TState,
                TCompletion>(
                    selectionId,
                    summary,
                    selectsPreviousMultiCollectionSequenceSequence,
                    startSequenceSequenceSummaryIndex,
                    multiCollectionSequenceSequenceSummaries,
                    boundarySupersessions,
                    adjacentBoundarySupersession,
                    incomingSupersession,
                    outgoingSupersession,
                    selectedTick,
                    checked(summary.Revision + 1));

        var status = selectsPreviousMultiCollectionSequenceSequence
            ? HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus
                .PreviousAdjacentMultiCollectionSequenceSequenceSelected
            : HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus
                .NextAdjacentMultiCollectionSequenceSequenceSelected;

        return new HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelectionResult<
            TRequest,
            TState,
            TCompletion>(status, summary, selection);
    }

    private static HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> SummaryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus status,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion> range)
        where TRequest : IHostRuntimeRequest
        where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, range, summary: null);

    private static HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus status,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjection<
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

    private static void EnsureMultiCollectionSequenceSequenceCount(int count, string parameterName)
    {
        if (count <= 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                count,
                "Recovery adjacent multi-collection-sequence-sequence counts must be positive.");
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
