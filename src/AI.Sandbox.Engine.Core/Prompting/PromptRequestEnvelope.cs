namespace AI.Sandbox.Engine.Core.Prompting;

/// <summary>
/// Captures one immutable owner-scoped prompt request against explicit
/// authoritative snapshot coordinates.
/// </summary>
/// <typeparam name="TRequest">The exact prompt-request payload type.</typeparam>
public sealed record PromptRequestEnvelope<TRequest>
    where TRequest : IPromptRequest
{
    private PromptRequestEnvelope(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<PromptRequestIdKind>
            requestId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        PromptBudget budget,
        TRequest payload)
    {
        RequestId = requestId;
        OwnerEntityId = ownerEntityId;
        WorldId = worldId;
        WorldStateVersion = worldStateVersion;
        SimulationTick = simulationTick;
        Budget = budget;
        Payload = payload;
    }

    /// <summary>
    /// Gets the externally assigned prompt-request ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<PromptRequestIdKind>
        RequestId { get; }

    /// <summary>
    /// Gets the owner whose subjective context is being composed.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OwnerEntityId
        { get; }

    /// <summary>
    /// Gets the authoritative world observed for this request.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId
        { get; }

    /// <summary>
    /// Gets the authoritative version observed for this request.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
        WorldStateVersion { get; }

    /// <summary>
    /// Gets the logical simulation tick observed for this request.
    /// </summary>
    public ulong SimulationTick { get; }

    /// <summary>
    /// Gets the provider-neutral composition budget.
    /// </summary>
    public PromptBudget Budget { get; }

    /// <summary>
    /// Gets the exact prompt-request payload.
    /// </summary>
    public TRequest Payload { get; }

    /// <summary>
    /// Creates one immutable owner-scoped prompt request.
    /// </summary>
    /// <param name="requestId">The externally assigned request ID.</param>
    /// <param name="ownerEntityId">The subjective owner entity.</param>
    /// <param name="worldId">The authoritative world ID.</param>
    /// <param name="worldStateVersion">The observed world version.</param>
    /// <param name="simulationTick">The observed logical tick.</param>
    /// <param name="budget">The provider-neutral composition budget.</param>
    /// <param name="payload">The exact request payload.</param>
    /// <returns>The validated immutable request.</returns>
    public static PromptRequestEnvelope<TRequest> Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<PromptRequestIdKind>
            requestId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        PromptBudget budget,
        TRequest payload)
    {
        PromptTypePolicy.EnsureExactType(
            typeof(TRequest),
            typeof(IPromptRequest),
            "prompt request");

        EnsureNonEmpty(requestId, nameof(requestId));
        EnsureNonEmpty(ownerEntityId, nameof(ownerEntityId));
        EnsureNonEmpty(worldId, nameof(worldId));

        if (!budget.IsInitialized)
        {
            throw new ArgumentException(
                "The prompt budget must be initialized.",
                nameof(budget));
        }

        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return new PromptRequestEnvelope<TRequest>(
            requestId,
            ownerEntityId,
            worldId,
            worldStateVersion,
            simulationTick,
            budget,
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
                "Prompt identifiers cannot be empty.",
                parameterName);
        }
    }
}
