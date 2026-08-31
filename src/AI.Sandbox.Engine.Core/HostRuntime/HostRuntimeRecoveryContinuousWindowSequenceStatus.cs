namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit continuous-window sequence validation and multi-window
/// checkpoint-range query outcomes.
/// </summary>
public enum HostRuntimeRecoveryContinuousWindowSequenceStatus
{
    /// <summary>The bounded continuous-window sequence was validated.</summary>
    ContinuousWindowSequenceValidated = 0,

    /// <summary>The bounded inclusive multi-window checkpoint range was queried.</summary>
    MultiWindowCheckpointRangeQueried = 1,

    /// <summary>No continuous-window pair summaries were supplied.</summary>
    PairCollectionEmpty = 2,

    /// <summary>The supplied pair collection exceeded the bounded limit.</summary>
    PairCollectionTooLarge = 3,

    /// <summary>The optimistic pair-revision count did not match.</summary>
    PairRevisionCountMismatch = 4,

    /// <summary>One optimistic pair-summary revision did not match.</summary>
    StalePairSummaryRevision = 5,

    /// <summary>The sequence-validation tick preceded one pair-summary tick.</summary>
    SequenceValidationTickRegressed = 6,

    /// <summary>The pair sequence repeated one externally identified summary.</summary>
    DuplicatePairSummaryId = 7,

    /// <summary>One pair did not retain the exact shared source projection and chain.</summary>
    PairSourceMismatch = 8,

    /// <summary>One pair did not retain its exact internal connecting supersession.</summary>
    PairSupersessionMismatch = 9,

    /// <summary>Consecutive pairs were not ordered as one continuous sequence.</summary>
    SequenceNotContinuous = 10,

    /// <summary>The supersession connecting consecutive pairs did not match.</summary>
    SequenceBoundarySupersessionMismatch = 11,

    /// <summary>The optimistic sequence-validation revision did not match.</summary>
    StaleSequenceRevision = 12,

    /// <summary>The multi-window query tick preceded sequence-validation time.</summary>
    MultiWindowRangeQueryTickRegressed = 13,

    /// <summary>The requested start checkpoint was absent from the sequence.</summary>
    RangeStartNotFound = 14,

    /// <summary>The requested end checkpoint was absent from the sequence.</summary>
    RangeEndNotFound = 15,

    /// <summary>The requested inclusive checkpoint order was invalid.</summary>
    RangeOrderInvalid = 16,

    /// <summary>The requested range did not cross a validated window boundary.</summary>
    RangeDoesNotCrossWindowBoundary = 17,

    /// <summary>The requested multi-window range exceeded the bounded limit.</summary>
    RangeTooLarge = 18,

    /// <summary>The materialized range did not retain every crossed boundary.</summary>
    RangeSupersessionMismatch = 19,
}
