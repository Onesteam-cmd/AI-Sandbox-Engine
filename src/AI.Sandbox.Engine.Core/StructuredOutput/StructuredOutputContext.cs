namespace AI.Sandbox.Engine.Core.StructuredOutput;

/// <summary>
/// Supplies one decoder with a stable authoritative snapshot and one exact
/// completed model-response request.
/// </summary>
/// <typeparam name="TState">The immutable authoritative world-state type.</typeparam>
/// <typeparam name="TResponse">The exact model-response payload type.</typeparam>
public sealed class StructuredOutputContext<TState, TResponse>
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TResponse : global::AI.Sandbox.Engine.Core.Modeling.IModelResponse
{
    internal StructuredOutputContext(
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateSnapshot<TState>
            snapshot,
        StructuredOutputRequestEnvelope<TResponse> request,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            StructuredOutputDecoderIdKind> decoderId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            StructuredOutputSchemaIdKind> schemaId,
        StructuredOutputSchemaVersion schemaVersion)
    {
        Snapshot = snapshot;
        Request = request;
        DecoderId = decoderId;
        SchemaId = schemaId;
        SchemaVersion = schemaVersion;
    }

    /// <summary>
    /// Gets the stable authoritative snapshot used for decoding.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateSnapshot<TState>
        Snapshot { get; }

    /// <summary>
    /// Gets the exact structured-output request.
    /// </summary>
    public StructuredOutputRequestEnvelope<TResponse> Request { get; }

    /// <summary>
    /// Gets the configured decoder ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        StructuredOutputDecoderIdKind> DecoderId { get; }

    /// <summary>
    /// Gets the configured opaque schema ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        StructuredOutputSchemaIdKind> SchemaId { get; }

    /// <summary>
    /// Gets the configured positive schema version.
    /// </summary>
    public StructuredOutputSchemaVersion SchemaVersion { get; }
}
