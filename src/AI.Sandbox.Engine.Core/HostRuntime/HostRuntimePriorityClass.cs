namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Defines bounded Host request priority classes.</summary>
public enum HostRuntimePriorityClass
{
    /// <summary>Deferrable maintenance or background work.</summary>
    Background = 0,

    /// <summary>Ordinary interactive or scheduled work.</summary>
    Normal = 1,

    /// <summary>Time-sensitive work that should precede normal work.</summary>
    Urgent = 2,

    /// <summary>Critical work that should precede all lower classes.</summary>
    Critical = 3,
}
