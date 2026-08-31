using AI.Sandbox.Engine.Core.Entities;
using AI.Sandbox.Engine.Core.Identifiers;

namespace AI.Sandbox.Engine.Core.Components;

/// <summary>
/// Reports the result of removing every component attached to one entity.
/// </summary>
public sealed class ComponentPurgeResult
{
    internal ComponentPurgeResult(
        Id<EntityIdKind> entityId,
        int removedComponentCount,
        ComponentRegistry registry)
    {
        EntityId = entityId;
        RemovedComponentCount = removedComponentCount;
        Registry = registry;
    }

    /// <summary>
    /// Gets the entity whose components were removed.
    /// </summary>
    public Id<EntityIdKind> EntityId { get; }

    /// <summary>
    /// Gets the number of distinct component values removed.
    /// </summary>
    public int RemovedComponentCount { get; }

    /// <summary>
    /// Gets the resulting immutable component registry.
    /// </summary>
    public ComponentRegistry Registry { get; }

    /// <summary>
    /// Gets a value indicating whether any component was removed.
    /// </summary>
    public bool WasApplied => RemovedComponentCount > 0;
}
