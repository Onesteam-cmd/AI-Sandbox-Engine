namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Projects compact summaries over exact continuous recovery-window pairs and
/// resolves bounded inclusive ranges crossing their shared boundary without
/// discovery, reordering, storage, indexing, history mutation, archival,
/// compaction, pagination, diagnostics, scheduling, supervision, waiting,
/// restart, transport, or execution.
/// </summary>
public static class HostRuntimeRecoveryContinuousWindowPairFlow
{
    /// <summary>Maximum checkpoints represented by one cross-window query.</summary>
    public const int MaximumCheckpointCount =
        HostRuntimeRecoveryLineageWindowFlow.MaximumCheckpointCount;

    /// <summary>Projects one compact summary over an exact validated window pair.</summary>
    public static HostRuntimeRecoveryContinuousWindowPairSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> ProjectPair<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryContinuousWindowPairSummaryProjectionIdKind> summaryId,
            HostRuntimeRecoveryCheckpointRangeContinuityValidation<
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
            return PairResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowPairStatus
                    .StaleContinuityRevision,
                continuity);
        }
        if (projectedTick < continuity.ValidatedTick)
        {
            return PairResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowPairStatus
                    .PairSummaryProjectionTickRegressed,
                continuity);
        }

        var summary = continuity.Summary;
        var adjacentWindow = continuity.AdjacentWindow;
        if (!global::System.Object.ReferenceEquals(
                adjacentWindow.Summary,
                summary))
        {
            return PairResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowPairStatus
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
            TCompletion>? windowBoundary;

        if (continuity.ValidatesPreviousWindow)
        {
            if (adjacentWindow.EndCheckpointIndex + 1 != summary.StartChainIndex)
            {
                return PairResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousWindowPairStatus
                        .WindowPairNotContinuous,
                    continuity);
            }

            priorCheckpoint = adjacentWindow.EndCheckpoint;
            successorCheckpoint = summary.StartCheckpoint;
            summaryBoundary = summary.IncomingSupersession;
            windowBoundary = adjacentWindow.OutgoingSupersession;
        }
        else
        {
            if (summary.EndChainIndex + 1 != adjacentWindow.StartCheckpointIndex)
            {
                return PairResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoveryContinuousWindowPairStatus
                        .WindowPairNotContinuous,
                    continuity);
            }

            priorCheckpoint = summary.EndCheckpoint;
            successorCheckpoint = adjacentWindow.StartCheckpoint;
            summaryBoundary = summary.OutgoingSupersession;
            windowBoundary = adjacentWindow.IncomingSupersession;
        }

        if (summaryBoundary is null ||
            !global::System.Object.ReferenceEquals(summaryBoundary, windowBoundary) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary,
                continuity.ConnectingSupersession))
        {
            return PairResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowPairStatus
                    .WindowPairSupersessionMismatch,
                continuity);
        }
        if (!global::System.Object.ReferenceEquals(
                summaryBoundary.PriorCheckpoint,
                priorCheckpoint) ||
            !global::System.Object.ReferenceEquals(
                summaryBoundary.SuccessorCheckpoint,
                successorCheckpoint))
        {
            return PairResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowPairStatus
                    .WindowPairCheckpointMismatch,
                continuity);
        }

        var pairSummary =
            new HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                TRequest,
                TState,
                TCompletion>(
                    summaryId,
                    continuity,
                    projectedTick,
                    checked(continuity.Revision + 1));

        return new HostRuntimeRecoveryContinuousWindowPairSummaryProjectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousWindowPairStatus
                    .ContinuousWindowPairSummaryProjected,
                continuity,
                pairSummary);
    }

    /// <summary>
    /// Resolves one exact bounded inclusive checkpoint range that crosses the
    /// shared supersession boundary of a continuous recovery-window pair.
    /// </summary>
    public static HostRuntimeRecoveryCrossWindowCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryRange<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCrossWindowCheckpointRangeQueryIdKind> queryId,
            HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                TRequest,
                TState,
                TCompletion> pairSummary,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> startCheckpointId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryCheckpointIdKind> endCheckpointId,
            long expectedPairSummaryRevision,
            long queriedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(queryId.IsEmpty, nameof(queryId));
        global::System.ArgumentNullException.ThrowIfNull(pairSummary);
        EnsureId(startCheckpointId.IsEmpty, nameof(startCheckpointId));
        EnsureId(endCheckpointId.IsEmpty, nameof(endCheckpointId));
        EnsureRevision(expectedPairSummaryRevision, nameof(expectedPairSummaryRevision));
        EnsureTick(queriedTick, nameof(queriedTick));

        if (pairSummary.Revision != expectedPairSummaryRevision)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowPairStatus
                    .StalePairSummaryRevision,
                pairSummary);
        }
        if (queriedTick < pairSummary.ProjectedTick)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowPairStatus
                    .CrossWindowRangeQueryTickRegressed,
                pairSummary);
        }

        var chain = pairSummary.Chain;
        var startCheckpointIndex = FindCheckpointIndex(
            chain,
            pairSummary.StartCheckpointIndex,
            pairSummary.EndCheckpointIndex,
            startCheckpointId);
        if (startCheckpointIndex < 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowPairStatus.RangeStartNotFound,
                pairSummary);
        }

        var endCheckpointIndex = FindCheckpointIndex(
            chain,
            pairSummary.StartCheckpointIndex,
            pairSummary.EndCheckpointIndex,
            endCheckpointId);
        if (endCheckpointIndex < 0)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowPairStatus.RangeEndNotFound,
                pairSummary);
        }
        if (endCheckpointIndex < startCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowPairStatus.RangeOrderInvalid,
                pairSummary);
        }
        if (startCheckpointIndex > pairSummary.ConnectingPriorCheckpointIndex ||
            endCheckpointIndex < pairSummary.ConnectingSuccessorCheckpointIndex)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowPairStatus
                    .RangeDoesNotCrossWindowBoundary,
                pairSummary);
        }

        var checkpointCount =
            checked(endCheckpointIndex - startCheckpointIndex + 1);
        if (checkpointCount > MaximumCheckpointCount)
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowPairStatus.RangeTooLarge,
                pairSummary);
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
            pairSummary.ConnectingPriorCheckpointIndex - startCheckpointIndex);
        if (connectingOffset < 0 ||
            connectingOffset >= supersessions.Length ||
            !global::System.Object.ReferenceEquals(
                supersessions[connectingOffset],
                pairSummary.ConnectingSupersession))
        {
            return QueryResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoveryContinuousWindowPairStatus
                    .RangeSupersessionMismatch,
                pairSummary);
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
            new HostRuntimeRecoveryCrossWindowCheckpointRangeQuery<
                TRequest,
                TState,
                TCompletion>(
                    queryId,
                    pairSummary,
                    checkpoints,
                    supersessions,
                    incomingSupersession,
                    outgoingSupersession,
                    startCheckpointIndex,
                    endCheckpointIndex,
                    queriedTick,
                    checked(pairSummary.Revision + 1));

        return new HostRuntimeRecoveryCrossWindowCheckpointRangeQueryResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoveryContinuousWindowPairStatus
                    .CrossWindowCheckpointRangeQueried,
                pairSummary,
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

    private static HostRuntimeRecoveryContinuousWindowPairSummaryProjectionResult<
        TRequest,
        TState,
        TCompletion> PairResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousWindowPairStatus status,
            HostRuntimeRecoveryCheckpointRangeContinuityValidation<
                TRequest,
                TState,
                TCompletion> continuity)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, continuity, summary: null);

    private static HostRuntimeRecoveryCrossWindowCheckpointRangeQueryResult<
        TRequest,
        TState,
        TCompletion> QueryResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoveryContinuousWindowPairStatus status,
            HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                TRequest,
                TState,
                TCompletion> pairSummary)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, pairSummary, query: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new global::System.ArgumentException(
                "Recovery continuous-window pair identifiers cannot be empty.",
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
                "Recovery continuous-window pair revisions cannot be negative.");
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new global::System.ArgumentOutOfRangeException(
                parameterName,
                tick,
                "Recovery continuous-window pair ticks cannot be negative.");
        }
    }
}
