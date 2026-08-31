namespace AI.Sandbox.Engine.Core.Dialogue;

/// <summary>
/// Supplies one exact orchestrator with a stable authority snapshot, current
/// conversation state, and immutable exchange request.
/// </summary>
/// <typeparam name="TState">The authoritative world-state type.</typeparam>
/// <typeparam name="TInput">The exact input payload type.</typeparam>
/// <typeparam name="TTopic">The exact conversation-topic payload type.</typeparam>
public sealed class DialogueOrchestrationContext<TState, TInput, TTopic>
    where TState : class,
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TInput : IDialogueInput
    where TTopic : global::AI.Sandbox.Engine.Core.Conversation.IConversationTopic
{
    internal DialogueOrchestrationContext(
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateSnapshot<TState>
            snapshot,
        DialogueOrchestrationRequestEnvelope<TInput> request,
        global::AI.Sandbox.Engine.Core.Conversation.ConversationState<TTopic>
            conversation)
    {
        Snapshot = snapshot;
        Request = request;
        Conversation = conversation;
    }

    /// <summary>Gets the stable authority snapshot read before orchestration.</summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateSnapshot<TState>
        Snapshot { get; }

    /// <summary>Gets the immutable orchestration request.</summary>
    public DialogueOrchestrationRequestEnvelope<TInput> Request { get; }

    /// <summary>Gets the immutable current conversation state.</summary>
    public global::AI.Sandbox.Engine.Core.Conversation.ConversationState<TTopic>
        Conversation { get; }

    /// <summary>Gets deterministic prior exchange artifacts.</summary>
    public IReadOnlyList<DialogueArtifactEnvelope> Artifacts =>
        Request.Artifacts;
}
