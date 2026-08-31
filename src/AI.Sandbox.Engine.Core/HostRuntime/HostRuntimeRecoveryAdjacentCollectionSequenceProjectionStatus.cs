namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit adjacent collection-sequence projection and multi-collection
/// checkpoint-range continuity-validation outcomes.
/// </summary>
public enum HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
{
    /// <summary>The exact selected adjacent collection sequence was projected.</summary>
    AdjacentCollectionSequenceProjected = 0,

    /// <summary>Continuity between the range and adjacent collection sequence was validated.</summary>
    MultiCollectionCheckpointRangeContinuityValidated = 1,

    /// <summary>The optimistic adjacent collection-sequence selection revision did not match.</summary>
    StaleSelectionRevision = 2,

    /// <summary>The adjacent collection-sequence projection tick preceded selection time.</summary>
    AdjacentCollectionSequenceProjectionTickRegressed = 3,

    /// <summary>The selected collection-pair summary authorities did not match the source collection sequence.</summary>
    SelectionCollectionPairSummaryMismatch = 4,

    /// <summary>The selected internal boundary authorities did not match the source collection sequence.</summary>
    SelectionBoundarySupersessionMismatch = 5,

    /// <summary>The selected checkpoint endpoints did not match the source chain.</summary>
    SelectionCheckpointMismatch = 6,

    /// <summary>The selected incoming or outgoing supersession did not match the source chain.</summary>
    SelectionSupersessionMismatch = 7,

    /// <summary>The optimistic multi-collection range-summary revision did not match.</summary>
    StaleRangeSummaryRevision = 8,

    /// <summary>The optimistic adjacent collection-sequence projection revision did not match.</summary>
    StaleAdjacentCollectionSequenceRevision = 9,

    /// <summary>The continuity-validation tick preceded source authority time.</summary>
    ContinuityValidationTickRegressed = 10,

    /// <summary>The adjacent collection sequence did not originate from the supplied range summary.</summary>
    RangeSummaryMismatch = 11,

    /// <summary>The selected collection-pair interval was not immediately adjacent to the range interval.</summary>
    CollectionPairRangeNotAdjacent = 12,

    /// <summary>The selected checkpoint interval was not immediately adjacent to the range.</summary>
    CheckpointRangeNotAdjacent = 13,

    /// <summary>The two authorities did not expose the same connecting supersession.</summary>
    SupersessionBoundaryMismatch = 14,

    /// <summary>The connecting supersession endpoints did not match both authorities.</summary>
    CheckpointBoundaryMismatch = 15,
}
