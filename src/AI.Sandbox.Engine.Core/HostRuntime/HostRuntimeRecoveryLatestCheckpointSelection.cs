namespace AI.Sandbox.Engine.Core.HostRuntime;
/// <summary>
/// Contains immutable advisory authority selecting the exact latest checkpoint
/// from one validated recovery supersession chain.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryLatestCheckpointSelection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryLatestCheckpointSelection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryLatestCheckpointSelectionIdKind> selectionId,
        HostRuntimeRecoverySupersessionChain<
            TRequest,
            TState,
            TCompletion> chain,
        long selectedTick,
        long revision)
    {
        SelectionId = selectionId;
        Chain = chain;
        SelectedTick = selectedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned latest-checkpoint selection ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryLatestCheckpointSelectionIdKind> SelectionId { get; }

    /// <summary>Gets unchanged validated supersession-chain authority.</summary>
    public HostRuntimeRecoverySupersessionChain<
        TRequest,
        TState,
        TCompletion> Chain { get; }

    /// <summary>Gets the exact latest checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> Checkpoint =>
        Chain.LatestCheckpoint;

    /// <summary>Gets the external monotonic selection tick.</summary>
    public long SelectedTick { get; }

    /// <summary>Gets the latest-checkpoint selection authority revision.</summary>
    public long Revision { get; }

    /// <summary>Gets the selected checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> CheckpointId =>
        Checkpoint.CheckpointId;

    /// <summary>Gets the source supersession-chain identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoverySupersessionChainIdKind> ChainId =>
        Chain.ChainId;
}
