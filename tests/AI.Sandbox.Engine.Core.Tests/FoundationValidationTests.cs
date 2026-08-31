namespace AI.Sandbox.Engine.Core.Tests;

public sealed class FoundationValidationTests
{
    private readonly record struct Counter(int Value) :
        global::AI.Sandbox.Engine.Core.Components.IComponent;

    private readonly record struct Lifetime(int RemainingTicks) :
        global::AI.Sandbox.Engine.Core.Components.IComponent;

    private sealed record FoundationWorldState(
        global::AI.Sandbox.Engine.Core.Entities.EntityRegistry Entities,
        global::AI.Sandbox.Engine.Core.Components.ComponentRegistry Components) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    private readonly record struct FoundationCommitted(
        ulong SimulationTick,
        ulong WorldStateVersion,
        string Checksum) :
        global::AI.Sandbox.Engine.Core.Events.IEngineEvent;

    private sealed record ScenarioResult(
        global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<FoundationWorldState> Manager,
        global::AI.Sandbox.Engine.Core.Persistence.WorldSnapshotDocument Document);

    [Xunit.Fact]
    public async Task UninterruptedAndSaveRestoreContinuationAreIdentical()
    {
        var uninterrupted = RunScenario(
            totalTicks: 8,
            checkpointTick: null,
            reverseInitialization: false);
        var resumed = RunScenario(
            totalTicks: 8,
            checkpointTick: 2,
            reverseInitialization: false);

        AssertEquivalent(uninterrupted, resumed);

        var state = resumed.Manager.Read().State;
        var persistentEntity = CreateEntityId(1);
        var expiringEntity = CreateEntityId(2);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Entities
                .EntityLifecycleStatus.Active,
            state.Entities.GetLifecycleStatus(persistentEntity));
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Entities
                .EntityLifecycleStatus.Destroyed,
            state.Entities.GetLifecycleStatus(expiringEntity));
        Xunit.Assert.True(
            state.Components.TryGet<Counter>(
                persistentEntity,
                out var persistentCounter));
        Xunit.Assert.Equal(8, persistentCounter.Value);
        Xunit.Assert.False(
            state.Components.Contains<Counter>(expiringEntity));
        Xunit.Assert.False(
            state.Components.Contains<Lifetime>(expiringEntity));
        Xunit.Assert.True(
            state.Components.IsConsistentWith(state.Entities));

        var recreate = state.Entities.CreateEntity(expiringEntity);
        Xunit.Assert.False(recreate.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Entities
                .EntityMutationStatus.AlreadyKnown,
            recreate.Status);

        var beforeDispatch = resumed.Manager.Read();
        var calls = new List<string>();
        var dispatcher =
            new global::AI.Sandbox.Engine.Core.Events.EventDispatcherBuilder()
                .Add<FoundationCommitted>(
                    new RecordingHandler("first", calls))
                .Add<FoundationCommitted>(
                    new RecordingHandler("second", calls))
                .Build();
        var envelope = global::AI.Sandbox.Engine.Core.Events
            .EventEnvelope<FoundationCommitted>.Create(
                CreateEventId(),
                sequence: 1,
                simulationTick: beforeDispatch.SimulationTick,
                new FoundationCommitted(
                    beforeDispatch.SimulationTick,
                    beforeDispatch.Version.Value,
                    resumed.Document.Checksum.Value));

        await dispatcher.DispatchAsync(envelope);

        Xunit.Assert.Equal(new[] { "first", "second" }, calls);
        Xunit.Assert.Same(beforeDispatch, resumed.Manager.Read());
    }

    [Xunit.Fact]
    public void IndependentConstructionOrdersProduceIdenticalSnapshots()
    {
        var forward = RunScenario(
            totalTicks: 8,
            checkpointTick: null,
            reverseInitialization: false);
        var reverse = RunScenario(
            totalTicks: 8,
            checkpointTick: null,
            reverseInitialization: true);

        AssertEquivalent(forward, reverse);
    }

    [Xunit.Fact]
    public void EverySuccessfulTickAdvancesVersionAndLogicalTimeExactlyOnce()
    {
        var manager = CreateManager(reverseInitialization: false);
        var scheduler = CreateScheduler(manager);

        for (var expected = 1UL; expected <= 12UL; expected++)
        {
            var result = scheduler.RunNextTick();

            Xunit.Assert.True(result.WasApplied);
            Xunit.Assert.Equal(expected, result.Snapshot.Version.Value);
            Xunit.Assert.Equal(expected, result.Snapshot.SimulationTick);
            Xunit.Assert.Equal(2, result.ExecutedSystemCount);
        }
    }

    [Xunit.Fact]
    public void FinalSnapshotCanBeCapturedRestoredAndContinuedAgain()
    {
        var initial = RunScenario(
            totalTicks: 8,
            checkpointTick: 2,
            reverseInitialization: false);
        var persistence = CreatePersistence();
        var restored = persistence.Restore(initial.Document);

        Xunit.Assert.True(restored.WasRestored);
        Xunit.Assert.NotNull(restored.Snapshot);

        var manager = global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<FoundationWorldState>.Restore(
                restored.Snapshot!);
        var scheduler = CreateScheduler(manager);

        for (var index = 0; index < 4; index++)
        {
            Xunit.Assert.True(scheduler.RunNextTick().WasApplied);
        }

        var continued = persistence.Capture(manager.Read());

        Xunit.Assert.Equal(12UL, continued.WorldStateVersion.Value);
        Xunit.Assert.Equal(12UL, continued.SimulationTick);
        Xunit.Assert.True(continued.HasValidChecksum);
        Xunit.Assert.True(
            manager.Read().State.Components.IsConsistentWith(
                manager.Read().State.Entities));
    }

    private static ScenarioResult RunScenario(
        int totalTicks,
        int? checkpointTick,
        bool reverseInitialization)
    {
        var persistence = CreatePersistence();
        var manager = CreateManager(reverseInitialization);
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
                .WorldStateManager<FoundationWorldState>.Restore(
                    restored.Snapshot!);
            scheduler = CreateScheduler(manager);

            RunTicks(
                scheduler,
                totalTicks - checkpointTick.Value);
        }

        return new ScenarioResult(
            manager,
            persistence.Capture(manager.Read()));
    }

    private static void RunTicks(
        global::AI.Sandbox.Engine.Core.Simulation
            .SimulationScheduler<FoundationWorldState> scheduler,
        int count)
    {
        for (var index = 0; index < count; index++)
        {
            var result = scheduler.RunNextTick();
            Xunit.Assert.True(result.WasApplied);
        }
    }

    private static void AssertEquivalent(
        ScenarioResult expected,
        ScenarioResult actual)
    {
        Xunit.Assert.Equal(
            expected.Document.WorldId,
            actual.Document.WorldId);
        Xunit.Assert.Equal(
            expected.Document.WorldStateVersion,
            actual.Document.WorldStateVersion);
        Xunit.Assert.Equal(
            expected.Document.SimulationTick,
            actual.Document.SimulationTick);
        Xunit.Assert.Equal(
            expected.Document.SchemaId,
            actual.Document.SchemaId);
        Xunit.Assert.Equal(
            expected.Document.SchemaVersion,
            actual.Document.SchemaVersion);
        Xunit.Assert.Equal(
            expected.Document.Checksum,
            actual.Document.Checksum);
        Xunit.Assert.True(
            expected.Document.Payload.ContentEquals(
                actual.Document.Payload));
    }

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<FoundationWorldState> CreateManager(
            bool reverseInitialization)
    {
        var persistentEntity = CreateEntityId(1);
        var expiringEntity = CreateEntityId(2);
        var inputOrder = reverseInitialization
            ? new[] { expiringEntity, persistentEntity }
            : new[] { persistentEntity, expiringEntity };
        var entities =
            global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
                .FromActiveEntities(inputOrder);
        var builder =
            new global::AI.Sandbox.Engine.Core.Components
                .ComponentRegistryBuilder(entities);

        if (reverseInitialization)
        {
            _ = builder
                .Add(expiringEntity, new Lifetime(3))
                .Add(expiringEntity, new Counter(100))
                .Add(persistentEntity, new Counter(0));
        }
        else
        {
            _ = builder
                .Add(persistentEntity, new Counter(0))
                .Add(expiringEntity, new Counter(100))
                .Add(expiringEntity, new Lifetime(3));
        }

        var state = new FoundationWorldState(
            entities,
            builder.Build());

        return global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<FoundationWorldState>.Create(
                CreateWorldId(),
                state);
    }

    private static global::AI.Sandbox.Engine.Core.Simulation
        .SimulationScheduler<FoundationWorldState> CreateScheduler(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<FoundationWorldState> manager)
    {
        return new global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSchedulerBuilder<FoundationWorldState>()
            .Add(CreateSystemId(1), new CounterSystem())
            .Add(CreateSystemId(2), new LifetimeSystem())
            .Build(manager);
    }

    private static global::AI.Sandbox.Engine.Core.Persistence
        .WorldStateSnapshotPersistence<FoundationWorldState>
        CreatePersistence()
    {
        return new global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateSnapshotPersistence<FoundationWorldState>(
                new FoundationCodec());
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> CreateWorldId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000000700");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> CreateEntityId(
            int suffix)
    {
        var text = $"019b0000-0000-7000-8700-{suffix:D12}";
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(text);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Simulation.SimulationSystemIdKind>
        CreateSystemId(int suffix)
    {
        var text = $"019b0000-0000-7000-8800-{suffix:D12}";
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemIdKind>.Parse(text);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Events.EventIdKind> CreateEventId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Events.EventIdKind>.Parse(
                "019b0000-0000-7000-8900-000000000001");
    }

    private sealed class CounterSystem :
        global::AI.Sandbox.Engine.Core.Simulation
            .ISimulationSystem<FoundationWorldState>
    {
        public global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<FoundationWorldState> Execute(
                global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemContext<FoundationWorldState> context)
        {
            var components = context.State.Components;

            foreach (var entityId in
                context.State.Components.GetEntityIds<Counter>())
            {
                if (!components.TryGet<Counter>(
                    entityId,
                    out var counter))
                {
                    return global::AI.Sandbox.Engine.Core.Simulation
                        .SimulationSystemDecision<FoundationWorldState>.Reject(
                            "Counter index and value store diverged.");
                }

                var mutation = components.Set(
                    context.State.Entities,
                    entityId,
                    new Counter(counter.Value + 1));

                if (!mutation.WasApplied)
                {
                    return global::AI.Sandbox.Engine.Core.Simulation
                        .SimulationSystemDecision<FoundationWorldState>.Reject(
                            mutation.Status.ToString());
                }

                components = mutation.Registry;
            }

            return global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemDecision<FoundationWorldState>.Update(
                    context.State with
                    {
                        Components = components,
                    });
        }
    }

    private sealed class LifetimeSystem :
        global::AI.Sandbox.Engine.Core.Simulation
            .ISimulationSystem<FoundationWorldState>
    {
        public global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<FoundationWorldState> Execute(
                global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemContext<FoundationWorldState> context)
        {
            var entities = context.State.Entities;
            var components = context.State.Components;
            var lifetimeOwners =
                context.State.Components.GetEntityIds<Lifetime>().ToArray();

            foreach (var entityId in lifetimeOwners)
            {
                if (!components.TryGet<Lifetime>(
                    entityId,
                    out var lifetime))
                {
                    return global::AI.Sandbox.Engine.Core.Simulation
                        .SimulationSystemDecision<FoundationWorldState>.Reject(
                            "Lifetime index and value store diverged.");
                }

                var nextRemaining = lifetime.RemainingTicks - 1;
                if (nextRemaining > 0)
                {
                    var updated = components.Set(
                        entities,
                        entityId,
                        new Lifetime(nextRemaining));

                    if (!updated.WasApplied)
                    {
                        return global::AI.Sandbox.Engine.Core.Simulation
                            .SimulationSystemDecision<FoundationWorldState>
                            .Reject(updated.Status.ToString());
                    }

                    components = updated.Registry;
                    continue;
                }

                var destroyed = entities.DestroyEntity(entityId);
                if (!destroyed.WasApplied)
                {
                    return global::AI.Sandbox.Engine.Core.Simulation
                        .SimulationSystemDecision<FoundationWorldState>.Reject(
                            destroyed.Status.ToString());
                }

                entities = destroyed.Registry;
                components = components.PurgeEntity(entityId).Registry;
            }

            if (!components.IsConsistentWith(entities))
            {
                return global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemDecision<FoundationWorldState>.Reject(
                        "Entity and Component registries are inconsistent.");
            }

            return global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemDecision<FoundationWorldState>.Update(
                    context.State with
                    {
                        Entities = entities,
                        Components = components,
                    });
        }
    }

    private sealed class FoundationCodec :
        global::AI.Sandbox.Engine.Core.Persistence
            .IWorldStateSnapshotCodec<FoundationWorldState>
    {
        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaId SchemaId { get; } =
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaId.Parse("foundation.validation");

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
            .SnapshotPayload Encode(FoundationWorldState state)
        {
            if (!state.Components.IsConsistentWith(state.Entities))
            {
                throw new InvalidOperationException(
                    "Cannot encode an inconsistent foundation state.");
            }

            var builder = new System.Text.StringBuilder();
            _ = builder.Append("foundation-validation-v1");
            _ = builder.Append('\n');

            foreach (var entityId in state.Entities.KnownEntities)
            {
                var lifecycle =
                    state.Entities.GetLifecycleStatus(entityId);
                _ = builder.Append(entityId);
                _ = builder.Append('|');

                if (lifecycle ==
                    global::AI.Sandbox.Engine.Core.Entities
                        .EntityLifecycleStatus.Destroyed)
                {
                    _ = builder.Append('D');
                    _ = builder.Append('\n');
                    continue;
                }

                if (lifecycle !=
                        global::AI.Sandbox.Engine.Core.Entities
                            .EntityLifecycleStatus.Active ||
                    !state.Components.TryGet<Counter>(
                        entityId,
                        out var counter))
                {
                    throw new InvalidOperationException(
                        "Active foundation entity is missing required data.");
                }

                _ = builder.Append("A|");
                _ = builder.Append(
                    counter.Value.ToString(
                        System.Globalization.CultureInfo.InvariantCulture));
                _ = builder.Append('|');

                if (state.Components.TryGet<Lifetime>(
                    entityId,
                    out var lifetime))
                {
                    _ = builder.Append(
                        lifetime.RemainingTicks.ToString(
                            System.Globalization.CultureInfo.InvariantCulture));
                }
                else
                {
                    _ = builder.Append('-');
                }

                _ = builder.Append('\n');
            }

            return global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotPayload.From(
                    System.Text.Encoding.UTF8.GetBytes(
                        builder.ToString()));
        }

        public global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<FoundationWorldState> Decode(
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion version,
                global::AI.Sandbox.Engine.Core.Persistence
                    .SnapshotPayload payload)
        {
            if (version != CurrentSchemaVersion)
            {
                return Reject("Unsupported foundation schema version.");
            }

            var text = System.Text.Encoding.UTF8.GetString(
                payload.ToArray());
            var lines = text.Split(
                '\n',
                StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length < 2 ||
                !string.Equals(
                    lines[0],
                    "foundation-validation-v1",
                    StringComparison.Ordinal))
            {
                return Reject("Invalid foundation payload header.");
            }

            var decoded = new List<DecodedEntity>();

            for (var index = 1; index < lines.Length; index++)
            {
                var parts = lines[index].Split('|');

                if (!global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.Entities
                            .EntityIdKind>.TryParse(
                            parts[0],
                            out var entityId))
                {
                    return Reject("Invalid entity ID.");
                }

                if (parts.Length == 2 &&
                    string.Equals(
                        parts[1],
                        "D",
                        StringComparison.Ordinal))
                {
                    decoded.Add(
                        new DecodedEntity(
                            EntityId: entityId,
                            IsActive: false,
                            CounterValue: 0,
                            LifetimeValue: null));
                    continue;
                }

                if (parts.Length != 4 ||
                    !string.Equals(
                        parts[1],
                        "A",
                        StringComparison.Ordinal) ||
                    !int.TryParse(
                        parts[2],
                        System.Globalization.NumberStyles.Integer,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var counterValue) ||
                    counterValue < 0)
                {
                    return Reject("Invalid active entity payload.");
                }

                int? lifetimeValue = null;
                if (!string.Equals(
                    parts[3],
                    "-",
                    StringComparison.Ordinal))
                {
                    if (!int.TryParse(
                        parts[3],
                        System.Globalization.NumberStyles.Integer,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var parsedLifetime) ||
                        parsedLifetime <= 0)
                    {
                        return Reject("Invalid lifetime payload.");
                    }

                    lifetimeValue = parsedLifetime;
                }

                decoded.Add(
                    new DecodedEntity(
                        EntityId: entityId,
                        IsActive: true,
                        CounterValue: counterValue,
                        LifetimeValue: lifetimeValue));
            }

            var allIds = decoded
                .Select(item => item.EntityId)
                .ToArray();

            global::AI.Sandbox.Engine.Core.Entities.EntityRegistry entities;

            try
            {
                entities = global::AI.Sandbox.Engine.Core.Entities
                    .EntityRegistry.FromActiveEntities(allIds);
            }
            catch (ArgumentException)
            {
                return Reject("Duplicate or invalid entity identity.");
            }

            foreach (var item in decoded.Where(item => !item.IsActive))
            {
                var destroyed = entities.DestroyEntity(item.EntityId);
                if (!destroyed.WasApplied)
                {
                    return Reject("Destroyed entity restoration failed.");
                }

                entities = destroyed.Registry;
            }

            var componentBuilder =
                new global::AI.Sandbox.Engine.Core.Components
                    .ComponentRegistryBuilder(entities);

            foreach (var item in decoded.Where(item => item.IsActive))
            {
                _ = componentBuilder.Add(
                    item.EntityId,
                    new Counter(item.CounterValue));

                if (item.LifetimeValue is { } lifetime)
                {
                    _ = componentBuilder.Add(
                        item.EntityId,
                        new Lifetime(lifetime));
                }
            }

            var state = new FoundationWorldState(
                entities,
                componentBuilder.Build());

            return global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateDecodeDecision<FoundationWorldState>.Accept(
                    state);
        }

        private static global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<FoundationWorldState> Reject(
                string reason)
        {
            return global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateDecodeDecision<FoundationWorldState>.Reject(
                    reason);
        }
    }

    private sealed record DecodedEntity(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> EntityId,
        bool IsActive,
        int CounterValue,
        int? LifetimeValue);

    private sealed class RecordingHandler :
        global::AI.Sandbox.Engine.Core.Events
            .IEventHandler<FoundationCommitted>
    {
        private readonly string name;
        private readonly List<string> calls;

        public RecordingHandler(
            string name,
            List<string> calls)
        {
            this.name = name;
            this.calls = calls;
        }

        public ValueTask HandleAsync(
            global::AI.Sandbox.Engine.Core.Events
                .EventEnvelope<FoundationCommitted> envelope,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Xunit.Assert.True(envelope.Payload.SimulationTick > 0);
            Xunit.Assert.True(envelope.Payload.WorldStateVersion > 0);
            Xunit.Assert.False(
                string.IsNullOrWhiteSpace(envelope.Payload.Checksum));

            calls.Add(name);
            return ValueTask.CompletedTask;
        }
    }
}
