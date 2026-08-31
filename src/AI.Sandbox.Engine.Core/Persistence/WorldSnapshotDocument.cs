using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Persistence;

/// <summary>
/// Represents a transport-independent encoded World State snapshot envelope.
/// </summary>
public sealed class WorldSnapshotDocument
{
    private WorldSnapshotDocument(
        SnapshotFormatVersion formatVersion,
        PersistenceSchemaId schemaId,
        PersistenceSchemaVersion schemaVersion,
        Id<WorldIdKind> worldId,
        WorldStateVersion worldStateVersion,
        ulong simulationTick,
        SnapshotPayload payload,
        SnapshotChecksum checksum)
    {
        FormatVersion = formatVersion;
        SchemaId = schemaId;
        SchemaVersion = schemaVersion;
        WorldId = worldId;
        WorldStateVersion = worldStateVersion;
        SimulationTick = simulationTick;
        Payload = payload;
        Checksum = checksum;
    }

    /// <summary>
    /// Gets the outer snapshot envelope format.
    /// </summary>
    public SnapshotFormatVersion FormatVersion { get; }

    /// <summary>
    /// Gets the stable payload schema ID.
    /// </summary>
    public PersistenceSchemaId SchemaId { get; }

    /// <summary>
    /// Gets the encoded payload schema version.
    /// </summary>
    public PersistenceSchemaVersion SchemaVersion { get; }

    /// <summary>
    /// Gets the world identity contained in the snapshot.
    /// </summary>
    public Id<WorldIdKind> WorldId { get; }

    /// <summary>
    /// Gets the authoritative World State version contained in the snapshot.
    /// </summary>
    public WorldStateVersion WorldStateVersion { get; }

    /// <summary>
    /// Gets the authoritative logical simulation tick.
    /// </summary>
    public ulong SimulationTick { get; }

    /// <summary>
    /// Gets the immutable encoded state payload.
    /// </summary>
    public SnapshotPayload Payload { get; }

    /// <summary>
    /// Gets the payload checksum stored in the envelope.
    /// </summary>
    public SnapshotChecksum Checksum { get; }

    /// <summary>
    /// Gets a value indicating whether the stored checksum matches the payload.
    /// </summary>
    public bool HasValidChecksum => Checksum.Matches(Payload);

    /// <summary>
    /// Creates a validated snapshot document from parsed envelope fields.
    /// </summary>
    /// <param name="formatVersion">The positive envelope format version.</param>
    /// <param name="schemaId">The non-empty stable schema ID.</param>
    /// <param name="schemaVersion">The positive payload schema version.</param>
    /// <param name="worldId">The non-empty world ID.</param>
    /// <param name="worldStateVersion">The authoritative state version.</param>
    /// <param name="simulationTick">The authoritative logical tick.</param>
    /// <param name="payload">The immutable encoded payload.</param>
    /// <param name="checksum">The parsed payload checksum.</param>
    /// <returns>A transport-independent snapshot document.</returns>
    public static WorldSnapshotDocument Create(
        SnapshotFormatVersion formatVersion,
        PersistenceSchemaId schemaId,
        PersistenceSchemaVersion schemaVersion,
        Id<WorldIdKind> worldId,
        WorldStateVersion worldStateVersion,
        ulong simulationTick,
        SnapshotPayload payload,
        SnapshotChecksum checksum)
    {
        if (formatVersion.IsEmpty)
        {
            throw new ArgumentException(
                "A snapshot format version cannot be empty.",
                nameof(formatVersion));
        }

        if (schemaId.IsEmpty)
        {
            throw new ArgumentException(
                "A persistence schema ID cannot be empty.",
                nameof(schemaId));
        }

        if (schemaVersion.IsEmpty)
        {
            throw new ArgumentException(
                "A persistence schema version cannot be empty.",
                nameof(schemaVersion));
        }

        if (worldId.IsEmpty)
        {
            throw new ArgumentException(
                "A snapshot world ID cannot be empty.",
                nameof(worldId));
        }

        ArgumentNullException.ThrowIfNull(payload);

        if (checksum.IsEmpty)
        {
            throw new ArgumentException(
                "A snapshot checksum cannot be empty.",
                nameof(checksum));
        }

        return new WorldSnapshotDocument(
            formatVersion,
            schemaId,
            schemaVersion,
            worldId,
            worldStateVersion,
            simulationTick,
            payload,
            checksum);
    }
}
