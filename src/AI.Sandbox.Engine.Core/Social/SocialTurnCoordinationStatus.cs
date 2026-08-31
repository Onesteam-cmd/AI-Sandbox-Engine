namespace AI.Sandbox.Engine.Core.Social;

/// <summary>
/// Describes the validated outcome of one social turn-coordination attempt.
/// </summary>
public enum SocialTurnCoordinationStatus
{
    /// <summary>
    /// One valid proposal received the speaking floor.
    /// </summary>
    Granted = 0,

    /// <summary>
    /// The coordinator explicitly selected no next speaker.
    /// </summary>
    NoTurn = 1,

    /// <summary>
    /// The coordinator explicitly rejected the request.
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// No participant submitted a proposal, so the coordinator was skipped.
    /// </summary>
    NoEligibleProposals = 3,

    /// <summary>
    /// The request targets another configured coordinator.
    /// </summary>
    CoordinatorMismatch = 4,

    /// <summary>
    /// The request targets another world.
    /// </summary>
    WorldMismatch = 5,

    /// <summary>
    /// The request authority version is stale.
    /// </summary>
    VersionConflict = 6,

    /// <summary>
    /// The request logical tick is stale.
    /// </summary>
    SimulationTickMismatch = 7,

    /// <summary>
    /// The request targets another conversation.
    /// </summary>
    ConversationMismatch = 8,

    /// <summary>
    /// The supplied conversation belongs to another world.
    /// </summary>
    ConversationWorldMismatch = 9,

    /// <summary>
    /// The expected conversation revision is stale.
    /// </summary>
    ConversationRevisionConflict = 10,

    /// <summary>
    /// The conversation is already closed.
    /// </summary>
    ConversationClosed = 11,

    /// <summary>
    /// The conversation has no completed current turn.
    /// </summary>
    CurrentTurnMissing = 12,

    /// <summary>
    /// The request speaker does not match the current completed turn.
    /// </summary>
    CurrentSpeakerMismatch = 13,

    /// <summary>
    /// The request audience does not match the current completed turn.
    /// </summary>
    CurrentAudienceMismatch = 14,

    /// <summary>
    /// One or more proposals do not belong to eligible participants.
    /// </summary>
    ProposalInvalid = 15,

    /// <summary>
    /// Authoritative version or tick changed during coordination.
    /// </summary>
    AuthorityChanged = 16,

    /// <summary>
    /// A grant references no candidate proposal.
    /// </summary>
    SelectionInvalid = 17,
}
