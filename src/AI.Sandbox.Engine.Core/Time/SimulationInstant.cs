namespace AI.Sandbox.Engine.Core.Time;

/// <summary>
/// Represents one deterministic point measured in integer microseconds from the
/// internal simulation epoch.
/// </summary>
public readonly record struct SimulationInstant :
    IComparable<SimulationInstant>
{
    private SimulationInstant(ulong microsecondsSinceEpoch)
    {
        MicrosecondsSinceEpoch = microsecondsSinceEpoch;
    }

    /// <summary>
    /// Gets the internal simulation epoch.
    /// </summary>
    public static SimulationInstant Epoch { get; } = default;

    /// <summary>
    /// Gets the integer microseconds elapsed since the simulation epoch.
    /// </summary>
    public ulong MicrosecondsSinceEpoch { get; }

    /// <summary>
    /// Creates an instant from integer microseconds since the simulation epoch.
    /// </summary>
    /// <param name="value">The elapsed microseconds.</param>
    /// <returns>The instant.</returns>
    public static SimulationInstant FromMicroseconds(ulong value)
    {
        return new SimulationInstant(value);
    }

    /// <summary>
    /// Adds a non-negative duration with overflow checking.
    /// </summary>
    /// <param name="duration">The duration to add.</param>
    /// <returns>The later instant.</returns>
    public SimulationInstant Add(SimulationDuration duration)
    {
        return new SimulationInstant(
            checked(
                MicrosecondsSinceEpoch +
                duration.Microseconds));
    }

    /// <summary>
    /// Calculates the non-negative duration since an earlier instant.
    /// </summary>
    /// <param name="earlier">The earlier instant.</param>
    /// <returns>The elapsed duration.</returns>
    public SimulationDuration DurationSince(SimulationInstant earlier)
    {
        if (earlier.MicrosecondsSinceEpoch >
            MicrosecondsSinceEpoch)
        {
            throw new ArgumentOutOfRangeException(
                nameof(earlier),
                earlier,
                "The supplied instant is later than this instant.");
        }

        return SimulationDuration.FromMicroseconds(
            MicrosecondsSinceEpoch -
            earlier.MicrosecondsSinceEpoch);
    }

    /// <inheritdoc />
    public int CompareTo(SimulationInstant other)
    {
        return MicrosecondsSinceEpoch.CompareTo(
            other.MicrosecondsSinceEpoch);
    }

    /// <summary>
    /// Returns the invariant elapsed microsecond representation.
    /// </summary>
    /// <returns>The instant followed by <c> us since epoch</c>.</returns>
    public override string ToString()
    {
        return string.Concat(
            MicrosecondsSinceEpoch.ToString(
                System.Globalization.CultureInfo.InvariantCulture),
            " us since epoch");
    }
}
