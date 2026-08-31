namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Creates pure bounded recovery resumption plans and advisory resumed-work
/// selections without restart, scheduling, lease acquisition, dispatch, or
/// execution.
/// </summary>
public static class HostRuntimeRecoveryResumptionFlow
{
    /// <summary>
    /// Plans pending checkpoint work eligible for external resumption.
    /// </summary>
    /// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
    /// <typeparam name="TState">Exact immutable World State root type.</typeparam>
    /// <param name="planId">Externally assigned resumption plan ID.</param>
    /// <param name="continuation">Validated recovery continuation authority.</param>
    /// <param name="expectedContinuationRevision">
    /// Continuation revision observed by the caller.
    /// </param>
    /// <param name="plannedTick">External monotonic planning tick.</param>
    /// <param name="revision">Optimistic resumption plan revision.</param>
    /// <returns>An explicit immutable resumption planning result.</returns>
    public static HostRuntimeRecoveryResumptionPlanResult<TRequest, TState>
        Plan<TRequest, TState>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeRecoveryResumptionPlanIdKind> planId,
            HostRuntimeRecoveryContinuation<TRequest, TState> continuation,
            long expectedContinuationRevision,
            long plannedTick,
            long revision)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    {
        EnsureId(planId.IsEmpty, nameof(planId));
        ArgumentNullException.ThrowIfNull(continuation);

        if (expectedContinuationRevision < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedContinuationRevision));
        }
        if (plannedTick < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(plannedTick));
        }
        if (revision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(revision));
        }

        if (continuation.Revision != expectedContinuationRevision)
        {
            return PlanResult<TRequest, TState>(
                HostRuntimeRecoveryResumptionStatus
                    .StaleContinuationRevision,
                continuation);
        }
        if (plannedTick < continuation.ContinuedTick)
        {
            return PlanResult<TRequest, TState>(
                HostRuntimeRecoveryResumptionStatus
                    .PlanningTickRegressed,
                continuation);
        }

        var candidates = new List<HostRuntimeActiveWorkItem<TRequest>>();
        var suppressedAttemptIds = new List<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeAttemptIdKind>>();

        foreach (var item in continuation.Checkpoint.ActiveWorkSnapshot.Items)
        {
            if (item.Request.State == HostRuntimeRequestState.Pending)
            {
                candidates.Add(item);
            }
            else if (
                item.Request.State ==
                HostRuntimeRequestState.CancellationRequested)
            {
                suppressedAttemptIds.Add(item.AttemptId);
            }
        }

        var plan = new HostRuntimeRecoveryResumptionPlan<TRequest, TState>(
            planId,
            continuation,
            candidates.ToArray(),
            suppressedAttemptIds.ToArray(),
            plannedTick,
            revision);
        var status = candidates.Count == 0
            ? HostRuntimeRecoveryResumptionStatus.NoResumableWork
            : HostRuntimeRecoveryResumptionStatus.PlanCreated;

        return new HostRuntimeRecoveryResumptionPlanResult<TRequest, TState>(
            status,
            continuation,
            plan);
    }

    /// <summary>
    /// Selects one planned attempt for external recovery resumption.
    /// </summary>
    /// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
    /// <typeparam name="TState">Exact immutable World State root type.</typeparam>
    /// <param name="selectionId">Externally assigned selection ID.</param>
    /// <param name="plan">Existing immutable resumption plan.</param>
    /// <param name="expectedPlanRevision">
    /// Plan revision observed by the caller.
    /// </param>
    /// <param name="attemptId">Stable planned attempt ID to select.</param>
    /// <param name="selectedTick">External monotonic selection tick.</param>
    /// <returns>An explicit immutable resumed-work selection result.</returns>
    public static HostRuntimeResumedWorkSelectionResult<TRequest, TState>
        Select<TRequest, TState>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeResumedWorkSelectionIdKind> selectionId,
            HostRuntimeRecoveryResumptionPlan<TRequest, TState> plan,
            long expectedPlanRevision,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeAttemptIdKind> attemptId,
            long selectedTick)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    {
        EnsureId(selectionId.IsEmpty, nameof(selectionId));
        ArgumentNullException.ThrowIfNull(plan);
        EnsureId(attemptId.IsEmpty, nameof(attemptId));

        if (expectedPlanRevision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(expectedPlanRevision));
        }
        if (selectedTick < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(selectedTick));
        }

        if (plan.Revision != expectedPlanRevision)
        {
            return SelectionResult<TRequest, TState>(
                HostRuntimeRecoveryResumptionStatus.StalePlanRevision,
                plan,
                attemptId);
        }
        if (selectedTick < plan.PlannedTick)
        {
            return SelectionResult<TRequest, TState>(
                HostRuntimeRecoveryResumptionStatus.SelectionTickRegressed,
                plan,
                attemptId);
        }

        HostRuntimeActiveWorkItem<TRequest>? candidate = null;
        foreach (var item in plan.Candidates)
        {
            if (item.AttemptId == attemptId)
            {
                candidate = item;
                break;
            }
        }

        if (candidate is null)
        {
            return SelectionResult<TRequest, TState>(
                HostRuntimeRecoveryResumptionStatus.AttemptNotPlanned,
                plan,
                attemptId);
        }

        var selection =
            new HostRuntimeResumedWorkSelection<TRequest, TState>(
                selectionId,
                plan,
                candidate,
                selectedTick,
                checked(plan.Revision + 1));

        return new HostRuntimeResumedWorkSelectionResult<TRequest, TState>(
            HostRuntimeRecoveryResumptionStatus.SelectionCreated,
            plan,
            selection,
            attemptId);
    }

    private static HostRuntimeRecoveryResumptionPlanResult<TRequest, TState>
        PlanResult<TRequest, TState>(
            HostRuntimeRecoveryResumptionStatus status,
            HostRuntimeRecoveryContinuation<TRequest, TState> continuation)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState =>
        new(status, continuation, plan: null);

    private static HostRuntimeResumedWorkSelectionResult<TRequest, TState>
        SelectionResult<TRequest, TState>(
            HostRuntimeRecoveryResumptionStatus status,
            HostRuntimeRecoveryResumptionPlan<TRequest, TState> plan,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeAttemptIdKind> relatedAttemptId)
        where TRequest : IHostRuntimeRequest
        where TState : class,
            global::AI.Sandbox.Engine.Core.WorldState.IWorldState =>
        new(status, plan, selection: null, relatedAttemptId);

    private static void EnsureId(bool isEmpty, string parameterName)
    {
        if (isEmpty)
        {
            throw new ArgumentException(
                "The identifier must be initialized.",
                parameterName);
        }
    }
}
