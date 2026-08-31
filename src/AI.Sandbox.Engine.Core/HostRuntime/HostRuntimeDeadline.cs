namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains one immutable deadline on an externally owned monotonic clock.
/// </summary>
public sealed record HostRuntimeDeadline
{
    internal HostRuntimeDeadline(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeClockIdKind> clockId,
        long dueTick)
    {
        ClockId = clockId;
        DueTick = dueTick;
    }

    /// <summary>Gets the externally assigned monotonic clock domain.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId { get; }

    /// <summary>Gets the exclusive deadline tick in the selected clock domain.</summary>
    public long DueTick { get; }

    /// <summary>Creates one validated immutable Host deadline.</summary>
    /// <param name="clockId">Externally assigned non-empty clock ID.</param>
    /// <param name="dueTick">Non-negative exclusive deadline tick.</param>
    /// <returns>A validated immutable deadline.</returns>
    public static HostRuntimeDeadline Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeClockIdKind> clockId,
        long dueTick)
    {
        if (clockId.IsEmpty)
        {
            throw new ArgumentException(
                "The clock ID must be initialized.",
                nameof(clockId));
        }
        if (dueTick < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dueTick));
        }

        return new HostRuntimeDeadline(clockId, dueTick);
    }
}
