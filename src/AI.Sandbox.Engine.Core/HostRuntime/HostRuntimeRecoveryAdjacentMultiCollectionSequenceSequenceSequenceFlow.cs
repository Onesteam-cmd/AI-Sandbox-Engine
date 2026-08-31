namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects exact selected adjacent multi-collection-sequence-sequence-sequences and validates their immediate
/// continuity with summarized multi-collection-sequence-sequence-sequence-sequence checkpoint ranges without
/// discovery, reordering, storage, indexing, history mutation, archival,
/// compaction, pagination, diagnostics, scheduling, supervision, waiting,
/// restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceFlow
{
    /// <summary>Projects one exact selected adjacent multi-collection-sequence-sequence-sequence.</summary>
    public static HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectMultiCollectionSequenceSequenceSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionIdKind> projectionId,
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelection<
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
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
                    .StaleSelectionRevision,
                selection);
        }
        if (projectedTick < selection.SelectedTick)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
                    .AdjacentMultiCollectionSequenceSequenceSequenceProjectionTickRegressed,
                selection);
        }

        var sourceSequence = selection.SourceSequence;
        if (selection.StartSequenceSequenceSequenceSummaryIndex < 0 ||
            selection.EndSequenceSequenceSequenceSummaryIndex >= sourceSequence.MultiCollectionSequenceSequenceSequenceSummaryCount)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
                    .SelectionMultiCollectionSequenceSequenceSequenceSummaryMismatch,
                selection);
        }
        for (var index = 0; index < selection.MultiCollectionSequenceSequenceSequenceCount; index++)
        {
            if (!global::System.Object.ReferenceEquals(
                    selection.MultiCollectionSequenceSequenceSequenceSummaries[index],
                    sourceSequence.MultiCollectionSequenceSequenceSequenceSummaries[
                        checked(selection.StartSequenceSequenceSequenceSummaryIndex + index)]))
            {
                return ProjectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
                        .SelectionMultiCollectionSequenceSequenceSequenceSummaryMismatch,
                    selection);
            }
        }

        var expectedBoundaryCount = checked((selection.MultiCollectionSequenceSequenceSequenceCount * 2) - 1);
        var boundaryStartIndex = checked(selection.StartSequenceSequenceSequenceSummaryIndex * 2);
        if (selection.BoundarySupersessions.Count != expectedBoundaryCount ||
            boundaryStartIndex < 0 ||
            boundaryStartIndex + expectedBoundaryCount >
                sourceSequence.BoundarySupersessions.Count)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
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
                    HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
                        .SelectionBoundarySupersessionMismatch,
                    selection);
            }
        }

        var adjacentBoundaryIndex = selection.SelectsPreviousMultiCollectionSequenceSequenceSequence
            ? checked((selection.EndSequenceSequenceSequenceSummaryIndex * 2) + 1)
            : checked((selection.Summary.EndSummaryIndex * 2) + 1);
        if (adjacentBoundaryIndex < 0 ||
            adjacentBoundaryIndex >= sourceSequence.BoundarySupersessions.Count ||
            !global::System.Object.ReferenceEquals(
                selection.AdjacentBoundarySupersession,
                sourceSequence.BoundarySupersessions[adjacentBoundaryIndex]))
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
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
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
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
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
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
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
                    .SelectionSupersessionMismatch,
                selection);
        }

        var projection = new HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjection<
            TRequest,
            TState,
            TCompletion>(
                projectionId,
                selection,
                checkpoints,
                supersessions,
                projectedTick,
                checked(selection.Revision + 1));
        return new HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
                    .AdjacentMultiCollectionSequenceSequenceSequenceProjected,
                selection,
                projection);
    }

    /// <summary>
    /// Validates one exact summary, checkpoint, and supersession boundary
    /// connecting a summarized range and its projected previous or next
    /// adjacent multi-collection-sequence-sequence-sequence.
    /// </summary>
    public static HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationResult<
        TRequest,
        TState,
        TCompletion> ValidateContinuity<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationIdKind>
                    validationId,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjection<
                TRequest,
                TState,
                TCompletion> adjacentMultiCollectionSequenceSequenceSequence,
            long expectedSummaryRevision,
            long expectedAdjacentMultiCollectionSequenceSequenceSequenceRevision,
            long validatedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(validationId.IsEmpty, nameof(validationId));
        global::System.ArgumentNullException.ThrowIfNull(summary);
        global::System.ArgumentNullException.ThrowIfNull(adjacentMultiCollectionSequenceSequenceSequence);
        EnsureRevision(expectedSummaryRevision, nameof(expectedSummaryRevision));
        EnsureRevision(
            expectedAdjacentMultiCollectionSequenceSequenceSequenceRevision,
            nameof(expectedAdjacentMultiCollectionSequenceSequenceSequenceRevision));
        EnsureTick(validatedTick, nameof(validatedTick));

        if (summary.Revision != expectedSummaryRevision)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
                    .StaleRangeSummaryRevision,
                summary,
                adjacentMultiCollectionSequenceSequenceSequence);
        }
        if (adjacentMultiCollectionSequenceSequenceSequence.Revision != expectedAdjacentMultiCollectionSequenceSequenceSequenceRevision)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
                    .StaleAdjacentMultiCollectionSequenceSequenceSequenceRevision,
                summary,
                adjacentMultiCollectionSequenceSequenceSequence);
        }
        if (validatedTick < summary.ProjectedTick ||
            validatedTick < adjacentMultiCollectionSequenceSequenceSequence.ProjectedTick)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
                    .ContinuityValidationTickRegressed,
                summary,
                adjacentMultiCollectionSequenceSequenceSequence);
        }
        if (!global::System.Object.ReferenceEquals(
                adjacentMultiCollectionSequenceSequenceSequence.Summary,
                summary))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
                    .RangeSummaryMismatch,
                summary,
                adjacentMultiCollectionSequenceSequenceSequence);
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
            TCompletion>? multiCollectionSequenceSequenceSequenceBoundary;

        if (adjacentMultiCollectionSequenceSequenceSequence.SelectsPreviousMultiCollectionSequenceSequenceSequence)
        {
            if (adjacentMultiCollectionSequenceSequenceSequence.EndSequenceSequenceSequenceSummaryIndex + 1 !=
                summary.StartSummaryIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
                        .MultiCollectionSequenceSequenceSequenceRangeNotAdjacent,
                    summary,
                    adjacentMultiCollectionSequenceSequenceSequence);
            }
            if (adjacentMultiCollectionSequenceSequenceSequence.EndCheckpointIndex + 1 !=
                summary.StartCheckpointIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
                        .CheckpointRangeNotAdjacent,
                    summary,
                    adjacentMultiCollectionSequenceSequenceSequence);
            }

            priorCheckpoint = adjacentMultiCollectionSequenceSequenceSequence.EndCheckpoint;
            successorCheckpoint = summary.StartCheckpoint;
            summaryBoundary = summary.IncomingSupersession;
            multiCollectionSequenceSequenceSequenceBoundary = adjacentMultiCollectionSequenceSequenceSequence.OutgoingSupersession;
        }
        else
        {
            if (summary.EndSummaryIndex + 1 !=
                adjacentMultiCollectionSequenceSequenceSequence.StartSequenceSequenceSequenceSummaryIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
                        .MultiCollectionSequenceSequenceSequenceRangeNotAdjacent,
                    summary,
                    adjacentMultiCollectionSequenceSequenceSequence);
            }
            if (summary.EndCheckpointIndex + 1 !=
                adjacentMultiCollectionSequenceSequenceSequence.StartCheckpointIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
                        .CheckpointRangeNotAdjacent,
                    summary,
                    adjacentMultiCollectionSequenceSequenceSequence);
            }

            priorCheckpoint = summary.EndCheckpoint;
            successorCheckpoint = adjacentMultiCollectionSequenceSequenceSequence.StartCheckpoint;
            summaryBoundary = summary.OutgoingSupersession;
            multiCollectionSequenceSequenceSequenceBoundary = adjacentMultiCollectionSequenceSequenceSequence.IncomingSupersession;
        }

        if (summaryBoundary is null ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                multiCollectionSequenceSequenceSequenceBoundary) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                adjacentMultiCollectionSequenceSequenceSequence.AdjacentBoundarySupersession))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
                    .SupersessionBoundaryMismatch,
                summary,
                adjacentMultiCollectionSequenceSequenceSequence);
        }
        if (!global::System.Object.ReferenceEquals(
                summaryBoundary.PriorCheckpoint,
                priorCheckpoint) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary.SuccessorCheckpoint,
                successorCheckpoint))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
                    .CheckpointBoundaryMismatch,
                summary,
                adjacentMultiCollectionSequenceSequenceSequence);
        }

        var validation =
            new HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidation<
                TRequest,
                TState,
                TCompletion>(
                    validationId,
                    summary,
                    adjacentMultiCollectionSequenceSequenceSequence,
                    summaryBoundary,
                    validatedTick,
                    checked(global::System.Math.Max(
                        summary.Revision,
                        adjacentMultiCollectionSequenceSequenceSequence.Revision) + 1));
        return new HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
                    .MultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidated,
                summary,
                adjacentMultiCollectionSequenceSequenceSequence,
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

    private static HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus status,
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelection<
                TRequest,
                TState,
                TCompletion> selection)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, selection, projection: null);

    private static HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationResult<
        TRequest,
        TState,
        TCompletion> ContinuityResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus status,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjection<
                TRequest,
                TState,
                TCompletion> adjacentMultiCollectionSequenceSequenceSequence)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, summary, adjacentMultiCollectionSequenceSequenceSequence, validation: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery adjacent multi-collection-sequence-sequence-sequence identifiers cannot be empty.",
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
                "Recovery adjacent multi-collection-sequence-sequence-sequence revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery adjacent multi-collection-sequence-sequence-sequence ticks cannot be negative.");
        }
    }
}
