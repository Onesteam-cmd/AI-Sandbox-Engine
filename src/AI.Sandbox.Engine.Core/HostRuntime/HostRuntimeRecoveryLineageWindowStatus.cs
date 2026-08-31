namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit Host recovery lineage-window projection and bounded
/// checkpoint-range query outcomes.
/// </summary>
public enum HostRuntimeRecoveryLineageWindowStatus
{
    /// <summary>The requested contiguous lineage window was projected.</summary>
    LineageWindowProjected = 0,

    /// <summary>The requested inclusive checkpoint range was resolved.</summary>
    CheckpointRangeResolved = 1,

    /// <summary>The optimistic chain-summary projection revision did not match.</summary>
    StaleProjectionRevision = 2,

    /// <summary>The window projection tick preceded source projection time.</summary>
    WindowProjectionTickRegressed = 3,

    /// <summary>The requested window start index is outside the source chain.</summary>
    WindowStartOutOfRange = 4,

    /// <summary>The requested window end index is outside the source chain.</summary>
    WindowEndOutOfRange = 5,

    /// <summary>The requested window exceeds the bounded checkpoint count.</summary>
    TooManyWindowCheckpoints = 6,

    /// <summary>The optimistic lineage-window revision did not match.</summary>
    StaleWindowRevision = 7,

    /// <summary>The range query tick preceded window projection time.</summary>
    RangeQueryTickRegressed = 8,

    /// <summary>The requested range-start checkpoint is absent from the window.</summary>
    RangeStartNotFound = 9,

    /// <summary>The requested range-end checkpoint is absent from the window.</summary>
    RangeEndNotFound = 10,

    /// <summary>The requested range ends before it starts.</summary>
    RangeOrderInvalid = 11,
}
