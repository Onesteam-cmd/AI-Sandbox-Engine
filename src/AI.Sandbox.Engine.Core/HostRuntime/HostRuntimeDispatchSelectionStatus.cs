namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit Host dequeue-and-dispatch-selection outcomes.
/// </summary>
public enum HostRuntimeDispatchSelectionStatus
{
    /// <summary>
    /// The selected active lease was dequeued and received dispatch
    /// authority.
    /// </summary>
    Selected = 0,

    /// <summary>
    /// The optimistic expected queue revision did not match.
    /// </summary>
    StaleQueueRevision = 1,

    /// <summary>The represented queue contains no queued work.</summary>
    EmptyQueue = 2,

    /// <summary>
    /// The selected lease belongs to a different queue.
    /// </summary>
    QueueMismatch = 3,

    /// <summary>
    /// The selected lease is not currently active.
    /// </summary>
    InvalidLeaseState = 4,

    /// <summary>
    /// The supplied monotonic clock does not match the lease clock.
    /// </summary>
    ClockMismatch = 5,

    /// <summary>
    /// External monotonic time reached the lease expiry boundary.
    /// </summary>
    LeaseExpired = 6,
}
