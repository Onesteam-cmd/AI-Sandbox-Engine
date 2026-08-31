namespace AI.Sandbox.Engine.Core.Conversation;

/// <summary>
/// Captures one explicit immutable address-resolution outcome.
/// </summary>
public sealed record AddressResolutionResult
{
    internal AddressResolutionResult(
        AddressResolutionStatus status,
        bool resolverWasInvoked,
        bool hasStableDecision,
        AddressResolutionDecision? decision)
    {
        Status = status;
        ResolverWasInvoked = resolverWasInvoked;
        HasStableDecision = hasStableDecision;
        Decision = decision;
    }

    /// <summary>
    /// Gets the explicit processor status.
    /// </summary>
    public AddressResolutionStatus Status { get; }

    /// <summary>
    /// Gets a value indicating whether the resolver was invoked.
    /// </summary>
    public bool ResolverWasInvoked { get; }

    /// <summary>
    /// Gets a value indicating whether a stable validated decision is present.
    /// </summary>
    public bool HasStableDecision { get; }

    /// <summary>
    /// Gets the stable resolved or rejected decision when present.
    /// </summary>
    public AddressResolutionDecision? Decision { get; }
}
