namespace AI.Sandbox.Engine.Core.Speech;

/// <summary>
/// Represents the explicit validated result of one speech invocation attempt.
/// </summary>
/// <typeparam name="TRequest">The exact speech-request payload type.</typeparam>
/// <typeparam name="TResponse">The exact speech-response payload type.</typeparam>
public sealed class SpeechInvocationResult<TRequest, TResponse>
    where TRequest : ISpeechRequest
    where TResponse : ISpeechResponse
{
    private SpeechInvocationResult(
        SpeechInvocationStatus status,
        SpeechInvocationRequestEnvelope<TRequest> request,
        bool adapterWasInvoked,
        SpeechInvocationDecision<TResponse>? decision)
    {
        Status = status;
        Request = request;
        AdapterWasInvoked = adapterWasInvoked;
        Decision = decision;
    }

    /// <summary>
    /// Gets the complete validated invocation status.
    /// </summary>
    public SpeechInvocationStatus Status { get; }

    /// <summary>
    /// Gets the exact speech invocation request.
    /// </summary>
    public SpeechInvocationRequestEnvelope<TRequest> Request { get; }

    /// <summary>
    /// Gets a value indicating whether the adapter was invoked.
    /// </summary>
    public bool AdapterWasInvoked { get; }

    /// <summary>
    /// Gets the direct adapter decision when invocation occurred.
    /// </summary>
    public SpeechInvocationDecision<TResponse>? Decision { get; }

    /// <summary>
    /// Gets a value indicating whether one response completed validation.
    /// </summary>
    public bool WasCompleted => Status == SpeechInvocationStatus.Completed;

    internal static SpeechInvocationResult<TRequest, TResponse> NotInvoked(
        SpeechInvocationStatus status,
        SpeechInvocationRequestEnvelope<TRequest> request) =>
        new(status, request, false, null);

    internal static SpeechInvocationResult<TRequest, TResponse> Evaluated(
        SpeechInvocationStatus status,
        SpeechInvocationRequestEnvelope<TRequest> request,
        SpeechInvocationDecision<TResponse> decision) =>
        new(status, request, true, decision);
}
