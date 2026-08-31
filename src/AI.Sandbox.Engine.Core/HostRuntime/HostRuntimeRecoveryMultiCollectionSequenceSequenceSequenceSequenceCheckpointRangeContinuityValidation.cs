namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority proving exact summary, checkpoint, and
/// supersession continuity between one summarized range and one projected
/// adjacent multi-collection-sequence-sequence-sequence.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidation<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidation(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationIdKind>
                validationId,
        HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjection<
            TRequest,
            TState,
            TCompletion> adjacentMultiCollectionSequenceSequenceSequence,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion> connectingSupersession,
        long validatedTick,
        long revision)
    {
        ValidationId = validationId;
        Summary = summary;
        AdjacentMultiCollectionSequenceSequenceSequence = adjacentMultiCollectionSequenceSequenceSequence;
        ConnectingSupersession = connectingSupersession;
        ValidatedTick = validatedTick;
        Revision = revision;
    }

    /// <summary>Gets externally assigned continuity-validation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationIdKind>
            ValidationId { get; }

    /// <summary>Gets unchanged multi-collection-sequence-sequence-sequence-sequence range-summary authority.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets unchanged projected adjacent multi-collection-sequence-sequence-sequence authority.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjection<
        TRequest,
        TState,
        TCompletion> AdjacentMultiCollectionSequenceSequenceSequence { get; }

    /// <summary>Gets unchanged adjacent multi-collection-sequence-sequence-sequence selection authority.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelection<
        TRequest,
        TState,
        TCompletion> Selection => AdjacentMultiCollectionSequenceSequenceSequence.Selection;

    /// <summary>Gets whether validation addresses the previous collection sequence.</summary>
    public bool ValidatesPreviousMultiCollectionSequenceSequenceSequence =>
        AdjacentMultiCollectionSequenceSequenceSequence.SelectsPreviousMultiCollectionSequenceSequenceSequence;

    /// <summary>Gets whether validation addresses the next collection sequence.</summary>
    public bool ValidatesNextMultiCollectionSequenceSequenceSequence =>
        AdjacentMultiCollectionSequenceSequenceSequence.SelectsNextMultiCollectionSequenceSequenceSequence;

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
