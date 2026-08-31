namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit continuous multi-collection-sequence-sequence-sequence summary and cross-multi-collection-sequence-sequence range-query outcomes.
/// </summary>
public enum HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus
{
    /// <summary>The continuous multi-collection-sequence-sequence-sequence summary was projected.</summary>
    ContinuousMultiCollectionSequenceSequenceSequenceSummaryProjected = 0,

    /// <summary>The bounded inclusive cross-multi-collection-sequence-sequence checkpoint range was queried.</summary>
    CrossMultiCollectionSequenceSequenceCheckpointRangeQueried = 1,

    /// <summary>The optimistic continuity-validation revision did not match.</summary>
    StaleContinuityRevision = 2,

    /// <summary>The multi-collection-sequence-sequence-summary projection tick preceded continuity time.</summary>
    MultiCollectionSequenceSequenceSequenceSummaryProjectionTickRegressed = 3,

    /// <summary>The continuity authority did not retain one exact summary and adjacent multi-collection-sequence-sequence.</summary>
    ContinuitySourceMismatch = 4,

    /// <summary>The source multi-collection-sequence-sequence-summary indexes were not immediately adjacent.</summary>
    MultiCollectionSequenceSequenceRangeNotContinuous = 5,

    /// <summary>The source checkpoint ranges were not immediately adjacent.</summary>
    MultiCollectionSequenceSequenceCheckpointRangeNotContinuous = 6,

    /// <summary>The multi-collection-sequence-sequence did not expose one exact connecting supersession.</summary>
    MultiCollectionSequenceSequenceSupersessionMismatch = 7,

    /// <summary>The connecting supersession endpoints did not match the multi-collection-sequence-sequence boundary.</summary>
    MultiCollectionSequenceSequenceCheckpointMismatch = 8,

    /// <summary>The optimistic multi-collection-sequence-sequence-summary revision did not match.</summary>
    StaleMultiCollectionSequenceSequenceSequenceSummaryRevision = 9,

    /// <summary>The cross-multi-collection-sequence-sequence query tick preceded multi-collection-sequence-summary time.</summary>
    CrossMultiCollectionSequenceSequenceRangeQueryTickRegressed = 10,

    /// <summary>The requested start checkpoint was absent from the multi-collection-sequence-sequence.</summary>
    RangeStartNotFound = 11,

    /// <summary>The requested end checkpoint was absent from the multi-collection-sequence-sequence.</summary>
    RangeEndNotFound = 12,

    /// <summary>The requested inclusive checkpoint order was invalid.</summary>
    RangeOrderInvalid = 13,

    /// <summary>The requested range did not cross the shared multi-collection-sequence-sequence boundary.</summary>
    RangeDoesNotCrossMultiCollectionSequenceSequenceBoundary = 14,

    /// <summary>The requested cross-multi-collection-sequence-sequence range exceeded the bounded limit.</summary>
    RangeTooLarge = 15,

    /// <summary>The materialized range did not retain the connecting supersession.</summary>
    RangeSupersessionMismatch = 16,
}
