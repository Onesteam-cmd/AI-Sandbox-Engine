namespace AI.Sandbox.Engine.Core.StructuredOutput;

/// <summary>
/// Represents one initialized positive structured-output schema version.
/// </summary>
public readonly record struct StructuredOutputSchemaVersion :
    IComparable<StructuredOutputSchemaVersion>
{
    private readonly bool initialized;
    private readonly int value;

    private StructuredOutputSchemaVersion(int value)
    {
        initialized = true;
        this.value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this schema version was initialized.
    /// </summary>
    public bool IsInitialized => initialized;

    /// <summary>
    /// Gets the positive schema version.
    /// </summary>
    public int Value =>
        initialized
            ? value
            : throw new InvalidOperationException(
                "The structured-output schema version is not initialized.");

    /// <summary>
    /// Creates one positive schema version.
    /// </summary>
    /// <param name="value">The positive version number.</param>
    /// <returns>The initialized schema version.</returns>
    public static StructuredOutputSchemaVersion From(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "Structured-output schema versions must be positive.");
        }

        return new StructuredOutputSchemaVersion(value);
    }

    /// <inheritdoc />
    public int CompareTo(StructuredOutputSchemaVersion other)
    {
        if (!IsInitialized)
        {
            return other.IsInitialized ? -1 : 0;
        }

        if (!other.IsInitialized)
        {
            return 1;
        }

        return Value.CompareTo(other.Value);
    }

    /// <inheritdoc />
    public override string ToString() =>
        initialized
            ? value.ToString(
                global::System.Globalization.CultureInfo.InvariantCulture)
            : string.Empty;
}
