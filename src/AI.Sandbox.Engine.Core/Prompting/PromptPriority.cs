namespace AI.Sandbox.Engine.Core.Prompting;

/// <summary>
/// Represents deterministic candidate priority in initialized basis points.
/// </summary>
public readonly record struct PromptPriority : IComparable<PromptPriority>
{
    private const int MaximumValue = 10000;
    private readonly int value;

    private PromptPriority(int value)
    {
        this.value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this priority was initialized.
    /// </summary>
    public bool IsInitialized => value > 0;

    /// <summary>
    /// Gets priority basis points from 1 through 10000.
    /// </summary>
    public int BasisPoints =>
        IsInitialized
            ? value
            : throw new InvalidOperationException(
                "Prompt priority is not initialized.");

    /// <summary>
    /// Creates initialized deterministic prompt priority.
    /// </summary>
    /// <param name="basisPoints">A value from 1 through 10000.</param>
    /// <returns>The validated priority.</returns>
    public static PromptPriority FromBasisPoints(int basisPoints)
    {
        if (!TryFromBasisPoints(basisPoints, out var priority))
        {
            throw new ArgumentOutOfRangeException(
                nameof(basisPoints),
                basisPoints,
                "Prompt priority must be from 1 through 10000 basis points.");
        }

        return priority;
    }

    /// <summary>
    /// Attempts to create initialized deterministic prompt priority.
    /// </summary>
    /// <param name="basisPoints">The candidate basis-point value.</param>
    /// <param name="priority">The validated priority when successful.</param>
    /// <returns><see langword="true"/> when the value is valid.</returns>
    public static bool TryFromBasisPoints(
        int basisPoints,
        out PromptPriority priority)
    {
        if (basisPoints is < 1 or > MaximumValue)
        {
            priority = default;
            return false;
        }

        priority = new PromptPriority(basisPoints);
        return true;
    }

    /// <inheritdoc />
    public int CompareTo(PromptPriority other) =>
        value.CompareTo(other.value);

    /// <inheritdoc />
    public override string ToString() =>
        IsInitialized
            ? value.ToString(
                global::System.Globalization.CultureInfo.InvariantCulture)
            : string.Empty;
}
