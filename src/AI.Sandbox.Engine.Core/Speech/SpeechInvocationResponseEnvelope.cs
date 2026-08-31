namespace AI.Sandbox.Engine.Core.Speech;

/// <summary>
/// Captures one immutable completed recognition or synthesis response with
/// full request correlation and adapter-reported usage.
/// </summary>
/// <typeparam name="TResponse">The exact speech-response payload type.</typeparam>
public sealed record SpeechInvocationResponseEnvelope<TResponse>
    where TResponse : ISpeechResponse
{
    private SpeechInvocationResponseEnvelope(
        SpeechOperationKind operationKind,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<SpeechInvocationIdKind>
            invocationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<SpeechAdapterIdKind>
            adapterId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<SpeechProfileIdKind>
            speechProfileId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        SpeechUsage usage,
        TResponse payload)
    {
        OperationKind = operationKind;
        InvocationId = invocationId;
        AdapterId = adapterId;
        SpeechProfileId = speechProfileId;
        OwnerEntityId = ownerEntityId;
        WorldId = worldId;
        WorldStateVersion = worldStateVersion;
        SimulationTick = simulationTick;
        Usage = usage;
        Payload = payload;
    }

    /// <summary>
    /// Gets the correlated recognition or synthesis operation.
    /// </summary>
    public SpeechOperationKind OperationKind { get; }

    /// <summary>
    /// Gets the correlated invocation ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<SpeechInvocationIdKind>
        InvocationId { get; }

    /// <summary>
    /// Gets the adapter that produced this response.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<SpeechAdapterIdKind>
        AdapterId { get; }

    /// <summary>
    /// Gets the opaque recognition or voice profile used by the adapter.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<SpeechProfileIdKind>
        SpeechProfileId { get; }

    /// <summary>
    /// Gets the correlated subjective owner.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OwnerEntityId
        { get; }

    /// <summary>
    /// Gets the correlated authoritative world.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId
        { get; }

    /// <summary>
    /// Gets the correlated authoritative version.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
        WorldStateVersion { get; }

    /// <summary>
    /// Gets the correlated logical simulation tick.
    /// </summary>
    public ulong SimulationTick { get; }

    /// <summary>
    /// Gets the initialized adapter-reported usage.
    /// </summary>
    public SpeechUsage Usage { get; }

    /// <summary>
    /// Gets the exact speech-response payload.
    /// </summary>
    public TResponse Payload { get; }

    /// <summary>
    /// Creates one immutable completed speech response.
    /// </summary>
    /// <param name="operationKind">The correlated speech operation.</param>
    /// <param name="invocationId">The correlated invocation ID.</param>
    /// <param name="adapterId">The producing adapter ID.</param>
    /// <param name="speechProfileId">The used opaque profile ID.</param>
    /// <param name="ownerEntityId">The correlated subjective owner.</param>
    /// <param name="worldId">The correlated authoritative world.</param>
    /// <param name="worldStateVersion">The correlated world version.</param>
    /// <param name="simulationTick">The correlated logical tick.</param>
    /// <param name="usage">The initialized adapter-reported usage.</param>
    /// <param name="payload">The exact response payload.</param>
    /// <returns>The validated immutable response.</returns>
    public static SpeechInvocationResponseEnvelope<TResponse> Create(
        SpeechOperationKind operationKind,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<SpeechInvocationIdKind>
            invocationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<SpeechAdapterIdKind>
            adapterId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<SpeechProfileIdKind>
            speechProfileId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        SpeechUsage usage,
        TResponse payload)
    {
        SpeechTypePolicy.EnsureExactType(
            typeof(TResponse),
            typeof(ISpeechResponse),
            "speech response");

        if (!Enum.IsDefined(operationKind))
        {
            throw new ArgumentOutOfRangeException(
                nameof(operationKind),
                operationKind,
                "Unknown speech operation kind.");
        }

        EnsureNonEmpty(invocationId, nameof(invocationId));
        EnsureNonEmpty(adapterId, nameof(adapterId));
        EnsureNonEmpty(speechProfileId, nameof(speechProfileId));
        EnsureNonEmpty(ownerEntityId, nameof(ownerEntityId));
        EnsureNonEmpty(worldId, nameof(worldId));

        if (!usage.IsInitialized)
        {
            throw new ArgumentException(
                "The speech usage value must be initialized.",
                nameof(usage));
        }

        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return new SpeechInvocationResponseEnvelope<TResponse>(
            operationKind,
            invocationId,
            adapterId,
            speechProfileId,
            ownerEntityId,
            worldId,
            worldStateVersion,
            simulationTick,
            usage,
            payload);
    }

    private static void EnsureNonEmpty<TKind>(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind> id,
        string parameterName)
        where TKind : struct
    {
        if (id.IsEmpty)
        {
            throw new ArgumentException(
                "Speech response identifiers cannot be empty.",
                parameterName);
        }
    }
}
