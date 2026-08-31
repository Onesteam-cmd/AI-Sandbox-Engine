namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Creates and advances immutable host lifecycle and health authority records.
/// </summary>
public static class HostRuntimeLifecycle
{
    /// <summary>Creates one initialized immutable lifecycle snapshot.</summary>
    /// <param name="instanceId">Externally assigned non-empty runtime ID.</param>
    /// <param name="compositionId">Validated non-empty composition ID.</param>
    /// <returns>A created snapshot at revision zero with unknown health.</returns>
    public static HostRuntimeLifecycleSnapshot Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeInstanceIdKind> instanceId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeCompositionIdKind> compositionId)
    {
        if (instanceId.IsEmpty)
        {
            throw new ArgumentException(
                "The runtime-instance ID must be initialized.",
                nameof(instanceId));
        }
        if (compositionId.IsEmpty)
        {
            throw new ArgumentException(
                "The composition ID must be initialized.",
                nameof(compositionId));
        }

        return new HostRuntimeLifecycleSnapshot(
            instanceId,
            compositionId,
            HostRuntimeLifecycleState.Created,
            revision: 0,
            HostRuntimeHealthStatus.Unknown,
            default,
            healthDetail: null);
    }

    /// <summary>Attempts one direct optimistic lifecycle transition.</summary>
    /// <param name="snapshot">Current immutable lifecycle authority.</param>
    /// <param name="expectedRevision">Revision the caller observed.</param>
    /// <param name="targetState">Requested direct target lifecycle state.</param>
    /// <returns>An explicit transition result without executing lifecycle work.</returns>
    public static HostRuntimeLifecycleTransitionResult Transition(
        HostRuntimeLifecycleSnapshot snapshot,
        long expectedRevision,
        HostRuntimeLifecycleState targetState)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        if (expectedRevision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(expectedRevision));
        }
        if (!Enum.IsDefined(targetState))
        {
            throw new ArgumentOutOfRangeException(nameof(targetState));
        }
        if (snapshot.Revision != expectedRevision)
        {
            return new HostRuntimeLifecycleTransitionResult(
                HostRuntimeLifecycleTransitionStatus.StaleRevision,
                snapshot);
        }
        if (!IsAllowedTransition(snapshot.State, targetState))
        {
            return new HostRuntimeLifecycleTransitionResult(
                HostRuntimeLifecycleTransitionStatus.InvalidTransition,
                snapshot);
        }

        var healthStatus = snapshot.HealthStatus;
        var healthProbeId = snapshot.HealthProbeId;
        var healthDetail = snapshot.HealthDetail;
        if (targetState == HostRuntimeLifecycleState.Stopped)
        {
            healthStatus = HostRuntimeHealthStatus.Unknown;
            healthProbeId = default;
            healthDetail = null;
        }

        var next = new HostRuntimeLifecycleSnapshot(
            snapshot.InstanceId,
            snapshot.CompositionId,
            targetState,
            checked(snapshot.Revision + 1),
            healthStatus,
            healthProbeId,
            healthDetail);
        return new HostRuntimeLifecycleTransitionResult(
            HostRuntimeLifecycleTransitionStatus.Applied,
            next);
    }

    /// <summary>Attempts one optimistic exact health observation.</summary>
    /// <param name="snapshot">Current immutable lifecycle authority.</param>
    /// <param name="expectedRevision">Revision the caller observed.</param>
    /// <param name="probeId">Externally assigned non-empty probe ID.</param>
    /// <param name="healthStatus">Explicit non-unknown health classification.</param>
    /// <param name="detail">Exact immutable health-detail payload.</param>
    /// <returns>An explicit update result without running a health probe.</returns>
    public static HostRuntimeHealthUpdateResult ObserveHealth(
        HostRuntimeLifecycleSnapshot snapshot,
        long expectedRevision,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeHealthProbeIdKind> probeId,
        HostRuntimeHealthStatus healthStatus,
        IHostRuntimeHealthDetail detail)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        if (expectedRevision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(expectedRevision));
        }
        if (probeId.IsEmpty)
        {
            throw new ArgumentException(
                "The health-probe ID must be initialized.",
                nameof(probeId));
        }
        if (healthStatus == HostRuntimeHealthStatus.Unknown ||
            !Enum.IsDefined(healthStatus))
        {
            throw new ArgumentOutOfRangeException(nameof(healthStatus));
        }
        HostRuntimeTypePolicy.EnsureExactHealthDetail(detail);

        if (snapshot.Revision != expectedRevision)
        {
            return new HostRuntimeHealthUpdateResult(
                HostRuntimeHealthUpdateStatus.StaleRevision,
                snapshot);
        }
        if (!AcceptsHealth(snapshot.State))
        {
            return new HostRuntimeHealthUpdateResult(
                HostRuntimeHealthUpdateStatus.InvalidLifecycleState,
                snapshot);
        }

        var next = new HostRuntimeLifecycleSnapshot(
            snapshot.InstanceId,
            snapshot.CompositionId,
            snapshot.State,
            checked(snapshot.Revision + 1),
            healthStatus,
            probeId,
            detail);
        return new HostRuntimeHealthUpdateResult(
            HostRuntimeHealthUpdateStatus.Applied,
            next);
    }

    private static bool IsAllowedTransition(
        HostRuntimeLifecycleState currentState,
        HostRuntimeLifecycleState targetState) =>
        (currentState, targetState) switch
        {
            (HostRuntimeLifecycleState.Created,
                HostRuntimeLifecycleState.Starting) => true,
            (HostRuntimeLifecycleState.Starting,
                HostRuntimeLifecycleState.Running) => true,
            (HostRuntimeLifecycleState.Starting,
                HostRuntimeLifecycleState.Faulted) => true,
            (HostRuntimeLifecycleState.Running,
                HostRuntimeLifecycleState.Stopping) => true,
            (HostRuntimeLifecycleState.Running,
                HostRuntimeLifecycleState.Faulted) => true,
            (HostRuntimeLifecycleState.Stopping,
                HostRuntimeLifecycleState.Stopped) => true,
            (HostRuntimeLifecycleState.Stopping,
                HostRuntimeLifecycleState.Faulted) => true,
            (HostRuntimeLifecycleState.Faulted,
                HostRuntimeLifecycleState.Stopping) => true,
            _ => false,
        };

    private static bool AcceptsHealth(HostRuntimeLifecycleState state) =>
        state is
            HostRuntimeLifecycleState.Starting or
            HostRuntimeLifecycleState.Running or
            HostRuntimeLifecycleState.Stopping or
            HostRuntimeLifecycleState.Faulted;
}
