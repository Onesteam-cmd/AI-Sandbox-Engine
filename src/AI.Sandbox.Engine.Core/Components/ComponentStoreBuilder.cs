using AI.Sandbox.Engine.Core.Entities;
using AI.Sandbox.Engine.Core.Identifiers;

namespace AI.Sandbox.Engine.Core.Components;

internal sealed class ComponentStoreBuilder<TComponent> :
    IComponentStoreBuilder
    where TComponent : notnull, IComponent
{
    private readonly Dictionary<Id<EntityIdKind>, TComponent> components = [];

    public void Add(
        Id<EntityIdKind> entityId,
        TComponent component)
    {
        if (!components.TryAdd(entityId, component))
        {
            throw new ArgumentException(
                $"Entity '{entityId}' already has component " +
                $"'{typeof(TComponent)}'.",
                nameof(entityId));
        }
    }

    public IComponentStore Build()
    {
        if (components.Count == 0)
        {
            return ComponentStore<TComponent>.Empty;
        }

        var ordered = components
            .OrderBy(pair => pair.Key)
            .ToArray();
        var entityIds = new Id<EntityIdKind>[ordered.Length];
        var values = new TComponent[ordered.Length];

        for (var index = 0; index < ordered.Length; index++)
        {
            entityIds[index] = ordered[index].Key;
            values[index] = ordered[index].Value;
        }

        return ComponentStore<TComponent>.FromSorted(entityIds, values);
    }
}
