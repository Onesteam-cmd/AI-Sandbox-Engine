namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable deterministic bounded Host recovery resumption plan.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
public sealed record HostRuntimeRecoveryResumptionPlan<TRequest, TState>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
{
    private readonly global::System.Collections.ObjectModel.ReadOnlyCollection<
        HostRuntimeActiveWorkItem<TRequest>> candidates;
    private readonly global::System.Collections.ObjectModel.ReadOnlyCollection<
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeAttemptIdKind>> suppressedAttemptIds;

    internal HostRuntimeRecoveryResumptionPlan(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryResumptionPlanIdKind> planId,
        HostRuntimeRecoveryContinuation<TRequest, TState> continuation,
        HostRuntimeActiveWorkItem<TRequest>[] candidates,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeAttemptIdKind>[] suppressedAttemptIds,
        long plannedTick,
        long revision)
    {
        PlanId = planId;
        Continuation = continuation;
        this.candidates = Array.AsReadOnly(candidates);
        this.suppressedAttemptIds = Array.AsReadOnly(suppressedAttemptIds);
        PlannedTick = plannedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned resumption plan ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryResumptionPlanIdKind> PlanId { get; }

    /// <summary>Gets unchanged validated recovery continuation authority.</summary>
    public HostRuntimeRecoveryContinuation<TRequest, TState> Continuation
    {
        get;
    }

    /// <summary>
    /// Gets pending checkpoint work ordered by stable attempt ID.
    /// </summary>
    public IReadOnlyList<HostRuntimeActiveWorkItem<TRequest>> Candidates =>
        candidates;

    /// <summary>
    /// Gets cancellation-requested attempt IDs excluded from resumption.
    /// </summary>
    public IReadOnlyList<global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind>> SuppressedAttemptIds =>
        suppressedAttemptIds;

    /// <summary>Gets the number of resumable candidates.</summary>
    public int CandidateCount => candidates.Count;

    /// <summary>Gets the number of explicitly suppressed attempts.</summary>
    public int SuppressedCount => suppressedAttemptIds.Count;

    /// <summary>Gets the external monotonic planning tick.</summary>
    public long PlannedTick { get; }

    /// <summary>Gets the optimistic resumption plan revision.</summary>
    public long Revision { get; }

    /// <summary>Gets the represented logical runtime instance ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeInstanceIdKind> RuntimeInstanceId =>
        Continuation.Checkpoint.RuntimeInstanceId;

    /// <summary>Gets the represented monotonic Host clock domain.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId =>
        Continuation.Checkpoint.ClockId;
}
