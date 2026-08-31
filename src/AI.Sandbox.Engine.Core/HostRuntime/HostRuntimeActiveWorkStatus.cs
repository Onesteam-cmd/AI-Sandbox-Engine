namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit Host active-work capture and reconciliation outcomes.
/// </summary>
public enum HostRuntimeActiveWorkStatus
{
    /// <summary>A bounded deterministic active-work snapshot was captured.</summary>
    Captured = 0,

    /// <summary>Sequential active-work snapshots were reconciled.</summary>
    Reconciled = 1,

    /// <summary>The supplied active-work collection exceeded its bound.</summary>
    TooManyItems = 2,

    /// <summary>The supplied collection repeated an attempt ID.</summary>
    DuplicateAttempt = 3,

    /// <summary>An item or snapshot belonged to another runtime instance.</summary>
    RuntimeMismatch = 4,

    /// <summary>An item or snapshot used another monotonic clock domain.</summary>
    ClockMismatch = 5,

    /// <summary>Current request authority did not belong to its attempt.</summary>
    AttemptRequestMismatch = 6,

    /// <summary>Current lease authority did not belong to its attempt.</summary>
    AttemptLeaseMismatch = 7,

    /// <summary>Current lease worker did not match attempt authority.</summary>
    AttemptWorkerMismatch = 8,

    /// <summary>Current request revision preceded acknowledged authority.</summary>
    RequestRevisionRegressed = 9,

    /// <summary>Current lease revision preceded acknowledged authority.</summary>
    LeaseRevisionRegressed = 10,

    /// <summary>Current request state was not active work.</summary>
    InvalidRequestState = 11,

    /// <summary>Current lease state was not active ownership.</summary>
    InvalidLeaseState = 12,

    /// <summary>The snapshot tick preceded attempt acknowledgement.</summary>
    BeforeAcknowledgement = 13,

    /// <summary>The snapshot tick reached an active lease expiry boundary.</summary>
    LeaseExpired = 14,

    /// <summary>The optimistic previous snapshot revision did not match.</summary>
    StaleSnapshotRevision = 15,

    /// <summary>The current snapshot revision was not the next revision.</summary>
    NonSequentialSnapshotRevision = 16,

    /// <summary>The current observation tick preceded the previous tick.</summary>
    ObservationTickRegressed = 17,

    /// <summary>A retained attempt changed stable lineage.</summary>
    RetainedAttemptLineageMismatch = 18,

    /// <summary>A retained request authority revision moved backwards.</summary>
    RetainedRequestRevisionRegressed = 19,

    /// <summary>A retained lease authority revision moved backwards.</summary>
    RetainedLeaseRevisionRegressed = 20,
}
