namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Defines explicit Host attempt-settlement outcomes.</summary>
public enum HostRuntimeAttemptSettlementStatus
{
    /// <summary>
    /// Completion and lease release produced terminal settlement authority.
    /// </summary>
    Settled = 0,

    /// <summary>The optimistic request revision did not match.</summary>
    StaleRequestRevision = 1,

    /// <summary>The optimistic lease revision did not match.</summary>
    StaleLeaseRevision = 2,

    /// <summary>
    /// Current request authority does not belong to the attempt.
    /// </summary>
    AttemptRequestMismatch = 3,

    /// <summary>
    /// Current lease authority does not belong to the attempt.
    /// </summary>
    AttemptLeaseMismatch = 4,

    /// <summary>The request state cannot accept terminal settlement.</summary>
    InvalidRequestState = 5,

    /// <summary>The lease is no longer actively owned.</summary>
    InvalidLeaseState = 6,

    /// <summary>The settling worker does not own the attempt lease.</summary>
    WorkerMismatch = 7,

    /// <summary>The supplied monotonic clock does not match.</summary>
    ClockMismatch = 8,

    /// <summary>The settlement tick precedes acknowledgement.</summary>
    BeforeAcknowledgement = 9,

    /// <summary>The lease expired before settlement.</summary>
    LeaseExpired = 10,

    /// <summary>The completion does not match attempt authority.</summary>
    CompletionMismatch = 11,

    /// <summary>
    /// Existing request transition contracts rejected terminal routing.
    /// </summary>
    RequestTransitionRejected = 12,

    /// <summary>
    /// Existing lease transition contracts rejected ownership release.
    /// </summary>
    LeaseTransitionRejected = 13,
}
