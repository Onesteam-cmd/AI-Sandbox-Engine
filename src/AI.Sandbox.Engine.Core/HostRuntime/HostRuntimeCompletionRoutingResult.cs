namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable Host completion-routing result.</summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
/// <typeparam name="TCompletion">Exact completion payload type.</typeparam>
public sealed record HostRuntimeCompletionRoutingResult<
    TRequest,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeCompletionRoutingResult(
        HostRuntimeCompletionRoutingStatus status,
        HostRuntimeRequestEnvelope<TRequest> request,
        HostRuntimeCompletionEnvelope<TCompletion> completion)
    {
        Status = status;
        Request = request;
        Completion = completion;
    }

    /// <summary>Gets the explicit routing outcome.</summary>
    public HostRuntimeCompletionRoutingStatus Status { get; }

    /// <summary>Gets the resulting or unchanged request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request { get; }

    /// <summary>Gets the immutable external completion.</summary>
    public HostRuntimeCompletionEnvelope<TCompletion> Completion { get; }

    /// <summary>Gets whether routing produced terminal request authority.</summary>
    public bool Succeeded =>
        Status == HostRuntimeCompletionRoutingStatus.Routed;
}
