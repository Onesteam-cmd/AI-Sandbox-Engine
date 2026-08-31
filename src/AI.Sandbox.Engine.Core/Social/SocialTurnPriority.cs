namespace AI.Sandbox.Engine.Core.Social;

/// <summary>
/// Represents initialized host-defined social turn priority in basis points.
/// </summary>
public readonly record struct SocialTurnPriority :
    IComparable<SocialTurnPriority>
{
    private const int MaximumBasisPoints = 10000;
    private readonly int basisPoints;

    private SocialTurnPriority(int basisPoints)
    {
        this.basisPoints = basisPoints;
    }

    /// <summary>
    /// Gets a value indicating whether this priority was initialized.
    /// </summary>
    public bool IsInitialized => basisPoints > 0;

    /// <summary>
    /// Gets priority from 1 through 10000 basis points.
    /// </summary>
    public int BasisPoints =>
        IsInitialized
            ? basisPoints
            : throw new InvalidOperationException(
                "Social turn priority is not initialized.");

    /// <summary>
    /// Creates initialized social turn priority.
    /// </summary>
    /// <param name="basisPoints">A value from 1 through 10000.</param>
    /// <returns>The validated priority.</returns>
    public static SocialTurnPriority FromBasisPoints(int basisPoints)
    {
        if (!TryFromBasisPoints(basisPoints, out var priority))
        {
            throw new ArgumentOutOfRangeException(
                nameof(basisPoints),
                basisPoints,
                "Social turn priority must be from 1 through 10000.");
        }

        return priority;
    }

    /// <summary>
    /// Attempts to create initialized social turn priority.
    /// </summary>
    /// <param name="basisPoints">The candidate basis points.</param>
    /// <param name="priority">The validated priority when successful.</param>
    /// <returns><see langword="true"/> when the value is valid.</returns>
    public static bool TryFromBasisPoints(
        int basisPoints,
        out SocialTurnPriority priority)
    {
        if (basisPoints is < 1 or > MaximumBasisPoints)
        {
            priority = default;
            return false;
        }

        priority = new SocialTurnPriority(basisPoints);
        return true;
    }

    /// <inheritdoc />
    public int CompareTo(SocialTurnPriority other) =>
        BasisPoints.CompareTo(other.BasisPoints);

    /// <inheritdoc />
    public override string ToString() =>
        IsInitialized
            ? basisPoints.ToString(
                global::System.Globalization.CultureInfo.InvariantCulture)
            : string.Empty;
}
