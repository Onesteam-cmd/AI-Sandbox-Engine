namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery dispatch-reconstruction result.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
public sealed record HostRuntimeRecoveryDispatchReconstructionResult<TRequest, TState>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
{
    internal HostRuntimeRecoveryDispatchReconstructionResult(
        HostRuntimeRecoveryDispatchStatus status,
        HostRuntimeRecoveryLeaseReacquisition<TRequest, TState> reacquisition,
        HostRuntimeQueueSnapshot snapshot,
        HostRuntimeDispatchSelectionStatus? selectionStatus,
        HostRuntimeRecoveryDispatchReconstruction<TRequest, TState>?
            reconstruction)
    {
        Status = status;
        Reacquisition = reacquisition;
        Snapshot = snapshot;
        SelectionStatus = selectionStatus;
        Reconstruction = reconstruction;
    }

    /// <summary>Gets the explicit reconstruction outcome.</summary>
    public HostRuntimeRecoveryDispatchStatus Status { get; }

    /// <summary>Gets unchanged lease-reacquisition authority.</summary>
    public HostRuntimeRecoveryLeaseReacquisition<TRequest, TState>
        Reacquisition { get; }

    /// <summary>Gets resulting or unchanged recovery queue authority.</summary>
    public HostRuntimeQueueSnapshot Snapshot { get; }

    /// <summary>Gets the underlying dispatch-selection outcome when invoked.</summary>
    public HostRuntimeDispatchSelectionStatus? SelectionStatus { get; }

    /// <summary>Gets reconstruction authority when successful.</summary>
    public HostRuntimeRecoveryDispatchReconstruction<TRequest, TState>?
        Reconstruction { get; }

    /// <summary>Gets whether recovery dispatch authority was reconstructed.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryDispatchStatus.DispatchReconstructed;
}
