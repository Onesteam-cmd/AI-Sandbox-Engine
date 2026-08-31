namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Contains one immutable bounded retry policy.</summary>
public sealed record HostRuntimeRetryPolicy
{
    /// <summary>Gets the maximum supported attempt count.</summary>
    public const int MaximumAttemptCount = 32;

    /// <summary>Gets the maximum supported advisory retry delay.</summary>
    public const long MaximumRetryDelayTicks = 1_000_000_000;

    internal HostRuntimeRetryPolicy(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRetryPolicyIdKind> policyId,
        int maximumAttempts,
        long retryDelayTicks)
    {
        PolicyId = policyId;
        MaximumAttempts = maximumAttempts;
        RetryDelayTicks = retryDelayTicks;
    }

    /// <summary>Gets the externally assigned retry-policy ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRetryPolicyIdKind> PolicyId { get; }

    /// <summary>Gets the inclusive maximum number of attempts.</summary>
    public int MaximumAttempts { get; }

    /// <summary>Gets the advisory delay before the next attempt.</summary>
    public long RetryDelayTicks { get; }

    /// <summary>Creates one validated immutable bounded retry policy.</summary>
    /// <param name="policyId">Externally assigned non-empty policy ID.</param>
    /// <param name="maximumAttempts">Inclusive attempt limit from 1 through 32.</param>
    /// <param name="retryDelayTicks">Advisory non-negative delay ticks.</param>
    /// <returns>A validated immutable retry policy.</returns>
    public static HostRuntimeRetryPolicy Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRetryPolicyIdKind> policyId,
        int maximumAttempts,
        long retryDelayTicks)
    {
        if (policyId.IsEmpty)
        {
            throw new ArgumentException(
                "The retry-policy ID must be initialized.",
                nameof(policyId));
        }
        if (maximumAttempts < 1 ||
            maximumAttempts > MaximumAttemptCount)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumAttempts));
        }
        if (retryDelayTicks < 0 ||
            retryDelayTicks > MaximumRetryDelayTicks)
        {
            throw new ArgumentOutOfRangeException(nameof(retryDelayTicks));
        }

        return new HostRuntimeRetryPolicy(
            policyId,
            maximumAttempts,
            retryDelayTicks);
    }
}
