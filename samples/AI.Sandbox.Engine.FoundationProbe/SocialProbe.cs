internal static class SocialProbe
{
    private readonly record struct Topic(string Value) :
        global::AI.Sandbox.Engine.Core.Conversation.IConversationTopic;

    private readonly record struct Proposal(string Value) :
        global::AI.Sandbox.Engine.Core.Social.ISocialTurnProposal;

    private sealed record SocialWorldState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    internal sealed record Result(
        string Status,
        bool CoordinatorWasInvoked,
        int CoordinatorCallCount,
        bool StableDecision,
        bool DecisionGranted,
        bool SelectedProposalPresent,
        bool SelectedProposalIdentityValid,
        bool TwoProposalRequest,
        bool DeterministicProposalOrderValid,
        bool RequestAuthorityMetadataValid,
        bool ConversationRevisionPreserved,
        bool WorldAuthorityUnchanged,
        int BeforeValue,
        int AfterValue,
        ulong BeforeVersion,
        ulong AfterVersion,
        ulong BeforeSimulationTick,
        ulong AfterSimulationTick);

    internal static Result Run()
    {
        var manager =
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<SocialWorldState>.Create(
                    WorldId(),
                    new SocialWorldState(0));

        var before = manager.Read();

        var audience =
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressAudience.AllParticipants(
                    new[] { EntityId(2), EntityId(3) });

        var conversation =
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationState<Topic>.Start(
                    before.WorldId,
                    ConversationId(91),
                    new[] { EntityId(1), EntityId(2), EntityId(3) },
                    new Topic("social-probe"));

        var turn =
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationTurn.Create(
                    global::AI.Sandbox.Engine.Core.Conversation
                        .ConversationTurnNumber.First,
                    EntityId(1),
                    audience);

        var recorded =
            conversation.RecordTurn(
                conversation.Revision,
                turn);

        if (!recorded.WasChanged)
        {
            throw new global::System.InvalidOperationException(
                "Social probe could not record the immutable source turn.");
        }

        conversation = recorded.State;
        var conversationRevisionBefore = conversation.Revision;

        var low =
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnProposalEnvelope<Proposal>.Create(
                    ProposalId(91),
                    EntityId(2),
                    global::AI.Sandbox.Engine.Core.Social
                        .SocialTurnRequestKind.Response,
                    global::AI.Sandbox.Engine.Core.Social
                        .SocialTurnPriority.FromBasisPoints(4_000),
                    new Proposal("response"));

        var selected =
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnProposalEnvelope<Proposal>.Create(
                    ProposalId(92),
                    EntityId(3),
                    global::AI.Sandbox.Engine.Core.Social
                        .SocialTurnRequestKind.Interruption,
                    global::AI.Sandbox.Engine.Core.Social
                        .SocialTurnPriority.FromBasisPoints(8_000),
                    new Proposal("interruption"));

        var coordinator =
            new FixedCoordinator(
                CoordinatorId(91),
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinationDecision.Grant(
                        selected.ProposalId));

        var request =
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationRequestEnvelope<Proposal>.Create(
                    CoordinationId(91),
                    coordinator.CoordinatorId,
                    conversation.ConversationId,
                    conversation.LastTurn!.SpeakerEntityId,
                    conversation.LastTurn.Audience,
                    before.WorldId,
                    before.Version,
                    before.SimulationTick,
                    conversation.Revision,
                    new[] { low, selected });

        var twoProposalRequest =
            request.Proposals.Count == 2;

        var deterministicProposalOrderValid =
            request.Proposals[0].ProposalId == selected.ProposalId &&
            request.Proposals[1].ProposalId == low.ProposalId;

        if (!twoProposalRequest ||
            !deterministicProposalOrderValid)
        {
            throw new global::System.InvalidOperationException(
                "Social turn request proposal ordering was not deterministic.");
        }

        var requestAuthorityMetadataValid =
            request.WorldId == before.WorldId &&
            request.WorldStateVersion == before.Version &&
            request.SimulationTick == before.SimulationTick &&
            request.ConversationId == conversation.ConversationId &&
            request.ExpectedConversationRevision == conversation.Revision &&
            request.CurrentSpeakerEntityId ==
                conversation.LastTurn.SpeakerEntityId;

        if (!requestAuthorityMetadataValid)
        {
            throw new global::System.InvalidOperationException(
                "Social turn request authority metadata did not match.");
        }

        var processor =
            global::AI.Sandbox.Engine.Core.Social
                .SocialTurnCoordinationProcessor<
                    SocialWorldState,
                    Proposal,
                    Topic>.Create(
                        manager,
                        coordinator);

        var result =
            processor.Coordinate(
                request,
                conversation);

        if (result.Status !=
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinationStatus.Granted ||
            !result.CoordinatorWasInvoked ||
            !result.HasStableDecision)
        {
            throw new global::System.InvalidOperationException(
                $"Social turn coordination failed: {result.Status}.");
        }

        if (coordinator.CallCount != 1)
        {
            throw new global::System.InvalidOperationException(
                $"Social coordinator call count was {coordinator.CallCount}.");
        }

        if (result.Decision is null ||
            result.Decision.Status !=
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinationDecisionStatus.Granted ||
            result.Decision.SelectedProposalId != selected.ProposalId)
        {
            throw new global::System.InvalidOperationException(
                "Social turn decision was not the expected stable grant.");
        }

        var selectedProposalPresent =
            result.SelectedProposal is not null;

        var selectedProposalIdentityValid =
            global::System.Object.ReferenceEquals(
                selected,
                result.SelectedProposal) &&
            result.SelectedProposal!.ProposalId == selected.ProposalId &&
            result.SelectedProposal.ParticipantEntityId == EntityId(3) &&
            result.SelectedProposal.RequestKind ==
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnRequestKind.Interruption;

        if (!selectedProposalPresent ||
            !selectedProposalIdentityValid)
        {
            throw new global::System.InvalidOperationException(
                "Social turn result did not preserve the selected proposal.");
        }

        var after = manager.Read();

        var conversationRevisionPreserved =
            conversation.Revision == conversationRevisionBefore;

        var worldAuthorityUnchanged =
            global::System.Object.ReferenceEquals(before, after) &&
            before.State.Value == after.State.Value &&
            before.Version == after.Version &&
            before.SimulationTick == after.SimulationTick;

        if (!conversationRevisionPreserved)
        {
            throw new global::System.InvalidOperationException(
                "Social turn coordination changed ConversationState.");
        }

        if (!worldAuthorityUnchanged)
        {
            throw new global::System.InvalidOperationException(
                "Social turn coordination changed authoritative World State.");
        }

        return new Result(
            result.Status.ToString(),
            result.CoordinatorWasInvoked,
            coordinator.CallCount,
            result.HasStableDecision,
            true,
            selectedProposalPresent,
            selectedProposalIdentityValid,
            twoProposalRequest,
            deterministicProposalOrderValid,
            requestAuthorityMetadataValid,
            conversationRevisionPreserved,
            worldAuthorityUnchanged,
            before.State.Value,
            after.State.Value,
            before.Version.Value,
            after.Version.Value,
            before.SimulationTick,
            after.SimulationTick);
    }

    private sealed class FixedCoordinator :
        global::AI.Sandbox.Engine.Core.Social.ISocialTurnCoordinator<
            SocialWorldState,
            Proposal,
            Topic>
    {
        private readonly global::AI.Sandbox.Engine.Core.Social
            .SocialTurnCoordinationDecision decision;

        internal FixedCoordinator(
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

        internal int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Social
            .SocialTurnCoordinationDecision Coordinate(
                global::AI.Sandbox.Engine.Core.Social
                    .SocialTurnCoordinationContext<
                        SocialWorldState,
                        Proposal,
                        Topic> context)
        {
            CallCount = checked(CallCount + 1);

            if (context.Request.Proposals.Count !=
                context.Proposals.Count)
            {
                throw new global::System.InvalidOperationException(
                    "Social coordinator context proposal count mismatch.");
            }

            return decision;
        }
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>
        WorldId() =>
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                    "019d0000-0000-7000-8000-000000009100");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
        EntityId(int suffix) =>
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
}
