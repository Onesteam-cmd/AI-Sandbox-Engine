namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Contains one immutable deterministic Host priority key.</summary>
public sealed record HostRuntimePriority
{
    internal HostRuntimePriority(
        HostRuntimePriorityClass priorityClass,
        long sequence)
    {
        Class = priorityClass;
        Sequence = sequence;
    }

    /// <summary>Gets the bounded priority class.</summary>
    public HostRuntimePriorityClass Class { get; }

    /// <summary>
    /// Gets the externally assigned FIFO sequence inside the priority class.
    /// </summary>
    public long Sequence { get; }

    /// <summary>Creates one validated immutable priority key.</summary>
    public static HostRuntimePriority Create(
        HostRuntimePriorityClass priorityClass,
        long sequence)
    {
        if (!Enum.IsDefined(priorityClass))
        {
            throw new ArgumentOutOfRangeException(nameof(priorityClass));
        }
        if (sequence < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sequence));
        }

        return new HostRuntimePriority(priorityClass, sequence);
    }
}
