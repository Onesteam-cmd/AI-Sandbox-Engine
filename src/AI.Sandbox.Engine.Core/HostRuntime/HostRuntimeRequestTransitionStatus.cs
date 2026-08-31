namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Defines explicit Host request transition outcomes.</summary>
public enum HostRuntimeRequestTransitionStatus
{
    /// <summary>The requested transition produced a new immutable envelope.</summary>
    Applied = 0,

    /// <summary>The optimistic expected revision did not match.</summary>
    StaleRevision = 1,

    /// <summary>The requested state edge is not allowed.</summary>
    InvalidState = 2,
}
