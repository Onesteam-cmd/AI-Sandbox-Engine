namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery checkpoint capture result.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
public sealed record HostRuntimeRecoveryCheckpointResult<TRequest>
    where TRequest : IHostRuntimeRequest
{
    internal HostRuntimeRecoveryCheckpointResult(
        HostRuntimeRecoveryStatus status,
        HostRuntimeRecoveryCheckpoint<TRequest>? checkpoint)
    {
        Status = status;
        Checkpoint = checkpoint;
    }

    /// <summary>Gets the explicit checkpoint capture outcome.</summary>
    public HostRuntimeRecoveryStatus Status { get; }

    /// <summary>Gets checkpoint authority when capture succeeded.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest>? Checkpoint { get; }

    /// <summary>Gets whether checkpoint authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryStatus.CheckpointCreated;
}
