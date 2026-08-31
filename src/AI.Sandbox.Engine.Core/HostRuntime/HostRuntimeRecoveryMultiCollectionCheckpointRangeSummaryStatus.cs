namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit multi-collection checkpoint-range summary projection and
/// adjacent collection-sequence selection outcomes.
/// </summary>
public enum HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus
{
    /// <summary>The multi-collection checkpoint-range summary was projected.</summary>
    MultiCollectionCheckpointRangeSummaryProjected = 0,

    /// <summary>The exact previous adjacent collection sequence was selected.</summary>
    PreviousAdjacentCollectionSequenceSelected = 1,

    /// <summary>The exact next adjacent collection sequence was selected.</summary>
    NextAdjacentCollectionSequenceSelected = 2,

    /// <summary>The optimistic multi-collection range revision did not match.</summary>
    StaleRangeRevision = 3,

    /// <summary>The range-summary projection tick preceded range query time.</summary>
    RangeSummaryProjectionTickRegressed = 4,

    /// <summary>The optimistic multi-collection summary revision did not match.</summary>
    StaleSummaryRevision = 5,

    /// <summary>The adjacent collection-sequence selection tick preceded summary time.</summary>
    AdjacentCollectionSequenceSelectionTickRegressed = 6,

    /// <summary>The requested adjacent collection sequence exceeds the bounded collection-pair count.</summary>
    TooManyAdjacentCollectionSequencePairs = 7,

    /// <summary>No collection-pair summary exists immediately before the summarized interval.</summary>
    NoPreviousAdjacentCollectionSequence = 8,

    /// <summary>No collection-pair summary exists immediately after the summarized interval.</summary>
    NoNextAdjacentCollectionSequence = 9,

    /// <summary>The requested previous collection sequence exceeds available prior collection pairs.</summary>
    PreviousAdjacentCollectionSequenceTooShort = 10,

    /// <summary>The requested next collection sequence exceeds available subsequent collection pairs.</summary>
    NextAdjacentCollectionSequenceTooShort = 11,

    /// <summary>The selected collection sequence did not retain exact boundary evidence.</summary>
    AdjacentCollectionSequenceBoundaryMismatch = 12,
}
