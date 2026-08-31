namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host resumed-attempt acknowledgement result.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
public sealed record HostRuntimeRecoveryResumedAttemptAcknowledgementResult<
    TRequest,
    TState>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
{
    internal HostRuntimeRecoveryResumedAttemptAcknowledgementResult(
        HostRuntimeRecoveryDispatchStatus status,
        HostRuntimeRecoveryDispatchReconstruction<TRequest, TState>
            reconstruction,
        HostRuntimeDispatchAcknowledgementStatus? acknowledgementStatus,
        HostRuntimeRecoveryResumedAttemptAcknowledgement<TRequest, TState>?
            acknowledgement)
    {
        Status = status;
        Reconstruction = reconstruction;
        AcknowledgementStatus = acknowledgementStatus;
        Acknowledgement = acknowledgement;
    }

    /// <summary>Gets the explicit resumed-attempt outcome.</summary>
    public HostRuntimeRecoveryDispatchStatus Status { get; }

    /// <summary>Gets unchanged recovery dispatch reconstruction authority.</summary>
    public HostRuntimeRecoveryDispatchReconstruction<TRequest, TState>
        Reconstruction { get; }

    /// <summary>Gets the underlying dispatch-acknowledgement outcome when invoked.</summary>
    public HostRuntimeDispatchAcknowledgementStatus? AcknowledgementStatus { get; }

    /// <summary>Gets resumed-attempt acknowledgement authority when successful.</summary>
    public HostRuntimeRecoveryResumedAttemptAcknowledgement<TRequest, TState>?
        Acknowledgement { get; }

    /// <summary>Gets whether a new resumed in-flight attempt was acknowledged.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryDispatchStatus.AttemptAcknowledged;
}
