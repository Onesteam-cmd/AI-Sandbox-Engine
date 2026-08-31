namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable continuous multi-sequence collection-validation result.</summary>
public sealed record HostRuntimeRecoveryContinuousMultiSequenceCollectionValidationResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousMultiSequenceCollectionValidationResult(
        HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus status,
        global::System.Collections.Generic.IReadOnlyList<
            HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
                TRequest,
                TState,
                TCompletion>> multiSequenceSummaries,
        HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
            TRequest,
            TState,
            TCompletion>? validation)
    {
        Status = status;
        MultiSequenceSummaries = multiSequenceSummaries;
        Validation = validation;
    }

    /// <summary>Gets the explicit collection-validation outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus Status { get; }

    /// <summary>Gets the unchanged caller-supplied summary snapshot.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion>> MultiSequenceSummaries { get; }

    /// <summary>Gets the created collection validation, when successful.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
        TRequest,
        TState,
        TCompletion>? Validation { get; }

    /// <summary>Gets whether collection validation succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .ContinuousMultiSequenceCollectionValidated;
}
