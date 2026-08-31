namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeQueueAdmissionPriorityTests
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
    public void IdsCapacityAndBoundsAreValidated()
    {
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueCapacity.Create(0));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimePriority.Create(
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimePriorityClass.Normal,
                    -1));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueSnapshot.Create(
                    default,
                    Capacity(2),
                    queuedCount: 0,
                    revision: 0));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueSnapshot.Create(
                    QueueId(),
                    Capacity(2),
                    queuedCount: 3,
                    revision: 0));
        Xunit.Assert.Throws<ArgumentException>(
            () => Decide(
                default,
                Snapshot(2, 0, 0),
                expectedRevision: 0,
                Pending(),
                Priority(
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimePriorityClass.Normal,
                    1)));
    }

    [Xunit.Fact]
    public void PriorityOrderingIsDeterministic()
    {
        var background = Priority(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimePriorityClass.Background,
            0);
        var urgentLater = Priority(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimePriorityClass.Urgent,
            20);
        var urgentEarlier = Priority(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimePriorityClass.Urgent,
            10);

        Xunit.Assert.True(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimePriorityOrder.Compare(
                    urgentLater,
                    background) < 0);
        Xunit.Assert.True(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimePriorityOrder.Compare(
                    urgentEarlier,
                    urgentLater) < 0);
        Xunit.Assert.Equal(
            0,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimePriorityOrder.Compare(
                    urgentEarlier,
                    Priority(
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimePriorityClass.Urgent,
                        10)));
    }

    [Xunit.Fact]
    public void QueueSnapshotPreservesExternalAuthority()
    {
        var capacity = Capacity(4);
        var snapshot =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueSnapshot.Create(
                    QueueId(),
                    capacity,
                    queuedCount: 2,
                    revision: 7);

        Xunit.Assert.Equal(QueueId(), snapshot.QueueId);
        Xunit.Assert.Same(capacity, snapshot.Capacity);
        Xunit.Assert.Equal(2, snapshot.QueuedCount);
        Xunit.Assert.Equal(7, snapshot.Revision);
        Xunit.Assert.False(snapshot.IsFull);
    }

    [Xunit.Fact]
    public void PendingRequestIsAdmittedImmutably()
    {
        var snapshot = Snapshot(2, 0, 5);
        var request = Pending();
        var priority = Priority(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimePriorityClass.Normal,
            42);

        var result = Decide(
            AdmissionId(),
            snapshot,
            snapshot.Revision,
            request,
            priority);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmissionStatus.Admitted,
            result.Status);
        Xunit.Assert.Equal(1, result.Snapshot.QueuedCount);
        Xunit.Assert.Equal(6, result.Snapshot.Revision);
        Xunit.Assert.Equal(0, snapshot.QueuedCount);
        Xunit.Assert.Equal(5, snapshot.Revision);
        Xunit.Assert.NotNull(result.Admission);
        Xunit.Assert.Same(request, result.Admission!.Request);
        Xunit.Assert.Same(priority, result.Admission.Priority);
    }

    [Xunit.Fact]
    public void StaleRevisionIsExplicitAndUnchanged()
    {
        var snapshot = Snapshot(2, 0, 4);

        var result = Decide(
            AdmissionId(),
            snapshot,
            expectedRevision: 3,
            Pending(),
            Priority(
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimePriorityClass.Normal,
                1));

        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmissionStatus.StaleQueueRevision,
            result.Status);
        Xunit.Assert.Same(snapshot, result.Snapshot);
        Xunit.Assert.Null(result.Admission);
    }

    [Xunit.Fact]
    public void FullQueueAndInvalidRequestStateAreExplicit()
    {
        var full = Snapshot(1, 1, 0);
        var pending = Pending();
        var priority = Priority(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimePriorityClass.Urgent,
            1);

        var fullResult = Decide(
            AdmissionId(),
            full,
            full.Revision,
            pending,
            priority);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmissionStatus.QueueFull,
            fullResult.Status);
        Xunit.Assert.Same(full, fullResult.Snapshot);

        var failed = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.Finalize(
                pending,
                pending.Revision,
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeRequestState.Failed).Envelope;
        var invalid = Decide(
            AdmissionId(),
            Snapshot(2, 0, 0),
            expectedRevision: 0,
            failed,
            priority);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmissionStatus.InvalidRequestState,
            invalid.Status);
    }

    [Xunit.Fact]
    public void AdmissionPreservesRequestAndPriorityIdentity()
    {
        var request = Pending();
        var priority = Priority(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimePriorityClass.Critical,
            9);
        var snapshot = Snapshot(5, 2, 11);

        var result = Decide(
            AdmissionId(),
            snapshot,
            snapshot.Revision,
            request,
            priority);

        var admission = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmission<RequestPayload>>(
                    result.Admission);
        Xunit.Assert.Equal(AdmissionId(), admission.AdmissionId);
        Xunit.Assert.Equal(QueueId(), admission.QueueId);
        Xunit.Assert.Equal(request.RequestId, admission.RequestId);
        Xunit.Assert.Equal(11, admission.ObservedQueueRevision);
        Xunit.Assert.Equal(12, admission.Snapshot.Revision);
        Xunit.Assert.Same(request, admission.Request);
        Xunit.Assert.Same(priority, admission.Priority);
    }

    [Xunit.Fact]
    public void ContractsDoNotStoreScheduleOrExecuteRequest()
    {
        var request = new CountingRequest();
        var envelope =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.Create(
                    RequestId(),
                    RuntimeId(),
                    OperationId(),
                    CorrelationId(),
                    default,
                    request);
        var result =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmissionFlow.Decide(
                    AdmissionId(),
                    Snapshot(2, 0, 0),
                    expectedQueueRevision: 0,
                    envelope,
                    Priority(
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimePriorityClass.Normal,
                        1));

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(0, request.InvocationCount);
        Xunit.Assert.Same(envelope, result.Admission!.Request);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeQueueAdmissionResult<TRequest> Decide<TRequest>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeAdmissionIdKind> admissionId,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueSnapshot snapshot,
            long expectedRevision,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestEnvelope<TRequest> request,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimePriority priority)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeQueueAdmissionFlow.Decide(
                admissionId,
                snapshot,
                expectedRevision,
                request,
                priority);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeQueueCapacity Capacity(int count) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeQueueCapacity.Create(count);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeQueueSnapshot Snapshot(
            int capacity,
            int queuedCount,
            long revision) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeQueueSnapshot.Create(
                QueueId(),
                Capacity(capacity),
                queuedCount,
                revision);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimePriority Priority(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimePriorityClass priorityClass,
            long sequence) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimePriority.Create(
                priorityClass,
                sequence);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRequestEnvelope<RequestPayload> Pending() =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.Create(
                RequestId(),
                RuntimeId(),
                OperationId(),
                CorrelationId(),
                default,
                new RequestPayload("payload"));

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>
        Id<TKind>(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(
            $"019d0000-0000-7000-8000-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueIdKind>
        QueueId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueIdKind>(3401);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeAdmissionIdKind>
        AdmissionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeAdmissionIdKind>(3402);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestIdKind>
        RequestId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestIdKind>(3403);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeInstanceIdKind>
        RuntimeId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInstanceIdKind>(3404);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeOperationIdKind>
        OperationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeOperationIdKind>(3405);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeCorrelationIdKind>
        CorrelationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCorrelationIdKind>(3406);
}
