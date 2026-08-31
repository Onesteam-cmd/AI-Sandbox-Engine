namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Validates bounded ordered sequences of exact continuous-window pairs and
/// resolves bounded inclusive ranges across their validated window boundaries
/// without discovery, reordering, storage, indexing, history mutation,
/// archival, compaction, pagination, diagnostics, scheduling, supervision,
/// waiting, restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryContinuousWindowSequenceFlow
{
    /// <summary>Maximum pair summaries represented by one sequence.</summary>
    public const int MaximumPairCount = 8;

    /// <summary>Maximum checkpoints represented by one multi-window query.</summary>
    public const int MaximumCheckpointCount =
        HostRuntimeRecoveryContinuousWindowPairFlow.MaximumCheckpointCount;

    /// <summary>Validates one bounded exact sequence of continuous-window pairs.</summary>
    public static HostRuntimeRecoveryContinuousWindowSequenceValidationResult<
        TRequest,
        TState,
        TCompletion> ValidateSequence<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryContinuousWindowSequenceValidationIdKind> validationId,
            global::System.Collections.Generic.IReadOnlyList<
                HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                    TRequest,
                    TState,
                    TCompletion>> pairSummaries,
            global::System.Collections.Generic.IReadOnlyList<long>
                expectedPairSummaryRevisions,
            long validatedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(validationId.IsEmpty, nameof(validationId));
        global::System.ArgumentNullException.ThrowIfNull(pairSummaries);
        global::System.ArgumentNullException.ThrowIfNull(expectedPairSummaryRevisions);
        EnsureTick(validatedTick, nameof(validatedTick));

        var snapshot =
            new HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                TRequest,
                TState,
                TCompletion>[pairSummaries.Count];
        for (var index = 0; index < snapshot.Length; index++)
        {
            global::System.ArgumentNullException.ThrowIfNull(pairSummaries[index]);
            snapshot[index] = pairSummaries[index];
        }

        var readOnlySnapshot = global::System.Array.AsReadOnly(snapshot);
        if (snapshot.Length == 0)
        {
            return SequenceResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowSequenceStatus.PairCollectionEmpty,
                readOnlySnapshot);
        }
        if (snapshot.Length > MaximumPairCount)
        {
            return SequenceResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowSequenceStatus.PairCollectionTooLarge,
                readOnlySnapshot);
        }
        if (expectedPairSummaryRevisions.Count != snapshot.Length)
        {
            return SequenceResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowSequenceStatus
                    .PairRevisionCountMismatch,
                readOnlySnapshot);
        }

        var first = snapshot[0];
        var sourceProjection = first.SourceProjection;
        var chain = first.Chain;
        var pairIds = new global::System.Collections.Generic.HashSet<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryContinuousWindowPairSummaryProjectionIdKind>>();
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
            var expectedRevision = expectedPairSummaryRevisions[index];
            EnsureRevision(expectedRevision, nameof(expectedPairSummaryRevisions));

            if (current.Revision != expectedRevision)
            {
                return SequenceResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousWindowSequenceStatus
                        .StalePairSummaryRevision,
                    readOnlySnapshot);
            }
            if (validatedTick < current.ProjectedTick)
            {
                return SequenceResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousWindowSequenceStatus
                        .SequenceValidationTickRegressed,
                    readOnlySnapshot);
            }
            if (!pairIds.Add(current.SummaryId))
            {
                return SequenceResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousWindowSequenceStatus
                        .DuplicatePairSummaryId,
                    readOnlySnapshot);
            }
            if (!global::System.Object.ReferenceEquals(
                    current.SourceProjection,
                    sourceProjection) ||
                !global::System.Object.ReferenceEquals(current.Chain, chain))
            {
                return SequenceResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousWindowSequenceStatus
                        .PairSourceMismatch,
                    readOnlySnapshot);
            }
            if (current.StartCheckpointIndex < 0 ||
                current.EndCheckpointIndex <= current.StartCheckpointIndex ||
                current.EndCheckpointIndex > chain.SupersessionCount ||
                current.ConnectingPriorCheckpointIndex < current.StartCheckpointIndex ||
                current.ConnectingPriorCheckpointIndex >= current.EndCheckpointIndex ||
                current.ConnectingSuccessorCheckpointIndex !=
                    current.ConnectingPriorCheckpointIndex + 1 ||
                !global::System.Object.ReferenceEquals(
                    chain.Supersessions[current.ConnectingPriorCheckpointIndex],
                    current.ConnectingSupersession) ||
                !global::System.Object.ReferenceEquals(
                    current.ConnectingSupersession.PriorCheckpoint,
                    CheckpointAt(chain, current.ConnectingPriorCheckpointIndex)) ||
                !global::System.Object.ReferenceEquals(
                    current.ConnectingSupersession.SuccessorCheckpoint,
                    CheckpointAt(chain, current.ConnectingSuccessorCheckpointIndex)))
            {
                return SequenceResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousWindowSequenceStatus
                        .PairSupersessionMismatch,
                    readOnlySnapshot);
            }

            if (index > 0)
            {
                var previous = snapshot[index - 1];
                if (previous.EndCheckpointIndex + 1 != current.StartCheckpointIndex ||
                    previous.EndCheckpointIndex < 0 ||
                    previous.EndCheckpointIndex >= chain.SupersessionCount)
                {
                    return SequenceResult<TRequest, TState, TCompletion>(
                        HostRuntimeRecoveryContinuousWindowSequenceStatus
                            .SequenceNotContinuous,
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
                        HostRuntimeRecoveryContinuousWindowSequenceStatus
                            .SequenceBoundarySupersessionMismatch,
                        readOnlySnapshot);
                }

                boundarySupersessions[boundaryIndex++] = sequenceBoundary;
            }

            boundarySupersessions[boundaryIndex++] = current.ConnectingSupersession;
            maximumRevision = global::System.Math.Max(maximumRevision, current.Revision);
        }

        var validation =
            new HostRuntimeRecoveryContinuousWindowSequenceValidation<
                TRequest,
                TState,
                TCompletion>(
                    validationId,
                    snapshot,
                    boundarySupersessions,
                    validatedTick,
                    checked(maximumRevision + 1));

        return new HostRuntimeRecoveryContinuousWindowSequenceValidationResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousWindowSequenceStatus
                    .ContinuousWindowSequenceValidated,
                readOnlySnapshot,
                validation);
    }

    /// <summary>
    /// Resolves one exact bounded inclusive checkpoint range that crosses at
    /// least one validated window boundary in a continuous-window sequence.
    /// </summary>
    public static HostRuntimeRecoveryMultiWindowCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryRange<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryMultiWindowCheckpointRangeQueryIdKind> queryId,
            HostRuntimeRecoveryContinuousWindowSequenceValidation<
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
                HostRuntimeRecoveryContinuousWindowSequenceStatus
                    .StaleSequenceRevision,
                sequence);
        }
        if (queriedTick < sequence.ValidatedTick)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowSequenceStatus
                    .MultiWindowRangeQueryTickRegressed,
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
                HostRuntimeRecoveryContinuousWindowSequenceStatus.RangeStartNotFound,
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
                HostRuntimeRecoveryContinuousWindowSequenceStatus.RangeEndNotFound,
                sequence);
        }
        if (endCheckpointIndex < startCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowSequenceStatus.RangeOrderInvalid,
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
                HostRuntimeRecoveryContinuousWindowSequenceStatus
                    .RangeDoesNotCrossWindowBoundary,
                sequence);
        }

        var checkpointCount = checked(endCheckpointIndex - startCheckpointIndex + 1);
        if (checkpointCount > MaximumCheckpointCount)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowSequenceStatus.RangeTooLarge,
                sequence);
        }

        var checkpoints = new HostRuntimeRecoveryCheckpoint<TRequest>[checkpointCount];
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
            supersessions[index] = chain.Supersessions[
                checked(startCheckpointIndex + index)];
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
                    HostRuntimeRecoveryContinuousWindowSequenceStatus
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
        var startPairIndex = FindPairIndex(sequence, startCheckpointIndex);
        var endPairIndex = FindPairIndex(sequence, endCheckpointIndex);

        var query =
            new HostRuntimeRecoveryMultiWindowCheckpointRangeQuery<
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
                    startPairIndex,
                    endPairIndex,
                    queriedTick,
                    checked(sequence.Revision + 1));

        return new HostRuntimeRecoveryMultiWindowCheckpointRangeQueryResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousWindowSequenceStatus
                    .MultiWindowCheckpointRangeQueried,
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
        for (var index = startCheckpointIndex; index <= endCheckpointIndex; index++)
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

    private static int FindPairIndex<TRequest, TState, TCompletion>(
        HostRuntimeRecoveryContinuousWindowSequenceValidation<
            TRequest,
            TState,
            TCompletion> sequence,
        int checkpointIndex)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        for (var index = 0; index < sequence.PairSummaries.Count; index++)
        {
            var pair = sequence.PairSummaries[index];
            if (checkpointIndex >= pair.StartCheckpointIndex &&
                checkpointIndex <= pair.EndCheckpointIndex)
            {
                return index;
            }
        }

        return -1;
    }

    private static HostRuntimeRecoveryContinuousWindowSequenceValidationResult<
        TRequest,
        TState,
        TCompletion> SequenceResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousWindowSequenceStatus status,
            global::System.Collections.Generic.IReadOnlyList<
                HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                    TRequest,
                    TState,
                    TCompletion>> pairSummaries)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, pairSummaries, validation: null);

    private static HostRuntimeRecoveryMultiWindowCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousWindowSequenceStatus status,
            HostRuntimeRecoveryContinuousWindowSequenceValidation<
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
                "Recovery continuous-window sequence identifiers cannot be empty.",
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
                "Recovery continuous-window sequence revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery continuous-window sequence ticks cannot be negative.");
        }
    }
}
