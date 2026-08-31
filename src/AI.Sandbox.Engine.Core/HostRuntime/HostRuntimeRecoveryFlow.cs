namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Creates pure recovery checkpoint and continuation authority without storage,
/// serialization, process control, scheduling, or automatic restart.
/// </summary>
public static class HostRuntimeRecoveryFlow
{
    /// <summary>
    /// Captures one recovery checkpoint from existing validated authorities.
    /// </summary>
    /// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
    /// <param name="checkpointId">Externally assigned checkpoint ID.</param>
    /// <param name="lifecycleSnapshot">Current immutable lifecycle authority.</param>
    /// <param name="composition">Validated deterministic composition.</param>
    /// <param name="queueSnapshot">Current immutable queue authority.</param>
    /// <param name="activeWorkSnapshot">Current bounded active-work authority.</param>
    /// <param name="worldSnapshotDocument">Checksum-protected World Snapshot.</param>
    /// <param name="capturedTick">External monotonic checkpoint tick.</param>
    /// <param name="revision">Optimistic recovery checkpoint revision.</param>
    /// <returns>An explicit immutable checkpoint result.</returns>
    public static HostRuntimeRecoveryCheckpointResult<TRequest>
        CaptureCheckpoint<TRequest>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> checkpointId,
            HostRuntimeLifecycleSnapshot lifecycleSnapshot,
            HostRuntimeComposition composition,
            HostRuntimeQueueSnapshot queueSnapshot,
            HostRuntimeActiveWorkSnapshot<TRequest> activeWorkSnapshot,
            global::AI.Sandbox.Engine.Core.Persistence.WorldSnapshotDocument
                worldSnapshotDocument,
            long capturedTick,
            long revision)
        where TRequest : IHostRuntimeRequest
    {
        EnsureId(checkpointId.IsEmpty, nameof(checkpointId));
        ArgumentNullException.ThrowIfNull(lifecycleSnapshot);
        ArgumentNullException.ThrowIfNull(composition);
        ArgumentNullException.ThrowIfNull(queueSnapshot);
        ArgumentNullException.ThrowIfNull(activeWorkSnapshot);
        ArgumentNullException.ThrowIfNull(worldSnapshotDocument);

        if (capturedTick < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capturedTick));
        }
        if (revision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(revision));
        }

        if (lifecycleSnapshot.InstanceId !=
            activeWorkSnapshot.RuntimeInstanceId)
        {
            return CheckpointResult<TRequest>(
                HostRuntimeRecoveryStatus.RuntimeMismatch);
        }
        if (lifecycleSnapshot.CompositionId != composition.CompositionId)
        {
            return CheckpointResult<TRequest>(
                HostRuntimeRecoveryStatus.CompositionMismatch);
        }
        if (capturedTick < activeWorkSnapshot.ObservedTick)
        {
            return CheckpointResult<TRequest>(
                HostRuntimeRecoveryStatus.BeforeActiveWorkObservation);
        }
        if (worldSnapshotDocument.FormatVersion !=
            global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotFormatVersion.Current)
        {
            return CheckpointResult<TRequest>(
                HostRuntimeRecoveryStatus.UnsupportedWorldSnapshotFormat);
        }
        if (!worldSnapshotDocument.HasValidChecksum)
        {
            return CheckpointResult<TRequest>(
                HostRuntimeRecoveryStatus.WorldSnapshotChecksumMismatch);
        }

        var checkpoint = new HostRuntimeRecoveryCheckpoint<TRequest>(
            checkpointId,
            lifecycleSnapshot,
            composition,
            queueSnapshot,
            activeWorkSnapshot,
            worldSnapshotDocument,
            capturedTick,
            revision);

        return new HostRuntimeRecoveryCheckpointResult<TRequest>(
            HostRuntimeRecoveryStatus.CheckpointCreated,
            checkpoint);
    }

    /// <summary>
    /// Validates one restored World State snapshot against checkpoint authority.
    /// </summary>
    /// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
    /// <typeparam name="TState">Exact immutable World State root type.</typeparam>
    /// <param name="continuationId">Externally assigned continuation ID.</param>
    /// <param name="checkpoint">Existing immutable recovery checkpoint.</param>
    /// <param name="expectedCheckpointRevision">Expected checkpoint revision.</param>
    /// <param name="restoreResult">Result from an explicit persistence codec.</param>
    /// <param name="continuedTick">External monotonic continuation tick.</param>
    /// <returns>An explicit immutable continuation result.</returns>
    public static HostRuntimeRecoveryContinuationResult<TRequest, TState>
        Continue<TRequest, TState>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryContinuationIdKind> continuationId,
            HostRuntimeRecoveryCheckpoint<TRequest> checkpoint,
            long expectedCheckpointRevision,
            global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotRestoreResult<TState> restoreResult,
            long continuedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    {
        EnsureId(continuationId.IsEmpty, nameof(continuationId));
        ArgumentNullException.ThrowIfNull(checkpoint);
        ArgumentNullException.ThrowIfNull(restoreResult);

        if (expectedCheckpointRevision < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedCheckpointRevision));
        }
        if (continuedTick < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(continuedTick));
        }

        if (checkpoint.Revision != expectedCheckpointRevision)
        {
            return ContinuationResult<TRequest, TState>(
                HostRuntimeRecoveryStatus.StaleCheckpointRevision);
        }

        var restored = restoreResult.Snapshot;
        if (!restoreResult.WasRestored || restored is null)
        {
            return ContinuationResult<TRequest, TState>(
                HostRuntimeRecoveryStatus.RestoreFailed,
                restoreResult.Status);
        }

        var document = checkpoint.WorldSnapshotDocument;
        if (restored.WorldId != document.WorldId)
        {
            return ContinuationResult<TRequest, TState>(
                HostRuntimeRecoveryStatus.RestoredWorldMismatch);
        }
        if (restored.Version != document.WorldStateVersion)
        {
            return ContinuationResult<TRequest, TState>(
                HostRuntimeRecoveryStatus.RestoredWorldVersionMismatch);
        }
        if (restored.SimulationTick != document.SimulationTick)
        {
            return ContinuationResult<TRequest, TState>(
                HostRuntimeRecoveryStatus.RestoredSimulationTickMismatch);
        }
        if (continuedTick < checkpoint.CapturedTick)
        {
            return ContinuationResult<TRequest, TState>(
                HostRuntimeRecoveryStatus.ContinuationTickRegressed);
        }

        var continuation =
            new HostRuntimeRecoveryContinuation<TRequest, TState>(
                continuationId,
                checkpoint,
                restored,
                continuedTick,
                checked(checkpoint.Revision + 1));

        return new HostRuntimeRecoveryContinuationResult<TRequest, TState>(
            HostRuntimeRecoveryStatus.ContinuationCreated,
            continuation,
            restoreStatus: null);
    }

    private static HostRuntimeRecoveryCheckpointResult<TRequest>
        CheckpointResult<TRequest>(HostRuntimeRecoveryStatus status)
        where TRequest : IHostRuntimeRequest =>
        new(status, checkpoint: null);

    private static HostRuntimeRecoveryContinuationResult<TRequest, TState>
        ContinuationResult<TRequest, TState>(
            HostRuntimeRecoveryStatus status,
            global::AI.Sandbox.Engine.Core.Persistence.SnapshotRestoreStatus?
                restoreStatus = null)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState =>
        new(status, continuation: null, restoreStatus);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new ArgumentException(
                "The ID must be initialized.",
                parameterName);
        }
    }
}
