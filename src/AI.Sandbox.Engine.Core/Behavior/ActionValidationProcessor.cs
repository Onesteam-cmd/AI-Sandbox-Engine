namespace AI.Sandbox.Engine.Core.Behavior;

/// <summary>
/// Validates exact action proposals against authoritative snapshots without
/// applying the returned command payload.
/// </summary>
/// <typeparam name="TState">The immutable authoritative world-state type.</typeparam>
/// <typeparam name="TAction">The exact action-proposal payload type.</typeparam>
/// <typeparam name="TCommand">
/// The exact command payload type returned when approved.
/// </typeparam>
public sealed class ActionValidationProcessor<TState, TAction, TCommand>
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TAction : IActionProposal
    where TCommand : global::AI.Sandbox.Engine.Core.Commands.IEngineCommand
{
    private readonly global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<TState> manager;
    private readonly IActionValidator<TState, TAction, TCommand> validator;

    private ActionValidationProcessor(
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateManager<TState>
            manager,
        IActionValidator<TState, TAction, TCommand> validator)
    {
        this.manager = manager;
        this.validator = validator;
    }

    /// <summary>
    /// Creates a processor bound to one authority manager and one exact
    /// validator.
    /// </summary>
    /// <param name="manager">The authoritative World State manager.</param>
    /// <param name="validator">The synchronous pure action validator.</param>
    /// <returns>The configured processor.</returns>
    public static ActionValidationProcessor<TState, TAction, TCommand> Create(
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateManager<TState>
            manager,
        IActionValidator<TState, TAction, TCommand> validator)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(validator);

        BehaviorTypePolicy.EnsureExactType(
            typeof(TAction),
            typeof(IActionProposal),
            "action proposal");
        BehaviorTypePolicy.EnsureExactType(
            typeof(TCommand),
            typeof(global::AI.Sandbox.Engine.Core.Commands.IEngineCommand),
            "command output");

        return new ActionValidationProcessor<TState, TAction, TCommand>(
            manager,
            validator);
    }

    /// <summary>
    /// Validates one action proposal exactly once when its snapshot coordinates
    /// are current.
    /// </summary>
    /// <param name="proposal">The exact action proposal.</param>
    /// <returns>The explicit validation result.</returns>
    public ActionValidationResult<TAction, TCommand> Validate(
        ActionProposalEnvelope<TAction> proposal)
    {
        ArgumentNullException.ThrowIfNull(proposal);

        var before = manager.Read();

        if (proposal.WorldId != before.WorldId)
        {
            return ActionValidationResult<TAction, TCommand>.NotEvaluated(
                ActionValidationStatus.WorldMismatch,
                proposal);
        }

        if (proposal.WorldStateVersion != before.Version)
        {
            return ActionValidationResult<TAction, TCommand>.NotEvaluated(
                ActionValidationStatus.VersionConflict,
                proposal);
        }

        if (proposal.SimulationTick != before.SimulationTick)
        {
            return ActionValidationResult<TAction, TCommand>.NotEvaluated(
                ActionValidationStatus.SimulationTickMismatch,
                proposal);
        }

        var context = new ActionValidationContext<TState, TAction>(
            before,
            proposal);
        var decision = validator.Evaluate(context) ??
            throw new InvalidOperationException(
                "Action validators cannot return null decisions.");

        var after = manager.Read();

        if (after.SimulationTick != before.SimulationTick)
        {
            return ActionValidationResult<TAction, TCommand>.Discarded(
                ActionValidationStatus.SimulationTickMismatch,
                proposal);
        }

        if (after.Version != before.Version)
        {
            return ActionValidationResult<TAction, TCommand>.Discarded(
                ActionValidationStatus.VersionConflict,
                proposal);
        }

        var status = decision.Status switch
        {
            ActionDecisionStatus.Approved =>
                ActionValidationStatus.Approved,
            ActionDecisionStatus.Rejected =>
                ActionValidationStatus.Rejected,
            _ => throw new InvalidOperationException(
                "Unknown action decision status."),
        };

        return ActionValidationResult<TAction, TCommand>.Evaluated(
            status,
            proposal,
            decision);
    }
}
