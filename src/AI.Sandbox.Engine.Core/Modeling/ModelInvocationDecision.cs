namespace AI.Sandbox.Engine.Core.Modeling;

/// <summary>
/// Represents one exact completed, rejected, or failed adapter decision.
/// </summary>
/// <typeparam name="TResponse">The exact model-response payload type.</typeparam>
public sealed class ModelInvocationDecision<TResponse>
    where TResponse : IModelResponse
{
    private ModelInvocationDecision(
        ModelInvocationDecisionStatus status,
        ModelInvocationResponseEnvelope<TResponse>? response,
        ModelRejectionCode? rejectionCode,
        ModelFailureCode? failureCode)
    {
        Status = status;
        Response = response;
        RejectionCode = rejectionCode;
        FailureCode = failureCode;
    }

    /// <summary>
    /// Gets the direct adapter-decision status.
    /// </summary>
    public ModelInvocationDecisionStatus Status { get; }

    /// <summary>
    /// Gets the completed response when status is <c>Completed</c>.
    /// </summary>
    public ModelInvocationResponseEnvelope<TResponse>? Response { get; }

    /// <summary>
    /// Gets the stable rejection code when status is <c>Rejected</c>.
    /// </summary>
    public ModelRejectionCode? RejectionCode { get; }

    /// <summary>
    /// Gets the stable failure code when status is <c>Failed</c>.
    /// </summary>
    public ModelFailureCode? FailureCode { get; }

    /// <summary>
    /// Creates a completed decision with one exact response.
    /// </summary>
    /// <param name="response">The completed correlated response.</param>
    /// <returns>The completed decision.</returns>
    public static ModelInvocationDecision<TResponse> Complete(
        ModelInvocationResponseEnvelope<TResponse> response)
    {
        ArgumentNullException.ThrowIfNull(response);
        ModelTypePolicy.EnsureExactType(
            typeof(TResponse),
            typeof(IModelResponse),
            "model response");

        return new ModelInvocationDecision<TResponse>(
            ModelInvocationDecisionStatus.Completed,
            response,
            null,
            null);
    }

    /// <summary>
    /// Creates an explicit adapter rejection.
    /// </summary>
    /// <param name="code">The initialized stable rejection code.</param>
    /// <returns>The rejected decision.</returns>
    public static ModelInvocationDecision<TResponse> Reject(
        ModelRejectionCode code)
    {
        ModelTypePolicy.EnsureExactType(
            typeof(TResponse),
            typeof(IModelResponse),
            "model response");
        EnsureInitialized(code.IsInitialized, nameof(code));
        return new ModelInvocationDecision<TResponse>(
            ModelInvocationDecisionStatus.Rejected,
            null,
            code,
            null);
    }

    /// <summary>
    /// Creates an explicit adapter failure.
    /// </summary>
    /// <param name="code">The initialized stable failure code.</param>
    /// <returns>The failed decision.</returns>
    public static ModelInvocationDecision<TResponse> Fail(
        ModelFailureCode code)
    {
        ModelTypePolicy.EnsureExactType(
            typeof(TResponse),
            typeof(IModelResponse),
            "model response");
        EnsureInitialized(code.IsInitialized, nameof(code));
        return new ModelInvocationDecision<TResponse>(
            ModelInvocationDecisionStatus.Failed,
            null,
            null,
            code);
    }

    private static void EnsureInitialized(bool initialized, string parameterName)
    {
        if (!initialized)
        {
            throw new ArgumentException(
                "Model outcome codes must be initialized.",
                parameterName);
        }
    }
}
