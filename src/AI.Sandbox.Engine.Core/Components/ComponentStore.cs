using AI.Sandbox.Engine.Core.Entities;
using AI.Sandbox.Engine.Core.Identifiers;

namespace AI.Sandbox.Engine.Core.Components;

internal sealed class ComponentStore<TComponent> : IComponentStore
    where TComponent : notnull, IComponent
{
    private readonly Id<EntityIdKind>[] entityIds;
    private readonly TComponent[] components;
    private readonly System.Collections.ObjectModel.ReadOnlyCollection<
        Id<EntityIdKind>> readOnlyEntityIds;

    private ComponentStore(
        Id<EntityIdKind>[] entityIds,
        TComponent[] components)
    {
        this.entityIds = entityIds;
        this.components = components;
        readOnlyEntityIds = Array.AsReadOnly(entityIds);
    }

    public static ComponentStore<TComponent> Empty { get; } = new(
        Array.Empty<Id<EntityIdKind>>(),
        Array.Empty<TComponent>());

    public int Count => entityIds.Length;

    public IReadOnlyList<Id<EntityIdKind>> EntityIds => readOnlyEntityIds;

    public static ComponentStore<TComponent> FromSorted(
        Id<EntityIdKind>[] entityIds,
        TComponent[] components)
    {
        if (entityIds.Length != components.Length)
        {
            throw new ArgumentException(
                "Component entity and value arrays must have equal lengths.");
        }

        return entityIds.Length == 0
            ? Empty
            : new ComponentStore<TComponent>(entityIds, components);
    }

    public bool Contains(Id<EntityIdKind> entityId)
    {
        return Array.BinarySearch(entityIds, entityId) >= 0;
    }

    public bool TryGet(
        Id<EntityIdKind> entityId,
        out TComponent component)
    {
        var index = Array.BinarySearch(entityIds, entityId);
        if (index < 0)
        {
            component = default!;
            return false;
        }

        component = components[index];
        return true;
    }

    public ComponentStore<TComponent> Set(
        Id<EntityIdKind> entityId,
        TComponent component,
        out ComponentMutationStatus status)
    {
        var index = Array.BinarySearch(entityIds, entityId);
        if (index >= 0)
        {
            if (EqualityComparer<TComponent>.Default.Equals(
                components[index],
                component))
            {
                status = ComponentMutationStatus.Unchanged;
                return this;
            }

            var nextComponents = (TComponent[])components.Clone();
            nextComponents[index] = component;
            status = ComponentMutationStatus.Replaced;

            return new ComponentStore<TComponent>(
                entityIds,
                nextComponents);
        }

        var insertionIndex = ~index;
        var nextEntityIds = new Id<EntityIdKind>[entityIds.Length + 1];
        var nextValues = new TComponent[components.Length + 1];

        Array.Copy(entityIds, 0, nextEntityIds, 0, insertionIndex);
        Array.Copy(components, 0, nextValues, 0, insertionIndex);

        nextEntityIds[insertionIndex] = entityId;
        nextValues[insertionIndex] = component;

        Array.Copy(
            entityIds,
            insertionIndex,
            nextEntityIds,
            insertionIndex + 1,
            entityIds.Length - insertionIndex);
        Array.Copy(
            components,
            insertionIndex,
            nextValues,
            insertionIndex + 1,
            components.Length - insertionIndex);

        status = ComponentMutationStatus.Added;
        return new ComponentStore<TComponent>(nextEntityIds, nextValues);
    }

    public ComponentStore<TComponent> Remove(
        Id<EntityIdKind> entityId,
        out bool wasRemoved)
    {
        var index = Array.BinarySearch(entityIds, entityId);
        if (index < 0)
        {
            wasRemoved = false;
            return this;
        }

        if (entityIds.Length == 1)
        {
            wasRemoved = true;
            return Empty;
        }

        var nextEntityIds = new Id<EntityIdKind>[entityIds.Length - 1];
        var nextValues = new TComponent[components.Length - 1];

        Array.Copy(entityIds, 0, nextEntityIds, 0, index);
        Array.Copy(components, 0, nextValues, 0, index);
        Array.Copy(
            entityIds,
            index + 1,
            nextEntityIds,
            index,
            entityIds.Length - index - 1);
        Array.Copy(
            components,
            index + 1,
            nextValues,
            index,
            components.Length - index - 1);

        wasRemoved = true;
        return new ComponentStore<TComponent>(nextEntityIds, nextValues);
    }

    IComponentStore IComponentStore.Remove(
        Id<EntityIdKind> entityId,
        out bool wasRemoved)
    {
        return Remove(entityId, out wasRemoved);
    }
}
