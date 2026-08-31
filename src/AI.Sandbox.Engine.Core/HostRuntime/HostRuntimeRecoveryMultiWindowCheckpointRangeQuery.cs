namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable bounded inclusive checkpoint range crossing exact
/// validated boundaries of a continuous recovery-window sequence.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryMultiWindowCheckpointRangeQuery<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiWindowCheckpointRangeQuery(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryMultiWindowCheckpointRangeQueryIdKind> queryId,
        HostRuntimeRecoveryContinuousWindowSequenceValidation<
            TRequest,
            TState,
            TCompletion> sequence,
        HostRuntimeRecoveryCheckpoint<TRequest>[] checkpoints,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>[] supersessions,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>[] crossedBoundarySupersessions,
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
        int startPairIndex,
        int endPairIndex,
        long queriedTick,
        long revision)
    {
        QueryId = queryId;
        Sequence = sequence;
        Checkpoints = global::System.Array.AsReadOnly(checkpoints);
        Supersessions = global::System.Array.AsReadOnly(supersessions);
        CrossedBoundarySupersessions =
            global::System.Array.AsReadOnly(crossedBoundarySupersessions);
        IncomingSupersession = incomingSupersession;
        OutgoingSupersession = outgoingSupersession;
        StartCheckpointIndex = startCheckpointIndex;
        EndCheckpointIndex = endCheckpointIndex;
        StartPairIndex = startPairIndex;
        EndPairIndex = endPairIndex;
        QueriedTick = queriedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned multi-window query ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryMultiWindowCheckpointRangeQueryIdKind> QueryId { get; }

    /// <summary>Gets unchanged continuous-window sequence authority.</summary>
    public HostRuntimeRecoveryContinuousWindowSequenceValidation<
        TRequest,
        TState,
        TCompletion> Sequence { get; }

    /// <summary>Gets unchanged source chain-summary projection authority.</summary>
    public HostRuntimeRecoveryChainSummaryProjection<
        TRequest,
        TState,
        TCompletion> SourceProjection => Sequence.SourceProjection;

    /// <summary>Gets unchanged validated supersession-chain authority.</summary>
    public HostRuntimeRecoverySupersessionChain<
        TRequest,
        TState,
        TCompletion> Chain => Sequence.Chain;

    /// <summary>Gets exact checkpoint authorities in inclusive range order.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpoint<TRequest>> Checkpoints { get; }

    /// <summary>Gets exact supersession authorities inside the range.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> Supersessions { get; }

    /// <summary>Gets exact validated boundaries crossed by the range.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> CrossedBoundarySupersessions { get; }

    /// <summary>Gets the exact supersession entering the range.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? IncomingSupersession { get; }

    /// <summary>Gets the exact supersession leaving the range.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? OutgoingSupersession { get; }

    /// <summary>Gets the first range checkpoint index in the source chain.</summary>
    public int StartCheckpointIndex { get; }

    /// <summary>Gets the last range checkpoint index in the source chain.</summary>
    public int EndCheckpointIndex { get; }

    /// <summary>Gets the first intersected pair index in the sequence.</summary>
    public int StartPairIndex { get; }

    /// <summary>Gets the last intersected pair index in the sequence.</summary>
    public int EndPairIndex { get; }

    /// <summary>Gets the exact first checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint => Checkpoints[0];

    /// <summary>Gets the exact last checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint => Checkpoints[^1];

    /// <summary>Gets the first checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> StartCheckpointId =>
        StartCheckpoint.CheckpointId;

    /// <summary>Gets the last checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> EndCheckpointId =>
        EndCheckpoint.CheckpointId;

    /// <summary>Gets the number of represented checkpoints.</summary>
    public int CheckpointCount => Checkpoints.Count;

    /// <summary>Gets the number of represented supersession edges.</summary>
    public int SupersessionCount => Supersessions.Count;

    /// <summary>Gets the number of intersected pair summaries.</summary>
    public int PairCount => checked(EndPairIndex - StartPairIndex + 1);

    /// <summary>Gets the number of exact validated boundaries crossed.</summary>
    public int CrossedBoundaryCount => CrossedBoundarySupersessions.Count;

    /// <summary>Gets the number of represented windows separated by boundaries.</summary>
    public int CrossedWindowCount => checked(CrossedBoundaryCount + 1);

    /// <summary>Gets whether the query starts at the sequence boundary.</summary>
    public bool StartsAtSequenceStart =>
        StartCheckpointIndex == Sequence.StartCheckpointIndex;

    /// <summary>Gets whether the query ends at the sequence boundary.</summary>
    public bool EndsAtSequenceEnd =>
        EndCheckpointIndex == Sequence.EndCheckpointIndex;

    /// <summary>Gets the external monotonic query tick.</summary>
    public long QueriedTick { get; }

    /// <summary>Gets the multi-window query authority revision.</summary>
    public long Revision { get; }
}
