namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable compact summary over one exact multi-window range and
/// one immediately adjacent projected sequence joined by validated continuity.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionIdKind> summaryId,
        HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidation<
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

    /// <summary>Gets the externally assigned continuous multi-collection summary ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionIdKind> SummaryId { get; }

    /// <summary>Gets unchanged checkpoint-range continuity authority.</summary>
    public HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion> Continuity { get; }

    /// <summary>Gets unchanged source checkpoint-range summary authority.</summary>
    public HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> RangeSummary => Continuity.Summary;

    /// <summary>Gets unchanged projected adjacent-sequence authority.</summary>
    public HostRuntimeRecoveryAdjacentCollectionSequenceProjection<
        TRequest,
        TState,
        TCompletion> AdjacentCollectionSequence => Continuity.AdjacentCollectionSequence;

    /// <summary>Gets unchanged source collection-sequence authority.</summary>
    public HostRuntimeRecoveryContinuousCollectionSequenceValidation<
        TRequest,
        TState,
        TCompletion> SourceSequence => RangeSummary.Sequence;

    /// <summary>Gets unchanged source collection-validation authority.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
        TRequest,
        TState,
        TCompletion> SourceCollection => RangeSummary.SourceCollection;

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

    /// <summary>Gets whether the adjacent sequence precedes the summarized range.</summary>
    public bool ConnectsPreviousCollectionSequence => Continuity.ValidatesPreviousCollectionSequence;

    /// <summary>Gets whether the adjacent sequence follows the summarized range.</summary>
    public bool ConnectsNextCollectionSequence => Continuity.ValidatesNextCollectionSequence;

    /// <summary>Gets the exact supersession connecting both collection sequences.</summary>
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

    /// <summary>Gets the first represented pair index in the source sequence.</summary>
    public int StartCollectionPairIndex =>
        ConnectsPreviousCollectionSequence
            ? AdjacentCollectionSequence.StartCollectionPairIndex
            : RangeSummary.StartCollectionPairIndex;

    /// <summary>Gets the last represented pair index in the source sequence.</summary>
    public int EndCollectionPairIndex =>
        ConnectsPreviousCollectionSequence
            ? RangeSummary.EndCollectionPairIndex
            : AdjacentCollectionSequence.EndCollectionPairIndex;

    /// <summary>Gets the source pair index before the connecting boundary.</summary>
    public int ConnectingPriorCollectionPairIndex =>
        ConnectsPreviousCollectionSequence
            ? AdjacentCollectionSequence.EndCollectionPairIndex
            : RangeSummary.EndCollectionPairIndex;

    /// <summary>Gets the source pair index after the connecting boundary.</summary>
    public int ConnectingSuccessorCollectionPairIndex =>
        checked(ConnectingPriorCollectionPairIndex + 1);

    /// <summary>Gets the first represented collection-pair authority.</summary>
    public HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
        TRequest,
        TState,
        TCompletion> FirstCollectionPair =>
        SourceSequence.CollectionPairSummaries[StartCollectionPairIndex];

    /// <summary>Gets the last represented collection-pair authority.</summary>
    public HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
        TRequest,
        TState,
        TCompletion> LastCollectionPair =>
        SourceSequence.CollectionPairSummaries[EndCollectionPairIndex];

    /// <summary>Gets the number of represented collection-pair summaries.</summary>
    public int CollectionPairCount => checked(
        RangeSummary.CollectionPairCount +
        AdjacentCollectionSequence.CollectionPairCount);

    /// <summary>Gets the number of represented collection parts.</summary>
    public int CollectionCount => checked(CollectionPairCount * 2);

    /// <summary>Gets the number of represented multi-sequence summaries.</summary>
    public int SummaryCount => CountSummaries();

    /// <summary>Gets the number of represented sequence authorities.</summary>
    public int SequenceCount => CountSequences();

    /// <summary>Gets the number of represented pair summaries.</summary>
    public int PairCount => CountPairs();

    /// <summary>Gets the number of represented recovery windows.</summary>
    public int WindowCount => CountWindows();

    /// <summary>Gets the first checkpoint index represented by the multi-collection.</summary>
    public int StartCheckpointIndex =>
        ConnectsPreviousCollectionSequence
            ? AdjacentCollectionSequence.StartCheckpointIndex
            : RangeSummary.StartCheckpointIndex;

    /// <summary>Gets the last checkpoint index represented by the multi-collection.</summary>
    public int EndCheckpointIndex =>
        ConnectsPreviousCollectionSequence
            ? RangeSummary.EndCheckpointIndex
            : AdjacentCollectionSequence.EndCheckpointIndex;

    /// <summary>Gets the source-chain index before the connecting edge.</summary>
    public int ConnectingPriorCheckpointIndex =>
        ConnectsPreviousCollectionSequence
            ? AdjacentCollectionSequence.EndCheckpointIndex
            : RangeSummary.EndCheckpointIndex;

    /// <summary>Gets the source-chain index after the connecting edge.</summary>
    public int ConnectingSuccessorCheckpointIndex =>
        checked(ConnectingPriorCheckpointIndex + 1);

    /// <summary>Gets the exact first checkpoint authority represented by the multi-collection.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint =>
        ConnectsPreviousCollectionSequence
            ? AdjacentCollectionSequence.StartCheckpoint
            : RangeSummary.StartCheckpoint;

    /// <summary>Gets the exact last checkpoint authority represented by the multi-collection.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint =>
        ConnectsPreviousCollectionSequence
            ? RangeSummary.EndCheckpoint
            : AdjacentCollectionSequence.EndCheckpoint;

    /// <summary>Gets the first represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> StartCheckpointId =>
        StartCheckpoint.CheckpointId;

    /// <summary>Gets the last represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> EndCheckpointId =>
        EndCheckpoint.CheckpointId;

    /// <summary>Gets the number of checkpoints compactly represented by the multi-collection.</summary>
    public int CheckpointCount => checked(
        RangeSummary.CheckpointCount + AdjacentCollectionSequence.CheckpointCount);

    /// <summary>Gets the number of supersessions compactly represented by the multi-collection.</summary>
    public int SupersessionCount => checked(
        RangeSummary.SupersessionCount + AdjacentCollectionSequence.SupersessionCount + 1);

    /// <summary>Gets whether the multi-collection starts at source sequence start.</summary>
    public bool StartsAtSourceSequenceStart =>
        StartCollectionPairIndex == 0;

    /// <summary>Gets whether the multi-collection ends at source sequence end.</summary>
    public bool EndsAtSourceSequenceEnd =>
        EndCollectionPairIndex == checked(SourceSequence.CollectionPairCount - 1);

    /// <summary>Gets whether the multi-collection starts at source collection start.</summary>
    public bool StartsAtSourceCollectionStart =>
        FirstCollectionPair.StartSummaryIndex == 0;

    /// <summary>Gets whether the multi-collection ends at source collection end.</summary>
    public bool EndsAtSourceCollectionEnd =>
        LastCollectionPair.EndSummaryIndex ==
            checked(SourceCollection.SummaryCount - 1);

    /// <summary>Gets whether the multi-collection starts at the chain root.</summary>
    public bool StartsAtRoot => StartCheckpointIndex == 0;

    /// <summary>Gets whether the multi-collection ends at the latest checkpoint.</summary>
    public bool EndsAtLatest =>
        EndCheckpointIndex == SourceProjection.SupersessionCount;

    /// <summary>Gets the external monotonic multi-collection-summary projection tick.</summary>
    public long ProjectedTick { get; }

    /// <summary>Gets the continuous multi-collection summary authority revision.</summary>
    public long Revision { get; }

    private int CountSummaries()
    {
        var count = 0;
        for (var index = StartCollectionPairIndex;
             index <= EndCollectionPairIndex;
             index++)
        {
            count = checked(
                count + SourceSequence.CollectionPairSummaries[index].SummaryCount);
        }

        return count;
    }

    private int CountSequences()
    {
        var count = 0;
        for (var index = StartCollectionPairIndex;
             index <= EndCollectionPairIndex;
             index++)
        {
            count = checked(
                count + SourceSequence.CollectionPairSummaries[index].SequenceCount);
        }

        return count;
    }

    private int CountPairs()
    {
        var count = 0;
        for (var index = StartCollectionPairIndex;
             index <= EndCollectionPairIndex;
             index++)
        {
            count = checked(
                count + SourceSequence.CollectionPairSummaries[index].PairCount);
        }

        return count;
    }

    private int CountWindows()
    {
        var count = 0;
        for (var index = StartCollectionPairIndex;
             index <= EndCollectionPairIndex;
             index++)
        {
            count = checked(
                count + SourceSequence.CollectionPairSummaries[index].WindowCount);
        }

        return count;
    }
}
