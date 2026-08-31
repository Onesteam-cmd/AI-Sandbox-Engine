using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Perception;

/// <summary>
/// Provides one pure evaluator with the immutable candidate stimulus and exact
/// authoritative snapshot being evaluated.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
/// <typeparam name="TStimulus">The exact concrete stimulus type.</typeparam>
public sealed class PerceptionContext<TState, TStimulus>
    where TState : class, IWorldState
    where TStimulus : notnull, IPerceptionStimulus
{
    internal PerceptionContext(
        WorldStateSnapshot<TState> snapshot,
        PerceptionStimulusEnvelope<TStimulus> envelope)
    {
        Snapshot = snapshot;
        Envelope = envelope;
    }

    /// <summary>
    /// Gets the immutable authoritative snapshot being evaluated.
    /// </summary>
    public WorldStateSnapshot<TState> Snapshot { get; }

    /// <summary>
    /// Gets the immutable candidate stimulus envelope.
    /// </summary>
    public PerceptionStimulusEnvelope<TStimulus> Envelope { get; }
}
