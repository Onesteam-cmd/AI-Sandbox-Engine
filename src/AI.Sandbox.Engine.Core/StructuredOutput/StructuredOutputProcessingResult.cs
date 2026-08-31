namespace AI.Sandbox.Engine.Core.StructuredOutput;

/// <summary>
/// Captures the explicit result of one structured-output decoding operation.
/// </summary>
/// <typeparam name="TResponse">The exact model-response payload type.</typeparam>
/// <typeparam name="TOutput">The exact structured-output payload type.</typeparam>
public sealed class StructuredOutputProcessingResult<TResponse, TOutput>
    where TResponse : global::AI.Sandbox.Engine.Core.Modeling.IModelResponse
    where TOutput : IStructuredModelOutput
{
    private StructuredOutputProcessingResult(
        StructuredOutputProcessingStatus status,
        StructuredOutputRequestEnvelope<TResponse> request,
        bool decoderWasInvoked,
        bool hasStableDecision,
        StructuredOutputDecision<TOutput>? decision,
        StructuredModelOutputEnvelope<TOutput>? output)
    {
        Status = status;
        Request = request;
        DecoderWasInvoked = decoderWasInvoked;
        HasStableDecision = hasStableDecision;
        Decision = decision;
        Output = output;
    }

    /// <summary>
    /// Gets the complete processing status.
    /// </summary>
    public StructuredOutputProcessingStatus Status { get; }

    /// <summary>
    /// Gets the exact request that was processed.
    /// </summary>
    public StructuredOutputRequestEnvelope<TResponse> Request { get; }

    /// <summary>
    /// Gets a value indicating whether the decoder was invoked.
    /// </summary>
    public bool DecoderWasInvoked { get; }

    /// <summary>
    /// Gets a value indicating whether the decoder decision remained stable
    /// against authority.
    /// </summary>
    public bool HasStableDecision { get; }

    /// <summary>
    /// Gets the stable decoder decision when one exists.
    /// </summary>
    public StructuredOutputDecision<TOutput>? Decision { get; }

    /// <summary>
    /// Gets the validated structured output only when decoding succeeded.
    /// </summary>
    public StructuredModelOutputEnvelope<TOutput>? Output { get; }

    /// <summary>
    /// Gets a value indicating whether a validated output is available.
    /// </summary>
    public bool WasDecoded =>
        Status == StructuredOutputProcessingStatus.Decoded;

    internal static StructuredOutputProcessingResult<TResponse, TOutput>
        NotEvaluated(
            StructuredOutputProcessingStatus status,
            StructuredOutputRequestEnvelope<TResponse> request)
    {
        return new StructuredOutputProcessingResult<TResponse, TOutput>(
            status,
            request,
            false,
            false,
            null,
            null);
    }

    internal static StructuredOutputProcessingResult<TResponse, TOutput>
        Discarded(
            StructuredOutputRequestEnvelope<TResponse> request)
    {
        return new StructuredOutputProcessingResult<TResponse, TOutput>(
            StructuredOutputProcessingStatus.AuthorityChanged,
            request,
            true,
            false,
            null,
            null);
    }

    internal static StructuredOutputProcessingResult<TResponse, TOutput>
        Evaluated(
            StructuredOutputProcessingStatus status,
            StructuredOutputRequestEnvelope<TResponse> request,
            StructuredOutputDecision<TOutput> decision,
            StructuredModelOutputEnvelope<TOutput>? output)
    {
        return new StructuredOutputProcessingResult<TResponse, TOutput>(
            status,
            request,
            true,
            true,
            decision,
            output);
    }
}
