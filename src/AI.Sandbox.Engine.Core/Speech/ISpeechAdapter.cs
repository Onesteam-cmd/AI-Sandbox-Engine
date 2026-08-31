namespace AI.Sandbox.Engine.Core.Speech;

/// <summary>
/// Defines one exact asynchronous provider-neutral speech adapter boundary.
/// Implementations live outside generic Core and may perform explicit I/O.
/// </summary>
/// <typeparam name="TRequest">The exact speech-request payload type.</typeparam>
/// <typeparam name="TResponse">The exact speech-response payload type.</typeparam>
public interface ISpeechAdapter<TRequest, TResponse>
    where TRequest : ISpeechRequest
    where TResponse : ISpeechResponse
{
    /// <summary>
    /// Invokes the configured recognition or synthesis adapter exactly once.
    /// </summary>
    /// <param name="context">The immutable invocation context.</param>
    /// <param name="cancellationToken">The explicit host cancellation token.</param>
    /// <returns>The completed, rejected, or failed adapter decision.</returns>
    public ValueTask<SpeechInvocationDecision<TResponse>> InvokeAsync(
        SpeechInvocationContext<TRequest> context,
        CancellationToken cancellationToken);
}
