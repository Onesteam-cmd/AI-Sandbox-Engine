namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Contains one immutable externally reported Host completion.</summary>
/// <typeparam name="TCompletion">Exact completion payload type.</typeparam>
public sealed record HostRuntimeCompletionEnvelope<TCompletion>
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeCompletionEnvelope(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeDispatchIdKind> dispatchId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRequestIdKind> requestId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeInstanceIdKind> runtimeInstanceId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeOperationIdKind> operationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeCorrelationIdKind> correlationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRouteIdKind> routeId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeEndpointIdKind> endpointId,
        int attemptNumber,
        HostRuntimeCompletionKind kind,
        TCompletion payload)
    {
        DispatchId = dispatchId;
        RequestId = requestId;
        RuntimeInstanceId = runtimeInstanceId;
        OperationId = operationId;
        CorrelationId = correlationId;
        RouteId = routeId;
        EndpointId = endpointId;
        AttemptNumber = attemptNumber;
        Kind = kind;
        Payload = payload;
    }

    /// <summary>Gets the externally assigned dispatch ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeDispatchIdKind> DispatchId { get; }

    /// <summary>Gets the stable request ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRequestIdKind> RequestId { get; }

    /// <summary>Gets the stable runtime instance ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeInstanceIdKind> RuntimeInstanceId { get; }

    /// <summary>Gets the stable operation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeOperationIdKind> OperationId { get; }

    /// <summary>Gets the stable correlation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeCorrelationIdKind> CorrelationId { get; }

    /// <summary>Gets the externally assigned route ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRouteIdKind> RouteId { get; }

    /// <summary>Gets the externally assigned endpoint ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeEndpointIdKind> EndpointId { get; }

    /// <summary>Gets the one-based completed attempt number.</summary>
    public int AttemptNumber { get; }

    /// <summary>Gets the explicit external completion kind.</summary>
    public HostRuntimeCompletionKind Kind { get; }

    /// <summary>Gets the exact immutable completion payload.</summary>
    public TCompletion Payload { get; }
}
