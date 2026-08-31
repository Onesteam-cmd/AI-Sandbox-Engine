namespace AI.Sandbox.Engine.Core.Behavior;

/// <summary>
/// Describes the complete outcome of one action-validation invocation.
/// </summary>
public enum ActionValidationStatus
{
    /// <summary>
    /// The validator approved the proposal.
    /// </summary>
    Approved = 1,

    /// <summary>
    /// The validator rejected the proposal.
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// The proposal belongs to another world.
    /// </summary>
    WorldMismatch = 3,

    /// <summary>
    /// The proposal version is stale or authority changed during evaluation.
    /// </summary>
    VersionConflict = 4,

    /// <summary>
    /// The proposal tick is stale or authority advanced during evaluation.
    /// </summary>
    SimulationTickMismatch = 5,
}
