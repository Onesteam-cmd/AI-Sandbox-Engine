namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host dead-letter disposition result.
/// </summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
/// <typeparam name="TCompletion">Exact completion payload type.</typeparam>
public sealed record HostRuntimeDeadLetterDispositionResult<
    TRequest,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeDeadLetterDispositionResult(
        HostRuntimeDeadLetterDispositionStatus status,
        HostRuntimeDeadLetterDisposition<TRequest, TCompletion>?
            disposition,
        HostRuntimeAttemptSettlement<TRequest, TCompletion> settlement)
    {
        Status = status;
        Disposition = disposition;
        Settlement = settlement;
    }

    /// <summary>Gets the explicit disposition outcome.</summary>
    public HostRuntimeDeadLetterDispositionStatus Status { get; }

    /// <summary>
    /// Gets immutable dead-letter authority when disposition succeeded.
    /// </summary>
    public HostRuntimeDeadLetterDisposition<TRequest, TCompletion>?
        Disposition { get; }

    /// <summary>Gets the unchanged terminal settlement authority.</summary>
    public HostRuntimeAttemptSettlement<TRequest, TCompletion>
        Settlement { get; }

    /// <summary>Gets the unchanged terminal request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request =>
        Settlement.Request;

    /// <summary>Gets whether dead-letter authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeDeadLetterDispositionStatus.Disposed;
}
