namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one explicit lifecycle-transition result.</summary>
public sealed record HostRuntimeLifecycleTransitionResult
{
    internal HostRuntimeLifecycleTransitionResult(
        HostRuntimeLifecycleTransitionStatus status,
        HostRuntimeLifecycleSnapshot snapshot)
    {
        Status = status;
        Snapshot = snapshot;
    }

    /// <summary>Gets the explicit transition outcome.</summary>
    public HostRuntimeLifecycleTransitionStatus Status { get; }

    /// <summary>Gets the resulting or unchanged immutable snapshot.</summary>
    public HostRuntimeLifecycleSnapshot Snapshot { get; }

    /// <summary>Gets whether the transition produced a new snapshot.</summary>
    public bool Succeeded =>
        Status == HostRuntimeLifecycleTransitionStatus.Applied;
}
