namespace AI.Sandbox.Engine.Core.Social;

/// <summary>
/// Coordinates one exact set of social speaking-turn proposals without
/// mutating authority or maintaining a hidden queue.
/// </summary>
/// <typeparam name="TWorldState">The authoritative world-state type.</typeparam>
/// <typeparam name="TProposal">The exact proposal payload type.</typeparam>
/// <typeparam name="TTopic">The exact conversation-topic payload type.</typeparam>
public interface ISocialTurnCoordinator<TWorldState, TProposal, TTopic>
    where TWorldState : class,
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TProposal : ISocialTurnProposal
    where TTopic : global::AI.Sandbox.Engine.Core.Conversation.IConversationTopic
{
    /// <summary>
    /// Gets the stable coordinator identity expected by requests.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        SocialTurnCoordinatorIdKind> CoordinatorId { get; }

    /// <summary>
    /// Coordinates one immutable context exactly once.
    /// </summary>
    /// <param name="context">The immutable coordination context.</param>
    /// <returns>A grant, no-turn decision, or explicit rejection.</returns>
    public SocialTurnCoordinationDecision Coordinate(
        SocialTurnCoordinationContext<TWorldState, TProposal, TTopic> context);
}
