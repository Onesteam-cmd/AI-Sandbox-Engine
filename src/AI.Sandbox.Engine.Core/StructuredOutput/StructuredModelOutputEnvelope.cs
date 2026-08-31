namespace AI.Sandbox.Engine.Core.StructuredOutput;

/// <summary>
/// Captures one validated exact structured model output with complete source
/// model-response and schema correlation.
/// </summary>
/// <typeparam name="TOutput">The exact structured-output payload type.</typeparam>
public sealed record StructuredModelOutputEnvelope<TOutput>
    where TOutput : IStructuredModelOutput
{
    private StructuredModelOutputEnvelope(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<StructuredOutputIdKind>
            outputId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            StructuredOutputDecoderIdKind> decoderId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            StructuredOutputSchemaIdKind> schemaId,
        StructuredOutputSchemaVersion schemaVersion,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Modeling.ModelInvocationIdKind>
                sourceInvocationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Modeling.ModelAdapterIdKind>
                sourceAdapterId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Modeling.ModelProfileIdKind>
                sourceModelProfileId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Prompting.PromptDocumentIdKind>
                sourcePromptDocumentId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        global::AI.Sandbox.Engine.Core.Modeling.ModelUsage sourceUsage,
        TOutput payload)
    {
        OutputId = outputId;
        DecoderId = decoderId;
        SchemaId = schemaId;
        SchemaVersion = schemaVersion;
        SourceInvocationId = sourceInvocationId;
        SourceAdapterId = sourceAdapterId;
        SourceModelProfileId = sourceModelProfileId;
        SourcePromptDocumentId = sourcePromptDocumentId;
        OwnerEntityId = ownerEntityId;
        WorldId = worldId;
        WorldStateVersion = worldStateVersion;
        SimulationTick = simulationTick;
        SourceUsage = sourceUsage;
        Payload = payload;
    }

    /// <summary>
    /// Gets the externally assigned structured-output operation ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<StructuredOutputIdKind>
        OutputId { get; }

    /// <summary>
    /// Gets the decoder that produced this structured payload.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        StructuredOutputDecoderIdKind> DecoderId { get; }

    /// <summary>
    /// Gets the opaque schema ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        StructuredOutputSchemaIdKind> SchemaId { get; }

    /// <summary>
    /// Gets the positive schema version.
    /// </summary>
    public StructuredOutputSchemaVersion SchemaVersion { get; }

    /// <summary>
    /// Gets the source model invocation ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Modeling.ModelInvocationIdKind>
            SourceInvocationId { get; }

    /// <summary>
    /// Gets the source model adapter ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Modeling.ModelAdapterIdKind>
            SourceAdapterId { get; }

    /// <summary>
    /// Gets the source model profile ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Modeling.ModelProfileIdKind>
            SourceModelProfileId { get; }

    /// <summary>
    /// Gets the source prompt-document ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Prompting.PromptDocumentIdKind>
            SourcePromptDocumentId { get; }

    /// <summary>
    /// Gets the subjective owner represented by the output.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OwnerEntityId
        { get; }

    /// <summary>
    /// Gets the authoritative world represented by the output.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId
        { get; }

    /// <summary>
    /// Gets the authoritative version represented by the output.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
        WorldStateVersion { get; }

    /// <summary>
    /// Gets the logical simulation tick represented by the output.
    /// </summary>
    public ulong SimulationTick { get; }

    /// <summary>
    /// Gets the adapter-reported usage from the source response.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Modeling.ModelUsage SourceUsage
        { get; }

    /// <summary>
    /// Gets the exact decoded structured payload.
    /// </summary>
    public TOutput Payload { get; }

    internal static StructuredModelOutputEnvelope<TOutput> Create<TResponse>(
        StructuredOutputRequestEnvelope<TResponse> request,
        TOutput payload)
        where TResponse :
            global::AI.Sandbox.Engine.Core.Modeling.IModelResponse
    {
        ArgumentNullException.ThrowIfNull(request);
        StructuredOutputTypePolicy.EnsureExactType(
            typeof(TOutput),
            typeof(IStructuredModelOutput),
            "structured model output");

        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        var response = request.SourceResponse;

        return new StructuredModelOutputEnvelope<TOutput>(
            request.OutputId,
            request.DecoderId,
            request.SchemaId,
            request.SchemaVersion,
            response.InvocationId,
            response.AdapterId,
            response.ModelProfileId,
            response.PromptDocumentId,
            response.OwnerEntityId,
            response.WorldId,
            response.WorldStateVersion,
            response.SimulationTick,
            response.Usage,
            payload);
    }
}
