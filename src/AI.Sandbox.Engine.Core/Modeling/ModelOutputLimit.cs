namespace AI.Sandbox.Engine.Core.Modeling;

/// <summary>
/// Represents a positive provider-neutral output limit measured in
/// adapter-defined units.
/// </summary>
public readonly record struct ModelOutputLimit : IComparable<ModelOutputLimit>
{
    private const int MaximumValue = 1000000;
    private readonly int value;

    private ModelOutputLimit(int value)
    {
        this.value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this limit was initialized.
    /// </summary>
    public bool IsInitialized => value > 0;

    /// <summary>
    /// Gets the validated positive output-unit limit.
    /// </summary>
    public int Units =>
        IsInitialized
            ? value
            : throw new InvalidOperationException(
                "The model output limit is not initialized.");

    /// <summary>
    /// Creates a positive provider-neutral model output limit.
    /// </summary>
    /// <param name="units">A value from 1 through 1000000.</param>
    /// <returns>The validated limit.</returns>
    public static ModelOutputLimit FromUnits(int units)
    {
        if (!TryFromUnits(units, out var limit))
        {
            throw new ArgumentOutOfRangeException(
                nameof(units),
                units,
                "Model output limits must be from 1 through 1000000 units.");
        }

        return limit;
    }

    /// <summary>
    /// Attempts to create a positive provider-neutral output limit.
    /// </summary>
    /// <param name="units">The candidate output-unit count.</param>
    /// <param name="limit">The validated limit when successful.</param>
    /// <returns><see langword="true"/> when the value is valid.</returns>
    public static bool TryFromUnits(int units, out ModelOutputLimit limit)
    {
        if (units is < 1 or > MaximumValue)
        {
            limit = default;
            return false;
        }

        limit = new ModelOutputLimit(units);
        return true;
    }

    /// <inheritdoc />
    public int CompareTo(ModelOutputLimit other) =>
        value.CompareTo(other.value);

    /// <inheritdoc />
    public override string ToString() =>
        IsInitialized
            ? value.ToString(
                global::System.Globalization.CultureInfo.InvariantCulture)
            : string.Empty;
}
