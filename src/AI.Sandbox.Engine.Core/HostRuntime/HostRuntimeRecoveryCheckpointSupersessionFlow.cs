namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Creates pure recovery checkpoint-supersession and completed-cycle summary
/// authority without storage, deletion, archival, compaction, scheduling,
/// supervision, waiting, or execution.
/// </summary>
public static class HostRuntimeRecoveryCheckpointSupersessionFlow
{
    /// <summary>
    /// Links one exact completed recovery cycle to one successor checkpoint.
    /// </summary>
    /// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
    /// <typeparam name="TState">Exact immutable World State root type.</typeparam>
    /// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
    /// <param name="supersessionId">
    /// Externally assigned checkpoint-supersession ID.
    /// </param>
    /// <param name="cycleCompletion">
    /// Existing immutable completed recovery-cycle authority.
    /// </param>
    /// <param name="expectedCycleCompletionRevision">
    /// Recovery-cycle completion revision observed by the caller.
    /// </param>
    /// <param name="successorCheckpoint">
    /// Existing immutable successor recovery checkpoint.
    /// </param>
    /// <param name="expectedSuccessorCheckpointRevision">
    /// Successor-checkpoint revision observed by the caller.
    /// </param>
    /// <param name="supersededTick">
    /// External monotonic checkpoint-supersession tick.
    /// </param>
    /// <returns>An explicit immutable checkpoint-supersession result.</returns>
    public static HostRuntimeRecoveryCheckpointSupersessionResult<
        TRequest,
        TState,
        TCompletion> Supersede<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointSupersessionIdKind>
                    supersessionId,
            HostRuntimeRecoveryCycleCompletion<TRequest, TState, TCompletion>
                cycleCompletion,
            long expectedCycleCompletionRevision,
            HostRuntimeRecoveryCheckpoint<TRequest> successorCheckpoint,
            long expectedSuccessorCheckpointRevision,
            long supersededTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(supersessionId.IsEmpty, nameof(supersessionId));
        ArgumentNullException.ThrowIfNull(cycleCompletion);
        EnsureRevision(
            expectedCycleCompletionRevision,
            nameof(expectedCycleCompletionRevision));
        ArgumentNullException.ThrowIfNull(successorCheckpoint);
        EnsureRevision(
            expectedSuccessorCheckpointRevision,
            nameof(expectedSuccessorCheckpointRevision));
        EnsureTick(supersededTick, nameof(supersededTick));

        if (cycleCompletion.Revision != expectedCycleCompletionRevision)
        {
            return Unchanged(
                HostRuntimeRecoveryCheckpointSupersessionStatus
                    .StaleCycleCompletionRevision,
                cycleCompletion,
                successorCheckpoint);
        }
        if (successorCheckpoint.Revision !=
            expectedSuccessorCheckpointRevision)
        {
            return Unchanged(
                HostRuntimeRecoveryCheckpointSupersessionStatus
                    .StaleSuccessorCheckpointRevision,
                cycleCompletion,
                successorCheckpoint);
        }

        var priorCheckpoint = cycleCompletion.Checkpoint;
        if (successorCheckpoint.CheckpointId == priorCheckpoint.CheckpointId)
        {
            return Unchanged(
                HostRuntimeRecoveryCheckpointSupersessionStatus
                    .CheckpointIdReused,
                cycleCompletion,
                successorCheckpoint);
        }
        if (successorCheckpoint.RuntimeInstanceId !=
            priorCheckpoint.RuntimeInstanceId)
        {
            return Unchanged(
                HostRuntimeRecoveryCheckpointSupersessionStatus.RuntimeMismatch,
                cycleCompletion,
                successorCheckpoint);
        }
        if (successorCheckpoint.Composition.CompositionId !=
            priorCheckpoint.Composition.CompositionId)
        {
            return Unchanged(
                HostRuntimeRecoveryCheckpointSupersessionStatus
                    .CompositionMismatch,
                cycleCompletion,
                successorCheckpoint);
        }
        if (successorCheckpoint.QueueSnapshot.QueueId !=
            priorCheckpoint.QueueSnapshot.QueueId)
        {
            return Unchanged(
                HostRuntimeRecoveryCheckpointSupersessionStatus.QueueMismatch,
                cycleCompletion,
                successorCheckpoint);
        }
        if (successorCheckpoint.ClockId != priorCheckpoint.ClockId)
        {
            return Unchanged(
                HostRuntimeRecoveryCheckpointSupersessionStatus.ClockMismatch,
                cycleCompletion,
                successorCheckpoint);
        }
        if (successorCheckpoint.WorldSnapshotDocument.WorldId !=
            priorCheckpoint.WorldSnapshotDocument.WorldId)
        {
            return Unchanged(
                HostRuntimeRecoveryCheckpointSupersessionStatus.WorldMismatch,
                cycleCompletion,
                successorCheckpoint);
        }
        if (successorCheckpoint.CapturedTick < cycleCompletion.CompletedTick)
        {
            return Unchanged(
                HostRuntimeRecoveryCheckpointSupersessionStatus
                    .SuccessorCheckpointTickRegressed,
                cycleCompletion,
                successorCheckpoint);
        }
        if (successorCheckpoint.Revision <= priorCheckpoint.Revision)
        {
            return Unchanged(
                HostRuntimeRecoveryCheckpointSupersessionStatus
                    .SuccessorCheckpointRevisionNotAdvanced,
                cycleCompletion,
                successorCheckpoint);
        }
        if (successorCheckpoint.WorldSnapshotDocument.WorldStateVersion
                .CompareTo(
                    priorCheckpoint.WorldSnapshotDocument.WorldStateVersion) < 0)
        {
            return Unchanged(
                HostRuntimeRecoveryCheckpointSupersessionStatus
                    .WorldStateVersionRegressed,
                cycleCompletion,
                successorCheckpoint);
        }
        if (successorCheckpoint.WorldSnapshotDocument.SimulationTick <
            priorCheckpoint.WorldSnapshotDocument.SimulationTick)
        {
            return Unchanged(
                HostRuntimeRecoveryCheckpointSupersessionStatus
                    .SimulationTickRegressed,
                cycleCompletion,
                successorCheckpoint);
        }
        if (supersededTick < successorCheckpoint.CapturedTick)
        {
            return Unchanged(
                HostRuntimeRecoveryCheckpointSupersessionStatus
                    .SupersessionTickRegressed,
                cycleCompletion,
                successorCheckpoint);
        }

        var supersession =
            new HostRuntimeRecoveryCheckpointSupersession<
                TRequest,
                TState,
                TCompletion>(
                    supersessionId,
                    cycleCompletion,
                    successorCheckpoint,
                    supersededTick,
                    checked(
                        global::System.Math.Max(
                            cycleCompletion.Revision,
                            successorCheckpoint.Revision) + 1));

        return new HostRuntimeRecoveryCheckpointSupersessionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryCheckpointSupersessionStatus
                    .CheckpointSuperseded,
                cycleCompletion,
                successorCheckpoint,
                supersession);
    }

    /// <summary>
    /// Projects one successful checkpoint supersession into a compact immutable
    /// completed-cycle summary.
    /// </summary>
    /// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
    /// <typeparam name="TState">Exact immutable World State root type.</typeparam>
    /// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
    /// <param name="summaryId">Externally assigned completed-cycle summary ID.</param>
    /// <param name="supersession">
    /// Existing immutable checkpoint-supersession authority.
    /// </param>
    /// <param name="expectedSupersessionRevision">
    /// Checkpoint-supersession revision observed by the caller.
    /// </param>
    /// <param name="summarizedTick">External monotonic summary tick.</param>
    /// <returns>An explicit immutable completed-cycle summary result.</returns>
    public static HostRuntimeRecoveryCompletedCycleSummaryResult<
        TRequest,
        TState,
        TCompletion> Summarize<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCompletedCycleSummaryIdKind> summaryId,
            HostRuntimeRecoveryCheckpointSupersession<
                TRequest,
                TState,
                TCompletion> supersession,
            long expectedSupersessionRevision,
            long summarizedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(summaryId.IsEmpty, nameof(summaryId));
        ArgumentNullException.ThrowIfNull(supersession);
        EnsureRevision(
            expectedSupersessionRevision,
            nameof(expectedSupersessionRevision));
        EnsureTick(summarizedTick, nameof(summarizedTick));

        if (supersession.Revision != expectedSupersessionRevision)
        {
            return SummaryResult(
                HostRuntimeRecoveryCheckpointSupersessionStatus
                    .StaleSupersessionRevision,
                supersession);
        }
        if (summarizedTick < supersession.SupersededTick)
        {
            return SummaryResult(
                HostRuntimeRecoveryCheckpointSupersessionStatus
                    .SummaryTickRegressed,
                supersession);
        }

        var summary =
            new HostRuntimeRecoveryCompletedCycleSummary<
                TRequest,
                TState,
                TCompletion>(
                    summaryId,
                    supersession,
                    summarizedTick,
                    checked(supersession.Revision + 1));

        return new HostRuntimeRecoveryCompletedCycleSummaryResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryCheckpointSupersessionStatus.SummaryCreated,
                supersession,
                summary);
    }

    private static HostRuntimeRecoveryCheckpointSupersessionResult<
        TRequest,
        TState,
        TCompletion> Unchanged<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryCheckpointSupersessionStatus status,
            HostRuntimeRecoveryCycleCompletion<TRequest, TState, TCompletion>
                cycleCompletion,
            HostRuntimeRecoveryCheckpoint<TRequest> successorCheckpoint)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, cycleCompletion, successorCheckpoint, supersession: null);

    private static HostRuntimeRecoveryCompletedCycleSummaryResult<
        TRequest,
        TState,
        TCompletion> SummaryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryCheckpointSupersessionStatus status,
            HostRuntimeRecoveryCheckpointSupersession<
                TRequest,
                TState,
                TCompletion> supersession)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, supersession, summary: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new ArgumentException(
                "The identifier must be initialized.",
                parameterName);
        }
    }

    private static void EnsureRevision(long revision, string parameterName)
    {
        if (revision < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}
