namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable advisory retry decision.</summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeRetryDecision<TRequest>
    where TRequest : IHostRuntimeRequest
{
    internal HostRuntimeRetryDecision(
        HostRuntimeRetryDecisionStatus status,
        HostRuntimeRequestEnvelope<TRequest> request,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeClockIdKind> clockId,
        int completedAttemptNumber,
        int nextAttemptNumber,
        long? retryAtTick,
        HostRuntimeRetryPolicy policy,
        HostRuntimeDeadline? deadline,
        IHostRuntimeRetryReason reason)
    {
        Status = status;
        Request = request;
        ClockId = clockId;
        CompletedAttemptNumber = completedAttemptNumber;
        NextAttemptNumber = nextAttemptNumber;
        RetryAtTick = retryAtTick;
        Policy = policy;
        Deadline = deadline;
        Reason = reason;
    }

    /// <summary>Gets the explicit advisory decision status.</summary>
    public HostRuntimeRetryDecisionStatus Status { get; }

    /// <summary>Gets the unchanged immutable request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request { get; }

    /// <summary>Gets the externally owned monotonic clock domain.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId { get; }

    /// <summary>Gets the one-based attempt number that just completed.</summary>
    public int CompletedAttemptNumber { get; }

    /// <summary>Gets the one-based next attempt number, or zero when denied.</summary>
    public int NextAttemptNumber { get; }

    /// <summary>Gets the advisory retry tick, or null when retry is denied.</summary>
    public long? RetryAtTick { get; }

    /// <summary>Gets the exact immutable retry policy.</summary>
    public HostRuntimeRetryPolicy Policy { get; }

    /// <summary>Gets the optional immutable deadline.</summary>
    public HostRuntimeDeadline? Deadline { get; }

    /// <summary>Gets the exact immutable retry reason.</summary>
    public IHostRuntimeRetryReason Reason { get; }

    /// <summary>Gets whether the external Host may schedule another attempt.</summary>
    public bool ShouldRetry =>
        Status == HostRuntimeRetryDecisionStatus.RetryAllowed;
}
