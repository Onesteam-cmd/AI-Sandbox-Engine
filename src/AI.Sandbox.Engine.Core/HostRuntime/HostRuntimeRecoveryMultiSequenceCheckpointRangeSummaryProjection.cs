namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains a compact immutable projection of one exact bounded multi-sequence
/// checkpoint-range query.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjectionIdKind> summaryId,
        HostRuntimeRecoveryMultiSequenceCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion> range,
        long projectedTick,
        long revision)
    {
        SummaryId = summaryId;
        Range = range;
        ProjectedTick = projectedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned multi-sequence summary ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjectionIdKind> SummaryId { get; }

    /// <summary>Gets unchanged source multi-sequence range authority.</summary>
    public HostRuntimeRecoveryMultiSequenceCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion> Range { get; }

    /// <summary>Gets unchanged source collection-validation authority.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
        TRequest,
        TState,
        TCompletion> Collection => Range.Collection;

    /// <summary>Gets unchanged source chain-summary projection authority.</summary>
    public HostRuntimeRecoveryChainSummaryProjection<
        TRequest,
        TState,
        TCompletion> SourceProjection => Range.SourceProjection;

    /// <summary>Gets unchanged validated supersession-chain authority.</summary>
    public HostRuntimeRecoverySupersessionChain<
        TRequest,
        TState,
        TCompletion> Chain => Range.Chain;

    /// <summary>Gets exact first checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint => Range.StartCheckpoint;

    /// <summary>Gets exact last checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint => Range.EndCheckpoint;

    /// <summary>Gets exact supersession entering the summarized range.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? IncomingSupersession => Range.IncomingSupersession;

    /// <summary>Gets exact supersession leaving the summarized range.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? OutgoingSupersession => Range.OutgoingSupersession;

    /// <summary>Gets exact crossed sequence-boundary authorities.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> CrossedBoundarySupersessions =>
        Range.CrossedBoundarySupersessions;

    /// <summary>Gets first checkpoint index in the source chain.</summary>
    public int StartCheckpointIndex => Range.StartCheckpointIndex;

    /// <summary>Gets last checkpoint index in the source chain.</summary>
    public int EndCheckpointIndex => Range.EndCheckpointIndex;

    /// <summary>Gets first intersected summary index in the source collection.</summary>
    public int StartSummaryIndex => Range.StartSummaryIndex;

    /// <summary>Gets last intersected summary index in the source collection.</summary>
    public int EndSummaryIndex => Range.EndSummaryIndex;

    /// <summary>Gets represented checkpoint count.</summary>
    public int CheckpointCount => Range.CheckpointCount;

    /// <summary>Gets represented supersession count.</summary>
    public int SupersessionCount => Range.SupersessionCount;

    /// <summary>Gets intersected summary count.</summary>
    public int SummaryCount => Range.SummaryCount;

    /// <summary>Gets intersected sequence count.</summary>
    public int SequenceCount => Range.SequenceCount;

    /// <summary>Gets crossed validated-boundary count.</summary>
    public int CrossedBoundaryCount => Range.CrossedBoundaryCount;

    /// <summary>Gets whether the range starts at the source collection start.</summary>
    public bool StartsAtCollectionStart => Range.StartsAtCollectionStart;

    /// <summary>Gets whether the range ends at the source collection end.</summary>
    public bool EndsAtCollectionEnd => Range.EndsAtCollectionEnd;

    /// <summary>Gets whether the range covers the entire source collection.</summary>
    public bool CoversEntireCollection => StartsAtCollectionStart && EndsAtCollectionEnd;

    /// <summary>Gets external monotonic summary-projection tick.</summary>
    public long ProjectedTick { get; }

    /// <summary>Gets multi-sequence summary authority revision.</summary>
    public long Revision { get; }
}
