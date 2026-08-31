namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority proving one bounded ordered sequence of exact
/// continuous recovery collection-pair summary projections.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryContinuousCollectionSequenceValidation<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousCollectionSequenceValidation(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryContinuousCollectionSequenceValidationIdKind> validationId,
        HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
            TRequest,
            TState,
            TCompletion>[] collectionPairSummaries,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>[] boundarySupersessions,
        long validatedTick,
        long revision)
    {
        ValidationId = validationId;
        CollectionPairSummaries =
            global::System.Array.AsReadOnly(collectionPairSummaries);
        BoundarySupersessions =
            global::System.Array.AsReadOnly(boundarySupersessions);
        ValidatedTick = validatedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned collection-sequence validation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryContinuousCollectionSequenceValidationIdKind> ValidationId { get; }

    /// <summary>Gets exact ordered continuous collection-pair summary authorities.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
            TRequest,
            TState,
            TCompletion>> CollectionPairSummaries { get; }

    /// <summary>Gets every exact validated collection boundary in chain order.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> BoundarySupersessions { get; }

    /// <summary>Gets the first exact collection-pair summary authority.</summary>
    public HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
        TRequest,
        TState,
        TCompletion> FirstCollectionPair => CollectionPairSummaries[0];

    /// <summary>Gets the last exact collection-pair summary authority.</summary>
    public HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
        TRequest,
        TState,
        TCompletion> LastCollectionPair => CollectionPairSummaries[^1];

    /// <summary>Gets the unchanged source collection-validation authority.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
        TRequest,
        TState,
        TCompletion> SourceCollection => FirstCollectionPair.SourceCollection;

    /// <summary>Gets the unchanged source chain-summary projection authority.</summary>
    public HostRuntimeRecoveryChainSummaryProjection<
        TRequest,
        TState,
        TCompletion> SourceProjection => FirstCollectionPair.SourceProjection;

    /// <summary>Gets the unchanged validated supersession-chain authority.</summary>
    public HostRuntimeRecoverySupersessionChain<
        TRequest,
        TState,
        TCompletion> Chain => SourceProjection.Chain;

    /// <summary>Gets the first represented source-collection summary index.</summary>
    public int StartSummaryIndex => FirstCollectionPair.StartSummaryIndex;

    /// <summary>Gets the last represented source-collection summary index.</summary>
    public int EndSummaryIndex => LastCollectionPair.EndSummaryIndex;

    /// <summary>Gets the first represented source-chain checkpoint index.</summary>
    public int StartCheckpointIndex => FirstCollectionPair.StartCheckpointIndex;

    /// <summary>Gets the last represented source-chain checkpoint index.</summary>
    public int EndCheckpointIndex => LastCollectionPair.EndCheckpointIndex;

    /// <summary>Gets the exact first represented checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint =>
        FirstCollectionPair.StartCheckpoint;

    /// <summary>Gets the exact last represented checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint =>
        LastCollectionPair.EndCheckpoint;

    /// <summary>Gets the first represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> StartCheckpointId =>
        StartCheckpoint.CheckpointId;

    /// <summary>Gets the last represented checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> EndCheckpointId =>
        EndCheckpoint.CheckpointId;

    /// <summary>Gets the number of exact collection-pair summary authorities.</summary>
    public int CollectionPairCount => CollectionPairSummaries.Count;

    /// <summary>Gets the number of joined collection authorities.</summary>
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

    /// <summary>Gets whether the sequence starts at the source collection boundary.</summary>
    public bool StartsAtSourceCollectionStart => StartSummaryIndex == 0;

    /// <summary>Gets whether the sequence ends at the source collection boundary.</summary>
    public bool EndsAtSourceCollectionEnd =>
        EndSummaryIndex == checked(SourceCollection.SummaryCount - 1);

    /// <summary>Gets whether the sequence starts at the chain root.</summary>
    public bool StartsAtRoot => StartCheckpointIndex == 0;

    /// <summary>Gets whether the sequence ends at the latest checkpoint.</summary>
    public bool EndsAtLatest =>
        EndCheckpointIndex == SourceProjection.SupersessionCount;

    /// <summary>Gets the external monotonic collection-sequence validation tick.</summary>
    public long ValidatedTick { get; }

    /// <summary>Gets the continuous collection-sequence authority revision.</summary>
    public long Revision { get; }

    private int CountSummaries()
    {
        var count = 0;
        foreach (var collectionPair in CollectionPairSummaries)
        {
            count = checked(count + collectionPair.SummaryCount);
        }

        return count;
    }

    private int CountSequences()
    {
        var count = 0;
        foreach (var collectionPair in CollectionPairSummaries)
        {
            count = checked(count + collectionPair.SequenceCount);
        }

        return count;
    }

    private int CountPairs()
    {
        var count = 0;
        foreach (var collectionPair in CollectionPairSummaries)
        {
            count = checked(count + collectionPair.PairCount);
        }

        return count;
    }

    private int CountWindows()
    {
        var count = 0;
        foreach (var collectionPair in CollectionPairSummaries)
        {
            count = checked(count + collectionPair.WindowCount);
        }

        return count;
    }
}
