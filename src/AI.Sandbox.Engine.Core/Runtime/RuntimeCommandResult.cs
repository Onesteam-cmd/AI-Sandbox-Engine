using AI.Sandbox.Engine.Core.Commands;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Runtime;

/// <summary>
/// Reports one admitted or busy runtime command invocation.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
public sealed class RuntimeCommandResult<TState>
    where TState : class, IWorldState
{
    private RuntimeCommandResult(
        RuntimeInvocationStatus invocationStatus,
        CommandExecutionResult<TState>? commandResult,
        WorldStateSnapshot<TState> snapshot,
        RuntimeCommitFact? commitFact)
    {
        InvocationStatus = invocationStatus;
        CommandResult = commandResult;
        Snapshot = snapshot;
        CommitFact = commitFact;
    }

    /// <summary>
    /// Gets whether the runtime admitted this invocation.
    /// </summary>
    public RuntimeInvocationStatus InvocationStatus { get; }

    /// <summary>
    /// Gets the normal command result when admitted.
    /// </summary>
    public CommandExecutionResult<TState>? CommandResult { get; }

    /// <summary>
    /// Gets the authoritative snapshot observed after this invocation.
    /// </summary>
    public WorldStateSnapshot<TState> Snapshot { get; }

    /// <summary>
    /// Gets the completed commit fact only when the command committed.
    /// </summary>
    public RuntimeCommitFact? CommitFact { get; }

    /// <summary>
    /// Gets a value indicating whether the runtime admitted the call.
    /// </summary>
    public bool WasInvoked =>
        InvocationStatus == RuntimeInvocationStatus.Completed;

    /// <summary>
    /// Gets a value indicating whether the admitted command committed.
    /// </summary>
    public bool WasCommitted => CommitFact is not null;

    internal static RuntimeCommandResult<TState> Busy(
        WorldStateSnapshot<TState> snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        return new RuntimeCommandResult<TState>(
            RuntimeInvocationStatus.Busy,
            null,
            snapshot,
            null);
    }

    internal static RuntimeCommandResult<TState> Completed(
        CommandExecutionResult<TState> commandResult)
    {
        ArgumentNullException.ThrowIfNull(commandResult);

        var commitFact = commandResult.WasApplied
            ? RuntimeCommitFact.FromCommand(
                commandResult.CommandId,
                commandResult.Snapshot)
            : null;

        return new RuntimeCommandResult<TState>(
            RuntimeInvocationStatus.Completed,
            commandResult,
            commandResult.Snapshot,
            commitFact);
    }
}
