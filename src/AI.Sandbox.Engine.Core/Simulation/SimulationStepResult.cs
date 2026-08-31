using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Simulation;

/// <summary>
/// Reports the result of one attempted logical simulation tick.
/// </summary>
/// <typeparam name="TState">The immutable world-state root type.</typeparam>
public sealed class SimulationStepResult<TState>
    where TState : class, IWorldState
{
    private SimulationStepResult(
        SimulationStepStatus status,
        WorldStateSnapshot<TState> snapshot,
        ulong targetSimulationTick,
        int executedSystemCount,
        Id<SimulationSystemIdKind>? rejectedSystemId,
        string? rejectionReason)
    {
        Status = status;
        Snapshot = snapshot;
        TargetSimulationTick = targetSimulationTick;
        ExecutedSystemCount = executedSystemCount;
        RejectedSystemId = rejectedSystemId;
        RejectionReason = rejectionReason;
    }

    /// <summary>
    /// Gets the scheduler-step outcome.
    /// </summary>
    public SimulationStepStatus Status { get; }

    /// <summary>
    /// Gets the authoritative snapshot after the attempt.
    /// </summary>
    public WorldStateSnapshot<TState> Snapshot { get; }

    /// <summary>
    /// Gets the logical tick this step attempted to evaluate.
    /// </summary>
    public ulong TargetSimulationTick { get; }

    /// <summary>
    /// Gets the number of systems whose <c>Execute</c> method ran.
    /// </summary>
    public int ExecutedSystemCount { get; }

    /// <summary>
    /// Gets the rejecting system ID when
    /// <see cref="Status"/> is <see cref="SimulationStepStatus.SystemRejected"/>.
    /// </summary>
    public Id<SimulationSystemIdKind>? RejectedSystemId { get; }

    /// <summary>
    /// Gets the rejecting system's internal reason when available.
    /// </summary>
    public string? RejectionReason { get; }

    /// <summary>
    /// Gets a value indicating whether one new authoritative tick committed.
    /// </summary>
    public bool WasApplied => Status == SimulationStepStatus.Applied;

    internal static SimulationStepResult<TState> Applied(
        WorldStateSnapshot<TState> snapshot,
        ulong targetSimulationTick,
        int executedSystemCount)
    {
        return new SimulationStepResult<TState>(
            SimulationStepStatus.Applied,
            snapshot,
            targetSimulationTick,
            executedSystemCount,
            null,
            null);
    }

    internal static SimulationStepResult<TState> VersionConflict(
        WorldStateSnapshot<TState> snapshot,
        ulong targetSimulationTick,
        int executedSystemCount)
    {
        return new SimulationStepResult<TState>(
            SimulationStepStatus.VersionConflict,
            snapshot,
            targetSimulationTick,
            executedSystemCount,
            null,
            null);
    }

    internal static SimulationStepResult<TState> SystemRejected(
        WorldStateSnapshot<TState> snapshot,
        ulong targetSimulationTick,
        int executedSystemCount,
        Id<SimulationSystemIdKind> rejectedSystemId,
        string rejectionReason)
    {
        return new SimulationStepResult<TState>(
            SimulationStepStatus.SystemRejected,
            snapshot,
            targetSimulationTick,
            executedSystemCount,
            rejectedSystemId,
            rejectionReason);
    }
}
