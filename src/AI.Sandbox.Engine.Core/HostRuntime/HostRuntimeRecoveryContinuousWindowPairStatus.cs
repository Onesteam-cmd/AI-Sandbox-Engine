namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit continuous-window pair summary and cross-window range-query outcomes.
/// </summary>
public enum HostRuntimeRecoveryContinuousWindowPairStatus
{
    /// <summary>The continuous-window pair summary was projected.</summary>
    ContinuousWindowPairSummaryProjected = 0,

    /// <summary>The bounded inclusive cross-window checkpoint range was queried.</summary>
    CrossWindowCheckpointRangeQueried = 1,

    /// <summary>The optimistic continuity-validation revision did not match.</summary>
    StaleContinuityRevision = 2,

    /// <summary>The pair-summary projection tick preceded continuity time.</summary>
    PairSummaryProjectionTickRegressed = 3,

    /// <summary>The continuity authority did not retain one exact source pair.</summary>
    ContinuitySourceMismatch = 4,

    /// <summary>The source authorities were not an exact continuous window pair.</summary>
    WindowPairNotContinuous = 5,

    /// <summary>The pair did not expose one exact connecting supersession.</summary>
    WindowPairSupersessionMismatch = 6,

    /// <summary>The connecting supersession endpoints did not match the pair boundary.</summary>
    WindowPairCheckpointMismatch = 7,

    /// <summary>The optimistic pair-summary revision did not match.</summary>
    StalePairSummaryRevision = 8,

    /// <summary>The cross-window query tick preceded pair-summary time.</summary>
    CrossWindowRangeQueryTickRegressed = 9,

    /// <summary>The requested start checkpoint was absent from the pair.</summary>
    RangeStartNotFound = 10,

    /// <summary>The requested end checkpoint was absent from the pair.</summary>
    RangeEndNotFound = 11,

    /// <summary>The requested inclusive checkpoint order was invalid.</summary>
    RangeOrderInvalid = 12,

    /// <summary>The requested range did not cross the shared window boundary.</summary>
    RangeDoesNotCrossWindowBoundary = 13,

    /// <summary>The requested cross-window range exceeded the bounded limit.</summary>
    RangeTooLarge = 14,

    /// <summary>The materialized range did not retain the connecting supersession.</summary>
    RangeSupersessionMismatch = 15,
}
