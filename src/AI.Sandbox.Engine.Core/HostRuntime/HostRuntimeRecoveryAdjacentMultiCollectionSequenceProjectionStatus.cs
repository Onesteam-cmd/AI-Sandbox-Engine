namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit adjacent multi-collection-sequence projection and multi-collection-sequence-sequence
/// checkpoint-range continuity-validation outcomes.
/// </summary>
public enum HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
{
    /// <summary>The exact selected adjacent multi-collection-sequence was projected.</summary>
    AdjacentMultiCollectionSequenceProjected = 0,

    /// <summary>Continuity between the range and adjacent multi-collection-sequence was validated.</summary>
    MultiCollectionSequenceSequenceCheckpointRangeContinuityValidated = 1,

    /// <summary>The optimistic adjacent multi-collection-sequence selection revision did not match.</summary>
    StaleSelectionRevision = 2,

    /// <summary>The adjacent multi-collection-sequence projection tick preceded selection time.</summary>
    AdjacentMultiCollectionSequenceProjectionTickRegressed = 3,

    /// <summary>The selected multi-collection-sequence summary authorities did not match the source multi-collection-sequence sequence.</summary>
    SelectionMultiCollectionSequenceSummaryMismatch = 4,

    /// <summary>The selected internal boundary authorities did not match the source multi-collection-sequence sequence.</summary>
    SelectionBoundarySupersessionMismatch = 5,

    /// <summary>The selected checkpoint endpoints did not match the source chain.</summary>
    SelectionCheckpointMismatch = 6,

    /// <summary>The selected incoming or outgoing supersession did not match the source chain.</summary>
    SelectionSupersessionMismatch = 7,

    /// <summary>The optimistic multi-collection-sequence-sequence range-summary revision did not match.</summary>
    StaleRangeSummaryRevision = 8,

    /// <summary>The optimistic adjacent multi-collection-sequence projection revision did not match.</summary>
    StaleAdjacentMultiCollectionSequenceRevision = 9,

    /// <summary>The continuity-validation tick preceded source authority time.</summary>
    ContinuityValidationTickRegressed = 10,

    /// <summary>The adjacent multi-collection-sequence did not originate from the supplied range summary.</summary>
    RangeSummaryMismatch = 11,

    /// <summary>The selected multi-collection-sequence interval was not immediately adjacent to the range interval.</summary>
    MultiCollectionSequenceRangeNotAdjacent = 12,

    /// <summary>The selected checkpoint interval was not immediately adjacent to the range.</summary>
    CheckpointRangeNotAdjacent = 13,

    /// <summary>The two authorities did not expose the same connecting supersession.</summary>
    SupersessionBoundaryMismatch = 14,

    /// <summary>The connecting supersession endpoints did not match both authorities.</summary>
    CheckpointBoundaryMismatch = 15,
}
