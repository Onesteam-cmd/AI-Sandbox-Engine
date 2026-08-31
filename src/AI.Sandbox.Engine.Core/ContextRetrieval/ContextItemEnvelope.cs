namespace AI.Sandbox.Engine.Core.ContextRetrieval;

/// <summary>
/// Captures one immutable context item with explicit owner, world, retriever,
/// identity, and deterministic relevance.
/// </summary>
/// <typeparam name="TItem">The exact context-item payload type.</typeparam>
public sealed record ContextItemEnvelope<TItem>
    where TItem : IContextItem
{
    private ContextItemEnvelope(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ContextItemIdKind> itemId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ContextRetrieverIdKind>
            retrieverId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        ContextRelevance relevance,
        TItem payload)
    {
        ItemId = itemId;
        RetrieverId = retrieverId;
        OwnerEntityId = ownerEntityId;
        WorldId = worldId;
        Relevance = relevance;
        Payload = payload;
    }

    /// <summary>
    /// Gets the externally assigned stable context-item ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<ContextItemIdKind>
        ItemId { get; }

    /// <summary>
    /// Gets the exact retriever that produced this item.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<ContextRetrieverIdKind>
        RetrieverId { get; }

    /// <summary>
    /// Gets the owner of the subjective context item.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OwnerEntityId
        { get; }

    /// <summary>
    /// Gets the authoritative world to which this item belongs.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId
        { get; }

    /// <summary>
    /// Gets deterministic relevance assigned by the retriever.
    /// </summary>
    public ContextRelevance Relevance { get; }

    /// <summary>
    /// Gets the exact context-item payload.
    /// </summary>
    public TItem Payload { get; }

    /// <summary>
    /// Creates one immutable typed context item.
    /// </summary>
    /// <param name="itemId">The externally assigned item ID.</param>
    /// <param name="retrieverId">The producing retriever ID.</param>
    /// <param name="ownerEntityId">The subjective owner entity ID.</param>
    /// <param name="worldId">The authoritative world ID.</param>
    /// <param name="relevance">The deterministic item relevance.</param>
    /// <param name="payload">The exact item payload.</param>
    /// <returns>The validated immutable item.</returns>
    public static ContextItemEnvelope<TItem> Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ContextItemIdKind> itemId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ContextRetrieverIdKind>
            retrieverId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        ContextRelevance relevance,
        TItem payload)
    {
        ContextTypePolicy.EnsureExactType(
            typeof(TItem),
            typeof(IContextItem),
            "context item");

        EnsureNonEmpty(itemId, nameof(itemId));
        EnsureNonEmpty(retrieverId, nameof(retrieverId));
        EnsureNonEmpty(ownerEntityId, nameof(ownerEntityId));
        EnsureNonEmpty(worldId, nameof(worldId));

        if (!relevance.IsInitialized)
        {
            throw new ArgumentException(
                "Context relevance must be initialized.",
                nameof(relevance));
        }

        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return new ContextItemEnvelope<TItem>(
            itemId,
            retrieverId,
            ownerEntityId,
            worldId,
            relevance,
            payload);
    }

    private static void EnsureNonEmpty<TKind>(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind> id,
        string parameterName)
        where TKind : struct
    {
        if (id.IsEmpty)
        {
            throw new ArgumentException(
                "Context identifiers cannot be empty.",
                parameterName);
        }
    }
}
