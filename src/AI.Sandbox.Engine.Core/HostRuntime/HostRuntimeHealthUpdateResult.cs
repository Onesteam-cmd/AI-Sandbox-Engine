namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one explicit host-health update result.</summary>
public sealed record HostRuntimeHealthUpdateResult
{
    internal HostRuntimeHealthUpdateResult(
        HostRuntimeHealthUpdateStatus status,
        HostRuntimeLifecycleSnapshot snapshot)
    {
        Status = status;
        Snapshot = snapshot;
    }

    /// <summary>Gets the explicit health-update outcome.</summary>
    public HostRuntimeHealthUpdateStatus Status { get; }

    /// <summary>Gets the resulting or unchanged immutable snapshot.</summary>
    public HostRuntimeLifecycleSnapshot Snapshot { get; }

    /// <summary>Gets whether the observation produced a new snapshot.</summary>
    public bool Succeeded =>
        Status == HostRuntimeHealthUpdateStatus.Applied;
}
