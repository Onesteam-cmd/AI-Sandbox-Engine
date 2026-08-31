namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Groups one acknowledged attempt with current request and lease authority.
/// </summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeActiveWorkItem<TRequest>
    where TRequest : IHostRuntimeRequest
{
    private HostRuntimeActiveWorkItem(
        HostRuntimeInFlightAttempt<TRequest> attempt,
        HostRuntimeRequestEnvelope<TRequest> request,
        HostRuntimeWorkLease<TRequest> lease)
    {
        Attempt = attempt;
        Request = request;
        Lease = lease;
    }

    /// <summary>Gets the unchanged acknowledged attempt authority.</summary>
    public HostRuntimeInFlightAttempt<TRequest> Attempt { get; }

    /// <summary>Gets current immutable request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request { get; }

    /// <summary>Gets current immutable lease authority.</summary>
    public HostRuntimeWorkLease<TRequest> Lease { get; }

    /// <summary>Gets the stable attempt ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind> AttemptId => Attempt.AttemptId;

    /// <summary>Gets the stable request ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRequestIdKind> RequestId => Request.RequestId;

    /// <summary>Gets the stable lease ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeLeaseIdKind> LeaseId => Lease.LeaseId;

    /// <summary>Gets the stable worker ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeWorkerIdKind> WorkerId => Lease.WorkerId;

    /// <summary>Gets the stable dispatch ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeDispatchIdKind> DispatchId => Attempt.DispatchId;

    /// <summary>Gets the dispatch attempt number.</summary>
    public int AttemptNumber => Attempt.AttemptNumber;

    /// <summary>Gets the monotonic clock domain.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId => Lease.ClockId;

    /// <summary>
    /// Creates one unvalidated active-work observation for snapshot capture.
    /// </summary>
    /// <param name="attempt">Acknowledged attempt authority.</param>
    /// <param name="request">Current request authority.</param>
    /// <param name="lease">Current lease authority.</param>
    /// <returns>An immutable grouped active-work observation.</returns>
    public static HostRuntimeActiveWorkItem<TRequest> Create(
        HostRuntimeInFlightAttempt<TRequest> attempt,
        HostRuntimeRequestEnvelope<TRequest> request,
        HostRuntimeWorkLease<TRequest> lease)
    {
        ArgumentNullException.ThrowIfNull(attempt);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(lease);

        return new HostRuntimeActiveWorkItem<TRequest>(
            attempt,
            request,
            lease);
    }
}
