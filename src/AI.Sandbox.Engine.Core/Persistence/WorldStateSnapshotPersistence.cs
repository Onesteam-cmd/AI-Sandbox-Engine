using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Persistence;

/// <summary>
/// Converts versioned authoritative snapshots to and from transport-independent
/// encoded documents using one explicit codec.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
/// <remarks>
/// This class performs no file, database, stream, or network I/O. Storage
/// adapters serialize the returned document fields and reconstruct a document
/// before restoration.
/// </remarks>
public sealed class WorldStateSnapshotPersistence<TState>
    where TState : class, IWorldState
{
    private readonly IWorldStateSnapshotCodec<TState> codec;

    /// <summary>
    /// Initializes persistence with one explicit stable payload codec.
    /// </summary>
    /// <param name="codec">The deterministic codec.</param>
    public WorldStateSnapshotPersistence(
        IWorldStateSnapshotCodec<TState> codec)
    {
        ArgumentNullException.ThrowIfNull(codec);

        if (codec.SchemaId.IsEmpty)
        {
            throw new ArgumentException(
                "A persistence codec must expose a non-empty stable schema ID.",
                nameof(codec));
        }

        if (codec.CurrentSchemaVersion.IsEmpty)
        {
            throw new ArgumentException(
                "A persistence codec must expose a positive current version.",
                nameof(codec));
        }

        this.codec = codec;
    }

    /// <summary>
    /// Captures one authoritative snapshot as an encoded document.
    /// </summary>
    /// <param name="snapshot">The immutable snapshot to encode.</param>
    /// <returns>A checksum-protected snapshot document.</returns>
    public WorldSnapshotDocument Capture(
        WorldStateSnapshot<TState> snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var payload = codec.Encode(snapshot.State);
        if (payload is null)
        {
            throw new InvalidOperationException(
                "A persistence codec returned a null encoded payload.");
        }

        var checksum = SnapshotChecksum.Compute(payload);

        return WorldSnapshotDocument.Create(
            SnapshotFormatVersion.Current,
            codec.SchemaId,
            codec.CurrentSchemaVersion,
            snapshot.WorldId,
            snapshot.Version,
            snapshot.SimulationTick,
            payload,
            checksum);
    }

    /// <summary>
    /// Validates and restores one encoded snapshot document.
    /// </summary>
    /// <param name="document">The parsed transport-independent document.</param>
    /// <returns>An explicit restore result.</returns>
    public SnapshotRestoreResult<TState> Restore(
        WorldSnapshotDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (document.FormatVersion != SnapshotFormatVersion.Current)
        {
            return SnapshotRestoreResult<TState>.Failed(
                SnapshotRestoreStatus.UnsupportedFormatVersion,
                $"Snapshot format '{document.FormatVersion}' is unsupported.");
        }

        if (document.SchemaId != codec.SchemaId)
        {
            return SnapshotRestoreResult<TState>.Failed(
                SnapshotRestoreStatus.SchemaMismatch,
                $"Snapshot schema '{document.SchemaId}' does not match " +
                $"codec schema '{codec.SchemaId}'.");
        }

        if (!codec.CanDecode(document.SchemaVersion))
        {
            return SnapshotRestoreResult<TState>.Failed(
                SnapshotRestoreStatus.UnsupportedSchemaVersion,
                $"Schema version '{document.SchemaVersion}' is unsupported.");
        }

        if (!document.HasValidChecksum)
        {
            return SnapshotRestoreResult<TState>.Failed(
                SnapshotRestoreStatus.ChecksumMismatch,
                "Snapshot payload checksum verification failed.");
        }

        var decision = codec.Decode(
            document.SchemaVersion,
            document.Payload);
        if (decision is null)
        {
            throw new InvalidOperationException(
                "A persistence codec returned a null decode decision.");
        }

        var state = decision.State;
        var rejectionReason = decision.RejectionReason;

        if (!decision.IsDecoded)
        {
            if (state is not null ||
                string.IsNullOrWhiteSpace(rejectionReason))
            {
                throw new InvalidOperationException(
                    "A rejected decode decision is internally inconsistent.");
            }

            return SnapshotRestoreResult<TState>.Failed(
                SnapshotRestoreStatus.CodecRejected,
                rejectionReason);
        }

        if (state is null || rejectionReason is not null)
        {
            throw new InvalidOperationException(
                "An accepted decode decision is internally inconsistent.");
        }

        var snapshot = new WorldStateSnapshot<TState>(
            document.WorldId,
            document.WorldStateVersion,
            document.SimulationTick,
            state);

        return SnapshotRestoreResult<TState>.Restored(snapshot);
    }
}
