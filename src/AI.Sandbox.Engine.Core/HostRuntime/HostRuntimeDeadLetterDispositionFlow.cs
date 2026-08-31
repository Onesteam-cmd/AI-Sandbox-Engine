namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Converts one exhausted terminal retry decision into pure immutable
/// dead-letter disposition authority.
/// </summary>
public static class HostRuntimeDeadLetterDispositionFlow
{
    /// <summary>
    /// Disposes one retry-exhausted settlement without storing, transporting,
    /// scheduling, or executing external work.
    /// </summary>
    /// <typeparam name="TRequest">Exact request payload type.</typeparam>
    /// <typeparam name="TCompletion">Exact completion payload type.</typeparam>
    /// <param name="dispositionId">
    /// Externally assigned non-empty dead-letter disposition ID.
    /// </param>
    /// <param name="settlement">Terminal attempt-settlement authority.</param>
    /// <param name="retryDecision">
    /// Denied advisory retry decision for the exact settlement request.
    /// </param>
    /// <param name="clockId">
    /// Matching externally owned monotonic clock domain.
    /// </param>
    /// <param name="disposedTick">
    /// Non-negative external monotonic disposition tick.
    /// </param>
    /// <returns>An explicit immutable dead-letter disposition result.</returns>
    public static HostRuntimeDeadLetterDispositionResult<
        TRequest,
        TCompletion> Dispose<TRequest, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeDeadLetterDispositionIdKind> dispositionId,
            HostRuntimeAttemptSettlement<TRequest, TCompletion> settlement,
            HostRuntimeRetryDecision<TRequest> retryDecision,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeClockIdKind> clockId,
            long disposedTick)
        where TRequest : IHostRuntimeRequest
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(dispositionId.IsEmpty, nameof(dispositionId));
        ArgumentNullException.ThrowIfNull(settlement);
        ArgumentNullException.ThrowIfNull(retryDecision);
        EnsureId(clockId.IsEmpty, nameof(clockId));
        if (disposedTick < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(disposedTick));
        }

        if (settlement.OutcomeKind is not (
            HostRuntimeCompletionKind.Failed or
            HostRuntimeCompletionKind.Rejected))
        {
            return Unchanged<TRequest, TCompletion>(
                HostRuntimeDeadLetterDispositionStatus
                    .InvalidSettlementOutcome,
                settlement);
        }
        if (retryDecision.Request != settlement.Request)
        {
            return Unchanged<TRequest, TCompletion>(
                HostRuntimeDeadLetterDispositionStatus
                    .SettlementRequestMismatch,
                settlement);
        }
        if (retryDecision.CompletedAttemptNumber !=
            settlement.AttemptNumber)
        {
            return Unchanged<TRequest, TCompletion>(
                HostRuntimeDeadLetterDispositionStatus
                    .AttemptNumberMismatch,
                settlement);
        }
        if (retryDecision.ClockId != settlement.ClockId ||
            clockId != settlement.ClockId)
        {
            return Unchanged<TRequest, TCompletion>(
                HostRuntimeDeadLetterDispositionStatus.ClockMismatch,
                settlement);
        }
        if (disposedTick < settlement.SettledTick)
        {
            return Unchanged<TRequest, TCompletion>(
                HostRuntimeDeadLetterDispositionStatus.BeforeSettlement,
                settlement);
        }
        if (retryDecision.ShouldRetry)
        {
            return Unchanged<TRequest, TCompletion>(
                HostRuntimeDeadLetterDispositionStatus.RetryStillAllowed,
                settlement);
        }

        var kind = retryDecision.Status switch
        {
            HostRuntimeRetryDecisionStatus.AttemptLimitReached =>
                HostRuntimeDeadLetterDispositionKind.AttemptLimitReached,
            HostRuntimeRetryDecisionStatus.DeadlineExceeded =>
                HostRuntimeDeadLetterDispositionKind.DeadlineExceeded,
            _ => (HostRuntimeDeadLetterDispositionKind?)null,
        };

        if (kind is null)
        {
            return Unchanged<TRequest, TCompletion>(
                HostRuntimeDeadLetterDispositionStatus
                    .UnsupportedRetryDenial,
                settlement);
        }

        var disposition =
            new HostRuntimeDeadLetterDisposition<TRequest, TCompletion>(
                dispositionId,
                kind.Value,
                settlement,
                retryDecision,
                disposedTick);

        return new HostRuntimeDeadLetterDispositionResult<
            TRequest,
            TCompletion>(
                HostRuntimeDeadLetterDispositionStatus.Disposed,
                disposition,
                settlement);
    }

    private static HostRuntimeDeadLetterDispositionResult<
        TRequest,
        TCompletion> Unchanged<TRequest, TCompletion>(
            HostRuntimeDeadLetterDispositionStatus status,
            HostRuntimeAttemptSettlement<TRequest, TCompletion> settlement)
        where TRequest : IHostRuntimeRequest
        where TCompletion : IHostRuntimeCompletion =>
        new(status, null, settlement);

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
