namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Reopens one retryable terminal request and re-admits it as one pure
/// immutable Host decision.
/// </summary>
public static class HostRuntimeRetryRequeueFlow
{
    /// <summary>
    /// Applies an allowed retry decision without storing, scheduling, waiting,
    /// or executing the next attempt.
    /// </summary>
    /// <typeparam name="TRequest">Exact request payload type.</typeparam>
    /// <typeparam name="TCompletion">Exact completion payload type.</typeparam>
    /// <param name="requeueId">Externally assigned non-empty requeue ID.</param>
    /// <param name="admissionId">Externally assigned non-empty admission ID.</param>
    /// <param name="settlement">Terminal attempt-settlement authority.</param>
    /// <param name="retryDecision">Advisory retry decision for the settlement.</param>
    /// <param name="snapshot">Current immutable queue authority.</param>
    /// <param name="expectedQueueRevision">Queue revision observed by the caller.</param>
    /// <param name="priority">Deterministic priority for the next attempt.</param>
    /// <param name="clockId">Matching externally owned monotonic clock.</param>
    /// <param name="observedTick">Current non-negative external clock tick.</param>
    /// <returns>An explicit immutable retry requeue result.</returns>
    public static HostRuntimeRetryRequeueResult<TRequest, TCompletion>
        Requeue<TRequest, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRetryRequeueIdKind> requeueId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeAdmissionIdKind> admissionId,
            HostRuntimeAttemptSettlement<TRequest, TCompletion> settlement,
            HostRuntimeRetryDecision<TRequest> retryDecision,
            HostRuntimeQueueSnapshot snapshot,
            long expectedQueueRevision,
            HostRuntimePriority priority,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeClockIdKind> clockId,
            long observedTick)
        where TRequest : IHostRuntimeRequest
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(requeueId.IsEmpty, nameof(requeueId));
        EnsureId(admissionId.IsEmpty, nameof(admissionId));
        ArgumentNullException.ThrowIfNull(settlement);
        ArgumentNullException.ThrowIfNull(retryDecision);
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(priority);
        if (expectedQueueRevision < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedQueueRevision));
        }
        EnsureId(clockId.IsEmpty, nameof(clockId));
        if (observedTick < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(observedTick));
        }

        if (settlement.OutcomeKind is not (
            HostRuntimeCompletionKind.Failed or
            HostRuntimeCompletionKind.Rejected))
        {
            return Unchanged<TRequest, TCompletion>(
                HostRuntimeRetryRequeueStatus.InvalidSettlementOutcome,
                settlement.Request,
                snapshot);
        }
        if (!retryDecision.ShouldRetry ||
            retryDecision.RetryAtTick is null)
        {
            return Unchanged<TRequest, TCompletion>(
                HostRuntimeRetryRequeueStatus.RetryDenied,
                settlement.Request,
                snapshot);
        }
        if (retryDecision.Request != settlement.Request)
        {
            return Unchanged<TRequest, TCompletion>(
                HostRuntimeRetryRequeueStatus.SettlementRequestMismatch,
                settlement.Request,
                snapshot);
        }
        if (retryDecision.CompletedAttemptNumber !=
                settlement.AttemptNumber ||
            retryDecision.NextAttemptNumber !=
                settlement.AttemptNumber + 1)
        {
            return Unchanged<TRequest, TCompletion>(
                HostRuntimeRetryRequeueStatus.AttemptNumberMismatch,
                settlement.Request,
                snapshot);
        }
        if (retryDecision.ClockId != settlement.ClockId ||
            clockId != settlement.ClockId)
        {
            return Unchanged<TRequest, TCompletion>(
                HostRuntimeRetryRequeueStatus.ClockMismatch,
                settlement.Request,
                snapshot);
        }

        var retryAtTick = retryDecision.RetryAtTick.Value;
        var decisionObservedTick =
            retryAtTick - retryDecision.Policy.RetryDelayTicks;
        if (decisionObservedTick < settlement.SettledTick)
        {
            return Unchanged<TRequest, TCompletion>(
                HostRuntimeRetryRequeueStatus.RetryDecisionBeforeSettlement,
                settlement.Request,
                snapshot);
        }
        if (observedTick < settlement.SettledTick)
        {
            return Unchanged<TRequest, TCompletion>(
                HostRuntimeRetryRequeueStatus.BeforeSettlement,
                settlement.Request,
                snapshot);
        }
        if (observedTick < retryAtTick)
        {
            return Unchanged<TRequest, TCompletion>(
                HostRuntimeRetryRequeueStatus.BeforeRetryTick,
                settlement.Request,
                snapshot);
        }

        var pendingRequest = new HostRuntimeRequestEnvelope<TRequest>(
            settlement.Request.RequestId,
            settlement.Request.RuntimeInstanceId,
            settlement.Request.OperationId,
            settlement.Request.CorrelationId,
            settlement.Request.ParentRequestId,
            settlement.Request.Payload,
            HostRuntimeRequestState.Pending,
            checked(settlement.Request.Revision + 1),
            cancellationReason: null);

        var admission = HostRuntimeQueueAdmissionFlow.Decide(
            admissionId,
            snapshot,
            expectedQueueRevision,
            pendingRequest,
            priority);
        if (!admission.Succeeded)
        {
            var status = admission.Status switch
            {
                HostRuntimeQueueAdmissionStatus.StaleQueueRevision =>
                    HostRuntimeRetryRequeueStatus.StaleQueueRevision,
                HostRuntimeQueueAdmissionStatus.QueueFull =>
                    HostRuntimeRetryRequeueStatus.QueueFull,
                _ => HostRuntimeRetryRequeueStatus.AdmissionRejected,
            };

            return Unchanged<TRequest, TCompletion>(
                status,
                settlement.Request,
                admission.Snapshot);
        }

        var requeue = new HostRuntimeRetryRequeue<TRequest, TCompletion>(
            requeueId,
            settlement,
            retryDecision,
            pendingRequest,
            admission.Admission!,
            observedTick);

        return new HostRuntimeRetryRequeueResult<TRequest, TCompletion>(
            HostRuntimeRetryRequeueStatus.Requeued,
            requeue,
            pendingRequest,
            admission.Snapshot);
    }

    private static HostRuntimeRetryRequeueResult<TRequest, TCompletion>
        Unchanged<TRequest, TCompletion>(
            HostRuntimeRetryRequeueStatus status,
            HostRuntimeRequestEnvelope<TRequest> request,
            HostRuntimeQueueSnapshot snapshot)
        where TRequest : IHostRuntimeRequest
        where TCompletion : IHostRuntimeCompletion =>
        new(status, null, request, snapshot);

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
