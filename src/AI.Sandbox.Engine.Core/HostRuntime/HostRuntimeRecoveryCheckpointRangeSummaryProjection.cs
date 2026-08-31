namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable compact summary projection over an exact bounded
/// Host recovery checkpoint-range query.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryCheckpointRangeSummaryProjection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCheckpointRangeSummaryProjection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryCheckpointRangeSummaryProjectionIdKind> summaryId,
        HostRuntimeRecoveryCheckpointRangeQuery<
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

    /// <summary>Gets the externally assigned checkpoint-range summary ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointRangeSummaryProjectionIdKind> SummaryId { get; }

    /// <summary>Gets unchanged source checkpoint-range query authority.</summary>
    public HostRuntimeRecoveryCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion> Range { get; }

    /// <summary>Gets unchanged source lineage-window authority.</summary>
    public HostRuntimeRecoveryLineageWindowProjection<
        TRequest,
        TState,
        TCompletion> Window => Range.Window;

    /// <summary>Gets unchanged source chain-summary projection authority.</summary>
    public HostRuntimeRecoveryChainSummaryProjection<
        TRequest,
        TState,
        TCompletion> SourceProjection => Window.SourceProjection;

    /// <summary>Gets unchanged validated supersession-chain authority.</summary>
    public HostRuntimeRecoverySupersessionChain<
        TRequest,
        TState,
        TCompletion> Chain => SourceProjection.Chain;

    /// <summary>Gets the exact first checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint =>
        Range.StartCheckpoint;

    /// <summary>Gets the exact last checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint =>
        Range.EndCheckpoint;

    /// <summary>Gets the exact incoming range-boundary supersession.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? IncomingSupersession => Range.IncomingSupersession;

    /// <summary>Gets the exact outgoing range-boundary supersession.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? OutgoingSupersession => Range.OutgoingSupersession;

    /// <summary>Gets the first checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> StartCheckpointId =>
        Range.StartCheckpointId;

    /// <summary>Gets the last checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> EndCheckpointId =>
        Range.EndCheckpointId;

    /// <summary>Gets the first checkpoint index in the source window.</summary>
    public int StartWindowIndex => Range.StartWindowIndex;

    /// <summary>Gets the last checkpoint index in the source window.</summary>
    public int EndWindowIndex => Range.EndWindowIndex;

    /// <summary>Gets the first checkpoint index in the source chain.</summary>
    public int StartChainIndex => Range.StartChainIndex;

    /// <summary>Gets the last checkpoint index in the source chain.</summary>
    public int EndChainIndex => Range.EndChainIndex;

    /// <summary>Gets the number of represented checkpoints.</summary>
    public int CheckpointCount => Range.CheckpointCount;

    /// <summary>Gets the number of represented supersession edges.</summary>
    public int SupersessionCount => Range.SupersessionCount;

    /// <summary>Gets whether the range begins at the source window start.</summary>
    public bool StartsAtWindowStart => StartWindowIndex == 0;

    /// <summary>Gets whether the range ends at the source window end.</summary>
    public bool EndsAtWindowEnd => EndWindowIndex == Window.CheckpointCount - 1;

    /// <summary>Gets whether the range covers the entire source window.</summary>
    public bool CoversEntireWindow => StartsAtWindowStart && EndsAtWindowEnd;

    /// <summary>Gets whether the range begins at the chain root.</summary>
    public bool StartsAtRoot => StartChainIndex == 0;

    /// <summary>Gets whether the range ends at the latest checkpoint.</summary>
    public bool EndsAtLatest =>
        EndChainIndex == SourceProjection.SupersessionCount;

    /// <summary>Gets the external monotonic range-summary projection tick.</summary>
    public long ProjectedTick { get; }

    /// <summary>Gets the checkpoint-range summary authority revision.</summary>
    public long Revision { get; }
}
