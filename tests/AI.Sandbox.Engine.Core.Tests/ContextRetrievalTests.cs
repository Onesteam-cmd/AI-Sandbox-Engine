namespace AI.Sandbox.Engine.Core.Tests;

public sealed class ContextRetrievalTests
{
    private readonly record struct FindRelevant(string Topic) :
        global::AI.Sandbox.Engine.Core.ContextRetrieval.IContextQuery;

    private abstract record AbstractQuery :
        global::AI.Sandbox.Engine.Core.ContextRetrieval.IContextQuery;

    private sealed record ConcreteAbstractQuery(string Topic) : AbstractQuery;

    private record OpenQuery(string Topic) :
        global::AI.Sandbox.Engine.Core.ContextRetrieval.IContextQuery;

    private readonly record struct TextContext(string Text) :
        global::AI.Sandbox.Engine.Core.ContextRetrieval.IContextItem;

    private abstract record AbstractItem :
        global::AI.Sandbox.Engine.Core.ContextRetrieval.IContextItem;

    private sealed record ConcreteAbstractItem(string Text) : AbstractItem;

    private record OpenItem(string Text) :
        global::AI.Sandbox.Engine.Core.ContextRetrieval.IContextItem;

    private readonly record struct KnowledgeReference(string Claim) :
        global::AI.Sandbox.Engine.Core.ContextRetrieval.IContextItem;

    private readonly record struct MemoryReference(string Episode) :
        global::AI.Sandbox.Engine.Core.ContextRetrieval.IContextItem;

    private readonly record struct RelationshipReference(string State) :
        global::AI.Sandbox.Engine.Core.ContextRetrieval.IContextItem;

    private readonly record struct EventReference(string Fact) :
        global::AI.Sandbox.Engine.Core.ContextRetrieval.IContextItem;

    private readonly record struct AdvanceValue(int Delta) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    private sealed record ContextWorldState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    [Xunit.Fact]
    public void QueryAndItemPayloadTypesMustBeExact()
    {
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextQueryEnvelope<AbstractQuery>.Create(
                    QueryId(1),
                    OwnerId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    Limit(2),
                    new ConcreteAbstractQuery("door")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextQueryEnvelope<OpenQuery>.Create(
                    QueryId(2),
                    OwnerId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    Limit(2),
                    new OpenQuery("door")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextItemEnvelope<AbstractItem>.Create(
                    ItemId(1),
                    RetrieverId(1),
                    OwnerId(),
                    WorldId(),
                    Relevance(5000),
                    new ConcreteAbstractItem("seen")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextItemEnvelope<OpenItem>.Create(
                    ItemId(2),
                    RetrieverId(1),
                    OwnerId(),
                    WorldId(),
                    Relevance(5000),
                    new OpenItem("seen")));

        var query = CreateQuery(CreateManager().Read(), 3, 2);
        var item = CreateItem(
            ItemId(3),
            RetrieverId(1),
            OwnerId(),
            WorldId(),
            5000,
            "seen");

        Xunit.Assert.Equal("topic", query.Payload.Topic);
        Xunit.Assert.Equal("seen", item.Payload.Text);
    }

    [Xunit.Fact]
    public void QueryEnvelopePreservesOwnerScopeAndBoundedLimit()
    {
        var snapshot = CreateManager().Read();
        var query = CreateQuery(snapshot, 10, 7);

        Xunit.Assert.Equal(QueryId(10), query.QueryId);
        Xunit.Assert.Equal(OwnerId(), query.OwnerEntityId);
        Xunit.Assert.Equal(snapshot.WorldId, query.WorldId);
        Xunit.Assert.Equal(snapshot.Version, query.WorldStateVersion);
        Xunit.Assert.Equal(snapshot.SimulationTick, query.SimulationTick);
        Xunit.Assert.Equal(7, query.ItemLimit.Value);
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => Limit(0));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => Limit(1025));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextQueryEnvelope<FindRelevant>.Create(
                    QueryId(11),
                    OwnerId(),
                    snapshot.WorldId,
                    snapshot.Version,
                    snapshot.SimulationTick,
                    default,
                    new FindRelevant("topic")));
    }

    [Xunit.Fact]
    public void RetrievedItemsAreBoundedAndDeterministicallyOrdered()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var retrieverId = RetrieverId(20);
        var retriever = new FixedRetriever(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalDecision<TextContext>.Retrieve(
                    new[]
                    {
                        CreateItem(
                            ItemId(3),
                            retrieverId,
                            OwnerId(),
                            snapshot.WorldId,
                            7000,
                            "third"),
                        CreateItem(
                            ItemId(2),
                            retrieverId,
                            OwnerId(),
                            snapshot.WorldId,
                            9000,
                            "second"),
                        CreateItem(
                            ItemId(1),
                            retrieverId,
                            OwnerId(),
                            snapshot.WorldId,
                            9000,
                            "first"),
                    }));
        var processor = CreateProcessor(manager, retrieverId, retriever);

        var result = processor.Retrieve(CreateQuery(snapshot, 20, 3));
        var decision = Xunit.Assert.IsType<
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalDecision<TextContext>>(result.Decision);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalStatus.Retrieved,
            result.Status);
        Xunit.Assert.True(result.WasRetrieved);
        Xunit.Assert.True(result.RetrieverWasInvoked);
        Xunit.Assert.Equal(1, retriever.CallCount);
        Xunit.Assert.Equal(3, decision.Items.Count);
        Xunit.Assert.Equal(ItemId(1), decision.Items[0].ItemId);
        Xunit.Assert.Equal(ItemId(2), decision.Items[1].ItemId);
        Xunit.Assert.Equal(ItemId(3), decision.Items[2].ItemId);
        Xunit.Assert.Equal(snapshot.Version, manager.Read().Version);
    }

    [Xunit.Fact]
    public void EmptyAndRejectedRetrievalAreExplicit()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var retrieverId = RetrieverId(30);
        var emptyRetriever = new FixedRetriever(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalDecision<TextContext>.Empty());
        var rejectedRetriever = new FixedRetriever(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalDecision<TextContext>.Reject(
                    RejectionCode("context.blocked")));

        var empty = CreateProcessor(
            manager,
            retrieverId,
            emptyRetriever).Retrieve(CreateQuery(snapshot, 30, 4));
        var rejected = CreateProcessor(
            manager,
            retrieverId,
            rejectedRetriever).Retrieve(CreateQuery(snapshot, 31, 4));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalStatus.Empty,
            empty.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalStatus.Rejected,
            rejected.Status);
        Xunit.Assert.Equal(
            "context.blocked",
            rejected.Decision!.RejectionCode.Value);
        Xunit.Assert.Throws<InvalidOperationException>(
            () => _ = empty.Decision!.Items);
        Xunit.Assert.Equal(1, emptyRetriever.CallCount);
        Xunit.Assert.Equal(1, rejectedRetriever.CallCount);
    }

    [Xunit.Fact]
    public void StaleScopeIsRejectedBeforeRetrieverRuns()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var retriever = new FixedRetriever(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalDecision<TextContext>.Empty());
        var processor = CreateProcessor(manager, RetrieverId(40), retriever);
        var wrongWorld = global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextQueryEnvelope<FindRelevant>.Create(
                QueryId(40),
                OwnerId(),
                OtherWorldId(),
                snapshot.Version,
                snapshot.SimulationTick,
                Limit(3),
                new FindRelevant("topic"));
        var wrongVersion = global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextQueryEnvelope<FindRelevant>.Create(
                QueryId(41),
                OwnerId(),
                snapshot.WorldId,
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateVersion.From(
                        checked(snapshot.Version.Value + 1)),
                snapshot.SimulationTick,
                Limit(3),
                new FindRelevant("topic"));
        var wrongTick = global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextQueryEnvelope<FindRelevant>.Create(
                QueryId(42),
                OwnerId(),
                snapshot.WorldId,
                snapshot.Version,
                checked(snapshot.SimulationTick + 1),
                Limit(3),
                new FindRelevant("topic"));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalStatus.WorldMismatch,
            processor.Retrieve(wrongWorld).Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalStatus.VersionConflict,
            processor.Retrieve(wrongVersion).Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalStatus.SimulationTickMismatch,
            processor.Retrieve(wrongTick).Status);
        Xunit.Assert.Equal(0, retriever.CallCount);
    }

    [Xunit.Fact]
    public void ResultScopeAndItemLimitAreValidated()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var retrieverId = RetrieverId(50);

        var wrongWorld = RetrieveWithSingleItem(
            manager,
            snapshot,
            retrieverId,
            CreateItem(
                ItemId(50),
                retrieverId,
                OwnerId(),
                OtherWorldId(),
                5000,
                "wrong-world"),
            3,
            50);
        var wrongOwner = RetrieveWithSingleItem(
            manager,
            snapshot,
            retrieverId,
            CreateItem(
                ItemId(51),
                retrieverId,
                OtherOwnerId(),
                snapshot.WorldId,
                5000,
                "wrong-owner"),
            3,
            51);
        var wrongRetriever = RetrieveWithSingleItem(
            manager,
            snapshot,
            retrieverId,
            CreateItem(
                ItemId(52),
                RetrieverId(99),
                OwnerId(),
                snapshot.WorldId,
                5000,
                "wrong-retriever"),
            3,
            52);
        var overLimitRetriever = new FixedRetriever(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalDecision<TextContext>.Retrieve(
                    new[]
                    {
                        CreateItem(
                            ItemId(53),
                            retrieverId,
                            OwnerId(),
                            snapshot.WorldId,
                            5000,
                            "one"),
                        CreateItem(
                            ItemId(54),
                            retrieverId,
                            OwnerId(),
                            snapshot.WorldId,
                            4000,
                            "two"),
                    }));
        var overLimit = CreateProcessor(
            manager,
            retrieverId,
            overLimitRetriever).Retrieve(CreateQuery(snapshot, 53, 1));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalStatus.ResultWorldMismatch,
            wrongWorld.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalStatus.ResultOwnerMismatch,
            wrongOwner.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalStatus.ResultRetrieverMismatch,
            wrongRetriever.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalStatus.ItemLimitExceeded,
            overLimit.Status);
    }

    [Xunit.Fact]
    public void AuthorityChangeDuringRetrievalDiscardsDecisionWithoutRetry()
    {
        var manager = CreateManager();
        var runtime = CreateRuntime(manager);
        var retrieverId = RetrieverId(60);
        var retriever = new MutatingRetriever(runtime, retrieverId);
        var processor = CreateProcessor(manager, retrieverId, retriever);
        var before = manager.Read();

        var result = processor.Retrieve(CreateQuery(before, 60, 2));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalStatus.VersionConflict,
            result.Status);
        Xunit.Assert.True(result.RetrieverWasInvoked);
        Xunit.Assert.False(result.HasStableDecision);
        Xunit.Assert.Null(result.Decision);
        Xunit.Assert.Equal(1, retriever.CallCount);
        Xunit.Assert.Equal(1, manager.Read().State.Value);
    }

    [Xunit.Fact]
    public void RetrieverExceptionPropagatesWithoutRetry()
    {
        var manager = CreateManager();
        var retriever = new ThrowingRetriever();
        var processor = CreateProcessor(manager, RetrieverId(70), retriever);
        var snapshot = manager.Read();

        var exception = Xunit.Assert.Throws<InvalidOperationException>(
            () => processor.Retrieve(CreateQuery(snapshot, 70, 2)));

        Xunit.Assert.Equal("retriever failure", exception.Message);
        Xunit.Assert.Equal(1, retriever.CallCount);
        Xunit.Assert.Equal(snapshot.Version, manager.Read().Version);
    }

    [Xunit.Fact]
    public void SourceSpecificPayloadsRemainExactWithoutClosedSourceEnum()
    {
        var retrieverId = RetrieverId(80);
        var worldId = WorldId();
        var ownerId = OwnerId();

        var knowledge = global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextItemEnvelope<KnowledgeReference>.Create(
                ItemId(80),
                retrieverId,
                ownerId,
                worldId,
                Relevance(9000),
                new KnowledgeReference("claim"));
        var memory = global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextItemEnvelope<MemoryReference>.Create(
                ItemId(81),
                retrieverId,
                ownerId,
                worldId,
                Relevance(8000),
                new MemoryReference("episode"));
        var relationship = global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextItemEnvelope<RelationshipReference>.Create(
                ItemId(82),
                retrieverId,
                ownerId,
                worldId,
                Relevance(7000),
                new RelationshipReference("directed-state"));
        var eventItem = global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextItemEnvelope<EventReference>.Create(
                ItemId(83),
                retrieverId,
                ownerId,
                worldId,
                Relevance(6000),
                new EventReference("completed-fact"));

        Xunit.Assert.Equal("claim", knowledge.Payload.Claim);
        Xunit.Assert.Equal("episode", memory.Payload.Episode);
        Xunit.Assert.Equal("directed-state", relationship.Payload.State);
        Xunit.Assert.Equal("completed-fact", eventItem.Payload.Fact);
        Xunit.Assert.Equal(ownerId, knowledge.OwnerEntityId);
        Xunit.Assert.Equal(worldId, eventItem.WorldId);
    }

    private static global::AI.Sandbox.Engine.Core.ContextRetrieval
        .ContextRetrievalResult<FindRelevant, TextContext>
        RetrieveWithSingleItem(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<ContextWorldState> manager,
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateSnapshot<ContextWorldState> snapshot,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.ContextRetrieval
                    .ContextRetrieverIdKind> retrieverId,
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextItemEnvelope<TextContext> item,
            int limit,
            int querySuffix)
    {
        var retriever = new FixedRetriever(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalDecision<TextContext>.Retrieve(
                    new[] { item }));

        return CreateProcessor(
            manager,
            retrieverId,
            retriever).Retrieve(CreateQuery(snapshot, querySuffix, limit));
    }

    private static global::AI.Sandbox.Engine.Core.ContextRetrieval
        .ContextRetrievalProcessor<ContextWorldState, FindRelevant, TextContext>
        CreateProcessor(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<ContextWorldState> manager,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.ContextRetrieval
                    .ContextRetrieverIdKind> retrieverId,
            global::AI.Sandbox.Engine.Core.ContextRetrieval.IContextRetriever<
                ContextWorldState,
                FindRelevant,
                TextContext> retriever) =>
        global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextRetrievalProcessor<
                ContextWorldState,
                FindRelevant,
                TextContext>.Create(
                    manager,
                    retrieverId,
                    retriever);

    private static global::AI.Sandbox.Engine.Core.ContextRetrieval
        .ContextQueryEnvelope<FindRelevant> CreateQuery(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateSnapshot<ContextWorldState> snapshot,
            int querySuffix,
            int limit) =>
        global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextQueryEnvelope<FindRelevant>.Create(
                QueryId(querySuffix),
                OwnerId(),
                snapshot.WorldId,
                snapshot.Version,
                snapshot.SimulationTick,
                Limit(limit),
                new FindRelevant("topic"));

    private static global::AI.Sandbox.Engine.Core.ContextRetrieval
        .ContextItemEnvelope<TextContext> CreateItem(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.ContextRetrieval
                    .ContextItemIdKind> itemId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.ContextRetrieval
                    .ContextRetrieverIdKind> retrieverId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
            int relevance,
            string text) =>
        global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextItemEnvelope<TextContext>.Create(
                itemId,
                retrieverId,
                ownerId,
                worldId,
                Relevance(relevance),
                new TextContext(text));

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<ContextWorldState> CreateManager() =>
        global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<ContextWorldState>.Create(
                WorldId(),
                new ContextWorldState(0));

    private static global::AI.Sandbox.Engine.Core.Runtime
        .RuntimeOrchestrator<ContextWorldState> CreateRuntime(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<ContextWorldState> manager) =>
        new global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestratorBuilder<ContextWorldState>()
            .AddCommandHandler(new AdvanceValueHandler())
            .Build(manager);

    private sealed class AdvanceValueHandler :
        global::AI.Sandbox.Engine.Core.Commands.ICommandHandler<
            ContextWorldState,
            AdvanceValue>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<ContextWorldState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<ContextWorldState, AdvanceValue> context)
        {
            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<ContextWorldState>.Accept(
                    context.Snapshot.State with
                    {
                        Value = checked(
                            context.Snapshot.State.Value +
                            context.Envelope.Payload.Delta),
                    });
        }
    }

    private sealed class FixedRetriever :
        global::AI.Sandbox.Engine.Core.ContextRetrieval.IContextRetriever<
            ContextWorldState,
            FindRelevant,
            TextContext>
    {
        private readonly global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextRetrievalDecision<TextContext> decision;

        public FixedRetriever(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalDecision<TextContext> decision)
        {
            this.decision = decision;
        }

        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextRetrievalDecision<TextContext> Retrieve(
                global::AI.Sandbox.Engine.Core.ContextRetrieval
                    .ContextRetrievalContext<ContextWorldState, FindRelevant>
                        context)
        {
            _ = context;
            CallCount = checked(CallCount + 1);
            return decision;
        }
    }

    private sealed class MutatingRetriever :
        global::AI.Sandbox.Engine.Core.ContextRetrieval.IContextRetriever<
            ContextWorldState,
            FindRelevant,
            TextContext>
    {
        private readonly global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestrator<ContextWorldState> runtime;
        private readonly global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrieverIdKind> retrieverId;

        public MutatingRetriever(
            global::AI.Sandbox.Engine.Core.Runtime
                .RuntimeOrchestrator<ContextWorldState> runtime,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.ContextRetrieval
                    .ContextRetrieverIdKind> retrieverId)
        {
            this.runtime = runtime;
            this.retrieverId = retrieverId;
        }

        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextRetrievalDecision<TextContext> Retrieve(
                global::AI.Sandbox.Engine.Core.ContextRetrieval
                    .ContextRetrievalContext<ContextWorldState, FindRelevant>
                        context)
        {
            CallCount = checked(CallCount + 1);
            var snapshot = runtime.Read();
            var result = runtime.ExecuteCommand(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandEnvelope<AdvanceValue>.Create(
                        CommandId(900 + CallCount),
                        snapshot.WorldId,
                        snapshot.Version,
                        snapshot.SimulationTick,
                        new AdvanceValue(1)));

            if (!result.WasCommitted)
            {
                throw new InvalidOperationException(
                    "The conflict test could not advance authority.");
            }

            return global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalDecision<TextContext>.Retrieve(
                    new[]
                    {
                        CreateItem(
                            ItemId(900 + CallCount),
                            retrieverId,
                            context.Query.OwnerEntityId,
                            context.Query.WorldId,
                            5000,
                            "discarded"),
                    });
        }
    }

    private sealed class ThrowingRetriever :
        global::AI.Sandbox.Engine.Core.ContextRetrieval.IContextRetriever<
            ContextWorldState,
            FindRelevant,
            TextContext>
    {
        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextRetrievalDecision<TextContext> Retrieve(
                global::AI.Sandbox.Engine.Core.ContextRetrieval
                    .ContextRetrievalContext<ContextWorldState, FindRelevant>
                        context)
        {
            _ = context;
            CallCount = checked(CallCount + 1);
            throw new InvalidOperationException("retriever failure");
        }
    }

    private static global::AI.Sandbox.Engine.Core.ContextRetrieval
        .ContextItemLimit Limit(int value) =>
        global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextItemLimit.From(value);

    private static global::AI.Sandbox.Engine.Core.ContextRetrieval
        .ContextRelevance Relevance(int value) =>
        global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextRelevance.FromBasisPoints(value);

    private static global::AI.Sandbox.Engine.Core.ContextRetrieval
        .ContextRejectionCode RejectionCode(string text) =>
        global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextRejectionCode.Parse(text);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000002100");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> OtherWorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000002101");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OwnerId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(
                "019b0000-0000-7100-8100-000000002100");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OtherOwnerId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(
                "019b0000-0000-7100-8100-000000002101");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.ContextRetrieval.ContextQueryIdKind>
            QueryId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextQueryIdKind>.Parse(
                    $"019b0000-0000-7200-8200-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.ContextRetrieval.ContextRetrieverIdKind>
            RetrieverId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrieverIdKind>.Parse(
                    $"019b0000-0000-7300-8300-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.ContextRetrieval.ContextItemIdKind>
            ItemId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextItemIdKind>.Parse(
                    $"019b0000-0000-7400-8400-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Commands.CommandIdKind>
            CommandId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Commands.CommandIdKind>.Parse(
                $"019b0000-0000-7500-8500-{suffix:D12}");
}
