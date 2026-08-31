namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery checkpoint-supersession result.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryCheckpointSupersessionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCheckpointSupersessionResult(
        HostRuntimeRecoveryCheckpointSupersessionStatus status,
        HostRuntimeRecoveryCycleCompletion<TRequest, TState, TCompletion>
            cycleCompletion,
        HostRuntimeRecoveryCheckpoint<TRequest> successorCheckpoint,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>? supersession)
    {
        Status = status;
        CycleCompletion = cycleCompletion;
        SuccessorCheckpoint = successorCheckpoint;
        Supersession = supersession;
    }

    /// <summary>Gets the explicit checkpoint-supersession outcome.</summary>
    public HostRuntimeRecoveryCheckpointSupersessionStatus Status { get; }

    /// <summary>Gets unchanged completed recovery-cycle authority.</summary>
    public HostRuntimeRecoveryCycleCompletion<TRequest, TState, TCompletion>
        CycleCompletion { get; }

    /// <summary>Gets unchanged successor checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> SuccessorCheckpoint { get; }

    /// <summary>Gets checkpoint-supersession authority when successful.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion>? Supersession { get; }

    /// <summary>Gets whether checkpoint-supersession authority was created.</summary>
    public bool Succeeded =>
        Status ==
        HostRuntimeRecoveryCheckpointSupersessionStatus.CheckpointSuperseded;
}
