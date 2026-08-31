namespace AI.Sandbox.Engine.Core.Tests;

public sealed class DeterministicRandomnessTests
{
    private readonly record struct RandomComponent(
        global::AI.Sandbox.Engine.Core.Randomness.DeterministicRandomState State,
        int LastValue) :
        global::AI.Sandbox.Engine.Core.Components.IComponent;

    private sealed record RandomWorldState(
        global::AI.Sandbox.Engine.Core.Entities.EntityRegistry Entities,
        global::AI.Sandbox.Engine.Core.Components.ComponentRegistry Components) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    [Xunit.Fact]
    public void ZeroSeed_IsExplicitlyValidWhileDefaultSeedIsEmpty()
    {
        global::AI.Sandbox.Engine.Core.Randomness.RandomSeed empty = default;
        var zero = global::AI.Sandbox.Engine.Core.Randomness.RandomSeed.From(0);

        Xunit.Assert.True(empty.IsEmpty);
        Xunit.Assert.False(zero.IsEmpty);
        Xunit.Assert.Equal(0UL, zero.Value);
        Xunit.Assert.Equal("0000000000000000", zero.ToString());
    }

    [Xunit.Fact]
    public void SplitMix64VersionOne_MatchesKnownVector()
    {
        var state = global::AI.Sandbox.Engine.Core.Randomness
            .DeterministicRandomState.Restore(
                global::AI.Sandbox.Engine.Core.Randomness
                    .RandomAlgorithmVersion.Current,
                CreateStreamId(1),
                stateValue: 0,
                drawCount: 0);
        var expected = new ulong[]
        {
            0xe220a8397b1dcdafUL,
            0x6e789e6aa1b965f4UL,
            0x06c45d188009454fUL,
            0xf88bb8a8724c81ecUL,
            0x1b39896a51a8749bUL,
        };

        foreach (var value in expected)
        {
            var draw = state.NextUInt64();
            Xunit.Assert.Equal(value, draw.Value);
            state = draw.NextState;
        }

        Xunit.Assert.Equal(5UL, state.DrawCount);
        Xunit.Assert.Equal(
            0x1715609f7c746c69UL,
            state.StateValue);
    }

    [Xunit.Fact]
    public void SameSeedAndStream_ProduceIdenticalSequence()
    {
        var seed =
            global::AI.Sandbox.Engine.Core.Randomness.RandomSeed.From(12345);
        var first = global::AI.Sandbox.Engine.Core.Randomness
            .DeterministicRandomState.Create(seed, CreateStreamId(1));
        var second = global::AI.Sandbox.Engine.Core.Randomness
            .DeterministicRandomState.Create(seed, CreateStreamId(1));

        for (var index = 0; index < 100; index++)
        {
            var firstDraw = first.NextUInt64();
            var secondDraw = second.NextUInt64();

            Xunit.Assert.Equal(firstDraw.Value, secondDraw.Value);
            Xunit.Assert.Equal(firstDraw.NextState, secondDraw.NextState);

            first = firstDraw.NextState;
            second = secondDraw.NextState;
        }
    }

    [Xunit.Fact]
    public void DifferentStreamIds_DeriveIndependentSequences()
    {
        var seed =
            global::AI.Sandbox.Engine.Core.Randomness.RandomSeed.From(12345);
        var first = global::AI.Sandbox.Engine.Core.Randomness
            .DeterministicRandomState.Create(seed, CreateStreamId(1));
        var second = global::AI.Sandbox.Engine.Core.Randomness
            .DeterministicRandomState.Create(seed, CreateStreamId(2));
        var firstValues = new List<ulong>();
        var secondValues = new List<ulong>();

        for (var index = 0; index < 16; index++)
        {
            var firstDraw = first.NextUInt64();
            var secondDraw = second.NextUInt64();
            firstValues.Add(firstDraw.Value);
            secondValues.Add(secondDraw.Value);
            first = firstDraw.NextState;
            second = secondDraw.NextState;
        }

        Xunit.Assert.False(firstValues.SequenceEqual(secondValues));
    }

    [Xunit.Fact]
    public void ConsumingOneStream_DoesNotAdvanceAnotherStream()
    {
        var seed =
            global::AI.Sandbox.Engine.Core.Randomness.RandomSeed.From(7);
        var first = global::AI.Sandbox.Engine.Core.Randomness
            .DeterministicRandomState.Create(seed, CreateStreamId(1));
        var untouchedSecond = global::AI.Sandbox.Engine.Core.Randomness
            .DeterministicRandomState.Create(seed, CreateStreamId(2));

        for (var index = 0; index < 1_000; index++)
        {
            first = first.NextUInt64().NextState;
        }

        var expectedSecond = global::AI.Sandbox.Engine.Core.Randomness
            .DeterministicRandomState.Create(seed, CreateStreamId(2));
        var actualDraw = untouchedSecond.NextUInt64();
        var expectedDraw = expectedSecond.NextUInt64();

        Xunit.Assert.Equal(expectedDraw, actualDraw);
        Xunit.Assert.Equal(1_000UL, first.DrawCount);
    }

    [Xunit.Fact]
    public void BoundedIntegersStayInsideRequestedRanges()
    {
        var state = CreateState();
        var seen = new bool[10];

        for (var index = 0; index < 10_000; index++)
        {
            var unsignedDraw = state.NextUInt64(10);
            Xunit.Assert.InRange(unsignedDraw.Value, 0UL, 9UL);
            seen[(int)unsignedDraw.Value] = true;
            state = unsignedDraw.NextState;

            var signedDraw = state.NextInt32(-5, 6);
            Xunit.Assert.InRange(signedDraw.Value, -5, 5);
            state = signedDraw.NextState;
        }

        Xunit.Assert.All(seen, value => Xunit.Assert.True(value));
    }

    [Xunit.Fact]
    public void DoubleAndBooleanDrawsAreDeterministicAndAdvanceState()
    {
        var state = CreateState();

        var doubleDraw = state.NextDouble();
        var booleanDraw = doubleDraw.NextState.NextBoolean();

        Xunit.Assert.InRange(doubleDraw.Value, 0.0, 1.0);
        Xunit.Assert.True(doubleDraw.Value < 1.0);
        Xunit.Assert.Equal(1UL, doubleDraw.NextState.DrawCount);
        Xunit.Assert.Equal(2UL, booleanDraw.NextState.DrawCount);

        var repeatedDouble = CreateState().NextDouble();
        var repeatedBoolean =
            repeatedDouble.NextState.NextBoolean();

        Xunit.Assert.Equal(doubleDraw.Value, repeatedDouble.Value);
        Xunit.Assert.Equal(booleanDraw.Value, repeatedBoolean.Value);
    }

    [Xunit.Fact]
    public void InvalidStateBoundsAndOverflowFailBeforeReturningValue()
    {
        global::AI.Sandbox.Engine.Core.Randomness
            .DeterministicRandomState empty = default;

        Xunit.Assert.Throws<InvalidOperationException>(
            () => empty.NextUInt64());

        var state = CreateState();
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => state.NextUInt64(0));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => state.NextInt32(5, 5));

        var exhausted = global::AI.Sandbox.Engine.Core.Randomness
            .DeterministicRandomState.Restore(
                global::AI.Sandbox.Engine.Core.Randomness
                    .RandomAlgorithmVersion.Current,
                CreateStreamId(1),
                stateValue: 0,
                drawCount: ulong.MaxValue);

        Xunit.Assert.Throws<OverflowException>(
            () => exhausted.NextUInt64());
    }

    [Xunit.Fact]
    public void RestoreRejectsInvalidOrUnsupportedMetadata()
    {
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Randomness
                .DeterministicRandomState.Create(
                    default,
                    CreateStreamId(1)));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Randomness
                .DeterministicRandomState.Create(
                    global::AI.Sandbox.Engine.Core.Randomness
                        .RandomSeed.From(1),
                    default));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Randomness
                .DeterministicRandomState.Restore(
                    default,
                    CreateStreamId(1),
                    0,
                    0));
        Xunit.Assert.Throws<NotSupportedException>(
            () => global::AI.Sandbox.Engine.Core.Randomness
                .DeterministicRandomState.Restore(
                    global::AI.Sandbox.Engine.Core.Randomness
                        .RandomAlgorithmVersion.From(2),
                    CreateStreamId(1),
                    0,
                    0));
    }

    [Xunit.Fact]
    public void PersistedStateContinuesWithExactlyTheSameNextDraw()
    {
        var state = CreateState();

        for (var index = 0; index < 37; index++)
        {
            state = state.NextUInt64().NextState;
        }

        var restored = global::AI.Sandbox.Engine.Core.Randomness
            .DeterministicRandomState.Restore(
                state.AlgorithmVersion,
                state.StreamId,
                state.StateValue,
                state.DrawCount);

        Xunit.Assert.Equal(state, restored);
        Xunit.Assert.Equal(
            state.NextUInt64(),
            restored.NextUInt64());
    }

    [Xunit.Fact]
    public void SchedulerSaveRestoreContinuation_IsByteIdentical()
    {
        var uninterrupted = RunScenario(
            totalTicks: 200,
            checkpointTick: null);
        var resumed = RunScenario(
            totalTicks: 200,
            checkpointTick: 73);

        Xunit.Assert.Equal(
            uninterrupted.WorldId,
            resumed.WorldId);
        Xunit.Assert.Equal(
            uninterrupted.WorldStateVersion,
            resumed.WorldStateVersion);
        Xunit.Assert.Equal(
            uninterrupted.SimulationTick,
            resumed.SimulationTick);
        Xunit.Assert.Equal(
            uninterrupted.Checksum,
            resumed.Checksum);
        Xunit.Assert.True(
            uninterrupted.Payload.ContentEquals(
                resumed.Payload));
    }

    private static global::AI.Sandbox.Engine.Core.Persistence
        .WorldSnapshotDocument RunScenario(
            int totalTicks,
            int? checkpointTick)
    {
        var persistence =
            new global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateSnapshotPersistence<RandomWorldState>(
                    new RandomWorldCodec());
        var manager = CreateManager();
        var scheduler = CreateScheduler(manager);

        if (checkpointTick is null)
        {
            RunTicks(scheduler, totalTicks);
        }
        else
        {
            RunTicks(scheduler, checkpointTick.Value);
            var checkpoint = persistence.Capture(manager.Read());
            var restored = persistence.Restore(checkpoint);

            Xunit.Assert.True(restored.WasRestored);
            Xunit.Assert.NotNull(restored.Snapshot);

            manager = global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<RandomWorldState>.Restore(
                    restored.Snapshot!);
            scheduler = CreateScheduler(manager);
            RunTicks(
                scheduler,
                totalTicks - checkpointTick.Value);
        }

        return persistence.Capture(manager.Read());
    }

    private static void RunTicks(
        global::AI.Sandbox.Engine.Core.Simulation
            .SimulationScheduler<RandomWorldState> scheduler,
        int count)
    {
        for (var index = 0; index < count; index++)
        {
            Xunit.Assert.True(scheduler.RunNextTick().WasApplied);
        }
    }

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<RandomWorldState> CreateManager()
    {
        var entityId = CreateEntityId();
        var entities =
            global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
                .FromActiveEntities(new[] { entityId });
        var components =
            new global::AI.Sandbox.Engine.Core.Components
                .ComponentRegistryBuilder(entities)
                .Add(
                    entityId,
                    new RandomComponent(
                        CreateState(),
                        LastValue: 0))
                .Build();

        return global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<RandomWorldState>.Create(
                CreateWorldId(),
                new RandomWorldState(entities, components));
    }

    private static global::AI.Sandbox.Engine.Core.Simulation
        .SimulationScheduler<RandomWorldState> CreateScheduler(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<RandomWorldState> manager)
    {
        return new global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSchedulerBuilder<RandomWorldState>()
            .Add(CreateSystemId(), new RandomDrawSystem())
            .Build(manager);
    }

    private static global::AI.Sandbox.Engine.Core.Randomness
        .DeterministicRandomState CreateState()
    {
        return global::AI.Sandbox.Engine.Core.Randomness
            .DeterministicRandomState.Create(
                global::AI.Sandbox.Engine.Core.Randomness
                    .RandomSeed.From(0x0123456789abcdefUL),
                CreateStreamId(1));
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Randomness.RandomStreamIdKind>
        CreateStreamId(int suffix)
    {
        var text = $"019b0000-0000-7000-8c00-{suffix:D12}";
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Randomness
                .RandomStreamIdKind>.Parse(text);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> CreateEntityId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(
                "019b0000-0000-7000-8d00-000000000001");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> CreateWorldId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000000900");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Simulation.SimulationSystemIdKind>
        CreateSystemId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemIdKind>.Parse(
                    "019b0000-0000-7000-8e00-000000000001");
    }

    private sealed class RandomDrawSystem :
        global::AI.Sandbox.Engine.Core.Simulation
            .ISimulationSystem<RandomWorldState>
    {
        public global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<RandomWorldState> Execute(
                global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemContext<RandomWorldState> context)
        {
            var entityId = CreateEntityId();

            if (!context.State.Components.TryGet<RandomComponent>(
                entityId,
                out var random))
            {
                return global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemDecision<RandomWorldState>.Reject(
                        "Random component is missing.");
            }

            var draw = random.State.NextInt32(-1_000_000, 1_000_001);
            var mutation = context.State.Components.Set(
                context.State.Entities,
                entityId,
                new RandomComponent(
                    draw.NextState,
                    draw.Value));

            if (!mutation.WasApplied)
            {
                return global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemDecision<RandomWorldState>.Reject(
                        mutation.Status.ToString());
            }

            return global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemDecision<RandomWorldState>.Update(
                    context.State with
                    {
                        Components = mutation.Registry,
                    });
        }
    }

    private sealed class RandomWorldCodec :
        global::AI.Sandbox.Engine.Core.Persistence
            .IWorldStateSnapshotCodec<RandomWorldState>
    {
        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaId SchemaId { get; } =
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaId.Parse("randomness.validation");

        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaVersion CurrentSchemaVersion { get; } =
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion.From(1);

        public bool CanDecode(
            global::AI.Sandbox.Engine.Core.Persistence
                .PersistenceSchemaVersion version)
        {
            return version == CurrentSchemaVersion;
        }

        public global::AI.Sandbox.Engine.Core.Persistence
            .SnapshotPayload Encode(RandomWorldState state)
        {
            if (!state.Components.TryGet<RandomComponent>(
                CreateEntityId(),
                out var random))
            {
                throw new InvalidOperationException(
                    "Random component is missing.");
            }

            var text = string.Join(
                '|',
                random.State.AlgorithmVersion.Value.ToString(
                    System.Globalization.CultureInfo.InvariantCulture),
                random.State.StreamId.ToString(),
                random.State.StateValue.ToString(
                    "x16",
                    System.Globalization.CultureInfo.InvariantCulture),
                random.State.DrawCount.ToString(
                    System.Globalization.CultureInfo.InvariantCulture),
                random.LastValue.ToString(
                    System.Globalization.CultureInfo.InvariantCulture));

            return global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotPayload.From(
                    System.Text.Encoding.UTF8.GetBytes(text));
        }

        public global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<RandomWorldState> Decode(
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion version,
                global::AI.Sandbox.Engine.Core.Persistence
                    .SnapshotPayload payload)
        {
            if (version != CurrentSchemaVersion)
            {
                return Reject("Unsupported schema version.");
            }

            var parts = System.Text.Encoding.UTF8.GetString(
                payload.ToArray()).Split('|');

            if (parts.Length != 5 ||
                !uint.TryParse(
                    parts[0],
                    System.Globalization.NumberStyles.None,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var algorithmVersion) ||
                !global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.Randomness
                        .RandomStreamIdKind>.TryParse(
                            parts[1],
                            out var streamId) ||
                !ulong.TryParse(
                    parts[2],
                    System.Globalization.NumberStyles.AllowHexSpecifier,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var stateValue) ||
                !ulong.TryParse(
                    parts[3],
                    System.Globalization.NumberStyles.None,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var drawCount) ||
                !int.TryParse(
                    parts[4],
                    System.Globalization.NumberStyles.Integer,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var lastValue))
            {
                return Reject("Malformed random state payload.");
            }

            global::AI.Sandbox.Engine.Core.Randomness
                .DeterministicRandomState randomState;

            try
            {
                randomState = global::AI.Sandbox.Engine.Core.Randomness
                    .DeterministicRandomState.Restore(
                        global::AI.Sandbox.Engine.Core.Randomness
                            .RandomAlgorithmVersion.From(algorithmVersion),
                        streamId,
                        stateValue,
                        drawCount);
            }
            catch (Exception exception)
                when (exception is ArgumentException or NotSupportedException)
            {
                return Reject(exception.Message);
            }

            var entityId = CreateEntityId();
            var entities =
                global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
                    .FromActiveEntities(new[] { entityId });
            var components =
                new global::AI.Sandbox.Engine.Core.Components
                    .ComponentRegistryBuilder(entities)
                    .Add(
                        entityId,
                        new RandomComponent(
                            randomState,
                            lastValue))
                    .Build();

            return global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateDecodeDecision<RandomWorldState>.Accept(
                    new RandomWorldState(entities, components));
        }

        private static global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<RandomWorldState> Reject(
                string reason)
        {
            return global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateDecodeDecision<RandomWorldState>.Reject(reason);
        }
    }
}
