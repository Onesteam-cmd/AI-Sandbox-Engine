using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Knowledge;

/// <summary>
/// Stores one current subjective claim revision and its latest evidence.
/// </summary>
/// <typeparam name="TClaim">The exact concrete claim type.</typeparam>
/// <remarks>
/// Revision history is intentionally not retained here. Historical episodes,
/// decay, rehearsal, and forgetting belong to the Memory Model.
/// </remarks>
public sealed class KnowledgeEntry<TClaim>
    where TClaim : notnull, IKnowledgeClaim
{
    private KnowledgeEntry(
        Id<KnowledgeClaimIdKind> claimId,
        uint revision,
        TClaim claim,
        KnowledgeConfidence confidence,
        KnowledgeEvidenceReference evidence,
        WorldStateVersion firstAcquiredWorldStateVersion,
        ulong firstAcquiredSimulationTick)
    {
        ClaimId = claimId;
        Revision = revision;
        Claim = claim;
        Confidence = confidence;
        Evidence = evidence;
        FirstAcquiredWorldStateVersion =
            firstAcquiredWorldStateVersion;
        FirstAcquiredSimulationTick =
            firstAcquiredSimulationTick;
    }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public Id<KnowledgeClaimIdKind> ClaimId { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public uint Revision { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public TClaim Claim { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public KnowledgeConfidence Confidence { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public KnowledgeEvidenceReference Evidence { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public WorldStateVersion FirstAcquiredWorldStateVersion { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public ulong FirstAcquiredSimulationTick { get; }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public WorldStateVersion LastRevisedWorldStateVersion =>
        Evidence.WorldStateVersion;

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public ulong LastRevisedSimulationTick =>
        Evidence.SimulationTick;

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public static KnowledgeEntry<TClaim> Restore(
        Id<KnowledgeClaimIdKind> claimId,
        uint revision,
        TClaim claim,
        KnowledgeConfidence confidence,
        KnowledgeEvidenceReference evidence,
        WorldStateVersion firstAcquiredWorldStateVersion,
        ulong firstAcquiredSimulationTick)
    {
        if (claimId.IsEmpty)
        {
            throw new ArgumentException(
                "A knowledge claim ID cannot be empty.",
                nameof(claimId));
        }

        if (revision == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(revision),
                revision,
                "A knowledge claim revision must be positive.");
        }

        KnowledgeTypePolicy.EnsureConcrete<TClaim>(nameof(TClaim));
        KnowledgeTypePolicy.EnsureValue(claim, nameof(claim));
        confidence.EnsureUsableForActiveClaim();
        ArgumentNullException.ThrowIfNull(evidence);

        if (firstAcquiredWorldStateVersion.Value >
            evidence.WorldStateVersion.Value)
        {
            throw new ArgumentOutOfRangeException(
                nameof(firstAcquiredWorldStateVersion),
                firstAcquiredWorldStateVersion,
                "First acquisition version cannot follow latest evidence.");
        }

        if (firstAcquiredSimulationTick > evidence.SimulationTick)
        {
            throw new ArgumentOutOfRangeException(
                nameof(firstAcquiredSimulationTick),
                firstAcquiredSimulationTick,
                "First acquisition tick cannot follow latest evidence.");
        }

        return new KnowledgeEntry<TClaim>(
            claimId,
            revision,
            claim,
            confidence,
            evidence,
            firstAcquiredWorldStateVersion,
            firstAcquiredSimulationTick);
    }

    internal static KnowledgeEntry<TClaim> Create(
        Id<KnowledgeClaimIdKind> claimId,
        TClaim claim,
        KnowledgeConfidence confidence,
        KnowledgeEvidenceReference evidence)
    {
        return Restore(
            claimId,
            revision: 1,
            claim,
            confidence,
            evidence,
            evidence.WorldStateVersion,
            evidence.SimulationTick);
    }

    internal KnowledgeEntry<TClaim> Revise(
        TClaim claim,
        KnowledgeConfidence confidence,
        KnowledgeEvidenceReference evidence)
    {
        return Restore(
            ClaimId,
            checked(Revision + 1),
            claim,
            confidence,
            evidence,
            FirstAcquiredWorldStateVersion,
            FirstAcquiredSimulationTick);
    }
}
