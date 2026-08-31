namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable Host work-lease transition result.</summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeLeaseTransitionResult<TRequest>
    where TRequest : IHostRuntimeRequest
{
    internal HostRuntimeLeaseTransitionResult(
        HostRuntimeLeaseTransitionStatus status,
        HostRuntimeWorkLease<TRequest> lease)
    {
        Status = status;
        Lease = lease;
    }

    /// <summary>Gets the explicit lease transition outcome.</summary>
    public HostRuntimeLeaseTransitionStatus Status { get; }

    /// <summary>Gets the resulting or unchanged immutable lease authority.</summary>
    public HostRuntimeWorkLease<TRequest> Lease { get; }

    /// <summary>Gets whether the requested transition changed lease authority.</summary>
    public bool Succeeded => Status is
        HostRuntimeLeaseTransitionStatus.Renewed or
        HostRuntimeLeaseTransitionStatus.Released or
        HostRuntimeLeaseTransitionStatus.Expired;
}
