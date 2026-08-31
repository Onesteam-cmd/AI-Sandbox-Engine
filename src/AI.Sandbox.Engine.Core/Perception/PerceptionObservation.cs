using AI.Sandbox.Engine.Core.Entities;
using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Perception;

/// <summary>
/// Describes one subjective signal obtained by one observer from one candidate
/// stimulus at one exact authoritative snapshot.
/// </summary>
/// <typeparam name="TSignal">The exact concrete signal type.</typeparam>
public sealed class PerceptionObservation<TSignal>
    where TSignal : notnull, IPerceptionSignal
{
    internal PerceptionObservation(
        Id<PerceptionStimulusIdKind> stimulusId,
        Id<PerceptionChannelIdKind> channelId,
        Id<EntityIdKind> observerEntityId,
        Id<WorldIdKind> worldId,
        WorldStateVersion worldStateVersion,
        ulong simulationTick,
        PerceptionConfidence confidence,
        TSignal signal)
    {
        StimulusId = stimulusId;
        ChannelId = channelId;
        ObserverEntityId = observerEntityId;
        WorldId = worldId;
        WorldStateVersion = worldStateVersion;
        SimulationTick = simulationTick;
        Confidence = confidence;
        Signal = signal;
    }

    /// <summary>
    /// Gets the source candidate stimulus ID.
    /// </summary>
    public Id<PerceptionStimulusIdKind> StimulusId { get; }

    /// <summary>
    /// Gets the sensory channel ID.
    /// </summary>
    public Id<PerceptionChannelIdKind> ChannelId { get; }

    /// <summary>
    /// Gets the observer entity ID.
    /// </summary>
    public Id<EntityIdKind> ObserverEntityId { get; }

    /// <summary>
    /// Gets the world in which evaluation occurred.
    /// </summary>
    public Id<WorldIdKind> WorldId { get; }

    /// <summary>
    /// Gets the exact World State version evaluated.
    /// </summary>
    public WorldStateVersion WorldStateVersion { get; }

    /// <summary>
    /// Gets the exact logical simulation tick evaluated.
    /// </summary>
    public ulong SimulationTick { get; }

    /// <summary>
    /// Gets the evaluator-supplied subjective confidence.
    /// </summary>
    public PerceptionConfidence Confidence { get; }

    /// <summary>
    /// Gets the immutable subjective signal payload.
    /// </summary>
    public TSignal Signal { get; }
}
