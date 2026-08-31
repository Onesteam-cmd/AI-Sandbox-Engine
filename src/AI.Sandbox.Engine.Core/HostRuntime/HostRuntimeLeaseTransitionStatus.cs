namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Defines explicit Host work-lease transition outcomes.</summary>
public enum HostRuntimeLeaseTransitionStatus
{
    /// <summary>An active lease received a new exclusive expiry tick.</summary>
    Renewed = 0,

    /// <summary>The exact owner released the active lease.</summary>
    Released = 1,

    /// <summary>The active lease was proven expired.</summary>
    Expired = 2,

    /// <summary>The optimistic lease revision did not match.</summary>
    StaleRevision = 3,

    /// <summary>The lease state cannot accept the requested transition.</summary>
    InvalidState = 4,

    /// <summary>The supplied worker is not the lease owner.</summary>
    WorkerMismatch = 5,

    /// <summary>The supplied monotonic clock does not match the lease clock.</summary>
    ClockMismatch = 6,

    /// <summary>The observed tick has not reached the exclusive expiry.</summary>
    NotExpired = 7,
}
