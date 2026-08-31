namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit continuous multi-collection sequence validation and
/// multi-collection-sequence checkpoint-range query outcomes.
/// </summary>
public enum HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
{
    /// <summary>The bounded continuous multi-collection sequence was validated.</summary>
    ContinuousMultiCollectionSequenceValidated = 0,

    /// <summary>The bounded inclusive multi-collection-sequence checkpoint range was queried.</summary>
    MultiCollectionSequenceCheckpointRangeQueried = 1,

    /// <summary>No continuous multi-collection summaries were supplied.</summary>
    MultiCollectionSequenceEmpty = 2,

    /// <summary>The supplied summary collection exceeded the bounded limit.</summary>
    MultiCollectionSequenceTooLarge = 3,

    /// <summary>The optimistic summary-revision count did not match.</summary>
    MultiCollectionSummaryRevisionCountMismatch = 4,

    /// <summary>One optimistic multi-collection summary revision did not match.</summary>
    StaleMultiCollectionSummaryRevision = 5,

    /// <summary>The collection-validation tick preceded one summary projection tick.</summary>
    MultiCollectionSequenceValidationTickRegressed = 6,

    /// <summary>The collection repeated one externally identified summary.</summary>
    DuplicateMultiCollectionSummaryId = 7,

    /// <summary>One summary did not retain the exact shared source projection and chain.</summary>
    MultiCollectionSummarySourceMismatch = 8,

    /// <summary>One summary did not retain its exact internal connecting supersession.</summary>
    MultiCollectionSummarySupersessionMismatch = 9,

    /// <summary>Consecutive summary pair ranges were not immediately adjacent.</summary>
    MultiCollectionSummaryPairRangeNotContinuous = 10,

    /// <summary>Consecutive summary checkpoint ranges were not immediately adjacent.</summary>
    MultiCollectionSummaryCheckpointRangeNotContinuous = 11,

    /// <summary>The supersession connecting consecutive summaries did not match.</summary>
    MultiCollectionSequenceBoundarySupersessionMismatch = 12,

    /// <summary>The optimistic collection-validation revision did not match.</summary>
    StaleMultiCollectionSequenceRevision = 13,

    /// <summary>The multi-collection-sequence query tick preceded collection-validation time.</summary>
    MultiCollectionSequenceRangeQueryTickRegressed = 14,

    /// <summary>The requested start checkpoint was absent from the collection.</summary>
    RangeStartNotFound = 15,

    /// <summary>The requested end checkpoint was absent from the collection.</summary>
    RangeEndNotFound = 16,

    /// <summary>The requested inclusive checkpoint order was invalid.</summary>
    RangeOrderInvalid = 17,

    /// <summary>The requested range did not cross a validated sequence boundary.</summary>
    RangeDoesNotCrossCollectionSequenceBoundary = 18,

    /// <summary>The requested multi-collection-sequence range exceeded the bounded limit.</summary>
    RangeTooLarge = 19,

    /// <summary>The materialized range did not retain every crossed boundary.</summary>
    RangeSupersessionMismatch = 20,
}
