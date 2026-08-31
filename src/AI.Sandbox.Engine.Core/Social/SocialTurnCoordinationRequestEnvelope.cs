namespace AI.Sandbox.Engine.Core.Social;

/// <summary>
/// Captures one immutable request to coordinate the next conversational
/// speaker from zero or more participant proposals.
/// </summary>
/// <typeparam name="TProposal">The exact social turn proposal payload.</typeparam>
public sealed class SocialTurnCoordinationRequestEnvelope<TProposal>
    where TProposal : ISocialTurnProposal
{
    private const int MaximumProposalCount = 63;
    private readonly global::System.Collections.ObjectModel.ReadOnlyCollection<
        SocialTurnProposalEnvelope<TProposal>> proposals;

    private SocialTurnCoordinationRequestEnvelope(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            SocialTurnCoordinationIdKind> coordinationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            SocialTurnCoordinatorIdKind> coordinatorId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Conversation.ConversationIdKind>
            conversationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
            currentSpeakerEntityId,
        global::AI.Sandbox.Engine.Core.Conversation.AddressAudience audience,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        global::AI.Sandbox.Engine.Core.Conversation.ConversationRevision
            expectedConversationRevision,
        SocialTurnProposalEnvelope<TProposal>[] orderedProposals)
    {
        CoordinationId = coordinationId;
        CoordinatorId = coordinatorId;
        ConversationId = conversationId;
        CurrentSpeakerEntityId = currentSpeakerEntityId;
        Audience = audience;
        WorldId = worldId;
        WorldStateVersion = worldStateVersion;
        SimulationTick = simulationTick;
        ExpectedConversationRevision = expectedConversationRevision;
        proposals = Array.AsReadOnly(orderedProposals);
    }

    /// <summary>
    /// Gets the externally assigned coordination ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        SocialTurnCoordinationIdKind> CoordinationId { get; }

    /// <summary>
    /// Gets the configured coordinator ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        SocialTurnCoordinatorIdKind> CoordinatorId { get; }

    /// <summary>
    /// Gets the conversation being coordinated.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Conversation.ConversationIdKind>
        ConversationId { get; }

    /// <summary>
    /// Gets the participant who produced the current completed turn.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
        CurrentSpeakerEntityId { get; }

    /// <summary>
    /// Gets the resolved audience of the current completed turn.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Conversation.AddressAudience
        Audience { get; }

    /// <summary>
    /// Gets the authoritative world ID observed for this request.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId { get; }

    /// <summary>
    /// Gets the authoritative version observed for this request.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
        WorldStateVersion { get; }

    /// <summary>
    /// Gets the logical simulation tick observed for this request.
    /// </summary>
    public ulong SimulationTick { get; }

    /// <summary>
    /// Gets the expected current conversation revision.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Conversation.ConversationRevision
        ExpectedConversationRevision { get; }

    /// <summary>
    /// Gets proposals ordered by descending priority and then stable proposal
    /// ID.
    /// </summary>
    public IReadOnlyList<SocialTurnProposalEnvelope<TProposal>> Proposals =>
        proposals;

    /// <summary>
    /// Creates one immutable coordination request.
    /// </summary>
    /// <param name="coordinationId">The externally assigned coordination ID.</param>
    /// <param name="coordinatorId">The configured coordinator ID.</param>
    /// <param name="conversationId">The current conversation ID.</param>
    /// <param name="currentSpeakerEntityId">
    /// The speaker of the current completed turn.
    /// </param>
    /// <param name="audience">The current turn's resolved audience.</param>
    /// <param name="worldId">The observed authoritative world ID.</param>
    /// <param name="worldStateVersion">The observed authority version.</param>
    /// <param name="simulationTick">The observed logical tick.</param>
    /// <param name="expectedConversationRevision">
    /// The expected current conversation revision.
    /// </param>
    /// <param name="proposals">Zero through 63 participant proposals.</param>
    /// <returns>The validated immutable coordination request.</returns>
    public static SocialTurnCoordinationRequestEnvelope<TProposal> Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            SocialTurnCoordinationIdKind> coordinationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            SocialTurnCoordinatorIdKind> coordinatorId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Conversation.ConversationIdKind>
            conversationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
            currentSpeakerEntityId,
        global::AI.Sandbox.Engine.Core.Conversation.AddressAudience audience,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        global::AI.Sandbox.Engine.Core.Conversation.ConversationRevision
            expectedConversationRevision,
        IEnumerable<SocialTurnProposalEnvelope<TProposal>> proposals)
    {
        SocialTurnTypePolicy.EnsureExactType(
            typeof(TProposal),
            typeof(ISocialTurnProposal),
            "social turn proposal");

        EnsureNonEmpty(coordinationId, nameof(coordinationId));
        EnsureNonEmpty(coordinatorId, nameof(coordinatorId));
        EnsureNonEmpty(conversationId, nameof(conversationId));
        EnsureNonEmpty(
            currentSpeakerEntityId,
            nameof(currentSpeakerEntityId));
        EnsureNonEmpty(worldId, nameof(worldId));
        ArgumentNullException.ThrowIfNull(audience);
        ArgumentNullException.ThrowIfNull(proposals);

        if (!expectedConversationRevision.IsInitialized)
        {
            throw new ArgumentException(
                "The expected conversation revision must be initialized.",
                nameof(expectedConversationRevision));
        }

        var proposalArray = proposals.ToArray();
        if (proposalArray.Length > MaximumProposalCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(proposals),
                "Social turn requests may contain at most 63 proposals.");
        }

        foreach (var proposal in proposalArray)
        {
            ArgumentNullException.ThrowIfNull(proposal);
        }

        var ordered = proposalArray
            .OrderByDescending(
                static proposal => proposal.Priority.BasisPoints)
            .ThenBy(static proposal => proposal.ProposalId)
            .ToArray();

        var proposalIds = new HashSet<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                SocialTurnProposalIdKind>>();
        var participantIds = new HashSet<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>();

        foreach (var proposal in ordered)
        {
            if (!proposalIds.Add(proposal.ProposalId))
            {
                throw new ArgumentException(
                    "Social turn proposal IDs must be unique.",
                    nameof(proposals));
            }

            if (!participantIds.Add(proposal.ParticipantEntityId))
            {
                throw new ArgumentException(
                    "Each participant may submit at most one proposal.",
                    nameof(proposals));
            }
        }

        return new SocialTurnCoordinationRequestEnvelope<TProposal>(
            coordinationId,
            coordinatorId,
            conversationId,
            currentSpeakerEntityId,
            audience,
            worldId,
            worldStateVersion,
            simulationTick,
            expectedConversationRevision,
            ordered);
    }

    private static void EnsureNonEmpty<TKind>(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind> id,
        string parameterName)
    {
        if (id.IsEmpty)
        {
            throw new ArgumentException(
                "Identifiers must be non-empty.",
                parameterName);
        }
    }
}
