using AI.Sandbox.Engine.Core.Entities;
using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.Knowledge;
using AI.Sandbox.Engine.Core.Memory;
using AI.Sandbox.Engine.Core.Perception;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Relationships;

/// <summary>
/// Records the latest explicit provenance used to change one directed
/// relationship.
/// </summary>
/// <remarks>
/// This reference stores compact causal provenance, not a narrative history.
/// Detailed episodes remain in events or memory.
/// </remarks>
public sealed record RelationshipChangeReference
{
    private RelationshipChangeReference(
        Id<RelationshipChangeIdKind> changeId,
        RelationshipChangeKind kind,
        Id<EntityIdKind> ownerEntityId,
        Id<EntityIdKind> targetEntityId,
        Id<WorldIdKind> worldId,
        WorldStateVersion worldStateVersion,
        ulong simulationTick,
        Id<EntityIdKind>? sourceEntityId,
        Id<KnowledgeClaimIdKind>? knowledgeClaimId,
        Id<KnowledgeEvidenceIdKind>? knowledgeEvidenceId,
        Id<MemoryIdKind>? memoryId,
        Id<MemoryOriginIdKind>? memoryOriginId,
        Id<PerceptionStimulusIdKind>? perceptionStimulusId,
        Id<PerceptionChannelIdKind>? perceptionChannelId)
    {
        ChangeId = changeId;
        Kind = kind;
        OwnerEntityId = ownerEntityId;
        TargetEntityId = targetEntityId;
        WorldId = worldId;
        WorldStateVersion = worldStateVersion;
        SimulationTick = simulationTick;
        SourceEntityId = sourceEntityId;
        KnowledgeClaimId = knowledgeClaimId;
        KnowledgeEvidenceId = knowledgeEvidenceId;
        MemoryId = memoryId;
        MemoryOriginId = memoryOriginId;
        PerceptionStimulusId = perceptionStimulusId;
        PerceptionChannelId = perceptionChannelId;
    }

    /// <summary>
    /// Gets the externally assigned identity of this change reference.
    /// </summary>
    public Id<RelationshipChangeIdKind> ChangeId { get; }

    /// <summary>
    /// Gets the broad provenance category.
    /// </summary>
    public RelationshipChangeKind Kind { get; }

    /// <summary>
    /// Gets the entity whose subjective relationship changed.
    /// </summary>
    public Id<EntityIdKind> OwnerEntityId { get; }

    /// <summary>
    /// Gets the entity toward which the relationship is directed.
    /// </summary>
    public Id<EntityIdKind> TargetEntityId { get; }

    /// <summary>
    /// Gets the world in which the change was evaluated.
    /// </summary>
    public Id<WorldIdKind> WorldId { get; }

    /// <summary>
    /// Gets the authoritative version observed for the change.
    /// </summary>
    public WorldStateVersion WorldStateVersion { get; }

    /// <summary>
    /// Gets the logical simulation tick observed for the change.
    /// </summary>
    public ulong SimulationTick { get; }

    /// <summary>
    /// Gets the optional entity that supplied or caused the information.
    /// </summary>
    public Id<EntityIdKind>? SourceEntityId { get; }

    /// <summary>
    /// Gets the optional current-knowledge claim identity.
    /// </summary>
    public Id<KnowledgeClaimIdKind>? KnowledgeClaimId { get; }

    /// <summary>
    /// Gets the optional current-knowledge evidence identity.
    /// </summary>
    public Id<KnowledgeEvidenceIdKind>? KnowledgeEvidenceId { get; }

    /// <summary>
    /// Gets the optional retained-memory identity.
    /// </summary>
    public Id<MemoryIdKind>? MemoryId { get; }

    /// <summary>
    /// Gets the optional retained-memory origin identity.
    /// </summary>
    public Id<MemoryOriginIdKind>? MemoryOriginId { get; }

    /// <summary>
    /// Gets the optional perception stimulus identity.
    /// </summary>
    public Id<PerceptionStimulusIdKind>? PerceptionStimulusId { get; }

    /// <summary>
    /// Gets the optional perception channel identity.
    /// </summary>
    public Id<PerceptionChannelIdKind>? PerceptionChannelId { get; }

    /// <summary>
    /// Gets a value indicating whether current-knowledge provenance is present.
    /// </summary>
    public bool HasKnowledgeProvenance =>
        KnowledgeClaimId.HasValue &&
        KnowledgeEvidenceId.HasValue;

    /// <summary>
    /// Gets a value indicating whether retained-memory provenance is present.
    /// </summary>
    public bool HasMemoryProvenance =>
        MemoryId.HasValue &&
        MemoryOriginId.HasValue;

    /// <summary>
    /// Gets a value indicating whether perception provenance is present.
    /// </summary>
    public bool HasPerceptionProvenance =>
        PerceptionStimulusId.HasValue &&
        PerceptionChannelId.HasValue;

    /// <summary>
    /// Creates interaction, communication, inference, or external provenance.
    /// </summary>
    /// <param name="changeId">The externally assigned change identity.</param>
    /// <param name="kind">The broad non-typed provenance category.</param>
    /// <param name="ownerEntityId">The relationship owner.</param>
    /// <param name="targetEntityId">The directed relationship target.</param>
    /// <param name="worldId">The world identity.</param>
    /// <param name="worldStateVersion">The observed World State version.</param>
    /// <param name="simulationTick">The observed logical tick.</param>
    /// <param name="sourceEntityId">An optional source entity.</param>
    /// <returns>The validated immutable change reference.</returns>
    public static RelationshipChangeReference Create(
        Id<RelationshipChangeIdKind> changeId,
        RelationshipChangeKind kind,
        Id<EntityIdKind> ownerEntityId,
        Id<EntityIdKind> targetEntityId,
        Id<WorldIdKind> worldId,
        WorldStateVersion worldStateVersion,
        ulong simulationTick,
        Id<EntityIdKind>? sourceEntityId = null)
    {
        if (kind is RelationshipChangeKind.Perception or
            RelationshipChangeKind.Knowledge or
            RelationshipChangeKind.Memory)
        {
            throw new ArgumentException(
                "Typed perception, knowledge, and memory changes require " +
                "their dedicated factories.",
                nameof(kind));
        }

        return Restore(
            changeId,
            kind,
            ownerEntityId,
            targetEntityId,
            worldId,
            worldStateVersion,
            simulationTick,
            sourceEntityId,
            null,
            null,
            null,
            null,
            null,
            null);
    }

    /// <summary>
    /// Creates relationship-change provenance from a perception observation.
    /// </summary>
    /// <typeparam name="TSignal">The exact perception signal type.</typeparam>
    /// <param name="changeId">The externally assigned change identity.</param>
    /// <param name="targetEntityId">The directed relationship target.</param>
    /// <param name="observation">The source observation.</param>
    /// <returns>The validated immutable change reference.</returns>
    public static RelationshipChangeReference FromPerception<TSignal>(
        Id<RelationshipChangeIdKind> changeId,
        Id<EntityIdKind> targetEntityId,
        PerceptionObservation<TSignal> observation)
        where TSignal : notnull, IPerceptionSignal
    {
        ArgumentNullException.ThrowIfNull(observation);

        return Restore(
            changeId,
            RelationshipChangeKind.Perception,
            observation.ObserverEntityId,
            targetEntityId,
            observation.WorldId,
            observation.WorldStateVersion,
            observation.SimulationTick,
            null,
            null,
            null,
            null,
            null,
            observation.StimulusId,
            observation.ChannelId);
    }

    /// <summary>
    /// Creates relationship-change provenance from current subjective
    /// knowledge.
    /// </summary>
    /// <typeparam name="TClaim">The exact knowledge claim type.</typeparam>
    /// <param name="changeId">The externally assigned change identity.</param>
    /// <param name="targetEntityId">The directed relationship target.</param>
    /// <param name="entry">The source knowledge entry.</param>
    /// <returns>The validated immutable change reference.</returns>
    public static RelationshipChangeReference FromKnowledge<TClaim>(
        Id<RelationshipChangeIdKind> changeId,
        Id<EntityIdKind> targetEntityId,
        KnowledgeEntry<TClaim> entry)
        where TClaim : notnull, IKnowledgeClaim
    {
        ArgumentNullException.ThrowIfNull(entry);

        return Restore(
            changeId,
            RelationshipChangeKind.Knowledge,
            entry.Evidence.RecipientEntityId,
            targetEntityId,
            entry.Evidence.WorldId,
            entry.LastRevisedWorldStateVersion,
            entry.LastRevisedSimulationTick,
            entry.Evidence.SourceEntityId,
            entry.ClaimId,
            entry.Evidence.EvidenceId,
            null,
            null,
            entry.Evidence.PerceptionStimulusId,
            entry.Evidence.PerceptionChannelId);
    }

    /// <summary>
    /// Creates relationship-change provenance from a retained memory episode.
    /// </summary>
    /// <typeparam name="TContent">The exact memory-content type.</typeparam>
    /// <param name="changeId">The externally assigned change identity.</param>
    /// <param name="targetEntityId">The directed relationship target.</param>
    /// <param name="entry">The source memory entry.</param>
    /// <returns>The validated immutable change reference.</returns>
    public static RelationshipChangeReference FromMemory<TContent>(
        Id<RelationshipChangeIdKind> changeId,
        Id<EntityIdKind> targetEntityId,
        MemoryEntry<TContent> entry)
        where TContent : notnull, IMemoryContent
    {
        ArgumentNullException.ThrowIfNull(entry);

        return Restore(
            changeId,
            RelationshipChangeKind.Memory,
            entry.Origin.OwnerEntityId,
            targetEntityId,
            entry.Origin.WorldId,
            entry.LastUpdatedWorldStateVersion,
            entry.LastUpdatedSimulationTick,
            entry.Origin.SourceEntityId,
            entry.Origin.KnowledgeClaimId,
            entry.Origin.KnowledgeEvidenceId,
            entry.MemoryId,
            entry.Origin.OriginId,
            entry.Origin.PerceptionStimulusId,
            entry.Origin.PerceptionChannelId);
    }

    /// <summary>
    /// Restores a validated relationship-change reference from persistence.
    /// </summary>
    /// <param name="changeId">The externally assigned change identity.</param>
    /// <param name="kind">The broad provenance category.</param>
    /// <param name="ownerEntityId">The relationship owner.</param>
    /// <param name="targetEntityId">The directed relationship target.</param>
    /// <param name="worldId">The world identity.</param>
    /// <param name="worldStateVersion">The observed World State version.</param>
    /// <param name="simulationTick">The observed logical tick.</param>
    /// <param name="sourceEntityId">An optional source entity.</param>
    /// <param name="knowledgeClaimId">An optional knowledge claim identity.</param>
    /// <param name="knowledgeEvidenceId">An optional knowledge evidence identity.</param>
    /// <param name="memoryId">An optional memory identity.</param>
    /// <param name="memoryOriginId">An optional memory-origin identity.</param>
    /// <param name="perceptionStimulusId">An optional stimulus identity.</param>
    /// <param name="perceptionChannelId">An optional channel identity.</param>
    /// <returns>The validated immutable change reference.</returns>
    public static RelationshipChangeReference Restore(
        Id<RelationshipChangeIdKind> changeId,
        RelationshipChangeKind kind,
        Id<EntityIdKind> ownerEntityId,
        Id<EntityIdKind> targetEntityId,
        Id<WorldIdKind> worldId,
        WorldStateVersion worldStateVersion,
        ulong simulationTick,
        Id<EntityIdKind>? sourceEntityId,
        Id<KnowledgeClaimIdKind>? knowledgeClaimId,
        Id<KnowledgeEvidenceIdKind>? knowledgeEvidenceId,
        Id<MemoryIdKind>? memoryId,
        Id<MemoryOriginIdKind>? memoryOriginId,
        Id<PerceptionStimulusIdKind>? perceptionStimulusId,
        Id<PerceptionChannelIdKind>? perceptionChannelId)
    {
        EnsureNonEmpty(changeId, nameof(changeId));
        EnsureNonEmpty(ownerEntityId, nameof(ownerEntityId));
        EnsureNonEmpty(targetEntityId, nameof(targetEntityId));
        EnsureNonEmpty(worldId, nameof(worldId));

        if (ownerEntityId == targetEntityId)
        {
            throw new ArgumentException(
                "A directed relationship target must differ from its owner.",
                nameof(targetEntityId));
        }

        if (!Enum.IsDefined(kind))
        {
            throw new ArgumentOutOfRangeException(
                nameof(kind),
                kind,
                "Unknown relationship-change kind.");
        }

        EnsureOptionalNonEmpty(sourceEntityId, nameof(sourceEntityId));
        EnsureOptionalNonEmpty(knowledgeClaimId, nameof(knowledgeClaimId));
        EnsureOptionalNonEmpty(
            knowledgeEvidenceId,
            nameof(knowledgeEvidenceId));
        EnsureOptionalNonEmpty(memoryId, nameof(memoryId));
        EnsureOptionalNonEmpty(memoryOriginId, nameof(memoryOriginId));
        EnsureOptionalNonEmpty(
            perceptionStimulusId,
            nameof(perceptionStimulusId));
        EnsureOptionalNonEmpty(
            perceptionChannelId,
            nameof(perceptionChannelId));

        var hasKnowledgeClaim = knowledgeClaimId.HasValue;
        var hasKnowledgeEvidence = knowledgeEvidenceId.HasValue;
        if (hasKnowledgeClaim != hasKnowledgeEvidence)
        {
            throw new ArgumentException(
                "Knowledge claim and evidence provenance must be present " +
                "together.",
                nameof(knowledgeClaimId));
        }

        var hasMemory = memoryId.HasValue;
        var hasMemoryOrigin = memoryOriginId.HasValue;
        if (hasMemory != hasMemoryOrigin)
        {
            throw new ArgumentException(
                "Memory and memory-origin provenance must be present together.",
                nameof(memoryId));
        }

        var hasStimulus = perceptionStimulusId.HasValue;
        var hasChannel = perceptionChannelId.HasValue;
        if (hasStimulus != hasChannel)
        {
            throw new ArgumentException(
                "Perception stimulus and channel provenance must be present " +
                "together.",
                nameof(perceptionStimulusId));
        }

        switch (kind)
        {
            case RelationshipChangeKind.Interaction:
            case RelationshipChangeKind.Inference:
            case RelationshipChangeKind.External:
                if (hasKnowledgeClaim || hasMemory || hasStimulus)
                {
                    throw CreateProvenanceException(kind);
                }

                break;

            case RelationshipChangeKind.Communication:
                if (!sourceEntityId.HasValue ||
                    hasKnowledgeClaim ||
                    hasMemory ||
                    hasStimulus)
                {
                    throw CreateProvenanceException(kind);
                }

                break;

            case RelationshipChangeKind.Perception:
                if (!hasStimulus ||
                    sourceEntityId.HasValue ||
                    hasKnowledgeClaim ||
                    hasMemory)
                {
                    throw CreateProvenanceException(kind);
                }

                break;

            case RelationshipChangeKind.Knowledge:
                if (!hasKnowledgeClaim || hasMemory)
                {
                    throw CreateProvenanceException(kind);
                }

                break;

            case RelationshipChangeKind.Memory:
                if (!hasMemory)
                {
                    throw CreateProvenanceException(kind);
                }

                break;
        }

        return new RelationshipChangeReference(
            changeId,
            kind,
            ownerEntityId,
            targetEntityId,
            worldId,
            worldStateVersion,
            simulationTick,
            sourceEntityId,
            knowledgeClaimId,
            knowledgeEvidenceId,
            memoryId,
            memoryOriginId,
            perceptionStimulusId,
            perceptionChannelId);
    }

    private static ArgumentException CreateProvenanceException(
        RelationshipChangeKind kind)
    {
        return new ArgumentException(
            $"Relationship-change kind '{kind}' has inconsistent provenance.",
            nameof(kind));
    }

    private static void EnsureNonEmpty<TKind>(
        Id<TKind> id,
        string parameterName)
        where TKind : struct
    {
        if (id.IsEmpty)
        {
            throw new ArgumentException(
                "Relationship-change identifiers cannot be empty.",
                parameterName);
        }
    }

    private static void EnsureOptionalNonEmpty<TKind>(
        Id<TKind>? id,
        string parameterName)
        where TKind : struct
    {
        if (id is { } value && value.IsEmpty)
        {
            throw new ArgumentException(
                "A present relationship-change identifier cannot be empty.",
                parameterName);
        }
    }
}
