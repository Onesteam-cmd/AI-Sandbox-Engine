namespace AI.Sandbox.Engine.Core.Tests;

public sealed class EntitySystemTests
{
    private sealed record EntityWorldState(
        global::AI.Sandbox.Engine.Core.Entities.EntityRegistry Entities) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    [Xunit.Fact]
    public void Empty_HasNoKnownOrActiveEntities()
    {
        var registry =
            global::AI.Sandbox.Engine.Core.Entities.EntityRegistry.Empty;

        Xunit.Assert.Equal(0, registry.KnownCount);
        Xunit.Assert.Equal(0, registry.ActiveCount);
        Xunit.Assert.Empty(registry.KnownEntities);
        Xunit.Assert.Empty(registry.ActiveEntities);
    }

    [Xunit.Fact]
    public void FromActiveEntities_SortsDeterministically()
    {
        var higher = CreateEntityId(2);
        var lower = CreateEntityId(1);

        var registry = global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
            .FromActiveEntities(new[] { higher, lower });

        Xunit.Assert.Equal(new[] { lower, higher }, registry.KnownEntities);
        Xunit.Assert.Equal(new[] { lower, higher }, registry.ActiveEntities);
    }

    [Xunit.Fact]
    public void FromActiveEntities_EnumeratesInputExactlyOnce()
    {
        var enumerable = new SingleUseEnumerable(
            new[] { CreateEntityId(1), CreateEntityId(2) });

        var registry = global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
            .FromActiveEntities(enumerable);

        Xunit.Assert.Equal(2, registry.ActiveCount);
        Xunit.Assert.Equal(1, enumerable.EnumerationCount);
    }

    [Xunit.Fact]
    public void FromActiveEntities_RejectsDuplicateIdentifiers()
    {
        var entityId = CreateEntityId(1);

        var exception = Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
                .FromActiveEntities(new[] { entityId, entityId }));

        Xunit.Assert.Equal("entityIds", exception.ParamName);
    }

    [Xunit.Fact]
    public void FromActiveEntities_RejectsEmptyIdentifier()
    {
        var exception = Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
                .FromActiveEntities(
                    new[]
                    {
                        default(
                            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                                global::AI.Sandbox.Engine.Core.Entities
                                    .EntityIdKind>),
                    }));

        Xunit.Assert.Equal("entityId", exception.ParamName);
    }

    [Xunit.Fact]
    public void FromActiveEntities_EmptyInputReturnsSharedEmptyRegistry()
    {
        var registry = global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
            .FromActiveEntities(
                Array.Empty<
                    global::AI.Sandbox.Engine.Core.Identifiers.Id<
                        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>());

        Xunit.Assert.Same(
            global::AI.Sandbox.Engine.Core.Entities.EntityRegistry.Empty,
            registry);
    }

    [Xunit.Fact]
    public void GetLifecycleStatus_DistinguishesUnknownAndActive()
    {
        var active = CreateEntityId(1);
        var unknown = CreateEntityId(2);
        var registry = global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
            .FromActiveEntities(new[] { active });

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Entities.EntityLifecycleStatus.Active,
            registry.GetLifecycleStatus(active));
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Entities.EntityLifecycleStatus.Unknown,
            registry.GetLifecycleStatus(unknown));
    }

    [Xunit.Fact]
    public void CreateEntity_AddsPreviouslyUnknownIdentity()
    {
        var original =
            global::AI.Sandbox.Engine.Core.Entities.EntityRegistry.Empty;
        var entityId = CreateEntityId(1);

        var result = original.CreateEntity(entityId);

        Xunit.Assert.True(result.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Entities.EntityMutationStatus.Created,
            result.Status);
        Xunit.Assert.Equal(entityId, result.EntityId);
        Xunit.Assert.NotSame(original, result.Registry);
        Xunit.Assert.Equal(1, result.Registry.KnownCount);
        Xunit.Assert.Equal(1, result.Registry.ActiveCount);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Entities.EntityLifecycleStatus.Active,
            result.Registry.GetLifecycleStatus(entityId));
        Xunit.Assert.Equal(0, original.KnownCount);
    }

    [Xunit.Fact]
    public void CreateEntity_AlreadyKnownReturnsOriginalRegistry()
    {
        var entityId = CreateEntityId(1);
        var registry = global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
            .FromActiveEntities(new[] { entityId });

        var result = registry.CreateEntity(entityId);

        Xunit.Assert.False(result.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Entities
                .EntityMutationStatus.AlreadyKnown,
            result.Status);
        Xunit.Assert.Same(registry, result.Registry);
    }

    [Xunit.Fact]
    public void DestroyEntity_PreservesKnownIdentityAsTombstone()
    {
        var entityId = CreateEntityId(1);
        var registry = global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
            .FromActiveEntities(new[] { entityId });

        var result = registry.DestroyEntity(entityId);

        Xunit.Assert.True(result.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Entities
                .EntityMutationStatus.Destroyed,
            result.Status);
        Xunit.Assert.Equal(1, result.Registry.KnownCount);
        Xunit.Assert.Equal(0, result.Registry.ActiveCount);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Entities
                .EntityLifecycleStatus.Destroyed,
            result.Registry.GetLifecycleStatus(entityId));
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Entities.EntityLifecycleStatus.Active,
            registry.GetLifecycleStatus(entityId));
    }

    [Xunit.Fact]
    public void DestroyEntity_UnknownReturnsOriginalRegistry()
    {
        var registry =
            global::AI.Sandbox.Engine.Core.Entities.EntityRegistry.Empty;
        var entityId = CreateEntityId(1);

        var result = registry.DestroyEntity(entityId);

        Xunit.Assert.False(result.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Entities.EntityMutationStatus.Unknown,
            result.Status);
        Xunit.Assert.Same(registry, result.Registry);
    }

    [Xunit.Fact]
    public void DestroyEntity_AlreadyDestroyedReturnsOriginalRegistry()
    {
        var entityId = CreateEntityId(1);
        var active = global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
            .FromActiveEntities(new[] { entityId });
        var destroyed = active.DestroyEntity(entityId).Registry;

        var result = destroyed.DestroyEntity(entityId);

        Xunit.Assert.False(result.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Entities
                .EntityMutationStatus.AlreadyDestroyed,
            result.Status);
        Xunit.Assert.Same(destroyed, result.Registry);
    }

    [Xunit.Fact]
    public void DestroyedIdentifier_CannotBeCreatedAgain()
    {
        var entityId = CreateEntityId(1);
        var active = global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
            .FromActiveEntities(new[] { entityId });
        var destroyed = active.DestroyEntity(entityId).Registry;

        var result = destroyed.CreateEntity(entityId);

        Xunit.Assert.False(result.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Entities
                .EntityMutationStatus.AlreadyKnown,
            result.Status);
        Xunit.Assert.Same(destroyed, result.Registry);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Entities
                .EntityLifecycleStatus.Destroyed,
            destroyed.GetLifecycleStatus(entityId));
    }

    [Xunit.Fact]
    public void ReadOnlyViews_CannotMutateRegistryArrays()
    {
        var entityId = CreateEntityId(1);
        var registry = global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
            .FromActiveEntities(new[] { entityId });
        var list = Xunit.Assert.IsAssignableFrom<
            IList<
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>>(
                        registry.ActiveEntities);

        Xunit.Assert.Throws<NotSupportedException>(
            () => list.Add(CreateEntityId(2)));
        Xunit.Assert.Equal(1, registry.ActiveCount);
    }

    [Xunit.Fact]
    public void EmptyIdentifier_IsRejectedByLifecycleOperations()
    {
        var registry =
            global::AI.Sandbox.Engine.Core.Entities.EntityRegistry.Empty;
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> empty =
                default;

        Xunit.Assert.Throws<ArgumentException>(
            () => registry.GetLifecycleStatus(empty));
        Xunit.Assert.Throws<ArgumentException>(
            () => registry.CreateEntity(empty));
        Xunit.Assert.Throws<ArgumentException>(
            () => registry.DestroyEntity(empty));
    }

    [Xunit.Fact]
    public void WorldStateTransition_ComposesEntityRegistryWithoutHiddenWrites()
    {
        var initialRegistry =
            global::AI.Sandbox.Engine.Core.Entities.EntityRegistry.Empty;
        var initialState = new EntityWorldState(initialRegistry);
        var manager = global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<EntityWorldState>.Create(
                CreateWorldId(),
                initialState);
        var entityId = CreateEntityId(1);

        var result = manager.TryApply(
            global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion.Initial,
            1,
            new CreateEntityTransition(entityId));

        Xunit.Assert.True(result.WasApplied);
        Xunit.Assert.Equal(1, result.Snapshot.State.Entities.ActiveCount);
        Xunit.Assert.Equal(0, initialRegistry.ActiveCount);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Entities.EntityLifecycleStatus.Active,
            result.Snapshot.State.Entities.GetLifecycleStatus(entityId));
    }

    [Xunit.Fact]
    public void InitialBatchFactory_SupportsLargeWorldGenerationWithoutRepeatedAdds()
    {
        var entityIds = Enumerable.Range(1, 1_000)
            .Select(CreateEntityId)
            .Reverse()
            .ToArray();

        var registry = global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
            .FromActiveEntities(entityIds);

        Xunit.Assert.Equal(1_000, registry.KnownCount);
        Xunit.Assert.Equal(1_000, registry.ActiveCount);
        Xunit.Assert.Equal(CreateEntityId(1), registry.ActiveEntities[0]);
        Xunit.Assert.Equal(CreateEntityId(1_000), registry.ActiveEntities[^1]);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> CreateEntityId(
            int suffix)
    {
        var text = $"019b0000-0000-7000-8100-{suffix:D12}";
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(text);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> CreateWorldId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000000200");
    }

    private sealed class CreateEntityTransition :
        global::AI.Sandbox.Engine.Core.WorldState
            .IWorldStateTransition<EntityWorldState>
    {
        private readonly global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> entityId;

        public CreateEntityTransition(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> entityId)
        {
            this.entityId = entityId;
        }

        public global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateTransitionDecision<EntityWorldState> Evaluate(
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateSnapshot<EntityWorldState> current)
        {
            var mutation = current.State.Entities.CreateEntity(entityId);

            return mutation.WasApplied
                ? global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateTransitionDecision<EntityWorldState>.Accept(
                        current.State with
                        {
                            Entities = mutation.Registry,
                        })
                : global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateTransitionDecision<EntityWorldState>.Reject(
                        mutation.Status.ToString());
        }
    }

    private sealed class SingleUseEnumerable :
        IEnumerable<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>
    {
        private readonly IReadOnlyList<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>> values;

        public SingleUseEnumerable(
            IReadOnlyList<
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>
                values)
        {
            this.values = values;
        }

        public int EnumerationCount { get; private set; }

        public IEnumerator<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>
            GetEnumerator()
        {
            EnumerationCount++;
            if (EnumerationCount > 1)
            {
                throw new InvalidOperationException(
                    "The input sequence was enumerated more than once.");
            }

            return values.GetEnumerator();
        }

        System.Collections.IEnumerator
            System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
