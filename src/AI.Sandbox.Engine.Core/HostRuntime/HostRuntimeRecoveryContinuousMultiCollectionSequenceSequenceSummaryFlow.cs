namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects compact summaries over one exact continuous recovery multi-collection-sequence-sequence
/// formed by a summarized range and one projected adjacent multi-collection-sequence, and resolves
/// bounded inclusive ranges crossing their shared boundary without discovery, reordering,
/// storage, indexing, history mutation, archival,
/// compaction, pagination, diagnostics, scheduling, supervision, waiting,
/// restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryFlow
{
    /// <summary>Maximum checkpoints represented by one cross-multi-collection-sequence query.</summary>
    public const int MaximumCheckpointCount =
        HostRuntimeRecoveryLineageWindowFlow.MaximumCheckpointCount;

    /// <summary>Projects one compact summary over an exact validated continuous multi-collection-sequence-sequence.</summary>
    public static HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectSummary<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjectionIdKind> summaryId,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidation<
                TRequest,
                TState,
                TCompletion> continuity,
            long expectedContinuityRevision,
            long projectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(summaryId.IsEmpty, nameof(summaryId));
        global::System.ArgumentNullException.ThrowIfNull(continuity);
        EnsureRevision(expectedContinuityRevision, nameof(expectedContinuityRevision));
        EnsureTick(projectedTick, nameof(projectedTick));

        if (continuity.Revision != expectedContinuityRevision)
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus
                    .StaleContinuityRevision,
                continuity);
        }
        if (projectedTick < continuity.ValidatedTick)
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus
                    .MultiCollectionSequenceSequenceSummaryProjectionTickRegressed,
                continuity);
        }

        var summary = continuity.Summary;
        var adjacentMultiCollectionSequence = continuity.AdjacentMultiCollectionSequence;
        if (!global::System.Object.ReferenceEquals(
                adjacentMultiCollectionSequence.Summary,
                summary))
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus
                    .ContinuitySourceMismatch,
                continuity);
        }

        HostRuntimeRecoveryCheckpoint<TRequest> priorCheckpoint;
        HostRuntimeRecoveryCheckpoint<TRequest> successorCheckpoint;
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>? summaryBoundary;
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>? multiCollectionSequenceBoundary;

        if (continuity.ValidatesPreviousMultiCollectionSequence)
        {
            if (adjacentMultiCollectionSequence.EndSequenceSummaryIndex + 1 != summary.StartSummaryIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus
                        .MultiCollectionSequenceRangeNotContinuous,
                    continuity);
            }
            if (adjacentMultiCollectionSequence.EndCheckpointIndex + 1 != summary.StartCheckpointIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus
                        .MultiCollectionSequenceCheckpointRangeNotContinuous,
                    continuity);
            }

            priorCheckpoint = adjacentMultiCollectionSequence.EndCheckpoint;
            successorCheckpoint = summary.StartCheckpoint;
            summaryBoundary = summary.IncomingSupersession;
            multiCollectionSequenceBoundary = adjacentMultiCollectionSequence.OutgoingSupersession;
        }
        else
        {
            if (summary.EndSummaryIndex + 1 != adjacentMultiCollectionSequence.StartSequenceSummaryIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus
                        .MultiCollectionSequenceRangeNotContinuous,
                    continuity);
            }
            if (summary.EndCheckpointIndex + 1 != adjacentMultiCollectionSequence.StartCheckpointIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus
                        .MultiCollectionSequenceCheckpointRangeNotContinuous,
                    continuity);
            }

            priorCheckpoint = summary.EndCheckpoint;
            successorCheckpoint = adjacentMultiCollectionSequence.StartCheckpoint;
            summaryBoundary = summary.OutgoingSupersession;
            multiCollectionSequenceBoundary = adjacentMultiCollectionSequence.IncomingSupersession;
        }

        if (summaryBoundary is null ||
            !global::System.Object.ReferenceEquals(summaryBoundary, multiCollectionSequenceBoundary) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                continuity.ConnectingSupersession))
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus
                    .MultiCollectionSequenceSupersessionMismatch,
                continuity);
        }
        if (!global::System.Object.ReferenceEquals(
                summaryBoundary.PriorCheckpoint,
                priorCheckpoint) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary.SuccessorCheckpoint,
                successorCheckpoint))
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus
                    .MultiCollectionSequenceCheckpointMismatch,
                continuity);
        }

        var multiCollectionSequenceSequenceSummary =
            new HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion>(
                    summaryId,
                    continuity,
                    projectedTick,
                    checked(continuity.Revision + 1));

        return new HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus
                    .ContinuousMultiCollectionSequenceSequenceSummaryProjected,
                continuity,
                multiCollectionSequenceSequenceSummary);
    }

    /// <summary>
    /// Resolves one exact bounded inclusive checkpoint range that crosses the
    /// shared supersession boundary of one continuous recovery multi-collection-sequence-sequence.
    /// </summary>
    public static HostRuntimeRecoveryCrossMultiCollectionSequenceCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryRange<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCrossMultiCollectionSequenceCheckpointRangeQueryIdKind> queryId,
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion> multiCollectionSequenceSequenceSummary,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> startCheckpointId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> endCheckpointId,
            long expectedMultiCollectionSequenceSequenceSummaryRevision,
            long queriedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(queryId.IsEmpty, nameof(queryId));
        global::System.ArgumentNullException.ThrowIfNull(multiCollectionSequenceSequenceSummary);
        EnsureId(startCheckpointId.IsEmpty, nameof(startCheckpointId));
        EnsureId(endCheckpointId.IsEmpty, nameof(endCheckpointId));
        EnsureRevision(expectedMultiCollectionSequenceSequenceSummaryRevision, nameof(expectedMultiCollectionSequenceSequenceSummaryRevision));
        EnsureTick(queriedTick, nameof(queriedTick));

        if (multiCollectionSequenceSequenceSummary.Revision != expectedMultiCollectionSequenceSequenceSummaryRevision)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus
                    .StaleMultiCollectionSequenceSequenceSummaryRevision,
                multiCollectionSequenceSequenceSummary);
        }
        if (queriedTick < multiCollectionSequenceSequenceSummary.ProjectedTick)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus
                    .CrossMultiCollectionSequenceRangeQueryTickRegressed,
                multiCollectionSequenceSequenceSummary);
        }

        var chain = multiCollectionSequenceSequenceSummary.Chain;
        var startCheckpointIndex = FindCheckpointIndex(
            chain,
            multiCollectionSequenceSequenceSummary.StartCheckpointIndex,
            multiCollectionSequenceSequenceSummary.EndCheckpointIndex,
            startCheckpointId);
        if (startCheckpointIndex < 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus.RangeStartNotFound,
                multiCollectionSequenceSequenceSummary);
        }

        var endCheckpointIndex = FindCheckpointIndex(
            chain,
            multiCollectionSequenceSequenceSummary.StartCheckpointIndex,
            multiCollectionSequenceSequenceSummary.EndCheckpointIndex,
            endCheckpointId);
        if (endCheckpointIndex < 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus.RangeEndNotFound,
                multiCollectionSequenceSequenceSummary);
        }
        if (endCheckpointIndex < startCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus.RangeOrderInvalid,
                multiCollectionSequenceSequenceSummary);
        }
        if (startCheckpointIndex > multiCollectionSequenceSequenceSummary.ConnectingPriorCheckpointIndex ||
            endCheckpointIndex < multiCollectionSequenceSequenceSummary.ConnectingSuccessorCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus
                    .RangeDoesNotCrossMultiCollectionSequenceBoundary,
                multiCollectionSequenceSequenceSummary);
        }

        var checkpointCount =
            checked(endCheckpointIndex - startCheckpointIndex + 1);
        if (checkpointCount > MaximumCheckpointCount)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus.RangeTooLarge,
                multiCollectionSequenceSequenceSummary);
        }

        var checkpoints =
            new HostRuntimeRecoveryCheckpoint<TRequest>[checkpointCount];
        for (var index = 0; index < checkpoints.Length; index++)
        {
            checkpoints[index] = CheckpointAt(
                chain,
                checked(startCheckpointIndex + index));
        }

        var supersessions =
            new HostRuntimeRecoveryCheckpointSupersession<
                TRequest,
                TState,
                TCompletion>[checkpointCount - 1];
        for (var index = 0; index < supersessions.Length; index++)
        {
            supersessions[index] = chain.Supersessions[
                checked(startCheckpointIndex + index)];
        }

        var connectingOffset = checked(
            multiCollectionSequenceSequenceSummary.ConnectingPriorCheckpointIndex - startCheckpointIndex);
        if (connectingOffset < 0 ||
            connectingOffset >= supersessions.Length ||
            !global::System.Object.ReferenceEquals(
                supersessions[connectingOffset],
                multiCollectionSequenceSequenceSummary.ConnectingSupersession))
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus
                    .RangeSupersessionMismatch,
                multiCollectionSequenceSequenceSummary);
        }

        var incomingSupersession =
            startCheckpointIndex > 0
                ? chain.Supersessions[startCheckpointIndex - 1]
                : null;
        var outgoingSupersession =
            endCheckpointIndex < chain.SupersessionCount
                ? chain.Supersessions[endCheckpointIndex]
                : null;

        var query =
            new HostRuntimeRecoveryCrossMultiCollectionSequenceCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion>(
                    queryId,
                    multiCollectionSequenceSequenceSummary,
                    checkpoints,
                    supersessions,
                    incomingSupersession,
                    outgoingSupersession,
                    startCheckpointIndex,
                    endCheckpointIndex,
                    queriedTick,
                    checked(multiCollectionSequenceSequenceSummary.Revision + 1));

        return new HostRuntimeRecoveryCrossMultiCollectionSequenceCheckpointRangeQueryResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus
                    .CrossMultiCollectionSequenceCheckpointRangeQueried,
                multiCollectionSequenceSequenceSummary,
                query);
    }

    private static HostRuntimeRecoveryCheckpoint<TRequest>
        CheckpointAt<TRequest, TState, TCompletion>(
            HostRuntimeRecoverySupersessionChain<
                TRequest,
                TState,
                TCompletion> chain,
            int checkpointIndex)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        checkpointIndex == 0
            ? chain.RootCheckpoint
            : chain.Supersessions[checkpointIndex - 1].SuccessorCheckpoint;

    private static int FindCheckpointIndex<TRequest, TState, TCompletion>(
        HostRuntimeRecoverySupersessionChain<
            TRequest,
            TState,
            TCompletion> chain,
        int startCheckpointIndex,
        int endCheckpointIndex,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryCheckpointIdKind> checkpointId)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        for (var index = startCheckpointIndex; index <= endCheckpointIndex; index++)
        {
            if (CheckpointAt(chain, index).CheckpointId == checkpointId)
            {
                return index;
            }
        }

        return -1;
    }

    private static HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> SummaryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus status,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidation<
                TRequest,
                TState,
                TCompletion> continuity)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, continuity, summary: null);

    private static HostRuntimeRecoveryCrossMultiCollectionSequenceCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus status,
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion> multiCollectionSequenceSequenceSummary)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, multiCollectionSequenceSequenceSummary, query: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery continuous multi-collection-sequence-sequence summary identifiers cannot be empty.",
                parameterName);
        }
    }

    private static void EnsureRevision(long revision, string parameterName)
    {
        if (revision < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                revision,
                "Recovery continuous multi-collection-sequence-sequence summary revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery continuous multi-collection-sequence-sequence summary ticks cannot be negative.");
        }
    }
}
