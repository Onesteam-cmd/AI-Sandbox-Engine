namespace AI.Sandbox.Engine.Core.Speech;

/// <summary>
/// Provides one immutable request and its configured adapter identity to an
/// exact provider-neutral speech adapter.
/// </summary>
/// <typeparam name="TRequest">The exact speech-request payload type.</typeparam>
public sealed class SpeechInvocationContext<TRequest>
    where TRequest : ISpeechRequest
{
    internal SpeechInvocationContext(
        SpeechInvocationRequestEnvelope<TRequest> request,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<SpeechAdapterIdKind>
            adapterId)
    {
        Request = request;
        AdapterId = adapterId;
    }

    /// <summary>
    /// Gets the exact immutable speech request.
    /// </summary>
    public SpeechInvocationRequestEnvelope<TRequest> Request { get; }

    /// <summary>
    /// Gets the stable configured adapter ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<SpeechAdapterIdKind>
        AdapterId { get; }
}
