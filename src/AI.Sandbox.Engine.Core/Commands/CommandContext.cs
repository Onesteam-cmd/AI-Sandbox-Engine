using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Commands;

/// <summary>
/// Provides one command handler with the immutable command and exact
/// authoritative snapshot being evaluated.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
/// <typeparam name="TCommand">The exact concrete command type.</typeparam>
public sealed class CommandContext<TState, TCommand>
    where TState : class, IWorldState
    where TCommand : notnull, IEngineCommand
{
    internal CommandContext(
        WorldStateSnapshot<TState> snapshot,
        CommandEnvelope<TCommand> envelope)
    {
        Snapshot = snapshot;
        Envelope = envelope;
    }

    /// <summary>
    /// Gets the immutable snapshot against which the command is evaluated.
    /// </summary>
    public WorldStateSnapshot<TState> Snapshot { get; }

    /// <summary>
    /// Gets the immutable command envelope.
    /// </summary>
    public CommandEnvelope<TCommand> Envelope { get; }
}
