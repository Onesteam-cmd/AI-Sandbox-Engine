namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Purely matches external completions to dispatch and request authority.
/// </summary>
public static class HostRuntimeCompletionRouter
{
    /// <summary>
    /// Routes one immutable completion without receiving transport or executing work.
    /// </summary>
    public static HostRuntimeCompletionRoutingResult<
        TRequest,
        TCompletion> Route<TRequest, TCompletion>(
            HostRuntimeDispatchEnvelope<TRequest> dispatch,
            HostRuntimeRequestEnvelope<TRequest> currentRequest,
            long expectedRequestRevision,
            HostRuntimeCompletionEnvelope<TCompletion> completion)
        where TRequest : IHostRuntimeRequest
        where TCompletion : IHostRuntimeCompletion
    {
        ArgumentNullException.ThrowIfNull(dispatch);
        ArgumentNullException.ThrowIfNull(currentRequest);
        ArgumentNullException.ThrowIfNull(completion);

        if (expectedRequestRevision < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedRequestRevision));
        }

        if (currentRequest.Revision != expectedRequestRevision)
        {
            return Unchanged(
                HostRuntimeCompletionRoutingStatus.StaleRevision,
                currentRequest,
                completion);
        }

        if (currentRequest.State is not (
            HostRuntimeRequestState.Pending or
            HostRuntimeRequestState.CancellationRequested))
        {
            return Unchanged(
                HostRuntimeCompletionRoutingStatus.InvalidRequestState,
                currentRequest,
                completion);
        }

        if (dispatch.RequestId != currentRequest.RequestId ||
            dispatch.RuntimeInstanceId !=
                currentRequest.RuntimeInstanceId ||
            dispatch.OperationId != currentRequest.OperationId ||
            dispatch.CorrelationId != currentRequest.CorrelationId ||
            dispatch.ObservedRequestRevision >
                currentRequest.Revision)
        {
            return Unchanged(
                HostRuntimeCompletionRoutingStatus.DispatchMismatch,
                currentRequest,
                completion);
        }

        if (completion.DispatchId != dispatch.DispatchId ||
            completion.RequestId != dispatch.RequestId ||
            completion.RuntimeInstanceId !=
                dispatch.RuntimeInstanceId ||
            completion.OperationId != dispatch.OperationId ||
            completion.CorrelationId != dispatch.CorrelationId ||
            completion.RouteId != dispatch.RouteId ||
            completion.EndpointId != dispatch.EndpointId ||
            completion.AttemptNumber != dispatch.AttemptNumber)
        {
            return Unchanged(
                HostRuntimeCompletionRoutingStatus.CompletionMismatch,
                currentRequest,
                completion);
        }

        var terminalState = completion.Kind switch
        {
            HostRuntimeCompletionKind.Completed =>
                HostRuntimeRequestState.Completed,
            HostRuntimeCompletionKind.Rejected =>
                HostRuntimeRequestState.Rejected,
            HostRuntimeCompletionKind.Failed =>
                HostRuntimeRequestState.Failed,
            HostRuntimeCompletionKind.Cancelled =>
                HostRuntimeRequestState.Cancelled,
            _ => throw new ArgumentOutOfRangeException(
                nameof(completion)),
        };

        var transition = HostRuntimeRequestFlow.Finalize(
            currentRequest,
            expectedRequestRevision,
            terminalState);

        return transition.Succeeded
            ? new HostRuntimeCompletionRoutingResult<
                TRequest,
                TCompletion>(
                    HostRuntimeCompletionRoutingStatus.Routed,
                    transition.Envelope,
                    completion)
            : Unchanged(
                HostRuntimeCompletionRoutingStatus.InvalidRequestState,
                currentRequest,
                completion);
    }

    private static HostRuntimeCompletionRoutingResult<
        TRequest,
        TCompletion> Unchanged<TRequest, TCompletion>(
            HostRuntimeCompletionRoutingStatus status,
            HostRuntimeRequestEnvelope<TRequest> request,
            HostRuntimeCompletionEnvelope<TCompletion> completion)
        where TRequest : IHostRuntimeRequest
        where TCompletion : IHostRuntimeCompletion =>
        new(status, request, completion);
}
