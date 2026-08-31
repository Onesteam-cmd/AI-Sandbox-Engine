namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit continuous multi-collection-sequence sequence validation and
/// multi-collection-sequence-sequence checkpoint-range query outcomes.
/// </summary>
public enum HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceStatus
{
    /// <summary>The bounded continuous multi-collection-sequence sequence was validated.</summary>
    ContinuousMultiCollectionSequenceSequenceValidated = 0,

    /// <summary>The bounded inclusive multi-collection-sequence-sequence checkpoint range was queried.</summary>
    MultiCollectionSequenceSequenceCheckpointRangeQueried = 1,

    /// <summary>No continuous multi-collection-sequence summaries were supplied.</summary>
    MultiCollectionSequenceSequenceEmpty = 2,

    /// <summary>The supplied summary collection exceeded the bounded limit.</summary>
    MultiCollectionSequenceSequenceTooLarge = 3,

    /// <summary>The optimistic summary-revision count did not match.</summary>
    MultiCollectionSequenceSummaryRevisionCountMismatch = 4,

    /// <summary>One optimistic multi-collection-sequence summary revision did not match.</summary>
    StaleMultiCollectionSequenceSummaryRevision = 5,

    /// <summary>The validation tick preceded one summary projection tick.</summary>
    MultiCollectionSequenceSequenceValidationTickRegressed = 6,

    /// <summary>The sequence repeated one externally identified summary.</summary>
    DuplicateMultiCollectionSequenceSummaryId = 7,

    /// <summary>One summary did not retain the exact shared source authorities.</summary>
    MultiCollectionSequenceSummarySourceMismatch = 8,

    /// <summary>One summary did not retain its exact internal connecting supersession.</summary>
    MultiCollectionSequenceSummarySupersessionMismatch = 9,

    /// <summary>Consecutive summary ranges were not immediately adjacent.</summary>
    MultiCollectionSequenceSummaryRangeNotContinuous = 10,

    /// <summary>Consecutive summary checkpoint ranges were not immediately adjacent.</summary>
    MultiCollectionSequenceSummaryCheckpointRangeNotContinuous = 11,

    /// <summary>The supersession connecting consecutive summaries did not match.</summary>
    MultiCollectionSequenceSequenceBoundarySupersessionMismatch = 12,

    /// <summary>The optimistic sequence-validation revision did not match.</summary>
    StaleMultiCollectionSequenceSequenceRevision = 13,

    /// <summary>The range-query tick preceded sequence-validation time.</summary>
    MultiCollectionSequenceSequenceRangeQueryTickRegressed = 14,

    /// <summary>The requested start checkpoint was absent from the sequence.</summary>
    RangeStartNotFound = 15,

    /// <summary>The requested end checkpoint was absent from the sequence.</summary>
    RangeEndNotFound = 16,

    /// <summary>The requested inclusive checkpoint order was invalid.</summary>
    RangeOrderInvalid = 17,

    /// <summary>The requested range did not cross a validated multi-collection-sequence boundary.</summary>
    RangeDoesNotCrossMultiCollectionSequenceBoundary = 18,

    /// <summary>The requested checkpoint range exceeded the bounded limit.</summary>
    RangeTooLarge = 19,

    /// <summary>The materialized range did not retain every crossed boundary.</summary>
    RangeSupersessionMismatch = 20,
}
