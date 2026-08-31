namespace AI.Sandbox.Engine.Core.Persistence;

/// <summary>
/// Represents the outer World Snapshot envelope format version.
/// </summary>
public readonly record struct SnapshotFormatVersion :
    IComparable<SnapshotFormatVersion>
{
    private SnapshotFormatVersion(uint value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the currently supported envelope format.
    /// </summary>
    public static SnapshotFormatVersion Current { get; } =
        new(1);

    /// <summary>
    /// Gets the numeric envelope format version.
    /// </summary>
    public uint Value { get; }

    /// <summary>
    /// Gets a value indicating whether this is the invalid default value.
    /// </summary>
    public bool IsEmpty => Value == 0;

    /// <summary>
    /// Creates a positive envelope format version.
    /// </summary>
    /// <param name="value">The positive version value.</param>
    /// <returns>The format version.</returns>
    public static SnapshotFormatVersion From(uint value)
    {
        if (value == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Snapshot format versions begin at one.");
        }

        return new SnapshotFormatVersion(value);
    }

    /// <inheritdoc />
    public int CompareTo(SnapshotFormatVersion other)
    {
        return Value.CompareTo(other.Value);
    }

    /// <summary>
    /// Returns the invariant decimal representation.
    /// </summary>
    /// <returns>The numeric version as invariant text.</returns>
    public override string ToString()
    {
        return Value.ToString(
            System.Globalization.CultureInfo.InvariantCulture);
    }
}
