namespace AI.Sandbox.Engine.Core.StructuredOutput;

/// <summary>
/// Describes the complete outcome of one structured-output decoding operation.
/// </summary>
public enum StructuredOutputProcessingStatus
{
    /// <summary>
    /// One exact structured payload was decoded and correlated.
    /// </summary>
    Decoded = 1,

    /// <summary>
    /// The decoder explicitly rejected the response.
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// The request selected another decoder.
    /// </summary>
    DecoderMismatch = 3,

    /// <summary>
    /// The request selected another schema.
    /// </summary>
    SchemaMismatch = 4,

    /// <summary>
    /// The request selected another schema version.
    /// </summary>
    SchemaVersionMismatch = 5,

    /// <summary>
    /// The response belongs to another authoritative world.
    /// </summary>
    WorldMismatch = 6,

    /// <summary>
    /// The response authority version is stale.
    /// </summary>
    VersionConflict = 7,

    /// <summary>
    /// The response simulation tick is stale.
    /// </summary>
    SimulationTickMismatch = 8,

    /// <summary>
    /// Authority changed while the decoder was running.
    /// </summary>
    AuthorityChanged = 9,
}
