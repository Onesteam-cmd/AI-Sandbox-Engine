using AI.Sandbox.Engine.Core.Identifiers;

namespace AI.Sandbox.Engine.Core.Entities;

/// <summary>
/// Reports the outcome of one immutable entity-registry mutation.
/// </summary>
public sealed class EntityMutationResult
{
    internal EntityMutationResult(
        EntityMutationStatus status,
        Id<EntityIdKind> entityId,
        EntityRegistry registry)
    {
        Status = status;
        EntityId = entityId;
        Registry = registry;
    }

    /// <summary>
    /// Gets the mutation outcome.
    /// </summary>
    public EntityMutationStatus Status { get; }

    /// <summary>
    /// Gets the entity identifier targeted by the mutation.
    /// </summary>
    public Id<EntityIdKind> EntityId { get; }

    /// <summary>
    /// Gets the resulting immutable registry. For a rejected mutation this is
    /// the original registry instance.
    /// </summary>
    public EntityRegistry Registry { get; }

    /// <summary>
    /// Gets a value indicating whether the registry changed.
    /// </summary>
    public bool WasApplied =>
        Status is EntityMutationStatus.Created or
            EntityMutationStatus.Destroyed;
}
