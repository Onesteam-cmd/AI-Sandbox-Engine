namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable materialized adjacent recovery sequence projected from
/// an exact adjacent-sequence selection.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryAdjacentSequenceProjection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentSequenceProjection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryAdjacentSequenceProjectionIdKind> projectionId,
        HostRuntimeRecoveryAdjacentSequenceSelection<
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

    /// <summary>Gets the externally assigned adjacent-sequence projection ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryAdjacentSequenceProjectionIdKind> ProjectionId { get; }

    /// <summary>Gets unchanged adjacent-sequence selection authority.</summary>
    public HostRuntimeRecoveryAdjacentSequenceSelection<
        TRequest,
        TState,
        TCompletion> Selection { get; }

    /// <summary>Gets unchanged multi-window checkpoint-range summary authority.</summary>
    public HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary => Selection.Summary;

    /// <summary>Gets unchanged source continuous-window sequence authority.</summary>
    public HostRuntimeRecoveryContinuousWindowSequenceValidation<
        TRequest,
        TState,
        TCompletion> SourceSequence => Selection.SourceSequence;

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

    /// <summary>Gets whether this is the selected previous sequence.</summary>
    public bool SelectsPreviousSequence => Selection.SelectsPreviousSequence;

    /// <summary>Gets whether this is the selected next sequence.</summary>
    public bool SelectsNextSequence => Selection.SelectsNextSequence;

    /// <summary>Gets exact selected pair-summary authorities in source order.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
            TRequest,
            TState,
            TCompletion>> PairSummaries => Selection.PairSummaries;

    /// <summary>Gets exact internal selected-sequence boundary authorities.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> BoundarySupersessions => Selection.BoundarySupersessions;

    /// <summary>Gets exact projected checkpoint authorities.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpoint<TRequest>> Checkpoints { get; }

    /// <summary>Gets exact projected supersession authorities.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> Supersessions { get; }

    /// <summary>Gets exact supersession connecting selection and summarized range.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion> AdjacentBoundarySupersession =>
        Selection.AdjacentBoundarySupersession;

    /// <summary>Gets first selected pair index in the source sequence.</summary>
    public int StartPairIndex => Selection.StartPairIndex;

    /// <summary>Gets last selected pair index in the source sequence.</summary>
    public int EndPairIndex => Selection.EndPairIndex;

    /// <summary>Gets selected pair count.</summary>
    public int PairCount => PairSummaries.Count;

    /// <summary>Gets selected window count.</summary>
    public int WindowCount => Selection.WindowCount;

    /// <summary>Gets first projected checkpoint index in the source chain.</summary>
    public int StartCheckpointIndex => Selection.StartCheckpointIndex;

    /// <summary>Gets last projected checkpoint index in the source chain.</summary>
    public int EndCheckpointIndex => Selection.EndCheckpointIndex;

    /// <summary>Gets projected checkpoint count.</summary>
    public int CheckpointCount => Checkpoints.Count;

    /// <summary>Gets projected supersession count.</summary>
    public int SupersessionCount => Supersessions.Count;

    /// <summary>Gets exact first projected checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint => Checkpoints[0];

    /// <summary>Gets exact last projected checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint => Checkpoints[^1];

    /// <summary>Gets exact supersession entering the projected sequence.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? IncomingSupersession => Selection.IncomingSupersession;

    /// <summary>Gets exact supersession leaving the projected sequence.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? OutgoingSupersession => Selection.OutgoingSupersession;

    /// <summary>Gets whether the projection starts at source-sequence start.</summary>
    public bool StartsAtSourceSequenceStart => Selection.StartsAtSourceSequenceStart;

    /// <summary>Gets whether the projection ends at source-sequence end.</summary>
    public bool EndsAtSourceSequenceEnd => Selection.EndsAtSourceSequenceEnd;

    /// <summary>Gets external monotonic adjacent-sequence projection tick.</summary>
    public long ProjectedTick { get; }

    /// <summary>Gets adjacent-sequence projection authority revision.</summary>
    public long Revision { get; }
}
