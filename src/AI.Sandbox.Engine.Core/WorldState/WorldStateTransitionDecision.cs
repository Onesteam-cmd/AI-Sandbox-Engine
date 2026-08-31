namespace AI.Sandbox.Engine.Core.WorldState;

/// <summary>
/// Describes whether a proposed world-state transition is accepted.
/// </summary>
/// <typeparam name="TState">The immutable world-state root type.</typeparam>
public sealed class WorldStateTransitionDecision<TState>
    where TState : class, IWorldState
{
    private WorldStateTransitionDecision(
        bool isAccepted,
        TState? nextState,
        string? rejectionReason)
    {
        IsAccepted = isAccepted;
        NextState = nextState;
        RejectionReason = rejectionReason;
    }

    /// <summary>
    /// Gets a value indicating whether the proposed transition is accepted.
    /// </summary>
    public bool IsAccepted { get; }

    /// <summary>
    /// Gets the proposed next state when accepted; otherwise
    /// <see langword="null"/>.
    /// </summary>
    public TState? NextState { get; }

    /// <summary>
    /// Gets the rejection reason when rejected; otherwise
    /// <see langword="null"/>.
    /// </summary>
    public string? RejectionReason { get; }

    /// <summary>
    /// Accepts a proposed immutable next state.
    /// </summary>
    /// <param name="nextState">The next immutable world-state root.</param>
    /// <returns>An accepted transition decision.</returns>
    public static WorldStateTransitionDecision<TState> Accept(TState nextState)
    {
        ArgumentNullException.ThrowIfNull(nextState);

        return new WorldStateTransitionDecision<TState>(
            true,
            nextState,
            null);
    }

    /// <summary>
    /// Rejects a proposed transition without changing authoritative state.
    /// </summary>
    /// <param name="reason">A non-empty internal rejection reason.</param>
    /// <returns>A rejected transition decision.</returns>
    public static WorldStateTransitionDecision<TState> Reject(string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        return new WorldStateTransitionDecision<TState>(
            false,
            null,
            reason);
    }
}
