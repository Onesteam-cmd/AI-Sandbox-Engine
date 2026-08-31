namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority acknowledging one reconstructed recovery dispatch
/// as a new resumed in-flight attempt.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
public sealed record HostRuntimeRecoveryResumedAttemptAcknowledgement<TRequest, TState>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
{
    internal HostRuntimeRecoveryResumedAttemptAcknowledgement(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryResumedAttemptAcknowledgementIdKind>
                acknowledgementId,
        HostRuntimeRecoveryDispatchReconstruction<TRequest, TState>
            reconstruction,
        HostRuntimeInFlightAttempt<TRequest> attempt,
        long acknowledgedTick,
        long revision)
    {
        AcknowledgementId = acknowledgementId;
        Reconstruction = reconstruction;
        Attempt = attempt;
        AcknowledgedTick = acknowledgedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned acknowledgement ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryResumedAttemptAcknowledgementIdKind>
            AcknowledgementId { get; }

    /// <summary>Gets unchanged recovery dispatch reconstruction authority.</summary>
    public HostRuntimeRecoveryDispatchReconstruction<TRequest, TState>
        Reconstruction { get; }

    /// <summary>Gets the new acknowledged resumed in-flight attempt.</summary>
    public HostRuntimeInFlightAttempt<TRequest> Attempt { get; }

    /// <summary>Gets the external monotonic acknowledgement tick.</summary>
    public long AcknowledgedTick { get; }

    /// <summary>Gets the acknowledgement authority revision.</summary>
    public long Revision { get; }

    /// <summary>Gets unchanged lease-reacquisition authority.</summary>
    public HostRuntimeRecoveryLeaseReacquisition<TRequest, TState>
        Reacquisition => Reconstruction.Reacquisition;

    /// <summary>Gets the prior checkpoint attempt retained as evidence.</summary>
    public HostRuntimeInFlightAttempt<TRequest> PriorAttempt =>
        Reconstruction.PriorAttempt;

    /// <summary>Gets the prior checkpoint attempt ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind> PriorAttemptId =>
        Reconstruction.PriorAttemptId;

    /// <summary>Gets the new resumed attempt ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind> AttemptId => Attempt.AttemptId;

    /// <summary>Gets the new recovery dispatch ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeDispatchIdKind> DispatchId => Attempt.DispatchId;

    /// <summary>Gets the stable request ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRequestIdKind> RequestId => Attempt.RequestId;

    /// <summary>Gets the new recovery lease ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeLeaseIdKind> LeaseId => Attempt.LeaseId;

    /// <summary>Gets the recovery worker ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeWorkerIdKind> WorkerId => Attempt.WorkerId;

    /// <summary>Gets the resumed attempt number.</summary>
    public int AttemptNumber => Attempt.AttemptNumber;
}
