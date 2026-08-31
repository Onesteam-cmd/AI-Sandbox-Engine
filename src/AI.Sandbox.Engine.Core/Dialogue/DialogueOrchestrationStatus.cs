namespace AI.Sandbox.Engine.Core.Dialogue;

/// <summary>
/// Describes one explicit dialogue-orchestration processing outcome.
/// </summary>
public enum DialogueOrchestrationStatus
{
    /// <summary>One stable next-step directive was produced.</summary>
    Continued = 0,

    /// <summary>One stable completion payload was produced.</summary>
    Completed = 1,

    /// <summary>The orchestrator explicitly rejected the request.</summary>
    Rejected = 2,

    /// <summary>The request selected another orchestrator.</summary>
    OrchestratorMismatch = 3,

    /// <summary>The request belongs to another world.</summary>
    WorldMismatch = 4,

    /// <summary>The expected World State version is stale.</summary>
    VersionConflict = 5,

    /// <summary>The expected simulation tick is stale.</summary>
    SimulationTickMismatch = 6,

    /// <summary>The current conversation ID does not match.</summary>
    ConversationMismatch = 7,

    /// <summary>The expected conversation revision is stale.</summary>
    ConversationRevisionConflict = 8,

    /// <summary>The current conversation is closed.</summary>
    ConversationClosed = 9,

    /// <summary>The perspective owner is not a participant.</summary>
    PerspectiveOwnerNotParticipant = 10,

    /// <summary>The source speaker is not a participant.</summary>
    SourceSpeakerNotParticipant = 11,

    /// <summary>The supplied audience is invalid for the conversation.</summary>
    AudienceInvalid = 12,

    /// <summary>One or more supplied artifacts are invalid.</summary>
    ArtifactInvalid = 13,

    /// <summary>Authority changed while the orchestrator was evaluating.</summary>
    AuthorityChanged = 14,

    /// <summary>The orchestrator returned an invalid decision.</summary>
    DecisionInvalid = 15,
}
