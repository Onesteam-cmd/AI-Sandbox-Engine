namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects exact selected adjacent multi-collections and validates their immediate
/// continuity with summarized multi-collection-sequence checkpoint ranges without
/// discovery, reordering, storage, indexing, history mutation, archival,
/// compaction, pagination, diagnostics, scheduling, supervision, waiting,
/// restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryAdjacentMultiCollectionFlow
{
    /// <summary>Projects one exact selected adjacent multi-collection.</summary>
    public static HostRuntimeRecoveryAdjacentMultiCollectionProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectMultiCollection<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentMultiCollectionProjectionIdKind> projectionId,
            HostRuntimeRecoveryAdjacentMultiCollectionSelection<
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
                HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
                    .StaleSelectionRevision,
                selection);
        }
        if (projectedTick < selection.SelectedTick)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
                    .AdjacentMultiCollectionProjectionTickRegressed,
                selection);
        }

        var sourceSequence = selection.SourceSequence;
        if (selection.StartSummaryIndex < 0 ||
            selection.EndSummaryIndex >= sourceSequence.MultiCollectionCount)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
                    .SelectionMultiCollectionSummaryMismatch,
                selection);
        }
        for (var index = 0; index < selection.MultiCollectionCount; index++)
        {
            if (!global::System.Object.ReferenceEquals(
                    selection.MultiCollectionSummaries[index],
                    sourceSequence.MultiCollectionSummaries[
                        checked(selection.StartSummaryIndex + index)]))
            {
                return ProjectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
                        .SelectionMultiCollectionSummaryMismatch,
                    selection);
            }
        }

        var expectedBoundaryCount = checked((selection.MultiCollectionCount * 2) - 1);
        var boundaryStartIndex = checked(selection.StartSummaryIndex * 2);
        if (selection.BoundarySupersessions.Count != expectedBoundaryCount ||
            boundaryStartIndex < 0 ||
            boundaryStartIndex + expectedBoundaryCount >
                sourceSequence.BoundarySupersessions.Count)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
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
                    HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
                        .SelectionBoundarySupersessionMismatch,
                    selection);
            }
        }

        var adjacentBoundaryIndex = selection.SelectsPreviousMultiCollection
            ? checked((selection.EndSummaryIndex * 2) + 1)
            : checked((selection.Summary.EndSummaryIndex * 2) + 1);
        if (adjacentBoundaryIndex < 0 ||
            adjacentBoundaryIndex >= sourceSequence.BoundarySupersessions.Count ||
            !global::System.Object.ReferenceEquals(
                selection.AdjacentBoundarySupersession,
                sourceSequence.BoundarySupersessions[adjacentBoundaryIndex]))
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
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
                HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
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
                HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
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
                HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
                    .SelectionSupersessionMismatch,
                selection);
        }

        var projection = new HostRuntimeRecoveryAdjacentMultiCollectionProjection<
            TRequest,
            TState,
            TCompletion>(
                projectionId,
                selection,
                checkpoints,
                supersessions,
                projectedTick,
                checked(selection.Revision + 1));
        return new HostRuntimeRecoveryAdjacentMultiCollectionProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
                    .AdjacentMultiCollectionProjected,
                selection,
                projection);
    }

    /// <summary>
    /// Validates one exact summary, checkpoint, and supersession boundary
    /// connecting a summarized range and its projected previous or next
    /// adjacent multi-collection.
    /// </summary>
    public static HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidationResult<
        TRequest,
        TState,
        TCompletion> ValidateContinuity<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidationIdKind>
                    validationId,
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            HostRuntimeRecoveryAdjacentMultiCollectionProjection<
                TRequest,
                TState,
                TCompletion> adjacentMultiCollection,
            long expectedSummaryRevision,
            long expectedAdjacentMultiCollectionRevision,
            long validatedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(validationId.IsEmpty, nameof(validationId));
        global::System.ArgumentNullException.ThrowIfNull(summary);
        global::System.ArgumentNullException.ThrowIfNull(adjacentMultiCollection);
        EnsureRevision(expectedSummaryRevision, nameof(expectedSummaryRevision));
        EnsureRevision(
            expectedAdjacentMultiCollectionRevision,
            nameof(expectedAdjacentMultiCollectionRevision));
        EnsureTick(validatedTick, nameof(validatedTick));

        if (summary.Revision != expectedSummaryRevision)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
                    .StaleRangeSummaryRevision,
                summary,
                adjacentMultiCollection);
        }
        if (adjacentMultiCollection.Revision != expectedAdjacentMultiCollectionRevision)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
                    .StaleAdjacentMultiCollectionRevision,
                summary,
                adjacentMultiCollection);
        }
        if (validatedTick < summary.ProjectedTick ||
            validatedTick < adjacentMultiCollection.ProjectedTick)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
                    .ContinuityValidationTickRegressed,
                summary,
                adjacentMultiCollection);
        }
        if (!global::System.Object.ReferenceEquals(
                adjacentMultiCollection.Summary,
                summary))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
                    .RangeSummaryMismatch,
                summary,
                adjacentMultiCollection);
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
            TCompletion>? multiCollectionBoundary;

        if (adjacentMultiCollection.SelectsPreviousMultiCollection)
        {
            if (adjacentMultiCollection.EndSummaryIndex + 1 !=
                summary.StartSummaryIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
                        .MultiCollectionRangeNotAdjacent,
                    summary,
                    adjacentMultiCollection);
            }
            if (adjacentMultiCollection.EndCheckpointIndex + 1 !=
                summary.StartCheckpointIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
                        .CheckpointRangeNotAdjacent,
                    summary,
                    adjacentMultiCollection);
            }

            priorCheckpoint = adjacentMultiCollection.EndCheckpoint;
            successorCheckpoint = summary.StartCheckpoint;
            summaryBoundary = summary.IncomingSupersession;
            multiCollectionBoundary = adjacentMultiCollection.OutgoingSupersession;
        }
        else
        {
            if (summary.EndSummaryIndex + 1 !=
                adjacentMultiCollection.StartSummaryIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
                        .MultiCollectionRangeNotAdjacent,
                    summary,
                    adjacentMultiCollection);
            }
            if (summary.EndCheckpointIndex + 1 !=
                adjacentMultiCollection.StartCheckpointIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
                        .CheckpointRangeNotAdjacent,
                    summary,
                    adjacentMultiCollection);
            }

            priorCheckpoint = summary.EndCheckpoint;
            successorCheckpoint = adjacentMultiCollection.StartCheckpoint;
            summaryBoundary = summary.OutgoingSupersession;
            multiCollectionBoundary = adjacentMultiCollection.IncomingSupersession;
        }

        if (summaryBoundary is null ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                multiCollectionBoundary) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                adjacentMultiCollection.AdjacentBoundarySupersession))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
                    .SupersessionBoundaryMismatch,
                summary,
                adjacentMultiCollection);
        }
        if (!global::System.Object.ReferenceEquals(
                summaryBoundary.PriorCheckpoint,
                priorCheckpoint) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary.SuccessorCheckpoint,
                successorCheckpoint))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
                    .CheckpointBoundaryMismatch,
                summary,
                adjacentMultiCollection);
        }

        var validation =
            new HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidation<
                TRequest,
                TState,
                TCompletion>(
                    validationId,
                    summary,
                    adjacentMultiCollection,
                    summaryBoundary,
                    validatedTick,
                    checked(global::System.Math.Max(
                        summary.Revision,
                        adjacentMultiCollection.Revision) + 1));
        return new HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidationResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
                    .MultiCollectionSequenceCheckpointRangeContinuityValidated,
                summary,
                adjacentMultiCollection,
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

    private static HostRuntimeRecoveryAdjacentMultiCollectionProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus status,
            HostRuntimeRecoveryAdjacentMultiCollectionSelection<
                TRequest,
                TState,
                TCompletion> selection)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, selection, projection: null);

    private static HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidationResult<
        TRequest,
        TState,
        TCompletion> ContinuityResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus status,
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            HostRuntimeRecoveryAdjacentMultiCollectionProjection<
                TRequest,
                TState,
                TCompletion> adjacentMultiCollection)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, summary, adjacentMultiCollection, validation: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery adjacent multi-collection identifiers cannot be empty.",
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
                "Recovery adjacent multi-collection revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery adjacent multi-collection ticks cannot be negative.");
        }
    }
}
