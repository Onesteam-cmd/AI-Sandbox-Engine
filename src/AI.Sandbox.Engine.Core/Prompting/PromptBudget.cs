namespace AI.Sandbox.Engine.Core.Prompting;

/// <summary>
/// Represents a positive provider-neutral budget measured in host-defined units.
/// </summary>
public readonly record struct PromptBudget : IComparable<PromptBudget>
{
    private const int MaximumValue = 1000000;
    private readonly int value;

    private PromptBudget(int value)
    {
        this.value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this budget was initialized.
    /// </summary>
    public bool IsInitialized => value > 0;

    /// <summary>
    /// Gets the validated positive budget units.
    /// </summary>
    public int Units =>
        IsInitialized
            ? value
            : throw new InvalidOperationException(
                "The prompt budget is not initialized.");

    /// <summary>
    /// Creates a positive provider-neutral prompt budget.
    /// </summary>
    /// <param name="units">A value from 1 through 1000000.</param>
    /// <returns>The validated budget.</returns>
    public static PromptBudget FromUnits(int units)
    {
        if (!TryFromUnits(units, out var budget))
        {
            throw new ArgumentOutOfRangeException(
                nameof(units),
                units,
                "Prompt budgets must be from 1 through 1000000 units.");
        }

        return budget;
    }

    /// <summary>
    /// Attempts to create a positive provider-neutral prompt budget.
    /// </summary>
    /// <param name="units">The candidate unit count.</param>
    /// <param name="budget">The validated budget when successful.</param>
    /// <returns><see langword="true"/> when the value is valid.</returns>
    public static bool TryFromUnits(int units, out PromptBudget budget)
    {
        if (units is < 1 or > MaximumValue)
        {
            budget = default;
            return false;
        }

        budget = new PromptBudget(units);
        return true;
    }

    /// <inheritdoc />
    public int CompareTo(PromptBudget other) =>
        value.CompareTo(other.value);

    /// <inheritdoc />
    public override string ToString() =>
        IsInitialized
            ? value.ToString(
                global::System.Globalization.CultureInfo.InvariantCulture)
            : string.Empty;
}
