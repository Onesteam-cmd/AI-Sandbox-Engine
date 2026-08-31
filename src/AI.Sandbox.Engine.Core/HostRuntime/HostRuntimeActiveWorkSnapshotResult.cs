namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host active-work snapshot capture result.
/// </summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeActiveWorkSnapshotResult<TRequest>
    where TRequest : IHostRuntimeRequest
{
    internal HostRuntimeActiveWorkSnapshotResult(
        HostRuntimeActiveWorkStatus status,
        HostRuntimeActiveWorkSnapshot<TRequest>? snapshot,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeAttemptIdKind> relatedAttemptId)
    {
        Status = status;
        Snapshot = snapshot;
        RelatedAttemptId = relatedAttemptId;
    }

    /// <summary>Gets the explicit capture outcome.</summary>
    public HostRuntimeActiveWorkStatus Status { get; }

    /// <summary>Gets captured snapshot authority when successful.</summary>
    public HostRuntimeActiveWorkSnapshot<TRequest>? Snapshot { get; }

    /// <summary>Gets the related attempt ID for an item-specific rejection.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind> RelatedAttemptId { get; }

    /// <summary>Gets whether snapshot authority was captured.</summary>
    public bool Succeeded => Status == HostRuntimeActiveWorkStatus.Captured;
}
