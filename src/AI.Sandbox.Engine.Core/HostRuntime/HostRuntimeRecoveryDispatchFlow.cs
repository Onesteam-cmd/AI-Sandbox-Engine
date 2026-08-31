namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Creates pure recovery dispatch reconstruction and resumed-attempt
/// acknowledgement authority without transport, scheduling, supervision,
/// persistence, waiting, or execution.
/// </summary>
public static class HostRuntimeRecoveryDispatchFlow
{
    /// <summary>
    /// Reconstructs one new advisory dispatch from exact recovery
    /// lease-reacquisition authority.
    /// </summary>
    /// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
    /// <typeparam name="TState">Exact immutable World State root type.</typeparam>
    /// <param name="reconstructionId">Externally assigned reconstruction ID.</param>
    /// <param name="selectionId">Externally assigned new dispatch-selection ID.</param>
    /// <param name="reacquisition">Existing immutable lease-reacquisition authority.</param>
    /// <param name="expectedReacquisitionRevision">
    /// Lease-reacquisition revision observed by the caller.
    /// </param>
    /// <param name="snapshot">Current immutable recovery queue authority.</param>
    /// <param name="expectedQueueRevision">Queue revision observed by the caller.</param>
    /// <param name="clockId">Matching externally owned monotonic clock.</param>
    /// <param name="reconstructedTick">External monotonic reconstruction tick.</param>
    /// <param name="dispatchId">Externally assigned new dispatch ID.</param>
    /// <param name="routeId">Externally assigned recovery route ID.</param>
    /// <param name="endpointId">Externally assigned recovery endpoint ID.</param>
    /// <param name="attemptNumber">Expected next one-based attempt number.</param>
    /// <returns>An explicit immutable recovery reconstruction result.</returns>
    public static HostRuntimeRecoveryDispatchReconstructionResult<
        TRequest,
        TState> Reconstruct<TRequest, TState>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryDispatchReconstructionIdKind>
                    reconstructionId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeDispatchSelectionIdKind> selectionId,
            HostRuntimeRecoveryLeaseReacquisition<TRequest, TState>
                reacquisition,
            long expectedReacquisitionRevision,
            HostRuntimeQueueSnapshot snapshot,
            long expectedQueueRevision,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeClockIdKind> clockId,
            long reconstructedTick,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeDispatchIdKind> dispatchId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRouteIdKind> routeId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeEndpointIdKind> endpointId,
            int attemptNumber)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    {
        EnsureId(reconstructionId.IsEmpty, nameof(reconstructionId));
        EnsureId(selectionId.IsEmpty, nameof(selectionId));
        ArgumentNullException.ThrowIfNull(reacquisition);
        ArgumentNullException.ThrowIfNull(snapshot);
        EnsureRevision(
            expectedReacquisitionRevision,
            nameof(expectedReacquisitionRevision));
        EnsureRevision(expectedQueueRevision, nameof(expectedQueueRevision));
        EnsureId(clockId.IsEmpty, nameof(clockId));
        EnsureTick(reconstructedTick, nameof(reconstructedTick));
        EnsureId(dispatchId.IsEmpty, nameof(dispatchId));
        EnsureId(routeId.IsEmpty, nameof(routeId));
        EnsureId(endpointId.IsEmpty, nameof(endpointId));
        EnsureAttemptNumber(attemptNumber, nameof(attemptNumber));

        if (reacquisition.Revision != expectedReacquisitionRevision)
        {
            return ReconstructionResult<TRequest, TState>(
                HostRuntimeRecoveryDispatchStatus.StaleReacquisitionRevision,
                reacquisition,
                snapshot);
        }
        if (reconstructedTick < reacquisition.ReacquiredTick)
        {
            return ReconstructionResult<TRequest, TState>(
                HostRuntimeRecoveryDispatchStatus.ReconstructionTickRegressed,
                reacquisition,
                snapshot);
        }
        if (snapshot.QueueId != reacquisition.QueueId)
        {
            return ReconstructionResult<TRequest, TState>(
                HostRuntimeRecoveryDispatchStatus.QueueMismatch,
                reacquisition,
                snapshot);
        }
        if (selectionId == reacquisition.Selection.Candidate.Attempt
                .Selection.SelectionId)
        {
            return ReconstructionResult<TRequest, TState>(
                HostRuntimeRecoveryDispatchStatus.PriorSelectionIdReused,
                reacquisition,
                snapshot);
        }
        if (dispatchId == reacquisition.Selection.Candidate.Attempt.DispatchId)
        {
            return ReconstructionResult<TRequest, TState>(
                HostRuntimeRecoveryDispatchStatus.PriorDispatchIdReused,
                reacquisition,
                snapshot);
        }

        var expectedAttemptNumber = checked(
            reacquisition.Selection.Candidate.AttemptNumber + 1);
        if (attemptNumber != expectedAttemptNumber)
        {
            return ReconstructionResult<TRequest, TState>(
                HostRuntimeRecoveryDispatchStatus.AttemptNumberMismatch,
                reacquisition,
                snapshot);
        }

        var selectionResult = HostRuntimeDispatchSelectionFlow.Select(
            selectionId,
            snapshot,
            expectedQueueRevision,
            reacquisition.Lease,
            clockId,
            reconstructedTick,
            dispatchId,
            routeId,
            endpointId,
            attemptNumber);
        if (!selectionResult.Succeeded)
        {
            return new HostRuntimeRecoveryDispatchReconstructionResult<
                TRequest,
                TState>(
                    HostRuntimeRecoveryDispatchStatus.DispatchSelectionRejected,
                    reacquisition,
                    selectionResult.Snapshot,
                    selectionResult.Status,
                    reconstruction: null);
        }

        var reconstruction =
            new HostRuntimeRecoveryDispatchReconstruction<TRequest, TState>(
                reconstructionId,
                reacquisition,
                selectionResult.Selection!,
                reconstructedTick,
                checked(reacquisition.Revision + 1));

        return new HostRuntimeRecoveryDispatchReconstructionResult<
            TRequest,
            TState>(
                HostRuntimeRecoveryDispatchStatus.DispatchReconstructed,
                reacquisition,
                selectionResult.Snapshot,
                selectionResult.Status,
                reconstruction);
    }

    /// <summary>
    /// Acknowledges one reconstructed recovery dispatch as a new resumed attempt.
    /// </summary>
    /// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
    /// <typeparam name="TState">Exact immutable World State root type.</typeparam>
    /// <param name="acknowledgementId">
    /// Externally assigned resumed-attempt acknowledgement ID.
    /// </param>
    /// <param name="attemptId">Externally assigned new resumed attempt ID.</param>
    /// <param name="reconstruction">
    /// Existing immutable recovery dispatch reconstruction authority.
    /// </param>
    /// <param name="expectedReconstructionRevision">
    /// Reconstruction revision observed by the caller.
    /// </param>
    /// <param name="request">Current immutable request authority.</param>
    /// <param name="lease">Current immutable recovery lease authority.</param>
    /// <param name="expectedRequestRevision">
    /// Request revision observed by the acknowledging caller.
    /// </param>
    /// <param name="expectedLeaseRevision">
    /// Lease revision observed by the acknowledging caller.
    /// </param>
    /// <param name="acknowledgedLeaseId">Lease ID carried by acknowledgement.</param>
    /// <param name="acknowledgedWorkerId">Worker ID carried by acknowledgement.</param>
    /// <param name="acknowledgedDispatchId">
    /// Dispatch ID carried by acknowledgement.
    /// </param>
    /// <param name="acknowledgedRequestId">
    /// Request ID carried by acknowledgement.
    /// </param>
    /// <param name="acknowledgedAttemptNumber">
    /// Attempt number carried by acknowledgement.
    /// </param>
    /// <param name="clockId">Matching externally owned monotonic clock.</param>
    /// <param name="acknowledgedTick">External monotonic acknowledgement tick.</param>
    /// <returns>An explicit immutable resumed-attempt acknowledgement result.</returns>
    public static HostRuntimeRecoveryResumedAttemptAcknowledgementResult<
        TRequest,
        TState> Acknowledge<TRequest, TState>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryResumedAttemptAcknowledgementIdKind>
                    acknowledgementId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeAttemptIdKind> attemptId,
            HostRuntimeRecoveryDispatchReconstruction<TRequest, TState>
                reconstruction,
            long expectedReconstructionRevision,
            HostRuntimeRequestEnvelope<TRequest> request,
            HostRuntimeWorkLease<TRequest> lease,
            long expectedRequestRevision,
            long expectedLeaseRevision,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeLeaseIdKind> acknowledgedLeaseId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeWorkerIdKind> acknowledgedWorkerId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeDispatchIdKind> acknowledgedDispatchId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRequestIdKind> acknowledgedRequestId,
            int acknowledgedAttemptNumber,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeClockIdKind> clockId,
            long acknowledgedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    {
        EnsureId(acknowledgementId.IsEmpty, nameof(acknowledgementId));
        EnsureId(attemptId.IsEmpty, nameof(attemptId));
        ArgumentNullException.ThrowIfNull(reconstruction);
        EnsureRevision(
            expectedReconstructionRevision,
            nameof(expectedReconstructionRevision));
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(lease);
        EnsureRevision(expectedRequestRevision, nameof(expectedRequestRevision));
        EnsureRevision(expectedLeaseRevision, nameof(expectedLeaseRevision));
        EnsureId(acknowledgedLeaseId.IsEmpty, nameof(acknowledgedLeaseId));
        EnsureId(acknowledgedWorkerId.IsEmpty, nameof(acknowledgedWorkerId));
        EnsureId(acknowledgedDispatchId.IsEmpty, nameof(acknowledgedDispatchId));
        EnsureId(acknowledgedRequestId.IsEmpty, nameof(acknowledgedRequestId));
        EnsureAttemptNumber(
            acknowledgedAttemptNumber,
            nameof(acknowledgedAttemptNumber));
        EnsureId(clockId.IsEmpty, nameof(clockId));
        EnsureTick(acknowledgedTick, nameof(acknowledgedTick));

        if (reconstruction.Revision != expectedReconstructionRevision)
        {
            return AcknowledgementResult<TRequest, TState>(
                HostRuntimeRecoveryDispatchStatus.StaleReconstructionRevision,
                reconstruction);
        }
        if (acknowledgedTick < reconstruction.ReconstructedTick)
        {
            return AcknowledgementResult<TRequest, TState>(
                HostRuntimeRecoveryDispatchStatus.AcknowledgementTickRegressed,
                reconstruction);
        }
        if (attemptId == reconstruction.PriorAttemptId)
        {
            return AcknowledgementResult<TRequest, TState>(
                HostRuntimeRecoveryDispatchStatus.PriorAttemptIdReused,
                reconstruction);
        }

        var acknowledgementResult =
            HostRuntimeDispatchAcknowledgementFlow.Acknowledge(
                attemptId,
                reconstruction.Selection,
                request,
                lease,
                expectedRequestRevision,
                expectedLeaseRevision,
                acknowledgedLeaseId,
                acknowledgedWorkerId,
                acknowledgedDispatchId,
                acknowledgedRequestId,
                acknowledgedAttemptNumber,
                clockId,
                acknowledgedTick);
        if (!acknowledgementResult.Succeeded)
        {
            return new HostRuntimeRecoveryResumedAttemptAcknowledgementResult<
                TRequest,
                TState>(
                    HostRuntimeRecoveryDispatchStatus
                        .DispatchAcknowledgementRejected,
                    reconstruction,
                    acknowledgementResult.Status,
                    acknowledgement: null);
        }

        var acknowledgement =
            new HostRuntimeRecoveryResumedAttemptAcknowledgement<
                TRequest,
                TState>(
                    acknowledgementId,
                    reconstruction,
                    acknowledgementResult.Attempt!,
                    acknowledgedTick,
                    checked(reconstruction.Revision + 1));

        return new HostRuntimeRecoveryResumedAttemptAcknowledgementResult<
            TRequest,
            TState>(
                HostRuntimeRecoveryDispatchStatus.AttemptAcknowledged,
                reconstruction,
                acknowledgementResult.Status,
                acknowledgement);
    }

    private static HostRuntimeRecoveryDispatchReconstructionResult<
        TRequest,
        TState> ReconstructionResult<TRequest, TState>(
            HostRuntimeRecoveryDispatchStatus status,
            HostRuntimeRecoveryLeaseReacquisition<TRequest, TState>
                reacquisition,
            HostRuntimeQueueSnapshot snapshot)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState =>
        new(
            status,
            reacquisition,
            snapshot,
            selectionStatus: null,
            reconstruction: null);

    private static HostRuntimeRecoveryResumedAttemptAcknowledgementResult<
        TRequest,
        TState> AcknowledgementResult<TRequest, TState>(
            HostRuntimeRecoveryDispatchStatus status,
            HostRuntimeRecoveryDispatchReconstruction<TRequest, TState>
                reconstruction)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState =>
        new(
            status,
            reconstruction,
            acknowledgementStatus: null,
            acknowledgement: null);

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

    private static void EnsureAttemptNumber(
        int attemptNumber,
        string parameterName)
    {
        if (attemptNumber < 1 ||
            attemptNumber > HostRuntimeRetryPolicy.MaximumAttemptCount)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}
