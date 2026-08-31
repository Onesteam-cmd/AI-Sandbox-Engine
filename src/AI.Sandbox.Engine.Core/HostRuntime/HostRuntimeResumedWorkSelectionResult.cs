namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable resumed-work selection result.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
public sealed record HostRuntimeResumedWorkSelectionResult<TRequest, TState>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
{
    internal HostRuntimeResumedWorkSelectionResult(
        HostRuntimeRecoveryResumptionStatus status,
        HostRuntimeRecoveryResumptionPlan<TRequest, TState> plan,
        HostRuntimeResumedWorkSelection<TRequest, TState>? selection,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeAttemptIdKind> relatedAttemptId)
    {
        Status = status;
        Plan = plan;
        Selection = selection;
        RelatedAttemptId = relatedAttemptId;
    }

    /// <summary>Gets the explicit selection outcome.</summary>
    public HostRuntimeRecoveryResumptionStatus Status { get; }

    /// <summary>Gets unchanged resumption plan authority.</summary>
    public HostRuntimeRecoveryResumptionPlan<TRequest, TState> Plan { get; }

    /// <summary>Gets resumed-work selection authority when successful.</summary>
    public HostRuntimeResumedWorkSelection<TRequest, TState>? Selection
    {
        get;
    }

    /// <summary>Gets the related attempt ID for attempt-specific outcomes.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind> RelatedAttemptId { get; }

    /// <summary>Gets whether selection authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryResumptionStatus.SelectionCreated;
}
