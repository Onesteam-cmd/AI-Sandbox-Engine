namespace AI.Sandbox.Engine.Core.Modeling;

/// <summary>
/// Represents the explicit validated result of one model invocation attempt.
/// </summary>
/// <typeparam name="TRequest">The exact model-request payload type.</typeparam>
/// <typeparam name="TResponse">The exact model-response payload type.</typeparam>
public sealed class ModelInvocationResult<TRequest, TResponse>
    where TRequest : IModelRequest
    where TResponse : IModelResponse
{
    private ModelInvocationResult(
        ModelInvocationStatus status,
        ModelInvocationRequestEnvelope<TRequest> request,
        bool adapterWasInvoked,
        ModelInvocationDecision<TResponse>? decision)
    {
        Status = status;
        Request = request;
        AdapterWasInvoked = adapterWasInvoked;
        Decision = decision;
    }

    /// <summary>
    /// Gets the complete validated invocation status.
    /// </summary>
    public ModelInvocationStatus Status { get; }

    /// <summary>
    /// Gets the exact invocation request.
    /// </summary>
    public ModelInvocationRequestEnvelope<TRequest> Request { get; }

    /// <summary>
    /// Gets a value indicating whether the adapter was invoked.
    /// </summary>
    public bool AdapterWasInvoked { get; }

    /// <summary>
    /// Gets the direct adapter decision when invocation occurred.
    /// </summary>
    public ModelInvocationDecision<TResponse>? Decision { get; }

    /// <summary>
    /// Gets a value indicating whether one response completed validation.
    /// </summary>
    public bool WasCompleted => Status == ModelInvocationStatus.Completed;

    internal static ModelInvocationResult<TRequest, TResponse> NotInvoked(
        ModelInvocationStatus status,
        ModelInvocationRequestEnvelope<TRequest> request) =>
        new(status, request, false, null);

    internal static ModelInvocationResult<TRequest, TResponse> Evaluated(
        ModelInvocationStatus status,
        ModelInvocationRequestEnvelope<TRequest> request,
        ModelInvocationDecision<TResponse> decision) =>
        new(status, request, true, decision);
}
