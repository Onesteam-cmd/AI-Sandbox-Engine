namespace AI.Sandbox.Engine.Core.Memory;

/// <summary>
/// Provides this memory-model API member.
/// </summary>
public enum MemoryMutationStatus
{
    /// <summary>
    /// Represents this memory-model outcome.
    /// </summary>
    Encoded = 0,
    /// <summary>
    /// Represents this memory-model outcome.
    /// </summary>
    Reinforced = 1,
    /// <summary>
    /// Represents this memory-model outcome.
    /// </summary>
    Weakened = 2,
    /// <summary>
    /// Represents this memory-model outcome.
    /// </summary>
    Forgotten = 3,
    /// <summary>
    /// Represents this memory-model outcome.
    /// </summary>
    Removed = 4,
    /// <summary>
    /// Represents this memory-model outcome.
    /// </summary>
    Unchanged = 5,
    /// <summary>
    /// Represents this memory-model outcome.
    /// </summary>
    MemoryAlreadyExists = 6,
    /// <summary>
    /// Represents this memory-model outcome.
    /// </summary>
    MemoryNotFound = 7,
    /// <summary>
    /// Represents this memory-model outcome.
    /// </summary>
    RevisionConflict = 8,
    /// <summary>
    /// Represents this memory-model outcome.
    /// </summary>
    OriginWorldMismatch = 9,
    /// <summary>
    /// Represents this memory-model outcome.
    /// </summary>
    OriginOwnerMismatch = 10,
    /// <summary>
    /// Represents this memory-model outcome.
    /// </summary>
    TemporalRegression = 11,
}
