namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeLeaseWorkerOwnershipTests
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
    public void IdsTicksAndDurationsAreValidated()
    {
        var admission = Admission(Pending());
        Xunit.Assert.Throws<ArgumentException>(() => Acquire(default, WorkerId(), admission, ClockId(), 0, 10));
        Xunit.Assert.Throws<ArgumentException>(() => Acquire(LeaseId(), default, admission, ClockId(), 0, 10));
        Xunit.Assert.Throws<ArgumentException>(() => Acquire(LeaseId(), WorkerId(), admission, default, 0, 10));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => Acquire(LeaseId(), WorkerId(), admission, ClockId(), -1, 10));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => Acquire(LeaseId(), WorkerId(), admission, ClockId(), 0, 0));
    }

    [Xunit.Fact]
    public void AcquisitionPreservesAdmissionAndOwner()
    {
        var admission = Admission(Pending());
        var lease = Acquire(LeaseId(), WorkerId(), admission, ClockId(), 100, 25);
        Xunit.Assert.Equal(LeaseId(), lease.LeaseId);
        Xunit.Assert.Equal(WorkerId(), lease.WorkerId);
        Xunit.Assert.Same(admission, lease.Admission);
        Xunit.Assert.Equal(ClockId(), lease.ClockId);
        Xunit.Assert.Equal(100, lease.AcquiredTick);
        Xunit.Assert.Equal(125, lease.ExpiresTick);
        Xunit.Assert.Equal(0, lease.Revision);
        Xunit.Assert.True(lease.IsActive);
    }

    [Xunit.Fact]
    public void RenewalExtendsExclusiveExpiryImmutably()
    {
        var lease = Acquire(LeaseId(), WorkerId(), Admission(Pending()), ClockId(), 10, 20);
        var result = global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkLeaseFlow.Renew(
            lease, lease.Revision, ClockId(), observedTick: 20, durationTicks: 40);
        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseTransitionStatus.Renewed, result.Status);
        Xunit.Assert.Equal(60, result.Lease.ExpiresTick);
        Xunit.Assert.Equal(1, result.Lease.Revision);
        Xunit.Assert.Equal(30, lease.ExpiresTick);
        Xunit.Assert.Equal(0, lease.Revision);
    }

    [Xunit.Fact]
    public void ReleaseRequiresExactWorker()
    {
        var lease = Acquire(LeaseId(), WorkerId(), Admission(Pending()), ClockId(), 0, 50);
        var mismatch = global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkLeaseFlow.Release(
            lease, lease.Revision, OtherWorkerId());
        Xunit.Assert.Equal(global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseTransitionStatus.WorkerMismatch, mismatch.Status);
        Xunit.Assert.Same(lease, mismatch.Lease);

        var released = global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkLeaseFlow.Release(
            lease, lease.Revision, WorkerId());
        Xunit.Assert.True(released.Succeeded);
        Xunit.Assert.Equal(global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseState.Released, released.Lease.State);
        Xunit.Assert.Equal(1, released.Lease.Revision);
    }

    [Xunit.Fact]
    public void ExpiryRequiresDueTickAndMatchingClock()
    {
        var lease = Acquire(LeaseId(), WorkerId(), Admission(Pending()), ClockId(), 10, 20);
        var early = global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkLeaseFlow.Expire(
            lease, lease.Revision, ClockId(), observedTick: 29);
        Xunit.Assert.Equal(global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseTransitionStatus.NotExpired, early.Status);
        Xunit.Assert.Same(lease, early.Lease);

        var wrongClock = global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkLeaseFlow.Expire(
            lease, lease.Revision, OtherClockId(), observedTick: 30);
        Xunit.Assert.Equal(global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseTransitionStatus.ClockMismatch, wrongClock.Status);

        var expired = global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkLeaseFlow.Expire(
            lease, lease.Revision, ClockId(), observedTick: 30);
        Xunit.Assert.True(expired.Succeeded);
        Xunit.Assert.Equal(global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseState.Expired, expired.Lease.State);
    }

    [Xunit.Fact]
    public void StaleRevisionAndInvalidStateAreExplicit()
    {
        var lease = Acquire(LeaseId(), WorkerId(), Admission(Pending()), ClockId(), 0, 100);
        var stale = global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkLeaseFlow.Renew(
            lease, expectedRevision: 1, ClockId(), observedTick: 1, durationTicks: 10);
        Xunit.Assert.Equal(global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseTransitionStatus.StaleRevision, stale.Status);

        var released = global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkLeaseFlow.Release(
            lease, lease.Revision, WorkerId()).Lease;
        var invalid = global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkLeaseFlow.Renew(
            released, released.Revision, ClockId(), observedTick: 2, durationTicks: 10);
        Xunit.Assert.Equal(global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseTransitionStatus.InvalidState, invalid.Status);
    }

    [Xunit.Fact]
    public void LeasePreservesRequestQueueAndPriorityAuthority()
    {
        var request = Pending();
        var admission = Admission(request);
        var lease = Acquire(LeaseId(), WorkerId(), admission, ClockId(), 0, 10);
        Xunit.Assert.Equal(request.RequestId, lease.RequestId);
        Xunit.Assert.Equal(QueueId(), lease.QueueId);
        Xunit.Assert.Same(request, lease.Admission.Request);
        Xunit.Assert.Equal(global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimePriorityClass.Urgent, lease.Admission.Priority.Class);
    }

    [Xunit.Fact]
    public void ContractsDoNotWaitScheduleOrExecuteWorker()
    {
        var payload = new CountingRequest();
        var request = global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestFlow.Create(
            RequestId(), RuntimeId(), OperationId(), CorrelationId(), default, payload);
        var lease = Acquire(LeaseId(), WorkerId(), Admission(request), ClockId(), 0, 10);
        Xunit.Assert.Equal(0, payload.InvocationCount);
        Xunit.Assert.Same(request, lease.Admission.Request);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkLease<TRequest> Acquire<TRequest>(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseIdKind> leaseId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkerIdKind> workerId,
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueAdmission<TRequest> admission,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind> clockId,
        long acquiredTick,
        long durationTicks)
        where TRequest : global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest =>
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkLeaseFlow.Acquire(
            leaseId, workerId, admission, clockId, acquiredTick, durationTicks);

    private static global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueAdmission<TRequest> Admission<TRequest>(
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestEnvelope<TRequest> request)
        where TRequest : global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
    {
        var snapshot = global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueSnapshot.Create(
            QueueId(), global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueCapacity.Create(4), 0, 0);
        var result = global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueAdmissionFlow.Decide(
            AdmissionId(), snapshot, 0, request,
            global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimePriority.Create(
                global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimePriorityClass.Urgent, 7));
        return Xunit.Assert.IsType<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueAdmission<TRequest>>(result.Admission);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestEnvelope<RequestPayload> Pending() =>
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestFlow.Create(
            RequestId(), RuntimeId(), OperationId(), CorrelationId(), default, new RequestPayload("payload"));

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind> Id<TKind>(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse($"019d0000-0000-7000-8000-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseIdKind> LeaseId() => Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseIdKind>(3501);
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkerIdKind> WorkerId() => Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkerIdKind>(3502);
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkerIdKind> OtherWorkerId() => Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkerIdKind>(3503);
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind> ClockId() => Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>(3504);
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind> OtherClockId() => Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>(3505);
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueIdKind> QueueId() => Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueIdKind>(3506);
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeAdmissionIdKind> AdmissionId() => Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeAdmissionIdKind>(3507);
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestIdKind> RequestId() => Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestIdKind>(3508);
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeInstanceIdKind> RuntimeId() => Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeInstanceIdKind>(3509);
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeOperationIdKind> OperationId() => Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeOperationIdKind>(3510);
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeCorrelationIdKind> CorrelationId() => Id<global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeCorrelationIdKind>(3511);
}
