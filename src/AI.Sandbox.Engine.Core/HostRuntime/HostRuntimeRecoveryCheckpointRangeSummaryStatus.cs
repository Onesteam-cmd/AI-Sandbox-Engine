namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit Host recovery checkpoint-range summary projection and
/// adjacent-window selection outcomes.
/// </summary>
public enum HostRuntimeRecoveryCheckpointRangeSummaryStatus
{
    /// <summary>The checkpoint-range summary was projected.</summary>
    CheckpointRangeSummaryProjected = 0,

    /// <summary>The exact previous adjacent window was selected.</summary>
    PreviousAdjacentWindowSelected = 1,

    /// <summary>The exact next adjacent window was selected.</summary>
    NextAdjacentWindowSelected = 2,

    /// <summary>The optimistic checkpoint-range query revision did not match.</summary>
    StaleRangeRevision = 3,

    /// <summary>The range-summary projection tick preceded range query time.</summary>
    RangeSummaryProjectionTickRegressed = 4,

    /// <summary>The optimistic checkpoint-range summary revision did not match.</summary>
    StaleSummaryRevision = 5,

    /// <summary>The adjacent-window selection tick preceded summary time.</summary>
    AdjacentWindowSelectionTickRegressed = 6,

    /// <summary>The requested adjacent window exceeds the bounded checkpoint count.</summary>
    TooManyAdjacentWindowCheckpoints = 7,

    /// <summary>No checkpoint exists immediately before the source range.</summary>
    NoPreviousAdjacentWindow = 8,

    /// <summary>No checkpoint exists immediately after the source range.</summary>
    NoNextAdjacentWindow = 9,

    /// <summary>The requested previous window exceeds available prior lineage.</summary>
    PreviousAdjacentWindowTooShort = 10,

    /// <summary>The requested next window exceeds available subsequent lineage.</summary>
    NextAdjacentWindowTooShort = 11,
}
