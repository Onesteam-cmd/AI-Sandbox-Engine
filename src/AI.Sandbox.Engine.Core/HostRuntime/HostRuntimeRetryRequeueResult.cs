namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable Host retry requeue result.</summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
/// <typeparam name="TCompletion">Exact completion payload type.</typeparam>
public sealed record HostRuntimeRetryRequeueResult<TRequest, TCompletion>
    where TRequest : IHostRuntimeRequest
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRetryRequeueResult(
        HostRuntimeRetryRequeueStatus status,
        HostRuntimeRetryRequeue<TRequest, TCompletion>? requeue,
        HostRuntimeRequestEnvelope<TRequest> request,
        HostRuntimeQueueSnapshot snapshot)
    {
        Status = status;
        Requeue = requeue;
        Request = request;
        Snapshot = snapshot;
    }

    /// <summary>Gets the explicit retry requeue outcome.</summary>
    public HostRuntimeRetryRequeueStatus Status { get; }

    /// <summary>Gets retry requeue authority when successful.</summary>
    public HostRuntimeRetryRequeue<TRequest, TCompletion>? Requeue { get; }

    /// <summary>Gets resulting or unchanged request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request { get; }

    /// <summary>Gets resulting or unchanged queue authority.</summary>
    public HostRuntimeQueueSnapshot Snapshot { get; }

    /// <summary>Gets queue admission authority when successful.</summary>
    public HostRuntimeQueueAdmission<TRequest>? Admission =>
        Requeue?.Admission;

    /// <summary>Gets whether retry requeue authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRetryRequeueStatus.Requeued;
}
