namespace AI.Sandbox.Engine.Core.WorldState;

/// <summary>
/// Identifies the outcome of one attempted authoritative world-state
/// transition.
/// </summary>
public enum WorldStateApplyStatus
{
    /// <summary>
    /// The proposed next state was committed atomically.
    /// </summary>
    Applied = 0,

    /// <summary>
    /// The expected version did not match the current authoritative version.
    /// </summary>
    VersionConflict = 1,

    /// <summary>
    /// The transition evaluated successfully but rejected its own proposal.
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// The requested simulation tick was older than authoritative state.
    /// </summary>
    SimulationTickRegression = 3,
}
