using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Commands;

/// <summary>
/// Reports the result of one attempted command execution.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
public sealed class CommandExecutionResult<TState>
    where TState : class, IWorldState
{
    private CommandExecutionResult(
        CommandExecutionStatus status,
        Id<CommandIdKind> commandId,
        WorldStateSnapshot<TState> snapshot,
        bool handlerWasEvaluated,
        string? failureReason)
    {
        Status = status;
        CommandId = commandId;
        Snapshot = snapshot;
        HandlerWasEvaluated = handlerWasEvaluated;
        FailureReason = failureReason;
    }

    /// <summary>
    /// Gets the execution outcome.
    /// </summary>
    public CommandExecutionStatus Status { get; }

    /// <summary>
    /// Gets the command identifier.
    /// </summary>
    public Id<CommandIdKind> CommandId { get; }

    /// <summary>
    /// Gets the authoritative snapshot after the attempt.
    /// </summary>
    public WorldStateSnapshot<TState> Snapshot { get; }

    /// <summary>
    /// Gets a value indicating whether the command handler ran.
    /// </summary>
    public bool HandlerWasEvaluated { get; }

    /// <summary>
    /// Gets an internal diagnostic reason when execution did not apply.
    /// </summary>
    public string? FailureReason { get; }

    /// <summary>
    /// Gets a value indicating whether the command changed authoritative state.
    /// </summary>
    public bool WasApplied => Status == CommandExecutionStatus.Applied;

    internal static CommandExecutionResult<TState> Applied(
        Id<CommandIdKind> commandId,
        WorldStateSnapshot<TState> snapshot)
    {
        return new CommandExecutionResult<TState>(
            CommandExecutionStatus.Applied,
            commandId,
            snapshot,
            true,
            null);
    }

    internal static CommandExecutionResult<TState> Failed(
        CommandExecutionStatus status,
        Id<CommandIdKind> commandId,
        WorldStateSnapshot<TState> snapshot,
        bool handlerWasEvaluated,
        string failureReason)
    {
        if (status == CommandExecutionStatus.Applied)
        {
            throw new ArgumentOutOfRangeException(
                nameof(status),
                status,
                "A failed command result cannot use Applied status.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(failureReason);

        return new CommandExecutionResult<TState>(
            status,
            commandId,
            snapshot,
            handlerWasEvaluated,
            failureReason);
    }
}
