using AI.Sandbox.Engine.Core.Commands;
using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.Simulation;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Runtime;

/// <summary>
/// Configures command handlers and simulation systems against one shared World
/// State authority before creating a runtime orchestrator.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
public sealed class RuntimeOrchestratorBuilder<TState>
    where TState : class, IWorldState
{
    private readonly CommandProcessorBuilder<TState> commandBuilder = new();
    private readonly SimulationSchedulerBuilder<TState> simulationBuilder =
        new();
    private bool isBuilt;

    /// <summary>
    /// Registers one exact command handler.
    /// </summary>
    /// <typeparam name="TCommand">The exact concrete command type.</typeparam>
    /// <param name="handler">The pure command handler.</param>
    /// <returns>This builder.</returns>
    public RuntimeOrchestratorBuilder<TState> AddCommandHandler<TCommand>(
        ICommandHandler<TState, TCommand> handler)
        where TCommand : notnull, IEngineCommand
    {
        ThrowIfBuilt();
        _ = commandBuilder.Add(handler);

        return this;
    }

    /// <summary>
    /// Registers one simulation system in deterministic execution order.
    /// </summary>
    /// <param name="systemId">The non-empty stable system ID.</param>
    /// <param name="system">The pure simulation system.</param>
    /// <returns>This builder.</returns>
    public RuntimeOrchestratorBuilder<TState> AddSimulationSystem(
        Id<SimulationSystemIdKind> systemId,
        ISimulationSystem<TState> system)
    {
        ThrowIfBuilt();
        _ = simulationBuilder.Add(systemId, system);

        return this;
    }

    /// <summary>
    /// Builds both subsystems against the same authoritative manager and
    /// permanently consumes this builder.
    /// </summary>
    /// <param name="worldStateManager">The sole shared authority.</param>
    /// <returns>The caller-driven runtime orchestrator.</returns>
    public RuntimeOrchestrator<TState> Build(
        WorldStateManager<TState> worldStateManager)
    {
        ThrowIfBuilt();
        ArgumentNullException.ThrowIfNull(worldStateManager);
        isBuilt = true;

        var commandProcessor =
            commandBuilder.Build(worldStateManager);
        var simulationScheduler =
            simulationBuilder.Build(worldStateManager);

        return new RuntimeOrchestrator<TState>(
            worldStateManager,
            commandProcessor,
            simulationScheduler);
    }

    private void ThrowIfBuilt()
    {
        if (isBuilt)
        {
            throw new InvalidOperationException(
                "A runtime orchestrator builder cannot be reused after Build.");
        }
    }
}
