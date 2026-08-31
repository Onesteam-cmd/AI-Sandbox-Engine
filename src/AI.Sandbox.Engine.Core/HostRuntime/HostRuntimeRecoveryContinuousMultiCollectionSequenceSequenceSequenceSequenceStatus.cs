namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit continuous multi-collection-sequence-sequence-sequence sequence validation and
/// multi-collection-sequence-sequence-sequence-sequence checkpoint-range query outcomes.
/// </summary>
public enum HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
{
    /// <summary>The bounded continuous multi-collection-sequence-sequence-sequence sequence was validated.</summary>
    ContinuousMultiCollectionSequenceSequenceSequenceSequenceValidated = 0,

    /// <summary>The bounded inclusive multi-collection-sequence-sequence-sequence-sequence checkpoint range was queried.</summary>
    MultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQueried = 1,

    /// <summary>No continuous multi-collection-sequence-sequence-sequence summaries were supplied.</summary>
    MultiCollectionSequenceSequenceSequenceSequenceEmpty = 2,

    /// <summary>The supplied summary collection exceeded the bounded limit.</summary>
    MultiCollectionSequenceSequenceSequenceSequenceTooLarge = 3,

    /// <summary>The optimistic summary-revision count did not match.</summary>
    MultiCollectionSequenceSequenceSequenceSummaryRevisionCountMismatch = 4,

    /// <summary>One optimistic multi-collection-sequence-sequence-sequence summary revision did not match.</summary>
    StaleMultiCollectionSequenceSequenceSequenceSummaryRevision = 5,

    /// <summary>The validation tick preceded one summary projection tick.</summary>
    MultiCollectionSequenceSequenceSequenceSequenceValidationTickRegressed = 6,

    /// <summary>The sequence repeated one externally identified summary.</summary>
    DuplicateMultiCollectionSequenceSequenceSequenceSummaryId = 7,

    /// <summary>One summary did not retain the exact shared source authorities.</summary>
    MultiCollectionSequenceSequenceSequenceSummarySourceMismatch = 8,

    /// <summary>One summary did not retain its exact internal connecting supersession.</summary>
    MultiCollectionSequenceSequenceSequenceSummarySupersessionMismatch = 9,

    /// <summary>Consecutive summary ranges were not immediately adjacent.</summary>
    MultiCollectionSequenceSequenceSequenceSummaryRangeNotContinuous = 10,

    /// <summary>Consecutive summary checkpoint ranges were not immediately adjacent.</summary>
    MultiCollectionSequenceSequenceSequenceSummaryCheckpointRangeNotContinuous = 11,

    /// <summary>The supersession connecting consecutive summaries did not match.</summary>
    MultiCollectionSequenceSequenceSequenceSequenceBoundarySupersessionMismatch = 12,

    /// <summary>The optimistic sequence-validation revision did not match.</summary>
    StaleMultiCollectionSequenceSequenceSequenceSequenceRevision = 13,

    /// <summary>The range-query tick preceded sequence-validation time.</summary>
    MultiCollectionSequenceSequenceSequenceSequenceRangeQueryTickRegressed = 14,

    /// <summary>The requested start checkpoint was absent from the sequence.</summary>
    RangeStartNotFound = 15,

    /// <summary>The requested end checkpoint was absent from the sequence.</summary>
    RangeEndNotFound = 16,

    /// <summary>The requested inclusive checkpoint order was invalid.</summary>
    RangeOrderInvalid = 17,

    /// <summary>The requested range did not cross a validated multi-collection-sequence-sequence-sequence boundary.</summary>
    RangeDoesNotCrossMultiCollectionSequenceSequenceSequenceBoundary = 18,

    /// <summary>The requested checkpoint range exceeded the bounded limit.</summary>
    RangeTooLarge = 19,

    /// <summary>The materialized range did not retain every crossed boundary.</summary>
    RangeSupersessionMismatch = 20,
}
