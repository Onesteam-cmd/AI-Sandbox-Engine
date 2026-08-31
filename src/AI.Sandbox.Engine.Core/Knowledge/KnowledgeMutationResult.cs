namespace AI.Sandbox.Engine.Core.Knowledge;

/// <summary>
/// Reports one immutable knowledge-set mutation attempt.
/// </summary>
/// <typeparam name="TClaim">The exact concrete claim type.</typeparam>
public sealed class KnowledgeMutationResult<TClaim>
    where TClaim : notnull, IKnowledgeClaim
{
    internal KnowledgeMutationResult(
        KnowledgeMutationStatus status,
        KnowledgeSet<TClaim> knowledgeSet,
        KnowledgeEntry<TClaim>? entry)
    {
        Status = status;
        KnowledgeSet = knowledgeSet;
        Entry = entry;
    }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public KnowledgeMutationStatus Status { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public KnowledgeSet<TClaim> KnowledgeSet { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public KnowledgeEntry<TClaim>? Entry { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public bool WasApplied =>
        Status is KnowledgeMutationStatus.Added or
            KnowledgeMutationStatus.Revised or
            KnowledgeMutationStatus.Removed;
}
