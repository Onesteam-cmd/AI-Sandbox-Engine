namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains a compact immutable projection of one exact bounded multi-collection
/// checkpoint-range query.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjectionIdKind> summaryId,
        HostRuntimeRecoveryMultiCollectionCheckpointRangeQuery<
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

    /// <summary>Gets the externally assigned multi-collection summary ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjectionIdKind> SummaryId { get; }

    /// <summary>Gets unchanged source multi-collection range authority.</summary>
    public HostRuntimeRecoveryMultiCollectionCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion> Range { get; }

    /// <summary>Gets unchanged source sequence-validation authority.</summary>
    public HostRuntimeRecoveryContinuousCollectionSequenceValidation<
        TRequest,
        TState,
        TCompletion> Sequence => Range.Sequence;

    /// <summary>Gets unchanged source collection-validation authority.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
        TRequest,
        TState,
        TCompletion> SourceCollection => Range.SourceCollection;

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

    /// <summary>Gets first intersected collection-pair index in the source sequence.</summary>
    public int StartCollectionPairIndex => Range.StartCollectionPairIndex;

    /// <summary>Gets last intersected collection-pair index in the source sequence.</summary>
    public int EndCollectionPairIndex => Range.EndCollectionPairIndex;

    /// <summary>Gets represented checkpoint count.</summary>
    public int CheckpointCount => Range.CheckpointCount;

    /// <summary>Gets represented supersession count.</summary>
    public int SupersessionCount => Range.SupersessionCount;

    /// <summary>Gets intersected collection-pair count.</summary>
    public int CollectionPairCount => Range.CollectionPairCount;

    /// <summary>Gets crossed validated-boundary count.</summary>
    public int CrossedBoundaryCount => Range.CrossedBoundaryCount;

    /// <summary>Gets represented crossed-collection count.</summary>
    public int CrossedCollectionCount => Range.CrossedCollectionCount;

    /// <summary>Gets whether the range starts at the source sequence start.</summary>
    public bool StartsAtSequenceStart => Range.StartsAtSequenceStart;

    /// <summary>Gets whether the range ends at the source sequence end.</summary>
    public bool EndsAtSequenceEnd => Range.EndsAtSequenceEnd;

    /// <summary>Gets whether the range covers the entire source sequence.</summary>
    public bool CoversEntireSequence => StartsAtSequenceStart && EndsAtSequenceEnd;

    /// <summary>Gets external monotonic summary-projection tick.</summary>
    public long ProjectedTick { get; }

    /// <summary>Gets multi-collection summary authority revision.</summary>
    public long Revision { get; }
}
