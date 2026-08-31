namespace AI.Sandbox.Engine.Core.Tests;

public sealed class KnowledgeModelTests
{
    private sealed record KnowledgeWorldState(
        global::AI.Sandbox.Engine.Core.Entities.EntityRegistry Entities,
        global::AI.Sandbox.Engine.Core.Components.ComponentRegistry Components) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    private readonly record struct LocatedClaim(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> SubjectId,
        int Sequence) :
        global::AI.Sandbox.Engine.Core.Knowledge.IKnowledgeClaim;

    private abstract record AbstractClaim :
        global::AI.Sandbox.Engine.Core.Knowledge.IKnowledgeClaim;

    private record OpenClaim(string Text) :
        global::AI.Sandbox.Engine.Core.Knowledge.IKnowledgeClaim;

    private sealed record TextStimulus(string Text) :
        global::AI.Sandbox.Engine.Core.Perception.IPerceptionStimulus;

    private readonly record struct TextSignal(string Text) :
        global::AI.Sandbox.Engine.Core.Perception.IPerceptionSignal;

    private readonly record struct StoreClaim(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Knowledge.KnowledgeClaimIdKind>
            ClaimId,
        uint ExpectedRevision,
        LocatedClaim Claim,
        global::AI.Sandbox.Engine.Core.Knowledge.KnowledgeConfidence Confidence,
        global::AI.Sandbox.Engine.Core.Knowledge
            .KnowledgeEvidenceReference Evidence) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    [Xunit.Fact]
    public void ConfidenceAndEvidenceInvariantsAreExplicit()
    {
        global::AI.Sandbox.Engine.Core.Knowledge
            .KnowledgeConfidence empty = default;
        var partial = global::AI.Sandbox.Engine.Core.Knowledge
            .KnowledgeConfidence.FromBasisPoints(6_500);

        Xunit.Assert.True(empty.IsEmpty);
        Xunit.Assert.Equal((ushort)6_500, partial.BasisPoints);
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeConfidence.FromBasisPoints(10_001));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeEvidenceReference.Create(
                    EvidenceId(1),
                    global::AI.Sandbox.Engine.Core.Knowledge
                        .KnowledgeEvidenceKind.Perception,
                    OwnerId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeEvidenceReference.Create(
                    EvidenceId(2),
                    global::AI.Sandbox.Engine.Core.Knowledge
                        .KnowledgeEvidenceKind.Communication,
                    OwnerId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0));
    }

    [Xunit.Fact]
    public void PerceptionEvidencePreservesObservationProvenance()
    {
        var manager = CreateManager();
        var processor =
            new global::AI.Sandbox.Engine.Core.Perception
                .PerceptionProcessorBuilder<KnowledgeWorldState>()
                .Add<TextStimulus, TextSignal>(new TextEvaluator())
                .Build(manager);
        var snapshot = manager.Read();
        var observation = processor.Evaluate<TextStimulus, TextSignal>(
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionStimulusEnvelope<TextStimulus>.Create(
                    StimulusId(1),
                    ChannelId(),
                    OwnerId(),
                    snapshot.WorldId,
                    snapshot.Version,
                    snapshot.SimulationTick,
                    new TextStimulus("seen"))).Observation!;
        var evidence =
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeEvidenceReference.FromPerception(
                    EvidenceId(3),
                    observation);

        Xunit.Assert.Equal(
            observation.ObserverEntityId,
            evidence.RecipientEntityId);
        Xunit.Assert.Equal(
            observation.StimulusId,
            evidence.PerceptionStimulusId);
        Xunit.Assert.Equal(
            observation.ChannelId,
            evidence.PerceptionChannelId);
        Xunit.Assert.True(evidence.HasPerceptionProvenance);
    }

    [Xunit.Fact]
    public void SetRejectsInvalidTypesAndScopesEvidence()
    {
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeSet<AbstractClaim>.Create(
                    WorldId(),
                    OwnerId()));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeSet<OpenClaim>.Create(
                    WorldId(),
                    OwnerId()));

        var set = EmptySet();
        var wrongOwner = set.Add(
            ClaimId(1),
            new LocatedClaim(SubjectId(), 1),
            Confidence(7_000),
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeEvidenceReference.Create(
                    EvidenceId(4),
                    global::AI.Sandbox.Engine.Core.Knowledge
                        .KnowledgeEvidenceKind.Inference,
                    SourceId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0));
        var wrongWorld = set.Add(
            ClaimId(2),
            new LocatedClaim(SubjectId(), 2),
            Confidence(7_000),
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeEvidenceReference.Create(
                    EvidenceId(5),
                    global::AI.Sandbox.Engine.Core.Knowledge
                        .KnowledgeEvidenceKind.Inference,
                    OwnerId(),
                    OtherWorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeMutationStatus.EvidenceOwnerMismatch,
            wrongOwner.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeMutationStatus.EvidenceWorldMismatch,
            wrongWorld.Status);
    }

    [Xunit.Fact]
    public void AddReviseRegressionConflictAndRemoveAreDeterministic()
    {
        var added = EmptySet().Add(
            ClaimId(2),
            new LocatedClaim(SubjectId(), 1),
            Confidence(6_000),
            Evidence(10, 2, 1));
        var withTwo = added.KnowledgeSet.Add(
            ClaimId(1),
            new LocatedClaim(SubjectId(), 2),
            Confidence(6_500),
            Evidence(11, 2, 1));

        Xunit.Assert.Equal(
            new[] { ClaimId(1), ClaimId(2) },
            withTwo.KnowledgeSet.Entries
                .Select(entry => entry.ClaimId)
                .ToArray());

        var stale = added.KnowledgeSet.Revise(
            ClaimId(2),
            2,
            new LocatedClaim(SubjectId(), 3),
            Confidence(7_000),
            Evidence(12, 3, 1));
        var regression = added.KnowledgeSet.Revise(
            ClaimId(2),
            1,
            new LocatedClaim(SubjectId(), 3),
            Confidence(7_000),
            Evidence(13, 1, 0));
        var unchanged = added.KnowledgeSet.Revise(
            ClaimId(2),
            1,
            new LocatedClaim(SubjectId(), 1),
            Confidence(6_000),
            Evidence(10, 2, 1));
        var revised = added.KnowledgeSet.Revise(
            ClaimId(2),
            1,
            new LocatedClaim(SubjectId(), 3),
            Confidence(8_000),
            Evidence(14, 3, 1));
        var removed = revised.KnowledgeSet.Remove(
            ClaimId(2),
            2);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeMutationStatus.RevisionConflict,
            stale.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeMutationStatus.EvidenceRegression,
            regression.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeMutationStatus.Unchanged,
            unchanged.Status);
        Xunit.Assert.Equal((uint)2, revised.Entry!.Revision);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeMutationStatus.Removed,
            removed.Status);
        Xunit.Assert.True(removed.KnowledgeSet.IsEmpty);
    }

    [Xunit.Fact]
    public void ObservationDoesNotBecomeKnowledgeUntilExplicitCommand()
    {
        var manager = CreateManager();
        var processor =
            new global::AI.Sandbox.Engine.Core.Perception
                .PerceptionProcessorBuilder<KnowledgeWorldState>()
                .Add<TextStimulus, TextSignal>(new TextEvaluator())
                .Build(manager);
        var runtime = CreateRuntime(manager);
        var before = manager.Read();
        var observation = processor.Evaluate<TextStimulus, TextSignal>(
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionStimulusEnvelope<TextStimulus>.Create(
                    StimulusId(2),
                    ChannelId(),
                    OwnerId(),
                    before.WorldId,
                    before.Version,
                    before.SimulationTick,
                    new TextStimulus("location"))).Observation!;

        Xunit.Assert.True(GetSet(manager.Read().State).IsEmpty);
        Xunit.Assert.Same(before, manager.Read());

        var evidence =
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeEvidenceReference.FromPerception(
                    EvidenceId(20),
                    observation);
        var result = runtime.ExecuteCommand(
            global::AI.Sandbox.Engine.Core.Commands
                .CommandEnvelope<StoreClaim>.Create(
                    CommandId(1),
                    before.WorldId,
                    before.Version,
                    before.SimulationTick,
                    new StoreClaim(
                        ClaimId(1),
                        0,
                        new LocatedClaim(SubjectId(), 1),
                        Confidence(8_500),
                        evidence)));

        Xunit.Assert.True(result.WasCommitted);
        Xunit.Assert.Equal(1, GetSet(manager.Read().State).Count);
    }

    [Xunit.Fact]
    public void SaveRestoreContinuationWithKnowledgeCommandsIsByteIdentical()
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
                .WorldStateSnapshotPersistence<KnowledgeWorldState>(
                    new KnowledgeCodec());
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
                .WorldStateManager<KnowledgeWorldState>.Restore(
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
            .RuntimeOrchestrator<KnowledgeWorldState> runtime,
        int start,
        int count)
    {
        for (var offset = 0; offset < count; offset++)
        {
            var cycle = start + offset;
            var snapshot = runtime.Read();
            var set = GetSet(snapshot.State);
            var expected = set.TryGet(ClaimId(1), out var current)
                ? current!.Revision
                : 0;
            var result = runtime.ExecuteCommand(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandEnvelope<StoreClaim>.Create(
                        CommandId(cycle + 10),
                        snapshot.WorldId,
                        snapshot.Version,
                        snapshot.SimulationTick,
                        new StoreClaim(
                            ClaimId(1),
                            expected,
                            new LocatedClaim(
                                SubjectId(),
                                cycle + 1),
                            Confidence((ushort)(5_000 + cycle)),
                            global::AI.Sandbox.Engine.Core.Knowledge
                                .KnowledgeEvidenceReference.Create(
                                    EvidenceId(cycle + 100),
                                    global::AI.Sandbox.Engine.Core.Knowledge
                                        .KnowledgeEvidenceKind.Inference,
                                    OwnerId(),
                                    snapshot.WorldId,
                                    snapshot.Version,
                                    snapshot.SimulationTick))));

            Xunit.Assert.True(
                result.WasCommitted,
                $"Cycle {cycle}: {result.CommandResult?.Status.ToString() ?? "none"}");
        }
    }

    private static global::AI.Sandbox.Engine.Core.Runtime
        .RuntimeOrchestrator<KnowledgeWorldState> CreateRuntime(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<KnowledgeWorldState> manager)
    {
        return new global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestratorBuilder<KnowledgeWorldState>()
            .AddCommandHandler(new StoreHandler())
            .Build(manager);
    }

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<KnowledgeWorldState> CreateManager()
    {
        var entities =
            global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
                .FromActiveEntities(
                    new[] { OwnerId(), SubjectId(), SourceId() });
        var components =
            new global::AI.Sandbox.Engine.Core.Components
                .ComponentRegistryBuilder(entities)
                .Add(OwnerId(), EmptySet())
                .Build();

        return global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<KnowledgeWorldState>.Create(
                WorldId(),
                new KnowledgeWorldState(entities, components));
    }

    private static global::AI.Sandbox.Engine.Core.Knowledge
        .KnowledgeSet<LocatedClaim> GetSet(KnowledgeWorldState state)
    {
        Xunit.Assert.True(
            state.Components.TryGet<
                global::AI.Sandbox.Engine.Core.Knowledge
                    .KnowledgeSet<LocatedClaim>>(
                        OwnerId(),
                        out var set));
        return set!;
    }

    private static global::AI.Sandbox.Engine.Core.Knowledge
        .KnowledgeSet<LocatedClaim> EmptySet()
    {
        return global::AI.Sandbox.Engine.Core.Knowledge
            .KnowledgeSet<LocatedClaim>.Create(
                WorldId(),
                OwnerId());
    }

    private static global::AI.Sandbox.Engine.Core.Knowledge
        .KnowledgeConfidence Confidence(ushort value)
    {
        return global::AI.Sandbox.Engine.Core.Knowledge
            .KnowledgeConfidence.FromBasisPoints(value);
    }

    private static global::AI.Sandbox.Engine.Core.Knowledge
        .KnowledgeEvidenceReference Evidence(
            int suffix,
            ulong version,
            ulong tick)
    {
        return global::AI.Sandbox.Engine.Core.Knowledge
            .KnowledgeEvidenceReference.Create(
                EvidenceId(suffix),
                global::AI.Sandbox.Engine.Core.Knowledge
                    .KnowledgeEvidenceKind.Inference,
                OwnerId(),
                WorldId(),
                version == 0
                    ? global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial
                    : global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.From(version),
                tick);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000001400");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> OtherWorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000001401");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OwnerId() =>
        EntityId("019b0000-0000-7000-a400-000000000001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> SubjectId() =>
        EntityId("019b0000-0000-7000-a400-000000000002");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> SourceId() =>
        EntityId("019b0000-0000-7000-a400-000000000003");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> EntityId(
            string text) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(text);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Knowledge.KnowledgeClaimIdKind> ClaimId(
            int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeClaimIdKind>.Parse(
                    $"019b0000-0000-7000-a500-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Knowledge.KnowledgeEvidenceIdKind>
        EvidenceId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeEvidenceIdKind>.Parse(
                    $"019b0000-0000-7000-a600-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Perception.PerceptionStimulusIdKind>
        StimulusId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionStimulusIdKind>.Parse(
                    $"019b0000-0000-7000-a700-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Perception.PerceptionChannelIdKind>
        ChannelId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionChannelIdKind>.Parse(
                    "019b0000-0000-7000-a800-000000000001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Commands.CommandIdKind> CommandId(
            int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Commands.CommandIdKind>.Parse(
                $"019b0000-0000-7000-a900-{suffix:D12}");

    private sealed class TextEvaluator :
        global::AI.Sandbox.Engine.Core.Perception.IPerceptionEvaluator<
            KnowledgeWorldState,
            TextStimulus,
            TextSignal>
    {
        public global::AI.Sandbox.Engine.Core.Perception
            .PerceptionDecision<TextSignal> Evaluate(
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionContext<
                        KnowledgeWorldState,
                        TextStimulus> context)
        {
            return global::AI.Sandbox.Engine.Core.Perception
                .PerceptionDecision<TextSignal>.Observe(
                    new TextSignal(context.Envelope.Payload.Text),
                    global::AI.Sandbox.Engine.Core.Perception
                        .PerceptionConfidence.Certain);
        }
    }

    private sealed class StoreHandler :
        global::AI.Sandbox.Engine.Core.Commands.ICommandHandler<
            KnowledgeWorldState,
            StoreClaim>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<KnowledgeWorldState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands.CommandContext<
                    KnowledgeWorldState,
                    StoreClaim> context)
        {
            var command = context.Envelope.Payload;
            var current = GetSet(context.Snapshot.State);
            var mutation = command.ExpectedRevision == 0
                ? current.Add(
                    command.ClaimId,
                    command.Claim,
                    command.Confidence,
                    command.Evidence)
                : current.Revise(
                    command.ClaimId,
                    command.ExpectedRevision,
                    command.Claim,
                    command.Confidence,
                    command.Evidence);

            if (!mutation.WasApplied)
            {
                return global::AI.Sandbox.Engine.Core.Commands
                    .CommandDecision<KnowledgeWorldState>.Reject(
                        mutation.Status.ToString());
            }

            var componentResult =
                context.Snapshot.State.Components.Set(
                    context.Snapshot.State.Entities,
                    OwnerId(),
                    mutation.KnowledgeSet);

            if (!componentResult.WasApplied)
            {
                return global::AI.Sandbox.Engine.Core.Commands
                    .CommandDecision<KnowledgeWorldState>.Reject(
                        componentResult.Status.ToString());
            }

            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<KnowledgeWorldState>.Accept(
                    context.Snapshot.State with
                    {
                        Components = componentResult.Registry,
                    });
        }
    }

    private sealed class KnowledgeCodec :
        global::AI.Sandbox.Engine.Core.Persistence
            .IWorldStateSnapshotCodec<KnowledgeWorldState>
    {
        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaId SchemaId { get; } =
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaId.Parse("knowledge.validation");

        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaVersion CurrentSchemaVersion { get; } =
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion.From(1);

        public bool CanDecode(
            global::AI.Sandbox.Engine.Core.Persistence
                .PersistenceSchemaVersion version) =>
            version == CurrentSchemaVersion;

        public global::AI.Sandbox.Engine.Core.Persistence
            .SnapshotPayload Encode(KnowledgeWorldState state)
        {
            var entry = GetSet(state).Entries.SingleOrDefault();
            var text = entry is null
                ? string.Empty
                : string.Join(
                    '|',
                    entry.ClaimId,
                    entry.Revision.ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
                    entry.Claim.Sequence.ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
                    entry.Confidence.BasisPoints.ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
                    entry.Evidence.EvidenceId,
                    entry.Evidence.WorldStateVersion.Value.ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
                    entry.Evidence.SimulationTick.ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
                    entry.FirstAcquiredWorldStateVersion.Value.ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
                    entry.FirstAcquiredSimulationTick.ToString(
                        System.Globalization.CultureInfo.InvariantCulture));

            return global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotPayload.From(
                    System.Text.Encoding.UTF8.GetBytes(text));
        }

        public global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<KnowledgeWorldState> Decode(
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion version,
                global::AI.Sandbox.Engine.Core.Persistence
                    .SnapshotPayload payload)
        {
            if (version != CurrentSchemaVersion)
            {
                return Reject("Unsupported schema version.");
            }

            var text = System.Text.Encoding.UTF8.GetString(payload.ToArray());
            var entries = new List<
                global::AI.Sandbox.Engine.Core.Knowledge
                    .KnowledgeEntry<LocatedClaim>>();

            if (!string.IsNullOrEmpty(text))
            {
                var parts = text.Split('|');
                if (parts.Length != 9 ||
                    !global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.Knowledge
                            .KnowledgeClaimIdKind>.TryParse(
                                parts[0],
                                out var claimId) ||
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
                        out var confidence) ||
                    !global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.Knowledge
                            .KnowledgeEvidenceIdKind>.TryParse(
                                parts[4],
                                out var evidenceId) ||
                    !ulong.TryParse(
                        parts[5],
                        System.Globalization.NumberStyles.None,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var evidenceVersion) ||
                    !ulong.TryParse(
                        parts[6],
                        System.Globalization.NumberStyles.None,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var evidenceTick) ||
                    !ulong.TryParse(
                        parts[7],
                        System.Globalization.NumberStyles.None,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var firstVersion) ||
                    !ulong.TryParse(
                        parts[8],
                        System.Globalization.NumberStyles.None,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var firstTick))
                {
                    return Reject("Malformed knowledge payload.");
                }

                try
                {
                    var evidence =
                        global::AI.Sandbox.Engine.Core.Knowledge
                            .KnowledgeEvidenceReference.Create(
                                evidenceId,
                                global::AI.Sandbox.Engine.Core.Knowledge
                                    .KnowledgeEvidenceKind.Inference,
                                OwnerId(),
                                WorldId(),
                                evidenceVersion == 0
                                    ? global::AI.Sandbox.Engine.Core.WorldState
                                        .WorldStateVersion.Initial
                                    : global::AI.Sandbox.Engine.Core.WorldState
                                        .WorldStateVersion.From(evidenceVersion),
                                evidenceTick);
                    entries.Add(
                        global::AI.Sandbox.Engine.Core.Knowledge
                            .KnowledgeEntry<LocatedClaim>.Restore(
                                claimId,
                                revision,
                                new LocatedClaim(SubjectId(), sequence),
                                Confidence(confidence),
                                evidence,
                                firstVersion == 0
                                    ? global::AI.Sandbox.Engine.Core.WorldState
                                        .WorldStateVersion.Initial
                                    : global::AI.Sandbox.Engine.Core.WorldState
                                        .WorldStateVersion.From(firstVersion),
                                firstTick));
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
                        new[] { OwnerId(), SubjectId(), SourceId() });
            var set =
                global::AI.Sandbox.Engine.Core.Knowledge
                    .KnowledgeSet<LocatedClaim>.Restore(
                        WorldId(),
                        OwnerId(),
                        entries);
            var components =
                new global::AI.Sandbox.Engine.Core.Components
                    .ComponentRegistryBuilder(entities)
                    .Add(OwnerId(), set)
                    .Build();

            return global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateDecodeDecision<KnowledgeWorldState>.Accept(
                    new KnowledgeWorldState(entities, components));
        }

        private static global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<KnowledgeWorldState> Reject(
                string reason) =>
            global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateDecodeDecision<KnowledgeWorldState>.Reject(reason);
    }
}
