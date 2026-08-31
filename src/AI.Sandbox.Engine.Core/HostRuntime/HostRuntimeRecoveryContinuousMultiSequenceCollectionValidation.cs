namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority proving one bounded ordered collection of exact
/// continuous recovery multi-sequence summary projections.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryContinuousMultiSequenceCollectionValidationIdKind> validationId,
        HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion>[] multiSequenceSummaries,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>[] boundarySupersessions,
        long validatedTick,
        long revision)
    {
        ValidationId = validationId;
        MultiSequenceSummaries =
            global::System.Array.AsReadOnly(multiSequenceSummaries);
        BoundarySupersessions =
            global::System.Array.AsReadOnly(boundarySupersessions);
        ValidatedTick = validatedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned collection-validation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryContinuousMultiSequenceCollectionValidationIdKind> ValidationId { get; }

    /// <summary>Gets exact ordered continuous multi-sequence summary authorities.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion>> MultiSequenceSummaries { get; }

    /// <summary>Gets every exact validated sequence boundary in chain order.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> BoundarySupersessions { get; }

    /// <summary>Gets the first exact multi-sequence summary authority.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
        TRequest,
        TState,
        TCompletion> FirstSummary => MultiSequenceSummaries[0];

    /// <summary>Gets the last exact multi-sequence summary authority.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
        TRequest,
        TState,
        TCompletion> LastSummary => MultiSequenceSummaries[^1];

    /// <summary>Gets the unchanged source chain-summary projection authority.</summary>
    public HostRuntimeRecoveryChainSummaryProjection<
        TRequest,
        TState,
        TCompletion> SourceProjection => FirstSummary.SourceProjection;

    /// <summary>Gets the unchanged validated supersession-chain authority.</summary>
    public HostRuntimeRecoverySupersessionChain<
        TRequest,
        TState,
        TCompletion> Chain => SourceProjection.Chain;

    /// <summary>Gets the first represented pair index.</summary>
    public int StartPairIndex => FirstSummary.StartPairIndex;

    /// <summary>Gets the last represented pair index.</summary>
    public int EndPairIndex => LastSummary.EndPairIndex;

    /// <summary>Gets the first represented source-chain checkpoint index.</summary>
    public int StartCheckpointIndex => FirstSummary.StartCheckpointIndex;

    /// <summary>Gets the last represented source-chain checkpoint index.</summary>
    public int EndCheckpointIndex => LastSummary.EndCheckpointIndex;

    /// <summary>Gets the exact first represented checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint =>
        FirstSummary.StartCheckpoint;

    /// <summary>Gets the exact last represented checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint =>
        LastSummary.EndCheckpoint;

    /// <summary>Gets the first represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> StartCheckpointId =>
        StartCheckpoint.CheckpointId;

    /// <summary>Gets the last represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> EndCheckpointId =>
        EndCheckpoint.CheckpointId;

    /// <summary>Gets the number of exact multi-sequence summaries.</summary>
    public int SummaryCount => MultiSequenceSummaries.Count;

    /// <summary>Gets the number of joined sequence authorities.</summary>
    public int SequenceCount => checked(SummaryCount * 2);

    /// <summary>Gets the number of represented pair summaries.</summary>
    public int PairCount => checked(EndPairIndex - StartPairIndex + 1);

    /// <summary>Gets the number of represented recovery windows.</summary>
    public int WindowCount => checked(PairCount * 2);

    /// <summary>Gets the number of represented checkpoints.</summary>
    public int CheckpointCount =>
        checked(EndCheckpointIndex - StartCheckpointIndex + 1);

    /// <summary>Gets the number of represented supersession edges.</summary>
    public int SupersessionCount => checked(CheckpointCount - 1);

    /// <summary>Gets whether the collection starts at the chain root.</summary>
    public bool StartsAtRoot => StartCheckpointIndex == 0;

    /// <summary>Gets whether the collection ends at the latest checkpoint.</summary>
    public bool EndsAtLatest =>
        EndCheckpointIndex == SourceProjection.SupersessionCount;

    /// <summary>Gets the external monotonic collection-validation tick.</summary>
    public long ValidatedTick { get; }

    /// <summary>Gets the collection-validation authority revision.</summary>
    public long Revision { get; }
}
