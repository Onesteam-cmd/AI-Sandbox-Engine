namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Captures and reconciles bounded deterministic Host active-work authority.
/// </summary>
public static class HostRuntimeActiveWorkFlow
{
    private const int MaximumActiveWorkItemCount = 256;

    /// <summary>
    /// Captures one bounded active-work snapshot without polling or storage.
    /// </summary>
    /// <typeparam name="TRequest">Exact request payload type.</typeparam>
    /// <param name="snapshotId">Externally assigned snapshot ID.</param>
    /// <param name="runtimeInstanceId">Represented runtime instance ID.</param>
    /// <param name="clockId">External monotonic clock domain.</param>
    /// <param name="observedTick">External monotonic observation tick.</param>
    /// <param name="revision">Optimistic snapshot revision.</param>
    /// <param name="items">Current active-work observations.</param>
    /// <returns>An explicit immutable snapshot result.</returns>
    public static HostRuntimeActiveWorkSnapshotResult<TRequest>
        Capture<TRequest>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeActiveWorkSnapshotIdKind> snapshotId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeInstanceIdKind> runtimeInstanceId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeClockIdKind> clockId,
            long observedTick,
            long revision,
            IEnumerable<HostRuntimeActiveWorkItem<TRequest>> items)
        where TRequest : IHostRuntimeRequest
    {
        EnsureId(snapshotId.IsEmpty, nameof(snapshotId));
        EnsureId(runtimeInstanceId.IsEmpty, nameof(runtimeInstanceId));
        EnsureId(clockId.IsEmpty, nameof(clockId));

        if (observedTick < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(observedTick));
        }
        if (revision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(revision));
        }

        ArgumentNullException.ThrowIfNull(items);
        var input = items.ToArray();

        if (input.Length > MaximumActiveWorkItemCount)
        {
            return SnapshotResult<TRequest>(
                HostRuntimeActiveWorkStatus.TooManyItems);
        }
        if (input.Any(static item => item is null))
        {
            throw new ArgumentException(
                "Active-work items cannot contain null.",
                nameof(items));
        }

        var attemptIds = new HashSet<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeAttemptIdKind>>();

        foreach (var item in input)
        {
            if (!attemptIds.Add(item.AttemptId))
            {
                return SnapshotResult<TRequest>(
                    HostRuntimeActiveWorkStatus.DuplicateAttempt,
                    item.AttemptId);
            }

            var status = ValidateItem(
                item,
                runtimeInstanceId,
                clockId,
                observedTick);
            if (status != HostRuntimeActiveWorkStatus.Captured)
            {
                return SnapshotResult<TRequest>(
                    status,
                    item.AttemptId);
            }
        }

        var ordered = input
            .OrderBy(static item => item.AttemptId)
            .ToArray();
        var snapshot = new HostRuntimeActiveWorkSnapshot<TRequest>(
            snapshotId,
            runtimeInstanceId,
            clockId,
            observedTick,
            revision,
            ordered);

        return new HostRuntimeActiveWorkSnapshotResult<TRequest>(
            HostRuntimeActiveWorkStatus.Captured,
            snapshot,
            default);
    }

    /// <summary>
    /// Reconciles two validated sequential active-work snapshots.
    /// </summary>
    /// <typeparam name="TRequest">Exact request payload type.</typeparam>
    /// <param name="reconciliationId">
    /// Externally assigned reconciliation ID.
    /// </param>
    /// <param name="previousSnapshot">Previous active-work snapshot.</param>
    /// <param name="expectedPreviousRevision">
    /// Previous revision observed by the caller.
    /// </param>
    /// <param name="currentSnapshot">Current active-work snapshot.</param>
    /// <returns>An explicit immutable reconciliation result.</returns>
    public static HostRuntimeActiveWorkReconciliationResult<TRequest>
        Reconcile<TRequest>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeActiveWorkReconciliationIdKind> reconciliationId,
            HostRuntimeActiveWorkSnapshot<TRequest> previousSnapshot,
            long expectedPreviousRevision,
            HostRuntimeActiveWorkSnapshot<TRequest> currentSnapshot)
        where TRequest : IHostRuntimeRequest
    {
        EnsureId(reconciliationId.IsEmpty, nameof(reconciliationId));
        ArgumentNullException.ThrowIfNull(previousSnapshot);
        ArgumentNullException.ThrowIfNull(currentSnapshot);

        if (expectedPreviousRevision < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedPreviousRevision));
        }

        if (previousSnapshot.Revision != expectedPreviousRevision)
        {
            return ReconciliationResult<TRequest>(
                HostRuntimeActiveWorkStatus.StaleSnapshotRevision);
        }
        if (currentSnapshot.RuntimeInstanceId !=
            previousSnapshot.RuntimeInstanceId)
        {
            return ReconciliationResult<TRequest>(
                HostRuntimeActiveWorkStatus.RuntimeMismatch);
        }
        if (currentSnapshot.ClockId != previousSnapshot.ClockId)
        {
            return ReconciliationResult<TRequest>(
                HostRuntimeActiveWorkStatus.ClockMismatch);
        }
        if (currentSnapshot.ObservedTick < previousSnapshot.ObservedTick)
        {
            return ReconciliationResult<TRequest>(
                HostRuntimeActiveWorkStatus.ObservationTickRegressed);
        }

        long nextRevision;
        try
        {
            nextRevision = checked(previousSnapshot.Revision + 1);
        }
        catch (OverflowException)
        {
            return ReconciliationResult<TRequest>(
                HostRuntimeActiveWorkStatus.NonSequentialSnapshotRevision);
        }

        if (currentSnapshot.Revision != nextRevision)
        {
            return ReconciliationResult<TRequest>(
                HostRuntimeActiveWorkStatus.NonSequentialSnapshotRevision);
        }

        var previousById = previousSnapshot.Items.ToDictionary(
            static item => item.AttemptId);
        var currentById = currentSnapshot.Items.ToDictionary(
            static item => item.AttemptId);
        var retainedIds = previousById.Keys
            .Where(currentById.ContainsKey)
            .OrderBy(static id => id)
            .ToArray();

        foreach (var attemptId in retainedIds)
        {
            var previous = previousById[attemptId];
            var current = currentById[attemptId];

            if (current.RequestId != previous.RequestId ||
                current.LeaseId != previous.LeaseId ||
                current.WorkerId != previous.WorkerId ||
                current.DispatchId != previous.DispatchId ||
                current.AttemptNumber != previous.AttemptNumber ||
                current.ClockId != previous.ClockId)
            {
                return ReconciliationResult<TRequest>(
                    HostRuntimeActiveWorkStatus
                        .RetainedAttemptLineageMismatch,
                    attemptId);
            }
            if (current.Request.Revision < previous.Request.Revision)
            {
                return ReconciliationResult<TRequest>(
                    HostRuntimeActiveWorkStatus
                        .RetainedRequestRevisionRegressed,
                    attemptId);
            }
            if (current.Lease.Revision < previous.Lease.Revision)
            {
                return ReconciliationResult<TRequest>(
                    HostRuntimeActiveWorkStatus
                        .RetainedLeaseRevisionRegressed,
                    attemptId);
            }
        }

        var addedIds = currentById.Keys
            .Where(id => !previousById.ContainsKey(id))
            .OrderBy(static id => id)
            .ToArray();
        var removedIds = previousById.Keys
            .Where(id => !currentById.ContainsKey(id))
            .OrderBy(static id => id)
            .ToArray();

        var reconciliation =
            new HostRuntimeActiveWorkReconciliation<TRequest>(
                reconciliationId,
                previousSnapshot,
                currentSnapshot,
                addedIds,
                retainedIds,
                removedIds);

        return new HostRuntimeActiveWorkReconciliationResult<TRequest>(
            HostRuntimeActiveWorkStatus.Reconciled,
            reconciliation,
            default);
    }

    private static HostRuntimeActiveWorkStatus ValidateItem<TRequest>(
        HostRuntimeActiveWorkItem<TRequest> item,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeInstanceIdKind> runtimeInstanceId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeClockIdKind> clockId,
        long observedTick)
        where TRequest : IHostRuntimeRequest
    {
        if (item.Request.RuntimeInstanceId != runtimeInstanceId ||
            item.Attempt.Request.RuntimeInstanceId != runtimeInstanceId)
        {
            return HostRuntimeActiveWorkStatus.RuntimeMismatch;
        }
        if (item.Attempt.ClockId != clockId ||
            item.Lease.ClockId != clockId)
        {
            return HostRuntimeActiveWorkStatus.ClockMismatch;
        }
        if (item.RequestId != item.Attempt.RequestId ||
            item.Lease.RequestId != item.Attempt.RequestId)
        {
            return HostRuntimeActiveWorkStatus.AttemptRequestMismatch;
        }
        if (item.LeaseId != item.Attempt.LeaseId)
        {
            return HostRuntimeActiveWorkStatus.AttemptLeaseMismatch;
        }
        if (item.WorkerId != item.Attempt.WorkerId)
        {
            return HostRuntimeActiveWorkStatus.AttemptWorkerMismatch;
        }
        if (item.Request.Revision < item.Attempt.ObservedRequestRevision)
        {
            return HostRuntimeActiveWorkStatus.RequestRevisionRegressed;
        }
        if (item.Lease.Revision < item.Attempt.ObservedLeaseRevision)
        {
            return HostRuntimeActiveWorkStatus.LeaseRevisionRegressed;
        }
        if (item.Request.State is not (
            HostRuntimeRequestState.Pending or
            HostRuntimeRequestState.CancellationRequested))
        {
            return HostRuntimeActiveWorkStatus.InvalidRequestState;
        }
        if (!item.Lease.IsActive)
        {
            return HostRuntimeActiveWorkStatus.InvalidLeaseState;
        }
        if (observedTick < item.Attempt.AcknowledgedTick)
        {
            return HostRuntimeActiveWorkStatus.BeforeAcknowledgement;
        }
        if (observedTick >= item.Lease.ExpiresTick)
        {
            return HostRuntimeActiveWorkStatus.LeaseExpired;
        }

        return HostRuntimeActiveWorkStatus.Captured;
    }

    private static HostRuntimeActiveWorkSnapshotResult<TRequest>
        SnapshotResult<TRequest>(
            HostRuntimeActiveWorkStatus status,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeAttemptIdKind> relatedAttemptId = default)
        where TRequest : IHostRuntimeRequest =>
        new(status, snapshot: null, relatedAttemptId);

    private static HostRuntimeActiveWorkReconciliationResult<TRequest>
        ReconciliationResult<TRequest>(
            HostRuntimeActiveWorkStatus status,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeAttemptIdKind> relatedAttemptId = default)
        where TRequest : IHostRuntimeRequest =>
        new(status, reconciliation: null, relatedAttemptId);

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
