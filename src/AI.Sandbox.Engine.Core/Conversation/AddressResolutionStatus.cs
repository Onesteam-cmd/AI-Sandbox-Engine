namespace AI.Sandbox.Engine.Core.Conversation;

/// <summary>
/// Describes one complete address-resolution processor outcome.
/// </summary>
public enum AddressResolutionStatus
{
    /// <summary>A stable audience was resolved and validated.</summary>
    Resolved = 0,

    /// <summary>The resolver explicitly rejected the query.</summary>
    Rejected = 1,

    /// <summary>The request named a different resolver.</summary>
    ResolverMismatch = 2,

    /// <summary>The request world did not match current authority.</summary>
    WorldMismatch = 3,

    /// <summary>The request world version was stale.</summary>
    VersionConflict = 4,

    /// <summary>The request simulation tick was stale.</summary>
    SimulationTickMismatch = 5,

    /// <summary>The supplied conversation ID did not match the state.</summary>
    ConversationMismatch = 6,

    /// <summary>The supplied conversation state belongs to another world.</summary>
    ConversationWorldMismatch = 7,

    /// <summary>The expected conversation revision was stale.</summary>
    ConversationRevisionConflict = 8,

    /// <summary>The supplied conversation was already closed.</summary>
    ConversationClosed = 9,

    /// <summary>The current speaker is not a conversation participant.</summary>
    SpeakerNotParticipant = 10,

    /// <summary>Authority changed while the resolver was executing.</summary>
    AuthorityChanged = 11,

    /// <summary>The resolver returned an invalid audience.</summary>
    AudienceInvalid = 12,
}
