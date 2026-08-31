namespace AI.Sandbox.Engine.Core.ContextRetrieval;

/// <summary>
/// Describes the complete outcome of one context-retrieval invocation.
/// </summary>
public enum ContextRetrievalStatus
{
    /// <summary>
    /// One or more validated context items were returned.
    /// </summary>
    Retrieved = 1,

    /// <summary>
    /// Retrieval completed successfully with no relevant items.
    /// </summary>
    Empty = 2,

    /// <summary>
    /// Retrieval was explicitly rejected by the retriever.
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// The query belongs to another world.
    /// </summary>
    WorldMismatch = 4,

    /// <summary>
    /// The query version is stale or authority changed during retrieval.
    /// </summary>
    VersionConflict = 5,

    /// <summary>
    /// The query tick is stale or authority advanced during retrieval.
    /// </summary>
    SimulationTickMismatch = 6,

    /// <summary>
    /// A returned item belongs to another world.
    /// </summary>
    ResultWorldMismatch = 7,

    /// <summary>
    /// A returned item belongs to another subjective owner.
    /// </summary>
    ResultOwnerMismatch = 8,

    /// <summary>
    /// A returned item claims another retriever identity.
    /// </summary>
    ResultRetrieverMismatch = 9,

    /// <summary>
    /// The retriever returned more items than the query allowed.
    /// </summary>
    ItemLimitExceeded = 10,
}
