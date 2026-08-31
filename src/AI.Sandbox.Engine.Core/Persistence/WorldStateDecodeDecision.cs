using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Persistence;

/// <summary>
/// Describes whether a codec accepted an encoded state payload.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
public sealed class WorldStateDecodeDecision<TState>
    where TState : class, IWorldState
{
    private WorldStateDecodeDecision(
        bool isDecoded,
        TState? state,
        string? rejectionReason)
    {
        IsDecoded = isDecoded;
        State = state;
        RejectionReason = rejectionReason;
    }

    /// <summary>
    /// Gets a value indicating whether decoding produced a valid state.
    /// </summary>
    public bool IsDecoded { get; }

    /// <summary>
    /// Gets the decoded immutable state when accepted.
    /// </summary>
    public TState? State { get; }

    /// <summary>
    /// Gets the codec-defined rejection reason when decoding is rejected.
    /// </summary>
    public string? RejectionReason { get; }

    /// <summary>
    /// Accepts one decoded immutable state.
    /// </summary>
    /// <param name="state">The decoded state.</param>
    /// <returns>An accepted decode decision.</returns>
    public static WorldStateDecodeDecision<TState> Accept(TState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        return new WorldStateDecodeDecision<TState>(
            true,
            state,
            null);
    }

    /// <summary>
    /// Rejects malformed or semantically invalid encoded data.
    /// </summary>
    /// <param name="reason">A non-empty internal rejection reason.</param>
    /// <returns>A rejected decode decision.</returns>
    public static WorldStateDecodeDecision<TState> Reject(string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        return new WorldStateDecodeDecision<TState>(
            false,
            null,
            reason);
    }
}
