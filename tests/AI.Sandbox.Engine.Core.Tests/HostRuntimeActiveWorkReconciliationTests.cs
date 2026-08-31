namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeActiveWorkReconciliationTests
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
    public void IdsRevisionsTicksAndCollectionsAreValidated()
    {
        var context = CreateContext(seed: 1);
        var item = Item(context);

        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkItem<RequestPayload>.Create(
                    null!,
                    context.Request,
                    context.Lease));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkItem<RequestPayload>.Create(
                    context.Attempt,
                    null!,
                    context.Lease));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkItem<RequestPayload>.Create(
                    context.Attempt,
                    context.Request,
                    null!));

        Xunit.Assert.Throws<ArgumentException>(
            () => Capture(default, items: new[] { item }));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkFlow.Capture<RequestPayload>(
                    SnapshotId(1),
                    runtimeInstanceId: default,
                    ClockId(),
                    observedTick: 30,
                    revision: 0,
                    items: new[] { item }));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkFlow.Capture<RequestPayload>(
                    SnapshotId(1),
                    RuntimeId(),
                    clockId: default,
                    observedTick: 30,
                    revision: 0,
                    items: new[] { item }));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Capture(
                SnapshotId(1),
                observedTick: -1,
                items: new[] { item }));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Capture(
                SnapshotId(1),
                revision: -1,
                items: new[] { item }));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkFlow.Capture<RequestPayload>(
                    SnapshotId(1),
                    RuntimeId(),
                    ClockId(),
                    observedTick: 30,
                    revision: 0,
                    items: null!));
        Xunit.Assert.Throws<ArgumentException>(
            () => Capture<RequestPayload>(
                SnapshotId(1),
                items:
                new global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeActiveWorkItem<RequestPayload>[]
                {
                    null!,
                }));

        var previous = Snapshot(10, 0, 30, new[] { item });
        var current = Snapshot(11, 1, 31, new[] { item });

        Xunit.Assert.Throws<ArgumentException>(
            () => Reconcile(default, previous, current));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkFlow.Reconcile<RequestPayload>(
                    ReconciliationId(),
                    null!,
                    expectedPreviousRevision: 0,
                    current));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkFlow.Reconcile<RequestPayload>(
                    ReconciliationId(),
                    previous,
                    expectedPreviousRevision: 0,
                    null!));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkFlow.Reconcile(
                    ReconciliationId(),
                    previous,
                    expectedPreviousRevision: -1,
                    current));
    }

    [Xunit.Fact]
    public void SnapshotCapturesEmptyAndDeterministicallyOrdersActiveWork()
    {
        var empty = Snapshot(
            snapshotSuffix: 20,
            revision: 0,
            observedTick: 30,
            items:
                Array.Empty<
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeActiveWorkItem<RequestPayload>>());

        Xunit.Assert.Empty(empty.Items);
        Xunit.Assert.Equal(0, empty.Count);

        var first = CreateContext(seed: 1);
        var second = CreateContext(seed: 2);
        var third = CreateContext(seed: 3);
        var snapshot = Snapshot(
            snapshotSuffix: 21,
            revision: 4,
            observedTick: 30,
            items:
            new[]
            {
                Item(third),
                Item(first),
                Item(second),
            });

        Xunit.Assert.Equal(3, snapshot.Count);
        Xunit.Assert.Equal(
            new[]
            {
                first.Attempt.AttemptId,
                second.Attempt.AttemptId,
                third.Attempt.AttemptId,
            },
            snapshot.Items.Select(static item => item.AttemptId));
        Xunit.Assert.Equal(RuntimeId(), snapshot.RuntimeInstanceId);
        Xunit.Assert.Equal(ClockId(), snapshot.ClockId);
        Xunit.Assert.Equal(30, snapshot.ObservedTick);
        Xunit.Assert.Equal(4, snapshot.Revision);
    }

    [Xunit.Fact]
    public void SnapshotRejectsTooManyAndDuplicateAttempts()
    {
        var context = CreateContext(seed: 1);
        var item = Item(context);

        AssertCaptureStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus.TooManyItems,
            Capture(
                SnapshotId(30),
                items: Enumerable.Repeat(item, 257)));

        var duplicate = Capture(
            SnapshotId(31),
            items:
            new[]
            {
                item,
                item,
            });

        AssertCaptureStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus.DuplicateAttempt,
            duplicate);
        Xunit.Assert.Equal(
            context.Attempt.AttemptId,
            duplicate.RelatedAttemptId);
    }

    [Xunit.Fact]
    public void SnapshotLineageRuntimeWorkerAndClockMismatchesAreExplicit()
    {
        var first = CreateContext(seed: 1);
        var second = CreateContext(seed: 2);

        AssertCaptureStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus.AttemptRequestMismatch,
            Capture(
                SnapshotId(40),
                items:
                new[]
                {
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeActiveWorkItem<RequestPayload>.Create(
                            first.Attempt,
                            second.Request,
                            first.Lease),
                }));

        var otherLease =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Acquire(
                    OtherLeaseId(),
                    first.Lease.WorkerId,
                    first.Admission,
                    first.Lease.ClockId,
                    acquiredTick: 10,
                    durationTicks: 100);

        AssertCaptureStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus.AttemptLeaseMismatch,
            Capture(
                SnapshotId(41),
                items:
                new[]
                {
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeActiveWorkItem<RequestPayload>.Create(
                            first.Attempt,
                            first.Request,
                            otherLease),
                }));

        var otherWorkerLease =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Acquire(
                    first.Lease.LeaseId,
                    OtherWorkerId(),
                    first.Admission,
                    first.Lease.ClockId,
                    acquiredTick: 10,
                    durationTicks: 100);

        AssertCaptureStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus.AttemptWorkerMismatch,
            Capture(
                SnapshotId(42),
                items:
                new[]
                {
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeActiveWorkItem<RequestPayload>.Create(
                            first.Attempt,
                            first.Request,
                            otherWorkerLease),
                }));

        AssertCaptureStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus.RuntimeMismatch,
            Capture(
                SnapshotId(43),
                runtimeInstanceId: OtherRuntimeId(),
                items: new[] { Item(first) }));

        AssertCaptureStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus.ClockMismatch,
            Capture(
                SnapshotId(44),
                clockId: OtherClockId(),
                items: new[] { Item(first) }));
    }

    [Xunit.Fact]
    public void SnapshotStateRevisionAndTimeBoundariesAreExplicit()
    {
        var context = CreateContext(seed: 1);
        var cancelled =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.RequestCancellation(
                    context.Request,
                    context.Request.Revision,
                    new CancellationReason("cancel")).Envelope;
        var failed =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.Finalize(
                    context.Request,
                    context.Request.Revision,
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRequestState.Failed).Envelope;
        var released =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Release(
                    context.Lease,
                    context.Lease.Revision,
                    context.Lease.WorkerId).Lease;

        var cancellationSnapshot = Snapshot(
            snapshotSuffix: 50,
            revision: 0,
            observedTick: 30,
            items:
            new[]
            {
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeActiveWorkItem<RequestPayload>.Create(
                        context.Attempt,
                        cancelled,
                        context.Lease),
            });
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.CancellationRequested,
            cancellationSnapshot.Items[0].Request.State);

        AssertCaptureStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus.InvalidRequestState,
            Capture(
                SnapshotId(51),
                items:
                new[]
                {
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeActiveWorkItem<RequestPayload>.Create(
                            context.Attempt,
                            failed,
                            context.Lease),
                }));

        AssertCaptureStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus.InvalidLeaseState,
            Capture(
                SnapshotId(52),
                items:
                new[]
                {
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeActiveWorkItem<RequestPayload>.Create(
                            context.Attempt,
                            context.Request,
                            released),
                }));

        AssertCaptureStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus.BeforeAcknowledgement,
            Capture(
                SnapshotId(53),
                observedTick: context.Attempt.AcknowledgedTick - 1,
                items: new[] { Item(context) }));

        AssertCaptureStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus.LeaseExpired,
            Capture(
                SnapshotId(54),
                observedTick: context.Lease.ExpiresTick,
                items: new[] { Item(context) }));

        var renewedContext = CreateContext(
            seed: 2,
            renewBeforeAcknowledgement: true);
        AssertCaptureStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus.LeaseRevisionRegressed,
            Capture(
                SnapshotId(55),
                items:
                new[]
                {
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeActiveWorkItem<RequestPayload>.Create(
                            renewedContext.Attempt,
                            renewedContext.Request,
                            renewedContext.OriginalLease),
                }));
    }

    [Xunit.Fact]
    public void ReconciliationComputesAddedRetainedAndRemovedAttempts()
    {
        var first = CreateContext(seed: 1);
        var second = CreateContext(seed: 2);
        var third = CreateContext(seed: 3);

        var previous = Snapshot(
            snapshotSuffix: 60,
            revision: 7,
            observedTick: 30,
            items:
            new[]
            {
                Item(second),
                Item(first),
            });
        var current = Snapshot(
            snapshotSuffix: 61,
            revision: 8,
            observedTick: 40,
            items:
            new[]
            {
                Item(third),
                Item(second),
            });

        var result = Reconcile(
            ReconciliationId(),
            previous,
            current);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus.Reconciled,
            result.Status);

        var reconciliation = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkReconciliation<RequestPayload>>(
                    result.Reconciliation);
        Xunit.Assert.Same(previous, reconciliation.PreviousSnapshot);
        Xunit.Assert.Same(current, reconciliation.CurrentSnapshot);
        Xunit.Assert.Equal(
            new[] { third.Attempt.AttemptId },
            reconciliation.AddedAttemptIds);
        Xunit.Assert.Equal(
            new[] { second.Attempt.AttemptId },
            reconciliation.RetainedAttemptIds);
        Xunit.Assert.Equal(
            new[] { first.Attempt.AttemptId },
            reconciliation.RemovedAttemptIds);
    }

    [Xunit.Fact]
    public void ReconciliationSnapshotMismatchesAreExplicit()
    {
        var context = CreateContext(seed: 1);
        var item = Item(context);
        var previous = Snapshot(70, 10, 30, new[] { item });

        AssertReconciliationStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus.StaleSnapshotRevision,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkFlow.Reconcile(
                    ReconciliationId(),
                    previous,
                    expectedPreviousRevision: 11,
                    Snapshot(71, 11, 31, new[] { item })));

        AssertReconciliationStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus.NonSequentialSnapshotRevision,
            Reconcile(
                ReconciliationId(),
                previous,
                Snapshot(72, 12, 31, new[] { item })));

        AssertReconciliationStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus.ObservationTickRegressed,
            Reconcile(
                ReconciliationId(),
                previous,
                Snapshot(73, 11, 29, new[] { item })));

        AssertReconciliationStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus.RuntimeMismatch,
            Reconcile(
                ReconciliationId(),
                previous,
                Snapshot(
                    74,
                    11,
                    31,
                    Array.Empty<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeActiveWorkItem<RequestPayload>>(),
                    runtimeInstanceId: OtherRuntimeId())));

        AssertReconciliationStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus.ClockMismatch,
            Reconcile(
                ReconciliationId(),
                previous,
                Snapshot(
                    75,
                    11,
                    31,
                    Array.Empty<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeActiveWorkItem<RequestPayload>>(),
                    clockId: OtherClockId())));
    }

    [Xunit.Fact]
    public void ReconciliationRejectsChangedOrRegressedRetainedAuthority()
    {
        var first = CreateContext(seed: 1);
        var changed = CreateContext(
            seed: 2,
            attemptSuffix: AttemptSuffix(seed: 1));
        var previous = Snapshot(80, 0, 30, new[] { Item(first) });
        var changedCurrent = Snapshot(81, 1, 31, new[] { Item(changed) });

        var lineage = Reconcile(
            ReconciliationId(),
            previous,
            changedCurrent);

        AssertReconciliationStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus
                .RetainedAttemptLineageMismatch,
            lineage);
        Xunit.Assert.Equal(
            first.Attempt.AttemptId,
            lineage.RelatedAttemptId);

        var cancellation =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.RequestCancellation(
                    first.Request,
                    first.Request.Revision,
                    new CancellationReason("cancel")).Envelope;
        var advancedPrevious = Snapshot(
            82,
            2,
            32,
            new[]
            {
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeActiveWorkItem<RequestPayload>.Create(
                        first.Attempt,
                        cancellation,
                        first.Lease),
            });
        var regressedCurrent = Snapshot(
            83,
            3,
            33,
            new[] { Item(first) });

        AssertReconciliationStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus
                .RetainedRequestRevisionRegressed,
            Reconcile(
                ReconciliationId(),
                advancedPrevious,
                regressedCurrent));

        var renewed =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Renew(
                    first.Lease,
                    first.Lease.Revision,
                    first.Lease.ClockId,
                    observedTick: 30,
                    durationTicks: 100).Lease;
        var renewedPrevious = Snapshot(
            84,
            4,
            31,
            new[]
            {
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeActiveWorkItem<RequestPayload>.Create(
                        first.Attempt,
                        first.Request,
                        renewed),
            });
        var leaseRegressedCurrent = Snapshot(
            85,
            5,
            32,
            new[] { Item(first) });

        AssertReconciliationStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkStatus
                .RetainedLeaseRevisionRegressed,
            Reconcile(
                ReconciliationId(),
                renewedPrevious,
                leaseRegressedCurrent));
    }

    [Xunit.Fact]
    public void ContractsDefensivelyCopyCollectionsWithoutInferringOutcomes()
    {
        var payload = new CountingRequest();
        var first = CreateContext(1, payload);
        var second = CreateContext(2, payload);
        var mutable = new List<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkItem<CountingRequest>>
        {
            Item(first),
        };

        var previous = Snapshot(
            snapshotSuffix: 90,
            revision: 0,
            observedTick: 30,
            items: mutable);
        mutable.Add(Item(second));

        Xunit.Assert.Single(previous.Items);
        Xunit.Assert.Equal(0, payload.InvocationCount);

        var current = Snapshot(
            snapshotSuffix: 91,
            revision: 1,
            observedTick: 31,
            items:
                Array.Empty<
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeActiveWorkItem<CountingRequest>>());
        var reconciliation =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkFlow.Reconcile(
                    ReconciliationId(),
                    previous,
                    previous.Revision,
                    current).Reconciliation!;

        Xunit.Assert.Single(reconciliation.RemovedAttemptIds);
        Xunit.Assert.Empty(reconciliation.AddedAttemptIds);
        Xunit.Assert.Empty(reconciliation.RetainedAttemptIds);
        Xunit.Assert.Equal(0, payload.InvocationCount);
        Xunit.Assert.False(previous.Items[0].Request.IsTerminal);
        Xunit.Assert.True(previous.Items[0].Lease.IsActive);
    }

    private static void AssertCaptureStatus<TRequest>(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkStatus expected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkSnapshotResult<TRequest> result)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
    {
        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(expected, result.Status);
        Xunit.Assert.Null(result.Snapshot);
    }

    private static void AssertReconciliationStatus<TRequest>(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkStatus expected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkReconciliationResult<TRequest> result)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
    {
        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(expected, result.Status);
        Xunit.Assert.Null(result.Reconciliation);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeActiveWorkSnapshotResult<TRequest>
        Capture<TRequest>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeActiveWorkSnapshotIdKind> snapshotId,
            IEnumerable<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeActiveWorkItem<TRequest>> items,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeInstanceIdKind>? runtimeInstanceId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeClockIdKind>? clockId = null,
            long observedTick = 30,
            long revision = 0)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkFlow.Capture(
                snapshotId,
                runtimeInstanceId ?? RuntimeId(),
                clockId ?? ClockId(),
                observedTick,
                revision,
                items);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeActiveWorkSnapshot<TRequest>
        Snapshot<TRequest>(
            int snapshotSuffix,
            long revision,
            long observedTick,
            IEnumerable<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeActiveWorkItem<TRequest>> items,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeInstanceIdKind>? runtimeInstanceId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeClockIdKind>? clockId = null)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest =>
        Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkSnapshot<TRequest>>(
                    Capture(
                        SnapshotId(snapshotSuffix),
                        items,
                        runtimeInstanceId,
                        clockId,
                        observedTick,
                        revision).Snapshot);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeActiveWorkReconciliationResult<TRequest>
        Reconcile<TRequest>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeActiveWorkReconciliationIdKind>
                reconciliationId,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkSnapshot<TRequest> previous,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkSnapshot<TRequest> current)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkFlow.Reconcile(
                reconciliationId,
                previous,
                previous.Revision,
                current);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeActiveWorkItem<TRequest>
        Item<TRequest>(ActiveWorkContext<TRequest> context)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkItem<TRequest>.Create(
                context.Attempt,
                context.Request,
                context.Lease);

    private static ActiveWorkContext<RequestPayload> CreateContext(
        int seed,
        int? attemptSuffix = null,
        bool renewBeforeAcknowledgement = false) =>
        CreateContext(
            seed,
            new RequestPayload($"payload-{seed}"),
            attemptSuffix,
            renewBeforeAcknowledgement);

    private static ActiveWorkContext<TRequest> CreateContext<TRequest>(
        int seed,
        TRequest payload,
        int? attemptSuffix = null,
        bool renewBeforeAcknowledgement = false)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest
    {
        var pending =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestFlow.Create(
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeRequestIdKind>(Suffix(seed, 1)),
                    RuntimeId(),
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeOperationIdKind>(Suffix(seed, 2)),
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeCorrelationIdKind>(Suffix(seed, 3)),
                    default,
                    payload);
        var queueSnapshot =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueSnapshot.Create(
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeQueueIdKind>(Suffix(seed, 4)),
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeQueueCapacity.Create(8),
                    queuedCount: 0,
                    revision: 10);
        var priority =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimePriority.Create(
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimePriorityClass.Urgent,
                    sequence: seed);
        var admissionResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmissionFlow.Decide(
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeAdmissionIdKind>(Suffix(seed, 5)),
                    queueSnapshot,
                    queueSnapshot.Revision,
                    pending,
                    priority);
        var admission = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueAdmission<TRequest>>(
                    admissionResult.Admission);
        var originalLease =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Acquire(
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeLeaseIdKind>(Suffix(seed, 6)),
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeWorkerIdKind>(Suffix(seed, 7)),
                    admission,
                    ClockId(),
                    acquiredTick: 10,
                    durationTicks: 100);
        var lease = renewBeforeAcknowledgement
            ? global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkLeaseFlow.Renew(
                    originalLease,
                    originalLease.Revision,
                    originalLease.ClockId,
                    observedTick: 15,
                    durationTicks: 100).Lease
            : originalLease;
        var selectionResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelectionFlow.Select(
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeDispatchSelectionIdKind>(
                                Suffix(seed, 8)),
                    admissionResult.Snapshot,
                    admissionResult.Snapshot.Revision,
                    originalLease,
                    ClockId(),
                    observedTick: 20,
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeDispatchIdKind>(Suffix(seed, 9)),
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeRouteIdKind>(Suffix(seed, 10)),
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeEndpointIdKind>(Suffix(seed, 11)),
                    attemptNumber: 1);
        var selection = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchSelection<TRequest>>(
                    selectionResult.Selection);
        var acknowledgement =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDispatchAcknowledgementFlow.Acknowledge(
                    Id<
                        global::AI.Sandbox.Engine.Core.HostRuntime
                            .HostRuntimeAttemptIdKind>(
                                attemptSuffix ?? AttemptSuffix(seed)),
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

        return new ActiveWorkContext<TRequest>(
            admission,
            pending,
            originalLease,
            lease,
            attempt);
    }

    private static int Suffix(int seed, int offset) =>
        420000 + (seed * 100) + offset;

    private static int AttemptSuffix(int seed) => Suffix(seed, 12);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>
        Id<TKind>(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(
            $"019d0000-0000-7000-8000-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkSnapshotIdKind>
        SnapshotId(int suffix) => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkSnapshotIdKind>(429000 + suffix);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkReconciliationIdKind>
        ReconciliationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkReconciliationIdKind>(429901);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeInstanceIdKind>
        RuntimeId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInstanceIdKind>(429902);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeInstanceIdKind>
        OtherRuntimeId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInstanceIdKind>(429903);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        ClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(429904);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        OtherClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(429905);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLeaseIdKind>
        OtherLeaseId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLeaseIdKind>(429906);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeWorkerIdKind>
        OtherWorkerId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeWorkerIdKind>(429907);

    private sealed record CancellationReason(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime
            .IHostRuntimeCancellationReason;

    private sealed record ActiveWorkContext<TRequest>(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeQueueAdmission<TRequest> Admission,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestEnvelope<TRequest> Request,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeWorkLease<TRequest> OriginalLease,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeWorkLease<TRequest> Lease,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeInFlightAttempt<TRequest> Attempt)
        where TRequest :
            global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;
}
