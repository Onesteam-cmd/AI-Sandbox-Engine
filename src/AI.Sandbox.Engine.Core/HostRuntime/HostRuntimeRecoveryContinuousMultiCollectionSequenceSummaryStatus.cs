namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit continuous multi-collection-sequence summary and cross-multi-collection range-query outcomes.
/// </summary>
public enum HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus
{
    /// <summary>The continuous multi-collection-sequence summary was projected.</summary>
    ContinuousMultiCollectionSequenceSummaryProjected = 0,

    /// <summary>The bounded inclusive cross-multi-collection checkpoint range was queried.</summary>
    CrossMultiCollectionCheckpointRangeQueried = 1,

    /// <summary>The optimistic continuity-validation revision did not match.</summary>
    StaleContinuityRevision = 2,

    /// <summary>The multi-collection-summary projection tick preceded continuity time.</summary>
    MultiCollectionSequenceSummaryProjectionTickRegressed = 3,

    /// <summary>The continuity authority did not retain one exact summary and adjacent multi-collection.</summary>
    ContinuitySourceMismatch = 4,

    /// <summary>The source multi-collection-summary indexes were not immediately adjacent.</summary>
    MultiCollectionRangeNotContinuous = 5,

    /// <summary>The source checkpoint ranges were not immediately adjacent.</summary>
    MultiCollectionCheckpointRangeNotContinuous = 6,

    /// <summary>The multi-collection did not expose one exact connecting supersession.</summary>
    MultiCollectionSupersessionMismatch = 7,

    /// <summary>The connecting supersession endpoints did not match the multi-collection boundary.</summary>
    MultiCollectionCheckpointMismatch = 8,

    /// <summary>The optimistic multi-collection-sequence-summary revision did not match.</summary>
    StaleMultiCollectionSequenceSummaryRevision = 9,

    /// <summary>The cross-multi-collection query tick preceded multi-collection-sequence-summary time.</summary>
    CrossMultiCollectionRangeQueryTickRegressed = 10,

    /// <summary>The requested start checkpoint was absent from the multi-collection.</summary>
    RangeStartNotFound = 11,

    /// <summary>The requested end checkpoint was absent from the multi-collection.</summary>
    RangeEndNotFound = 12,

    /// <summary>The requested inclusive checkpoint order was invalid.</summary>
    RangeOrderInvalid = 13,

    /// <summary>The requested range did not cross the shared multi-collection boundary.</summary>
    RangeDoesNotCrossMultiCollectionBoundary = 14,

    /// <summary>The requested cross-multi-collection range exceeded the bounded limit.</summary>
    RangeTooLarge = 15,

    /// <summary>The materialized range did not retain the connecting supersession.</summary>
    RangeSupersessionMismatch = 16,
}
