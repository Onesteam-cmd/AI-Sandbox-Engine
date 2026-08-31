using AI.Sandbox.Engine.Core.Entities;
using AI.Sandbox.Engine.Core.Identifiers;

namespace AI.Sandbox.Engine.Core.Components;

/// <summary>
/// Builds one immutable component registry efficiently during world creation or
/// restoration.
/// </summary>
/// <remarks>
/// The builder captures one immutable entity-registry snapshot. Every component
/// must target an entity that is active in that snapshot. The builder is
/// single-use and preserves exact component-type separation.
/// </remarks>
public sealed class ComponentRegistryBuilder
{
    private readonly EntityRegistry entities;
    private readonly Dictionary<Type, IComponentStoreBuilder> builders = [];
    private bool isBuilt;

    /// <summary>
    /// Initializes a builder against one immutable entity-registry snapshot.
    /// </summary>
    /// <param name="entities">
    /// The entity registry whose active identities may receive components.
    /// </param>
    public ComponentRegistryBuilder(EntityRegistry entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        this.entities = entities;
    }

    /// <summary>
    /// Adds one component to one active entity.
    /// </summary>
    /// <typeparam name="TComponent">
    /// The exact concrete component type.
    /// </typeparam>
    /// <param name="entityId">The active target entity.</param>
    /// <param name="component">The immutable component value.</param>
    /// <returns>This builder.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown for an empty ID, invalid component type, duplicate assignment, or
    /// inactive target entity.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown for a null reference component.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown after the builder has been consumed.
    /// </exception>
    public ComponentRegistryBuilder Add<TComponent>(
        Id<EntityIdKind> entityId,
        TComponent component)
        where TComponent : notnull, IComponent
    {
        ThrowIfBuilt();
        ComponentRegistry.EnsureValidEntityId(entityId);
        ComponentTypePolicy.EnsureConcrete<TComponent>();
        ComponentTypePolicy.EnsureValue(component);

        if (entities.GetLifecycleStatus(entityId) !=
            EntityLifecycleStatus.Active)
        {
            throw new ArgumentException(
                "Components can be assigned only to active entities.",
                nameof(entityId));
        }

        if (!builders.TryGetValue(typeof(TComponent), out var untypedBuilder))
        {
            untypedBuilder = new ComponentStoreBuilder<TComponent>();
            builders.Add(typeof(TComponent), untypedBuilder);
        }

        var typedBuilder =
            (ComponentStoreBuilder<TComponent>)untypedBuilder;
        typedBuilder.Add(entityId, component);

        return this;
    }

    /// <summary>
    /// Creates the immutable component registry and permanently consumes this
    /// builder.
    /// </summary>
    /// <returns>The immutable component registry.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown after the builder has already been consumed.
    /// </exception>
    public ComponentRegistry Build()
    {
        ThrowIfBuilt();
        isBuilt = true;

        if (builders.Count == 0)
        {
            return ComponentRegistry.Empty;
        }

        var stores = new Dictionary<Type, IComponentStore>(builders.Count);

        foreach (var pair in builders)
        {
            var store = pair.Value.Build();
            if (store.Count > 0)
            {
                stores.Add(pair.Key, store);
            }
        }

        return ComponentRegistry.FromOwnedStores(stores);
    }

    private void ThrowIfBuilt()
    {
        if (isBuilt)
        {
            throw new InvalidOperationException(
                "A component registry builder cannot be reused after Build.");
        }
    }
}
