namespace AI.Sandbox.Engine.Core.Runtime;

/// <summary>
/// Identifies the authoritative operation that produced one completed commit
/// fact.
/// </summary>
public enum RuntimeCommitKind
{
    /// <summary>
    /// A validated command changed state at the current logical tick.
    /// </summary>
    Command = 0,

    /// <summary>
    /// One complete simulation tick committed.
    /// </summary>
    SimulationTick = 1,
}
