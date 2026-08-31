namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Creates pure recovery queue re-admission and lease-reacquisition authority
/// without dispatch, attempt creation, scheduling, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryReadmissionFlow
{
    /// <summary>
    /// Re-admits one exact resumed-work selection to its recovery queue.
    /// </summary>
    /// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
    /// <typeparam name="TState">Exact immutable World State root type.</typeparam>
    /// <param name="readmissionId">Externally assigned recovery re-admission ID.</param>
    /// <param name="admissionId">Externally assigned new queue admission ID.</param>
    /// <param name="selection">Existing immutable resumed-work selection.</param>
    /// <param name="expectedSelectionRevision">
    /// Selection revision observed by the caller.
    /// </param>
    /// <param name="snapshot">Current immutable recovery queue authority.</param>
    /// <param name="expectedQueueRevision">
    /// Queue revision observed by the caller.
    /// </param>
    /// <param name="readmittedTick">External monotonic re-admission tick.</param>
    /// <returns>An explicit immutable recovery re-admission result.</returns>
    public static HostRuntimeRecoveryReadmissionResult<TRequest, TState>
        Readmit<TRequest, TState>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryReadmissionIdKind> readmissionId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeAdmissionIdKind> admissionId,
            HostRuntimeResumedWorkSelection<TRequest, TState> selection,
            long expectedSelectionRevision,
            HostRuntimeQueueSnapshot snapshot,
            long expectedQueueRevision,
            long readmittedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    {
        EnsureId(readmissionId.IsEmpty, nameof(readmissionId));
        EnsureId(admissionId.IsEmpty, nameof(admissionId));
        ArgumentNullException.ThrowIfNull(selection);
        ArgumentNullException.ThrowIfNull(snapshot);
        EnsureRevision(expectedSelectionRevision, nameof(expectedSelectionRevision));
        EnsureRevision(expectedQueueRevision, nameof(expectedQueueRevision));
        EnsureTick(readmittedTick, nameof(readmittedTick));

        if (selection.Revision != expectedSelectionRevision)
        {
            return ReadmissionResult<TRequest, TState>(
                HostRuntimeRecoveryReadmissionStatus.StaleSelectionRevision,
                selection,
                snapshot);
        }
        if (readmittedTick < selection.SelectedTick)
        {
            return ReadmissionResult<TRequest, TState>(
                HostRuntimeRecoveryReadmissionStatus.ReadmissionTickRegressed,
                selection,
                snapshot);
        }

        var candidate = selection.Candidate;
        var priorAdmission = candidate.Lease.Admission;
        var checkpointQueue =
            selection.Plan.Continuation.Checkpoint.QueueSnapshot;

        if (priorAdmission.QueueId != checkpointQueue.QueueId ||
            snapshot.QueueId != checkpointQueue.QueueId)
        {
            return ReadmissionResult<TRequest, TState>(
                HostRuntimeRecoveryReadmissionStatus.QueueMismatch,
                selection,
                snapshot);
        }
        if (admissionId == priorAdmission.AdmissionId)
        {
            return ReadmissionResult<TRequest, TState>(
                HostRuntimeRecoveryReadmissionStatus.PriorAdmissionIdReused,
                selection,
                snapshot);
        }

        var admissionResult = HostRuntimeQueueAdmissionFlow.Decide(
            admissionId,
            snapshot,
            expectedQueueRevision,
            candidate.Request,
            priorAdmission.Priority);
        if (!admissionResult.Succeeded)
        {
            var status = admissionResult.Status switch
            {
                HostRuntimeQueueAdmissionStatus.StaleQueueRevision =>
                    HostRuntimeRecoveryReadmissionStatus.StaleQueueRevision,
                HostRuntimeQueueAdmissionStatus.QueueFull =>
                    HostRuntimeRecoveryReadmissionStatus.QueueFull,
                _ =>
                    HostRuntimeRecoveryReadmissionStatus.AdmissionRejected,
            };

            return ReadmissionResult<TRequest, TState>(
                status,
                selection,
                admissionResult.Snapshot);
        }

        var readmission =
            new HostRuntimeRecoveryReadmission<TRequest, TState>(
                readmissionId,
                selection,
                admissionResult.Admission!,
                readmittedTick,
                checked(selection.Revision + 1));

        return new HostRuntimeRecoveryReadmissionResult<TRequest, TState>(
            HostRuntimeRecoveryReadmissionStatus.Readmitted,
            selection,
            admissionResult.Snapshot,
            readmission);
    }

    /// <summary>
    /// Reacquires one new active lease for re-admitted recovery work.
    /// </summary>
    /// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
    /// <typeparam name="TState">Exact immutable World State root type.</typeparam>
    /// <param name="reacquisitionId">
    /// Externally assigned lease-reacquisition authority ID.
    /// </param>
    /// <param name="leaseId">Externally assigned new lease ID.</param>
    /// <param name="workerId">Externally assigned recovery worker ID.</param>
    /// <param name="readmission">Existing immutable recovery re-admission.</param>
    /// <param name="expectedReadmissionRevision">
    /// Re-admission revision observed by the caller.
    /// </param>
    /// <param name="clockId">Matching externally owned monotonic clock.</param>
    /// <param name="reacquiredTick">External monotonic reacquisition tick.</param>
    /// <param name="durationTicks">Positive bounded lease duration.</param>
    /// <returns>An explicit immutable lease-reacquisition result.</returns>
    public static HostRuntimeRecoveryLeaseReacquisitionResult<TRequest, TState>
        Reacquire<TRequest, TState>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryLeaseReacquisitionIdKind> reacquisitionId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeLeaseIdKind> leaseId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeWorkerIdKind> workerId,
            HostRuntimeRecoveryReadmission<TRequest, TState> readmission,
            long expectedReadmissionRevision,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeClockIdKind> clockId,
            long reacquiredTick,
            long durationTicks)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    {
        EnsureId(reacquisitionId.IsEmpty, nameof(reacquisitionId));
        EnsureId(leaseId.IsEmpty, nameof(leaseId));
        EnsureId(workerId.IsEmpty, nameof(workerId));
        ArgumentNullException.ThrowIfNull(readmission);
        EnsureRevision(
            expectedReadmissionRevision,
            nameof(expectedReadmissionRevision));
        EnsureId(clockId.IsEmpty, nameof(clockId));
        EnsureTick(reacquiredTick, nameof(reacquiredTick));
        EnsureDuration(durationTicks, nameof(durationTicks));

        if (readmission.Revision != expectedReadmissionRevision)
        {
            return ReacquisitionResult<TRequest, TState>(
                HostRuntimeRecoveryReadmissionStatus.StaleReadmissionRevision,
                readmission);
        }
        if (reacquiredTick < readmission.ReadmittedTick)
        {
            return ReacquisitionResult<TRequest, TState>(
                HostRuntimeRecoveryReadmissionStatus.ReacquisitionTickRegressed,
                readmission);
        }
        if (clockId != readmission.ClockId)
        {
            return ReacquisitionResult<TRequest, TState>(
                HostRuntimeRecoveryReadmissionStatus.ClockMismatch,
                readmission);
        }
        if (leaseId == readmission.Selection.Candidate.LeaseId)
        {
            return ReacquisitionResult<TRequest, TState>(
                HostRuntimeRecoveryReadmissionStatus.PriorLeaseIdReused,
                readmission);
        }

        var lease = HostRuntimeWorkLeaseFlow.Acquire(
            leaseId,
            workerId,
            readmission.Admission,
            clockId,
            reacquiredTick,
            durationTicks);
        var reacquisition =
            new HostRuntimeRecoveryLeaseReacquisition<TRequest, TState>(
                reacquisitionId,
                readmission,
                lease,
                reacquiredTick,
                checked(readmission.Revision + 1));

        return new HostRuntimeRecoveryLeaseReacquisitionResult<TRequest, TState>(
            HostRuntimeRecoveryReadmissionStatus.LeaseReacquired,
            readmission,
            reacquisition);
    }

    private static HostRuntimeRecoveryReadmissionResult<TRequest, TState>
        ReadmissionResult<TRequest, TState>(
            HostRuntimeRecoveryReadmissionStatus status,
            HostRuntimeResumedWorkSelection<TRequest, TState> selection,
            HostRuntimeQueueSnapshot snapshot)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState =>
        new(status, selection, snapshot, readmission: null);

    private static HostRuntimeRecoveryLeaseReacquisitionResult<TRequest, TState>
        ReacquisitionResult<TRequest, TState>(
            HostRuntimeRecoveryReadmissionStatus status,
            HostRuntimeRecoveryReadmission<TRequest, TState> readmission)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState =>
        new(status, readmission, reacquisition: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new ArgumentException(
                "The identifier must be initialized.",
                parameterName);
        }
    }

    private static void EnsureRevision(long revision, string parameterName)
    {
        if (revision < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }

    private static void EnsureDuration(long duration, string parameterName)
    {
        if (duration < 1 ||
            duration > HostRuntimeWorkLeaseFlow.MaximumLeaseDurationTicks)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}
