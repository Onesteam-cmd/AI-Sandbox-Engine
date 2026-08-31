namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit Host recovery adjacent-window projection and checkpoint-range
/// continuity-validation outcomes.
/// </summary>
public enum HostRuntimeRecoveryAdjacentWindowProjectionStatus
{
    /// <summary>The exact selected adjacent window was projected.</summary>
    AdjacentWindowProjected = 0,

    /// <summary>Continuity between the range and adjacent window was validated.</summary>
    CheckpointRangeContinuityValidated = 1,

    /// <summary>The optimistic adjacent-window selection revision did not match.</summary>
    StaleSelectionRevision = 2,

    /// <summary>The adjacent-window projection tick preceded selection time.</summary>
    AdjacentWindowProjectionTickRegressed = 3,

    /// <summary>The selection checkpoint authorities did not match its source chain.</summary>
    SelectionCheckpointMismatch = 4,

    /// <summary>The selection boundary supersessions did not match its source chain.</summary>
    SelectionSupersessionMismatch = 5,

    /// <summary>The optimistic range-summary revision did not match.</summary>
    StaleRangeSummaryRevision = 6,

    /// <summary>The optimistic adjacent-window projection revision did not match.</summary>
    StaleAdjacentWindowRevision = 7,

    /// <summary>The continuity-validation tick preceded source authority time.</summary>
    ContinuityValidationTickRegressed = 8,

    /// <summary>The adjacent window did not originate from the supplied range summary.</summary>
    RangeSummaryMismatch = 9,

    /// <summary>The range and projected window indexes were not exactly adjacent.</summary>
    CheckpointRangeNotAdjacent = 10,

    /// <summary>The adjacent checkpoint authorities did not match the connecting edge.</summary>
    CheckpointBoundaryMismatch = 11,

    /// <summary>The two authorities did not expose the same connecting supersession.</summary>
    SupersessionBoundaryMismatch = 12,
}
