namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Contains one immutable bounded Host queue capacity.</summary>
public sealed record HostRuntimeQueueCapacity
{
    /// <summary>Gets the maximum supported queued request count.</summary>
    public const int MaximumQueuedRequests = 1_000_000;

    internal HostRuntimeQueueCapacity(int maxQueuedRequests)
    {
        MaxQueuedRequests = maxQueuedRequests;
    }

    /// <summary>Gets the inclusive queued request limit.</summary>
    public int MaxQueuedRequests { get; }

    /// <summary>Creates one validated immutable queue capacity.</summary>
    public static HostRuntimeQueueCapacity Create(int maxQueuedRequests)
    {
        if (maxQueuedRequests < 1 ||
            maxQueuedRequests > MaximumQueuedRequests)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxQueuedRequests));
        }

        return new HostRuntimeQueueCapacity(maxQueuedRequests);
    }
}
