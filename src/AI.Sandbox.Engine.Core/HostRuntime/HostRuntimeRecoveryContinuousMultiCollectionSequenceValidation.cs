namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority proving one bounded ordered collection of exact
/// continuous recovery multi-collection summary projections.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryContinuousMultiCollectionSequenceValidation<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousMultiCollectionSequenceValidation(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryContinuousMultiCollectionSequenceValidationIdKind> validationId,
        HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
            TRequest,
            TState,
            TCompletion>[] multiCollectionSummaries,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>[] boundarySupersessions,
        long validatedTick,
        long revision)
    {
        ValidationId = validationId;
        MultiCollectionSummaries =
            global::System.Array.AsReadOnly(multiCollectionSummaries);
        BoundarySupersessions =
            global::System.Array.AsReadOnly(boundarySupersessions);
        ValidatedTick = validatedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned collection-validation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryContinuousMultiCollectionSequenceValidationIdKind> ValidationId { get; }

    /// <summary>Gets exact ordered continuous multi-collection summary authorities.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
            TRequest,
            TState,
            TCompletion>> MultiCollectionSummaries { get; }

    /// <summary>Gets every exact validated sequence boundary in chain order.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> BoundarySupersessions { get; }

    /// <summary>Gets the first exact multi-collection summary authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
        TRequest,
        TState,
        TCompletion> FirstSummary => MultiCollectionSummaries[0];

    /// <summary>Gets the last exact multi-collection summary authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
        TRequest,
        TState,
        TCompletion> LastSummary => MultiCollectionSummaries[^1];

    /// <summary>Gets the unchanged source collection-sequence authority.</summary>
    public HostRuntimeRecoveryContinuousCollectionSequenceValidation<
        TRequest,
        TState,
        TCompletion> SourceSequence => FirstSummary.SourceSequence;

    /// <summary>Gets the unchanged source collection-validation authority.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
        TRequest,
        TState,
        TCompletion> SourceCollection => FirstSummary.SourceCollection;

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
    public int StartCollectionPairIndex => FirstSummary.StartCollectionPairIndex;

    /// <summary>Gets the last represented pair index.</summary>
    public int EndCollectionPairIndex => LastSummary.EndCollectionPairIndex;

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

    /// <summary>Gets the number of exact multi-collection summary authorities.</summary>
    public int MultiCollectionCount => MultiCollectionSummaries.Count;

    /// <summary>Gets the number of represented collection-pair summaries.</summary>
    public int CollectionPairCount =>
        checked(EndCollectionPairIndex - StartCollectionPairIndex + 1);

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

    /// <summary>Gets the number of represented checkpoints.</summary>
    public int CheckpointCount =>
        checked(EndCheckpointIndex - StartCheckpointIndex + 1);

    /// <summary>Gets the number of represented supersession edges.</summary>
    public int SupersessionCount => checked(CheckpointCount - 1);

    /// <summary>Gets whether the sequence starts at source collection start.</summary>
    public bool StartsAtSourceCollectionStart =>
        FirstSummary.StartsAtSourceCollectionStart;

    /// <summary>Gets whether the sequence ends at source collection end.</summary>
    public bool EndsAtSourceCollectionEnd =>
        LastSummary.EndsAtSourceCollectionEnd;

    /// <summary>Gets whether the sequence starts at the chain root.</summary>
    public bool StartsAtRoot => StartCheckpointIndex == 0;

    /// <summary>Gets whether the sequence ends at the latest checkpoint.</summary>
    public bool EndsAtLatest =>
        EndCheckpointIndex == SourceProjection.SupersessionCount;

    /// <summary>Gets the external monotonic collection-validation tick.</summary>
    public long ValidatedTick { get; }

    /// <summary>Gets the collection-validation authority revision.</summary>
    public long Revision { get; }

    private int CountSummaries()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSummaries)
        {
            count = checked(count + summary.SummaryCount);
        }

        return count;
    }

    private int CountSequences()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSummaries)
        {
            count = checked(count + summary.SequenceCount);
        }

        return count;
    }

    private int CountPairs()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSummaries)
        {
            count = checked(count + summary.PairCount);
        }

        return count;
    }

    private int CountWindows()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSummaries)
        {
            count = checked(count + summary.WindowCount);
        }

        return count;
    }
}
