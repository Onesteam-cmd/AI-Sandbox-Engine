namespace AI.Sandbox.Engine.Core.Behavior;

/// <summary>
/// Captures one immutable concrete action proposal against an explicit
/// authoritative world snapshot.
/// </summary>
/// <typeparam name="TAction">The exact action-proposal payload type.</typeparam>
public sealed record ActionProposalEnvelope<TAction>
    where TAction : IActionProposal
{
    private ActionProposalEnvelope(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ActionProposalIdKind>
            proposalId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> actorEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<BehaviorIntentIdKind>?
            sourceIntentId,
        TAction payload)
    {
        ProposalId = proposalId;
        ActorEntityId = actorEntityId;
        WorldId = worldId;
        WorldStateVersion = worldStateVersion;
        SimulationTick = simulationTick;
        SourceIntentId = sourceIntentId;
        Payload = payload;
    }

    /// <summary>
    /// Gets the externally assigned action-proposal ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<ActionProposalIdKind>
        ProposalId { get; }

    /// <summary>
    /// Gets the entity requesting the concrete action.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ActorEntityId
        { get; }

    /// <summary>
    /// Gets the authoritative world observed for this proposal.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId
        { get; }

    /// <summary>
    /// Gets the authoritative version observed for this proposal.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
        WorldStateVersion { get; }

    /// <summary>
    /// Gets the logical simulation tick observed for this proposal.
    /// </summary>
    public ulong SimulationTick { get; }

    /// <summary>
    /// Gets the optional behavior intent from which this proposal was derived.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<BehaviorIntentIdKind>?
        SourceIntentId { get; }

    /// <summary>
    /// Gets the exact concrete action payload.
    /// </summary>
    public TAction Payload { get; }

    /// <summary>
    /// Creates a proposal derived from one validated behavior intent.
    /// </summary>
    /// <typeparam name="TIntent">The exact source-intent payload type.</typeparam>
    /// <param name="proposalId">The externally assigned proposal ID.</param>
    /// <param name="sourceIntent">The source behavior intent.</param>
    /// <param name="payload">The exact action payload.</param>
    /// <returns>The validated immutable proposal.</returns>
    public static ActionProposalEnvelope<TAction> CreateFromIntent<TIntent>(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ActionProposalIdKind>
            proposalId,
        BehaviorIntentEnvelope<TIntent> sourceIntent,
        TAction payload)
        where TIntent : IBehaviorIntent
    {
        ArgumentNullException.ThrowIfNull(sourceIntent);

        return CreateCore(
            proposalId,
            sourceIntent.ActorEntityId,
            sourceIntent.WorldId,
            sourceIntent.WorldStateVersion,
            sourceIntent.SimulationTick,
            sourceIntent.IntentId,
            payload);
    }

    /// <summary>
    /// Creates a proposal whose source is outside the behavior-intent layer.
    /// </summary>
    /// <param name="proposalId">The externally assigned proposal ID.</param>
    /// <param name="actorEntityId">The actor requesting the action.</param>
    /// <param name="worldId">The authoritative world ID.</param>
    /// <param name="worldStateVersion">The observed world version.</param>
    /// <param name="simulationTick">The observed logical tick.</param>
    /// <param name="payload">The exact action payload.</param>
    /// <returns>The validated immutable proposal.</returns>
    public static ActionProposalEnvelope<TAction> CreateExternal(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ActionProposalIdKind>
            proposalId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> actorEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        TAction payload) =>
        CreateCore(
            proposalId,
            actorEntityId,
            worldId,
            worldStateVersion,
            simulationTick,
            null,
            payload);

    private static ActionProposalEnvelope<TAction> CreateCore(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ActionProposalIdKind>
            proposalId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> actorEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            worldStateVersion,
        ulong simulationTick,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<BehaviorIntentIdKind>?
            sourceIntentId,
        TAction payload)
    {
        BehaviorTypePolicy.EnsureExactType(
            typeof(TAction),
            typeof(IActionProposal),
            "action proposal");

        EnsureNonEmpty(proposalId, nameof(proposalId));
        EnsureNonEmpty(actorEntityId, nameof(actorEntityId));
        EnsureNonEmpty(worldId, nameof(worldId));

        if (sourceIntentId.HasValue && sourceIntentId.Value.IsEmpty)
        {
            throw new ArgumentException(
                "The source behavior-intent ID cannot be empty.",
                nameof(sourceIntentId));
        }

        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return new ActionProposalEnvelope<TAction>(
            proposalId,
            actorEntityId,
            worldId,
            worldStateVersion,
            simulationTick,
            sourceIntentId,
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
