namespace AI.Sandbox.Engine.Core.Tests;

public sealed class ComponentSystemTests
{
    private readonly record struct Position(int X, int Y) :
        global::AI.Sandbox.Engine.Core.Components.IComponent;

    private readonly record struct Velocity(int X, int Y) :
        global::AI.Sandbox.Engine.Core.Components.IComponent;

    private sealed record DisplayName(string Value) :
        global::AI.Sandbox.Engine.Core.Components.IComponent;

    private abstract record AbstractComponent :
        global::AI.Sandbox.Engine.Core.Components.IComponent;

    private record OpenReferenceComponent(int Value) :
        global::AI.Sandbox.Engine.Core.Components.IComponent;

    private sealed record ComponentWorldState(
        global::AI.Sandbox.Engine.Core.Entities.EntityRegistry Entities,
        global::AI.Sandbox.Engine.Core.Components.ComponentRegistry Components) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    [Xunit.Fact]
    public void Empty_HasNoComponentData()
    {
        var registry =
            global::AI.Sandbox.Engine.Core.Components.ComponentRegistry.Empty;

        Xunit.Assert.Equal(0, registry.ComponentTypeCount);
        Xunit.Assert.Equal(0, registry.ComponentCount);
        Xunit.Assert.Equal(0, registry.GetCount<Position>());
        Xunit.Assert.Empty(registry.GetEntityIds<Position>());
    }

    [Xunit.Fact]
    public void Builder_BuildsMultipleExactComponentTypes()
    {
        var first = CreateEntityId(1);
        var second = CreateEntityId(2);
        var entities = ActiveEntities(first, second);

        var registry =
            new global::AI.Sandbox.Engine.Core.Components
                .ComponentRegistryBuilder(entities)
                .Add(first, new Position(10, 20))
                .Add(first, new DisplayName("First"))
                .Add(second, new Position(30, 40))
                .Build();

        Xunit.Assert.Equal(2, registry.ComponentTypeCount);
        Xunit.Assert.Equal(3, registry.ComponentCount);
        Xunit.Assert.Equal(2, registry.GetCount<Position>());
        Xunit.Assert.Equal(1, registry.GetCount<DisplayName>());
        Xunit.Assert.True(registry.IsConsistentWith(entities));
    }

    [Xunit.Fact]
    public void Builder_SortsEntityIdsDeterministically()
    {
        var lower = CreateEntityId(1);
        var higher = CreateEntityId(2);
        var entities = ActiveEntities(lower, higher);

        var registry =
            new global::AI.Sandbox.Engine.Core.Components
                .ComponentRegistryBuilder(entities)
                .Add(higher, new Position(2, 2))
                .Add(lower, new Position(1, 1))
                .Build();

        Xunit.Assert.Equal(
            new[] { lower, higher },
            registry.GetEntityIds<Position>());
    }

    [Xunit.Fact]
    public void Builder_RejectsDuplicateTypeAssignment()
    {
        var entityId = CreateEntityId(1);
        var entities = ActiveEntities(entityId);
        var builder =
            new global::AI.Sandbox.Engine.Core.Components
                .ComponentRegistryBuilder(entities)
                .Add(entityId, new Position(1, 1));

        Xunit.Assert.Throws<ArgumentException>(
            () => builder.Add(entityId, new Position(2, 2)));
    }

    [Xunit.Fact]
    public void Builder_RejectsUnknownAndDestroyedEntities()
    {
        var active = CreateEntityId(1);
        var unknown = CreateEntityId(2);
        var entities = ActiveEntities(active);
        var destroyed = entities.DestroyEntity(active).Registry;

        Xunit.Assert.Throws<ArgumentException>(
            () => new global::AI.Sandbox.Engine.Core.Components
                .ComponentRegistryBuilder(entities)
                .Add(unknown, new Position(0, 0)));

        Xunit.Assert.Throws<ArgumentException>(
            () => new global::AI.Sandbox.Engine.Core.Components
                .ComponentRegistryBuilder(destroyed)
                .Add(active, new Position(0, 0)));
    }

    [Xunit.Fact]
    public void Builder_RejectsNullReferenceComponent()
    {
        var entityId = CreateEntityId(1);
        var entities = ActiveEntities(entityId);
        var builder =
            new global::AI.Sandbox.Engine.Core.Components
                .ComponentRegistryBuilder(entities);

        Xunit.Assert.Throws<ArgumentNullException>(
            () => builder.Add<DisplayName>(entityId, null!));
    }

    [Xunit.Fact]
    public void Builder_RejectsAbstractAndUnsealedReferenceTypes()
    {
        var entityId = CreateEntityId(1);
        var entities = ActiveEntities(entityId);
        var builder =
            new global::AI.Sandbox.Engine.Core.Components
                .ComponentRegistryBuilder(entities);

        Xunit.Assert.Throws<ArgumentException>(
            () => builder.Add<AbstractComponent>(entityId, null!));
        Xunit.Assert.Throws<ArgumentException>(
            () => builder.Add(
                entityId,
                new OpenReferenceComponent(1)));
    }

    [Xunit.Fact]
    public void Builder_CannotBeReusedAfterBuild()
    {
        var entityId = CreateEntityId(1);
        var entities = ActiveEntities(entityId);
        var builder =
            new global::AI.Sandbox.Engine.Core.Components
                .ComponentRegistryBuilder(entities)
                .Add(entityId, new Position(1, 1));

        _ = builder.Build();

        Xunit.Assert.Throws<InvalidOperationException>(
            () => builder.Add(entityId, new Velocity(1, 1)));
        Xunit.Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Xunit.Fact]
    public void Builder_HandlesLargeInitialPopulationWithoutRepeatedImmutableAdds()
    {
        var ids = Enumerable.Range(1, 1_000)
            .Select(CreateEntityId)
            .ToArray();
        var entities =
            global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
                .FromActiveEntities(ids);
        var builder =
            new global::AI.Sandbox.Engine.Core.Components
                .ComponentRegistryBuilder(entities);

        foreach (var id in ids.Reverse())
        {
            _ = builder.Add(id, new Position(1, 2));
        }

        var registry = builder.Build();

        Xunit.Assert.Equal(1_000, registry.ComponentCount);
        Xunit.Assert.Equal(ids, registry.GetEntityIds<Position>());
    }

    [Xunit.Fact]
    public void Set_AddsComponentWithoutChangingPreviousRegistry()
    {
        var entityId = CreateEntityId(1);
        var entities = ActiveEntities(entityId);
        var original =
            global::AI.Sandbox.Engine.Core.Components.ComponentRegistry.Empty;

        var result = original.Set(
            entities,
            entityId,
            new Position(10, 20));

        Xunit.Assert.True(result.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Components
                .ComponentMutationStatus.Added,
            result.Status);
        Xunit.Assert.NotSame(original, result.Registry);
        Xunit.Assert.Equal(0, original.ComponentCount);
        Xunit.Assert.True(
            result.Registry.TryGet<Position>(
                entityId,
                out var position));
        Xunit.Assert.Equal(new Position(10, 20), position);
    }

    [Xunit.Fact]
    public void Set_ReplacesExistingComponent()
    {
        var entityId = CreateEntityId(1);
        var entities = ActiveEntities(entityId);
        var original =
            global::AI.Sandbox.Engine.Core.Components.ComponentRegistry.Empty
                .Set(entities, entityId, new Position(1, 1))
                .Registry;

        var result = original.Set(
            entities,
            entityId,
            new Position(2, 3));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Components
                .ComponentMutationStatus.Replaced,
            result.Status);
        Xunit.Assert.True(
            result.Registry.TryGet<Position>(
                entityId,
                out var position));
        Xunit.Assert.Equal(new Position(2, 3), position);
        Xunit.Assert.True(
            original.TryGet<Position>(
                entityId,
                out var oldPosition));
        Xunit.Assert.Equal(new Position(1, 1), oldPosition);
    }

    [Xunit.Fact]
    public void Set_EqualValueReturnsOriginalRegistry()
    {
        var entityId = CreateEntityId(1);
        var entities = ActiveEntities(entityId);
        var original =
            global::AI.Sandbox.Engine.Core.Components.ComponentRegistry.Empty
                .Set(entities, entityId, new Position(1, 1))
                .Registry;

        var result = original.Set(
            entities,
            entityId,
            new Position(1, 1));

        Xunit.Assert.False(result.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Components
                .ComponentMutationStatus.Unchanged,
            result.Status);
        Xunit.Assert.Same(original, result.Registry);
    }

    [Xunit.Fact]
    public void Set_RejectsUnknownAndDestroyedEntitiesWithoutMutation()
    {
        var active = CreateEntityId(1);
        var unknown = CreateEntityId(2);
        var entities = ActiveEntities(active);
        var destroyed = entities.DestroyEntity(active).Registry;
        var components =
            global::AI.Sandbox.Engine.Core.Components.ComponentRegistry.Empty;

        var unknownResult = components.Set(
            entities,
            unknown,
            new Position(0, 0));
        var destroyedResult = components.Set(
            destroyed,
            active,
            new Position(0, 0));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Components
                .ComponentMutationStatus.EntityNotActive,
            unknownResult.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Components
                .ComponentMutationStatus.EntityNotActive,
            destroyedResult.Status);
        Xunit.Assert.Same(components, unknownResult.Registry);
        Xunit.Assert.Same(components, destroyedResult.Registry);
    }

    [Xunit.Fact]
    public void ExactComponentTypesRemainIndependent()
    {
        var entityId = CreateEntityId(1);
        var entities = ActiveEntities(entityId);
        var registry =
            global::AI.Sandbox.Engine.Core.Components.ComponentRegistry.Empty
                .Set(entities, entityId, new Position(1, 2))
                .Registry
                .Set(entities, entityId, new Velocity(3, 4))
                .Registry;

        Xunit.Assert.True(registry.Contains<Position>(entityId));
        Xunit.Assert.True(registry.Contains<Velocity>(entityId));
        Xunit.Assert.Equal(2, registry.ComponentTypeCount);
        Xunit.Assert.Equal(2, registry.ComponentCount);
    }

    [Xunit.Fact]
    public void Remove_DeletesOnlyRequestedExactComponent()
    {
        var entityId = CreateEntityId(1);
        var entities = ActiveEntities(entityId);
        var original =
            global::AI.Sandbox.Engine.Core.Components.ComponentRegistry.Empty
                .Set(entities, entityId, new Position(1, 2))
                .Registry
                .Set(entities, entityId, new Velocity(3, 4))
                .Registry;

        var result = original.Remove<Position>(entityId);

        Xunit.Assert.True(result.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Components
                .ComponentMutationStatus.Removed,
            result.Status);
        Xunit.Assert.False(result.Registry.Contains<Position>(entityId));
        Xunit.Assert.True(result.Registry.Contains<Velocity>(entityId));
        Xunit.Assert.Equal(1, result.Registry.ComponentTypeCount);
        Xunit.Assert.Equal(1, result.Registry.ComponentCount);
    }

    [Xunit.Fact]
    public void Remove_MissingComponentReturnsOriginalRegistry()
    {
        var entityId = CreateEntityId(1);
        var registry =
            global::AI.Sandbox.Engine.Core.Components.ComponentRegistry.Empty;

        var result = registry.Remove<Position>(entityId);

        Xunit.Assert.False(result.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Components
                .ComponentMutationStatus.NotFound,
            result.Status);
        Xunit.Assert.Same(registry, result.Registry);
    }

    [Xunit.Fact]
    public void PurgeEntity_RemovesEveryComponentTypeForOneEntity()
    {
        var first = CreateEntityId(1);
        var second = CreateEntityId(2);
        var entities = ActiveEntities(first, second);
        var original =
            global::AI.Sandbox.Engine.Core.Components.ComponentRegistry.Empty
                .Set(entities, first, new Position(1, 1))
                .Registry
                .Set(entities, first, new Velocity(2, 2))
                .Registry
                .Set(entities, second, new Position(3, 3))
                .Registry;

        var result = original.PurgeEntity(first);

        Xunit.Assert.True(result.WasApplied);
        Xunit.Assert.Equal(2, result.RemovedComponentCount);
        Xunit.Assert.False(result.Registry.Contains<Position>(first));
        Xunit.Assert.False(result.Registry.Contains<Velocity>(first));
        Xunit.Assert.True(result.Registry.Contains<Position>(second));
        Xunit.Assert.Equal(1, result.Registry.ComponentCount);
    }

    [Xunit.Fact]
    public void PurgeEntity_WithoutComponentsReturnsOriginalRegistry()
    {
        var entityId = CreateEntityId(1);
        var registry =
            global::AI.Sandbox.Engine.Core.Components.ComponentRegistry.Empty;

        var result = registry.PurgeEntity(entityId);

        Xunit.Assert.False(result.WasApplied);
        Xunit.Assert.Equal(0, result.RemovedComponentCount);
        Xunit.Assert.Same(registry, result.Registry);
    }

    [Xunit.Fact]
    public void ConsistencyDetectsComponentsOwnedByDestroyedEntity()
    {
        var entityId = CreateEntityId(1);
        var activeEntities = ActiveEntities(entityId);
        var components =
            global::AI.Sandbox.Engine.Core.Components.ComponentRegistry.Empty
                .Set(
                    activeEntities,
                    entityId,
                    new Position(1, 1))
                .Registry;
        var destroyedEntities =
            activeEntities.DestroyEntity(entityId).Registry;

        Xunit.Assert.True(components.IsConsistentWith(activeEntities));
        Xunit.Assert.False(components.IsConsistentWith(destroyedEntities));

        var cleaned = components.PurgeEntity(entityId).Registry;
        Xunit.Assert.True(cleaned.IsConsistentWith(destroyedEntities));
    }

    [Xunit.Fact]
    public void EntityIdViews_AreReadOnly()
    {
        var entityId = CreateEntityId(1);
        var entities = ActiveEntities(entityId);
        var registry =
            global::AI.Sandbox.Engine.Core.Components.ComponentRegistry.Empty
                .Set(entities, entityId, new Position(1, 1))
                .Registry;
        var ids = Xunit.Assert.IsAssignableFrom<
            IList<
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>>(
                        registry.GetEntityIds<Position>());

        Xunit.Assert.Throws<NotSupportedException>(
            () => ids.Add(CreateEntityId(2)));
    }

    [Xunit.Fact]
    public void EmptyEntityId_IsRejectedByPublicOperations()
    {
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> empty =
                default;
        var registry =
            global::AI.Sandbox.Engine.Core.Components.ComponentRegistry.Empty;
        var entities =
            global::AI.Sandbox.Engine.Core.Entities.EntityRegistry.Empty;

        Xunit.Assert.Throws<ArgumentException>(
            () => registry.Contains<Position>(empty));
        Xunit.Assert.Throws<ArgumentException>(
            () => registry.TryGet<Position>(empty, out _));
        Xunit.Assert.Throws<ArgumentException>(
            () => registry.Set(entities, empty, new Position(1, 1)));
        Xunit.Assert.Throws<ArgumentException>(
            () => registry.Remove<Position>(empty));
        Xunit.Assert.Throws<ArgumentException>(
            () => registry.PurgeEntity(empty));
    }

    [Xunit.Fact]
    public void WorldStateTransition_ComposesEntityAndComponentChangesAtomically()
    {
        var entityId = CreateEntityId(1);
        var entities = ActiveEntities(entityId);
        var initial = new ComponentWorldState(
            entities,
            global::AI.Sandbox.Engine.Core.Components.ComponentRegistry.Empty);
        var manager = global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<ComponentWorldState>.Create(
                CreateWorldId(),
                initial);

        var result = manager.TryApply(
            global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion.Initial,
            1,
            new DestroyEntityTransition(entityId));

        Xunit.Assert.True(result.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Entities
                .EntityLifecycleStatus.Destroyed,
            result.Snapshot.State.Entities.GetLifecycleStatus(entityId));
        Xunit.Assert.Equal(0, result.Snapshot.State.Components.ComponentCount);
        Xunit.Assert.True(
            result.Snapshot.State.Components.IsConsistentWith(
                result.Snapshot.State.Entities));
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Entities.EntityLifecycleStatus.Active,
            initial.Entities.GetLifecycleStatus(entityId));
    }

    private static global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
        ActiveEntities(
            params global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>[] ids)
    {
        return global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
            .FromActiveEntities(ids);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> CreateEntityId(
            int suffix)
    {
        var text = $"019b0000-0000-7000-8200-{suffix:D12}";
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(text);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> CreateWorldId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000000300");
    }

    private sealed class DestroyEntityTransition :
        global::AI.Sandbox.Engine.Core.WorldState
            .IWorldStateTransition<ComponentWorldState>
    {
        private readonly global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> entityId;

        public DestroyEntityTransition(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> entityId)
        {
            this.entityId = entityId;
        }

        public global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateTransitionDecision<ComponentWorldState> Evaluate(
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateSnapshot<ComponentWorldState> current)
        {
            var destroyed =
                current.State.Entities.DestroyEntity(entityId);
            if (!destroyed.WasApplied)
            {
                return global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateTransitionDecision<ComponentWorldState>.Reject(
                        destroyed.Status.ToString());
            }

            var purged =
                current.State.Components.PurgeEntity(entityId);

            return global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateTransitionDecision<ComponentWorldState>.Accept(
                    current.State with
                    {
                        Entities = destroyed.Registry,
                        Components = purged.Registry,
                    });
        }
    }
}
