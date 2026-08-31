namespace AI.Sandbox.Engine.Core.Relationships;

/// <summary>
/// Identifies the outcome of one current relationship mutation.
/// </summary>
public enum RelationshipMutationStatus
{
    /// <summary>
    /// A new directed relationship was added.
    /// </summary>
    Added = 0,

    /// <summary>
    /// An existing directed relationship was revised.
    /// </summary>
    Revised = 1,

    /// <summary>
    /// An existing directed relationship was removed.
    /// </summary>
    Removed = 2,

    /// <summary>
    /// The requested payload and latest change were already current.
    /// </summary>
    Unchanged = 3,

    /// <summary>
    /// A relationship for the target already exists.
    /// </summary>
    RelationshipAlreadyExists = 4,

    /// <summary>
    /// No relationship for the target exists.
    /// </summary>
    RelationshipNotFound = 5,

    /// <summary>
    /// The expected relationship revision did not match.
    /// </summary>
    RevisionConflict = 6,

    /// <summary>
    /// The change reference belongs to another world.
    /// </summary>
    ChangeWorldMismatch = 7,

    /// <summary>
    /// The change reference belongs to another owner.
    /// </summary>
    ChangeOwnerMismatch = 8,

    /// <summary>
    /// The change reference names another target.
    /// </summary>
    ChangeTargetMismatch = 9,

    /// <summary>
    /// The change metadata precedes the current relationship update.
    /// </summary>
    TemporalRegression = 10,
}
