using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Commands;

/// <summary>
/// Captures one immutable command together with the exact world snapshot
/// metadata on which the command was based.
/// </summary>
/// <typeparam name="TCommand">The exact concrete command type.</typeparam>
public sealed class CommandEnvelope<TCommand>
    where TCommand : notnull, IEngineCommand
{
    private CommandEnvelope(
        Id<CommandIdKind> commandId,
        Id<WorldIdKind> worldId,
        WorldStateVersion expectedWorldStateVersion,
        ulong expectedSimulationTick,
        TCommand payload)
    {
        CommandId = commandId;
        WorldId = worldId;
        ExpectedWorldStateVersion = expectedWorldStateVersion;
        ExpectedSimulationTick = expectedSimulationTick;
        Payload = payload;
    }

    /// <summary>
    /// Gets the stable externally assigned command identifier.
    /// </summary>
    public Id<CommandIdKind> CommandId { get; }

    /// <summary>
    /// Gets the intended world.
    /// </summary>
    public Id<WorldIdKind> WorldId { get; }

    /// <summary>
    /// Gets the World State version observed when the command was formed.
    /// </summary>
    public WorldStateVersion ExpectedWorldStateVersion { get; }

    /// <summary>
    /// Gets the logical simulation tick observed when the command was formed.
    /// </summary>
    public ulong ExpectedSimulationTick { get; }

    /// <summary>
    /// Gets the immutable command payload.
    /// </summary>
    public TCommand Payload { get; }

    /// <summary>
    /// Creates an immutable command envelope.
    /// </summary>
    /// <param name="commandId">A non-empty externally assigned command ID.</param>
    /// <param name="worldId">The non-empty intended world ID.</param>
    /// <param name="expectedWorldStateVersion">
    /// The authoritative version used to make this command.
    /// </param>
    /// <param name="expectedSimulationTick">
    /// The logical tick used to make this command.
    /// </param>
    /// <param name="payload">The immutable concrete command payload.</param>
    /// <returns>The command envelope.</returns>
    public static CommandEnvelope<TCommand> Create(
        Id<CommandIdKind> commandId,
        Id<WorldIdKind> worldId,
        WorldStateVersion expectedWorldStateVersion,
        ulong expectedSimulationTick,
        TCommand payload)
    {
        if (commandId.IsEmpty)
        {
            throw new ArgumentException(
                "A command identifier cannot be empty.",
                nameof(commandId));
        }

        if (worldId.IsEmpty)
        {
            throw new ArgumentException(
                "A command world identifier cannot be empty.",
                nameof(worldId));
        }

        CommandTypePolicy.EnsureConcrete<TCommand>();
        CommandTypePolicy.EnsureValue(payload);

        return new CommandEnvelope<TCommand>(
            commandId,
            worldId,
            expectedWorldStateVersion,
            expectedSimulationTick,
            payload);
    }
}
