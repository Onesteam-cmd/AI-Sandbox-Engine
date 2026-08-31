namespace AI.Sandbox.Engine.Core.Conversation;

/// <summary>
/// Describes one pure conversation-state mutation outcome.
/// </summary>
public enum ConversationMutationStatus
{
    /// <summary>A completed turn was recorded.</summary>
    TurnRecorded = 0,

    /// <summary>The current exact topic payload was revised.</summary>
    TopicRevised = 1,

    /// <summary>The conversation was closed.</summary>
    Closed = 2,

    /// <summary>The requested state was already current.</summary>
    Unchanged = 3,

    /// <summary>The expected conversation revision was stale.</summary>
    RevisionConflict = 4,

    /// <summary>The conversation is already closed.</summary>
    ConversationClosed = 5,

    /// <summary>The supplied turn number was not the next sequential value.</summary>
    TurnSequenceConflict = 6,

    /// <summary>The supplied speaker is not a participant.</summary>
    SpeakerNotParticipant = 7,

    /// <summary>The supplied audience is invalid for the current roster.</summary>
    AudienceInvalid = 8,
}
