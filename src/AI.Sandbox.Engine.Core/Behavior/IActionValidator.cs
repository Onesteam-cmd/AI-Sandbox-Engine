namespace AI.Sandbox.Engine.Core.Behavior;

/// <summary>
/// Defines one synchronous, pure validator for an exact action-proposal type.
/// </summary>
/// <typeparam name="TState">The immutable authoritative world-state type.</typeparam>
/// <typeparam name="TAction">The exact action-proposal payload type.</typeparam>
/// <typeparam name="TCommand">
/// The exact command payload returned when approved.
/// </typeparam>
public interface IActionValidator<TState, TAction, TCommand>
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TAction : IActionProposal
    where TCommand : global::AI.Sandbox.Engine.Core.Commands.IEngineCommand
{
    /// <summary>
    /// Evaluates one proposal exactly once without changing World State.
    /// </summary>
    /// <param name="context">The stable validation context.</param>
    /// <returns>An approved command payload or explicit rejection.</returns>
    public ActionDecision<TCommand> Evaluate(
        ActionValidationContext<TState, TAction> context);
}
