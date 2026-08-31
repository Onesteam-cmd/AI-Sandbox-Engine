namespace AI.Sandbox.Engine.Core.Social;

/// <summary>
/// Describes one coordinator decision.
/// </summary>
public enum SocialTurnCoordinationDecisionStatus
{
    /// <summary>
    /// One proposal was granted the speaking floor.
    /// </summary>
    Granted = 0,

    /// <summary>
    /// No participant should receive a speaking turn.
    /// </summary>
    NoTurn = 1,

    /// <summary>
    /// Coordination was explicitly rejected.
    /// </summary>
    Rejected = 2,
}
