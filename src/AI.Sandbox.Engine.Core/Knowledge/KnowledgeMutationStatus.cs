namespace AI.Sandbox.Engine.Core.Knowledge;

/// <summary>
/// Identifies one immutable knowledge-set mutation outcome.
/// </summary>
public enum KnowledgeMutationStatus
{
    /// <summary>
    /// Represents this knowledge-model outcome.
    /// </summary>
    Added = 0,
    /// <summary>
    /// Represents this knowledge-model outcome.
    /// </summary>
    Revised = 1,
    /// <summary>
    /// Represents this knowledge-model outcome.
    /// </summary>
    Removed = 2,
    /// <summary>
    /// Represents this knowledge-model outcome.
    /// </summary>
    Unchanged = 3,
    /// <summary>
    /// Represents this knowledge-model outcome.
    /// </summary>
    ClaimAlreadyExists = 4,
    /// <summary>
    /// Represents this knowledge-model outcome.
    /// </summary>
    ClaimNotFound = 5,
    /// <summary>
    /// Represents this knowledge-model outcome.
    /// </summary>
    RevisionConflict = 6,
    /// <summary>
    /// Represents this knowledge-model outcome.
    /// </summary>
    EvidenceWorldMismatch = 7,
    /// <summary>
    /// Represents this knowledge-model outcome.
    /// </summary>
    EvidenceOwnerMismatch = 8,
    /// <summary>
    /// Represents this knowledge-model outcome.
    /// </summary>
    EvidenceRegression = 9,
}
