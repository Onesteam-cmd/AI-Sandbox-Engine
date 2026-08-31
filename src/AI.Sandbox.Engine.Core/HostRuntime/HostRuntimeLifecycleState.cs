namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Defines explicit host-runtime lifecycle states.</summary>
public enum HostRuntimeLifecycleState
{
    /// <summary>The immutable lifecycle record exists but startup has not begun.</summary>
    Created = 0,

    /// <summary>External startup work is in progress.</summary>
    Starting = 1,

    /// <summary>The external Host reports that the runtime is available.</summary>
    Running = 2,

    /// <summary>External shutdown work is in progress.</summary>
    Stopping = 3,

    /// <summary>The runtime has completed shutdown and cannot transition again.</summary>
    Stopped = 4,

    /// <summary>The external Host reports a terminal operational fault.</summary>
    Faulted = 5,
}
