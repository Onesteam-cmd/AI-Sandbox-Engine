namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects compact summaries over one exact continuous recovery multi-sequence
/// formed by a summarized range and one projected adjacent sequence, and resolves
/// bounded inclusive ranges crossing their shared boundary without
/// discovery, reordering, storage, indexing, history mutation, archival,
/// compaction, pagination, diagnostics, scheduling, supervision, waiting,
/// restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryContinuousMultiSequenceFlow
{
    /// <summary>Maximum checkpoints represented by one cross-sequence query.</summary>
    public const int MaximumCheckpointCount =
        HostRuntimeRecoveryLineageWindowFlow.MaximumCheckpointCount;

    /// <summary>Projects one compact summary over an exact validated continuous multi-sequence.</summary>
    public static HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectSummary<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionIdKind> summaryId,
            HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation<
                TRequest,
                TState,
                TCompletion> continuity,
            long expectedContinuityRevision,
            long projectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(summaryId.IsEmpty, nameof(summaryId));
        global::System.ArgumentNullException.ThrowIfNull(continuity);
        EnsureRevision(expectedContinuityRevision, nameof(expectedContinuityRevision));
        EnsureTick(projectedTick, nameof(projectedTick));

        if (continuity.Revision != expectedContinuityRevision)
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceStatus
                    .StaleContinuityRevision,
                continuity);
        }
        if (projectedTick < continuity.ValidatedTick)
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceStatus
                    .MultiSequenceSummaryProjectionTickRegressed,
                continuity);
        }

        var summary = continuity.Summary;
        var adjacentSequence = continuity.AdjacentSequence;
        if (!global::System.Object.ReferenceEquals(
                adjacentSequence.Summary,
                summary))
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceStatus
                    .ContinuitySourceMismatch,
                continuity);
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
            TCompletion>? sequenceBoundary;

        if (continuity.ValidatesPreviousSequence)
        {
            if (adjacentSequence.EndPairIndex + 1 != summary.StartPairIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiSequenceStatus
                        .SequencePairNotContinuous,
                    continuity);
            }
            if (adjacentSequence.EndCheckpointIndex + 1 != summary.StartCheckpointIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiSequenceStatus
                        .SequenceCheckpointRangeNotContinuous,
                    continuity);
            }

            priorCheckpoint = adjacentSequence.EndCheckpoint;
            successorCheckpoint = summary.StartCheckpoint;
            summaryBoundary = summary.IncomingSupersession;
            sequenceBoundary = adjacentSequence.OutgoingSupersession;
        }
        else
        {
            if (summary.EndPairIndex + 1 != adjacentSequence.StartPairIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiSequenceStatus
                        .SequencePairNotContinuous,
                    continuity);
            }
            if (summary.EndCheckpointIndex + 1 != adjacentSequence.StartCheckpointIndex)
            {
                return SummaryResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousMultiSequenceStatus
                        .SequenceCheckpointRangeNotContinuous,
                    continuity);
            }

            priorCheckpoint = summary.EndCheckpoint;
            successorCheckpoint = adjacentSequence.StartCheckpoint;
            summaryBoundary = summary.OutgoingSupersession;
            sequenceBoundary = adjacentSequence.IncomingSupersession;
        }

        if (summaryBoundary is null ||
            !global::System.Object.ReferenceEquals(summaryBoundary, sequenceBoundary) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                continuity.ConnectingSupersession))
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceStatus
                    .SequencePairSupersessionMismatch,
                continuity);
        }
        if (!global::System.Object.ReferenceEquals(
                summaryBoundary.PriorCheckpoint,
                priorCheckpoint) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary.SuccessorCheckpoint,
                successorCheckpoint))
        {
            return SummaryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceStatus
                    .SequencePairCheckpointMismatch,
                continuity);
        }

        var multiSequenceSummary =
            new HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion>(
                    summaryId,
                    continuity,
                    projectedTick,
                    checked(continuity.Revision + 1));

        return new HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceStatus
                    .ContinuousMultiSequenceSummaryProjected,
                continuity,
                multiSequenceSummary);
    }

    /// <summary>
    /// Resolves one exact bounded inclusive checkpoint range that crosses the
    /// shared supersession boundary of one continuous recovery multi-sequence.
    /// </summary>
    public static HostRuntimeRecoveryCrossSequenceCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryRange<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCrossSequenceCheckpointRangeQueryIdKind> queryId,
            HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion> multiSequenceSummary,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> startCheckpointId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> endCheckpointId,
            long expectedMultiSequenceSummaryRevision,
            long queriedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(queryId.IsEmpty, nameof(queryId));
        global::System.ArgumentNullException.ThrowIfNull(multiSequenceSummary);
        EnsureId(startCheckpointId.IsEmpty, nameof(startCheckpointId));
        EnsureId(endCheckpointId.IsEmpty, nameof(endCheckpointId));
        EnsureRevision(expectedMultiSequenceSummaryRevision, nameof(expectedMultiSequenceSummaryRevision));
        EnsureTick(queriedTick, nameof(queriedTick));

        if (multiSequenceSummary.Revision != expectedMultiSequenceSummaryRevision)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceStatus
                    .StaleMultiSequenceSummaryRevision,
                multiSequenceSummary);
        }
        if (queriedTick < multiSequenceSummary.ProjectedTick)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceStatus
                    .CrossSequenceRangeQueryTickRegressed,
                multiSequenceSummary);
        }

        var chain = multiSequenceSummary.Chain;
        var startCheckpointIndex = FindCheckpointIndex(
            chain,
            multiSequenceSummary.StartCheckpointIndex,
            multiSequenceSummary.EndCheckpointIndex,
            startCheckpointId);
        if (startCheckpointIndex < 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceStatus.RangeStartNotFound,
                multiSequenceSummary);
        }

        var endCheckpointIndex = FindCheckpointIndex(
            chain,
            multiSequenceSummary.StartCheckpointIndex,
            multiSequenceSummary.EndCheckpointIndex,
            endCheckpointId);
        if (endCheckpointIndex < 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceStatus.RangeEndNotFound,
                multiSequenceSummary);
        }
        if (endCheckpointIndex < startCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceStatus.RangeOrderInvalid,
                multiSequenceSummary);
        }
        if (startCheckpointIndex > multiSequenceSummary.ConnectingPriorCheckpointIndex ||
            endCheckpointIndex < multiSequenceSummary.ConnectingSuccessorCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceStatus
                    .RangeDoesNotCrossSequenceBoundary,
                multiSequenceSummary);
        }

        var checkpointCount =
            checked(endCheckpointIndex - startCheckpointIndex + 1);
        if (checkpointCount > MaximumCheckpointCount)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceStatus.RangeTooLarge,
                multiSequenceSummary);
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
            supersessions[index] = chain.Supersessions[
                checked(startCheckpointIndex + index)];
        }

        var connectingOffset = checked(
            multiSequenceSummary.ConnectingPriorCheckpointIndex - startCheckpointIndex);
        if (connectingOffset < 0 ||
            connectingOffset >= supersessions.Length ||
            !global::System.Object.ReferenceEquals(
                supersessions[connectingOffset],
                multiSequenceSummary.ConnectingSupersession))
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceStatus
                    .RangeSupersessionMismatch,
                multiSequenceSummary);
        }

        var incomingSupersession =
            startCheckpointIndex > 0
                ? chain.Supersessions[startCheckpointIndex - 1]
                : null;
        var outgoingSupersession =
            endCheckpointIndex < chain.SupersessionCount
                ? chain.Supersessions[endCheckpointIndex]
                : null;

        var query =
            new HostRuntimeRecoveryCrossSequenceCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion>(
                    queryId,
                    multiSequenceSummary,
                    checkpoints,
                    supersessions,
                    incomingSupersession,
                    outgoingSupersession,
                    startCheckpointIndex,
                    endCheckpointIndex,
                    queriedTick,
                    checked(multiSequenceSummary.Revision + 1));

        return new HostRuntimeRecoveryCrossSequenceCheckpointRangeQueryResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousMultiSequenceStatus
                    .CrossSequenceCheckpointRangeQueried,
                multiSequenceSummary,
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

    private static HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> SummaryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousMultiSequenceStatus status,
            HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation<
                TRequest,
                TState,
                TCompletion> continuity)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, continuity, summary: null);

    private static HostRuntimeRecoveryCrossSequenceCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousMultiSequenceStatus status,
            HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion> multiSequenceSummary)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, multiSequenceSummary, query: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery continuous multi-sequence identifiers cannot be empty.",
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
                "Recovery continuous multi-sequence revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery continuous multi-sequence ticks cannot be negative.");
        }
    }
}
