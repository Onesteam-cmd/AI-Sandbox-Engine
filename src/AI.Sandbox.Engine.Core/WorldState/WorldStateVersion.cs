namespace AI.Sandbox.Engine.Core.WorldState;

/// <summary>
/// Represents the monotonically increasing version of one authoritative world
/// state.
/// </summary>
public readonly record struct WorldStateVersion :
    IComparable<WorldStateVersion>
{
    private WorldStateVersion(ulong value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the numeric version value.
    /// </summary>
    public ulong Value { get; }

    /// <summary>
    /// Gets the initial world-state version.
    /// </summary>
    public static WorldStateVersion Initial => default;

    /// <summary>
    /// Gets a value indicating whether this is the initial version.
    /// </summary>
    public bool IsInitial => Value == 0;

    /// <summary>
    /// Creates a version from an externally supplied value.
    /// </summary>
    /// <param name="value">The version value.</param>
    /// <returns>A world-state version.</returns>
    public static WorldStateVersion From(ulong value)
    {
        return new WorldStateVersion(value);
    }

    /// <inheritdoc />
    public int CompareTo(WorldStateVersion other)
    {
        return Value.CompareTo(other.Value);
    }

    /// <summary>
    /// Returns the invariant decimal representation of this version.
    /// </summary>
    /// <returns>The numeric version as invariant text.</returns>
    public override string ToString()
    {
        return Value.ToString(
            System.Globalization.CultureInfo.InvariantCulture);
    }

    internal WorldStateVersion Next()
    {
        return new WorldStateVersion(checked(Value + 1));
    }
}
