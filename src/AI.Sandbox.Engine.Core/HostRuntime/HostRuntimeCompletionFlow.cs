namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Creates validated immutable external Host completion records.</summary>
public static class HostRuntimeCompletionFlow
{
    /// <summary>Creates one validated immutable Host completion.</summary>
    public static HostRuntimeCompletionEnvelope<TCompletion>
        Create<TCompletion>(
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
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(dispatchId.IsEmpty, nameof(dispatchId));
        EnsureId(requestId.IsEmpty, nameof(requestId));
        EnsureId(runtimeInstanceId.IsEmpty, nameof(runtimeInstanceId));
        EnsureId(operationId.IsEmpty, nameof(operationId));
        EnsureId(correlationId.IsEmpty, nameof(correlationId));
        EnsureId(routeId.IsEmpty, nameof(routeId));
        EnsureId(endpointId.IsEmpty, nameof(endpointId));

        if (attemptNumber < 1 ||
            attemptNumber >
                HostRuntimeRetryPolicy.MaximumAttemptCount)
        {
            throw new ArgumentOutOfRangeException(nameof(attemptNumber));
        }
        if (!Enum.IsDefined(kind))
        {
            throw new ArgumentOutOfRangeException(nameof(kind));
        }

        HostRuntimeTypePolicy.EnsureExactCompletion(payload);

        return new HostRuntimeCompletionEnvelope<TCompletion>(
            dispatchId,
            requestId,
            runtimeInstanceId,
            operationId,
            correlationId,
            routeId,
            endpointId,
            attemptNumber,
            kind,
            payload);
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
