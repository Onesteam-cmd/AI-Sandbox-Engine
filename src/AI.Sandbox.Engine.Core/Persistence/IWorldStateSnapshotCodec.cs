using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Persistence;

/// <summary>
/// Encodes and decodes one stable versioned World State payload schema.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
/// <remarks>
/// Codecs must be deterministic and side-effect-free. They do not read or write
/// files, databases, streams, networks, clocks, random generators, or World
/// State Manager. Stable schema IDs must not depend on CLR type names.
/// </remarks>
public interface IWorldStateSnapshotCodec<TState>
    where TState : class, IWorldState
{
    /// <summary>
    /// Gets the stable transport-independent payload schema ID.
    /// </summary>
    public PersistenceSchemaId SchemaId { get; }

    /// <summary>
    /// Gets the schema version emitted by <see cref="Encode"/>.
    /// </summary>
    public PersistenceSchemaVersion CurrentSchemaVersion { get; }

    /// <summary>
    /// Determines whether this codec can decode one stored schema version.
    /// </summary>
    /// <param name="version">The stored schema version.</param>
    /// <returns>
    /// <see langword="true"/> when <see cref="Decode"/> supports the version.
    /// </returns>
    public bool CanDecode(PersistenceSchemaVersion version);

    /// <summary>
    /// Deterministically encodes one immutable state root.
    /// </summary>
    /// <param name="state">The immutable state to encode.</param>
    /// <returns>The immutable encoded payload.</returns>
    public SnapshotPayload Encode(TState state);

    /// <summary>
    /// Decodes and validates one supported encoded state payload.
    /// </summary>
    /// <param name="version">The stored schema version.</param>
    /// <param name="payload">The checksum-verified payload.</param>
    /// <returns>An accepted state or an explicit codec rejection.</returns>
    public WorldStateDecodeDecision<TState> Decode(
        PersistenceSchemaVersion version,
        SnapshotPayload payload);
}
