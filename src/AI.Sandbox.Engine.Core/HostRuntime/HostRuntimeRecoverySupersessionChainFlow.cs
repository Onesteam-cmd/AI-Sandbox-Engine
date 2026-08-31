namespace AI.Sandbox.Engine.Core.HostRuntime;
/// <summary>
/// Validates bounded caller-supplied recovery supersession chains and selects
/// their exact latest checkpoint without storage, history mutation, archival,
/// compaction, scheduling, supervision, waiting, restart, or execution.
/// </summary>
public static class HostRuntimeRecoverySupersessionChainFlow
{
    /// <summary>Maximum number of supersession edges accepted per validation.</summary>
    public const int MaximumSupersessionCount = 256;

    /// <summary>
    /// Validates one externally ordered bounded checkpoint-supersession chain.
    /// </summary>
    public static HostRuntimeRecoverySupersessionChainResult<
        TRequest,
        TState,
        TCompletion> Validate<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoverySupersessionChainIdKind> chainId,
            global::System.Collections.Generic.IReadOnlyList<
                HostRuntimeRecoveryCheckpointSupersession<
                    TRequest,
                    TState,
                    TCompletion>> supersessions,
            global::System.Collections.Generic.IReadOnlyList<long>
                expectedSupersessionRevisions,
            long validatedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(chainId.IsEmpty, nameof(chainId));
        ArgumentNullException.ThrowIfNull(supersessions);
        ArgumentNullException.ThrowIfNull(expectedSupersessionRevisions);
        EnsureTick(validatedTick, nameof(validatedTick));

        var snapshot =
            new HostRuntimeRecoveryCheckpointSupersession<
                TRequest,
                TState,
                TCompletion>[supersessions.Count];
        for (var index = 0; index < supersessions.Count; index++)
        {
            snapshot[index] = supersessions[index] ??
                throw new ArgumentException(
                    "The supersession collection must not contain null.",
                    nameof(supersessions));
        }

        var readOnlySnapshot = global::System.Array.AsReadOnly(snapshot);
        if (snapshot.Length == 0)
        {
            return ChainResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoverySupersessionChainStatus.EmptyChain,
                readOnlySnapshot);
        }
        if (snapshot.Length > MaximumSupersessionCount)
        {
            return ChainResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoverySupersessionChainStatus
                    .TooManySupersessions,
                readOnlySnapshot);
        }
        if (expectedSupersessionRevisions.Count != snapshot.Length)
        {
            return ChainResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoverySupersessionChainStatus
                    .SupersessionRevisionCountMismatch,
                readOnlySnapshot);
        }

        var supersessionIds =
            new global::System.Collections.Generic.HashSet<
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    HostRuntimeRecoveryCheckpointSupersessionIdKind>>();
        var priorCheckpointIds =
            new global::System.Collections.Generic.HashSet<
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    HostRuntimeRecoveryCheckpointIdKind>>();
        var successorCheckpointIds =
            new global::System.Collections.Generic.HashSet<
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    HostRuntimeRecoveryCheckpointIdKind>>();

        var first = snapshot[0];
        var maximumRevision = first.Revision;

        for (var index = 0; index < snapshot.Length; index++)
        {
            var current = snapshot[index];
            var expectedRevision = expectedSupersessionRevisions[index];
            EnsureRevision(
                expectedRevision,
                nameof(expectedSupersessionRevisions));

            if (current.Revision != expectedRevision)
            {
                return ChainResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoverySupersessionChainStatus
                        .StaleSupersessionRevision,
                    readOnlySnapshot);
            }
            if (!supersessionIds.Add(current.SupersessionId))
            {
                return ChainResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoverySupersessionChainStatus
                        .DuplicateSupersessionId,
                    readOnlySnapshot);
            }

            if (index > 0)
            {
                var previous = snapshot[index - 1];
                if (current.PriorCheckpointId !=
                    previous.SuccessorCheckpointId)
                {
                    return ChainResult<TRequest, TState, TCompletion>(
                        HostRuntimeRecoverySupersessionChainStatus
                            .DisconnectedChain,
                        readOnlySnapshot);
                }
                if (!CheckpointAuthorityMatches(
                    previous.SuccessorCheckpoint,
                    current.PriorCheckpoint))
                {
                    return ChainResult<TRequest, TState, TCompletion>(
                        HostRuntimeRecoverySupersessionChainStatus
                            .CheckpointAuthorityMismatch,
                        readOnlySnapshot);
                }
                if (current.SupersededTick < previous.SupersededTick)
                {
                    return ChainResult<TRequest, TState, TCompletion>(
                        HostRuntimeRecoverySupersessionChainStatus
                            .SupersessionTickRegressed,
                        readOnlySnapshot);
                }
                if (current.SuccessorCheckpoint.Revision <
                    previous.SuccessorCheckpoint.Revision)
                {
                    return ChainResult<TRequest, TState, TCompletion>(
                        HostRuntimeRecoverySupersessionChainStatus
                            .CheckpointRevisionRegressed,
                        readOnlySnapshot);
                }
                if (current.SuccessorCheckpoint.CapturedTick <
                    previous.SuccessorCheckpoint.CapturedTick)
                {
                    return ChainResult<TRequest, TState, TCompletion>(
                        HostRuntimeRecoverySupersessionChainStatus
                            .CheckpointCaptureTickRegressed,
                        readOnlySnapshot);
                }
                if (current.SuccessorCheckpoint.WorldSnapshotDocument
                        .WorldStateVersion.CompareTo(
                            previous.SuccessorCheckpoint.WorldSnapshotDocument
                                .WorldStateVersion) < 0)
                {
                    return ChainResult<TRequest, TState, TCompletion>(
                        HostRuntimeRecoverySupersessionChainStatus
                            .WorldStateVersionRegressed,
                        readOnlySnapshot);
                }
                if (current.SuccessorCheckpoint.WorldSnapshotDocument
                        .SimulationTick <
                    previous.SuccessorCheckpoint.WorldSnapshotDocument
                        .SimulationTick)
                {
                    return ChainResult<TRequest, TState, TCompletion>(
                        HostRuntimeRecoverySupersessionChainStatus
                            .SimulationTickRegressed,
                        readOnlySnapshot);
                }
            }

            if (!priorCheckpointIds.Add(current.PriorCheckpointId))
            {
                return ChainResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoverySupersessionChainStatus
                        .DuplicatePriorCheckpointId,
                    readOnlySnapshot);
            }
            if (!successorCheckpointIds.Add(current.SuccessorCheckpointId))
            {
                return ChainResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoverySupersessionChainStatus
                        .DuplicateSuccessorCheckpointId,
                    readOnlySnapshot);
            }
            if (current.RuntimeInstanceId != first.RuntimeInstanceId)
            {
                return ChainResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoverySupersessionChainStatus.RuntimeMismatch,
                    readOnlySnapshot);
            }
            if (current.CompositionId != first.CompositionId)
            {
                return ChainResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoverySupersessionChainStatus
                        .CompositionMismatch,
                    readOnlySnapshot);
            }
            if (current.QueueId != first.QueueId)
            {
                return ChainResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoverySupersessionChainStatus.QueueMismatch,
                    readOnlySnapshot);
            }
            if (current.ClockId != first.ClockId)
            {
                return ChainResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoverySupersessionChainStatus.ClockMismatch,
                    readOnlySnapshot);
            }
            if (current.WorldId != first.WorldId)
            {
                return ChainResult<TRequest, TState, TCompletion>(
                    HostRuntimeRecoverySupersessionChainStatus.WorldMismatch,
                    readOnlySnapshot);
            }

            maximumRevision =
                global::System.Math.Max(maximumRevision, current.Revision);
        }

        if (priorCheckpointIds.Contains(
            snapshot[^1].SuccessorCheckpointId))
        {
            return ChainResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoverySupersessionChainStatus.CycleDetected,
                readOnlySnapshot);
        }
        if (validatedTick < snapshot[^1].SupersededTick)
        {
            return ChainResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoverySupersessionChainStatus
                    .ValidationTickRegressed,
                readOnlySnapshot);
        }

        var chain =
            new HostRuntimeRecoverySupersessionChain<
                TRequest,
                TState,
                TCompletion>(
                    chainId,
                    snapshot,
                    validatedTick,
                    checked(maximumRevision + 1));

        return new HostRuntimeRecoverySupersessionChainResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoverySupersessionChainStatus.ChainValidated,
                readOnlySnapshot,
                chain);
    }

    /// <summary>
    /// Selects the exact latest checkpoint from one validated chain.
    /// </summary>
    public static HostRuntimeRecoveryLatestCheckpointSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectLatest<TRequest, TState, TCompletion>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryLatestCheckpointSelectionIdKind>
                    selectionId,
            HostRuntimeRecoverySupersessionChain<
                TRequest,
                TState,
                TCompletion> chain,
            long expectedChainRevision,
            long selectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion
    {
        EnsureId(selectionId.IsEmpty, nameof(selectionId));
        ArgumentNullException.ThrowIfNull(chain);
        EnsureRevision(expectedChainRevision, nameof(expectedChainRevision));
        EnsureTick(selectedTick, nameof(selectedTick));

        if (chain.Revision != expectedChainRevision)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoverySupersessionChainStatus.StaleChainRevision,
                chain);
        }
        if (selectedTick < chain.ValidatedTick)
        {
            return SelectionResult<TRequest, TState, TCompletion>(
                HostRuntimeRecoverySupersessionChainStatus
                    .SelectionTickRegressed,
                chain);
        }

        var selection =
            new HostRuntimeRecoveryLatestCheckpointSelection<
                TRequest,
                TState,
                TCompletion>(
                    selectionId,
                    chain,
                    selectedTick,
                    checked(chain.Revision + 1));

        return new HostRuntimeRecoveryLatestCheckpointSelectionResult<
            TRequest,
            TState,
            TCompletion>(
                HostRuntimeRecoverySupersessionChainStatus
                    .LatestCheckpointSelected,
                chain,
                selection);
    }

    private static bool CheckpointAuthorityMatches<TRequest>(
        HostRuntimeRecoveryCheckpoint<TRequest> left,
        HostRuntimeRecoveryCheckpoint<TRequest> right)
        where TRequest : IHostRuntimeRequest =>
        left.CheckpointId == right.CheckpointId &&
        left.Revision == right.Revision &&
        left.CapturedTick == right.CapturedTick &&
        left.RuntimeInstanceId == right.RuntimeInstanceId &&
        left.Composition.CompositionId ==
            right.Composition.CompositionId &&
        left.QueueSnapshot.QueueId == right.QueueSnapshot.QueueId &&
        left.ClockId == right.ClockId &&
        left.WorldSnapshotDocument.WorldId ==
            right.WorldSnapshotDocument.WorldId &&
        left.WorldSnapshotDocument.WorldStateVersion.CompareTo(
            right.WorldSnapshotDocument.WorldStateVersion) == 0 &&
        left.WorldSnapshotDocument.SimulationTick ==
            right.WorldSnapshotDocument.SimulationTick;

    private static HostRuntimeRecoverySupersessionChainResult<
        TRequest,
        TState,
        TCompletion> ChainResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoverySupersessionChainStatus status,
            global::System.Collections.Generic.IReadOnlyList<
                HostRuntimeRecoveryCheckpointSupersession<
                    TRequest,
                    TState,
                    TCompletion>> supersessions)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, supersessions, chain: null);

    private static HostRuntimeRecoveryLatestCheckpointSelectionResult<
        TRequest,
        TState,
        TCompletion> SelectionResult<TRequest, TState, TCompletion>(
            HostRuntimeRecoverySupersessionChainStatus status,
            HostRuntimeRecoverySupersessionChain<
                TRequest,
                TState,
                TCompletion> chain)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
        where TCompletion : IHostRuntimeCompletion =>
        new(status, chain, selection: null);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new ArgumentException(
                "The identifier must be initialized.",
                parameterName);
        }
    }

    private static void EnsureRevision(long revision, string parameterName)
    {
        if (revision < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }

    private static void EnsureTick(long tick, string parameterName)
    {
        if (tick < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}
