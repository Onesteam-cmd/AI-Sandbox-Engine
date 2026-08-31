namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable bounded inclusive checkpoint-range query crossing the
/// exact shared boundary of one continuous recovery collection-pair.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryCrossCollectionCheckpointRangeQuery<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCrossCollectionCheckpointRangeQuery(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryCrossCollectionCheckpointRangeQueryIdKind> queryId,
        HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
            TRequest,
            TState,
            TCompletion> collectionPairSummary,
        HostRuntimeRecoveryCheckpoint<TRequest>[] checkpoints,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>[] supersessions,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>? incomingSupersession,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>? outgoingSupersession,
        int startCheckpointIndex,
        int endCheckpointIndex,
        long queriedTick,
        long revision)
    {
        QueryId = queryId;
        CollectionPairSummary = collectionPairSummary;
        Checkpoints = global::System.Array.AsReadOnly(checkpoints);
        Supersessions = global::System.Array.AsReadOnly(supersessions);
        IncomingSupersession = incomingSupersession;
        OutgoingSupersession = outgoingSupersession;
        StartCheckpointIndex = startCheckpointIndex;
        EndCheckpointIndex = endCheckpointIndex;
        QueriedTick = queriedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned cross-collection query ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCrossCollectionCheckpointRangeQueryIdKind> QueryId { get; }

    /// <summary>Gets unchanged continuous collection-pair summary authority.</summary>
    public HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
        TRequest,
        TState,
        TCompletion> CollectionPairSummary { get; }

    /// <summary>Gets unchanged validated source chain.</summary>
    public HostRuntimeRecoverySupersessionChain<
        TRequest,
        TState,
        TCompletion> Chain => CollectionPairSummary.Chain;

    /// <summary>Gets exact checkpoint authorities in the inclusive range.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpoint<TRequest>> Checkpoints { get; }

    /// <summary>Gets exact supersession authorities inside the range.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> Supersessions { get; }

    /// <summary>Gets the exact supersession entering the range, when present.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? IncomingSupersession { get; }

    /// <summary>Gets the exact supersession leaving the range, when present.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? OutgoingSupersession { get; }

    /// <summary>Gets the first checkpoint index in the source chain.</summary>
    public int StartCheckpointIndex { get; }

    /// <summary>Gets the last checkpoint index in the source chain.</summary>
    public int EndCheckpointIndex { get; }

    /// <summary>Gets the number of represented checkpoints.</summary>
    public int CheckpointCount => Checkpoints.Count;

    /// <summary>Gets the number of represented supersessions.</summary>
    public int SupersessionCount => Supersessions.Count;

    /// <summary>Gets the first represented checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint => Checkpoints[0];

    /// <summary>Gets the last represented checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint => Checkpoints[^1];

    /// <summary>Gets the first represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> StartCheckpointId =>
        StartCheckpoint.CheckpointId;

    /// <summary>Gets the last represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> EndCheckpointId =>
        EndCheckpoint.CheckpointId;

    /// <summary>Gets the exact supersession connecting both source collections.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion> ConnectingSupersession =>
        CollectionPairSummary.ConnectingSupersession;

    /// <summary>Gets whether the represented range crosses the shared boundary.</summary>
    public bool CrossesCollectionBoundary =>
        StartCheckpointIndex <= CollectionPairSummary.ConnectingPriorCheckpointIndex &&
        EndCheckpointIndex >= CollectionPairSummary.ConnectingSuccessorCheckpointIndex;

    /// <summary>Gets whether the range starts at the collection-pair boundary.</summary>
    public bool StartsAtCollectionPairStart =>
        StartCheckpointIndex == CollectionPairSummary.StartCheckpointIndex;

    /// <summary>Gets whether the range ends at the collection-pair boundary.</summary>
    public bool EndsAtCollectionPairEnd =>
        EndCheckpointIndex == CollectionPairSummary.EndCheckpointIndex;

    /// <summary>Gets the external monotonic cross-collection query tick.</summary>
    public long QueriedTick { get; }

    /// <summary>Gets the cross-collection checkpoint-range query authority revision.</summary>
    public long Revision { get; }
}
