namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeInFlightDispatchAcknowledgementTests
{
    private readonly record struct RequestPayload(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;

    private sealed class CountingRequest :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
    {
        public int InvocationCount { get; private set; }

        public void Invoke() => InvocationCount++;
    }

    [Xunit.Fact]
    public void IdsRevisionsAttemptAndTickAreValidated()
    {
        var context = CreateContext();

        Xunit.Assert.Throws<ArgumentException>(
            () => Acknowledge(
                context,
                attemptId: default(
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeAttemptIdKind>)));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Acknowledge(
                context,
                expectedRequestRevision: -1));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Acknowledge(
                context,
                expectedLeaseRevision: -1));
        Xunit.Assert.Throws<ArgumentException>(
            () => Acknowledge(
                context,
                acknowledgedLeaseId: default(
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeLeaseIdKind>)));
        Xunit.Assert.Throws<ArgumentException>(
            () => Acknowledge(
                context,
                acknowledgedWorkerId: default(
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeWorkerIdKind>)));
        Xunit.Assert.Throws<ArgumentException>(
            () => Acknowledge(
                context,
                acknowledgedDispatchId: default(
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeDispatchIdKind>)));
        Xunit.Assert.Throws<ArgumentException>(
            () => Acknowledge(
                context,
                acknowledgedRequestId: default(
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeRequestIdKind>)));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Acknowledge(
                context,
                acknowledgedAttemptNumber: 0));
        Xunit.Assert.Throws<ArgumentException>(
            () => Acknowledge(
                context,
                clockId: default(
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeClockIdKind>)));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Acknowledge(
                context,
                acknowledgedTick: -1));
    }

    [Xunit.Fact]
    public void AcknowledgementCreatesImmutableInFlightAuthority()
    {
        var context = CreateContext();

        var result = Acknowledge(context);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus.Acknowledged,
            result.Status);

        var attempt = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInFlightAttempt<RequestPayload>>(
                    result.Attempt);
        Xunit.Assert.Equal(AttemptId(), attempt.AttemptId);
        Xunit.Assert.Same(context.Selection, attempt.Selection);
        Xunit.Assert.Same(context.Request, attempt.Request);
        Xunit.Assert.Same(context.Lease, attempt.Lease);
        Xunit.Assert.Same(context.Selection.Dispatch, attempt.Dispatch);
        Xunit.Assert.Equal(context.Selection.QueueId, attempt.QueueId);
        Xunit.Assert.Equal(context.Request.RequestId, attempt.RequestId);
        Xunit.Assert.Equal(context.Lease.LeaseId, attempt.LeaseId);
        Xunit.Assert.Equal(context.Lease.WorkerId, attempt.WorkerId);
        Xunit.Assert.Equal(
            context.Selection.Dispatch.DispatchId,
            attempt.DispatchId);
        Xunit.Assert.Equal(
            context.Selection.Dispatch.AttemptNumber,
            attempt.AttemptNumber);
        Xunit.Assert.Equal(context.Lease.ClockId, attempt.ClockId);
        Xunit.Assert.Equal(20, attempt.AcknowledgedTick);
        Xunit.Assert.Equal(
            context.Request.Revision,
            attempt.ObservedRequestRevision);
        Xunit.Assert.Equal(
            context.Lease.Revision,
            attempt.ObservedLeaseRevision);
    }

    [Xunit.Fact]
    public void StaleRequestAndLeaseRevisionsAreExplicit()
    {
        var context = CreateContext();

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus
                .StaleRequestRevision,
            Acknowledge(
                context,
                expectedRequestRevision: context.Request.Revision + 1));

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus
                .StaleLeaseRevision,
            Acknowledge(
                context,
                expectedLeaseRevision: context.Lease.Revision + 1));
    }

    [Xunit.Fact]
    public void SelectionAndAcknowledgementMismatchesAreExplicit()
    {
        var context = CreateContext();
        var otherRequest = Pending(requestSuffix: 3790);
        var otherLease =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Acquire(
                    OtherLeaseId(),
                    context.Lease.WorkerId,
                    context.Lease.Admission,
                    context.Lease.ClockId,
                    context.Lease.AcquiredTick,
                    context.Lease.ExpiresTick -
                        context.Lease.AcquiredTick);

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus
                .SelectionRequestMismatch,
            Acknowledge(context, request: otherRequest));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus
                .SelectionLeaseMismatch,
            Acknowledge(context, lease: otherLease));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus.LeaseMismatch,
            Acknowledge(
                context,
                acknowledgedLeaseId: OtherLeaseId()));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus.WorkerMismatch,
            Acknowledge(
                context,
                acknowledgedWorkerId: OtherWorkerId()));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus.DispatchMismatch,
            Acknowledge(
                context,
                acknowledgedDispatchId: OtherDispatchId()));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus.RequestMismatch,
            Acknowledge(
                context,
                acknowledgedRequestId: OtherRequestId()));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus
                .AttemptNumberMismatch,
            Acknowledge(
                context,
                acknowledgedAttemptNumber: 2));
    }

    [Xunit.Fact]
    public void RequestLeaseClockAndTimeBoundariesAreExplicit()
    {
        var context = CreateContext();
        var failedRequest =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.Finalize(
                    context.Request,
                    context.Request.Revision,
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRequestState.Failed).Envelope;
        var releasedLease =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Release(
                    context.Lease,
                    context.Lease.Revision,
                    context.Lease.WorkerId).Lease;

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus
                .InvalidRequestState,
            Acknowledge(context, request: failedRequest));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus
                .InvalidLeaseState,
            Acknowledge(context, lease: releasedLease));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus.ClockMismatch,
            Acknowledge(context, clockId: OtherClockId()));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus
                .BeforeLeaseAcquisition,
            Acknowledge(
                context,
                acknowledgedTick: context.Lease.AcquiredTick - 1));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementStatus.LeaseExpired,
            Acknowledge(
                context,
                acknowledgedTick: context.Lease.ExpiresTick));
    }

    [Xunit.Fact]
    public void RenewedActiveLeaseCanBeAcknowledged()
    {
        var context = CreateContext();
        var renewed =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Renew(
                    context.Lease,
                    context.Lease.Revision,
                    context.Lease.ClockId,
                    observedTick: 20,
                    durationTicks: 200).Lease;

        var result = Acknowledge(
            context,
            lease: renewed,
            acknowledgedTick: 21);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Same(renewed, result.Attempt!.Lease);
        Xunit.Assert.Equal(renewed.Revision, result.Attempt.ObservedLeaseRevision);
        Xunit.Assert.Equal(21, result.Attempt.AcknowledgedTick);
    }

    [Xunit.Fact]
    public void ContractsDoNotExecuteOrMutateAuthority()
    {
        var payload = new CountingRequest();
        var context = CreateContext(payload);

        var result = Acknowledge(context);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(0, payload.InvocationCount);
        Xunit.Assert.Same(context.Request, result.Attempt!.Request);
        Xunit.Assert.Same(
            context.Selection.Dispatch.Request,
            result.Attempt.Dispatch.Request);
        Xunit.Assert.Same(context.Lease, result.Attempt.Lease);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Pending,
            context.Request.State);
        Xunit.Assert.True(context.Lease.IsActive);
    }

    private static void AssertStatus<TRequest>(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDispatchAcknowledgementStatus expected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDispatchAcknowledgementResult<TRequest> result)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
    {
        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(expected, result.Status);
        Xunit.Assert.Null(result.Attempt);
    }

    private static AcknowledgementContext<RequestPayload>
        CreateContext() =>
        CreateContext(new RequestPayload("payload"));

    private static AcknowledgementContext<TRequest>
        CreateContext<TRequest>(TRequest payload)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
    {
        var pending =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.Create(
                    RequestId(),
                    RuntimeId(),
                    OperationId(),
                    CorrelationId(),
                    default,
                    payload);
        var snapshot =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueSnapshot.Create(
                    QueueId(),
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeQueueCapacity.Create(8),
                    queuedCount: 0,
                    revision: 10);
        var priority =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimePriority.Create(
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimePriorityClass.Urgent,
                    sequence: 7);
        var admissionResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmissionFlow.Decide(
                    AdmissionId(),
                    snapshot,
                    snapshot.Revision,
                    pending,
                    priority);
        var admission = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmission<TRequest>>(
                    admissionResult.Admission);
        var lease =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Acquire(
                    LeaseId(),
                    WorkerId(),
                    admission,
                    ClockId(),
                    acquiredTick: 10,
                    durationTicks: 100);
        var selectionResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionFlow.Select(
                    SelectionId(),
                    admissionResult.Snapshot,
                    admissionResult.Snapshot.Revision,
                    lease,
                    ClockId(),
                    observedTick: 20,
                    DispatchId(),
                    RouteId(),
                    EndpointId(),
                    attemptNumber: 1);
        var selection = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelection<TRequest>>(
                    selectionResult.Selection);

        return new AcknowledgementContext<TRequest>(
            pending,
            selection,
            lease);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRequestEnvelope<RequestPayload> Pending(
            int requestSuffix) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.Create(
                Id<
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRequestIdKind>(requestSuffix),
                RuntimeId(),
                OperationId(),
                CorrelationId(),
                default,
                new RequestPayload("other"));

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeDispatchAcknowledgementResult<TRequest>
        Acknowledge<TRequest>(
            AcknowledgementContext<TRequest> context,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestEnvelope<TRequest>? request = null,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLease<TRequest>? lease = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeAttemptIdKind>? attemptId = null,
            long? expectedRequestRevision = null,
            long? expectedLeaseRevision = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeLeaseIdKind>? acknowledgedLeaseId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeWorkerIdKind>? acknowledgedWorkerId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeDispatchIdKind>? acknowledgedDispatchId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeRequestIdKind>? acknowledgedRequestId = null,
            int? acknowledgedAttemptNumber = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeClockIdKind>? clockId = null,
            long? acknowledgedTick = null)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
    {
        var currentRequest = request ?? context.Request;
        var currentLease = lease ?? context.Lease;

        return global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDispatchAcknowledgementFlow.Acknowledge(
                attemptId ?? AttemptId(),
                context.Selection,
                currentRequest,
                currentLease,
                expectedRequestRevision ?? currentRequest.Revision,
                expectedLeaseRevision ?? currentLease.Revision,
                acknowledgedLeaseId ?? currentLease.LeaseId,
                acknowledgedWorkerId ?? currentLease.WorkerId,
                acknowledgedDispatchId ??
                    context.Selection.Dispatch.DispatchId,
                acknowledgedRequestId ?? context.Selection.RequestId,
                acknowledgedAttemptNumber ??
                    context.Selection.Dispatch.AttemptNumber,
                clockId ?? currentLease.ClockId,
                acknowledgedTick ?? 20);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>
        Id<TKind>(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(
            $"019d0000-0000-7000-8000-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeAttemptIdKind>
        AttemptId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptIdKind>(3701);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDispatchSelectionIdKind>
        SelectionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionIdKind>(3702);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueIdKind>
        QueueId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueIdKind>(3703);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeAdmissionIdKind>
        AdmissionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAdmissionIdKind>(3704);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseIdKind>
        LeaseId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseIdKind>(3705);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseIdKind>
        OtherLeaseId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseIdKind>(3706);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkerIdKind>
        WorkerId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkerIdKind>(3707);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkerIdKind>
        OtherWorkerId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkerIdKind>(3708);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        ClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(3709);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        OtherClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(3710);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeDispatchIdKind>
        DispatchId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchIdKind>(3711);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeDispatchIdKind>
        OtherDispatchId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchIdKind>(3712);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRouteIdKind>
        RouteId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRouteIdKind>(3713);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeEndpointIdKind>
        EndpointId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeEndpointIdKind>(3714);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestIdKind>
        RequestId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestIdKind>(3715);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestIdKind>
        OtherRequestId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestIdKind>(3716);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeInstanceIdKind>
        RuntimeId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInstanceIdKind>(3717);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeOperationIdKind>
        OperationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeOperationIdKind>(3718);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCorrelationIdKind>
        CorrelationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCorrelationIdKind>(3719);

    private sealed record AcknowledgementContext<TRequest>(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestEnvelope<TRequest> Request,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDispatchSelection<TRequest> Selection,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeWorkLease<TRequest> Lease)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;
}
