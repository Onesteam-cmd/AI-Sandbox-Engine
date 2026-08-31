namespace AI.Sandbox.Engine.Core.Dialogue;

/// <summary>
/// Captures one exact immutable dialogue artifact with complete exchange and
/// authority correlation.
/// </summary>
public sealed record DialogueArtifactEnvelope
{
    private DialogueArtifactEnvelope(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<DialogueArtifactIdKind>
            artifactId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            DialogueArtifactSourceIdKind> sourceId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<DialogueExchangeIdKind>
            exchangeId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Conversation.ConversationIdKind>
            conversationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
            perspectiveOwnerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        DialogueArtifactSequence sequence,
        IDialogueArtifact payload)
    {
        ArtifactId = artifactId;
        SourceId = sourceId;
        ExchangeId = exchangeId;
        ConversationId = conversationId;
        PerspectiveOwnerEntityId = perspectiveOwnerEntityId;
        WorldId = worldId;
        WorldStateVersion = worldStateVersion;
        SimulationTick = simulationTick;
        Sequence = sequence;
        Payload = payload;
    }

    /// <summary>Gets the externally assigned artifact ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<DialogueArtifactIdKind>
        ArtifactId { get; }

    /// <summary>Gets the opaque producer/source ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        DialogueArtifactSourceIdKind> SourceId { get; }

    /// <summary>Gets the dialogue exchange ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<DialogueExchangeIdKind>
        ExchangeId { get; }

    /// <summary>Gets the conversation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Conversation.ConversationIdKind>
        ConversationId { get; }

    /// <summary>Gets the subjective perspective owner.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
        PerspectiveOwnerEntityId { get; }

    /// <summary>Gets the authoritative world ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId { get; }

    /// <summary>Gets the represented World State version.</summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
        WorldStateVersion { get; }

    /// <summary>Gets the represented simulation tick.</summary>
    public ulong SimulationTick { get; }

    /// <summary>Gets the deterministic host-assigned artifact sequence.</summary>
    public DialogueArtifactSequence Sequence { get; }

    /// <summary>Gets the exact immutable artifact payload.</summary>
    public IDialogueArtifact Payload { get; }

    /// <summary>
    /// Creates one validated immutable dialogue artifact.
    /// </summary>
    /// <param name="artifactId">The externally assigned artifact ID.</param>
    /// <param name="sourceId">The opaque artifact producer ID.</param>
    /// <param name="exchangeId">The dialogue exchange ID.</param>
    /// <param name="conversationId">The conversation ID.</param>
    /// <param name="perspectiveOwnerEntityId">The perspective owner.</param>
    /// <param name="worldId">The authoritative world ID.</param>
    /// <param name="worldStateVersion">The represented state version.</param>
    /// <param name="simulationTick">The represented simulation tick.</param>
    /// <param name="sequence">The positive artifact sequence.</param>
    /// <param name="payload">The exact immutable artifact payload.</param>
    /// <returns>The validated artifact envelope.</returns>
    public static DialogueArtifactEnvelope Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<DialogueArtifactIdKind>
            artifactId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            DialogueArtifactSourceIdKind> sourceId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<DialogueExchangeIdKind>
            exchangeId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Conversation.ConversationIdKind>
            conversationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
            perspectiveOwnerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        DialogueArtifactSequence sequence,
        IDialogueArtifact payload)
    {
        EnsureNonEmpty(artifactId, nameof(artifactId));
        EnsureNonEmpty(sourceId, nameof(sourceId));
        EnsureNonEmpty(exchangeId, nameof(exchangeId));
        EnsureNonEmpty(conversationId, nameof(conversationId));
        EnsureNonEmpty(
            perspectiveOwnerEntityId,
            nameof(perspectiveOwnerEntityId));
        EnsureNonEmpty(worldId, nameof(worldId));
        if (!sequence.IsInitialized)
        {
            throw new ArgumentException(
                "The dialogue artifact sequence must be initialized.",
                nameof(sequence));
        }

        ArgumentNullException.ThrowIfNull(payload);
        DialogueTypePolicy.EnsureExactType(
            payload.GetType(),
            typeof(IDialogueArtifact),
            "dialogue artifact");

        return new DialogueArtifactEnvelope(
            artifactId,
            sourceId,
            exchangeId,
            conversationId,
            perspectiveOwnerEntityId,
            worldId,
            worldStateVersion,
            simulationTick,
            sequence,
            payload);
    }

    private static void EnsureNonEmpty<TKind>(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind> id,
        string parameterName)
    {
        if (id.IsEmpty)
        {
            throw new ArgumentException(
                "Identifiers must be non-empty.",
                parameterName);
        }
    }
}
