namespace AI.Sandbox.Engine.Core.Behavior;

/// <summary>
/// Supplies one action validator with a stable authoritative snapshot and the
/// exact proposal being evaluated.
/// </summary>
/// <typeparam name="TState">The immutable authoritative world-state type.</typeparam>
/// <typeparam name="TAction">The exact action-proposal payload type.</typeparam>
public sealed class ActionValidationContext<TState, TAction>
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TAction : IActionProposal
{
    internal ActionValidationContext(
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateSnapshot<TState>
            snapshot,
        ActionProposalEnvelope<TAction> proposal)
    {
        Snapshot = snapshot;
        Proposal = proposal;
    }

    /// <summary>
    /// Gets the stable authoritative snapshot used for this evaluation.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateSnapshot<TState>
        Snapshot { get; }

    /// <summary>
    /// Gets the exact action proposal being evaluated.
    /// </summary>
    public ActionProposalEnvelope<TAction> Proposal { get; }
}
