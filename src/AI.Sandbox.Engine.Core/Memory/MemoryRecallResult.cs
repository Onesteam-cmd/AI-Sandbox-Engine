namespace AI.Sandbox.Engine.Core.Memory;

/// <summary>
/// Provides this memory-model API member.
/// </summary>
public sealed class MemoryRecallResult<TContent>
    where TContent : notnull, IMemoryContent
{
    internal MemoryRecallResult(
        MemoryRecallQuery query,
        MemoryEntry<TContent>[] entries)
    {
        Query = query;
        this.entries = Array.AsReadOnly(
            (MemoryEntry<TContent>[])entries.Clone());
    }

    private readonly IReadOnlyList<MemoryEntry<TContent>> entries;

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemoryRecallQuery Query { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public IReadOnlyList<MemoryEntry<TContent>> Entries => entries;

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public int Count => entries.Count;
}
