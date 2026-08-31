namespace AI.Sandbox.Engine.Core.StructuredOutput;

/// <summary>
/// Invokes one exact structured-output decoder once against a stable
/// authoritative snapshot and validates the resulting payload correlation.
/// </summary>
/// <typeparam name="TState">The immutable authoritative world-state type.</typeparam>
/// <typeparam name="TResponse">The exact model-response payload type.</typeparam>
/// <typeparam name="TOutput">The exact structured-output payload type.</typeparam>
public sealed class StructuredOutputProcessor<TState, TResponse, TOutput>
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TResponse : global::AI.Sandbox.Engine.Core.Modeling.IModelResponse
    where TOutput : IStructuredModelOutput
{
    private readonly global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<TState> worldStateManager;
    private readonly global::AI.Sandbox.Engine.Core.Identifiers.Id<
        StructuredOutputDecoderIdKind> decoderId;
    private readonly global::AI.Sandbox.Engine.Core.Identifiers.Id<
        StructuredOutputSchemaIdKind> schemaId;
    private readonly StructuredOutputSchemaVersion schemaVersion;
    private readonly IStructuredOutputDecoder<TState, TResponse, TOutput>
        decoder;

    private StructuredOutputProcessor(
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateManager<TState>
            worldStateManager,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            StructuredOutputDecoderIdKind> decoderId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            StructuredOutputSchemaIdKind> schemaId,
        StructuredOutputSchemaVersion schemaVersion,
        IStructuredOutputDecoder<TState, TResponse, TOutput> decoder)
    {
        this.worldStateManager = worldStateManager;
        this.decoderId = decoderId;
        this.schemaId = schemaId;
        this.schemaVersion = schemaVersion;
        this.decoder = decoder;
    }

    /// <summary>
    /// Creates a processor bound to one authority manager, decoder, and schema.
    /// </summary>
    /// <param name="worldStateManager">The authoritative World State manager.</param>
    /// <param name="decoderId">The stable exact decoder ID.</param>
    /// <param name="schemaId">The opaque host-defined schema ID.</param>
    /// <param name="schemaVersion">The positive schema version.</param>
    /// <param name="decoder">The synchronous pure exact decoder.</param>
    /// <returns>The configured processor.</returns>
    public static StructuredOutputProcessor<TState, TResponse, TOutput> Create(
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateManager<TState>
            worldStateManager,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            StructuredOutputDecoderIdKind> decoderId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            StructuredOutputSchemaIdKind> schemaId,
        StructuredOutputSchemaVersion schemaVersion,
        IStructuredOutputDecoder<TState, TResponse, TOutput> decoder)
    {
        ArgumentNullException.ThrowIfNull(worldStateManager);
        ArgumentNullException.ThrowIfNull(decoder);

        if (decoderId.IsEmpty)
        {
            throw new ArgumentException(
                "The structured-output decoder ID cannot be empty.",
                nameof(decoderId));
        }

        if (schemaId.IsEmpty)
        {
            throw new ArgumentException(
                "The structured-output schema ID cannot be empty.",
                nameof(schemaId));
        }

        if (!schemaVersion.IsInitialized)
        {
            throw new ArgumentException(
                "The structured-output schema version must be initialized.",
                nameof(schemaVersion));
        }

        StructuredOutputTypePolicy.EnsureExactType(
            typeof(TResponse),
            typeof(global::AI.Sandbox.Engine.Core.Modeling.IModelResponse),
            "model response");
        StructuredOutputTypePolicy.EnsureExactType(
            typeof(TOutput),
            typeof(IStructuredModelOutput),
            "structured model output");

        return new StructuredOutputProcessor<TState, TResponse, TOutput>(
            worldStateManager,
            decoderId,
            schemaId,
            schemaVersion,
            decoder);
    }

    /// <summary>
    /// Decodes one completed model response at most once when its authority
    /// coordinates and schema selection remain current.
    /// </summary>
    /// <param name="request">The exact structured-output request.</param>
    /// <returns>The explicit processing result.</returns>
    public StructuredOutputProcessingResult<TResponse, TOutput> Process(
        StructuredOutputRequestEnvelope<TResponse> request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.DecoderId != decoderId)
        {
            return StructuredOutputProcessingResult<TResponse, TOutput>
                .NotEvaluated(
                    StructuredOutputProcessingStatus.DecoderMismatch,
                    request);
        }

        if (request.SchemaId != schemaId)
        {
            return StructuredOutputProcessingResult<TResponse, TOutput>
                .NotEvaluated(
                    StructuredOutputProcessingStatus.SchemaMismatch,
                    request);
        }

        if (request.SchemaVersion != schemaVersion)
        {
            return StructuredOutputProcessingResult<TResponse, TOutput>
                .NotEvaluated(
                    StructuredOutputProcessingStatus.SchemaVersionMismatch,
                    request);
        }

        var before = worldStateManager.Read();
        var response = request.SourceResponse;

        if (response.WorldId != before.WorldId)
        {
            return StructuredOutputProcessingResult<TResponse, TOutput>
                .NotEvaluated(
                    StructuredOutputProcessingStatus.WorldMismatch,
                    request);
        }

        if (response.WorldStateVersion != before.Version)
        {
            return StructuredOutputProcessingResult<TResponse, TOutput>
                .NotEvaluated(
                    StructuredOutputProcessingStatus.VersionConflict,
                    request);
        }

        if (response.SimulationTick != before.SimulationTick)
        {
            return StructuredOutputProcessingResult<TResponse, TOutput>
                .NotEvaluated(
                    StructuredOutputProcessingStatus.SimulationTickMismatch,
                    request);
        }

        var context = new StructuredOutputContext<TState, TResponse>(
            before,
            request,
            decoderId,
            schemaId,
            schemaVersion);
        var decision = decoder.Decode(context) ??
            throw new InvalidOperationException(
                "Structured-output decoders cannot return null decisions.");

        var after = worldStateManager.Read();
        if (after.WorldId != before.WorldId ||
            after.Version != before.Version ||
            after.SimulationTick != before.SimulationTick)
        {
            return StructuredOutputProcessingResult<TResponse, TOutput>
                .Discarded(request);
        }

        if (decision.Status == StructuredOutputDecisionStatus.Rejected)
        {
            return StructuredOutputProcessingResult<TResponse, TOutput>
                .Evaluated(
                    StructuredOutputProcessingStatus.Rejected,
                    request,
                    decision,
                    null);
        }

        if (decision.Status != StructuredOutputDecisionStatus.Decoded)
        {
            throw new InvalidOperationException(
                "Unknown structured-output decoder decision status.");
        }

        var output = StructuredModelOutputEnvelope<TOutput>.Create(
            request,
            decision.Payload);

        return StructuredOutputProcessingResult<TResponse, TOutput>.Evaluated(
            StructuredOutputProcessingStatus.Decoded,
            request,
            decision,
            output);
    }
}
