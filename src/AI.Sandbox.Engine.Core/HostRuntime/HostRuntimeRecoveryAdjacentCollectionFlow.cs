namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects exact selected adjacent collections and validates their immediate
/// continuity with summarized multi-sequence checkpoint ranges without
/// discovery, reordering, storage, indexing, history mutation, archival,
/// compaction, pagination, diagnostics, scheduling, supervision, waiting,
/// restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryAdjacentCollectionFlow
{
    /// <summary>Projects one exact selected adjacent collection.</summary>
    public static HostRuntimeRecoveryAdjacentCollectionProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectCollection<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryAdjacentCollectionProjectionIdKind> projectionId,
            HostRuntimeRecoveryAdjacentCollectionSelection<
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
                HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                    .StaleSelectionRevision,
                selection);
        }
        if (projectedTick < selection.SelectedTick)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                    .AdjacentCollectionProjectionTickRegressed,
                selection);
        }

        var sourceCollection = selection.SourceCollection;
        if (selection.StartSummaryIndex < 0 ||
            selection.EndSummaryIndex >= sourceCollection.SummaryCount)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                    .SelectionSummaryMismatch,
                selection);
        }
        for (var index = 0; index < selection.SummaryCount; index++)
        {
            if (!global::System.Object.ReferenceEquals(
                    selection.MultiSequenceSummaries[index],
                    sourceCollection.MultiSequenceSummaries[
                        checked(selection.StartSummaryIndex + index)]))
            {
                return ProjectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                        .SelectionSummaryMismatch,
                    selection);
            }
        }

        var expectedBoundaryCount = checked((selection.SummaryCount * 2) - 1);
        var boundaryStartIndex = checked(selection.StartSummaryIndex * 2);
        if (selection.BoundarySupersessions.Count != expectedBoundaryCount ||
            boundaryStartIndex < 0 ||
            boundaryStartIndex + expectedBoundaryCount >
                sourceCollection.BoundarySupersessions.Count)
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                    .SelectionBoundarySupersessionMismatch,
                selection);
        }
        for (var index = 0; index < expectedBoundaryCount; index++)
        {
            if (!global::System.Object.ReferenceEquals(
                    selection.BoundarySupersessions[index],
                    sourceCollection.BoundarySupersessions[
                        checked(boundaryStartIndex + index)]))
            {
                return ProjectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                        .SelectionBoundarySupersessionMismatch,
                    selection);
            }
        }

        var adjacentBoundaryIndex = selection.SelectsPreviousCollection
            ? checked((selection.EndSummaryIndex * 2) + 1)
            : checked((selection.Summary.EndSummaryIndex * 2) + 1);
        if (adjacentBoundaryIndex < 0 ||
            adjacentBoundaryIndex >= sourceCollection.BoundarySupersessions.Count ||
            !global::System.Object.ReferenceEquals(
                selection.AdjacentBoundarySupersession,
                sourceCollection.BoundarySupersessions[adjacentBoundaryIndex]))
        {
            return ProjectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionProjectionStatus
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
                HostRuntimeRecoveryAdjacentCollectionProjectionStatus
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
                HostRuntimeRecoveryAdjacentCollectionProjectionStatus
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
                HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                    .SelectionSupersessionMismatch,
                selection);
        }

        var projection = new HostRuntimeRecoveryAdjacentCollectionProjection<
            TRequest,
            TState,
            TCompletion>(
                projectionId,
                selection,
                checkpoints,
                supersessions,
                projectedTick,
                checked(selection.Revision + 1));
        return new HostRuntimeRecoveryAdjacentCollectionProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                    .AdjacentCollectionProjected,
                selection,
                projection);
    }

    /// <summary>
    /// Validates one exact summary, checkpoint, and supersession boundary
    /// connecting a summarized range and its projected previous or next
    /// adjacent collection.
    /// </summary>
    public static HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidationResult<
        TRequest,
        TState,
        TCompletion> ValidateContinuity<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidationIdKind>
                    validationId,
            HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            HostRuntimeRecoveryAdjacentCollectionProjection<
                TRequest,
                TState,
                TCompletion> adjacentCollection,
            long expectedSummaryRevision,
            long expectedAdjacentCollectionRevision,
            long validatedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(validationId.IsEmpty, nameof(validationId));
        global::System.ArgumentNullException.ThrowIfNull(summary);
        global::System.ArgumentNullException.ThrowIfNull(adjacentCollection);
        EnsureRevision(expectedSummaryRevision, nameof(expectedSummaryRevision));
        EnsureRevision(
            expectedAdjacentCollectionRevision,
            nameof(expectedAdjacentCollectionRevision));
        EnsureTick(validatedTick, nameof(validatedTick));

        if (summary.Revision != expectedSummaryRevision)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                    .StaleRangeSummaryRevision,
                summary,
                adjacentCollection);
        }
        if (adjacentCollection.Revision != expectedAdjacentCollectionRevision)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                    .StaleAdjacentCollectionRevision,
                summary,
                adjacentCollection);
        }
        if (validatedTick < summary.ProjectedTick ||
            validatedTick < adjacentCollection.ProjectedTick)
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                    .ContinuityValidationTickRegressed,
                summary,
                adjacentCollection);
        }
        if (!global::System.Object.ReferenceEquals(
                adjacentCollection.Summary,
                summary))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                    .RangeSummaryMismatch,
                summary,
                adjacentCollection);
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
            TCompletion>? collectionBoundary;

        if (adjacentCollection.SelectsPreviousCollection)
        {
            if (adjacentCollection.EndSummaryIndex + 1 !=
                summary.StartSummaryIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                        .SummaryRangeNotAdjacent,
                    summary,
                    adjacentCollection);
            }
            if (adjacentCollection.EndCheckpointIndex + 1 !=
                summary.StartCheckpointIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                        .CheckpointRangeNotAdjacent,
                    summary,
                    adjacentCollection);
            }

            priorCheckpoint = adjacentCollection.EndCheckpoint;
            successorCheckpoint = summary.StartCheckpoint;
            summaryBoundary = summary.IncomingSupersession;
            collectionBoundary = adjacentCollection.OutgoingSupersession;
        }
        else
        {
            if (summary.EndSummaryIndex + 1 !=
                adjacentCollection.StartSummaryIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                        .SummaryRangeNotAdjacent,
                    summary,
                    adjacentCollection);
            }
            if (summary.EndCheckpointIndex + 1 !=
                adjacentCollection.StartCheckpointIndex)
            {
                return ContinuityResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                        .CheckpointRangeNotAdjacent,
                    summary,
                    adjacentCollection);
            }

            priorCheckpoint = summary.EndCheckpoint;
            successorCheckpoint = adjacentCollection.StartCheckpoint;
            summaryBoundary = summary.OutgoingSupersession;
            collectionBoundary = adjacentCollection.IncomingSupersession;
        }

        if (summaryBoundary is null ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                collectionBoundary) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                adjacentCollection.AdjacentBoundarySupersession))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                    .SupersessionBoundaryMismatch,
                summary,
                adjacentCollection);
        }
        if (!global::System.Object.ReferenceEquals(
                summaryBoundary.PriorCheckpoint,
                priorCheckpoint) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary.SuccessorCheckpoint,
                successorCheckpoint))
        {
            return ContinuityResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                    .CheckpointBoundaryMismatch,
                summary,
                adjacentCollection);
        }

        var validation =
            new HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidation<
                TRequest,
                TState,
                TCompletion>(
                    validationId,
                    summary,
                    adjacentCollection,
                    summaryBoundary,
                    validatedTick,
                    checked(global::System.Math.Max(
                        summary.Revision,
                        adjacentCollection.Revision) + 1));
        return new HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidationResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryAdjacentCollectionProjectionStatus
                    .MultiSequenceCheckpointRangeContinuityValidated,
                summary,
                adjacentCollection,
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

    private static HostRuntimeRecoveryAdjacentCollectionProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryAdjacentCollectionProjectionStatus status,
            HostRuntimeRecoveryAdjacentCollectionSelection<
                TRequest,
                TState,
                TCompletion> selection)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, selection, projection: null);

    private static HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidationResult<
        TRequest,
        TState,
        TCompletion> ContinuityResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryAdjacentCollectionProjectionStatus status,
            HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection<
                TRequest,
                TState,
                TCompletion> summary,
            HostRuntimeRecoveryAdjacentCollectionProjection<
                TRequest,
                TState,
                TCompletion> adjacentCollection)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, summary, adjacentCollection, validation: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery adjacent-collection identifiers cannot be empty.",
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
                "Recovery adjacent-collection revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery adjacent-collection ticks cannot be negative.");
        }
    }
}
