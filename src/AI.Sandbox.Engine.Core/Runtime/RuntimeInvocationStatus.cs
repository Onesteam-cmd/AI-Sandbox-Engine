namespace AI.Sandbox.Engine.Core.Runtime;

/// <summary>
/// Identifies whether a caller-driven runtime operation was admitted.
/// </summary>
public enum RuntimeInvocationStatus
{
    /// <summary>
    /// The operation was admitted and produced its normal subsystem result.
    /// </summary>
    Completed = 0,

    /// <summary>
    /// Another runtime operation was already in progress.
    /// </summary>
    Busy = 1,
}
