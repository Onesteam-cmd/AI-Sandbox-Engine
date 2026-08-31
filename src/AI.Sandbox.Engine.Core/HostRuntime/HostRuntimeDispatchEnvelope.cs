namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Contains one immutable advisory Host dispatch record.</summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeDispatchEnvelope<TRequest>
    where TRequest : IHostRuntimeRequest
{
    internal HostRuntimeDispatchEnvelope(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeDispatchIdKind> dispatchId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRouteIdKind> routeId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeEndpointIdKind> endpointId,
        HostRuntimeRequestEnvelope<TRequest> request,
        int attemptNumber)
    {
        DispatchId = dispatchId;
        RouteId = routeId;
        EndpointId = endpointId;
        Request = request;
        AttemptNumber = attemptNumber;
        ObservedRequestRevision = request.Revision;
    }

    /// <summary>Gets the externally assigned dispatch ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeDispatchIdKind> DispatchId { get; }

    /// <summary>Gets the externally assigned route ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRouteIdKind> RouteId { get; }

    /// <summary>Gets the externally assigned endpoint ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeEndpointIdKind> EndpointId { get; }

    /// <summary>Gets the unchanged immutable request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request { get; }

    /// <summary>Gets the one-based dispatch attempt number.</summary>
    public int AttemptNumber { get; }

    /// <summary>Gets the request revision observed at dispatch creation.</summary>
    public long ObservedRequestRevision { get; }

    /// <summary>Gets the stable request ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRequestIdKind> RequestId => Request.RequestId;

    /// <summary>Gets the stable runtime instance ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeInstanceIdKind> RuntimeInstanceId =>
        Request.RuntimeInstanceId;

    /// <summary>Gets the stable operation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeOperationIdKind> OperationId => Request.OperationId;

    /// <summary>Gets the stable correlation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeCorrelationIdKind> CorrelationId =>
        Request.CorrelationId;
}
