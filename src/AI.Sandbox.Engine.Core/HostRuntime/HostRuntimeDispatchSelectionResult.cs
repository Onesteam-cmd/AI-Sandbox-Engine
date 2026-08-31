namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host dequeue-and-selection result.
/// </summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeDispatchSelectionResult<TRequest>
    where TRequest : IHostRuntimeRequest
{
    internal HostRuntimeDispatchSelectionResult(
        HostRuntimeDispatchSelectionStatus status,
        HostRuntimeQueueSnapshot snapshot,
        HostRuntimeDispatchSelection<TRequest>? selection)
    {
        Status = status;
        Snapshot = snapshot;
        Selection = selection;
    }

    /// <summary>Gets the explicit selection outcome.</summary>
    public HostRuntimeDispatchSelectionStatus Status { get; }

    /// <summary>
    /// Gets resulting or unchanged immutable queue authority.
    /// </summary>
    public HostRuntimeQueueSnapshot Snapshot { get; }

    /// <summary>
    /// Gets dequeue and dispatch authority when selection succeeded.
    /// </summary>
    public HostRuntimeDispatchSelection<TRequest>? Selection { get; }

    /// <summary>
    /// Gets whether queue and dispatch authority changed.
    /// </summary>
    public bool Succeeded =>
        Status == HostRuntimeDispatchSelectionStatus.Selected;
}
