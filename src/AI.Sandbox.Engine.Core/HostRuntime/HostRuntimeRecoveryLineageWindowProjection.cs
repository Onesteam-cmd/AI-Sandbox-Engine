namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable bounded contiguous lineage-window projection over an
/// exact Host recovery chain-summary projection.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryLineageWindowProjection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryLineageWindowProjection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryLineageWindowProjectionIdKind> windowId,
        HostRuntimeRecoveryChainSummaryProjection<
            TRequest,
            TState,
            TCompletion> sourceProjection,
        HostRuntimeRecoveryCheckpoint<TRequest>[] checkpoints,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>[] supersessions,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>? incomingSupersession,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>? outgoingSupersession,
        int startCheckpointIndex,
        long projectedTick,
        long revision)
    {
        WindowId = windowId;
        SourceProjection = sourceProjection;
        Checkpoints = global::System.Array.AsReadOnly(checkpoints);
        Supersessions = global::System.Array.AsReadOnly(supersessions);
        IncomingSupersession = incomingSupersession;
        OutgoingSupersession = outgoingSupersession;
        StartCheckpointIndex = startCheckpointIndex;
        ProjectedTick = projectedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned lineage-window projection ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryLineageWindowProjectionIdKind> WindowId { get; }

    /// <summary>Gets unchanged source chain-summary projection authority.</summary>
    public HostRuntimeRecoveryChainSummaryProjection<
        TRequest,
        TState,
        TCompletion> SourceProjection { get; }

    /// <summary>Gets exact contiguous checkpoint authorities in the window.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpoint<TRequest>> Checkpoints { get; }

    /// <summary>Gets exact supersession authorities inside the window.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> Supersessions { get; }

    /// <summary>Gets the supersession entering the window, when present.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? IncomingSupersession { get; }

    /// <summary>Gets the supersession leaving the window, when present.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? OutgoingSupersession { get; }

    /// <summary>Gets the first checkpoint index in the source chain.</summary>
    public int StartCheckpointIndex { get; }

    /// <summary>Gets the last checkpoint index in the source chain.</summary>
    public int EndCheckpointIndex =>
        checked(StartCheckpointIndex + CheckpointCount - 1);

    /// <summary>Gets the number of represented checkpoints.</summary>
    public int CheckpointCount => Checkpoints.Count;

    /// <summary>Gets the number of represented supersession edges.</summary>
    public int SupersessionCount => Supersessions.Count;

    /// <summary>Gets the first represented checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint =>
        Checkpoints[0];

    /// <summary>Gets the last represented checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint =>
        Checkpoints[^1];

    /// <summary>Gets the first represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> StartCheckpointId =>
        StartCheckpoint.CheckpointId;

    /// <summary>Gets the last represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> EndCheckpointId =>
        EndCheckpoint.CheckpointId;

    /// <summary>Gets the external monotonic window-projection tick.</summary>
    public long ProjectedTick { get; }

    /// <summary>Gets the lineage-window projection authority revision.</summary>
    public long Revision { get; }

    /// <summary>Gets whether the window starts at the chain root.</summary>
    public bool StartsAtRoot => StartCheckpointIndex == 0;

    /// <summary>Gets whether the window ends at the latest checkpoint.</summary>
    public bool EndsAtLatest =>
        EndCheckpointIndex == SourceProjection.SupersessionCount;
}
