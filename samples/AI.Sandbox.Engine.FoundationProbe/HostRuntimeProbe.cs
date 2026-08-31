namespace AI.Sandbox.Engine.FoundationProbe;

internal static class HostRuntimeProbe
{
    private sealed record ProbeRequest(string Value) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;

    private sealed record ProbeCompletion(string Value) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCompletion;

    private sealed record ProbeRetryReason(string Value) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRetryReason;

    private sealed record ProbeCancellationReason(string Value) :
        global::AI.Sandbox.Engine.Core.HostRuntime
            .IHostRuntimeCancellationReason;

    internal sealed record Result(
        string AdmissionStatus,
        string SelectionStatus,
        string AcknowledgementStatus,
        string SettlementStatus,
        string RequestState,
        string LeaseState,
        int QueuedCount);

    internal sealed record RetryResult(
        string RetryDecisionStatus,
        string RequeueStatus,
        string RequeuedRequestState,
        int RequeuedQueueCount,
        string DeadLetterDecisionStatus,
        string DeadLetterStatus,
        string DeadLetterKind);

    internal sealed record AbandonmentResult(
        string CancellationStatus,
        string CancellationKind,
        string CancelledRequestState,
        string ReleasedLeaseState,
        string LeaseExpiryStatus,
        string LeaseExpiryKind,
        string FailedRequestState,
        string ExpiredLeaseState);

    internal static Result Run()
    {
        var request =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.Create(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRequestIdKind>(8301),
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeInstanceIdKind>(8302),
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeOperationIdKind>(8303),
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeCorrelationIdKind>(8304),
                    default,
                    new ProbeRequest("foundation-host-runtime"));

        var queue =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueSnapshot.Create(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeQueueIdKind>(8305),
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeQueueCapacity.Create(4),
                    queuedCount: 0,
                    revision: 0);

        var admissionResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmissionFlow.Decide(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeAdmissionIdKind>(8306),
                    queue,
                    queue.Revision,
                    request,
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimePriority.Create(
                            global::AI.Sandbox.Engine.Core.HostRuntime
                                .HostRuntimePriorityClass.Normal,
                            sequence: 1));

        if (!admissionResult.Succeeded ||
            admissionResult.Admission is null)
        {
            throw Failure(
                "queue admission",
                admissionResult.Status);
        }

        var workerId =
            Id<global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkerIdKind>(8307);
        var clockId =
            Id<global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(8308);

        var lease =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Acquire(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeLeaseIdKind>(8309),
                    workerId,
                    admissionResult.Admission,
                    clockId,
                    acquiredTick: 10,
                    durationTicks: 100);

        var selectionResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionFlow.Select(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeDispatchSelectionIdKind>(8310),
                    admissionResult.Snapshot,
                    admissionResult.Snapshot.Revision,
                    lease,
                    clockId,
                    observedTick: 20,
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeDispatchIdKind>(8311),
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRouteIdKind>(8312),
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeEndpointIdKind>(8313),
                    attemptNumber: 1);

        if (!selectionResult.Succeeded ||
            selectionResult.Selection is null)
        {
            throw Failure(
                "dispatch selection",
                selectionResult.Status);
        }

        var acknowledgementResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementFlow.Acknowledge(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeAttemptIdKind>(8314),
                    selectionResult.Selection,
                    request,
                    lease,
                    request.Revision,
                    lease.Revision,
                    lease.LeaseId,
                    lease.WorkerId,
                    selectionResult.Selection.Dispatch.DispatchId,
                    selectionResult.Selection.RequestId,
                    selectionResult.Selection.Dispatch.AttemptNumber,
                    clockId,
                    acknowledgedTick: 30);

        if (!acknowledgementResult.Succeeded ||
            acknowledgementResult.Attempt is null)
        {
            throw Failure(
                "dispatch acknowledgement",
                acknowledgementResult.Status);
        }

        var completionKinds =
            global::System.Enum.GetValues<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeCompletionKind>();
        if (completionKinds.Length == 0)
        {
            throw new global::System.InvalidOperationException(
                "Host Runtime defines no completion kinds.");
        }

        var completionKind = completionKinds[0];
        foreach (var candidate in completionKinds)
        {
            var candidateName = candidate.ToString();
            if (candidateName.Contains(
                    "Success",
                    global::System.StringComparison.OrdinalIgnoreCase) ||
                candidateName.Contains(
                    "Complete",
                    global::System.StringComparison.OrdinalIgnoreCase))
            {
                completionKind = candidate;
                break;
            }
        }

        var attempt = acknowledgementResult.Attempt;
        var completion =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionFlow.Create(
                    attempt.DispatchId,
                    attempt.RequestId,
                    attempt.Dispatch.RuntimeInstanceId,
                    attempt.Dispatch.OperationId,
                    attempt.Dispatch.CorrelationId,
                    attempt.Dispatch.RouteId,
                    attempt.Dispatch.EndpointId,
                    attempt.AttemptNumber,
                    completionKind,
                    new ProbeCompletion("foundation-host-runtime-completed"));

        var settlementResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementFlow.Settle(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeSettlementIdKind>(8315),
                    attempt,
                    request,
                    lease,
                    request.Revision,
                    lease.Revision,
                    workerId,
                    clockId,
                    settledTick: 40,
                    completion);

        if (!settlementResult.Succeeded ||
            settlementResult.Settlement is null)
        {
            throw Failure(
                "attempt settlement",
                settlementResult.Status);
        }

        return new Result(
            admissionResult.Status.ToString(),
            selectionResult.Status.ToString(),
            acknowledgementResult.Status.ToString(),
            settlementResult.Status.ToString(),
            settlementResult.Request.State.ToString(),
            settlementResult.Lease.State.ToString(),
            selectionResult.Snapshot.QueuedCount);
    }

    internal static RetryResult RunRetry()
    {
        var request =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.Create(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRequestIdKind>(8401),
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeInstanceIdKind>(8402),
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeOperationIdKind>(8403),
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeCorrelationIdKind>(8404),
                    default,
                    new ProbeRequest("foundation-host-runtime-retry"));

        var queue =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueSnapshot.Create(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeQueueIdKind>(8405),
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeQueueCapacity.Create(4),
                    queuedCount: 0,
                    revision: 0);

        var admissionResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmissionFlow.Decide(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeAdmissionIdKind>(8406),
                    queue,
                    queue.Revision,
                    request,
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimePriority.Create(
                            global::AI.Sandbox.Engine.Core.HostRuntime
                                .HostRuntimePriorityClass.Normal,
                            sequence: 1));

        if (!admissionResult.Succeeded ||
            admissionResult.Admission is null)
        {
            throw Failure(
                "retry queue admission",
                admissionResult.Status);
        }

        var workerId =
            Id<global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkerIdKind>(8407);
        var clockId =
            Id<global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(8408);

        var lease =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Acquire(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeLeaseIdKind>(8409),
                    workerId,
                    admissionResult.Admission,
                    clockId,
                    acquiredTick: 10,
                    durationTicks: 100);

        var selectionResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionFlow.Select(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeDispatchSelectionIdKind>(8410),
                    admissionResult.Snapshot,
                    admissionResult.Snapshot.Revision,
                    lease,
                    clockId,
                    observedTick: 20,
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeDispatchIdKind>(8411),
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRouteIdKind>(8412),
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeEndpointIdKind>(8413),
                    attemptNumber: 1);

        if (!selectionResult.Succeeded ||
            selectionResult.Selection is null)
        {
            throw Failure(
                "retry dispatch selection",
                selectionResult.Status);
        }

        var acknowledgementResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementFlow.Acknowledge(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeAttemptIdKind>(8414),
                    selectionResult.Selection,
                    request,
                    lease,
                    request.Revision,
                    lease.Revision,
                    lease.LeaseId,
                    lease.WorkerId,
                    selectionResult.Selection.Dispatch.DispatchId,
                    selectionResult.Selection.RequestId,
                    selectionResult.Selection.Dispatch.AttemptNumber,
                    clockId,
                    acknowledgedTick: 30);

        if (!acknowledgementResult.Succeeded ||
            acknowledgementResult.Attempt is null)
        {
            throw Failure(
                "retry dispatch acknowledgement",
                acknowledgementResult.Status);
        }

        var attempt = acknowledgementResult.Attempt;
        var completion =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompletionFlow.Create(
                    attempt.DispatchId,
                    attempt.RequestId,
                    attempt.Dispatch.RuntimeInstanceId,
                    attempt.Dispatch.OperationId,
                    attempt.Dispatch.CorrelationId,
                    attempt.Dispatch.RouteId,
                    attempt.Dispatch.EndpointId,
                    attempt.AttemptNumber,
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeCompletionKind.Failed,
                    new ProbeCompletion("foundation-host-runtime-failed"));

        var settlementResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptSettlementFlow.Settle(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeSettlementIdKind>(8415),
                    attempt,
                    request,
                    lease,
                    request.Revision,
                    lease.Revision,
                    workerId,
                    clockId,
                    settledTick: 40,
                    completion);

        if (!settlementResult.Succeeded ||
            settlementResult.Settlement is null)
        {
            throw Failure(
                "failed attempt settlement",
                settlementResult.Status);
        }

        var settlement = settlementResult.Settlement;
        var retryReason =
            new ProbeRetryReason("foundation-host-runtime-retry");

        var retryPolicy =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryPolicy.Create(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRetryPolicyIdKind>(8416),
                    maximumAttempts: 3,
                    retryDelayTicks: 10);

        var retryDecision =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryDecisionFlow.Decide(
                    settlement.Request,
                    settlement.Request.Revision,
                    clockId,
                    observedTick: 50,
                    completedAttemptNumber: settlement.AttemptNumber,
                    retryPolicy,
                    deadline: null,
                    retryReason);

        if (!retryDecision.ShouldRetry)
        {
            throw Failure(
                "retry decision",
                retryDecision.Status);
        }

        var requeueResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryRequeueFlow.Requeue(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRetryRequeueIdKind>(8417),
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeAdmissionIdKind>(8418),
                    settlement,
                    retryDecision,
                    selectionResult.Snapshot,
                    selectionResult.Snapshot.Revision,
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimePriority.Create(
                            global::AI.Sandbox.Engine.Core.HostRuntime
                                .HostRuntimePriorityClass.Urgent,
                            sequence: 2),
                    clockId,
                    observedTick: 60);

        if (!requeueResult.Succeeded ||
            requeueResult.Requeue is null)
        {
            throw Failure(
                "retry requeue",
                requeueResult.Status);
        }

        var exhaustedPolicy =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryPolicy.Create(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRetryPolicyIdKind>(8419),
                    maximumAttempts: 1,
                    retryDelayTicks: 0);

        var exhaustedDecision =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryDecisionFlow.Decide(
                    settlement.Request,
                    settlement.Request.Revision,
                    clockId,
                    observedTick: 50,
                    completedAttemptNumber: settlement.AttemptNumber,
                    exhaustedPolicy,
                    deadline: null,
                    retryReason);

        if (exhaustedDecision.ShouldRetry)
        {
            throw new global::System.InvalidOperationException(
                "Exhausted retry policy unexpectedly allowed another attempt.");
        }

        var dispositionResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDeadLetterDispositionFlow.Dispose(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeDeadLetterDispositionIdKind>(8420),
                    settlement,
                    exhaustedDecision,
                    clockId,
                    disposedTick: 50);

        if (!dispositionResult.Succeeded ||
            dispositionResult.Disposition is null)
        {
            throw Failure(
                "dead-letter disposition",
                dispositionResult.Status);
        }

        return new RetryResult(
            retryDecision.Status.ToString(),
            requeueResult.Status.ToString(),
            requeueResult.Request.State.ToString(),
            requeueResult.Snapshot.QueuedCount,
            exhaustedDecision.Status.ToString(),
            dispositionResult.Status.ToString(),
            dispositionResult.Disposition.Kind.ToString());
    }

    internal static AbandonmentResult RunAbandonment()
    {
        var request =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.Create(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRequestIdKind>(8501),
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeInstanceIdKind>(8502),
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeOperationIdKind>(8503),
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeCorrelationIdKind>(8504),
                    default,
                    new ProbeRequest("foundation-host-runtime-abandonment"));

        var queue =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueSnapshot.Create(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeQueueIdKind>(8505),
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeQueueCapacity.Create(4),
                    queuedCount: 0,
                    revision: 0);

        var admissionResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmissionFlow.Decide(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeAdmissionIdKind>(8506),
                    queue,
                    queue.Revision,
                    request,
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimePriority.Create(
                            global::AI.Sandbox.Engine.Core.HostRuntime
                                .HostRuntimePriorityClass.Normal,
                            sequence: 1));

        if (!admissionResult.Succeeded ||
            admissionResult.Admission is null)
        {
            throw Failure(
                "abandonment queue admission",
                admissionResult.Status);
        }

        var workerId =
            Id<global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkerIdKind>(8507);
        var clockId =
            Id<global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(8508);

        var lease =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Acquire(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeLeaseIdKind>(8509),
                    workerId,
                    admissionResult.Admission,
                    clockId,
                    acquiredTick: 10,
                    durationTicks: 100);

        var selectionResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionFlow.Select(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeDispatchSelectionIdKind>(8510),
                    admissionResult.Snapshot,
                    admissionResult.Snapshot.Revision,
                    lease,
                    clockId,
                    observedTick: 20,
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeDispatchIdKind>(8511),
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRouteIdKind>(8512),
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeEndpointIdKind>(8513),
                    attemptNumber: 1);

        if (!selectionResult.Succeeded ||
            selectionResult.Selection is null)
        {
            throw Failure(
                "abandonment dispatch selection",
                selectionResult.Status);
        }

        var acknowledgementResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementFlow.Acknowledge(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeAttemptIdKind>(8514),
                    selectionResult.Selection,
                    request,
                    lease,
                    request.Revision,
                    lease.Revision,
                    lease.LeaseId,
                    lease.WorkerId,
                    selectionResult.Selection.Dispatch.DispatchId,
                    selectionResult.Selection.RequestId,
                    selectionResult.Selection.Dispatch.AttemptNumber,
                    clockId,
                    acknowledgedTick: 20);

        if (!acknowledgementResult.Succeeded ||
            acknowledgementResult.Attempt is null)
        {
            throw Failure(
                "abandonment dispatch acknowledgement",
                acknowledgementResult.Status);
        }

        var attempt = acknowledgementResult.Attempt;
        var cancellationRequested =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.RequestCancellation(
                    request,
                    request.Revision,
                    new ProbeCancellationReason(
                        "foundation-host-runtime-cancel")).Envelope;

        var cancellationResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionFlow.Dispose(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeAbandonedAttemptDispositionIdKind>(8515),
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeAbandonedAttemptDispositionKind
                        .CancellationRequested,
                    attempt,
                    cancellationRequested,
                    lease,
                    cancellationRequested.Revision,
                    lease.Revision,
                    clockId,
                    observedTick: 30);

        if (!cancellationResult.Succeeded ||
            cancellationResult.Disposition is null)
        {
            throw Failure(
                "cancellation abandonment",
                cancellationResult.Status);
        }

        var expiryResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionFlow.Dispose(
                    Id<global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeAbandonedAttemptDispositionIdKind>(8516),
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeAbandonedAttemptDispositionKind.LeaseExpired,
                    attempt,
                    request,
                    lease,
                    request.Revision,
                    lease.Revision,
                    clockId,
                    observedTick: lease.ExpiresTick);

        if (!expiryResult.Succeeded ||
            expiryResult.Disposition is null)
        {
            throw Failure(
                "lease-expiry abandonment",
                expiryResult.Status);
        }

        return new AbandonmentResult(
            cancellationResult.Status.ToString(),
            cancellationResult.Disposition.Kind.ToString(),
            cancellationResult.Request.State.ToString(),
            cancellationResult.Lease.State.ToString(),
            expiryResult.Status.ToString(),
            expiryResult.Disposition.Kind.ToString(),
            expiryResult.Request.State.ToString(),
            expiryResult.Lease.State.ToString());
    }

    private static global::System.InvalidOperationException Failure(
        string stage,
        object status) =>
        new($"Host Runtime {stage} failed with status '{status}'.");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>
        Id<TKind>(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(
            $"019d0000-0000-7000-8000-{suffix:D12}");
}
