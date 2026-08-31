namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit multi-collection-sequence-sequence-sequence checkpoint-range summary projection
/// and adjacent multi-collection-sequence-sequence selection outcomes.
/// </summary>
public enum HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus
{
    /// <summary>The compact checkpoint-range summary was projected.</summary>
    MultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjected = 0,

    /// <summary>The previous adjacent multi-collection-sequence-sequence was selected.</summary>
    PreviousAdjacentMultiCollectionSequenceSequenceSelected = 1,

    /// <summary>The next adjacent multi-collection-sequence-sequence was selected.</summary>
    NextAdjacentMultiCollectionSequenceSequenceSelected = 2,

    /// <summary>The optimistic source-range revision did not match.</summary>
    StaleRangeRevision = 3,

    /// <summary>The range-summary projection tick preceded range-query time.</summary>
    RangeSummaryProjectionTickRegressed = 4,

    /// <summary>The optimistic summary revision did not match.</summary>
    StaleSummaryRevision = 5,

    /// <summary>The adjacent multi-collection-sequence-sequence selection tick preceded summary time.</summary>
    AdjacentMultiCollectionSequenceSequenceSelectionTickRegressed = 6,

    /// <summary>The requested adjacent multi-collection-sequence-sequence exceeds the bounded summary count.</summary>
    TooManyAdjacentMultiCollectionSequenceSequences = 7,

    /// <summary>No multi-collection-sequence-sequence summary exists immediately before the summarized interval.</summary>
    NoPreviousAdjacentMultiCollectionSequenceSequence = 8,

    /// <summary>No multi-collection-sequence-sequence summary exists immediately after the summarized interval.</summary>
    NoNextAdjacentMultiCollectionSequenceSequence = 9,

    /// <summary>The requested previous multi-collection-sequence-sequence exceeds available prior summaries.</summary>
    PreviousAdjacentMultiCollectionSequenceSequenceTooShort = 10,

    /// <summary>The requested next multi-collection-sequence-sequence exceeds available subsequent summaries.</summary>
    NextAdjacentMultiCollectionSequenceSequenceTooShort = 11,

    /// <summary>The selected multi-collection-sequence-sequence did not retain exact boundary evidence.</summary>
    AdjacentMultiCollectionSequenceSequenceBoundaryMismatch = 12,
}
