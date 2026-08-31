namespace AI.Sandbox.Engine.Core.Tests;

public sealed class RelationshipModelTests
{
    private readonly record struct Disposition(int Regard, int Familiarity) :
        global::AI.Sandbox.Engine.Core.Relationships.IRelationshipState;

    private readonly record struct AlternateDisposition(int Distance) :
        global::AI.Sandbox.Engine.Core.Relationships.IRelationshipState;

    private abstract record AbstractRelationship :
        global::AI.Sandbox.Engine.Core.Relationships.IRelationshipState;

    private sealed record ConcreteAbstractRelationship(int Value) :
        AbstractRelationship;

    private record OpenRelationship(int Value) :
        global::AI.Sandbox.Engine.Core.Relationships.IRelationshipState;

    private readonly record struct LocatedClaim(int Sequence) :
        global::AI.Sandbox.Engine.Core.Knowledge.IKnowledgeClaim;

    private readonly record struct Episode(int Sequence) :
        global::AI.Sandbox.Engine.Core.Memory.IMemoryContent;

    private sealed record RelationshipWorldState(
        global::AI.Sandbox.Engine.Core.Entities.EntityRegistry Entities,
        global::AI.Sandbox.Engine.Core.Components.ComponentRegistry Components) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    private sealed record SetRelationship(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> TargetEntityId,
        uint ExpectedRevision,
        Disposition State,
        global::AI.Sandbox.Engine.Core.Relationships
            .RelationshipChangeReference Change) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    [Xunit.Fact]
    public void ExactRelationshipStateTypesAreRequired()
    {
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipSet<AbstractRelationship>.Create(
                    WorldId(),
                    OwnerId()));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipSet<OpenRelationship>.Create(
                    WorldId(),
                    OwnerId()));

        var disposition =
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipSet<Disposition>.Create(
                    WorldId(),
                    OwnerId());
        var alternate =
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipSet<AlternateDisposition>.Create(
                    WorldId(),
                    OwnerId());

        Xunit.Assert.True(disposition.IsEmpty);
        Xunit.Assert.True(alternate.IsEmpty);
        Xunit.Assert.NotEqual(disposition.GetType(), alternate.GetType());
        _ = new ConcreteAbstractRelationship(1);
    }

    [Xunit.Fact]
    public void RelationshipsAreDirectedAndIndependent()
    {
        var ownerToTarget =
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipSet<Disposition>.Create(
                    WorldId(),
                    OwnerId())
                .Add(
                    TargetId(1),
                    new Disposition(7, 3),
                    ExternalChange(
                        1,
                        OwnerId(),
                        TargetId(1),
                        0,
                        0));
        var targetToOwner =
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipSet<Disposition>.Create(
                    WorldId(),
                    TargetId(1))
                .Add(
                    OwnerId(),
                    new Disposition(-4, 9),
                    ExternalChange(
                        2,
                        TargetId(1),
                        OwnerId(),
                        0,
                        0));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipMutationStatus.Added,
            ownerToTarget.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipMutationStatus.Added,
            targetToOwner.Status);
        Xunit.Assert.Equal(
            new Disposition(7, 3),
            ownerToTarget.Entry!.State);
        Xunit.Assert.Equal(
            new Disposition(-4, 9),
            targetToOwner.Entry!.State);
        Xunit.Assert.NotEqual(
            ownerToTarget.Entry.State,
            targetToOwner.Entry.State);

        Xunit.Assert.Throws<ArgumentException>(
            () => ExternalChange(
                3,
                OwnerId(),
                OwnerId(),
                0,
                0));
    }

    [Xunit.Fact]
    public void TypedChangeProvenanceIsPreserved()
    {
        var evidence =
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
        var knowledgeMutation =
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeSet<LocatedClaim>.Create(
                    WorldId(),
                    OwnerId())
                .Add(
                    KnowledgeClaimId(),
                    new LocatedClaim(5),
                    global::AI.Sandbox.Engine.Core.Knowledge
                        .KnowledgeConfidence.FromBasisPoints(8_000),
                    evidence);
        var knowledgeEntry = knowledgeMutation.Entry!;
        var knowledgeChange =
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipChangeReference.FromKnowledge(
                    ChangeId(4),
                    TargetId(1),
                    knowledgeEntry);

        Xunit.Assert.True(knowledgeChange.HasKnowledgeProvenance);
        Xunit.Assert.Equal(
            KnowledgeClaimId(),
            knowledgeChange.KnowledgeClaimId);
        Xunit.Assert.Equal(
            KnowledgeEvidenceId(),
            knowledgeChange.KnowledgeEvidenceId);

        var memoryMutation =
            global::AI.Sandbox.Engine.Core.Memory
                .MemoryStore<Episode>.Create(
                    WorldId(),
                    OwnerId())
                .Encode(
                    MemoryId(),
                    new Episode(knowledgeEntry.Claim.Sequence),
                    global::AI.Sandbox.Engine.Core.Memory
                        .MemoryOriginReference.FromKnowledge(
                            MemoryOriginId(),
                            knowledgeEntry),
                    global::AI.Sandbox.Engine.Core.Memory
                        .MemoryStrength.FromBasisPoints(7_000),
                    global::AI.Sandbox.Engine.Core.Memory
                        .MemorySalience.FromBasisPoints(6_000));
        var memoryChange =
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipChangeReference.FromMemory(
                    ChangeId(5),
                    TargetId(1),
                    memoryMutation.Entry!);

        Xunit.Assert.True(memoryChange.HasMemoryProvenance);
        Xunit.Assert.True(memoryChange.HasKnowledgeProvenance);
        Xunit.Assert.Equal(MemoryId(), memoryChange.MemoryId);
        Xunit.Assert.Equal(MemoryOriginId(), memoryChange.MemoryOriginId);

        var perceptionChange =
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipChangeReference.Restore(
                    ChangeId(6),
                    global::AI.Sandbox.Engine.Core.Relationships
                        .RelationshipChangeKind.Perception,
                    OwnerId(),
                    TargetId(1),
                    WorldId(),
                    Version(3),
                    9,
                    null,
                    null,
                    null,
                    null,
                    null,
                    PerceptionStimulusId(),
                    PerceptionChannelId());

        Xunit.Assert.True(perceptionChange.HasPerceptionProvenance);
        Xunit.Assert.False(perceptionChange.HasKnowledgeProvenance);
        Xunit.Assert.False(perceptionChange.HasMemoryProvenance);

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipChangeReference.Create(
                    ChangeId(7),
                    global::AI.Sandbox.Engine.Core.Relationships
                        .RelationshipChangeKind.Communication,
                    OwnerId(),
                    TargetId(1),
                    WorldId(),
                    Version(3),
                    9));
    }

    [Xunit.Fact]
    public void OptimisticRevisionTemporalAndRemovalOutcomesAreExplicit()
    {
        var empty =
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipSet<Disposition>.Create(
                    WorldId(),
                    OwnerId());
        var initialChange = ExternalChange(
            10,
            OwnerId(),
            TargetId(1),
            1,
            1);
        var added = empty.Add(
            TargetId(1),
            new Disposition(1, 2),
            initialChange);

        Xunit.Assert.True(added.WasApplied);
        Xunit.Assert.Equal(1U, added.Entry!.Revision);

        var duplicate = added.RelationshipSet.Add(
            TargetId(1),
            new Disposition(8, 8),
            ExternalChange(
                11,
                OwnerId(),
                TargetId(1),
                2,
                2));
        var conflict = added.RelationshipSet.Revise(
            TargetId(1),
            99,
            new Disposition(3, 4),
            ExternalChange(
                12,
                OwnerId(),
                TargetId(1),
                2,
                2));
        var revised = added.RelationshipSet.Revise(
            TargetId(1),
            1,
            new Disposition(3, 4),
            ExternalChange(
                13,
                OwnerId(),
                TargetId(1),
                2,
                2));
        var temporal = revised.RelationshipSet.Revise(
            TargetId(1),
            2,
            new Disposition(5, 6),
            ExternalChange(
                14,
                OwnerId(),
                TargetId(1),
                1,
                1));
        var unchanged = revised.RelationshipSet.Revise(
            TargetId(1),
            2,
            revised.Entry!.State,
            revised.Entry.LastChange);
        var removeConflict = revised.RelationshipSet.Remove(
            TargetId(1),
            1,
            ExternalChange(
                15,
                OwnerId(),
                TargetId(1),
                3,
                3));
        var removed = revised.RelationshipSet.Remove(
            TargetId(1),
            2,
            ExternalChange(
                16,
                OwnerId(),
                TargetId(1),
                3,
                3));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipMutationStatus.RelationshipAlreadyExists,
            duplicate.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipMutationStatus.RevisionConflict,
            conflict.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipMutationStatus.Revised,
            revised.Status);
        Xunit.Assert.Equal(2U, revised.Entry.Revision);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipMutationStatus.TemporalRegression,
            temporal.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipMutationStatus.Unchanged,
            unchanged.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipMutationStatus.RevisionConflict,
            removeConflict.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipMutationStatus.Removed,
            removed.Status);
        Xunit.Assert.True(removed.RelationshipSet.IsEmpty);
    }

    [Xunit.Fact]
    public void RestoreValidatesScopeAndOrdersTargetsDeterministically()
    {
        var first =
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipEntry<Disposition>.Restore(
                    TargetId(2),
                    1,
                    new Disposition(2, 2),
                    ExternalChange(
                        20,
                        OwnerId(),
                        TargetId(2),
                        2,
                        2),
                    Version(2),
                    2,
                    Version(2),
                    2);
        var second =
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipEntry<Disposition>.Restore(
                    TargetId(1),
                    1,
                    new Disposition(1, 1),
                    ExternalChange(
                        21,
                        OwnerId(),
                        TargetId(1),
                        1,
                        1),
                    Version(1),
                    1,
                    Version(1),
                    1);

        var restored =
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipSet<Disposition>.Restore(
                    WorldId(),
                    OwnerId(),
                    new[] { first, second });

        Xunit.Assert.Equal(
            new[] { TargetId(1), TargetId(2) },
            restored.Entries.Select(entry => entry.TargetEntityId).ToArray());
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipSet<Disposition>.Restore(
                    WorldId(),
                    SourceId(),
                    new[] { first }));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipSet<Disposition>.Restore(
                    OtherWorldId(),
                    OwnerId(),
                    new[] { first }));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipSet<Disposition>.Restore(
                    WorldId(),
                    OwnerId(),
                    new[] { first, first }));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipEntry<Disposition>.Restore(
                    TargetId(1),
                    1,
                    new Disposition(1, 1),
                    ExternalChange(
                        22,
                        OwnerId(),
                        TargetId(1),
                        3,
                        3),
                    Version(1),
                    1,
                    Version(2),
                    2));
    }

    [Xunit.Fact]
    public void RelationshipChangesDoNotRewriteKnowledgeOrMemory()
    {
        var manager = CreateManager();
        var runtime = CreateRuntime(manager);
        var before = runtime.Read();

        Xunit.Assert.True(GetRelationships(before.State).IsEmpty);
        Xunit.Assert.True(GetKnowledge(before.State).IsEmpty);
        Xunit.Assert.True(GetMemory(before.State).IsEmpty);

        var result = runtime.ExecuteCommand(
            global::AI.Sandbox.Engine.Core.Commands
                .CommandEnvelope<SetRelationship>.Create(
                    CommandId(1),
                    before.WorldId,
                    before.Version,
                    before.SimulationTick,
                    new SetRelationship(
                        TargetId(1),
                        0,
                        new Disposition(5, 6),
                        ExternalChange(
                            30,
                            OwnerId(),
                            TargetId(1),
                            before.Version.Value,
                            before.SimulationTick))));

        Xunit.Assert.True(result.WasCommitted);
        var after = manager.Read();
        Xunit.Assert.Equal(1, GetRelationships(after.State).Count);
        Xunit.Assert.True(GetKnowledge(after.State).IsEmpty);
        Xunit.Assert.True(GetMemory(after.State).IsEmpty);
    }

    [Xunit.Fact]
    public void SaveRestoreContinuationWithRelationshipCommandsIsByteIdentical()
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
                .WorldStateSnapshotPersistence<RelationshipWorldState>(
                    new RelationshipCodec());
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
                .WorldStateManager<RelationshipWorldState>.Restore(
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
            .RuntimeOrchestrator<RelationshipWorldState> runtime,
        int start,
        int count)
    {
        for (var offset = 0; offset < count; offset++)
        {
            var cycle = start + offset;
            var snapshot = runtime.Read();
            var target = TargetId((cycle % 2) + 1);
            var set = GetRelationships(snapshot.State);
            var exists = set.TryGet(target, out var current);
            var command = new SetRelationship(
                target,
                exists ? current!.Revision : 0,
                new Disposition(cycle + 1, checked((cycle + 1) * 2)),
                ExternalChange(
                    100 + cycle,
                    OwnerId(),
                    target,
                    snapshot.Version.Value,
                    snapshot.SimulationTick));
            var result = runtime.ExecuteCommand(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandEnvelope<SetRelationship>.Create(
                        CommandId(100 + cycle),
                        snapshot.WorldId,
                        snapshot.Version,
                        snapshot.SimulationTick,
                        command));

            Xunit.Assert.True(
                result.WasCommitted,
                $"Cycle {cycle}: " +
                $"{result.CommandResult?.Status.ToString() ?? "none"}");
        }
    }

    private static global::AI.Sandbox.Engine.Core.Runtime
        .RuntimeOrchestrator<RelationshipWorldState> CreateRuntime(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<RelationshipWorldState> manager)
    {
        return new global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestratorBuilder<RelationshipWorldState>()
            .AddCommandHandler(new SetRelationshipHandler())
            .Build(manager);
    }

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<RelationshipWorldState> CreateManager()
    {
        var entities =
            global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
                .FromActiveEntities(
                    new[]
                    {
                        OwnerId(),
                        TargetId(1),
                        TargetId(2),
                        SourceId(),
                    });
        var relationships =
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipSet<Disposition>.Create(
                    WorldId(),
                    OwnerId());
        var knowledge =
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeSet<LocatedClaim>.Create(
                    WorldId(),
                    OwnerId());
        var memory =
            global::AI.Sandbox.Engine.Core.Memory
                .MemoryStore<Episode>.Create(
                    WorldId(),
                    OwnerId());
        var components =
            new global::AI.Sandbox.Engine.Core.Components
                .ComponentRegistryBuilder(entities)
                .Add(OwnerId(), relationships)
                .Add(OwnerId(), knowledge)
                .Add(OwnerId(), memory)
                .Build();

        return global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<RelationshipWorldState>.Create(
                WorldId(),
                new RelationshipWorldState(entities, components));
    }

    private static global::AI.Sandbox.Engine.Core.Relationships
        .RelationshipSet<Disposition> GetRelationships(
            RelationshipWorldState state)
    {
        Xunit.Assert.True(
            state.Components.TryGet<
                global::AI.Sandbox.Engine.Core.Relationships
                    .RelationshipSet<Disposition>>(
                        OwnerId(),
                        out var relationships));
        return relationships!;
    }

    private static global::AI.Sandbox.Engine.Core.Knowledge
        .KnowledgeSet<LocatedClaim> GetKnowledge(
            RelationshipWorldState state)
    {
        Xunit.Assert.True(
            state.Components.TryGet<
                global::AI.Sandbox.Engine.Core.Knowledge
                    .KnowledgeSet<LocatedClaim>>(
                        OwnerId(),
                        out var knowledge));
        return knowledge!;
    }

    private static global::AI.Sandbox.Engine.Core.Memory
        .MemoryStore<Episode> GetMemory(RelationshipWorldState state)
    {
        Xunit.Assert.True(
            state.Components.TryGet<
                global::AI.Sandbox.Engine.Core.Memory.MemoryStore<Episode>>(
                    OwnerId(),
                    out var memory));
        return memory!;
    }

    private sealed class SetRelationshipHandler :
        global::AI.Sandbox.Engine.Core.Commands.ICommandHandler<
            RelationshipWorldState,
            SetRelationship>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<RelationshipWorldState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<RelationshipWorldState, SetRelationship>
                        context)
        {
            var current = GetRelationships(context.Snapshot.State);
            var mutation = CommandIsAdd(context.Envelope.Payload)
                ? current.Add(
                    context.Envelope.Payload.TargetEntityId,
                    context.Envelope.Payload.State,
                    context.Envelope.Payload.Change)
                : current.Revise(
                    context.Envelope.Payload.TargetEntityId,
                    context.Envelope.Payload.ExpectedRevision,
                    context.Envelope.Payload.State,
                    context.Envelope.Payload.Change);

            if (!mutation.WasApplied)
            {
                return global::AI.Sandbox.Engine.Core.Commands
                    .CommandDecision<RelationshipWorldState>.Reject(
                        mutation.Status.ToString());
            }

            var componentResult =
                context.Snapshot.State.Components.Set(
                    context.Snapshot.State.Entities,
                    OwnerId(),
                    mutation.RelationshipSet);

            if (!componentResult.WasApplied)
            {
                return global::AI.Sandbox.Engine.Core.Commands
                    .CommandDecision<RelationshipWorldState>.Reject(
                        componentResult.Status.ToString());
            }

            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<RelationshipWorldState>.Accept(
                    context.Snapshot.State with
                    {
                        Components = componentResult.Registry,
                    });
        }

        private static bool CommandIsAdd(SetRelationship command) =>
            command.ExpectedRevision == 0;
    }

    private sealed class RelationshipCodec :
        global::AI.Sandbox.Engine.Core.Persistence
            .IWorldStateSnapshotCodec<RelationshipWorldState>
    {
        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaId SchemaId { get; } =
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaId.Parse("relationship.validation");

        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaVersion CurrentSchemaVersion { get; } =
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion.From(1);

        public bool CanDecode(
            global::AI.Sandbox.Engine.Core.Persistence
                .PersistenceSchemaVersion version) =>
            version == CurrentSchemaVersion;

        public global::AI.Sandbox.Engine.Core.Persistence
            .SnapshotPayload Encode(RelationshipWorldState state)
        {
            var text = string.Join(
                "\n",
                GetRelationships(state).Entries.Select(
                    entry => string.Join(
                        '|',
                        entry.TargetEntityId,
                        entry.Revision.ToString(
                            System.Globalization.CultureInfo.InvariantCulture),
                        entry.State.Regard.ToString(
                            System.Globalization.CultureInfo.InvariantCulture),
                        entry.State.Familiarity.ToString(
                            System.Globalization.CultureInfo.InvariantCulture),
                        entry.LastChange.ChangeId,
                        ((int)entry.LastChange.Kind).ToString(
                            System.Globalization.CultureInfo.InvariantCulture),
                        entry.LastChange.WorldStateVersion.Value.ToString(
                            System.Globalization.CultureInfo.InvariantCulture),
                        entry.LastChange.SimulationTick.ToString(
                            System.Globalization.CultureInfo.InvariantCulture),
                        Optional(entry.LastChange.SourceEntityId),
                        entry.EstablishedWorldStateVersion.Value.ToString(
                            System.Globalization.CultureInfo.InvariantCulture),
                        entry.EstablishedSimulationTick.ToString(
                            System.Globalization.CultureInfo.InvariantCulture),
                        entry.LastUpdatedWorldStateVersion.Value.ToString(
                            System.Globalization.CultureInfo.InvariantCulture),
                        entry.LastUpdatedSimulationTick.ToString(
                            System.Globalization.CultureInfo.InvariantCulture))));

            return global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotPayload.From(
                    System.Text.Encoding.UTF8.GetBytes(text));
        }

        public global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<RelationshipWorldState> Decode(
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion version,
                global::AI.Sandbox.Engine.Core.Persistence
                    .SnapshotPayload payload)
        {
            if (version != CurrentSchemaVersion)
            {
                return Reject("Unsupported relationship schema version.");
            }

            try
            {
                var entries = new List<
                    global::AI.Sandbox.Engine.Core.Relationships
                        .RelationshipEntry<Disposition>>();
                var text = System.Text.Encoding.UTF8.GetString(
                    payload.ToArray());

                if (!string.IsNullOrEmpty(text))
                {
                    foreach (var line in text.Split('\n'))
                    {
                        var parts = line.Split('|');
                        if (parts.Length != 13 ||
                            !ParseEntityId(parts[0], out var targetId) ||
                            !uint.TryParse(
                                parts[1],
                                System.Globalization.NumberStyles.None,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out var revision) ||
                            !int.TryParse(
                                parts[2],
                                System.Globalization.NumberStyles.Integer,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out var regard) ||
                            !int.TryParse(
                                parts[3],
                                System.Globalization.NumberStyles.Integer,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out var familiarity) ||
                            !ParseChangeId(parts[4], out var changeId) ||
                            !int.TryParse(
                                parts[5],
                                System.Globalization.NumberStyles.Integer,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out var kindValue) ||
                            !ulong.TryParse(
                                parts[6],
                                System.Globalization.NumberStyles.None,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out var changeVersion) ||
                            !ulong.TryParse(
                                parts[7],
                                System.Globalization.NumberStyles.None,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out var changeTick) ||
                            !TryParseOptionalEntityId(
                                parts[8],
                                out var sourceEntityId) ||
                            !ulong.TryParse(
                                parts[9],
                                System.Globalization.NumberStyles.None,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out var establishedVersion) ||
                            !ulong.TryParse(
                                parts[10],
                                System.Globalization.NumberStyles.None,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out var establishedTick) ||
                            !ulong.TryParse(
                                parts[11],
                                System.Globalization.NumberStyles.None,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out var lastUpdatedVersion) ||
                            !ulong.TryParse(
                                parts[12],
                                System.Globalization.NumberStyles.None,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out var lastUpdatedTick))
                        {
                            return Reject("Malformed relationship payload.");
                        }

                        var kind =
                            (global::AI.Sandbox.Engine.Core.Relationships
                                .RelationshipChangeKind)kindValue;
                        var change =
                            global::AI.Sandbox.Engine.Core.Relationships
                                .RelationshipChangeReference.Restore(
                                    changeId,
                                    kind,
                                    OwnerId(),
                                    targetId,
                                    WorldId(),
                                    Version(changeVersion),
                                    changeTick,
                                    sourceEntityId,
                                    null,
                                    null,
                                    null,
                                    null,
                                    null,
                                    null);
                        entries.Add(
                            global::AI.Sandbox.Engine.Core.Relationships
                                .RelationshipEntry<Disposition>.Restore(
                                    targetId,
                                    revision,
                                    new Disposition(regard, familiarity),
                                    change,
                                    Version(establishedVersion),
                                    establishedTick,
                                    Version(lastUpdatedVersion),
                                    lastUpdatedTick));
                    }
                }

                var relationships =
                    global::AI.Sandbox.Engine.Core.Relationships
                        .RelationshipSet<Disposition>.Restore(
                            WorldId(),
                            OwnerId(),
                            entries);
                var entities =
                    global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
                        .FromActiveEntities(
                            new[]
                            {
                                OwnerId(),
                                TargetId(1),
                                TargetId(2),
                                SourceId(),
                            });
                var knowledge =
                    global::AI.Sandbox.Engine.Core.Knowledge
                        .KnowledgeSet<LocatedClaim>.Create(
                            WorldId(),
                            OwnerId());
                var memory =
                    global::AI.Sandbox.Engine.Core.Memory
                        .MemoryStore<Episode>.Create(
                            WorldId(),
                            OwnerId());
                var components =
                    new global::AI.Sandbox.Engine.Core.Components
                        .ComponentRegistryBuilder(entities)
                        .Add(OwnerId(), relationships)
                        .Add(OwnerId(), knowledge)
                        .Add(OwnerId(), memory)
                        .Build();

                return global::AI.Sandbox.Engine.Core.Persistence
                    .WorldStateDecodeDecision<RelationshipWorldState>.Accept(
                        new RelationshipWorldState(entities, components));
            }
            catch (ArgumentException exception)
            {
                return Reject(exception.Message);
            }
            catch (OverflowException exception)
            {
                return Reject(exception.Message);
            }
        }

        private static global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<RelationshipWorldState> Reject(
                string reason) =>
            global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateDecodeDecision<RelationshipWorldState>.Reject(reason);
    }

    private static global::AI.Sandbox.Engine.Core.Relationships
        .RelationshipChangeReference ExternalChange(
            int suffix,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> targetId,
            ulong version,
            ulong tick) =>
        global::AI.Sandbox.Engine.Core.Relationships
            .RelationshipChangeReference.Create(
                ChangeId(suffix),
                global::AI.Sandbox.Engine.Core.Relationships
                    .RelationshipChangeKind.External,
                ownerId,
                targetId,
                WorldId(),
                Version(version),
                tick);

    private static string Optional<TKind>(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>? value)
        where TKind : struct =>
        value.HasValue
            ? value.Value.ToString()
            : string.Empty;

    private static bool ParseEntityId(
        string text,
        out global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> value) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.TryParse(
                text,
                out value);

    private static bool ParseChangeId(
        string text,
        out global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipChangeIdKind> value) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipChangeIdKind>.TryParse(
                    text,
                    out value);

    private static bool TryParseOptionalEntityId(
        string text,
        out global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>? value)
    {
        if (string.IsNullOrEmpty(text))
        {
            value = null;
            return true;
        }

        if (ParseEntityId(text, out var parsed))
        {
            value = parsed;
            return true;
        }

        value = null;
        return false;
    }

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateVersion Version(ulong value) =>
        value == 0
            ? global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateVersion.Initial
            : global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateVersion.From(value);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000001900");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> OtherWorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000001901");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OwnerId() =>
        EntityId("019b0000-0000-7100-8100-000000000001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> TargetId(
            int suffix) =>
        EntityId(
            $"019b0000-0000-7100-8100-{suffix + 1:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> SourceId() =>
        EntityId("019b0000-0000-7100-8100-000000000004");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> EntityId(
            string text) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(text);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Relationships
            .RelationshipChangeIdKind> ChangeId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Relationships
                .RelationshipChangeIdKind>.Parse(
                    $"019b0000-0000-7200-8200-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Commands.CommandIdKind> CommandId(
            int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Commands.CommandIdKind>.Parse(
                $"019b0000-0000-7300-8300-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Knowledge
            .KnowledgeClaimIdKind> KnowledgeClaimId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeClaimIdKind>.Parse(
                    "019b0000-0000-7400-8400-000000000001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Knowledge
            .KnowledgeEvidenceIdKind> KnowledgeEvidenceId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Knowledge
                .KnowledgeEvidenceIdKind>.Parse(
                    "019b0000-0000-7500-8500-000000000001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Memory.MemoryIdKind> MemoryId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Memory.MemoryIdKind>.Parse(
                "019b0000-0000-7600-8600-000000000001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Memory
            .MemoryOriginIdKind> MemoryOriginId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Memory
                .MemoryOriginIdKind>.Parse(
                    "019b0000-0000-7700-8700-000000000001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Perception
            .PerceptionStimulusIdKind> PerceptionStimulusId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionStimulusIdKind>.Parse(
                    "019b0000-0000-7800-8800-000000000001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Perception
            .PerceptionChannelIdKind> PerceptionChannelId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionChannelIdKind>.Parse(
                    "019b0000-0000-7900-8900-000000000001");
}
