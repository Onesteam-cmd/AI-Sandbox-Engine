namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable advisory authority selecting one exact previous or next
/// bounded collection-sequence of exact collection-pair summaries adjacent to a summarized multi-collection range.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryAdjacentCollectionSequenceSelection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentCollectionSequenceSelection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryAdjacentCollectionSequenceSelectionIdKind> selectionId,
        HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        bool selectsPreviousCollectionSequence,
        int startCollectionPairIndex,
        HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
            TRequest,
            TState,
            TCompletion>[] collectionPairSummaries,
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
        SelectsPreviousCollectionSequence = selectsPreviousCollectionSequence;
        StartCollectionPairIndex = startCollectionPairIndex;
        CollectionPairSummaries = global::System.Array.AsReadOnly(collectionPairSummaries);
        BoundarySupersessions = global::System.Array.AsReadOnly(boundarySupersessions);
        AdjacentBoundarySupersession = adjacentBoundarySupersession;
        IncomingSupersession = incomingSupersession;
        OutgoingSupersession = outgoingSupersession;
        SelectedTick = selectedTick;
        Revision = revision;
    }

    /// <summary>Gets externally assigned adjacent collection-sequence selection ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryAdjacentCollectionSequenceSelectionIdKind> SelectionId { get; }

    /// <summary>Gets unchanged multi-collection range summary authority.</summary>
    public HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets unchanged source sequence-validation authority.</summary>
    public HostRuntimeRecoveryContinuousCollectionSequenceValidation<
        TRequest,
        TState,
        TCompletion> SourceSequence => Summary.Sequence;

    /// <summary>Gets whether the selection addresses the previous collection sequence.</summary>
    public bool SelectsPreviousCollectionSequence { get; }

    /// <summary>Gets whether the selection addresses the next collection sequence.</summary>
    public bool SelectsNextCollectionSequence => !SelectsPreviousCollectionSequence;

    /// <summary>Gets exact selected collection-pair summary authorities in source order.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
            TRequest,
            TState,
            TCompletion>> CollectionPairSummaries { get; }

    /// <summary>Gets exact internal selected collection-sequence boundary authorities.</summary>
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

    /// <summary>Gets exact first selected collection-pair authority.</summary>
    public HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
        TRequest,
        TState,
        TCompletion> FirstCollectionPair => CollectionPairSummaries[0];

    /// <summary>Gets exact last selected collection-pair authority.</summary>
    public HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
        TRequest,
        TState,
        TCompletion> LastCollectionPair => CollectionPairSummaries[^1];

    /// <summary>Gets first selected collection-pair index in source sequence.</summary>
    public int StartCollectionPairIndex { get; }

    /// <summary>Gets last selected collection-pair index in source sequence.</summary>
    public int EndCollectionPairIndex => checked(StartCollectionPairIndex + CollectionPairCount - 1);

    /// <summary>Gets selected collection-pair count.</summary>
    public int CollectionPairCount => CollectionPairSummaries.Count;

    /// <summary>Gets selected collection count.</summary>
    public int CollectionCount => checked(CollectionPairCount * 2);

    /// <summary>Gets first selected source-collection summary index.</summary>
    public int StartSummaryIndex => FirstCollectionPair.StartSummaryIndex;

    /// <summary>Gets last selected source-collection summary index.</summary>
    public int EndSummaryIndex => LastCollectionPair.EndSummaryIndex;

    /// <summary>Gets represented multi-sequence summary count.</summary>
    public int SummaryCount => CountSummaries();

    /// <summary>Gets represented sequence count.</summary>
    public int SequenceCount => CountSequences();

    /// <summary>Gets represented pair-summary count.</summary>
    public int PairCount => CountPairs();

    /// <summary>Gets represented recovery-window count.</summary>
    public int WindowCount => CountWindows();

    /// <summary>Gets first selected checkpoint index in source chain.</summary>
    public int StartCheckpointIndex => FirstCollectionPair.StartCheckpointIndex;

    /// <summary>Gets last selected checkpoint index in source chain.</summary>
    public int EndCheckpointIndex => LastCollectionPair.EndCheckpointIndex;

    /// <summary>Gets represented checkpoint count.</summary>
    public int CheckpointCount => checked(EndCheckpointIndex - StartCheckpointIndex + 1);

    /// <summary>Gets represented supersession count.</summary>
    public int SupersessionCount => checked(CheckpointCount - 1);

    /// <summary>Gets exact first selected checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint =>
        FirstCollectionPair.StartCheckpoint;

    /// <summary>Gets exact last selected checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint =>
        LastCollectionPair.EndCheckpoint;

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

    /// <summary>Gets whether selection starts at source collection-sequence start.</summary>
    public bool StartsAtSourceSequenceStart => StartCollectionPairIndex == 0;

    /// <summary>Gets whether selection ends at source collection-sequence end.</summary>
    public bool EndsAtSourceSequenceEnd =>
        EndCollectionPairIndex == SourceSequence.CollectionPairCount - 1;

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

    /// <summary>Gets external monotonic selection tick.</summary>
    public long SelectedTick { get; }

    /// <summary>Gets adjacent collection-sequence selection authority revision.</summary>
    public long Revision { get; }
}
