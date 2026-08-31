namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects compact summaries over one exact continuous recovery collection-pair
/// formed by a summarized range and one projected adjacent collection, and resolves
/// bounded inclusive ranges crossing their shared boundary without
/// discovery, reordering, storage, indexing, history mutation, archival,
/// compaction, pagination, diagnostics, scheduling, supervision, waiting,
/// restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryContinuousCollectionPairFlow
{
    /// <summary>Maximum checkpoints represented by one cross-collection query.</summary>
    public const int MaximumCheckpointCount =
        HostRuntimeRecoveryLineageWindowFlow.MaximumCheckpointCount;

    /// <summary>Projects one compact summary over an exact validated continuous collection-pair.</summary>
    public static HostRuntimeRecoveryContinuousCollectionPairSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectSummary<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryContinuousCollectionPairSummaryProjectionIdKind> summaryId,
            HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidation<
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
                HostRuntimeRecoveryContinuousCollectionPairStatus
                    .StaleContinuityRevision,
                continuity);
        }
        if (projectedTick < continuity.ValidatedTick)
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionPairStatus
                    .CollectionPairSummaryProjectionTickRegressed,
                continuity);
        }

        var summary = continuity.Summary;
        var adjacentCollection = continuity.AdjacentCollection;
        if (!global::System.Object.ReferenceEquals(
                adjacentCollection.Summary,
                summary))
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionPairStatus
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
            TCompletion>? collectionBoundary;

        if (continuity.ValidatesPreviousCollection)
        {
            if (adjacentCollection.EndSummaryIndex + 1 != summary.StartSummaryIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousCollectionPairStatus
                        .CollectionPairNotContinuous,
                    continuity);
            }
            if (adjacentCollection.EndCheckpointIndex + 1 != summary.StartCheckpointIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousCollectionPairStatus
                        .CollectionCheckpointRangeNotContinuous,
                    continuity);
            }

            priorCheckpoint = adjacentCollection.EndCheckpoint;
            successorCheckpoint = summary.StartCheckpoint;
            summaryBoundary = summary.IncomingSupersession;
            collectionBoundary = adjacentCollection.OutgoingSupersession;
        }
        else
        {
            if (summary.EndSummaryIndex + 1 != adjacentCollection.StartSummaryIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousCollectionPairStatus
                        .CollectionPairNotContinuous,
                    continuity);
            }
            if (summary.EndCheckpointIndex + 1 != adjacentCollection.StartCheckpointIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousCollectionPairStatus
                        .CollectionCheckpointRangeNotContinuous,
                    continuity);
            }

            priorCheckpoint = summary.EndCheckpoint;
            successorCheckpoint = adjacentCollection.StartCheckpoint;
            summaryBoundary = summary.OutgoingSupersession;
            collectionBoundary = adjacentCollection.IncomingSupersession;
        }

        if (summaryBoundary is null ||
            !global::System.Object.ReferenceEquals(summaryBoundary, collectionBoundary) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                continuity.ConnectingSupersession))
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionPairStatus
                    .CollectionPairSupersessionMismatch,
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
                HostRuntimeRecoveryContinuousCollectionPairStatus
                    .CollectionPairCheckpointMismatch,
                continuity);
        }

        var collectionPairSummary =
            new HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
                TRequest,
                TState,
                TCompletion>(
                    summaryId,
                    continuity,
                    projectedTick,
                    checked(continuity.Revision + 1));

        return new HostRuntimeRecoveryContinuousCollectionPairSummaryProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousCollectionPairStatus
                    .ContinuousCollectionPairSummaryProjected,
                continuity,
                collectionPairSummary);
    }

    /// <summary>
    /// Resolves one exact bounded inclusive checkpoint range that crosses the
    /// shared supersession boundary of one continuous recovery collection-pair.
    /// </summary>
    public static HostRuntimeRecoveryCrossCollectionCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryRange<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCrossCollectionCheckpointRangeQueryIdKind> queryId,
            HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
                TRequest,
                TState,
                TCompletion> collectionPairSummary,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> startCheckpointId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> endCheckpointId,
            long expectedCollectionPairSummaryRevision,
            long queriedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(queryId.IsEmpty, nameof(queryId));
        global::System.ArgumentNullException.ThrowIfNull(collectionPairSummary);
        EnsureId(startCheckpointId.IsEmpty, nameof(startCheckpointId));
        EnsureId(endCheckpointId.IsEmpty, nameof(endCheckpointId));
        EnsureRevision(expectedCollectionPairSummaryRevision, nameof(expectedCollectionPairSummaryRevision));
        EnsureTick(queriedTick, nameof(queriedTick));

        if (collectionPairSummary.Revision != expectedCollectionPairSummaryRevision)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionPairStatus
                    .StaleCollectionPairSummaryRevision,
                collectionPairSummary);
        }
        if (queriedTick < collectionPairSummary.ProjectedTick)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionPairStatus
                    .CrossCollectionRangeQueryTickRegressed,
                collectionPairSummary);
        }

        var chain = collectionPairSummary.Chain;
        var startCheckpointIndex = FindCheckpointIndex(
            chain,
            collectionPairSummary.StartCheckpointIndex,
            collectionPairSummary.EndCheckpointIndex,
            startCheckpointId);
        if (startCheckpointIndex < 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionPairStatus.RangeStartNotFound,
                collectionPairSummary);
        }

        var endCheckpointIndex = FindCheckpointIndex(
            chain,
            collectionPairSummary.StartCheckpointIndex,
            collectionPairSummary.EndCheckpointIndex,
            endCheckpointId);
        if (endCheckpointIndex < 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionPairStatus.RangeEndNotFound,
                collectionPairSummary);
        }
        if (endCheckpointIndex < startCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionPairStatus.RangeOrderInvalid,
                collectionPairSummary);
        }
        if (startCheckpointIndex > collectionPairSummary.ConnectingPriorCheckpointIndex ||
            endCheckpointIndex < collectionPairSummary.ConnectingSuccessorCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionPairStatus
                    .RangeDoesNotCrossCollectionBoundary,
                collectionPairSummary);
        }

        var checkpointCount =
            checked(endCheckpointIndex - startCheckpointIndex + 1);
        if (checkpointCount > MaximumCheckpointCount)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionPairStatus.RangeTooLarge,
                collectionPairSummary);
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
            collectionPairSummary.ConnectingPriorCheckpointIndex - startCheckpointIndex);
        if (connectingOffset < 0 ||
            connectingOffset >= supersessions.Length ||
            !global::System.Object.ReferenceEquals(
                supersessions[connectingOffset],
                collectionPairSummary.ConnectingSupersession))
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionPairStatus
                    .RangeSupersessionMismatch,
                collectionPairSummary);
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
            new HostRuntimeRecoveryCrossCollectionCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion>(
                    queryId,
                    collectionPairSummary,
                    checkpoints,
                    supersessions,
                    incomingSupersession,
                    outgoingSupersession,
                    startCheckpointIndex,
                    endCheckpointIndex,
                    queriedTick,
                    checked(collectionPairSummary.Revision + 1));

        return new HostRuntimeRecoveryCrossCollectionCheckpointRangeQueryResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousCollectionPairStatus
                    .CrossCollectionCheckpointRangeQueried,
                collectionPairSummary,
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

    private static HostRuntimeRecoveryContinuousCollectionPairSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> SummaryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousCollectionPairStatus status,
            HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidation<
                TRequest,
                TState,
                TCompletion> continuity)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, continuity, summary: null);

    private static HostRuntimeRecoveryCrossCollectionCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousCollectionPairStatus status,
            HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
                TRequest,
                TState,
                TCompletion> collectionPairSummary)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, collectionPairSummary, query: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery continuous collection-pair identifiers cannot be empty.",
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
                "Recovery continuous collection-pair revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery continuous collection-pair ticks cannot be negative.");
        }
    }
}
