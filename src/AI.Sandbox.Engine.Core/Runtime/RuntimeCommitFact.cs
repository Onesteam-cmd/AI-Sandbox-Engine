using AI.Sandbox.Engine.Core.Commands;
using AI.Sandbox.Engine.Core.Events;
using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Runtime;

/// <summary>
/// Describes one already committed authoritative runtime change.
/// </summary>
/// <remarks>
/// This is a completed fact suitable for explicit post-commit dispatch. The
/// runtime orchestrator returns it but never creates an event envelope or
/// dispatches it.
/// </remarks>
public sealed class RuntimeCommitFact : IEngineEvent
{
    private RuntimeCommitFact(
        RuntimeCommitKind kind,
        Id<WorldIdKind> worldId,
        WorldStateVersion previousWorldStateVersion,
        WorldStateVersion currentWorldStateVersion,
        ulong previousSimulationTick,
        ulong currentSimulationTick,
        Id<CommandIdKind>? commandId)
    {
        Kind = kind;
        WorldId = worldId;
        PreviousWorldStateVersion = previousWorldStateVersion;
        CurrentWorldStateVersion = currentWorldStateVersion;
        PreviousSimulationTick = previousSimulationTick;
        CurrentSimulationTick = currentSimulationTick;
        CommandId = commandId;
    }

    /// <summary>
    /// Gets the authoritative operation kind.
    /// </summary>
    public RuntimeCommitKind Kind { get; }

    /// <summary>
    /// Gets the world whose authority changed.
    /// </summary>
    public Id<WorldIdKind> WorldId { get; }

    /// <summary>
    /// Gets the authority version immediately before the commit.
    /// </summary>
    public WorldStateVersion PreviousWorldStateVersion { get; }

    /// <summary>
    /// Gets the authority version produced by the commit.
    /// </summary>
    public WorldStateVersion CurrentWorldStateVersion { get; }

    /// <summary>
    /// Gets the logical simulation tick immediately before the commit.
    /// </summary>
    public ulong PreviousSimulationTick { get; }

    /// <summary>
    /// Gets the logical simulation tick produced by the commit.
    /// </summary>
    public ulong CurrentSimulationTick { get; }

    /// <summary>
    /// Gets the committed command ID for command commits, or
    /// <see langword="null"/> for simulation-tick commits.
    /// </summary>
    public Id<CommandIdKind>? CommandId { get; }

    /// <summary>
    /// Gets a value indicating whether this fact describes a command commit.
    /// </summary>
    public bool IsCommandCommit =>
        Kind == RuntimeCommitKind.Command;

    internal static RuntimeCommitFact FromCommand<TState>(
        Id<CommandIdKind> commandId,
        WorldStateSnapshot<TState> snapshot)
        where TState : class, IWorldState
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (commandId.IsEmpty)
        {
            throw new ArgumentException(
                "A command commit fact requires a non-empty command ID.",
                nameof(commandId));
        }

        var previousVersion = GetPreviousVersion(snapshot.Version);

        return new RuntimeCommitFact(
            RuntimeCommitKind.Command,
            snapshot.WorldId,
            previousVersion,
            snapshot.Version,
            snapshot.SimulationTick,
            snapshot.SimulationTick,
            commandId);
    }

    internal static RuntimeCommitFact FromSimulationTick<TState>(
        WorldStateSnapshot<TState> snapshot)
        where TState : class, IWorldState
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (snapshot.SimulationTick == 0)
        {
            throw new InvalidOperationException(
                "A committed simulation tick cannot produce tick zero.");
        }

        var previousVersion = GetPreviousVersion(snapshot.Version);

        return new RuntimeCommitFact(
            RuntimeCommitKind.SimulationTick,
            snapshot.WorldId,
            previousVersion,
            snapshot.Version,
            snapshot.SimulationTick - 1,
            snapshot.SimulationTick,
            null);
    }

    private static WorldStateVersion GetPreviousVersion(
        WorldStateVersion current)
    {
        if (current.Value == 0)
        {
            throw new InvalidOperationException(
                "A runtime commit must advance World State version.");
        }

        return current.Value == 1
            ? WorldStateVersion.Initial
            : WorldStateVersion.From(current.Value - 1);
    }
}
