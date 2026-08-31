namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable deterministic bounded Host active-work snapshot.
/// </summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeActiveWorkSnapshot<TRequest>
    where TRequest : IHostRuntimeRequest
{
    private readonly global::System.Collections.ObjectModel.ReadOnlyCollection<
        HostRuntimeActiveWorkItem<TRequest>> items;

    internal HostRuntimeActiveWorkSnapshot(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeActiveWorkSnapshotIdKind> snapshotId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeInstanceIdKind> runtimeInstanceId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeClockIdKind> clockId,
        long observedTick,
        long revision,
        HostRuntimeActiveWorkItem<TRequest>[] items)
    {
        SnapshotId = snapshotId;
        RuntimeInstanceId = runtimeInstanceId;
        ClockId = clockId;
        ObservedTick = observedTick;
        Revision = revision;
        this.items = Array.AsReadOnly(items);
    }

    /// <summary>Gets the externally assigned snapshot ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeActiveWorkSnapshotIdKind> SnapshotId { get; }

    /// <summary>Gets the represented runtime instance ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeInstanceIdKind> RuntimeInstanceId { get; }

    /// <summary>Gets the external monotonic clock domain.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId { get; }

    /// <summary>Gets the external monotonic observation tick.</summary>
    public long ObservedTick { get; }

    /// <summary>Gets the optimistic active-work snapshot revision.</summary>
    public long Revision { get; }

    /// <summary>Gets active items ordered by stable attempt ID.</summary>
    public IReadOnlyList<HostRuntimeActiveWorkItem<TRequest>> Items => items;

    /// <summary>Gets the represented active-work count.</summary>
    public int Count => items.Count;
}
