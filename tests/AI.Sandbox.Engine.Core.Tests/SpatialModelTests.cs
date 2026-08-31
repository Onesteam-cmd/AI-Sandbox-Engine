namespace AI.Sandbox.Engine.Core.Tests;

public sealed class SpatialModelTests
{
    private sealed record SpatialWorldState(
        global::AI.Sandbox.Engine.Core.Spatial.SpatialTopology Topology,
        global::AI.Sandbox.Engine.Core.Entities.EntityRegistry Entities,
        global::AI.Sandbox.Engine.Core.Components.ComponentRegistry Components) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    private readonly record struct MoveEntity(
        global::AI.Sandbox.Engine.Core.Spatial.SpatialPosition Target) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    [Xunit.Fact]
    public void DistanceFactoriesAndArithmeticUseExactMillimeters()
    {
        Xunit.Assert.Equal(
            10UL,
            global::AI.Sandbox.Engine.Core.Spatial
                .SpatialDistance.FromCentimeters(1).Millimeters);
        Xunit.Assert.Equal(
            1_000UL,
            global::AI.Sandbox.Engine.Core.Spatial
                .SpatialDistance.FromMeters(1).Millimeters);
        Xunit.Assert.Equal(
            1_000_000UL,
            global::AI.Sandbox.Engine.Core.Spatial
                .SpatialDistance.FromKilometers(1).Millimeters);

        var distance = global::AI.Sandbox.Engine.Core.Spatial
            .SpatialDistance.FromMeters(2);

        Xunit.Assert.Equal(
            2_500UL,
            distance.Add(
                global::AI.Sandbox.Engine.Core.Spatial
                    .SpatialDistance.FromCentimeters(50)).Millimeters);
        Xunit.Assert.Equal(
            6_000UL,
            distance.Multiply(3).Millimeters);
        Xunit.Assert.Throws<OverflowException>(
            () => global::AI.Sandbox.Engine.Core.Spatial
                .SpatialDistance.FromMillimeters(ulong.MaxValue)
                .Add(
                    global::AI.Sandbox.Engine.Core.Spatial
                        .SpatialDistance.FromMillimeters(1)));
    }

    [Xunit.Fact]
    public void PointsUseExactSquaredDistanceWithoutSquareRoot()
    {
        var origin =
            global::AI.Sandbox.Engine.Core.Spatial.SpatialPoint.Origin;
        var point = global::AI.Sandbox.Engine.Core.Spatial
            .SpatialPoint.Create(
                3_000,
                4_000,
                12_000);

        Xunit.Assert.Equal(
            (System.UInt128)169_000_000,
            origin.GetSquaredDistanceTo(point));
        Xunit.Assert.True(
            origin.IsWithin(
                point,
                global::AI.Sandbox.Engine.Core.Spatial
                    .SpatialDistance.FromMeters(13)));
        Xunit.Assert.False(
            origin.IsWithin(
                point,
                global::AI.Sandbox.Engine.Core.Spatial
                    .SpatialDistance.FromMillimeters(12_999)));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.Spatial
                .SpatialPoint.Create(
                    1_000_000_000_001,
                    0,
                    0));
    }

    [Xunit.Fact]
    public void PositionsRequirePlacesAndMeasureOnlyInsideOnePlace()
    {
        global::AI.Sandbox.Engine.Core.Spatial.SpatialPosition empty =
            default;
        var first = global::AI.Sandbox.Engine.Core.Spatial
            .SpatialPosition.Create(
                CreatePlaceId(3),
                global::AI.Sandbox.Engine.Core.Spatial
                    .SpatialPoint.Origin);
        var second = global::AI.Sandbox.Engine.Core.Spatial
            .SpatialPosition.Create(
                CreatePlaceId(3),
                global::AI.Sandbox.Engine.Core.Spatial
                    .SpatialPoint.Create(0, 3, 4));
        var otherPlace = global::AI.Sandbox.Engine.Core.Spatial
            .SpatialPosition.Create(
                CreatePlaceId(4),
                global::AI.Sandbox.Engine.Core.Spatial
                    .SpatialPoint.Create(0, 3, 4));

        Xunit.Assert.True(empty.IsEmpty);
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Spatial
                .SpatialPosition.Create(
                    default,
                    global::AI.Sandbox.Engine.Core.Spatial
                        .SpatialPoint.Origin));
        Xunit.Assert.True(
            first.TryGetSquaredDistanceTo(
                second,
                out var squaredDistance));
        Xunit.Assert.Equal(
            (System.UInt128)25,
            squaredDistance);
        Xunit.Assert.False(
            first.TryGetSquaredDistanceTo(
                otherPlace,
                out var unavailableDistance));
        Xunit.Assert.Equal(
            (System.UInt128)0,
            unavailableDistance);
        Xunit.Assert.Throws<InvalidOperationException>(
            () => empty.TryGetSquaredDistanceTo(
                first,
                out _));
    }

    [Xunit.Fact]
    public void BuilderRejectsDuplicatesUnknownEndpointsAndContainmentCycles()
    {
        var duplicateBuilder =
            new global::AI.Sandbox.Engine.Core.Spatial
                .SpatialTopologyBuilder()
                .AddPlace(CreatePlaceId(1));

        Xunit.Assert.Throws<ArgumentException>(
            () => duplicateBuilder.AddPlace(
                CreatePlaceId(1)));

        var unknownParent =
            new global::AI.Sandbox.Engine.Core.Spatial
                .SpatialTopologyBuilder()
                .AddPlace(
                    CreatePlaceId(2),
                    CreatePlaceId(99));

        Xunit.Assert.Throws<InvalidOperationException>(
            () => unknownParent.Build());

        var cycle =
            new global::AI.Sandbox.Engine.Core.Spatial
                .SpatialTopologyBuilder()
                .AddPlace(
                    CreatePlaceId(1),
                    CreatePlaceId(2))
                .AddPlace(
                    CreatePlaceId(2),
                    CreatePlaceId(1));

        Xunit.Assert.Throws<InvalidOperationException>(
            () => cycle.Build());

        var unknownEndpoint =
            new global::AI.Sandbox.Engine.Core.Spatial
                .SpatialTopologyBuilder()
                .AddPlace(CreatePlaceId(1))
                .AddDirectedConnection(
                    CreatePlaceId(1),
                    CreatePlaceId(2),
                    global::AI.Sandbox.Engine.Core.Spatial
                        .SpatialDistance.Zero);

        Xunit.Assert.Throws<InvalidOperationException>(
            () => unknownEndpoint.Build());
        Xunit.Assert.Throws<ArgumentException>(
            () => new global::AI.Sandbox.Engine.Core.Spatial
                .SpatialTopologyBuilder()
                .AddDirectedConnection(
                    CreatePlaceId(1),
                    CreatePlaceId(1),
                    global::AI.Sandbox.Engine.Core.Spatial
                        .SpatialDistance.Zero));
    }

    [Xunit.Fact]
    public void TopologyIsDeterministicAcrossRegistrationOrder()
    {
        var forward = CreateTopology(reverseOrder: false);
        var reverse = CreateTopology(reverseOrder: true);

        Xunit.Assert.Equal(
            forward.Places.ToArray(),
            reverse.Places.ToArray());
        Xunit.Assert.Equal(
            forward.Connections.ToArray(),
            reverse.Connections.ToArray());
        Xunit.Assert.Equal(4, forward.PlaceCount);
        Xunit.Assert.Equal(4, forward.ConnectionCount);
    }

    [Xunit.Fact]
    public void ContainmentAndDirectedConnectionsRemainExplicit()
    {
        var topology = CreateTopology(reverseOrder: false);

        Xunit.Assert.True(
            topology.IsContainedWithin(
                CreatePlaceId(4),
                CreatePlaceId(1)));
        Xunit.Assert.True(
            topology.IsContainedWithin(
                CreatePlaceId(4),
                CreatePlaceId(4)));
        Xunit.Assert.False(
            topology.IsContainedWithin(
                CreatePlaceId(3),
                CreatePlaceId(4)));
        Xunit.Assert.Equal(
            new[]
            {
                CreatePlaceId(2),
                CreatePlaceId(1),
            },
            topology.GetAncestors(
                CreatePlaceId(4)).ToArray());
        Xunit.Assert.Single(
            topology.GetOutgoingConnections(
                CreatePlaceId(3)));
        Xunit.Assert.True(
            topology.TryGetDirectedConnection(
                CreatePlaceId(3),
                CreatePlaceId(4),
                out var connection));
        Xunit.Assert.Equal(
            7_500UL,
            connection.Distance.Millimeters);
        Xunit.Assert.False(
            topology.TryGetDirectedConnection(
                CreatePlaceId(1),
                CreatePlaceId(4),
                out _));
    }

    [Xunit.Fact]
    public void PositionContainmentUsesPlaceHierarchyNotLocalCoordinates()
    {
        var topology = CreateTopology(reverseOrder: false);
        var position = global::AI.Sandbox.Engine.Core.Spatial
            .SpatialPosition.Create(
                CreatePlaceId(4),
                global::AI.Sandbox.Engine.Core.Spatial
                    .SpatialPoint.Create(
                        999_999,
                        -500,
                        25));

        Xunit.Assert.True(
            topology.IsPositionWithin(
                position,
                CreatePlaceId(1)));
        Xunit.Assert.True(
            topology.IsPositionWithin(
                position,
                CreatePlaceId(2)));
        Xunit.Assert.False(
            topology.IsPositionWithin(
                position,
                CreatePlaceId(3)));
        Xunit.Assert.Throws<ArgumentException>(
            () => topology.IsPositionWithin(
                position,
                CreatePlaceId(99)));
    }

    [Xunit.Fact]
    public void RuntimeMovementAndTickUseSpatialPositionAsAuthoritativeData()
    {
        var manager = CreateManager();
        var runtime = CreateRuntime(manager);
        var target = global::AI.Sandbox.Engine.Core.Spatial
            .SpatialPosition.Create(
                CreatePlaceId(4),
                global::AI.Sandbox.Engine.Core.Spatial
                    .SpatialPoint.Create(
                        100,
                        200,
                        300));
        var command = runtime.ExecuteCommand(
            global::AI.Sandbox.Engine.Core.Commands
                .CommandEnvelope<MoveEntity>.Create(
                    CreateCommandId(1),
                    runtime.Read().WorldId,
                    runtime.Read().Version,
                    runtime.Read().SimulationTick,
                    new MoveEntity(target)));
        var tick = runtime.RunNextTick();

        Xunit.Assert.True(command.WasCommitted);
        Xunit.Assert.True(tick.WasCommitted);

        var finalPosition = GetPosition(
            runtime.Read().State);
        Xunit.Assert.Equal(
            CreatePlaceId(4),
            finalPosition.PlaceId);
        Xunit.Assert.Equal(
            101L,
            finalPosition.Point.XMillimeters);
        Xunit.Assert.Equal(
            200L,
            finalPosition.Point.YMillimeters);
        Xunit.Assert.Equal(
            300L,
            finalPosition.Point.ZMillimeters);
        Xunit.Assert.True(
            runtime.Read().State.Topology.IsPositionWithin(
                finalPosition,
                CreatePlaceId(1)));
    }

    [Xunit.Fact]
    public void SaveRestoreContinuationWithSpatialRuntimeIsByteIdentical()
    {
        var uninterrupted = RunScenario(
            cycles: 40,
            checkpointCycle: null);
        var resumed = RunScenario(
            cycles: 40,
            checkpointCycle: 13);

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
            int cycles,
            int? checkpointCycle)
    {
        var persistence =
            new global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateSnapshotPersistence<SpatialWorldState>(
                    new SpatialWorldCodec());
        var manager = CreateManager();
        var runtime = CreateRuntime(manager);

        if (checkpointCycle is null)
        {
            RunCycles(
                runtime,
                startCycle: 0,
                count: cycles);
        }
        else
        {
            RunCycles(
                runtime,
                startCycle: 0,
                count: checkpointCycle.Value);
            var checkpoint = persistence.Capture(
                runtime.Read());
            var restored = persistence.Restore(
                checkpoint);

            Xunit.Assert.True(restored.WasRestored);
            Xunit.Assert.NotNull(restored.Snapshot);

            manager = global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<SpatialWorldState>.Restore(
                    restored.Snapshot!);
            runtime = CreateRuntime(manager);
            RunCycles(
                runtime,
                startCycle: checkpointCycle.Value,
                count: cycles - checkpointCycle.Value);
        }

        return persistence.Capture(
            runtime.Read());
    }

    private static void RunCycles(
        global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestrator<SpatialWorldState> runtime,
        int startCycle,
        int count)
    {
        for (var offset = 0;
            offset < count;
            offset++)
        {
            var cycle = startCycle + offset;
            var ordinal = cycle + 1;
            var placeId = cycle % 2 == 0
                ? CreatePlaceId(3)
                : CreatePlaceId(4);
            var target = global::AI.Sandbox.Engine.Core.Spatial
                .SpatialPosition.Create(
                    placeId,
                    global::AI.Sandbox.Engine.Core.Spatial
                        .SpatialPoint.Create(
                            ordinal * 10L,
                            ordinal * -3L,
                            ordinal));
            var observed = runtime.Read();
            var command = runtime.ExecuteCommand(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandEnvelope<MoveEntity>.Create(
                        CreateCommandId(cycle + 10),
                        observed.WorldId,
                        observed.Version,
                        observed.SimulationTick,
                        new MoveEntity(target)));
            var tick = runtime.RunNextTick();

            Xunit.Assert.True(
                command.WasCommitted,
                $"Cycle {cycle}: command invocation " +
                $"'{command.InvocationStatus}', result " +
                $"'{command.CommandResult?.Status.ToString() ?? "none"}'.");
            Xunit.Assert.True(
                tick.WasCommitted,
                $"Cycle {cycle}: tick invocation " +
                $"'{tick.InvocationStatus}', result " +
                $"'{tick.SimulationResult?.Status.ToString() ?? "none"}'.");
        }
    }

    private static global::AI.Sandbox.Engine.Core.Runtime
        .RuntimeOrchestrator<SpatialWorldState> CreateRuntime(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<SpatialWorldState> manager)
    {
        return new global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestratorBuilder<SpatialWorldState>()
            .AddCommandHandler(new MoveHandler())
            .AddSimulationSystem(
                CreateSystemId(),
                new NudgeSystem())
            .Build(manager);
    }

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<SpatialWorldState> CreateManager()
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
                    global::AI.Sandbox.Engine.Core.Spatial
                        .SpatialPosition.Create(
                            CreatePlaceId(3),
                            global::AI.Sandbox.Engine.Core.Spatial
                                .SpatialPoint.Origin))
                .Build();

        return global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<SpatialWorldState>.Create(
                CreateWorldId(),
                new SpatialWorldState(
                    CreateTopology(reverseOrder: false),
                    entities,
                    components));
    }

    private static global::AI.Sandbox.Engine.Core.Spatial
        .SpatialTopology CreateTopology(bool reverseOrder)
    {
        var builder =
            new global::AI.Sandbox.Engine.Core.Spatial
                .SpatialTopologyBuilder();

        if (reverseOrder)
        {
            _ = builder
                .AddPlace(
                    CreatePlaceId(4),
                    CreatePlaceId(2))
                .AddPlace(
                    CreatePlaceId(3),
                    CreatePlaceId(2))
                .AddPlace(
                    CreatePlaceId(2),
                    CreatePlaceId(1))
                .AddPlace(CreatePlaceId(1))
                .AddBidirectionalConnection(
                    CreatePlaceId(4),
                    CreatePlaceId(3),
                    global::AI.Sandbox.Engine.Core.Spatial
                        .SpatialDistance.FromMillimeters(7_500))
                .AddBidirectionalConnection(
                    CreatePlaceId(2),
                    CreatePlaceId(1),
                    global::AI.Sandbox.Engine.Core.Spatial
                        .SpatialDistance.FromMeters(20));
        }
        else
        {
            _ = builder
                .AddPlace(CreatePlaceId(1))
                .AddPlace(
                    CreatePlaceId(2),
                    CreatePlaceId(1))
                .AddPlace(
                    CreatePlaceId(3),
                    CreatePlaceId(2))
                .AddPlace(
                    CreatePlaceId(4),
                    CreatePlaceId(2))
                .AddBidirectionalConnection(
                    CreatePlaceId(1),
                    CreatePlaceId(2),
                    global::AI.Sandbox.Engine.Core.Spatial
                        .SpatialDistance.FromMeters(20))
                .AddBidirectionalConnection(
                    CreatePlaceId(3),
                    CreatePlaceId(4),
                    global::AI.Sandbox.Engine.Core.Spatial
                        .SpatialDistance.FromMillimeters(7_500));
        }

        return builder.Build();
    }

    private static global::AI.Sandbox.Engine.Core.Spatial
        .SpatialPosition GetPosition(
            SpatialWorldState state)
    {
        Xunit.Assert.True(
            state.Components.TryGet<
                global::AI.Sandbox.Engine.Core.Spatial
                    .SpatialPosition>(
                        CreateEntityId(),
                        out var position));

        return position;
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Spatial.SpatialPlaceIdKind>
        CreatePlaceId(int suffix)
    {
        var text = $"019b0000-0000-7000-9500-{suffix:D12}";
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Spatial
                .SpatialPlaceIdKind>.Parse(text);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> CreateEntityId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(
                "019b0000-0000-7000-9600-000000000001");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> CreateWorldId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000001200");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Commands.CommandIdKind> CreateCommandId(
            int suffix)
    {
        var text = $"019b0000-0000-7000-9700-{suffix:D12}";
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Commands.CommandIdKind>.Parse(text);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Simulation.SimulationSystemIdKind>
        CreateSystemId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemIdKind>.Parse(
                    "019b0000-0000-7000-9800-000000000001");
    }

    private sealed class MoveHandler :
        global::AI.Sandbox.Engine.Core.Commands
            .ICommandHandler<SpatialWorldState, MoveEntity>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<SpatialWorldState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<SpatialWorldState, MoveEntity> context)
        {
            var target = context.Envelope.Payload.Target;

            if (target.IsEmpty ||
                !context.Snapshot.State.Topology.ContainsPlace(
                    target.PlaceId))
            {
                return global::AI.Sandbox.Engine.Core.Commands
                    .CommandDecision<SpatialWorldState>.Reject(
                        "The target spatial place is unavailable.");
            }

            var mutation =
                context.Snapshot.State.Components.Set(
                    context.Snapshot.State.Entities,
                    CreateEntityId(),
                    target);

            if (!mutation.WasApplied)
            {
                return global::AI.Sandbox.Engine.Core.Commands
                    .CommandDecision<SpatialWorldState>.Reject(
                        mutation.Status.ToString());
            }

            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<SpatialWorldState>.Accept(
                    context.Snapshot.State with
                    {
                        Components = mutation.Registry,
                    });
        }
    }

    private sealed class NudgeSystem :
        global::AI.Sandbox.Engine.Core.Simulation
            .ISimulationSystem<SpatialWorldState>
    {
        public global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<SpatialWorldState> Execute(
                global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemContext<SpatialWorldState> context)
        {
            var current = GetPosition(context.State);
            var nextPoint =
                global::AI.Sandbox.Engine.Core.Spatial
                    .SpatialPoint.Create(
                        checked(
                            current.Point.XMillimeters + 1),
                        current.Point.YMillimeters,
                        current.Point.ZMillimeters);
            var mutation = context.State.Components.Set(
                context.State.Entities,
                CreateEntityId(),
                global::AI.Sandbox.Engine.Core.Spatial
                    .SpatialPosition.Create(
                        current.PlaceId,
                        nextPoint));

            if (!mutation.WasApplied)
            {
                return global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemDecision<SpatialWorldState>.Reject(
                        mutation.Status.ToString());
            }

            return global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemDecision<SpatialWorldState>.Update(
                    context.State with
                    {
                        Components = mutation.Registry,
                    });
        }
    }

    private sealed class SpatialWorldCodec :
        global::AI.Sandbox.Engine.Core.Persistence
            .IWorldStateSnapshotCodec<SpatialWorldState>
    {
        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaId SchemaId { get; } =
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaId.Parse("spatial.validation");

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
            .SnapshotPayload Encode(SpatialWorldState state)
        {
            var builder = new System.Text.StringBuilder();

            foreach (var place in state.Topology.Places)
            {
                _ = builder.Append("P|");
                _ = builder.Append(place.PlaceId);
                _ = builder.Append('|');
                _ = builder.Append(
                    place.ParentPlaceId is { } parent
                        ? parent.ToString()
                        : "-");
                _ = builder.Append('\n');
            }

            foreach (var connection in state.Topology.Connections)
            {
                _ = builder.Append("C|");
                _ = builder.Append(connection.FromPlaceId);
                _ = builder.Append('|');
                _ = builder.Append(connection.ToPlaceId);
                _ = builder.Append('|');
                _ = builder.Append(
                    connection.Distance.Millimeters.ToString(
                        System.Globalization.CultureInfo.InvariantCulture));
                _ = builder.Append('\n');
            }

            var position = GetPosition(state);
            _ = builder.Append("E|");
            _ = builder.Append(CreateEntityId());
            _ = builder.Append('|');
            _ = builder.Append(position.PlaceId);
            _ = builder.Append('|');
            _ = builder.Append(
                position.Point.XMillimeters.ToString(
                    System.Globalization.CultureInfo.InvariantCulture));
            _ = builder.Append('|');
            _ = builder.Append(
                position.Point.YMillimeters.ToString(
                    System.Globalization.CultureInfo.InvariantCulture));
            _ = builder.Append('|');
            _ = builder.Append(
                position.Point.ZMillimeters.ToString(
                    System.Globalization.CultureInfo.InvariantCulture));
            _ = builder.Append('\n');

            return global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotPayload.From(
                    System.Text.Encoding.UTF8.GetBytes(
                        builder.ToString()));
        }

        public global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<SpatialWorldState> Decode(
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion version,
                global::AI.Sandbox.Engine.Core.Persistence
                    .SnapshotPayload payload)
        {
            if (version != CurrentSchemaVersion)
            {
                return Reject("Unsupported schema version.");
            }

            var placeRecords = new List<PlaceRecord>();
            var connectionRecords = new List<ConnectionRecord>();
            EntityRecord? entityRecord = null;
            var lines = System.Text.Encoding.UTF8.GetString(
                payload.ToArray()).Split(
                    '\n',
                    StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var parts = line.Split('|');

                if (parts.Length == 3 &&
                    string.Equals(
                        parts[0],
                        "P",
                        StringComparison.Ordinal) &&
                    TryParsePlaceId(
                        parts[1],
                        out var placeId))
                {
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.Spatial
                            .SpatialPlaceIdKind>? parentPlaceId = null;

                    if (!string.Equals(
                        parts[2],
                        "-",
                        StringComparison.Ordinal))
                    {
                        if (!TryParsePlaceId(
                            parts[2],
                            out var parsedParent))
                        {
                            return Reject("Malformed parent place ID.");
                        }

                        parentPlaceId = parsedParent;
                    }

                    placeRecords.Add(
                        new PlaceRecord(
                            placeId,
                            parentPlaceId));
                    continue;
                }

                if (parts.Length == 4 &&
                    string.Equals(
                        parts[0],
                        "C",
                        StringComparison.Ordinal) &&
                    TryParsePlaceId(
                        parts[1],
                        out var fromPlaceId) &&
                    TryParsePlaceId(
                        parts[2],
                        out var toPlaceId) &&
                    ulong.TryParse(
                        parts[3],
                        System.Globalization.NumberStyles.None,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var distance))
                {
                    connectionRecords.Add(
                        new ConnectionRecord(
                            fromPlaceId,
                            toPlaceId,
                            distance));
                    continue;
                }

                if (parts.Length == 6 &&
                    string.Equals(
                        parts[0],
                        "E",
                        StringComparison.Ordinal) &&
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.Entities
                            .EntityIdKind>.TryParse(
                                parts[1],
                                out var entityId) &&
                    TryParsePlaceId(
                        parts[2],
                        out var entityPlaceId) &&
                    long.TryParse(
                        parts[3],
                        System.Globalization.NumberStyles.Integer,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var x) &&
                    long.TryParse(
                        parts[4],
                        System.Globalization.NumberStyles.Integer,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var y) &&
                    long.TryParse(
                        parts[5],
                        System.Globalization.NumberStyles.Integer,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var z) &&
                    entityRecord is null)
                {
                    entityRecord = new EntityRecord(
                        entityId,
                        entityPlaceId,
                        x,
                        y,
                        z);
                    continue;
                }

                return Reject("Malformed spatial payload record.");
            }

            if (entityRecord is null)
            {
                return Reject("Spatial payload is missing entity position.");
            }

            try
            {
                var topologyBuilder =
                    new global::AI.Sandbox.Engine.Core.Spatial
                        .SpatialTopologyBuilder();

                foreach (var place in placeRecords)
                {
                    _ = topologyBuilder.AddPlace(
                        place.PlaceId,
                        place.ParentPlaceId);
                }

                foreach (var connection in connectionRecords)
                {
                    _ = topologyBuilder.AddDirectedConnection(
                        connection.FromPlaceId,
                        connection.ToPlaceId,
                        global::AI.Sandbox.Engine.Core.Spatial
                            .SpatialDistance.FromMillimeters(
                                connection.DistanceMillimeters));
                }

                var topology = topologyBuilder.Build();
                var position =
                    global::AI.Sandbox.Engine.Core.Spatial
                        .SpatialPosition.Create(
                            entityRecord.PlaceId,
                            global::AI.Sandbox.Engine.Core.Spatial
                                .SpatialPoint.Create(
                                    entityRecord.X,
                                    entityRecord.Y,
                                    entityRecord.Z));

                if (!topology.ContainsPlace(position.PlaceId))
                {
                    return Reject(
                        "Entity position references an unknown place.");
                }

                var entities =
                    global::AI.Sandbox.Engine.Core.Entities
                        .EntityRegistry.FromActiveEntities(
                            new[] { entityRecord.EntityId });
                var components =
                    new global::AI.Sandbox.Engine.Core.Components
                        .ComponentRegistryBuilder(entities)
                        .Add(
                            entityRecord.EntityId,
                            position)
                        .Build();

                return global::AI.Sandbox.Engine.Core.Persistence
                    .WorldStateDecodeDecision<SpatialWorldState>.Accept(
                        new SpatialWorldState(
                            topology,
                            entities,
                            components));
            }
            catch (Exception exception)
                when (exception is ArgumentException or
                    InvalidOperationException)
            {
                return Reject(exception.Message);
            }
        }

        private static bool TryParsePlaceId(
            string text,
            out global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Spatial
                    .SpatialPlaceIdKind> placeId)
        {
            return global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Spatial
                    .SpatialPlaceIdKind>.TryParse(
                        text,
                        out placeId);
        }

        private static global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<SpatialWorldState> Reject(
                string reason)
        {
            return global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateDecodeDecision<SpatialWorldState>.Reject(reason);
        }

        private sealed record PlaceRecord(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Spatial
                    .SpatialPlaceIdKind> PlaceId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Spatial
                    .SpatialPlaceIdKind>? ParentPlaceId);

        private sealed record ConnectionRecord(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Spatial
                    .SpatialPlaceIdKind> FromPlaceId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Spatial
                    .SpatialPlaceIdKind> ToPlaceId,
            ulong DistanceMillimeters);

        private sealed record EntityRecord(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities
                    .EntityIdKind> EntityId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Spatial
                    .SpatialPlaceIdKind> PlaceId,
            long X,
            long Y,
            long Z);
    }
}
