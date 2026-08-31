namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit Host dispatch acknowledgement outcomes.
/// </summary>
public enum HostRuntimeDispatchAcknowledgementStatus
{
    /// <summary>
    /// The dispatch was acknowledged and became an in-flight attempt.
    /// </summary>
    Acknowledged = 0,

    /// <summary>The optimistic request revision did not match.</summary>
    StaleRequestRevision = 1,

    /// <summary>The optimistic lease revision did not match.</summary>
    StaleLeaseRevision = 2,

    /// <summary>
    /// Current request authority did not match the selected request ID.
    /// </summary>
    SelectionRequestMismatch = 3,

    /// <summary>
    /// Current lease authority did not match the selected lease ID.
    /// </summary>
    SelectionLeaseMismatch = 4,

    /// <summary>
    /// Current request authority is no longer pending.
    /// </summary>
    InvalidRequestState = 5,

    /// <summary>The current lease is not active.</summary>
    InvalidLeaseState = 6,

    /// <summary>The acknowledged lease ID did not match.</summary>
    LeaseMismatch = 7,

    /// <summary>The acknowledging worker ID did not match.</summary>
    WorkerMismatch = 8,

    /// <summary>The acknowledged dispatch ID did not match.</summary>
    DispatchMismatch = 9,

    /// <summary>The acknowledged request ID did not match.</summary>
    RequestMismatch = 10,

    /// <summary>The acknowledged attempt number did not match.</summary>
    AttemptNumberMismatch = 11,

    /// <summary>The supplied monotonic clock did not match.</summary>
    ClockMismatch = 12,

    /// <summary>The acknowledgement preceded lease acquisition.</summary>
    BeforeLeaseAcquisition = 13,

    /// <summary>
    /// External monotonic time reached the lease expiry boundary.
    /// </summary>
    LeaseExpired = 14,
}
