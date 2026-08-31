namespace AI.Sandbox.Engine.Core.Social;

/// <summary>
/// Captures one immutable participant proposal to acquire the conversational
/// speaking floor.
/// </summary>
/// <typeparam name="TProposal">The exact social turn proposal payload.</typeparam>
public sealed record SocialTurnProposalEnvelope<TProposal>
    where TProposal : ISocialTurnProposal
{
    private SocialTurnProposalEnvelope(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<SocialTurnProposalIdKind>
            proposalId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
            participantEntityId,
        SocialTurnRequestKind requestKind,
        SocialTurnPriority priority,
        TProposal payload)
    {
        ProposalId = proposalId;
        ParticipantEntityId = participantEntityId;
        RequestKind = requestKind;
        Priority = priority;
        Payload = payload;
    }

    /// <summary>
    /// Gets the externally assigned proposal ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        SocialTurnProposalIdKind> ProposalId { get; }

    /// <summary>
    /// Gets the participant requesting the floor.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
        ParticipantEntityId { get; }

    /// <summary>
    /// Gets whether this is an ordinary response or interruption request.
    /// </summary>
    public SocialTurnRequestKind RequestKind { get; }

    /// <summary>
    /// Gets the initialized host-defined priority.
    /// </summary>
    public SocialTurnPriority Priority { get; }

    /// <summary>
    /// Gets the exact immutable semantic proposal payload.
    /// </summary>
    public TProposal Payload { get; }

    /// <summary>
    /// Creates one immutable social turn proposal.
    /// </summary>
    /// <param name="proposalId">The externally assigned proposal ID.</param>
    /// <param name="participantEntityId">The requesting participant.</param>
    /// <param name="requestKind">Response or interruption.</param>
    /// <param name="priority">Initialized host-defined priority.</param>
    /// <param name="payload">The exact immutable proposal payload.</param>
    /// <returns>The validated proposal envelope.</returns>
    public static SocialTurnProposalEnvelope<TProposal> Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<SocialTurnProposalIdKind>
            proposalId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
            participantEntityId,
        SocialTurnRequestKind requestKind,
        SocialTurnPriority priority,
        TProposal payload)
    {
        SocialTurnTypePolicy.EnsureExactType(
            typeof(TProposal),
            typeof(ISocialTurnProposal),
            "social turn proposal");

        if (proposalId.IsEmpty)
        {
            throw new ArgumentException(
                "The proposal ID must be non-empty.",
                nameof(proposalId));
        }

        if (participantEntityId.IsEmpty)
        {
            throw new ArgumentException(
                "The participant entity ID must be non-empty.",
                nameof(participantEntityId));
        }

        if (!Enum.IsDefined(requestKind))
        {
            throw new ArgumentOutOfRangeException(
                nameof(requestKind),
                requestKind,
                "The social turn request kind is not defined.");
        }

        if (!priority.IsInitialized)
        {
            throw new ArgumentException(
                "The social turn priority must be initialized.",
                nameof(priority));
        }

        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return new SocialTurnProposalEnvelope<TProposal>(
            proposalId,
            participantEntityId,
            requestKind,
            priority,
            payload);
    }
}
