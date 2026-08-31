namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable advisory authority selecting one exact previous or next
/// bounded collection of multi-sequence summaries adjacent to a summarized range.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryAdjacentCollectionSelection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentCollectionSelection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryAdjacentCollectionSelectionIdKind> selectionId,
        HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        bool selectsPreviousCollection,
        int startSummaryIndex,
        HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion>[] multiSequenceSummaries,
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
        SelectsPreviousCollection = selectsPreviousCollection;
        StartSummaryIndex = startSummaryIndex;
        MultiSequenceSummaries =
            global::System.Array.AsReadOnly(multiSequenceSummaries);
        BoundarySupersessions =
            global::System.Array.AsReadOnly(boundarySupersessions);
        AdjacentBoundarySupersession = adjacentBoundarySupersession;
        IncomingSupersession = incomingSupersession;
        OutgoingSupersession = outgoingSupersession;
        SelectedTick = selectedTick;
        Revision = revision;
    }

    /// <summary>Gets externally assigned adjacent-collection selection ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryAdjacentCollectionSelectionIdKind> SelectionId { get; }

    /// <summary>Gets unchanged multi-sequence range summary authority.</summary>
    public HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets unchanged source collection-validation authority.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
        TRequest,
        TState,
        TCompletion> SourceCollection => Summary.Collection;

    /// <summary>Gets whether the selection addresses the previous collection.</summary>
    public bool SelectsPreviousCollection { get; }

    /// <summary>Gets whether the selection addresses the next collection.</summary>
    public bool SelectsNextCollection => !SelectsPreviousCollection;

    /// <summary>Gets exact selected multi-sequence summaries in source order.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion>> MultiSequenceSummaries { get; }

    /// <summary>Gets exact internal selected-collection boundary authorities.</summary>
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
    public HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
        TRequest,
        TState,
        TCompletion> FirstSummary => MultiSequenceSummaries[0];

    /// <summary>Gets exact last selected summary authority.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
        TRequest,
        TState,
        TCompletion> LastSummary => MultiSequenceSummaries[^1];

    /// <summary>Gets first selected summary index in source collection.</summary>
    public int StartSummaryIndex { get; }

    /// <summary>Gets last selected summary index in source collection.</summary>
    public int EndSummaryIndex => checked(StartSummaryIndex + SummaryCount - 1);

    /// <summary>Gets selected summary count.</summary>
    public int SummaryCount => MultiSequenceSummaries.Count;

    /// <summary>Gets selected sequence count.</summary>
    public int SequenceCount => checked(SummaryCount * 2);

    /// <summary>Gets first selected pair index in source chain projection.</summary>
    public int StartPairIndex => FirstSummary.StartPairIndex;

    /// <summary>Gets last selected pair index in source chain projection.</summary>
    public int EndPairIndex => LastSummary.EndPairIndex;

    /// <summary>Gets selected pair count.</summary>
    public int PairCount => checked(EndPairIndex - StartPairIndex + 1);

    /// <summary>Gets selected window count.</summary>
    public int WindowCount => checked(PairCount * 2);

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

    /// <summary>Gets whether selection starts at source collection start.</summary>
    public bool StartsAtSourceCollectionStart => StartSummaryIndex == 0;

    /// <summary>Gets whether selection ends at source collection end.</summary>
    public bool EndsAtSourceCollectionEnd =>
        EndSummaryIndex == SourceCollection.SummaryCount - 1;

    /// <summary>Gets external monotonic selection tick.</summary>
    public long SelectedTick { get; }

    /// <summary>Gets adjacent-collection selection authority revision.</summary>
    public long Revision { get; }
}
