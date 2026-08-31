namespace AI.Sandbox.Engine.Core.Persistence;

/// <summary>
/// Identifies the result of restoring one World Snapshot document.
/// </summary>
public enum SnapshotRestoreStatus
{
    /// <summary>
    /// The envelope and payload were restored successfully.
    /// </summary>
    Restored = 0,

    /// <summary>
    /// The outer snapshot envelope format is unsupported.
    /// </summary>
    UnsupportedFormatVersion = 1,

    /// <summary>
    /// The document belongs to a different stable payload schema.
    /// </summary>
    SchemaMismatch = 2,

    /// <summary>
    /// The codec does not support the stored payload schema version.
    /// </summary>
    UnsupportedSchemaVersion = 3,

    /// <summary>
    /// The stored payload checksum does not match its bytes.
    /// </summary>
    ChecksumMismatch = 4,

    /// <summary>
    /// The codec rejected malformed or semantically invalid payload data.
    /// </summary>
    CodecRejected = 5,
}
