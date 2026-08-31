namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable continuous-window sequence-validation result.</summary>
public sealed record HostRuntimeRecoveryContinuousWindowSequenceValidationResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousWindowSequenceValidationResult(
        HostRuntimeRecoveryContinuousWindowSequenceStatus status,
        global::System.Collections.Generic.IReadOnlyList<
            HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
                TRequest,
                TState,
                TCompletion>> pairSummaries,
        HostRuntimeRecoveryContinuousWindowSequenceValidation<
            TRequest,
            TState,
            TCompletion>? validation)
    {
        Status = status;
        PairSummaries = pairSummaries;
        Validation = validation;
    }

    /// <summary>Gets the explicit sequence-validation outcome.</summary>
    public HostRuntimeRecoveryContinuousWindowSequenceStatus Status { get; }

    /// <summary>Gets the unchanged caller-supplied pair-summary snapshot.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
            TRequest,
            TState,
            TCompletion>> PairSummaries { get; }

    /// <summary>Gets the created sequence validation, when successful.</summary>
    public HostRuntimeRecoveryContinuousWindowSequenceValidation<
        TRequest,
        TState,
        TCompletion>? Validation { get; }

    /// <summary>Gets whether continuous-window sequence validation succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousWindowSequenceStatus
            .ContinuousWindowSequenceValidated;
}
