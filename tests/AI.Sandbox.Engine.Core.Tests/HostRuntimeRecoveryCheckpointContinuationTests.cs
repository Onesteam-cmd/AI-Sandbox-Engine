namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoveryCheckpointContinuationTests
{
    private readonly record struct RequestPayload(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;

    private sealed record RecoveryWorldState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    private sealed class Capability :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCapability
    {
    }

    [Xunit.Fact]
    public void IdsTicksRevisionsAndArgumentsAreValidated()
    {
        var context = CreateContext();

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryFlow.CaptureCheckpoint<RequestPayload>(
                    default,
                    context.LifecycleSnapshot,
                    context.Composition,
                    context.QueueSnapshot,
                    context.ActiveWorkSnapshot,
                    context.WorldDocument,
                    capturedTick: 40,
                    revision: 0));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryFlow.CaptureCheckpoint<RequestPayload>(
                    CheckpointId(),
                    null!,
                    context.Composition,
                    context.QueueSnapshot,
                    context.ActiveWorkSnapshot,
                    context.WorldDocument,
                    capturedTick: 40,
                    revision: 0));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryFlow.CaptureCheckpoint<RequestPayload>(
                    CheckpointId(),
                    context.LifecycleSnapshot,
                    null!,
                    context.QueueSnapshot,
                    context.ActiveWorkSnapshot,
                    context.WorldDocument,
                    capturedTick: 40,
                    revision: 0));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryFlow.CaptureCheckpoint<RequestPayload>(
                    CheckpointId(),
                    context.LifecycleSnapshot,
                    context.Composition,
                    null!,
                    context.ActiveWorkSnapshot,
                    context.WorldDocument,
                    capturedTick: 40,
                    revision: 0));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryFlow.CaptureCheckpoint<RequestPayload>(
                    CheckpointId(),
                    context.LifecycleSnapshot,
                    context.Composition,
                    context.QueueSnapshot,
                    null!,
                    context.WorldDocument,
                    capturedTick: 40,
                    revision: 0));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryFlow.CaptureCheckpoint<RequestPayload>(
                    CheckpointId(),
                    context.LifecycleSnapshot,
                    context.Composition,
                    context.QueueSnapshot,
                    context.ActiveWorkSnapshot,
                    null!,
                    capturedTick: 40,
                    revision: 0));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Capture(context, capturedTick: -1));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Capture(context, revision: -1));

        var checkpoint = Checkpoint(context);

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryFlow.Continue<
                    RequestPayload,
                    RecoveryWorldState>(
                        default,
                        checkpoint,
                        expectedCheckpointRevision: checkpoint.Revision,
                        context.Persistence.Restore(context.WorldDocument),
                        continuedTick: 40));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryFlow.Continue<
                    RequestPayload,
                    RecoveryWorldState>(
                        ContinuationId(),
                        null!,
                        expectedCheckpointRevision: 0,
                        context.Persistence.Restore(context.WorldDocument),
                        continuedTick: 40));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryFlow.Continue<
                    RequestPayload,
                    RecoveryWorldState>(
                        ContinuationId(),
                        checkpoint,
                        expectedCheckpointRevision: checkpoint.Revision,
                        null!,
                        continuedTick: 40));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Continue(
                checkpoint,
                context.Persistence.Restore(context.WorldDocument),
                expectedCheckpointRevision: -1));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => Continue(
                checkpoint,
                context.Persistence.Restore(context.WorldDocument),
                continuedTick: -1));
    }

    [Xunit.Fact]
    public void CheckpointCapturesExactImmutableAuthorities()
    {
        var context = CreateContext();
        var result = Capture(context, capturedTick: 40, revision: 7);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryStatus.CheckpointCreated,
            result.Status);

        var checkpoint = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpoint<RequestPayload>>(
                    result.Checkpoint);
        Xunit.Assert.Same(context.LifecycleSnapshot, checkpoint.LifecycleSnapshot);
        Xunit.Assert.Same(context.Composition, checkpoint.Composition);
        Xunit.Assert.Same(context.QueueSnapshot, checkpoint.QueueSnapshot);
        Xunit.Assert.Same(
            context.ActiveWorkSnapshot,
            checkpoint.ActiveWorkSnapshot);
        Xunit.Assert.Same(context.WorldDocument, checkpoint.WorldSnapshotDocument);
        Xunit.Assert.Equal(RuntimeId(), checkpoint.RuntimeInstanceId);
        Xunit.Assert.Equal(ClockId(), checkpoint.ClockId);
        Xunit.Assert.Equal(40, checkpoint.CapturedTick);
        Xunit.Assert.Equal(7, checkpoint.Revision);
    }

    [Xunit.Fact]
    public void CheckpointRuntimeAndCompositionMismatchesAreExplicit()
    {
        var context = CreateContext();
        var otherActive = ActiveWorkSnapshot(
            runtimeInstanceId: OtherRuntimeId());
        var runtimeResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryFlow.CaptureCheckpoint(
                    CheckpointId(),
                    context.LifecycleSnapshot,
                    context.Composition,
                    context.QueueSnapshot,
                    otherActive,
                    context.WorldDocument,
                    capturedTick: 40,
                    revision: 0);

        AssertCheckpointStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryStatus.RuntimeMismatch,
            runtimeResult);

        var otherComposition = Composition(OtherCompositionId());
        var compositionResult =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryFlow.CaptureCheckpoint(
                    CheckpointId(),
                    context.LifecycleSnapshot,
                    otherComposition,
                    context.QueueSnapshot,
                    context.ActiveWorkSnapshot,
                    context.WorldDocument,
                    capturedTick: 40,
                    revision: 0);

        AssertCheckpointStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryStatus.CompositionMismatch,
            compositionResult);
    }

    [Xunit.Fact]
    public void CheckpointTimeAndWorldDocumentBoundariesAreExplicit()
    {
        var context = CreateContext();

        AssertCheckpointStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryStatus.BeforeActiveWorkObservation,
            Capture(context, capturedTick: 29));

        var unsupported = global::AI.Sandbox.Engine.Core.Persistence
            .WorldSnapshotDocument.Create(
                global::AI.Sandbox.Engine.Core.Persistence
                    .SnapshotFormatVersion.From(2),
                context.WorldDocument.SchemaId,
                context.WorldDocument.SchemaVersion,
                context.WorldDocument.WorldId,
                context.WorldDocument.WorldStateVersion,
                context.WorldDocument.SimulationTick,
                context.WorldDocument.Payload,
                context.WorldDocument.Checksum);
        AssertCheckpointStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryStatus.UnsupportedWorldSnapshotFormat,
            Capture(context with { WorldDocument = unsupported }));

        var differentPayload =
            global::AI.Sandbox.Engine.Core.Persistence.SnapshotPayload.From(
                new byte[] { 1, 2, 3 });
        var invalidChecksum = global::AI.Sandbox.Engine.Core.Persistence
            .WorldSnapshotDocument.Create(
                context.WorldDocument.FormatVersion,
                context.WorldDocument.SchemaId,
                context.WorldDocument.SchemaVersion,
                context.WorldDocument.WorldId,
                context.WorldDocument.WorldStateVersion,
                context.WorldDocument.SimulationTick,
                differentPayload,
                context.WorldDocument.Checksum);
        AssertCheckpointStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryStatus.WorldSnapshotChecksumMismatch,
            Capture(context with { WorldDocument = invalidChecksum }));
    }

    [Xunit.Fact]
    public void ContinuationRestoresMatchingWorldAndAdvancesRevision()
    {
        var context = CreateContext();
        var checkpoint = Checkpoint(context, revision: 4);
        var restored = context.Persistence.Restore(context.WorldDocument);
        var result = Continue(
            checkpoint,
            restored,
            continuedTick: 45);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryStatus.ContinuationCreated,
            result.Status);
        Xunit.Assert.Null(result.RestoreStatus);

        var continuation = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuation<
                    RequestPayload,
                    RecoveryWorldState>>(result.Continuation);
        Xunit.Assert.Same(checkpoint, continuation.Checkpoint);
        Xunit.Assert.Same(restored.Snapshot, continuation.RestoredWorldSnapshot);
        Xunit.Assert.Equal(45, continuation.ContinuedTick);
        Xunit.Assert.Equal(5, continuation.Revision);
    }

    [Xunit.Fact]
    public void ContinuationStaleRevisionAndTickRegressionAreExplicit()
    {
        var context = CreateContext();
        var checkpoint = Checkpoint(context, capturedTick: 40, revision: 3);
        var restored = context.Persistence.Restore(context.WorldDocument);

        AssertContinuationStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryStatus.StaleCheckpointRevision,
            Continue(
                checkpoint,
                restored,
                expectedCheckpointRevision: 4));

        AssertContinuationStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryStatus.ContinuationTickRegressed,
            Continue(
                checkpoint,
                restored,
                continuedTick: 39));
    }

    [Xunit.Fact]
    public void ContinuationRestoreFailureIsExplicit()
    {
        var context = CreateContext();
        var checkpoint = Checkpoint(context);
        var invalid = InvalidChecksumDocument(context.WorldDocument);
        var failedRestore = context.Persistence.Restore(invalid);

        var result = Continue(checkpoint, failedRestore);

        AssertContinuationStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryStatus.RestoreFailed,
            result);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotRestoreStatus.ChecksumMismatch,
            result.RestoreStatus);
    }

    [Xunit.Fact]
    public void ContinuationWorldIdentityVersionAndTickMismatchesAreExplicit()
    {
        var context = CreateContext();
        var checkpoint = Checkpoint(context);

        var otherWorld = RecreateDocument(
            context.WorldDocument,
            worldId: OtherWorldId());
        AssertContinuationStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryStatus.RestoredWorldMismatch,
            Continue(
                checkpoint,
                context.Persistence.Restore(otherWorld)));

        var otherVersion = RecreateDocument(
            context.WorldDocument,
            worldStateVersion:
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateVersion.From(1));
        AssertContinuationStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryStatus.RestoredWorldVersionMismatch,
            Continue(
                checkpoint,
                context.Persistence.Restore(otherVersion)));

        var otherTick = RecreateDocument(
            context.WorldDocument,
            simulationTick: context.WorldDocument.SimulationTick + 1);
        AssertContinuationStatus(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryStatus.RestoredSimulationTickMismatch,
            Continue(
                checkpoint,
                context.Persistence.Restore(otherTick)));
    }

    [Xunit.Fact]
    public void ContractsPreserveAuthorityWithoutStorageOrAutomaticRestart()
    {
        var context = CreateContext();
        var checkpoint = Checkpoint(context);
        var result = Continue(
            checkpoint,
            context.Persistence.Restore(context.WorldDocument));

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Same(
            context.LifecycleSnapshot,
            result.Continuation!.Checkpoint.LifecycleSnapshot);
        Xunit.Assert.Same(
            context.QueueSnapshot,
            result.Continuation.Checkpoint.QueueSnapshot);
        Xunit.Assert.Same(
            context.ActiveWorkSnapshot,
            result.Continuation.Checkpoint.ActiveWorkSnapshot);
        Xunit.Assert.Equal(0, context.ActiveWorkSnapshot.Count);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeLifecycleState.Created,
            context.LifecycleSnapshot.State);
    }

    private static void AssertCheckpointStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRecoveryStatus
            expected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointResult<RequestPayload> result)
    {
        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(expected, result.Status);
        Xunit.Assert.Null(result.Checkpoint);
    }

    private static void AssertContinuationStatus(
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRecoveryStatus
            expected,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuationResult<
                RequestPayload,
                RecoveryWorldState> result)
    {
        Xunit.Assert.False(result.Succeeded);
        Xunit.Assert.Equal(expected, result.Status);
        Xunit.Assert.Null(result.Continuation);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpointResult<RequestPayload>
        Capture(
            RecoveryContext context,
            long capturedTick = 40,
            long revision = 0) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryFlow.CaptureCheckpoint(
                CheckpointId(),
                context.LifecycleSnapshot,
                context.Composition,
                context.QueueSnapshot,
                context.ActiveWorkSnapshot,
                context.WorldDocument,
                capturedTick,
                revision);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryCheckpoint<RequestPayload>
        Checkpoint(
            RecoveryContext context,
            long capturedTick = 40,
            long revision = 0) =>
        Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpoint<RequestPayload>>(
                    Capture(
                        context,
                        capturedTick: capturedTick,
                        revision: revision).Checkpoint);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRecoveryContinuationResult<
            RequestPayload,
            RecoveryWorldState>
        Continue(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpoint<RequestPayload> checkpoint,
            global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotRestoreResult<RecoveryWorldState> restoreResult,
            long? expectedCheckpointRevision = null,
            long continuedTick = 40) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryFlow.Continue<
                RequestPayload,
                RecoveryWorldState>(
                    ContinuationId(),
                    checkpoint,
                    expectedCheckpointRevision ?? checkpoint.Revision,
                    restoreResult,
                    continuedTick);

    private static RecoveryContext CreateContext()
    {
        var composition = Composition(CompositionId());
        var lifecycle =
            global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeLifecycle
                .Create(RuntimeId(), composition.CompositionId);
        var queue =
            global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueSnapshot
                .Create(
                    QueueId(),
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeQueueCapacity.Create(8),
                    queuedCount: 0,
                    revision: 2);
        var active = ActiveWorkSnapshot();
        var persistence =
            new global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateSnapshotPersistence<RecoveryWorldState>(
                    new RecoveryCodec());
        var manager =
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<RecoveryWorldState>.Create(
                    WorldId(),
                    new RecoveryWorldState(7),
                    initialSimulationTick: 5);
        var document = persistence.Capture(manager.Read());

        return new RecoveryContext(
            lifecycle,
            composition,
            queue,
            active,
            persistence,
            document);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeComposition Composition(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeCompositionIdKind> compositionId)
    {
        var descriptor =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCapabilityDescriptor.Create(
                    CapabilityId(),
                    new Capability(),
                    Array.Empty<
                        global::AI.Sandbox.Engine.Core.Identifiers.Id<
                            global::AI.Sandbox.Engine.Core.HostRuntime
                                .HostRuntimeCapabilityIdKind>>());
        var result =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompositionBuilder.Compose(
                    compositionId,
                    new[] { descriptor });

        return Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeComposition>(
                result.Composition);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeActiveWorkSnapshot<RequestPayload>
        ActiveWorkSnapshot(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeInstanceIdKind>? runtimeInstanceId = null)
    {
        var result =
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkFlow.Capture<RequestPayload>(
                    ActiveWorkSnapshotId(),
                    runtimeInstanceId ?? RuntimeId(),
                    ClockId(),
                    observedTick: 30,
                    revision: 6,
                    items:
                        Array.Empty<
                            global::AI.Sandbox.Engine.Core.HostRuntime
                                .HostRuntimeActiveWorkItem<RequestPayload>>());

        return Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkSnapshot<RequestPayload>>(
                    result.Snapshot);
    }

    private static global::AI.Sandbox.Engine.Core.Persistence
        .WorldSnapshotDocument InvalidChecksumDocument(
            global::AI.Sandbox.Engine.Core.Persistence.WorldSnapshotDocument
                source)
    {
        var payload =
            global::AI.Sandbox.Engine.Core.Persistence.SnapshotPayload.From(
                new byte[] { 9, 9, 9 });
        return global::AI.Sandbox.Engine.Core.Persistence
            .WorldSnapshotDocument.Create(
                source.FormatVersion,
                source.SchemaId,
                source.SchemaVersion,
                source.WorldId,
                source.WorldStateVersion,
                source.SimulationTick,
                payload,
                source.Checksum);
    }

    private static global::AI.Sandbox.Engine.Core.Persistence
        .WorldSnapshotDocument RecreateDocument(
            global::AI.Sandbox.Engine.Core.Persistence.WorldSnapshotDocument
                source,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>?
                worldId = null,
            global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion?
                worldStateVersion = null,
            ulong? simulationTick = null) =>
        global::AI.Sandbox.Engine.Core.Persistence.WorldSnapshotDocument.Create(
            source.FormatVersion,
            source.SchemaId,
            source.SchemaVersion,
            worldId ?? source.WorldId,
            worldStateVersion ?? source.WorldStateVersion,
            simulationTick ?? source.SimulationTick,
            source.Payload,
            source.Checksum);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>
        Id<TKind>(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(
            $"019e0000-0000-7000-8000-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCheckpointIdKind>
        CheckpointId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCheckpointIdKind>(439001);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuationIdKind>
        ContinuationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuationIdKind>(439002);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeInstanceIdKind>
        RuntimeId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInstanceIdKind>(439003);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeInstanceIdKind>
        OtherRuntimeId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInstanceIdKind>(439004);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompositionIdKind>
        CompositionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompositionIdKind>(439005);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompositionIdKind>
        OtherCompositionId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompositionIdKind>(439006);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCapabilityIdKind>
        CapabilityId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCapabilityIdKind>(439007);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeQueueIdKind>
        QueueId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeQueueIdKind>(439008);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkSnapshotIdKind>
        ActiveWorkSnapshotId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeActiveWorkSnapshotIdKind>(439009);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        ClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(439010);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>
        WorldId() => Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>(439011);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>
        OtherWorldId() => Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>(439012);

    private sealed class RecoveryCodec :
        global::AI.Sandbox.Engine.Core.Persistence
            .IWorldStateSnapshotCodec<RecoveryWorldState>
    {
        public global::AI.Sandbox.Engine.Core.Persistence.PersistenceSchemaId
            SchemaId { get; } =
            global::AI.Sandbox.Engine.Core.Persistence.PersistenceSchemaId
                .Parse("host.recovery");

        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaVersion CurrentSchemaVersion { get; } =
            global::AI.Sandbox.Engine.Core.Persistence
                .PersistenceSchemaVersion.From(1);

        public bool CanDecode(
            global::AI.Sandbox.Engine.Core.Persistence
                .PersistenceSchemaVersion version) =>
            version == CurrentSchemaVersion;

        public global::AI.Sandbox.Engine.Core.Persistence.SnapshotPayload
            Encode(RecoveryWorldState state) =>
            global::AI.Sandbox.Engine.Core.Persistence.SnapshotPayload.From(
                System.Text.Encoding.UTF8.GetBytes(
                    state.Value.ToString(
                        System.Globalization.CultureInfo.InvariantCulture)));

        public global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<RecoveryWorldState> Decode(
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion version,
                global::AI.Sandbox.Engine.Core.Persistence.SnapshotPayload
                    payload)
        {
            var text = System.Text.Encoding.UTF8.GetString(payload.ToArray());
            return int.TryParse(
                text,
                System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture,
                out var value)
                ? global::AI.Sandbox.Engine.Core.Persistence
                    .WorldStateDecodeDecision<RecoveryWorldState>.Accept(
                        new RecoveryWorldState(value))
                : global::AI.Sandbox.Engine.Core.Persistence
                    .WorldStateDecodeDecision<RecoveryWorldState>.Reject(
                        "Invalid recovery state.");
        }
    }

    private sealed record RecoveryContext(
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeLifecycleSnapshot LifecycleSnapshot,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeComposition Composition,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeQueueSnapshot QueueSnapshot,
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeActiveWorkSnapshot<RequestPayload> ActiveWorkSnapshot,
        global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateSnapshotPersistence<RecoveryWorldState> Persistence,
        global::AI.Sandbox.Engine.Core.Persistence
            .WorldSnapshotDocument WorldDocument);
}
