namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host abandoned-attempt disposition result.
/// </summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeAbandonedAttemptDispositionResult<TRequest>
    where TRequest : IHostRuntimeRequest
{
    internal HostRuntimeAbandonedAttemptDispositionResult(
        HostRuntimeAbandonedAttemptDispositionStatus status,
        HostRuntimeAbandonedAttemptDisposition<TRequest>? disposition,
        HostRuntimeRequestEnvelope<TRequest> request,
        HostRuntimeWorkLease<TRequest> lease)
    {
        Status = status;
        Disposition = disposition;
        Request = request;
        Lease = lease;
    }

    /// <summary>Gets the explicit disposition outcome.</summary>
    public HostRuntimeAbandonedAttemptDispositionStatus Status { get; }

    /// <summary>
    /// Gets abandoned-attempt authority when disposition succeeded.
    /// </summary>
    public HostRuntimeAbandonedAttemptDisposition<TRequest>? Disposition { get; }

    /// <summary>Gets resulting or unchanged request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request { get; }

    /// <summary>Gets resulting or unchanged lease authority.</summary>
    public HostRuntimeWorkLease<TRequest> Lease { get; }

    /// <summary>Gets whether disposition authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeAbandonedAttemptDispositionStatus.Disposed;
}
