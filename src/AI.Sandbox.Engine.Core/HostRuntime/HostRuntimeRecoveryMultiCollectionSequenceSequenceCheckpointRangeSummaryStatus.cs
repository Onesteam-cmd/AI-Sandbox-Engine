namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit multi-collection-sequence-sequence checkpoint-range summary projection
/// and adjacent multi-collection-sequence selection outcomes.
/// </summary>
public enum HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus
{
    /// <summary>The compact checkpoint-range summary was projected.</summary>
    MultiCollectionSequenceSequenceCheckpointRangeSummaryProjected = 0,

    /// <summary>The previous adjacent multi-collection-sequence was selected.</summary>
    PreviousAdjacentMultiCollectionSequenceSelected = 1,

    /// <summary>The next adjacent multi-collection-sequence was selected.</summary>
    NextAdjacentMultiCollectionSequenceSelected = 2,

    /// <summary>The optimistic source-range revision did not match.</summary>
    StaleRangeRevision = 3,

    /// <summary>The range-summary projection tick preceded range-query time.</summary>
    RangeSummaryProjectionTickRegressed = 4,

    /// <summary>The optimistic summary revision did not match.</summary>
    StaleSummaryRevision = 5,

    /// <summary>The adjacent multi-collection-sequence selection tick preceded summary time.</summary>
    AdjacentMultiCollectionSequenceSelectionTickRegressed = 6,

    /// <summary>The requested adjacent multi-collection-sequence exceeds the bounded summary count.</summary>
    TooManyAdjacentMultiCollectionSequences = 7,

    /// <summary>No multi-collection-sequence summary exists immediately before the summarized interval.</summary>
    NoPreviousAdjacentMultiCollectionSequence = 8,

    /// <summary>No multi-collection-sequence summary exists immediately after the summarized interval.</summary>
    NoNextAdjacentMultiCollectionSequence = 9,

    /// <summary>The requested previous multi-collection-sequence exceeds available prior summaries.</summary>
    PreviousAdjacentMultiCollectionSequenceTooShort = 10,

    /// <summary>The requested next multi-collection-sequence exceeds available subsequent summaries.</summary>
    NextAdjacentMultiCollectionSequenceTooShort = 11,

    /// <summary>The selected multi-collection-sequence did not retain exact boundary evidence.</summary>
    AdjacentMultiCollectionSequenceBoundaryMismatch = 12,
}
