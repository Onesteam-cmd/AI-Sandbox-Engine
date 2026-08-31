namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Defines explicit advisory Host retry-decision outcomes.</summary>
public enum HostRuntimeRetryDecisionStatus
{
    /// <summary>A bounded retry is permitted at the reported tick.</summary>
    RetryAllowed = 0,

    /// <summary>The optimistic request revision did not match.</summary>
    StaleRevision = 1,

    /// <summary>The request state is not eligible for retry evaluation.</summary>
    InvalidRequestState = 2,

    /// <summary>The external monotonic deadline has expired or blocks the retry.</summary>
    DeadlineExceeded = 3,

    /// <summary>The inclusive maximum attempt count has been reached.</summary>
    AttemptLimitReached = 4,
}
