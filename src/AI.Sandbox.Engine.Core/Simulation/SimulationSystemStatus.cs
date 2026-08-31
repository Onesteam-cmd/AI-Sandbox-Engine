namespace AI.Sandbox.Engine.Core.Simulation;

/// <summary>
/// Identifies the outcome proposed by one simulation system.
/// </summary>
public enum SimulationSystemStatus
{
    /// <summary>
    /// The system accepted the current working state without changing it.
    /// </summary>
    Unchanged = 0,

    /// <summary>
    /// The system produced a new immutable working state.
    /// </summary>
    Updated = 1,

    /// <summary>
    /// The system rejected the complete simulation tick.
    /// </summary>
    Rejected = 2,
}
