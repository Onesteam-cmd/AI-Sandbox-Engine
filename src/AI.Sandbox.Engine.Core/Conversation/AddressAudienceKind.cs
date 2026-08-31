namespace AI.Sandbox.Engine.Core.Conversation;

/// <summary>
/// Describes how one utterance addresses eligible conversation participants.
/// </summary>
public enum AddressAudienceKind
{
    /// <summary>No response addressee was selected.</summary>
    None = 0,

    /// <summary>One or more specific participants were selected.</summary>
    SpecificParticipants = 1,

    /// <summary>Every eligible participant except the speaker was selected.</summary>
    AllParticipants = 2,
}
