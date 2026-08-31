namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable materialized adjacent recovery multi-collection-sequence
/// projected from an exact adjacent multi-collection-sequence selection.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionIdKind> projectionId,
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection<
            TRequest,
            TState,
            TCompletion> selection,
        HostRuntimeRecoveryCheckpoint<TRequest>[] checkpoints,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>[] supersessions,
        long projectedTick,
        long revision)
    {
        ProjectionId = projectionId;
        Selection = selection;
        Checkpoints = global::System.Array.AsReadOnly(checkpoints);
        Supersessions = global::System.Array.AsReadOnly(supersessions);
        ProjectedTick = projectedTick;
        Revision = revision;
    }

    /// <summary>Gets externally assigned adjacent multi-collection-sequence projection ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionIdKind> ProjectionId { get; }

    /// <summary>Gets unchanged adjacent multi-collection-sequence selection authority.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection<
        TRequest,
        TState,
        TCompletion> Selection { get; }

    /// <summary>Gets unchanged source range-summary authority.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary => Selection.Summary;

    /// <summary>Gets unchanged source multi-collection-sequence-sequence validation authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceValidation<
        TRequest,
        TState,
        TCompletion> SourceSequence => Selection.SourceSequence;

    /// <summary>Gets unchanged source collection-validation authority.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
        TRequest,
        TState,
        TCompletion> SourceCollection => Summary.SourceCollection;

    /// <summary>Gets unchanged source chain-summary projection authority.</summary>
    public HostRuntimeRecoveryChainSummaryProjection<
        TRequest,
        TState,
        TCompletion> SourceProjection => Summary.SourceProjection;

    /// <summary>Gets unchanged validated supersession-chain authority.</summary>
    public HostRuntimeRecoverySupersessionChain<
        TRequest,
        TState,
        TCompletion> Chain => Summary.Chain;

    /// <summary>Gets whether the multi-collection-sequence precedes the summarized range.</summary>
    public bool SelectsPreviousMultiCollectionSequence => Selection.SelectsPreviousMultiCollectionSequence;

    /// <summary>Gets whether the multi-collection-sequence follows the summarized range.</summary>
    public bool SelectsNextMultiCollectionSequence => Selection.SelectsNextMultiCollectionSequence;

    /// <summary>Gets exact selected multi-collection-sequence summaries in source order.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion>> MultiCollectionSequenceSummaries =>
        Selection.MultiCollectionSequenceSummaries;

    /// <summary>Gets exact selected internal boundary authorities.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> BoundarySupersessions =>
        Selection.BoundarySupersessions;

    /// <summary>Gets exact materialized checkpoint authorities.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpoint<TRequest>> Checkpoints { get; }

    /// <summary>Gets exact materialized supersession authorities.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> Supersessions { get; }

    /// <summary>Gets exact supersession connecting selection and source range.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion> AdjacentBoundarySupersession =>
        Selection.AdjacentBoundarySupersession;

    /// <summary>Gets first selected sequence-summary index in source sequence.</summary>
    public int StartSequenceSummaryIndex => Selection.StartSequenceSummaryIndex;

    /// <summary>Gets last selected sequence-summary index in source sequence.</summary>
    public int EndSequenceSummaryIndex => Selection.EndSequenceSummaryIndex;

    /// <summary>Gets selected multi-collection-sequence count.</summary>
    public int MultiCollectionSequenceCount => MultiCollectionSequenceSummaries.Count;

    /// <summary>Gets first represented multi-collection-summary index.</summary>
    public int StartSummaryIndex => Selection.StartSummaryIndex;

    /// <summary>Gets last represented multi-collection-summary index.</summary>
    public int EndSummaryIndex => Selection.EndSummaryIndex;

    /// <summary>Gets represented multi-collection count.</summary>
    public int MultiCollectionCount => Selection.MultiCollectionCount;

    /// <summary>Gets represented collection-pair summary count.</summary>
    public int CollectionPairCount => Selection.CollectionPairCount;

    /// <summary>Gets represented collection count.</summary>
    public int CollectionCount => Selection.CollectionCount;

    /// <summary>Gets represented multi-sequence summary count.</summary>
    public int SummaryCount => Selection.SummaryCount;

    /// <summary>Gets represented sequence count.</summary>
    public int SequenceCount => Selection.SequenceCount;

    /// <summary>Gets represented pair-summary count.</summary>
    public int PairCount => Selection.PairCount;

    /// <summary>Gets selected window count.</summary>
    public int WindowCount => Selection.WindowCount;

    /// <summary>Gets first selected checkpoint index in source chain.</summary>
    public int StartCheckpointIndex => Selection.StartCheckpointIndex;

    /// <summary>Gets last selected checkpoint index in source chain.</summary>
    public int EndCheckpointIndex => Selection.EndCheckpointIndex;

    /// <summary>Gets represented checkpoint count.</summary>
    public int CheckpointCount => Checkpoints.Count;

    /// <summary>Gets represented supersession count.</summary>
    public int SupersessionCount => Supersessions.Count;

    /// <summary>Gets exact first selected checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint => Checkpoints[0];

    /// <summary>Gets exact last selected checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint => Checkpoints[^1];

    /// <summary>Gets exact supersession entering the selected multi-collection-sequence.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? IncomingSupersession => Selection.IncomingSupersession;

    /// <summary>Gets exact supersession leaving the selected multi-collection-sequence.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? OutgoingSupersession => Selection.OutgoingSupersession;

    /// <summary>Gets whether selection starts at source multi-collection-sequence-sequence start.</summary>
    public bool StartsAtSourceSequenceStart =>
        Selection.StartsAtSourceSequenceStart;

    /// <summary>Gets whether selection ends at source multi-collection-sequence-sequence end.</summary>
    public bool EndsAtSourceSequenceEnd =>
        Selection.EndsAtSourceSequenceEnd;

    /// <summary>Gets external monotonic projection tick.</summary>
    public long ProjectedTick { get; }

    /// <summary>Gets adjacent multi-collection-sequence projection revision.</summary>
    public long Revision { get; }
}
