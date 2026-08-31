namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable continuous multi-collection sequence-validation result.</summary>
public sealed record HostRuntimeRecoveryContinuousMultiCollectionSequenceValidationResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousMultiCollectionSequenceValidationResult(
        HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus status,
        global::System.Collections.Generic.IReadOnlyList<
            HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
                TRequest,
                TState,
                TCompletion>> multiCollectionSummaries,
        HostRuntimeRecoveryContinuousMultiCollectionSequenceValidation<
            TRequest,
            TState,
            TCompletion>? validation)
    {
        Status = status;
        MultiCollectionSummaries = multiCollectionSummaries;
        Validation = validation;
    }

    /// <summary>Gets the explicit multi-collection-sequence validation outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus Status { get; }

    /// <summary>Gets the unchanged caller-supplied summary snapshot.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
            TRequest,
            TState,
            TCompletion>> MultiCollectionSummaries { get; }

    /// <summary>Gets the created multi-collection-sequence validation, when successful.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceValidation<
        TRequest,
        TState,
        TCompletion>? Validation { get; }

    /// <summary>Gets whether multi-collection-sequence validation succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
            .ContinuousMultiCollectionSequenceValidated;
}
