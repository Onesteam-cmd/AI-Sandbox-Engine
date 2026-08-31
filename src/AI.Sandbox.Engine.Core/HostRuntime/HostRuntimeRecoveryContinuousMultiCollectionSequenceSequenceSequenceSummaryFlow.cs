namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects compact summaries over one exact continuous recovery multi-collection-sequence-sequence-sequence
/// formed by a summarized range and one projected adjacent multi-collection-sequence-sequence, and resolves
/// bounded inclusive ranges crossing their shared boundary without discovery, reordering,
/// storage, indexing, history mutation, archival,
/// compaction, pagination, diagnostics, scheduling, supervision, waiting,
/// restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryFlow
{
    /// <summary>Maximum checkpoints represented by one cross-multi-collection-sequence-sequence query.</summary>
    public const int MaximumCheckpointCount =
        HostRuntimeRecoveryLineageWindowFlow.MaximumCheckpointCount;

    /// <summary>Projects one compact summary over an exact validated continuous multi-collection-sequence-sequence-sequence.</summary>
    public static HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectSummary<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjectionIdKind> summaryId,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidation<
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
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus
                    .StaleContinuityRevision,
                continuity);
        }
        if (projectedTick < continuity.ValidatedTick)
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus
                    .MultiCollectionSequenceSequenceSequenceSummaryProjectionTickRegressed,
                continuity);
        }

        var summary = continuity.Summary;
        var adjacentMultiCollectionSequenceSequence = continuity.AdjacentMultiCollectionSequenceSequence;
        if (!global::System.Object.ReferenceEquals(
                adjacentMultiCollectionSequenceSequence.Summary,
                summary))
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus
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
            TCompletion>? multiCollectionSequenceSequenceBoundary;

        if (continuity.ValidatesPreviousMultiCollectionSequenceSequence)
        {
            if (adjacentMultiCollectionSequenceSequence.EndSequenceSequenceSummaryIndex + 1 != summary.StartSummaryIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus
                        .MultiCollectionSequenceSequenceRangeNotContinuous,
                    continuity);
            }
            if (adjacentMultiCollectionSequenceSequence.EndCheckpointIndex + 1 != summary.StartCheckpointIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus
                        .MultiCollectionSequenceSequenceCheckpointRangeNotContinuous,
                    continuity);
            }

            priorCheckpoint = adjacentMultiCollectionSequenceSequence.EndCheckpoint;
            successorCheckpoint = summary.StartCheckpoint;
            summaryBoundary = summary.IncomingSupersession;
            multiCollectionSequenceSequenceBoundary = adjacentMultiCollectionSequenceSequence.OutgoingSupersession;
        }
        else
        {
            if (summary.EndSummaryIndex + 1 != adjacentMultiCollectionSequenceSequence.StartSequenceSequenceSummaryIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus
                        .MultiCollectionSequenceSequenceRangeNotContinuous,
                    continuity);
            }
            if (summary.EndCheckpointIndex + 1 != adjacentMultiCollectionSequenceSequence.StartCheckpointIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus
                        .MultiCollectionSequenceSequenceCheckpointRangeNotContinuous,
                    continuity);
            }

            priorCheckpoint = summary.EndCheckpoint;
            successorCheckpoint = adjacentMultiCollectionSequenceSequence.StartCheckpoint;
            summaryBoundary = summary.OutgoingSupersession;
            multiCollectionSequenceSequenceBoundary = adjacentMultiCollectionSequenceSequence.IncomingSupersession;
        }

        if (summaryBoundary is null ||
            !global::System.Object.ReferenceEquals(summaryBoundary, multiCollectionSequenceSequenceBoundary) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                continuity.ConnectingSupersession))
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus
                    .MultiCollectionSequenceSequenceSupersessionMismatch,
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
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus
                    .MultiCollectionSequenceSequenceCheckpointMismatch,
                continuity);
        }

        var multiCollectionSequenceSequenceSequenceSummary =
            new HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion>(
                    summaryId,
                    continuity,
                    projectedTick,
                    checked(continuity.Revision + 1));

        return new HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus
                    .ContinuousMultiCollectionSequenceSequenceSequenceSummaryProjected,
                continuity,
                multiCollectionSequenceSequenceSequenceSummary);
    }

    /// <summary>
    /// Resolves one exact bounded inclusive checkpoint range that crosses the
    /// shared supersession boundary of one continuous recovery multi-collection-sequence-sequence-sequence.
    /// </summary>
    public static HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryRange<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceCheckpointRangeQueryIdKind> queryId,
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion> multiCollectionSequenceSequenceSequenceSummary,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> startCheckpointId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> endCheckpointId,
            long expectedMultiCollectionSequenceSequenceSequenceSummaryRevision,
            long queriedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(queryId.IsEmpty, nameof(queryId));
        global::System.ArgumentNullException.ThrowIfNull(multiCollectionSequenceSequenceSequenceSummary);
        EnsureId(startCheckpointId.IsEmpty, nameof(startCheckpointId));
        EnsureId(endCheckpointId.IsEmpty, nameof(endCheckpointId));
        EnsureRevision(expectedMultiCollectionSequenceSequenceSequenceSummaryRevision, nameof(expectedMultiCollectionSequenceSequenceSequenceSummaryRevision));
        EnsureTick(queriedTick, nameof(queriedTick));

        if (multiCollectionSequenceSequenceSequenceSummary.Revision != expectedMultiCollectionSequenceSequenceSequenceSummaryRevision)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus
                    .StaleMultiCollectionSequenceSequenceSequenceSummaryRevision,
                multiCollectionSequenceSequenceSequenceSummary);
        }
        if (queriedTick < multiCollectionSequenceSequenceSequenceSummary.ProjectedTick)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus
                    .CrossMultiCollectionSequenceSequenceRangeQueryTickRegressed,
                multiCollectionSequenceSequenceSequenceSummary);
        }

        var chain = multiCollectionSequenceSequenceSequenceSummary.Chain;
        var startCheckpointIndex = FindCheckpointIndex(
            chain,
            multiCollectionSequenceSequenceSequenceSummary.StartCheckpointIndex,
            multiCollectionSequenceSequenceSequenceSummary.EndCheckpointIndex,
            startCheckpointId);
        if (startCheckpointIndex < 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus.RangeStartNotFound,
                multiCollectionSequenceSequenceSequenceSummary);
        }

        var endCheckpointIndex = FindCheckpointIndex(
            chain,
            multiCollectionSequenceSequenceSequenceSummary.StartCheckpointIndex,
            multiCollectionSequenceSequenceSequenceSummary.EndCheckpointIndex,
            endCheckpointId);
        if (endCheckpointIndex < 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus.RangeEndNotFound,
                multiCollectionSequenceSequenceSequenceSummary);
        }
        if (endCheckpointIndex < startCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus.RangeOrderInvalid,
                multiCollectionSequenceSequenceSequenceSummary);
        }
        if (startCheckpointIndex > multiCollectionSequenceSequenceSequenceSummary.ConnectingPriorCheckpointIndex ||
            endCheckpointIndex < multiCollectionSequenceSequenceSequenceSummary.ConnectingSuccessorCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus
                    .RangeDoesNotCrossMultiCollectionSequenceSequenceBoundary,
                multiCollectionSequenceSequenceSequenceSummary);
        }

        var checkpointCount =
            checked(endCheckpointIndex - startCheckpointIndex + 1);
        if (checkpointCount > MaximumCheckpointCount)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus.RangeTooLarge,
                multiCollectionSequenceSequenceSequenceSummary);
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
            multiCollectionSequenceSequenceSequenceSummary.ConnectingPriorCheckpointIndex - startCheckpointIndex);
        if (connectingOffset < 0 ||
            connectingOffset >= supersessions.Length ||
            !global::System.Object.ReferenceEquals(
                supersessions[connectingOffset],
                multiCollectionSequenceSequenceSequenceSummary.ConnectingSupersession))
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus
                    .RangeSupersessionMismatch,
                multiCollectionSequenceSequenceSequenceSummary);
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
            new HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion>(
                    queryId,
                    multiCollectionSequenceSequenceSequenceSummary,
                    checkpoints,
                    supersessions,
                    incomingSupersession,
                    outgoingSupersession,
                    startCheckpointIndex,
                    endCheckpointIndex,
                    queriedTick,
                    checked(multiCollectionSequenceSequenceSequenceSummary.Revision + 1));

        return new HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceCheckpointRangeQueryResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus
                    .CrossMultiCollectionSequenceSequenceCheckpointRangeQueried,
                multiCollectionSequenceSequenceSequenceSummary,
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

    private static HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> SummaryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus status,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidation<
                TRequest,
                TState,
                TCompletion> continuity)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, continuity, summary: null);

    private static HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus status,
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion> multiCollectionSequenceSequenceSequenceSummary)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, multiCollectionSequenceSequenceSequenceSummary, query: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery continuous multi-collection-sequence-sequence-sequence summary identifiers cannot be empty.",
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
                "Recovery continuous multi-collection-sequence-sequence-sequence summary revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery continuous multi-collection-sequence-sequence-sequence summary ticks cannot be negative.");
        }
    }
}
