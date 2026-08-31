using AI.Sandbox.Engine.Core.Entities;
using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Perception;

/// <summary>
/// Captures one immutable candidate stimulus and the exact observer and World
/// State metadata on which evaluation must be based.
/// </summary>
/// <typeparam name="TStimulus">The exact concrete stimulus type.</typeparam>
public sealed class PerceptionStimulusEnvelope<TStimulus>
    where TStimulus : notnull, IPerceptionStimulus
{
    private PerceptionStimulusEnvelope(
        Id<PerceptionStimulusIdKind> stimulusId,
        Id<PerceptionChannelIdKind> channelId,
        Id<EntityIdKind> observerEntityId,
        Id<WorldIdKind> worldId,
        WorldStateVersion expectedWorldStateVersion,
        ulong expectedSimulationTick,
        TStimulus payload)
    {
        StimulusId = stimulusId;
        ChannelId = channelId;
        ObserverEntityId = observerEntityId;
        WorldId = worldId;
        ExpectedWorldStateVersion = expectedWorldStateVersion;
        ExpectedSimulationTick = expectedSimulationTick;
        Payload = payload;
    }

    /// <summary>
    /// Gets the externally assigned candidate stimulus ID.
    /// </summary>
    public Id<PerceptionStimulusIdKind> StimulusId { get; }

    /// <summary>
    /// Gets the stable sensory channel ID.
    /// </summary>
    public Id<PerceptionChannelIdKind> ChannelId { get; }

    /// <summary>
    /// Gets the entity for whom perception is evaluated.
    /// </summary>
    public Id<EntityIdKind> ObserverEntityId { get; }

    /// <summary>
    /// Gets the intended world.
    /// </summary>
    public Id<WorldIdKind> WorldId { get; }

    /// <summary>
    /// Gets the World State version observed when the stimulus was formed.
    /// </summary>
    public WorldStateVersion ExpectedWorldStateVersion { get; }

    /// <summary>
    /// Gets the logical tick observed when the stimulus was formed.
    /// </summary>
    public ulong ExpectedSimulationTick { get; }

    /// <summary>
    /// Gets the immutable exact stimulus payload.
    /// </summary>
    public TStimulus Payload { get; }

    /// <summary>
    /// Creates one version-gated candidate stimulus.
    /// </summary>
    /// <param name="stimulusId">The non-empty externally assigned ID.</param>
    /// <param name="channelId">The non-empty stable channel ID.</param>
    /// <param name="observerEntityId">The non-empty observer entity ID.</param>
    /// <param name="worldId">The non-empty intended world ID.</param>
    /// <param name="expectedWorldStateVersion">
    /// The World State version used to form the candidate.
    /// </param>
    /// <param name="expectedSimulationTick">
    /// The logical tick used to form the candidate.
    /// </param>
    /// <param name="payload">The immutable concrete stimulus.</param>
    /// <returns>The stimulus envelope.</returns>
    public static PerceptionStimulusEnvelope<TStimulus> Create(
        Id<PerceptionStimulusIdKind> stimulusId,
        Id<PerceptionChannelIdKind> channelId,
        Id<EntityIdKind> observerEntityId,
        Id<WorldIdKind> worldId,
        WorldStateVersion expectedWorldStateVersion,
        ulong expectedSimulationTick,
        TStimulus payload)
    {
        EnsureNonEmpty(
            stimulusId,
            nameof(stimulusId));
        EnsureNonEmpty(
            channelId,
            nameof(channelId));
        EnsureNonEmpty(
            observerEntityId,
            nameof(observerEntityId));
        EnsureNonEmpty(
            worldId,
            nameof(worldId));
        PerceptionTypePolicy.EnsureConcrete<TStimulus>(
            nameof(TStimulus));
        PerceptionTypePolicy.EnsureValue(
            payload,
            nameof(payload));

        return new PerceptionStimulusEnvelope<TStimulus>(
            stimulusId,
            channelId,
            observerEntityId,
            worldId,
            expectedWorldStateVersion,
            expectedSimulationTick,
            payload);
    }

    private static void EnsureNonEmpty<TKind>(
        Id<TKind> id,
        string parameterName)
        where TKind : struct
    {
        if (id.IsEmpty)
        {
            throw new ArgumentException(
                "Perception envelope IDs cannot be empty.",
                parameterName);
        }
    }
}
