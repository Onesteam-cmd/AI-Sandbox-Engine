namespace AI.Sandbox.Engine.Core.Memory;

/// <summary>
/// Provides this memory-model API member.
/// </summary>
public sealed class MemoryMutationResult<TContent>
    where TContent : notnull, IMemoryContent
{
    internal MemoryMutationResult(
        MemoryMutationStatus status,
        MemoryStore<TContent> memoryStore,
        MemoryEntry<TContent>? entry)
    {
        Status = status;
        MemoryStore = memoryStore;
        Entry = entry;
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemoryMutationStatus Status { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemoryStore<TContent> MemoryStore { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemoryEntry<TContent>? Entry { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public bool WasApplied =>
        Status is MemoryMutationStatus.Encoded or
            MemoryMutationStatus.Reinforced or
            MemoryMutationStatus.Weakened or
            MemoryMutationStatus.Forgotten or
            MemoryMutationStatus.Removed;
}
