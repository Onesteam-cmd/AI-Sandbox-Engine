using AI.Sandbox.Engine.Core.Entities;
using AI.Sandbox.Engine.Core.Identifiers;

namespace AI.Sandbox.Engine.Core.Components;

/// <summary>
/// Represents immutable, exact-type component storage for one world-state
/// snapshot.
/// </summary>
/// <remarks>
/// Each concrete component type has an independent typed store. Component
/// values may be attached only to active entities. Removing a component or
/// purging an entity remains valid even after entity destruction, allowing one
/// World State transition to clean stale data explicitly.
/// </remarks>
public sealed class ComponentRegistry
{
    private readonly IReadOnlyDictionary<Type, IComponentStore> stores;

    private ComponentRegistry(Dictionary<Type, IComponentStore> stores)
    {
        this.stores =
            new System.Collections.ObjectModel.ReadOnlyDictionary<
                Type,
                IComponentStore>(stores);
        ComponentCount = stores.Values.Sum(store => store.Count);
    }

    /// <summary>
    /// Gets the empty component registry.
    /// </summary>
    public static ComponentRegistry Empty { get; } = new(
        new Dictionary<Type, IComponentStore>());

    /// <summary>
    /// Gets the number of exact concrete component types currently stored.
    /// </summary>
    public int ComponentTypeCount => stores.Count;

    /// <summary>
    /// Gets the total number of entity-component values.
    /// </summary>
    public int ComponentCount { get; }

    internal static ComponentRegistry FromOwnedStores(
        Dictionary<Type, IComponentStore> stores)
    {
        return stores.Count == 0
            ? Empty
            : new ComponentRegistry(stores);
    }

    /// <summary>
    /// Gets the number of values stored for one exact component type.
    /// </summary>
    /// <typeparam name="TComponent">
    /// The exact concrete component type.
    /// </typeparam>
    /// <returns>The number of stored values.</returns>
    public int GetCount<TComponent>()
        where TComponent : notnull, IComponent
    {
        ComponentTypePolicy.EnsureConcrete<TComponent>();

        return stores.TryGetValue(typeof(TComponent), out var store)
            ? store.Count
            : 0;
    }

    /// <summary>
    /// Determines whether one entity has an exact component type.
    /// </summary>
    /// <typeparam name="TComponent">
    /// The exact concrete component type.
    /// </typeparam>
    /// <param name="entityId">The non-empty entity identifier.</param>
    /// <returns>
    /// <see langword="true"/> when the exact component is present.
    /// </returns>
    public bool Contains<TComponent>(Id<EntityIdKind> entityId)
        where TComponent : notnull, IComponent
    {
        EnsureValidEntityId(entityId);
        ComponentTypePolicy.EnsureConcrete<TComponent>();

        return stores.TryGetValue(typeof(TComponent), out var untypedStore) &&
            ((ComponentStore<TComponent>)untypedStore).Contains(entityId);
    }

    /// <summary>
    /// Attempts to read one exact component value.
    /// </summary>
    /// <typeparam name="TComponent">
    /// The exact concrete component type.
    /// </typeparam>
    /// <param name="entityId">The non-empty entity identifier.</param>
    /// <param name="component">
    /// The stored component when found; otherwise the default value.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when the exact component is present.
    /// </returns>
    public bool TryGet<TComponent>(
        Id<EntityIdKind> entityId,
        out TComponent component)
        where TComponent : notnull, IComponent
    {
        EnsureValidEntityId(entityId);
        ComponentTypePolicy.EnsureConcrete<TComponent>();

        if (!stores.TryGetValue(
            typeof(TComponent),
            out var untypedStore))
        {
            component = default!;
            return false;
        }

        return ((ComponentStore<TComponent>)untypedStore)
            .TryGet(entityId, out component);
    }

    /// <summary>
    /// Gets the entity IDs that currently own one exact component type.
    /// </summary>
    /// <typeparam name="TComponent">
    /// The exact concrete component type.
    /// </typeparam>
    /// <returns>
    /// A read-only list in deterministic entity-ID order.
    /// </returns>
    public IReadOnlyList<Id<EntityIdKind>> GetEntityIds<TComponent>()
        where TComponent : notnull, IComponent
    {
        ComponentTypePolicy.EnsureConcrete<TComponent>();

        return stores.TryGetValue(typeof(TComponent), out var store)
            ? store.EntityIds
            : Array.Empty<Id<EntityIdKind>>();
    }

    /// <summary>
    /// Adds or replaces one component on an active entity.
    /// </summary>
    /// <typeparam name="TComponent">
    /// The exact concrete component type.
    /// </typeparam>
    /// <param name="entities">
    /// The immutable entity-registry snapshot used to validate activity.
    /// </param>
    /// <param name="entityId">The target entity.</param>
    /// <param name="component">The immutable component value.</param>
    /// <returns>An explicit immutable mutation result.</returns>
    public ComponentMutationResult Set<TComponent>(
        EntityRegistry entities,
        Id<EntityIdKind> entityId,
        TComponent component)
        where TComponent : notnull, IComponent
    {
        ArgumentNullException.ThrowIfNull(entities);
        EnsureValidEntityId(entityId);
        ComponentTypePolicy.EnsureConcrete<TComponent>();
        ComponentTypePolicy.EnsureValue(component);

        if (entities.GetLifecycleStatus(entityId) !=
            EntityLifecycleStatus.Active)
        {
            return new ComponentMutationResult(
                ComponentMutationStatus.EntityNotActive,
                entityId,
                typeof(TComponent),
                this);
        }

        var currentStore =
            stores.TryGetValue(typeof(TComponent), out var untypedStore)
                ? (ComponentStore<TComponent>)untypedStore
                : ComponentStore<TComponent>.Empty;
        var nextStore = currentStore.Set(
            entityId,
            component,
            out var status);

        if (status == ComponentMutationStatus.Unchanged)
        {
            return new ComponentMutationResult(
                status,
                entityId,
                typeof(TComponent),
                this);
        }

        var nextStores = new Dictionary<Type, IComponentStore>(stores)
        {
            [typeof(TComponent)] = nextStore,
        };
        var nextRegistry = new ComponentRegistry(nextStores);

        return new ComponentMutationResult(
            status,
            entityId,
            typeof(TComponent),
            nextRegistry);
    }

    /// <summary>
    /// Removes one exact component type from an entity.
    /// </summary>
    /// <typeparam name="TComponent">
    /// The exact concrete component type.
    /// </typeparam>
    /// <param name="entityId">The non-empty entity identifier.</param>
    /// <returns>An explicit immutable mutation result.</returns>
    /// <remarks>
    /// Entity activity is not required for removal. This allows component
    /// cleanup after an entity lifecycle transition has marked the identity as
    /// destroyed.
    /// </remarks>
    public ComponentMutationResult Remove<TComponent>(
        Id<EntityIdKind> entityId)
        where TComponent : notnull, IComponent
    {
        EnsureValidEntityId(entityId);
        ComponentTypePolicy.EnsureConcrete<TComponent>();

        if (!stores.TryGetValue(
            typeof(TComponent),
            out var untypedStore))
        {
            return new ComponentMutationResult(
                ComponentMutationStatus.NotFound,
                entityId,
                typeof(TComponent),
                this);
        }

        var currentStore = (ComponentStore<TComponent>)untypedStore;
        var nextStore = currentStore.Remove(entityId, out var wasRemoved);

        if (!wasRemoved)
        {
            return new ComponentMutationResult(
                ComponentMutationStatus.NotFound,
                entityId,
                typeof(TComponent),
                this);
        }

        var nextStores = new Dictionary<Type, IComponentStore>(stores);
        if (nextStore.Count == 0)
        {
            _ = nextStores.Remove(typeof(TComponent));
        }
        else
        {
            nextStores[typeof(TComponent)] = nextStore;
        }

        var nextRegistry = FromOwnedStores(nextStores);

        return new ComponentMutationResult(
            ComponentMutationStatus.Removed,
            entityId,
            typeof(TComponent),
            nextRegistry);
    }

    /// <summary>
    /// Removes every stored component value attached to one entity.
    /// </summary>
    /// <param name="entityId">The non-empty entity identifier.</param>
    /// <returns>The purge result and resulting immutable registry.</returns>
    public ComponentPurgeResult PurgeEntity(Id<EntityIdKind> entityId)
    {
        EnsureValidEntityId(entityId);

        if (stores.Count == 0)
        {
            return new ComponentPurgeResult(entityId, 0, this);
        }

        Dictionary<Type, IComponentStore>? nextStores = null;
        var removedCount = 0;

        foreach (var pair in stores)
        {
            var nextStore = pair.Value.Remove(
                entityId,
                out var wasRemoved);

            if (!wasRemoved)
            {
                continue;
            }

            nextStores ??= new Dictionary<Type, IComponentStore>(stores);
            removedCount++;

            if (nextStore.Count == 0)
            {
                _ = nextStores.Remove(pair.Key);
            }
            else
            {
                nextStores[pair.Key] = nextStore;
            }
        }

        if (nextStores is null)
        {
            return new ComponentPurgeResult(entityId, 0, this);
        }

        return new ComponentPurgeResult(
            entityId,
            removedCount,
            FromOwnedStores(nextStores));
    }

    /// <summary>
    /// Determines whether every stored component belongs to an active entity in
    /// the supplied immutable entity registry.
    /// </summary>
    /// <param name="entities">
    /// The entity-registry snapshot to validate against.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when every stored component owner is active.
    /// </returns>
    public bool IsConsistentWith(EntityRegistry entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        foreach (var store in stores.Values)
        {
            foreach (var entityId in store.EntityIds)
            {
                if (entities.GetLifecycleStatus(entityId) !=
                    EntityLifecycleStatus.Active)
                {
                    return false;
                }
            }
        }

        return true;
    }

    internal static void EnsureValidEntityId(Id<EntityIdKind> entityId)
    {
        if (entityId.IsEmpty)
        {
            throw new ArgumentException(
                "A component owner entity identifier cannot be empty.",
                nameof(entityId));
        }
    }
}
