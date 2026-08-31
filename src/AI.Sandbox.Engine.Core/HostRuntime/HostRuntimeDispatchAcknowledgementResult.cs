namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host dispatch acknowledgement result.
/// </summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeDispatchAcknowledgementResult<TRequest>
    where TRequest : IHostRuntimeRequest
{
    internal HostRuntimeDispatchAcknowledgementResult(
        HostRuntimeDispatchAcknowledgementStatus status,
        HostRuntimeInFlightAttempt<TRequest>? attempt)
    {
        Status = status;
        Attempt = attempt;
    }

    /// <summary>Gets the explicit acknowledgement outcome.</summary>
    public HostRuntimeDispatchAcknowledgementStatus Status { get; }

    /// <summary>
    /// Gets in-flight attempt authority when acknowledgement succeeded.
    /// </summary>
    public HostRuntimeInFlightAttempt<TRequest>? Attempt { get; }

    /// <summary>Gets whether in-flight authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeDispatchAcknowledgementStatus.Acknowledged;
}
