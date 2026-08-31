namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Defines explicit external Host completion kinds.</summary>
public enum HostRuntimeCompletionKind
{
    /// <summary>The external Host reports successful completion.</summary>
    Completed = 0,

    /// <summary>The external Host rejected the operation.</summary>
    Rejected = 1,

    /// <summary>The external Host reports operational failure.</summary>
    Failed = 2,

    /// <summary>The external Host confirms cancellation.</summary>
    Cancelled = 3,
}
