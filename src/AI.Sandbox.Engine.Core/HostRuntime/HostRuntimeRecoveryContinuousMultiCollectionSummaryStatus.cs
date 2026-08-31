namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit continuous multi-collection summary and cross-collection-sequence range-query outcomes.
/// </summary>
public enum HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus
{
    /// <summary>The continuous multi-collection summary was projected.</summary>
    ContinuousMultiCollectionSummaryProjected = 0,

    /// <summary>The bounded inclusive cross-collection-sequence checkpoint range was queried.</summary>
    CrossCollectionSequenceCheckpointRangeQueried = 1,

    /// <summary>The optimistic continuity-validation revision did not match.</summary>
    StaleContinuityRevision = 2,

    /// <summary>The multi-collection-summary projection tick preceded continuity time.</summary>
    MultiCollectionSummaryProjectionTickRegressed = 3,

    /// <summary>The continuity authority did not retain one exact summary and adjacent sequence.</summary>
    ContinuitySourceMismatch = 4,

    /// <summary>The source collection-pair indexes were not immediately adjacent.</summary>
    CollectionPairRangeNotContinuous = 5,

    /// <summary>The source checkpoint ranges were not immediately adjacent.</summary>
    CollectionSequenceCheckpointRangeNotContinuous = 6,

    /// <summary>The multi-collection did not expose one exact connecting supersession.</summary>
    CollectionSequenceSupersessionMismatch = 7,

    /// <summary>The connecting supersession endpoints did not match the multi-collection boundary.</summary>
    CollectionSequenceCheckpointMismatch = 8,

    /// <summary>The optimistic multi-collection-summary revision did not match.</summary>
    StaleMultiCollectionSummaryRevision = 9,

    /// <summary>The cross-collection-sequence query tick preceded multi-collection-summary time.</summary>
    CrossCollectionSequenceRangeQueryTickRegressed = 10,

    /// <summary>The requested start checkpoint was absent from the multi-collection.</summary>
    RangeStartNotFound = 11,

    /// <summary>The requested end checkpoint was absent from the multi-collection.</summary>
    RangeEndNotFound = 12,

    /// <summary>The requested inclusive checkpoint order was invalid.</summary>
    RangeOrderInvalid = 13,

    /// <summary>The requested range did not cross the shared sequence boundary.</summary>
    RangeDoesNotCrossCollectionSequenceBoundary = 14,

    /// <summary>The requested cross-collection-sequence range exceeded the bounded limit.</summary>
    RangeTooLarge = 15,

    /// <summary>The materialized range did not retain the connecting supersession.</summary>
    RangeSupersessionMismatch = 16,
}
