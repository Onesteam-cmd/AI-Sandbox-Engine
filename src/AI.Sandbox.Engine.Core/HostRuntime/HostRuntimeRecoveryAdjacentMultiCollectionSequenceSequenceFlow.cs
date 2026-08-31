namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects exact selected adjacent multi-collection-sequence-sequences and validates their immediate
/// continuity with summarized multi-collection-sequence-sequence-sequence checkpoint ranges without
/// discovery, reordering, storage, indexing, history mutation, archival,
/// compaction, pagination, diagnostics, scheduling, supervision, waiting,
/// restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceFlow
{
    /// <summary>Projects one exact selected adjacent multi-collection-sequence-sequence.</summary>
    public static HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectMultiCollectionSequenceSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionIdKind> projectionId,
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelection<
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
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
                    .StaleSelectionRevision,
                selection);
        }
        if (projectedTick < selection.SelectedTick)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
                    .AdjacentMultiCollectionSequenceSequenceProjectionTickRegressed,
                selection);
        }

        var sourceSequence = selection.SourceSequence;
        if (selection.StartSequenceSequenceSummaryIndex < 0 ||
            selection.EndSequenceSequenceSummaryIndex >= sourceSequence.MultiCollectionSequenceSequenceSummaryCount)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
                    .SelectionMultiCollectionSequenceSequenceSummaryMismatch,
                selection);
        }
        for (var index = 0; index < selection.MultiCollectionSequenceSequenceCount; index++)
        {
            if (!global::System.Object.ReferenceEquals(
                    selection.MultiCollectionSequenceSequenceSummaries[index],
                    sourceSequence.MultiCollectionSequenceSequenceSummaries[
                        checked(selection.StartSequenceSequenceSummaryIndex + index)]))
            {
                return ProjectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
                        .SelectionMultiCollectionSequenceSequenceSummaryMismatch,
                    selection);
            }
        }

        var expectedBoundaryCount = checked((selection.MultiCollectionSequenceSequenceCount * 2) - 1);
        var boundaryStartIndex = checked(selection.StartSequenceSequenceSummaryIndex * 2);
        if (selection.BoundarySupersessions.Count != expectedBoundaryCount ||
            boundaryStartIndex < 0 ||
            boundaryStartIndex + expectedBoundaryCount >
                sourceSequence.BoundarySupersessions.Count)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
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
                    HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
                        .SelectionBoundarySupersessionMismatch,
                    selection);
            }
        }

        var adjacentBoundaryIndex = selection.SelectsPreviousMultiCollectionSequenceSequence
            ? checked((selection.EndSequenceSequenceSummaryIndex * 2) + 1)
            : checked((selection.Summary.EndSummaryIndex * 2) + 1);
        if (adjacentBoundaryIndex < 0 ||
            adjacentBoundaryIndex >= sourceSequence.BoundarySupersessions.Count ||
            !global::System.Object.ReferenceEquals(
                selection.AdjacentBoundarySupersession,
                sourceSequence.BoundarySupersessions[adjacentBoundaryIndex]))
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
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
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
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
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
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
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
                    .SelectionSupersessionMismatch,
                selection);
        }

        var projection = new HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjection<
            TRequest,
            TState,
            TCompletion>(
                projectionId,
                selection,
                checkpoints,
                supersessions,
                projectedTick,
                checked(selection.Revision + 1));
        return new HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
                    .AdjacentMultiCollectionSequenceSequenceProjected,
                selection,
                projection);
    }

    /// <summary>
    /// Validates one exact summary, checkpoint, and supersession boundary
    /// connecting a summarized range and its projected previous or next
    /// adjacent multi-collection-sequence-sequence.
    /// </summary>
    public static HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidationResult<
        TRequest,
        TState,
        TCompletion> ValidateContinuity<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidationIdKind>
                    validationId,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjection<
                TRequest,
                TState,
                TCompletion> adjacentMultiCollectionSequenceSequence,
            long expectedSummaryRevision,
            long expectedAdjacentMultiCollectionSequenceSequenceRevision,
            long validatedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(validationId.IsEmpty, nameof(validationId));
        global::System.ArgumentNullException.ThrowIfNull(summary);
        global::System.ArgumentNullException.ThrowIfNull(adjacentMultiCollectionSequenceSequence);
        EnsureRevision(expectedSummaryRevision, nameof(expectedSummaryRevision));
        EnsureRevision(
            expectedAdjacentMultiCollectionSequenceSequenceRevision,
            nameof(expectedAdjacentMultiCollectionSequenceSequenceRevision));
        EnsureTick(validatedTick, nameof(validatedTick));

        if (summary.Revision != expectedSummaryRevision)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
                    .StaleRangeSummaryRevision,
                summary,
                adjacentMultiCollectionSequenceSequence);
        }
        if (adjacentMultiCollectionSequenceSequence.Revision != expectedAdjacentMultiCollectionSequenceSequenceRevision)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
                    .StaleAdjacentMultiCollectionSequenceSequenceRevision,
                summary,
                adjacentMultiCollectionSequenceSequence);
        }
        if (validatedTick < summary.ProjectedTick ||
            validatedTick < adjacentMultiCollectionSequenceSequence.ProjectedTick)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
                    .ContinuityValidationTickRegressed,
                summary,
                adjacentMultiCollectionSequenceSequence);
        }
        if (!global::System.Object.ReferenceEquals(
                adjacentMultiCollectionSequenceSequence.Summary,
                summary))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
                    .RangeSummaryMismatch,
                summary,
                adjacentMultiCollectionSequenceSequence);
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
            TCompletion>? multiCollectionSequenceSequenceBoundary;

        if (adjacentMultiCollectionSequenceSequence.SelectsPreviousMultiCollectionSequenceSequence)
        {
            if (adjacentMultiCollectionSequenceSequence.EndSequenceSequenceSummaryIndex + 1 !=
                summary.StartSummaryIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
                        .MultiCollectionSequenceSequenceRangeNotAdjacent,
                    summary,
                    adjacentMultiCollectionSequenceSequence);
            }
            if (adjacentMultiCollectionSequenceSequence.EndCheckpointIndex + 1 !=
                summary.StartCheckpointIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
                        .CheckpointRangeNotAdjacent,
                    summary,
                    adjacentMultiCollectionSequenceSequence);
            }

            priorCheckpoint = adjacentMultiCollectionSequenceSequence.EndCheckpoint;
            successorCheckpoint = summary.StartCheckpoint;
            summaryBoundary = summary.IncomingSupersession;
            multiCollectionSequenceSequenceBoundary = adjacentMultiCollectionSequenceSequence.OutgoingSupersession;
        }
        else
        {
            if (summary.EndSummaryIndex + 1 !=
                adjacentMultiCollectionSequenceSequence.StartSequenceSequenceSummaryIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
                        .MultiCollectionSequenceSequenceRangeNotAdjacent,
                    summary,
                    adjacentMultiCollectionSequenceSequence);
            }
            if (summary.EndCheckpointIndex + 1 !=
                adjacentMultiCollectionSequenceSequence.StartCheckpointIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
                        .CheckpointRangeNotAdjacent,
                    summary,
                    adjacentMultiCollectionSequenceSequence);
            }

            priorCheckpoint = summary.EndCheckpoint;
            successorCheckpoint = adjacentMultiCollectionSequenceSequence.StartCheckpoint;
            summaryBoundary = summary.OutgoingSupersession;
            multiCollectionSequenceSequenceBoundary = adjacentMultiCollectionSequenceSequence.IncomingSupersession;
        }

        if (summaryBoundary is null ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                multiCollectionSequenceSequenceBoundary) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                adjacentMultiCollectionSequenceSequence.AdjacentBoundarySupersession))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
                    .SupersessionBoundaryMismatch,
                summary,
                adjacentMultiCollectionSequenceSequence);
        }
        if (!global::System.Object.ReferenceEquals(
                summaryBoundary.PriorCheckpoint,
                priorCheckpoint) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary.SuccessorCheckpoint,
                successorCheckpoint))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
                    .CheckpointBoundaryMismatch,
                summary,
                adjacentMultiCollectionSequenceSequence);
        }

        var validation =
            new HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidation<
                TRequest,
                TState,
                TCompletion>(
                    validationId,
                    summary,
                    adjacentMultiCollectionSequenceSequence,
                    summaryBoundary,
                    validatedTick,
                    checked(global::System.Math.Max(
                        summary.Revision,
                        adjacentMultiCollectionSequenceSequence.Revision) + 1));
        return new HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidationResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
                    .MultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidated,
                summary,
                adjacentMultiCollectionSequenceSequence,
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

    private static HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus status,
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelection<
                TRequest,
                TState,
                TCompletion> selection)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, selection, projection: null);

    private static HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidationResult<
        TRequest,
        TState,
        TCompletion> ContinuityResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus status,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjection<
                TRequest,
                TState,
                TCompletion> adjacentMultiCollectionSequenceSequence)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, summary, adjacentMultiCollectionSequenceSequence, validation: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery adjacent multi-collection-sequence-sequence identifiers cannot be empty.",
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
                "Recovery adjacent multi-collection-sequence-sequence revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery adjacent multi-collection-sequence-sequence ticks cannot be negative.");
        }
    }
}
