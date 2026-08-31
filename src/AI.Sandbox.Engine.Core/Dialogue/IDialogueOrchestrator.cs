namespace AI.Sandbox.Engine.Core.Dialogue;

/// <summary>
/// Defines one synchronous pure host-facing dialogue orchestrator.
/// </summary>
/// <typeparam name="TState">The authoritative world-state type.</typeparam>
/// <typeparam name="TInput">The exact input payload type.</typeparam>
/// <typeparam name="TTopic">The exact conversation-topic payload type.</typeparam>
/// <typeparam name="TDirective">The exact next-step directive type.</typeparam>
/// <typeparam name="TCompletion">The exact completion payload type.</typeparam>
public interface IDialogueOrchestrator<
    TState,
    TInput,
    TTopic,
    TDirective,
    TCompletion>
    where TState : class,
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TInput : IDialogueInput
    where TTopic : global::AI.Sandbox.Engine.Core.Conversation.IConversationTopic
    where TDirective : IDialogueDirective
    where TCompletion : IDialogueCompletion
{
    /// <summary>Gets the externally assigned stable orchestrator ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        DialogueOrchestratorIdKind> OrchestratorId { get; }

    /// <summary>
    /// Chooses exactly one next directive, completion, or explicit rejection.
    /// </summary>
    /// <param name="context">The stable read-only orchestration context.</param>
    /// <returns>One immutable decision.</returns>
    public DialogueOrchestrationDecision<TDirective, TCompletion> Decide(
        DialogueOrchestrationContext<TState, TInput, TTopic> context);
}
