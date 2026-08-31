namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority proving one bounded ordered sequence of exact
/// continuous recovery-window pair summaries.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryContinuousWindowSequenceValidation<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousWindowSequenceValidation(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryContinuousWindowSequenceValidationIdKind> validationId,
        HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
            TRequest,
            TState,
            TCompletion>[] pairSummaries,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>[] boundarySupersessions,
        long validatedTick,
        long revision)
    {
        ValidationId = validationId;
        PairSummaries = global::System.Array.AsReadOnly(pairSummaries);
        BoundarySupersessions = global::System.Array.AsReadOnly(boundarySupersessions);
        ValidatedTick = validatedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned sequence-validation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryContinuousWindowSequenceValidationIdKind> ValidationId { get; }

    /// <summary>Gets exact ordered continuous-window pair-summary authorities.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
            TRequest,
            TState,
            TCompletion>> PairSummaries { get; }

    /// <summary>Gets every exact validated boundary supersession in chain order.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> BoundarySupersessions { get; }

    /// <summary>Gets the first exact pair-summary authority.</summary>
    public HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
        TRequest,
        TState,
        TCompletion> FirstPair => PairSummaries[0];

    /// <summary>Gets the last exact pair-summary authority.</summary>
    public HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
        TRequest,
        TState,
        TCompletion> LastPair => PairSummaries[^1];

    /// <summary>Gets the unchanged source chain-summary projection authority.</summary>
    public HostRuntimeRecoveryChainSummaryProjection<
        TRequest,
        TState,
        TCompletion> SourceProjection => FirstPair.SourceProjection;

    /// <summary>Gets the unchanged validated supersession-chain authority.</summary>
    public HostRuntimeRecoverySupersessionChain<
        TRequest,
        TState,
        TCompletion> Chain => SourceProjection.Chain;

    /// <summary>Gets the first represented source-chain checkpoint index.</summary>
    public int StartCheckpointIndex => FirstPair.StartCheckpointIndex;

    /// <summary>Gets the last represented source-chain checkpoint index.</summary>
    public int EndCheckpointIndex => LastPair.EndCheckpointIndex;

    /// <summary>Gets the exact first represented checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint =>
        FirstPair.StartCheckpoint;

    /// <summary>Gets the exact last represented checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint =>
        LastPair.EndCheckpoint;

    /// <summary>Gets the first represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> StartCheckpointId =>
        StartCheckpoint.CheckpointId;

    /// <summary>Gets the last represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> EndCheckpointId =>
        EndCheckpoint.CheckpointId;

    /// <summary>Gets the number of exact pair-summary authorities.</summary>
    public int PairCount => PairSummaries.Count;

    /// <summary>Gets the number of represented recovery windows.</summary>
    public int WindowCount => checked(PairCount * 2);

    /// <summary>Gets the number of represented checkpoints.</summary>
    public int CheckpointCount =>
        checked(EndCheckpointIndex - StartCheckpointIndex + 1);

    /// <summary>Gets the number of represented supersession edges.</summary>
    public int SupersessionCount => checked(CheckpointCount - 1);

    /// <summary>Gets whether the sequence starts at the chain root.</summary>
    public bool StartsAtRoot => StartCheckpointIndex == 0;

    /// <summary>Gets whether the sequence ends at the latest checkpoint.</summary>
    public bool EndsAtLatest => EndCheckpointIndex == SourceProjection.SupersessionCount;

    /// <summary>Gets the external monotonic sequence-validation tick.</summary>
    public long ValidatedTick { get; }

    /// <summary>Gets the continuous-window sequence authority revision.</summary>
    public long Revision { get; }
}
