namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable advisory authority selecting one planned attempt for
/// external recovery resumption.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
public sealed record HostRuntimeResumedWorkSelection<TRequest, TState>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
{
    internal HostRuntimeResumedWorkSelection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeResumedWorkSelectionIdKind> selectionId,
        HostRuntimeRecoveryResumptionPlan<TRequest, TState> plan,
        HostRuntimeActiveWorkItem<TRequest> candidate,
        long selectedTick,
        long revision)
    {
        SelectionId = selectionId;
        Plan = plan;
        Candidate = candidate;
        SelectedTick = selectedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned selection ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeResumedWorkSelectionIdKind> SelectionId { get; }

    /// <summary>Gets unchanged resumption plan authority.</summary>
    public HostRuntimeRecoveryResumptionPlan<TRequest, TState> Plan { get; }

    /// <summary>Gets the exact selected checkpoint work authority.</summary>
    public HostRuntimeActiveWorkItem<TRequest> Candidate { get; }

    /// <summary>Gets the external monotonic selection tick.</summary>
    public long SelectedTick { get; }

    /// <summary>Gets the selection authority revision.</summary>
    public long Revision { get; }

    /// <summary>Gets the selected stable attempt ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind> AttemptId => Candidate.AttemptId;

    /// <summary>Gets the selected stable request ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRequestIdKind> RequestId => Candidate.RequestId;

    /// <summary>Gets the selected prior lease ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeLeaseIdKind> LeaseId => Candidate.LeaseId;

    /// <summary>Gets the selected prior worker ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeWorkerIdKind> WorkerId => Candidate.WorkerId;

    /// <summary>Gets the selected prior dispatch ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeDispatchIdKind> DispatchId => Candidate.DispatchId;
}
