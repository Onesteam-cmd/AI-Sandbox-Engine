namespace AI.Sandbox.Engine.Core.Conversation;

/// <summary>
/// Resolves one exact semantic address query without mutating authority.
/// </summary>
/// <typeparam name="TWorldState">The authoritative world-state type.</typeparam>
/// <typeparam name="TQuery">The exact address-query payload type.</typeparam>
/// <typeparam name="TTopic">The exact conversation-topic payload type.</typeparam>
public interface IAddressResolver<TWorldState, TQuery, TTopic>
    where TWorldState : class,
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TQuery : IAddressQuery
    where TTopic : IConversationTopic
{
    /// <summary>
    /// Gets the stable resolver identity expected by requests.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<AddressResolverIdKind>
        ResolverId { get; }

    /// <summary>
    /// Resolves one immutable semantic address context exactly once.
    /// </summary>
    /// <param name="context">The immutable resolution context.</param>
    /// <returns>A resolved audience or explicit rejection.</returns>
    public AddressResolutionDecision Resolve(
        AddressResolutionContext<TWorldState, TQuery, TTopic> context);
}
