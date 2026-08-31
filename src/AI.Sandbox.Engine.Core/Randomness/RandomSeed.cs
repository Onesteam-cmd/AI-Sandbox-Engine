namespace AI.Sandbox.Engine.Core.Randomness;

/// <summary>
/// Represents one explicitly initialized 64-bit root seed.
/// </summary>
public readonly record struct RandomSeed
{
    private readonly bool isInitialized;
    private readonly ulong value;

    private RandomSeed(ulong value)
    {
        this.value = value;
        isInitialized = true;
    }

    /// <summary>
    /// Gets the seed value.
    /// </summary>
    public ulong Value => value;

    /// <summary>
    /// Gets a value indicating whether this is the invalid default value.
    /// </summary>
    public bool IsEmpty => !isInitialized;

    /// <summary>
    /// Creates an initialized seed. Every 64-bit value, including zero, is
    /// valid.
    /// </summary>
    /// <param name="value">The explicit root seed.</param>
    /// <returns>The initialized seed.</returns>
    public static RandomSeed From(ulong value)
    {
        return new RandomSeed(value);
    }

    /// <summary>
    /// Returns the invariant hexadecimal seed representation.
    /// </summary>
    /// <returns>Sixteen lowercase hexadecimal digits.</returns>
    public override string ToString()
    {
        return value.ToString(
            "x16",
            System.Globalization.CultureInfo.InvariantCulture);
    }
}
