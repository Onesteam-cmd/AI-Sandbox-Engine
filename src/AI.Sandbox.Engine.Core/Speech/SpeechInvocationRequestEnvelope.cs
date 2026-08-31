namespace AI.Sandbox.Engine.Core.Speech;

/// <summary>
/// Captures one immutable recognition or synthesis request with explicit
/// subject, authority, operation, adapter, profile, and usage limits.
/// </summary>
/// <typeparam name="TRequest">The exact speech-request payload type.</typeparam>
public sealed record SpeechInvocationRequestEnvelope<TRequest>
    where TRequest : ISpeechRequest
{
    private SpeechInvocationRequestEnvelope(
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
        SpeechInputLimit inputLimit,
        SpeechOutputLimit outputLimit,
        TRequest payload)
    {
        OperationKind = operationKind;
        InvocationId = invocationId;
        AdapterId = adapterId;
        SpeechProfileId = speechProfileId;
        OwnerEntityId = ownerEntityId;
        WorldId = worldId;
        WorldStateVersion = worldStateVersion;
        SimulationTick = simulationTick;
        InputLimit = inputLimit;
        OutputLimit = outputLimit;
        Payload = payload;
    }

    /// <summary>
    /// Gets the recognition or synthesis operation.
    /// </summary>
    public SpeechOperationKind OperationKind { get; }

    /// <summary>
    /// Gets the externally assigned invocation ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<SpeechInvocationIdKind>
        InvocationId { get; }

    /// <summary>
    /// Gets the host-configured speech adapter selected for this invocation.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<SpeechAdapterIdKind>
        AdapterId { get; }

    /// <summary>
    /// Gets the opaque recognition or voice profile.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<SpeechProfileIdKind>
        SpeechProfileId { get; }

    /// <summary>
    /// Gets the subjective owner represented by this request.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OwnerEntityId
        { get; }

    /// <summary>
    /// Gets the authoritative world represented by this request.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId
        { get; }

    /// <summary>
    /// Gets the authoritative version represented by this request.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
        WorldStateVersion { get; }

    /// <summary>
    /// Gets the logical simulation tick represented by this request.
    /// </summary>
    public ulong SimulationTick { get; }

    /// <summary>
    /// Gets the positive provider-neutral input limit.
    /// </summary>
    public SpeechInputLimit InputLimit { get; }

    /// <summary>
    /// Gets the positive provider-neutral output limit.
    /// </summary>
    public SpeechOutputLimit OutputLimit { get; }

    /// <summary>
    /// Gets the exact speech-request payload.
    /// </summary>
    public TRequest Payload { get; }

    /// <summary>
    /// Creates one immutable provider-neutral speech request.
    /// </summary>
    /// <param name="operationKind">The recognition or synthesis operation.</param>
    /// <param name="invocationId">The externally assigned invocation ID.</param>
    /// <param name="adapterId">The selected speech adapter ID.</param>
    /// <param name="speechProfileId">The selected opaque profile ID.</param>
    /// <param name="ownerEntityId">The subjective owner entity.</param>
    /// <param name="worldId">The represented authoritative world.</param>
    /// <param name="worldStateVersion">The represented world version.</param>
    /// <param name="simulationTick">The represented logical tick.</param>
    /// <param name="inputLimit">The positive input limit.</param>
    /// <param name="outputLimit">The positive output limit.</param>
    /// <param name="payload">The exact request payload.</param>
    /// <returns>The validated immutable request.</returns>
    public static SpeechInvocationRequestEnvelope<TRequest> Create(
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
        SpeechInputLimit inputLimit,
        SpeechOutputLimit outputLimit,
        TRequest payload)
    {
        SpeechTypePolicy.EnsureExactType(
            typeof(TRequest),
            typeof(ISpeechRequest),
            "speech request");

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

        if (!inputLimit.IsInitialized)
        {
            throw new ArgumentException(
                "The speech input limit must be initialized.",
                nameof(inputLimit));
        }

        if (!outputLimit.IsInitialized)
        {
            throw new ArgumentException(
                "The speech output limit must be initialized.",
                nameof(outputLimit));
        }

        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return new SpeechInvocationRequestEnvelope<TRequest>(
            operationKind,
            invocationId,
            adapterId,
            speechProfileId,
            ownerEntityId,
            worldId,
            worldStateVersion,
            simulationTick,
            inputLimit,
            outputLimit,
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
                "Speech invocation identifiers cannot be empty.",
                parameterName);
        }
    }
}
