namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Produces pure advisory deadline and retry decisions for immutable Host requests.
/// </summary>
public static class HostRuntimeRetryDecisionFlow
{
    /// <summary>Evaluates one immutable advisory retry decision.</summary>
    /// <typeparam name="TRequest">Exact request payload type.</typeparam>
    /// <param name="request">Current immutable request authority.</param>
    /// <param name="expectedRevision">Request revision observed by the caller.</param>
    /// <param name="clockId">Externally owned monotonic clock domain.</param>
    /// <param name="observedTick">Current non-negative external clock tick.</param>
    /// <param name="completedAttemptNumber">One-based attempt that just completed.</param>
    /// <param name="policy">Validated immutable retry policy.</param>
    /// <param name="deadline">Optional immutable deadline in the same clock.</param>
    /// <param name="reason">Exact immutable retry reason.</param>
    /// <returns>An advisory decision without waiting or scheduling.</returns>
    public static HostRuntimeRetryDecision<TRequest> Decide<TRequest>(
        HostRuntimeRequestEnvelope<TRequest> request,
        long expectedRevision,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeClockIdKind> clockId,
        long observedTick,
        int completedAttemptNumber,
        HostRuntimeRetryPolicy policy,
        HostRuntimeDeadline? deadline,
        IHostRuntimeRetryReason reason)
        where TRequest : IHostRuntimeRequest
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(policy);
        if (expectedRevision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(expectedRevision));
        }
        if (clockId.IsEmpty)
        {
            throw new ArgumentException(
                "The clock ID must be initialized.",
                nameof(clockId));
        }
        if (observedTick < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(observedTick));
        }
        if (completedAttemptNumber < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(completedAttemptNumber));
        }
        if (deadline is not null && deadline.ClockId != clockId)
        {
            throw new ArgumentException(
                "The deadline clock must match the observed clock.",
                nameof(deadline));
        }
        HostRuntimeTypePolicy.EnsureExactRetryReason(reason);

        if (request.Revision != expectedRevision)
        {
            return Denied(
                HostRuntimeRetryDecisionStatus.StaleRevision,
                request,
                clockId,
                completedAttemptNumber,
                policy,
                deadline,
                reason);
        }
        if (request.State is not (
            HostRuntimeRequestState.Failed or
            HostRuntimeRequestState.Rejected))
        {
            return Denied(
                HostRuntimeRetryDecisionStatus.InvalidRequestState,
                request,
                clockId,
                completedAttemptNumber,
                policy,
                deadline,
                reason);
        }
        if (completedAttemptNumber >= policy.MaximumAttempts)
        {
            return Denied(
                HostRuntimeRetryDecisionStatus.AttemptLimitReached,
                request,
                clockId,
                completedAttemptNumber,
                policy,
                deadline,
                reason);
        }
        if (deadline is not null && observedTick >= deadline.DueTick)
        {
            return Denied(
                HostRuntimeRetryDecisionStatus.DeadlineExceeded,
                request,
                clockId,
                completedAttemptNumber,
                policy,
                deadline,
                reason);
        }

        var retryAtTick = checked(observedTick + policy.RetryDelayTicks);
        if (deadline is not null && retryAtTick >= deadline.DueTick)
        {
            return Denied(
                HostRuntimeRetryDecisionStatus.DeadlineExceeded,
                request,
                clockId,
                completedAttemptNumber,
                policy,
                deadline,
                reason);
        }

        return new HostRuntimeRetryDecision<TRequest>(
            HostRuntimeRetryDecisionStatus.RetryAllowed,
            request,
            clockId,
            completedAttemptNumber,
            checked(completedAttemptNumber + 1),
            retryAtTick,
            policy,
            deadline,
            reason);
    }

    private static HostRuntimeRetryDecision<TRequest> Denied<TRequest>(
        HostRuntimeRetryDecisionStatus status,
        HostRuntimeRequestEnvelope<TRequest> request,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeClockIdKind> clockId,
        int completedAttemptNumber,
        HostRuntimeRetryPolicy policy,
        HostRuntimeDeadline? deadline,
        IHostRuntimeRetryReason reason)
        where TRequest : IHostRuntimeRequest =>
        new(
            status,
            request,
            clockId,
            completedAttemptNumber,
            nextAttemptNumber: 0,
            retryAtTick: null,
            policy,
            deadline,
            reason);
}
