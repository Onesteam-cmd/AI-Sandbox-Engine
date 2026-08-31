namespace AI.Sandbox.Engine.Core.Time;

/// <summary>
/// Represents a non-negative deterministic simulation duration in integer
/// microseconds.
/// </summary>
public readonly record struct SimulationDuration :
    IComparable<SimulationDuration>
{
    private const ulong MicrosecondsPerMillisecond = 1_000;
    private const ulong MicrosecondsPerSecond = 1_000_000;
    private const ulong MicrosecondsPerMinute = 60_000_000;
    private const ulong MicrosecondsPerHour = 3_600_000_000;
    private const ulong MicrosecondsPerDay = 86_400_000_000;

    private SimulationDuration(ulong microseconds)
    {
        Microseconds = microseconds;
    }

    /// <summary>
    /// Gets the zero duration.
    /// </summary>
    public static SimulationDuration Zero { get; } = default;

    /// <summary>
    /// Gets the total integer microseconds.
    /// </summary>
    public ulong Microseconds { get; }

    /// <summary>
    /// Gets a value indicating whether the duration is zero.
    /// </summary>
    public bool IsZero => Microseconds == 0;

    /// <summary>
    /// Creates a duration from integer microseconds.
    /// </summary>
    /// <param name="value">The total microseconds.</param>
    /// <returns>The duration.</returns>
    public static SimulationDuration FromMicroseconds(ulong value)
    {
        return new SimulationDuration(value);
    }

    /// <summary>
    /// Creates a duration from integer milliseconds.
    /// </summary>
    /// <param name="value">The total milliseconds.</param>
    /// <returns>The duration.</returns>
    public static SimulationDuration FromMilliseconds(ulong value)
    {
        return new SimulationDuration(
            checked(value * MicrosecondsPerMillisecond));
    }

    /// <summary>
    /// Creates a duration from integer seconds.
    /// </summary>
    /// <param name="value">The total seconds.</param>
    /// <returns>The duration.</returns>
    public static SimulationDuration FromSeconds(ulong value)
    {
        return new SimulationDuration(
            checked(value * MicrosecondsPerSecond));
    }

    /// <summary>
    /// Creates a duration from integer minutes.
    /// </summary>
    /// <param name="value">The total minutes.</param>
    /// <returns>The duration.</returns>
    public static SimulationDuration FromMinutes(ulong value)
    {
        return new SimulationDuration(
            checked(value * MicrosecondsPerMinute));
    }

    /// <summary>
    /// Creates a duration from integer hours.
    /// </summary>
    /// <param name="value">The total hours.</param>
    /// <returns>The duration.</returns>
    public static SimulationDuration FromHours(ulong value)
    {
        return new SimulationDuration(
            checked(value * MicrosecondsPerHour));
    }

    /// <summary>
    /// Creates a duration from integer 24-hour days.
    /// </summary>
    /// <param name="value">The total days.</param>
    /// <returns>The duration.</returns>
    public static SimulationDuration FromDays(ulong value)
    {
        return new SimulationDuration(
            checked(value * MicrosecondsPerDay));
    }

    /// <summary>
    /// Adds two durations with overflow checking.
    /// </summary>
    /// <param name="other">The duration to add.</param>
    /// <returns>The sum.</returns>
    public SimulationDuration Add(SimulationDuration other)
    {
        return new SimulationDuration(
            checked(Microseconds + other.Microseconds));
    }

    /// <summary>
    /// Subtracts a duration without allowing a negative result.
    /// </summary>
    /// <param name="other">The duration to subtract.</param>
    /// <returns>The non-negative difference.</returns>
    public SimulationDuration Subtract(SimulationDuration other)
    {
        if (other.Microseconds > Microseconds)
        {
            throw new ArgumentOutOfRangeException(
                nameof(other),
                other,
                "A simulation duration cannot become negative.");
        }

        return new SimulationDuration(
            Microseconds - other.Microseconds);
    }

    /// <summary>
    /// Multiplies this duration by an integer factor with overflow checking.
    /// </summary>
    /// <param name="factor">The non-negative integer factor.</param>
    /// <returns>The multiplied duration.</returns>
    public SimulationDuration Multiply(ulong factor)
    {
        return new SimulationDuration(
            checked(Microseconds * factor));
    }

    /// <inheritdoc />
    public int CompareTo(SimulationDuration other)
    {
        return Microseconds.CompareTo(other.Microseconds);
    }

    /// <summary>
    /// Returns the invariant integer microsecond representation.
    /// </summary>
    /// <returns>The duration followed by <c> us</c>.</returns>
    public override string ToString()
    {
        return string.Concat(
            Microseconds.ToString(
                System.Globalization.CultureInfo.InvariantCulture),
            " us");
    }
}
