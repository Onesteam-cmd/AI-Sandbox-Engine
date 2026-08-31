namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery resumption planning result.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
public sealed record HostRuntimeRecoveryResumptionPlanResult<TRequest, TState>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
{
    internal HostRuntimeRecoveryResumptionPlanResult(
        HostRuntimeRecoveryResumptionStatus status,
        HostRuntimeRecoveryContinuation<TRequest, TState> continuation,
        HostRuntimeRecoveryResumptionPlan<TRequest, TState>? plan)
    {
        Status = status;
        Continuation = continuation;
        Plan = plan;
    }

    /// <summary>Gets the explicit planning outcome.</summary>
    public HostRuntimeRecoveryResumptionStatus Status { get; }

    /// <summary>Gets unchanged recovery continuation authority.</summary>
    public HostRuntimeRecoveryContinuation<TRequest, TState> Continuation
    {
        get;
    }

    /// <summary>Gets resumption plan authority when planning succeeded.</summary>
    public HostRuntimeRecoveryResumptionPlan<TRequest, TState>? Plan { get; }

    /// <summary>Gets whether resumption plan authority was created.</summary>
    public bool Succeeded =>
        Status is
            HostRuntimeRecoveryResumptionStatus.PlanCreated or
            HostRuntimeRecoveryResumptionStatus.NoResumableWork;
}
