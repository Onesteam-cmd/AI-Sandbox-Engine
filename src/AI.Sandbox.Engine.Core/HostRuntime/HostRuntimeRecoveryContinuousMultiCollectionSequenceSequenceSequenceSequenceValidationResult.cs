namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable continuous multi-collection-sequence-sequence-sequence sequence-validation result.</summary>
public sealed record HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidationResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidationResult(
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus status,
        global::System.Collections.Generic.IReadOnlyList<
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion>> multiCollectionSequenceSequenceSequenceSummaries,
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidation<
            TRequest,
            TState,
            TCompletion>? validation)
    {
        Status = status;
        MultiCollectionSequenceSequenceSequenceSummaries = multiCollectionSequenceSequenceSequenceSummaries;
        Validation = validation;
    }

    /// <summary>Gets the explicit multi-collection-sequence-sequence-sequence-sequence validation outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus Status { get; }

    /// <summary>Gets the unchanged caller-supplied multi-collection-sequence-sequence-sequence summary snapshot.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion>> MultiCollectionSequenceSequenceSequenceSummaries { get; }

    /// <summary>Gets the created multi-collection-sequence-sequence-sequence-sequence validation, when successful.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidation<
        TRequest,
        TState,
        TCompletion>? Validation { get; }

    /// <summary>Gets whether multi-collection-sequence-sequence-sequence-sequence validation succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
            .ContinuousMultiCollectionSequenceSequenceSequenceSequenceValidated;
}
