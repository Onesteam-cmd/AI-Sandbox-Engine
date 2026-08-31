using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Persistence;

/// <summary>
/// Reports the result of restoring one versioned World Snapshot document.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
public sealed class SnapshotRestoreResult<TState>
    where TState : class, IWorldState
{
    private SnapshotRestoreResult(
        SnapshotRestoreStatus status,
        WorldStateSnapshot<TState>? snapshot,
        string? failureReason)
    {
        Status = status;
        Snapshot = snapshot;
        FailureReason = failureReason;
    }

    /// <summary>
    /// Gets the restore outcome.
    /// </summary>
    public SnapshotRestoreStatus Status { get; }

    /// <summary>
    /// Gets the restored immutable snapshot when successful.
    /// </summary>
    public WorldStateSnapshot<TState>? Snapshot { get; }

    /// <summary>
    /// Gets a diagnostic reason when restoration fails.
    /// </summary>
    public string? FailureReason { get; }

    /// <summary>
    /// Gets a value indicating whether restoration succeeded.
    /// </summary>
    public bool WasRestored => Status == SnapshotRestoreStatus.Restored;

    internal static SnapshotRestoreResult<TState> Restored(
        WorldStateSnapshot<TState> snapshot)
    {
        return new SnapshotRestoreResult<TState>(
            SnapshotRestoreStatus.Restored,
            snapshot,
            null);
    }

    internal static SnapshotRestoreResult<TState> Failed(
        SnapshotRestoreStatus status,
        string failureReason)
    {
        if (status == SnapshotRestoreStatus.Restored)
        {
            throw new ArgumentOutOfRangeException(
                nameof(status),
                status,
                "A failed restore result cannot use Restored status.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(failureReason);

        return new SnapshotRestoreResult<TState>(
            status,
            null,
            failureReason);
    }
}
