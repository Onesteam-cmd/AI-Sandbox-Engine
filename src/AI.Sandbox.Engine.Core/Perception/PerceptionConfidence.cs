namespace AI.Sandbox.Engine.Core.Perception;

/// <summary>
/// Represents initialized subjective signal confidence in integer basis points.
/// </summary>
/// <remarks>
/// Confidence describes the evaluator's signal reliability metadata. It is not
/// an authoritative truth probability.
/// </remarks>
public readonly record struct PerceptionConfidence :
    IComparable<PerceptionConfidence>
{
    private const ushort MaximumBasisPoints = 10_000;
    private readonly bool isInitialized;
    private readonly ushort basisPoints;

    private PerceptionConfidence(ushort basisPoints)
    {
        this.basisPoints = basisPoints;
        isInitialized = true;
    }

    /// <summary>
    /// Gets initialized zero confidence.
    /// </summary>
    public static PerceptionConfidence Zero { get; } = new(0);

    /// <summary>
    /// Gets initialized maximum confidence.
    /// </summary>
    public static PerceptionConfidence Certain { get; } =
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
    /// <param name="value">The confidence basis points.</param>
    /// <returns>The initialized confidence.</returns>
    public static PerceptionConfidence FromBasisPoints(ushort value)
    {
        if (value > MaximumBasisPoints)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"Perception confidence cannot exceed " +
                $"{MaximumBasisPoints} basis points.");
        }

        return new PerceptionConfidence(value);
    }

    /// <inheritdoc />
    public int CompareTo(PerceptionConfidence other)
    {
        EnsureInitialized();
        other.EnsureInitialized();

        return basisPoints.CompareTo(other.basisPoints);
    }

    /// <summary>
    /// Returns the invariant basis-point representation.
    /// </summary>
    /// <returns>The basis points followed by <c> bp</c>.</returns>
    public override string ToString()
    {
        return IsEmpty
            ? "uninitialized"
            : string.Concat(
                basisPoints.ToString(
                    System.Globalization.CultureInfo.InvariantCulture),
                " bp");
    }

    internal void EnsureUsableForObservation()
    {
        EnsureInitialized();

        if (IsZero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(PerceptionConfidence),
                this,
                "An observed signal requires confidence above zero.");
        }
    }

    private void EnsureInitialized()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException(
                "The default perception confidence is not initialized.");
        }
    }
}
