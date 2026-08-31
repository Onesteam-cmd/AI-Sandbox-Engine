namespace AI.Sandbox.Engine.Core.Tests;

public sealed class SimulationTimeTests
{
    private readonly record struct ClockComponent(
        global::AI.Sandbox.Engine.Core.Time.SimulationTimeline Timeline,
        global::AI.Sandbox.Engine.Core.Time.SimulationInstant CurrentInstant,
        int CommandCounter) :
        global::AI.Sandbox.Engine.Core.Components.IComponent;

    private sealed record TimedWorldState(
        global::AI.Sandbox.Engine.Core.Entities.EntityRegistry Entities,
        global::AI.Sandbox.Engine.Core.Components.ComponentRegistry Components) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    private readonly record struct AddCounter(int Amount) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    [Xunit.Fact]
    public void DurationFactoriesUseExactIntegerMicroseconds()
    {
        Xunit.Assert.Equal(
            1UL,
            global::AI.Sandbox.Engine.Core.Time
                .SimulationDuration.FromMicroseconds(1).Microseconds);
        Xunit.Assert.Equal(
            1_000UL,
            global::AI.Sandbox.Engine.Core.Time
                .SimulationDuration.FromMilliseconds(1).Microseconds);
        Xunit.Assert.Equal(
            1_000_000UL,
            global::AI.Sandbox.Engine.Core.Time
                .SimulationDuration.FromSeconds(1).Microseconds);
        Xunit.Assert.Equal(
            60_000_000UL,
            global::AI.Sandbox.Engine.Core.Time
                .SimulationDuration.FromMinutes(1).Microseconds);
        Xunit.Assert.Equal(
            3_600_000_000UL,
            global::AI.Sandbox.Engine.Core.Time
                .SimulationDuration.FromHours(1).Microseconds);
        Xunit.Assert.Equal(
            86_400_000_000UL,
            global::AI.Sandbox.Engine.Core.Time
                .SimulationDuration.FromDays(1).Microseconds);
    }

    [Xunit.Fact]
    public void DurationArithmeticIsCheckedAndNeverNegative()
    {
        var first = global::AI.Sandbox.Engine.Core.Time
            .SimulationDuration.FromSeconds(2);
        var second = global::AI.Sandbox.Engine.Core.Time
            .SimulationDuration.FromMilliseconds(500);

        Xunit.Assert.Equal(
            2_500_000UL,
            first.Add(second).Microseconds);
        Xunit.Assert.Equal(
            1_500_000UL,
            first.Subtract(second).Microseconds);
        Xunit.Assert.Equal(
            6_000_000UL,
            first.Multiply(3).Microseconds);
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => second.Subtract(first));
        Xunit.Assert.Throws<OverflowException>(
            () => global::AI.Sandbox.Engine.Core.Time
                .SimulationDuration.FromMicroseconds(ulong.MaxValue)
                .Add(
                    global::AI.Sandbox.Engine.Core.Time
                        .SimulationDuration.FromMicroseconds(1)));
    }

    [Xunit.Fact]
    public void InstantsAddDurationsAndMeasureElapsedTime()
    {
        var start = global::AI.Sandbox.Engine.Core.Time
            .SimulationInstant.FromMicroseconds(500);
        var end = start.Add(
            global::AI.Sandbox.Engine.Core.Time
                .SimulationDuration.FromMilliseconds(2));

        Xunit.Assert.Equal(2_500UL, end.MicrosecondsSinceEpoch);
        Xunit.Assert.Equal(
            2_000UL,
            end.DurationSince(start).Microseconds);
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => start.DurationSince(end));
        Xunit.Assert.Throws<OverflowException>(
            () => global::AI.Sandbox.Engine.Core.Time
                .SimulationInstant.FromMicroseconds(ulong.MaxValue)
                .Add(
                    global::AI.Sandbox.Engine.Core.Time
                        .SimulationDuration.FromMicroseconds(1)));
    }

    [Xunit.Fact]
    public void TickDurationMustBePositive()
    {
        global::AI.Sandbox.Engine.Core.Time
            .SimulationTickDuration empty = default;

        Xunit.Assert.True(empty.IsEmpty);
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.Time
                .SimulationTickDuration.FromMicroseconds(0));
        Xunit.Assert.Throws<InvalidOperationException>(
            () => empty.AsDuration());

        var duration = global::AI.Sandbox.Engine.Core.Time
            .SimulationTickDuration.FromMilliseconds(50);

        Xunit.Assert.False(duration.IsEmpty);
        Xunit.Assert.Equal(50_000UL, duration.Microseconds);
    }

    [Xunit.Fact]
    public void TimelineMapsFixedTicksToExactInstants()
    {
        var timeline = CreateTimeline();

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Time
                .SimulationInstant.Epoch,
            timeline.GetInstant(0));
        Xunit.Assert.Equal(
            50_000UL,
            timeline.GetInstant(1).MicrosecondsSinceEpoch);
        Xunit.Assert.Equal(
            1_000_000UL,
            timeline.GetInstant(20).MicrosecondsSinceEpoch);
        Xunit.Assert.Equal(
            2_000_000UL,
            timeline.GetElapsedDuration(10, 50).Microseconds);
        Xunit.Assert.Equal(
            2_000_000UL,
            timeline.GetDurationUntilTick(10, 50).Microseconds);
    }

    [Xunit.Fact]
    public void TimelineFloorAndCeilingMappingsAreExact()
    {
        var timeline = CreateTimeline();

        Xunit.Assert.Equal(
            0UL,
            timeline.GetTickAtOrBefore(
                global::AI.Sandbox.Engine.Core.Time
                    .SimulationInstant.FromMicroseconds(49_999)));
        Xunit.Assert.Equal(
            1UL,
            timeline.GetFirstTickAtOrAfter(
                global::AI.Sandbox.Engine.Core.Time
                    .SimulationInstant.FromMicroseconds(1)));
        Xunit.Assert.Equal(
            1UL,
            timeline.GetTickAtOrBefore(
                global::AI.Sandbox.Engine.Core.Time
                    .SimulationInstant.FromMicroseconds(50_000)));
        Xunit.Assert.Equal(
            1UL,
            timeline.GetFirstTickAtOrAfter(
                global::AI.Sandbox.Engine.Core.Time
                    .SimulationInstant.FromMicroseconds(50_000)));
        Xunit.Assert.Equal(
            2UL,
            timeline.GetFirstTickAtOrAfter(
                global::AI.Sandbox.Engine.Core.Time
                    .SimulationInstant.FromMicroseconds(50_001)));
    }

    [Xunit.Fact]
    public void DelaysRoundUpToTheFirstRepresentableTickBoundary()
    {
        var timeline = CreateTimeline();

        Xunit.Assert.Equal(
            10UL,
            timeline.GetFirstTickAtOrAfter(
                currentTick: 10,
                global::AI.Sandbox.Engine.Core.Time
                    .SimulationDuration.Zero));
        Xunit.Assert.Equal(
            11UL,
            timeline.GetFirstTickAtOrAfter(
                currentTick: 10,
                global::AI.Sandbox.Engine.Core.Time
                    .SimulationDuration.FromMicroseconds(1)));
        Xunit.Assert.Equal(
            11UL,
            timeline.GetFirstTickAtOrAfter(
                currentTick: 10,
                global::AI.Sandbox.Engine.Core.Time
                    .SimulationDuration.FromMilliseconds(50)));
        Xunit.Assert.Equal(
            12UL,
            timeline.GetFirstTickAtOrAfter(
                currentTick: 10,
                global::AI.Sandbox.Engine.Core.Time
                    .SimulationDuration.FromMicroseconds(50_001)));
    }

    [Xunit.Fact]
    public void InvalidTimelinesRangesAndOverflowFailExplicitly()
    {
        global::AI.Sandbox.Engine.Core.Time.SimulationTimeline empty = default;

        Xunit.Assert.True(empty.IsEmpty);
        Xunit.Assert.Throws<InvalidOperationException>(
            () => empty.GetInstant(0));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => CreateTimeline().GetElapsedDuration(5, 4));
        Xunit.Assert.Throws<OverflowException>(
            () => global::AI.Sandbox.Engine.Core.Time
                .SimulationTimeline.Create(
                    global::AI.Sandbox.Engine.Core.Time
                        .SimulationTickDuration.FromMicroseconds(2))
                .GetInstant(ulong.MaxValue));
    }

    [Xunit.Fact]
    public void SchedulerUsesTargetTickAsTheOnlyAuthoritativeTimeInput()
    {
        var manager = CreateManager();
        var scheduler = CreateScheduler(manager);

        for (var expectedTick = 1UL;
            expectedTick <= 20UL;
            expectedTick++)
        {
            var result = scheduler.RunNextTick();

            Xunit.Assert.True(result.WasApplied);
            Xunit.Assert.True(
                result.Snapshot.State.Components.TryGet<ClockComponent>(
                    CreateEntityId(),
                    out var clock));
            Xunit.Assert.Equal(
                expectedTick * 50_000UL,
                clock.CurrentInstant.MicrosecondsSinceEpoch);
        }

        Xunit.Assert.Equal(20UL, manager.Read().SimulationTick);
        Xunit.Assert.Equal(
            1_000_000UL,
            GetClock(manager.Read().State)
                .CurrentInstant.MicrosecondsSinceEpoch);
    }

    [Xunit.Fact]
    public void CommandChangesVersionWithoutAdvancingSimulationTime()
    {
        var manager = CreateManager();
        var scheduler = CreateScheduler(manager);
        var processor =
            new global::AI.Sandbox.Engine.Core.Commands
                .CommandProcessorBuilder<TimedWorldState>()
                .Add(new AddCounterHandler())
                .Build(manager);

        var firstTick = scheduler.RunNextTick();
        var beforeCommand = manager.Read();
        var command = processor.Execute(
            global::AI.Sandbox.Engine.Core.Commands
                .CommandEnvelope<AddCounter>.Create(
                    CreateCommandId(),
                    beforeCommand.WorldId,
                    beforeCommand.Version,
                    beforeCommand.SimulationTick,
                    new AddCounter(10)));
        var secondTick = scheduler.RunNextTick();

        Xunit.Assert.True(firstTick.WasApplied);
        Xunit.Assert.True(command.WasApplied);
        Xunit.Assert.True(secondTick.WasApplied);
        Xunit.Assert.Equal(3UL, manager.Read().Version.Value);
        Xunit.Assert.Equal(2UL, manager.Read().SimulationTick);
        Xunit.Assert.Equal(
            50_000UL,
            GetClock(command.Snapshot.State)
                .CurrentInstant.MicrosecondsSinceEpoch);
        Xunit.Assert.Equal(
            100_000UL,
            GetClock(secondTick.Snapshot.State)
                .CurrentInstant.MicrosecondsSinceEpoch);
        Xunit.Assert.Equal(
            10,
            GetClock(manager.Read().State).CommandCounter);
    }

    [Xunit.Fact]
    public void SaveRestoreContinuationPreservesTimeByteForByte()
    {
        var uninterrupted = RunScenario(
            totalTicks: 1_000,
            checkpointTick: null);
        var resumed = RunScenario(
            totalTicks: 1_000,
            checkpointTick: 333);

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
                .WorldStateSnapshotPersistence<TimedWorldState>(
                    new TimedWorldCodec());
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
                .WorldStateManager<TimedWorldState>.Restore(
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
            .SimulationScheduler<TimedWorldState> scheduler,
        int count)
    {
        for (var index = 0; index < count; index++)
        {
            Xunit.Assert.True(scheduler.RunNextTick().WasApplied);
        }
    }

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<TimedWorldState> CreateManager()
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
                    new ClockComponent(
                        CreateTimeline(),
                        global::AI.Sandbox.Engine.Core.Time
                            .SimulationInstant.Epoch,
                        CommandCounter: 0))
                .Build();

        return global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<TimedWorldState>.Create(
                CreateWorldId(),
                new TimedWorldState(entities, components));
    }

    private static global::AI.Sandbox.Engine.Core.Simulation
        .SimulationScheduler<TimedWorldState> CreateScheduler(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<TimedWorldState> manager)
    {
        return new global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSchedulerBuilder<TimedWorldState>()
            .Add(CreateSystemId(), new ClockSystem())
            .Build(manager);
    }

    private static global::AI.Sandbox.Engine.Core.Time
        .SimulationTimeline CreateTimeline()
    {
        return global::AI.Sandbox.Engine.Core.Time
            .SimulationTimeline.Create(
                global::AI.Sandbox.Engine.Core.Time
                    .SimulationTickDuration.FromMilliseconds(50));
    }

    private static ClockComponent GetClock(TimedWorldState state)
    {
        Xunit.Assert.True(
            state.Components.TryGet<ClockComponent>(
                CreateEntityId(),
                out var clock));

        return clock;
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> CreateEntityId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(
                "019b0000-0000-7000-8f00-000000000001");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> CreateWorldId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000001000");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Simulation.SimulationSystemIdKind>
        CreateSystemId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemIdKind>.Parse(
                    "019b0000-0000-7000-9000-000000000001");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Commands.CommandIdKind> CreateCommandId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Commands.CommandIdKind>.Parse(
                "019b0000-0000-7000-9100-000000000001");
    }

    private sealed class ClockSystem :
        global::AI.Sandbox.Engine.Core.Simulation
            .ISimulationSystem<TimedWorldState>
    {
        public global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<TimedWorldState> Execute(
                global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemContext<TimedWorldState> context)
        {
            var entityId = CreateEntityId();

            if (!context.State.Components.TryGet<ClockComponent>(
                entityId,
                out var clock))
            {
                return global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemDecision<TimedWorldState>.Reject(
                        "Clock component is missing.");
            }

            var mutation = context.State.Components.Set(
                context.State.Entities,
                entityId,
                clock with
                {
                    CurrentInstant = clock.Timeline.GetInstant(
                        context.TargetSimulationTick),
                });

            if (!mutation.WasApplied)
            {
                return global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemDecision<TimedWorldState>.Reject(
                        mutation.Status.ToString());
            }

            return global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemDecision<TimedWorldState>.Update(
                    context.State with
                    {
                        Components = mutation.Registry,
                    });
        }
    }

    private sealed class AddCounterHandler :
        global::AI.Sandbox.Engine.Core.Commands
            .ICommandHandler<TimedWorldState, AddCounter>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<TimedWorldState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<TimedWorldState, AddCounter> context)
        {
            var entityId = CreateEntityId();

            if (!context.Snapshot.State.Components.TryGet<ClockComponent>(
                entityId,
                out var clock))
            {
                return global::AI.Sandbox.Engine.Core.Commands
                    .CommandDecision<TimedWorldState>.Reject(
                        "Clock component is missing.");
            }

            var mutation = context.Snapshot.State.Components.Set(
                context.Snapshot.State.Entities,
                entityId,
                clock with
                {
                    CommandCounter =
                        clock.CommandCounter +
                        context.Envelope.Payload.Amount,
                });

            if (!mutation.WasApplied)
            {
                return global::AI.Sandbox.Engine.Core.Commands
                    .CommandDecision<TimedWorldState>.Reject(
                        mutation.Status.ToString());
            }

            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<TimedWorldState>.Accept(
                    context.Snapshot.State with
                    {
                        Components = mutation.Registry,
                    });
        }
    }

    private sealed class TimedWorldCodec :
        global::AI.Sandbox.Engine.Core.Persistence
            .IWorldStateSnapshotCodec<TimedWorldState>
    {
        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaId SchemaId { get; } =
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaId.Parse("time.validation");

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
            .SnapshotPayload Encode(TimedWorldState state)
        {
            var clock = GetClock(state);
            var text = string.Join(
                '|',
                clock.Timeline.TickDuration.Microseconds.ToString(
                    System.Globalization.CultureInfo.InvariantCulture),
                clock.CurrentInstant.MicrosecondsSinceEpoch.ToString(
                    System.Globalization.CultureInfo.InvariantCulture),
                clock.CommandCounter.ToString(
                    System.Globalization.CultureInfo.InvariantCulture));

            return global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotPayload.From(
                    System.Text.Encoding.UTF8.GetBytes(text));
        }

        public global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<TimedWorldState> Decode(
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

            if (parts.Length != 3 ||
                !ulong.TryParse(
                    parts[0],
                    System.Globalization.NumberStyles.None,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var tickMicroseconds) ||
                !ulong.TryParse(
                    parts[1],
                    System.Globalization.NumberStyles.None,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var instantMicroseconds) ||
                !int.TryParse(
                    parts[2],
                    System.Globalization.NumberStyles.Integer,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var commandCounter))
            {
                return Reject("Malformed simulation time payload.");
            }

            global::AI.Sandbox.Engine.Core.Time.SimulationTimeline timeline;

            try
            {
                timeline = global::AI.Sandbox.Engine.Core.Time
                    .SimulationTimeline.Create(
                        global::AI.Sandbox.Engine.Core.Time
                            .SimulationTickDuration.FromMicroseconds(
                                tickMicroseconds));
            }
            catch (ArgumentOutOfRangeException exception)
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
                        new ClockComponent(
                            timeline,
                            global::AI.Sandbox.Engine.Core.Time
                                .SimulationInstant.FromMicroseconds(
                                    instantMicroseconds),
                            commandCounter))
                    .Build();

            return global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateDecodeDecision<TimedWorldState>.Accept(
                    new TimedWorldState(entities, components));
        }

        private static global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<TimedWorldState> Reject(
                string reason)
        {
            return global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateDecodeDecision<TimedWorldState>.Reject(reason);
        }
    }
}
