namespace AI.Sandbox.Engine.Core.Commands;

/// <summary>
/// Identifies the pure decision made by one command handler.
/// </summary>
public enum CommandDecisionStatus
{
    /// <summary>
    /// The command is valid and proposes a new immutable state.
    /// </summary>
    Accepted = 0,

    /// <summary>
    /// The command is invalid for the observed state.
    /// </summary>
    Rejected = 1,
}
