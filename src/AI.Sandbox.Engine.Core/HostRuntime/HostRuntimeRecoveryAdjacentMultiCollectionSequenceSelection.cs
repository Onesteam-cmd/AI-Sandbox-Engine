namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable advisory authority selecting one exact previous or next bounded
/// multi-collection-sequence of continuous multi-collection-sequence summaries adjacent
/// to one summarized range.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionIdKind> selectionId,
        HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        bool selectsPreviousMultiCollectionSequence,
        int startSequenceSummaryIndex,
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion>[] multiCollectionSequenceSummaries,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>[] boundarySupersessions,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion> adjacentBoundarySupersession,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>? incomingSupersession,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>? outgoingSupersession,
        long selectedTick,
        long revision)
    {
        SelectionId = selectionId;
        Summary = summary;
        SelectsPreviousMultiCollectionSequence = selectsPreviousMultiCollectionSequence;
        StartSequenceSummaryIndex = startSequenceSummaryIndex;
        MultiCollectionSequenceSummaries =
            global::System.Array.AsReadOnly(multiCollectionSequenceSummaries);
        BoundarySupersessions =
            global::System.Array.AsReadOnly(boundarySupersessions);
        AdjacentBoundarySupersession = adjacentBoundarySupersession;
        IncomingSupersession = incomingSupersession;
        OutgoingSupersession = outgoingSupersession;
        SelectedTick = selectedTick;
        Revision = revision;
    }

    /// <summary>Gets externally assigned adjacent multi-collection-sequence selection ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionIdKind> SelectionId { get; }

    /// <summary>Gets unchanged range-summary authority.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets unchanged source sequence-validation authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceValidation<
        TRequest,
        TState,
        TCompletion> SourceSequence => Summary.Sequence;

    /// <summary>Gets whether the selection addresses the previous multi-collection-sequence.</summary>
    public bool SelectsPreviousMultiCollectionSequence { get; }

    /// <summary>Gets whether the selection addresses the next multi-collection-sequence.</summary>
    public bool SelectsNextMultiCollectionSequence => !SelectsPreviousMultiCollectionSequence;

    /// <summary>Gets exact selected multi-collection-sequence summaries in source order.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion>> MultiCollectionSequenceSummaries { get; }

    /// <summary>Gets exact internal selected boundary authorities.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> BoundarySupersessions { get; }

    /// <summary>Gets exact supersession connecting selection and source range.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion> AdjacentBoundarySupersession { get; }

    /// <summary>Gets exact first selected summary authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection<
        TRequest,
        TState,
        TCompletion> FirstSummary => MultiCollectionSequenceSummaries[0];

    /// <summary>Gets exact last selected summary authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection<
        TRequest,
        TState,
        TCompletion> LastSummary => MultiCollectionSequenceSummaries[^1];

    /// <summary>Gets first selected summary position in source validation sequence.</summary>
    public int StartSequenceSummaryIndex { get; }

    /// <summary>Gets last selected summary position in source validation sequence.</summary>
    public int EndSequenceSummaryIndex =>
        checked(StartSequenceSummaryIndex + MultiCollectionSequenceCount - 1);

    /// <summary>Gets selected multi-collection-sequence count.</summary>
    public int MultiCollectionSequenceCount => MultiCollectionSequenceSummaries.Count;

    /// <summary>Gets first represented multi-collection-summary index.</summary>
    public int StartSummaryIndex => FirstSummary.StartSummaryIndex;

    /// <summary>Gets last represented multi-collection-summary index.</summary>
    public int EndSummaryIndex => LastSummary.EndSummaryIndex;

    /// <summary>Gets represented multi-collection count.</summary>
    public int MultiCollectionCount => CountMultiCollections();

    /// <summary>Gets represented collection-pair count.</summary>
    public int CollectionPairCount => CountCollectionPairs();

    /// <summary>Gets represented collection count.</summary>
    public int CollectionCount => CountCollections();

    /// <summary>Gets represented summary count.</summary>
    public int SummaryCount => CountSummaries();

    /// <summary>Gets represented sequence count.</summary>
    public int SequenceCount => CountSequences();

    /// <summary>Gets represented pair count.</summary>
    public int PairCount => CountPairs();

    /// <summary>Gets represented recovery-window count.</summary>
    public int WindowCount => CountWindows();

    /// <summary>Gets first selected checkpoint index in source chain.</summary>
    public int StartCheckpointIndex => FirstSummary.StartCheckpointIndex;

    /// <summary>Gets last selected checkpoint index in source chain.</summary>
    public int EndCheckpointIndex => LastSummary.EndCheckpointIndex;

    /// <summary>Gets represented checkpoint count.</summary>
    public int CheckpointCount => checked(EndCheckpointIndex - StartCheckpointIndex + 1);

    /// <summary>Gets represented supersession count.</summary>
    public int SupersessionCount => checked(CheckpointCount - 1);

    /// <summary>Gets exact first selected checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint => FirstSummary.StartCheckpoint;

    /// <summary>Gets exact last selected checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint => LastSummary.EndCheckpoint;

    /// <summary>Gets exact supersession entering selected sequence.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? IncomingSupersession { get; }

    /// <summary>Gets exact supersession leaving selected sequence.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? OutgoingSupersession { get; }

    /// <summary>Gets whether selection starts at source sequence start.</summary>
    public bool StartsAtSourceSequenceStart => StartSequenceSummaryIndex == 0;

    /// <summary>Gets whether selection ends at source sequence end.</summary>
    public bool EndsAtSourceSequenceEnd =>
        EndSequenceSummaryIndex == SourceSequence.MultiCollectionSequenceSummaryCount - 1;

    /// <summary>Gets external monotonic selection tick.</summary>
    public long SelectedTick { get; }

    /// <summary>Gets adjacent selection authority revision.</summary>
    public long Revision { get; }

    private int CountMultiCollections()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSequenceSummaries)
        {
            count = checked(count + summary.MultiCollectionCount);
        }
        return count;
    }

    private int CountCollectionPairs()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSequenceSummaries)
        {
            count = checked(count + summary.CollectionPairCount);
        }
        return count;
    }

    private int CountCollections()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSequenceSummaries)
        {
            count = checked(count + summary.CollectionCount);
        }
        return count;
    }

    private int CountSummaries()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSequenceSummaries)
        {
            count = checked(count + summary.SummaryCount);
        }
        return count;
    }

    private int CountSequences()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSequenceSummaries)
        {
            count = checked(count + summary.SequenceCount);
        }
        return count;
    }

    private int CountPairs()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSequenceSummaries)
        {
            count = checked(count + summary.PairCount);
        }
        return count;
    }

    private int CountWindows()
    {
        var count = 0;
        foreach (var summary in MultiCollectionSequenceSummaries)
        {
            count = checked(count + summary.WindowCount);
        }
        return count;
    }
}
