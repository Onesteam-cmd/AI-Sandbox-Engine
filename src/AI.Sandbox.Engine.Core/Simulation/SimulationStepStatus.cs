namespace AI.Sandbox.Engine.Core.Simulation;

/// <summary>
/// Identifies the externally visible outcome of one scheduler step.
/// </summary>
public enum SimulationStepStatus
{
    /// <summary>
    /// Every system accepted and one new authoritative tick was committed.
    /// </summary>
    Applied = 0,

    /// <summary>
    /// Another writer changed authoritative World State before this step could
    /// commit.
    /// </summary>
    VersionConflict = 1,

    /// <summary>
    /// One simulation system rejected the complete tick.
    /// </summary>
    SystemRejected = 2,
}
