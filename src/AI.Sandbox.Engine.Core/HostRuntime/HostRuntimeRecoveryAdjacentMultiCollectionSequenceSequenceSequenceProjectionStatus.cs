namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit adjacent multi-collection-sequence-sequence-sequence projection and multi-collection-sequence-sequence-sequence-sequence
/// checkpoint-range continuity-validation outcomes.
/// </summary>
public enum HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
{
    /// <summary>The exact selected adjacent multi-collection-sequence-sequence-sequence was projected.</summary>
    AdjacentMultiCollectionSequenceSequenceSequenceProjected = 0,

    /// <summary>Continuity between the range and adjacent multi-collection-sequence-sequence-sequence was validated.</summary>
    MultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidated = 1,

    /// <summary>The optimistic adjacent multi-collection-sequence-sequence-sequence selection revision did not match.</summary>
    StaleSelectionRevision = 2,

    /// <summary>The adjacent multi-collection-sequence-sequence-sequence projection tick preceded selection time.</summary>
    AdjacentMultiCollectionSequenceSequenceSequenceProjectionTickRegressed = 3,

    /// <summary>The selected multi-collection-sequence-sequence-sequence summary authorities did not match the source multi-collection-sequence-sequence-sequence sequence.</summary>
    SelectionMultiCollectionSequenceSequenceSequenceSummaryMismatch = 4,

    /// <summary>The selected internal boundary authorities did not match the source multi-collection-sequence-sequence-sequence sequence.</summary>
    SelectionBoundarySupersessionMismatch = 5,

    /// <summary>The selected checkpoint endpoints did not match the source chain.</summary>
    SelectionCheckpointMismatch = 6,

    /// <summary>The selected incoming or outgoing supersession did not match the source chain.</summary>
    SelectionSupersessionMismatch = 7,

    /// <summary>The optimistic multi-collection-sequence-sequence-sequence-sequence range-summary revision did not match.</summary>
    StaleRangeSummaryRevision = 8,

    /// <summary>The optimistic adjacent multi-collection-sequence-sequence-sequence projection revision did not match.</summary>
    StaleAdjacentMultiCollectionSequenceSequenceSequenceRevision = 9,

    /// <summary>The continuity-validation tick preceded source authority time.</summary>
    ContinuityValidationTickRegressed = 10,

    /// <summary>The adjacent multi-collection-sequence-sequence-sequence did not originate from the supplied range summary.</summary>
    RangeSummaryMismatch = 11,

    /// <summary>The selected multi-collection-sequence-sequence-sequence interval was not immediately adjacent to the range interval.</summary>
    MultiCollectionSequenceSequenceSequenceRangeNotAdjacent = 12,

    /// <summary>The selected checkpoint interval was not immediately adjacent to the range.</summary>
    CheckpointRangeNotAdjacent = 13,

    /// <summary>The two authorities did not expose the same connecting supersession.</summary>
    SupersessionBoundaryMismatch = 14,

    /// <summary>The connecting supersession endpoints did not match both authorities.</summary>
    CheckpointBoundaryMismatch = 15,
}
