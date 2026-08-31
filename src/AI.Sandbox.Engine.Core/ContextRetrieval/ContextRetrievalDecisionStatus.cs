namespace AI.Sandbox.Engine.Core.ContextRetrieval;

/// <summary>
/// Describes the semantic decision returned by one context retriever.
/// </summary>
public enum ContextRetrievalDecisionStatus
{
    /// <summary>
    /// One or more typed context items were retrieved.
    /// </summary>
    Retrieved = 1,

    /// <summary>
    /// The query was valid but no relevant items were found.
    /// </summary>
    Empty = 2,

    /// <summary>
    /// Retrieval was explicitly refused for a stable reason.
    /// </summary>
    Rejected = 3,
}
