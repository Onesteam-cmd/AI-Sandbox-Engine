namespace AI.Sandbox.Engine.Core.Social;

/// <summary>
/// Represents one immutable social turn-coordination decision.
/// </summary>
public sealed class SocialTurnCoordinationDecision
{
    private SocialTurnCoordinationDecision(
        SocialTurnCoordinationDecisionStatus status,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            SocialTurnProposalIdKind> selectedProposalId,
        SocialTurnNoTurnCode noTurnCode,
        SocialTurnRejectionCode rejectionCode)
    {
        Status = status;
        SelectedProposalId = selectedProposalId;
        NoTurnCode = noTurnCode;
        RejectionCode = rejectionCode;
    }

    /// <summary>
    /// Gets the semantic decision status.
    /// </summary>
    public SocialTurnCoordinationDecisionStatus Status { get; }

    /// <summary>
    /// Gets the selected proposal ID when granted.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        SocialTurnProposalIdKind> SelectedProposalId { get; }

    /// <summary>
    /// Gets the stable no-turn reason when no turn was selected.
    /// </summary>
    public SocialTurnNoTurnCode NoTurnCode { get; }

    /// <summary>
    /// Gets the stable rejection code when coordination was rejected.
    /// </summary>
    public SocialTurnRejectionCode RejectionCode { get; }

    /// <summary>
    /// Grants the speaking floor to one proposal.
    /// </summary>
    /// <param name="proposalId">The selected non-empty proposal ID.</param>
    /// <returns>The validated grant decision.</returns>
    public static SocialTurnCoordinationDecision Grant(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            SocialTurnProposalIdKind> proposalId)
    {
        if (proposalId.IsEmpty)
        {
            throw new ArgumentException(
                "The selected proposal ID must be non-empty.",
                nameof(proposalId));
        }

        return new SocialTurnCoordinationDecision(
            SocialTurnCoordinationDecisionStatus.Granted,
            proposalId,
            default,
            default);
    }

    /// <summary>
    /// Selects no participant for the next speaking turn.
    /// </summary>
    /// <param name="noTurnCode">The stable no-turn reason.</param>
    /// <returns>The validated no-turn decision.</returns>
    public static SocialTurnCoordinationDecision SelectNoTurn(
        SocialTurnNoTurnCode noTurnCode)
    {
        if (!noTurnCode.IsInitialized)
        {
            throw new ArgumentException(
                "The no-turn code must be initialized.",
                nameof(noTurnCode));
        }

        return new SocialTurnCoordinationDecision(
            SocialTurnCoordinationDecisionStatus.NoTurn,
            default,
            noTurnCode,
            default);
    }

    /// <summary>
    /// Rejects the coordination request explicitly.
    /// </summary>
    /// <param name="rejectionCode">The stable rejection code.</param>
    /// <returns>The validated rejection decision.</returns>
    public static SocialTurnCoordinationDecision Reject(
        SocialTurnRejectionCode rejectionCode)
    {
        if (!rejectionCode.IsInitialized)
        {
            throw new ArgumentException(
                "The rejection code must be initialized.",
                nameof(rejectionCode));
        }

        return new SocialTurnCoordinationDecision(
            SocialTurnCoordinationDecisionStatus.Rejected,
            default,
            default,
            rejectionCode);
    }
}
