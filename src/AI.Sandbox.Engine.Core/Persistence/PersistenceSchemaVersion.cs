namespace AI.Sandbox.Engine.Core.Persistence;

/// <summary>
/// Represents a positive version of one persistent payload schema.
/// </summary>
public readonly record struct PersistenceSchemaVersion :
    IComparable<PersistenceSchemaVersion>
{
    private PersistenceSchemaVersion(uint value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the numeric schema version.
    /// </summary>
    public uint Value { get; }

    /// <summary>
    /// Gets a value indicating whether this is the invalid default value.
    /// </summary>
    public bool IsEmpty => Value == 0;

    /// <summary>
    /// Creates a positive schema version.
    /// </summary>
    /// <param name="value">The positive version value.</param>
    /// <returns>The schema version.</returns>
    public static PersistenceSchemaVersion From(uint value)
    {
        if (value == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Persistence schema versions begin at one.");
        }

        return new PersistenceSchemaVersion(value);
    }

    /// <inheritdoc />
    public int CompareTo(PersistenceSchemaVersion other)
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
