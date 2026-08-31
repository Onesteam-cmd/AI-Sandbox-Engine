namespace AI.Sandbox.Engine.Core.Modeling;

/// <summary>
/// Defines one exact asynchronous provider-neutral model adapter boundary.
/// Implementations live outside generic Core and may perform explicit I/O.
/// </summary>
/// <typeparam name="TRequest">The exact model-request payload type.</typeparam>
/// <typeparam name="TResponse">The exact model-response payload type.</typeparam>
public interface IModelAdapter<TRequest, TResponse>
    where TRequest : IModelRequest
    where TResponse : IModelResponse
{
    /// <summary>
    /// Invokes the configured adapter exactly once for the supplied context.
    /// </summary>
    /// <param name="context">The immutable invocation context.</param>
    /// <param name="cancellationToken">The explicit host cancellation token.</param>
    /// <returns>The completed, rejected, or failed adapter decision.</returns>
    public ValueTask<ModelInvocationDecision<TResponse>> InvokeAsync(
        ModelInvocationContext<TRequest> context,
        CancellationToken cancellationToken);
}
