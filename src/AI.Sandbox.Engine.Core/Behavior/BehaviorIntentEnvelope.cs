namespace AI.Sandbox.Engine.Core.Behavior;

/// <summary>
/// Captures one immutable behavior intent against an explicit authoritative
/// world snapshot.
/// </summary>
/// <typeparam name="TIntent">The exact behavior-intent payload type.</typeparam>
public sealed record BehaviorIntentEnvelope<TIntent>
    where TIntent : IBehaviorIntent
{
    private BehaviorIntentEnvelope(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<BehaviorIntentIdKind>
            intentId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> actorEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        TIntent payload)
    {
        IntentId = intentId;
        ActorEntityId = actorEntityId;
        WorldId = worldId;
        WorldStateVersion = worldStateVersion;
        SimulationTick = simulationTick;
        Payload = payload;
    }

    /// <summary>
    /// Gets the externally assigned intent ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<BehaviorIntentIdKind>
        IntentId { get; }

    /// <summary>
    /// Gets the entity whose desired outcome this intent represents.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ActorEntityId
        { get; }

    /// <summary>
    /// Gets the authoritative world observed when the intent was formed.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId
        { get; }

    /// <summary>
    /// Gets the authoritative version observed when the intent was formed.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
        WorldStateVersion { get; }

    /// <summary>
    /// Gets the logical simulation tick observed when the intent was formed.
    /// </summary>
    public ulong SimulationTick { get; }

    /// <summary>
    /// Gets the exact desired-outcome payload.
    /// </summary>
    public TIntent Payload { get; }

    /// <summary>
    /// Creates an immutable behavior-intent envelope.
    /// </summary>
    /// <param name="intentId">The externally assigned intent ID.</param>
    /// <param name="actorEntityId">The actor that owns the intent.</param>
    /// <param name="worldId">The authoritative world ID.</param>
    /// <param name="worldStateVersion">The observed world version.</param>
    /// <param name="simulationTick">The observed logical tick.</param>
    /// <param name="payload">The exact intent payload.</param>
    /// <returns>The validated immutable envelope.</returns>
    public static BehaviorIntentEnvelope<TIntent> Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<BehaviorIntentIdKind>
            intentId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> actorEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        TIntent payload)
    {
        BehaviorTypePolicy.EnsureExactType(
            typeof(TIntent),
            typeof(IBehaviorIntent),
            "behavior intent");

        EnsureNonEmpty(intentId, nameof(intentId));
        EnsureNonEmpty(actorEntityId, nameof(actorEntityId));
        EnsureNonEmpty(worldId, nameof(worldId));

        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return new BehaviorIntentEnvelope<TIntent>(
            intentId,
            actorEntityId,
            worldId,
            worldStateVersion,
            simulationTick,
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
                "Behavior identifiers cannot be empty.",
                parameterName);
        }
    }
}
