namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Defines explicit immutable Host request states.</summary>
public enum HostRuntimeRequestState
{
    /// <summary>The external Host has accepted the request record.</summary>
    Pending = 0,

    /// <summary>A cancellation intention has been recorded but not executed.</summary>
    CancellationRequested = 1,

    /// <summary>The external Host reports successful completion.</summary>
    Completed = 2,

    /// <summary>The external Host rejected the request before completion.</summary>
    Rejected = 3,

    /// <summary>The external Host reports an operational failure.</summary>
    Failed = 4,

    /// <summary>The external Host confirms cancellation completion.</summary>
    Cancelled = 5,
}
