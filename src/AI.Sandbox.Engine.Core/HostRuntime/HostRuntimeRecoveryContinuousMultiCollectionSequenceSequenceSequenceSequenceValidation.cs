namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority proving one bounded ordered collection of exact
/// continuous recovery multi-collection-sequence-sequence-sequence summary projections.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidation<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidation(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidationIdKind> validationId,
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion>[] multiCollectionSequenceSequenceSequenceSummaries,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>[] boundarySupersessions,
        long validatedTick,
        long revision)
    {
        ValidationId = validationId;
        MultiCollectionSequenceSequenceSequenceSummaries =
            global::System.Array.AsReadOnly(multiCollectionSequenceSequenceSequenceSummaries);
        BoundarySupersessions =
            global::System.Array.AsReadOnly(boundarySupersessions);
        ValidatedTick = validatedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned sequence-validation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidationIdKind> ValidationId { get; }

    /// <summary>Gets exact ordered continuous multi-collection-sequence-sequence-sequence summary authorities.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion>> MultiCollectionSequenceSequenceSequenceSummaries { get; }

    /// <summary>Gets every exact validated sequence boundary in chain order.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> BoundarySupersessions { get; }

    /// <summary>Gets the first exact multi-collection-sequence-sequence-sequence summary authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection<
        TRequest,
        TState,
        TCompletion> FirstSummary => MultiCollectionSequenceSequenceSequenceSummaries[0];

    /// <summary>Gets the last exact multi-collection-sequence-sequence-sequence summary authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection<
        TRequest,
        TState,
        TCompletion> LastSummary => MultiCollectionSequenceSequenceSequenceSummaries[^1];

    /// <summary>Gets the unchanged source multi-collection-sequence-sequence-sequence authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidation<
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

    /// <summary>Gets the first represented multi-collection-sequence-summary index.</summary>
    public int StartSequenceSequenceSummaryIndex => FirstSummary.StartSequenceSequenceSummaryIndex;

    /// <summary>Gets the last represented multi-collection-sequence-summary index.</summary>
    public int EndSequenceSequenceSummaryIndex => LastSummary.EndSequenceSequenceSummaryIndex;

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

    /// <summary>Gets the number of exact multi-collection-sequence-sequence-sequence summary authorities.</summary>
    public int MultiCollectionSequenceSequenceSequenceSummaryCount =>
        MultiCollectionSequenceSequenceSequenceSummaries.Count;

    /// <summary>Gets the number of represented multi-collection-sequence-sequence summaries.</summary>
    public int MultiCollectionSequenceSequenceCount => CountMultiCollectionSequenceSequences();

    /// <summary>Gets the number of represented multi-collection-sequence summaries.</summary>
    public int MultiCollectionSequenceCount => CountMultiCollectionSequences();

    /// <summary>Gets the number of represented multi-collection summaries.</summary>
    public int MultiCollectionCount => CountMultiCollections();

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

    /// <summary>Gets the number of represented checkpoints.</summary>
    public int CheckpointCount =>
        checked(EndCheckpointIndex - StartCheckpointIndex + 1);

    /// <summary>Gets the number of represented supersession edges.</summary>
    public int SupersessionCount => checked(CheckpointCount - 1);

    /// <summary>Gets whether the sequence starts at source sequence start.</summary>
    public bool StartsAtSourceSequenceStart =>
        FirstSummary.StartsAtSourceSequenceStart;

    /// <summary>Gets whether the sequence ends at source sequence end.</summary>
    public bool EndsAtSourceSequenceEnd =>
        LastSummary.EndsAtSourceSequenceEnd;

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

    /// <summary>Gets the external monotonic sequence-sequence-sequence-validation tick.</summary>
    public long ValidatedTick { get; }

    /// <summary>Gets the sequence-sequence-sequence-validation authority revision.</summary>
    public long Revision { get; }

    private int CountMultiCollectionSequenceSequences()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSequenceSequenceSequenceSummaries)
        {
            count = checked(count + summary.MultiCollectionSequenceSequenceCount);
        }

        return count;
    }

    private int CountMultiCollectionSequences()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSequenceSequenceSequenceSummaries)
        {
            count = checked(count + summary.MultiCollectionSequenceCount);
        }

        return count;
    }

    private int CountMultiCollections()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSequenceSequenceSequenceSummaries)
        {
            count = checked(count + summary.MultiCollectionCount);
        }

        return count;
    }

    private int CountCollectionPairs()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSequenceSequenceSequenceSummaries)
        {
            count = checked(count + summary.CollectionPairCount);
        }

        return count;
    }

    private int CountCollections()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSequenceSequenceSequenceSummaries)
        {
            count = checked(count + summary.CollectionCount);
        }

        return count;
    }

    private int CountSummaries()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSequenceSequenceSequenceSummaries)
        {
            count = checked(count + summary.SummaryCount);
        }

        return count;
    }

    private int CountSequences()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSequenceSequenceSequenceSummaries)
        {
            count = checked(count + summary.SequenceCount);
        }

        return count;
    }

    private int CountPairs()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSequenceSequenceSequenceSummaries)
        {
            count = checked(count + summary.PairCount);
        }

        return count;
    }

    private int CountWindows()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSequenceSequenceSequenceSummaries)
        {
            count = checked(count + summary.WindowCount);
        }

        return count;
    }
}
