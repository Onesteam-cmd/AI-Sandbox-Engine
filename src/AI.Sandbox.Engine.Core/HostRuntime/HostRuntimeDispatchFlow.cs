namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Creates immutable advisory Host dispatch records.</summary>
public static class HostRuntimeDispatchFlow
{
    /// <summary>Creates one validated immutable dispatch record.</summary>
    /// <typeparam name="TRequest">Exact request payload type.</typeparam>
    /// <param name="dispatchId">Externally assigned non-empty dispatch ID.</param>
    /// <param name="routeId">Externally assigned non-empty route ID.</param>
    /// <param name="endpointId">Externally assigned non-empty endpoint ID.</param>
    /// <param name="request">Pending immutable request authority.</param>
    /// <param name="attemptNumber">One-based bounded attempt number.</param>
    /// <returns>An immutable advisory dispatch record.</returns>
    public static HostRuntimeDispatchEnvelope<TRequest> Create<TRequest>(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeDispatchIdKind> dispatchId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRouteIdKind> routeId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeEndpointIdKind> endpointId,
        HostRuntimeRequestEnvelope<TRequest> request,
        int attemptNumber)
        where TRequest : IHostRuntimeRequest
    {
        EnsureId(dispatchId.IsEmpty, nameof(dispatchId));
        EnsureId(routeId.IsEmpty, nameof(routeId));
        EnsureId(endpointId.IsEmpty, nameof(endpointId));
        ArgumentNullException.ThrowIfNull(request);

        if (request.State != HostRuntimeRequestState.Pending)
        {
            throw new ArgumentException(
                "Only pending Host requests may be dispatched.",
                nameof(request));
        }
        if (attemptNumber < 1 ||
            attemptNumber >
                HostRuntimeRetryPolicy.MaximumAttemptCount)
        {
            throw new ArgumentOutOfRangeException(nameof(attemptNumber));
        }

        return new HostRuntimeDispatchEnvelope<TRequest>(
            dispatchId,
            routeId,
            endpointId,
            request,
            attemptNumber);
    }

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new ArgumentException(
                "The identifier must be initialized.",
                parameterName);
        }
    }
}
