namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable host-runtime lifecycle and health authority snapshot.
/// </summary>
public sealed record HostRuntimeLifecycleSnapshot
{
    internal HostRuntimeLifecycleSnapshot(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeInstanceIdKind> instanceId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeCompositionIdKind> compositionId,
        HostRuntimeLifecycleState state,
        long revision,
        HostRuntimeHealthStatus healthStatus,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeHealthProbeIdKind> healthProbeId,
        IHostRuntimeHealthDetail? healthDetail)
    {
        InstanceId = instanceId;
        CompositionId = compositionId;
        State = state;
        Revision = revision;
        HealthStatus = healthStatus;
        HealthProbeId = healthProbeId;
        HealthDetail = healthDetail;
    }

    /// <summary>Gets the externally assigned runtime-instance ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeInstanceIdKind> InstanceId { get; }

    /// <summary>Gets the validated composition represented by this runtime.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeCompositionIdKind> CompositionId { get; }

    /// <summary>Gets the current explicit lifecycle state.</summary>
    public HostRuntimeLifecycleState State { get; }

    /// <summary>Gets the optimistic lifecycle authority revision.</summary>
    public long Revision { get; }

    /// <summary>Gets the latest explicit provider-neutral health status.</summary>
    public HostRuntimeHealthStatus HealthStatus { get; }

    /// <summary>Gets the probe that produced the latest health observation.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeHealthProbeIdKind> HealthProbeId { get; }

    /// <summary>Gets the exact latest health detail, when one exists.</summary>
    public IHostRuntimeHealthDetail? HealthDetail { get; }
}
