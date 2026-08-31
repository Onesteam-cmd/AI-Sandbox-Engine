namespace AI.Sandbox.Engine.Core.Social;

/// <summary>
/// Captures one explicit validated social turn-coordination outcome.
/// </summary>
/// <typeparam name="TProposal">The exact proposal payload type.</typeparam>
public sealed class SocialTurnCoordinationResult<TProposal>
    where TProposal : ISocialTurnProposal
{
    internal SocialTurnCoordinationResult(
        SocialTurnCoordinationStatus status,
        bool coordinatorWasInvoked,
        bool hasStableDecision,
        SocialTurnCoordinationDecision? decision,
        SocialTurnProposalEnvelope<TProposal>? selectedProposal)
    {
        Status = status;
        CoordinatorWasInvoked = coordinatorWasInvoked;
        HasStableDecision = hasStableDecision;
        Decision = decision;
        SelectedProposal = selectedProposal;
    }

    /// <summary>
    /// Gets the explicit validated status.
    /// </summary>
    public SocialTurnCoordinationStatus Status { get; }

    /// <summary>
    /// Gets whether the configured coordinator was invoked.
    /// </summary>
    public bool CoordinatorWasInvoked { get; }

    /// <summary>
    /// Gets whether the returned coordinator decision remained valid.
    /// </summary>
    public bool HasStableDecision { get; }

    /// <summary>
    /// Gets the stable coordinator decision when available.
    /// </summary>
    public SocialTurnCoordinationDecision? Decision { get; }

    /// <summary>
    /// Gets the selected proposal only for a valid grant.
    /// </summary>
    public SocialTurnProposalEnvelope<TProposal>? SelectedProposal { get; }
}
