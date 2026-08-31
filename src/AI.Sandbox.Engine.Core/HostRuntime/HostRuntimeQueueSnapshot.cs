namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Contains immutable optimistic Host queue admission authority.</summary>
public sealed record HostRuntimeQueueSnapshot
{
    internal HostRuntimeQueueSnapshot(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeQueueIdKind> queueId,
        HostRuntimeQueueCapacity capacity,
        int queuedCount,
        long revision)
    {
        QueueId = queueId;
        Capacity = capacity;
        QueuedCount = queuedCount;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned queue ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeQueueIdKind> QueueId { get; }

    /// <summary>Gets immutable queue capacity.</summary>
    public HostRuntimeQueueCapacity Capacity { get; }

    /// <summary>Gets the represented queued request count.</summary>
    public int QueuedCount { get; }

    /// <summary>Gets the optimistic queue authority revision.</summary>
    public long Revision { get; }

    /// <summary>Gets whether the represented queue has no free admission slot.</summary>
    public bool IsFull => QueuedCount >= Capacity.MaxQueuedRequests;

    /// <summary>Creates one validated immutable queue snapshot.</summary>
    public static HostRuntimeQueueSnapshot Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeQueueIdKind> queueId,
        HostRuntimeQueueCapacity capacity,
        int queuedCount,
        long revision)
    {
        if (queueId.IsEmpty)
        {
            throw new ArgumentException(
                "The queue ID must be initialized.",
                nameof(queueId));
        }
        ArgumentNullException.ThrowIfNull(capacity);
        if (queuedCount < 0 ||
            queuedCount > capacity.MaxQueuedRequests)
        {
            throw new ArgumentOutOfRangeException(nameof(queuedCount));
        }
        if (revision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(revision));
        }

        return new HostRuntimeQueueSnapshot(
            queueId,
            capacity,
            queuedCount,
            revision);
    }
}
