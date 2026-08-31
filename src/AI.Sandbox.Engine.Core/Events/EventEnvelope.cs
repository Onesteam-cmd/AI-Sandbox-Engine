using AI.Sandbox.Engine.Core.Identifiers;

namespace AI.Sandbox.Engine.Core.Events;

/// <summary>
/// Associates an engine event payload with authoritative ordering metadata.
/// </summary>
/// <typeparam name="TEvent">The exact event payload type.</typeparam>
/// <remarks>
/// Sequence and simulation tick values are supplied by the authoritative
/// runtime. This type does not read wall-clock time or generate identifiers.
/// </remarks>
public readonly record struct EventEnvelope<TEvent>
    where TEvent : IEngineEvent
{
    private EventEnvelope(
        Id<EventIdKind> eventId,
        ulong sequence,
        ulong simulationTick,
        TEvent payload)
    {
        EventId = eventId;
        Sequence = sequence;
        SimulationTick = simulationTick;
        Payload = payload;
    }

    /// <summary>
    /// Gets the unique identifier assigned to this event occurrence.
    /// </summary>
    public Id<EventIdKind> EventId { get; }

    /// <summary>
    /// Gets the authoritative total-order sequence assigned to the event.
    /// </summary>
    public ulong Sequence { get; }

    /// <summary>
    /// Gets the simulation tick at which the event occurred.
    /// </summary>
    public ulong SimulationTick { get; }

    /// <summary>
    /// Gets the immutable event payload.
    /// </summary>
    public TEvent Payload { get; }

    /// <summary>
    /// Gets a value indicating whether this envelope was created through its
    /// validated factory.
    /// </summary>
    public bool IsValid => !EventId.IsEmpty && Payload is not null;

    /// <summary>
    /// Creates a validated event envelope.
    /// </summary>
    /// <param name="eventId">The externally assigned non-empty event ID.</param>
    /// <param name="sequence">The authoritative event sequence.</param>
    /// <param name="simulationTick">The authoritative simulation tick.</param>
    /// <param name="payload">The event payload.</param>
    /// <returns>A validated event envelope.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="eventId"/> is empty.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a reference-type <paramref name="payload"/> is null.
    /// </exception>
    public static EventEnvelope<TEvent> Create(
        Id<EventIdKind> eventId,
        ulong sequence,
        ulong simulationTick,
        TEvent payload)
    {
        if (eventId.IsEmpty)
        {
            throw new ArgumentException(
                "An event envelope requires a non-empty event identifier.",
                nameof(eventId));
        }

        ArgumentNullException.ThrowIfNull(payload);

        return new EventEnvelope<TEvent>(
            eventId,
            sequence,
            simulationTick,
            payload);
    }
}
