namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable advisory authority selecting one exact previous or next
/// bounded multi-collection of continuous multi-collection summaries adjacent to a summarized range.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryAdjacentMultiCollectionSelection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentMultiCollectionSelection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryAdjacentMultiCollectionSelectionIdKind> selectionId,
        HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        bool selectsPreviousMultiCollection,
        int startSummaryIndex,
        HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
            TRequest,
            TState,
            TCompletion>[] multiCollectionSummaries,
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
        SelectsPreviousMultiCollection = selectsPreviousMultiCollection;
        StartSummaryIndex = startSummaryIndex;
        MultiCollectionSummaries =
            global::System.Array.AsReadOnly(multiCollectionSummaries);
        BoundarySupersessions =
            global::System.Array.AsReadOnly(boundarySupersessions);
        AdjacentBoundarySupersession = adjacentBoundarySupersession;
        IncomingSupersession = incomingSupersession;
        OutgoingSupersession = outgoingSupersession;
        SelectedTick = selectedTick;
        Revision = revision;
    }

    /// <summary>Gets externally assigned adjacent multi-collection selection ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryAdjacentMultiCollectionSelectionIdKind> SelectionId { get; }

    /// <summary>Gets unchanged multi-collection-sequence range summary authority.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets unchanged source multi-collection-sequence validation authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceValidation<
        TRequest,
        TState,
        TCompletion> SourceSequence => Summary.Sequence;

    /// <summary>Gets whether the selection addresses the previous multi-collection.</summary>
    public bool SelectsPreviousMultiCollection { get; }

    /// <summary>Gets whether the selection addresses the next multi-collection.</summary>
    public bool SelectsNextMultiCollection => !SelectsPreviousMultiCollection;

    /// <summary>Gets exact selected continuous multi-collection summaries in source order.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
            TRequest,
            TState,
            TCompletion>> MultiCollectionSummaries { get; }

    /// <summary>Gets exact internal selected multi-collection boundary authorities.</summary>
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
    public HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
        TRequest,
        TState,
        TCompletion> FirstSummary => MultiCollectionSummaries[0];

    /// <summary>Gets exact last selected summary authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
        TRequest,
        TState,
        TCompletion> LastSummary => MultiCollectionSummaries[^1];

    /// <summary>Gets first selected multi-collection-summary index in source sequence.</summary>
    public int StartSummaryIndex { get; }

    /// <summary>Gets last selected multi-collection-summary index in source sequence.</summary>
    public int EndSummaryIndex => checked(StartSummaryIndex + MultiCollectionCount - 1);

    /// <summary>Gets selected multi-collection summary count.</summary>
    public int MultiCollectionCount => MultiCollectionSummaries.Count;

    /// <summary>Gets represented collection-pair summary count.</summary>
    public int CollectionPairCount =>
        checked(EndCollectionPairIndex - StartCollectionPairIndex + 1);

    /// <summary>Gets represented collection count.</summary>
    public int CollectionCount => checked(CollectionPairCount * 2);

    /// <summary>Gets represented multi-sequence summary count.</summary>
    public int SummaryCount => CountSummaries();

    /// <summary>Gets represented sequence count.</summary>
    public int SequenceCount => CountSequences();

    /// <summary>Gets represented pair-summary count.</summary>
    public int PairCount => CountPairs();

    /// <summary>Gets represented recovery-window count.</summary>
    public int WindowCount => CountWindows();

    /// <summary>Gets first selected collection-pair index.</summary>
    public int StartCollectionPairIndex => FirstSummary.StartCollectionPairIndex;

    /// <summary>Gets last selected collection-pair index.</summary>
    public int EndCollectionPairIndex => LastSummary.EndCollectionPairIndex;

    /// <summary>Gets first selected checkpoint index in source chain.</summary>
    public int StartCheckpointIndex => FirstSummary.StartCheckpointIndex;

    /// <summary>Gets last selected checkpoint index in source chain.</summary>
    public int EndCheckpointIndex => LastSummary.EndCheckpointIndex;

    /// <summary>Gets represented checkpoint count.</summary>
    public int CheckpointCount =>
        checked(EndCheckpointIndex - StartCheckpointIndex + 1);

    /// <summary>Gets represented supersession count.</summary>
    public int SupersessionCount => checked(CheckpointCount - 1);

    /// <summary>Gets exact first selected checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint =>
        FirstSummary.StartCheckpoint;

    /// <summary>Gets exact last selected checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint =>
        LastSummary.EndCheckpoint;

    /// <summary>Gets exact supersession entering selected collection.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? IncomingSupersession { get; }

    /// <summary>Gets exact supersession leaving selected collection.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? OutgoingSupersession { get; }

    /// <summary>Gets whether selection starts at source multi-collection sequence start.</summary>
    public bool StartsAtSourceSequenceStart => StartSummaryIndex == 0;

    /// <summary>Gets whether selection ends at source multi-collection sequence end.</summary>
    public bool EndsAtSourceSequenceEnd =>
        EndSummaryIndex == SourceSequence.MultiCollectionCount - 1;

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

    /// <summary>Gets external monotonic selection tick.</summary>
    public long SelectedTick { get; }

    /// <summary>Gets adjacent multi-collection selection authority revision.</summary>
    public long Revision { get; }
}
