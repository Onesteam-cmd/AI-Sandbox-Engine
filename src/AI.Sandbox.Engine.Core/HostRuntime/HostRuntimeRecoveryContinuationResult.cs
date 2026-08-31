namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery continuation result.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
public sealed record HostRuntimeRecoveryContinuationResult<TRequest, TState>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
{
    internal HostRuntimeRecoveryContinuationResult(
        HostRuntimeRecoveryStatus status,
        HostRuntimeRecoveryContinuation<TRequest, TState>? continuation,
        global::AI.Sandbox.Engine.Core.Persistence.SnapshotRestoreStatus?
            restoreStatus)
    {
        Status = status;
        Continuation = continuation;
        RestoreStatus = restoreStatus;
    }

    /// <summary>Gets the explicit continuation outcome.</summary>
    public HostRuntimeRecoveryStatus Status { get; }

    /// <summary>Gets continuation authority when validation succeeded.</summary>
    public HostRuntimeRecoveryContinuation<TRequest, TState>? Continuation
    {
        get;
    }

    /// <summary>Gets the persistence restore outcome when restoration failed.</summary>
    public global::AI.Sandbox.Engine.Core.Persistence.SnapshotRestoreStatus?
        RestoreStatus { get; }

    /// <summary>Gets whether continuation authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryStatus.ContinuationCreated;
}
