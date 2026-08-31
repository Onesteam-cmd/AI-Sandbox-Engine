using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Simulation;

/// <summary>
/// Describes one simulation system's pure decision for the current tick.
/// </summary>
/// <typeparam name="TState">The immutable world-state root type.</typeparam>
public sealed class SimulationSystemDecision<TState>
    where TState : class, IWorldState
{
    private static readonly SimulationSystemDecision<TState>
        UnchangedInstance = new(
            SimulationSystemStatus.Unchanged,
            null,
            null);

    private SimulationSystemDecision(
        SimulationSystemStatus status,
        TState? nextState,
        string? rejectionReason)
    {
        Status = status;
        NextState = nextState;
        RejectionReason = rejectionReason;
    }

    /// <summary>
    /// Gets the system decision status.
    /// </summary>
    public SimulationSystemStatus Status { get; }

    /// <summary>
    /// Gets the proposed immutable state when
    /// <see cref="Status"/> is <see cref="SimulationSystemStatus.Updated"/>.
    /// </summary>
    public TState? NextState { get; }

    /// <summary>
    /// Gets the internal rejection reason when
    /// <see cref="Status"/> is <see cref="SimulationSystemStatus.Rejected"/>.
    /// </summary>
    public string? RejectionReason { get; }

    /// <summary>
    /// Accepts the current working state without modification.
    /// </summary>
    /// <returns>A shared unchanged decision.</returns>
    public static SimulationSystemDecision<TState> Unchanged()
    {
        return UnchangedInstance;
    }

    /// <summary>
    /// Proposes a new immutable working state for later systems in this tick.
    /// </summary>
    /// <param name="nextState">The next immutable working state.</param>
    /// <returns>An updated decision.</returns>
    public static SimulationSystemDecision<TState> Update(TState nextState)
    {
        ArgumentNullException.ThrowIfNull(nextState);

        return new SimulationSystemDecision<TState>(
            SimulationSystemStatus.Updated,
            nextState,
            null);
    }

    /// <summary>
    /// Rejects the complete tick without changing authoritative World State.
    /// </summary>
    /// <param name="reason">A non-empty internal rejection reason.</param>
    /// <returns>A rejected decision.</returns>
    public static SimulationSystemDecision<TState> Reject(string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        return new SimulationSystemDecision<TState>(
            SimulationSystemStatus.Rejected,
            null,
            reason);
    }
}
