namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable materialized Host recovery window projected from an
/// exact adjacent-window selection.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryAdjacentWindowProjection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentWindowProjection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryAdjacentWindowProjectionIdKind> projectionId,
        HostRuntimeRecoveryAdjacentWindowSelection<
            TRequest,
            TState,
            TCompletion> selection,
        HostRuntimeRecoveryCheckpoint<TRequest>[] checkpoints,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>[] supersessions,
        long projectedTick,
        long revision)
    {
        ProjectionId = projectionId;
        Selection = selection;
        Checkpoints = global::System.Array.AsReadOnly(checkpoints);
        Supersessions = global::System.Array.AsReadOnly(supersessions);
        ProjectedTick = projectedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned adjacent-window projection ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryAdjacentWindowProjectionIdKind> ProjectionId { get; }

    /// <summary>Gets unchanged adjacent-window selection authority.</summary>
    public HostRuntimeRecoveryAdjacentWindowSelection<
        TRequest,
        TState,
        TCompletion> Selection { get; }

    /// <summary>Gets unchanged checkpoint-range summary authority.</summary>
    public HostRuntimeRecoveryCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary => Selection.Summary;

    /// <summary>Gets unchanged source chain-summary projection authority.</summary>
    public HostRuntimeRecoveryChainSummaryProjection<
        TRequest,
        TState,
        TCompletion> SourceProjection => Summary.SourceProjection;

    /// <summary>Gets unchanged validated supersession-chain authority.</summary>
    public HostRuntimeRecoverySupersessionChain<
        TRequest,
        TState,
        TCompletion> Chain => SourceProjection.Chain;

    /// <summary>Gets whether this is the selected previous window.</summary>
    public bool SelectsPreviousWindow => Selection.SelectsPreviousWindow;

    /// <summary>Gets whether this is the selected next window.</summary>
    public bool SelectsNextWindow => Selection.SelectsNextWindow;

    /// <summary>Gets exact projected checkpoint authorities.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpoint<TRequest>> Checkpoints { get; }

    /// <summary>Gets exact supersession authorities inside the projected window.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> Supersessions { get; }

    /// <summary>Gets the first projected checkpoint index in the source chain.</summary>
    public int StartCheckpointIndex => Selection.StartCheckpointIndex;

    /// <summary>Gets the last projected checkpoint index in the source chain.</summary>
    public int EndCheckpointIndex => Selection.EndCheckpointIndex;

    /// <summary>Gets the number of projected checkpoints.</summary>
    public int CheckpointCount => Checkpoints.Count;

    /// <summary>Gets the number of internal projected supersessions.</summary>
    public int SupersessionCount => Supersessions.Count;

    /// <summary>Gets the exact first projected checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint => Checkpoints[0];

    /// <summary>Gets the exact last projected checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint => Checkpoints[^1];

    /// <summary>Gets the exact supersession entering the projected window.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? IncomingSupersession => Selection.IncomingSupersession;

    /// <summary>Gets the exact supersession leaving the projected window.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? OutgoingSupersession => Selection.OutgoingSupersession;

    /// <summary>Gets the first projected checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> StartCheckpointId =>
        StartCheckpoint.CheckpointId;

    /// <summary>Gets the last projected checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> EndCheckpointId =>
        EndCheckpoint.CheckpointId;

    /// <summary>Gets whether the projected window starts at the chain root.</summary>
    public bool StartsAtRoot => StartCheckpointIndex == 0;

    /// <summary>Gets whether the projected window ends at the latest checkpoint.</summary>
    public bool EndsAtLatest =>
        EndCheckpointIndex == SourceProjection.SupersessionCount;

    /// <summary>Gets the external monotonic projection tick.</summary>
    public long ProjectedTick { get; }

    /// <summary>Gets the adjacent-window projection authority revision.</summary>
    public long Revision { get; }
}
