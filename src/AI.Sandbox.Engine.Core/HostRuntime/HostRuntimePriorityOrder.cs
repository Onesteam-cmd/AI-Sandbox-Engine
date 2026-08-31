namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Defines deterministic pure Host priority ordering.</summary>
public static class HostRuntimePriorityOrder
{
    /// <summary>
    /// Compares priorities for dispatch order: higher class first, then lower
    /// external sequence first.
    /// </summary>
    public static int Compare(
        HostRuntimePriority left,
        HostRuntimePriority right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        var classOrder = right.Class.CompareTo(left.Class);
        return classOrder != 0
            ? classOrder
            : left.Sequence.CompareTo(right.Sequence);
    }
}
