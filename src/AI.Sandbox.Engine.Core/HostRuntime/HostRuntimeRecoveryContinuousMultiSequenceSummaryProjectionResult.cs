namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery continuous multi-sequence summary result.
/// </summary>
public sealed record HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionResult(
        HostRuntimeRecoveryContinuousMultiSequenceStatus status,
        HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation<
            TRequest,
            TState,
            TCompletion> continuity,
        HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion>? summary)
    {
        Status = status;
        Continuity = continuity;
        Summary = summary;
    }

    /// <summary>Gets the explicit multi-sequence-summary outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceStatus Status { get; }

    /// <summary>Gets unchanged source continuity-validation authority.</summary>
    public HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion> Continuity { get; }

    /// <summary>Gets the created multi-sequence summary, when successful.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
        TRequest,
        TState,
        TCompletion>? Summary { get; }

    /// <summary>Gets whether multi-sequence-summary projection succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiSequenceStatus
            .ContinuousMultiSequenceSummaryProjected;
}
