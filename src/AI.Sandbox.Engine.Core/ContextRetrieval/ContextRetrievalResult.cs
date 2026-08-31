namespace AI.Sandbox.Engine.Core.ContextRetrieval;

/// <summary>
/// Represents the explicit result of one context-retrieval invocation.
/// </summary>
/// <typeparam name="TQuery">The exact context-query payload type.</typeparam>
/// <typeparam name="TItem">The exact context-item payload type.</typeparam>
public sealed class ContextRetrievalResult<TQuery, TItem>
    where TQuery : IContextQuery
    where TItem : IContextItem
{
    private ContextRetrievalResult(
        ContextRetrievalStatus status,
        ContextQueryEnvelope<TQuery> query,
        bool retrieverWasInvoked,
        ContextRetrievalDecision<TItem>? decision)
    {
        Status = status;
        Query = query;
        RetrieverWasInvoked = retrieverWasInvoked;
        Decision = decision;
    }

    /// <summary>
    /// Gets the complete invocation status.
    /// </summary>
    public ContextRetrievalStatus Status { get; }

    /// <summary>
    /// Gets the query supplied to retrieval.
    /// </summary>
    public ContextQueryEnvelope<TQuery> Query { get; }

    /// <summary>
    /// Gets a value indicating whether the retriever was invoked.
    /// </summary>
    public bool RetrieverWasInvoked { get; }

    /// <summary>
    /// Gets the stable retriever decision when authority did not invalidate it.
    /// </summary>
    public ContextRetrievalDecision<TItem>? Decision { get; }

    /// <summary>
    /// Gets a value indicating whether a stable retriever decision is present.
    /// </summary>
    public bool HasStableDecision => Decision is not null;

    /// <summary>
    /// Gets a value indicating whether validated context items were returned.
    /// </summary>
    public bool WasRetrieved => Status == ContextRetrievalStatus.Retrieved;

    internal static ContextRetrievalResult<TQuery, TItem> NotEvaluated(
        ContextRetrievalStatus status,
        ContextQueryEnvelope<TQuery> query) =>
        new(status, query, false, null);

    internal static ContextRetrievalResult<TQuery, TItem> Discarded(
        ContextRetrievalStatus status,
        ContextQueryEnvelope<TQuery> query) =>
        new(status, query, true, null);

    internal static ContextRetrievalResult<TQuery, TItem> Evaluated(
        ContextRetrievalStatus status,
        ContextQueryEnvelope<TQuery> query,
        ContextRetrievalDecision<TItem> decision) =>
        new(status, query, true, decision);
}
