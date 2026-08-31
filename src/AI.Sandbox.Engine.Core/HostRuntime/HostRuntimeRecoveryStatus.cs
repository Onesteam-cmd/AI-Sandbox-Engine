namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit Host recovery checkpoint and continuation outcomes.
/// </summary>
public enum HostRuntimeRecoveryStatus
{
    /// <summary>A recovery checkpoint authority was created.</summary>
    CheckpointCreated = 0,

    /// <summary>A recovery continuation authority was created.</summary>
    ContinuationCreated = 1,

    /// <summary>The lifecycle and active-work snapshots belong to different runtimes.</summary>
    RuntimeMismatch = 2,

    /// <summary>The lifecycle snapshot does not represent the supplied composition.</summary>
    CompositionMismatch = 3,

    /// <summary>The checkpoint tick precedes its active-work observation.</summary>
    BeforeActiveWorkObservation = 4,

    /// <summary>The World Snapshot envelope format is not currently supported.</summary>
    UnsupportedWorldSnapshotFormat = 5,

    /// <summary>The World Snapshot payload checksum is invalid.</summary>
    WorldSnapshotChecksumMismatch = 6,

    /// <summary>The optimistic recovery checkpoint revision did not match.</summary>
    StaleCheckpointRevision = 7,

    /// <summary>The supplied persistence restore result did not restore a snapshot.</summary>
    RestoreFailed = 8,

    /// <summary>The restored snapshot belongs to another world.</summary>
    RestoredWorldMismatch = 9,

    /// <summary>The restored World State version differs from checkpoint authority.</summary>
    RestoredWorldVersionMismatch = 10,

    /// <summary>The restored logical simulation tick differs from checkpoint authority.</summary>
    RestoredSimulationTickMismatch = 11,

    /// <summary>The continuation tick precedes checkpoint capture.</summary>
    ContinuationTickRegressed = 12,
}
