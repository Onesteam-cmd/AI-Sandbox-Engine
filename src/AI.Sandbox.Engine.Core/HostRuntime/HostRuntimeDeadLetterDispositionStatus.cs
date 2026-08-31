namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Defines explicit Host dead-letter disposition outcomes.</summary>
public enum HostRuntimeDeadLetterDispositionStatus
{
    /// <summary>
    /// Retry exhaustion produced immutable dead-letter disposition authority.
    /// </summary>
    Disposed = 0,

    /// <summary>The settlement outcome cannot be dead-lettered.</summary>
    InvalidSettlementOutcome = 1,

    /// <summary>The retry decision does not contain settlement request authority.</summary>
    SettlementRequestMismatch = 2,

    /// <summary>The completed attempt number does not match settlement authority.</summary>
    AttemptNumberMismatch = 3,

    /// <summary>The supplied or decided monotonic clock does not match.</summary>
    ClockMismatch = 4,

    /// <summary>The disposition tick precedes terminal settlement.</summary>
    BeforeSettlement = 5,

    /// <summary>The retry decision still permits another attempt.</summary>
    RetryStillAllowed = 6,

    /// <summary>The denied retry status is not a terminal disposition reason.</summary>
    UnsupportedRetryDenial = 7,
}
