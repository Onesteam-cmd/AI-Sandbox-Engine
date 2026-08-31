namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit continuous collection-sequence validation and multi-collection
/// checkpoint-range query outcomes.
/// </summary>
public enum HostRuntimeRecoveryContinuousCollectionSequenceStatus
{
    /// <summary>The bounded continuous collection sequence was validated.</summary>
    ContinuousCollectionSequenceValidated = 0,

    /// <summary>The bounded inclusive multi-collection checkpoint range was queried.</summary>
    MultiCollectionCheckpointRangeQueried = 1,

    /// <summary>No continuous collection-pair summaries were supplied.</summary>
    CollectionPairSequenceEmpty = 2,

    /// <summary>The supplied collection-pair sequence exceeded the bounded limit.</summary>
    CollectionPairSequenceTooLarge = 3,

    /// <summary>The optimistic collection-pair revision count did not match.</summary>
    CollectionPairRevisionCountMismatch = 4,

    /// <summary>One optimistic collection-pair summary revision did not match.</summary>
    StaleCollectionPairSummaryRevision = 5,

    /// <summary>The sequence-validation tick preceded one collection-pair summary tick.</summary>
    CollectionSequenceValidationTickRegressed = 6,

    /// <summary>The sequence repeated one externally identified collection-pair summary.</summary>
    DuplicateCollectionPairSummaryId = 7,

    /// <summary>One collection-pair did not retain the exact shared source authorities.</summary>
    CollectionPairSourceMismatch = 8,

    /// <summary>One collection-pair did not retain its exact internal boundary authority.</summary>
    CollectionPairSupersessionMismatch = 9,

    /// <summary>Consecutive collection-pair summary intervals were not immediately adjacent.</summary>
    CollectionSummaryRangeNotContinuous = 10,

    /// <summary>Consecutive collection-pair checkpoint intervals were not immediately adjacent.</summary>
    CollectionCheckpointRangeNotContinuous = 11,

    /// <summary>The supersession connecting consecutive collection-pairs did not match.</summary>
    CollectionSequenceBoundarySupersessionMismatch = 12,

    /// <summary>The optimistic collection-sequence revision did not match.</summary>
    StaleCollectionSequenceRevision = 13,

    /// <summary>The multi-collection query tick preceded sequence-validation time.</summary>
    MultiCollectionRangeQueryTickRegressed = 14,

    /// <summary>The requested start checkpoint was absent from the collection sequence.</summary>
    RangeStartNotFound = 15,

    /// <summary>The requested end checkpoint was absent from the collection sequence.</summary>
    RangeEndNotFound = 16,

    /// <summary>The requested inclusive checkpoint order was invalid.</summary>
    RangeOrderInvalid = 17,

    /// <summary>The requested range did not cross a validated collection boundary.</summary>
    RangeDoesNotCrossCollectionBoundary = 18,

    /// <summary>The requested multi-collection range exceeded the bounded limit.</summary>
    RangeTooLarge = 19,

    /// <summary>The materialized range did not retain every crossed boundary.</summary>
    RangeSupersessionMismatch = 20,
}
