namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable deterministic differences between sequential active-work
/// snapshots.
/// </summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeActiveWorkReconciliation<TRequest>
    where TRequest : IHostRuntimeRequest
{
    private readonly global::System.Collections.ObjectModel.ReadOnlyCollection<
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeAttemptIdKind>> addedAttemptIds;
    private readonly global::System.Collections.ObjectModel.ReadOnlyCollection<
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeAttemptIdKind>> retainedAttemptIds;
    private readonly global::System.Collections.ObjectModel.ReadOnlyCollection<
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeAttemptIdKind>> removedAttemptIds;

    internal HostRuntimeActiveWorkReconciliation(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeActiveWorkReconciliationIdKind> reconciliationId,
        HostRuntimeActiveWorkSnapshot<TRequest> previousSnapshot,
        HostRuntimeActiveWorkSnapshot<TRequest> currentSnapshot,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeAttemptIdKind>[] addedAttemptIds,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeAttemptIdKind>[] retainedAttemptIds,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeAttemptIdKind>[] removedAttemptIds)
    {
        ReconciliationId = reconciliationId;
        PreviousSnapshot = previousSnapshot;
        CurrentSnapshot = currentSnapshot;
        this.addedAttemptIds = Array.AsReadOnly(addedAttemptIds);
        this.retainedAttemptIds = Array.AsReadOnly(retainedAttemptIds);
        this.removedAttemptIds = Array.AsReadOnly(removedAttemptIds);
    }

    /// <summary>Gets the externally assigned reconciliation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeActiveWorkReconciliationIdKind> ReconciliationId { get; }

    /// <summary>Gets the unchanged previous snapshot authority.</summary>
    public HostRuntimeActiveWorkSnapshot<TRequest> PreviousSnapshot { get; }

    /// <summary>Gets the unchanged current snapshot authority.</summary>
    public HostRuntimeActiveWorkSnapshot<TRequest> CurrentSnapshot { get; }

    /// <summary>Gets attempt IDs newly present in the current snapshot.</summary>
    public IReadOnlyList<global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind>> AddedAttemptIds => addedAttemptIds;

    /// <summary>Gets attempt IDs retained across both snapshots.</summary>
    public IReadOnlyList<global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind>> RetainedAttemptIds => retainedAttemptIds;

    /// <summary>
    /// Gets attempt IDs absent from the current snapshot without inferring a
    /// completion or disposition reason.
    /// </summary>
    public IReadOnlyList<global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind>> RemovedAttemptIds => removedAttemptIds;
}
