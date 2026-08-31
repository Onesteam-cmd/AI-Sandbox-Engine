namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable Host recovery checkpoint assembled from existing
/// validated authority snapshots.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
public sealed record HostRuntimeRecoveryCheckpoint<TRequest>
    where TRequest : IHostRuntimeRequest
{
    internal HostRuntimeRecoveryCheckpoint(
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
    {
        CheckpointId = checkpointId;
        LifecycleSnapshot = lifecycleSnapshot;
        Composition = composition;
        QueueSnapshot = queueSnapshot;
        ActiveWorkSnapshot = activeWorkSnapshot;
        WorldSnapshotDocument = worldSnapshotDocument;
        CapturedTick = capturedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned checkpoint ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> CheckpointId { get; }

    /// <summary>Gets unchanged lifecycle authority at checkpoint capture.</summary>
    public HostRuntimeLifecycleSnapshot LifecycleSnapshot { get; }

    /// <summary>Gets unchanged deterministic composition authority.</summary>
    public HostRuntimeComposition Composition { get; }

    /// <summary>Gets unchanged queue authority at checkpoint capture.</summary>
    public HostRuntimeQueueSnapshot QueueSnapshot { get; }

    /// <summary>Gets unchanged bounded active-work authority.</summary>
    public HostRuntimeActiveWorkSnapshot<TRequest> ActiveWorkSnapshot { get; }

    /// <summary>Gets the checksum-protected World Snapshot document.</summary>
    public global::AI.Sandbox.Engine.Core.Persistence.WorldSnapshotDocument
        WorldSnapshotDocument { get; }

    /// <summary>Gets the external monotonic checkpoint tick.</summary>
    public long CapturedTick { get; }

    /// <summary>Gets the optimistic recovery checkpoint revision.</summary>
    public long Revision { get; }

    /// <summary>Gets the represented logical runtime instance ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeInstanceIdKind> RuntimeInstanceId =>
        LifecycleSnapshot.InstanceId;

    /// <summary>Gets the represented monotonic Host clock domain.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId => ActiveWorkSnapshot.ClockId;
}
