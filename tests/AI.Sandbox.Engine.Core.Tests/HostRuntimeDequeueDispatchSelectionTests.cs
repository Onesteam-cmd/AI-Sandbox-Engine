namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeDequeueDispatchSelectionTests
{
    private readonly record struct RequestPayload(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime
            .IHostRuntimeRequest;

    private sealed class CountingRequest :
        global::AI.Sandbox.Engine.Core.HostRuntime
            .IHostRuntimeRequest
    {
        public int InvocationCount { get; private set; }

        public void Invoke() => InvocationCount++;
    }

    [Xunit.Fact]
    public void IdsTicksAndQueueBoundsAreValidated()
    {
        var context = CreateContext();

        Xunit.Assert.Throws<ArgumentException>(
            () => Select(
                default,
                context.Snapshot,
                context.Snapshot.Revision,
                context.Lease,
                ClockId(),
                observedTick: 20,
                DispatchId(),
                RouteId(),
                EndpointId(),
                attemptNumber: 1));

        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Select(
                SelectionId(),
                context.Snapshot,
                expectedQueueRevision: -1,
                context.Lease,
                ClockId(),
                observedTick: 20,
                DispatchId(),
                RouteId(),
                EndpointId(),
                attemptNumber: 1));

        Xunit.Assert.Throws<ArgumentException>(
            () => Select(
                SelectionId(),
                context.Snapshot,
                context.Snapshot.Revision,
                context.Lease,
                default,
                observedTick: 20,
                DispatchId(),
                RouteId(),
                EndpointId(),
                attemptNumber: 1));

        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Select(
                SelectionId(),
                context.Snapshot,
                context.Snapshot.Revision,
                context.Lease,
                ClockId(),
                observedTick: -1,
                DispatchId(),
                RouteId(),
                EndpointId(),
                attemptNumber: 1));
    }

    [Xunit.Fact]
    public void SelectionDequeuesAndCreatesDispatchImmutably()
    {
        var context = CreateContext();

        var result = Select(
            SelectionId(),
            context.Snapshot,
            context.Snapshot.Revision,
            context.Lease,
            ClockId(),
            observedTick: 20,
            DispatchId(),
            RouteId(),
            EndpointId(),
            attemptNumber: 1);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionStatus.Selected,
            result.Status);
        Xunit.Assert.Equal(
            context.Snapshot.QueuedCount - 1,
            result.Snapshot.QueuedCount);
        Xunit.Assert.Equal(
            context.Snapshot.Revision + 1,
            result.Snapshot.Revision);
        Xunit.Assert.Equal(1, context.Snapshot.QueuedCount);
        Xunit.Assert.NotNull(result.Selection);
        Xunit.Assert.Same(
            context.Lease,
            result.Selection!.Lease);
        Xunit.Assert.Equal(
            DispatchId(),
            result.Selection.Dispatch.DispatchId);
        Xunit.Assert.Equal(
            context.Lease.RequestId,
            result.Selection.Dispatch.RequestId);
    }

    [Xunit.Fact]
    public void StaleRevisionEmptyQueueAndQueueMismatchAreExplicit()
    {
        var context = CreateContext();

        var stale = Select(
            SelectionId(),
            context.Snapshot,
            context.Snapshot.Revision - 1,
            context.Lease,
            ClockId(),
            observedTick: 20,
            DispatchId(),
            RouteId(),
            EndpointId(),
            attemptNumber: 1);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionStatus
                .StaleQueueRevision,
            stale.Status);
        Xunit.Assert.Same(context.Snapshot, stale.Snapshot);
        Xunit.Assert.Null(stale.Selection);

        var emptySnapshot = Snapshot(
            QueueId(),
            queuedCount: 0,
            revision: context.Snapshot.Revision);
        var empty = Select(
            SelectionId(),
            emptySnapshot,
            emptySnapshot.Revision,
            context.Lease,
            ClockId(),
            observedTick: 20,
            DispatchId(),
            RouteId(),
            EndpointId(),
            attemptNumber: 1);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionStatus.EmptyQueue,
            empty.Status);
        Xunit.Assert.Same(emptySnapshot, empty.Snapshot);

        var otherQueueSnapshot = Snapshot(
            OtherQueueId(),
            queuedCount: 1,
            revision: context.Snapshot.Revision);
        var mismatch = Select(
            SelectionId(),
            otherQueueSnapshot,
            otherQueueSnapshot.Revision,
            context.Lease,
            ClockId(),
            observedTick: 20,
            DispatchId(),
            RouteId(),
            EndpointId(),
            attemptNumber: 1);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionStatus.QueueMismatch,
            mismatch.Status);
        Xunit.Assert.Same(
            otherQueueSnapshot,
            mismatch.Snapshot);
    }

    [Xunit.Fact]
    public void LeaseStateClockAndExpiryAreValidated()
    {
        var context = CreateContext();

        var released =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Release(
                    context.Lease,
                    context.Lease.Revision,
                    context.Lease.WorkerId).Lease;
        var invalidState = Select(
            SelectionId(),
            context.Snapshot,
            context.Snapshot.Revision,
            released,
            ClockId(),
            observedTick: 20,
            DispatchId(),
            RouteId(),
            EndpointId(),
            attemptNumber: 1);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionStatus
                .InvalidLeaseState,
            invalidState.Status);

        var clockMismatch = Select(
            SelectionId(),
            context.Snapshot,
            context.Snapshot.Revision,
            context.Lease,
            OtherClockId(),
            observedTick: 20,
            DispatchId(),
            RouteId(),
            EndpointId(),
            attemptNumber: 1);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionStatus.ClockMismatch,
            clockMismatch.Status);

        var expired = Select(
            SelectionId(),
            context.Snapshot,
            context.Snapshot.Revision,
            context.Lease,
            ClockId(),
            observedTick: context.Lease.ExpiresTick,
            DispatchId(),
            RouteId(),
            EndpointId(),
            attemptNumber: 1);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionStatus.LeaseExpired,
            expired.Status);
    }

    [Xunit.Fact]
    public void SelectionPreservesLeasePriorityAndRoutingAuthority()
    {
        var context = CreateContext();

        var result = Select(
            SelectionId(),
            context.Snapshot,
            context.Snapshot.Revision,
            context.Lease,
            ClockId(),
            observedTick: 20,
            DispatchId(),
            RouteId(),
            EndpointId(),
            attemptNumber: 2);

        var selection = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelection<RequestPayload>>(
                    result.Selection);
        Xunit.Assert.Equal(
            SelectionId(),
            selection.SelectionId);
        Xunit.Assert.Equal(QueueId(), selection.QueueId);
        Xunit.Assert.Equal(
            context.Lease.RequestId,
            selection.RequestId);
        Xunit.Assert.Equal(
            context.Lease.LeaseId,
            selection.LeaseId);
        Xunit.Assert.Equal(
            context.Lease.WorkerId,
            selection.WorkerId);
        Xunit.Assert.Same(
            context.Lease.Admission.Priority,
            selection.Priority);
        Xunit.Assert.Equal(
            RouteId(),
            selection.Dispatch.RouteId);
        Xunit.Assert.Equal(
            EndpointId(),
            selection.Dispatch.EndpointId);
        Xunit.Assert.Equal(
            2,
            selection.Dispatch.AttemptNumber);
    }

    [Xunit.Fact]
    public void DequeueRevisionAndCountAdvanceExactlyOnce()
    {
        var context = CreateContext();

        var result = Select(
            SelectionId(),
            context.Snapshot,
            context.Snapshot.Revision,
            context.Lease,
            ClockId(),
            observedTick: 20,
            DispatchId(),
            RouteId(),
            EndpointId(),
            attemptNumber: 1);

        Xunit.Assert.Equal(
            context.Snapshot.Revision,
            result.Selection!.ObservedQueueRevision);
        Xunit.Assert.Equal(
            context.Snapshot.Revision + 1,
            result.Snapshot.Revision);
        Xunit.Assert.Equal(0, result.Snapshot.QueuedCount);
        Xunit.Assert.Equal(1, context.Snapshot.QueuedCount);
        Xunit.Assert.Equal(
            context.Lease.Revision,
            result.Selection.Lease.Revision);
    }

    [Xunit.Fact]
    public void DispatchValidationRemainsAuthoritative()
    {
        var context = CreateContext();

        Xunit.Assert.Throws<ArgumentException>(
            () => Select(
                SelectionId(),
                context.Snapshot,
                context.Snapshot.Revision,
                context.Lease,
                ClockId(),
                observedTick: 20,
                default,
                RouteId(),
                EndpointId(),
                attemptNumber: 1));

        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Select(
                SelectionId(),
                context.Snapshot,
                context.Snapshot.Revision,
                context.Lease,
                ClockId(),
                observedTick: 20,
                DispatchId(),
                RouteId(),
                EndpointId(),
                attemptNumber: 0));
    }

    [Xunit.Fact]
    public void ContractsDoNotStoreQueueDispatchOrExecuteRequest()
    {
        var request = new CountingRequest();
        var context = CreateContext(request);

        var result =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionFlow.Select(
                    SelectionId(),
                    context.Snapshot,
                    context.Snapshot.Revision,
                    context.Lease,
                    ClockId(),
                    observedTick: 20,
                    DispatchId(),
                    RouteId(),
                    EndpointId(),
                    attemptNumber: 1);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(0, request.InvocationCount);
        Xunit.Assert.Same(
            context.Lease.Admission.Request,
            result.Selection!.Dispatch.Request);
    }

    private static SelectionContext<RequestPayload>
        CreateContext() =>
        CreateContext(new RequestPayload("payload"));

    private static SelectionContext<TRequest>
        CreateContext<TRequest>(TRequest request)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime
                .IHostRuntimeRequest
    {
        var pending =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.Create(
                    RequestId(),
                    RuntimeId(),
                    OperationId(),
                    CorrelationId(),
                    default,
                    request);
        var initial = Snapshot(
            QueueId(),
            queuedCount: 0,
            revision: 10);
        var priority =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimePriority.Create(
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimePriorityClass.Urgent,
                    sequence: 7);
        var admitted =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmissionFlow.Decide(
                    AdmissionId(),
                    initial,
                    initial.Revision,
                    pending,
                    priority);
        var admission = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmission<TRequest>>(
                    admitted.Admission);
        var lease =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Acquire(
                    LeaseId(),
                    WorkerId(),
                    admission,
                    ClockId(),
                    acquiredTick: 10,
                    durationTicks: 100);

        return new SelectionContext<TRequest>(
            admitted.Snapshot,
            lease);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeDispatchSelectionResult<TRequest>
        Select<TRequest>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeDispatchSelectionIdKind> selectionId,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueSnapshot snapshot,
            long expectedQueueRevision,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLease<TRequest> lease,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeClockIdKind> clockId,
            long observedTick,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeDispatchIdKind> dispatchId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeRouteIdKind> routeId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeEndpointIdKind> endpointId,
            int attemptNumber)
            where TRequest :
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .IHostRuntimeRequest =>
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionFlow.Select(
                    selectionId,
                    snapshot,
                    expectedQueueRevision,
                    lease,
                    clockId,
                    observedTick,
                    dispatchId,
                    routeId,
                    endpointId,
                    attemptNumber);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeQueueSnapshot Snapshot(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeQueueIdKind> queueId,
            int queuedCount,
            long revision) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeQueueSnapshot.Create(
                queueId,
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeQueueCapacity.Create(8),
                queuedCount,
                revision);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>
        Id<TKind>(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(
            $"019d0000-0000-7000-8000-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDispatchSelectionIdKind>
        SelectionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionIdKind>(3601);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeQueueIdKind>
        QueueId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueIdKind>(3602);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeQueueIdKind>
        OtherQueueId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueIdKind>(3603);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeAdmissionIdKind>
        AdmissionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAdmissionIdKind>(3604);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeLeaseIdKind>
        LeaseId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseIdKind>(3605);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeWorkerIdKind>
        WorkerId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkerIdKind>(3606);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeClockIdKind>
        ClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(3607);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeClockIdKind>
        OtherClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(3608);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDispatchIdKind>
        DispatchId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchIdKind>(3609);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRouteIdKind>
        RouteId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRouteIdKind>(3610);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeEndpointIdKind>
        EndpointId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeEndpointIdKind>(3611);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestIdKind>
        RequestId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestIdKind>(3612);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeInstanceIdKind>
        RuntimeId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInstanceIdKind>(3613);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeOperationIdKind>
        OperationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeOperationIdKind>(3614);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCorrelationIdKind>
        CorrelationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCorrelationIdKind>(3615);

    private sealed record SelectionContext<TRequest>(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeQueueSnapshot Snapshot,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeWorkLease<TRequest> Lease)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime
                .IHostRuntimeRequest;
}
