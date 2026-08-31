namespace AI.Sandbox.Engine.Core.ContextRetrieval;

/// <summary>
/// Represents deterministic context relevance in initialized basis points.
/// </summary>
public readonly record struct ContextRelevance : IComparable<ContextRelevance>
{
    private const int MaximumValue = 10000;
    private readonly int value;

    private ContextRelevance(int value)
    {
        this.value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this relevance was initialized.
    /// </summary>
    public bool IsInitialized => value > 0;

    /// <summary>
    /// Gets relevance basis points from 1 through 10000.
    /// </summary>
    public int BasisPoints =>
        IsInitialized
            ? value
            : throw new InvalidOperationException(
                "Context relevance is not initialized.");

    /// <summary>
    /// Creates initialized context relevance.
    /// </summary>
    /// <param name="basisPoints">A value from 1 through 10000.</param>
    /// <returns>The validated relevance.</returns>
    public static ContextRelevance FromBasisPoints(int basisPoints)
    {
        if (!TryFromBasisPoints(basisPoints, out var relevance))
        {
            throw new ArgumentOutOfRangeException(
                nameof(basisPoints),
                basisPoints,
                "Context relevance must be from 1 through 10000 basis points.");
        }

        return relevance;
    }

    /// <summary>
    /// Attempts to create initialized context relevance.
    /// </summary>
    /// <param name="basisPoints">The candidate basis-point value.</param>
    /// <param name="relevance">The validated relevance when successful.</param>
    /// <returns><see langword="true"/> when the value is valid.</returns>
    public static bool TryFromBasisPoints(
        int basisPoints,
        out ContextRelevance relevance)
    {
        if (basisPoints is < 1 or > MaximumValue)
        {
            relevance = default;
            return false;
        }

        relevance = new ContextRelevance(basisPoints);
        return true;
    }

    /// <inheritdoc />
    public int CompareTo(ContextRelevance other) =>
        value.CompareTo(other.value);

    /// <inheritdoc />
    public override string ToString() =>
        IsInitialized ? value.ToString(global::System.Globalization.CultureInfo.InvariantCulture) : string.Empty;
}
