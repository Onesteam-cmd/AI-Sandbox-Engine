namespace AI.Sandbox.Engine.Core.Modeling;

/// <summary>
/// Identifies the direct outcome returned by one model adapter invocation.
/// </summary>
public enum ModelInvocationDecisionStatus
{
    /// <summary>The adapter completed with one correlated response.</summary>
    Completed = 0,

    /// <summary>The adapter explicitly rejected the request.</summary>
    Rejected = 1,

    /// <summary>The adapter explicitly reported an operational failure.</summary>
    Failed = 2,
}
