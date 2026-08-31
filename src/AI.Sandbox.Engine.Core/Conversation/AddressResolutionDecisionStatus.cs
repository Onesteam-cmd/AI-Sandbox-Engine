namespace AI.Sandbox.Engine.Core.Conversation;

/// <summary>
/// Describes the semantic decision returned by an exact address resolver.
/// </summary>
public enum AddressResolutionDecisionStatus
{
    /// <summary>A stable response audience was resolved.</summary>
    Resolved = 0,

    /// <summary>The resolver explicitly declined to resolve an audience.</summary>
    Rejected = 1,
}
