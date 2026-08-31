namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority proving exact summary, checkpoint, and
/// supersession continuity between one summarized range and one projected
/// adjacent collection sequence.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidation<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidation(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidationIdKind>
                validationId,
        HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentCollectionSequenceProjection<
            TRequest,
            TState,
            TCompletion> adjacentCollectionSequence,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion> connectingSupersession,
        long validatedTick,
        long revision)
    {
        ValidationId = validationId;
        Summary = summary;
        AdjacentCollectionSequence = adjacentCollectionSequence;
        ConnectingSupersession = connectingSupersession;
        ValidatedTick = validatedTick;
        Revision = revision;
    }

    /// <summary>Gets externally assigned continuity-validation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidationIdKind>
            ValidationId { get; }

    /// <summary>Gets unchanged multi-collection range-summary authority.</summary>
    public HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets unchanged projected adjacent collection-sequence authority.</summary>
    public HostRuntimeRecoveryAdjacentCollectionSequenceProjection<
        TRequest,
        TState,
        TCompletion> AdjacentCollectionSequence { get; }

    /// <summary>Gets unchanged adjacent collection-sequence selection authority.</summary>
    public HostRuntimeRecoveryAdjacentCollectionSequenceSelection<
        TRequest,
        TState,
        TCompletion> Selection => AdjacentCollectionSequence.Selection;

    /// <summary>Gets whether validation addresses the previous collection sequence.</summary>
    public bool ValidatesPreviousCollectionSequence =>
        AdjacentCollectionSequence.SelectsPreviousCollectionSequence;

    /// <summary>Gets whether validation addresses the next collection sequence.</summary>
    public bool ValidatesNextCollectionSequence =>
        AdjacentCollectionSequence.SelectsNextCollectionSequence;

    /// <summary>Gets exact supersession connecting both authorities.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion> ConnectingSupersession { get; }

    /// <summary>Gets exact checkpoint before the shared boundary.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> PriorCheckpoint =>
        ConnectingSupersession.PriorCheckpoint;

    /// <summary>Gets exact checkpoint after the shared boundary.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> SuccessorCheckpoint =>
        ConnectingSupersession.SuccessorCheckpoint;

    /// <summary>Gets external monotonic continuity-validation tick.</summary>
    public long ValidatedTick { get; }

    /// <summary>Gets continuity-validation authority revision.</summary>
    public long Revision { get; }
}
