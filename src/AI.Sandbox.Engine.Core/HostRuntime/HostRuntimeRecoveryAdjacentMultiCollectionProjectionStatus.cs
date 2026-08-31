namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit adjacent multi-collection projection and multi-collection-sequence
/// checkpoint-range continuity-validation outcomes.
/// </summary>
public enum HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
{
    /// <summary>The exact selected adjacent multi-collection was projected.</summary>
    AdjacentMultiCollectionProjected = 0,

    /// <summary>Continuity between the range and adjacent multi-collection was validated.</summary>
    MultiCollectionSequenceCheckpointRangeContinuityValidated = 1,

    /// <summary>The optimistic adjacent multi-collection selection revision did not match.</summary>
    StaleSelectionRevision = 2,

    /// <summary>The adjacent multi-collection projection tick preceded selection time.</summary>
    AdjacentMultiCollectionProjectionTickRegressed = 3,

    /// <summary>The selected multi-collection summary authorities did not match the source multi-collection sequence.</summary>
    SelectionMultiCollectionSummaryMismatch = 4,

    /// <summary>The selected internal boundary authorities did not match the source multi-collection sequence.</summary>
    SelectionBoundarySupersessionMismatch = 5,

    /// <summary>The selected checkpoint endpoints did not match the source chain.</summary>
    SelectionCheckpointMismatch = 6,

    /// <summary>The selected incoming or outgoing supersession did not match the source chain.</summary>
    SelectionSupersessionMismatch = 7,

    /// <summary>The optimistic multi-collection-sequence range-summary revision did not match.</summary>
    StaleRangeSummaryRevision = 8,

    /// <summary>The optimistic adjacent multi-collection projection revision did not match.</summary>
    StaleAdjacentMultiCollectionRevision = 9,

    /// <summary>The continuity-validation tick preceded source authority time.</summary>
    ContinuityValidationTickRegressed = 10,

    /// <summary>The adjacent multi-collection did not originate from the supplied range summary.</summary>
    RangeSummaryMismatch = 11,

    /// <summary>The selected multi-collection interval was not immediately adjacent to the range interval.</summary>
    MultiCollectionRangeNotAdjacent = 12,

    /// <summary>The selected checkpoint interval was not immediately adjacent to the range.</summary>
    CheckpointRangeNotAdjacent = 13,

    /// <summary>The two authorities did not expose the same connecting supersession.</summary>
    SupersessionBoundaryMismatch = 14,

    /// <summary>The connecting supersession endpoints did not match both authorities.</summary>
    CheckpointBoundaryMismatch = 15,
}
