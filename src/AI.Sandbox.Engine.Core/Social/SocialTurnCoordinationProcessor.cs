namespace AI.Sandbox.Engine.Core.Social;

/// <summary>
/// Validates one immutable social turn request around one exact coordinator
/// call without mutating authority or creating a speaking queue.
/// </summary>
/// <typeparam name="TWorldState">The authoritative world-state type.</typeparam>
/// <typeparam name="TProposal">The exact proposal payload type.</typeparam>
/// <typeparam name="TTopic">The exact conversation-topic payload type.</typeparam>
public sealed class SocialTurnCoordinationProcessor<
    TWorldState,
    TProposal,
    TTopic>
    where TWorldState : class,
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TProposal : ISocialTurnProposal
    where TTopic : global::AI.Sandbox.Engine.Core.Conversation.IConversationTopic
{
    private readonly global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<TWorldState> worldStateManager;
    private readonly ISocialTurnCoordinator<TWorldState, TProposal, TTopic>
        coordinator;

    private SocialTurnCoordinationProcessor(
        global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<TWorldState> worldStateManager,
        ISocialTurnCoordinator<TWorldState, TProposal, TTopic> coordinator)
    {
        this.worldStateManager = worldStateManager;
        this.coordinator = coordinator;
    }

    /// <summary>
    /// Creates one processor around current authority and one exact
    /// coordinator.
    /// </summary>
    /// <param name="worldStateManager">The authoritative state manager.</param>
    /// <param name="coordinator">The exact coordinator invoked at most once.</param>
    /// <returns>The configured processor.</returns>
    public static SocialTurnCoordinationProcessor<
        TWorldState,
        TProposal,
        TTopic> Create(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<TWorldState> worldStateManager,
            ISocialTurnCoordinator<TWorldState, TProposal, TTopic> coordinator)
    {
        ArgumentNullException.ThrowIfNull(worldStateManager);
        ArgumentNullException.ThrowIfNull(coordinator);

        SocialTurnTypePolicy.EnsureExactType(
            typeof(TProposal),
            typeof(ISocialTurnProposal),
            "social turn proposal");

        if (coordinator.CoordinatorId.IsEmpty)
        {
            throw new ArgumentException(
                "Social turn coordinators must expose a non-empty ID.",
                nameof(coordinator));
        }

        return new SocialTurnCoordinationProcessor<
            TWorldState,
            TProposal,
            TTopic>(worldStateManager, coordinator);
    }

    /// <summary>
    /// Coordinates one immutable request against current authority and
    /// conversation state.
    /// </summary>
    /// <param name="request">The immutable coordination request.</param>
    /// <param name="conversation">The immutable current conversation.</param>
    /// <returns>An explicit validated outcome.</returns>
    public SocialTurnCoordinationResult<TProposal> Coordinate(
        SocialTurnCoordinationRequestEnvelope<TProposal> request,
        global::AI.Sandbox.Engine.Core.Conversation.ConversationState<TTopic>
            conversation)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(conversation);

        var before = worldStateManager.Read();
        var preflightStatus = ValidatePreflight(
            before,
            request,
            conversation);
        if (preflightStatus is not null)
        {
            return Result(preflightStatus.Value, false, false, null, null);
        }

        if (request.Proposals.Count == 0)
        {
            return Result(
                SocialTurnCoordinationStatus.NoEligibleProposals,
                false,
                false,
                null,
                null);
        }

        var context =
            new SocialTurnCoordinationContext<
                TWorldState,
                TProposal,
                TTopic>(
                    before,
                    request,
                    conversation);

        var decision = coordinator.Coordinate(context) ??
            throw new InvalidOperationException(
                "Social turn coordinators must return a decision.");

        var after = worldStateManager.Read();
        if (after.Version != before.Version ||
            after.SimulationTick != before.SimulationTick)
        {
            return Result(
                SocialTurnCoordinationStatus.AuthorityChanged,
                true,
                false,
                null,
                null);
        }

        if (decision.Status ==
            SocialTurnCoordinationDecisionStatus.Rejected)
        {
            return Result(
                SocialTurnCoordinationStatus.Rejected,
                true,
                true,
                decision,
                null);
        }

        if (decision.Status ==
            SocialTurnCoordinationDecisionStatus.NoTurn)
        {
            return Result(
                SocialTurnCoordinationStatus.NoTurn,
                true,
                true,
                decision,
                null);
        }

        if (decision.Status !=
            SocialTurnCoordinationDecisionStatus.Granted)
        {
            return Result(
                SocialTurnCoordinationStatus.SelectionInvalid,
                true,
                false,
                null,
                null);
        }

        var selectedProposal = request.Proposals.SingleOrDefault(
            proposal => proposal.ProposalId == decision.SelectedProposalId);
        if (selectedProposal is null)
        {
            return Result(
                SocialTurnCoordinationStatus.SelectionInvalid,
                true,
                false,
                null,
                null);
        }

        return Result(
            SocialTurnCoordinationStatus.Granted,
            true,
            true,
            decision,
            selectedProposal);
    }

    private SocialTurnCoordinationStatus? ValidatePreflight(
        global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateSnapshot<TWorldState> snapshot,
        SocialTurnCoordinationRequestEnvelope<TProposal> request,
        global::AI.Sandbox.Engine.Core.Conversation.ConversationState<TTopic>
            conversation)
    {
        if (request.CoordinatorId != coordinator.CoordinatorId)
        {
            return SocialTurnCoordinationStatus.CoordinatorMismatch;
        }

        if (request.WorldId != snapshot.WorldId)
        {
            return SocialTurnCoordinationStatus.WorldMismatch;
        }

        if (request.WorldStateVersion != snapshot.Version)
        {
            return SocialTurnCoordinationStatus.VersionConflict;
        }

        if (request.SimulationTick != snapshot.SimulationTick)
        {
            return SocialTurnCoordinationStatus.SimulationTickMismatch;
        }

        if (request.ConversationId != conversation.ConversationId)
        {
            return SocialTurnCoordinationStatus.ConversationMismatch;
        }

        if (request.WorldId != conversation.WorldId)
        {
            return SocialTurnCoordinationStatus.ConversationWorldMismatch;
        }

        if (request.ExpectedConversationRevision != conversation.Revision)
        {
            return SocialTurnCoordinationStatus
                .ConversationRevisionConflict;
        }

        if (conversation.IsClosed)
        {
            return SocialTurnCoordinationStatus.ConversationClosed;
        }

        if (conversation.LastTurn is null)
        {
            return SocialTurnCoordinationStatus.CurrentTurnMissing;
        }

        if (conversation.LastTurn.SpeakerEntityId !=
            request.CurrentSpeakerEntityId)
        {
            return SocialTurnCoordinationStatus.CurrentSpeakerMismatch;
        }

        if (!AudienceEquals(conversation.LastTurn.Audience, request.Audience))
        {
            return SocialTurnCoordinationStatus.CurrentAudienceMismatch;
        }

        foreach (var proposal in request.Proposals)
        {
            if (proposal.ParticipantEntityId ==
                    request.CurrentSpeakerEntityId ||
                !conversation.ContainsParticipant(
                    proposal.ParticipantEntityId))
            {
                return SocialTurnCoordinationStatus.ProposalInvalid;
            }
        }

        return null;
    }

    private static bool AudienceEquals(
        global::AI.Sandbox.Engine.Core.Conversation.AddressAudience left,
        global::AI.Sandbox.Engine.Core.Conversation.AddressAudience right) =>
        left.Kind == right.Kind &&
        left.TargetEntityIds.SequenceEqual(right.TargetEntityIds);

    private static SocialTurnCoordinationResult<TProposal> Result(
        SocialTurnCoordinationStatus status,
        bool coordinatorWasInvoked,
        bool hasStableDecision,
        SocialTurnCoordinationDecision? decision,
        SocialTurnProposalEnvelope<TProposal>? selectedProposal) =>
        new(
            status,
            coordinatorWasInvoked,
            hasStableDecision,
            decision,
            selectedProposal);
}
