namespace AI.Sandbox.Engine.Core.Dialogue;

/// <summary>
/// Captures one explicit immutable dialogue-orchestration result.
/// </summary>
/// <typeparam name="TDirective">The exact directive payload type.</typeparam>
/// <typeparam name="TCompletion">The exact completion payload type.</typeparam>
public sealed record DialogueOrchestrationResult<TDirective, TCompletion>
    where TDirective : IDialogueDirective
    where TCompletion : IDialogueCompletion
{
    internal DialogueOrchestrationResult(
        DialogueOrchestrationStatus status,
        bool orchestratorWasInvoked,
        bool hasStableDecision,
        DialogueOrchestrationDecision<TDirective, TCompletion>? decision)
    {
        Status = status;
        OrchestratorWasInvoked = orchestratorWasInvoked;
        HasStableDecision = hasStableDecision;
        Decision = decision;
    }

    /// <summary>Gets the explicit processing status.</summary>
    public DialogueOrchestrationStatus Status { get; }

    /// <summary>Gets whether the orchestrator was invoked.</summary>
    public bool OrchestratorWasInvoked { get; }

    /// <summary>Gets whether one stable validated decision is present.</summary>
    public bool HasStableDecision { get; }

    /// <summary>Gets the stable decision when present.</summary>
    public DialogueOrchestrationDecision<TDirective, TCompletion>? Decision
        { get; }

    /// <summary>Gets whether a next-step directive was produced.</summary>
    public bool WasContinued => Status == DialogueOrchestrationStatus.Continued;

    /// <summary>Gets whether a completion payload was produced.</summary>
    public bool WasCompleted => Status == DialogueOrchestrationStatus.Completed;
}
