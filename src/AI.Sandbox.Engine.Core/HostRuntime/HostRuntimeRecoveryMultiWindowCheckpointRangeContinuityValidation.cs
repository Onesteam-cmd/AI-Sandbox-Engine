namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority proving exact pair, checkpoint, and supersession
/// continuity between one summarized range and one projected adjacent sequence.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidationIdKind>
                validationId,
        HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentSequenceProjection<
            TRequest,
            TState,
            TCompletion> adjacentSequence,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion> connectingSupersession,
        long validatedTick,
        long revision)
    {
        ValidationId = validationId;
        Summary = summary;
        AdjacentSequence = adjacentSequence;
        ConnectingSupersession = connectingSupersession;
        ValidatedTick = validatedTick;
        Revision = revision;
    }

    /// <summary>Gets externally assigned continuity-validation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidationIdKind>
            ValidationId { get; }

    /// <summary>Gets unchanged multi-window checkpoint-range summary authority.</summary>
    public HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets unchanged projected adjacent-sequence authority.</summary>
    public HostRuntimeRecoveryAdjacentSequenceProjection<
        TRequest,
        TState,
        TCompletion> AdjacentSequence { get; }

    /// <summary>Gets unchanged adjacent-sequence selection authority.</summary>
    public HostRuntimeRecoveryAdjacentSequenceSelection<
        TRequest,
        TState,
        TCompletion> Selection => AdjacentSequence.Selection;

    /// <summary>Gets whether continuity was validated to a previous sequence.</summary>
    public bool ValidatesPreviousSequence => AdjacentSequence.SelectsPreviousSequence;

    /// <summary>Gets whether continuity was validated to a next sequence.</summary>
    public bool ValidatesNextSequence => AdjacentSequence.SelectsNextSequence;

    /// <summary>Gets exact supersession connecting both authorities.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion> ConnectingSupersession { get; }

    /// <summary>Gets exact checkpoint before the connecting supersession.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> PriorCheckpoint =>
        ConnectingSupersession.PriorCheckpoint;

    /// <summary>Gets exact checkpoint after the connecting supersession.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> SuccessorCheckpoint =>
        ConnectingSupersession.SuccessorCheckpoint;

    /// <summary>Gets external monotonic continuity-validation tick.</summary>
    public long ValidatedTick { get; }

    /// <summary>Gets multi-window checkpoint-range continuity authority revision.</summary>
    public long Revision { get; }
}
