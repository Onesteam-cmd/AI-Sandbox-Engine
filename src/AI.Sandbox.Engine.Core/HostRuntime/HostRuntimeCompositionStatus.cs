namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Describes one deterministic host-runtime composition outcome.</summary>
public enum HostRuntimeCompositionStatus
{
    /// <summary>Composition completed successfully.</summary>
    Composed = 0,

    /// <summary>No capability descriptors were supplied.</summary>
    Empty = 1,

    /// <summary>The bounded capability count was exceeded.</summary>
    TooManyCapabilities = 2,

    /// <summary>More than one descriptor used the same capability ID.</summary>
    DuplicateCapability = 3,

    /// <summary>A declared dependency was absent from the composition input.</summary>
    MissingDependency = 4,

    /// <summary>The capability dependency graph contained a cycle.</summary>
    CycleDetected = 5,
}
