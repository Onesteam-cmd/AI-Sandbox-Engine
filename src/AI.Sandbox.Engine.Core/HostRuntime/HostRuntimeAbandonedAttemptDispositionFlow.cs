namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Disposes one acknowledged attempt after cancellation or proven lease expiry.
/// </summary>
public static class HostRuntimeAbandonedAttemptDispositionFlow
{
    /// <summary>
    /// Produces immutable abandoned-attempt authority without interrupting work,
    /// observing a wall clock, or mutating stored authority.
    /// </summary>
    /// <typeparam name="TRequest">Exact request payload type.</typeparam>
    /// <param name="dispositionId">
    /// Externally assigned non-empty disposition ID.
    /// </param>
    /// <param name="kind">Exact abandonment reason.</param>
    /// <param name="attempt">Acknowledged in-flight attempt authority.</param>
    /// <param name="request">Current immutable request authority.</param>
    /// <param name="lease">Current immutable lease authority.</param>
    /// <param name="expectedRequestRevision">
    /// Request revision observed by the caller.
    /// </param>
    /// <param name="expectedLeaseRevision">
    /// Lease revision observed by the caller.
    /// </param>
    /// <param name="clockId">
    /// Matching externally owned monotonic clock domain.
    /// </param>
    /// <param name="observedTick">
    /// Non-negative external monotonic disposition tick.
    /// </param>
    /// <returns>An explicit immutable disposition result.</returns>
    public static HostRuntimeAbandonedAttemptDispositionResult<TRequest>
        Dispose<TRequest>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeAbandonedAttemptDispositionIdKind> dispositionId,
            HostRuntimeAbandonedAttemptDispositionKind kind,
            HostRuntimeInFlightAttempt<TRequest> attempt,
            HostRuntimeRequestEnvelope<TRequest> request,
            HostRuntimeWorkLease<TRequest> lease,
            long expectedRequestRevision,
            long expectedLeaseRevision,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeClockIdKind> clockId,
            long observedTick)
        where TRequest : IHostRuntimeRequest
    {
        EnsureId(dispositionId.IsEmpty, nameof(dispositionId));
        if (!Enum.IsDefined(kind))
        {
            throw new ArgumentOutOfRangeException(nameof(kind));
        }

        ArgumentNullException.ThrowIfNull(attempt);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(lease);

        if (expectedRequestRevision < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedRequestRevision));
        }
        if (expectedLeaseRevision < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedLeaseRevision));
        }

        EnsureId(clockId.IsEmpty, nameof(clockId));
        if (observedTick < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(observedTick));
        }

        if (request.Revision != expectedRequestRevision)
        {
            return Unchanged(
                HostRuntimeAbandonedAttemptDispositionStatus
                    .StaleRequestRevision,
                request,
                lease);
        }
        if (lease.Revision != expectedLeaseRevision)
        {
            return Unchanged(
                HostRuntimeAbandonedAttemptDispositionStatus
                    .StaleLeaseRevision,
                request,
                lease);
        }
        if (request.RequestId != attempt.RequestId ||
            lease.RequestId != attempt.RequestId)
        {
            return Unchanged(
                HostRuntimeAbandonedAttemptDispositionStatus
                    .AttemptRequestMismatch,
                request,
                lease);
        }
        if (lease.LeaseId != attempt.LeaseId)
        {
            return Unchanged(
                HostRuntimeAbandonedAttemptDispositionStatus
                    .AttemptLeaseMismatch,
                request,
                lease);
        }
        if (lease.WorkerId != attempt.WorkerId)
        {
            return Unchanged(
                HostRuntimeAbandonedAttemptDispositionStatus
                    .AttemptWorkerMismatch,
                request,
                lease);
        }
        if (request.State is not (
            HostRuntimeRequestState.Pending or
            HostRuntimeRequestState.CancellationRequested))
        {
            return Unchanged(
                HostRuntimeAbandonedAttemptDispositionStatus
                    .InvalidRequestState,
                request,
                lease);
        }
        if (!lease.IsActive)
        {
            return Unchanged(
                HostRuntimeAbandonedAttemptDispositionStatus
                    .InvalidLeaseState,
                request,
                lease);
        }
        if (lease.ClockId != attempt.ClockId ||
            clockId != attempt.ClockId)
        {
            return Unchanged(
                HostRuntimeAbandonedAttemptDispositionStatus.ClockMismatch,
                request,
                lease);
        }
        if (observedTick < attempt.AcknowledgedTick)
        {
            return Unchanged(
                HostRuntimeAbandonedAttemptDispositionStatus
                    .BeforeAcknowledgement,
                request,
                lease);
        }

        HostRuntimeRequestTransitionResult<TRequest> requestTransition;
        HostRuntimeLeaseTransitionResult<TRequest> leaseTransition;

        switch (kind)
        {
            case HostRuntimeAbandonedAttemptDispositionKind
                .CancellationRequested:
            {
                if (request.State !=
                    HostRuntimeRequestState.CancellationRequested)
                {
                    return Unchanged(
                        HostRuntimeAbandonedAttemptDispositionStatus
                            .CancellationNotRequested,
                        request,
                        lease);
                }
                if (observedTick >= lease.ExpiresTick)
                {
                    return Unchanged(
                        HostRuntimeAbandonedAttemptDispositionStatus
                            .LeaseAlreadyExpired,
                        request,
                        lease);
                }

                requestTransition = HostRuntimeRequestFlow.Finalize(
                    request,
                    expectedRequestRevision,
                    HostRuntimeRequestState.Cancelled);
                leaseTransition = HostRuntimeWorkLeaseFlow.Release(
                    lease,
                    expectedLeaseRevision,
                    attempt.WorkerId);
                break;
            }

            case HostRuntimeAbandonedAttemptDispositionKind.LeaseExpired:
            {
                if (observedTick < lease.ExpiresTick)
                {
                    return Unchanged(
                        HostRuntimeAbandonedAttemptDispositionStatus
                            .LeaseNotExpired,
                        request,
                        lease);
                }

                var terminalState =
                    request.State ==
                        HostRuntimeRequestState.CancellationRequested
                    ? HostRuntimeRequestState.Cancelled
                    : HostRuntimeRequestState.Failed;
                requestTransition = HostRuntimeRequestFlow.Finalize(
                    request,
                    expectedRequestRevision,
                    terminalState);
                leaseTransition = HostRuntimeWorkLeaseFlow.Expire(
                    lease,
                    expectedLeaseRevision,
                    clockId,
                    observedTick);
                break;
            }

            default:
                throw new ArgumentOutOfRangeException(nameof(kind));
        }

        if (!requestTransition.Succeeded)
        {
            return Unchanged(
                HostRuntimeAbandonedAttemptDispositionStatus
                    .RequestTransitionRejected,
                request,
                lease);
        }
        if (!leaseTransition.Succeeded)
        {
            return Unchanged(
                HostRuntimeAbandonedAttemptDispositionStatus
                    .LeaseTransitionRejected,
                request,
                lease);
        }

        var disposition =
            new HostRuntimeAbandonedAttemptDisposition<TRequest>(
                dispositionId,
                kind,
                attempt,
                requestTransition.Envelope,
                leaseTransition.Lease,
                observedTick);

        return new HostRuntimeAbandonedAttemptDispositionResult<TRequest>(
            HostRuntimeAbandonedAttemptDispositionStatus.Disposed,
            disposition,
            requestTransition.Envelope,
            leaseTransition.Lease);
    }

    private static HostRuntimeAbandonedAttemptDispositionResult<TRequest>
        Unchanged<TRequest>(
            HostRuntimeAbandonedAttemptDispositionStatus status,
            HostRuntimeRequestEnvelope<TRequest> request,
            HostRuntimeWorkLease<TRequest> lease)
        where TRequest : IHostRuntimeRequest =>
        new(status, null, request, lease);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new ArgumentException(
                "The identifier must be initialized.",
                parameterName);
        }
    }
}
