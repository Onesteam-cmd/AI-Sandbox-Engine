namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Creates pure recovery resumed-attempt settlement and recovery-cycle completion
/// authority without retry routing, transport, scheduling, supervision,
/// persistence, waiting, or execution.
/// </summary>
public static class HostRuntimeRecoverySettlementFlow
{
    /// <summary>
    /// Settles one acknowledged resumed attempt through existing terminal
    /// attempt-settlement contracts.
    /// </summary>
    /// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
    /// <typeparam name="TState">Exact immutable World State root type.</typeparam>
    /// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
    /// <param name="recoverySettlementId">
    /// Externally assigned recovery settlement ID.
    /// </param>
    /// <param name="settlementId">Externally assigned terminal settlement ID.</param>
    /// <param name="acknowledgement">
    /// Existing immutable resumed-attempt acknowledgement authority.
    /// </param>
    /// <param name="expectedAcknowledgementRevision">
    /// Acknowledgement revision observed by the settling caller.
    /// </param>
    /// <param name="request">Current immutable request authority.</param>
    /// <param name="lease">Current immutable recovery lease authority.</param>
    /// <param name="expectedRequestRevision">
    /// Request revision observed by the settling caller.
    /// </param>
    /// <param name="expectedLeaseRevision">
    /// Lease revision observed by the settling caller.
    /// </param>
    /// <param name="settlingWorkerId">Worker reporting terminal settlement.</param>
    /// <param name="clockId">Matching externally owned monotonic clock.</param>
    /// <param name="settledTick">External monotonic recovery settlement tick.</param>
    /// <param name="completion">Immutable external completion for the resumed attempt.</param>
    /// <returns>An explicit immutable recovery settlement result.</returns>
    public static HostRuntimeRecoveryResumedAttemptSettlementResult<
        TRequest,
        TState,
        TCompletion> Settle<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryResumedAttemptSettlementIdKind>
                    recoverySettlementId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeSettlementIdKind> settlementId,
            HostRuntimeRecoveryResumedAttemptAcknowledgement<TRequest, TState>
                acknowledgement,
            long expectedAcknowledgementRevision,
            HostRuntimeRequestEnvelope<TRequest> request,
            HostRuntimeWorkLease<TRequest> lease,
            long expectedRequestRevision,
            long expectedLeaseRevision,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeWorkerIdKind> settlingWorkerId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeClockIdKind> clockId,
            long settledTick,
            HostRuntimeCompletionEnvelope<TCompletion> completion)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(recoverySettlementId.IsEmpty, nameof(recoverySettlementId));
        EnsureId(settlementId.IsEmpty, nameof(settlementId));
        ArgumentNullException.ThrowIfNull(acknowledgement);
        EnsureRevision(
            expectedAcknowledgementRevision,
            nameof(expectedAcknowledgementRevision));
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(lease);
        EnsureRevision(expectedRequestRevision, nameof(expectedRequestRevision));
        EnsureRevision(expectedLeaseRevision, nameof(expectedLeaseRevision));
        EnsureId(settlingWorkerId.IsEmpty, nameof(settlingWorkerId));
        EnsureId(clockId.IsEmpty, nameof(clockId));
        EnsureTick(settledTick, nameof(settledTick));
        ArgumentNullException.ThrowIfNull(completion);

        if (acknowledgement.Revision != expectedAcknowledgementRevision)
        {
            return Unchanged(
                HostRuntimeRecoverySettlementStatus
                    .StaleAcknowledgementRevision,
                acknowledgement,
                request,
                lease,
                completion);
        }
        if (settledTick < acknowledgement.AcknowledgedTick)
        {
            return Unchanged(
                HostRuntimeRecoverySettlementStatus.SettlementTickRegressed,
                acknowledgement,
                request,
                lease,
                completion);
        }
        var settlementResult = HostRuntimeAttemptSettlementFlow.Settle(
            settlementId,
            acknowledgement.Attempt,
            request,
            lease,
            expectedRequestRevision,
            expectedLeaseRevision,
            settlingWorkerId,
            clockId,
            settledTick,
            completion);
        if (!settlementResult.Succeeded)
        {
            return new HostRuntimeRecoveryResumedAttemptSettlementResult<
                TRequest,
                TState,
                TCompletion>(
                    HostRuntimeRecoverySettlementStatus
                        .AttemptSettlementRejected,
                    acknowledgement,
                    settlementResult.Request,
                    settlementResult.Lease,
                    settlementResult.Completion,
                    settlementResult.Status,
                    settlement: null);
        }

        var settlement =
            new HostRuntimeRecoveryResumedAttemptSettlement<
                TRequest,
                TState,
                TCompletion>(
                    recoverySettlementId,
                    acknowledgement,
                    settlementResult.Settlement!,
                    checked(acknowledgement.Revision + 1));

        return new HostRuntimeRecoveryResumedAttemptSettlementResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoverySettlementStatus.AttemptSettled,
                acknowledgement,
                settlementResult.Request,
                settlementResult.Lease,
                settlementResult.Completion,
                settlementResult.Status,
                settlement);
    }

    /// <summary>
    /// Closes one exact recovery cycle after successful resumed-attempt settlement.
    /// </summary>
    /// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
    /// <typeparam name="TState">Exact immutable World State root type.</typeparam>
    /// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
    /// <param name="cycleCompletionId">
    /// Externally assigned recovery-cycle completion ID.
    /// </param>
    /// <param name="settlement">
    /// Existing immutable recovery resumed-attempt settlement authority.
    /// </param>
    /// <param name="expectedSettlementRevision">
    /// Recovery settlement revision observed by the completing caller.
    /// </param>
    /// <param name="completedTick">
    /// External monotonic recovery-cycle completion tick.
    /// </param>
    /// <returns>An explicit immutable recovery-cycle completion result.</returns>
    public static HostRuntimeRecoveryCycleCompletionResult<
        TRequest,
        TState,
        TCompletion> Complete<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCycleCompletionIdKind> cycleCompletionId,
            HostRuntimeRecoveryResumedAttemptSettlement<
                TRequest,
                TState,
                TCompletion> settlement,
            long expectedSettlementRevision,
            long completedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(cycleCompletionId.IsEmpty, nameof(cycleCompletionId));
        ArgumentNullException.ThrowIfNull(settlement);
        EnsureRevision(
            expectedSettlementRevision,
            nameof(expectedSettlementRevision));
        EnsureTick(completedTick, nameof(completedTick));

        if (settlement.Revision != expectedSettlementRevision)
        {
            return CompletionResult(
                HostRuntimeRecoverySettlementStatus.StaleSettlementRevision,
                settlement);
        }
        if (completedTick < settlement.SettledTick)
        {
            return CompletionResult(
                HostRuntimeRecoverySettlementStatus.CompletionTickRegressed,
                settlement);
        }

        var cycleCompletion =
            new HostRuntimeRecoveryCycleCompletion<
                TRequest,
                TState,
                TCompletion>(
                    cycleCompletionId,
                    settlement,
                    completedTick,
                    checked(settlement.Revision + 1));

        return new HostRuntimeRecoveryCycleCompletionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoverySettlementStatus.CycleCompleted,
                settlement,
                cycleCompletion);
    }

    private static HostRuntimeRecoveryResumedAttemptSettlementResult<
        TRequest,
        TState,
        TCompletion> Unchanged<TRequest, TState, TCompletion>(
            HostRuntimeRecoverySettlementStatus status,
            HostRuntimeRecoveryResumedAttemptAcknowledgement<TRequest, TState>
                acknowledgement,
            HostRuntimeRequestEnvelope<TRequest> request,
            HostRuntimeWorkLease<TRequest> lease,
            HostRuntimeCompletionEnvelope<TCompletion> completion)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(
            status,
            acknowledgement,
            request,
            lease,
            completion,
            attemptSettlementStatus: null,
            settlement: null);

    private static HostRuntimeRecoveryCycleCompletionResult<
        TRequest,
        TState,
        TCompletion> CompletionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoverySettlementStatus status,
            HostRuntimeRecoveryResumedAttemptSettlement<
                TRequest,
                TState,
                TCompletion> settlement)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, settlement, cycleCompletion: null);

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
}
