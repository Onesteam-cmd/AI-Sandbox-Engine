namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects exact selected recovery windows and validates their checkpoint-range
/// continuity without discovery, reordering, storage, indexing, history mutation,
/// archival, compaction, pagination, diagnostics, scheduling, supervision,
/// waiting, restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryAdjacentWindowFlow
{
    /// <summary>Maximum checkpoints represented by one projected adjacent window.</summary>
    public const int MaximumCheckpointCount =
        HostRuntimeRecoveryLineageWindowFlow.MaximumCheckpointCount;

    /// <summary>Projects the exact bounded window represented by one selection.</summary>
    public static HostRuntimeRecoveryAdjacentWindowProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectWindow<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentWindowProjectionIdKind> projectionId,
            HostRuntimeRecoveryAdjacentWindowSelection<
                TRequest,
                TState,
                TCompletion> selection,
            long expectedSelectionRevision,
            long projectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(projectionId.IsEmpty, nameof(projectionId));
        global::System.ArgumentNullException.ThrowIfNull(selection);
        EnsureRevision(expectedSelectionRevision, nameof(expectedSelectionRevision));
        EnsureTick(projectedTick, nameof(projectedTick));

        if (selection.Revision != expectedSelectionRevision)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentWindowProjectionStatus
                    .StaleSelectionRevision,
                selection);
        }
        if (projectedTick < selection.SelectedTick)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentWindowProjectionStatus
                    .AdjacentWindowProjectionTickRegressed,
                selection);
        }

        var chain = selection.Summary.SourceProjection.Chain;
        var checkpoints =
            new HostRuntimeRecoveryCheckpoint<TRequest>[selection.CheckpointCount];
        for (var index = 0; index < checkpoints.Length; index++)
        {
            checkpoints[index] = CheckpointAt(
                chain,
                checked(selection.StartCheckpointIndex + index));
        }

        var supersessions =
            new HostRuntimeRecoveryCheckpointSupersession<
                TRequest,
                TState,
                TCompletion>[selection.CheckpointCount - 1];
        for (var index = 0; index < supersessions.Length; index++)
        {
            supersessions[index] =
                chain.Supersessions[selection.StartCheckpointIndex + index];
        }

        if (!global::System.Object.ReferenceEquals(
                checkpoints[0],
                selection.StartCheckpoint) ||
            !global::System.Object.ReferenceEquals(
                checkpoints[^1],
                selection.EndCheckpoint))
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentWindowProjectionStatus
                    .SelectionCheckpointMismatch,
                selection);
        }

        var expectedIncoming =
            selection.StartCheckpointIndex > 0
                ? chain.Supersessions[selection.StartCheckpointIndex - 1]
                : null;
        var expectedOutgoing =
            selection.EndCheckpointIndex < chain.SupersessionCount
                ? chain.Supersessions[selection.EndCheckpointIndex]
                : null;
        if (!global::System.Object.ReferenceEquals(
                expectedIncoming,
                selection.IncomingSupersession) ||
            !global::System.Object.ReferenceEquals(
                expectedOutgoing,
                selection.OutgoingSupersession))
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentWindowProjectionStatus
                    .SelectionSupersessionMismatch,
                selection);
        }

        var projection =
            new HostRuntimeRecoveryAdjacentWindowProjection<
                TRequest,
                TState,
                TCompletion>(
                    projectionId,
                    selection,
                    checkpoints,
                    supersessions,
                    projectedTick,
                    checked(selection.Revision + 1));

        return new HostRuntimeRecoveryAdjacentWindowProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryAdjacentWindowProjectionStatus
                    .AdjacentWindowProjected,
                selection,
                projection);
    }

    /// <summary>
    /// Validates the one exact checkpoint-supersession boundary connecting a
    /// summarized range and its projected previous or next adjacent window.
    /// </summary>
    public static HostRuntimeRecoveryCheckpointRangeContinuityValidationResult<
        TRequest,
        TState,
        TCompletion> ValidateContinuity<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointRangeContinuityValidationIdKind> validationId,
            HostRuntimeRecoveryCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            HostRuntimeRecoveryAdjacentWindowProjection<
                TRequest,
                TState,
                TCompletion> adjacentWindow,
            long expectedSummaryRevision,
            long expectedAdjacentWindowRevision,
            long validatedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(validationId.IsEmpty, nameof(validationId));
        global::System.ArgumentNullException.ThrowIfNull(summary);
        global::System.ArgumentNullException.ThrowIfNull(adjacentWindow);
        EnsureRevision(expectedSummaryRevision, nameof(expectedSummaryRevision));
        EnsureRevision(
            expectedAdjacentWindowRevision,
            nameof(expectedAdjacentWindowRevision));
        EnsureTick(validatedTick, nameof(validatedTick));

        if (summary.Revision != expectedSummaryRevision)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentWindowProjectionStatus
                    .StaleRangeSummaryRevision,
                summary,
                adjacentWindow);
        }
        if (adjacentWindow.Revision != expectedAdjacentWindowRevision)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentWindowProjectionStatus
                    .StaleAdjacentWindowRevision,
                summary,
                adjacentWindow);
        }
        if (validatedTick < summary.ProjectedTick ||
            validatedTick < adjacentWindow.ProjectedTick)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentWindowProjectionStatus
                    .ContinuityValidationTickRegressed,
                summary,
                adjacentWindow);
        }
        if (!global::System.Object.ReferenceEquals(
                adjacentWindow.Summary,
                summary))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentWindowProjectionStatus
                    .RangeSummaryMismatch,
                summary,
                adjacentWindow);
        }

        HostRuntimeRecoveryCheckpoint<TRequest> priorCheckpoint;
        HostRuntimeRecoveryCheckpoint<TRequest> successorCheckpoint;
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>? summaryBoundary;
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>? windowBoundary;

        if (adjacentWindow.SelectsPreviousWindow)
        {
            if (adjacentWindow.EndCheckpointIndex + 1 != summary.StartChainIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentWindowProjectionStatus
                        .CheckpointRangeNotAdjacent,
                    summary,
                    adjacentWindow);
            }

            priorCheckpoint = adjacentWindow.EndCheckpoint;
            successorCheckpoint = summary.StartCheckpoint;
            summaryBoundary = summary.IncomingSupersession;
            windowBoundary = adjacentWindow.OutgoingSupersession;
        }
        else
        {
            if (summary.EndChainIndex + 1 != adjacentWindow.StartCheckpointIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentWindowProjectionStatus
                        .CheckpointRangeNotAdjacent,
                    summary,
                    adjacentWindow);
            }

            priorCheckpoint = summary.EndCheckpoint;
            successorCheckpoint = adjacentWindow.StartCheckpoint;
            summaryBoundary = summary.OutgoingSupersession;
            windowBoundary = adjacentWindow.IncomingSupersession;
        }

        if (summaryBoundary is null ||
            !global::System.Object.ReferenceEquals(summaryBoundary, windowBoundary))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentWindowProjectionStatus
                    .SupersessionBoundaryMismatch,
                summary,
                adjacentWindow);
        }
        if (!global::System.Object.ReferenceEquals(
                summaryBoundary.PriorCheckpoint,
                priorCheckpoint) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary.SuccessorCheckpoint,
                successorCheckpoint))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentWindowProjectionStatus
                    .CheckpointBoundaryMismatch,
                summary,
                adjacentWindow);
        }

        var validation =
            new HostRuntimeRecoveryCheckpointRangeContinuityValidation<
                TRequest,
                TState,
                TCompletion>(
                    validationId,
                    summary,
                    adjacentWindow,
                    summaryBoundary,
                    validatedTick,
                    checked(global::System.Math.Max(
                        summary.Revision,
                        adjacentWindow.Revision) + 1));

        return new HostRuntimeRecoveryCheckpointRangeContinuityValidationResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryAdjacentWindowProjectionStatus
                    .CheckpointRangeContinuityValidated,
                summary,
                adjacentWindow,
                validation);
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

    private static HostRuntimeRecoveryAdjacentWindowProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryAdjacentWindowProjectionStatus status,
            HostRuntimeRecoveryAdjacentWindowSelection<
                TRequest,
                TState,
                TCompletion> selection)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, selection, projection: null);

    private static HostRuntimeRecoveryCheckpointRangeContinuityValidationResult<
        TRequest,
        TState,
        TCompletion> ContinuityResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryAdjacentWindowProjectionStatus status,
            HostRuntimeRecoveryCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            HostRuntimeRecoveryAdjacentWindowProjection<
                TRequest,
                TState,
                TCompletion> adjacentWindow)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, summary, adjacentWindow, validation: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery adjacent-window identifiers cannot be empty.",
                parameterName);
        }
    }

    private static void EnsureRevision(long revision, string parameterName)
    {
        if (revision < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                revision,
                "Recovery adjacent-window revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery adjacent-window ticks cannot be negative.");
        }
    }
}
