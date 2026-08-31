using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Commands;

/// <summary>
/// Registers exactly one pure handler for each concrete command type before
/// creating a command processor.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
public sealed class CommandProcessorBuilder<TState>
    where TState : class, IWorldState
{
    private readonly Dictionary<Type, object> handlers = [];
    private bool isBuilt;

    /// <summary>
    /// Registers one handler for one exact concrete command type.
    /// </summary>
    /// <typeparam name="TCommand">The exact concrete command type.</typeparam>
    /// <param name="handler">The pure command handler.</param>
    /// <returns>This builder.</returns>
    public CommandProcessorBuilder<TState> Add<TCommand>(
        ICommandHandler<TState, TCommand> handler)
        where TCommand : notnull, IEngineCommand
    {
        ThrowIfBuilt();
        CommandTypePolicy.EnsureConcrete<TCommand>();
        ArgumentNullException.ThrowIfNull(handler);

        if (!handlers.TryAdd(typeof(TCommand), handler))
        {
            throw new ArgumentException(
                $"A handler for command type '{typeof(TCommand)}' is " +
                "already registered.",
                nameof(handler));
        }

        return this;
    }

    /// <summary>
    /// Creates a processor bound to one authoritative World State Manager and
    /// permanently consumes this builder.
    /// </summary>
    /// <param name="worldStateManager">The sole authority to update.</param>
    /// <returns>The exact-type command processor.</returns>
    public CommandProcessor<TState> Build(
        WorldStateManager<TState> worldStateManager)
    {
        ThrowIfBuilt();
        ArgumentNullException.ThrowIfNull(worldStateManager);
        isBuilt = true;

        return new CommandProcessor<TState>(
            worldStateManager,
            new Dictionary<Type, object>(handlers));
    }

    private void ThrowIfBuilt()
    {
        if (isBuilt)
        {
            throw new InvalidOperationException(
                "A command processor builder cannot be reused after Build.");
        }
    }
}
