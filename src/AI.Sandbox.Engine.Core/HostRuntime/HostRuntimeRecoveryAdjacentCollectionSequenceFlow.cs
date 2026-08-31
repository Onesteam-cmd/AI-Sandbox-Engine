namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects exact selected adjacent collection sequences and validates their immediate
/// continuity with summarized multi-collection checkpoint ranges without
/// discovery, reordering, storage, indexing, history mutation, archival,
/// compaction, pagination, diagnostics, scheduling, supervision, waiting,
/// restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryAdjacentCollectionSequenceFlow
{
    /// <summary>Projects one exact selected adjacent collection sequence.</summary>
    public static HostRuntimeRecoveryAdjacentCollectionSequenceProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectCollectionSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentCollectionSequenceProjectionIdKind> projectionId,
            HostRuntimeRecoveryAdjacentCollectionSequenceSelection<
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
                HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
                    .StaleSelectionRevision,
                selection);
        }
        if (projectedTick < selection.SelectedTick)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
                    .AdjacentCollectionSequenceProjectionTickRegressed,
                selection);
        }

        var sourceSequence = selection.SourceSequence;
        if (selection.StartCollectionPairIndex < 0 ||
            selection.EndCollectionPairIndex >= sourceSequence.CollectionPairCount)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
                    .SelectionCollectionPairSummaryMismatch,
                selection);
        }
        for (var index = 0; index < selection.CollectionPairCount; index++)
        {
            if (!global::System.Object.ReferenceEquals(
                    selection.CollectionPairSummaries[index],
                    sourceSequence.CollectionPairSummaries[
                        checked(selection.StartCollectionPairIndex + index)]))
            {
                return ProjectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
                        .SelectionCollectionPairSummaryMismatch,
                    selection);
            }
        }

        var expectedBoundaryCount = checked((selection.CollectionPairCount * 2) - 1);
        var boundaryStartIndex = checked(selection.StartCollectionPairIndex * 2);
        if (selection.BoundarySupersessions.Count != expectedBoundaryCount ||
            boundaryStartIndex < 0 ||
            boundaryStartIndex + expectedBoundaryCount >
                sourceSequence.BoundarySupersessions.Count)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
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
                    HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
                        .SelectionBoundarySupersessionMismatch,
                    selection);
            }
        }

        var adjacentBoundaryIndex = selection.SelectsPreviousCollectionSequence
            ? checked((selection.EndCollectionPairIndex * 2) + 1)
            : checked((selection.Summary.EndCollectionPairIndex * 2) + 1);
        if (adjacentBoundaryIndex < 0 ||
            adjacentBoundaryIndex >= sourceSequence.BoundarySupersessions.Count ||
            !global::System.Object.ReferenceEquals(
                selection.AdjacentBoundarySupersession,
                sourceSequence.BoundarySupersessions[adjacentBoundaryIndex]))
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
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
                HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
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
                HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
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
                HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
                    .SelectionSupersessionMismatch,
                selection);
        }

        var projection = new HostRuntimeRecoveryAdjacentCollectionSequenceProjection<
            TRequest,
            TState,
            TCompletion>(
                projectionId,
                selection,
                checkpoints,
                supersessions,
                projectedTick,
                checked(selection.Revision + 1));
        return new HostRuntimeRecoveryAdjacentCollectionSequenceProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
                    .AdjacentCollectionSequenceProjected,
                selection,
                projection);
    }

    /// <summary>
    /// Validates one exact summary, checkpoint, and supersession boundary
    /// connecting a summarized range and its projected previous or next
    /// adjacent collection sequence.
    /// </summary>
    public static HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidationResult<
        TRequest,
        TState,
        TCompletion> ValidateContinuity<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidationIdKind>
                    validationId,
            HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            HostRuntimeRecoveryAdjacentCollectionSequenceProjection<
                TRequest,
                TState,
                TCompletion> adjacentCollectionSequence,
            long expectedSummaryRevision,
            long expectedAdjacentCollectionSequenceRevision,
            long validatedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(validationId.IsEmpty, nameof(validationId));
        global::System.ArgumentNullException.ThrowIfNull(summary);
        global::System.ArgumentNullException.ThrowIfNull(adjacentCollectionSequence);
        EnsureRevision(expectedSummaryRevision, nameof(expectedSummaryRevision));
        EnsureRevision(
            expectedAdjacentCollectionSequenceRevision,
            nameof(expectedAdjacentCollectionSequenceRevision));
        EnsureTick(validatedTick, nameof(validatedTick));

        if (summary.Revision != expectedSummaryRevision)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
                    .StaleRangeSummaryRevision,
                summary,
                adjacentCollectionSequence);
        }
        if (adjacentCollectionSequence.Revision != expectedAdjacentCollectionSequenceRevision)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
                    .StaleAdjacentCollectionSequenceRevision,
                summary,
                adjacentCollectionSequence);
        }
        if (validatedTick < summary.ProjectedTick ||
            validatedTick < adjacentCollectionSequence.ProjectedTick)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
                    .ContinuityValidationTickRegressed,
                summary,
                adjacentCollectionSequence);
        }
        if (!global::System.Object.ReferenceEquals(
                adjacentCollectionSequence.Summary,
                summary))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
                    .RangeSummaryMismatch,
                summary,
                adjacentCollectionSequence);
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
            TCompletion>? collectionSequenceBoundary;

        if (adjacentCollectionSequence.SelectsPreviousCollectionSequence)
        {
            if (adjacentCollectionSequence.EndCollectionPairIndex + 1 !=
                summary.StartCollectionPairIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
                        .CollectionPairRangeNotAdjacent,
                    summary,
                    adjacentCollectionSequence);
            }
            if (adjacentCollectionSequence.EndCheckpointIndex + 1 !=
                summary.StartCheckpointIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
                        .CheckpointRangeNotAdjacent,
                    summary,
                    adjacentCollectionSequence);
            }

            priorCheckpoint = adjacentCollectionSequence.EndCheckpoint;
            successorCheckpoint = summary.StartCheckpoint;
            summaryBoundary = summary.IncomingSupersession;
            collectionSequenceBoundary = adjacentCollectionSequence.OutgoingSupersession;
        }
        else
        {
            if (summary.EndCollectionPairIndex + 1 !=
                adjacentCollectionSequence.StartCollectionPairIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
                        .CollectionPairRangeNotAdjacent,
                    summary,
                    adjacentCollectionSequence);
            }
            if (summary.EndCheckpointIndex + 1 !=
                adjacentCollectionSequence.StartCheckpointIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
                        .CheckpointRangeNotAdjacent,
                    summary,
                    adjacentCollectionSequence);
            }

            priorCheckpoint = summary.EndCheckpoint;
            successorCheckpoint = adjacentCollectionSequence.StartCheckpoint;
            summaryBoundary = summary.OutgoingSupersession;
            collectionSequenceBoundary = adjacentCollectionSequence.IncomingSupersession;
        }

        if (summaryBoundary is null ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                collectionSequenceBoundary) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                adjacentCollectionSequence.AdjacentBoundarySupersession))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
                    .SupersessionBoundaryMismatch,
                summary,
                adjacentCollectionSequence);
        }
        if (!global::System.Object.ReferenceEquals(
                summaryBoundary.PriorCheckpoint,
                priorCheckpoint) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary.SuccessorCheckpoint,
                successorCheckpoint))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
                    .CheckpointBoundaryMismatch,
                summary,
                adjacentCollectionSequence);
        }

        var validation =
            new HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidation<
                TRequest,
                TState,
                TCompletion>(
                    validationId,
                    summary,
                    adjacentCollectionSequence,
                    summaryBoundary,
                    validatedTick,
                    checked(global::System.Math.Max(
                        summary.Revision,
                        adjacentCollectionSequence.Revision) + 1));
        return new HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidationResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
                    .MultiCollectionCheckpointRangeContinuityValidated,
                summary,
                adjacentCollectionSequence,
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

    private static HostRuntimeRecoveryAdjacentCollectionSequenceProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus status,
            HostRuntimeRecoveryAdjacentCollectionSequenceSelection<
                TRequest,
                TState,
                TCompletion> selection)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, selection, projection: null);

    private static HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidationResult<
        TRequest,
        TState,
        TCompletion> ContinuityResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus status,
            HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            HostRuntimeRecoveryAdjacentCollectionSequenceProjection<
                TRequest,
                TState,
                TCompletion> adjacentCollectionSequence)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, summary, adjacentCollectionSequence, validation: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery adjacent collection-sequence identifiers cannot be empty.",
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
                "Recovery adjacent collection-sequence revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery adjacent collection-sequence ticks cannot be negative.");
        }
    }
}
