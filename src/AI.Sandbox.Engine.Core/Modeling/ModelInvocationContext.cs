namespace AI.Sandbox.Engine.Core.Modeling;

/// <summary>
/// Provides one immutable request and its configured adapter identity to an
/// exact provider-neutral model adapter.
/// </summary>
/// <typeparam name="TRequest">The exact model-request payload type.</typeparam>
public sealed class ModelInvocationContext<TRequest>
    where TRequest : IModelRequest
{
    internal ModelInvocationContext(
        ModelInvocationRequestEnvelope<TRequest> request,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ModelAdapterIdKind>
            adapterId)
    {
        Request = request;
        AdapterId = adapterId;
    }

    /// <summary>
    /// Gets the exact immutable invocation request.
    /// </summary>
    public ModelInvocationRequestEnvelope<TRequest> Request { get; }

    /// <summary>
    /// Gets the stable configured adapter ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<ModelAdapterIdKind>
        AdapterId { get; }
}
