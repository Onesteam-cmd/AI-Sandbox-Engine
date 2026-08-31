using AI.Sandbox.Engine.Core.Identifiers;

namespace AI.Sandbox.Engine.Core.Spatial;

/// <summary>
/// Stores one immutable deterministic place hierarchy and directed connection
/// graph.
/// </summary>
public sealed class SpatialTopology
{
    private readonly IReadOnlyList<SpatialPlace> places;
    private readonly IReadOnlyList<SpatialConnection> connections;
    private readonly Dictionary<
        Id<SpatialPlaceIdKind>,
        SpatialPlace> placeById;
    private readonly Dictionary<
        (Id<SpatialPlaceIdKind> From, Id<SpatialPlaceIdKind> To),
        SpatialConnection> connectionByEndpoints;
    private readonly Dictionary<
        Id<SpatialPlaceIdKind>,
        IReadOnlyList<SpatialConnection>> outgoingByPlace;

    internal SpatialTopology(
        SpatialPlace[] places,
        SpatialConnection[] connections)
    {
        ArgumentNullException.ThrowIfNull(places);
        ArgumentNullException.ThrowIfNull(connections);

        var placeCopy =
            (SpatialPlace[])places.Clone();
        var connectionCopy =
            (SpatialConnection[])connections.Clone();

        this.places = Array.AsReadOnly(placeCopy);
        this.connections = Array.AsReadOnly(connectionCopy);
        placeById = placeCopy.ToDictionary(
            place => place.PlaceId);
        connectionByEndpoints = connectionCopy.ToDictionary(
            connection => (
                connection.FromPlaceId,
                connection.ToPlaceId));

        var mutableOutgoing = placeCopy.ToDictionary(
            place => place.PlaceId,
            _ => new List<SpatialConnection>());

        foreach (var connection in connectionCopy)
        {
            mutableOutgoing[connection.FromPlaceId]
                .Add(connection);
        }

        outgoingByPlace = mutableOutgoing.ToDictionary(
            pair => pair.Key,
            pair => (IReadOnlyList<SpatialConnection>)
                Array.AsReadOnly(pair.Value.ToArray()));
    }

    /// <summary>
    /// Gets the number of registered places.
    /// </summary>
    public int PlaceCount => places.Count;

    /// <summary>
    /// Gets the number of directed connections.
    /// </summary>
    public int ConnectionCount => connections.Count;

    /// <summary>
    /// Gets places in stable place-ID order.
    /// </summary>
    public IReadOnlyList<SpatialPlace> Places => places;

    /// <summary>
    /// Gets directed connections in stable origin and destination order.
    /// </summary>
    public IReadOnlyList<SpatialConnection> Connections => connections;

    /// <summary>
    /// Determines whether a place exists.
    /// </summary>
    /// <param name="placeId">The place ID.</param>
    /// <returns>
    /// <see langword="true"/> when the place exists.
    /// </returns>
    public bool ContainsPlace(
        Id<SpatialPlaceIdKind> placeId)
    {
        return !placeId.IsEmpty &&
            placeById.ContainsKey(placeId);
    }

    /// <summary>
    /// Attempts to read one place.
    /// </summary>
    /// <param name="placeId">The place ID.</param>
    /// <param name="place">The place when found.</param>
    /// <returns>
    /// <see langword="true"/> when the place exists.
    /// </returns>
    public bool TryGetPlace(
        Id<SpatialPlaceIdKind> placeId,
        out SpatialPlace place)
    {
        return placeById.TryGetValue(
            placeId,
            out place);
    }

    /// <summary>
    /// Gets the immediate parent place ID.
    /// </summary>
    /// <param name="placeId">The known place ID.</param>
    /// <returns>The optional immediate parent.</returns>
    public Id<SpatialPlaceIdKind>? GetParentPlaceId(
        Id<SpatialPlaceIdKind> placeId)
    {
        return GetRequiredPlace(
            placeId,
            nameof(placeId)).ParentPlaceId;
    }

    /// <summary>
    /// Gets all ancestors from immediate parent to root.
    /// </summary>
    /// <param name="placeId">The known descendant place.</param>
    /// <returns>The deterministic ancestor chain.</returns>
    public IReadOnlyList<Id<SpatialPlaceIdKind>> GetAncestors(
        Id<SpatialPlaceIdKind> placeId)
    {
        var current = GetRequiredPlace(
            placeId,
            nameof(placeId));
        var ancestors = new List<
            Id<SpatialPlaceIdKind>>();

        while (current.ParentPlaceId is { } parentId)
        {
            ancestors.Add(parentId);
            current = placeById[parentId];
        }

        return Array.AsReadOnly(
            ancestors.ToArray());
    }

    /// <summary>
    /// Determines whether a place is equal to or transitively contained by
    /// another place.
    /// </summary>
    /// <param name="placeId">The known candidate descendant.</param>
    /// <param name="containerPlaceId">The known candidate container.</param>
    /// <returns>
    /// <see langword="true"/> for equality or transitive containment.
    /// </returns>
    public bool IsContainedWithin(
        Id<SpatialPlaceIdKind> placeId,
        Id<SpatialPlaceIdKind> containerPlaceId)
    {
        var current = GetRequiredPlace(
            placeId,
            nameof(placeId));
        _ = GetRequiredPlace(
            containerPlaceId,
            nameof(containerPlaceId));

        while (true)
        {
            if (current.PlaceId == containerPlaceId)
            {
                return true;
            }

            if (current.ParentPlaceId is not { } parentId)
            {
                return false;
            }

            current = placeById[parentId];
        }
    }

    /// <summary>
    /// Determines whether an initialized position is equal to or contained by a
    /// known place hierarchy.
    /// </summary>
    /// <param name="position">The initialized position.</param>
    /// <param name="containerPlaceId">The known candidate container.</param>
    /// <returns>
    /// <see langword="true"/> when the position's place is within the container.
    /// </returns>
    public bool IsPositionWithin(
        SpatialPosition position,
        Id<SpatialPlaceIdKind> containerPlaceId)
    {
        position.EnsureInitialized();

        return IsContainedWithin(
            position.PlaceId,
            containerPlaceId);
    }

    /// <summary>
    /// Gets all outgoing directed connections in stable destination order.
    /// </summary>
    /// <param name="fromPlaceId">The known origin place.</param>
    /// <returns>The immutable outgoing connection list.</returns>
    public IReadOnlyList<SpatialConnection> GetOutgoingConnections(
        Id<SpatialPlaceIdKind> fromPlaceId)
    {
        _ = GetRequiredPlace(
            fromPlaceId,
            nameof(fromPlaceId));

        return outgoingByPlace[fromPlaceId];
    }

    /// <summary>
    /// Attempts to read one exact directed connection.
    /// </summary>
    /// <param name="fromPlaceId">The origin place.</param>
    /// <param name="toPlaceId">The destination place.</param>
    /// <param name="connection">The connection when found.</param>
    /// <returns>
    /// <see langword="true"/> when the directed connection exists.
    /// </returns>
    public bool TryGetDirectedConnection(
        Id<SpatialPlaceIdKind> fromPlaceId,
        Id<SpatialPlaceIdKind> toPlaceId,
        out SpatialConnection connection)
    {
        return connectionByEndpoints.TryGetValue(
            (
                fromPlaceId,
                toPlaceId),
            out connection);
    }

    private SpatialPlace GetRequiredPlace(
        Id<SpatialPlaceIdKind> placeId,
        string parameterName)
    {
        if (placeId.IsEmpty)
        {
            throw new ArgumentException(
                "A spatial place ID cannot be empty.",
                parameterName);
        }

        if (!placeById.TryGetValue(
            placeId,
            out var place))
        {
            throw new ArgumentException(
                $"Spatial place '{placeId}' is not registered.",
                parameterName);
        }

        return place;
    }
}
