namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable Host queue-admission result.</summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeQueueAdmissionResult<TRequest>
    where TRequest : IHostRuntimeRequest
{
    internal HostRuntimeQueueAdmissionResult(
        HostRuntimeQueueAdmissionStatus status,
        HostRuntimeQueueSnapshot snapshot,
        HostRuntimeQueueAdmission<TRequest>? admission)
    {
        Status = status;
        Snapshot = snapshot;
        Admission = admission;
    }

    /// <summary>Gets the explicit admission outcome.</summary>
    public HostRuntimeQueueAdmissionStatus Status { get; }

    /// <summary>Gets resulting or unchanged immutable queue authority.</summary>
    public HostRuntimeQueueSnapshot Snapshot { get; }

    /// <summary>Gets admission authority when successful.</summary>
    public HostRuntimeQueueAdmission<TRequest>? Admission { get; }

    /// <summary>Gets whether admission produced new queue authority.</summary>
    public bool Succeeded =>
        Status == HostRuntimeQueueAdmissionStatus.Admitted;
}
