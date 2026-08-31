namespace AI.Sandbox.Engine.Core.Modeling;

/// <summary>
/// Identifies the complete validated result of one model invocation attempt.
/// </summary>
public enum ModelInvocationStatus
{
    /// <summary>The response completed and passed all correlation checks.</summary>
    Completed = 0,

    /// <summary>The adapter explicitly rejected the request.</summary>
    Rejected = 1,

    /// <summary>The adapter explicitly reported an operational failure.</summary>
    Failed = 2,

    /// <summary>The request targets a different configured adapter.</summary>
    RequestAdapterMismatch = 3,

    /// <summary>The response references a different invocation.</summary>
    ResponseInvocationMismatch = 4,

    /// <summary>The response references a different adapter.</summary>
    ResponseAdapterMismatch = 5,

    /// <summary>The response references a different model profile.</summary>
    ResponseProfileMismatch = 6,

    /// <summary>The response references a different source prompt.</summary>
    ResponsePromptDocumentMismatch = 7,

    /// <summary>The response references a different subjective owner.</summary>
    ResponseOwnerMismatch = 8,

    /// <summary>The response references a different authoritative world.</summary>
    ResponseWorldMismatch = 9,

    /// <summary>The response references a different authoritative version.</summary>
    ResponseVersionMismatch = 10,

    /// <summary>The response references a different logical simulation tick.</summary>
    ResponseSimulationTickMismatch = 11,

    /// <summary>The reported output usage exceeds the requested limit.</summary>
    ResponseOutputLimitExceeded = 12,
}
