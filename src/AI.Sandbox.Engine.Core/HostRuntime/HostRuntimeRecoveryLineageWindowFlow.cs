namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects bounded contiguous recovery lineage windows and resolves
/// inclusive checkpoint ranges without discovery, storage, indexing, history mutation,
/// archival, compaction, diagnostics, scheduling, supervision, waiting,
/// restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryLineageWindowFlow
{
    /// <summary>Maximum checkpoints represented by one lineage window.</summary>
    public const int MaximumCheckpointCount = 64;

    /// <summary>
    /// Projects one contiguous bounded checkpoint window from an exact
    /// chain-summary projection.
    /// </summary>
    public static HostRuntimeRecoveryLineageWindowProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectWindow<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryLineageWindowProjectionIdKind> windowId,
            HostRuntimeRecoveryChainSummaryProjection<
                TRequest,
                TState,
                TCompletion> sourceProjection,
            int startCheckpointIndex,
            int checkpointCount,
            long expectedProjectionRevision,
            long projectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(windowId.IsEmpty, nameof(windowId));
        global::System.ArgumentNullException.ThrowIfNull(sourceProjection);
        EnsureIndex(startCheckpointIndex, nameof(startCheckpointIndex));
        EnsureCount(checkpointCount, nameof(checkpointCount));
        EnsureRevision(
            expectedProjectionRevision,
            nameof(expectedProjectionRevision));
        EnsureTick(projectedTick, nameof(projectedTick));

        if (sourceProjection.Revision != expectedProjectionRevision)
        {
            return WindowResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryLineageWindowStatus
                    .StaleProjectionRevision,
                sourceProjection);
        }
        if (projectedTick < sourceProjection.ProjectedTick)
        {
            return WindowResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryLineageWindowStatus
                    .WindowProjectionTickRegressed,
                sourceProjection);
        }
        if (checkpointCount > MaximumCheckpointCount)
        {
            return WindowResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryLineageWindowStatus
                    .TooManyWindowCheckpoints,
                sourceProjection);
        }
        if (startCheckpointIndex >= sourceProjection.CheckpointCount)
        {
            return WindowResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryLineageWindowStatus
                    .WindowStartOutOfRange,
                sourceProjection);
        }

        var endCheckpointIndex =
            (long)startCheckpointIndex + checkpointCount - 1L;
        if (endCheckpointIndex >= sourceProjection.CheckpointCount)
        {
            return WindowResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryLineageWindowStatus
                    .WindowEndOutOfRange,
                sourceProjection);
        }

        var chain = sourceProjection.Chain;
        var checkpoints =
            new HostRuntimeRecoveryCheckpoint<TRequest>[checkpointCount];
        for (var index = 0; index < checkpointCount; index++)
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
            supersessions[index] =
                chain.Supersessions[startCheckpointIndex + index];
        }

        var incomingSupersession =
            startCheckpointIndex > 0
                ? chain.Supersessions[startCheckpointIndex - 1]
                : null;
        var outgoingSupersession =
            endCheckpointIndex < chain.SupersessionCount
                ? chain.Supersessions[(int)endCheckpointIndex]
                : null;

        var window =
            new HostRuntimeRecoveryLineageWindowProjection<
                TRequest,
                TState,
                TCompletion>(
                    windowId,
                    sourceProjection,
                    checkpoints,
                    supersessions,
                    incomingSupersession,
                    outgoingSupersession,
                    startCheckpointIndex,
                    projectedTick,
                    checked(sourceProjection.Revision + 1));

        return new HostRuntimeRecoveryLineageWindowProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryLineageWindowStatus
                    .LineageWindowProjected,
                sourceProjection,
                window);
    }

    /// <summary>
    /// Resolves one exact inclusive checkpoint range inside a bounded lineage
    /// window.
    /// </summary>
    public static HostRuntimeRecoveryCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryRange<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointRangeQueryIdKind> queryId,
            HostRuntimeRecoveryLineageWindowProjection<
                TRequest,
                TState,
                TCompletion> window,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> startCheckpointId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> endCheckpointId,
            long expectedWindowRevision,
            long queriedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(queryId.IsEmpty, nameof(queryId));
        global::System.ArgumentNullException.ThrowIfNull(window);
        EnsureId(startCheckpointId.IsEmpty, nameof(startCheckpointId));
        EnsureId(endCheckpointId.IsEmpty, nameof(endCheckpointId));
        EnsureRevision(expectedWindowRevision, nameof(expectedWindowRevision));
        EnsureTick(queriedTick, nameof(queriedTick));

        if (window.Revision != expectedWindowRevision)
        {
            return RangeResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryLineageWindowStatus.StaleWindowRevision,
                window);
        }
        if (queriedTick < window.ProjectedTick)
        {
            return RangeResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryLineageWindowStatus
                    .RangeQueryTickRegressed,
                window);
        }

        var startWindowIndex =
            FindCheckpointIndex(window.Checkpoints, startCheckpointId);
        if (startWindowIndex < 0)
        {
            return RangeResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryLineageWindowStatus.RangeStartNotFound,
                window);
        }

        var endWindowIndex =
            FindCheckpointIndex(window.Checkpoints, endCheckpointId);
        if (endWindowIndex < 0)
        {
            return RangeResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryLineageWindowStatus.RangeEndNotFound,
                window);
        }
        if (endWindowIndex < startWindowIndex)
        {
            return RangeResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryLineageWindowStatus.RangeOrderInvalid,
                window);
        }

        var checkpointCount =
            checked(endWindowIndex - startWindowIndex + 1);
        var checkpoints =
            new HostRuntimeRecoveryCheckpoint<TRequest>[checkpointCount];
        for (var index = 0; index < checkpointCount; index++)
        {
            checkpoints[index] =
                window.Checkpoints[startWindowIndex + index];
        }

        var supersessions =
            new HostRuntimeRecoveryCheckpointSupersession<
                TRequest,
                TState,
                TCompletion>[checkpointCount - 1];
        for (var index = 0; index < supersessions.Length; index++)
        {
            supersessions[index] =
                window.Supersessions[startWindowIndex + index];
        }

        var incomingSupersession =
            startWindowIndex > 0
                ? window.Supersessions[startWindowIndex - 1]
                : window.IncomingSupersession;
        var outgoingSupersession =
            endWindowIndex < window.SupersessionCount
                ? window.Supersessions[endWindowIndex]
                : window.OutgoingSupersession;

        var query =
            new HostRuntimeRecoveryCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion>(
                    queryId,
                    window,
                    checkpoints,
                    supersessions,
                    incomingSupersession,
                    outgoingSupersession,
                    startWindowIndex,
                    endWindowIndex,
                    queriedTick,
                    checked(window.Revision + 1));

        return new HostRuntimeRecoveryCheckpointRangeQueryResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryLineageWindowStatus
                    .CheckpointRangeResolved,
                window,
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
            : chain.Supersessions[checkpointIndex - 1]
                .SuccessorCheckpoint;

    private static int FindCheckpointIndex<TRequest>(
        global::System.Collections.Generic.IReadOnlyList<
            HostRuntimeRecoveryCheckpoint<TRequest>> checkpoints,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryCheckpointIdKind> checkpointId)
        where TRequest : IHostRuntimeRequest
    {
        for (var index = 0; index < checkpoints.Count; index++)
        {
            if (checkpoints[index].CheckpointId == checkpointId)
            {
                return index;
            }
        }

        return -1;
    }

    private static HostRuntimeRecoveryLineageWindowProjectionResult<
        TRequest,
        TState,
        TCompletion> WindowResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryLineageWindowStatus status,
            HostRuntimeRecoveryChainSummaryProjection<
                TRequest,
                TState,
                TCompletion> sourceProjection)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, sourceProjection, window: null);

    private static HostRuntimeRecoveryCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> RangeResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryLineageWindowStatus status,
            HostRuntimeRecoveryLineageWindowProjection<
                TRequest,
                TState,
                TCompletion> window)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, window, query: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery lineage-window identifiers cannot be empty.",
                parameterName);
        }
    }

    private static void EnsureIndex(int index, string parameterName)
    {
        if (index < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                index,
                "Recovery lineage-window indexes cannot be negative.");
        }
    }

    private static void EnsureCount(int count, string parameterName)
    {
        if (count <= 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                count,
                "Recovery lineage-window counts must be positive.");
        }
    }

    private static void EnsureRevision(long revision, string parameterName)
    {
        if (revision < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                revision,
                "Recovery lineage-window revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery lineage-window ticks cannot be negative.");
        }
    }
}
