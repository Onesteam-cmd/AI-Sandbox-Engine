namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects compact recovery checkpoint-range summaries and selects exact
/// bounded adjacent windows without discovery, reordering, storage, indexing,
/// history mutation, archival, compaction, pagination, diagnostics, scheduling,
/// supervision, waiting, restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryCheckpointRangeSummaryFlow
{
    /// <summary>Maximum checkpoints represented by an adjacent-window selection.</summary>
    public const int MaximumAdjacentWindowCheckpointCount =
        HostRuntimeRecoveryLineageWindowFlow.MaximumCheckpointCount;

    /// <summary>Projects one compact immutable summary from an exact range query.</summary>
    public static HostRuntimeRecoveryCheckpointRangeSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectSummary<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointRangeSummaryProjectionIdKind> summaryId,
            HostRuntimeRecoveryCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion> range,
            long expectedRangeRevision,
            long projectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(summaryId.IsEmpty, nameof(summaryId));
        global::System.ArgumentNullException.ThrowIfNull(range);
        EnsureRevision(expectedRangeRevision, nameof(expectedRangeRevision));
        EnsureTick(projectedTick, nameof(projectedTick));

        if (range.Revision != expectedRangeRevision)
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryCheckpointRangeSummaryStatus
                    .StaleRangeRevision,
                range);
        }
        if (projectedTick < range.QueriedTick)
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryCheckpointRangeSummaryStatus
                    .RangeSummaryProjectionTickRegressed,
                range);
        }

        var summary =
            new HostRuntimeRecoveryCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion>(
                    summaryId,
                    range,
                    projectedTick,
                    checked(range.Revision + 1));

        return new HostRuntimeRecoveryCheckpointRangeSummaryProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryCheckpointRangeSummaryStatus
                    .CheckpointRangeSummaryProjected,
                range,
                summary);
    }

    /// <summary>Selects one exact bounded window immediately before a summarized range.</summary>
    public static HostRuntimeRecoveryAdjacentWindowSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectPreviousWindow<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentWindowSelectionIdKind> selectionId,
            HostRuntimeRecoveryCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int checkpointCount,
            long expectedSummaryRevision,
            long selectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        SelectAdjacentWindow(
            selectionId,
            summary,
            checkpointCount,
            expectedSummaryRevision,
            selectedTick,
            selectsPreviousWindow: true);

    /// <summary>Selects one exact bounded window immediately after a summarized range.</summary>
    public static HostRuntimeRecoveryAdjacentWindowSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectNextWindow<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentWindowSelectionIdKind> selectionId,
            HostRuntimeRecoveryCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int checkpointCount,
            long expectedSummaryRevision,
            long selectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        SelectAdjacentWindow(
            selectionId,
            summary,
            checkpointCount,
            expectedSummaryRevision,
            selectedTick,
            selectsPreviousWindow: false);

    private static HostRuntimeRecoveryAdjacentWindowSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectAdjacentWindow<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentWindowSelectionIdKind> selectionId,
            HostRuntimeRecoveryCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            int checkpointCount,
            long expectedSummaryRevision,
            long selectedTick,
            bool selectsPreviousWindow)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(selectionId.IsEmpty, nameof(selectionId));
        global::System.ArgumentNullException.ThrowIfNull(summary);
        EnsureCount(checkpointCount, nameof(checkpointCount));
        EnsureRevision(expectedSummaryRevision, nameof(expectedSummaryRevision));
        EnsureTick(selectedTick, nameof(selectedTick));

        if (summary.Revision != expectedSummaryRevision)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryCheckpointRangeSummaryStatus
                    .StaleSummaryRevision,
                summary);
        }
        if (selectedTick < summary.ProjectedTick)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryCheckpointRangeSummaryStatus
                    .AdjacentWindowSelectionTickRegressed,
                summary);
        }
        if (checkpointCount > MaximumAdjacentWindowCheckpointCount)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryCheckpointRangeSummaryStatus
                    .TooManyAdjacentWindowCheckpoints,
                summary);
        }

        int startCheckpointIndex;
        if (selectsPreviousWindow)
        {
            if (summary.StartChainIndex == 0)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryCheckpointRangeSummaryStatus
                        .NoPreviousAdjacentWindow,
                    summary);
            }

            var endCheckpointIndex = summary.StartChainIndex - 1;
            startCheckpointIndex =
                checked(endCheckpointIndex - checkpointCount + 1);
            if (startCheckpointIndex < 0)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryCheckpointRangeSummaryStatus
                        .PreviousAdjacentWindowTooShort,
                    summary);
            }
        }
        else
        {
            startCheckpointIndex = checked(summary.EndChainIndex + 1);
            if (startCheckpointIndex >= summary.SourceProjection.CheckpointCount)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryCheckpointRangeSummaryStatus
                        .NoNextAdjacentWindow,
                    summary);
            }

            var requestedEndIndex =
                (long)startCheckpointIndex + checkpointCount - 1L;
            if (requestedEndIndex >= summary.SourceProjection.CheckpointCount)
            {
                return SelectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryCheckpointRangeSummaryStatus
                        .NextAdjacentWindowTooShort,
                    summary);
            }
        }

        var chain = summary.Chain;
        var endIndex = checked(startCheckpointIndex + checkpointCount - 1);
        var startCheckpoint = CheckpointAt(chain, startCheckpointIndex);
        var endCheckpoint = CheckpointAt(chain, endIndex);
        var incomingSupersession =
            startCheckpointIndex > 0
                ? chain.Supersessions[startCheckpointIndex - 1]
                : null;
        var outgoingSupersession =
            endIndex < chain.SupersessionCount
                ? chain.Supersessions[endIndex]
                : null;

        var selection =
            new HostRuntimeRecoveryAdjacentWindowSelection<
                TRequest,
                TState,
                TCompletion>(
                    selectionId,
                    summary,
                    selectsPreviousWindow,
                    startCheckpointIndex,
                    checkpointCount,
                    startCheckpoint,
                    endCheckpoint,
                    incomingSupersession,
                    outgoingSupersession,
                    selectedTick,
                    checked(summary.Revision + 1));

        var status = selectsPreviousWindow
            ? HostRuntimeRecoveryCheckpointRangeSummaryStatus
                .PreviousAdjacentWindowSelected
            : HostRuntimeRecoveryCheckpointRangeSummaryStatus
                .NextAdjacentWindowSelected;

        return new HostRuntimeRecoveryAdjacentWindowSelectionResult<
            TRequest,
            TState,
            TCompletion>(status, summary, selection);
    }

    private static HostRuntimeRecoveryCheckpoint<TRequest>
        CheckpointAt<TRequest, TState, TCompletion>(
            HostRuntimeRecoverySupersessionChain<
                TRequest,
                TState,
                TCompletion> chain,
            int checkpointIndex)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        checkpointIndex == 0
            ? chain.RootCheckpoint
            : chain.Supersessions[checkpointIndex - 1]
                .SuccessorCheckpoint;

    private static HostRuntimeRecoveryCheckpointRangeSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> SummaryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryCheckpointRangeSummaryStatus status,
            HostRuntimeRecoveryCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion> range)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, range, summary: null);

    private static HostRuntimeRecoveryAdjacentWindowSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryCheckpointRangeSummaryStatus status,
            HostRuntimeRecoveryCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, summary, selection: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery checkpoint-range summary identifiers cannot be empty.",
                parameterName);
        }
    }

    private static void EnsureCount(int count, string parameterName)
    {
        if (count <= 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                count,
                "Recovery adjacent-window counts must be positive.");
        }
    }

    private static void EnsureRevision(long revision, string parameterName)
    {
        if (revision < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                revision,
                "Recovery checkpoint-range summary revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery checkpoint-range summary ticks cannot be negative.");
        }
    }
}
