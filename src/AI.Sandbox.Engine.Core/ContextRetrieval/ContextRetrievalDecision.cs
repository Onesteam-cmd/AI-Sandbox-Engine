namespace AI.Sandbox.Engine.Core.ContextRetrieval;

/// <summary>
/// Represents one pure deterministic context-retrieval decision.
/// </summary>
/// <typeparam name="TItem">The exact context-item payload type.</typeparam>
public sealed class ContextRetrievalDecision<TItem>
    where TItem : IContextItem
{
    private readonly IReadOnlyList<ContextItemEnvelope<TItem>> items;
    private readonly ContextRejectionCode rejectionCode;

    private ContextRetrievalDecision(
        ContextRetrievalDecisionStatus status,
        IReadOnlyList<ContextItemEnvelope<TItem>> items,
        ContextRejectionCode rejectionCode)
    {
        Status = status;
        this.items = items;
        this.rejectionCode = rejectionCode;
    }

    /// <summary>
    /// Gets the retriever decision status.
    /// </summary>
    public ContextRetrievalDecisionStatus Status { get; }

    /// <summary>
    /// Gets deterministically ordered retrieved items.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The decision did not retrieve items.
    /// </exception>
    public IReadOnlyList<ContextItemEnvelope<TItem>> Items =>
        Status == ContextRetrievalDecisionStatus.Retrieved
            ? items
            : throw new InvalidOperationException(
                "A non-retrieved context decision has no items.");

    /// <summary>
    /// Gets the stable retrieval rejection code.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The decision is not rejected.
    /// </exception>
    public ContextRejectionCode RejectionCode =>
        Status == ContextRetrievalDecisionStatus.Rejected
            ? rejectionCode
            : throw new InvalidOperationException(
                "A non-rejected context decision has no rejection code.");

    /// <summary>
    /// Creates a retrieved decision from one or more context items.
    /// </summary>
    /// <param name="items">The items to validate and order.</param>
    /// <returns>The immutable retrieved decision.</returns>
    public static ContextRetrievalDecision<TItem> Retrieve(
        IEnumerable<ContextItemEnvelope<TItem>> items)
    {
        ContextTypePolicy.EnsureExactType(
            typeof(TItem),
            typeof(IContextItem),
            "context item");
        ArgumentNullException.ThrowIfNull(items);

        var materialized = items.ToArray();
        if (materialized.Length == 0)
        {
            throw new ArgumentException(
                "Retrieved context decisions require at least one item.",
                nameof(items));
        }

        var itemIds = new HashSet<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<ContextItemIdKind>>();
        foreach (var item in materialized)
        {
            ArgumentNullException.ThrowIfNull(item);
            if (!itemIds.Add(item.ItemId))
            {
                throw new ArgumentException(
                    "Retrieved context items cannot repeat an item ID.",
                    nameof(items));
            }
        }

        Array.Sort(
            materialized,
            static (left, right) =>
            {
                var relevanceOrder = right.Relevance.CompareTo(left.Relevance);
                return relevanceOrder != 0
                    ? relevanceOrder
                    : left.ItemId.CompareTo(right.ItemId);
            });

        return new ContextRetrievalDecision<TItem>(
            ContextRetrievalDecisionStatus.Retrieved,
            Array.AsReadOnly(materialized),
            default);
    }

    /// <summary>
    /// Creates an explicit successful decision with no relevant items.
    /// </summary>
    /// <returns>The empty decision.</returns>
    public static ContextRetrievalDecision<TItem> Empty()
    {
        ContextTypePolicy.EnsureExactType(
            typeof(TItem),
            typeof(IContextItem),
            "context item");

        return new ContextRetrievalDecision<TItem>(
            ContextRetrievalDecisionStatus.Empty,
            Array.Empty<ContextItemEnvelope<TItem>>(),
            default);
    }

    /// <summary>
    /// Creates an explicitly rejected retrieval decision.
    /// </summary>
    /// <param name="rejectionCode">The initialized stable rejection code.</param>
    /// <returns>The rejected decision.</returns>
    public static ContextRetrievalDecision<TItem> Reject(
        ContextRejectionCode rejectionCode)
    {
        ContextTypePolicy.EnsureExactType(
            typeof(TItem),
            typeof(IContextItem),
            "context item");

        if (!rejectionCode.IsInitialized)
        {
            throw new ArgumentException(
                "The context rejection code must be initialized.",
                nameof(rejectionCode));
        }

        return new ContextRetrievalDecision<TItem>(
            ContextRetrievalDecisionStatus.Rejected,
            Array.Empty<ContextItemEnvelope<TItem>>(),
            rejectionCode);
    }
}
