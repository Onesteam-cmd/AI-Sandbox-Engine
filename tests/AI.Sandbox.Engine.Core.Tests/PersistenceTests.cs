namespace AI.Sandbox.Engine.Core.Tests;

public sealed class PersistenceTests
{
    private sealed record CounterState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    [Xunit.Theory]
    [Xunit.InlineData("game.world")]
    [Xunit.InlineData("component.position-v2")]
    [Xunit.InlineData("a")]
    public void SchemaId_ParsesCanonicalStableNames(string text)
    {
        var schema = global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaId.Parse(text);

        Xunit.Assert.Equal(text, schema.Value);
        Xunit.Assert.Equal(text, schema.ToString());
        Xunit.Assert.False(schema.IsEmpty);
    }

    [Xunit.Theory]
    [Xunit.InlineData("")]
    [Xunit.InlineData(".world")]
    [Xunit.InlineData("world.")]
    [Xunit.InlineData("game..world")]
    [Xunit.InlineData("Game.world")]
    [Xunit.InlineData("game_world")]
    [Xunit.InlineData("game.2world")]
    public void SchemaId_RejectsUnstableOrNonCanonicalNames(string text)
    {
        Xunit.Assert.False(
            global::AI.Sandbox.Engine.Core.Persistence
                .PersistenceSchemaId.TryParse(text, out var schema));
        Xunit.Assert.True(schema.IsEmpty);
        Xunit.Assert.Throws<FormatException>(
            () => global::AI.Sandbox.Engine.Core.Persistence
                .PersistenceSchemaId.Parse(text));
    }

    [Xunit.Fact]
    public void VersionTypes_RejectZeroAndOrderNumerically()
    {
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.Persistence
                .PersistenceSchemaVersion.From(0));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotFormatVersion.From(0));

        var lower = global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaVersion.From(1);
        var higher = global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaVersion.From(2);

        Xunit.Assert.True(lower.CompareTo(higher) < 0);
        Xunit.Assert.Equal("2", higher.ToString());
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotFormatVersion.From(1),
            global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotFormatVersion.Current);
    }

    [Xunit.Fact]
    public void Payload_DefensivelyCopiesInputAndOutput()
    {
        var source = new byte[] { 1, 2, 3 };
        var payload = global::AI.Sandbox.Engine.Core.Persistence
            .SnapshotPayload.From(source);

        source[0] = 9;
        var firstCopy = payload.ToArray();
        firstCopy[1] = 8;
        var secondCopy = payload.ToArray();

        Xunit.Assert.Equal(new byte[] { 1, 2, 3 }, secondCopy);
        Xunit.Assert.Equal(3, payload.Length);
        Xunit.Assert.False(payload.IsEmpty);
    }

    [Xunit.Fact]
    public void Checksum_UsesCanonicalSha256()
    {
        var payload = global::AI.Sandbox.Engine.Core.Persistence
            .SnapshotPayload.From(
                System.Text.Encoding.UTF8.GetBytes("abc"));

        var checksum = global::AI.Sandbox.Engine.Core.Persistence
            .SnapshotChecksum.Compute(payload);

        Xunit.Assert.Equal(
            "ba7816bf8f01cfea414140de5dae2223" +
            "b00361a396177a9cb410ff61f20015ad",
            checksum.Value);
        Xunit.Assert.True(checksum.Matches(payload));
        Xunit.Assert.Equal(
            checksum,
            global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotChecksum.Parse(checksum.Value.ToUpperInvariant()));
    }

    [Xunit.Fact]
    public void Document_ReportsChecksumMismatchWithoutMutatingPayload()
    {
        var payload = global::AI.Sandbox.Engine.Core.Persistence
            .SnapshotPayload.From(new byte[] { 1, 2, 3 });
        var wrongPayload = global::AI.Sandbox.Engine.Core.Persistence
            .SnapshotPayload.From(new byte[] { 4, 5, 6 });
        var document = CreateDocument(
            payload,
            global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotChecksum.Compute(wrongPayload));

        Xunit.Assert.False(document.HasValidChecksum);
        Xunit.Assert.Equal(new byte[] { 1, 2, 3 }, document.Payload.ToArray());
    }

    [Xunit.Fact]
    public void Capture_PreservesMetadataAndEncodesExactlyOnce()
    {
        var codec = new CounterCodec();
        var persistence = CreatePersistence(codec);
        var snapshot = CreateSnapshot(
            new CounterState(42),
            version: 7,
            tick: 99);

        var document = persistence.Capture(snapshot);

        Xunit.Assert.Equal(1, codec.EncodeCount);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotFormatVersion.Current,
            document.FormatVersion);
        Xunit.Assert.Equal(codec.SchemaId, document.SchemaId);
        Xunit.Assert.Equal(
            codec.CurrentSchemaVersion,
            document.SchemaVersion);
        Xunit.Assert.Equal(snapshot.WorldId, document.WorldId);
        Xunit.Assert.Equal(snapshot.Version, document.WorldStateVersion);
        Xunit.Assert.Equal(99UL, document.SimulationTick);
        Xunit.Assert.True(document.HasValidChecksum);
    }

    [Xunit.Fact]
    public void Capture_IsDeterministicForEqualState()
    {
        var codec = new CounterCodec();
        var persistence = CreatePersistence(codec);
        var first = persistence.Capture(
            CreateSnapshot(new CounterState(12)));
        var second = persistence.Capture(
            CreateSnapshot(new CounterState(12)));

        Xunit.Assert.True(first.Payload.ContentEquals(second.Payload));
        Xunit.Assert.Equal(first.Checksum, second.Checksum);
    }

    [Xunit.Fact]
    public void Restore_RoundTripsMetadataAndState()
    {
        var codec = new CounterCodec();
        var persistence = CreatePersistence(codec);
        var original = CreateSnapshot(
            new CounterState(123),
            version: 9,
            tick: 70);
        var document = persistence.Capture(original);

        var result = persistence.Restore(document);

        Xunit.Assert.True(result.WasRestored);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotRestoreStatus.Restored,
            result.Status);
        Xunit.Assert.Null(result.FailureReason);
        Xunit.Assert.NotNull(result.Snapshot);
        Xunit.Assert.Equal(original.WorldId, result.Snapshot!.WorldId);
        Xunit.Assert.Equal(original.Version, result.Snapshot.Version);
        Xunit.Assert.Equal(70UL, result.Snapshot.SimulationTick);
        Xunit.Assert.Equal(new CounterState(123), result.Snapshot.State);
        Xunit.Assert.Equal(1, codec.DecodeCount);
    }

    [Xunit.Fact]
    public void RestoredManager_ContinuesVersionAndTick()
    {
        var codec = new CounterCodec();
        var persistence = CreatePersistence(codec);
        var restored = persistence.Restore(
            persistence.Capture(
                CreateSnapshot(
                    new CounterState(5),
                    version: 4,
                    tick: 10)));

        Xunit.Assert.NotNull(restored.Snapshot);
        var manager = global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<CounterState>.Restore(restored.Snapshot!);

        var applied = manager.TryApply(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateVersion.From(4),
            11,
            new IncrementTransition());

        Xunit.Assert.True(applied.WasApplied);
        Xunit.Assert.Equal(5UL, applied.Snapshot.Version.Value);
        Xunit.Assert.Equal(11UL, applied.Snapshot.SimulationTick);
        Xunit.Assert.Equal(6, applied.Snapshot.State.Value);
    }

    [Xunit.Fact]
    public void Restore_RejectsUnsupportedFormatBeforeCodec()
    {
        var codec = new CounterCodec();
        var persistence = CreatePersistence(codec);
        var valid = persistence.Capture(CreateSnapshot(new CounterState(1)));
        var document = global::AI.Sandbox.Engine.Core.Persistence
            .WorldSnapshotDocument.Create(
                global::AI.Sandbox.Engine.Core.Persistence
                    .SnapshotFormatVersion.From(2),
                valid.SchemaId,
                valid.SchemaVersion,
                valid.WorldId,
                valid.WorldStateVersion,
                valid.SimulationTick,
                valid.Payload,
                valid.Checksum);

        var result = persistence.Restore(document);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotRestoreStatus.UnsupportedFormatVersion,
            result.Status);
        Xunit.Assert.Equal(0, codec.CanDecodeCount);
        Xunit.Assert.Equal(0, codec.DecodeCount);
    }

    [Xunit.Fact]
    public void Restore_RejectsSchemaMismatchBeforeCodec()
    {
        var codec = new CounterCodec();
        var persistence = CreatePersistence(codec);
        var valid = persistence.Capture(CreateSnapshot(new CounterState(1)));
        var document = global::AI.Sandbox.Engine.Core.Persistence
            .WorldSnapshotDocument.Create(
                valid.FormatVersion,
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaId.Parse("other.world"),
                valid.SchemaVersion,
                valid.WorldId,
                valid.WorldStateVersion,
                valid.SimulationTick,
                valid.Payload,
                valid.Checksum);

        var result = persistence.Restore(document);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotRestoreStatus.SchemaMismatch,
            result.Status);
        Xunit.Assert.Equal(0, codec.CanDecodeCount);
        Xunit.Assert.Equal(0, codec.DecodeCount);
    }

    [Xunit.Fact]
    public void Restore_RejectsUnsupportedSchemaVersionBeforeDecode()
    {
        var codec = new CounterCodec();
        var persistence = CreatePersistence(codec);
        var valid = persistence.Capture(CreateSnapshot(new CounterState(1)));
        var document = global::AI.Sandbox.Engine.Core.Persistence
            .WorldSnapshotDocument.Create(
                valid.FormatVersion,
                valid.SchemaId,
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion.From(2),
                valid.WorldId,
                valid.WorldStateVersion,
                valid.SimulationTick,
                valid.Payload,
                valid.Checksum);

        var result = persistence.Restore(document);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotRestoreStatus.UnsupportedSchemaVersion,
            result.Status);
        Xunit.Assert.Equal(1, codec.CanDecodeCount);
        Xunit.Assert.Equal(0, codec.DecodeCount);
    }

    [Xunit.Fact]
    public void Restore_RejectsChecksumMismatchBeforeDecode()
    {
        var codec = new CounterCodec();
        var persistence = CreatePersistence(codec);
        var valid = persistence.Capture(CreateSnapshot(new CounterState(1)));
        var tampered = global::AI.Sandbox.Engine.Core.Persistence
            .SnapshotPayload.From(
                System.Text.Encoding.UTF8.GetBytes("999"));
        var document = global::AI.Sandbox.Engine.Core.Persistence
            .WorldSnapshotDocument.Create(
                valid.FormatVersion,
                valid.SchemaId,
                valid.SchemaVersion,
                valid.WorldId,
                valid.WorldStateVersion,
                valid.SimulationTick,
                tampered,
                valid.Checksum);

        var result = persistence.Restore(document);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotRestoreStatus.ChecksumMismatch,
            result.Status);
        Xunit.Assert.Equal(0, codec.DecodeCount);
    }

    [Xunit.Fact]
    public void Restore_ReturnsCodecRejectionWithoutSnapshot()
    {
        var codec = new CounterCodec
        {
            RejectDecode = true,
        };
        var persistence = CreatePersistence(codec);
        var document = persistence.Capture(
            CreateSnapshot(new CounterState(1)));

        var result = persistence.Restore(document);

        Xunit.Assert.False(result.WasRestored);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotRestoreStatus.CodecRejected,
            result.Status);
        Xunit.Assert.Equal("payload rejected", result.FailureReason);
        Xunit.Assert.Null(result.Snapshot);
        Xunit.Assert.Equal(1, codec.DecodeCount);
    }

    [Xunit.Fact]
    public void Capture_NullCodecPayloadThrows()
    {
        var codec = new CounterCodec
        {
            ReturnNullPayload = true,
        };
        var persistence = CreatePersistence(codec);

        Xunit.Assert.Throws<InvalidOperationException>(
            () => persistence.Capture(
                CreateSnapshot(new CounterState(1))));
    }

    [Xunit.Fact]
    public void Restore_NullCodecDecisionThrows()
    {
        var codec = new CounterCodec
        {
            ReturnNullDecision = true,
        };
        var persistence = CreatePersistence(codec);
        var document = persistence.Capture(
            CreateSnapshot(new CounterState(1)));

        Xunit.Assert.Throws<InvalidOperationException>(
            () => persistence.Restore(document));
    }

    private static global::AI.Sandbox.Engine.Core.Persistence
        .WorldStateSnapshotPersistence<CounterState> CreatePersistence(
            CounterCodec codec)
    {
        return new global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateSnapshotPersistence<CounterState>(codec);
    }

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateSnapshot<CounterState> CreateSnapshot(
            CounterState state,
            ulong version = 0,
            ulong tick = 0)
    {
        var manager = global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<CounterState>.Create(
                CreateWorldId(),
                state,
                tick);

        if (version == 0)
        {
            return manager.Read();
        }

        return CreateRestoredSnapshot(state, version, tick);
    }

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateSnapshot<CounterState> CreateRestoredSnapshot(
            CounterState state,
            ulong version,
            ulong tick)
    {
        var codec = new CounterCodec();
        var persistence = CreatePersistence(codec);
        var payload = codec.Encode(state);
        var document = global::AI.Sandbox.Engine.Core.Persistence
            .WorldSnapshotDocument.Create(
                global::AI.Sandbox.Engine.Core.Persistence
                    .SnapshotFormatVersion.Current,
                codec.SchemaId,
                codec.CurrentSchemaVersion,
                CreateWorldId(),
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateVersion.From(version),
                tick,
                payload,
                global::AI.Sandbox.Engine.Core.Persistence
                    .SnapshotChecksum.Compute(payload));
        var restored = persistence.Restore(document);

        Xunit.Assert.NotNull(restored.Snapshot);
        return restored.Snapshot!;
    }

    private static global::AI.Sandbox.Engine.Core.Persistence
        .WorldSnapshotDocument CreateDocument(
            global::AI.Sandbox.Engine.Core.Persistence.SnapshotPayload payload,
            global::AI.Sandbox.Engine.Core.Persistence.SnapshotChecksum checksum)
    {
        return global::AI.Sandbox.Engine.Core.Persistence
            .WorldSnapshotDocument.Create(
                global::AI.Sandbox.Engine.Core.Persistence
                    .SnapshotFormatVersion.Current,
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaId.Parse("game.world"),
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion.From(1),
                CreateWorldId(),
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateVersion.Initial,
                0,
                payload,
                checksum);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> CreateWorldId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000000500");
    }

    private sealed class CounterCodec :
        global::AI.Sandbox.Engine.Core.Persistence
            .IWorldStateSnapshotCodec<CounterState>
    {
        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaId SchemaId { get; } =
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaId.Parse("game.world");

        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaVersion CurrentSchemaVersion { get; } =
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion.From(1);

        public int EncodeCount { get; private set; }

        public int CanDecodeCount { get; private set; }

        public int DecodeCount { get; private set; }

        public bool RejectDecode { get; init; }

        public bool ReturnNullPayload { get; init; }

        public bool ReturnNullDecision { get; init; }

        public bool CanDecode(
            global::AI.Sandbox.Engine.Core.Persistence
                .PersistenceSchemaVersion version)
        {
            CanDecodeCount++;
            return version == CurrentSchemaVersion;
        }

        public global::AI.Sandbox.Engine.Core.Persistence
            .SnapshotPayload Encode(CounterState state)
        {
            EncodeCount++;

            if (ReturnNullPayload)
            {
                return null!;
            }

            var text = state.Value.ToString(
                System.Globalization.CultureInfo.InvariantCulture);
            return global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotPayload.From(
                    System.Text.Encoding.UTF8.GetBytes(text));
        }

        public global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<CounterState> Decode(
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion version,
                global::AI.Sandbox.Engine.Core.Persistence
                    .SnapshotPayload payload)
        {
            _ = version;
            DecodeCount++;

            if (ReturnNullDecision)
            {
                return null!;
            }

            if (RejectDecode)
            {
                return global::AI.Sandbox.Engine.Core.Persistence
                    .WorldStateDecodeDecision<CounterState>.Reject(
                        "payload rejected");
            }

            var text = System.Text.Encoding.UTF8.GetString(
                payload.ToArray());

            if (!int.TryParse(
                text,
                System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture,
                out var value))
            {
                return global::AI.Sandbox.Engine.Core.Persistence
                    .WorldStateDecodeDecision<CounterState>.Reject(
                        "invalid integer payload");
            }

            return global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateDecodeDecision<CounterState>.Accept(
                    new CounterState(value));
        }
    }

    private sealed class IncrementTransition :
        global::AI.Sandbox.Engine.Core.WorldState
            .IWorldStateTransition<CounterState>
    {
        public global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateTransitionDecision<CounterState> Evaluate(
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateSnapshot<CounterState> current)
        {
            return global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateTransitionDecision<CounterState>.Accept(
                    current.State with
                    {
                        Value = current.State.Value + 1,
                    });
        }
    }
}
