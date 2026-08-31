namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Defines immutable Host work-lease ownership states.</summary>
public enum HostRuntimeLeaseState
{
    /// <summary>The named worker currently owns the admitted work.</summary>
    Active = 0,

    /// <summary>The named worker released ownership explicitly.</summary>
    Released = 1,

    /// <summary>External monotonic time proved the lease expired.</summary>
    Expired = 2,
}
