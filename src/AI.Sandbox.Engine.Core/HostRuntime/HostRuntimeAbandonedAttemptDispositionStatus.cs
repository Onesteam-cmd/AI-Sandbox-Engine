namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit Host abandoned-attempt disposition outcomes.
/// </summary>
public enum HostRuntimeAbandonedAttemptDispositionStatus
{
    /// <summary>
    /// Cancellation or lease expiry produced immutable disposition authority.
    /// </summary>
    Disposed = 0,

    /// <summary>The optimistic request revision did not match.</summary>
    StaleRequestRevision = 1,

    /// <summary>The optimistic lease revision did not match.</summary>
    StaleLeaseRevision = 2,

    /// <summary>Current request authority does not belong to the attempt.</summary>
    AttemptRequestMismatch = 3,

    /// <summary>Current lease authority does not belong to the attempt.</summary>
    AttemptLeaseMismatch = 4,

    /// <summary>Current lease worker does not match attempt authority.</summary>
    AttemptWorkerMismatch = 5,

    /// <summary>The request state cannot accept abandonment disposition.</summary>
    InvalidRequestState = 6,

    /// <summary>The lease is no longer actively owned.</summary>
    InvalidLeaseState = 7,

    /// <summary>The supplied monotonic clock does not match.</summary>
    ClockMismatch = 8,

    /// <summary>The disposition tick precedes acknowledgement.</summary>
    BeforeAcknowledgement = 9,

    /// <summary>Cancellation disposition was requested without cancellation intent.</summary>
    CancellationNotRequested = 10,

    /// <summary>Cancellation disposition arrived at or after lease expiry.</summary>
    LeaseAlreadyExpired = 11,

    /// <summary>Lease-expiry disposition arrived before the expiry boundary.</summary>
    LeaseNotExpired = 12,

    /// <summary>Existing request transition contracts rejected disposition.</summary>
    RequestTransitionRejected = 13,

    /// <summary>Existing lease transition contracts rejected disposition.</summary>
    LeaseTransitionRejected = 14,
}
