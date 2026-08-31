namespace AI.Sandbox.Engine.Core.Entities;

/// <summary>
/// Identifies the outcome of one immutable entity-registry mutation.
/// </summary>
public enum EntityMutationStatus
{
    /// <summary>
    /// A previously unknown entity identifier became active.
    /// </summary>
    Created = 0,

    /// <summary>
    /// The identifier was already known and therefore cannot be created again.
    /// </summary>
    AlreadyKnown = 1,

    /// <summary>
    /// An active entity became destroyed.
    /// </summary>
    Destroyed = 2,

    /// <summary>
    /// The identifier has never been registered in this world.
    /// </summary>
    Unknown = 3,

    /// <summary>
    /// The identifier belongs to an entity that was already destroyed.
    /// </summary>
    AlreadyDestroyed = 4,
}
