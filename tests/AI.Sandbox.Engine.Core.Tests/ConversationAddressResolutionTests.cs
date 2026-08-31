namespace AI.Sandbox.Engine.Core.Tests;

public sealed class ConversationAddressResolutionTests
{
    private readonly record struct Topic(string Value) :
        global::AI.Sandbox.Engine.Core.Conversation.IConversationTopic;

    private abstract record AbstractTopic :
        global::AI.Sandbox.Engine.Core.Conversation.IConversationTopic;

    private sealed record ConcreteAbstractTopic(string Value) : AbstractTopic;

    private record OpenTopic(string Value) :
        global::AI.Sandbox.Engine.Core.Conversation.IConversationTopic;

    private readonly record struct AddressQuery(string Utterance) :
        global::AI.Sandbox.Engine.Core.Conversation.IAddressQuery;

    private abstract record AbstractQuery :
        global::AI.Sandbox.Engine.Core.Conversation.IAddressQuery;

    private sealed record ConcreteAbstractQuery(string Value) : AbstractQuery;

    private record OpenQuery(string Value) :
        global::AI.Sandbox.Engine.Core.Conversation.IAddressQuery;

    private readonly record struct AdvanceValue(int Delta) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    private sealed record ConversationWorldState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    [Xunit.Fact]
    public void PayloadTypesAndValueObjectsMustBeExactAndBounded()
    {
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Conversation
                .ConversationState<AbstractTopic>.Start(
                    WorldId(),
                    ConversationId(1),
                    new[] { EntityId(1), EntityId(2) },
                    new ConcreteAbstractTopic("topic")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Conversation
                .ConversationState<OpenTopic>.Start(
                    WorldId(),
                    ConversationId(2),
                    new[] { EntityId(1), EntityId(2) },
                    new OpenTopic("topic")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionRequestEnvelope<AbstractQuery>.Create(
                    ResolutionId(1),
                    ResolverId(1),
                    ConversationId(1),
                    EntityId(1),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    global::AI.Sandbox.Engine.Core.Conversation
                        .ConversationRevision.Initial,
                    new ConcreteAbstractQuery("hello")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionRequestEnvelope<OpenQuery>.Create(
                    ResolutionId(2),
                    ResolverId(1),
                    ConversationId(1),
                    EntityId(1),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    global::AI.Sandbox.Engine.Core.Conversation
                        .ConversationRevision.Initial,
                    new OpenQuery("hello")));

        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.Conversation
                .ConversationRevision.From(0));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.Conversation
                .ConversationTurnNumber.From(0));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionConfidence.FromBasisPoints(0));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Conversation
                .AddressRejectionCode.From("Address.Blocked"));

        Xunit.Assert.Equal(
            2,
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationRevision.Initial.Next().Value);
        Xunit.Assert.Equal(
            2,
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationTurnNumber.First.Next().Value);
        Xunit.Assert.Equal(
            8500,
            Confidence(8500).BasisPoints);
        Xunit.Assert.Equal(
            "address.blocked",
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressRejectionCode.From("address.blocked").Value);
    }

    [Xunit.Fact]
    public void ConversationStateSortsRosterAndPreservesScope()
    {
        var source = new[] { EntityId(3), EntityId(1), EntityId(2) };
        var state = global::AI.Sandbox.Engine.Core.Conversation
            .ConversationState<Topic>.Start(
                WorldId(),
                ConversationId(10),
                source,
                new Topic("door"));

        source[0] = EntityId(9);

        Xunit.Assert.Equal(WorldId(), state.WorldId);
        Xunit.Assert.Equal(ConversationId(10), state.ConversationId);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationRevision.Initial,
            state.Revision);
        Xunit.Assert.Equal(
            new[] { EntityId(1), EntityId(2), EntityId(3) },
            state.ParticipantEntityIds.ToArray());
        Xunit.Assert.Equal("door", state.Topic.Value);
        Xunit.Assert.Null(state.LastTurn);
        Xunit.Assert.False(state.IsClosed);
        Xunit.Assert.True(state.ContainsParticipant(EntityId(2)));
        Xunit.Assert.False(state.ContainsParticipant(EntityId(9)));

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Conversation
                .ConversationState<Topic>.Start(
                    WorldId(),
                    ConversationId(11),
                    new[] { EntityId(1), EntityId(1) },
                    new Topic("duplicate")));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.Conversation
                .ConversationState<Topic>.Start(
                    WorldId(),
                    ConversationId(12),
                    new[] { EntityId(1) },
                    new Topic("small")));
    }

    [Xunit.Fact]
    public void TurnsUseOptimisticRevisionAndValidateAudience()
    {
        var state = CreateConversation(20, EntityId(1), EntityId(2), EntityId(3));
        var audience = global::AI.Sandbox.Engine.Core.Conversation
            .AddressAudience.SpecificParticipants(
                new[] { EntityId(3), EntityId(2) });
        var turn = global::AI.Sandbox.Engine.Core.Conversation
            .ConversationTurn.Create(
                global::AI.Sandbox.Engine.Core.Conversation
                    .ConversationTurnNumber.First,
                EntityId(1),
                audience);

        var recorded = state.RecordTurn(state.Revision, turn);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationMutationStatus.TurnRecorded,
            recorded.Status);
        Xunit.Assert.True(recorded.WasChanged);
        Xunit.Assert.Equal(2, recorded.State.Revision.Value);
        Xunit.Assert.Equal(
            new[] { EntityId(2), EntityId(3) },
            recorded.State.LastTurn!.Audience.TargetEntityIds.ToArray());

        var stale = recorded.State.RecordTurn(state.Revision, turn);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationMutationStatus.RevisionConflict,
            stale.Status);

        var wrongSequence = recorded.State.RecordTurn(
            recorded.State.Revision,
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationTurn.Create(
                    global::AI.Sandbox.Engine.Core.Conversation
                        .ConversationTurnNumber.From(3),
                    EntityId(2),
                    global::AI.Sandbox.Engine.Core.Conversation
                        .AddressAudience.None()));
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationMutationStatus.TurnSequenceConflict,
            wrongSequence.Status);

        var outsiderSpeaker = state.RecordTurn(
            state.Revision,
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationTurn.Create(
                    global::AI.Sandbox.Engine.Core.Conversation
                        .ConversationTurnNumber.First,
                    EntityId(9),
                    global::AI.Sandbox.Engine.Core.Conversation
                        .AddressAudience.None()));
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationMutationStatus.SpeakerNotParticipant,
            outsiderSpeaker.Status);

        var selfAddress = state.RecordTurn(
            state.Revision,
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationTurn.Create(
                    global::AI.Sandbox.Engine.Core.Conversation
                        .ConversationTurnNumber.First,
                    EntityId(1),
                    global::AI.Sandbox.Engine.Core.Conversation
                        .AddressAudience.SpecificParticipants(
                            new[] { EntityId(1) })));
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationMutationStatus.AudienceInvalid,
            selfAddress.Status);

        var incompleteAll = state.RecordTurn(
            state.Revision,
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationTurn.Create(
                    global::AI.Sandbox.Engine.Core.Conversation
                        .ConversationTurnNumber.First,
                    EntityId(1),
                    global::AI.Sandbox.Engine.Core.Conversation
                        .AddressAudience.AllParticipants(
                            new[] { EntityId(2) })));
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationMutationStatus.AudienceInvalid,
            incompleteAll.Status);
    }

    [Xunit.Fact]
    public void TopicRevisionAndClosureRemainExplicit()
    {
        var state = CreateConversation(30, EntityId(1), EntityId(2));

        var unchanged = state.ReviseTopic(state.Revision, state.Topic);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationMutationStatus.Unchanged,
            unchanged.Status);
        Xunit.Assert.False(unchanged.WasChanged);

        var revised = state.ReviseTopic(
            state.Revision,
            new Topic("new-topic"));
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationMutationStatus.TopicRevised,
            revised.Status);
        Xunit.Assert.Equal("new-topic", revised.State.Topic.Value);
        Xunit.Assert.Equal(2, revised.State.Revision.Value);

        var closed = revised.State.Close(revised.State.Revision);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationMutationStatus.Closed,
            closed.Status);
        Xunit.Assert.True(closed.State.IsClosed);
        Xunit.Assert.Equal(3, closed.State.Revision.Value);

        var reviseClosed = closed.State.ReviseTopic(
            closed.State.Revision,
            new Topic("blocked"));
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationMutationStatus.ConversationClosed,
            reviseClosed.Status);

        var closeAgain = closed.State.Close(closed.State.Revision);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationMutationStatus.Unchanged,
            closeAgain.Status);
    }

    [Xunit.Fact]
    public void ResolverReturnsNoneSpecificGroupAndAllExactlyOnce()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var conversation = CreateConversation(
            40,
            EntityId(1),
            EntityId(2),
            EntityId(3),
            EntityId(4));

        var audiences = new[]
        {
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressAudience.None(),
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressAudience.SpecificParticipants(
                    new[] { EntityId(2) }),
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressAudience.SpecificParticipants(
                    new[] { EntityId(2), EntityId(3) }),
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressAudience.AllParticipants(
                    new[] { EntityId(2), EntityId(3), EntityId(4) }),
        };

        for (var index = 0; index < audiences.Length; index++)
        {
            var resolver = new FixedResolver(
                ResolverId(40 + index),
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolutionDecision.Resolve(
                        audiences[index],
                        Confidence(9000 - index)));
            var processor = CreateProcessor(manager, resolver);
            var result = processor.Resolve(
                CreateRequest(
                    snapshot,
                    conversation,
                    resolver.ResolverId,
                    40 + index,
                    EntityId(1)),
                conversation);

            Xunit.Assert.Equal(
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolutionStatus.Resolved,
                result.Status);
            Xunit.Assert.True(result.ResolverWasInvoked);
            Xunit.Assert.True(result.HasStableDecision);
            Xunit.Assert.Same(
                audiences[index],
                result.Decision!.Audience);
            Xunit.Assert.Equal(1, resolver.CallCount);
        }
    }

    [Xunit.Fact]
    public void PreflightMismatchesSkipResolver()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var resolver = new FixedResolver(
            ResolverId(50),
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionDecision.Resolve(
                    global::AI.Sandbox.Engine.Core.Conversation
                        .AddressAudience.None(),
                    Confidence(7000)));
        var processor = CreateProcessor(manager, resolver);
        var conversation = CreateConversation(
            50,
            EntityId(1),
            EntityId(2),
            EntityId(3));

        var cases = new[]
        {
            processor.Resolve(
                CreateRequest(
                    snapshot,
                    conversation,
                    ResolverId(999),
                    501,
                    EntityId(1)),
                conversation),
            processor.Resolve(
                CreateRequest(
                    snapshot,
                    conversation,
                    resolver.ResolverId,
                    502,
                    EntityId(1),
                    worldId: OtherWorldId()),
                conversation),
            processor.Resolve(
                CreateRequest(
                    snapshot,
                    conversation,
                    resolver.ResolverId,
                    503,
                    EntityId(1),
                    version: global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.From(
                            checked(snapshot.Version.Value + 1))),
                conversation),
            processor.Resolve(
                CreateRequest(
                    snapshot,
                    conversation,
                    resolver.ResolverId,
                    504,
                    EntityId(1),
                    simulationTick: checked(snapshot.SimulationTick + 1)),
                conversation),
            processor.Resolve(
                CreateRequest(
                    snapshot,
                    conversation,
                    resolver.ResolverId,
                    505,
                    EntityId(1),
                    conversationId: ConversationId(999)),
                conversation),
            processor.Resolve(
                CreateRequest(
                    snapshot,
                    conversation,
                    resolver.ResolverId,
                    506,
                    EntityId(1),
                    expectedRevision:
                        global::AI.Sandbox.Engine.Core.Conversation
                            .ConversationRevision.From(99)),
                conversation),
            processor.Resolve(
                CreateRequest(
                    snapshot,
                    conversation,
                    resolver.ResolverId,
                    507,
                    EntityId(9)),
                conversation),
        };

        Xunit.Assert.Equal(
            new[]
            {
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolutionStatus.ResolverMismatch,
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolutionStatus.WorldMismatch,
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolutionStatus.VersionConflict,
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolutionStatus.SimulationTickMismatch,
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolutionStatus.ConversationMismatch,
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolutionStatus.ConversationRevisionConflict,
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolutionStatus.SpeakerNotParticipant,
            },
            cases.Select(result => result.Status).ToArray());

        var otherWorldConversation =
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationState<Topic>.Start(
                    OtherWorldId(),
                    conversation.ConversationId,
                    conversation.ParticipantEntityIds,
                    conversation.Topic);
        var otherWorldResult = processor.Resolve(
            CreateRequest(
                snapshot,
                conversation,
                resolver.ResolverId,
                508,
                EntityId(1)),
            otherWorldConversation);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionStatus.ConversationWorldMismatch,
            otherWorldResult.Status);

        var closed = conversation.Close(conversation.Revision).State;
        var closedResult = processor.Resolve(
            CreateRequest(
                snapshot,
                closed,
                resolver.ResolverId,
                509,
                EntityId(1)),
            closed);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionStatus.ConversationClosed,
            closedResult.Status);

        Xunit.Assert.Equal(0, resolver.CallCount);
    }

    [Xunit.Fact]
    public void ResolverRejectionAndInvalidAudienceRemainExplicit()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var conversation = CreateConversation(
            60,
            EntityId(1),
            EntityId(2),
            EntityId(3));

        var rejectedResolver = new FixedResolver(
            ResolverId(60),
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionDecision.Reject(
                    global::AI.Sandbox.Engine.Core.Conversation
                        .AddressRejectionCode.From("address.ambiguous")));
        var rejected = CreateProcessor(manager, rejectedResolver).Resolve(
            CreateRequest(
                snapshot,
                conversation,
                rejectedResolver.ResolverId,
                60,
                EntityId(1)),
            conversation);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionStatus.Rejected,
            rejected.Status);
        Xunit.Assert.True(rejected.HasStableDecision);
        Xunit.Assert.Equal(
            "address.ambiguous",
            rejected.Decision!.RejectionCode.Value);

        var invalidResolver = new FixedResolver(
            ResolverId(61),
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionDecision.Resolve(
                    global::AI.Sandbox.Engine.Core.Conversation
                        .AddressAudience.SpecificParticipants(
                            new[] { EntityId(9) }),
                    Confidence(6000)));
        var invalid = CreateProcessor(manager, invalidResolver).Resolve(
            CreateRequest(
                snapshot,
                conversation,
                invalidResolver.ResolverId,
                61,
                EntityId(1)),
            conversation);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionStatus.AudienceInvalid,
            invalid.Status);
        Xunit.Assert.True(invalid.ResolverWasInvoked);
        Xunit.Assert.False(invalid.HasStableDecision);
        Xunit.Assert.Null(invalid.Decision);
        Xunit.Assert.Equal(1, rejectedResolver.CallCount);
        Xunit.Assert.Equal(1, invalidResolver.CallCount);
    }

    [Xunit.Fact]
    public void AuthorityChangeDiscardsDecisionWithoutRetry()
    {
        var manager = CreateManager();
        var runtime = CreateRuntime(manager);
        var snapshot = manager.Read();
        var conversation = CreateConversation(
            70,
            EntityId(1),
            EntityId(2));
        var resolver = new MutatingResolver(runtime, ResolverId(70));
        var processor = CreateProcessor(manager, resolver);

        var result = processor.Resolve(
            CreateRequest(
                snapshot,
                conversation,
                resolver.ResolverId,
                70,
                EntityId(1)),
            conversation);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionStatus.AuthorityChanged,
            result.Status);
        Xunit.Assert.True(result.ResolverWasInvoked);
        Xunit.Assert.False(result.HasStableDecision);
        Xunit.Assert.Null(result.Decision);
        Xunit.Assert.Equal(1, resolver.CallCount);
        Xunit.Assert.Equal(1, manager.Read().State.Value);
    }

    [Xunit.Fact]
    public void ResolverExceptionPropagatesWithoutRetry()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var conversation = CreateConversation(
            80,
            EntityId(1),
            EntityId(2));
        var resolver = new ThrowingResolver(ResolverId(80));
        var processor = CreateProcessor(manager, resolver);

        var exception = Xunit.Assert.Throws<InvalidOperationException>(
            () => processor.Resolve(
                CreateRequest(
                    snapshot,
                    conversation,
                    resolver.ResolverId,
                    80,
                    EntityId(1)),
                conversation));

        Xunit.Assert.Equal("resolver failure", exception.Message);
        Xunit.Assert.Equal(1, resolver.CallCount);
        Xunit.Assert.Equal(snapshot.Version, manager.Read().Version);
    }

    private static global::AI.Sandbox.Engine.Core.Conversation
        .ConversationState<Topic> CreateConversation(
            int suffix,
            params global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>[]
                participants) =>
        global::AI.Sandbox.Engine.Core.Conversation
            .ConversationState<Topic>.Start(
                WorldId(),
                ConversationId(suffix),
                participants,
                new Topic("topic"));

    private static global::AI.Sandbox.Engine.Core.Conversation
        .AddressResolutionProcessor<
            ConversationWorldState,
            AddressQuery,
            Topic> CreateProcessor(
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateManager<ConversationWorldState> manager,
                global::AI.Sandbox.Engine.Core.Conversation.IAddressResolver<
                    ConversationWorldState,
                    AddressQuery,
                    Topic> resolver) =>
        global::AI.Sandbox.Engine.Core.Conversation
            .AddressResolutionProcessor<
                ConversationWorldState,
                AddressQuery,
                Topic>.Create(manager, resolver);

    private static global::AI.Sandbox.Engine.Core.Conversation
        .AddressResolutionRequestEnvelope<AddressQuery> CreateRequest(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateSnapshot<ConversationWorldState> snapshot,
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationState<Topic> conversation,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolverIdKind> resolverId,
            int suffix,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
                speakerEntityId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>?
                worldId = null,
            global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion?
                version = null,
            ulong? simulationTick = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Conversation
                    .ConversationIdKind>? conversationId = null,
            global::AI.Sandbox.Engine.Core.Conversation.ConversationRevision?
                expectedRevision = null) =>
        global::AI.Sandbox.Engine.Core.Conversation
            .AddressResolutionRequestEnvelope<AddressQuery>.Create(
                ResolutionId(suffix),
                resolverId,
                conversationId ?? conversation.ConversationId,
                speakerEntityId,
                worldId ?? snapshot.WorldId,
                version ?? snapshot.Version,
                simulationTick ?? snapshot.SimulationTick,
                expectedRevision ?? conversation.Revision,
                new AddressQuery("hello"));

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<ConversationWorldState> CreateManager() =>
        global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<ConversationWorldState>.Create(
                WorldId(),
                new ConversationWorldState(0));

    private static global::AI.Sandbox.Engine.Core.Runtime
        .RuntimeOrchestrator<ConversationWorldState> CreateRuntime(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<ConversationWorldState> manager) =>
        new global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestratorBuilder<ConversationWorldState>()
            .AddCommandHandler(new AdvanceValueHandler())
            .Build(manager);

    private sealed class AdvanceValueHandler :
        global::AI.Sandbox.Engine.Core.Commands.ICommandHandler<
            ConversationWorldState,
            AdvanceValue>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<ConversationWorldState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<ConversationWorldState, AdvanceValue>
                    context)
        {
            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<ConversationWorldState>.Accept(
                    context.Snapshot.State with
                    {
                        Value = checked(
                            context.Snapshot.State.Value +
                            context.Envelope.Payload.Delta),
                    });
        }
    }

    private sealed class FixedResolver :
        global::AI.Sandbox.Engine.Core.Conversation.IAddressResolver<
            ConversationWorldState,
            AddressQuery,
            Topic>
    {
        private readonly global::AI.Sandbox.Engine.Core.Conversation
            .AddressResolutionDecision decision;

        public FixedResolver(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolverIdKind> resolverId,
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionDecision decision)
        {
            ResolverId = resolverId;
            this.decision = decision;
        }

        public global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Conversation.AddressResolverIdKind>
            ResolverId { get; }

        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Conversation
            .AddressResolutionDecision Resolve(
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolutionContext<
                        ConversationWorldState,
                        AddressQuery,
                        Topic> context)
        {
            CallCount = checked(CallCount + 1);
            Xunit.Assert.Equal(
                context.Conversation.ParticipantEntityIds.Count - 1,
                context.CandidateEntityIds.Count);
            return decision;
        }
    }

    private sealed class MutatingResolver :
        global::AI.Sandbox.Engine.Core.Conversation.IAddressResolver<
            ConversationWorldState,
            AddressQuery,
            Topic>
    {
        private readonly global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestrator<ConversationWorldState> runtime;

        public MutatingResolver(
            global::AI.Sandbox.Engine.Core.Runtime
                .RuntimeOrchestrator<ConversationWorldState> runtime,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolverIdKind> resolverId)
        {
            this.runtime = runtime;
            ResolverId = resolverId;
        }

        public global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Conversation.AddressResolverIdKind>
            ResolverId { get; }

        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Conversation
            .AddressResolutionDecision Resolve(
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolutionContext<
                        ConversationWorldState,
                        AddressQuery,
                        Topic> context)
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

            return global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionDecision.Resolve(
                    global::AI.Sandbox.Engine.Core.Conversation
                        .AddressAudience.None(),
                    Confidence(5000));
        }
    }

    private sealed class ThrowingResolver :
        global::AI.Sandbox.Engine.Core.Conversation.IAddressResolver<
            ConversationWorldState,
            AddressQuery,
            Topic>
    {
        public ThrowingResolver(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolverIdKind> resolverId)
        {
            ResolverId = resolverId;
        }

        public global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Conversation.AddressResolverIdKind>
            ResolverId { get; }

        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Conversation
            .AddressResolutionDecision Resolve(
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolutionContext<
                        ConversationWorldState,
                        AddressQuery,
                        Topic> context)
        {
            CallCount = checked(CallCount + 1);
            throw new InvalidOperationException("resolver failure");
        }
    }

    private static global::AI.Sandbox.Engine.Core.Conversation
        .AddressResolutionConfidence Confidence(int basisPoints) =>
        global::AI.Sandbox.Engine.Core.Conversation
            .AddressResolutionConfidence.FromBasisPoints(basisPoints);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019c0000-0000-7000-8000-000000000001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> OtherWorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019c0000-0000-7000-8000-000000000002");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> EntityId(
            int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(
                $"019c0000-0000-7100-8100-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Conversation.ConversationIdKind>
            ConversationId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationIdKind>.Parse(
                    $"019c0000-0000-7200-8200-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Conversation.AddressResolutionIdKind>
            ResolutionId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionIdKind>.Parse(
                    $"019c0000-0000-7300-8300-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Conversation.AddressResolverIdKind>
            ResolverId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolverIdKind>.Parse(
                    $"019c0000-0000-7400-8400-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Commands.CommandIdKind> CommandId(
            int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Commands.CommandIdKind>.Parse(
                $"019c0000-0000-7500-8500-{suffix:D12}");
}
