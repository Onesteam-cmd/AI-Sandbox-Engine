namespace AI.Sandbox.Engine.Core.Speech;

/// <summary>
/// Represents a positive provider-neutral speech input limit measured in
/// adapter-defined units.
/// </summary>
public readonly record struct SpeechInputLimit : IComparable<SpeechInputLimit>
{
    private const int MaximumValue = 1000000;
    private readonly int value;

    private SpeechInputLimit(int value)
    {
        this.value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this limit was initialized.
    /// </summary>
    public bool IsInitialized => value > 0;

    /// <summary>
    /// Gets the validated positive input-unit limit.
    /// </summary>
    public int Units =>
        IsInitialized
            ? value
            : throw new InvalidOperationException(
                "The speech input limit is not initialized.");

    /// <summary>
    /// Creates a positive provider-neutral speech input limit.
    /// </summary>
    /// <param name="units">A value from 1 through 1000000.</param>
    /// <returns>The validated limit.</returns>
    public static SpeechInputLimit FromUnits(int units)
    {
        if (!TryFromUnits(units, out var limit))
        {
            throw new ArgumentOutOfRangeException(
                nameof(units),
                units,
                "Speech input limits must be from 1 through 1000000 units.");
        }

        return limit;
    }

    /// <summary>
    /// Attempts to create a positive provider-neutral speech input limit.
    /// </summary>
    /// <param name="units">The candidate input-unit count.</param>
    /// <param name="limit">The validated limit when successful.</param>
    /// <returns><see langword="true"/> when the value is valid.</returns>
    public static bool TryFromUnits(int units, out SpeechInputLimit limit)
    {
        if (units is < 1 or > MaximumValue)
        {
            limit = default;
            return false;
        }

        limit = new SpeechInputLimit(units);
        return true;
    }

    /// <inheritdoc />
    public int CompareTo(SpeechInputLimit other) =>
        value.CompareTo(other.value);

    /// <inheritdoc />
    public override string ToString() =>
        IsInitialized
            ? value.ToString(
                global::System.Globalization.CultureInfo.InvariantCulture)
            : string.Empty;
}
