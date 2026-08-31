namespace AI.Sandbox.Engine.Core.Modeling;

/// <summary>
/// Invokes one exact provider-neutral model adapter and validates completed
/// response correlation without retries or authoritative mutation.
/// </summary>
/// <typeparam name="TRequest">The exact model-request payload type.</typeparam>
/// <typeparam name="TResponse">The exact model-response payload type.</typeparam>
public sealed class ModelInvocationProcessor<TRequest, TResponse>
    where TRequest : IModelRequest
    where TResponse : IModelResponse
{
    private readonly global::AI.Sandbox.Engine.Core.Identifiers
        .Id<ModelAdapterIdKind> adapterId;
    private readonly IModelAdapter<TRequest, TResponse> adapter;

    private ModelInvocationProcessor(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ModelAdapterIdKind>
            adapterId,
        IModelAdapter<TRequest, TResponse> adapter)
    {
        this.adapterId = adapterId;
        this.adapter = adapter;
    }

    /// <summary>
    /// Creates a processor bound to one stable exact adapter.
    /// </summary>
    /// <param name="adapterId">The stable configured adapter ID.</param>
    /// <param name="adapter">The exact asynchronous adapter.</param>
    /// <returns>The configured model invocation processor.</returns>
    public static ModelInvocationProcessor<TRequest, TResponse> Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ModelAdapterIdKind>
            adapterId,
        IModelAdapter<TRequest, TResponse> adapter)
    {
        ArgumentNullException.ThrowIfNull(adapter);

        if (adapterId.IsEmpty)
        {
            throw new ArgumentException(
                "The model adapter ID cannot be empty.",
                nameof(adapterId));
        }

        ModelTypePolicy.EnsureExactType(
            typeof(TRequest),
            typeof(IModelRequest),
            "model request");
        ModelTypePolicy.EnsureExactType(
            typeof(TResponse),
            typeof(IModelResponse),
            "model response");

        return new ModelInvocationProcessor<TRequest, TResponse>(
            adapterId,
            adapter);
    }

    /// <summary>
    /// Invokes the adapter at most once and validates any completed response.
    /// </summary>
    /// <param name="request">The exact immutable invocation request.</param>
    /// <param name="cancellationToken">The explicit host cancellation token.</param>
    /// <returns>The explicit validated invocation result.</returns>
    public async ValueTask<ModelInvocationResult<TRequest, TResponse>>
        InvokeAsync(
            ModelInvocationRequestEnvelope<TRequest> request,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.AdapterId != adapterId)
        {
            return ModelInvocationResult<TRequest, TResponse>.NotInvoked(
                ModelInvocationStatus.RequestAdapterMismatch,
                request);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var context = new ModelInvocationContext<TRequest>(request, adapterId);
        var decision = await adapter
            .InvokeAsync(context, cancellationToken)
            .ConfigureAwait(false) ??
            throw new InvalidOperationException(
                "Model adapters cannot return null decisions.");

        var status = decision.Status switch
        {
            ModelInvocationDecisionStatus.Rejected =>
                ModelInvocationStatus.Rejected,
            ModelInvocationDecisionStatus.Failed =>
                ModelInvocationStatus.Failed,
            ModelInvocationDecisionStatus.Completed =>
                ValidateResponse(
                    request,
                    decision.Response ??
                    throw new InvalidOperationException(
                        "Completed model decisions require a response.")),
            _ => throw new InvalidOperationException(
                "Unknown model invocation decision status."),
        };

        return ModelInvocationResult<TRequest, TResponse>.Evaluated(
            status,
            request,
            decision);
    }

    private ModelInvocationStatus ValidateResponse(
        ModelInvocationRequestEnvelope<TRequest> request,
        ModelInvocationResponseEnvelope<TResponse> response)
    {
        if (response.InvocationId != request.InvocationId)
        {
            return ModelInvocationStatus.ResponseInvocationMismatch;
        }

        if (response.AdapterId != adapterId)
        {
            return ModelInvocationStatus.ResponseAdapterMismatch;
        }

        if (response.ModelProfileId != request.ModelProfileId)
        {
            return ModelInvocationStatus.ResponseProfileMismatch;
        }

        if (response.PromptDocumentId != request.PromptDocumentId)
        {
            return ModelInvocationStatus.ResponsePromptDocumentMismatch;
        }

        if (response.OwnerEntityId != request.OwnerEntityId)
        {
            return ModelInvocationStatus.ResponseOwnerMismatch;
        }

        if (response.WorldId != request.WorldId)
        {
            return ModelInvocationStatus.ResponseWorldMismatch;
        }

        if (response.WorldStateVersion != request.WorldStateVersion)
        {
            return ModelInvocationStatus.ResponseVersionMismatch;
        }

        if (response.SimulationTick != request.SimulationTick)
        {
            return ModelInvocationStatus.ResponseSimulationTickMismatch;
        }

        return response.Usage.OutputUnits > request.OutputLimit.Units
            ? ModelInvocationStatus.ResponseOutputLimitExceeded
            : ModelInvocationStatus.Completed;
    }
}
