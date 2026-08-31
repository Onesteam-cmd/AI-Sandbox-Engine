namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority proving exact checkpoint and supersession
/// continuity between one summarized range and one projected adjacent window.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryCheckpointRangeContinuityValidation<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCheckpointRangeContinuityValidation(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryCheckpointRangeContinuityValidationIdKind> validationId,
        HostRuntimeRecoveryCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentWindowProjection<
            TRequest,
            TState,
            TCompletion> adjacentWindow,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion> connectingSupersession,
        long validatedTick,
        long revision)
    {
        ValidationId = validationId;
        Summary = summary;
        AdjacentWindow = adjacentWindow;
        ConnectingSupersession = connectingSupersession;
        ValidatedTick = validatedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned continuity-validation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointRangeContinuityValidationIdKind> ValidationId { get; }

    /// <summary>Gets unchanged checkpoint-range summary authority.</summary>
    public HostRuntimeRecoveryCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets unchanged projected adjacent-window authority.</summary>
    public HostRuntimeRecoveryAdjacentWindowProjection<
        TRequest,
        TState,
        TCompletion> AdjacentWindow { get; }

    /// <summary>Gets unchanged adjacent-window selection authority.</summary>
    public HostRuntimeRecoveryAdjacentWindowSelection<
        TRequest,
        TState,
        TCompletion> Selection => AdjacentWindow.Selection;

    /// <summary>Gets whether continuity was validated to a previous window.</summary>
    public bool ValidatesPreviousWindow => AdjacentWindow.SelectsPreviousWindow;

    /// <summary>Gets whether continuity was validated to a next window.</summary>
    public bool ValidatesNextWindow => AdjacentWindow.SelectsNextWindow;

    /// <summary>Gets the exact supersession connecting both authorities.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion> ConnectingSupersession { get; }

    /// <summary>Gets the exact checkpoint before the connecting supersession.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> PriorCheckpoint =>
        ConnectingSupersession.PriorCheckpoint;

    /// <summary>Gets the exact checkpoint after the connecting supersession.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> SuccessorCheckpoint =>
        ConnectingSupersession.SuccessorCheckpoint;

    /// <summary>Gets the external monotonic continuity-validation tick.</summary>
    public long ValidatedTick { get; }

    /// <summary>Gets the checkpoint-range continuity authority revision.</summary>
    public long Revision { get; }
}
