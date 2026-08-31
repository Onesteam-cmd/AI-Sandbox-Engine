namespace AI.Sandbox.Engine.Core.WorldState;

/// <summary>
/// Reports the outcome of one attempted authoritative world-state transition.
/// </summary>
/// <typeparam name="TState">The immutable world-state root type.</typeparam>
public sealed class WorldStateApplyResult<TState>
    where TState : class, IWorldState
{
    private WorldStateApplyResult(
        WorldStateApplyStatus status,
        WorldStateSnapshot<TState> snapshot,
        string? rejectionReason)
    {
        Status = status;
        Snapshot = snapshot;
        RejectionReason = rejectionReason;
    }

    /// <summary>
    /// Gets the transition outcome.
    /// </summary>
    public WorldStateApplyStatus Status { get; }

    /// <summary>
    /// Gets the authoritative snapshot after the attempt.
    /// </summary>
    public WorldStateSnapshot<TState> Snapshot { get; }

    /// <summary>
    /// Gets the transition-defined rejection reason when
    /// <see cref="Status"/> is <see cref="WorldStateApplyStatus.Rejected"/>.
    /// </summary>
    public string? RejectionReason { get; }

    /// <summary>
    /// Gets a value indicating whether the proposed state was committed.
    /// </summary>
    public bool WasApplied => Status == WorldStateApplyStatus.Applied;

    internal static WorldStateApplyResult<TState> Applied(
        WorldStateSnapshot<TState> snapshot)
    {
        return new WorldStateApplyResult<TState>(
            WorldStateApplyStatus.Applied,
            snapshot,
            null);
    }

    internal static WorldStateApplyResult<TState> VersionConflict(
        WorldStateSnapshot<TState> snapshot)
    {
        return new WorldStateApplyResult<TState>(
            WorldStateApplyStatus.VersionConflict,
            snapshot,
            null);
    }

    internal static WorldStateApplyResult<TState> Rejected(
        WorldStateSnapshot<TState> snapshot,
        string reason)
    {
        return new WorldStateApplyResult<TState>(
            WorldStateApplyStatus.Rejected,
            snapshot,
            reason);
    }

    internal static WorldStateApplyResult<TState> SimulationTickRegression(
        WorldStateSnapshot<TState> snapshot)
    {
        return new WorldStateApplyResult<TState>(
            WorldStateApplyStatus.SimulationTickRegression,
            snapshot,
            null);
    }
}
