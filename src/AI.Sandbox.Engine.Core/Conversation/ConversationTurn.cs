namespace AI.Sandbox.Engine.Core.Conversation;

/// <summary>
/// Represents one immutable completed conversational turn.
/// </summary>
public sealed record ConversationTurn
{
    private ConversationTurn(
        ConversationTurnNumber turnNumber,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
            speakerEntityId,
        AddressAudience audience)
    {
        TurnNumber = turnNumber;
        SpeakerEntityId = speakerEntityId;
        Audience = audience;
    }

    /// <summary>
    /// Gets the positive sequential turn number.
    /// </summary>
    public ConversationTurnNumber TurnNumber { get; }

    /// <summary>
    /// Gets the participant who produced this turn.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
        SpeakerEntityId { get; }

    /// <summary>
    /// Gets the resolved response audience for this turn.
    /// </summary>
    public AddressAudience Audience { get; }

    /// <summary>
    /// Creates one immutable completed conversational turn.
    /// </summary>
    /// <param name="turnNumber">The positive sequential turn number.</param>
    /// <param name="speakerEntityId">The non-empty speaker entity ID.</param>
    /// <param name="audience">The resolved response audience.</param>
    /// <returns>The validated turn.</returns>
    public static ConversationTurn Create(
        ConversationTurnNumber turnNumber,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
            speakerEntityId,
        AddressAudience audience)
    {
        if (!turnNumber.IsInitialized)
        {
            throw new ArgumentException(
                "The conversation turn number must be initialized.",
                nameof(turnNumber));
        }

        if (speakerEntityId.IsEmpty)
        {
            throw new ArgumentException(
                "The speaker entity ID must be non-empty.",
                nameof(speakerEntityId));
        }

        ArgumentNullException.ThrowIfNull(audience);

        return new ConversationTurn(turnNumber, speakerEntityId, audience);
    }
}
