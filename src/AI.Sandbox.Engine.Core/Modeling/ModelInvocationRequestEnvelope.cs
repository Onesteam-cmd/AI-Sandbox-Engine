namespace AI.Sandbox.Engine.Core.Modeling;

/// <summary>
/// Captures one immutable model invocation request with explicit prompt,
/// subject, authority, adapter, profile, and output-limit correlation.
/// </summary>
/// <typeparam name="TRequest">The exact model-request payload type.</typeparam>
public sealed record ModelInvocationRequestEnvelope<TRequest>
    where TRequest : IModelRequest
{
    private ModelInvocationRequestEnvelope(
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
        ModelOutputLimit outputLimit,
        TRequest payload)
    {
        InvocationId = invocationId;
        AdapterId = adapterId;
        ModelProfileId = modelProfileId;
        PromptDocumentId = promptDocumentId;
        OwnerEntityId = ownerEntityId;
        WorldId = worldId;
        WorldStateVersion = worldStateVersion;
        SimulationTick = simulationTick;
        OutputLimit = outputLimit;
        Payload = payload;
    }

    /// <summary>
    /// Gets the externally assigned invocation ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<ModelInvocationIdKind>
        InvocationId { get; }

    /// <summary>
    /// Gets the host-configured adapter selected for this invocation.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<ModelAdapterIdKind>
        AdapterId { get; }

    /// <summary>
    /// Gets the opaque host-configured model profile.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<ModelProfileIdKind>
        ModelProfileId { get; }

    /// <summary>
    /// Gets the source prompt document ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Prompting.PromptDocumentIdKind>
            PromptDocumentId { get; }

    /// <summary>
    /// Gets the subjective owner represented by the request.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OwnerEntityId
        { get; }

    /// <summary>
    /// Gets the authoritative world represented by the source prompt.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId
        { get; }

    /// <summary>
    /// Gets the authoritative version represented by the source prompt.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
        WorldStateVersion { get; }

    /// <summary>
    /// Gets the logical simulation tick represented by the source prompt.
    /// </summary>
    public ulong SimulationTick { get; }

    /// <summary>
    /// Gets the positive provider-neutral output limit.
    /// </summary>
    public ModelOutputLimit OutputLimit { get; }

    /// <summary>
    /// Gets the exact model-request payload.
    /// </summary>
    public TRequest Payload { get; }

    /// <summary>
    /// Creates one immutable provider-neutral model invocation request.
    /// </summary>
    /// <param name="invocationId">The externally assigned invocation ID.</param>
    /// <param name="adapterId">The selected adapter ID.</param>
    /// <param name="modelProfileId">The selected opaque model profile.</param>
    /// <param name="promptDocumentId">The source prompt-document ID.</param>
    /// <param name="ownerEntityId">The subjective owner entity.</param>
    /// <param name="worldId">The represented authoritative world.</param>
    /// <param name="worldStateVersion">The represented world version.</param>
    /// <param name="simulationTick">The represented logical tick.</param>
    /// <param name="outputLimit">The positive output limit.</param>
    /// <param name="payload">The exact request payload.</param>
    /// <returns>The validated immutable request.</returns>
    public static ModelInvocationRequestEnvelope<TRequest> Create(
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
        ModelOutputLimit outputLimit,
        TRequest payload)
    {
        ModelTypePolicy.EnsureExactType(
            typeof(TRequest),
            typeof(IModelRequest),
            "model request");

        EnsureNonEmpty(invocationId, nameof(invocationId));
        EnsureNonEmpty(adapterId, nameof(adapterId));
        EnsureNonEmpty(modelProfileId, nameof(modelProfileId));
        EnsureNonEmpty(promptDocumentId, nameof(promptDocumentId));
        EnsureNonEmpty(ownerEntityId, nameof(ownerEntityId));
        EnsureNonEmpty(worldId, nameof(worldId));

        if (!outputLimit.IsInitialized)
        {
            throw new ArgumentException(
                "The model output limit must be initialized.",
                nameof(outputLimit));
        }

        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return new ModelInvocationRequestEnvelope<TRequest>(
            invocationId,
            adapterId,
            modelProfileId,
            promptDocumentId,
            ownerEntityId,
            worldId,
            worldStateVersion,
            simulationTick,
            outputLimit,
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
                "Model invocation identifiers cannot be empty.",
                parameterName);
        }
    }
}
