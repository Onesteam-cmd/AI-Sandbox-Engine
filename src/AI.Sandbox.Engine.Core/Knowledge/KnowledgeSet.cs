using AI.Sandbox.Engine.Core.Components;
using AI.Sandbox.Engine.Core.Entities;
using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Knowledge;

/// <summary>
/// Stores one entity's immutable current subjective claims of one exact type.
/// </summary>
/// <typeparam name="TClaim">The exact concrete claim type.</typeparam>
/// <remarks>
/// The set contains current epistemic state only. It does not retain episodic
/// history, automatic decay, rehearsal, forgetting schedules, or objective
/// truth validation.
/// </remarks>
public sealed class KnowledgeSet<TClaim> : IComponent
    where TClaim : notnull, IKnowledgeClaim
{
    private readonly IReadOnlyList<KnowledgeEntry<TClaim>> entries;
    private readonly Dictionary<
        Id<KnowledgeClaimIdKind>,
        KnowledgeEntry<TClaim>> entryById;

    private KnowledgeSet(
        Id<WorldIdKind> worldId,
        Id<EntityIdKind> ownerEntityId,
        KnowledgeEntry<TClaim>[] entries)
    {
        WorldId = worldId;
        OwnerEntityId = ownerEntityId;

        var copy =
            (KnowledgeEntry<TClaim>[])entries.Clone();
        this.entries = Array.AsReadOnly(copy);
        entryById = copy.ToDictionary(
            entry => entry.ClaimId);
    }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public Id<WorldIdKind> WorldId { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public Id<EntityIdKind> OwnerEntityId { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public int Count => entries.Count;

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public bool IsEmpty => entries.Count == 0;

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public IReadOnlyList<KnowledgeEntry<TClaim>> Entries => entries;

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public static KnowledgeSet<TClaim> Create(
        Id<WorldIdKind> worldId,
        Id<EntityIdKind> ownerEntityId)
    {
        EnsureNonEmpty(worldId, nameof(worldId));
        EnsureNonEmpty(ownerEntityId, nameof(ownerEntityId));
        KnowledgeTypePolicy.EnsureConcrete<TClaim>(nameof(TClaim));

        return new KnowledgeSet<TClaim>(
            worldId,
            ownerEntityId,
            []);
    }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public static KnowledgeSet<TClaim> Restore(
        Id<WorldIdKind> worldId,
        Id<EntityIdKind> ownerEntityId,
        IEnumerable<KnowledgeEntry<TClaim>> entries)
    {
        EnsureNonEmpty(worldId, nameof(worldId));
        EnsureNonEmpty(ownerEntityId, nameof(ownerEntityId));
        KnowledgeTypePolicy.EnsureConcrete<TClaim>(nameof(TClaim));
        ArgumentNullException.ThrowIfNull(entries);

        var materialized = entries
            .OrderBy(entry => entry.ClaimId)
            .ToArray();
        var seen = new HashSet<
            Id<KnowledgeClaimIdKind>>();

        foreach (var entry in materialized)
        {
            ArgumentNullException.ThrowIfNull(entry);

            if (!seen.Add(entry.ClaimId))
            {
                throw new ArgumentException(
                    $"Knowledge claim '{entry.ClaimId}' appears more than once.",
                    nameof(entries));
            }

            ValidateEvidenceOrThrow(
                worldId,
                ownerEntityId,
                entry.Evidence);
        }

        return new KnowledgeSet<TClaim>(
            worldId,
            ownerEntityId,
            materialized);
    }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public bool TryGet(
        Id<KnowledgeClaimIdKind> claimId,
        out KnowledgeEntry<TClaim>? entry)
    {
        if (claimId.IsEmpty)
        {
            entry = null;
            return false;
        }

        return entryById.TryGetValue(
            claimId,
            out entry);
    }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public KnowledgeMutationResult<TClaim> Add(
        Id<KnowledgeClaimIdKind> claimId,
        TClaim claim,
        KnowledgeConfidence confidence,
        KnowledgeEvidenceReference evidence)
    {
        if (claimId.IsEmpty)
        {
            throw new ArgumentException(
                "A knowledge claim ID cannot be empty.",
                nameof(claimId));
        }

        KnowledgeTypePolicy.EnsureValue(claim, nameof(claim));
        confidence.EnsureUsableForActiveClaim();
        ArgumentNullException.ThrowIfNull(evidence);

        if (entryById.TryGetValue(
            claimId,
            out var duplicate))
        {
            return Result(
                KnowledgeMutationStatus.ClaimAlreadyExists,
                this,
                duplicate);
        }

        var evidenceStatus = GetEvidenceStatus(evidence);
        if (evidenceStatus is { } status)
        {
            return Result(status, this, null);
        }

        var entry = KnowledgeEntry<TClaim>.Create(
            claimId,
            claim,
            confidence,
            evidence);

        return Result(
            KnowledgeMutationStatus.Added,
            WithAdded(entry),
            entry);
    }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public KnowledgeMutationResult<TClaim> Revise(
        Id<KnowledgeClaimIdKind> claimId,
        uint expectedRevision,
        TClaim claim,
        KnowledgeConfidence confidence,
        KnowledgeEvidenceReference evidence)
    {
        if (claimId.IsEmpty)
        {
            throw new ArgumentException(
                "A knowledge claim ID cannot be empty.",
                nameof(claimId));
        }

        if (expectedRevision == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedRevision),
                expectedRevision,
                "An expected knowledge revision must be positive.");
        }

        KnowledgeTypePolicy.EnsureValue(claim, nameof(claim));
        confidence.EnsureUsableForActiveClaim();
        ArgumentNullException.ThrowIfNull(evidence);

        if (!entryById.TryGetValue(
            claimId,
            out var current))
        {
            return Result(
                KnowledgeMutationStatus.ClaimNotFound,
                this,
                null);
        }

        if (current.Revision != expectedRevision)
        {
            return Result(
                KnowledgeMutationStatus.RevisionConflict,
                this,
                current);
        }

        var evidenceStatus = GetEvidenceStatus(evidence);
        if (evidenceStatus is { } status)
        {
            return Result(status, this, current);
        }

        if (evidence.WorldStateVersion.Value <
                current.LastRevisedWorldStateVersion.Value ||
            evidence.SimulationTick <
                current.LastRevisedSimulationTick)
        {
            return Result(
                KnowledgeMutationStatus.EvidenceRegression,
                this,
                current);
        }

        if (EqualityComparer<TClaim>.Default.Equals(
                current.Claim,
                claim) &&
            current.Confidence == confidence &&
            current.Evidence == evidence)
        {
            return Result(
                KnowledgeMutationStatus.Unchanged,
                this,
                current);
        }

        var revised = current.Revise(
            claim,
            confidence,
            evidence);

        return Result(
            KnowledgeMutationStatus.Revised,
            WithReplaced(revised),
            revised);
    }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public KnowledgeMutationResult<TClaim> Remove(
        Id<KnowledgeClaimIdKind> claimId,
        uint expectedRevision)
    {
        if (claimId.IsEmpty)
        {
            throw new ArgumentException(
                "A knowledge claim ID cannot be empty.",
                nameof(claimId));
        }

        if (expectedRevision == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedRevision),
                expectedRevision,
                "An expected knowledge revision must be positive.");
        }

        if (!entryById.TryGetValue(
            claimId,
            out var current))
        {
            return Result(
                KnowledgeMutationStatus.ClaimNotFound,
                this,
                null);
        }

        if (current.Revision != expectedRevision)
        {
            return Result(
                KnowledgeMutationStatus.RevisionConflict,
                this,
                current);
        }

        var remaining = entries
            .Where(entry => entry.ClaimId != claimId)
            .ToArray();

        return Result(
            KnowledgeMutationStatus.Removed,
            new KnowledgeSet<TClaim>(
                WorldId,
                OwnerEntityId,
                remaining),
            current);
    }

    private KnowledgeSet<TClaim> WithAdded(
        KnowledgeEntry<TClaim> entry)
    {
        var updated = entries
            .Append(entry)
            .OrderBy(item => item.ClaimId)
            .ToArray();

        return new KnowledgeSet<TClaim>(
            WorldId,
            OwnerEntityId,
            updated);
    }

    private KnowledgeSet<TClaim> WithReplaced(
        KnowledgeEntry<TClaim> entry)
    {
        var updated = entries
            .Select(current =>
                current.ClaimId == entry.ClaimId
                    ? entry
                    : current)
            .OrderBy(item => item.ClaimId)
            .ToArray();

        return new KnowledgeSet<TClaim>(
            WorldId,
            OwnerEntityId,
            updated);
    }

    private KnowledgeMutationStatus? GetEvidenceStatus(
        KnowledgeEvidenceReference evidence)
    {
        if (evidence.WorldId != WorldId)
        {
            return KnowledgeMutationStatus.EvidenceWorldMismatch;
        }

        if (evidence.RecipientEntityId != OwnerEntityId)
        {
            return KnowledgeMutationStatus.EvidenceOwnerMismatch;
        }

        return null;
    }

    private static void ValidateEvidenceOrThrow(
        Id<WorldIdKind> worldId,
        Id<EntityIdKind> ownerEntityId,
        KnowledgeEvidenceReference evidence)
    {
        ArgumentNullException.ThrowIfNull(evidence);

        if (evidence.WorldId != worldId)
        {
            throw new ArgumentException(
                "Knowledge evidence belongs to a different world.",
                nameof(evidence));
        }

        if (evidence.RecipientEntityId != ownerEntityId)
        {
            throw new ArgumentException(
                "Knowledge evidence belongs to a different recipient.",
                nameof(evidence));
        }
    }

    private static KnowledgeMutationResult<TClaim> Result(
        KnowledgeMutationStatus status,
        KnowledgeSet<TClaim> knowledgeSet,
        KnowledgeEntry<TClaim>? entry)
    {
        return new KnowledgeMutationResult<TClaim>(
            status,
            knowledgeSet,
            entry);
    }

    private static void EnsureNonEmpty<TKind>(
        Id<TKind> id,
        string parameterName)
        where TKind : struct
    {
        if (id.IsEmpty)
        {
            throw new ArgumentException(
                "Knowledge set IDs cannot be empty.",
                parameterName);
        }
    }
}
