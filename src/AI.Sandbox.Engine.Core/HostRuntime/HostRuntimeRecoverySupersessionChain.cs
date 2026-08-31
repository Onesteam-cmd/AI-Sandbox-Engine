namespace AI.Sandbox.Engine.Core.HostRuntime;
/// <summary>
/// Contains one bounded immutable ordered Host recovery checkpoint-supersession
/// chain validated from caller-supplied authority.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoverySupersessionChain<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoverySupersessionChain(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoverySupersessionChainIdKind> chainId,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>[] supersessions,
        long validatedTick,
        long revision)
    {
        ChainId = chainId;
        Supersessions = global::System.Array.AsReadOnly(supersessions);
        ValidatedTick = validatedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned supersession-chain ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoverySupersessionChainIdKind> ChainId { get; }

    /// <summary>Gets the exact ordered supersession authorities.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> Supersessions { get; }

    /// <summary>Gets the external monotonic validation tick.</summary>
    public long ValidatedTick { get; }

    /// <summary>Gets the supersession-chain authority revision.</summary>
    public long Revision { get; }

    /// <summary>Gets the number of validated supersession edges.</summary>
    public int SupersessionCount => Supersessions.Count;

    /// <summary>Gets the first supersession edge.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion> FirstSupersession => Supersessions[0];

    /// <summary>Gets the latest supersession edge.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion> LatestSupersession => Supersessions[^1];

    /// <summary>Gets the root checkpoint retained as immutable evidence.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> RootCheckpoint =>
        FirstSupersession.PriorCheckpoint;

    /// <summary>Gets the latest validated checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> LatestCheckpoint =>
        LatestSupersession.SuccessorCheckpoint;

    /// <summary>Gets the root checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> RootCheckpointId =>
        RootCheckpoint.CheckpointId;

    /// <summary>Gets the latest checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> LatestCheckpointId =>
        LatestCheckpoint.CheckpointId;

    /// <summary>Gets the represented runtime-instance identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeInstanceIdKind> RuntimeInstanceId =>
        LatestSupersession.RuntimeInstanceId;

    /// <summary>Gets the represented deterministic composition identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeCompositionIdKind> CompositionId =>
        LatestSupersession.CompositionId;

    /// <summary>Gets the represented Host queue identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeQueueIdKind> QueueId => LatestSupersession.QueueId;

    /// <summary>Gets the represented monotonic Host clock identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId => LatestSupersession.ClockId;

    /// <summary>Gets the represented World identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId =>
        LatestSupersession.WorldId;
}
