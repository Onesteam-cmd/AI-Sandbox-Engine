using AI.Sandbox.Engine.Core.Identifiers;

namespace AI.Sandbox.Engine.Core.WorldState;

/// <summary>
/// Owns the single authoritative snapshot for one world and atomically commits
/// validated immutable transitions.
/// </summary>
/// <typeparam name="TState">The immutable world-state root type.</typeparam>
/// <remarks>
/// Transition evaluation occurs outside the commit lock. The manager captures a
/// versioned snapshot, evaluates the caller-supplied transition once, and then
/// commits only if that exact version is still authoritative. The manager does
/// not retry, dispatch events, persist data, call external services, or read
/// wall-clock time.
/// </remarks>
public sealed class WorldStateManager<TState>
    where TState : class, IWorldState
{
    private readonly System.Threading.Lock gate = new();
    private WorldStateSnapshot<TState> current;

    private WorldStateManager(WorldStateSnapshot<TState> initialSnapshot)
    {
        current = initialSnapshot;
    }

    /// <summary>
    /// Creates a manager for one world at version zero.
    /// </summary>
    /// <param name="worldId">The non-empty world identifier.</param>
    /// <param name="initialState">The immutable initial world-state root.</param>
    /// <param name="initialSimulationTick">
    /// The logical simulation tick represented by the initial state.
    /// </param>
    /// <returns>A new authoritative world-state manager.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="worldId"/> is empty.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="initialState"/> is null.
    /// </exception>
    public static WorldStateManager<TState> Create(
        Id<WorldIdKind> worldId,
        TState initialState,
        ulong initialSimulationTick = 0)
    {
        if (worldId.IsEmpty)
        {
            throw new ArgumentException(
                "A World State Manager requires a non-empty world identifier.",
                nameof(worldId));
        }

        ArgumentNullException.ThrowIfNull(initialState);

        var snapshot = new WorldStateSnapshot<TState>(
            worldId,
            WorldStateVersion.Initial,
            initialSimulationTick,
            initialState);

        return new WorldStateManager<TState>(snapshot);
    }

    /// <summary>
    /// Restores a manager from one validated immutable snapshot without
    /// changing its world ID, version, logical tick, or state reference.
    /// </summary>
    /// <param name="snapshot">The validated snapshot to adopt as authority.</param>
    /// <returns>A manager whose current authority is <paramref name="snapshot"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="snapshot"/> is null.
    /// </exception>
    /// <remarks>
    /// This method does not read storage or decode data. Persistence adapters
    /// must validate and decode a snapshot before calling it.
    /// </remarks>
    public static WorldStateManager<TState> Restore(
        WorldStateSnapshot<TState> snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        return new WorldStateManager<TState>(snapshot);
    }

    /// <summary>
    /// Reads the currently authoritative immutable snapshot.
    /// </summary>
    /// <returns>The current snapshot.</returns>
    public WorldStateSnapshot<TState> Read()
    {
        lock (gate)
        {
            return current;
        }
    }

    /// <summary>
    /// Evaluates and attempts to atomically commit one world-state transition.
    /// </summary>
    /// <param name="expectedVersion">
    /// The authoritative version the caller expects to replace.
    /// </param>
    /// <param name="simulationTick">
    /// The logical simulation tick for the proposed state.
    /// </param>
    /// <param name="transition">The transition to evaluate exactly once.</param>
    /// <returns>
    /// The committed snapshot or the unchanged authoritative snapshot with a
    /// conflict, rejection, or tick-regression status.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="transition"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a transition returns a null or internally inconsistent
    /// decision.
    /// </exception>
    /// <exception cref="OverflowException">
    /// Thrown when the authoritative version cannot be incremented.
    /// </exception>
    public WorldStateApplyResult<TState> TryApply(
        WorldStateVersion expectedVersion,
        ulong simulationTick,
        IWorldStateTransition<TState> transition)
    {
        ArgumentNullException.ThrowIfNull(transition);

        WorldStateSnapshot<TState> observed;

        lock (gate)
        {
            observed = current;

            if (observed.Version != expectedVersion)
            {
                return WorldStateApplyResult<TState>.VersionConflict(observed);
            }

            if (simulationTick < observed.SimulationTick)
            {
                return WorldStateApplyResult<TState>
                    .SimulationTickRegression(observed);
            }
        }

        var decision = transition.Evaluate(observed);
        if (decision is null)
        {
            throw new InvalidOperationException(
                "A world-state transition returned a null decision.");
        }

        var nextState = decision.NextState;
        var rejectionReason = decision.RejectionReason;

        if (!decision.IsAccepted)
        {
            if (string.IsNullOrWhiteSpace(rejectionReason) ||
                nextState is not null)
            {
                throw new InvalidOperationException(
                    "A rejected transition decision is internally inconsistent.");
            }

            return WorldStateApplyResult<TState>.Rejected(
                observed,
                rejectionReason);
        }

        if (nextState is null || rejectionReason is not null)
        {
            throw new InvalidOperationException(
                "An accepted transition decision is internally inconsistent.");
        }

        lock (gate)
        {
            if (current.Version != observed.Version)
            {
                return WorldStateApplyResult<TState>.VersionConflict(current);
            }

            if (simulationTick < current.SimulationTick)
            {
                return WorldStateApplyResult<TState>
                    .SimulationTickRegression(current);
            }

            var committed = new WorldStateSnapshot<TState>(
                current.WorldId,
                current.Version.Next(),
                simulationTick,
                nextState);

            current = committed;
            return WorldStateApplyResult<TState>.Applied(committed);
        }
    }
}
