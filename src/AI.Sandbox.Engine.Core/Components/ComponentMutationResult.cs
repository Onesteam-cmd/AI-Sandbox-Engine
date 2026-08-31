using AI.Sandbox.Engine.Core.Entities;
using AI.Sandbox.Engine.Core.Identifiers;

namespace AI.Sandbox.Engine.Core.Components;

/// <summary>
/// Reports the outcome of one immutable component mutation.
/// </summary>
public sealed class ComponentMutationResult
{
    internal ComponentMutationResult(
        ComponentMutationStatus status,
        Id<EntityIdKind> entityId,
        Type componentType,
        ComponentRegistry registry)
    {
        Status = status;
        EntityId = entityId;
        ComponentType = componentType;
        Registry = registry;
    }

    /// <summary>
    /// Gets the mutation outcome.
    /// </summary>
    public ComponentMutationStatus Status { get; }

    /// <summary>
    /// Gets the target entity identifier.
    /// </summary>
    public Id<EntityIdKind> EntityId { get; }

    /// <summary>
    /// Gets the exact concrete component type involved in the mutation.
    /// </summary>
    public Type ComponentType { get; }

    /// <summary>
    /// Gets the resulting immutable registry. When no change was applied this
    /// is the original registry instance.
    /// </summary>
    public ComponentRegistry Registry { get; }

    /// <summary>
    /// Gets a value indicating whether component state changed.
    /// </summary>
    public bool WasApplied =>
        Status is ComponentMutationStatus.Added or
            ComponentMutationStatus.Replaced or
            ComponentMutationStatus.Removed;
}
