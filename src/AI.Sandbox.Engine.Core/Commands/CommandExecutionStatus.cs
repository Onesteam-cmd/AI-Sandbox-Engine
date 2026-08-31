namespace AI.Sandbox.Engine.Core.Commands;

/// <summary>
/// Identifies the externally visible outcome of one command processor call.
/// </summary>
public enum CommandExecutionStatus
{
    /// <summary>
    /// The handler accepted and the new state committed.
    /// </summary>
    Applied = 0,

    /// <summary>
    /// The handler rejected the command for the observed state.
    /// </summary>
    Rejected = 1,

    /// <summary>
    /// No handler is registered for the exact command type.
    /// </summary>
    HandlerNotRegistered = 2,

    /// <summary>
    /// The envelope targets a different world.
    /// </summary>
    WorldMismatch = 3,

    /// <summary>
    /// The expected World State version is stale or changed during evaluation.
    /// </summary>
    VersionConflict = 4,

    /// <summary>
    /// The envelope was formed at a different logical simulation tick.
    /// </summary>
    SimulationTickMismatch = 5,
}
