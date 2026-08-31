namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects exact selected adjacent multi-collection-sequences and validates their immediate
/// continuity with summarized multi-collection-sequence-sequence checkpoint ranges without
/// discovery, reordering, storage, indexing, history mutation, archival,
/// compaction, pagination, diagnostics, scheduling, supervision, waiting,
/// restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryAdjacentMultiCollectionSequenceFlow
{
    /// <summary>Projects one exact selected adjacent multi-collection-sequence.</summary>
    public static HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectMultiCollectionSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionIdKind> projectionId,
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection<
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
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                    .StaleSelectionRevision,
                selection);
        }
        if (projectedTick < selection.SelectedTick)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                    .AdjacentMultiCollectionSequenceProjectionTickRegressed,
                selection);
        }

        var sourceSequence = selection.SourceSequence;
        if (selection.StartSequenceSummaryIndex < 0 ||
            selection.EndSequenceSummaryIndex >= sourceSequence.MultiCollectionSequenceSummaryCount)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                    .SelectionMultiCollectionSequenceSummaryMismatch,
                selection);
        }
        for (var index = 0; index < selection.MultiCollectionSequenceCount; index++)
        {
            if (!global::System.Object.ReferenceEquals(
                    selection.MultiCollectionSequenceSummaries[index],
                    sourceSequence.MultiCollectionSequenceSummaries[
                        checked(selection.StartSequenceSummaryIndex + index)]))
            {
                return ProjectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                        .SelectionMultiCollectionSequenceSummaryMismatch,
                    selection);
            }
        }

        var expectedBoundaryCount = checked((selection.MultiCollectionSequenceCount * 2) - 1);
        var boundaryStartIndex = checked(selection.StartSequenceSummaryIndex * 2);
        if (selection.BoundarySupersessions.Count != expectedBoundaryCount ||
            boundaryStartIndex < 0 ||
            boundaryStartIndex + expectedBoundaryCount >
                sourceSequence.BoundarySupersessions.Count)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                    .SelectionBoundarySupersessionMismatch,
                selection);
        }
        for (var index = 0; index < expectedBoundaryCount; index++)
        {
            if (!global::System.Object.ReferenceEquals(
                    selection.BoundarySupersessions[index],
                    sourceSequence.BoundarySupersessions[
                        checked(boundaryStartIndex + index)]))
            {
                return ProjectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                        .SelectionBoundarySupersessionMismatch,
                    selection);
            }
        }

        var adjacentBoundaryIndex = selection.SelectsPreviousMultiCollectionSequence
            ? checked((selection.EndSequenceSummaryIndex * 2) + 1)
            : checked((selection.Summary.EndSummaryIndex * 2) + 1);
        if (adjacentBoundaryIndex < 0 ||
            adjacentBoundaryIndex >= sourceSequence.BoundarySupersessions.Count ||
            !global::System.Object.ReferenceEquals(
                selection.AdjacentBoundarySupersession,
                sourceSequence.BoundarySupersessions[adjacentBoundaryIndex]))
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                    .SelectionBoundarySupersessionMismatch,
                selection);
        }

        var chain = selection.Summary.Chain;
        var checkpointCount = selection.CheckpointCount;
        if (checkpointCount <= 0 ||
            selection.StartCheckpointIndex < 0 ||
            selection.EndCheckpointIndex > chain.SupersessionCount)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                    .SelectionCheckpointMismatch,
                selection);
        }

        var checkpoints = new HostRuntimeRecoveryCheckpoint<TRequest>[checkpointCount];
        for (var index = 0; index < checkpoints.Length; index++)
        {
            checkpoints[index] = CheckpointAt(
                chain,
                checked(selection.StartCheckpointIndex + index));
        }
        if (!global::System.Object.ReferenceEquals(
                checkpoints[0],
                selection.StartCheckpoint) ||
            !global::System.Object.ReferenceEquals(
                checkpoints[^1],
                selection.EndCheckpoint))
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                    .SelectionCheckpointMismatch,
                selection);
        }

        var supersessions =
            new HostRuntimeRecoveryCheckpointSupersession<
                TRequest,
                TState,
                TCompletion>[checked(checkpointCount - 1)];
        for (var index = 0; index < supersessions.Length; index++)
        {
            supersessions[index] = chain.Supersessions[
                checked(selection.StartCheckpointIndex + index)];
        }

        var expectedIncoming = selection.StartCheckpointIndex > 0
            ? chain.Supersessions[selection.StartCheckpointIndex - 1]
            : null;
        var expectedOutgoing = selection.EndCheckpointIndex < chain.SupersessionCount
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
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                    .SelectionSupersessionMismatch,
                selection);
        }

        var projection = new HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjection<
            TRequest,
            TState,
            TCompletion>(
                projectionId,
                selection,
                checkpoints,
                supersessions,
                projectedTick,
                checked(selection.Revision + 1));
        return new HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                    .AdjacentMultiCollectionSequenceProjected,
                selection,
                projection);
    }

    /// <summary>
    /// Validates one exact summary, checkpoint, and supersession boundary
    /// connecting a summarized range and its projected previous or next
    /// adjacent multi-collection-sequence.
    /// </summary>
    public static HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidationResult<
        TRequest,
        TState,
        TCompletion> ValidateContinuity<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidationIdKind>
                    validationId,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjection<
                TRequest,
                TState,
                TCompletion> adjacentMultiCollectionSequence,
            long expectedSummaryRevision,
            long expectedAdjacentMultiCollectionSequenceRevision,
            long validatedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(validationId.IsEmpty, nameof(validationId));
        global::System.ArgumentNullException.ThrowIfNull(summary);
        global::System.ArgumentNullException.ThrowIfNull(adjacentMultiCollectionSequence);
        EnsureRevision(expectedSummaryRevision, nameof(expectedSummaryRevision));
        EnsureRevision(
            expectedAdjacentMultiCollectionSequenceRevision,
            nameof(expectedAdjacentMultiCollectionSequenceRevision));
        EnsureTick(validatedTick, nameof(validatedTick));

        if (summary.Revision != expectedSummaryRevision)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                    .StaleRangeSummaryRevision,
                summary,
                adjacentMultiCollectionSequence);
        }
        if (adjacentMultiCollectionSequence.Revision != expectedAdjacentMultiCollectionSequenceRevision)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                    .StaleAdjacentMultiCollectionSequenceRevision,
                summary,
                adjacentMultiCollectionSequence);
        }
        if (validatedTick < summary.ProjectedTick ||
            validatedTick < adjacentMultiCollectionSequence.ProjectedTick)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                    .ContinuityValidationTickRegressed,
                summary,
                adjacentMultiCollectionSequence);
        }
        if (!global::System.Object.ReferenceEquals(
                adjacentMultiCollectionSequence.Summary,
                summary))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                    .RangeSummaryMismatch,
                summary,
                adjacentMultiCollectionSequence);
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
            TCompletion>? multiCollectionSequenceBoundary;

        if (adjacentMultiCollectionSequence.SelectsPreviousMultiCollectionSequence)
        {
            if (adjacentMultiCollectionSequence.EndSequenceSummaryIndex + 1 !=
                summary.StartSummaryIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                        .MultiCollectionSequenceRangeNotAdjacent,
                    summary,
                    adjacentMultiCollectionSequence);
            }
            if (adjacentMultiCollectionSequence.EndCheckpointIndex + 1 !=
                summary.StartCheckpointIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                        .CheckpointRangeNotAdjacent,
                    summary,
                    adjacentMultiCollectionSequence);
            }

            priorCheckpoint = adjacentMultiCollectionSequence.EndCheckpoint;
            successorCheckpoint = summary.StartCheckpoint;
            summaryBoundary = summary.IncomingSupersession;
            multiCollectionSequenceBoundary = adjacentMultiCollectionSequence.OutgoingSupersession;
        }
        else
        {
            if (summary.EndSummaryIndex + 1 !=
                adjacentMultiCollectionSequence.StartSequenceSummaryIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                        .MultiCollectionSequenceRangeNotAdjacent,
                    summary,
                    adjacentMultiCollectionSequence);
            }
            if (summary.EndCheckpointIndex + 1 !=
                adjacentMultiCollectionSequence.StartCheckpointIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                        .CheckpointRangeNotAdjacent,
                    summary,
                    adjacentMultiCollectionSequence);
            }

            priorCheckpoint = summary.EndCheckpoint;
            successorCheckpoint = adjacentMultiCollectionSequence.StartCheckpoint;
            summaryBoundary = summary.OutgoingSupersession;
            multiCollectionSequenceBoundary = adjacentMultiCollectionSequence.IncomingSupersession;
        }

        if (summaryBoundary is null ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                multiCollectionSequenceBoundary) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                adjacentMultiCollectionSequence.AdjacentBoundarySupersession))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                    .SupersessionBoundaryMismatch,
                summary,
                adjacentMultiCollectionSequence);
        }
        if (!global::System.Object.ReferenceEquals(
                summaryBoundary.PriorCheckpoint,
                priorCheckpoint) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary.SuccessorCheckpoint,
                successorCheckpoint))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                    .CheckpointBoundaryMismatch,
                summary,
                adjacentMultiCollectionSequence);
        }

        var validation =
            new HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidation<
                TRequest,
                TState,
                TCompletion>(
                    validationId,
                    summary,
                    adjacentMultiCollectionSequence,
                    summaryBoundary,
                    validatedTick,
                    checked(global::System.Math.Max(
                        summary.Revision,
                        adjacentMultiCollectionSequence.Revision) + 1));
        return new HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidationResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
                    .MultiCollectionSequenceSequenceCheckpointRangeContinuityValidated,
                summary,
                adjacentMultiCollectionSequence,
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
            : chain.Supersessions[checkpointIndex - 1].SuccessorCheckpoint;

    private static HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus status,
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection<
                TRequest,
                TState,
                TCompletion> selection)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, selection, projection: null);

    private static HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidationResult<
        TRequest,
        TState,
        TCompletion> ContinuityResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus status,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjection<
                TRequest,
                TState,
                TCompletion> adjacentMultiCollectionSequence)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, summary, adjacentMultiCollectionSequence, validation: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery adjacent multi-collection-sequence identifiers cannot be empty.",
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
                "Recovery adjacent multi-collection-sequence revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery adjacent multi-collection-sequence ticks cannot be negative.");
        }
    }
}
