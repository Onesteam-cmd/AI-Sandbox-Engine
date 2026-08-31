using AI.Sandbox.Engine.Core.Identifiers;

namespace AI.Sandbox.Engine.Core.Spatial;

/// <summary>
/// Describes one immutable directed topological connection between two places.
/// </summary>
public readonly record struct SpatialConnection
{
    internal SpatialConnection(
        Id<SpatialPlaceIdKind> fromPlaceId,
        Id<SpatialPlaceIdKind> toPlaceId,
        SpatialDistance distance)
    {
        FromPlaceId = fromPlaceId;
        ToPlaceId = toPlaceId;
        Distance = distance;
    }

    /// <summary>
    /// Gets the connection origin place.
    /// </summary>
    public Id<SpatialPlaceIdKind> FromPlaceId { get; }

    /// <summary>
    /// Gets the connection destination place.
    /// </summary>
    public Id<SpatialPlaceIdKind> ToPlaceId { get; }

    /// <summary>
    /// Gets the deterministic connection distance.
    /// </summary>
    public SpatialDistance Distance { get; }

    /// <summary>
    /// Gets a value indicating whether this is an invalid default connection.
    /// </summary>
    public bool IsEmpty =>
        FromPlaceId.IsEmpty ||
        ToPlaceId.IsEmpty;
}
