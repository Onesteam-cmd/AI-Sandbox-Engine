namespace AI.Sandbox.Engine.Core.Behavior;

/// <summary>
/// Describes the semantic decision returned by one action validator.
/// </summary>
public enum ActionDecisionStatus
{
    /// <summary>
    /// The proposal may be translated into the returned command payload.
    /// </summary>
    Approved = 1,

    /// <summary>
    /// The proposal is not currently permitted.
    /// </summary>
    Rejected = 2,
}
