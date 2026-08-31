namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority proving exact summary, checkpoint, and
/// supersession continuity between one summarized range and one projected
/// adjacent multi-collection-sequence.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidation<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidation(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidationIdKind>
                validationId,
        HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjection<
            TRequest,
            TState,
            TCompletion> adjacentMultiCollectionSequence,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion> connectingSupersession,
        long validatedTick,
        long revision)
    {
        ValidationId = validationId;
        Summary = summary;
        AdjacentMultiCollectionSequence = adjacentMultiCollectionSequence;
        ConnectingSupersession = connectingSupersession;
        ValidatedTick = validatedTick;
        Revision = revision;
    }

    /// <summary>Gets externally assigned continuity-validation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidationIdKind>
            ValidationId { get; }

    /// <summary>Gets unchanged multi-collection-sequence-sequence range-summary authority.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets unchanged projected adjacent multi-collection-sequence authority.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjection<
        TRequest,
        TState,
        TCompletion> AdjacentMultiCollectionSequence { get; }

    /// <summary>Gets unchanged adjacent multi-collection-sequence selection authority.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection<
        TRequest,
        TState,
        TCompletion> Selection => AdjacentMultiCollectionSequence.Selection;

    /// <summary>Gets whether validation addresses the previous collection sequence.</summary>
    public bool ValidatesPreviousMultiCollectionSequence =>
        AdjacentMultiCollectionSequence.SelectsPreviousMultiCollectionSequence;

    /// <summary>Gets whether validation addresses the next collection sequence.</summary>
    public bool ValidatesNextMultiCollectionSequence =>
        AdjacentMultiCollectionSequence.SelectsNextMultiCollectionSequence;

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
