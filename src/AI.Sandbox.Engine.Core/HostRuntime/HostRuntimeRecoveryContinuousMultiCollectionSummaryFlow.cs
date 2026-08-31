namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects compact summaries over one exact continuous recovery multi-collection
/// formed by a summarized range and one projected adjacent sequence, and resolves
/// bounded inclusive ranges crossing their shared boundary without discovery, reordering,
/// storage, indexing, history mutation, archival,
/// compaction, pagination, diagnostics, scheduling, supervision, waiting,
/// restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryContinuousMultiCollectionSummaryFlow
{
    /// <summary>Maximum checkpoints represented by one cross-collection-sequence query.</summary>
    public const int MaximumCheckpointCount =
        HostRuntimeRecoveryLineageWindowFlow.MaximumCheckpointCount;

    /// <summary>Projects one compact summary over an exact validated continuous multi-collection.</summary>
    public static HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectSummary<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionIdKind> summaryId,
            HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidation<
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
                HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus
                    .StaleContinuityRevision,
                continuity);
        }
        if (projectedTick < continuity.ValidatedTick)
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus
                    .MultiCollectionSummaryProjectionTickRegressed,
                continuity);
        }

        var summary = continuity.Summary;
        var adjacentCollectionSequence = continuity.AdjacentCollectionSequence;
        if (!global::System.Object.ReferenceEquals(
                adjacentCollectionSequence.Summary,
                summary))
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus
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
            TCompletion>? collectionSequenceBoundary;

        if (continuity.ValidatesPreviousCollectionSequence)
        {
            if (adjacentCollectionSequence.EndCollectionPairIndex + 1 != summary.StartCollectionPairIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus
                        .CollectionPairRangeNotContinuous,
                    continuity);
            }
            if (adjacentCollectionSequence.EndCheckpointIndex + 1 != summary.StartCheckpointIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus
                        .CollectionSequenceCheckpointRangeNotContinuous,
                    continuity);
            }

            priorCheckpoint = adjacentCollectionSequence.EndCheckpoint;
            successorCheckpoint = summary.StartCheckpoint;
            summaryBoundary = summary.IncomingSupersession;
            collectionSequenceBoundary = adjacentCollectionSequence.OutgoingSupersession;
        }
        else
        {
            if (summary.EndCollectionPairIndex + 1 != adjacentCollectionSequence.StartCollectionPairIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus
                        .CollectionPairRangeNotContinuous,
                    continuity);
            }
            if (summary.EndCheckpointIndex + 1 != adjacentCollectionSequence.StartCheckpointIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus
                        .CollectionSequenceCheckpointRangeNotContinuous,
                    continuity);
            }

            priorCheckpoint = summary.EndCheckpoint;
            successorCheckpoint = adjacentCollectionSequence.StartCheckpoint;
            summaryBoundary = summary.OutgoingSupersession;
            collectionSequenceBoundary = adjacentCollectionSequence.IncomingSupersession;
        }

        if (summaryBoundary is null ||
            !global::System.Object.ReferenceEquals(summaryBoundary, collectionSequenceBoundary) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                continuity.ConnectingSupersession))
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus
                    .CollectionSequenceSupersessionMismatch,
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
                HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus
                    .CollectionSequenceCheckpointMismatch,
                continuity);
        }

        var multiCollectionSummary =
            new HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
                TRequest,
                TState,
                TCompletion>(
                    summaryId,
                    continuity,
                    projectedTick,
                    checked(continuity.Revision + 1));

        return new HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus
                    .ContinuousMultiCollectionSummaryProjected,
                continuity,
                multiCollectionSummary);
    }

    /// <summary>
    /// Resolves one exact bounded inclusive checkpoint range that crosses the
    /// shared supersession boundary of one continuous recovery multi-collection.
    /// </summary>
    public static HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryRange<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQueryIdKind> queryId,
            HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
                TRequest,
                TState,
                TCompletion> multiCollectionSummary,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> startCheckpointId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> endCheckpointId,
            long expectedMultiCollectionSummaryRevision,
            long queriedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(queryId.IsEmpty, nameof(queryId));
        global::System.ArgumentNullException.ThrowIfNull(multiCollectionSummary);
        EnsureId(startCheckpointId.IsEmpty, nameof(startCheckpointId));
        EnsureId(endCheckpointId.IsEmpty, nameof(endCheckpointId));
        EnsureRevision(expectedMultiCollectionSummaryRevision, nameof(expectedMultiCollectionSummaryRevision));
        EnsureTick(queriedTick, nameof(queriedTick));

        if (multiCollectionSummary.Revision != expectedMultiCollectionSummaryRevision)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus
                    .StaleMultiCollectionSummaryRevision,
                multiCollectionSummary);
        }
        if (queriedTick < multiCollectionSummary.ProjectedTick)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus
                    .CrossCollectionSequenceRangeQueryTickRegressed,
                multiCollectionSummary);
        }

        var chain = multiCollectionSummary.Chain;
        var startCheckpointIndex = FindCheckpointIndex(
            chain,
            multiCollectionSummary.StartCheckpointIndex,
            multiCollectionSummary.EndCheckpointIndex,
            startCheckpointId);
        if (startCheckpointIndex < 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus.RangeStartNotFound,
                multiCollectionSummary);
        }

        var endCheckpointIndex = FindCheckpointIndex(
            chain,
            multiCollectionSummary.StartCheckpointIndex,
            multiCollectionSummary.EndCheckpointIndex,
            endCheckpointId);
        if (endCheckpointIndex < 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus.RangeEndNotFound,
                multiCollectionSummary);
        }
        if (endCheckpointIndex < startCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus.RangeOrderInvalid,
                multiCollectionSummary);
        }
        if (startCheckpointIndex > multiCollectionSummary.ConnectingPriorCheckpointIndex ||
            endCheckpointIndex < multiCollectionSummary.ConnectingSuccessorCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus
                    .RangeDoesNotCrossCollectionSequenceBoundary,
                multiCollectionSummary);
        }

        var checkpointCount =
            checked(endCheckpointIndex - startCheckpointIndex + 1);
        if (checkpointCount > MaximumCheckpointCount)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus.RangeTooLarge,
                multiCollectionSummary);
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
            multiCollectionSummary.ConnectingPriorCheckpointIndex - startCheckpointIndex);
        if (connectingOffset < 0 ||
            connectingOffset >= supersessions.Length ||
            !global::System.Object.ReferenceEquals(
                supersessions[connectingOffset],
                multiCollectionSummary.ConnectingSupersession))
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus
                    .RangeSupersessionMismatch,
                multiCollectionSummary);
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
            new HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion>(
                    queryId,
                    multiCollectionSummary,
                    checkpoints,
                    supersessions,
                    incomingSupersession,
                    outgoingSupersession,
                    startCheckpointIndex,
                    endCheckpointIndex,
                    queriedTick,
                    checked(multiCollectionSummary.Revision + 1));

        return new HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQueryResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus
                    .CrossCollectionSequenceCheckpointRangeQueried,
                multiCollectionSummary,
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

    private static HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> SummaryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus status,
            HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidation<
                TRequest,
                TState,
                TCompletion> continuity)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, continuity, summary: null);

    private static HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus status,
            HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
                TRequest,
                TState,
                TCompletion> multiCollectionSummary)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, multiCollectionSummary, query: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery continuous multi-collection summary identifiers cannot be empty.",
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
                "Recovery continuous multi-collection summary revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery continuous multi-collection summary ticks cannot be negative.");
        }
    }
}
