namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Produces pure immutable worker-ownership lease authority.</summary>
public static class HostRuntimeWorkLeaseFlow
{
    /// <summary>Gets the maximum supported lease duration in external ticks.</summary>
    public const long MaximumLeaseDurationTicks = 1_000_000_000;

    /// <summary>Acquires immutable worker ownership of one admitted request.</summary>
    /// <typeparam name="TRequest">Exact request payload type.</typeparam>
    /// <param name="leaseId">Externally assigned non-empty lease ID.</param>
    /// <param name="workerId">Externally assigned non-empty worker ID.</param>
    /// <param name="admission">Immutable queue admission authority.</param>
    /// <param name="clockId">Externally owned monotonic clock domain.</param>
    /// <param name="acquiredTick">Non-negative external acquisition tick.</param>
    /// <param name="durationTicks">Positive bounded lease duration.</param>
    /// <returns>New active immutable lease authority.</returns>
    public static HostRuntimeWorkLease<TRequest> Acquire<TRequest>(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<HostRuntimeLeaseIdKind> leaseId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<HostRuntimeWorkerIdKind> workerId,
        HostRuntimeQueueAdmission<TRequest> admission,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<HostRuntimeClockIdKind> clockId,
        long acquiredTick,
        long durationTicks)
        where TRequest : IHostRuntimeRequest
    {
        EnsureId(leaseId.IsEmpty, nameof(leaseId));
        EnsureId(workerId.IsEmpty, nameof(workerId));
        EnsureId(clockId.IsEmpty, nameof(clockId));
        ArgumentNullException.ThrowIfNull(admission);
        EnsureTick(acquiredTick, nameof(acquiredTick));
        EnsureDuration(durationTicks, nameof(durationTicks));

        return new HostRuntimeWorkLease<TRequest>(
            leaseId,
            workerId,
            admission,
            clockId,
            acquiredTick,
            checked(acquiredTick + durationTicks),
            HostRuntimeLeaseState.Active,
            revision: 0);
    }

    /// <summary>Renews one active lease using external monotonic time.</summary>
    /// <typeparam name="TRequest">Exact request payload type.</typeparam>
    /// <param name="lease">Current immutable lease authority.</param>
    /// <param name="expectedRevision">Lease revision observed by the caller.</param>
    /// <param name="clockId">Matching externally owned clock domain.</param>
    /// <param name="observedTick">Current non-negative external tick.</param>
    /// <param name="durationTicks">Positive bounded renewal duration.</param>
    /// <returns>An explicit immutable renewal result.</returns>
    public static HostRuntimeLeaseTransitionResult<TRequest> Renew<TRequest>(
        HostRuntimeWorkLease<TRequest> lease,
        long expectedRevision,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<HostRuntimeClockIdKind> clockId,
        long observedTick,
        long durationTicks)
        where TRequest : IHostRuntimeRequest
    {
        ArgumentNullException.ThrowIfNull(lease);
        EnsureRevision(expectedRevision);
        EnsureId(clockId.IsEmpty, nameof(clockId));
        EnsureTick(observedTick, nameof(observedTick));
        EnsureDuration(durationTicks, nameof(durationTicks));

        var guard = Guard(lease, expectedRevision, clockId);
        if (guard is not null)
        {
            return guard;
        }
        if (observedTick >= lease.ExpiresTick)
        {
            return Unchanged(HostRuntimeLeaseTransitionStatus.InvalidState, lease);
        }

        return new HostRuntimeLeaseTransitionResult<TRequest>(
            HostRuntimeLeaseTransitionStatus.Renewed,
            new HostRuntimeWorkLease<TRequest>(
                lease.LeaseId,
                lease.WorkerId,
                lease.Admission,
                lease.ClockId,
                lease.AcquiredTick,
                checked(observedTick + durationTicks),
                HostRuntimeLeaseState.Active,
                checked(lease.Revision + 1)));
    }

    /// <summary>Releases one active lease for its exact named worker.</summary>
    /// <typeparam name="TRequest">Exact request payload type.</typeparam>
    /// <param name="lease">Current immutable lease authority.</param>
    /// <param name="expectedRevision">Lease revision observed by the caller.</param>
    /// <param name="workerId">Externally assigned worker claiming release.</param>
    /// <returns>An explicit immutable release result.</returns>
    public static HostRuntimeLeaseTransitionResult<TRequest> Release<TRequest>(
        HostRuntimeWorkLease<TRequest> lease,
        long expectedRevision,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<HostRuntimeWorkerIdKind> workerId)
        where TRequest : IHostRuntimeRequest
    {
        ArgumentNullException.ThrowIfNull(lease);
        EnsureRevision(expectedRevision);
        EnsureId(workerId.IsEmpty, nameof(workerId));

        if (lease.Revision != expectedRevision)
        {
            return Unchanged(HostRuntimeLeaseTransitionStatus.StaleRevision, lease);
        }
        if (!lease.IsActive)
        {
            return Unchanged(HostRuntimeLeaseTransitionStatus.InvalidState, lease);
        }
        if (lease.WorkerId != workerId)
        {
            return Unchanged(HostRuntimeLeaseTransitionStatus.WorkerMismatch, lease);
        }

        return Transition(
            HostRuntimeLeaseTransitionStatus.Released,
            lease,
            HostRuntimeLeaseState.Released);
    }

    /// <summary>Expires one active lease when external time reaches its boundary.</summary>
    /// <typeparam name="TRequest">Exact request payload type.</typeparam>
    /// <param name="lease">Current immutable lease authority.</param>
    /// <param name="expectedRevision">Lease revision observed by the caller.</param>
    /// <param name="clockId">Matching externally owned clock domain.</param>
    /// <param name="observedTick">Current non-negative external tick.</param>
    /// <returns>An explicit immutable expiry result.</returns>
    public static HostRuntimeLeaseTransitionResult<TRequest> Expire<TRequest>(
        HostRuntimeWorkLease<TRequest> lease,
        long expectedRevision,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<HostRuntimeClockIdKind> clockId,
        long observedTick)
        where TRequest : IHostRuntimeRequest
    {
        ArgumentNullException.ThrowIfNull(lease);
        EnsureRevision(expectedRevision);
        EnsureId(clockId.IsEmpty, nameof(clockId));
        EnsureTick(observedTick, nameof(observedTick));

        var guard = Guard(lease, expectedRevision, clockId);
        if (guard is not null)
        {
            return guard;
        }
        if (observedTick < lease.ExpiresTick)
        {
            return Unchanged(HostRuntimeLeaseTransitionStatus.NotExpired, lease);
        }

        return Transition(
            HostRuntimeLeaseTransitionStatus.Expired,
            lease,
            HostRuntimeLeaseState.Expired);
    }

    private static HostRuntimeLeaseTransitionResult<TRequest>? Guard<TRequest>(
        HostRuntimeWorkLease<TRequest> lease,
        long expectedRevision,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<HostRuntimeClockIdKind> clockId)
        where TRequest : IHostRuntimeRequest
    {
        if (lease.Revision != expectedRevision)
        {
            return Unchanged(HostRuntimeLeaseTransitionStatus.StaleRevision, lease);
        }
        if (!lease.IsActive)
        {
            return Unchanged(HostRuntimeLeaseTransitionStatus.InvalidState, lease);
        }
        if (lease.ClockId != clockId)
        {
            return Unchanged(HostRuntimeLeaseTransitionStatus.ClockMismatch, lease);
        }
        return null;
    }

    private static HostRuntimeLeaseTransitionResult<TRequest> Transition<TRequest>(
        HostRuntimeLeaseTransitionStatus status,
        HostRuntimeWorkLease<TRequest> lease,
        HostRuntimeLeaseState state)
        where TRequest : IHostRuntimeRequest =>
        new(
            status,
            new HostRuntimeWorkLease<TRequest>(
                lease.LeaseId,
                lease.WorkerId,
                lease.Admission,
                lease.ClockId,
                lease.AcquiredTick,
                lease.ExpiresTick,
                state,
                checked(lease.Revision + 1)));

    private static HostRuntimeLeaseTransitionResult<TRequest> Unchanged<TRequest>(
        HostRuntimeLeaseTransitionStatus status,
        HostRuntimeWorkLease<TRequest> lease)
        where TRequest : IHostRuntimeRequest => new(status, lease);

    private static void EnsureId(bool empty, string parameterName)
    {
        if (empty)
        {
            throw new ArgumentException("The identifier must be initialized.", parameterName);
        }
    }

    private static void EnsureRevision(long revision)
    {
        if (revision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(revision));
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }

    private static void EnsureDuration(long duration, string parameterName)
    {
        if (duration < 1 || duration > MaximumLeaseDurationTicks)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}
