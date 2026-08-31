namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Defines explicit lifecycle-transition outcomes.</summary>
public enum HostRuntimeLifecycleTransitionStatus
{
    /// <summary>The requested transition produced a new immutable snapshot.</summary>
    Applied = 0,

    /// <summary>The optimistic expected revision did not match.</summary>
    StaleRevision = 1,

    /// <summary>The requested direct lifecycle edge is not allowed.</summary>
    InvalidTransition = 2,
}
