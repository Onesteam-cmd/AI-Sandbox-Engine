namespace AI.Sandbox.Engine.Core.StructuredOutput;

/// <summary>
/// Captures one immutable request to decode a completed model response using
/// one exact host-defined schema.
/// </summary>
/// <typeparam name="TResponse">The exact model-response payload type.</typeparam>
public sealed record StructuredOutputRequestEnvelope<TResponse>
    where TResponse : global::AI.Sandbox.Engine.Core.Modeling.IModelResponse
{
    private StructuredOutputRequestEnvelope(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<StructuredOutputIdKind>
            outputId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            StructuredOutputDecoderIdKind> decoderId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            StructuredOutputSchemaIdKind> schemaId,
        StructuredOutputSchemaVersion schemaVersion,
        global::AI.Sandbox.Engine.Core.Modeling
            .ModelInvocationResponseEnvelope<TResponse> sourceResponse)
    {
        OutputId = outputId;
        DecoderId = decoderId;
        SchemaId = schemaId;
        SchemaVersion = schemaVersion;
        SourceResponse = sourceResponse;
    }

    /// <summary>
    /// Gets the externally assigned structured-output operation ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<StructuredOutputIdKind>
        OutputId { get; }

    /// <summary>
    /// Gets the exact decoder selected by the host.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        StructuredOutputDecoderIdKind> DecoderId { get; }

    /// <summary>
    /// Gets the opaque host-defined schema ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        StructuredOutputSchemaIdKind> SchemaId { get; }

    /// <summary>
    /// Gets the positive host-defined schema version.
    /// </summary>
    public StructuredOutputSchemaVersion SchemaVersion { get; }

    /// <summary>
    /// Gets the completed provider-neutral model response being decoded.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Modeling
        .ModelInvocationResponseEnvelope<TResponse> SourceResponse { get; }

    /// <summary>
    /// Creates one immutable structured-output decoding request.
    /// </summary>
    /// <param name="outputId">The externally assigned operation ID.</param>
    /// <param name="decoderId">The configured exact decoder ID.</param>
    /// <param name="schemaId">The opaque host-defined schema ID.</param>
    /// <param name="schemaVersion">The positive schema version.</param>
    /// <param name="sourceResponse">The completed model response.</param>
    /// <returns>The validated immutable request.</returns>
    public static StructuredOutputRequestEnvelope<TResponse> Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<StructuredOutputIdKind>
            outputId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            StructuredOutputDecoderIdKind> decoderId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            StructuredOutputSchemaIdKind> schemaId,
        StructuredOutputSchemaVersion schemaVersion,
        global::AI.Sandbox.Engine.Core.Modeling
            .ModelInvocationResponseEnvelope<TResponse> sourceResponse)
    {
        StructuredOutputTypePolicy.EnsureExactType(
            typeof(TResponse),
            typeof(global::AI.Sandbox.Engine.Core.Modeling.IModelResponse),
            "model response");

        EnsureNonEmpty(outputId, nameof(outputId));
        EnsureNonEmpty(decoderId, nameof(decoderId));
        EnsureNonEmpty(schemaId, nameof(schemaId));

        if (!schemaVersion.IsInitialized)
        {
            throw new ArgumentException(
                "The structured-output schema version must be initialized.",
                nameof(schemaVersion));
        }

        ArgumentNullException.ThrowIfNull(sourceResponse);

        return new StructuredOutputRequestEnvelope<TResponse>(
            outputId,
            decoderId,
            schemaId,
            schemaVersion,
            sourceResponse);
    }

    private static void EnsureNonEmpty<TKind>(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind> id,
        string parameterName)
        where TKind : struct
    {
        if (id.IsEmpty)
        {
            throw new ArgumentException(
                "Structured-output identifiers cannot be empty.",
                parameterName);
        }
    }
}
