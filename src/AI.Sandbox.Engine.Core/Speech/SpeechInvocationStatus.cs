namespace AI.Sandbox.Engine.Core.Speech;

/// <summary>
/// Identifies the complete validated result of one speech invocation attempt.
/// </summary>
public enum SpeechInvocationStatus
{
    /// <summary>The response completed and passed all correlation checks.</summary>
    Completed = 0,

    /// <summary>The adapter explicitly rejected the request.</summary>
    Rejected = 1,

    /// <summary>The adapter explicitly reported an operational failure.</summary>
    Failed = 2,

    /// <summary>The request targets a different configured adapter.</summary>
    RequestAdapterMismatch = 3,

    /// <summary>The response references a different operation.</summary>
    ResponseOperationMismatch = 4,

    /// <summary>The response references a different invocation.</summary>
    ResponseInvocationMismatch = 5,

    /// <summary>The response references a different adapter.</summary>
    ResponseAdapterMismatch = 6,

    /// <summary>The response references a different speech profile.</summary>
    ResponseProfileMismatch = 7,

    /// <summary>The response references a different subjective owner.</summary>
    ResponseOwnerMismatch = 8,

    /// <summary>The response references a different authoritative world.</summary>
    ResponseWorldMismatch = 9,

    /// <summary>The response references a different authoritative version.</summary>
    ResponseVersionMismatch = 10,

    /// <summary>The response references a different logical simulation tick.</summary>
    ResponseSimulationTickMismatch = 11,

    /// <summary>The reported input usage exceeds the requested limit.</summary>
    ResponseInputLimitExceeded = 12,

    /// <summary>The reported output usage exceeds the requested limit.</summary>
    ResponseOutputLimitExceeded = 13,
}
