namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit multi-sequence checkpoint-range summary projection and
/// adjacent-collection selection outcomes.
/// </summary>
public enum HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus
{
    /// <summary>The multi-sequence checkpoint-range summary was projected.</summary>
    MultiSequenceCheckpointRangeSummaryProjected = 0,

    /// <summary>The exact previous adjacent collection was selected.</summary>
    PreviousAdjacentCollectionSelected = 1,

    /// <summary>The exact next adjacent collection was selected.</summary>
    NextAdjacentCollectionSelected = 2,

    /// <summary>The optimistic multi-sequence range revision did not match.</summary>
    StaleRangeRevision = 3,

    /// <summary>The range-summary projection tick preceded range query time.</summary>
    RangeSummaryProjectionTickRegressed = 4,

    /// <summary>The optimistic multi-sequence summary revision did not match.</summary>
    StaleSummaryRevision = 5,

    /// <summary>The adjacent-collection selection tick preceded summary time.</summary>
    AdjacentCollectionSelectionTickRegressed = 6,

    /// <summary>The requested adjacent collection exceeds the bounded summary count.</summary>
    TooManyAdjacentCollectionSummaries = 7,

    /// <summary>No summary exists immediately before the summarized interval.</summary>
    NoPreviousAdjacentCollection = 8,

    /// <summary>No summary exists immediately after the summarized interval.</summary>
    NoNextAdjacentCollection = 9,

    /// <summary>The requested previous collection exceeds available prior summaries.</summary>
    PreviousAdjacentCollectionTooShort = 10,

    /// <summary>The requested next collection exceeds available subsequent summaries.</summary>
    NextAdjacentCollectionTooShort = 11,

    /// <summary>The selected collection did not retain exact boundary evidence.</summary>
    AdjacentCollectionBoundaryMismatch = 12,
}
