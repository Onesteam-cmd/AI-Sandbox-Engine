using AI.Sandbox.Engine.Core.Commands;
using AI.Sandbox.Engine.Core.Simulation;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Runtime;

/// <summary>
/// Provides one caller-driven admission boundary over commands and simulation
/// ticks that share the same authoritative World State Manager.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
/// <remarks>
/// Operations never queue or wait. If one operation is already executing,
/// another invocation returns <see cref="RuntimeInvocationStatus.Busy"/>
/// immediately. Caller order is therefore explicit.
///
/// The orchestrator never dispatches events, persists snapshots, reads time,
/// generates IDs, retries conflicts, starts threads, or invokes providers.
/// </remarks>
public sealed class RuntimeOrchestrator<TState>
    where TState : class, IWorldState
{
    private readonly WorldStateManager<TState> worldStateManager;
    private readonly CommandProcessor<TState> commandProcessor;
    private readonly SimulationScheduler<TState> simulationScheduler;
    private int operationInProgress;

    internal RuntimeOrchestrator(
        WorldStateManager<TState> worldStateManager,
        CommandProcessor<TState> commandProcessor,
        SimulationScheduler<TState> simulationScheduler)
    {
        this.worldStateManager = worldStateManager;
        this.commandProcessor = commandProcessor;
        this.simulationScheduler = simulationScheduler;
    }

    /// <summary>
    /// Reads the currently authoritative immutable snapshot.
    /// </summary>
    /// <returns>The current snapshot.</returns>
    public WorldStateSnapshot<TState> Read()
    {
        return worldStateManager.Read();
    }

    /// <summary>
    /// Attempts to admit and execute one exact typed command.
    /// </summary>
    /// <typeparam name="TCommand">The exact concrete command type.</typeparam>
    /// <param name="envelope">The immutable version-gated command.</param>
    /// <returns>
    /// A normal command result, or an immediate busy result without evaluation.
    /// </returns>
    public RuntimeCommandResult<TState> ExecuteCommand<TCommand>(
        CommandEnvelope<TCommand> envelope)
        where TCommand : notnull, IEngineCommand
    {
        ArgumentNullException.ThrowIfNull(envelope);

        if (!TryEnterOperation())
        {
            return RuntimeCommandResult<TState>.Busy(
                worldStateManager.Read());
        }

        try
        {
            return RuntimeCommandResult<TState>.Completed(
                commandProcessor.Execute(envelope));
        }
        finally
        {
            ExitOperation();
        }
    }

    /// <summary>
    /// Attempts to admit and execute one complete logical simulation tick.
    /// </summary>
    /// <returns>
    /// A normal Scheduler result, or an immediate busy result without running
    /// any system.
    /// </returns>
    public RuntimeTickResult<TState> RunNextTick()
    {
        if (!TryEnterOperation())
        {
            return RuntimeTickResult<TState>.Busy(
                worldStateManager.Read());
        }

        try
        {
            return RuntimeTickResult<TState>.Completed(
                simulationScheduler.RunNextTick());
        }
        finally
        {
            ExitOperation();
        }
    }

    private bool TryEnterOperation()
    {
        return System.Threading.Interlocked.CompareExchange(
            ref operationInProgress,
            1,
            0) == 0;
    }

    private void ExitOperation()
    {
        System.Threading.Volatile.Write(
            ref operationInProgress,
            0);
    }
}
