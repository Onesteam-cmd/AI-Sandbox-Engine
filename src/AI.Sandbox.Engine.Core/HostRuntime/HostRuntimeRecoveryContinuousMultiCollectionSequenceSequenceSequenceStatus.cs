namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit continuous multi-collection-sequence-sequence sequence validation and
/// multi-collection-sequence-sequence-sequence checkpoint-range query outcomes.
/// </summary>
public enum HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceStatus
{
    /// <summary>The bounded continuous multi-collection-sequence-sequence sequence was validated.</summary>
    ContinuousMultiCollectionSequenceSequenceSequenceValidated = 0,

    /// <summary>The bounded inclusive multi-collection-sequence-sequence-sequence checkpoint range was queried.</summary>
    MultiCollectionSequenceSequenceSequenceCheckpointRangeQueried = 1,

    /// <summary>No continuous multi-collection-sequence-sequence summaries were supplied.</summary>
    MultiCollectionSequenceSequenceSequenceEmpty = 2,

    /// <summary>The supplied summary collection exceeded the bounded limit.</summary>
    MultiCollectionSequenceSequenceSequenceTooLarge = 3,

    /// <summary>The optimistic summary-revision count did not match.</summary>
    MultiCollectionSequenceSequenceSummaryRevisionCountMismatch = 4,

    /// <summary>One optimistic multi-collection-sequence-sequence summary revision did not match.</summary>
    StaleMultiCollectionSequenceSequenceSummaryRevision = 5,

    /// <summary>The validation tick preceded one summary projection tick.</summary>
    MultiCollectionSequenceSequenceSequenceValidationTickRegressed = 6,

    /// <summary>The sequence repeated one externally identified summary.</summary>
    DuplicateMultiCollectionSequenceSequenceSummaryId = 7,

    /// <summary>One summary did not retain the exact shared source authorities.</summary>
    MultiCollectionSequenceSequenceSummarySourceMismatch = 8,

    /// <summary>One summary did not retain its exact internal connecting supersession.</summary>
    MultiCollectionSequenceSequenceSummarySupersessionMismatch = 9,

    /// <summary>Consecutive summary ranges were not immediately adjacent.</summary>
    MultiCollectionSequenceSequenceSummaryRangeNotContinuous = 10,

    /// <summary>Consecutive summary checkpoint ranges were not immediately adjacent.</summary>
    MultiCollectionSequenceSequenceSummaryCheckpointRangeNotContinuous = 11,

    /// <summary>The supersession connecting consecutive summaries did not match.</summary>
    MultiCollectionSequenceSequenceSequenceBoundarySupersessionMismatch = 12,

    /// <summary>The optimistic sequence-validation revision did not match.</summary>
    StaleMultiCollectionSequenceSequenceSequenceRevision = 13,

    /// <summary>The range-query tick preceded sequence-validation time.</summary>
    MultiCollectionSequenceSequenceSequenceRangeQueryTickRegressed = 14,

    /// <summary>The requested start checkpoint was absent from the sequence.</summary>
    RangeStartNotFound = 15,

    /// <summary>The requested end checkpoint was absent from the sequence.</summary>
    RangeEndNotFound = 16,

    /// <summary>The requested inclusive checkpoint order was invalid.</summary>
    RangeOrderInvalid = 17,

    /// <summary>The requested range did not cross a validated multi-collection-sequence-sequence boundary.</summary>
    RangeDoesNotCrossMultiCollectionSequenceSequenceBoundary = 18,

    /// <summary>The requested checkpoint range exceeded the bounded limit.</summary>
    RangeTooLarge = 19,

    /// <summary>The materialized range did not retain every crossed boundary.</summary>
    RangeSupersessionMismatch = 20,
}
