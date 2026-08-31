namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines exact reasons for abandoning one acknowledged Host attempt.
/// </summary>
public enum HostRuntimeAbandonedAttemptDispositionKind
{
    /// <summary>
    /// A recorded cancellation request abandoned the attempt before lease expiry.
    /// </summary>
    CancellationRequested = 0,

    /// <summary>
    /// External monotonic time proved that the active lease expired.
    /// </summary>
    LeaseExpired = 1,
}
