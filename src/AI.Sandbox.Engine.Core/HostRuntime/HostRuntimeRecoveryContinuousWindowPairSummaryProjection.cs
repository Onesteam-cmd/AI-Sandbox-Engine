namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable compact summary over an exact validated continuous
/// pair formed by a checkpoint range and one adjacent projected window.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousWindowPairSummaryProjection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryContinuousWindowPairSummaryProjectionIdKind> summaryId,
        HostRuntimeRecoveryCheckpointRangeContinuityValidation<
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

    /// <summary>Gets the externally assigned continuous-window pair summary ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryContinuousWindowPairSummaryProjectionIdKind> SummaryId { get; }

    /// <summary>Gets unchanged checkpoint-range continuity authority.</summary>
    public HostRuntimeRecoveryCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion> Continuity { get; }

    /// <summary>Gets unchanged source checkpoint-range summary authority.</summary>
    public HostRuntimeRecoveryCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> RangeSummary => Continuity.Summary;

    /// <summary>Gets unchanged projected adjacent-window authority.</summary>
    public HostRuntimeRecoveryAdjacentWindowProjection<
        TRequest,
        TState,
        TCompletion> AdjacentWindow => Continuity.AdjacentWindow;

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

    /// <summary>Gets whether the adjacent window precedes the summarized range.</summary>
    public bool ConnectsPreviousWindow => Continuity.ValidatesPreviousWindow;

    /// <summary>Gets whether the adjacent window follows the summarized range.</summary>
    public bool ConnectsNextWindow => Continuity.ValidatesNextWindow;

    /// <summary>Gets the exact supersession connecting both windows.</summary>
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

    /// <summary>Gets the first checkpoint index represented by the pair.</summary>
    public int StartCheckpointIndex =>
        ConnectsPreviousWindow
            ? AdjacentWindow.StartCheckpointIndex
            : RangeSummary.StartChainIndex;

    /// <summary>Gets the last checkpoint index represented by the pair.</summary>
    public int EndCheckpointIndex =>
        ConnectsPreviousWindow
            ? RangeSummary.EndChainIndex
            : AdjacentWindow.EndCheckpointIndex;

    /// <summary>Gets the source-chain index before the connecting edge.</summary>
    public int ConnectingPriorCheckpointIndex =>
        ConnectsPreviousWindow
            ? AdjacentWindow.EndCheckpointIndex
            : RangeSummary.EndChainIndex;

    /// <summary>Gets the source-chain index after the connecting edge.</summary>
    public int ConnectingSuccessorCheckpointIndex =>
        checked(ConnectingPriorCheckpointIndex + 1);

    /// <summary>Gets the exact first checkpoint authority represented by the pair.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint =>
        ConnectsPreviousWindow
            ? AdjacentWindow.StartCheckpoint
            : RangeSummary.StartCheckpoint;

    /// <summary>Gets the exact last checkpoint authority represented by the pair.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint =>
        ConnectsPreviousWindow
            ? RangeSummary.EndCheckpoint
            : AdjacentWindow.EndCheckpoint;

    /// <summary>Gets the first represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> StartCheckpointId =>
        StartCheckpoint.CheckpointId;

    /// <summary>Gets the last represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> EndCheckpointId =>
        EndCheckpoint.CheckpointId;

    /// <summary>Gets the number of checkpoints compactly represented by the pair.</summary>
    public int CheckpointCount => checked(
        RangeSummary.CheckpointCount + AdjacentWindow.CheckpointCount);

    /// <summary>Gets the number of supersessions compactly represented by the pair.</summary>
    public int SupersessionCount => checked(
        RangeSummary.SupersessionCount + AdjacentWindow.SupersessionCount + 1);

    /// <summary>Gets whether the pair starts at the chain root.</summary>
    public bool StartsAtRoot => StartCheckpointIndex == 0;

    /// <summary>Gets whether the pair ends at the latest checkpoint.</summary>
    public bool EndsAtLatest =>
        EndCheckpointIndex == SourceProjection.SupersessionCount;

    /// <summary>Gets the external monotonic pair-summary projection tick.</summary>
    public long ProjectedTick { get; }

    /// <summary>Gets the continuous-window pair summary authority revision.</summary>
    public long Revision { get; }
}
