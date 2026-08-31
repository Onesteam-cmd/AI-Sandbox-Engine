namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable continuous multi-collection-sequence sequence-validation result.</summary>
public sealed record HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceValidationResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceValidationResult(
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceStatus status,
        global::System.Collections.Generic.IReadOnlyList<
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion>> multiCollectionSequenceSummaries,
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceValidation<
            TRequest,
            TState,
            TCompletion>? validation)
    {
        Status = status;
        MultiCollectionSequenceSummaries = multiCollectionSequenceSummaries;
        Validation = validation;
    }

    /// <summary>Gets the explicit multi-collection-sequence-sequence validation outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceStatus Status { get; }

    /// <summary>Gets the unchanged caller-supplied summary snapshot.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion>> MultiCollectionSequenceSummaries { get; }

    /// <summary>Gets the created multi-collection-sequence-sequence validation, when successful.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceValidation<
        TRequest,
        TState,
        TCompletion>? Validation { get; }

    /// <summary>Gets whether multi-collection-sequence-sequence validation succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceStatus
            .ContinuousMultiCollectionSequenceSequenceValidated;
}
