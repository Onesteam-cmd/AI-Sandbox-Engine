namespace AI.Sandbox.Engine.Core.Knowledge;

/// <summary>
/// Represents initialized subjective claim confidence in integer basis points.
/// </summary>
/// <remarks>
/// Confidence is owner-specific epistemic metadata, not objective truth
/// probability.
/// </remarks>
public readonly record struct KnowledgeConfidence :
    IComparable<KnowledgeConfidence>
{
    private const ushort MaximumBasisPoints = 10_000;
    private readonly bool isInitialized;
    private readonly ushort basisPoints;

    private KnowledgeConfidence(ushort basisPoints)
    {
        this.basisPoints = basisPoints;
        isInitialized = true;
    }

    /// <summary>
    /// Gets initialized zero confidence.
    /// </summary>
    public static KnowledgeConfidence Zero { get; } = new(0);

    /// <summary>
    /// Gets initialized maximum confidence.
    /// </summary>
    public static KnowledgeConfidence Certain { get; } =
        new(MaximumBasisPoints);

    /// <summary>
    /// Gets the initialized confidence basis points.
    /// </summary>
    public ushort BasisPoints => basisPoints;

    /// <summary>
    /// Gets a value indicating whether this is the invalid default value.
    /// </summary>
    public bool IsEmpty => !isInitialized;

    /// <summary>
    /// Gets a value indicating whether initialized confidence is zero.
    /// </summary>
    public bool IsZero =>
        isInitialized &&
        basisPoints == 0;

    /// <summary>
    /// Creates initialized confidence from zero through ten thousand basis
    /// points.
    /// </summary>
    public static KnowledgeConfidence FromBasisPoints(ushort value)
    {
        if (value > MaximumBasisPoints)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"Knowledge confidence cannot exceed " +
                $"{MaximumBasisPoints} basis points.");
        }

        return new KnowledgeConfidence(value);
    }

    /// <inheritdoc />
    public int CompareTo(KnowledgeConfidence other)
    {
        EnsureInitialized();
        other.EnsureInitialized();

        return basisPoints.CompareTo(other.basisPoints);
    }

    /// <summary>
    /// Returns the invariant basis-point representation.
    /// </summary>
    public override string ToString()
    {
        return IsEmpty
            ? "uninitialized"
            : string.Concat(
                basisPoints.ToString(
                    System.Globalization.CultureInfo.InvariantCulture),
                " bp");
    }

    internal void EnsureUsableForActiveClaim()
    {
        EnsureInitialized();

        if (IsZero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(KnowledgeConfidence),
                this,
                "An active knowledge claim requires confidence above zero.");
        }
    }

    private void EnsureInitialized()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException(
                "The default knowledge confidence is not initialized.");
        }
    }
}
