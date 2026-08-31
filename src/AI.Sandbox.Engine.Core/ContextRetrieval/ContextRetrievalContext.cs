namespace AI.Sandbox.Engine.Core.ContextRetrieval;

/// <summary>
/// Supplies one retriever with a stable authoritative snapshot, exact query,
/// and explicit retriever identity.
/// </summary>
/// <typeparam name="TState">The immutable authoritative world-state type.</typeparam>
/// <typeparam name="TQuery">The exact context-query payload type.</typeparam>
public sealed class ContextRetrievalContext<TState, TQuery>
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TQuery : IContextQuery
{
    internal ContextRetrievalContext(
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateSnapshot<TState>
            snapshot,
        ContextQueryEnvelope<TQuery> query,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ContextRetrieverIdKind>
            retrieverId)
    {
        Snapshot = snapshot;
        Query = query;
        RetrieverId = retrieverId;
    }

    /// <summary>
    /// Gets the stable authoritative snapshot used for retrieval.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateSnapshot<TState>
        Snapshot { get; }

    /// <summary>
    /// Gets the exact owner-scoped query being evaluated.
    /// </summary>
    public ContextQueryEnvelope<TQuery> Query { get; }

    /// <summary>
    /// Gets the exact retriever identity for returned item provenance.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<ContextRetrieverIdKind>
        RetrieverId { get; }
}
