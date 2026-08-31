namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable inclusive bounded checkpoint-range query over an exact
/// Host recovery lineage-window projection.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryCheckpointRangeQuery<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCheckpointRangeQuery(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryCheckpointRangeQueryIdKind> queryId,
        HostRuntimeRecoveryLineageWindowProjection<
            TRequest,
            TState,
            TCompletion> window,
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
        int startWindowIndex,
        int endWindowIndex,
        long queriedTick,
        long revision)
    {
        QueryId = queryId;
        Window = window;
        Checkpoints = global::System.Array.AsReadOnly(checkpoints);
        Supersessions = global::System.Array.AsReadOnly(supersessions);
        IncomingSupersession = incomingSupersession;
        OutgoingSupersession = outgoingSupersession;
        StartWindowIndex = startWindowIndex;
        EndWindowIndex = endWindowIndex;
        QueriedTick = queriedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned checkpoint-range query ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointRangeQueryIdKind> QueryId { get; }

    /// <summary>Gets unchanged source lineage-window projection authority.</summary>
    public HostRuntimeRecoveryLineageWindowProjection<
        TRequest,
        TState,
        TCompletion> Window { get; }

    /// <summary>Gets exact checkpoint authorities in the inclusive range.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpoint<TRequest>> Checkpoints { get; }

    /// <summary>Gets exact supersession authorities inside the range.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> Supersessions { get; }

    /// <summary>Gets the supersession entering the range, when present.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? IncomingSupersession { get; }

    /// <summary>Gets the supersession leaving the range, when present.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? OutgoingSupersession { get; }

    /// <summary>Gets the first checkpoint index in the source window.</summary>
    public int StartWindowIndex { get; }

    /// <summary>Gets the last checkpoint index in the source window.</summary>
    public int EndWindowIndex { get; }

    /// <summary>Gets the first checkpoint index in the source chain.</summary>
    public int StartChainIndex =>
        checked(Window.StartCheckpointIndex + StartWindowIndex);

    /// <summary>Gets the last checkpoint index in the source chain.</summary>
    public int EndChainIndex =>
        checked(Window.StartCheckpointIndex + EndWindowIndex);

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

    /// <summary>Gets the external monotonic range-query tick.</summary>
    public long QueriedTick { get; }

    /// <summary>Gets the checkpoint-range query authority revision.</summary>
    public long Revision { get; }
}
