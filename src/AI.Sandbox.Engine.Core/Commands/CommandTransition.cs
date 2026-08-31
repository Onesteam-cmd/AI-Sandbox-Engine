using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Commands;

internal sealed class CommandTransition<TState, TCommand> :
    IWorldStateTransition<TState>
    where TState : class, IWorldState
    where TCommand : notnull, IEngineCommand
{
    private readonly CommandEnvelope<TCommand> envelope;
    private readonly ICommandHandler<TState, TCommand> handler;

    public CommandTransition(
        CommandEnvelope<TCommand> envelope,
        ICommandHandler<TState, TCommand> handler)
    {
        this.envelope = envelope;
        this.handler = handler;
    }

    public bool WasEvaluated { get; private set; }

    public string? RejectionReason { get; private set; }

    public WorldStateTransitionDecision<TState> Evaluate(
        WorldStateSnapshot<TState> current)
    {
        WasEvaluated = true;

        var context = new CommandContext<TState, TCommand>(
            current,
            envelope);
        var decision = handler.Evaluate(context);

        if (decision is null)
        {
            throw new InvalidOperationException(
                $"Command handler for '{typeof(TCommand)}' returned null.");
        }

        var nextState = decision.NextState;
        var rejectionReason = decision.RejectionReason;

        switch (decision.Status)
        {
            case CommandDecisionStatus.Accepted:
                if (nextState is null ||
                    rejectionReason is not null ||
                    ReferenceEquals(nextState, current.State))
                {
                    throw CreateInconsistentDecisionException();
                }

                return WorldStateTransitionDecision<TState>.Accept(nextState);

            case CommandDecisionStatus.Rejected:
                if (nextState is not null ||
                    string.IsNullOrWhiteSpace(rejectionReason))
                {
                    throw CreateInconsistentDecisionException();
                }

                RejectionReason = rejectionReason;

                return WorldStateTransitionDecision<TState>.Reject(
                    rejectionReason);

            default:
                throw CreateInconsistentDecisionException();
        }
    }

    private static InvalidOperationException
        CreateInconsistentDecisionException()
    {
        return new InvalidOperationException(
            $"Command handler for '{typeof(TCommand)}' returned an " +
            "internally inconsistent decision.");
    }
}
