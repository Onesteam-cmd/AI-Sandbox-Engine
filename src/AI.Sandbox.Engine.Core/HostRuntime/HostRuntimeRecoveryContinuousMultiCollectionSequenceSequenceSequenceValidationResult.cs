namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable continuous multi-collection-sequence-sequence sequence-validation result.</summary>
public sealed record HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidationResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidationResult(
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceStatus status,
        global::System.Collections.Generic.IReadOnlyList<
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion>> multiCollectionSequenceSequenceSummaries,
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidation<
            TRequest,
            TState,
            TCompletion>? validation)
    {
        Status = status;
        MultiCollectionSequenceSequenceSummaries = multiCollectionSequenceSequenceSummaries;
        Validation = validation;
    }

    /// <summary>Gets the explicit multi-collection-sequence-sequence-sequence validation outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceStatus Status { get; }

    /// <summary>Gets the unchanged caller-supplied multi-collection-sequence-sequence summary snapshot.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion>> MultiCollectionSequenceSequenceSummaries { get; }

    /// <summary>Gets the created multi-collection-sequence-sequence-sequence validation, when successful.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidation<
        TRequest,
        TState,
        TCompletion>? Validation { get; }

    /// <summary>Gets whether multi-collection-sequence-sequence-sequence validation succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceStatus
            .ContinuousMultiCollectionSequenceSequenceSequenceValidated;
}
