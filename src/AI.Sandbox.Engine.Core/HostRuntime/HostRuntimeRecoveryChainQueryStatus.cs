namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit Host recovery chain-summary projection and
/// checkpoint-lineage query outcomes.
/// </summary>
public enum HostRuntimeRecoveryChainQueryStatus
{
    /// <summary>The validated chain was projected into a compact summary.</summary>
    ChainSummaryProjected = 0,

    /// <summary>The requested checkpoint lineage was resolved.</summary>
    CheckpointLineageResolved = 1,

    /// <summary>The optimistic supersession-chain revision did not match.</summary>
    StaleChainRevision = 2,

    /// <summary>The projection tick preceded chain validation.</summary>
    ProjectionTickRegressed = 3,

    /// <summary>The optimistic chain-summary projection revision did not match.</summary>
    StaleProjectionRevision = 4,

    /// <summary>The query tick preceded summary projection.</summary>
    QueryTickRegressed = 5,

    /// <summary>The requested checkpoint does not belong to the validated chain.</summary>
    CheckpointNotFound = 6,
}
