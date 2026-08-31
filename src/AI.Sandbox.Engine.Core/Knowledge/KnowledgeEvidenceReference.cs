using AI.Sandbox.Engine.Core.Entities;
using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.Perception;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Knowledge;

/// <summary>
/// Describes the current revision's explicit evidence provenance.
/// </summary>
/// <remarks>
/// This reference is not a memory history. A knowledge entry retains only its
/// current evidence; episodic retention belongs to the later Memory Model.
/// </remarks>
public sealed record KnowledgeEvidenceReference
{
    private KnowledgeEvidenceReference(
        Id<KnowledgeEvidenceIdKind> evidenceId,
        KnowledgeEvidenceKind kind,
        Id<EntityIdKind> recipientEntityId,
        Id<WorldIdKind> worldId,
        WorldStateVersion worldStateVersion,
        ulong simulationTick,
        Id<EntityIdKind>? sourceEntityId,
        Id<PerceptionStimulusIdKind>? perceptionStimulusId,
        Id<PerceptionChannelIdKind>? perceptionChannelId)
    {
        EvidenceId = evidenceId;
        Kind = kind;
        RecipientEntityId = recipientEntityId;
        WorldId = worldId;
        WorldStateVersion = worldStateVersion;
        SimulationTick = simulationTick;
        SourceEntityId = sourceEntityId;
        PerceptionStimulusId = perceptionStimulusId;
        PerceptionChannelId = perceptionChannelId;
    }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public Id<KnowledgeEvidenceIdKind> EvidenceId { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public KnowledgeEvidenceKind Kind { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public Id<EntityIdKind> RecipientEntityId { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public Id<WorldIdKind> WorldId { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public WorldStateVersion WorldStateVersion { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public ulong SimulationTick { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public Id<EntityIdKind>? SourceEntityId { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public Id<PerceptionStimulusIdKind>? PerceptionStimulusId { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public Id<PerceptionChannelIdKind>? PerceptionChannelId { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public bool HasPerceptionProvenance =>
        PerceptionStimulusId.HasValue &&
        PerceptionChannelId.HasValue;

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public static KnowledgeEvidenceReference Create(
        Id<KnowledgeEvidenceIdKind> evidenceId,
        KnowledgeEvidenceKind kind,
        Id<EntityIdKind> recipientEntityId,
        Id<WorldIdKind> worldId,
        WorldStateVersion worldStateVersion,
        ulong simulationTick,
        Id<EntityIdKind>? sourceEntityId = null)
    {
        if (kind == KnowledgeEvidenceKind.Perception)
        {
            throw new ArgumentException(
                "Perception evidence must be created from an observation.",
                nameof(kind));
        }

        return Restore(
            evidenceId,
            kind,
            recipientEntityId,
            worldId,
            worldStateVersion,
            simulationTick,
            sourceEntityId,
            null,
            null);
    }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public static KnowledgeEvidenceReference FromPerception<TSignal>(
        Id<KnowledgeEvidenceIdKind> evidenceId,
        PerceptionObservation<TSignal> observation)
        where TSignal : notnull, IPerceptionSignal
    {
        ArgumentNullException.ThrowIfNull(observation);

        return Restore(
            evidenceId,
            KnowledgeEvidenceKind.Perception,
            observation.ObserverEntityId,
            observation.WorldId,
            observation.WorldStateVersion,
            observation.SimulationTick,
            null,
            observation.StimulusId,
            observation.ChannelId);
    }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public static KnowledgeEvidenceReference Restore(
        Id<KnowledgeEvidenceIdKind> evidenceId,
        KnowledgeEvidenceKind kind,
        Id<EntityIdKind> recipientEntityId,
        Id<WorldIdKind> worldId,
        WorldStateVersion worldStateVersion,
        ulong simulationTick,
        Id<EntityIdKind>? sourceEntityId,
        Id<PerceptionStimulusIdKind>? perceptionStimulusId,
        Id<PerceptionChannelIdKind>? perceptionChannelId)
    {
        EnsureNonEmpty(evidenceId, nameof(evidenceId));
        EnsureNonEmpty(recipientEntityId, nameof(recipientEntityId));
        EnsureNonEmpty(worldId, nameof(worldId));

        if (!Enum.IsDefined(kind))
        {
            throw new ArgumentOutOfRangeException(
                nameof(kind),
                kind,
                "Unknown knowledge evidence kind.");
        }

        if (sourceEntityId is { } source && source.IsEmpty)
        {
            throw new ArgumentException(
                "A present source entity ID cannot be empty.",
                nameof(sourceEntityId));
        }

        var hasStimulus = perceptionStimulusId.HasValue;
        var hasChannel = perceptionChannelId.HasValue;

        if (hasStimulus != hasChannel)
        {
            throw new ArgumentException(
                "Perception stimulus and channel provenance must be present together.",
                nameof(perceptionStimulusId));
        }

        if (perceptionStimulusId is { } stimulus && stimulus.IsEmpty)
        {
            throw new ArgumentException(
                "A present perception stimulus ID cannot be empty.",
                nameof(perceptionStimulusId));
        }

        if (perceptionChannelId is { } channel && channel.IsEmpty)
        {
            throw new ArgumentException(
                "A present perception channel ID cannot be empty.",
                nameof(perceptionChannelId));
        }

        if (kind == KnowledgeEvidenceKind.Perception)
        {
            if (!hasStimulus)
            {
                throw new ArgumentException(
                    "Perception evidence requires stimulus and channel provenance.",
                    nameof(perceptionStimulusId));
            }

            if (sourceEntityId.HasValue)
            {
                throw new ArgumentException(
                    "Generic perception evidence does not assign a source entity.",
                    nameof(sourceEntityId));
            }
        }
        else if (hasStimulus)
        {
            throw new ArgumentException(
                "Only perception evidence may contain perception provenance.",
                nameof(perceptionStimulusId));
        }

        if (kind == KnowledgeEvidenceKind.Communication &&
            !sourceEntityId.HasValue)
        {
            throw new ArgumentException(
                "Communication evidence requires a source entity.",
                nameof(sourceEntityId));
        }

        return new KnowledgeEvidenceReference(
            evidenceId,
            kind,
            recipientEntityId,
            worldId,
            worldStateVersion,
            simulationTick,
            sourceEntityId,
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
                "Knowledge evidence IDs cannot be empty.",
                parameterName);
        }
    }
}
