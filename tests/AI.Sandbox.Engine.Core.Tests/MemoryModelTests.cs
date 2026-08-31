namespace AI.Sandbox.Engine.Core.Tests;

public sealed class MemoryModelTests
{
    private sealed record MemoryWorldState(
        global::AI.Sandbox.Engine.Core.Entities.EntityRegistry Entities,
        global::AI.Sandbox.Engine.Core.Components.ComponentRegistry Components) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    private readonly record struct LocatedClaim(int Sequence) :
        global::AI.Sandbox.Engine.Core.Knowledge.IKnowledgeClaim;

    private readonly record struct Episode(int Sequence) :
        global::AI.Sandbox.Engine.Core.Memory.IMemoryContent;

    private abstract record AbstractEpisode :
        global::AI.Sandbox.Engine.Core.Memory.IMemoryContent;

    private record OpenEpisode(string Text) :
        global::AI.Sandbox.Engine.Core.Memory.IMemoryContent;

    private sealed record TextStimulus(string Text) :
        global::AI.Sandbox.Engine.Core.Perception.IPerceptionStimulus;

    private readonly record struct TextSignal(string Text) :
        global::AI.Sandbox.Engine.Core.Perception.IPerceptionSignal;

    private readonly record struct StoreMemory(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Memory.MemoryIdKind> MemoryId,
        uint ExpectedRevision,
        Episode Content,
        global::AI.Sandbox.Engine.Core.Memory.MemoryOriginReference Origin,
        global::AI.Sandbox.Engine.Core.Memory.MemoryStrength Strength,
        global::AI.Sandbox.Engine.Core.Memory.MemorySalience Salience,
        ushort StrengthIncrease,
        ushort SalienceIncrease) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    [Xunit.Fact]
    public void StrengthAndSalienceUseInitializedFixedPointValues()
    {
        global::AI.Sandbox.Engine.Core.Memory.MemoryStrength empty = default;
        var strength = global::AI.Sandbox.Engine.Core.Memory
            .MemoryStrength.FromBasisPoints(6_000);
        var salience = global::AI.Sandbox.Engine.Core.Memory
            .MemorySalience.FromBasisPoints(2_500);

        Xunit.Assert.True(empty.IsEmpty);
        Xunit.Assert.Equal(
            (ushort)7_500,
            strength.Increase(1_500).BasisPoints);
        Xunit.Assert.Equal(
            (ushort)0,
            strength.Decrease(7_000).BasisPoints);
        Xunit.Assert.Equal(
            (ushort)3_500,
            salience.Increase(1_000).BasisPoints);
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.Memory
                .MemoryStrength.FromBasisPoints(10_001));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.Memory
                .MemorySalience.FromBasisPoints(10_001));
    }

    [Xunit.Fact]
    public void OriginsPreservePerceptionAndKnowledgeProvenance()
    {
        var manager = CreateManager();
        var perception =
            new global::AI.Sandbox.Engine.Core.Perception
                .PerceptionProcessorBuilder<MemoryWorldState>()
                .Add<TextStimulus, TextSignal>(new TextEvaluator())
                .Build(manager);
        var snapshot = manager.Read();
        var observation = perception.Evaluate<TextStimulus, TextSignal>(
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionStimulusEnvelope<TextStimulus>.Create(
                    StimulusId(1),
                    ChannelId(),
                    OwnerId(),
                    snapshot.WorldId,
                    snapshot.Version,
                    snapshot.SimulationTick,
                    new TextStimulus("seen"))).Observation!;
        var perceptionOrigin =
            global::AI.Sandbox.Engine.Core.Memory
                .MemoryOriginReference.FromPerception(
                    OriginId(1),
                    observation);
        var knowledgeEntry = GetKnowledge(snapshot.State).Entries.Single();
        var knowledgeOrigin =
            global::AI.Sandbox.Engine.Core.Memory
                .MemoryOriginReference.FromKnowledge(
                    OriginId(2),
                    knowledgeEntry);

        Xunit.Assert.True(
            perceptionOrigin.HasPerceptionProvenance);
        Xunit.Assert.Equal(
            observation.StimulusId,
            perceptionOrigin.PerceptionStimulusId);
        Xunit.Assert.True(
            knowledgeOrigin.HasKnowledgeProvenance);
        Xunit.Assert.Equal(
            knowledgeEntry.ClaimId,
            knowledgeOrigin.KnowledgeClaimId);
        Xunit.Assert.Equal(
            knowledgeEntry.Evidence.EvidenceId,
            knowledgeOrigin.KnowledgeEvidenceId);
    }

    [Xunit.Fact]
    public void StoreRejectsInvalidTypesAndOriginScope()
    {
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Memory
                .MemoryStore<AbstractEpisode>.Create(
                    WorldId(),
                    OwnerId()));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Memory
                .MemoryStore<OpenEpisode>.Create(
                    WorldId(),
                    OwnerId()));

        var store = EmptyStore();
        var wrongOwner = store.Encode(
            MemoryId(1),
            new Episode(1),
            global::AI.Sandbox.Engine.Core.Memory
                .MemoryOriginReference.Create(
                    OriginId(3),
                    global::AI.Sandbox.Engine.Core.Memory
                        .MemoryOriginKind.External,
                    SourceId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0),
            Strength(5_000),
            Salience(1_000));
        var wrongWorld = store.Encode(
            MemoryId(2),
            new Episode(2),
            global::AI.Sandbox.Engine.Core.Memory
                .MemoryOriginReference.Create(
                    OriginId(4),
                    global::AI.Sandbox.Engine.Core.Memory
                        .MemoryOriginKind.External,
                    OwnerId(),
                    OtherWorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0),
            Strength(5_000),
            Salience(1_000));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Memory
                .MemoryMutationStatus.OriginOwnerMismatch,
            wrongOwner.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Memory
                .MemoryMutationStatus.OriginWorldMismatch,
            wrongWorld.Status);
    }

    [Xunit.Fact]
    public void EncodingOrderRevisionAndForgettingAreExplicit()
    {
        var store = EmptyStore();
        var first = store.Encode(
            MemoryId(2),
            new Episode(2),
            ExternalOrigin(10, 0, 0),
            Strength(4_000),
            Salience(1_000));
        var second = first.MemoryStore.Encode(
            MemoryId(1),
            new Episode(1),
            ExternalOrigin(11, 0, 0),
            Strength(7_000),
            Salience(2_000));

        Xunit.Assert.Equal(
            new[] { MemoryId(1), MemoryId(2) },
            second.MemoryStore.Entries
                .Select(entry => entry.MemoryId)
                .ToArray());

        var conflict = second.MemoryStore.Reinforce(
            MemoryId(2),
            2,
            100,
            100,
            Version(1),
            0);
        var reinforced = second.MemoryStore.Reinforce(
            MemoryId(2),
            1,
            500,
            200,
            Version(1),
            0);
        var regression = reinforced.MemoryStore.Weaken(
            MemoryId(2),
            2,
            100,
            100,
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateVersion.Initial,
            0);
        var forgotten = reinforced.MemoryStore.Weaken(
            MemoryId(2),
            2,
            10_000,
            0,
            Version(2),
            0);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Memory
                .MemoryMutationStatus.RevisionConflict,
            conflict.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Memory
                .MemoryMutationStatus.Reinforced,
            reinforced.Status);
        Xunit.Assert.Equal((uint)2, reinforced.Entry!.Revision);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Memory
                .MemoryMutationStatus.TemporalRegression,
            regression.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Memory
                .MemoryMutationStatus.Forgotten,
            forgotten.Status);
        Xunit.Assert.False(
            forgotten.MemoryStore.TryGet(
                MemoryId(2),
                out _));
    }

    [Xunit.Fact]
    public void RecallRankingIsDeterministicAndReadOnly()
    {
        var store = EmptyStore()
            .Encode(
                MemoryId(3),
                new Episode(3),
                ExternalOrigin(20, 0, 0),
                Strength(5_000),
                Salience(1_000))
            .MemoryStore
            .Encode(
                MemoryId(1),
                new Episode(1),
                ExternalOrigin(21, 0, 0),
                Strength(4_000),
                Salience(4_000))
            .MemoryStore
            .Encode(
                MemoryId(2),
                new Episode(2),
                ExternalOrigin(22, 0, 0),
                Strength(7_000),
                Salience(500))
            .MemoryStore;
        var query =
            global::AI.Sandbox.Engine.Core.Memory
                .MemoryRecallQuery.Create(
                    2,
                    Strength(4_000),
                    Salience(0));
        var recalled = store.Recall(query);

        Xunit.Assert.Equal(
            new[] { MemoryId(1), MemoryId(2) },
            recalled.Entries
                .Select(entry => entry.MemoryId)
                .ToArray());
        Xunit.Assert.Equal(3, store.Count);
        Xunit.Assert.Equal((uint)1, store.Entries[0].Revision);
    }

    [Xunit.Fact]
    public void KnowledgeDoesNotBecomeMemoryUntilExplicitCommand()
    {
        var manager = CreateManager();
        var runtime = CreateRuntime(manager);
        var before = manager.Read();
        var knowledgeEntry = GetKnowledge(before.State).Entries.Single();

        Xunit.Assert.True(GetMemory(before.State).IsEmpty);

        var result = runtime.ExecuteCommand(
            global::AI.Sandbox.Engine.Core.Commands
                .CommandEnvelope<StoreMemory>.Create(
                    CommandId(1),
                    before.WorldId,
                    before.Version,
                    before.SimulationTick,
                    new StoreMemory(
                        MemoryId(1),
                        0,
                        new Episode(knowledgeEntry.Claim.Sequence),
                        global::AI.Sandbox.Engine.Core.Memory
                            .MemoryOriginReference.FromKnowledge(
                                OriginId(30),
                                knowledgeEntry),
                        Strength(6_000),
                        Salience(3_000),
                        0,
                        0)));

        Xunit.Assert.True(result.WasCommitted);
        Xunit.Assert.Equal(1, GetMemory(manager.Read().State).Count);
        Xunit.Assert.Equal(1, GetKnowledge(manager.Read().State).Count);
    }

    [Xunit.Fact]
    public void SaveRestoreContinuationWithMemoryCommandsIsByteIdentical()
    {
        var uninterrupted = RunScenario(30, null);
        var resumed = RunScenario(30, 11);

        Xunit.Assert.Equal(
            uninterrupted.WorldStateVersion,
            resumed.WorldStateVersion);
        Xunit.Assert.Equal(
            uninterrupted.Checksum,
            resumed.Checksum);
        Xunit.Assert.True(
            uninterrupted.Payload.ContentEquals(resumed.Payload));
    }

    private static global::AI.Sandbox.Engine.Core.Persistence
        .WorldSnapshotDocument RunScenario(int cycles, int? checkpoint)
    {
        var persistence =
            new global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateSnapshotPersistence<MemoryWorldState>(
                    new MemoryCodec());
        var manager = CreateManager();
        var runtime = CreateRuntime(manager);

        if (checkpoint is null)
        {
            RunCycles(runtime, 0, cycles);
        }
        else
        {
            RunCycles(runtime, 0, checkpoint.Value);
            var restored = persistence.Restore(
                persistence.Capture(runtime.Read()));

            Xunit.Assert.True(restored.WasRestored);
            manager = global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<MemoryWorldState>.Restore(
                    restored.Snapshot!);
            runtime = CreateRuntime(manager);
            RunCycles(
                runtime,
                checkpoint.Value,
                cycles - checkpoint.Value);
        }

        return persistence.Capture(runtime.Read());
    }

    private static void RunCycles(
        global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestrator<MemoryWorldState> runtime,
        int start,
        int count)
    {
        for (var offset = 0; offset < count; offset++)
        {
            var cycle = start + offset;
            var snapshot = runtime.Read();
            var store = GetMemory(snapshot.State);
            var exists = store.TryGet(
                MemoryId(1),
                out var current);
            var knowledgeEntry =
                GetKnowledge(snapshot.State).Entries.Single();
            var command = exists
                ? new StoreMemory(
                    MemoryId(1),
                    current!.Revision,
                    current.Content,
                    current.Origin,
                    current.Strength,
                    current.Salience,
                    10,
                    5)
                : new StoreMemory(
                    MemoryId(1),
                    0,
                    new Episode(knowledgeEntry.Claim.Sequence),
                    global::AI.Sandbox.Engine.Core.Memory
                        .MemoryOriginReference.FromKnowledge(
                            OriginId(40),
                            knowledgeEntry),
                    Strength(5_000),
                    Salience(2_000),
                    0,
                    0);
            var result = runtime.ExecuteCommand(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandEnvelope<StoreMemory>.Create(
                        CommandId(cycle + 10),
                        snapshot.WorldId,
                        snapshot.Version,
                        snapshot.SimulationTick,
                        command));

            Xunit.Assert.True(
                result.WasCommitted,
                $"Cycle {cycle}: {result.CommandResult?.Status.ToString() ?? "none"}");
        }
    }

    private static global::AI.Sandbox.Engine.Core.Runtime
        .RuntimeOrchestrator<MemoryWorldState> CreateRuntime(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<MemoryWorldState> manager)
    {
        return new global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestratorBuilder<MemoryWorldState>()
            .AddCommandHandler(new StoreMemoryHandler())
            .Build(manager);
    }

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<MemoryWorldState> CreateManager()
    {
        var entities =
            global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
                .FromActiveEntities(
                    new[] { OwnerId(), SourceId() });
        var knowledgeEvidence =
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeEvidenceReference.Create(
                    KnowledgeEvidenceId(),
                    global::AI.Sandbox.Engine.Core.Knowledge
                        .KnowledgeEvidenceKind.Inference,
                    OwnerId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0);
        var knowledge =
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeSet<LocatedClaim>.Create(
                    WorldId(),
                    OwnerId())
                .Add(
                    KnowledgeClaimId(),
                    new LocatedClaim(7),
                    global::AI.Sandbox.Engine.Core.Knowledge
                        .KnowledgeConfidence.FromBasisPoints(8_000),
                    knowledgeEvidence)
                .KnowledgeSet;
        var memory =
            global::AI.Sandbox.Engine.Core.Memory
                .MemoryStore<Episode>.Create(
                    WorldId(),
                    OwnerId());
        var components =
            new global::AI.Sandbox.Engine.Core.Components
                .ComponentRegistryBuilder(entities)
                .Add(OwnerId(), knowledge)
                .Add(OwnerId(), memory)
                .Build();

        return global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<MemoryWorldState>.Create(
                WorldId(),
                new MemoryWorldState(entities, components));
    }

    private static global::AI.Sandbox.Engine.Core.Knowledge
        .KnowledgeSet<LocatedClaim> GetKnowledge(MemoryWorldState state)
    {
        Xunit.Assert.True(
            state.Components.TryGet<
                global::AI.Sandbox.Engine.Core.Knowledge
                    .KnowledgeSet<LocatedClaim>>(
                        OwnerId(),
                        out var set));
        return set!;
    }

    private static global::AI.Sandbox.Engine.Core.Memory
        .MemoryStore<Episode> GetMemory(MemoryWorldState state)
    {
        Xunit.Assert.True(
            state.Components.TryGet<
                global::AI.Sandbox.Engine.Core.Memory
                    .MemoryStore<Episode>>(
                        OwnerId(),
                        out var store));
        return store!;
    }

    private static global::AI.Sandbox.Engine.Core.Memory
        .MemoryStore<Episode> EmptyStore() =>
        global::AI.Sandbox.Engine.Core.Memory
            .MemoryStore<Episode>.Create(
                WorldId(),
                OwnerId());

    private static global::AI.Sandbox.Engine.Core.Memory
        .MemoryOriginReference ExternalOrigin(
            int suffix,
            ulong version,
            ulong tick) =>
        global::AI.Sandbox.Engine.Core.Memory
            .MemoryOriginReference.Create(
                OriginId(suffix),
                global::AI.Sandbox.Engine.Core.Memory
                    .MemoryOriginKind.External,
                OwnerId(),
                WorldId(),
                version == 0
                    ? global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial
                    : global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.From(version),
                tick);

    private static global::AI.Sandbox.Engine.Core.Memory
        .MemoryStrength Strength(ushort value) =>
        global::AI.Sandbox.Engine.Core.Memory
            .MemoryStrength.FromBasisPoints(value);

    private static global::AI.Sandbox.Engine.Core.Memory
        .MemorySalience Salience(ushort value) =>
        global::AI.Sandbox.Engine.Core.Memory
            .MemorySalience.FromBasisPoints(value);

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateVersion Version(ulong value) =>
        global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateVersion.From(value);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000001500");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> OtherWorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000001501");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OwnerId() =>
        EntityId("019b0000-0000-7000-aa00-000000000001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> SourceId() =>
        EntityId("019b0000-0000-7000-aa00-000000000002");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> EntityId(
            string text) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(text);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Memory.MemoryIdKind> MemoryId(
            int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Memory.MemoryIdKind>.Parse(
                $"019b0000-0000-7000-ab00-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Memory.MemoryOriginIdKind> OriginId(
            int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Memory.MemoryOriginIdKind>.Parse(
                $"019b0000-0000-7000-ac00-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Knowledge.KnowledgeClaimIdKind>
        KnowledgeClaimId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeClaimIdKind>.Parse(
                    "019b0000-0000-7000-ad00-000000000001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Knowledge.KnowledgeEvidenceIdKind>
        KnowledgeEvidenceId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeEvidenceIdKind>.Parse(
                    "019b0000-0000-7000-ae00-000000000001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Perception.PerceptionStimulusIdKind>
        StimulusId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionStimulusIdKind>.Parse(
                    $"019b0000-0000-7000-af00-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Perception.PerceptionChannelIdKind>
        ChannelId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionChannelIdKind>.Parse(
                    "019b0000-0000-7000-b000-000000000001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Commands.CommandIdKind> CommandId(
            int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Commands.CommandIdKind>.Parse(
                $"019b0000-0000-7000-b100-{suffix:D12}");

    private sealed class TextEvaluator :
        global::AI.Sandbox.Engine.Core.Perception.IPerceptionEvaluator<
            MemoryWorldState,
            TextStimulus,
            TextSignal>
    {
        public global::AI.Sandbox.Engine.Core.Perception
            .PerceptionDecision<TextSignal> Evaluate(
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionContext<
                        MemoryWorldState,
                        TextStimulus> context)
        {
            return global::AI.Sandbox.Engine.Core.Perception
                .PerceptionDecision<TextSignal>.Observe(
                    new TextSignal(context.Envelope.Payload.Text),
                    global::AI.Sandbox.Engine.Core.Perception
                        .PerceptionConfidence.Certain);
        }
    }

    private sealed class StoreMemoryHandler :
        global::AI.Sandbox.Engine.Core.Commands.ICommandHandler<
            MemoryWorldState,
            StoreMemory>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<MemoryWorldState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands.CommandContext<
                    MemoryWorldState,
                    StoreMemory> context)
        {
            var command = context.Envelope.Payload;
            var current = GetMemory(context.Snapshot.State);
            var mutation = command.ExpectedRevision == 0
                ? current.Encode(
                    command.MemoryId,
                    command.Content,
                    command.Origin,
                    command.Strength,
                    command.Salience)
                : current.Reinforce(
                    command.MemoryId,
                    command.ExpectedRevision,
                    command.StrengthIncrease,
                    command.SalienceIncrease,
                    context.Snapshot.Version,
                    context.Snapshot.SimulationTick);

            if (!mutation.WasApplied)
            {
                return global::AI.Sandbox.Engine.Core.Commands
                    .CommandDecision<MemoryWorldState>.Reject(
                        mutation.Status.ToString());
            }

            var componentResult =
                context.Snapshot.State.Components.Set(
                    context.Snapshot.State.Entities,
                    OwnerId(),
                    mutation.MemoryStore);

            if (!componentResult.WasApplied)
            {
                return global::AI.Sandbox.Engine.Core.Commands
                    .CommandDecision<MemoryWorldState>.Reject(
                        componentResult.Status.ToString());
            }

            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<MemoryWorldState>.Accept(
                    context.Snapshot.State with
                    {
                        Components = componentResult.Registry,
                    });
        }
    }

    private sealed class MemoryCodec :
        global::AI.Sandbox.Engine.Core.Persistence
            .IWorldStateSnapshotCodec<MemoryWorldState>
    {
        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaId SchemaId { get; } =
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaId.Parse("memory.validation");

        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaVersion CurrentSchemaVersion { get; } =
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion.From(1);

        public bool CanDecode(
            global::AI.Sandbox.Engine.Core.Persistence
                .PersistenceSchemaVersion version) =>
            version == CurrentSchemaVersion;

        public global::AI.Sandbox.Engine.Core.Persistence
            .SnapshotPayload Encode(MemoryWorldState state)
        {
            var entry = GetMemory(state).Entries.SingleOrDefault();
            var text = entry is null
                ? string.Empty
                : string.Join(
                    '|',
                    entry.MemoryId,
                    entry.Revision.ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
                    entry.Content.Sequence.ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
                    entry.Strength.BasisPoints.ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
                    entry.Salience.BasisPoints.ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
                    entry.Origin.OriginId,
                    ((int)entry.Origin.Kind).ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
                    entry.Origin.WorldStateVersion.Value.ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
                    entry.Origin.SimulationTick.ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
                    Optional(entry.Origin.SourceEntityId),
                    Optional(entry.Origin.KnowledgeClaimId),
                    Optional(entry.Origin.KnowledgeEvidenceId),
                    Optional(entry.Origin.PerceptionStimulusId),
                    Optional(entry.Origin.PerceptionChannelId),
                    entry.EncodedWorldStateVersion.Value.ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
                    entry.EncodedSimulationTick.ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
                    entry.LastUpdatedWorldStateVersion.Value.ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
                    entry.LastUpdatedSimulationTick.ToString(
                        System.Globalization.CultureInfo.InvariantCulture));

            return global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotPayload.From(
                    System.Text.Encoding.UTF8.GetBytes(text));
        }

        public global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<MemoryWorldState> Decode(
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion version,
                global::AI.Sandbox.Engine.Core.Persistence
                    .SnapshotPayload payload)
        {
            if (version != CurrentSchemaVersion)
            {
                return Reject("Unsupported schema version.");
            }

            var entries = new List<
                global::AI.Sandbox.Engine.Core.Memory.MemoryEntry<Episode>>();
            var text = System.Text.Encoding.UTF8.GetString(payload.ToArray());

            if (!string.IsNullOrEmpty(text))
            {
                var parts = text.Split('|');
                if (parts.Length != 18 ||
                    !ParseMemoryId(parts[0], out var memoryId) ||
                    !uint.TryParse(
                        parts[1],
                        System.Globalization.NumberStyles.None,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var revision) ||
                    !int.TryParse(
                        parts[2],
                        System.Globalization.NumberStyles.Integer,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var sequence) ||
                    !ushort.TryParse(
                        parts[3],
                        System.Globalization.NumberStyles.None,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var strength) ||
                    !ushort.TryParse(
                        parts[4],
                        System.Globalization.NumberStyles.None,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var salience) ||
                    !ParseOriginId(parts[5], out var originId) ||
                    !int.TryParse(
                        parts[6],
                        System.Globalization.NumberStyles.Integer,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var originKind) ||
                    !ulong.TryParse(
                        parts[7],
                        System.Globalization.NumberStyles.None,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var originVersion) ||
                    !ulong.TryParse(
                        parts[8],
                        System.Globalization.NumberStyles.None,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var originTick) ||
                    !ParseOptionalEntity(parts[9], out var sourceId) ||
                    !ParseOptionalClaim(parts[10], out var claimId) ||
                    !ParseOptionalEvidence(parts[11], out var evidenceId) ||
                    !ParseOptionalStimulus(parts[12], out var stimulusId) ||
                    !ParseOptionalChannel(parts[13], out var channelId) ||
                    !ulong.TryParse(
                        parts[14],
                        System.Globalization.NumberStyles.None,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var encodedVersion) ||
                    !ulong.TryParse(
                        parts[15],
                        System.Globalization.NumberStyles.None,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var encodedTick) ||
                    !ulong.TryParse(
                        parts[16],
                        System.Globalization.NumberStyles.None,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var updatedVersion) ||
                    !ulong.TryParse(
                        parts[17],
                        System.Globalization.NumberStyles.None,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var updatedTick))
                {
                    return Reject("Malformed memory payload.");
                }

                try
                {
                    var origin =
                        global::AI.Sandbox.Engine.Core.Memory
                            .MemoryOriginReference.Restore(
                                originId,
                                (global::AI.Sandbox.Engine.Core.Memory
                                    .MemoryOriginKind)originKind,
                                OwnerId(),
                                WorldId(),
                                ToVersion(originVersion),
                                originTick,
                                sourceId,
                                claimId,
                                evidenceId,
                                stimulusId,
                                channelId);
                    entries.Add(
                        global::AI.Sandbox.Engine.Core.Memory
                            .MemoryEntry<Episode>.Restore(
                                memoryId,
                                revision,
                                new Episode(sequence),
                                origin,
                                Strength(strength),
                                Salience(salience),
                                ToVersion(encodedVersion),
                                encodedTick,
                                ToVersion(updatedVersion),
                                updatedTick));
                }
                catch (Exception exception)
                    when (exception is ArgumentException or
                        InvalidOperationException or
                        OverflowException)
                {
                    return Reject(exception.Message);
                }
            }

            var entities =
                global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
                    .FromActiveEntities(
                        new[] { OwnerId(), SourceId() });
            var knowledgeEvidence =
                global::AI.Sandbox.Engine.Core.Knowledge
                    .KnowledgeEvidenceReference.Create(
                        KnowledgeEvidenceId(),
                        global::AI.Sandbox.Engine.Core.Knowledge
                            .KnowledgeEvidenceKind.Inference,
                        OwnerId(),
                        WorldId(),
                        global::AI.Sandbox.Engine.Core.WorldState
                            .WorldStateVersion.Initial,
                        0);
            var knowledge =
                global::AI.Sandbox.Engine.Core.Knowledge
                    .KnowledgeSet<LocatedClaim>.Create(
                        WorldId(),
                        OwnerId())
                    .Add(
                        KnowledgeClaimId(),
                        new LocatedClaim(7),
                        global::AI.Sandbox.Engine.Core.Knowledge
                            .KnowledgeConfidence.FromBasisPoints(8_000),
                        knowledgeEvidence)
                    .KnowledgeSet;
            var memory =
                global::AI.Sandbox.Engine.Core.Memory
                    .MemoryStore<Episode>.Restore(
                        WorldId(),
                        OwnerId(),
                        entries);
            var components =
                new global::AI.Sandbox.Engine.Core.Components
                    .ComponentRegistryBuilder(entities)
                    .Add(OwnerId(), knowledge)
                    .Add(OwnerId(), memory)
                    .Build();

            return global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateDecodeDecision<MemoryWorldState>.Accept(
                    new MemoryWorldState(entities, components));
        }

        private static string Optional<TKind>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>? id)
            where TKind : struct =>
            id is { } value
                ? value.ToString()
                : "-";

        private static global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateVersion ToVersion(ulong value) =>
            value == 0
                ? global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateVersion.Initial
                : global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateVersion.From(value);

        private static bool ParseMemoryId(
            string text,
            out global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Memory.MemoryIdKind> value) =>
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Memory.MemoryIdKind>.TryParse(
                    text,
                    out value);

        private static bool ParseOriginId(
            string text,
            out global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Memory.MemoryOriginIdKind>
                value) =>
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Memory
                    .MemoryOriginIdKind>.TryParse(
                        text,
                        out value);

        private static bool ParseOptionalEntity(
            string text,
            out global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>? value)
        {
            if (text == "-")
            {
                value = null;
                return true;
            }

            if (global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities
                    .EntityIdKind>.TryParse(
                        text,
                        out var parsed))
            {
                value = parsed;
                return true;
            }

            value = null;
            return false;
        }

        private static bool ParseOptionalClaim(
            string text,
            out global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Knowledge
                    .KnowledgeClaimIdKind>? value)
        {
            if (text == "-")
            {
                value = null;
                return true;
            }

            if (global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Knowledge
                    .KnowledgeClaimIdKind>.TryParse(
                        text,
                        out var parsed))
            {
                value = parsed;
                return true;
            }

            value = null;
            return false;
        }

        private static bool ParseOptionalEvidence(
            string text,
            out global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Knowledge
                    .KnowledgeEvidenceIdKind>? value)
        {
            if (text == "-")
            {
                value = null;
                return true;
            }

            if (global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Knowledge
                    .KnowledgeEvidenceIdKind>.TryParse(
                        text,
                        out var parsed))
            {
                value = parsed;
                return true;
            }

            value = null;
            return false;
        }

        private static bool ParseOptionalStimulus(
            string text,
            out global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionStimulusIdKind>? value)
        {
            if (text == "-")
            {
                value = null;
                return true;
            }

            if (global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionStimulusIdKind>.TryParse(
                        text,
                        out var parsed))
            {
                value = parsed;
                return true;
            }

            value = null;
            return false;
        }

        private static bool ParseOptionalChannel(
            string text,
            out global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionChannelIdKind>? value)
        {
            if (text == "-")
            {
                value = null;
                return true;
            }

            if (global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionChannelIdKind>.TryParse(
                        text,
                        out var parsed))
            {
                value = parsed;
                return true;
            }

            value = null;
            return false;
        }

        private static global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<MemoryWorldState> Reject(
                string reason) =>
            global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateDecodeDecision<MemoryWorldState>.Reject(reason);
    }
}
