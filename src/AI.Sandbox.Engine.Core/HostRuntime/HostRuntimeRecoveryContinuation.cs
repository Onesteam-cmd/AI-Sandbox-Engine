namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority to continue from one validated recovery
/// checkpoint and one successfully restored World State snapshot.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
public sealed record HostRuntimeRecoveryContinuation<TRequest, TState>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
{
    internal HostRuntimeRecoveryContinuation(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryContinuationIdKind> continuationId,
        HostRuntimeRecoveryCheckpoint<TRequest> checkpoint,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateSnapshot<TState>
            restoredWorldSnapshot,
        long continuedTick,
        long revision)
    {
        ContinuationId = continuationId;
        Checkpoint = checkpoint;
        RestoredWorldSnapshot = restoredWorldSnapshot;
        ContinuedTick = continuedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned continuation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryContinuationIdKind> ContinuationId { get; }

    /// <summary>Gets unchanged recovery checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> Checkpoint { get; }

    /// <summary>Gets the successfully decoded immutable World State snapshot.</summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateSnapshot<TState>
        RestoredWorldSnapshot { get; }

    /// <summary>Gets the external monotonic continuation tick.</summary>
    public long ContinuedTick { get; }

    /// <summary>Gets the continuation authority revision.</summary>
    public long Revision { get; }
}
