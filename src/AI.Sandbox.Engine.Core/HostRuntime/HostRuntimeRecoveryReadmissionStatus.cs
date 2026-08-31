namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit Host recovery re-admission and lease-reacquisition outcomes.
/// </summary>
public enum HostRuntimeRecoveryReadmissionStatus
{
    /// <summary>The selected recovery work received new queue admission authority.</summary>
    Readmitted = 0,

    /// <summary>The re-admitted recovery work received a new active lease.</summary>
    LeaseReacquired = 1,

    /// <summary>The optimistic resumed-work selection revision did not match.</summary>
    StaleSelectionRevision = 2,

    /// <summary>The re-admission tick precedes resumed-work selection.</summary>
    ReadmissionTickRegressed = 3,

    /// <summary>The current, checkpoint, and prior admission queues do not match.</summary>
    QueueMismatch = 4,

    /// <summary>The prior admission ID was reused instead of creating re-admission.</summary>
    PriorAdmissionIdReused = 5,

    /// <summary>The optimistic queue revision did not match.</summary>
    StaleQueueRevision = 6,

    /// <summary>The represented recovery queue has no admission capacity.</summary>
    QueueFull = 7,

    /// <summary>Existing queue-admission contracts rejected recovery re-admission.</summary>
    AdmissionRejected = 8,

    /// <summary>The optimistic recovery re-admission revision did not match.</summary>
    StaleReadmissionRevision = 9,

    /// <summary>The lease-reacquisition tick precedes recovery re-admission.</summary>
    ReacquisitionTickRegressed = 10,

    /// <summary>The supplied monotonic clock does not match recovery authority.</summary>
    ClockMismatch = 11,

    /// <summary>The prior lease ID was reused instead of creating a new lease.</summary>
    PriorLeaseIdReused = 12,
}
