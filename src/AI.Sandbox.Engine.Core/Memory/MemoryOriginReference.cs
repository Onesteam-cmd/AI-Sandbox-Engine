using AI.Sandbox.Engine.Core.Entities;
using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.Knowledge;
using AI.Sandbox.Engine.Core.Perception;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Memory;

/// <summary>
/// Provides this memory-model API member.
/// </summary>
public sealed record MemoryOriginReference
{
    private MemoryOriginReference(
        Id<MemoryOriginIdKind> originId,
        MemoryOriginKind kind,
        Id<EntityIdKind> ownerEntityId,
        Id<WorldIdKind> worldId,
        WorldStateVersion worldStateVersion,
        ulong simulationTick,
        Id<EntityIdKind>? sourceEntityId,
        Id<KnowledgeClaimIdKind>? knowledgeClaimId,
        Id<KnowledgeEvidenceIdKind>? knowledgeEvidenceId,
        Id<PerceptionStimulusIdKind>? perceptionStimulusId,
        Id<PerceptionChannelIdKind>? perceptionChannelId)
    {
        OriginId = originId;
        Kind = kind;
        OwnerEntityId = ownerEntityId;
        WorldId = worldId;
        WorldStateVersion = worldStateVersion;
        SimulationTick = simulationTick;
        SourceEntityId = sourceEntityId;
        KnowledgeClaimId = knowledgeClaimId;
        KnowledgeEvidenceId = knowledgeEvidenceId;
        PerceptionStimulusId = perceptionStimulusId;
        PerceptionChannelId = perceptionChannelId;
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public Id<MemoryOriginIdKind> OriginId { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemoryOriginKind Kind { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public Id<EntityIdKind> OwnerEntityId { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public Id<WorldIdKind> WorldId { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public WorldStateVersion WorldStateVersion { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public ulong SimulationTick { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public Id<EntityIdKind>? SourceEntityId { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public Id<KnowledgeClaimIdKind>? KnowledgeClaimId { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public Id<KnowledgeEvidenceIdKind>? KnowledgeEvidenceId { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public Id<PerceptionStimulusIdKind>? PerceptionStimulusId { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public Id<PerceptionChannelIdKind>? PerceptionChannelId { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public bool HasKnowledgeProvenance =>
        KnowledgeClaimId.HasValue &&
        KnowledgeEvidenceId.HasValue;

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public bool HasPerceptionProvenance =>
        PerceptionStimulusId.HasValue &&
        PerceptionChannelId.HasValue;

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public static MemoryOriginReference Create(
        Id<MemoryOriginIdKind> originId,
        MemoryOriginKind kind,
        Id<EntityIdKind> ownerEntityId,
        Id<WorldIdKind> worldId,
        WorldStateVersion worldStateVersion,
        ulong simulationTick,
        Id<EntityIdKind>? sourceEntityId = null)
    {
        if (kind is MemoryOriginKind.Perception or
            MemoryOriginKind.Knowledge)
        {
            throw new ArgumentException(
                "Perception and knowledge origins require their typed factories.",
                nameof(kind));
        }

        return Restore(
            originId,
            kind,
            ownerEntityId,
            worldId,
            worldStateVersion,
            simulationTick,
            sourceEntityId,
            null,
            null,
            null,
            null);
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public static MemoryOriginReference FromPerception<TSignal>(
        Id<MemoryOriginIdKind> originId,
        PerceptionObservation<TSignal> observation)
        where TSignal : notnull, IPerceptionSignal
    {
        ArgumentNullException.ThrowIfNull(observation);

        return Restore(
            originId,
            MemoryOriginKind.Perception,
            observation.ObserverEntityId,
            observation.WorldId,
            observation.WorldStateVersion,
            observation.SimulationTick,
            null,
            null,
            null,
            observation.StimulusId,
            observation.ChannelId);
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public static MemoryOriginReference FromKnowledge<TClaim>(
        Id<MemoryOriginIdKind> originId,
        KnowledgeEntry<TClaim> entry)
        where TClaim : notnull, IKnowledgeClaim
    {
        ArgumentNullException.ThrowIfNull(entry);

        return Restore(
            originId,
            MemoryOriginKind.Knowledge,
            entry.Evidence.RecipientEntityId,
            entry.Evidence.WorldId,
            entry.LastRevisedWorldStateVersion,
            entry.LastRevisedSimulationTick,
            entry.Evidence.SourceEntityId,
            entry.ClaimId,
            entry.Evidence.EvidenceId,
            entry.Evidence.PerceptionStimulusId,
            entry.Evidence.PerceptionChannelId);
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public static MemoryOriginReference Restore(
        Id<MemoryOriginIdKind> originId,
        MemoryOriginKind kind,
        Id<EntityIdKind> ownerEntityId,
        Id<WorldIdKind> worldId,
        WorldStateVersion worldStateVersion,
        ulong simulationTick,
        Id<EntityIdKind>? sourceEntityId,
        Id<KnowledgeClaimIdKind>? knowledgeClaimId,
        Id<KnowledgeEvidenceIdKind>? knowledgeEvidenceId,
        Id<PerceptionStimulusIdKind>? perceptionStimulusId,
        Id<PerceptionChannelIdKind>? perceptionChannelId)
    {
        EnsureNonEmpty(originId, nameof(originId));
        EnsureNonEmpty(ownerEntityId, nameof(ownerEntityId));
        EnsureNonEmpty(worldId, nameof(worldId));

        if (!Enum.IsDefined(kind))
        {
            throw new ArgumentOutOfRangeException(
                nameof(kind),
                kind,
                "Unknown memory origin kind.");
        }

        EnsureOptionalNonEmpty(sourceEntityId, nameof(sourceEntityId));
        EnsureOptionalNonEmpty(knowledgeClaimId, nameof(knowledgeClaimId));
        EnsureOptionalNonEmpty(knowledgeEvidenceId, nameof(knowledgeEvidenceId));
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
                "Knowledge claim and evidence provenance must be present together.",
                nameof(knowledgeClaimId));
        }

        var hasStimulus = perceptionStimulusId.HasValue;
        var hasChannel = perceptionChannelId.HasValue;
        if (hasStimulus != hasChannel)
        {
            throw new ArgumentException(
                "Perception stimulus and channel provenance must be present together.",
                nameof(perceptionStimulusId));
        }

        switch (kind)
        {
            case MemoryOriginKind.Perception:
                if (!hasStimulus ||
                    hasKnowledgeClaim ||
                    sourceEntityId.HasValue)
                {
                    throw new ArgumentException(
                        "Perception origin requires only stimulus and channel provenance.",
                        nameof(kind));
                }

                break;

            case MemoryOriginKind.Knowledge:
                if (!hasKnowledgeClaim)
                {
                    throw new ArgumentException(
                        "Knowledge origin requires claim and evidence provenance.",
                        nameof(kind));
                }

                break;

            case MemoryOriginKind.Communication:
                if (!sourceEntityId.HasValue ||
                    hasKnowledgeClaim ||
                    hasStimulus)
                {
                    throw new ArgumentException(
                        "Communication origin requires only a source entity.",
                        nameof(kind));
                }

                break;

            case MemoryOriginKind.External:
                if (hasKnowledgeClaim || hasStimulus)
                {
                    throw new ArgumentException(
                        "External origin cannot contain knowledge or perception provenance.",
                        nameof(kind));
                }

                break;
        }

        return new MemoryOriginReference(
            originId,
            kind,
            ownerEntityId,
            worldId,
            worldStateVersion,
            simulationTick,
            sourceEntityId,
            knowledgeClaimId,
            knowledgeEvidenceId,
            perceptionStimulusId,
            perceptionChannelId);
    }

    private static void EnsureNonEmpty<TKind>(
        Id<TKind> id,
        string parameterName)
        where TKind : struct
    {
        if (id.IsEmpty)
        {
            throw new ArgumentException(
                "Memory origin IDs cannot be empty.",
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
                "A present memory origin ID cannot be empty.",
                parameterName);
        }
    }
}
