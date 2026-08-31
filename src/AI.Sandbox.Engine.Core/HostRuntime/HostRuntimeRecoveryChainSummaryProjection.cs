namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one compact immutable projection of a validated Host recovery
/// supersession chain while retaining the exact chain as evidence.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryChainSummaryProjection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryChainSummaryProjection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryChainSummaryProjectionIdKind> projectionId,
        HostRuntimeRecoverySupersessionChain<
            TRequest,
            TState,
            TCompletion> chain,
        long projectedTick,
        long revision)
    {
        ProjectionId = projectionId;
        Chain = chain;
        ProjectedTick = projectedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned chain-summary projection ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryChainSummaryProjectionIdKind> ProjectionId { get; }

    /// <summary>Gets unchanged validated supersession-chain authority.</summary>
    public HostRuntimeRecoverySupersessionChain<
        TRequest,
        TState,
        TCompletion> Chain { get; }

    /// <summary>Gets the external monotonic projection tick.</summary>
    public long ProjectedTick { get; }

    /// <summary>Gets the chain-summary projection authority revision.</summary>
    public long Revision { get; }

    /// <summary>Gets the source supersession-chain identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoverySupersessionChainIdKind> ChainId => Chain.ChainId;

    /// <summary>Gets the number of represented supersession edges.</summary>
    public int SupersessionCount => Chain.SupersessionCount;

    /// <summary>Gets the number of represented checkpoints.</summary>
    public int CheckpointCount => checked(Chain.SupersessionCount + 1);

    /// <summary>Gets the root checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> RootCheckpointId =>
        Chain.RootCheckpointId;

    /// <summary>Gets the latest checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> LatestCheckpointId =>
        Chain.LatestCheckpointId;

    /// <summary>Gets the represented runtime-instance identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeInstanceIdKind> RuntimeInstanceId =>
        Chain.RuntimeInstanceId;

    /// <summary>Gets the represented deterministic composition identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeCompositionIdKind> CompositionId => Chain.CompositionId;

    /// <summary>Gets the represented Host queue identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeQueueIdKind> QueueId => Chain.QueueId;

    /// <summary>Gets the represented monotonic Host clock identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId => Chain.ClockId;

    /// <summary>Gets the represented World identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId =>
        Chain.WorldId;

    /// <summary>Gets the root checkpoint capture tick.</summary>
    public long RootCheckpointCapturedTick =>
        Chain.RootCheckpoint.CapturedTick;

    /// <summary>Gets the latest checkpoint capture tick.</summary>
    public long LatestCheckpointCapturedTick =>
        Chain.LatestCheckpoint.CapturedTick;

    /// <summary>Gets the first checkpoint-supersession tick.</summary>
    public long FirstSupersededTick =>
        Chain.FirstSupersession.SupersededTick;

    /// <summary>Gets the latest checkpoint-supersession tick.</summary>
    public long LatestSupersededTick =>
        Chain.LatestSupersession.SupersededTick;

    /// <summary>Gets the root World State version.</summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
        RootWorldStateVersion =>
        Chain.RootCheckpoint.WorldSnapshotDocument.WorldStateVersion;

    /// <summary>Gets the latest World State version.</summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
        LatestWorldStateVersion =>
        Chain.LatestCheckpoint.WorldSnapshotDocument.WorldStateVersion;

    /// <summary>Gets the root logical simulation tick.</summary>
    public ulong RootSimulationTick =>
        Chain.RootCheckpoint.WorldSnapshotDocument.SimulationTick;

    /// <summary>Gets the latest logical simulation tick.</summary>
    public ulong LatestSimulationTick =>
        Chain.LatestCheckpoint.WorldSnapshotDocument.SimulationTick;
}
