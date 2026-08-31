namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Validates one externally selected active lease, dequeues one abstract
/// queue slot, and creates immutable advisory dispatch authority.
/// </summary>
public static class HostRuntimeDispatchSelectionFlow
{
    /// <summary>
    /// Selects one active leased request for dequeue and dispatch.
    /// </summary>
    /// <typeparam name="TRequest">Exact request payload type.</typeparam>
    /// <param name="selectionId">
    /// Externally assigned non-empty selection ID.
    /// </param>
    /// <param name="snapshot">Current immutable queue authority.</param>
    /// <param name="expectedQueueRevision">
    /// Queue revision observed by the caller.
    /// </param>
    /// <param name="lease">
    /// Active immutable worker-ownership lease selected externally.
    /// </param>
    /// <param name="clockId">
    /// Matching externally owned monotonic clock domain.
    /// </param>
    /// <param name="observedTick">
    /// Current non-negative external monotonic tick.
    /// </param>
    /// <param name="dispatchId">
    /// Externally assigned non-empty dispatch ID.
    /// </param>
    /// <param name="routeId">
    /// Externally assigned non-empty route ID.
    /// </param>
    /// <param name="endpointId">
    /// Externally assigned non-empty endpoint ID.
    /// </param>
    /// <param name="attemptNumber">
    /// One-based bounded dispatch attempt number.
    /// </param>
    /// <returns>
    /// An explicit immutable dequeue-and-selection result.
    /// </returns>
    public static HostRuntimeDispatchSelectionResult<TRequest>
        Select<TRequest>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeDispatchSelectionIdKind> selectionId,
            HostRuntimeQueueSnapshot snapshot,
            long expectedQueueRevision,
            HostRuntimeWorkLease<TRequest> lease,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeClockIdKind> clockId,
            long observedTick,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeDispatchIdKind> dispatchId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRouteIdKind> routeId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeEndpointIdKind> endpointId,
            int attemptNumber)
        where TRequest : IHostRuntimeRequest
    {
        if (selectionId.IsEmpty)
        {
            throw new ArgumentException(
                "The selection ID must be initialized.",
                nameof(selectionId));
        }

        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(lease);

        if (expectedQueueRevision < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedQueueRevision));
        }
        if (clockId.IsEmpty)
        {
            throw new ArgumentException(
                "The clock ID must be initialized.",
                nameof(clockId));
        }
        if (observedTick < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(observedTick));
        }

        if (snapshot.Revision != expectedQueueRevision)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchSelectionStatus
                    .StaleQueueRevision,
                snapshot);
        }
        if (snapshot.QueuedCount == 0)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchSelectionStatus.EmptyQueue,
                snapshot);
        }
        if (lease.QueueId != snapshot.QueueId)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchSelectionStatus.QueueMismatch,
                snapshot);
        }
        if (!lease.IsActive)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchSelectionStatus.InvalidLeaseState,
                snapshot);
        }
        if (lease.ClockId != clockId)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchSelectionStatus.ClockMismatch,
                snapshot);
        }
        if (observedTick >= lease.ExpiresTick)
        {
            return Unchanged<TRequest>(
                HostRuntimeDispatchSelectionStatus.LeaseExpired,
                snapshot);
        }

        var nextSnapshot = new HostRuntimeQueueSnapshot(
            snapshot.QueueId,
            snapshot.Capacity,
            checked(snapshot.QueuedCount - 1),
            checked(snapshot.Revision + 1));

        var dispatch = HostRuntimeDispatchFlow.Create(
            dispatchId,
            routeId,
            endpointId,
            lease.Admission.Request,
            attemptNumber);

        var selection =
            new HostRuntimeDispatchSelection<TRequest>(
                selectionId,
                nextSnapshot,
                lease,
                dispatch,
                snapshot.Revision);

        return new HostRuntimeDispatchSelectionResult<TRequest>(
            HostRuntimeDispatchSelectionStatus.Selected,
            nextSnapshot,
            selection);
    }

    private static HostRuntimeDispatchSelectionResult<TRequest>
        Unchanged<TRequest>(
            HostRuntimeDispatchSelectionStatus status,
            HostRuntimeQueueSnapshot snapshot)
        where TRequest : IHostRuntimeRequest =>
        new(status, snapshot, selection: null);
}
