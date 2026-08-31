namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit adjacent-collection projection and multi-sequence
/// checkpoint-range continuity-validation outcomes.
/// </summary>
public enum HostRuntimeRecoveryAdjacentCollectionProjectionStatus
{
    /// <summary>The exact selected adjacent collection was projected.</summary>
    AdjacentCollectionProjected = 0,

    /// <summary>Continuity between the range and adjacent collection was validated.</summary>
    MultiSequenceCheckpointRangeContinuityValidated = 1,

    /// <summary>The optimistic adjacent-collection selection revision did not match.</summary>
    StaleSelectionRevision = 2,

    /// <summary>The adjacent-collection projection tick preceded selection time.</summary>
    AdjacentCollectionProjectionTickRegressed = 3,

    /// <summary>The selected multi-sequence summary authorities did not match the source collection.</summary>
    SelectionSummaryMismatch = 4,

    /// <summary>The selected internal boundary authorities did not match the source collection.</summary>
    SelectionBoundarySupersessionMismatch = 5,

    /// <summary>The selected checkpoint endpoints did not match the source chain.</summary>
    SelectionCheckpointMismatch = 6,

    /// <summary>The selected incoming or outgoing supersession did not match the source chain.</summary>
    SelectionSupersessionMismatch = 7,

    /// <summary>The optimistic multi-sequence range-summary revision did not match.</summary>
    StaleRangeSummaryRevision = 8,

    /// <summary>The optimistic adjacent-collection projection revision did not match.</summary>
    StaleAdjacentCollectionRevision = 9,

    /// <summary>The continuity-validation tick preceded source authority time.</summary>
    ContinuityValidationTickRegressed = 10,

    /// <summary>The adjacent collection did not originate from the supplied range summary.</summary>
    RangeSummaryMismatch = 11,

    /// <summary>The selected summary interval was not immediately adjacent to the range interval.</summary>
    SummaryRangeNotAdjacent = 12,

    /// <summary>The selected checkpoint interval was not immediately adjacent to the range.</summary>
    CheckpointRangeNotAdjacent = 13,

    /// <summary>The two authorities did not expose the same connecting supersession.</summary>
    SupersessionBoundaryMismatch = 14,

    /// <summary>The connecting supersession endpoints did not match both authorities.</summary>
    CheckpointBoundaryMismatch = 15,
}
