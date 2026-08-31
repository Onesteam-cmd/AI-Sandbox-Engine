namespace AI.Sandbox.Engine.Core.Memory;

/// <summary>
/// Provides this memory-model API member.
/// </summary>
public readonly record struct MemoryRecallQuery
{
    private MemoryRecallQuery(
        int maximumResults,
        MemoryStrength minimumStrength,
        MemorySalience minimumSalience)
    {
        MaximumResults = maximumResults;
        MinimumStrength = minimumStrength;
        MinimumSalience = minimumSalience;
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public int MaximumResults { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemoryStrength MinimumStrength { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemorySalience MinimumSalience { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public static MemoryRecallQuery Create(
        int maximumResults,
        MemoryStrength minimumStrength,
        MemorySalience minimumSalience)
    {
        if (maximumResults is < 1 or > 1_024)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumResults),
                maximumResults,
                "Recall result count must be between 1 and 1024.");
        }

        minimumStrength.EnsureInitialized();
        minimumSalience.EnsureInitialized();

        return new MemoryRecallQuery(
            maximumResults,
            minimumStrength,
            minimumSalience);
    }
}
