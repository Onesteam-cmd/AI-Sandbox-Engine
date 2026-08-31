namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Validates bounded ordered sequences of exact continuous multi-collection-sequence-sequence-sequence summaries
/// and resolves bounded inclusive ranges across their validated multi-collection-sequence-sequence-sequence boundaries
/// without discovery, reordering, storage, indexing,
/// history mutation, archival, compaction, pagination, diagnostics,
/// scheduling, supervision, waiting, restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceFlow
{
    /// <summary>Maximum summary projections represented by one sequence.</summary>
    public const int MaximumSummaryCount = 8;

    /// <summary>Maximum checkpoints represented by one multi-collection-sequence-sequence-sequence-sequence query.</summary>
    public const int MaximumCheckpointCount =
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryFlow.MaximumCheckpointCount;

    /// <summary>Validates one bounded exact sequence of continuous multi-collection-sequence-sequence-sequence summaries.</summary>
    public static HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidationResult<
        TRequest,
        TState,
        TCompletion> ValidateSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidationIdKind> validationId,
            global::System.Collections.Generic.IReadOnlyList<
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection<
                    TRequest,
                    TState,
                    TCompletion>> multiCollectionSequenceSequenceSequenceSummaries,
            global::System.Collections.Generic.IReadOnlyList<long>
                expectedSummaryRevisions,
            long validatedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(validationId.IsEmpty, nameof(validationId));
        global::System.ArgumentNullException.ThrowIfNull(multiCollectionSequenceSequenceSequenceSummaries);
        global::System.ArgumentNullException.ThrowIfNull(expectedSummaryRevisions);
        EnsureTick(validatedTick, nameof(validatedTick));

        var snapshot =
            new HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion>[multiCollectionSequenceSequenceSequenceSummaries.Count];
        for (var index = 0; index < snapshot.Length; index++)
        {
            global::System.ArgumentNullException.ThrowIfNull(
                multiCollectionSequenceSequenceSequenceSummaries[index]);
            snapshot[index] = multiCollectionSequenceSequenceSequenceSummaries[index];
        }

        var readOnlySnapshot = global::System.Array.AsReadOnly(snapshot);
        if (snapshot.Length == 0)
        {
            return SequenceResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
                    .MultiCollectionSequenceSequenceSequenceSequenceEmpty,
                readOnlySnapshot);
        }
        if (snapshot.Length > MaximumSummaryCount)
        {
            return SequenceResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
                    .MultiCollectionSequenceSequenceSequenceSequenceTooLarge,
                readOnlySnapshot);
        }
        if (expectedSummaryRevisions.Count != snapshot.Length)
        {
            return SequenceResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
                    .MultiCollectionSequenceSequenceSequenceSummaryRevisionCountMismatch,
                readOnlySnapshot);
        }

        var first = snapshot[0];
        var sourceSequence = first.SourceSequence;
        var sourceCollection = first.SourceCollection;
        var sourceProjection = first.SourceProjection;
        var chain = first.Chain;
        var summaryIds = new global::System.Collections.Generic.HashSet<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjectionIdKind>>();
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
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
                        .StaleMultiCollectionSequenceSequenceSequenceSummaryRevision,
                    readOnlySnapshot);
            }
            if (validatedTick < current.ProjectedTick)
            {
                return SequenceResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
                        .MultiCollectionSequenceSequenceSequenceSequenceValidationTickRegressed,
                    readOnlySnapshot);
            }
            if (!summaryIds.Add(current.SummaryId))
            {
                return SequenceResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
                        .DuplicateMultiCollectionSequenceSequenceSequenceSummaryId,
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
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
                        .MultiCollectionSequenceSequenceSequenceSummarySourceMismatch,
                    readOnlySnapshot);
            }
            if (current.StartSequenceSequenceSummaryIndex < 0 ||
                current.EndSequenceSequenceSummaryIndex <= current.StartSequenceSequenceSummaryIndex ||
                current.EndSequenceSequenceSummaryIndex >=
                    sourceSequence.MultiCollectionSequenceSequenceSummaryCount ||
                current.StartCheckpointIndex < 0 ||
                current.EndCheckpointIndex <= current.StartCheckpointIndex ||
                current.EndCheckpointIndex > chain.SupersessionCount ||
                current.ConnectingPriorSequenceSequenceSummaryIndex < current.StartSequenceSequenceSummaryIndex ||
                current.ConnectingPriorSequenceSequenceSummaryIndex >= current.EndSequenceSequenceSummaryIndex ||
                current.ConnectingSuccessorSequenceSequenceSummaryIndex !=
                    current.ConnectingPriorSequenceSequenceSummaryIndex + 1 ||
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
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
                        .MultiCollectionSequenceSequenceSequenceSummarySupersessionMismatch,
                    readOnlySnapshot);
            }

            if (index > 0)
            {
                var previous = snapshot[index - 1];
                if (previous.EndSequenceSequenceSummaryIndex + 1 != current.StartSequenceSequenceSummaryIndex)
                {
                    return SequenceResult<TRequest, TState, TCompletion>(
                        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
                            .MultiCollectionSequenceSequenceSequenceSummaryRangeNotContinuous,
                        readOnlySnapshot);
                }
                if (previous.EndCheckpointIndex + 1 !=
                        current.StartCheckpointIndex ||
                    previous.EndCheckpointIndex < 0 ||
                    previous.EndCheckpointIndex >= chain.SupersessionCount)
                {
                    return SequenceResult<TRequest, TState, TCompletion>(
                        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
                            .MultiCollectionSequenceSequenceSequenceSummaryCheckpointRangeNotContinuous,
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
                        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
                            .MultiCollectionSequenceSequenceSequenceSequenceBoundarySupersessionMismatch,
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
            new HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidation<
                TRequest,
                TState,
                TCompletion>(
                    validationId,
                    snapshot,
                    boundarySupersessions,
                    validatedTick,
                    checked(maximumRevision + 1));

        return new HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidationResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
                    .ContinuousMultiCollectionSequenceSequenceSequenceSequenceValidated,
                readOnlySnapshot,
                validation);
    }

    /// <summary>
    /// Resolves one exact bounded inclusive checkpoint range that crosses at
    /// least one validated multi-collection-sequence boundary in a multi-collection-sequence sequence.
    /// </summary>
    public static HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryRange<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQueryIdKind> queryId,
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidation<
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
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
                    .StaleMultiCollectionSequenceSequenceSequenceSequenceRevision,
                sequence);
        }
        if (queriedTick < sequence.ValidatedTick)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
                    .MultiCollectionSequenceSequenceSequenceSequenceRangeQueryTickRegressed,
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
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
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
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
                    .RangeEndNotFound,
                sequence);
        }
        if (endCheckpointIndex < startCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
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
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
                    .RangeDoesNotCrossMultiCollectionSequenceSequenceSequenceBoundary,
                sequence);
        }

        var checkpointCount =
            checked(endCheckpointIndex - startCheckpointIndex + 1);
        if (checkpointCount > MaximumCheckpointCount)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
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
                    HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
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
            new HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQuery<
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

        return new HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQueryResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
                    .MultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQueried,
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
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidation<
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
            index < sequence.MultiCollectionSequenceSequenceSequenceSummaries.Count;
            index++)
        {
            var summary = sequence.MultiCollectionSequenceSequenceSequenceSummaries[index];
            if (checkpointIndex >= summary.StartCheckpointIndex &&
                checkpointIndex <= summary.EndCheckpointIndex)
            {
                return index;
            }
        }

        return -1;
    }

    private static HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidationResult<
        TRequest,
        TState,
        TCompletion> SequenceResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus status,
            global::System.Collections.Generic.IReadOnlyList<
                HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection<
                    TRequest,
                    TState,
                    TCompletion>> summaries)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, summaries, validation: null);

    private static HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus status,
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidation<
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
                "Recovery continuous multi-collection-sequence-sequence-sequence sequence identifiers cannot be empty.",
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
                "Recovery continuous multi-collection-sequence-sequence-sequence sequence revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery continuous multi-collection-sequence-sequence-sequence sequence ticks cannot be negative.");
        }
    }
}
