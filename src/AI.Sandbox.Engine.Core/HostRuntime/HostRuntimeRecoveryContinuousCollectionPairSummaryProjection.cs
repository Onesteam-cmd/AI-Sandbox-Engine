namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable compact summary over one exact multi-sequence range and
/// one immediately adjacent projected collection joined by validated continuity.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousCollectionPairSummaryProjection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryContinuousCollectionPairSummaryProjectionIdKind> summaryId,
        HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidation<
            TRequest,
            TState,
            TCompletion> continuity,
        long projectedTick,
        long revision)
    {
        SummaryId = summaryId;
        Continuity = continuity;
        ProjectedTick = projectedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned continuous collection-pair summary ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryContinuousCollectionPairSummaryProjectionIdKind> SummaryId { get; }

    /// <summary>Gets unchanged multi-sequence checkpoint-range continuity authority.</summary>
    public HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion> Continuity { get; }

    /// <summary>Gets unchanged source checkpoint-range summary authority.</summary>
    public HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> RangeSummary => Continuity.Summary;

    /// <summary>Gets unchanged projected adjacent-collection authority.</summary>
    public HostRuntimeRecoveryAdjacentCollectionProjection<
        TRequest,
        TState,
        TCompletion> AdjacentCollection => Continuity.AdjacentCollection;

    /// <summary>Gets unchanged source collection-validation authority.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
        TRequest,
        TState,
        TCompletion> SourceCollection => RangeSummary.Collection;

    /// <summary>Gets unchanged source chain-summary projection authority.</summary>
    public HostRuntimeRecoveryChainSummaryProjection<
        TRequest,
        TState,
        TCompletion> SourceProjection => RangeSummary.SourceProjection;

    /// <summary>Gets unchanged validated supersession-chain authority.</summary>
    public HostRuntimeRecoverySupersessionChain<
        TRequest,
        TState,
        TCompletion> Chain => SourceProjection.Chain;

    /// <summary>Gets whether the adjacent collection precedes the summarized range.</summary>
    public bool ConnectsPreviousCollection => Continuity.ValidatesPreviousCollection;

    /// <summary>Gets whether the adjacent collection follows the summarized range.</summary>
    public bool ConnectsNextCollection => Continuity.ValidatesNextCollection;

    /// <summary>Gets the exact supersession connecting both collection parts.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion> ConnectingSupersession => Continuity.ConnectingSupersession;

    /// <summary>Gets the exact checkpoint before the shared boundary.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> ConnectingPriorCheckpoint =>
        ConnectingSupersession.PriorCheckpoint;

    /// <summary>Gets the exact checkpoint after the shared boundary.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> ConnectingSuccessorCheckpoint =>
        ConnectingSupersession.SuccessorCheckpoint;

    /// <summary>Gets the first represented summary index in the source collection.</summary>
    public int StartSummaryIndex =>
        ConnectsPreviousCollection
            ? AdjacentCollection.StartSummaryIndex
            : RangeSummary.StartSummaryIndex;

    /// <summary>Gets the last represented summary index in the source collection.</summary>
    public int EndSummaryIndex =>
        ConnectsPreviousCollection
            ? RangeSummary.EndSummaryIndex
            : AdjacentCollection.EndSummaryIndex;

    /// <summary>Gets the source-summary index before the connecting boundary.</summary>
    public int ConnectingPriorSummaryIndex =>
        ConnectsPreviousCollection
            ? AdjacentCollection.EndSummaryIndex
            : RangeSummary.EndSummaryIndex;

    /// <summary>Gets the source-summary index after the connecting boundary.</summary>
    public int ConnectingSuccessorSummaryIndex =>
        checked(ConnectingPriorSummaryIndex + 1);

    /// <summary>Gets the number of represented multi-sequence summaries.</summary>
    public int SummaryCount => checked(
        RangeSummary.SummaryCount + AdjacentCollection.SummaryCount);

    /// <summary>Gets the number of represented sequence authorities.</summary>
    public int SequenceCount => checked(
        RangeSummary.SequenceCount + AdjacentCollection.SequenceCount);

    /// <summary>Gets the number of represented pair summaries.</summary>
    public int PairCount => CountPairs();

    /// <summary>Gets the number of represented recovery windows.</summary>
    public int WindowCount => checked(PairCount * 2);

    /// <summary>Gets the first checkpoint index represented by the collection pair.</summary>
    public int StartCheckpointIndex =>
        ConnectsPreviousCollection
            ? AdjacentCollection.StartCheckpointIndex
            : RangeSummary.StartCheckpointIndex;

    /// <summary>Gets the last checkpoint index represented by the collection pair.</summary>
    public int EndCheckpointIndex =>
        ConnectsPreviousCollection
            ? RangeSummary.EndCheckpointIndex
            : AdjacentCollection.EndCheckpointIndex;

    /// <summary>Gets the source-chain index before the connecting edge.</summary>
    public int ConnectingPriorCheckpointIndex =>
        ConnectsPreviousCollection
            ? AdjacentCollection.EndCheckpointIndex
            : RangeSummary.EndCheckpointIndex;

    /// <summary>Gets the source-chain index after the connecting edge.</summary>
    public int ConnectingSuccessorCheckpointIndex =>
        checked(ConnectingPriorCheckpointIndex + 1);

    /// <summary>Gets the exact first checkpoint authority represented by the collection pair.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint =>
        ConnectsPreviousCollection
            ? AdjacentCollection.StartCheckpoint
            : RangeSummary.StartCheckpoint;

    /// <summary>Gets the exact last checkpoint authority represented by the collection pair.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint =>
        ConnectsPreviousCollection
            ? RangeSummary.EndCheckpoint
            : AdjacentCollection.EndCheckpoint;

    /// <summary>Gets the first represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> StartCheckpointId =>
        StartCheckpoint.CheckpointId;

    /// <summary>Gets the last represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> EndCheckpointId =>
        EndCheckpoint.CheckpointId;

    /// <summary>Gets the number of checkpoints compactly represented by the collection pair.</summary>
    public int CheckpointCount => checked(
        RangeSummary.CheckpointCount + AdjacentCollection.CheckpointCount);

    /// <summary>Gets the number of supersessions compactly represented by the collection pair.</summary>
    public int SupersessionCount => checked(
        RangeSummary.SupersessionCount + AdjacentCollection.SupersessionCount + 1);

    /// <summary>Gets whether the pair starts at the source collection boundary.</summary>
    public bool StartsAtSourceCollectionStart => StartSummaryIndex == 0;

    /// <summary>Gets whether the pair ends at the source collection boundary.</summary>
    public bool EndsAtSourceCollectionEnd =>
        EndSummaryIndex == checked(SourceCollection.SummaryCount - 1);

    /// <summary>Gets whether the collection pair starts at the chain root.</summary>
    public bool StartsAtRoot => StartCheckpointIndex == 0;

    /// <summary>Gets whether the collection pair ends at the latest checkpoint.</summary>
    public bool EndsAtLatest =>
        EndCheckpointIndex == SourceProjection.SupersessionCount;

    /// <summary>Gets the external monotonic collection-pair-summary projection tick.</summary>
    public long ProjectedTick { get; }

    /// <summary>Gets the continuous collection-pair summary authority revision.</summary>
    public long Revision { get; }

    private int CountPairs()
    {
        var pairCount = 0;
        for (var index = StartSummaryIndex;
             index <= EndSummaryIndex;
             index++)
        {
            pairCount = checked(
                pairCount + SourceCollection.MultiSequenceSummaries[index].PairCount);
        }

        return pairCount;
    }
}
