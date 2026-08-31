namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit multi-window checkpoint-range summary projection and
/// adjacent-sequence selection outcomes.
/// </summary>
public enum HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus
{
    /// <summary>The multi-window checkpoint-range summary was projected.</summary>
    MultiWindowCheckpointRangeSummaryProjected = 0,

    /// <summary>The exact previous adjacent sequence was selected.</summary>
    PreviousAdjacentSequenceSelected = 1,

    /// <summary>The exact next adjacent sequence was selected.</summary>
    NextAdjacentSequenceSelected = 2,

    /// <summary>The optimistic multi-window range revision did not match.</summary>
    StaleRangeRevision = 3,

    /// <summary>The range-summary projection tick preceded range query time.</summary>
    RangeSummaryProjectionTickRegressed = 4,

    /// <summary>The optimistic multi-window summary revision did not match.</summary>
    StaleSummaryRevision = 5,

    /// <summary>The adjacent-sequence selection tick preceded summary time.</summary>
    AdjacentSequenceSelectionTickRegressed = 6,

    /// <summary>The requested adjacent sequence exceeds the bounded pair count.</summary>
    TooManyAdjacentSequencePairs = 7,

    /// <summary>No pair exists immediately before the summarized pair interval.</summary>
    NoPreviousAdjacentSequence = 8,

    /// <summary>No pair exists immediately after the summarized pair interval.</summary>
    NoNextAdjacentSequence = 9,

    /// <summary>The requested previous sequence exceeds available prior pairs.</summary>
    PreviousAdjacentSequenceTooShort = 10,

    /// <summary>The requested next sequence exceeds available subsequent pairs.</summary>
    NextAdjacentSequenceTooShort = 11,

    /// <summary>The selected sequence did not retain exact boundary evidence.</summary>
    AdjacentSequenceBoundaryMismatch = 12,
}
