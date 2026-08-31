namespace AI.Sandbox.Engine.Core.ContextRetrieval;

/// <summary>
/// Executes exact owner-scoped context retrieval against authoritative
/// snapshots without mutating World State or subjective stores.
/// </summary>
/// <typeparam name="TState">The immutable authoritative world-state type.</typeparam>
/// <typeparam name="TQuery">The exact context-query payload type.</typeparam>
/// <typeparam name="TItem">The exact context-item payload type.</typeparam>
public sealed class ContextRetrievalProcessor<TState, TQuery, TItem>
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TQuery : IContextQuery
    where TItem : IContextItem
{
    private readonly global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<TState> manager;
    private readonly global::AI.Sandbox.Engine.Core.Identifiers
        .Id<ContextRetrieverIdKind> retrieverId;
    private readonly IContextRetriever<TState, TQuery, TItem> retriever;

    private ContextRetrievalProcessor(
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateManager<TState>
            manager,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ContextRetrieverIdKind>
            retrieverId,
        IContextRetriever<TState, TQuery, TItem> retriever)
    {
        this.manager = manager;
        this.retrieverId = retrieverId;
        this.retriever = retriever;
    }

    /// <summary>
    /// Creates a processor bound to one authority manager and exact retriever.
    /// </summary>
    /// <param name="manager">The authoritative World State manager.</param>
    /// <param name="retrieverId">The stable exact retriever ID.</param>
    /// <param name="retriever">The synchronous pure retriever.</param>
    /// <returns>The configured retrieval processor.</returns>
    public static ContextRetrievalProcessor<TState, TQuery, TItem> Create(
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateManager<TState>
            manager,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ContextRetrieverIdKind>
            retrieverId,
        IContextRetriever<TState, TQuery, TItem> retriever)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(retriever);

        if (retrieverId.IsEmpty)
        {
            throw new ArgumentException(
                "The context retriever ID cannot be empty.",
                nameof(retrieverId));
        }

        ContextTypePolicy.EnsureExactType(
            typeof(TQuery),
            typeof(IContextQuery),
            "context query");
        ContextTypePolicy.EnsureExactType(
            typeof(TItem),
            typeof(IContextItem),
            "context item");

        return new ContextRetrievalProcessor<TState, TQuery, TItem>(
            manager,
            retrieverId,
            retriever);
    }

    /// <summary>
    /// Retrieves one exact query once when its snapshot coordinates are current.
    /// </summary>
    /// <param name="query">The exact owner-scoped context query.</param>
    /// <returns>The explicit retrieval result.</returns>
    public ContextRetrievalResult<TQuery, TItem> Retrieve(
        ContextQueryEnvelope<TQuery> query)
    {
        ArgumentNullException.ThrowIfNull(query);

        var before = manager.Read();

        if (query.WorldId != before.WorldId)
        {
            return ContextRetrievalResult<TQuery, TItem>.NotEvaluated(
                ContextRetrievalStatus.WorldMismatch,
                query);
        }

        if (query.WorldStateVersion != before.Version)
        {
            return ContextRetrievalResult<TQuery, TItem>.NotEvaluated(
                ContextRetrievalStatus.VersionConflict,
                query);
        }

        if (query.SimulationTick != before.SimulationTick)
        {
            return ContextRetrievalResult<TQuery, TItem>.NotEvaluated(
                ContextRetrievalStatus.SimulationTickMismatch,
                query);
        }

        var context = new ContextRetrievalContext<TState, TQuery>(
            before,
            query,
            retrieverId);
        var decision = retriever.Retrieve(context) ??
            throw new InvalidOperationException(
                "Context retrievers cannot return null decisions.");

        var after = manager.Read();

        if (after.SimulationTick != before.SimulationTick)
        {
            return ContextRetrievalResult<TQuery, TItem>.Discarded(
                ContextRetrievalStatus.SimulationTickMismatch,
                query);
        }

        if (after.Version != before.Version)
        {
            return ContextRetrievalResult<TQuery, TItem>.Discarded(
                ContextRetrievalStatus.VersionConflict,
                query);
        }

        var status = decision.Status switch
        {
            ContextRetrievalDecisionStatus.Empty =>
                ContextRetrievalStatus.Empty,
            ContextRetrievalDecisionStatus.Rejected =>
                ContextRetrievalStatus.Rejected,
            ContextRetrievalDecisionStatus.Retrieved =>
                ValidateRetrievedItems(query, decision.Items),
            _ => throw new InvalidOperationException(
                "Unknown context retrieval decision status."),
        };

        return ContextRetrievalResult<TQuery, TItem>.Evaluated(
            status,
            query,
            decision);
    }

    private ContextRetrievalStatus ValidateRetrievedItems(
        ContextQueryEnvelope<TQuery> query,
        IReadOnlyList<ContextItemEnvelope<TItem>> items)
    {
        foreach (var item in items)
        {
            if (item.WorldId != query.WorldId)
            {
                return ContextRetrievalStatus.ResultWorldMismatch;
            }

            if (item.OwnerEntityId != query.OwnerEntityId)
            {
                return ContextRetrievalStatus.ResultOwnerMismatch;
            }

            if (item.RetrieverId != retrieverId)
            {
                return ContextRetrievalStatus.ResultRetrieverMismatch;
            }
        }

        return items.Count > query.ItemLimit.Value
            ? ContextRetrievalStatus.ItemLimitExceeded
            : ContextRetrievalStatus.Retrieved;
    }
}
