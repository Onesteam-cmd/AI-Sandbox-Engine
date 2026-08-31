namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit adjacent-sequence projection and multi-window checkpoint-range
/// continuity-validation outcomes.
/// </summary>
public enum HostRuntimeRecoveryAdjacentSequenceProjectionStatus
{
    /// <summary>The exact selected adjacent sequence was projected.</summary>
    AdjacentSequenceProjected = 0,

    /// <summary>Continuity between the range and adjacent sequence was validated.</summary>
    MultiWindowCheckpointRangeContinuityValidated = 1,

    /// <summary>The optimistic adjacent-sequence selection revision did not match.</summary>
    StaleSelectionRevision = 2,

    /// <summary>The adjacent-sequence projection tick preceded selection time.</summary>
    AdjacentSequenceProjectionTickRegressed = 3,

    /// <summary>The selected pair-summary authorities did not match the source sequence.</summary>
    SelectionPairSummaryMismatch = 4,

    /// <summary>The selected internal boundary authorities did not match the source sequence.</summary>
    SelectionBoundarySupersessionMismatch = 5,

    /// <summary>The selected checkpoint endpoints did not match the source chain.</summary>
    SelectionCheckpointMismatch = 6,

    /// <summary>The selected incoming or outgoing supersession did not match the source chain.</summary>
    SelectionSupersessionMismatch = 7,

    /// <summary>The optimistic multi-window range-summary revision did not match.</summary>
    StaleRangeSummaryRevision = 8,

    /// <summary>The optimistic adjacent-sequence projection revision did not match.</summary>
    StaleAdjacentSequenceRevision = 9,

    /// <summary>The continuity-validation tick preceded source authority time.</summary>
    ContinuityValidationTickRegressed = 10,

    /// <summary>The adjacent sequence did not originate from the supplied range summary.</summary>
    RangeSummaryMismatch = 11,

    /// <summary>The selected pair interval was not immediately adjacent to the range interval.</summary>
    PairRangeNotAdjacent = 12,

    /// <summary>The selected checkpoint interval was not immediately adjacent to the range.</summary>
    CheckpointRangeNotAdjacent = 13,

    /// <summary>The two authorities did not expose the same connecting supersession.</summary>
    SupersessionBoundaryMismatch = 14,

    /// <summary>The connecting supersession endpoints did not match both authorities.</summary>
    CheckpointBoundaryMismatch = 15,
}
