namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Validates bounded ordered sequences of exact continuous multi-collection summaries
/// and resolves bounded inclusive ranges across their validated collection-sequence boundaries
/// without discovery, reordering, storage, indexing,
/// history mutation, archival, compaction, pagination, diagnostics,
/// scheduling, supervision, waiting, restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryContinuousMultiCollectionSequenceFlow
{
    /// <summary>Maximum summary projections represented by one sequence.</summary>
    public const int MaximumSummaryCount = 8;

    /// <summary>Maximum checkpoints represented by one multi-collection-sequence query.</summary>
    public const int MaximumCheckpointCount =
        HostRuntimeRecoveryContinuousMultiCollectionSummaryFlow.MaximumCheckpointCount;

    /// <summary>Validates one bounded exact sequence of continuous multi-collection summaries.</summary>
    public static HostRuntimeRecoveryContinuousMultiCollectionSequenceValidationResult<
        TRequest,
        TState,
        TCompletion> ValidateSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryContinuousMultiCollectionSequenceValidationIdKind> validationId,
            global::System.Collections.Generic.IReadOnlyList<
                HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
                    TRequest,
                    TState,
                    TCompletion>> multiCollectionSummaries,
            global::System.Collections.Generic.IReadOnlyList<long>
                expectedSummaryRevisions,
            long validatedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(validationId.IsEmpty, nameof(validationId));
        global::System.ArgumentNullException.ThrowIfNull(multiCollectionSummaries);
        global::System.ArgumentNullException.ThrowIfNull(expectedSummaryRevisions);
        EnsureTick(validatedTick, nameof(validatedTick));

        var snapshot =
            new HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
                TRequest,
                TState,
                TCompletion>[multiCollectionSummaries.Count];
        for (var index = 0; index < snapshot.Length; index++)
        {
            global::System.ArgumentNullException.ThrowIfNull(
                multiCollectionSummaries[index]);
            snapshot[index] = multiCollectionSummaries[index];
        }

        var readOnlySnapshot = global::System.Array.AsReadOnly(snapshot);
        if (snapshot.Length == 0)
        {
            return SequenceResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
                    .MultiCollectionSequenceEmpty,
                readOnlySnapshot);
        }
        if (snapshot.Length > MaximumSummaryCount)
        {
            return SequenceResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
                    .MultiCollectionSequenceTooLarge,
                readOnlySnapshot);
        }
        if (expectedSummaryRevisions.Count != snapshot.Length)
        {
            return SequenceResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
                    .MultiCollectionSummaryRevisionCountMismatch,
                readOnlySnapshot);
        }

        var first = snapshot[0];
        var sourceSequence = first.SourceSequence;
        var sourceCollection = first.SourceCollection;
        var sourceProjection = first.SourceProjection;
        var chain = first.Chain;
        var summaryIds = new global::System.Collections.Generic.HashSet<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionIdKind>>();
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
                return SequenceResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
                        .StaleMultiCollectionSummaryRevision,
                    readOnlySnapshot);
            }
            if (validatedTick < current.ProjectedTick)
            {
                return SequenceResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
                        .MultiCollectionSequenceValidationTickRegressed,
                    readOnlySnapshot);
            }
            if (!summaryIds.Add(current.SummaryId))
            {
                return SequenceResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
                        .DuplicateMultiCollectionSummaryId,
                    readOnlySnapshot);
            }
            if (!global::System.Object.ReferenceEquals(
                    current.SourceSequence,
                    sourceSequence) ||
                !global::System.Object.ReferenceEquals(
                    current.SourceCollection,
                    sourceCollection) ||
                !global::System.Object.ReferenceEquals(
                    current.SourceProjection,
                    sourceProjection) ||
                !global::System.Object.ReferenceEquals(current.Chain, chain))
            {
                return SequenceResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
                        .MultiCollectionSummarySourceMismatch,
                    readOnlySnapshot);
            }
            if (current.StartCollectionPairIndex < 0 ||
                current.EndCollectionPairIndex <= current.StartCollectionPairIndex ||
                current.EndCollectionPairIndex >=
                    sourceSequence.CollectionPairCount ||
                current.StartCheckpointIndex < 0 ||
                current.EndCheckpointIndex <= current.StartCheckpointIndex ||
                current.EndCheckpointIndex > chain.SupersessionCount ||
                current.ConnectingPriorCollectionPairIndex < current.StartCollectionPairIndex ||
                current.ConnectingPriorCollectionPairIndex >= current.EndCollectionPairIndex ||
                current.ConnectingSuccessorCollectionPairIndex !=
                    current.ConnectingPriorCollectionPairIndex + 1 ||
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
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
                        .MultiCollectionSummarySupersessionMismatch,
                    readOnlySnapshot);
            }

            if (index > 0)
            {
                var previous = snapshot[index - 1];
                if (previous.EndCollectionPairIndex + 1 != current.StartCollectionPairIndex)
                {
                    return SequenceResult<TRequest, TState, TCompletion>(
                        HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
                            .MultiCollectionSummaryPairRangeNotContinuous,
                        readOnlySnapshot);
                }
                if (previous.EndCheckpointIndex + 1 !=
                        current.StartCheckpointIndex ||
                    previous.EndCheckpointIndex < 0 ||
                    previous.EndCheckpointIndex >= chain.SupersessionCount)
                {
                    return SequenceResult<TRequest, TState, TCompletion>(
                        HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
                            .MultiCollectionSummaryCheckpointRangeNotContinuous,
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
                        HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
                            .MultiCollectionSequenceBoundarySupersessionMismatch,
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
            new HostRuntimeRecoveryContinuousMultiCollectionSequenceValidation<
                TRequest,
                TState,
                TCompletion>(
                    validationId,
                    snapshot,
                    boundarySupersessions,
                    validatedTick,
                    checked(maximumRevision + 1));

        return new HostRuntimeRecoveryContinuousMultiCollectionSequenceValidationResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
                    .ContinuousMultiCollectionSequenceValidated,
                readOnlySnapshot,
                validation);
    }

    /// <summary>
    /// Resolves one exact bounded inclusive checkpoint range that crosses at
    /// least one validated collection-sequence boundary in a multi-collection sequence.
    /// </summary>
    public static HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryRange<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQueryIdKind> queryId,
            HostRuntimeRecoveryContinuousMultiCollectionSequenceValidation<
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
                HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
                    .StaleMultiCollectionSequenceRevision,
                sequence);
        }
        if (queriedTick < sequence.ValidatedTick)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
                    .MultiCollectionSequenceRangeQueryTickRegressed,
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
                HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
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
                HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
                    .RangeEndNotFound,
                sequence);
        }
        if (endCheckpointIndex < startCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
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
            var boundaryIndex = FindSupersessionIndex(
                chain,
                sequence.StartCheckpointIndex,
                sequence.EndCheckpointIndex - 1,
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
                HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
                    .RangeDoesNotCrossCollectionSequenceBoundary,
                sequence);
        }

        var checkpointCount =
            checked(endCheckpointIndex - startCheckpointIndex + 1);
        if (checkpointCount > MaximumCheckpointCount)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
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
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
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
        var startSummaryIndex =
            FindSummaryIndex(sequence, startCheckpointIndex);
        var endSummaryIndex =
            FindSummaryIndex(sequence, endCheckpointIndex);

        var query =
            new HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQuery<
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
                    startSummaryIndex,
                    endSummaryIndex,
                    queriedTick,
                    checked(sequence.Revision + 1));

        return new HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQueryResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
                    .MultiCollectionSequenceCheckpointRangeQueried,
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

    private static int FindSummaryIndex<TRequest, TState, TCompletion>(
        HostRuntimeRecoveryContinuousMultiCollectionSequenceValidation<
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
            index < sequence.MultiCollectionSummaries.Count;
            index++)
        {
            var summary = sequence.MultiCollectionSummaries[index];
            if (checkpointIndex >= summary.StartCheckpointIndex &&
                checkpointIndex <= summary.EndCheckpointIndex)
            {
                return index;
            }
        }

        return -1;
    }

    private static HostRuntimeRecoveryContinuousMultiCollectionSequenceValidationResult<
        TRequest,
        TState,
        TCompletion> SequenceResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus status,
            global::System.Collections.Generic.IReadOnlyList<
                HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
                    TRequest,
                    TState,
                    TCompletion>> summaries)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, summaries, validation: null);

    private static HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus status,
            HostRuntimeRecoveryContinuousMultiCollectionSequenceValidation<
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
                "Recovery continuous multi-collection sequence identifiers cannot be empty.",
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
                "Recovery continuous multi-collection sequence revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery continuous multi-collection sequence ticks cannot be negative.");
        }
    }
}
