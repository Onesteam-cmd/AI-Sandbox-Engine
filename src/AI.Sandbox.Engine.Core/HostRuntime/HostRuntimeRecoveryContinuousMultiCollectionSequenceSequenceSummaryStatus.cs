namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit continuous multi-collection-sequence-sequence summary and cross-multi-collection-sequence range-query outcomes.
/// </summary>
public enum HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus
{
    /// <summary>The continuous multi-collection-sequence-sequence summary was projected.</summary>
    ContinuousMultiCollectionSequenceSequenceSummaryProjected = 0,

    /// <summary>The bounded inclusive cross-multi-collection-sequence checkpoint range was queried.</summary>
    CrossMultiCollectionSequenceCheckpointRangeQueried = 1,

    /// <summary>The optimistic continuity-validation revision did not match.</summary>
    StaleContinuityRevision = 2,

    /// <summary>The multi-collection-sequence-summary projection tick preceded continuity time.</summary>
    MultiCollectionSequenceSequenceSummaryProjectionTickRegressed = 3,

    /// <summary>The continuity authority did not retain one exact summary and adjacent multi-collection-sequence.</summary>
    ContinuitySourceMismatch = 4,

    /// <summary>The source multi-collection-sequence-summary indexes were not immediately adjacent.</summary>
    MultiCollectionSequenceRangeNotContinuous = 5,

    /// <summary>The source checkpoint ranges were not immediately adjacent.</summary>
    MultiCollectionSequenceCheckpointRangeNotContinuous = 6,

    /// <summary>The multi-collection-sequence did not expose one exact connecting supersession.</summary>
    MultiCollectionSequenceSupersessionMismatch = 7,

    /// <summary>The connecting supersession endpoints did not match the multi-collection-sequence boundary.</summary>
    MultiCollectionSequenceCheckpointMismatch = 8,

    /// <summary>The optimistic multi-collection-sequence-sequence-summary revision did not match.</summary>
    StaleMultiCollectionSequenceSequenceSummaryRevision = 9,

    /// <summary>The cross-multi-collection-sequence query tick preceded multi-collection-sequence-summary time.</summary>
    CrossMultiCollectionSequenceRangeQueryTickRegressed = 10,

    /// <summary>The requested start checkpoint was absent from the multi-collection-sequence.</summary>
    RangeStartNotFound = 11,

    /// <summary>The requested end checkpoint was absent from the multi-collection-sequence.</summary>
    RangeEndNotFound = 12,

    /// <summary>The requested inclusive checkpoint order was invalid.</summary>
    RangeOrderInvalid = 13,

    /// <summary>The requested range did not cross the shared multi-collection-sequence boundary.</summary>
    RangeDoesNotCrossMultiCollectionSequenceBoundary = 14,

    /// <summary>The requested cross-multi-collection-sequence range exceeded the bounded limit.</summary>
    RangeTooLarge = 15,

    /// <summary>The materialized range did not retain the connecting supersession.</summary>
    RangeSupersessionMismatch = 16,
}
