namespace AI.Sandbox.Engine.Core.Conversation;

/// <summary>
/// Validates authority and conversation scope around exactly one pure address
/// resolver invocation.
/// </summary>
/// <typeparam name="TWorldState">The authoritative world-state type.</typeparam>
/// <typeparam name="TQuery">The exact address-query payload type.</typeparam>
/// <typeparam name="TTopic">The exact conversation-topic payload type.</typeparam>
public sealed class AddressResolutionProcessor<TWorldState, TQuery, TTopic>
    where TWorldState : class,
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TQuery : IAddressQuery
    where TTopic : IConversationTopic
{
    private readonly global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<TWorldState> worldStateManager;
    private readonly IAddressResolver<TWorldState, TQuery, TTopic> resolver;

    private AddressResolutionProcessor(
        global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<TWorldState> worldStateManager,
        IAddressResolver<TWorldState, TQuery, TTopic> resolver)
    {
        this.worldStateManager = worldStateManager;
        this.resolver = resolver;
    }

    /// <summary>
    /// Creates one processor around a current authority manager and exact
    /// resolver.
    /// </summary>
    /// <param name="worldStateManager">The authoritative state manager.</param>
    /// <param name="resolver">The exact resolver invoked at most once.</param>
    /// <returns>The configured processor.</returns>
    public static AddressResolutionProcessor<TWorldState, TQuery, TTopic>
        Create(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<TWorldState> worldStateManager,
            IAddressResolver<TWorldState, TQuery, TTopic> resolver)
    {
        ArgumentNullException.ThrowIfNull(worldStateManager);
        ArgumentNullException.ThrowIfNull(resolver);

        ConversationTypePolicy.EnsureExactType(
            typeof(TQuery),
            typeof(IAddressQuery),
            "address query");
        ConversationTypePolicy.EnsureExactType(
            typeof(TTopic),
            typeof(IConversationTopic),
            "conversation topic");

        if (resolver.ResolverId.IsEmpty)
        {
            throw new ArgumentException(
                "Address resolvers must expose a non-empty resolver ID.",
                nameof(resolver));
        }

        return new AddressResolutionProcessor<TWorldState, TQuery, TTopic>(
            worldStateManager,
            resolver);
    }

    /// <summary>
    /// Resolves one request against one immutable conversation state.
    /// </summary>
    /// <param name="request">The immutable semantic request.</param>
    /// <param name="conversation">The immutable current conversation state.</param>
    /// <returns>An explicit validated outcome.</returns>
    public AddressResolutionResult Resolve(
        AddressResolutionRequestEnvelope<TQuery> request,
        ConversationState<TTopic> conversation)
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
            return Result(preflightStatus.Value, false, false, null);
        }

        var candidates = conversation.ParticipantEntityIds
            .Where(participant => participant != request.SpeakerEntityId)
            .ToArray();

        var context =
            new AddressResolutionContext<TWorldState, TQuery, TTopic>(
                before,
                request,
                conversation,
                candidates);

        var decision = resolver.Resolve(context) ??
            throw new InvalidOperationException(
                "Address resolvers must return a decision.");

        var after = worldStateManager.Read();
        if (after.Version != before.Version ||
            after.SimulationTick != before.SimulationTick)
        {
            return Result(
                AddressResolutionStatus.AuthorityChanged,
                true,
                false,
                null);
        }

        if (decision.Status == AddressResolutionDecisionStatus.Rejected)
        {
            return Result(
                AddressResolutionStatus.Rejected,
                true,
                true,
                decision);
        }

        if (decision.Status != AddressResolutionDecisionStatus.Resolved ||
            decision.Audience is null ||
            !decision.Confidence.IsInitialized ||
            !IsAudienceValid(
                request.SpeakerEntityId,
                candidates,
                decision.Audience))
        {
            return Result(
                AddressResolutionStatus.AudienceInvalid,
                true,
                false,
                null);
        }

        return Result(
            AddressResolutionStatus.Resolved,
            true,
            true,
            decision);
    }

    private AddressResolutionStatus? ValidatePreflight(
        global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateSnapshot<TWorldState> snapshot,
        AddressResolutionRequestEnvelope<TQuery> request,
        ConversationState<TTopic> conversation)
    {
        if (request.ResolverId != resolver.ResolverId)
        {
            return AddressResolutionStatus.ResolverMismatch;
        }

        if (request.WorldId != snapshot.WorldId)
        {
            return AddressResolutionStatus.WorldMismatch;
        }

        if (request.WorldStateVersion != snapshot.Version)
        {
            return AddressResolutionStatus.VersionConflict;
        }

        if (request.SimulationTick != snapshot.SimulationTick)
        {
            return AddressResolutionStatus.SimulationTickMismatch;
        }

        if (request.ConversationId != conversation.ConversationId)
        {
            return AddressResolutionStatus.ConversationMismatch;
        }

        if (request.WorldId != conversation.WorldId)
        {
            return AddressResolutionStatus.ConversationWorldMismatch;
        }

        if (request.ExpectedConversationRevision != conversation.Revision)
        {
            return AddressResolutionStatus.ConversationRevisionConflict;
        }

        if (conversation.IsClosed)
        {
            return AddressResolutionStatus.ConversationClosed;
        }

        return conversation.ContainsParticipant(request.SpeakerEntityId)
            ? null
            : AddressResolutionStatus.SpeakerNotParticipant;
    }

    private static bool IsAudienceValid(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> speaker,
        IReadOnlyList<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>
            candidates,
        AddressAudience audience)
    {
        var targets = audience.TargetEntityIds;

        if (audience.Kind == AddressAudienceKind.None)
        {
            return targets.Count == 0;
        }

        if (targets.Count == 0)
        {
            return false;
        }

        foreach (var target in targets)
        {
            if (target == speaker || !candidates.Contains(target))
            {
                return false;
            }
        }

        if (audience.Kind == AddressAudienceKind.SpecificParticipants)
        {
            return true;
        }

        return audience.Kind == AddressAudienceKind.AllParticipants &&
            targets.Count == candidates.Count &&
            targets.SequenceEqual(candidates);
    }

    private static AddressResolutionResult Result(
        AddressResolutionStatus status,
        bool resolverWasInvoked,
        bool hasStableDecision,
        AddressResolutionDecision? decision) =>
        new(status, resolverWasInvoked, hasStableDecision, decision);
}
