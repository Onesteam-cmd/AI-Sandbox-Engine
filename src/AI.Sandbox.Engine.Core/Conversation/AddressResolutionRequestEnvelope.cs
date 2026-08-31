namespace AI.Sandbox.Engine.Core.Conversation;

/// <summary>
/// Captures one immutable semantic address-resolution request.
/// </summary>
/// <typeparam name="TQuery">The exact address-query payload type.</typeparam>
public sealed record AddressResolutionRequestEnvelope<TQuery>
    where TQuery : IAddressQuery
{
    private AddressResolutionRequestEnvelope(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<AddressResolutionIdKind>
            resolutionId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<AddressResolverIdKind>
            resolverId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ConversationIdKind>
            conversationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
            speakerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        ConversationRevision expectedConversationRevision,
        TQuery payload)
    {
        ResolutionId = resolutionId;
        ResolverId = resolverId;
        ConversationId = conversationId;
        SpeakerEntityId = speakerEntityId;
        WorldId = worldId;
        WorldStateVersion = worldStateVersion;
        SimulationTick = simulationTick;
        ExpectedConversationRevision = expectedConversationRevision;
        Payload = payload;
    }

    /// <summary>
    /// Gets the externally assigned resolution ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<AddressResolutionIdKind>
        ResolutionId { get; }

    /// <summary>
    /// Gets the configured resolver ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<AddressResolverIdKind>
        ResolverId { get; }

    /// <summary>
    /// Gets the conversation being addressed.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<ConversationIdKind>
        ConversationId { get; }

    /// <summary>
    /// Gets the current speaker.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
        SpeakerEntityId { get; }

    /// <summary>
    /// Gets the authoritative world ID observed for this request.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId { get; }

    /// <summary>
    /// Gets the authoritative version observed for this request.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
        WorldStateVersion { get; }

    /// <summary>
    /// Gets the logical simulation tick observed for this request.
    /// </summary>
    public ulong SimulationTick { get; }

    /// <summary>
    /// Gets the expected current conversation revision.
    /// </summary>
    public ConversationRevision ExpectedConversationRevision { get; }

    /// <summary>
    /// Gets the exact semantic address-query payload.
    /// </summary>
    public TQuery Payload { get; }

    /// <summary>
    /// Creates one immutable semantic address-resolution request.
    /// </summary>
    /// <param name="resolutionId">The externally assigned resolution ID.</param>
    /// <param name="resolverId">The configured resolver ID.</param>
    /// <param name="conversationId">The target conversation ID.</param>
    /// <param name="speakerEntityId">The current speaker.</param>
    /// <param name="worldId">The observed authoritative world ID.</param>
    /// <param name="worldStateVersion">The observed authoritative version.</param>
    /// <param name="simulationTick">The observed logical tick.</param>
    /// <param name="expectedConversationRevision">
    /// The expected current conversation revision.
    /// </param>
    /// <param name="payload">The exact semantic query payload.</param>
    /// <returns>The validated immutable request.</returns>
    public static AddressResolutionRequestEnvelope<TQuery> Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<AddressResolutionIdKind>
            resolutionId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<AddressResolverIdKind>
            resolverId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ConversationIdKind>
            conversationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
            speakerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        ConversationRevision expectedConversationRevision,
        TQuery payload)
    {
        ConversationTypePolicy.EnsureExactType(
            typeof(TQuery),
            typeof(IAddressQuery),
            "address query");

        EnsureNonEmpty(resolutionId, nameof(resolutionId));
        EnsureNonEmpty(resolverId, nameof(resolverId));
        EnsureNonEmpty(conversationId, nameof(conversationId));
        EnsureNonEmpty(speakerEntityId, nameof(speakerEntityId));
        EnsureNonEmpty(worldId, nameof(worldId));

        if (!expectedConversationRevision.IsInitialized)
        {
            throw new ArgumentException(
                "The expected conversation revision must be initialized.",
                nameof(expectedConversationRevision));
        }

        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return new AddressResolutionRequestEnvelope<TQuery>(
            resolutionId,
            resolverId,
            conversationId,
            speakerEntityId,
            worldId,
            worldStateVersion,
            simulationTick,
            expectedConversationRevision,
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
