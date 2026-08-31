namespace AI.Sandbox.Engine.Core.Dialogue;

/// <summary>
/// Describes one pure dialogue-orchestrator decision.
/// </summary>
public enum DialogueOrchestrationDecisionStatus
{
    /// <summary>Host orchestration should continue with one exact directive.</summary>
    Continue = 0,

    /// <summary>The dialogue exchange is complete with one exact payload.</summary>
    Complete = 1,

    /// <summary>The orchestration request was explicitly rejected.</summary>
    Rejected = 2,
}
