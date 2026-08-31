using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Commands;

/// <summary>
/// Represents a command handler's pure accepted or rejected decision.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
public sealed class CommandDecision<TState>
    where TState : class, IWorldState
{
    private CommandDecision(
        CommandDecisionStatus status,
        TState? nextState,
        string? rejectionReason)
    {
        Status = status;
        NextState = nextState;
        RejectionReason = rejectionReason;
    }

    /// <summary>
    /// Gets the decision status.
    /// </summary>
    public CommandDecisionStatus Status { get; }

    /// <summary>
    /// Gets the proposed immutable state when accepted.
    /// </summary>
    public TState? NextState { get; }

    /// <summary>
    /// Gets the internal rejection reason when rejected.
    /// </summary>
    public string? RejectionReason { get; }

    /// <summary>
    /// Accepts a command with a new immutable World State root.
    /// </summary>
    /// <param name="nextState">The proposed immutable state.</param>
    /// <returns>An accepted decision.</returns>
    public static CommandDecision<TState> Accept(TState nextState)
    {
        ArgumentNullException.ThrowIfNull(nextState);

        return new CommandDecision<TState>(
            CommandDecisionStatus.Accepted,
            nextState,
            null);
    }

    /// <summary>
    /// Rejects a command for the observed state.
    /// </summary>
    /// <param name="reason">A non-empty internal rejection reason.</param>
    /// <returns>A rejected decision.</returns>
    public static CommandDecision<TState> Reject(string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        return new CommandDecision<TState>(
            CommandDecisionStatus.Rejected,
            null,
            reason);
    }
}
