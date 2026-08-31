namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host active-work reconciliation result.
/// </summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeActiveWorkReconciliationResult<TRequest>
    where TRequest : IHostRuntimeRequest
{
    internal HostRuntimeActiveWorkReconciliationResult(
        HostRuntimeActiveWorkStatus status,
        HostRuntimeActiveWorkReconciliation<TRequest>? reconciliation,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeAttemptIdKind> relatedAttemptId)
    {
        Status = status;
        Reconciliation = reconciliation;
        RelatedAttemptId = relatedAttemptId;
    }

    /// <summary>Gets the explicit reconciliation outcome.</summary>
    public HostRuntimeActiveWorkStatus Status { get; }

    /// <summary>Gets reconciliation authority when successful.</summary>
    public HostRuntimeActiveWorkReconciliation<TRequest>? Reconciliation
    {
        get;
    }

    /// <summary>Gets the related attempt ID for an item-specific rejection.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind> RelatedAttemptId { get; }

    /// <summary>Gets whether reconciliation authority was created.</summary>
    public bool Succeeded => Status == HostRuntimeActiveWorkStatus.Reconciled;
}
