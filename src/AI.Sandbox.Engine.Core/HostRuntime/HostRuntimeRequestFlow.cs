namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Creates and advances immutable Host request correlation and cancellation records.
/// </summary>
public static class HostRuntimeRequestFlow
{
    /// <summary>Creates one validated pending Host request envelope.</summary>
    /// <typeparam name="TRequest">Exact request payload type.</typeparam>
    /// <param name="requestId">Externally assigned non-empty request ID.</param>
    /// <param name="runtimeInstanceId">Non-empty responsible runtime ID.</param>
    /// <param name="operationId">Opaque non-empty operation ID.</param>
    /// <param name="correlationId">Non-empty cross-request correlation ID.</param>
    /// <param name="parentRequestId">Optional parent request ID.</param>
    /// <param name="payload">Exact immutable request payload.</param>
    /// <returns>A pending envelope at revision zero.</returns>
    public static HostRuntimeRequestEnvelope<TRequest> Create<TRequest>(
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
        TRequest payload)
        where TRequest : IHostRuntimeRequest
    {
        EnsureId(requestId.IsEmpty, nameof(requestId));
        EnsureId(runtimeInstanceId.IsEmpty, nameof(runtimeInstanceId));
        EnsureId(operationId.IsEmpty, nameof(operationId));
        EnsureId(correlationId.IsEmpty, nameof(correlationId));
        HostRuntimeTypePolicy.EnsureExactRequest(payload);

        if (!parentRequestId.IsEmpty && parentRequestId == requestId)
        {
            throw new ArgumentException(
                "A Host request cannot be its own parent.",
                nameof(parentRequestId));
        }

        return new HostRuntimeRequestEnvelope<TRequest>(
            requestId,
            runtimeInstanceId,
            operationId,
            correlationId,
            parentRequestId,
            payload,
            HostRuntimeRequestState.Pending,
            revision: 0,
            cancellationReason: null);
    }

    /// <summary>Records one optimistic cancellation intention.</summary>
    /// <typeparam name="TRequest">Exact request payload type.</typeparam>
    /// <param name="envelope">Current immutable request authority.</param>
    /// <param name="expectedRevision">Revision observed by the caller.</param>
    /// <param name="reason">Exact immutable cancellation reason.</param>
    /// <returns>An explicit result without cancelling external work.</returns>
    public static HostRuntimeRequestTransitionResult<TRequest>
        RequestCancellation<TRequest>(
            HostRuntimeRequestEnvelope<TRequest> envelope,
            long expectedRevision,
            IHostRuntimeCancellationReason reason)
        where TRequest : IHostRuntimeRequest
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ValidateRevision(expectedRevision);
        HostRuntimeTypePolicy.EnsureExactCancellationReason(reason);

        if (envelope.Revision != expectedRevision)
        {
            return Unchanged(
                HostRuntimeRequestTransitionStatus.StaleRevision,
                envelope);
        }
        if (envelope.State != HostRuntimeRequestState.Pending)
        {
            return Unchanged(
                HostRuntimeRequestTransitionStatus.InvalidState,
                envelope);
        }

        return Applied(
            envelope,
            HostRuntimeRequestState.CancellationRequested,
            reason);
    }

    /// <summary>Records one optimistic terminal external Host result.</summary>
    /// <typeparam name="TRequest">Exact request payload type.</typeparam>
    /// <param name="envelope">Current immutable request authority.</param>
    /// <param name="expectedRevision">Revision observed by the caller.</param>
    /// <param name="terminalState">Completed, rejected, failed, or cancelled.</param>
    /// <returns>An explicit result without executing the request.</returns>
    public static HostRuntimeRequestTransitionResult<TRequest>
        Finalize<TRequest>(
            HostRuntimeRequestEnvelope<TRequest> envelope,
            long expectedRevision,
            HostRuntimeRequestState terminalState)
        where TRequest : IHostRuntimeRequest
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ValidateRevision(expectedRevision);
        if (!IsTerminalState(terminalState))
        {
            throw new ArgumentOutOfRangeException(nameof(terminalState));
        }

        if (envelope.Revision != expectedRevision)
        {
            return Unchanged(
                HostRuntimeRequestTransitionStatus.StaleRevision,
                envelope);
        }
        if (envelope.State is not (
            HostRuntimeRequestState.Pending or
            HostRuntimeRequestState.CancellationRequested))
        {
            return Unchanged(
                HostRuntimeRequestTransitionStatus.InvalidState,
                envelope);
        }

        return Applied(
            envelope,
            terminalState,
            envelope.CancellationReason);
    }

    private static HostRuntimeRequestTransitionResult<TRequest>
        Applied<TRequest>(
            HostRuntimeRequestEnvelope<TRequest> envelope,
            HostRuntimeRequestState nextState,
            IHostRuntimeCancellationReason? cancellationReason)
        where TRequest : IHostRuntimeRequest
    {
        var next = new HostRuntimeRequestEnvelope<TRequest>(
            envelope.RequestId,
            envelope.RuntimeInstanceId,
            envelope.OperationId,
            envelope.CorrelationId,
            envelope.ParentRequestId,
            envelope.Payload,
            nextState,
            checked(envelope.Revision + 1),
            cancellationReason);
        return new HostRuntimeRequestTransitionResult<TRequest>(
            HostRuntimeRequestTransitionStatus.Applied,
            next);
    }

    private static HostRuntimeRequestTransitionResult<TRequest>
        Unchanged<TRequest>(
            HostRuntimeRequestTransitionStatus status,
            HostRuntimeRequestEnvelope<TRequest> envelope)
        where TRequest : IHostRuntimeRequest =>
        new(status, envelope);

    private static bool IsTerminalState(HostRuntimeRequestState state) =>
        state is
            HostRuntimeRequestState.Completed or
            HostRuntimeRequestState.Rejected or
            HostRuntimeRequestState.Failed or
            HostRuntimeRequestState.Cancelled;

    private static void ValidateRevision(long expectedRevision)
    {
        if (expectedRevision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(expectedRevision));
        }
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
