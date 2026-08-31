namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable advisory authority selecting one exact previous or next
/// bounded pair-summary sequence adjacent to a summarized multi-window range.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryAdjacentSequenceSelection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentSequenceSelection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryAdjacentSequenceSelectionIdKind> selectionId,
        HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        bool selectsPreviousSequence,
        int startPairIndex,
        HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
            TRequest,
            TState,
            TCompletion>[] pairSummaries,
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
        SelectsPreviousSequence = selectsPreviousSequence;
        StartPairIndex = startPairIndex;
        PairSummaries = global::System.Array.AsReadOnly(pairSummaries);
        BoundarySupersessions = global::System.Array.AsReadOnly(boundarySupersessions);
        AdjacentBoundarySupersession = adjacentBoundarySupersession;
        IncomingSupersession = incomingSupersession;
        OutgoingSupersession = outgoingSupersession;
        SelectedTick = selectedTick;
        Revision = revision;
    }

    /// <summary>Gets externally assigned adjacent-sequence selection ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryAdjacentSequenceSelectionIdKind> SelectionId { get; }

    /// <summary>Gets unchanged multi-window range summary authority.</summary>
    public HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets unchanged source sequence-validation authority.</summary>
    public HostRuntimeRecoveryContinuousWindowSequenceValidation<
        TRequest,
        TState,
        TCompletion> SourceSequence => Summary.Sequence;

    /// <summary>Gets whether the selection addresses the previous sequence.</summary>
    public bool SelectsPreviousSequence { get; }

    /// <summary>Gets whether the selection addresses the next sequence.</summary>
    public bool SelectsNextSequence => !SelectsPreviousSequence;

    /// <summary>Gets exact selected pair-summary authorities in source order.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
            TRequest,
            TState,
            TCompletion>> PairSummaries { get; }

    /// <summary>Gets exact internal selected-sequence boundary authorities.</summary>
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

    /// <summary>Gets exact first selected pair authority.</summary>
    public HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
        TRequest,
        TState,
        TCompletion> FirstPair => PairSummaries[0];

    /// <summary>Gets exact last selected pair authority.</summary>
    public HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
        TRequest,
        TState,
        TCompletion> LastPair => PairSummaries[^1];

    /// <summary>Gets first selected pair index in source sequence.</summary>
    public int StartPairIndex { get; }

    /// <summary>Gets last selected pair index in source sequence.</summary>
    public int EndPairIndex => checked(StartPairIndex + PairCount - 1);

    /// <summary>Gets selected pair count.</summary>
    public int PairCount => PairSummaries.Count;

    /// <summary>Gets selected window count.</summary>
    public int WindowCount => checked(PairCount * 2);

    /// <summary>Gets first selected checkpoint index in source chain.</summary>
    public int StartCheckpointIndex => FirstPair.StartCheckpointIndex;

    /// <summary>Gets last selected checkpoint index in source chain.</summary>
    public int EndCheckpointIndex => LastPair.EndCheckpointIndex;

    /// <summary>Gets represented checkpoint count.</summary>
    public int CheckpointCount => checked(EndCheckpointIndex - StartCheckpointIndex + 1);

    /// <summary>Gets represented supersession count.</summary>
    public int SupersessionCount => checked(CheckpointCount - 1);

    /// <summary>Gets exact first selected checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint =>
        FirstPair.StartCheckpoint;

    /// <summary>Gets exact last selected checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint =>
        LastPair.EndCheckpoint;

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
    public bool StartsAtSourceSequenceStart => StartPairIndex == 0;

    /// <summary>Gets whether selection ends at source sequence end.</summary>
    public bool EndsAtSourceSequenceEnd =>
        EndPairIndex == SourceSequence.PairCount - 1;

    /// <summary>Gets external monotonic selection tick.</summary>
    public long SelectedTick { get; }

    /// <summary>Gets adjacent-sequence selection authority revision.</summary>
    public long Revision { get; }
}
