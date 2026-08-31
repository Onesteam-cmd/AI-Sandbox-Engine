namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Routes one in-flight completion and releases worker ownership as one pure
/// immutable attempt-settlement decision.
/// </summary>
public static class HostRuntimeAttemptSettlementFlow
{
    /// <summary>
    /// Settles one acknowledged attempt without executing work, receiving
    /// transport, or mutating stored authority.
    /// </summary>
    /// <typeparam name="TRequest">Exact request payload type.</typeparam>
    /// <typeparam name="TCompletion">Exact completion payload type.</typeparam>
    /// <param name="settlementId">
    /// Externally assigned non-empty settlement ID.
    /// </param>
    /// <param name="attempt">Acknowledged in-flight attempt authority.</param>
    /// <param name="request">Current immutable request authority.</param>
    /// <param name="lease">Current immutable lease authority.</param>
    /// <param name="expectedRequestRevision">
    /// Request revision observed by the settling caller.
    /// </param>
    /// <param name="expectedLeaseRevision">
    /// Lease revision observed by the settling caller.
    /// </param>
    /// <param name="settlingWorkerId">
    /// Externally named worker reporting settlement.
    /// </param>
    /// <param name="clockId">
    /// Matching externally owned monotonic clock domain.
    /// </param>
    /// <param name="settledTick">
    /// Non-negative external monotonic settlement tick.
    /// </param>
    /// <param name="completion">
    /// Immutable external completion reported for the attempt.
    /// </param>
    /// <returns>An explicit immutable attempt-settlement result.</returns>
    public static HostRuntimeAttemptSettlementResult<
        TRequest,
        TCompletion> Settle<TRequest, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeSettlementIdKind> settlementId,
            HostRuntimeInFlightAttempt<TRequest> attempt,
            HostRuntimeRequestEnvelope<TRequest> request,
            HostRuntimeWorkLease<TRequest> lease,
            long expectedRequestRevision,
            long expectedLeaseRevision,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeWorkerIdKind> settlingWorkerId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeClockIdKind> clockId,
            long settledTick,
            HostRuntimeCompletionEnvelope<TCompletion> completion)
        where TRequest : IHostRuntimeRequest
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(settlementId.IsEmpty, nameof(settlementId));
        ArgumentNullException.ThrowIfNull(attempt);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(lease);
        ArgumentNullException.ThrowIfNull(completion);

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

        EnsureId(settlingWorkerId.IsEmpty, nameof(settlingWorkerId));
        EnsureId(clockId.IsEmpty, nameof(clockId));
        if (settledTick < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(settledTick));
        }

        if (request.Revision != expectedRequestRevision)
        {
            return Unchanged(
                HostRuntimeAttemptSettlementStatus.StaleRequestRevision,
                request,
                lease,
                completion);
        }
        if (lease.Revision != expectedLeaseRevision)
        {
            return Unchanged(
                HostRuntimeAttemptSettlementStatus.StaleLeaseRevision,
                request,
                lease,
                completion);
        }
        if (request.RequestId != attempt.RequestId ||
            lease.RequestId != attempt.RequestId)
        {
            return Unchanged(
                HostRuntimeAttemptSettlementStatus.AttemptRequestMismatch,
                request,
                lease,
                completion);
        }
        if (lease.LeaseId != attempt.LeaseId)
        {
            return Unchanged(
                HostRuntimeAttemptSettlementStatus.AttemptLeaseMismatch,
                request,
                lease,
                completion);
        }
        if (request.State is not (
            HostRuntimeRequestState.Pending or
            HostRuntimeRequestState.CancellationRequested))
        {
            return Unchanged(
                HostRuntimeAttemptSettlementStatus.InvalidRequestState,
                request,
                lease,
                completion);
        }
        if (!lease.IsActive)
        {
            return Unchanged(
                HostRuntimeAttemptSettlementStatus.InvalidLeaseState,
                request,
                lease,
                completion);
        }
        if (lease.WorkerId != attempt.WorkerId ||
            settlingWorkerId != attempt.WorkerId)
        {
            return Unchanged(
                HostRuntimeAttemptSettlementStatus.WorkerMismatch,
                request,
                lease,
                completion);
        }
        if (lease.ClockId != attempt.ClockId ||
            clockId != attempt.ClockId)
        {
            return Unchanged(
                HostRuntimeAttemptSettlementStatus.ClockMismatch,
                request,
                lease,
                completion);
        }
        if (settledTick < attempt.AcknowledgedTick)
        {
            return Unchanged(
                HostRuntimeAttemptSettlementStatus.BeforeAcknowledgement,
                request,
                lease,
                completion);
        }
        if (settledTick >= lease.ExpiresTick)
        {
            return Unchanged(
                HostRuntimeAttemptSettlementStatus.LeaseExpired,
                request,
                lease,
                completion);
        }

        var routing = HostRuntimeCompletionRouter.Route(
            attempt.Dispatch,
            request,
            expectedRequestRevision,
            completion);
        if (!routing.Succeeded)
        {
            var status = routing.Status switch
            {
                HostRuntimeCompletionRoutingStatus.InvalidRequestState =>
                    HostRuntimeAttemptSettlementStatus.InvalidRequestState,
                HostRuntimeCompletionRoutingStatus.DispatchMismatch or
                HostRuntimeCompletionRoutingStatus.CompletionMismatch =>
                    HostRuntimeAttemptSettlementStatus.CompletionMismatch,
                _ =>
                    HostRuntimeAttemptSettlementStatus
                        .RequestTransitionRejected,
            };

            return Unchanged(
                status,
                request,
                lease,
                completion);
        }

        var release = HostRuntimeWorkLeaseFlow.Release(
            lease,
            expectedLeaseRevision,
            settlingWorkerId);
        if (!release.Succeeded)
        {
            return Unchanged(
                HostRuntimeAttemptSettlementStatus.LeaseTransitionRejected,
                request,
                lease,
                completion);
        }

        var settlement =
            new HostRuntimeAttemptSettlement<TRequest, TCompletion>(
                settlementId,
                attempt,
                completion,
                routing.Request,
                release.Lease,
                settledTick);

        return new HostRuntimeAttemptSettlementResult<
            TRequest,
            TCompletion>(
                HostRuntimeAttemptSettlementStatus.Settled,
                settlement,
                routing.Request,
                release.Lease,
                completion);
    }

    private static HostRuntimeAttemptSettlementResult<
        TRequest,
        TCompletion> Unchanged<TRequest, TCompletion>(
            HostRuntimeAttemptSettlementStatus status,
            HostRuntimeRequestEnvelope<TRequest> request,
            HostRuntimeWorkLease<TRequest> lease,
            HostRuntimeCompletionEnvelope<TCompletion> completion)
        where TRequest : IHostRuntimeRequest
        where TCompletion : IHostRuntimeCompletion =>
        new(
            status,
            null,
            request,
            lease,
            completion);

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
