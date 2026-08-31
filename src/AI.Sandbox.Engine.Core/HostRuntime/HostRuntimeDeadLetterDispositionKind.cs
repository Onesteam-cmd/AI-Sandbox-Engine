namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines exact terminal reasons for Host dead-letter disposition.
/// </summary>
public enum HostRuntimeDeadLetterDispositionKind
{
    /// <summary>The inclusive retry-attempt limit was reached.</summary>
    AttemptLimitReached = 0,

    /// <summary>The external monotonic deadline denied another attempt.</summary>
    DeadlineExceeded = 1,
}
