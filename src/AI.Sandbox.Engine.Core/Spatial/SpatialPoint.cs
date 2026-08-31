namespace AI.Sandbox.Engine.Core.Spatial;

/// <summary>
/// Represents one deterministic local three-dimensional point in integer
/// millimeters relative to a spatial place.
/// </summary>
public readonly record struct SpatialPoint
{
    private const long MaximumAbsoluteCoordinateMillimeters =
        1_000_000_000_000;

    private SpatialPoint(
        long xMillimeters,
        long yMillimeters,
        long zMillimeters)
    {
        XMillimeters = xMillimeters;
        YMillimeters = yMillimeters;
        ZMillimeters = zMillimeters;
    }

    /// <summary>
    /// Gets the local origin.
    /// </summary>
    public static SpatialPoint Origin { get; } = default;

    /// <summary>
    /// Gets the local X coordinate in integer millimeters.
    /// </summary>
    public long XMillimeters { get; }

    /// <summary>
    /// Gets the local Y coordinate in integer millimeters.
    /// </summary>
    public long YMillimeters { get; }

    /// <summary>
    /// Gets the local Z coordinate in integer millimeters.
    /// </summary>
    public long ZMillimeters { get; }

    /// <summary>
    /// Creates a bounded deterministic local point.
    /// </summary>
    /// <param name="xMillimeters">The local X coordinate.</param>
    /// <param name="yMillimeters">The local Y coordinate.</param>
    /// <param name="zMillimeters">The local Z coordinate.</param>
    /// <returns>The point.</returns>
    public static SpatialPoint Create(
        long xMillimeters,
        long yMillimeters,
        long zMillimeters)
    {
        EnsureCoordinateInRange(
            xMillimeters,
            nameof(xMillimeters));
        EnsureCoordinateInRange(
            yMillimeters,
            nameof(yMillimeters));
        EnsureCoordinateInRange(
            zMillimeters,
            nameof(zMillimeters));

        return new SpatialPoint(
            xMillimeters,
            yMillimeters,
            zMillimeters);
    }

    /// <summary>
    /// Calculates the exact squared Euclidean distance in square millimeters.
    /// </summary>
    /// <param name="other">The other local point.</param>
    /// <returns>The exact squared distance.</returns>
    public System.UInt128 GetSquaredDistanceTo(
        SpatialPoint other)
    {
        var x = SquareDifference(
            XMillimeters,
            other.XMillimeters);
        var y = SquareDifference(
            YMillimeters,
            other.YMillimeters);
        var z = SquareDifference(
            ZMillimeters,
            other.ZMillimeters);

        return checked(x + y + z);
    }

    /// <summary>
    /// Determines whether another point is within an inclusive radius.
    /// </summary>
    /// <param name="other">The other local point.</param>
    /// <param name="radius">The inclusive non-negative radius.</param>
    /// <returns>
    /// <see langword="true"/> when the exact squared distance does not exceed
    /// the squared radius.
    /// </returns>
    public bool IsWithin(
        SpatialPoint other,
        SpatialDistance radius)
    {
        var radiusSquared =
            (System.UInt128)radius.Millimeters *
            radius.Millimeters;

        return GetSquaredDistanceTo(other) <= radiusSquared;
    }

    /// <summary>
    /// Returns the invariant local millimeter representation.
    /// </summary>
    /// <returns>The three coordinates in deterministic order.</returns>
    public override string ToString()
    {
        return string.Concat(
            XMillimeters.ToString(
                System.Globalization.CultureInfo.InvariantCulture),
            ",",
            YMillimeters.ToString(
                System.Globalization.CultureInfo.InvariantCulture),
            ",",
            ZMillimeters.ToString(
                System.Globalization.CultureInfo.InvariantCulture),
            " mm");
    }

    private static System.UInt128 SquareDifference(
        long first,
        long second)
    {
        var difference =
            (System.Int128)first -
            second;

        return (System.UInt128)(
            difference *
            difference);
    }

    private static void EnsureCoordinateInRange(
        long value,
        string parameterName)
    {
        if (value < -MaximumAbsoluteCoordinateMillimeters ||
            value > MaximumAbsoluteCoordinateMillimeters)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                value,
                $"Local spatial coordinates must be between " +
                $"{-MaximumAbsoluteCoordinateMillimeters} and " +
                $"{MaximumAbsoluteCoordinateMillimeters} millimeters.");
        }
    }
}
