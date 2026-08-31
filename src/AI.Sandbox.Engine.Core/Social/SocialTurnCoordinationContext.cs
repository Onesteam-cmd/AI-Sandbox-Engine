namespace AI.Sandbox.Engine.Core.Social;

/// <summary>
/// Supplies one exact coordinator with a stable authority snapshot,
/// conversation state, current audience, and deterministic proposals.
/// </summary>
/// <typeparam name="TWorldState">The authoritative world-state type.</typeparam>
/// <typeparam name="TProposal">The exact proposal payload type.</typeparam>
/// <typeparam name="TTopic">The exact conversation-topic payload type.</typeparam>
public sealed class SocialTurnCoordinationContext<
    TWorldState,
    TProposal,
    TTopic>
    where TWorldState : class,
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TProposal : ISocialTurnProposal
    where TTopic : global::AI.Sandbox.Engine.Core.Conversation.IConversationTopic
{
    internal SocialTurnCoordinationContext(
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateSnapshot<TWorldState>
            snapshot,
        SocialTurnCoordinationRequestEnvelope<TProposal> request,
        global::AI.Sandbox.Engine.Core.Conversation.ConversationState<TTopic>
            conversation)
    {
        Snapshot = snapshot;
        Request = request;
        Conversation = conversation;
    }

    /// <summary>
    /// Gets the stable authoritative snapshot observed before coordination.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateSnapshot<TWorldState> Snapshot { get; }

    /// <summary>
    /// Gets the immutable coordination request.
    /// </summary>
    public SocialTurnCoordinationRequestEnvelope<TProposal> Request { get; }

    /// <summary>
    /// Gets the immutable current conversation state.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Conversation.ConversationState<TTopic>
        Conversation { get; }

    /// <summary>
    /// Gets deterministic candidate proposals.
    /// </summary>
    public IReadOnlyList<SocialTurnProposalEnvelope<TProposal>> Proposals =>
        Request.Proposals;
}
