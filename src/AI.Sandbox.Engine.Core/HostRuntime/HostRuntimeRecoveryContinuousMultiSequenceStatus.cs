namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit continuous multi-sequence summary and cross-sequence range-query outcomes.
/// </summary>
public enum HostRuntimeRecoveryContinuousMultiSequenceStatus
{
    /// <summary>The continuous multi-sequence summary was projected.</summary>
    ContinuousMultiSequenceSummaryProjected = 0,

    /// <summary>The bounded inclusive cross-sequence checkpoint range was queried.</summary>
    CrossSequenceCheckpointRangeQueried = 1,

    /// <summary>The optimistic continuity-validation revision did not match.</summary>
    StaleContinuityRevision = 2,

    /// <summary>The multi-sequence-summary projection tick preceded continuity time.</summary>
    MultiSequenceSummaryProjectionTickRegressed = 3,

    /// <summary>The continuity authority did not retain one exact summary and adjacent sequence.</summary>
    ContinuitySourceMismatch = 4,

    /// <summary>The source sequence pair indexes were not immediately adjacent.</summary>
    SequencePairNotContinuous = 5,

    /// <summary>The source checkpoint ranges were not immediately adjacent.</summary>
    SequenceCheckpointRangeNotContinuous = 6,

    /// <summary>The multi-sequence did not expose one exact connecting supersession.</summary>
    SequencePairSupersessionMismatch = 7,

    /// <summary>The connecting supersession endpoints did not match the multi-sequence boundary.</summary>
    SequencePairCheckpointMismatch = 8,

    /// <summary>The optimistic multi-sequence-summary revision did not match.</summary>
    StaleMultiSequenceSummaryRevision = 9,

    /// <summary>The cross-sequence query tick preceded multi-sequence-summary time.</summary>
    CrossSequenceRangeQueryTickRegressed = 10,

    /// <summary>The requested start checkpoint was absent from the multi-sequence.</summary>
    RangeStartNotFound = 11,

    /// <summary>The requested end checkpoint was absent from the multi-sequence.</summary>
    RangeEndNotFound = 12,

    /// <summary>The requested inclusive checkpoint order was invalid.</summary>
    RangeOrderInvalid = 13,

    /// <summary>The requested range did not cross the shared sequence boundary.</summary>
    RangeDoesNotCrossSequenceBoundary = 14,

    /// <summary>The requested cross-sequence range exceeded the bounded limit.</summary>
    RangeTooLarge = 15,

    /// <summary>The materialized range did not retain the connecting supersession.</summary>
    RangeSupersessionMismatch = 16,
}
