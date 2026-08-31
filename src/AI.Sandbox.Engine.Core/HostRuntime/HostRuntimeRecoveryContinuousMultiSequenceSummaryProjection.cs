namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable compact summary over one exact multi-window range and
/// one immediately adjacent projected sequence joined by validated continuity.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionIdKind> summaryId,
        HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation<
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

    /// <summary>Gets the externally assigned continuous multi-sequence summary ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionIdKind> SummaryId { get; }

    /// <summary>Gets unchanged checkpoint-range continuity authority.</summary>
    public HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion> Continuity { get; }

    /// <summary>Gets unchanged source checkpoint-range summary authority.</summary>
    public HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> RangeSummary => Continuity.Summary;

    /// <summary>Gets unchanged projected adjacent-sequence authority.</summary>
    public HostRuntimeRecoveryAdjacentSequenceProjection<
        TRequest,
        TState,
        TCompletion> AdjacentSequence => Continuity.AdjacentSequence;

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
    public bool ConnectsPreviousSequence => Continuity.ValidatesPreviousSequence;

    /// <summary>Gets whether the adjacent sequence follows the summarized range.</summary>
    public bool ConnectsNextSequence => Continuity.ValidatesNextSequence;

    /// <summary>Gets the exact supersession connecting both sequences.</summary>
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
    public int StartPairIndex =>
        ConnectsPreviousSequence
            ? AdjacentSequence.StartPairIndex
            : RangeSummary.StartPairIndex;

    /// <summary>Gets the last represented pair index in the source sequence.</summary>
    public int EndPairIndex =>
        ConnectsPreviousSequence
            ? RangeSummary.EndPairIndex
            : AdjacentSequence.EndPairIndex;

    /// <summary>Gets the source pair index before the connecting boundary.</summary>
    public int ConnectingPriorPairIndex =>
        ConnectsPreviousSequence
            ? AdjacentSequence.EndPairIndex
            : RangeSummary.EndPairIndex;

    /// <summary>Gets the source pair index after the connecting boundary.</summary>
    public int ConnectingSuccessorPairIndex =>
        checked(ConnectingPriorPairIndex + 1);

    /// <summary>Gets the number of represented pair summaries.</summary>
    public int PairCount => checked(
        RangeSummary.PairCount + AdjacentSequence.PairCount);

    /// <summary>Gets the number of represented recovery windows.</summary>
    public int WindowCount => checked(PairCount * 2);

    /// <summary>Gets the number of joined sequence authorities.</summary>
    public int SequenceCount => 2;

    /// <summary>Gets the first checkpoint index represented by the multi-sequence.</summary>
    public int StartCheckpointIndex =>
        ConnectsPreviousSequence
            ? AdjacentSequence.StartCheckpointIndex
            : RangeSummary.StartCheckpointIndex;

    /// <summary>Gets the last checkpoint index represented by the multi-sequence.</summary>
    public int EndCheckpointIndex =>
        ConnectsPreviousSequence
            ? RangeSummary.EndCheckpointIndex
            : AdjacentSequence.EndCheckpointIndex;

    /// <summary>Gets the source-chain index before the connecting edge.</summary>
    public int ConnectingPriorCheckpointIndex =>
        ConnectsPreviousSequence
            ? AdjacentSequence.EndCheckpointIndex
            : RangeSummary.EndCheckpointIndex;

    /// <summary>Gets the source-chain index after the connecting edge.</summary>
    public int ConnectingSuccessorCheckpointIndex =>
        checked(ConnectingPriorCheckpointIndex + 1);

    /// <summary>Gets the exact first checkpoint authority represented by the multi-sequence.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint =>
        ConnectsPreviousSequence
            ? AdjacentSequence.StartCheckpoint
            : RangeSummary.StartCheckpoint;

    /// <summary>Gets the exact last checkpoint authority represented by the multi-sequence.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint =>
        ConnectsPreviousSequence
            ? RangeSummary.EndCheckpoint
            : AdjacentSequence.EndCheckpoint;

    /// <summary>Gets the first represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> StartCheckpointId =>
        StartCheckpoint.CheckpointId;

    /// <summary>Gets the last represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> EndCheckpointId =>
        EndCheckpoint.CheckpointId;

    /// <summary>Gets the number of checkpoints compactly represented by the multi-sequence.</summary>
    public int CheckpointCount => checked(
        RangeSummary.CheckpointCount + AdjacentSequence.CheckpointCount);

    /// <summary>Gets the number of supersessions compactly represented by the multi-sequence.</summary>
    public int SupersessionCount => checked(
        RangeSummary.SupersessionCount + AdjacentSequence.SupersessionCount + 1);

    /// <summary>Gets whether the multi-sequence starts at the chain root.</summary>
    public bool StartsAtRoot => StartCheckpointIndex == 0;

    /// <summary>Gets whether the multi-sequence ends at the latest checkpoint.</summary>
    public bool EndsAtLatest =>
        EndCheckpointIndex == SourceProjection.SupersessionCount;

    /// <summary>Gets the external monotonic multi-sequence-summary projection tick.</summary>
    public long ProjectedTick { get; }

    /// <summary>Gets the continuous multi-sequence summary authority revision.</summary>
    public long Revision { get; }
}
