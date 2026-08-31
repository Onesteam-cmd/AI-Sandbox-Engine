using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Commands;

/// <summary>
/// Purely validates one exact command type and proposes a new immutable state.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
/// <typeparam name="TCommand">The exact concrete command type.</typeparam>
/// <remarks>
/// Handlers execute at most once per processor call. They must not mutate the
/// supplied snapshot, dispatch events, perform I/O, read wall-clock time,
/// generate IDs, call external providers, or modify global state.
/// </remarks>
public interface ICommandHandler<TState, TCommand>
    where TState : class, IWorldState
    where TCommand : notnull, IEngineCommand
{
    /// <summary>
    /// Evaluates the command once against the supplied immutable snapshot.
    /// </summary>
    /// <param name="context">The command and observed snapshot.</param>
    /// <returns>An accepted or rejected pure decision.</returns>
    public CommandDecision<TState> Evaluate(
        CommandContext<TState, TCommand> context);
}
