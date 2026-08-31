namespace AI.Sandbox.Engine.Core.Time;

/// <summary>
/// Represents the positive deterministic duration of one logical simulation
/// tick.
/// </summary>
public readonly record struct SimulationTickDuration :
    IComparable<SimulationTickDuration>
{
    private SimulationTickDuration(ulong microseconds)
    {
        Microseconds = microseconds;
    }

    /// <summary>
    /// Gets the positive integer microseconds per logical tick.
    /// </summary>
    public ulong Microseconds { get; }

    /// <summary>
    /// Gets a value indicating whether this is the invalid default value.
    /// </summary>
    public bool IsEmpty => Microseconds == 0;

    /// <summary>
    /// Creates a positive tick duration from integer microseconds.
    /// </summary>
    /// <param name="value">The positive microseconds per tick.</param>
    /// <returns>The tick duration.</returns>
    public static SimulationTickDuration FromMicroseconds(ulong value)
    {
        if (value == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "A logical simulation tick must have positive duration.");
        }

        return new SimulationTickDuration(value);
    }

    /// <summary>
    /// Creates a positive tick duration from integer milliseconds.
    /// </summary>
    /// <param name="value">The positive milliseconds per tick.</param>
    /// <returns>The tick duration.</returns>
    public static SimulationTickDuration FromMilliseconds(ulong value)
    {
        var duration =
            SimulationDuration.FromMilliseconds(value);

        return FromMicroseconds(duration.Microseconds);
    }

    /// <summary>
    /// Creates a positive tick duration from integer seconds.
    /// </summary>
    /// <param name="value">The positive seconds per tick.</param>
    /// <returns>The tick duration.</returns>
    public static SimulationTickDuration FromSeconds(ulong value)
    {
        var duration = SimulationDuration.FromSeconds(value);

        return FromMicroseconds(duration.Microseconds);
    }

    /// <summary>
    /// Converts this positive tick duration to a general duration.
    /// </summary>
    /// <returns>The equivalent duration.</returns>
    public SimulationDuration AsDuration()
    {
        EnsureInitialized();

        return SimulationDuration.FromMicroseconds(
            Microseconds);
    }

    /// <inheritdoc />
    public int CompareTo(SimulationTickDuration other)
    {
        return Microseconds.CompareTo(other.Microseconds);
    }

    /// <summary>
    /// Returns the invariant integer microsecond representation.
    /// </summary>
    /// <returns>The tick duration followed by <c> us/tick</c>.</returns>
    public override string ToString()
    {
        return string.Concat(
            Microseconds.ToString(
                System.Globalization.CultureInfo.InvariantCulture),
            " us/tick");
    }

    internal void EnsureInitialized()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException(
                "The default simulation tick duration is not initialized.");
        }
    }
}
