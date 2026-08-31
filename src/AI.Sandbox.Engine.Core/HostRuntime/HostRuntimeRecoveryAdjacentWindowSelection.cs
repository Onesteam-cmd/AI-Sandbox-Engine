namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable advisory authority selecting one exact previous or next
/// bounded checkpoint window adjacent to a summarized recovery range.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryAdjacentWindowSelection<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentWindowSelection(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryAdjacentWindowSelectionIdKind> selectionId,
        HostRuntimeRecoveryCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        bool selectsPreviousWindow,
        int startCheckpointIndex,
        int checkpointCount,
        HostRuntimeRecoveryCheckpoint<TRequest> startCheckpoint,
        HostRuntimeRecoveryCheckpoint<TRequest> endCheckpoint,
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
        SelectsPreviousWindow = selectsPreviousWindow;
        StartCheckpointIndex = startCheckpointIndex;
        CheckpointCount = checkpointCount;
        StartCheckpoint = startCheckpoint;
        EndCheckpoint = endCheckpoint;
        IncomingSupersession = incomingSupersession;
        OutgoingSupersession = outgoingSupersession;
        SelectedTick = selectedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned adjacent-window selection ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryAdjacentWindowSelectionIdKind> SelectionId { get; }

    /// <summary>Gets unchanged checkpoint-range summary authority.</summary>
    public HostRuntimeRecoveryCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets whether the selection addresses the previous window.</summary>
    public bool SelectsPreviousWindow { get; }

    /// <summary>Gets whether the selection addresses the next window.</summary>
    public bool SelectsNextWindow => !SelectsPreviousWindow;

    /// <summary>Gets the first selected checkpoint index in the source chain.</summary>
    public int StartCheckpointIndex { get; }

    /// <summary>Gets the number of selected checkpoints.</summary>
    public int CheckpointCount { get; }

    /// <summary>Gets the last selected checkpoint index in the source chain.</summary>
    public int EndCheckpointIndex =>
        checked(StartCheckpointIndex + CheckpointCount - 1);

    /// <summary>Gets the exact first selected checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> StartCheckpoint { get; }

    /// <summary>Gets the exact last selected checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> EndCheckpoint { get; }

    /// <summary>Gets the exact supersession entering the selected window.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? IncomingSupersession { get; }

    /// <summary>Gets the exact supersession leaving the selected window.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? OutgoingSupersession { get; }

    /// <summary>Gets the first selected checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> StartCheckpointId =>
        StartCheckpoint.CheckpointId;

    /// <summary>Gets the last selected checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> EndCheckpointId =>
        EndCheckpoint.CheckpointId;

    /// <summary>Gets whether the selection starts at the chain root.</summary>
    public bool StartsAtRoot => StartCheckpointIndex == 0;

    /// <summary>Gets whether the selection ends at the latest checkpoint.</summary>
    public bool EndsAtLatest =>
        EndCheckpointIndex == Summary.SourceProjection.SupersessionCount;

    /// <summary>Gets the external monotonic adjacent-window selection tick.</summary>
    public long SelectedTick { get; }

    /// <summary>Gets the adjacent-window selection authority revision.</summary>
    public long Revision { get; }
}
