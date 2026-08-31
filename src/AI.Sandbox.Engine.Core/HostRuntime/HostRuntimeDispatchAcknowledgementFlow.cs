namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Validates an explicit worker dispatch acknowledgement and creates
/// immutable in-flight attempt authority.
/// </summary>
public static class HostRuntimeDispatchAcknowledgementFlow
{
    /// <summary>
    /// Acknowledges one selected dispatch without executing or transporting it.
    /// </summary>
    /// <typeparam name="TRequest">Exact request payload type.</typeparam>
    /// <param name="attemptId">
    /// Externally assigned non-empty in-flight attempt ID.
    /// </param>
    /// <param name="selection">
    /// Immutable dequeue, original lease, and dispatch selection authority.
    /// </param>
    /// <param name="request">
    /// Current immutable request authority observed at acknowledgement.
    /// </param>
    /// <param name="lease">
    /// Current immutable lease authority observed at acknowledgement.
    /// </param>
    /// <param name="expectedRequestRevision">
    /// Request revision observed by the acknowledging caller.
    /// </param>
    /// <param name="expectedLeaseRevision">
    /// Lease revision observed by the acknowledging caller.
    /// </param>
    /// <param name="acknowledgedLeaseId">
    /// Lease ID carried by the acknowledgement.
    /// </param>
    /// <param name="acknowledgedWorkerId">
    /// Worker ID carried by the acknowledgement.
    /// </param>
    /// <param name="acknowledgedDispatchId">
    /// Dispatch ID carried by the acknowledgement.
    /// </param>
    /// <param name="acknowledgedRequestId">
    /// Request ID carried by the acknowledgement.
    /// </param>
    /// <param name="acknowledgedAttemptNumber">
    /// Attempt number carried by the acknowledgement.
    /// </param>
    /// <param name="clockId">
    /// Matching externally owned monotonic clock domain.
    /// </param>
    /// <param name="acknowledgedTick">
    /// Non-negative external monotonic acknowledgement tick.
    /// </param>
    /// <returns>An explicit immutable acknowledgement result.</returns>
    public static HostRuntimeDispatchAcknowledgementResult<TRequest>
        Acknowledge<TRequest>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeAttemptIdKind> attemptId,
            HostRuntimeDispatchSelection<TRequest> selection,
            HostRuntimeRequestEnvelope<TRequest> request,
            HostRuntimeWorkLease<TRequest> lease,
            long expectedRequestRevision,
            long expectedLeaseRevision,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeLeaseIdKind> acknowledgedLeaseId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeWorkerIdKind> acknowledgedWorkerId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeDispatchIdKind> acknowledgedDispatchId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRequestIdKind> acknowledgedRequestId,
            int acknowledgedAttemptNumber,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeClockIdKind> clockId,
            long acknowledgedTick)
        where TRequest : IHostRuntimeRequest
    {
        EnsureId(attemptId.IsEmpty, nameof(attemptId));
        ArgumentNullException.ThrowIfNull(selection);
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

        EnsureId(
            acknowledgedLeaseId.IsEmpty,
            nameof(acknowledgedLeaseId));
        EnsureId(
            acknowledgedWorkerId.IsEmpty,
            nameof(acknowledgedWorkerId));
        EnsureId(
            acknowledgedDispatchId.IsEmpty,
            nameof(acknowledgedDispatchId));
        EnsureId(
            acknowledgedRequestId.IsEmpty,
            nameof(acknowledgedRequestId));

        if (acknowledgedAttemptNumber < 1 ||
            acknowledgedAttemptNumber >
                HostRuntimeRetryPolicy.MaximumAttemptCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(acknowledgedAttemptNumber));
        }

        EnsureId(clockId.IsEmpty, nameof(clockId));
        if (acknowledgedTick < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(acknowledgedTick));
        }

        var dispatch = selection.Dispatch;

        if (request.Revision != expectedRequestRevision)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchAcknowledgementStatus.StaleRequestRevision);
        }
        if (lease.Revision != expectedLeaseRevision)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchAcknowledgementStatus.StaleLeaseRevision);
        }
        if (request.RequestId != selection.RequestId)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchAcknowledgementStatus
                    .SelectionRequestMismatch);
        }
        if (lease.LeaseId != selection.LeaseId)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchAcknowledgementStatus
                    .SelectionLeaseMismatch);
        }
        if (request.State != HostRuntimeRequestState.Pending)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchAcknowledgementStatus.InvalidRequestState);
        }
        if (!lease.IsActive)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchAcknowledgementStatus.InvalidLeaseState);
        }
        if (lease.RequestId != request.RequestId)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchAcknowledgementStatus
                    .SelectionRequestMismatch);
        }
        if (lease.LeaseId != acknowledgedLeaseId)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchAcknowledgementStatus.LeaseMismatch);
        }
        if (lease.WorkerId != acknowledgedWorkerId)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchAcknowledgementStatus.WorkerMismatch);
        }
        if (dispatch.DispatchId != acknowledgedDispatchId)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchAcknowledgementStatus.DispatchMismatch);
        }
        if (dispatch.RequestId != acknowledgedRequestId)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchAcknowledgementStatus.RequestMismatch);
        }
        if (dispatch.AttemptNumber != acknowledgedAttemptNumber)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchAcknowledgementStatus
                    .AttemptNumberMismatch);
        }
        if (lease.ClockId != clockId)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchAcknowledgementStatus.ClockMismatch);
        }
        if (acknowledgedTick < lease.AcquiredTick)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchAcknowledgementStatus
                    .BeforeLeaseAcquisition);
        }
        if (acknowledgedTick >= lease.ExpiresTick)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchAcknowledgementStatus.LeaseExpired);
        }

        var attempt = new HostRuntimeInFlightAttempt<TRequest>(
            attemptId,
            selection,
            request,
            lease,
            acknowledgedTick);
        return new HostRuntimeDispatchAcknowledgementResult<TRequest>(
            HostRuntimeDispatchAcknowledgementStatus.Acknowledged,
            attempt);
    }

    private static HostRuntimeDispatchAcknowledgementResult<TRequest>
        Unchanged<TRequest>(
            HostRuntimeDispatchAcknowledgementStatus status)
        where TRequest : IHostRuntimeRequest =>
        new(status, attempt: null);

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
