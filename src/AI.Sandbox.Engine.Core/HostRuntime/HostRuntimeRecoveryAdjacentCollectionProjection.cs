namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable materialized adjacent recovery collection projected
/// from an exact adjacent-collection selection.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryAdjacentCollectionProjection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentCollectionProjection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryAdjacentCollectionProjectionIdKind> projectionId,
        HostRuntimeRecoveryAdjacentCollectionSelection<
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

    /// <summary>Gets externally assigned adjacent-collection projection ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryAdjacentCollectionProjectionIdKind> ProjectionId { get; }

    /// <summary>Gets unchanged adjacent-collection selection authority.</summary>
    public HostRuntimeRecoveryAdjacentCollectionSelection<
        TRequest,
        TState,
        TCompletion> Selection { get; }

    /// <summary>Gets unchanged source range-summary authority.</summary>
    public HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary => Selection.Summary;

    /// <summary>Gets unchanged source collection-validation authority.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
        TRequest,
        TState,
        TCompletion> SourceCollection => Selection.SourceCollection;

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

    /// <summary>Gets whether the collection precedes the summarized range.</summary>
    public bool SelectsPreviousCollection => Selection.SelectsPreviousCollection;

    /// <summary>Gets whether the collection follows the summarized range.</summary>
    public bool SelectsNextCollection => Selection.SelectsNextCollection;

    /// <summary>Gets exact selected multi-sequence summaries in source order.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion>> MultiSequenceSummaries =>
        Selection.MultiSequenceSummaries;

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

    /// <summary>Gets first selected summary index in source collection.</summary>
    public int StartSummaryIndex => Selection.StartSummaryIndex;

    /// <summary>Gets last selected summary index in source collection.</summary>
    public int EndSummaryIndex => Selection.EndSummaryIndex;

    /// <summary>Gets selected summary count.</summary>
    public int SummaryCount => MultiSequenceSummaries.Count;

    /// <summary>Gets selected sequence count.</summary>
    public int SequenceCount => Selection.SequenceCount;

    /// <summary>Gets first selected pair index in source projection.</summary>
    public int StartPairIndex => Selection.StartPairIndex;

    /// <summary>Gets last selected pair index in source projection.</summary>
    public int EndPairIndex => Selection.EndPairIndex;

    /// <summary>Gets selected pair count.</summary>
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

    /// <summary>Gets exact supersession entering the selected collection.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? IncomingSupersession => Selection.IncomingSupersession;

    /// <summary>Gets exact supersession leaving the selected collection.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? OutgoingSupersession => Selection.OutgoingSupersession;

    /// <summary>Gets whether selection starts at source collection start.</summary>
    public bool StartsAtSourceCollectionStart =>
        Selection.StartsAtSourceCollectionStart;

    /// <summary>Gets whether selection ends at source collection end.</summary>
    public bool EndsAtSourceCollectionEnd =>
        Selection.EndsAtSourceCollectionEnd;

    /// <summary>Gets external monotonic projection tick.</summary>
    public long ProjectedTick { get; }

    /// <summary>Gets adjacent-collection projection revision.</summary>
    public long Revision { get; }
}
