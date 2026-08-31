namespace AI.Sandbox.Engine.Core.Tests;

public sealed class PerceptionModelTests
{
    private sealed record CounterState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    private sealed record TextStimulus(string Text) :
        global::AI.Sandbox.Engine.Core.Perception.IPerceptionStimulus;

    private readonly record struct TextSignal(string Text) :
        global::AI.Sandbox.Engine.Core.Perception.IPerceptionSignal;

    private readonly record struct AlternateSignal(int Length) :
        global::AI.Sandbox.Engine.Core.Perception.IPerceptionSignal;

    private abstract record AbstractStimulus :
        global::AI.Sandbox.Engine.Core.Perception.IPerceptionStimulus;

    private record OpenSignal(string Value) :
        global::AI.Sandbox.Engine.Core.Perception.IPerceptionSignal;

    private sealed record SpatialPerceptionState(
        global::AI.Sandbox.Engine.Core.Spatial.SpatialTopology Topology,
        global::AI.Sandbox.Engine.Core.Entities.EntityRegistry Entities,
        global::AI.Sandbox.Engine.Core.Components.ComponentRegistry Components) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    private readonly record struct PresenceStimulus(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> TargetEntityId,
        global::AI.Sandbox.Engine.Core.Spatial.SpatialDistance MaximumDistance) :
        global::AI.Sandbox.Engine.Core.Perception.IPerceptionStimulus;

    private readonly record struct PresenceSignal(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> TargetEntityId,
        System.UInt128 SquaredDistanceMillimeters) :
        global::AI.Sandbox.Engine.Core.Perception.IPerceptionSignal;

    [Xunit.Fact]
    public void ConfidenceDistinguishesDefaultZeroAndCertain()
    {
        global::AI.Sandbox.Engine.Core.Perception
            .PerceptionConfidence empty = default;
        var zero = global::AI.Sandbox.Engine.Core.Perception
            .PerceptionConfidence.Zero;
        var partial = global::AI.Sandbox.Engine.Core.Perception
            .PerceptionConfidence.FromBasisPoints(7_500);

        Xunit.Assert.True(empty.IsEmpty);
        Xunit.Assert.False(zero.IsEmpty);
        Xunit.Assert.True(zero.IsZero);
        Xunit.Assert.Equal((ushort)7_500, partial.BasisPoints);
        Xunit.Assert.True(
            partial.CompareTo(
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionConfidence.Certain) < 0);
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.Perception
                .PerceptionConfidence.FromBasisPoints(10_001));
        Xunit.Assert.Throws<InvalidOperationException>(
            () => empty.CompareTo(partial));
    }

    [Xunit.Fact]
    public void ObservationDecisionRequiresConcreteSignalAndNonZeroConfidence()
    {
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.Perception
                .PerceptionDecision<TextSignal>.Observe(
                    new TextSignal("signal"),
                    global::AI.Sandbox.Engine.Core.Perception
                        .PerceptionConfidence.Zero));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Perception
                .PerceptionDecision<OpenSignal>.Observe(
                    new OpenSignal("signal"),
                    global::AI.Sandbox.Engine.Core.Perception
                        .PerceptionConfidence.Certain));

        var decision = global::AI.Sandbox.Engine.Core.Perception
            .PerceptionDecision<TextSignal>.Observe(
                new TextSignal("signal"),
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionConfidence.FromBasisPoints(8_000));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionDecisionStatus.Observed,
            decision.Status);
        Xunit.Assert.Equal(
            new TextSignal("signal"),
            decision.Signal);
        Xunit.Assert.Equal(
            (ushort)8_000,
            decision.Confidence!.Value.BasisPoints);
        Xunit.Assert.Null(decision.IgnoreReason);
    }

    [Xunit.Fact]
    public void EnvelopePreservesObserverChannelAndSnapshotMetadata()
    {
        var manager = CreateCounterManager();
        var snapshot = manager.Read();
        var payload = new TextStimulus("candidate");

        var envelope = CreateTextEnvelope(
            snapshot,
            payload,
            stimulusSuffix: 1);

        Xunit.Assert.Equal(
            CreateStimulusId(1),
            envelope.StimulusId);
        Xunit.Assert.Equal(
            CreateChannelId(),
            envelope.ChannelId);
        Xunit.Assert.Equal(
            CreateObserverId(),
            envelope.ObserverEntityId);
        Xunit.Assert.Equal(
            snapshot.WorldId,
            envelope.WorldId);
        Xunit.Assert.Equal(
            snapshot.Version,
            envelope.ExpectedWorldStateVersion);
        Xunit.Assert.Equal(
            snapshot.SimulationTick,
            envelope.ExpectedSimulationTick);
        Xunit.Assert.Same(payload, envelope.Payload);
    }

    [Xunit.Fact]
    public void EnvelopeRejectsEmptyIdsNullAndInvalidStimulusTypes()
    {
        var manager = CreateCounterManager();
        var snapshot = manager.Read();

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Perception
                .PerceptionStimulusEnvelope<TextStimulus>.Create(
                    default,
                    CreateChannelId(),
                    CreateObserverId(),
                    snapshot.WorldId,
                    snapshot.Version,
                    snapshot.SimulationTick,
                    new TextStimulus("candidate")));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.Perception
                .PerceptionStimulusEnvelope<TextStimulus>.Create(
                    CreateStimulusId(1),
                    CreateChannelId(),
                    CreateObserverId(),
                    snapshot.WorldId,
                    snapshot.Version,
                    snapshot.SimulationTick,
                    null!));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Perception
                .PerceptionStimulusEnvelope<AbstractStimulus>.Create(
                    CreateStimulusId(1),
                    CreateChannelId(),
                    CreateObserverId(),
                    snapshot.WorldId,
                    snapshot.Version,
                    snapshot.SimulationTick,
                    new ConcreteAbstractStimulus()));
    }

    [Xunit.Fact]
    public void BuilderRejectsDuplicatesInvalidPairsAndReuse()
    {
        var manager = CreateCounterManager();
        var builder =
            new global::AI.Sandbox.Engine.Core.Perception
                .PerceptionProcessorBuilder<CounterState>();

        Xunit.Assert.Throws<ArgumentException>(
            () => builder.Add<AbstractStimulus, TextSignal>(
                new AbstractEvaluator()));
        Xunit.Assert.Throws<ArgumentException>(
            () => builder.Add<TextStimulus, OpenSignal>(
                new OpenSignalEvaluator()));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => builder.Add<TextStimulus, TextSignal>(null!));

        _ = builder.Add<TextStimulus, TextSignal>(
            new TextEvaluator());
        Xunit.Assert.Throws<ArgumentException>(
            () => builder.Add<TextStimulus, TextSignal>(
                new TextEvaluator()));

        var processor = builder.Build(manager);

        Xunit.Assert.Equal(1, processor.EvaluatorCount);
        Xunit.Assert.Throws<InvalidOperationException>(
            () => builder.Add<TextStimulus, AlternateSignal>(
                new AlternateEvaluator()));
        Xunit.Assert.Throws<InvalidOperationException>(
            () => builder.Build(manager));
    }

    [Xunit.Fact]
    public void MissingWorldVersionAndTickAreRejectedBeforeEvaluator()
    {
        var evaluator = new CountingTextEvaluator();
        var manager = CreateCounterManager();
        var processor =
            new global::AI.Sandbox.Engine.Core.Perception
                .PerceptionProcessorBuilder<CounterState>()
                .Add<TextStimulus, TextSignal>(evaluator)
                .Build(manager);
        var snapshot = manager.Read();

        var missing = processor.Evaluate<
            TextStimulus,
            AlternateSignal>(
                CreateTextEnvelope(
                    snapshot,
                    new TextStimulus("missing"),
                    stimulusSuffix: 2));
        var wrongWorld = processor.Evaluate<
            TextStimulus,
            TextSignal>(
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionStimulusEnvelope<TextStimulus>.Create(
                        CreateStimulusId(3),
                        CreateChannelId(),
                        CreateObserverId(),
                        CreateOtherWorldId(),
                        snapshot.Version,
                        snapshot.SimulationTick,
                        new TextStimulus("world")));
        var staleVersion = manager.TryApply(
            snapshot.Version,
            snapshot.SimulationTick,
            new SetCounterTransition(5));
        Xunit.Assert.True(staleVersion.WasApplied);
        var versionResult = processor.Evaluate<
            TextStimulus,
            TextSignal>(
                CreateTextEnvelope(
                    snapshot,
                    new TextStimulus("version"),
                    stimulusSuffix: 4));
        var current = manager.Read();
        var tickEnvelope =
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionStimulusEnvelope<TextStimulus>.Create(
                    CreateStimulusId(5),
                    CreateChannelId(),
                    CreateObserverId(),
                    current.WorldId,
                    current.Version,
                    expectedSimulationTick:
                        checked(current.SimulationTick + 1),
                    new TextStimulus("tick"));
        var tickResult = processor.Evaluate<
            TextStimulus,
            TextSignal>(tickEnvelope);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionEvaluationStatus.EvaluatorNotRegistered,
            missing.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionEvaluationStatus.WorldMismatch,
            wrongWorld.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionEvaluationStatus.VersionConflict,
            versionResult.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionEvaluationStatus.SimulationTickMismatch,
            tickResult.Status);
        Xunit.Assert.False(missing.EvaluatorWasExecuted);
        Xunit.Assert.False(wrongWorld.EvaluatorWasExecuted);
        Xunit.Assert.False(versionResult.EvaluatorWasExecuted);
        Xunit.Assert.False(tickResult.EvaluatorWasExecuted);
        Xunit.Assert.Equal(0, evaluator.EvaluationCount);
    }

    [Xunit.Fact]
    public void ObservedSignalPreservesProvenanceWithoutChangingWorld()
    {
        var manager = CreateCounterManager();
        var processor =
            new global::AI.Sandbox.Engine.Core.Perception
                .PerceptionProcessorBuilder<CounterState>()
                .Add<TextStimulus, TextSignal>(
                    new TextEvaluator())
                .Build(manager);
        var before = manager.Read();
        var result = processor.Evaluate<
            TextStimulus,
            TextSignal>(
                CreateTextEnvelope(
                    before,
                    new TextStimulus("candidate"),
                    stimulusSuffix: 6));

        Xunit.Assert.True(result.WasObserved);
        Xunit.Assert.True(result.EvaluatorWasExecuted);
        Xunit.Assert.NotNull(result.Observation);
        var observation = result.Observation!;
        Xunit.Assert.Equal(
            CreateStimulusId(6),
            observation.StimulusId);
        Xunit.Assert.Equal(
            CreateChannelId(),
            observation.ChannelId);
        Xunit.Assert.Equal(
            CreateObserverId(),
            observation.ObserverEntityId);
        Xunit.Assert.Equal(before.WorldId, observation.WorldId);
        Xunit.Assert.Equal(
            before.Version,
            observation.WorldStateVersion);
        Xunit.Assert.Equal(
            before.SimulationTick,
            observation.SimulationTick);
        Xunit.Assert.Equal(
            (ushort)8_000,
            observation.Confidence.BasisPoints);
        Xunit.Assert.Equal(
            new TextSignal("candidate"),
            observation.Signal);
        Xunit.Assert.Same(before, result.Snapshot);
        Xunit.Assert.Same(before, manager.Read());
    }

    [Xunit.Fact]
    public void IgnoredAndExceptionResultsNeverMutateWorld()
    {
        var ignoredManager = CreateCounterManager();
        var ignoredProcessor =
            new global::AI.Sandbox.Engine.Core.Perception
                .PerceptionProcessorBuilder<CounterState>()
                .Add<TextStimulus, TextSignal>(
                    new IgnoringEvaluator())
                .Build(ignoredManager);
        var ignoredBefore = ignoredManager.Read();
        var ignored = ignoredProcessor.Evaluate<
            TextStimulus,
            TextSignal>(
                CreateTextEnvelope(
                    ignoredBefore,
                    new TextStimulus("ignore"),
                    stimulusSuffix: 7));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionEvaluationStatus.Ignored,
            ignored.Status);
        Xunit.Assert.True(ignored.EvaluatorWasExecuted);
        Xunit.Assert.Null(ignored.Observation);
        Xunit.Assert.Equal(
            "not available to observer",
            ignored.FailureReason);
        Xunit.Assert.Same(
            ignoredBefore,
            ignoredManager.Read());

        var throwingManager = CreateCounterManager();
        var throwingProcessor =
            new global::AI.Sandbox.Engine.Core.Perception
                .PerceptionProcessorBuilder<CounterState>()
                .Add<TextStimulus, TextSignal>(
                    new ThrowingEvaluator())
                .Build(throwingManager);
        var throwingBefore = throwingManager.Read();

        Xunit.Assert.Throws<InvalidOperationException>(
            () => throwingProcessor.Evaluate<
                TextStimulus,
                TextSignal>(
                    CreateTextEnvelope(
                        throwingBefore,
                        new TextStimulus("throw"),
                        stimulusSuffix: 8)));
        Xunit.Assert.Same(
            throwingBefore,
            throwingManager.Read());
    }

    [Xunit.Fact]
    public void ExactStimulusSignalPairsRemainIndependent()
    {
        var manager = CreateCounterManager();
        var processor =
            new global::AI.Sandbox.Engine.Core.Perception
                .PerceptionProcessorBuilder<CounterState>()
                .Add<TextStimulus, TextSignal>(
                    new TextEvaluator())
                .Add<TextStimulus, AlternateSignal>(
                    new AlternateEvaluator())
                .Build(manager);
        var snapshot = manager.Read();
        var envelope = CreateTextEnvelope(
            snapshot,
            new TextStimulus("four"),
            stimulusSuffix: 9);

        var text = processor.Evaluate<
            TextStimulus,
            TextSignal>(envelope);
        var alternate = processor.Evaluate<
            TextStimulus,
            AlternateSignal>(envelope);

        Xunit.Assert.Equal(
            new TextSignal("four"),
            text.Observation!.Signal);
        Xunit.Assert.Equal(
            new AlternateSignal(4),
            alternate.Observation!.Signal);
    }

    [Xunit.Fact]
    public async Task VersionConflictDuringEvaluationDoesNotRetry()
    {
        using var entered = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();
        var evaluator =
            new BlockingEvaluator(
                entered,
                release);
        var manager = CreateCounterManager();
        var processor =
            new global::AI.Sandbox.Engine.Core.Perception
                .PerceptionProcessorBuilder<CounterState>()
                .Add<TextStimulus, TextSignal>(evaluator)
                .Build(manager);
        var observed = manager.Read();
        var evaluationTask = Task.Run(
            () => processor.Evaluate<
                TextStimulus,
                TextSignal>(
                    CreateTextEnvelope(
                        observed,
                        new TextStimulus("blocked"),
                        stimulusSuffix: 10)));

        Xunit.Assert.True(
            entered.Wait(TimeSpan.FromSeconds(5)));

        var external = manager.TryApply(
            observed.Version,
            observed.SimulationTick,
            new SetCounterTransition(50));

        release.Set();
        var result = await evaluationTask;

        Xunit.Assert.True(external.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionEvaluationStatus.VersionConflict,
            result.Status);
        Xunit.Assert.True(result.EvaluatorWasExecuted);
        Xunit.Assert.Null(result.Observation);
        Xunit.Assert.Equal(1, evaluator.EvaluationCount);
        Xunit.Assert.Equal(50, manager.Read().State.Value);
    }

    [Xunit.Fact]
    public void SpatialEvaluatorProducesSubjectiveSignalNotWorldMutation()
    {
        var manager = CreateSpatialManager(
            targetInSamePlace: true);
        var processor =
            new global::AI.Sandbox.Engine.Core.Perception
                .PerceptionProcessorBuilder<SpatialPerceptionState>()
                .Add<PresenceStimulus, PresenceSignal>(
                    new PresenceEvaluator())
                .Build(manager);
        var before = manager.Read();
        var stimulus = new PresenceStimulus(
            CreateTargetId(),
            global::AI.Sandbox.Engine.Core.Spatial
                .SpatialDistance.FromMeters(5));
        var envelope =
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionStimulusEnvelope<PresenceStimulus>.Create(
                    CreateStimulusId(11),
                    CreateChannelId(),
                    CreateObserverId(),
                    before.WorldId,
                    before.Version,
                    before.SimulationTick,
                    stimulus);

        var result = processor.Evaluate<
            PresenceStimulus,
            PresenceSignal>(envelope);

        Xunit.Assert.True(result.WasObserved);
        Xunit.Assert.Equal(
            CreateTargetId(),
            result.Observation!.Signal.TargetEntityId);
        Xunit.Assert.Equal(
            (System.UInt128)25,
            result.Observation.Signal
                .SquaredDistanceMillimeters);
        Xunit.Assert.Equal(
            (ushort)9_000,
            result.Observation.Confidence.BasisPoints);
        Xunit.Assert.Same(before, manager.Read());
    }

    [Xunit.Fact]
    public void SpatialEvaluatorCanIgnoreDifferentPlaceWithoutInventingPathing()
    {
        var manager = CreateSpatialManager(
            targetInSamePlace: false);
        var processor =
            new global::AI.Sandbox.Engine.Core.Perception
                .PerceptionProcessorBuilder<SpatialPerceptionState>()
                .Add<PresenceStimulus, PresenceSignal>(
                    new PresenceEvaluator())
                .Build(manager);
        var snapshot = manager.Read();
        var envelope =
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionStimulusEnvelope<PresenceStimulus>.Create(
                    CreateStimulusId(12),
                    CreateChannelId(),
                    CreateObserverId(),
                    snapshot.WorldId,
                    snapshot.Version,
                    snapshot.SimulationTick,
                    new PresenceStimulus(
                        CreateTargetId(),
                        global::AI.Sandbox.Engine.Core.Spatial
                            .SpatialDistance.FromMeters(100)));

        var result = processor.Evaluate<
            PresenceStimulus,
            PresenceSignal>(envelope);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionEvaluationStatus.Ignored,
            result.Status);
        Xunit.Assert.Null(result.Observation);
        Xunit.Assert.Equal(
            "Observer and target do not share a local place.",
            result.FailureReason);
    }

    private static global::AI.Sandbox.Engine.Core.Perception
        .PerceptionStimulusEnvelope<TextStimulus> CreateTextEnvelope(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateSnapshot<CounterState> snapshot,
            TextStimulus payload,
            int stimulusSuffix)
    {
        return global::AI.Sandbox.Engine.Core.Perception
            .PerceptionStimulusEnvelope<TextStimulus>.Create(
                CreateStimulusId(stimulusSuffix),
                CreateChannelId(),
                CreateObserverId(),
                snapshot.WorldId,
                snapshot.Version,
                snapshot.SimulationTick,
                payload);
    }

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<CounterState> CreateCounterManager()
    {
        return global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<CounterState>.Create(
                CreateWorldId(),
                new CounterState(0));
    }

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<SpatialPerceptionState> CreateSpatialManager(
            bool targetInSamePlace)
    {
        var observerId = CreateObserverId();
        var targetId = CreateTargetId();
        var entities =
            global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
                .FromActiveEntities(
                    new[]
                    {
                        observerId,
                        targetId,
                    });
        var targetPlaceId = targetInSamePlace
            ? CreatePlaceId(1)
            : CreatePlaceId(2);
        var components =
            new global::AI.Sandbox.Engine.Core.Components
                .ComponentRegistryBuilder(entities)
                .Add(
                    observerId,
                    global::AI.Sandbox.Engine.Core.Spatial
                        .SpatialPosition.Create(
                            CreatePlaceId(1),
                            global::AI.Sandbox.Engine.Core.Spatial
                                .SpatialPoint.Origin))
                .Add(
                    targetId,
                    global::AI.Sandbox.Engine.Core.Spatial
                        .SpatialPosition.Create(
                            targetPlaceId,
                            global::AI.Sandbox.Engine.Core.Spatial
                                .SpatialPoint.Create(0, 3, 4)))
                .Build();
        var topology =
            new global::AI.Sandbox.Engine.Core.Spatial
                .SpatialTopologyBuilder()
                .AddPlace(CreatePlaceId(1))
                .AddPlace(CreatePlaceId(2))
                .Build();

        return global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<SpatialPerceptionState>.Create(
                CreateWorldId(),
                new SpatialPerceptionState(
                    topology,
                    entities,
                    components));
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> CreateWorldId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000001300");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>
        CreateOtherWorldId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000001301");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
        CreateObserverId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(
                "019b0000-0000-7000-9900-000000000001");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> CreateTargetId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(
                "019b0000-0000-7000-9900-000000000002");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Perception.PerceptionStimulusIdKind>
        CreateStimulusId(int suffix)
    {
        var text = $"019b0000-0000-7000-9a00-{suffix:D12}";
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionStimulusIdKind>.Parse(text);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Perception.PerceptionChannelIdKind>
        CreateChannelId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionChannelIdKind>.Parse(
                    "019b0000-0000-7000-9b00-000000000001");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Spatial.SpatialPlaceIdKind>
        CreatePlaceId(int suffix)
    {
        var text = $"019b0000-0000-7000-9c00-{suffix:D12}";
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Spatial
                .SpatialPlaceIdKind>.Parse(text);
    }

    private sealed class TextEvaluator :
        global::AI.Sandbox.Engine.Core.Perception
            .IPerceptionEvaluator<
                CounterState,
                TextStimulus,
                TextSignal>
    {
        public global::AI.Sandbox.Engine.Core.Perception
            .PerceptionDecision<TextSignal> Evaluate(
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionContext<
                        CounterState,
                        TextStimulus> context)
        {
            return global::AI.Sandbox.Engine.Core.Perception
                .PerceptionDecision<TextSignal>.Observe(
                    new TextSignal(
                        context.Envelope.Payload.Text),
                    global::AI.Sandbox.Engine.Core.Perception
                        .PerceptionConfidence.FromBasisPoints(8_000));
        }
    }

    private sealed class CountingTextEvaluator :
        global::AI.Sandbox.Engine.Core.Perception
            .IPerceptionEvaluator<
                CounterState,
                TextStimulus,
                TextSignal>
    {
        public int EvaluationCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Perception
            .PerceptionDecision<TextSignal> Evaluate(
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionContext<
                        CounterState,
                        TextStimulus> context)
        {
            EvaluationCount++;

            return global::AI.Sandbox.Engine.Core.Perception
                .PerceptionDecision<TextSignal>.Observe(
                    new TextSignal(
                        context.Envelope.Payload.Text),
                    global::AI.Sandbox.Engine.Core.Perception
                        .PerceptionConfidence.Certain);
        }
    }

    private sealed class AlternateEvaluator :
        global::AI.Sandbox.Engine.Core.Perception
            .IPerceptionEvaluator<
                CounterState,
                TextStimulus,
                AlternateSignal>
    {
        public global::AI.Sandbox.Engine.Core.Perception
            .PerceptionDecision<AlternateSignal> Evaluate(
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionContext<
                        CounterState,
                        TextStimulus> context)
        {
            return global::AI.Sandbox.Engine.Core.Perception
                .PerceptionDecision<AlternateSignal>.Observe(
                    new AlternateSignal(
                        context.Envelope.Payload.Text.Length),
                    global::AI.Sandbox.Engine.Core.Perception
                        .PerceptionConfidence.Certain);
        }
    }

    private sealed class IgnoringEvaluator :
        global::AI.Sandbox.Engine.Core.Perception
            .IPerceptionEvaluator<
                CounterState,
                TextStimulus,
                TextSignal>
    {
        public global::AI.Sandbox.Engine.Core.Perception
            .PerceptionDecision<TextSignal> Evaluate(
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionContext<
                        CounterState,
                        TextStimulus> context)
        {
            _ = context;

            return global::AI.Sandbox.Engine.Core.Perception
                .PerceptionDecision<TextSignal>.Ignore(
                    "not available to observer");
        }
    }

    private sealed class ThrowingEvaluator :
        global::AI.Sandbox.Engine.Core.Perception
            .IPerceptionEvaluator<
                CounterState,
                TextStimulus,
                TextSignal>
    {
        public global::AI.Sandbox.Engine.Core.Perception
            .PerceptionDecision<TextSignal> Evaluate(
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionContext<
                        CounterState,
                        TextStimulus> context)
        {
            _ = context;
            throw new InvalidOperationException("evaluator failure");
        }
    }

    private sealed class BlockingEvaluator :
        global::AI.Sandbox.Engine.Core.Perception
            .IPerceptionEvaluator<
                CounterState,
                TextStimulus,
                TextSignal>
    {
        private readonly ManualResetEventSlim entered;
        private readonly ManualResetEventSlim release;

        public BlockingEvaluator(
            ManualResetEventSlim entered,
            ManualResetEventSlim release)
        {
            this.entered = entered;
            this.release = release;
        }

        public int EvaluationCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Perception
            .PerceptionDecision<TextSignal> Evaluate(
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionContext<
                        CounterState,
                        TextStimulus> context)
        {
            EvaluationCount++;
            entered.Set();

            if (!release.Wait(TimeSpan.FromSeconds(5)))
            {
                throw new TimeoutException(
                    "The perception conflict test did not release.");
            }

            return global::AI.Sandbox.Engine.Core.Perception
                .PerceptionDecision<TextSignal>.Observe(
                    new TextSignal(
                        context.Envelope.Payload.Text),
                    global::AI.Sandbox.Engine.Core.Perception
                        .PerceptionConfidence.Certain);
        }
    }

    private sealed class PresenceEvaluator :
        global::AI.Sandbox.Engine.Core.Perception
            .IPerceptionEvaluator<
                SpatialPerceptionState,
                PresenceStimulus,
                PresenceSignal>
    {
        public global::AI.Sandbox.Engine.Core.Perception
            .PerceptionDecision<PresenceSignal> Evaluate(
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionContext<
                        SpatialPerceptionState,
                        PresenceStimulus> context)
        {
            var observerId =
                context.Envelope.ObserverEntityId;
            var targetId =
                context.Envelope.Payload.TargetEntityId;

            if (context.Snapshot.State.Entities.GetLifecycleStatus(
                    observerId) !=
                global::AI.Sandbox.Engine.Core.Entities
                    .EntityLifecycleStatus.Active ||
                context.Snapshot.State.Entities.GetLifecycleStatus(
                    targetId) !=
                global::AI.Sandbox.Engine.Core.Entities
                    .EntityLifecycleStatus.Active ||
                !context.Snapshot.State.Components.TryGet<
                    global::AI.Sandbox.Engine.Core.Spatial
                        .SpatialPosition>(
                            observerId,
                            out var observerPosition) ||
                !context.Snapshot.State.Components.TryGet<
                    global::AI.Sandbox.Engine.Core.Spatial
                        .SpatialPosition>(
                            targetId,
                            out var targetPosition))
            {
                return global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionDecision<PresenceSignal>.Ignore(
                        "Observer or target position is unavailable.");
            }

            if (!observerPosition.TryGetSquaredDistanceTo(
                targetPosition,
                out var squaredDistance))
            {
                return global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionDecision<PresenceSignal>.Ignore(
                        "Observer and target do not share a local place.");
            }

            var radius =
                context.Envelope.Payload.MaximumDistance;
            var radiusSquared =
                (System.UInt128)radius.Millimeters *
                radius.Millimeters;

            if (squaredDistance > radiusSquared)
            {
                return global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionDecision<PresenceSignal>.Ignore(
                        "Target is outside the evaluator radius.");
            }

            return global::AI.Sandbox.Engine.Core.Perception
                .PerceptionDecision<PresenceSignal>.Observe(
                    new PresenceSignal(
                        targetId,
                        squaredDistance),
                    global::AI.Sandbox.Engine.Core.Perception
                        .PerceptionConfidence.FromBasisPoints(9_000));
        }
    }

    private sealed record ConcreteAbstractStimulus :
        AbstractStimulus;

    private sealed class AbstractEvaluator :
        global::AI.Sandbox.Engine.Core.Perception
            .IPerceptionEvaluator<
                CounterState,
                AbstractStimulus,
                TextSignal>
    {
        public global::AI.Sandbox.Engine.Core.Perception
            .PerceptionDecision<TextSignal> Evaluate(
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionContext<
                        CounterState,
                        AbstractStimulus> context)
        {
            _ = context;

            return global::AI.Sandbox.Engine.Core.Perception
                .PerceptionDecision<TextSignal>.Ignore(
                    "invalid type");
        }
    }

    private sealed class OpenSignalEvaluator :
        global::AI.Sandbox.Engine.Core.Perception
            .IPerceptionEvaluator<
                CounterState,
                TextStimulus,
                OpenSignal>
    {
        public global::AI.Sandbox.Engine.Core.Perception
            .PerceptionDecision<OpenSignal> Evaluate(
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionContext<
                        CounterState,
                        TextStimulus> context)
        {
            _ = context;

            return global::AI.Sandbox.Engine.Core.Perception
                .PerceptionDecision<OpenSignal>.Ignore(
                    "invalid type");
        }
    }

    private sealed class SetCounterTransition :
        global::AI.Sandbox.Engine.Core.WorldState
            .IWorldStateTransition<CounterState>
    {
        private readonly int value;

        public SetCounterTransition(int value)
        {
            this.value = value;
        }

        public global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateTransitionDecision<CounterState> Evaluate(
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateSnapshot<CounterState> current)
        {
            return global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateTransitionDecision<CounterState>.Accept(
                    current.State with
                    {
                        Value = value,
                    });
        }
    }
}
