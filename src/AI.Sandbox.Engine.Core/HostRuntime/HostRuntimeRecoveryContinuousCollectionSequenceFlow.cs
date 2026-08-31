namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Validates bounded ordered sequences of exact continuous recovery
/// collection-pair summaries and resolves bounded inclusive ranges across
/// their validated collection boundaries without discovery, reordering,
/// storage, indexing, history mutation, archival, deletion, retention,
/// compaction, pagination, diagnostics, scheduling, supervision, waiting,
/// restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryContinuousCollectionSequenceFlow
{
    /// <summary>Maximum collection-pair summaries represented by one sequence.</summary>
    public const int MaximumCollectionPairCount = 8;

    /// <summary>Maximum checkpoints represented by one multi-collection query.</summary>
    public const int MaximumCheckpointCount =
        HostRuntimeRecoveryContinuousCollectionPairFlow.MaximumCheckpointCount;

    /// <summary>Validates one bounded exact sequence of continuous collection-pairs.</summary>
    public static HostRuntimeRecoveryContinuousCollectionSequenceValidationResult<
        TRequest,
        TState,
        TCompletion> ValidateSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryContinuousCollectionSequenceValidationIdKind> validationId,
            global::System.Collections.Generic.IReadOnlyList<
                HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
                    TRequest,
                    TState,
                    TCompletion>> collectionPairSummaries,
            global::System.Collections.Generic.IReadOnlyList<long>
                expectedCollectionPairRevisions,
            long validatedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(validationId.IsEmpty, nameof(validationId));
        global::System.ArgumentNullException.ThrowIfNull(collectionPairSummaries);
        global::System.ArgumentNullException.ThrowIfNull(
            expectedCollectionPairRevisions);
        EnsureTick(validatedTick, nameof(validatedTick));

        var snapshot =
            new HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
                TRequest,
                TState,
                TCompletion>[collectionPairSummaries.Count];
        for (var index = 0; index < snapshot.Length; index++)
        {
            global::System.ArgumentNullException.ThrowIfNull(
                collectionPairSummaries[index]);
            snapshot[index] = collectionPairSummaries[index];
        }

        var readOnlySnapshot = global::System.Array.AsReadOnly(snapshot);
        if (snapshot.Length == 0)
        {
            return SequenceResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionSequenceStatus
                    .CollectionPairSequenceEmpty,
                readOnlySnapshot);
        }
        if (snapshot.Length > MaximumCollectionPairCount)
        {
            return SequenceResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionSequenceStatus
                    .CollectionPairSequenceTooLarge,
                readOnlySnapshot);
        }
        if (expectedCollectionPairRevisions.Count != snapshot.Length)
        {
            return SequenceResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionSequenceStatus
                    .CollectionPairRevisionCountMismatch,
                readOnlySnapshot);
        }

        var first = snapshot[0];
        var sourceCollection = first.SourceCollection;
        var sourceProjection = first.SourceProjection;
        var chain = first.Chain;
        var summaryIds = new global::System.Collections.Generic.HashSet<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryContinuousCollectionPairSummaryProjectionIdKind>>();
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
            var expectedRevision = expectedCollectionPairRevisions[index];
            EnsureRevision(
                expectedRevision,
                nameof(expectedCollectionPairRevisions));

            if (current.Revision != expectedRevision)
            {
                return SequenceResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousCollectionSequenceStatus
                        .StaleCollectionPairSummaryRevision,
                    readOnlySnapshot);
            }
            if (validatedTick < current.ProjectedTick)
            {
                return SequenceResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousCollectionSequenceStatus
                        .CollectionSequenceValidationTickRegressed,
                    readOnlySnapshot);
            }
            if (!summaryIds.Add(current.SummaryId))
            {
                return SequenceResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousCollectionSequenceStatus
                        .DuplicateCollectionPairSummaryId,
                    readOnlySnapshot);
            }
            if (!global::System.Object.ReferenceEquals(
                    current.SourceCollection,
                    sourceCollection) ||
                !global::System.Object.ReferenceEquals(
                    current.SourceProjection,
                    sourceProjection) ||
                !global::System.Object.ReferenceEquals(current.Chain, chain))
            {
                return SequenceResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousCollectionSequenceStatus
                        .CollectionPairSourceMismatch,
                    readOnlySnapshot);
            }
            if (current.StartSummaryIndex < 0 ||
                current.EndSummaryIndex <= current.StartSummaryIndex ||
                current.EndSummaryIndex >= sourceCollection.SummaryCount ||
                current.StartCheckpointIndex < 0 ||
                current.EndCheckpointIndex <= current.StartCheckpointIndex ||
                current.EndCheckpointIndex > chain.SupersessionCount ||
                current.ConnectingPriorSummaryIndex <
                    current.StartSummaryIndex ||
                current.ConnectingPriorSummaryIndex >=
                    current.EndSummaryIndex ||
                current.ConnectingSuccessorSummaryIndex !=
                    current.ConnectingPriorSummaryIndex + 1 ||
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
                return SequenceResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousCollectionSequenceStatus
                        .CollectionPairSupersessionMismatch,
                    readOnlySnapshot);
            }

            if (index > 0)
            {
                var previous = snapshot[index - 1];
                if (previous.EndSummaryIndex + 1 !=
                    current.StartSummaryIndex)
                {
                    return SequenceResult<TRequest, TState, TCompletion>(
                        HostRuntimeRecoveryContinuousCollectionSequenceStatus
                            .CollectionSummaryRangeNotContinuous,
                        readOnlySnapshot);
                }
                if (previous.EndCheckpointIndex + 1 !=
                        current.StartCheckpointIndex ||
                    previous.EndCheckpointIndex < 0 ||
                    previous.EndCheckpointIndex >= chain.SupersessionCount)
                {
                    return SequenceResult<TRequest, TState, TCompletion>(
                        HostRuntimeRecoveryContinuousCollectionSequenceStatus
                            .CollectionCheckpointRangeNotContinuous,
                        readOnlySnapshot);
                }

                var sequenceBoundary =
                    chain.Supersessions[previous.EndCheckpointIndex];
                if (!global::System.Object.ReferenceEquals(
                        sequenceBoundary.PriorCheckpoint,
                        previous.EndCheckpoint) ||
                    !global::System.Object.ReferenceEquals(
                        sequenceBoundary.SuccessorCheckpoint,
                        current.StartCheckpoint))
                {
                    return SequenceResult<TRequest, TState, TCompletion>(
                        HostRuntimeRecoveryContinuousCollectionSequenceStatus
                            .CollectionSequenceBoundarySupersessionMismatch,
                        readOnlySnapshot);
                }

                boundarySupersessions[boundaryIndex++] = sequenceBoundary;
            }

            boundarySupersessions[boundaryIndex++] =
                current.ConnectingSupersession;
            maximumRevision =
                global::System.Math.Max(maximumRevision, current.Revision);
        }

        var validation =
            new HostRuntimeRecoveryContinuousCollectionSequenceValidation<
                TRequest,
                TState,
                TCompletion>(
                    validationId,
                    snapshot,
                    boundarySupersessions,
                    validatedTick,
                    checked(maximumRevision + 1));

        return new HostRuntimeRecoveryContinuousCollectionSequenceValidationResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousCollectionSequenceStatus
                    .ContinuousCollectionSequenceValidated,
                readOnlySnapshot,
                validation);
    }

    /// <summary>
    /// Resolves one exact bounded inclusive checkpoint range that crosses at
    /// least one validated collection boundary in a continuous collection sequence.
    /// </summary>
    public static HostRuntimeRecoveryMultiCollectionCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryRange<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryMultiCollectionCheckpointRangeQueryIdKind> queryId,
            HostRuntimeRecoveryContinuousCollectionSequenceValidation<
                TRequest,
                TState,
                TCompletion> sequence,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> startCheckpointId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> endCheckpointId,
            long expectedSequenceRevision,
            long queriedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(queryId.IsEmpty, nameof(queryId));
        global::System.ArgumentNullException.ThrowIfNull(sequence);
        EnsureId(startCheckpointId.IsEmpty, nameof(startCheckpointId));
        EnsureId(endCheckpointId.IsEmpty, nameof(endCheckpointId));
        EnsureRevision(expectedSequenceRevision, nameof(expectedSequenceRevision));
        EnsureTick(queriedTick, nameof(queriedTick));

        if (sequence.Revision != expectedSequenceRevision)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionSequenceStatus
                    .StaleCollectionSequenceRevision,
                sequence);
        }
        if (queriedTick < sequence.ValidatedTick)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionSequenceStatus
                    .MultiCollectionRangeQueryTickRegressed,
                sequence);
        }

        var chain = sequence.Chain;
        var startCheckpointIndex = FindCheckpointIndex(
            chain,
            sequence.StartCheckpointIndex,
            sequence.EndCheckpointIndex,
            startCheckpointId);
        if (startCheckpointIndex < 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionSequenceStatus
                    .RangeStartNotFound,
                sequence);
        }

        var endCheckpointIndex = FindCheckpointIndex(
            chain,
            sequence.StartCheckpointIndex,
            sequence.EndCheckpointIndex,
            endCheckpointId);
        if (endCheckpointIndex < 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionSequenceStatus
                    .RangeEndNotFound,
                sequence);
        }
        if (endCheckpointIndex < startCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionSequenceStatus
                    .RangeOrderInvalid,
                sequence);
        }

        var crossedBoundaries =
            new global::System.Collections.Generic.List<
                HostRuntimeRecoveryCheckpointSupersession<
                    TRequest,
                    TState,
                    TCompletion>>();
        foreach (var boundary in sequence.BoundarySupersessions)
        {
            var boundaryChainIndex = FindSupersessionIndex(
                chain,
                sequence.StartCheckpointIndex,
                sequence.EndCheckpointIndex - 1,
                boundary);
            if (boundaryChainIndex >= startCheckpointIndex &&
                boundaryChainIndex < endCheckpointIndex)
            {
                crossedBoundaries.Add(boundary);
            }
        }

        if (crossedBoundaries.Count == 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionSequenceStatus
                    .RangeDoesNotCrossCollectionBoundary,
                sequence);
        }

        var checkpointCount =
            checked(endCheckpointIndex - startCheckpointIndex + 1);
        if (checkpointCount > MaximumCheckpointCount)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousCollectionSequenceStatus
                    .RangeTooLarge,
                sequence);
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
            var boundaryChainIndex = FindSupersessionIndex(
                chain,
                startCheckpointIndex,
                endCheckpointIndex - 1,
                boundary);
            var offset = checked(boundaryChainIndex - startCheckpointIndex);
            if (boundaryChainIndex < 0 ||
                offset < 0 ||
                offset >= supersessions.Length ||
                !global::System.Object.ReferenceEquals(
                    supersessions[offset],
                    boundary))
            {
                return QueryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousCollectionSequenceStatus
                        .RangeSupersessionMismatch,
                    sequence);
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
        var startCollectionPairIndex =
            FindCollectionPairIndex(sequence, startCheckpointIndex);
        var endCollectionPairIndex =
            FindCollectionPairIndex(sequence, endCheckpointIndex);

        var query =
            new HostRuntimeRecoveryMultiCollectionCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion>(
                    queryId,
                    sequence,
                    checkpoints,
                    supersessions,
                    crossedBoundaries.ToArray(),
                    incomingSupersession,
                    outgoingSupersession,
                    startCheckpointIndex,
                    endCheckpointIndex,
                    startCollectionPairIndex,
                    endCollectionPairIndex,
                    queriedTick,
                    checked(sequence.Revision + 1));

        return new HostRuntimeRecoveryMultiCollectionCheckpointRangeQueryResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousCollectionSequenceStatus
                    .MultiCollectionCheckpointRangeQueried,
                sequence,
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

    private static int FindCollectionPairIndex<TRequest, TState, TCompletion>(
        HostRuntimeRecoveryContinuousCollectionSequenceValidation<
            TRequest,
            TState,
            TCompletion> sequence,
        int checkpointIndex)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        for (var index = 0;
            index < sequence.CollectionPairSummaries.Count;
            index++)
        {
            var summary = sequence.CollectionPairSummaries[index];
            if (checkpointIndex >= summary.StartCheckpointIndex &&
                checkpointIndex <= summary.EndCheckpointIndex)
            {
                return index;
            }
        }

        return -1;
    }

    private static HostRuntimeRecoveryContinuousCollectionSequenceValidationResult<
        TRequest,
        TState,
        TCompletion> SequenceResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousCollectionSequenceStatus status,
            global::System.Collections.Generic.IReadOnlyList<
                HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
                    TRequest,
                    TState,
                    TCompletion>> summaries)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, summaries, validation: null);

    private static HostRuntimeRecoveryMultiCollectionCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousCollectionSequenceStatus status,
            HostRuntimeRecoveryContinuousCollectionSequenceValidation<
                TRequest,
                TState,
                TCompletion> sequence)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, sequence, query: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery continuous collection-sequence identifiers cannot be empty.",
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
                "Recovery continuous collection-sequence revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery continuous collection-sequence ticks cannot be negative.");
        }
    }
}
