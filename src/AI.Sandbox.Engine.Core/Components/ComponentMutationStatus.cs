namespace AI.Sandbox.Engine.Core.Components;

/// <summary>
/// Identifies the outcome of one immutable component mutation.
/// </summary>
public enum ComponentMutationStatus
{
    /// <summary>
    /// A component was attached to an entity for the first time.
    /// </summary>
    Added = 0,

    /// <summary>
    /// An existing component value was replaced.
    /// </summary>
    Replaced = 1,

    /// <summary>
    /// The supplied value was equal to the existing value.
    /// </summary>
    Unchanged = 2,

    /// <summary>
    /// An existing component was removed.
    /// </summary>
    Removed = 3,

    /// <summary>
    /// The requested component was not present.
    /// </summary>
    NotFound = 4,

    /// <summary>
    /// The target entity was unknown or destroyed.
    /// </summary>
    EntityNotActive = 5,
}
