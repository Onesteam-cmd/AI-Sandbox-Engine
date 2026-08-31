namespace AI.Sandbox.Engine.Core.Time;

/// <summary>
/// Maps logical simulation ticks to deterministic integer time without reading
/// wall-clock time or rendering frame rate.
/// </summary>
public readonly record struct SimulationTimeline
{
    private SimulationTimeline(
        SimulationTickDuration tickDuration)
    {
        TickDuration = tickDuration;
    }

    /// <summary>
    /// Gets the positive duration of one logical simulation tick.
    /// </summary>
    public SimulationTickDuration TickDuration { get; }

    /// <summary>
    /// Gets a value indicating whether this is the invalid default timeline.
    /// </summary>
    public bool IsEmpty => TickDuration.IsEmpty;

    /// <summary>
    /// Creates a deterministic timeline from a positive fixed tick duration.
    /// </summary>
    /// <param name="tickDuration">The positive duration of one logical tick.</param>
    /// <returns>The timeline.</returns>
    public static SimulationTimeline Create(
        SimulationTickDuration tickDuration)
    {
        tickDuration.EnsureInitialized();

        return new SimulationTimeline(tickDuration);
    }

    /// <summary>
    /// Gets the exact instant at the start boundary of one logical tick.
    /// </summary>
    /// <param name="simulationTick">The logical tick index.</param>
    /// <returns>The exact boundary instant.</returns>
    public SimulationInstant GetInstant(ulong simulationTick)
    {
        EnsureInitialized();

        var microseconds = checked(
            simulationTick *
            TickDuration.Microseconds);

        return SimulationInstant.FromMicroseconds(
            microseconds);
    }

    /// <summary>
    /// Gets the exact duration between two logical tick boundaries.
    /// </summary>
    /// <param name="startTick">The inclusive starting tick boundary.</param>
    /// <param name="endTick">The ending tick boundary.</param>
    /// <returns>The non-negative elapsed duration.</returns>
    public SimulationDuration GetElapsedDuration(
        ulong startTick,
        ulong endTick)
    {
        EnsureInitialized();

        if (endTick < startTick)
        {
            throw new ArgumentOutOfRangeException(
                nameof(endTick),
                endTick,
                "The ending tick cannot precede the starting tick.");
        }

        var tickCount = endTick - startTick;

        return TickDuration
            .AsDuration()
            .Multiply(tickCount);
    }

    /// <summary>
    /// Gets the logical tick whose start boundary is at or immediately before
    /// an instant.
    /// </summary>
    /// <param name="instant">The deterministic instant.</param>
    /// <returns>The floor tick index.</returns>
    public ulong GetTickAtOrBefore(
        SimulationInstant instant)
    {
        EnsureInitialized();

        return instant.MicrosecondsSinceEpoch /
            TickDuration.Microseconds;
    }

    /// <summary>
    /// Gets the first logical tick whose start boundary is at or after an
    /// instant.
    /// </summary>
    /// <param name="instant">The deterministic instant.</param>
    /// <returns>The ceiling tick index.</returns>
    public ulong GetFirstTickAtOrAfter(
        SimulationInstant instant)
    {
        EnsureInitialized();

        var quotient =
            instant.MicrosecondsSinceEpoch /
            TickDuration.Microseconds;
        var remainder =
            instant.MicrosecondsSinceEpoch %
            TickDuration.Microseconds;

        return remainder == 0
            ? quotient
            : checked(quotient + 1);
    }

    /// <summary>
    /// Gets the first logical tick boundary at or after a delay measured from
    /// the start of a current logical tick.
    /// </summary>
    /// <param name="currentTick">The current logical tick boundary.</param>
    /// <param name="delay">The non-negative delay.</param>
    /// <returns>The first due tick boundary.</returns>
    public ulong GetFirstTickAtOrAfter(
        ulong currentTick,
        SimulationDuration delay)
    {
        EnsureInitialized();

        var dueInstant =
            GetInstant(currentTick).Add(delay);

        return GetFirstTickAtOrAfter(dueInstant);
    }

    /// <summary>
    /// Gets the exact duration from one tick boundary to a later tick boundary.
    /// </summary>
    /// <param name="currentTick">The current tick boundary.</param>
    /// <param name="targetTick">The target tick boundary.</param>
    /// <returns>The exact non-negative duration.</returns>
    public SimulationDuration GetDurationUntilTick(
        ulong currentTick,
        ulong targetTick)
    {
        return GetElapsedDuration(
            currentTick,
            targetTick);
    }

    private void EnsureInitialized()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException(
                "The default simulation timeline is not initialized.");
        }
    }
}
