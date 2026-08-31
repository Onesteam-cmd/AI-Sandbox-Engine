namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit continuous collection-pair summary and cross-collection range-query outcomes.
/// </summary>
public enum HostRuntimeRecoveryContinuousCollectionPairStatus
{
    /// <summary>The continuous collection-pair summary was projected.</summary>
    ContinuousCollectionPairSummaryProjected = 0,

    /// <summary>The bounded inclusive cross-collection checkpoint range was queried.</summary>
    CrossCollectionCheckpointRangeQueried = 1,

    /// <summary>The optimistic continuity-validation revision did not match.</summary>
    StaleContinuityRevision = 2,

    /// <summary>The collection-pair-summary projection tick preceded continuity time.</summary>
    CollectionPairSummaryProjectionTickRegressed = 3,

    /// <summary>The continuity authority did not retain one exact summary and adjacent collection.</summary>
    ContinuitySourceMismatch = 4,

    /// <summary>The source collection pair indexes were not immediately adjacent.</summary>
    CollectionPairNotContinuous = 5,

    /// <summary>The source checkpoint ranges were not immediately adjacent.</summary>
    CollectionCheckpointRangeNotContinuous = 6,

    /// <summary>The collection-pair did not expose one exact connecting supersession.</summary>
    CollectionPairSupersessionMismatch = 7,

    /// <summary>The connecting supersession endpoints did not match the collection-pair boundary.</summary>
    CollectionPairCheckpointMismatch = 8,

    /// <summary>The optimistic collection-pair-summary revision did not match.</summary>
    StaleCollectionPairSummaryRevision = 9,

    /// <summary>The cross-collection query tick preceded collection-pair-summary time.</summary>
    CrossCollectionRangeQueryTickRegressed = 10,

    /// <summary>The requested start checkpoint was absent from the collection-pair.</summary>
    RangeStartNotFound = 11,

    /// <summary>The requested end checkpoint was absent from the collection-pair.</summary>
    RangeEndNotFound = 12,

    /// <summary>The requested inclusive checkpoint order was invalid.</summary>
    RangeOrderInvalid = 13,

    /// <summary>The requested range did not cross the shared collection boundary.</summary>
    RangeDoesNotCrossCollectionBoundary = 14,

    /// <summary>The requested cross-collection range exceeded the bounded limit.</summary>
    RangeTooLarge = 15,

    /// <summary>The materialized range did not retain the connecting supersession.</summary>
    RangeSupersessionMismatch = 16,
}
