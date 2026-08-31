namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit continuous multi-sequence collection validation and
/// multi-sequence checkpoint-range query outcomes.
/// </summary>
public enum HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
{
    /// <summary>The bounded continuous multi-sequence collection was validated.</summary>
    ContinuousMultiSequenceCollectionValidated = 0,

    /// <summary>The bounded inclusive multi-sequence checkpoint range was queried.</summary>
    MultiSequenceCheckpointRangeQueried = 1,

    /// <summary>No continuous multi-sequence summaries were supplied.</summary>
    SummaryCollectionEmpty = 2,

    /// <summary>The supplied summary collection exceeded the bounded limit.</summary>
    SummaryCollectionTooLarge = 3,

    /// <summary>The optimistic summary-revision count did not match.</summary>
    SummaryRevisionCountMismatch = 4,

    /// <summary>One optimistic multi-sequence summary revision did not match.</summary>
    StaleMultiSequenceSummaryRevision = 5,

    /// <summary>The collection-validation tick preceded one summary projection tick.</summary>
    CollectionValidationTickRegressed = 6,

    /// <summary>The collection repeated one externally identified summary.</summary>
    DuplicateMultiSequenceSummaryId = 7,

    /// <summary>One summary did not retain the exact shared source projection and chain.</summary>
    SummarySourceMismatch = 8,

    /// <summary>One summary did not retain its exact internal connecting supersession.</summary>
    SummarySupersessionMismatch = 9,

    /// <summary>Consecutive summary pair ranges were not immediately adjacent.</summary>
    SummaryPairRangeNotContinuous = 10,

    /// <summary>Consecutive summary checkpoint ranges were not immediately adjacent.</summary>
    SummaryCheckpointRangeNotContinuous = 11,

    /// <summary>The supersession connecting consecutive summaries did not match.</summary>
    CollectionBoundarySupersessionMismatch = 12,

    /// <summary>The optimistic collection-validation revision did not match.</summary>
    StaleCollectionRevision = 13,

    /// <summary>The multi-sequence query tick preceded collection-validation time.</summary>
    MultiSequenceRangeQueryTickRegressed = 14,

    /// <summary>The requested start checkpoint was absent from the collection.</summary>
    RangeStartNotFound = 15,

    /// <summary>The requested end checkpoint was absent from the collection.</summary>
    RangeEndNotFound = 16,

    /// <summary>The requested inclusive checkpoint order was invalid.</summary>
    RangeOrderInvalid = 17,

    /// <summary>The requested range did not cross a validated sequence boundary.</summary>
    RangeDoesNotCrossSequenceBoundary = 18,

    /// <summary>The requested multi-sequence range exceeded the bounded limit.</summary>
    RangeTooLarge = 19,

    /// <summary>The materialized range did not retain every crossed boundary.</summary>
    RangeSupersessionMismatch = 20,
}
