using AI.Sandbox.Engine.Core.Identifiers;

namespace AI.Sandbox.Engine.Core.Spatial;

/// <summary>
/// Describes one stable place and its optional immediate containing place.
/// </summary>
public readonly record struct SpatialPlace
{
    internal SpatialPlace(
        Id<SpatialPlaceIdKind> placeId,
        Id<SpatialPlaceIdKind>? parentPlaceId)
    {
        PlaceId = placeId;
        ParentPlaceId = parentPlaceId;
    }

    /// <summary>
    /// Gets the stable place ID.
    /// </summary>
    public Id<SpatialPlaceIdKind> PlaceId { get; }

    /// <summary>
    /// Gets the immediate parent place, or <see langword="null"/> for a root.
    /// </summary>
    public Id<SpatialPlaceIdKind>? ParentPlaceId { get; }

    /// <summary>
    /// Gets a value indicating whether this is an invalid default value.
    /// </summary>
    public bool IsEmpty => PlaceId.IsEmpty;

    /// <summary>
    /// Gets a value indicating whether the place has no parent.
    /// </summary>
    public bool IsRoot => ParentPlaceId is null;
}
