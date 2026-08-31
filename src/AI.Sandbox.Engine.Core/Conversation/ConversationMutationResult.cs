namespace AI.Sandbox.Engine.Core.Conversation;

/// <summary>
/// Captures one explicit immutable conversation-state mutation result.
/// </summary>
/// <typeparam name="TTopic">The exact conversation-topic payload type.</typeparam>
public sealed record ConversationMutationResult<TTopic>
    where TTopic : IConversationTopic
{
    internal ConversationMutationResult(
        ConversationMutationStatus status,
        ConversationState<TTopic> state,
        bool wasChanged)
    {
        Status = status;
        State = state;
        WasChanged = wasChanged;
    }

    /// <summary>
    /// Gets the explicit mutation status.
    /// </summary>
    public ConversationMutationStatus Status { get; }

    /// <summary>
    /// Gets the resulting immutable state.
    /// </summary>
    public ConversationState<TTopic> State { get; }

    /// <summary>
    /// Gets a value indicating whether a new state was produced.
    /// </summary>
    public bool WasChanged { get; }
}
