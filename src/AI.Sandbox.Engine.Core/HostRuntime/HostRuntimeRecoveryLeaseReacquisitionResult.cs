namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable Host recovery lease-reacquisition result.</summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
public sealed record HostRuntimeRecoveryLeaseReacquisitionResult<TRequest, TState>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
{
    internal HostRuntimeRecoveryLeaseReacquisitionResult(
        HostRuntimeRecoveryReadmissionStatus status,
        HostRuntimeRecoveryReadmission<TRequest, TState> readmission,
        HostRuntimeRecoveryLeaseReacquisition<TRequest, TState>? reacquisition)
    {
        Status = status;
        Readmission = readmission;
        Reacquisition = reacquisition;
    }

    /// <summary>Gets the explicit lease-reacquisition outcome.</summary>
    public HostRuntimeRecoveryReadmissionStatus Status { get; }

    /// <summary>Gets unchanged recovery re-admission authority.</summary>
    public HostRuntimeRecoveryReadmission<TRequest, TState> Readmission { get; }

    /// <summary>Gets lease-reacquisition authority when successful.</summary>
    public HostRuntimeRecoveryLeaseReacquisition<TRequest, TState>?
        Reacquisition { get; }

    /// <summary>Gets whether new active lease authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryReadmissionStatus.LeaseReacquired;
}
