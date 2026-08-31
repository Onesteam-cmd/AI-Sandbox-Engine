namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable bounded checkpoint-lineage query result over an exact
/// Host recovery chain-summary projection.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryCheckpointLineageQuery<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCheckpointLineageQuery(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryCheckpointLineageQueryIdKind> queryId,
        HostRuntimeRecoveryChainSummaryProjection<
            TRequest,
            TState,
            TCompletion> projection,
        HostRuntimeRecoveryCheckpoint<TRequest> checkpoint,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>? incomingSupersession,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>? outgoingSupersession,
        int checkpointIndex,
        long queriedTick,
        long revision)
    {
        QueryId = queryId;
        Projection = projection;
        Checkpoint = checkpoint;
        IncomingSupersession = incomingSupersession;
        OutgoingSupersession = outgoingSupersession;
        CheckpointIndex = checkpointIndex;
        QueriedTick = queriedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned checkpoint-lineage query ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointLineageQueryIdKind> QueryId { get; }

    /// <summary>Gets unchanged chain-summary projection authority.</summary>
    public HostRuntimeRecoveryChainSummaryProjection<
        TRequest,
        TState,
        TCompletion> Projection { get; }

    /// <summary>Gets the exact checkpoint authority resolved by the query.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> Checkpoint { get; }

    /// <summary>Gets the supersession entering the checkpoint, when present.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? IncomingSupersession { get; }

    /// <summary>Gets the supersession leaving the checkpoint, when present.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? OutgoingSupersession { get; }

    /// <summary>Gets the checkpoint index from the root of the validated chain.</summary>
    public int CheckpointIndex { get; }

    /// <summary>Gets the external monotonic query tick.</summary>
    public long QueriedTick { get; }

    /// <summary>Gets the checkpoint-lineage query authority revision.</summary>
    public long Revision { get; }

    /// <summary>Gets the requested checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> CheckpointId =>
        Checkpoint.CheckpointId;

    /// <summary>Gets the source chain-summary projection identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryChainSummaryProjectionIdKind> ProjectionId =>
        Projection.ProjectionId;

    /// <summary>Gets whether the resolved checkpoint is the chain root.</summary>
    public bool IsRootCheckpoint => CheckpointIndex == 0;

    /// <summary>Gets whether the resolved checkpoint is the latest checkpoint.</summary>
    public bool IsLatestCheckpoint =>
        CheckpointIndex == Projection.SupersessionCount;
}
