namespace AI.Sandbox.Engine.Core.Randomness;

/// <summary>
/// Represents the persisted deterministic random algorithm version.
/// </summary>
public readonly record struct RandomAlgorithmVersion :
    IComparable<RandomAlgorithmVersion>
{
    private RandomAlgorithmVersion(uint value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the currently supported SplitMix64 algorithm contract.
    /// </summary>
    public static RandomAlgorithmVersion Current { get; } = new(1);

    /// <summary>
    /// Gets the positive algorithm version.
    /// </summary>
    public uint Value { get; }

    /// <summary>
    /// Gets a value indicating whether this is the invalid default value.
    /// </summary>
    public bool IsEmpty => Value == 0;

    /// <summary>
    /// Creates a positive algorithm version value for persistence metadata.
    /// </summary>
    /// <param name="value">The positive numeric version.</param>
    /// <returns>The version value.</returns>
    public static RandomAlgorithmVersion From(uint value)
    {
        if (value == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Random algorithm versions begin at one.");
        }

        return new RandomAlgorithmVersion(value);
    }

    /// <inheritdoc />
    public int CompareTo(RandomAlgorithmVersion other)
    {
        return Value.CompareTo(other.Value);
    }

    /// <summary>
    /// Returns the invariant decimal representation.
    /// </summary>
    /// <returns>The numeric version.</returns>
    public override string ToString()
    {
        return Value.ToString(
            System.Globalization.CultureInfo.InvariantCulture);
    }
}
