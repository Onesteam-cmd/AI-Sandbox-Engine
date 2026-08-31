namespace AI.Sandbox.Engine.Core.Spatial;

/// <summary>
/// Represents a non-negative deterministic spatial distance in integer
/// millimeters.
/// </summary>
public readonly record struct SpatialDistance :
    IComparable<SpatialDistance>
{
    private const ulong MillimetersPerCentimeter = 10;
    private const ulong MillimetersPerMeter = 1_000;
    private const ulong MillimetersPerKilometer = 1_000_000;

    private SpatialDistance(ulong millimeters)
    {
        Millimeters = millimeters;
    }

    /// <summary>
    /// Gets the zero distance.
    /// </summary>
    public static SpatialDistance Zero { get; } = default;

    /// <summary>
    /// Gets the total integer millimeters.
    /// </summary>
    public ulong Millimeters { get; }

    /// <summary>
    /// Gets a value indicating whether the distance is zero.
    /// </summary>
    public bool IsZero => Millimeters == 0;

    /// <summary>
    /// Creates a distance from integer millimeters.
    /// </summary>
    /// <param name="value">The total millimeters.</param>
    /// <returns>The distance.</returns>
    public static SpatialDistance FromMillimeters(ulong value)
    {
        return new SpatialDistance(value);
    }

    /// <summary>
    /// Creates a distance from integer centimeters.
    /// </summary>
    /// <param name="value">The total centimeters.</param>
    /// <returns>The distance.</returns>
    public static SpatialDistance FromCentimeters(ulong value)
    {
        return new SpatialDistance(
            checked(value * MillimetersPerCentimeter));
    }

    /// <summary>
    /// Creates a distance from integer meters.
    /// </summary>
    /// <param name="value">The total meters.</param>
    /// <returns>The distance.</returns>
    public static SpatialDistance FromMeters(ulong value)
    {
        return new SpatialDistance(
            checked(value * MillimetersPerMeter));
    }

    /// <summary>
    /// Creates a distance from integer kilometers.
    /// </summary>
    /// <param name="value">The total kilometers.</param>
    /// <returns>The distance.</returns>
    public static SpatialDistance FromKilometers(ulong value)
    {
        return new SpatialDistance(
            checked(value * MillimetersPerKilometer));
    }

    /// <summary>
    /// Adds two distances with overflow checking.
    /// </summary>
    /// <param name="other">The distance to add.</param>
    /// <returns>The sum.</returns>
    public SpatialDistance Add(SpatialDistance other)
    {
        return new SpatialDistance(
            checked(Millimeters + other.Millimeters));
    }

    /// <summary>
    /// Multiplies the distance by a non-negative integer factor.
    /// </summary>
    /// <param name="factor">The integer factor.</param>
    /// <returns>The multiplied distance.</returns>
    public SpatialDistance Multiply(ulong factor)
    {
        return new SpatialDistance(
            checked(Millimeters * factor));
    }

    /// <inheritdoc />
    public int CompareTo(SpatialDistance other)
    {
        return Millimeters.CompareTo(other.Millimeters);
    }

    /// <summary>
    /// Returns the invariant integer millimeter representation.
    /// </summary>
    /// <returns>The distance followed by <c> mm</c>.</returns>
    public override string ToString()
    {
        return string.Concat(
            Millimeters.ToString(
                System.Globalization.CultureInfo.InvariantCulture),
            " mm");
    }
}
