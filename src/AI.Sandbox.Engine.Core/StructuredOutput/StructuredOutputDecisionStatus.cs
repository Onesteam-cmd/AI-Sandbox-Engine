namespace AI.Sandbox.Engine.Core.StructuredOutput;

/// <summary>
/// Describes one exact structured-output decoder decision.
/// </summary>
public enum StructuredOutputDecisionStatus
{
    /// <summary>
    /// The response decoded to one exact structured payload.
    /// </summary>
    Decoded = 0,

    /// <summary>
    /// The decoder explicitly rejected the response.
    /// </summary>
    Rejected = 1,
}
