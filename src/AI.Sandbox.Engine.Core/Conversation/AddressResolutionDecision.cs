namespace AI.Sandbox.Engine.Core.Conversation;

/// <summary>
/// Represents one immutable semantic address-resolution decision.
/// </summary>
public sealed class AddressResolutionDecision
{
    private AddressResolutionDecision(
        AddressResolutionDecisionStatus status,
        AddressAudience? audience,
        AddressResolutionConfidence confidence,
        AddressRejectionCode rejectionCode)
    {
        Status = status;
        Audience = audience;
        Confidence = confidence;
        RejectionCode = rejectionCode;
    }

    /// <summary>
    /// Gets the semantic decision status.
    /// </summary>
    public AddressResolutionDecisionStatus Status { get; }

    /// <summary>
    /// Gets the resolved audience when <see cref="Status"/> is Resolved.
    /// </summary>
    public AddressAudience? Audience { get; }

    /// <summary>
    /// Gets initialized confidence when <see cref="Status"/> is Resolved.
    /// </summary>
    public AddressResolutionConfidence Confidence { get; }

    /// <summary>
    /// Gets the stable rejection code when <see cref="Status"/> is Rejected.
    /// </summary>
    public AddressRejectionCode RejectionCode { get; }

    /// <summary>
    /// Creates a resolved audience decision.
    /// </summary>
    /// <param name="audience">The resolved audience.</param>
    /// <param name="confidence">Initialized confidence.</param>
    /// <returns>The validated resolved decision.</returns>
    public static AddressResolutionDecision Resolve(
        AddressAudience audience,
        AddressResolutionConfidence confidence)
    {
        ArgumentNullException.ThrowIfNull(audience);

        if (!confidence.IsInitialized)
        {
            throw new ArgumentException(
                "Resolved address confidence must be initialized.",
                nameof(confidence));
        }

        return new AddressResolutionDecision(
            AddressResolutionDecisionStatus.Resolved,
            audience,
            confidence,
            default);
    }

    /// <summary>
    /// Creates an explicit rejection decision.
    /// </summary>
    /// <param name="rejectionCode">The stable rejection code.</param>
    /// <returns>The validated rejection decision.</returns>
    public static AddressResolutionDecision Reject(
        AddressRejectionCode rejectionCode)
    {
        if (!rejectionCode.IsInitialized)
        {
            throw new ArgumentException(
                "Address rejection codes must be initialized.",
                nameof(rejectionCode));
        }

        return new AddressResolutionDecision(
            AddressResolutionDecisionStatus.Rejected,
            null,
            default,
            rejectionCode);
    }
}
