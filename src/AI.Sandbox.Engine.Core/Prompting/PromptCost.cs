namespace AI.Sandbox.Engine.Core.Prompting;

/// <summary>
/// Represents one positive estimated prompt cost in host-defined units.
/// </summary>
public readonly record struct PromptCost : IComparable<PromptCost>
{
    private const int MaximumValue = 1000000;
    private readonly int value;

    private PromptCost(int value)
    {
        this.value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this cost was initialized.
    /// </summary>
    public bool IsInitialized => value > 0;

    /// <summary>
    /// Gets the validated positive estimated cost units.
    /// </summary>
    public int Units =>
        IsInitialized
            ? value
            : throw new InvalidOperationException(
                "The prompt cost is not initialized.");

    /// <summary>
    /// Creates a positive estimated prompt cost.
    /// </summary>
    /// <param name="units">A value from 1 through 1000000.</param>
    /// <returns>The validated cost.</returns>
    public static PromptCost FromUnits(int units)
    {
        if (!TryFromUnits(units, out var cost))
        {
            throw new ArgumentOutOfRangeException(
                nameof(units),
                units,
                "Prompt costs must be from 1 through 1000000 units.");
        }

        return cost;
    }

    /// <summary>
    /// Attempts to create a positive estimated prompt cost.
    /// </summary>
    /// <param name="units">The candidate unit count.</param>
    /// <param name="cost">The validated cost when successful.</param>
    /// <returns><see langword="true"/> when the value is valid.</returns>
    public static bool TryFromUnits(int units, out PromptCost cost)
    {
        if (units is < 1 or > MaximumValue)
        {
            cost = default;
            return false;
        }

        cost = new PromptCost(units);
        return true;
    }

    /// <inheritdoc />
    public int CompareTo(PromptCost other) =>
        value.CompareTo(other.value);

    /// <inheritdoc />
    public override string ToString() =>
        IsInitialized
            ? value.ToString(
                global::System.Globalization.CultureInfo.InvariantCulture)
            : string.Empty;
}
