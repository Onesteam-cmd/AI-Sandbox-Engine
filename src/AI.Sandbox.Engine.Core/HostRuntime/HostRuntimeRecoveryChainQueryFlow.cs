namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects validated recovery chains and resolves exact checkpoint lineage
/// without discovery, storage, history mutation, archival, compaction,
/// diagnostics, scheduling, supervision, waiting, restart, or execution.
/// </summary>
public static class HostRuntimeRecoveryChainQueryFlow
{
    /// <summary>
    /// Projects one validated supersession chain into compact immutable summary
    /// authority.
    /// </summary>
    public static HostRuntimeRecoveryChainSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectSummary<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryChainSummaryProjectionIdKind> projectionId,
            HostRuntimeRecoverySupersessionChain<
                TRequest,
                TState,
                TCompletion> chain,
            long expectedChainRevision,
            long projectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(projectionId.IsEmpty, nameof(projectionId));
        global::System.ArgumentNullException.ThrowIfNull(chain);
        EnsureRevision(expectedChainRevision, nameof(expectedChainRevision));
        EnsureTick(projectedTick, nameof(projectedTick));

        if (chain.Revision != expectedChainRevision)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryChainQueryStatus.StaleChainRevision,
                chain);
        }
        if (projectedTick < chain.ValidatedTick)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryChainQueryStatus.ProjectionTickRegressed,
                chain);
        }

        var projection =
            new HostRuntimeRecoveryChainSummaryProjection<
                TRequest,
                TState,
                TCompletion>(
                    projectionId,
                    chain,
                    projectedTick,
                    checked(chain.Revision + 1));

        return new HostRuntimeRecoveryChainSummaryProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryChainQueryStatus.ChainSummaryProjected,
                chain,
                projection);
    }

    /// <summary>
    /// Resolves one exact checkpoint and its incoming and outgoing
    /// supersession lineage from a chain-summary projection.
    /// </summary>
    public static HostRuntimeRecoveryCheckpointLineageQueryResult<
        TRequest,
        TState,
        TCompletion> QueryLineage<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointLineageQueryIdKind> queryId,
            HostRuntimeRecoveryChainSummaryProjection<
                TRequest,
                TState,
                TCompletion> projection,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> checkpointId,
            long expectedProjectionRevision,
            long queriedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(queryId.IsEmpty, nameof(queryId));
        global::System.ArgumentNullException.ThrowIfNull(projection);
        EnsureId(checkpointId.IsEmpty, nameof(checkpointId));
        EnsureRevision(
            expectedProjectionRevision,
            nameof(expectedProjectionRevision));
        EnsureTick(queriedTick, nameof(queriedTick));

        if (projection.Revision != expectedProjectionRevision)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryChainQueryStatus.StaleProjectionRevision,
                projection);
        }
        if (queriedTick < projection.ProjectedTick)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryChainQueryStatus.QueryTickRegressed,
                projection);
        }

        var chain = projection.Chain;
        HostRuntimeRecoveryCheckpoint<TRequest>? checkpoint = null;
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>? incomingSupersession = null;
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>? outgoingSupersession = null;
        var checkpointIndex = -1;

        if (checkpointId == chain.RootCheckpointId)
        {
            checkpoint = chain.RootCheckpoint;
            outgoingSupersession = chain.FirstSupersession;
            checkpointIndex = 0;
        }
        else
        {
            for (var index = 0; index < chain.SupersessionCount; index++)
            {
                var current = chain.Supersessions[index];
                if (current.SuccessorCheckpointId != checkpointId)
                {
                    continue;
                }

                checkpoint = current.SuccessorCheckpoint;
                incomingSupersession = current;
                outgoingSupersession =
                    index + 1 < chain.SupersessionCount
                        ? chain.Supersessions[index + 1]
                        : null;
                checkpointIndex = index + 1;
                break;
            }
        }

        if (checkpoint is null)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryChainQueryStatus.CheckpointNotFound,
                projection);
        }

        var query =
            new HostRuntimeRecoveryCheckpointLineageQuery<
                TRequest,
                TState,
                TCompletion>(
                    queryId,
                    projection,
                    checkpoint,
                    incomingSupersession,
                    outgoingSupersession,
                    checkpointIndex,
                    queriedTick,
                    checked(projection.Revision + 1));

        return new HostRuntimeRecoveryCheckpointLineageQueryResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryChainQueryStatus.CheckpointLineageResolved,
                projection,
                query);
    }

    private static HostRuntimeRecoveryChainSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryChainQueryStatus status,
            HostRuntimeRecoverySupersessionChain<
                TRequest,
                TState,
                TCompletion> chain)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, chain, projection: null);

    private static HostRuntimeRecoveryCheckpointLineageQueryResult<
        TRequest,
        TState,
        TCompletion> QueryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryChainQueryStatus status,
            HostRuntimeRecoveryChainSummaryProjection<
                TRequest,
                TState,
                TCompletion> projection)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, projection, query: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery query identifiers cannot be empty.",
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
                "Recovery query revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery query ticks cannot be negative.");
        }
    }
}
