namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeAbandonedAttemptDispositionTests
{
    private readonly record struct RequestPayload(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;

    private sealed record CancellationReason(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime
            .IHostRuntimeCancellationReason;

    private sealed class CountingRequest :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
    {
        public int InvocationCount { get; private set; }

        public void Invoke() => InvocationCount++;
    }

    [Xunit.Fact]
    public void IdsRevisionsAndTickAreValidated()
    {
        var context = CreateContext();

        Xunit.Assert.Throws<ArgumentException>(
            () => Dispose(
                context,
                dispositionId: default(
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeAbandonedAttemptDispositionIdKind>)));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Dispose(
                context,
                kind: (global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeAbandonedAttemptDispositionKind)99));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Dispose(
                context,
                expectedRequestRevision: -1));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Dispose(
                context,
                expectedLeaseRevision: -1));
        Xunit.Assert.Throws<ArgumentException>(
            () => Dispose(
                context,
                clockId: default(
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeClockIdKind>)));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Dispose(
                context,
                observedTick: -1));
    }

    [Xunit.Fact]
    public void CancellationDispositionCancelsRequestAndReleasesLease()
    {
        var context = CreateContext();
        var cancellationRequested = RequestCancellation(context);

        var result = Dispose(
            context,
            kind: global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionKind
                .CancellationRequested,
            request: cancellationRequested,
            observedTick: 30);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionStatus.Disposed,
            result.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Cancelled,
            result.Request.State);
        Xunit.Assert.Equal(cancellationRequested.Revision + 1, result.Request.Revision);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseState.Released,
            result.Lease.State);
        Xunit.Assert.Equal(context.Lease.Revision + 1, result.Lease.Revision);

        var disposition = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDisposition<RequestPayload>>(
                    result.Disposition);
        Xunit.Assert.Equal(DispositionId(), disposition.DispositionId);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionKind
                .CancellationRequested,
            disposition.Kind);
        Xunit.Assert.Same(context.Attempt, disposition.Attempt);
        Xunit.Assert.Same(result.Request, disposition.Request);
        Xunit.Assert.Same(result.Lease, disposition.Lease);
        Xunit.Assert.Equal(30, disposition.DisposedTick);
        Xunit.Assert.IsType<CancellationReason>(
            disposition.CancellationReason);
    }

    [Xunit.Fact]
    public void LeaseExpiryDispositionFailsPendingRequestAndExpiresLease()
    {
        var context = CreateContext();

        var result = Dispose(
            context,
            kind: global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionKind.LeaseExpired,
            observedTick: context.Lease.ExpiresTick);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Failed,
            result.Request.State);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseState.Expired,
            result.Lease.State);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionKind.LeaseExpired,
            result.Disposition!.Kind);
    }

    [Xunit.Fact]
    public void LeaseExpiryPreservesCancellationAsCancelledTerminalState()
    {
        var context = CreateContext();
        var cancellationRequested = RequestCancellation(context);

        var result = Dispose(
            context,
            kind: global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionKind.LeaseExpired,
            request: cancellationRequested,
            observedTick: context.Lease.ExpiresTick);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Cancelled,
            result.Request.State);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseState.Expired,
            result.Lease.State);
        Xunit.Assert.IsType<CancellationReason>(
            result.Disposition!.CancellationReason);
    }

    [Xunit.Fact]
    public void StaleRevisionsAreExplicit()
    {
        var context = CreateContext();

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionStatus
                .StaleRequestRevision,
            Dispose(
                context,
                expectedRequestRevision: context.Request.Revision + 1));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionStatus
                .StaleLeaseRevision,
            Dispose(
                context,
                expectedLeaseRevision: context.Lease.Revision + 1));
    }

    [Xunit.Fact]
    public void AttemptLineageWorkerAndClockMismatchesAreExplicit()
    {
        var context = CreateContext();
        var otherRequest =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.Create(
                    OtherRequestId(),
                    RuntimeId(),
                    OperationId(),
                    CorrelationId(),
                    default,
                    new RequestPayload("other"));
        var otherLease =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Acquire(
                    OtherLeaseId(),
                    context.Lease.WorkerId,
                    context.Admission,
                    context.Lease.ClockId,
                    acquiredTick: 10,
                    durationTicks: 100);
        var otherWorkerLease =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Acquire(
                    context.Lease.LeaseId,
                    OtherWorkerId(),
                    context.Admission,
                    context.Lease.ClockId,
                    acquiredTick: 10,
                    durationTicks: 100);

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionStatus
                .AttemptRequestMismatch,
            Dispose(context, request: otherRequest));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionStatus
                .AttemptLeaseMismatch,
            Dispose(context, lease: otherLease));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionStatus
                .AttemptWorkerMismatch,
            Dispose(context, lease: otherWorkerLease));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionStatus.ClockMismatch,
            Dispose(context, clockId: OtherClockId()));
    }

    [Xunit.Fact]
    public void InvalidRequestAndLeaseStatesAreExplicit()
    {
        var context = CreateContext();
        var terminalRequest =
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
                .HostRuntimeAbandonedAttemptDispositionStatus
                .InvalidRequestState,
            Dispose(context, request: terminalRequest));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionStatus
                .InvalidLeaseState,
            Dispose(context, lease: releasedLease));
    }

    [Xunit.Fact]
    public void CancellationAndExpiryTimeBoundariesAreExplicit()
    {
        var context = CreateContext();
        var cancellationRequested = RequestCancellation(context);

        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionStatus
                .CancellationNotRequested,
            Dispose(
                context,
                kind: global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeAbandonedAttemptDispositionKind
                    .CancellationRequested,
                observedTick: 30));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionStatus
                .LeaseAlreadyExpired,
            Dispose(
                context,
                kind: global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeAbandonedAttemptDispositionKind
                    .CancellationRequested,
                request: cancellationRequested,
                observedTick: context.Lease.ExpiresTick));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionStatus
                .LeaseNotExpired,
            Dispose(
                context,
                kind: global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeAbandonedAttemptDispositionKind.LeaseExpired,
                observedTick: context.Lease.ExpiresTick - 1));
        AssertStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionStatus
                .BeforeAcknowledgement,
            Dispose(
                context,
                observedTick: context.Attempt.AcknowledgedTick - 1));
    }

    [Xunit.Fact]
    public void DispositionPreservesAuthorityWithoutExecutingPayloads()
    {
        var payload = new CountingRequest();
        var context = CreateContext(payload);
        var originalRequest = context.Request;
        var originalLease = context.Lease;

        var result = Dispose(
            context,
            kind: global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionKind.LeaseExpired,
            observedTick: context.Lease.ExpiresTick);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(0, payload.InvocationCount);
        Xunit.Assert.Same(originalRequest, context.Request);
        Xunit.Assert.Same(originalLease, context.Lease);
        Xunit.Assert.Same(context.Attempt, result.Disposition!.Attempt);
        Xunit.Assert.Equal(context.Attempt.AttemptId, result.Disposition.AttemptId);
        Xunit.Assert.Equal(context.Attempt.RequestId, result.Disposition.RequestId);
        Xunit.Assert.Equal(context.Attempt.LeaseId, result.Disposition.LeaseId);
        Xunit.Assert.Equal(context.Attempt.WorkerId, result.Disposition.WorkerId);
        Xunit.Assert.Equal(context.Attempt.DispatchId, result.Disposition.DispatchId);
        Xunit.Assert.Equal(
            context.Attempt.AttemptNumber,
            result.Disposition.AttemptNumber);
        Xunit.Assert.Equal(context.Attempt.ClockId, result.Disposition.ClockId);
        Xunit.Assert.Equal(
            result.Request.Revision,
            result.Disposition.ObservedRequestRevision);
        Xunit.Assert.Equal(
            result.Lease.Revision,
            result.Disposition.ObservedLeaseRevision);
    }

    private static void AssertStatus<TRequest>(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeAbandonedAttemptDispositionStatus expected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeAbandonedAttemptDispositionResult<TRequest> result)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
    {
        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(expected, result.Status);
        Xunit.Assert.Null(result.Disposition);
    }

    private static DispositionContext<RequestPayload> CreateContext() =>
        CreateContext(new RequestPayload("payload"));

    private static DispositionContext<TRequest> CreateContext<TRequest>(
        TRequest payload)
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
                    sequence: 9);
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
        var acknowledgement =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementFlow.Acknowledge(
                    AttemptId(),
                    selection,
                    pending,
                    lease,
                    pending.Revision,
                    lease.Revision,
                    lease.LeaseId,
                    lease.WorkerId,
                    selection.Dispatch.DispatchId,
                    pending.RequestId,
                    selection.Dispatch.AttemptNumber,
                    lease.ClockId,
                    acknowledgedTick: 20);
        var attempt = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInFlightAttempt<TRequest>>(
                    acknowledgement.Attempt);

        return new DispositionContext<TRequest>(
            admission,
            pending,
            lease,
            attempt);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRequestEnvelope<TRequest>
        RequestCancellation<TRequest>(
            DispositionContext<TRequest> context)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.RequestCancellation(
                context.Request,
                context.Request.Revision,
                new CancellationReason("cancel")).Envelope;

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeAbandonedAttemptDispositionResult<TRequest>
        Dispose<TRequest>(
            DispositionContext<TRequest> context,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionKind kind =
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeAbandonedAttemptDispositionKind.LeaseExpired,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestEnvelope<TRequest>? request = null,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLease<TRequest>? lease = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeAbandonedAttemptDispositionIdKind>?
                dispositionId = null,
            long? expectedRequestRevision = null,
            long? expectedLeaseRevision = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeClockIdKind>? clockId = null,
            long? observedTick = null)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
    {
        var currentRequest = request ?? context.Request;
        var currentLease = lease ?? context.Lease;

        return global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeAbandonedAttemptDispositionFlow.Dispose(
                dispositionId ?? DispositionId(),
                kind,
                context.Attempt,
                currentRequest,
                currentLease,
                expectedRequestRevision ?? currentRequest.Revision,
                expectedLeaseRevision ?? currentLease.Revision,
                clockId ?? currentLease.ClockId,
                observedTick ?? currentLease.ExpiresTick);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>
        Id<TKind>(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(
            $"019d0000-0000-7000-8000-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeAbandonedAttemptDispositionIdKind>
        DispositionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAbandonedAttemptDispositionIdKind>(4101);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeAttemptIdKind>
        AttemptId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAttemptIdKind>(4102);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDispatchSelectionIdKind>
        SelectionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionIdKind>(4103);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueIdKind>
        QueueId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueIdKind>(4104);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeAdmissionIdKind>
        AdmissionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAdmissionIdKind>(4105);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseIdKind>
        LeaseId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseIdKind>(4106);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseIdKind>
        OtherLeaseId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseIdKind>(4107);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkerIdKind>
        WorkerId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkerIdKind>(4108);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkerIdKind>
        OtherWorkerId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkerIdKind>(4109);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        ClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(4110);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        OtherClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(4111);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeDispatchIdKind>
        DispatchId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchIdKind>(4112);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRouteIdKind>
        RouteId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRouteIdKind>(4113);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeEndpointIdKind>
        EndpointId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeEndpointIdKind>(4114);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestIdKind>
        RequestId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestIdKind>(4115);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestIdKind>
        OtherRequestId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestIdKind>(4116);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeInstanceIdKind>
        RuntimeId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInstanceIdKind>(4117);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeOperationIdKind>
        OperationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeOperationIdKind>(4118);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCorrelationIdKind>
        CorrelationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCorrelationIdKind>(4119);

    private sealed record DispositionContext<TRequest>(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeQueueAdmission<TRequest> Admission,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestEnvelope<TRequest> Request,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeWorkLease<TRequest> Lease,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeInFlightAttempt<TRequest> Attempt)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;
}
