namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Defines provider-neutral host-runtime health classifications.</summary>
public enum HostRuntimeHealthStatus
{
    /// <summary>No health observation has been recorded.</summary>
    Unknown = 0,

    /// <summary>The observed runtime is operating within its declared contract.</summary>
    Healthy = 1,

    /// <summary>The observed runtime remains usable with a declared impairment.</summary>
    Degraded = 2,

    /// <summary>The observed runtime is not usable within its declared contract.</summary>
    Unhealthy = 3,
}
