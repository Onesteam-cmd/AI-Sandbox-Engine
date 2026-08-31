namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one exact immutable Host request correlation and cancellation record.
/// </summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeRequestEnvelope<TRequest>
    where TRequest : IHostRuntimeRequest
{
    internal HostRuntimeRequestEnvelope(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRequestIdKind> requestId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeInstanceIdKind> runtimeInstanceId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeOperationIdKind> operationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeCorrelationIdKind> correlationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRequestIdKind> parentRequestId,
        TRequest payload,
        HostRuntimeRequestState state,
        long revision,
        IHostRuntimeCancellationReason? cancellationReason)
    {
        RequestId = requestId;
        RuntimeInstanceId = runtimeInstanceId;
        OperationId = operationId;
        CorrelationId = correlationId;
        ParentRequestId = parentRequestId;
        Payload = payload;
        State = state;
        Revision = revision;
        CancellationReason = cancellationReason;
    }

    /// <summary>Gets the externally assigned stable request ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRequestIdKind> RequestId { get; }

    /// <summary>Gets the runtime instance responsible for the request.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeInstanceIdKind> RuntimeInstanceId { get; }

    /// <summary>Gets the opaque operation ID selected by the external Host.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeOperationIdKind> OperationId { get; }

    /// <summary>Gets the cross-request correlation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeCorrelationIdKind> CorrelationId { get; }

    /// <summary>Gets the optional parent request ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRequestIdKind> ParentRequestId { get; }

    /// <summary>Gets the exact immutable request payload.</summary>
    public TRequest Payload { get; }

    /// <summary>Gets the explicit current request state.</summary>
    public HostRuntimeRequestState State { get; }

    /// <summary>Gets the optimistic request authority revision.</summary>
    public long Revision { get; }

    /// <summary>Gets the recorded exact cancellation reason, when present.</summary>
    public IHostRuntimeCancellationReason? CancellationReason { get; }

    /// <summary>Gets whether the external request record is terminal.</summary>
    public bool IsTerminal =>
        State is
            HostRuntimeRequestState.Completed or
            HostRuntimeRequestState.Rejected or
            HostRuntimeRequestState.Failed or
            HostRuntimeRequestState.Cancelled;
}
