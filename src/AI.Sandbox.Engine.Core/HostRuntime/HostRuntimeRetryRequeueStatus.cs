namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Defines explicit Host retry requeue outcomes.</summary>
public enum HostRuntimeRetryRequeueStatus
{
    /// <summary>
    /// Retry authority reopened the terminal request and produced admission.
    /// </summary>
    Requeued = 0,

    /// <summary>The settlement outcome cannot be retried.</summary>
    InvalidSettlementOutcome = 1,

    /// <summary>The advisory retry decision denied another attempt.</summary>
    RetryDenied = 2,

    /// <summary>The retry decision does not contain settlement request authority.</summary>
    SettlementRequestMismatch = 3,

    /// <summary>The retry attempt lineage does not match the settlement.</summary>
    AttemptNumberMismatch = 4,

    /// <summary>The supplied or decided monotonic clock does not match.</summary>
    ClockMismatch = 5,

    /// <summary>The retry decision was evaluated before settlement.</summary>
    RetryDecisionBeforeSettlement = 6,

    /// <summary>The requeue tick precedes settlement.</summary>
    BeforeSettlement = 7,

    /// <summary>The advisory retry tick has not been reached.</summary>
    BeforeRetryTick = 8,

    /// <summary>The optimistic queue revision did not match.</summary>
    StaleQueueRevision = 9,

    /// <summary>The represented queue has no admission capacity.</summary>
    QueueFull = 10,

    /// <summary>Existing queue-admission contracts rejected re-admission.</summary>
    AdmissionRejected = 11,
}
