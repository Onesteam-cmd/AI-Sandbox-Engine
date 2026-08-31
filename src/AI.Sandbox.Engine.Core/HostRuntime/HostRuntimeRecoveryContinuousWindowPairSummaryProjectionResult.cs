namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery continuous-window pair summary result.
/// </summary>
public sealed record HostRuntimeRecoveryContinuousWindowPairSummaryProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousWindowPairSummaryProjectionResult(
        HostRuntimeRecoveryContinuousWindowPairStatus status,
        HostRuntimeRecoveryCheckpointRangeContinuityValidation<
            TRequest,
            TState,
            TCompletion> continuity,
        HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
            TRequest,
            TState,
            TCompletion>? summary)
    {
        Status = status;
        Continuity = continuity;
        Summary = summary;
    }

    /// <summary>Gets the explicit pair-summary outcome.</summary>
    public HostRuntimeRecoveryContinuousWindowPairStatus Status { get; }

    /// <summary>Gets unchanged source continuity-validation authority.</summary>
    public HostRuntimeRecoveryCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion> Continuity { get; }

    /// <summary>Gets the created pair summary, when successful.</summary>
    public HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
        TRequest,
        TState,
        TCompletion>? Summary { get; }

    /// <summary>Gets whether pair-summary projection succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousWindowPairStatus
            .ContinuousWindowPairSummaryProjected;
}
