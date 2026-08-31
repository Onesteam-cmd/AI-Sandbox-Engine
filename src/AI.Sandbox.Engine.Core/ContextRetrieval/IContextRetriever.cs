namespace AI.Sandbox.Engine.Core.ContextRetrieval;

/// <summary>
/// Defines one synchronous pure retriever for an exact query and item type.
/// </summary>
/// <typeparam name="TState">The immutable authoritative world-state type.</typeparam>
/// <typeparam name="TQuery">The exact context-query payload type.</typeparam>
/// <typeparam name="TItem">The exact context-item payload type.</typeparam>
public interface IContextRetriever<TState, TQuery, TItem>
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TQuery : IContextQuery
    where TItem : IContextItem
{
    /// <summary>
    /// Retrieves context exactly once without changing World State.
    /// </summary>
    /// <param name="context">The stable owner-scoped retrieval context.</param>
    /// <returns>Retrieved items, empty success, or explicit rejection.</returns>
    public ContextRetrievalDecision<TItem> Retrieve(
        ContextRetrievalContext<TState, TQuery> context);
}
