namespace AI.Sandbox.Engine.Core.Prompting;

/// <summary>
/// Captures one immutable composed prompt document with explicit identity,
/// owner, world, composer, and final estimated cost.
/// </summary>
/// <typeparam name="TDocument">The exact prompt-document payload type.</typeparam>
public sealed record PromptDocumentEnvelope<TDocument>
    where TDocument : IPromptDocument
{
    private PromptDocumentEnvelope(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<PromptDocumentIdKind>
            documentId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<PromptComposerIdKind>
            composerId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        PromptCost cost,
        TDocument payload)
    {
        DocumentId = documentId;
        ComposerId = composerId;
        OwnerEntityId = ownerEntityId;
        WorldId = worldId;
        Cost = cost;
        Payload = payload;
    }

    /// <summary>
    /// Gets the externally assigned prompt-document ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<PromptDocumentIdKind>
        DocumentId { get; }

    /// <summary>
    /// Gets the exact composer that produced this document.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<PromptComposerIdKind>
        ComposerId { get; }

    /// <summary>
    /// Gets the subjective owner represented by the document.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OwnerEntityId
        { get; }

    /// <summary>
    /// Gets the authoritative world represented by the document.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId
        { get; }

    /// <summary>
    /// Gets the final positive estimated document cost.
    /// </summary>
    public PromptCost Cost { get; }

    /// <summary>
    /// Gets the exact composed-document payload.
    /// </summary>
    public TDocument Payload { get; }

    /// <summary>
    /// Creates one immutable composed prompt document.
    /// </summary>
    /// <param name="documentId">The externally assigned document ID.</param>
    /// <param name="composerId">The producing composer ID.</param>
    /// <param name="ownerEntityId">The subjective owner entity.</param>
    /// <param name="worldId">The authoritative world ID.</param>
    /// <param name="cost">The final positive estimated cost.</param>
    /// <param name="payload">The exact document payload.</param>
    /// <returns>The validated immutable document.</returns>
    public static PromptDocumentEnvelope<TDocument> Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<PromptDocumentIdKind>
            documentId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<PromptComposerIdKind>
            composerId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        PromptCost cost,
        TDocument payload)
    {
        PromptTypePolicy.EnsureExactType(
            typeof(TDocument),
            typeof(IPromptDocument),
            "prompt document");

        EnsureNonEmpty(documentId, nameof(documentId));
        EnsureNonEmpty(composerId, nameof(composerId));
        EnsureNonEmpty(ownerEntityId, nameof(ownerEntityId));
        EnsureNonEmpty(worldId, nameof(worldId));

        if (!cost.IsInitialized)
        {
            throw new ArgumentException(
                "Prompt document cost must be initialized.",
                nameof(cost));
        }

        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return new PromptDocumentEnvelope<TDocument>(
            documentId,
            composerId,
            ownerEntityId,
            worldId,
            cost,
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
                "Prompt identifiers cannot be empty.",
                parameterName);
        }
    }
}
