namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects compact summaries over one exact continuous recovery multi-collection sequence
/// formed by a summarized range and one projected adjacent multi-collection, and resolves
/// bounded inclusive ranges crossing their shared boundary without discovery, reordering,
/// storage, indexing, history mutation, archival,
/// compaction, pagination, diagnostics, scheduling, supervision, waiting,
/// restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryFlow
{
    /// <summary>Maximum checkpoints represented by one cross-multi-collection query.</summary>
    public const int MaximumCheckpointCount =
        HostRuntimeRecoveryLineageWindowFlow.MaximumCheckpointCount;

    /// <summary>Projects one compact summary over an exact validated continuous multi-collection sequence.</summary>
    public static HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectSummary<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjectionIdKind> summaryId,
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidation<
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
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus
                    .StaleContinuityRevision,
                continuity);
        }
        if (projectedTick < continuity.ValidatedTick)
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus
                    .MultiCollectionSequenceSummaryProjectionTickRegressed,
                continuity);
        }

        var summary = continuity.Summary;
        var adjacentMultiCollection = continuity.AdjacentMultiCollection;
        if (!global::System.Object.ReferenceEquals(
                adjacentMultiCollection.Summary,
                summary))
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus
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
            TCompletion>? multiCollectionBoundary;

        if (continuity.ValidatesPreviousMultiCollection)
        {
            if (adjacentMultiCollection.EndSummaryIndex + 1 != summary.StartSummaryIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus
                        .MultiCollectionRangeNotContinuous,
                    continuity);
            }
            if (adjacentMultiCollection.EndCheckpointIndex + 1 != summary.StartCheckpointIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus
                        .MultiCollectionCheckpointRangeNotContinuous,
                    continuity);
            }

            priorCheckpoint = adjacentMultiCollection.EndCheckpoint;
            successorCheckpoint = summary.StartCheckpoint;
            summaryBoundary = summary.IncomingSupersession;
            multiCollectionBoundary = adjacentMultiCollection.OutgoingSupersession;
        }
        else
        {
            if (summary.EndSummaryIndex + 1 != adjacentMultiCollection.StartSummaryIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus
                        .MultiCollectionRangeNotContinuous,
                    continuity);
            }
            if (summary.EndCheckpointIndex + 1 != adjacentMultiCollection.StartCheckpointIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus
                        .MultiCollectionCheckpointRangeNotContinuous,
                    continuity);
            }

            priorCheckpoint = summary.EndCheckpoint;
            successorCheckpoint = adjacentMultiCollection.StartCheckpoint;
            summaryBoundary = summary.OutgoingSupersession;
            multiCollectionBoundary = adjacentMultiCollection.IncomingSupersession;
        }

        if (summaryBoundary is null ||
            !global::System.Object.ReferenceEquals(summaryBoundary, multiCollectionBoundary) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                continuity.ConnectingSupersession))
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus
                    .MultiCollectionSupersessionMismatch,
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
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus
                    .MultiCollectionCheckpointMismatch,
                continuity);
        }

        var multiCollectionSequenceSummary =
            new HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion>(
                    summaryId,
                    continuity,
                    projectedTick,
                    checked(continuity.Revision + 1));

        return new HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus
                    .ContinuousMultiCollectionSequenceSummaryProjected,
                continuity,
                multiCollectionSequenceSummary);
    }

    /// <summary>
    /// Resolves one exact bounded inclusive checkpoint range that crosses the
    /// shared supersession boundary of one continuous recovery multi-collection sequence.
    /// </summary>
    public static HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryRange<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQueryIdKind> queryId,
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion> multiCollectionSequenceSummary,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> startCheckpointId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> endCheckpointId,
            long expectedMultiCollectionSequenceSummaryRevision,
            long queriedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(queryId.IsEmpty, nameof(queryId));
        global::System.ArgumentNullException.ThrowIfNull(multiCollectionSequenceSummary);
        EnsureId(startCheckpointId.IsEmpty, nameof(startCheckpointId));
        EnsureId(endCheckpointId.IsEmpty, nameof(endCheckpointId));
        EnsureRevision(expectedMultiCollectionSequenceSummaryRevision, nameof(expectedMultiCollectionSequenceSummaryRevision));
        EnsureTick(queriedTick, nameof(queriedTick));

        if (multiCollectionSequenceSummary.Revision != expectedMultiCollectionSequenceSummaryRevision)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus
                    .StaleMultiCollectionSequenceSummaryRevision,
                multiCollectionSequenceSummary);
        }
        if (queriedTick < multiCollectionSequenceSummary.ProjectedTick)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus
                    .CrossMultiCollectionRangeQueryTickRegressed,
                multiCollectionSequenceSummary);
        }

        var chain = multiCollectionSequenceSummary.Chain;
        var startCheckpointIndex = FindCheckpointIndex(
            chain,
            multiCollectionSequenceSummary.StartCheckpointIndex,
            multiCollectionSequenceSummary.EndCheckpointIndex,
            startCheckpointId);
        if (startCheckpointIndex < 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus.RangeStartNotFound,
                multiCollectionSequenceSummary);
        }

        var endCheckpointIndex = FindCheckpointIndex(
            chain,
            multiCollectionSequenceSummary.StartCheckpointIndex,
            multiCollectionSequenceSummary.EndCheckpointIndex,
            endCheckpointId);
        if (endCheckpointIndex < 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus.RangeEndNotFound,
                multiCollectionSequenceSummary);
        }
        if (endCheckpointIndex < startCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus.RangeOrderInvalid,
                multiCollectionSequenceSummary);
        }
        if (startCheckpointIndex > multiCollectionSequenceSummary.ConnectingPriorCheckpointIndex ||
            endCheckpointIndex < multiCollectionSequenceSummary.ConnectingSuccessorCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus
                    .RangeDoesNotCrossMultiCollectionBoundary,
                multiCollectionSequenceSummary);
        }

        var checkpointCount =
            checked(endCheckpointIndex - startCheckpointIndex + 1);
        if (checkpointCount > MaximumCheckpointCount)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus.RangeTooLarge,
                multiCollectionSequenceSummary);
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
            multiCollectionSequenceSummary.ConnectingPriorCheckpointIndex - startCheckpointIndex);
        if (connectingOffset < 0 ||
            connectingOffset >= supersessions.Length ||
            !global::System.Object.ReferenceEquals(
                supersessions[connectingOffset],
                multiCollectionSequenceSummary.ConnectingSupersession))
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus
                    .RangeSupersessionMismatch,
                multiCollectionSequenceSummary);
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
            new HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion>(
                    queryId,
                    multiCollectionSequenceSummary,
                    checkpoints,
                    supersessions,
                    incomingSupersession,
                    outgoingSupersession,
                    startCheckpointIndex,
                    endCheckpointIndex,
                    queriedTick,
                    checked(multiCollectionSequenceSummary.Revision + 1));

        return new HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQueryResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus
                    .CrossMultiCollectionCheckpointRangeQueried,
                multiCollectionSequenceSummary,
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

    private static HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> SummaryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus status,
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidation<
                TRequest,
                TState,
                TCompletion> continuity)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, continuity, summary: null);

    private static HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus status,
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion> multiCollectionSequenceSummary)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, multiCollectionSequenceSummary, query: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery continuous multi-collection-sequence summary identifiers cannot be empty.",
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
                "Recovery continuous multi-collection-sequence summary revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery continuous multi-collection-sequence summary ticks cannot be negative.");
        }
    }
}
