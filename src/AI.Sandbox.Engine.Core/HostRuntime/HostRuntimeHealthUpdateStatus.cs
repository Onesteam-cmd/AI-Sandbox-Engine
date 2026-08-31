namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Defines explicit host-health update outcomes.</summary>
public enum HostRuntimeHealthUpdateStatus
{
    /// <summary>The observation produced a new immutable snapshot.</summary>
    Applied = 0,

    /// <summary>The optimistic expected revision did not match.</summary>
    StaleRevision = 1,

    /// <summary>The lifecycle state does not accept health observations.</summary>
    InvalidLifecycleState = 2,
}
