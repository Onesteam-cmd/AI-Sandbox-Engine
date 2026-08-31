using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Commands;

/// <summary>
/// Validates versioned exact-type commands and attempts one authoritative World
/// State commit per accepted command.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
/// <remarks>
/// The processor is caller-driven and contains no queue, ordering policy,
/// retries, timers, threads, event dispatch, persistence, I/O, or provider
/// calls. Concurrent callers may receive explicit version conflicts.
/// </remarks>
public sealed class CommandProcessor<TState>
    where TState : class, IWorldState
{
    private readonly WorldStateManager<TState> worldStateManager;
    private readonly IReadOnlyDictionary<Type, object> handlers;

    internal CommandProcessor(
        WorldStateManager<TState> worldStateManager,
        Dictionary<Type, object> handlers)
    {
        this.worldStateManager = worldStateManager;
        this.handlers =
            new System.Collections.ObjectModel.ReadOnlyDictionary<Type, object>(
                handlers);
    }

    /// <summary>
    /// Gets the number of exact command types with registered handlers.
    /// </summary>
    public int HandlerCount => handlers.Count;

    /// <summary>
    /// Validates and executes one immutable exact-type command.
    /// </summary>
    /// <typeparam name="TCommand">The exact concrete command type.</typeparam>
    /// <param name="envelope">The versioned command envelope.</param>
    /// <returns>An explicit execution result.</returns>
    public CommandExecutionResult<TState> Execute<TCommand>(
        CommandEnvelope<TCommand> envelope)
        where TCommand : notnull, IEngineCommand
    {
        ArgumentNullException.ThrowIfNull(envelope);
        CommandTypePolicy.EnsureConcrete<TCommand>();

        var observed = worldStateManager.Read();

        if (envelope.WorldId != observed.WorldId)
        {
            return CommandExecutionResult<TState>.Failed(
                CommandExecutionStatus.WorldMismatch,
                envelope.CommandId,
                observed,
                false,
                "The command targets a different world.");
        }

        if (envelope.ExpectedWorldStateVersion != observed.Version)
        {
            return CommandExecutionResult<TState>.Failed(
                CommandExecutionStatus.VersionConflict,
                envelope.CommandId,
                observed,
                false,
                "The command was formed from a stale World State version.");
        }

        if (envelope.ExpectedSimulationTick != observed.SimulationTick)
        {
            return CommandExecutionResult<TState>.Failed(
                CommandExecutionStatus.SimulationTickMismatch,
                envelope.CommandId,
                observed,
                false,
                "The command was formed at a different simulation tick.");
        }

        if (!handlers.TryGetValue(
            typeof(TCommand),
            out var untypedHandler))
        {
            return CommandExecutionResult<TState>.Failed(
                CommandExecutionStatus.HandlerNotRegistered,
                envelope.CommandId,
                observed,
                false,
                $"No handler is registered for command type " +
                $"'{typeof(TCommand)}'.");
        }

        var handler =
            (ICommandHandler<TState, TCommand>)untypedHandler;
        var transition = new CommandTransition<TState, TCommand>(
            envelope,
            handler);
        var applyResult = worldStateManager.TryApply(
            envelope.ExpectedWorldStateVersion,
            observed.SimulationTick,
            transition);

        return applyResult.Status switch
        {
            WorldStateApplyStatus.Applied =>
                CommandExecutionResult<TState>.Applied(
                    envelope.CommandId,
                    applyResult.Snapshot),

            WorldStateApplyStatus.VersionConflict =>
                CommandExecutionResult<TState>.Failed(
                    CommandExecutionStatus.VersionConflict,
                    envelope.CommandId,
                    applyResult.Snapshot,
                    transition.WasEvaluated,
                    "Authoritative World State changed before command commit."),

            WorldStateApplyStatus.Rejected =>
                CreateRejectedResult(
                    envelope,
                    applyResult,
                    transition),

            _ => throw new InvalidOperationException(
                $"World State returned impossible command status " +
                $"'{applyResult.Status}'."),
        };
    }

    private static CommandExecutionResult<TState> CreateRejectedResult<TCommand>(
        CommandEnvelope<TCommand> envelope,
        WorldStateApplyResult<TState> applyResult,
        CommandTransition<TState, TCommand> transition)
        where TCommand : notnull, IEngineCommand
    {
        if (!transition.WasEvaluated ||
            string.IsNullOrWhiteSpace(transition.RejectionReason) ||
            string.IsNullOrWhiteSpace(applyResult.RejectionReason))
        {
            throw new InvalidOperationException(
                "World State reported a rejected command transition without " +
                "complete handler diagnostics.");
        }

        return CommandExecutionResult<TState>.Failed(
            CommandExecutionStatus.Rejected,
            envelope.CommandId,
            applyResult.Snapshot,
            true,
            transition.RejectionReason);
    }
}
