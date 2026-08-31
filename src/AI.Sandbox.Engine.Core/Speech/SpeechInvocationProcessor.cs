namespace AI.Sandbox.Engine.Core.Speech;

/// <summary>
/// Invokes one exact provider-neutral recognition or synthesis adapter and
/// validates completed-response correlation without retries, playback,
/// recording, or authoritative mutation.
/// </summary>
/// <typeparam name="TRequest">The exact speech-request payload type.</typeparam>
/// <typeparam name="TResponse">The exact speech-response payload type.</typeparam>
public sealed class SpeechInvocationProcessor<TRequest, TResponse>
    where TRequest : ISpeechRequest
    where TResponse : ISpeechResponse
{
    private readonly global::AI.Sandbox.Engine.Core.Identifiers
        .Id<SpeechAdapterIdKind> adapterId;
    private readonly ISpeechAdapter<TRequest, TResponse> adapter;

    private SpeechInvocationProcessor(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<SpeechAdapterIdKind>
            adapterId,
        ISpeechAdapter<TRequest, TResponse> adapter)
    {
        this.adapterId = adapterId;
        this.adapter = adapter;
    }

    /// <summary>
    /// Creates a processor bound to one stable exact speech adapter.
    /// </summary>
    /// <param name="adapterId">The stable configured adapter ID.</param>
    /// <param name="adapter">The exact asynchronous speech adapter.</param>
    /// <returns>The configured speech invocation processor.</returns>
    public static SpeechInvocationProcessor<TRequest, TResponse> Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<SpeechAdapterIdKind>
            adapterId,
        ISpeechAdapter<TRequest, TResponse> adapter)
    {
        ArgumentNullException.ThrowIfNull(adapter);

        if (adapterId.IsEmpty)
        {
            throw new ArgumentException(
                "The speech adapter ID cannot be empty.",
                nameof(adapterId));
        }

        SpeechTypePolicy.EnsureExactType(
            typeof(TRequest),
            typeof(ISpeechRequest),
            "speech request");
        SpeechTypePolicy.EnsureExactType(
            typeof(TResponse),
            typeof(ISpeechResponse),
            "speech response");

        return new SpeechInvocationProcessor<TRequest, TResponse>(
            adapterId,
            adapter);
    }

    /// <summary>
    /// Invokes the adapter at most once and validates any completed response.
    /// </summary>
    /// <param name="request">The exact immutable speech request.</param>
    /// <param name="cancellationToken">The explicit host cancellation token.</param>
    /// <returns>The explicit validated speech invocation result.</returns>
    public async ValueTask<SpeechInvocationResult<TRequest, TResponse>>
        InvokeAsync(
            SpeechInvocationRequestEnvelope<TRequest> request,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.AdapterId != adapterId)
        {
            return SpeechInvocationResult<TRequest, TResponse>.NotInvoked(
                SpeechInvocationStatus.RequestAdapterMismatch,
                request);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var context = new SpeechInvocationContext<TRequest>(request, adapterId);
        var decision = await adapter
            .InvokeAsync(context, cancellationToken)
            .ConfigureAwait(false) ??
            throw new InvalidOperationException(
                "Speech adapters cannot return null decisions.");

        var status = decision.Status switch
        {
            SpeechInvocationDecisionStatus.Rejected =>
                SpeechInvocationStatus.Rejected,
            SpeechInvocationDecisionStatus.Failed =>
                SpeechInvocationStatus.Failed,
            SpeechInvocationDecisionStatus.Completed =>
                ValidateResponse(
                    request,
                    decision.Response ??
                    throw new InvalidOperationException(
                        "Completed speech decisions require a response.")),
            _ => throw new InvalidOperationException(
                "Unknown speech invocation decision status."),
        };

        return SpeechInvocationResult<TRequest, TResponse>.Evaluated(
            status,
            request,
            decision);
    }

    private SpeechInvocationStatus ValidateResponse(
        SpeechInvocationRequestEnvelope<TRequest> request,
        SpeechInvocationResponseEnvelope<TResponse> response)
    {
        if (response.OperationKind != request.OperationKind)
        {
            return SpeechInvocationStatus.ResponseOperationMismatch;
        }

        if (response.InvocationId != request.InvocationId)
        {
            return SpeechInvocationStatus.ResponseInvocationMismatch;
        }

        if (response.AdapterId != adapterId)
        {
            return SpeechInvocationStatus.ResponseAdapterMismatch;
        }

        if (response.SpeechProfileId != request.SpeechProfileId)
        {
            return SpeechInvocationStatus.ResponseProfileMismatch;
        }

        if (response.OwnerEntityId != request.OwnerEntityId)
        {
            return SpeechInvocationStatus.ResponseOwnerMismatch;
        }

        if (response.WorldId != request.WorldId)
        {
            return SpeechInvocationStatus.ResponseWorldMismatch;
        }

        if (response.WorldStateVersion != request.WorldStateVersion)
        {
            return SpeechInvocationStatus.ResponseVersionMismatch;
        }

        if (response.SimulationTick != request.SimulationTick)
        {
            return SpeechInvocationStatus.ResponseSimulationTickMismatch;
        }

        if (response.Usage.InputUnits > request.InputLimit.Units)
        {
            return SpeechInvocationStatus.ResponseInputLimitExceeded;
        }

        return response.Usage.OutputUnits > request.OutputLimit.Units
            ? SpeechInvocationStatus.ResponseOutputLimitExceeded
            : SpeechInvocationStatus.Completed;
    }
}
