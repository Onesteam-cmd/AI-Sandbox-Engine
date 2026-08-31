using AI.Sandbox.Engine.Core.Components;
using AI.Sandbox.Engine.Core.Identifiers;

namespace AI.Sandbox.Engine.Core.Spatial;

/// <summary>
/// Stores one entity's exact local point inside one stable spatial place.
/// </summary>
public readonly record struct SpatialPosition : IComponent
{
    private SpatialPosition(
        Id<SpatialPlaceIdKind> placeId,
        SpatialPoint point)
    {
        PlaceId = placeId;
        Point = point;
    }

    /// <summary>
    /// Gets the stable containing place ID.
    /// </summary>
    public Id<SpatialPlaceIdKind> PlaceId { get; }

    /// <summary>
    /// Gets the exact local point inside the place.
    /// </summary>
    public SpatialPoint Point { get; }

    /// <summary>
    /// Gets a value indicating whether this is the invalid default position.
    /// </summary>
    public bool IsEmpty => PlaceId.IsEmpty;

    /// <summary>
    /// Creates a spatial position inside a non-empty place.
    /// </summary>
    /// <param name="placeId">The stable containing place ID.</param>
    /// <param name="point">The local deterministic point.</param>
    /// <returns>The position.</returns>
    public static SpatialPosition Create(
        Id<SpatialPlaceIdKind> placeId,
        SpatialPoint point)
    {
        if (placeId.IsEmpty)
        {
            throw new ArgumentException(
                "A spatial position requires a non-empty place ID.",
                nameof(placeId));
        }

        return new SpatialPosition(
            placeId,
            point);
    }

    /// <summary>
    /// Attempts to calculate exact local squared distance when both positions
    /// are in the same place.
    /// </summary>
    /// <param name="other">The other position.</param>
    /// <param name="squaredDistance">
    /// The exact squared millimeter distance when successful.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when both positions are initialized and share the
    /// same place; otherwise <see langword="false"/>.
    /// </returns>
    public bool TryGetSquaredDistanceTo(
        SpatialPosition other,
        out System.UInt128 squaredDistance)
    {
        EnsureInitialized();
        other.EnsureInitialized();

        if (PlaceId != other.PlaceId)
        {
            squaredDistance = 0;
            return false;
        }

        squaredDistance =
            Point.GetSquaredDistanceTo(other.Point);
        return true;
    }

    internal void EnsureInitialized()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException(
                "The default spatial position is not initialized.");
        }
    }
}
