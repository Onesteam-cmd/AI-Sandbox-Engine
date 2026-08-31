namespace AI.Sandbox.Engine.Core.ContextRetrieval;

/// <summary>
/// Captures one immutable owner-scoped context query against an explicit
/// authoritative world snapshot.
/// </summary>
/// <typeparam name="TQuery">The exact context-query payload type.</typeparam>
public sealed record ContextQueryEnvelope<TQuery>
    where TQuery : IContextQuery
{
    private ContextQueryEnvelope(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ContextQueryIdKind>
            queryId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        ContextItemLimit itemLimit,
        TQuery payload)
    {
        QueryId = queryId;
        OwnerEntityId = ownerEntityId;
        WorldId = worldId;
        WorldStateVersion = worldStateVersion;
        SimulationTick = simulationTick;
        ItemLimit = itemLimit;
        Payload = payload;
    }

    /// <summary>
    /// Gets the externally assigned query ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<ContextQueryIdKind>
        QueryId { get; }

    /// <summary>
    /// Gets the entity whose subjective context may be retrieved.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OwnerEntityId
        { get; }

    /// <summary>
    /// Gets the authoritative world observed for this query.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId
        { get; }

    /// <summary>
    /// Gets the authoritative version observed for this query.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
        WorldStateVersion { get; }

    /// <summary>
    /// Gets the logical simulation tick observed for this query.
    /// </summary>
    public ulong SimulationTick { get; }

    /// <summary>
    /// Gets the maximum number of returned context items.
    /// </summary>
    public ContextItemLimit ItemLimit { get; }

    /// <summary>
    /// Gets the exact context-query payload.
    /// </summary>
    public TQuery Payload { get; }

    /// <summary>
    /// Creates one immutable owner-scoped context query.
    /// </summary>
    /// <param name="queryId">The externally assigned query ID.</param>
    /// <param name="ownerEntityId">The owner of subjective context.</param>
    /// <param name="worldId">The authoritative world ID.</param>
    /// <param name="worldStateVersion">The observed world version.</param>
    /// <param name="simulationTick">The observed logical tick.</param>
    /// <param name="itemLimit">The maximum returned item count.</param>
    /// <param name="payload">The exact query payload.</param>
    /// <returns>The validated immutable envelope.</returns>
    public static ContextQueryEnvelope<TQuery> Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ContextQueryIdKind>
            queryId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        ContextItemLimit itemLimit,
        TQuery payload)
    {
        ContextTypePolicy.EnsureExactType(
            typeof(TQuery),
            typeof(IContextQuery),
            "context query");

        EnsureNonEmpty(queryId, nameof(queryId));
        EnsureNonEmpty(ownerEntityId, nameof(ownerEntityId));
        EnsureNonEmpty(worldId, nameof(worldId));

        if (!itemLimit.IsInitialized)
        {
            throw new ArgumentException(
                "The context item limit must be initialized.",
                nameof(itemLimit));
        }

        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return new ContextQueryEnvelope<TQuery>(
            queryId,
            ownerEntityId,
            worldId,
            worldStateVersion,
            simulationTick,
            itemLimit,
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
                "Context identifiers cannot be empty.",
                parameterName);
        }
    }
}
