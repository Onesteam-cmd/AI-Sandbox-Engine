namespace AI.Sandbox.Engine.Core.Tests;

public sealed class SocialTurnTakingTests
{
    private readonly record struct Topic(string Value) :
        global::AI.Sandbox.Engine.Core.Conversation.IConversationTopic;

    private readonly record struct Proposal(string Value) :
        global::AI.Sandbox.Engine.Core.Social.ISocialTurnProposal;

    private abstract record AbstractProposal :
        global::AI.Sandbox.Engine.Core.Social.ISocialTurnProposal;

    private sealed record ConcreteAbstractProposal(string Value) :
        AbstractProposal;

    private record OpenProposal(string Value) :
        global::AI.Sandbox.Engine.Core.Social.ISocialTurnProposal;

    private readonly record struct AdvanceValue(int Delta) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    private sealed record SocialWorldState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    [Xunit.Fact]
    public void PayloadTypesAndValuesMustBeExactAndBounded()
    {
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Social
                .SocialTurnProposalEnvelope<AbstractProposal>.Create(
                    ProposalId(1),
                    EntityId(2),
                    global::AI.Sandbox.Engine.Core.Social
                        .SocialTurnRequestKind.Response,
                    Priority(5000),
                    new ConcreteAbstractProposal("abstract")));

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Social
                .SocialTurnProposalEnvelope<OpenProposal>.Create(
                    ProposalId(2),
                    EntityId(2),
                    global::AI.Sandbox.Engine.Core.Social
                        .SocialTurnRequestKind.Response,
                    Priority(5000),
                    new OpenProposal("open")));

        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.Social
                .SocialTurnPriority.FromBasisPoints(0));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.Social
                .SocialTurnPriority.FromBasisPoints(10001));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Social
                .SocialTurnNoTurnCode.From("Not.Valid"));

        var proposal = CreateProposal(
            3,
            EntityId(2),
            5000,
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnRequestKind.Response);

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationRequestEnvelope<Proposal>.Create(
                    CoordinationId(1),
                    CoordinatorId(1),
                    ConversationId(1),
                    EntityId(1),
                    global::AI.Sandbox.Engine.Core.Conversation
                        .AddressAudience.SpecificParticipants(
                            new[] { EntityId(2) }),
                    WorldId(),
                    default,
                    0,
                    global::AI.Sandbox.Engine.Core.Conversation
                        .ConversationRevision.Initial,
                    new[] { proposal, proposal }));

        var sameParticipant = CreateProposal(
            4,
            EntityId(2),
            4000,
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnRequestKind.Interruption);
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationRequestEnvelope<Proposal>.Create(
                    CoordinationId(2),
                    CoordinatorId(1),
                    ConversationId(1),
                    EntityId(1),
                    global::AI.Sandbox.Engine.Core.Conversation
                        .AddressAudience.SpecificParticipants(
                            new[] { EntityId(2) }),
                    WorldId(),
                    default,
                    0,
                    global::AI.Sandbox.Engine.Core.Conversation
                        .ConversationRevision.Initial,
                    new[] { proposal, sameParticipant }));
    }

    [Xunit.Fact]
    public void RequestSortsProposalsDeterministicallyAndPreservesScope()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var audience = global::AI.Sandbox.Engine.Core.Conversation
            .AddressAudience.SpecificParticipants(new[] { EntityId(2) });
        var conversation = CreateConversation(
            10,
            audience,
            EntityId(1),
            EntityId(2),
            EntityId(3),
            EntityId(4));

        var low = CreateProposal(
            30,
            EntityId(2),
            2000,
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnRequestKind.Response);
        var highLaterId = CreateProposal(
            32,
            EntityId(3),
            9000,
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnRequestKind.Interruption);
        var highEarlierId = CreateProposal(
            31,
            EntityId(4),
            9000,
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnRequestKind.Response);

        var request = CreateRequest(
            snapshot,
            conversation,
            CoordinatorId(10),
            10,
            new[] { low, highLaterId, highEarlierId });

        Xunit.Assert.Equal(CoordinationId(10), request.CoordinationId);
        Xunit.Assert.Equal(CoordinatorId(10), request.CoordinatorId);
        Xunit.Assert.Equal(conversation.ConversationId, request.ConversationId);
        Xunit.Assert.Equal(EntityId(1), request.CurrentSpeakerEntityId);
        Xunit.Assert.Equal(snapshot.WorldId, request.WorldId);
        Xunit.Assert.Equal(snapshot.Version, request.WorldStateVersion);
        Xunit.Assert.Equal(snapshot.SimulationTick, request.SimulationTick);
        Xunit.Assert.Equal(
            conversation.Revision,
            request.ExpectedConversationRevision);
        Xunit.Assert.Equal(
            new[] { ProposalId(31), ProposalId(32), ProposalId(30) },
            request.Proposals.Select(proposal => proposal.ProposalId).ToArray());
    }

    [Xunit.Fact]
    public void NoEligibleProposalsSkipsCoordinator()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var conversation = CreateConversation(
            20,
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressAudience.None(),
            EntityId(1),
            EntityId(2));
        var coordinator = new FixedCoordinator(
            CoordinatorId(20),
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationDecision.SelectNoTurn(
                    global::AI.Sandbox.Engine.Core.Social
                        .SocialTurnNoTurnCode.From("social.silence")));
        var processor = CreateProcessor(manager, coordinator);

        var result = processor.Coordinate(
            CreateRequest(
                snapshot,
                conversation,
                coordinator.CoordinatorId,
                20,
                Array.Empty<
                    global::AI.Sandbox.Engine.Core.Social
                        .SocialTurnProposalEnvelope<Proposal>>()),
            conversation);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationStatus.NoEligibleProposals,
            result.Status);
        Xunit.Assert.False(result.CoordinatorWasInvoked);
        Xunit.Assert.False(result.HasStableDecision);
        Xunit.Assert.Null(result.Decision);
        Xunit.Assert.Null(result.SelectedProposal);
        Xunit.Assert.Equal(0, coordinator.CallCount);
    }

    [Xunit.Fact]
    public void CoordinatorGrantsResponseAndInterruptionExactlyOnce()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var audience = global::AI.Sandbox.Engine.Core.Conversation
            .AddressAudience.AllParticipants(
                new[] { EntityId(2), EntityId(3) });
        var conversation = CreateConversation(
            30,
            audience,
            EntityId(1),
            EntityId(2),
            EntityId(3));

        foreach (var caseData in new[]
        {
            (
                RequestKind:
                    global::AI.Sandbox.Engine.Core.Social
                        .SocialTurnRequestKind.Response,
                Suffix: 301),
            (
                RequestKind:
                    global::AI.Sandbox.Engine.Core.Social
                        .SocialTurnRequestKind.Interruption,
                Suffix: 302),
        })
        {
            var proposal = CreateProposal(
                caseData.Suffix,
                EntityId(2),
                7000,
                caseData.RequestKind);
            var coordinator = new FixedCoordinator(
                CoordinatorId(caseData.Suffix),
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinationDecision.Grant(
                        proposal.ProposalId));
            var result = CreateProcessor(manager, coordinator).Coordinate(
                CreateRequest(
                    snapshot,
                    conversation,
                    coordinator.CoordinatorId,
                    caseData.Suffix,
                    new[] { proposal }),
                conversation);

            Xunit.Assert.Equal(
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinationStatus.Granted,
                result.Status);
            Xunit.Assert.True(result.CoordinatorWasInvoked);
            Xunit.Assert.True(result.HasStableDecision);
            Xunit.Assert.Same(proposal, result.SelectedProposal);
            Xunit.Assert.Equal(
                caseData.RequestKind,
                result.SelectedProposal!.RequestKind);
            Xunit.Assert.Equal(1, coordinator.CallCount);
        }
    }

    [Xunit.Fact]
    public void CoordinatorMayReturnNoTurnOrReject()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var conversation = CreateConversation(
            40,
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressAudience.SpecificParticipants(
                    new[] { EntityId(2) }),
            EntityId(1),
            EntityId(2));
        var proposal = CreateProposal(
            40,
            EntityId(2),
            6000,
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnRequestKind.Response);

        var noTurnCoordinator = new FixedCoordinator(
            CoordinatorId(40),
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationDecision.SelectNoTurn(
                    global::AI.Sandbox.Engine.Core.Social
                        .SocialTurnNoTurnCode.From("social.no-response")));
        var noTurn = CreateProcessor(
            manager,
            noTurnCoordinator).Coordinate(
                CreateRequest(
                    snapshot,
                    conversation,
                    noTurnCoordinator.CoordinatorId,
                    40,
                    new[] { proposal }),
                conversation);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationStatus.NoTurn,
            noTurn.Status);
        Xunit.Assert.Equal(
            "social.no-response",
            noTurn.Decision!.NoTurnCode.Value);

        var rejectedCoordinator = new FixedCoordinator(
            CoordinatorId(41),
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationDecision.Reject(
                    global::AI.Sandbox.Engine.Core.Social
                        .SocialTurnRejectionCode.From(
                            "social.context-insufficient")));
        var rejected = CreateProcessor(
            manager,
            rejectedCoordinator).Coordinate(
                CreateRequest(
                    snapshot,
                    conversation,
                    rejectedCoordinator.CoordinatorId,
                    41,
                    new[] { proposal }),
                conversation);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationStatus.Rejected,
            rejected.Status);
        Xunit.Assert.True(rejected.HasStableDecision);
        Xunit.Assert.Equal(
            "social.context-insufficient",
            rejected.Decision!.RejectionCode.Value);
        Xunit.Assert.Equal(1, noTurnCoordinator.CallCount);
        Xunit.Assert.Equal(1, rejectedCoordinator.CallCount);
    }

    [Xunit.Fact]
    public void PreflightMismatchesSkipCoordinator()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var audience = global::AI.Sandbox.Engine.Core.Conversation
            .AddressAudience.SpecificParticipants(new[] { EntityId(2) });
        var conversation = CreateConversation(
            50,
            audience,
            EntityId(1),
            EntityId(2),
            EntityId(3));
        var proposal = CreateProposal(
            50,
            EntityId(2),
            5000,
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnRequestKind.Response);
        var coordinator = new FixedCoordinator(
            CoordinatorId(50),
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationDecision.Grant(
                    proposal.ProposalId));
        var processor = CreateProcessor(manager, coordinator);

        var cases = new[]
        {
            processor.Coordinate(
                CreateRequest(
                    snapshot,
                    conversation,
                    CoordinatorId(999),
                    501,
                    new[] { proposal }),
                conversation),
            processor.Coordinate(
                CreateRequest(
                    snapshot,
                    conversation,
                    coordinator.CoordinatorId,
                    502,
                    new[] { proposal },
                    worldId: OtherWorldId()),
                conversation),
            processor.Coordinate(
                CreateRequest(
                    snapshot,
                    conversation,
                    coordinator.CoordinatorId,
                    503,
                    new[] { proposal },
                    version:
                        global::AI.Sandbox.Engine.Core.WorldState
                            .WorldStateVersion.From(99)),
                conversation),
            processor.Coordinate(
                CreateRequest(
                    snapshot,
                    conversation,
                    coordinator.CoordinatorId,
                    504,
                    new[] { proposal },
                    simulationTick: 99),
                conversation),
            processor.Coordinate(
                CreateRequest(
                    snapshot,
                    conversation,
                    coordinator.CoordinatorId,
                    505,
                    new[] { proposal },
                    conversationId: ConversationId(999)),
                conversation),
            processor.Coordinate(
                CreateRequest(
                    snapshot,
                    conversation,
                    coordinator.CoordinatorId,
                    506,
                    new[] { proposal },
                    expectedRevision:
                        global::AI.Sandbox.Engine.Core.Conversation
                            .ConversationRevision.From(99)),
                conversation),
            processor.Coordinate(
                CreateRequest(
                    snapshot,
                    conversation,
                    coordinator.CoordinatorId,
                    507,
                    new[] { proposal },
                    currentSpeakerEntityId: EntityId(2)),
                conversation),
            processor.Coordinate(
                CreateRequest(
                    snapshot,
                    conversation,
                    coordinator.CoordinatorId,
                    508,
                    new[] { proposal },
                    audience:
                        global::AI.Sandbox.Engine.Core.Conversation
                            .AddressAudience.None()),
                conversation),
        };

        Xunit.Assert.Equal(
            new[]
            {
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinationStatus.CoordinatorMismatch,
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinationStatus.WorldMismatch,
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinationStatus.VersionConflict,
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinationStatus.SimulationTickMismatch,
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinationStatus.ConversationMismatch,
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinationStatus
                    .ConversationRevisionConflict,
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinationStatus.CurrentSpeakerMismatch,
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinationStatus.CurrentAudienceMismatch,
            },
            cases.Select(result => result.Status).ToArray());

        var otherWorldConversation = CreateConversation(
            51,
            audience,
            OtherWorldId(),
            EntityId(1),
            EntityId(2));
        var otherWorld = processor.Coordinate(
            CreateRequest(
                snapshot,
                conversation,
                coordinator.CoordinatorId,
                509,
                new[] { proposal }),
            otherWorldConversation);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationStatus.ConversationMismatch,
            otherWorld.Status);

        var sameIdOtherWorldConversation = CreateConversation(
            conversation.ConversationId,
            audience,
            OtherWorldId(),
            EntityId(1),
            EntityId(2));
        var sameIdOtherWorld = processor.Coordinate(
            CreateRequest(
                snapshot,
                conversation,
                coordinator.CoordinatorId,
                510,
                new[] { proposal }),
            sameIdOtherWorldConversation);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationStatus.ConversationWorldMismatch,
            sameIdOtherWorld.Status);

        var closed = conversation.Close(conversation.Revision).State;
        var closedResult = processor.Coordinate(
            CreateRequest(
                snapshot,
                closed,
                coordinator.CoordinatorId,
                511,
                new[] { proposal }),
            closed);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationStatus.ConversationClosed,
            closedResult.Status);

        var noTurnConversation =
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationState<Topic>.Start(
                    WorldId(),
                    ConversationId(52),
                    new[] { EntityId(1), EntityId(2) },
                    new Topic("topic"));
        var missingTurn = processor.Coordinate(
            CreateRequest(
                snapshot,
                noTurnConversation,
                coordinator.CoordinatorId,
                512,
                new[] { proposal },
                currentSpeakerEntityId: EntityId(1),
                audience:
                    global::AI.Sandbox.Engine.Core.Conversation
                        .AddressAudience.None()),
            noTurnConversation);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationStatus.CurrentTurnMissing,
            missingTurn.Status);

        Xunit.Assert.Equal(0, coordinator.CallCount);
    }

    [Xunit.Fact]
    public void InvalidProposalOrSelectionRemainExplicit()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var conversation = CreateConversation(
            60,
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressAudience.SpecificParticipants(
                    new[] { EntityId(2) }),
            EntityId(1),
            EntityId(2));

        var outsiderProposal = CreateProposal(
            60,
            EntityId(9),
            5000,
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnRequestKind.Response);
        var skippedCoordinator = new FixedCoordinator(
            CoordinatorId(60),
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationDecision.Grant(
                    outsiderProposal.ProposalId));
        var invalidProposal = CreateProcessor(
            manager,
            skippedCoordinator).Coordinate(
                CreateRequest(
                    snapshot,
                    conversation,
                    skippedCoordinator.CoordinatorId,
                    60,
                    new[] { outsiderProposal }),
                conversation);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationStatus.ProposalInvalid,
            invalidProposal.Status);
        Xunit.Assert.Equal(0, skippedCoordinator.CallCount);

        var validProposal = CreateProposal(
            61,
            EntityId(2),
            5000,
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnRequestKind.Response);
        var invalidSelectionCoordinator = new FixedCoordinator(
            CoordinatorId(61),
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationDecision.Grant(ProposalId(999)));
        var invalidSelection = CreateProcessor(
            manager,
            invalidSelectionCoordinator).Coordinate(
                CreateRequest(
                    snapshot,
                    conversation,
                    invalidSelectionCoordinator.CoordinatorId,
                    61,
                    new[] { validProposal }),
                conversation);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationStatus.SelectionInvalid,
            invalidSelection.Status);
        Xunit.Assert.True(invalidSelection.CoordinatorWasInvoked);
        Xunit.Assert.False(invalidSelection.HasStableDecision);
        Xunit.Assert.Null(invalidSelection.Decision);
        Xunit.Assert.Null(invalidSelection.SelectedProposal);
        Xunit.Assert.Equal(1, invalidSelectionCoordinator.CallCount);
    }

    [Xunit.Fact]
    public void AuthorityChangeDiscardsDecisionWithoutRetry()
    {
        var manager = CreateManager();
        var runtime = CreateRuntime(manager);
        var snapshot = manager.Read();
        var conversation = CreateConversation(
            70,
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressAudience.SpecificParticipants(
                    new[] { EntityId(2) }),
            EntityId(1),
            EntityId(2));
        var proposal = CreateProposal(
            70,
            EntityId(2),
            5000,
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnRequestKind.Response);
        var coordinator = new MutatingCoordinator(
            runtime,
            CoordinatorId(70),
            proposal.ProposalId);
        var processor = CreateProcessor(manager, coordinator);

        var result = processor.Coordinate(
            CreateRequest(
                snapshot,
                conversation,
                coordinator.CoordinatorId,
                70,
                new[] { proposal }),
            conversation);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationStatus.AuthorityChanged,
            result.Status);
        Xunit.Assert.True(result.CoordinatorWasInvoked);
        Xunit.Assert.False(result.HasStableDecision);
        Xunit.Assert.Null(result.Decision);
        Xunit.Assert.Null(result.SelectedProposal);
        Xunit.Assert.Equal(1, coordinator.CallCount);
        Xunit.Assert.Equal(1, manager.Read().State.Value);
    }

    [Xunit.Fact]
    public void CoordinatorExceptionPropagatesWithoutRetry()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var conversation = CreateConversation(
            80,
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressAudience.SpecificParticipants(
                    new[] { EntityId(2) }),
            EntityId(1),
            EntityId(2));
        var proposal = CreateProposal(
            80,
            EntityId(2),
            5000,
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnRequestKind.Response);
        var coordinator = new ThrowingCoordinator(CoordinatorId(80));
        var processor = CreateProcessor(manager, coordinator);

        var exception = Xunit.Assert.Throws<InvalidOperationException>(
            () => processor.Coordinate(
                CreateRequest(
                    snapshot,
                    conversation,
                    coordinator.CoordinatorId,
                    80,
                    new[] { proposal }),
                conversation));

        Xunit.Assert.Equal("coordinator failure", exception.Message);
        Xunit.Assert.Equal(1, coordinator.CallCount);
        Xunit.Assert.Equal(snapshot.Version, manager.Read().Version);
    }

    private static global::AI.Sandbox.Engine.Core.Conversation
        .ConversationState<Topic> CreateConversation(
            int suffix,
            global::AI.Sandbox.Engine.Core.Conversation.AddressAudience
                audience,
            params global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>[]
                participants) =>
        CreateConversation(
            ConversationId(suffix),
            audience,
            WorldId(),
            participants);

    private static global::AI.Sandbox.Engine.Core.Conversation
        .ConversationState<Topic> CreateConversation(
            int suffix,
            global::AI.Sandbox.Engine.Core.Conversation.AddressAudience
                audience,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
            params global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>[]
                participants) =>
        CreateConversation(
            ConversationId(suffix),
            audience,
            worldId,
            participants);

    private static global::AI.Sandbox.Engine.Core.Conversation
        .ConversationState<Topic> CreateConversation(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Conversation
                    .ConversationIdKind> conversationId,
            global::AI.Sandbox.Engine.Core.Conversation.AddressAudience
                audience,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
            params global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>[]
                participants)
    {
        var conversation =
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationState<Topic>.Start(
                    worldId,
                    conversationId,
                    participants,
                    new Topic("topic"));
        var turn =
            global::AI.Sandbox.Engine.Core.Conversation.ConversationTurn.Create(
                global::AI.Sandbox.Engine.Core.Conversation
                    .ConversationTurnNumber.First,
                EntityId(1),
                audience);
        var recorded = conversation.RecordTurn(conversation.Revision, turn);
        Xunit.Assert.True(recorded.WasChanged);
        return recorded.State;
    }

    private static global::AI.Sandbox.Engine.Core.Social
        .SocialTurnProposalEnvelope<Proposal> CreateProposal(
            int suffix,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
                participantEntityId,
            int priority,
            global::AI.Sandbox.Engine.Core.Social.SocialTurnRequestKind
                requestKind) =>
        global::AI.Sandbox.Engine.Core.Social
            .SocialTurnProposalEnvelope<Proposal>.Create(
                ProposalId(suffix),
                participantEntityId,
                requestKind,
                Priority(priority),
                new Proposal($"proposal-{suffix}"));

    private static global::AI.Sandbox.Engine.Core.Social
        .SocialTurnCoordinationRequestEnvelope<Proposal> CreateRequest(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateSnapshot<SocialWorldState> snapshot,
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationState<Topic> conversation,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinatorIdKind> coordinatorId,
            int suffix,
            IEnumerable<
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnProposalEnvelope<Proposal>> proposals,
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
                expectedRevision = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>?
                currentSpeakerEntityId = null,
            global::AI.Sandbox.Engine.Core.Conversation.AddressAudience?
                audience = null) =>
        global::AI.Sandbox.Engine.Core.Social
            .SocialTurnCoordinationRequestEnvelope<Proposal>.Create(
                CoordinationId(suffix),
                coordinatorId,
                conversationId ?? conversation.ConversationId,
                currentSpeakerEntityId ??
                    conversation.LastTurn!.SpeakerEntityId,
                audience ?? conversation.LastTurn!.Audience,
                worldId ?? snapshot.WorldId,
                version ?? snapshot.Version,
                simulationTick ?? snapshot.SimulationTick,
                expectedRevision ?? conversation.Revision,
                proposals);

    private static global::AI.Sandbox.Engine.Core.Social
        .SocialTurnCoordinationProcessor<
            SocialWorldState,
            Proposal,
            Topic> CreateProcessor(
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateManager<SocialWorldState> manager,
                global::AI.Sandbox.Engine.Core.Social.ISocialTurnCoordinator<
                    SocialWorldState,
                    Proposal,
                    Topic> coordinator) =>
        global::AI.Sandbox.Engine.Core.Social
            .SocialTurnCoordinationProcessor<
                SocialWorldState,
                Proposal,
                Topic>.Create(manager, coordinator);

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<SocialWorldState> CreateManager() =>
        global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<SocialWorldState>.Create(
                WorldId(),
                new SocialWorldState(0));

    private static global::AI.Sandbox.Engine.Core.Runtime
        .RuntimeOrchestrator<SocialWorldState> CreateRuntime(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<SocialWorldState> manager) =>
        new global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestratorBuilder<SocialWorldState>()
            .AddCommandHandler(new AdvanceValueHandler())
            .Build(manager);

    private sealed class AdvanceValueHandler :
        global::AI.Sandbox.Engine.Core.Commands.ICommandHandler<
            SocialWorldState,
            AdvanceValue>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<SocialWorldState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<SocialWorldState, AdvanceValue> context)
        {
            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<SocialWorldState>.Accept(
                    context.Snapshot.State with
                    {
                        Value = checked(
                            context.Snapshot.State.Value +
                            context.Envelope.Payload.Delta),
                    });
        }
    }

    private sealed class FixedCoordinator :
        global::AI.Sandbox.Engine.Core.Social.ISocialTurnCoordinator<
            SocialWorldState,
            Proposal,
            Topic>
    {
        private readonly global::AI.Sandbox.Engine.Core.Social
            .SocialTurnCoordinationDecision decision;

        public FixedCoordinator(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinatorIdKind> coordinatorId,
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationDecision decision)
        {
            CoordinatorId = coordinatorId;
            this.decision = decision;
        }

        public global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinatorIdKind> CoordinatorId { get; }

        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Social
            .SocialTurnCoordinationDecision Coordinate(
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinationContext<
                        SocialWorldState,
                        Proposal,
                        Topic> context)
        {
            CallCount = checked(CallCount + 1);
            Xunit.Assert.Equal(
                context.Request.Proposals.Count,
                context.Proposals.Count);
            return decision;
        }
    }

    private sealed class MutatingCoordinator :
        global::AI.Sandbox.Engine.Core.Social.ISocialTurnCoordinator<
            SocialWorldState,
            Proposal,
            Topic>
    {
        private readonly global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestrator<SocialWorldState> runtime;
        private readonly global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Social.SocialTurnProposalIdKind>
            proposalId;

        public MutatingCoordinator(
            global::AI.Sandbox.Engine.Core.Runtime
                .RuntimeOrchestrator<SocialWorldState> runtime,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinatorIdKind> coordinatorId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnProposalIdKind> proposalId)
        {
            this.runtime = runtime;
            CoordinatorId = coordinatorId;
            this.proposalId = proposalId;
        }

        public global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinatorIdKind> CoordinatorId { get; }

        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Social
            .SocialTurnCoordinationDecision Coordinate(
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinationContext<
                        SocialWorldState,
                        Proposal,
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

            return global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationDecision.Grant(proposalId);
        }
    }

    private sealed class ThrowingCoordinator :
        global::AI.Sandbox.Engine.Core.Social.ISocialTurnCoordinator<
            SocialWorldState,
            Proposal,
            Topic>
    {
        public ThrowingCoordinator(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinatorIdKind> coordinatorId)
        {
            CoordinatorId = coordinatorId;
        }

        public global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinatorIdKind> CoordinatorId { get; }

        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Social
            .SocialTurnCoordinationDecision Coordinate(
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinationContext<
                        SocialWorldState,
                        Proposal,
                        Topic> context)
        {
            CallCount = checked(CallCount + 1);
            throw new InvalidOperationException("coordinator failure");
        }
    }

    private static global::AI.Sandbox.Engine.Core.Social.SocialTurnPriority
        Priority(int basisPoints) =>
        global::AI.Sandbox.Engine.Core.Social
            .SocialTurnPriority.FromBasisPoints(basisPoints);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019d0000-0000-7000-8000-000000000001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> OtherWorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019d0000-0000-7000-8000-000000000002");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> EntityId(
            int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(
                $"019d0000-0000-7100-8100-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Conversation.ConversationIdKind>
            ConversationId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationIdKind>.Parse(
                    $"019d0000-0000-7200-8200-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Social.SocialTurnCoordinationIdKind>
            CoordinationId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationIdKind>.Parse(
                    $"019d0000-0000-7300-8300-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Social.SocialTurnCoordinatorIdKind>
            CoordinatorId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinatorIdKind>.Parse(
                    $"019d0000-0000-7400-8400-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Social.SocialTurnProposalIdKind>
            ProposalId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnProposalIdKind>.Parse(
                    $"019d0000-0000-7500-8500-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Commands.CommandIdKind> CommandId(
            int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Commands.CommandIdKind>.Parse(
                $"019d0000-0000-7600-8600-{suffix:D12}");
}
