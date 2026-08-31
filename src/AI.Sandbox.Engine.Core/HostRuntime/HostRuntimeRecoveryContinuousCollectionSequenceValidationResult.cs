namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable continuous collection-sequence validation result.</summary>
public sealed record HostRuntimeRecoveryContinuousCollectionSequenceValidationResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousCollectionSequenceValidationResult(
        HostRuntimeRecoveryContinuousCollectionSequenceStatus status,
        global::System.Collections.Generic.IReadOnlyList<
            HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
                TRequest,
                TState,
                TCompletion>> collectionPairSummaries,
        HostRuntimeRecoveryContinuousCollectionSequenceValidation<
            TRequest,
            TState,
            TCompletion>? validation)
    {
        Status = status;
        CollectionPairSummaries = collectionPairSummaries;
        Validation = validation;
    }

    /// <summary>Gets the explicit collection-sequence validation outcome.</summary>
    public HostRuntimeRecoveryContinuousCollectionSequenceStatus Status { get; }

    /// <summary>Gets the unchanged caller-supplied summary snapshot.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
            TRequest,
            TState,
            TCompletion>> CollectionPairSummaries { get; }

    /// <summary>Gets the created collection-sequence validation, when successful.</summary>
    public HostRuntimeRecoveryContinuousCollectionSequenceValidation<
        TRequest,
        TState,
        TCompletion>? Validation { get; }

    /// <summary>Gets whether collection-sequence validation succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousCollectionSequenceStatus
            .ContinuousCollectionSequenceValidated;
}
