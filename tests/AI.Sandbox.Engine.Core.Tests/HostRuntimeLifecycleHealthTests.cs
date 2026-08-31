namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeLifecycleHealthTests
{
    private readonly record struct ValueHealthDetail(string Message) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeHealthDetail;

    private sealed record SealedHealthDetail(string Message) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeHealthDetail;

    private record OpenHealthDetail(string Message) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeHealthDetail;

    private sealed class CountingHealthDetail :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeHealthDetail
    {
        public int InvocationCount { get; private set; }

        public void Invoke() => InvocationCount++;
    }

    [Xunit.Fact]
    public void SnapshotIdsAndInitialStateAreValidated()
    {
        Xunit.Assert.Throws<ArgumentException>(
            () => Create(default, CompositionId()));
        Xunit.Assert.Throws<ArgumentException>(
            () => Create(InstanceId(), default));

        var snapshot = Create(InstanceId(), CompositionId());

        Xunit.Assert.Equal(InstanceId(), snapshot.InstanceId);
        Xunit.Assert.Equal(CompositionId(), snapshot.CompositionId);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleState.Created,
            snapshot.State);
        Xunit.Assert.Equal(0, snapshot.Revision);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeHealthStatus.Unknown,
            snapshot.HealthStatus);
        Xunit.Assert.True(snapshot.HealthProbeId.IsEmpty);
        Xunit.Assert.Null(snapshot.HealthDetail);
    }

    [Xunit.Fact]
    public void LifecycleTransitionsAreExplicitAndImmutable()
    {
        var created = Create(InstanceId(), CompositionId());
        var starting = Transition(
            created,
            created.Revision,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleState.Starting);
        var running = Transition(
            starting.Snapshot,
            starting.Snapshot.Revision,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleState.Running);

        Xunit.Assert.True(starting.Succeeded);
        Xunit.Assert.True(running.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleState.Created,
            created.State);
        Xunit.Assert.Equal(0, created.Revision);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleState.Running,
            running.Snapshot.State);
        Xunit.Assert.Equal(2, running.Snapshot.Revision);
    }

    [Xunit.Fact]
    public void InvalidAndStaleTransitionsAreRejected()
    {
        var created = Create(InstanceId(), CompositionId());

        var stale = Transition(
            created,
            expectedRevision: 1,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleState.Starting);
        var invalid = Transition(
            created,
            created.Revision,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleState.Running);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleTransitionStatus.StaleRevision,
            stale.Status);
        Xunit.Assert.Same(created, stale.Snapshot);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleTransitionStatus.InvalidTransition,
            invalid.Status);
        Xunit.Assert.Same(created, invalid.Snapshot);
    }

    [Xunit.Fact]
    public void HealthObservationRequiresActiveLifecycleAndExactPayload()
    {
        var created = Create(InstanceId(), CompositionId());

        var inactive = Observe(
            created,
            new ValueHealthDetail("not active"));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeHealthUpdateStatus.InvalidLifecycleState,
            inactive.Status);
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycle.ObserveHealth(
                    created,
                    created.Revision,
                    ProbeId(),
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeHealthStatus.Healthy,
                    new OpenHealthDetail("invalid")));
    }

    [Xunit.Fact]
    public void HealthObservationIsImmutableAndRevisioned()
    {
        var running = Running();
        var detail = new SealedHealthDetail("ready");

        var result = Observe(running, detail);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(running.Revision + 1, result.Snapshot.Revision);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeHealthStatus.Healthy,
            result.Snapshot.HealthStatus);
        Xunit.Assert.Equal(ProbeId(), result.Snapshot.HealthProbeId);
        Xunit.Assert.Same(detail, result.Snapshot.HealthDetail);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeHealthStatus.Unknown,
            running.HealthStatus);
    }

    [Xunit.Fact]
    public void HealthStatusProbeAndRevisionValidationAreExplicit()
    {
        var running = Running();

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycle.ObserveHealth(
                    running,
                    running.Revision,
                    default,
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeHealthStatus.Healthy,
                    new ValueHealthDetail("invalid")));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycle.ObserveHealth(
                    running,
                    running.Revision,
                    ProbeId(),
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeHealthStatus.Unknown,
                    new ValueHealthDetail("invalid")));

        var stale = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeLifecycle.ObserveHealth(
                running,
                running.Revision + 1,
                ProbeId(),
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeHealthStatus.Degraded,
                new ValueHealthDetail("stale"));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeHealthUpdateStatus.StaleRevision,
            stale.Status);
        Xunit.Assert.Same(running, stale.Snapshot);
    }

    [Xunit.Fact]
    public void FaultAndShutdownPathIsExplicit()
    {
        var running = Running();
        var observed = Observe(
            running,
            new ValueHealthDetail("unhealthy"),
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeHealthStatus.Unhealthy);
        var faulted = Transition(
            observed.Snapshot,
            observed.Snapshot.Revision,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleState.Faulted);
        var stopping = Transition(
            faulted.Snapshot,
            faulted.Snapshot.Revision,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleState.Stopping);
        var stopped = Transition(
            stopping.Snapshot,
            stopping.Snapshot.Revision,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleState.Stopped);

        Xunit.Assert.True(faulted.Succeeded);
        Xunit.Assert.True(stopping.Succeeded);
        Xunit.Assert.True(stopped.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeHealthStatus.Unknown,
            stopped.Snapshot.HealthStatus);
        Xunit.Assert.Null(stopped.Snapshot.HealthDetail);

        var terminal = Transition(
            stopped.Snapshot,
            stopped.Snapshot.Revision,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleState.Starting);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleTransitionStatus.InvalidTransition,
            terminal.Status);
    }

    [Xunit.Fact]
    public void ContractsDoNotExecuteLifecycleOrHealthSideEffects()
    {
        var running = Running();
        var detail = new CountingHealthDetail();

        var result = Observe(running, detail);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(0, detail.InvocationCount);
        Xunit.Assert.Same(detail, result.Snapshot.HealthDetail);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeLifecycleSnapshot Running()
    {
        var created = Create(InstanceId(), CompositionId());
        var starting = Transition(
            created,
            created.Revision,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleState.Starting);
        return Transition(
            starting.Snapshot,
            starting.Snapshot.Revision,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleState.Running).Snapshot;
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeHealthUpdateResult Observe(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleSnapshot snapshot,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .IHostRuntimeHealthDetail detail,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeHealthStatus status =
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeHealthStatus.Healthy) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeLifecycle.ObserveHealth(
                snapshot,
                snapshot.Revision,
                ProbeId(),
                status,
                detail);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeLifecycleTransitionResult Transition(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleSnapshot snapshot,
            long expectedRevision,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleState targetState) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeLifecycle.Transition(
                snapshot,
                expectedRevision,
                targetState);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeLifecycleSnapshot Create(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeInstanceIdKind> instanceId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeCompositionIdKind> compositionId) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeLifecycle.Create(instanceId, compositionId);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeInstanceIdKind> InstanceId() =>
        Id<global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeInstanceIdKind>(3001);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompositionIdKind> CompositionId() =>
        Id<global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompositionIdKind>(3002);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeHealthProbeIdKind> ProbeId() =>
        Id<global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeHealthProbeIdKind>(3003);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind> Id<TKind>(
        int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(
            $"019d0000-0000-7000-8000-{suffix:D12}");
}
