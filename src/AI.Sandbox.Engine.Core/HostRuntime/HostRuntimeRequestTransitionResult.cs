namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one explicit immutable Host request transition result.</summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeRequestTransitionResult<TRequest>
    where TRequest : IHostRuntimeRequest
{
    internal HostRuntimeRequestTransitionResult(
        HostRuntimeRequestTransitionStatus status,
        HostRuntimeRequestEnvelope<TRequest> envelope)
    {
        Status = status;
        Envelope = envelope;
    }

    /// <summary>Gets the explicit transition outcome.</summary>
    public HostRuntimeRequestTransitionStatus Status { get; }

    /// <summary>Gets the resulting or unchanged immutable request envelope.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Envelope { get; }

    /// <summary>Gets whether the transition produced a new envelope.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRequestTransitionStatus.Applied;
}
