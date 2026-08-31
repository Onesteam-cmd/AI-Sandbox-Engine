namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Validates bounded ordered collections of exact continuous multi-sequence
/// summaries and resolves bounded inclusive ranges across their validated
/// sequence boundaries without discovery, reordering, storage, indexing,
/// history mutation, archival, compaction, pagination, diagnostics,
/// scheduling, supervision, waiting, restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryContinuousMultiSequenceCollectionFlow
{
    /// <summary>Maximum summary projections represented by one collection.</summary>
    public const int MaximumSummaryCount = 8;

    /// <summary>Maximum checkpoints represented by one multi-sequence query.</summary>
    public const int MaximumCheckpointCount =
        HostRuntimeRecoveryContinuousMultiSequenceFlow.MaximumCheckpointCount;

    /// <summary>Validates one bounded exact collection of continuous multi-sequences.</summary>
    public static HostRuntimeRecoveryContinuousMultiSequenceCollectionValidationResult<
        TRequest,
        TState,
        TCompletion> ValidateCollection<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryContinuousMultiSequenceCollectionValidationIdKind> validationId,
            global::System.Collections.Generic.IReadOnlyList<
                HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
                    TRequest,
                    TState,
                    TCompletion>> multiSequenceSummaries,
            global::System.Collections.Generic.IReadOnlyList<long>
                expectedSummaryRevisions,
            long validatedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(validationId.IsEmpty, nameof(validationId));
        global::System.ArgumentNullException.ThrowIfNull(multiSequenceSummaries);
        global::System.ArgumentNullException.ThrowIfNull(expectedSummaryRevisions);
        EnsureTick(validatedTick, nameof(validatedTick));

        var snapshot =
            new HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion>[multiSequenceSummaries.Count];
        for (var index = 0; index < snapshot.Length; index++)
        {
            global::System.ArgumentNullException.ThrowIfNull(
                multiSequenceSummaries[index]);
            snapshot[index] = multiSequenceSummaries[index];
        }

        var readOnlySnapshot = global::System.Array.AsReadOnly(snapshot);
        if (snapshot.Length == 0)
        {
            return CollectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                    .SummaryCollectionEmpty,
                readOnlySnapshot);
        }
        if (snapshot.Length > MaximumSummaryCount)
        {
            return CollectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                    .SummaryCollectionTooLarge,
                readOnlySnapshot);
        }
        if (expectedSummaryRevisions.Count != snapshot.Length)
        {
            return CollectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                    .SummaryRevisionCountMismatch,
                readOnlySnapshot);
        }

        var first = snapshot[0];
        var sourceProjection = first.SourceProjection;
        var chain = first.Chain;
        var summaryIds = new global::System.Collections.Generic.HashSet<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionIdKind>>();
        var boundarySupersessions =
            new HostRuntimeRecoveryCheckpointSupersession<
                TRequest,
                TState,
                TCompletion>[checked((snapshot.Length * 2) - 1)];
        var boundaryIndex = 0;
        var maximumRevision = first.Revision;

        for (var index = 0; index < snapshot.Length; index++)
        {
            var current = snapshot[index];
            var expectedRevision = expectedSummaryRevisions[index];
            EnsureRevision(expectedRevision, nameof(expectedSummaryRevisions));

            if (current.Revision != expectedRevision)
            {
                return CollectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                        .StaleMultiSequenceSummaryRevision,
                    readOnlySnapshot);
            }
            if (validatedTick < current.ProjectedTick)
            {
                return CollectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                        .CollectionValidationTickRegressed,
                    readOnlySnapshot);
            }
            if (!summaryIds.Add(current.SummaryId))
            {
                return CollectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                        .DuplicateMultiSequenceSummaryId,
                    readOnlySnapshot);
            }
            if (!global::System.Object.ReferenceEquals(
                    current.SourceProjection,
                    sourceProjection) ||
                !global::System.Object.ReferenceEquals(current.Chain, chain))
            {
                return CollectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                        .SummarySourceMismatch,
                    readOnlySnapshot);
            }
            if (current.StartPairIndex < 0 ||
                current.EndPairIndex <= current.StartPairIndex ||
                current.StartCheckpointIndex < 0 ||
                current.EndCheckpointIndex <= current.StartCheckpointIndex ||
                current.EndCheckpointIndex > chain.SupersessionCount ||
                current.ConnectingPriorPairIndex < current.StartPairIndex ||
                current.ConnectingPriorPairIndex >= current.EndPairIndex ||
                current.ConnectingSuccessorPairIndex !=
                    current.ConnectingPriorPairIndex + 1 ||
                current.ConnectingPriorCheckpointIndex <
                    current.StartCheckpointIndex ||
                current.ConnectingPriorCheckpointIndex >=
                    current.EndCheckpointIndex ||
                current.ConnectingSuccessorCheckpointIndex !=
                    current.ConnectingPriorCheckpointIndex + 1 ||
                !global::System.Object.ReferenceEquals(
                    chain.Supersessions[
                        current.ConnectingPriorCheckpointIndex],
                    current.ConnectingSupersession) ||
                !global::System.Object.ReferenceEquals(
                    current.ConnectingSupersession.PriorCheckpoint,
                    CheckpointAt(
                        chain,
                        current.ConnectingPriorCheckpointIndex)) ||
                !global::System.Object.ReferenceEquals(
                    current.ConnectingSupersession.SuccessorCheckpoint,
                    CheckpointAt(
                        chain,
                        current.ConnectingSuccessorCheckpointIndex)))
            {
                return CollectionResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                        .SummarySupersessionMismatch,
                    readOnlySnapshot);
            }

            if (index > 0)
            {
                var previous = snapshot[index - 1];
                if (previous.EndPairIndex + 1 != current.StartPairIndex)
                {
                    return CollectionResult<TRequest, TState, TCompletion>(
                        HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                            .SummaryPairRangeNotContinuous,
                        readOnlySnapshot);
                }
                if (previous.EndCheckpointIndex + 1 !=
                        current.StartCheckpointIndex ||
                    previous.EndCheckpointIndex < 0 ||
                    previous.EndCheckpointIndex >= chain.SupersessionCount)
                {
                    return CollectionResult<TRequest, TState, TCompletion>(
                        HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                            .SummaryCheckpointRangeNotContinuous,
                        readOnlySnapshot);
                }

                var collectionBoundary =
                    chain.Supersessions[previous.EndCheckpointIndex];
                if (!global::System.Object.ReferenceEquals(
                        collectionBoundary.PriorCheckpoint,
                        previous.EndCheckpoint) ||
                    !global::System.Object.ReferenceEquals(
                        collectionBoundary.SuccessorCheckpoint,
                        current.StartCheckpoint))
                {
                    return CollectionResult<TRequest, TState, TCompletion>(
                        HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                            .CollectionBoundarySupersessionMismatch,
                        readOnlySnapshot);
                }

                boundarySupersessions[boundaryIndex++] = collectionBoundary;
            }

            boundarySupersessions[boundaryIndex++] =
                current.ConnectingSupersession;
            maximumRevision =
                global::System.Math.Max(maximumRevision, current.Revision);
        }

        var validation =
            new HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
                TRequest,
                TState,
                TCompletion>(
                    validationId,
                    snapshot,
                    boundarySupersessions,
                    validatedTick,
                    checked(maximumRevision + 1));

        return new HostRuntimeRecoveryContinuousMultiSequenceCollectionValidationResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                    .ContinuousMultiSequenceCollectionValidated,
                readOnlySnapshot,
                validation);
    }

    /// <summary>
    /// Resolves one exact bounded inclusive checkpoint range that crosses at
    /// least one validated sequence boundary in a multi-sequence collection.
    /// </summary>
    public static HostRuntimeRecoveryMultiSequenceCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryRange<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryMultiSequenceCheckpointRangeQueryIdKind> queryId,
            HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
                TRequest,
                TState,
                TCompletion> collection,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> startCheckpointId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> endCheckpointId,
            long expectedCollectionRevision,
            long queriedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(queryId.IsEmpty, nameof(queryId));
        global::System.ArgumentNullException.ThrowIfNull(collection);
        EnsureId(startCheckpointId.IsEmpty, nameof(startCheckpointId));
        EnsureId(endCheckpointId.IsEmpty, nameof(endCheckpointId));
        EnsureRevision(expectedCollectionRevision, nameof(expectedCollectionRevision));
        EnsureTick(queriedTick, nameof(queriedTick));

        if (collection.Revision != expectedCollectionRevision)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                    .StaleCollectionRevision,
                collection);
        }
        if (queriedTick < collection.ValidatedTick)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                    .MultiSequenceRangeQueryTickRegressed,
                collection);
        }

        var chain = collection.Chain;
        var startCheckpointIndex = FindCheckpointIndex(
            chain,
            collection.StartCheckpointIndex,
            collection.EndCheckpointIndex,
            startCheckpointId);
        if (startCheckpointIndex < 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                    .RangeStartNotFound,
                collection);
        }

        var endCheckpointIndex = FindCheckpointIndex(
            chain,
            collection.StartCheckpointIndex,
            collection.EndCheckpointIndex,
            endCheckpointId);
        if (endCheckpointIndex < 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                    .RangeEndNotFound,
                collection);
        }
        if (endCheckpointIndex < startCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                    .RangeOrderInvalid,
                collection);
        }

        var crossedBoundaries =
            new global::System.Collections.Generic.List<
                HostRuntimeRecoveryCheckpointSupersession<
                    TRequest,
                    TState,
                    TCompletion>>();
        foreach (var boundary in collection.BoundarySupersessions)
        {
            var boundaryIndex = FindSupersessionIndex(
                chain,
                collection.StartCheckpointIndex,
                collection.EndCheckpointIndex - 1,
                boundary);
            if (boundaryIndex >= startCheckpointIndex &&
                boundaryIndex < endCheckpointIndex)
            {
                crossedBoundaries.Add(boundary);
            }
        }

        if (crossedBoundaries.Count == 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                    .RangeDoesNotCrossSequenceBoundary,
                collection);
        }

        var checkpointCount =
            checked(endCheckpointIndex - startCheckpointIndex + 1);
        if (checkpointCount > MaximumCheckpointCount)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                    .RangeTooLarge,
                collection);
        }

        var checkpoints =
            new HostRuntimeRecoveryCheckpoint<TRequest>[checkpointCount];
        for (var index = 0; index < checkpoints.Length; index++)
        {
            checkpoints[index] = CheckpointAt(
                chain,
                checked(startCheckpointIndex + index));
        }

        var supersessions =
            new HostRuntimeRecoveryCheckpointSupersession<
                TRequest,
                TState,
                TCompletion>[checkpointCount - 1];
        for (var index = 0; index < supersessions.Length; index++)
        {
            supersessions[index] =
                chain.Supersessions[checked(startCheckpointIndex + index)];
        }

        foreach (var boundary in crossedBoundaries)
        {
            var boundaryIndex = FindSupersessionIndex(
                chain,
                startCheckpointIndex,
                endCheckpointIndex - 1,
                boundary);
            var offset = checked(boundaryIndex - startCheckpointIndex);
            if (boundaryIndex < 0 ||
                offset < 0 ||
                offset >= supersessions.Length ||
                !global::System.Object.ReferenceEquals(
                    supersessions[offset],
                    boundary))
            {
                return QueryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                        .RangeSupersessionMismatch,
                    collection);
            }
        }

        var incomingSupersession =
            startCheckpointIndex > 0
                ? chain.Supersessions[startCheckpointIndex - 1]
                : null;
        var outgoingSupersession =
            endCheckpointIndex < chain.SupersessionCount
                ? chain.Supersessions[endCheckpointIndex]
                : null;
        var startSummaryIndex =
            FindSummaryIndex(collection, startCheckpointIndex);
        var endSummaryIndex =
            FindSummaryIndex(collection, endCheckpointIndex);

        var query =
            new HostRuntimeRecoveryMultiSequenceCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion>(
                    queryId,
                    collection,
                    checkpoints,
                    supersessions,
                    crossedBoundaries.ToArray(),
                    incomingSupersession,
                    outgoingSupersession,
                    startCheckpointIndex,
                    endCheckpointIndex,
                    startSummaryIndex,
                    endSummaryIndex,
                    queriedTick,
                    checked(collection.Revision + 1));

        return new HostRuntimeRecoveryMultiSequenceCheckpointRangeQueryResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
                    .MultiSequenceCheckpointRangeQueried,
                collection,
                query);
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

    private static int FindCheckpointIndex<TRequest, TState, TCompletion>(
        HostRuntimeRecoverySupersessionChain<
            TRequest,
            TState,
            TCompletion> chain,
        int startCheckpointIndex,
        int endCheckpointIndex,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryCheckpointIdKind> checkpointId)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        for (var index = startCheckpointIndex;
            index <= endCheckpointIndex;
            index++)
        {
            if (CheckpointAt(chain, index).CheckpointId == checkpointId)
            {
                return index;
            }
        }

        return -1;
    }

    private static int FindSupersessionIndex<TRequest, TState, TCompletion>(
        HostRuntimeRecoverySupersessionChain<
            TRequest,
            TState,
            TCompletion> chain,
        int startSupersessionIndex,
        int endSupersessionIndex,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion> supersession)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        for (var index = startSupersessionIndex;
            index <= endSupersessionIndex;
            index++)
        {
            if (global::System.Object.ReferenceEquals(
                    chain.Supersessions[index],
                    supersession))
            {
                return index;
            }
        }

        return -1;
    }

    private static int FindSummaryIndex<TRequest, TState, TCompletion>(
        HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
            TRequest,
            TState,
            TCompletion> collection,
        int checkpointIndex)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        for (var index = 0;
            index < collection.MultiSequenceSummaries.Count;
            index++)
        {
            var summary = collection.MultiSequenceSummaries[index];
            if (checkpointIndex >= summary.StartCheckpointIndex &&
                checkpointIndex <= summary.EndCheckpointIndex)
            {
                return index;
            }
        }

        return -1;
    }

    private static HostRuntimeRecoveryContinuousMultiSequenceCollectionValidationResult<
        TRequest,
        TState,
        TCompletion> CollectionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus status,
            global::System.Collections.Generic.IReadOnlyList<
                HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
                    TRequest,
                    TState,
                    TCompletion>> summaries)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, summaries, validation: null);

    private static HostRuntimeRecoveryMultiSequenceCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus status,
            HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
                TRequest,
                TState,
                TCompletion> collection)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, collection, query: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery continuous multi-sequence collection identifiers cannot be empty.",
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
                "Recovery continuous multi-sequence collection revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery continuous multi-sequence collection ticks cannot be negative.");
        }
    }
}
