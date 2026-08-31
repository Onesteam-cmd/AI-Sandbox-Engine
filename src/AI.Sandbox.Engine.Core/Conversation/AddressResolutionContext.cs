namespace AI.Sandbox.Engine.Core.Conversation;

/// <summary>
/// Supplies one exact resolver with an immutable authority snapshot,
/// conversation state, semantic query, and eligible candidates.
/// </summary>
/// <typeparam name="TWorldState">The authoritative world-state type.</typeparam>
/// <typeparam name="TQuery">The exact address-query payload type.</typeparam>
/// <typeparam name="TTopic">The exact conversation-topic payload type.</typeparam>
public sealed class AddressResolutionContext<TWorldState, TQuery, TTopic>
    where TWorldState : class,
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TQuery : IAddressQuery
    where TTopic : IConversationTopic
{
    private readonly global::System.Collections.ObjectModel.ReadOnlyCollection<
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>
        candidateEntityIds;

    internal AddressResolutionContext(
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateSnapshot<TWorldState>
            snapshot,
        AddressResolutionRequestEnvelope<TQuery> request,
        ConversationState<TTopic> conversation,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>[] candidates)
    {
        Snapshot = snapshot;
        Request = request;
        Conversation = conversation;
        candidateEntityIds = Array.AsReadOnly(candidates);
    }

    /// <summary>
    /// Gets the stable authoritative snapshot observed before resolution.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateSnapshot<TWorldState> Snapshot { get; }

    /// <summary>
    /// Gets the immutable address-resolution request.
    /// </summary>
    public AddressResolutionRequestEnvelope<TQuery> Request { get; }

    /// <summary>
    /// Gets the immutable current conversation state.
    /// </summary>
    public ConversationState<TTopic> Conversation { get; }

    /// <summary>
    /// Gets deterministically ordered eligible addressee candidates.
    /// </summary>
    public IReadOnlyList<
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>
        CandidateEntityIds => candidateEntityIds;
}
