namespace AI.Sandbox.Engine.Core.Social;

/// <summary>
/// Describes whether a participant requests a normal response or an
/// interruption.
/// </summary>
public enum SocialTurnRequestKind
{
    /// <summary>
    /// Requests the next ordinary speaking turn.
    /// </summary>
    Response = 0,

    /// <summary>
    /// Requests an interruption before the current flow yields naturally.
    /// </summary>
    Interruption = 1,
}
