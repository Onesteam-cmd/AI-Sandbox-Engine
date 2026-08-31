namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority for one selected dequeue and advisory
/// dispatch.
/// </summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeDispatchSelection<TRequest>
    where TRequest : IHostRuntimeRequest
{
    internal HostRuntimeDispatchSelection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeDispatchSelectionIdKind> selectionId,
        HostRuntimeQueueSnapshot snapshot,
        HostRuntimeWorkLease<TRequest> lease,
        HostRuntimeDispatchEnvelope<TRequest> dispatch,
        long observedQueueRevision)
    {
        SelectionId = selectionId;
        Snapshot = snapshot;
        Lease = lease;
        Dispatch = dispatch;
        ObservedQueueRevision = observedQueueRevision;
    }

    /// <summary>
    /// Gets the externally assigned selection ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeDispatchSelectionIdKind> SelectionId { get; }

    /// <summary>
    /// Gets the resulting immutable queue authority after dequeue.
    /// </summary>
    public HostRuntimeQueueSnapshot Snapshot { get; }

    /// <summary>
    /// Gets the unchanged active worker-ownership lease.
    /// </summary>
    public HostRuntimeWorkLease<TRequest> Lease { get; }

    /// <summary>
    /// Gets the immutable advisory dispatch record.
    /// </summary>
    public HostRuntimeDispatchEnvelope<TRequest> Dispatch { get; }

    /// <summary>
    /// Gets the queue revision observed before dequeue.
    /// </summary>
    public long ObservedQueueRevision { get; }

    /// <summary>Gets the stable queue ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeQueueIdKind> QueueId => Snapshot.QueueId;

    /// <summary>Gets the stable request ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRequestIdKind> RequestId => Lease.RequestId;

    /// <summary>Gets the stable lease ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeLeaseIdKind> LeaseId => Lease.LeaseId;

    /// <summary>Gets the stable worker ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeWorkerIdKind> WorkerId => Lease.WorkerId;

    /// <summary>Gets the preserved deterministic priority key.</summary>
    public HostRuntimePriority Priority =>
        Lease.Admission.Priority;
}
