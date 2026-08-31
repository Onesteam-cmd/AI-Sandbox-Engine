using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Simulation;

/// <summary>
/// Advances one authoritative world through deterministic logical simulation
/// ticks.
/// </summary>
/// <typeparam name="TState">The immutable world-state root type.</typeparam>
/// <remarks>
/// Every call to <see cref="RunNextTick"/> evaluates the fixed system sequence
/// once and attempts one World State commit. Calls on the same scheduler are
/// serialized. The scheduler does not create threads, run timers, read
/// wall-clock time, retry conflicts, dispatch events, persist state, or perform
/// external I/O.
/// </remarks>
public sealed class SimulationScheduler<TState>
    where TState : class, IWorldState
{
    private readonly System.Threading.Lock runGate = new();
    private readonly WorldStateManager<TState> worldStateManager;
    private readonly IReadOnlyList<SimulationSystemRegistration<TState>>
        registrations;

    internal SimulationScheduler(
        WorldStateManager<TState> worldStateManager,
        SimulationSystemRegistration<TState>[] registrations)
    {
        this.worldStateManager = worldStateManager;
        this.registrations = Array.AsReadOnly(registrations);
    }

    /// <summary>
    /// Gets the number of systems in the fixed deterministic sequence.
    /// </summary>
    public int SystemCount => registrations.Count;

    /// <summary>
    /// Evaluates one logical tick and attempts one authoritative commit.
    /// </summary>
    /// <returns>
    /// An applied, version-conflict, or system-rejected result.
    /// </returns>
    /// <exception cref="OverflowException">
    /// Thrown when the authoritative simulation tick is
    /// <see cref="ulong.MaxValue"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a system returns a null or inconsistent decision, or when an
    /// impossible World State result is observed.
    /// </exception>
    public SimulationStepResult<TState> RunNextTick()
    {
        lock (runGate)
        {
            var observed = worldStateManager.Read();
            var targetSimulationTick =
                checked(observed.SimulationTick + 1);
            var transition = new SimulationTickTransition<TState>(
                registrations,
                targetSimulationTick);
            var applyResult = worldStateManager.TryApply(
                observed.Version,
                targetSimulationTick,
                transition);

            return applyResult.Status switch
            {
                WorldStateApplyStatus.Applied =>
                    SimulationStepResult<TState>.Applied(
                        applyResult.Snapshot,
                        targetSimulationTick,
                        transition.ExecutedSystemCount),

                WorldStateApplyStatus.VersionConflict =>
                    SimulationStepResult<TState>.VersionConflict(
                        applyResult.Snapshot,
                        targetSimulationTick,
                        transition.ExecutedSystemCount),

                WorldStateApplyStatus.Rejected =>
                    CreateRejectedResult(
                        applyResult,
                        transition,
                        targetSimulationTick),

                _ => throw new InvalidOperationException(
                    $"World State returned impossible scheduler status " +
                    $"'{applyResult.Status}'."),
            };
        }
    }

    private static SimulationStepResult<TState> CreateRejectedResult(
        WorldStateApplyResult<TState> applyResult,
        SimulationTickTransition<TState> transition,
        ulong targetSimulationTick)
    {
        if (transition.RejectedSystemId is not { } rejectedSystemId ||
            string.IsNullOrWhiteSpace(transition.RejectionReason) ||
            string.IsNullOrWhiteSpace(applyResult.RejectionReason))
        {
            throw new InvalidOperationException(
                "World State reported a rejected simulation transition " +
                "without complete system diagnostics.");
        }

        return SimulationStepResult<TState>.SystemRejected(
            applyResult.Snapshot,
            targetSimulationTick,
            transition.ExecutedSystemCount,
            rejectedSystemId,
            transition.RejectionReason);
    }
}
