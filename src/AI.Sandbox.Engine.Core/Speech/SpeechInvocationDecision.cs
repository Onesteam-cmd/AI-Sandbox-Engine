namespace AI.Sandbox.Engine.Core.Speech;

/// <summary>
/// Represents one exact completed, rejected, or failed speech-adapter decision.
/// </summary>
/// <typeparam name="TResponse">The exact speech-response payload type.</typeparam>
public sealed class SpeechInvocationDecision<TResponse>
    where TResponse : ISpeechResponse
{
    private SpeechInvocationDecision(
        SpeechInvocationDecisionStatus status,
        SpeechInvocationResponseEnvelope<TResponse>? response,
        SpeechRejectionCode? rejectionCode,
        SpeechFailureCode? failureCode)
    {
        Status = status;
        Response = response;
        RejectionCode = rejectionCode;
        FailureCode = failureCode;
    }

    /// <summary>
    /// Gets the direct adapter-decision status.
    /// </summary>
    public SpeechInvocationDecisionStatus Status { get; }

    /// <summary>
    /// Gets the completed response when status is <c>Completed</c>.
    /// </summary>
    public SpeechInvocationResponseEnvelope<TResponse>? Response { get; }

    /// <summary>
    /// Gets the stable rejection code when status is <c>Rejected</c>.
    /// </summary>
    public SpeechRejectionCode? RejectionCode { get; }

    /// <summary>
    /// Gets the stable failure code when status is <c>Failed</c>.
    /// </summary>
    public SpeechFailureCode? FailureCode { get; }

    /// <summary>
    /// Creates a completed decision with one exact response.
    /// </summary>
    /// <param name="response">The completed correlated response.</param>
    /// <returns>The completed decision.</returns>
    public static SpeechInvocationDecision<TResponse> Complete(
        SpeechInvocationResponseEnvelope<TResponse> response)
    {
        ArgumentNullException.ThrowIfNull(response);
        SpeechTypePolicy.EnsureExactType(
            typeof(TResponse),
            typeof(ISpeechResponse),
            "speech response");

        return new SpeechInvocationDecision<TResponse>(
            SpeechInvocationDecisionStatus.Completed,
            response,
            null,
            null);
    }

    /// <summary>
    /// Creates an explicit adapter rejection.
    /// </summary>
    /// <param name="code">The initialized stable rejection code.</param>
    /// <returns>The rejected decision.</returns>
    public static SpeechInvocationDecision<TResponse> Reject(
        SpeechRejectionCode code)
    {
        SpeechTypePolicy.EnsureExactType(
            typeof(TResponse),
            typeof(ISpeechResponse),
            "speech response");
        EnsureInitialized(code.IsInitialized, nameof(code));
        return new SpeechInvocationDecision<TResponse>(
            SpeechInvocationDecisionStatus.Rejected,
            null,
            code,
            null);
    }

    /// <summary>
    /// Creates an explicit adapter failure.
    /// </summary>
    /// <param name="code">The initialized stable failure code.</param>
    /// <returns>The failed decision.</returns>
    public static SpeechInvocationDecision<TResponse> Fail(
        SpeechFailureCode code)
    {
        SpeechTypePolicy.EnsureExactType(
            typeof(TResponse),
            typeof(ISpeechResponse),
            "speech response");
        EnsureInitialized(code.IsInitialized, nameof(code));
        return new SpeechInvocationDecision<TResponse>(
            SpeechInvocationDecisionStatus.Failed,
            null,
            null,
            code);
    }

    private static void EnsureInitialized(bool initialized, string parameterName)
    {
        if (!initialized)
        {
            throw new ArgumentException(
                "Speech outcome codes must be initialized.",
                parameterName);
        }
    }
}
