namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit continuous multi-collection-sequence-sequence-sequence-sequence summary and cross-multi-collection-sequence-sequence-sequence range-query outcomes.
/// </summary>
public enum HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryStatus
{
    /// <summary>The continuous multi-collection-sequence-sequence-sequence-sequence summary was projected.</summary>
    ContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryProjected = 0,

    /// <summary>The bounded inclusive cross-multi-collection-sequence-sequence-sequence checkpoint range was queried.</summary>
    CrossMultiCollectionSequenceSequenceSequenceCheckpointRangeQueried = 1,

    /// <summary>The optimistic continuity-validation revision did not match.</summary>
    StaleContinuityRevision = 2,

    /// <summary>The multi-collection-sequence-sequence-sequence-summary projection tick preceded continuity time.</summary>
    MultiCollectionSequenceSequenceSequenceSequenceSummaryProjectionTickRegressed = 3,

    /// <summary>The continuity authority did not retain one exact summary and adjacent multi-collection-sequence-sequence-sequence.</summary>
    ContinuitySourceMismatch = 4,

    /// <summary>The source multi-collection-sequence-sequence-sequence-summary indexes were not immediately adjacent.</summary>
    MultiCollectionSequenceSequenceSequenceRangeNotContinuous = 5,

    /// <summary>The source checkpoint ranges were not immediately adjacent.</summary>
    MultiCollectionSequenceSequenceSequenceCheckpointRangeNotContinuous = 6,

    /// <summary>The multi-collection-sequence-sequence-sequence did not expose one exact connecting supersession.</summary>
    MultiCollectionSequenceSequenceSequenceSupersessionMismatch = 7,

    /// <summary>The connecting supersession endpoints did not match the multi-collection-sequence-sequence-sequence boundary.</summary>
    MultiCollectionSequenceSequenceSequenceCheckpointMismatch = 8,

    /// <summary>The optimistic multi-collection-sequence-sequence-sequence-summary revision did not match.</summary>
    StaleMultiCollectionSequenceSequenceSequenceSequenceSummaryRevision = 9,

    /// <summary>The cross-multi-collection-sequence-sequence-sequence query tick preceded multi-collection-sequence-summary time.</summary>
    CrossMultiCollectionSequenceSequenceSequenceRangeQueryTickRegressed = 10,

    /// <summary>The requested start checkpoint was absent from the multi-collection-sequence-sequence-sequence.</summary>
    RangeStartNotFound = 11,

    /// <summary>The requested end checkpoint was absent from the multi-collection-sequence-sequence-sequence.</summary>
    RangeEndNotFound = 12,

    /// <summary>The requested inclusive checkpoint order was invalid.</summary>
    RangeOrderInvalid = 13,

    /// <summary>The requested range did not cross the shared multi-collection-sequence-sequence-sequence boundary.</summary>
    RangeDoesNotCrossMultiCollectionSequenceSequenceSequenceBoundary = 14,

    /// <summary>The requested cross-multi-collection-sequence-sequence-sequence range exceeded the bounded limit.</summary>
    RangeTooLarge = 15,

    /// <summary>The materialized range did not retain the connecting supersession.</summary>
    RangeSupersessionMismatch = 16,
}
