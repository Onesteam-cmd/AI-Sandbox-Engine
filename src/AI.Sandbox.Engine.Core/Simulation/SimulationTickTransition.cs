using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Simulation;

internal sealed class SimulationTickTransition<TState> :
    IWorldStateTransition<TState>
    where TState : class, IWorldState
{
    private readonly IReadOnlyList<SimulationSystemRegistration<TState>>
        registrations;
    private readonly ulong targetSimulationTick;

    public SimulationTickTransition(
        IReadOnlyList<SimulationSystemRegistration<TState>> registrations,
        ulong targetSimulationTick)
    {
        this.registrations = registrations;
        this.targetSimulationTick = targetSimulationTick;
    }

    public int ExecutedSystemCount { get; private set; }

    public Id<SimulationSystemIdKind>? RejectedSystemId { get; private set; }

    public string? RejectionReason { get; private set; }

    public WorldStateTransitionDecision<TState> Evaluate(
        WorldStateSnapshot<TState> current)
    {
        var workingState = current.State;

        for (var index = 0; index < registrations.Count; index++)
        {
            var registration = registrations[index];
            var context = new SimulationSystemContext<TState>(
                current.WorldId,
                current.Version,
                current.SimulationTick,
                targetSimulationTick,
                registration.SystemId,
                index,
                workingState);
            var decision = registration.System.Execute(context);

            ExecutedSystemCount++;

            if (decision is null)
            {
                throw new InvalidOperationException(
                    $"Simulation system '{registration.SystemId}' returned " +
                    "a null decision.");
            }

            var nextState = decision.NextState;
            var rejectionReason = decision.RejectionReason;

            switch (decision.Status)
            {
                case SimulationSystemStatus.Unchanged:
                    if (nextState is not null ||
                        rejectionReason is not null)
                    {
                        throw CreateInconsistentDecisionException(
                            registration.SystemId);
                    }

                    break;

                case SimulationSystemStatus.Updated:
                    if (nextState is null ||
                        rejectionReason is not null)
                    {
                        throw CreateInconsistentDecisionException(
                            registration.SystemId);
                    }

                    workingState = nextState;
                    break;

                case SimulationSystemStatus.Rejected:
                    if (nextState is not null ||
                        string.IsNullOrWhiteSpace(rejectionReason))
                    {
                        throw CreateInconsistentDecisionException(
                            registration.SystemId);
                    }

                    RejectedSystemId = registration.SystemId;
                    RejectionReason = rejectionReason;

                    return WorldStateTransitionDecision<TState>.Reject(
                        $"Simulation system '{registration.SystemId}' " +
                        $"rejected tick {targetSimulationTick}: " +
                        rejectionReason);

                default:
                    throw CreateInconsistentDecisionException(
                        registration.SystemId);
            }
        }

        return WorldStateTransitionDecision<TState>.Accept(workingState);
    }

    private static InvalidOperationException
        CreateInconsistentDecisionException(
            Id<SimulationSystemIdKind> systemId)
    {
        return new InvalidOperationException(
            $"Simulation system '{systemId}' returned an internally " +
            "inconsistent decision.");
    }
}
