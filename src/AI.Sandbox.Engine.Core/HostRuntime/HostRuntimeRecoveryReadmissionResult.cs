namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable Host recovery re-admission result.</summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
public sealed record HostRuntimeRecoveryReadmissionResult<TRequest, TState>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
{
    internal HostRuntimeRecoveryReadmissionResult(
        HostRuntimeRecoveryReadmissionStatus status,
        HostRuntimeResumedWorkSelection<TRequest, TState> selection,
        HostRuntimeQueueSnapshot snapshot,
        HostRuntimeRecoveryReadmission<TRequest, TState>? readmission)
    {
        Status = status;
        Selection = selection;
        Snapshot = snapshot;
        Readmission = readmission;
    }

    /// <summary>Gets the explicit recovery re-admission outcome.</summary>
    public HostRuntimeRecoveryReadmissionStatus Status { get; }

    /// <summary>Gets unchanged resumed-work selection authority.</summary>
    public HostRuntimeResumedWorkSelection<TRequest, TState> Selection { get; }

    /// <summary>Gets resulting or unchanged recovery queue authority.</summary>
    public HostRuntimeQueueSnapshot Snapshot { get; }

    /// <summary>Gets recovery re-admission authority when successful.</summary>
    public HostRuntimeRecoveryReadmission<TRequest, TState>? Readmission { get; }

    /// <summary>Gets whether recovery re-admission authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryReadmissionStatus.Readmitted;
}
