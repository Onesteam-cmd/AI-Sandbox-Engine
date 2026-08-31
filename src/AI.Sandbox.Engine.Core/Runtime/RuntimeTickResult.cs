using AI.Sandbox.Engine.Core.Simulation;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Runtime;

/// <summary>
/// Reports one admitted or busy runtime simulation-tick invocation.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
public sealed class RuntimeTickResult<TState>
    where TState : class, IWorldState
{
    private RuntimeTickResult(
        RuntimeInvocationStatus invocationStatus,
        SimulationStepResult<TState>? simulationResult,
        WorldStateSnapshot<TState> snapshot,
        RuntimeCommitFact? commitFact)
    {
        InvocationStatus = invocationStatus;
        SimulationResult = simulationResult;
        Snapshot = snapshot;
        CommitFact = commitFact;
    }

    /// <summary>
    /// Gets whether the runtime admitted this invocation.
    /// </summary>
    public RuntimeInvocationStatus InvocationStatus { get; }

    /// <summary>
    /// Gets the normal Scheduler result when admitted.
    /// </summary>
    public SimulationStepResult<TState>? SimulationResult { get; }

    /// <summary>
    /// Gets the authoritative snapshot observed after this invocation.
    /// </summary>
    public WorldStateSnapshot<TState> Snapshot { get; }

    /// <summary>
    /// Gets the completed commit fact only when the tick committed.
    /// </summary>
    public RuntimeCommitFact? CommitFact { get; }

    /// <summary>
    /// Gets a value indicating whether the runtime admitted the call.
    /// </summary>
    public bool WasInvoked =>
        InvocationStatus == RuntimeInvocationStatus.Completed;

    /// <summary>
    /// Gets a value indicating whether the admitted tick committed.
    /// </summary>
    public bool WasCommitted => CommitFact is not null;

    internal static RuntimeTickResult<TState> Busy(
        WorldStateSnapshot<TState> snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        return new RuntimeTickResult<TState>(
            RuntimeInvocationStatus.Busy,
            null,
            snapshot,
            null);
    }

    internal static RuntimeTickResult<TState> Completed(
        SimulationStepResult<TState> simulationResult)
    {
        ArgumentNullException.ThrowIfNull(simulationResult);

        var commitFact = simulationResult.WasApplied
            ? RuntimeCommitFact.FromSimulationTick(
                simulationResult.Snapshot)
            : null;

        return new RuntimeTickResult<TState>(
            RuntimeInvocationStatus.Completed,
            simulationResult,
            simulationResult.Snapshot,
            commitFact);
    }
}
