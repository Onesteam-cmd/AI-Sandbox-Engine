using AI.Sandbox.Engine.Core.Identifiers;

namespace AI.Sandbox.Engine.Core.Entities;

/// <summary>
/// Represents the immutable lifecycle registry for all entity identifiers known
/// to one world.
/// </summary>
/// <remarks>
/// Active entities are a subset of all known entities. Destroying an entity
/// removes it from the active set but permanently retains its identifier in the
/// known set. This prevents identity reuse and preserves historical references
/// from events, memories, saves, and relationships.
/// </remarks>
public sealed class EntityRegistry
{
    private readonly Id<EntityIdKind>[] knownEntityIds;
    private readonly Id<EntityIdKind>[] activeEntityIds;
    private readonly System.Collections.ObjectModel.ReadOnlyCollection<
        Id<EntityIdKind>> knownEntities;
    private readonly System.Collections.ObjectModel.ReadOnlyCollection<
        Id<EntityIdKind>> activeEntities;

    private EntityRegistry(
        Id<EntityIdKind>[] knownEntityIds,
        Id<EntityIdKind>[] activeEntityIds)
    {
        this.knownEntityIds = knownEntityIds;
        this.activeEntityIds = activeEntityIds;
        knownEntities = Array.AsReadOnly(knownEntityIds);
        activeEntities = Array.AsReadOnly(activeEntityIds);
    }

    /// <summary>
    /// Gets the empty registry.
    /// </summary>
    public static EntityRegistry Empty { get; } = new(
        Array.Empty<Id<EntityIdKind>>(),
        Array.Empty<Id<EntityIdKind>>());

    /// <summary>
    /// Gets the number of identifiers ever registered in this world.
    /// </summary>
    public int KnownCount => knownEntityIds.Length;

    /// <summary>
    /// Gets the number of currently active entities.
    /// </summary>
    public int ActiveCount => activeEntityIds.Length;

    /// <summary>
    /// Gets all identifiers ever registered in deterministic sorted order.
    /// </summary>
    public IReadOnlyList<Id<EntityIdKind>> KnownEntities => knownEntities;

    /// <summary>
    /// Gets all currently active identifiers in deterministic sorted order.
    /// </summary>
    public IReadOnlyList<Id<EntityIdKind>> ActiveEntities => activeEntities;

    /// <summary>
    /// Creates an initial registry whose supplied entities are all active.
    /// </summary>
    /// <param name="entityIds">
    /// Unique, non-empty entity identifiers. Enumeration occurs exactly once.
    /// </param>
    /// <returns>
    /// An immutable registry with deterministic sorted identity order.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="entityIds"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when an identifier is empty or appears more than once.
    /// </exception>
    public static EntityRegistry FromActiveEntities(
        IEnumerable<Id<EntityIdKind>> entityIds)
    {
        ArgumentNullException.ThrowIfNull(entityIds);

        var materialized = entityIds.ToArray();

        foreach (var entityId in materialized)
        {
            EnsureValidEntityId(entityId);
        }

        if (materialized.Length == 0)
        {
            return Empty;
        }

        Array.Sort(materialized);

        for (var index = 1; index < materialized.Length; index++)
        {
            if (materialized[index] == materialized[index - 1])
            {
                throw new ArgumentException(
                    "Initial entity identifiers must be unique.",
                    nameof(entityIds));
            }
        }

        return new EntityRegistry(materialized, materialized);
    }

    /// <summary>
    /// Gets the lifecycle status of one entity identifier.
    /// </summary>
    /// <param name="entityId">The non-empty entity identifier to inspect.</param>
    /// <returns>The identifier lifecycle status in this registry.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="entityId"/> is empty.
    /// </exception>
    public EntityLifecycleStatus GetLifecycleStatus(
        Id<EntityIdKind> entityId)
    {
        EnsureValidEntityId(entityId);

        if (Array.BinarySearch(activeEntityIds, entityId) >= 0)
        {
            return EntityLifecycleStatus.Active;
        }

        return Array.BinarySearch(knownEntityIds, entityId) >= 0
            ? EntityLifecycleStatus.Destroyed
            : EntityLifecycleStatus.Unknown;
    }

    /// <summary>
    /// Attempts to register a previously unknown entity as active.
    /// </summary>
    /// <param name="entityId">The non-empty entity identifier to create.</param>
    /// <returns>
    /// A result containing a new registry when created, or this registry when
    /// the identifier was already known.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="entityId"/> is empty.
    /// </exception>
    public EntityMutationResult CreateEntity(Id<EntityIdKind> entityId)
    {
        EnsureValidEntityId(entityId);

        var knownIndex = Array.BinarySearch(knownEntityIds, entityId);
        if (knownIndex >= 0)
        {
            return new EntityMutationResult(
                EntityMutationStatus.AlreadyKnown,
                entityId,
                this);
        }

        var activeIndex = Array.BinarySearch(activeEntityIds, entityId);
        if (activeIndex >= 0)
        {
            throw new InvalidOperationException(
                "An active entity must also be present in the known set.");
        }

        var nextKnown = InsertAt(knownEntityIds, ~knownIndex, entityId);
        var nextActive = InsertAt(activeEntityIds, ~activeIndex, entityId);
        var nextRegistry = new EntityRegistry(nextKnown, nextActive);

        return new EntityMutationResult(
            EntityMutationStatus.Created,
            entityId,
            nextRegistry);
    }

    /// <summary>
    /// Attempts to destroy an active entity while permanently reserving its
    /// identifier.
    /// </summary>
    /// <param name="entityId">The non-empty entity identifier to destroy.</param>
    /// <returns>
    /// A result containing a new registry when destroyed, or this registry when
    /// the identifier is unknown or already destroyed.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="entityId"/> is empty.
    /// </exception>
    public EntityMutationResult DestroyEntity(Id<EntityIdKind> entityId)
    {
        EnsureValidEntityId(entityId);

        var knownIndex = Array.BinarySearch(knownEntityIds, entityId);
        if (knownIndex < 0)
        {
            return new EntityMutationResult(
                EntityMutationStatus.Unknown,
                entityId,
                this);
        }

        var activeIndex = Array.BinarySearch(activeEntityIds, entityId);
        if (activeIndex < 0)
        {
            return new EntityMutationResult(
                EntityMutationStatus.AlreadyDestroyed,
                entityId,
                this);
        }

        var nextActive = RemoveAt(activeEntityIds, activeIndex);
        var nextRegistry = new EntityRegistry(knownEntityIds, nextActive);

        return new EntityMutationResult(
            EntityMutationStatus.Destroyed,
            entityId,
            nextRegistry);
    }

    private static void EnsureValidEntityId(Id<EntityIdKind> entityId)
    {
        if (entityId.IsEmpty)
        {
            throw new ArgumentException(
                "An entity identifier cannot be empty.",
                nameof(entityId));
        }
    }

    private static Id<EntityIdKind>[] InsertAt(
        Id<EntityIdKind>[] source,
        int index,
        Id<EntityIdKind> value)
    {
        var result = new Id<EntityIdKind>[source.Length + 1];

        Array.Copy(source, 0, result, 0, index);
        result[index] = value;
        Array.Copy(
            source,
            index,
            result,
            index + 1,
            source.Length - index);

        return result;
    }

    private static Id<EntityIdKind>[] RemoveAt(
        Id<EntityIdKind>[] source,
        int index)
    {
        var result = new Id<EntityIdKind>[source.Length - 1];

        Array.Copy(source, 0, result, 0, index);
        Array.Copy(
            source,
            index + 1,
            result,
            index,
            source.Length - index - 1);

        return result;
    }
}
