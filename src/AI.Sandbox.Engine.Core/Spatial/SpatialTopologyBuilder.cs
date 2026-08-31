using AI.Sandbox.Engine.Core.Identifiers;

namespace AI.Sandbox.Engine.Core.Spatial;

/// <summary>
/// Builds one immutable deterministic place hierarchy and directed connection
/// graph.
/// </summary>
public sealed class SpatialTopologyBuilder
{
    private readonly Dictionary<
        Id<SpatialPlaceIdKind>,
        Id<SpatialPlaceIdKind>?> parents = [];
    private readonly Dictionary<
        (Id<SpatialPlaceIdKind> From, Id<SpatialPlaceIdKind> To),
        SpatialConnection> connections = [];
    private bool isBuilt;

    /// <summary>
    /// Adds one stable place and optional immediate parent.
    /// </summary>
    /// <param name="placeId">The non-empty stable place ID.</param>
    /// <param name="parentPlaceId">
    /// The optional non-empty immediate parent ID.
    /// </param>
    /// <returns>This builder.</returns>
    public SpatialTopologyBuilder AddPlace(
        Id<SpatialPlaceIdKind> placeId,
        Id<SpatialPlaceIdKind>? parentPlaceId = null)
    {
        ThrowIfBuilt();
        EnsurePlaceId(
            placeId,
            nameof(placeId));

        if (parentPlaceId is { } parent)
        {
            EnsurePlaceId(
                parent,
                nameof(parentPlaceId));

            if (parent == placeId)
            {
                throw new ArgumentException(
                    "A spatial place cannot contain itself.",
                    nameof(parentPlaceId));
            }
        }

        if (!parents.TryAdd(
            placeId,
            parentPlaceId))
        {
            throw new ArgumentException(
                $"Spatial place '{placeId}' is already registered.",
                nameof(placeId));
        }

        return this;
    }

    /// <summary>
    /// Adds one directed connection. Places may be registered before or after
    /// this call but must exist at build time.
    /// </summary>
    /// <param name="fromPlaceId">The non-empty origin place ID.</param>
    /// <param name="toPlaceId">The non-empty destination place ID.</param>
    /// <param name="distance">The deterministic non-negative distance.</param>
    /// <returns>This builder.</returns>
    public SpatialTopologyBuilder AddDirectedConnection(
        Id<SpatialPlaceIdKind> fromPlaceId,
        Id<SpatialPlaceIdKind> toPlaceId,
        SpatialDistance distance)
    {
        ThrowIfBuilt();
        ValidateConnectionIds(
            fromPlaceId,
            toPlaceId);

        var key = (
            From: fromPlaceId,
            To: toPlaceId);

        if (!connections.TryAdd(
            key,
            new SpatialConnection(
                fromPlaceId,
                toPlaceId,
                distance)))
        {
            throw new ArgumentException(
                $"Directed spatial connection '{fromPlaceId}' -> " +
                $"'{toPlaceId}' is already registered.",
                nameof(toPlaceId));
        }

        return this;
    }

    /// <summary>
    /// Atomically adds two opposite directed connections with equal distance.
    /// </summary>
    /// <param name="firstPlaceId">The first non-empty place ID.</param>
    /// <param name="secondPlaceId">The second non-empty place ID.</param>
    /// <param name="distance">The deterministic distance in both directions.</param>
    /// <returns>This builder.</returns>
    public SpatialTopologyBuilder AddBidirectionalConnection(
        Id<SpatialPlaceIdKind> firstPlaceId,
        Id<SpatialPlaceIdKind> secondPlaceId,
        SpatialDistance distance)
    {
        ThrowIfBuilt();
        ValidateConnectionIds(
            firstPlaceId,
            secondPlaceId);

        var forwardKey = (
            From: firstPlaceId,
            To: secondPlaceId);
        var reverseKey = (
            From: secondPlaceId,
            To: firstPlaceId);

        if (connections.ContainsKey(forwardKey) ||
            connections.ContainsKey(reverseKey))
        {
            throw new ArgumentException(
                "A requested bidirectional spatial connection already " +
                "exists in at least one direction.",
                nameof(secondPlaceId));
        }

        connections.Add(
            forwardKey,
            new SpatialConnection(
                firstPlaceId,
                secondPlaceId,
                distance));
        connections.Add(
            reverseKey,
            new SpatialConnection(
                secondPlaceId,
                firstPlaceId,
                distance));

        return this;
    }

    /// <summary>
    /// Validates and freezes the hierarchy and directed graph.
    /// </summary>
    /// <returns>The immutable deterministic topology.</returns>
    public SpatialTopology Build()
    {
        ThrowIfBuilt();
        ValidateParentGraph();
        ValidateConnectionEndpoints();

        var orderedPlaces = parents
            .Select(pair => new SpatialPlace(
                pair.Key,
                pair.Value))
            .OrderBy(place => place.PlaceId)
            .ToArray();
        var orderedConnections = connections.Values
            .OrderBy(connection => connection.FromPlaceId)
            .ThenBy(connection => connection.ToPlaceId)
            .ToArray();

        isBuilt = true;

        return new SpatialTopology(
            orderedPlaces,
            orderedConnections);
    }

    private void ValidateParentGraph()
    {
        foreach (var pair in parents)
        {
            if (pair.Value is { } parent &&
                !parents.ContainsKey(parent))
            {
                throw new InvalidOperationException(
                    $"Spatial place '{pair.Key}' references unknown parent " +
                    $"'{parent}'.");
            }
        }

        var completed = new HashSet<
            Id<SpatialPlaceIdKind>>();

        foreach (var start in parents.Keys.OrderBy(value => value))
        {
            if (completed.Contains(start))
            {
                continue;
            }

            var path = new HashSet<
                Id<SpatialPlaceIdKind>>();
            var current = start;

            while (true)
            {
                if (completed.Contains(current))
                {
                    break;
                }

                if (!path.Add(current))
                {
                    throw new InvalidOperationException(
                        $"Spatial containment contains a cycle involving " +
                        $"'{current}'.");
                }

                var parent = parents[current];
                if (parent is null)
                {
                    break;
                }

                current = parent.Value;
            }

            foreach (var visited in path)
            {
                _ = completed.Add(visited);
            }
        }
    }

    private void ValidateConnectionEndpoints()
    {
        foreach (var connection in connections.Values)
        {
            if (!parents.ContainsKey(connection.FromPlaceId))
            {
                throw new InvalidOperationException(
                    $"Spatial connection origin " +
                    $"'{connection.FromPlaceId}' is unknown.");
            }

            if (!parents.ContainsKey(connection.ToPlaceId))
            {
                throw new InvalidOperationException(
                    $"Spatial connection destination " +
                    $"'{connection.ToPlaceId}' is unknown.");
            }
        }
    }

    private static void ValidateConnectionIds(
        Id<SpatialPlaceIdKind> fromPlaceId,
        Id<SpatialPlaceIdKind> toPlaceId)
    {
        EnsurePlaceId(
            fromPlaceId,
            nameof(fromPlaceId));
        EnsurePlaceId(
            toPlaceId,
            nameof(toPlaceId));

        if (fromPlaceId == toPlaceId)
        {
            throw new ArgumentException(
                "A spatial connection cannot target its own origin.",
                nameof(toPlaceId));
        }
    }

    private static void EnsurePlaceId(
        Id<SpatialPlaceIdKind> placeId,
        string parameterName)
    {
        if (placeId.IsEmpty)
        {
            throw new ArgumentException(
                "A spatial place ID cannot be empty.",
                parameterName);
        }
    }

    private void ThrowIfBuilt()
    {
        if (isBuilt)
        {
            throw new InvalidOperationException(
                "A spatial topology builder cannot be reused after Build.");
        }
    }
}
