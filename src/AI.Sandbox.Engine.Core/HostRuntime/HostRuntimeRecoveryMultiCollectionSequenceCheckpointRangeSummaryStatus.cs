namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit multi-collection-sequence checkpoint-range summary projection and
/// adjacent multi-collection selection outcomes.
/// </summary>
public enum HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus
{
    /// <summary>The multi-collection-sequence checkpoint-range summary was projected.</summary>
    MultiCollectionSequenceCheckpointRangeSummaryProjected = 0,

    /// <summary>The exact previous adjacent multi-collection was selected.</summary>
    PreviousAdjacentMultiCollectionSelected = 1,

    /// <summary>The exact next adjacent multi-collection was selected.</summary>
    NextAdjacentMultiCollectionSelected = 2,

    /// <summary>The optimistic multi-collection-sequence range revision did not match.</summary>
    StaleRangeRevision = 3,

    /// <summary>The range-summary projection tick preceded range query time.</summary>
    RangeSummaryProjectionTickRegressed = 4,

    /// <summary>The optimistic multi-collection-sequence summary revision did not match.</summary>
    StaleSummaryRevision = 5,

    /// <summary>The adjacent multi-collection selection tick preceded summary time.</summary>
    AdjacentMultiCollectionSelectionTickRegressed = 6,

    /// <summary>The requested adjacent multi-collection exceeds the bounded summary count.</summary>
    TooManyAdjacentMultiCollections = 7,

    /// <summary>No multi-collection summary exists immediately before the summarized interval.</summary>
    NoPreviousAdjacentMultiCollection = 8,

    /// <summary>No multi-collection summary exists immediately after the summarized interval.</summary>
    NoNextAdjacentMultiCollection = 9,

    /// <summary>The requested previous multi-collection exceeds available prior summaries.</summary>
    PreviousAdjacentMultiCollectionTooShort = 10,

    /// <summary>The requested next multi-collection exceeds available subsequent summaries.</summary>
    NextAdjacentMultiCollectionTooShort = 11,

    /// <summary>The selected multi-collection did not retain exact boundary evidence.</summary>
    AdjacentMultiCollectionBoundaryMismatch = 12,
}
