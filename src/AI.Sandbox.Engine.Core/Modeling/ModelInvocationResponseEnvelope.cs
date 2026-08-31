namespace AI.Sandbox.Engine.Core.Modeling;

/// <summary>
/// Captures one immutable completed model response with full request
/// correlation and adapter-reported usage.
/// </summary>
/// <typeparam name="TResponse">The exact model-response payload type.</typeparam>
public sealed record ModelInvocationResponseEnvelope<TResponse>
    where TResponse : IModelResponse
{
    private ModelInvocationResponseEnvelope(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ModelInvocationIdKind>
            invocationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ModelAdapterIdKind>
            adapterId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ModelProfileIdKind>
            modelProfileId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Prompting.PromptDocumentIdKind>
                promptDocumentId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        ModelUsage usage,
        TResponse payload)
    {
        InvocationId = invocationId;
        AdapterId = adapterId;
        ModelProfileId = modelProfileId;
        PromptDocumentId = promptDocumentId;
        OwnerEntityId = ownerEntityId;
        WorldId = worldId;
        WorldStateVersion = worldStateVersion;
        SimulationTick = simulationTick;
        Usage = usage;
        Payload = payload;
    }

    /// <summary>
    /// Gets the correlated invocation ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<ModelInvocationIdKind>
        InvocationId { get; }

    /// <summary>
    /// Gets the adapter that produced the response.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<ModelAdapterIdKind>
        AdapterId { get; }

    /// <summary>
    /// Gets the opaque model profile used by the adapter.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<ModelProfileIdKind>
        ModelProfileId { get; }

    /// <summary>
    /// Gets the correlated source prompt-document ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Prompting.PromptDocumentIdKind>
            PromptDocumentId { get; }

    /// <summary>
    /// Gets the correlated subjective owner.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OwnerEntityId
        { get; }

    /// <summary>
    /// Gets the correlated authoritative world.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId
        { get; }

    /// <summary>
    /// Gets the correlated authoritative version.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
        WorldStateVersion { get; }

    /// <summary>
    /// Gets the correlated logical simulation tick.
    /// </summary>
    public ulong SimulationTick { get; }

    /// <summary>
    /// Gets the initialized adapter-reported usage.
    /// </summary>
    public ModelUsage Usage { get; }

    /// <summary>
    /// Gets the exact model-response payload.
    /// </summary>
    public TResponse Payload { get; }

    /// <summary>
    /// Creates one immutable completed model response.
    /// </summary>
    /// <param name="invocationId">The correlated invocation ID.</param>
    /// <param name="adapterId">The producing adapter ID.</param>
    /// <param name="modelProfileId">The used opaque model profile.</param>
    /// <param name="promptDocumentId">The source prompt-document ID.</param>
    /// <param name="ownerEntityId">The correlated subjective owner.</param>
    /// <param name="worldId">The correlated authoritative world.</param>
    /// <param name="worldStateVersion">The correlated world version.</param>
    /// <param name="simulationTick">The correlated logical tick.</param>
    /// <param name="usage">The initialized adapter-reported usage.</param>
    /// <param name="payload">The exact response payload.</param>
    /// <returns>The validated immutable response.</returns>
    public static ModelInvocationResponseEnvelope<TResponse> Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ModelInvocationIdKind>
            invocationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ModelAdapterIdKind>
            adapterId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ModelProfileIdKind>
            modelProfileId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Prompting.PromptDocumentIdKind>
                promptDocumentId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        ModelUsage usage,
        TResponse payload)
    {
        ModelTypePolicy.EnsureExactType(
            typeof(TResponse),
            typeof(IModelResponse),
            "model response");

        EnsureNonEmpty(invocationId, nameof(invocationId));
        EnsureNonEmpty(adapterId, nameof(adapterId));
        EnsureNonEmpty(modelProfileId, nameof(modelProfileId));
        EnsureNonEmpty(promptDocumentId, nameof(promptDocumentId));
        EnsureNonEmpty(ownerEntityId, nameof(ownerEntityId));
        EnsureNonEmpty(worldId, nameof(worldId));

        if (!usage.IsInitialized)
        {
            throw new ArgumentException(
                "The model usage value must be initialized.",
                nameof(usage));
        }

        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return new ModelInvocationResponseEnvelope<TResponse>(
            invocationId,
            adapterId,
            modelProfileId,
            promptDocumentId,
            ownerEntityId,
            worldId,
            worldStateVersion,
            simulationTick,
            usage,
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
                "Model response identifiers cannot be empty.",
                parameterName);
        }
    }
}
