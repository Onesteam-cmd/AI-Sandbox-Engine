namespace AI.Sandbox.Engine.Core.Conversation;

/// <summary>
/// Represents initialized address-resolution confidence in basis points.
/// </summary>
public readonly record struct AddressResolutionConfidence :
    IComparable<AddressResolutionConfidence>
{
    private const int MaximumBasisPoints = 10000;
    private readonly int basisPoints;

    private AddressResolutionConfidence(int basisPoints)
    {
        this.basisPoints = basisPoints;
    }

    /// <summary>
    /// Gets a value indicating whether this confidence was initialized.
    /// </summary>
    public bool IsInitialized => basisPoints > 0;

    /// <summary>
    /// Gets confidence from 1 through 10000 basis points.
    /// </summary>
    public int BasisPoints =>
        IsInitialized
            ? basisPoints
            : throw new InvalidOperationException(
                "Address-resolution confidence is not initialized.");

    /// <summary>
    /// Creates initialized address-resolution confidence.
    /// </summary>
    /// <param name="basisPoints">A value from 1 through 10000.</param>
    /// <returns>The validated confidence.</returns>
    public static AddressResolutionConfidence FromBasisPoints(int basisPoints)
    {
        if (!TryFromBasisPoints(basisPoints, out var confidence))
        {
            throw new ArgumentOutOfRangeException(
                nameof(basisPoints),
                basisPoints,
                "Address-resolution confidence must be from 1 through 10000.");
        }

        return confidence;
    }

    /// <summary>
    /// Attempts to create initialized address-resolution confidence.
    /// </summary>
    /// <param name="basisPoints">The candidate basis points.</param>
    /// <param name="confidence">The validated confidence when successful.</param>
    /// <returns><see langword="true"/> when the value is valid.</returns>
    public static bool TryFromBasisPoints(
        int basisPoints,
        out AddressResolutionConfidence confidence)
    {
        if (basisPoints is < 1 or > MaximumBasisPoints)
        {
            confidence = default;
            return false;
        }

        confidence = new AddressResolutionConfidence(basisPoints);
        return true;
    }

    /// <inheritdoc />
    public int CompareTo(AddressResolutionConfidence other) =>
        BasisPoints.CompareTo(other.BasisPoints);

    /// <inheritdoc />
    public override string ToString() =>
        IsInitialized ? basisPoints.ToString(
            global::System.Globalization.CultureInfo.InvariantCulture) :
            string.Empty;
}
