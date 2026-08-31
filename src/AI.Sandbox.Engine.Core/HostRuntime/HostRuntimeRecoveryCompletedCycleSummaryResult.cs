namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery completed-cycle summary result.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryCompletedCycleSummaryResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCompletedCycleSummaryResult(
        HostRuntimeRecoveryCheckpointSupersessionStatus status,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion> supersession,
        HostRuntimeRecoveryCompletedCycleSummary<
            TRequest,
            TState,
            TCompletion>? summary)
    {
        Status = status;
        Supersession = supersession;
        Summary = summary;
    }

    /// <summary>Gets the explicit completed-cycle summary outcome.</summary>
    public HostRuntimeRecoveryCheckpointSupersessionStatus Status { get; }

    /// <summary>Gets unchanged checkpoint-supersession authority.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion> Supersession { get; }

    /// <summary>Gets completed-cycle summary authority when successful.</summary>
    public HostRuntimeRecoveryCompletedCycleSummary<
        TRequest,
        TState,
        TCompletion>? Summary { get; }

    /// <summary>Gets whether completed-cycle summary authority was created.</summary>
    public bool Succeeded =>
        Status ==
        HostRuntimeRecoveryCheckpointSupersessionStatus.SummaryCreated;
}
