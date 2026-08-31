namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable compact summary over one exact multi-collection range and
/// one immediately adjacent projected multi-collection joined by validated continuity.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjectionIdKind> summaryId,
        HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidation<
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

    /// <summary>Gets the externally assigned continuous multi-collection-sequence summary ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjectionIdKind> SummaryId { get; }

    /// <summary>Gets unchanged checkpoint-range continuity authority.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion> Continuity { get; }

    /// <summary>Gets unchanged source checkpoint-range summary authority.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> RangeSummary => Continuity.Summary;

    /// <summary>Gets unchanged projected adjacent multi-collection authority.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionProjection<
        TRequest,
        TState,
        TCompletion> AdjacentMultiCollection => Continuity.AdjacentMultiCollection;

    /// <summary>Gets unchanged source multi-collection-sequence authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceValidation<
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

    /// <summary>Gets whether the adjacent multi-collection precedes the summarized range.</summary>
    public bool ConnectsPreviousMultiCollection => Continuity.ValidatesPreviousMultiCollection;

    /// <summary>Gets whether the adjacent multi-collection follows the summarized range.</summary>
    public bool ConnectsNextMultiCollection => Continuity.ValidatesNextMultiCollection;

    /// <summary>Gets the exact supersession connecting both multi-collection intervals.</summary>
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

    /// <summary>Gets the first represented multi-collection-summary index.</summary>
    public int StartSummaryIndex =>
        ConnectsPreviousMultiCollection
            ? AdjacentMultiCollection.StartSummaryIndex
            : RangeSummary.StartSummaryIndex;

    /// <summary>Gets the last represented multi-collection-summary index.</summary>
    public int EndSummaryIndex =>
        ConnectsPreviousMultiCollection
            ? RangeSummary.EndSummaryIndex
            : AdjacentMultiCollection.EndSummaryIndex;

    /// <summary>Gets the source summary index before the connecting boundary.</summary>
    public int ConnectingPriorSummaryIndex =>
        ConnectsPreviousMultiCollection
            ? AdjacentMultiCollection.EndSummaryIndex
            : RangeSummary.EndSummaryIndex;

    /// <summary>Gets the source summary index after the connecting boundary.</summary>
    public int ConnectingSuccessorSummaryIndex =>
        checked(ConnectingPriorSummaryIndex + 1);

    /// <summary>Gets the first represented multi-collection summary authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
        TRequest,
        TState,
        TCompletion> FirstMultiCollection =>
        SourceSequence.MultiCollectionSummaries[StartSummaryIndex];

    /// <summary>Gets the last represented multi-collection summary authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
        TRequest,
        TState,
        TCompletion> LastMultiCollection =>
        SourceSequence.MultiCollectionSummaries[EndSummaryIndex];

    /// <summary>Gets the number of represented multi-collection summaries.</summary>
    public int MultiCollectionCount => checked(
        EndSummaryIndex - StartSummaryIndex + 1);

    /// <summary>Gets the number of represented collection-pair summaries.</summary>
    public int CollectionPairCount => CountCollectionPairs();

    /// <summary>Gets the number of represented collection parts.</summary>
    public int CollectionCount => CountCollections();

    /// <summary>Gets the number of represented multi-sequence summaries.</summary>
    public int SummaryCount => CountSummaries();

    /// <summary>Gets the number of represented sequence authorities.</summary>
    public int SequenceCount => CountSequences();

    /// <summary>Gets the number of represented pair summaries.</summary>
    public int PairCount => CountPairs();

    /// <summary>Gets the number of represented recovery windows.</summary>
    public int WindowCount => CountWindows();

    /// <summary>Gets the first checkpoint index represented by the sequence summary.</summary>
    public int StartCheckpointIndex =>
        ConnectsPreviousMultiCollection
            ? AdjacentMultiCollection.StartCheckpointIndex
            : RangeSummary.StartCheckpointIndex;

    /// <summary>Gets the last checkpoint index represented by the sequence summary.</summary>
    public int EndCheckpointIndex =>
        ConnectsPreviousMultiCollection
            ? RangeSummary.EndCheckpointIndex
            : AdjacentMultiCollection.EndCheckpointIndex;

    /// <summary>Gets the source-chain index before the connecting edge.</summary>
    public int ConnectingPriorCheckpointIndex =>
        ConnectsPreviousMultiCollection
            ? AdjacentMultiCollection.EndCheckpointIndex
            : RangeSummary.EndCheckpointIndex;

    /// <summary>Gets the source-chain index after the connecting edge.</summary>
    public int ConnectingSuccessorCheckpointIndex =>
        checked(ConnectingPriorCheckpointIndex + 1);

    /// <summary>Gets the exact first represented checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint =>
        ConnectsPreviousMultiCollection
            ? AdjacentMultiCollection.StartCheckpoint
            : RangeSummary.StartCheckpoint;

    /// <summary>Gets the exact last represented checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint =>
        ConnectsPreviousMultiCollection
            ? RangeSummary.EndCheckpoint
            : AdjacentMultiCollection.EndCheckpoint;

    /// <summary>Gets the first represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> StartCheckpointId =>
        StartCheckpoint.CheckpointId;

    /// <summary>Gets the last represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> EndCheckpointId =>
        EndCheckpoint.CheckpointId;

    /// <summary>Gets the number of checkpoints compactly represented by the sequence summary.</summary>
    public int CheckpointCount => checked(
        RangeSummary.CheckpointCount + AdjacentMultiCollection.CheckpointCount);

    /// <summary>Gets the number of supersessions compactly represented by the sequence summary.</summary>
    public int SupersessionCount => checked(
        RangeSummary.SupersessionCount + AdjacentMultiCollection.SupersessionCount + 1);

    /// <summary>Gets whether the summary starts at source sequence start.</summary>
    public bool StartsAtSourceSequenceStart =>
        StartSummaryIndex == 0;

    /// <summary>Gets whether the summary ends at source sequence end.</summary>
    public bool EndsAtSourceSequenceEnd =>
        EndSummaryIndex == checked(SourceSequence.MultiCollectionCount - 1);

    /// <summary>Gets whether the summary starts at source collection start.</summary>
    public bool StartsAtSourceCollectionStart =>
        FirstMultiCollection.StartsAtSourceCollectionStart;

    /// <summary>Gets whether the summary ends at source collection end.</summary>
    public bool EndsAtSourceCollectionEnd =>
        LastMultiCollection.EndsAtSourceCollectionEnd;

    /// <summary>Gets whether the summary starts at the chain root.</summary>
    public bool StartsAtRoot => StartCheckpointIndex == 0;

    /// <summary>Gets whether the summary ends at the latest checkpoint.</summary>
    public bool EndsAtLatest =>
        EndCheckpointIndex == SourceProjection.SupersessionCount;

    /// <summary>Gets the external monotonic summary-projection tick.</summary>
    public long ProjectedTick { get; }

    /// <summary>Gets the continuous multi-collection-sequence summary authority revision.</summary>
    public long Revision { get; }

    private int CountCollectionPairs()
    {
        var count = 0;
        for (var index = StartSummaryIndex; index <= EndSummaryIndex; index++)
        {
            count = checked(
                count + SourceSequence.MultiCollectionSummaries[index].CollectionPairCount);
        }

        return count;
    }

    private int CountCollections()
    {
        var count = 0;
        for (var index = StartSummaryIndex; index <= EndSummaryIndex; index++)
        {
            count = checked(
                count + SourceSequence.MultiCollectionSummaries[index].CollectionCount);
        }

        return count;
    }

    private int CountSummaries()
    {
        var count = 0;
        for (var index = StartSummaryIndex; index <= EndSummaryIndex; index++)
        {
            count = checked(
                count + SourceSequence.MultiCollectionSummaries[index].SummaryCount);
        }

        return count;
    }

    private int CountSequences()
    {
        var count = 0;
        for (var index = StartSummaryIndex; index <= EndSummaryIndex; index++)
        {
            count = checked(
                count + SourceSequence.MultiCollectionSummaries[index].SequenceCount);
        }

        return count;
    }

    private int CountPairs()
    {
        var count = 0;
        for (var index = StartSummaryIndex; index <= EndSummaryIndex; index++)
        {
            count = checked(
                count + SourceSequence.MultiCollectionSummaries[index].PairCount);
        }

        return count;
    }

    private int CountWindows()
    {
        var count = 0;
        for (var index = StartSummaryIndex; index <= EndSummaryIndex; index++)
        {
            count = checked(
                count + SourceSequence.MultiCollectionSummaries[index].WindowCount);
        }

        return count;
    }
}
