namespace AI.Sandbox.Engine.Core.ContextRetrieval;

/// <summary>
/// Represents the maximum number of context items one query may return.
/// </summary>
public readonly record struct ContextItemLimit : IComparable<ContextItemLimit>
{
    private const int MaximumValue = 1024;
    private readonly int value;

    private ContextItemLimit(int value)
    {
        this.value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this limit was initialized.
    /// </summary>
    public bool IsInitialized => value > 0;

    /// <summary>
    /// Gets the validated positive item limit.
    /// </summary>
    public int Value =>
        IsInitialized
            ? value
            : throw new InvalidOperationException(
                "The context item limit is not initialized.");

    /// <summary>
    /// Creates a bounded context item limit.
    /// </summary>
    /// <param name="value">A value from 1 through 1024.</param>
    /// <returns>The validated limit.</returns>
    public static ContextItemLimit From(int value)
    {
        if (!TryFrom(value, out var limit))
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Context item limits must be from 1 through 1024.");
        }

        return limit;
    }

    /// <summary>
    /// Attempts to create a bounded context item limit.
    /// </summary>
    /// <param name="value">The candidate value.</param>
    /// <param name="limit">The validated limit when successful.</param>
    /// <returns><see langword="true"/> when the value is valid.</returns>
    public static bool TryFrom(int value, out ContextItemLimit limit)
    {
        if (value is < 1 or > MaximumValue)
        {
            limit = default;
            return false;
        }

        limit = new ContextItemLimit(value);
        return true;
    }

    /// <inheritdoc />
    public int CompareTo(ContextItemLimit other) =>
        value.CompareTo(other.value);

    /// <inheritdoc />
    public override string ToString() =>
        IsInitialized ? value.ToString(global::System.Globalization.CultureInfo.InvariantCulture) : string.Empty;
}
