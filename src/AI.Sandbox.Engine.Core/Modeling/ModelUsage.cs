namespace AI.Sandbox.Engine.Core.Modeling;

/// <summary>
/// Represents adapter-reported input and output usage in provider-neutral
/// adapter-defined units.
/// </summary>
public readonly record struct ModelUsage
{
    private const int MaximumValue = 1000000;
    private readonly bool initialized;
    private readonly int inputUnits;
    private readonly int outputUnits;

    private ModelUsage(int inputUnits, int outputUnits)
    {
        initialized = true;
        this.inputUnits = inputUnits;
        this.outputUnits = outputUnits;
    }

    /// <summary>
    /// Gets a value indicating whether this usage value was initialized.
    /// </summary>
    public bool IsInitialized => initialized;

    /// <summary>
    /// Gets the non-negative adapter-reported input usage.
    /// </summary>
    public int InputUnits =>
        initialized
            ? inputUnits
            : throw new InvalidOperationException(
                "The model usage value is not initialized.");

    /// <summary>
    /// Gets the non-negative adapter-reported output usage.
    /// </summary>
    public int OutputUnits =>
        initialized
            ? outputUnits
            : throw new InvalidOperationException(
                "The model usage value is not initialized.");

    /// <summary>
    /// Creates one initialized usage value.
    /// </summary>
    /// <param name="inputUnits">Input units from 0 through 1000000.</param>
    /// <param name="outputUnits">Output units from 0 through 1000000.</param>
    /// <returns>The validated usage value.</returns>
    public static ModelUsage Create(int inputUnits, int outputUnits)
    {
        if (!TryCreate(inputUnits, outputUnits, out var usage))
        {
            throw new ArgumentOutOfRangeException(
                nameof(inputUnits),
                "Model usage values must each be from 0 through 1000000.");
        }

        return usage;
    }

    /// <summary>
    /// Attempts to create one initialized usage value.
    /// </summary>
    /// <param name="inputUnits">The candidate input usage.</param>
    /// <param name="outputUnits">The candidate output usage.</param>
    /// <param name="usage">The initialized usage when successful.</param>
    /// <returns><see langword="true"/> when both values are valid.</returns>
    public static bool TryCreate(
        int inputUnits,
        int outputUnits,
        out ModelUsage usage)
    {
        if (inputUnits is < 0 or > MaximumValue ||
            outputUnits is < 0 or > MaximumValue)
        {
            usage = default;
            return false;
        }

        usage = new ModelUsage(inputUnits, outputUnits);
        return true;
    }

    /// <inheritdoc />
    public override string ToString() =>
        initialized
            ? string.Create(
                global::System.Globalization.CultureInfo.InvariantCulture,
                $"{inputUnits}:{outputUnits}")
            : string.Empty;
}
