namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Contains one immutable Host queue-admission record.</summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeQueueAdmission<TRequest>
    where TRequest : IHostRuntimeRequest
{
    internal HostRuntimeQueueAdmission(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeAdmissionIdKind> admissionId,
        HostRuntimeQueueSnapshot snapshot,
        HostRuntimeRequestEnvelope<TRequest> request,
        HostRuntimePriority priority,
        long observedQueueRevision)
    {
        AdmissionId = admissionId;
        Snapshot = snapshot;
        Request = request;
        Priority = priority;
        ObservedQueueRevision = observedQueueRevision;
    }

    /// <summary>Gets the externally assigned admission ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAdmissionIdKind> AdmissionId { get; }

    /// <summary>Gets resulting immutable queue authority.</summary>
    public HostRuntimeQueueSnapshot Snapshot { get; }

    /// <summary>Gets unchanged immutable request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request { get; }

    /// <summary>Gets the deterministic immutable priority key.</summary>
    public HostRuntimePriority Priority { get; }

    /// <summary>Gets the queue revision observed before admission.</summary>
    public long ObservedQueueRevision { get; }

    /// <summary>Gets the stable queue ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeQueueIdKind> QueueId => Snapshot.QueueId;

    /// <summary>Gets the stable request ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRequestIdKind> RequestId => Request.RequestId;
}
