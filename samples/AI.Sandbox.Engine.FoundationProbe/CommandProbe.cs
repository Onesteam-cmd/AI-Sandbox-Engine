namespace AI.Sandbox.Engine.FoundationProbe;

internal static class CommandProbe
{
    private sealed record CounterState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    private sealed record AddValue(int Amount) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    internal sealed record Result(
        string Status,
        bool WasApplied,
        bool HandlerWasEvaluated,
        int HandlerEvaluationCount,
        int BeforeValue,
        int AfterValue,
        ulong BeforeVersion,
        ulong AfterVersion,
        ulong BeforeSimulationTick,
        ulong AfterSimulationTick,
        bool VersionAdvancedExactlyOnce,
        bool SimulationTickPreserved,
        string RuntimeInvocationStatus,
        bool RuntimeWasInvoked,
        bool RuntimeWasCommitted,
        string RuntimeCommandStatus,
        bool RuntimeCommitFactValid,
        bool RuntimeAutoDispatchAbsent);

    internal static Result Run()
    {
        var manager =
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<CounterState>.Create(
                    WorldId(),
                    new CounterState(4));

        var handler = new CountingAddHandler();

        var processor =
            new global::AI.Sandbox.Engine.Core.Commands
                .CommandProcessorBuilder<CounterState>()
                .Add<AddValue>(handler)
                .Build(manager);

        if (processor.HandlerCount != 1)
        {
            throw new global::System.InvalidOperationException(
                $"Command processor handler count was {processor.HandlerCount}.");
        }

        var before = manager.Read();

        var envelope =
            global::AI.Sandbox.Engine.Core.Commands
                .CommandEnvelope<AddValue>.Create(
                    CommandId(),
                    before.WorldId,
                    before.Version,
                    before.SimulationTick,
                    new AddValue(3));

        var result = processor.Execute(envelope);

        if (!result.WasApplied ||
            result.Status !=
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandExecutionStatus.Applied)
        {
            throw new global::System.InvalidOperationException(
                $"Command execution failed: {result.Status}");
        }

        if (!result.HandlerWasEvaluated)
        {
            throw new global::System.InvalidOperationException(
                "Command handler was not evaluated.");
        }

        if (handler.EvaluationCount != 1)
        {
            throw new global::System.InvalidOperationException(
                $"Command handler evaluation count was " +
                $"{handler.EvaluationCount}.");
        }

        var after = manager.Read();

        if (!global::System.Object.ReferenceEquals(
            result.Snapshot,
            after))
        {
            throw new global::System.InvalidOperationException(
                "Command result did not retain the authoritative snapshot.");
        }

        if (before.State.Value != 4 ||
            after.State.Value != 7)
        {
            throw new global::System.InvalidOperationException(
                $"Command state transition was " +
                $"{before.State.Value}->{after.State.Value}.");
        }

        var versionAdvancedExactlyOnce =
            after.Version.Value == before.Version.Value + 1UL;

        if (!versionAdvancedExactlyOnce)
        {
            throw new global::System.InvalidOperationException(
                $"Command version transition was " +
                $"{before.Version.Value}->{after.Version.Value}.");
        }

        var simulationTickPreserved =
            after.SimulationTick == before.SimulationTick;

        if (!simulationTickPreserved)
        {
            throw new global::System.InvalidOperationException(
                $"Command changed simulation tick from " +
                $"{before.SimulationTick} to {after.SimulationTick}.");
        }

        var runtimeManager =
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<CounterState>.Create(
                    WorldId(),
                    new CounterState(10));

        var runtimeHandler = new CountingAddHandler();

        var runtime =
            new global::AI.Sandbox.Engine.Core.Runtime
                .RuntimeOrchestratorBuilder<CounterState>()
                .AddCommandHandler<AddValue>(runtimeHandler)
                .Build(runtimeManager);

        var runtimeBefore = runtime.Read();

        var runtimeEnvelope =
            global::AI.Sandbox.Engine.Core.Commands
                .CommandEnvelope<AddValue>.Create(
                    CommandId(),
                    runtimeBefore.WorldId,
                    runtimeBefore.Version,
                    runtimeBefore.SimulationTick,
                    new AddValue(5));

        var runtimeDispatchCalls =
            new global::System.Collections.Generic.List<
                global::AI.Sandbox.Engine.Core.Runtime.RuntimeCommitFact>();

        var runtimeDispatcher =
            new global::AI.Sandbox.Engine.Core.Events
                .EventDispatcherBuilder()
                .Add<
                    global::AI.Sandbox.Engine.Core.Runtime.RuntimeCommitFact>(
                    new RuntimeCommitFactHandler(runtimeDispatchCalls))
                .Build();

        var runtimeResult = runtime.ExecuteCommand(runtimeEnvelope);

        if (runtimeResult.InvocationStatus !=
            global::AI.Sandbox.Engine.Core.Runtime
                .RuntimeInvocationStatus.Completed ||
            !runtimeResult.WasInvoked ||
            !runtimeResult.WasCommitted)
        {
            throw new global::System.InvalidOperationException(
                $"Runtime command invocation failed: " +
                $"{runtimeResult.InvocationStatus}.");
        }

        if (runtimeResult.CommandResult is null ||
            !runtimeResult.CommandResult.WasApplied ||
            runtimeResult.CommandResult.Status !=
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandExecutionStatus.Applied)
        {
            throw new global::System.InvalidOperationException(
                "Runtime did not retain an Applied CommandExecutionResult.");
        }

        if (runtimeHandler.EvaluationCount != 1)
        {
            throw new global::System.InvalidOperationException(
                $"Runtime command handler evaluation count was " +
                $"{runtimeHandler.EvaluationCount}.");
        }

        var runtimeAfter = runtime.Read();

        if (!global::System.Object.ReferenceEquals(
            runtimeResult.Snapshot,
            runtimeAfter) ||
            !global::System.Object.ReferenceEquals(
                runtimeResult.CommandResult.Snapshot,
                runtimeAfter))
        {
            throw new global::System.InvalidOperationException(
                "Runtime did not retain the exact authoritative snapshot.");
        }

        if (runtimeBefore.State.Value != 10 ||
            runtimeAfter.State.Value != 15)
        {
            throw new global::System.InvalidOperationException(
                $"Runtime state transition was " +
                $"{runtimeBefore.State.Value}->{runtimeAfter.State.Value}.");
        }

        if (runtimeAfter.Version.Value !=
            runtimeBefore.Version.Value + 1UL)
        {
            throw new global::System.InvalidOperationException(
                $"Runtime version transition was " +
                $"{runtimeBefore.Version.Value}->" +
                $"{runtimeAfter.Version.Value}.");
        }

        if (runtimeAfter.SimulationTick != runtimeBefore.SimulationTick)
        {
            throw new global::System.InvalidOperationException(
                $"Runtime changed simulation tick from " +
                $"{runtimeBefore.SimulationTick} to " +
                $"{runtimeAfter.SimulationTick}.");
        }

        var runtimeCommitFact = runtimeResult.CommitFact;

        if (runtimeCommitFact is null ||
            !runtimeCommitFact.IsCommandCommit ||
            runtimeCommitFact.Kind !=
                global::AI.Sandbox.Engine.Core.Runtime
                    .RuntimeCommitKind.Command ||
            runtimeCommitFact.WorldId != runtimeAfter.WorldId ||
            runtimeCommitFact.PreviousWorldStateVersion !=
                runtimeBefore.Version ||
            runtimeCommitFact.CurrentWorldStateVersion !=
                runtimeAfter.Version ||
            runtimeCommitFact.PreviousSimulationTick !=
                runtimeBefore.SimulationTick ||
            runtimeCommitFact.CurrentSimulationTick !=
                runtimeAfter.SimulationTick ||
            !runtimeCommitFact.CommandId.HasValue ||
            runtimeCommitFact.CommandId.Value != CommandId())
        {
            throw new global::System.InvalidOperationException(
                "Runtime commit fact did not match the exact committed command.");
        }

        var runtimeCommitFactValid = true;
        var runtimeAutoDispatchAbsent = runtimeDispatchCalls.Count == 0;

        if (!runtimeAutoDispatchAbsent)
        {
            throw new global::System.InvalidOperationException(
                "Runtime dispatched the commit fact automatically.");
        }

        global::System.GC.KeepAlive(runtimeDispatcher);

        return new Result(
            result.Status.ToString(),
            result.WasApplied,
            result.HandlerWasEvaluated,
            handler.EvaluationCount,
            before.State.Value,
            after.State.Value,
            before.Version.Value,
            after.Version.Value,
            before.SimulationTick,
            after.SimulationTick,
            versionAdvancedExactlyOnce,
            simulationTickPreserved,
            runtimeResult.InvocationStatus.ToString(),
            runtimeResult.WasInvoked,
            runtimeResult.WasCommitted,
            runtimeResult.CommandResult.Status.ToString(),
            runtimeCommitFactValid,
            runtimeAutoDispatchAbsent);
    }

    private sealed class CountingAddHandler :
        global::AI.Sandbox.Engine.Core.Commands
            .ICommandHandler<CounterState, AddValue>
    {
        internal int EvaluationCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<CounterState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<CounterState, AddValue> context)
        {
            EvaluationCount++;

            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<CounterState>.Accept(
                    context.Snapshot.State with
                    {
                        Value = context.Snapshot.State.Value +
                            context.Envelope.Payload.Amount,
                    });
        }
    }

    private sealed class RuntimeCommitFactHandler :
        global::AI.Sandbox.Engine.Core.Events.IEventHandler<
            global::AI.Sandbox.Engine.Core.Runtime.RuntimeCommitFact>
    {
        private readonly global::System.Collections.Generic.List<
            global::AI.Sandbox.Engine.Core.Runtime.RuntimeCommitFact> calls;

        internal RuntimeCommitFactHandler(
            global::System.Collections.Generic.List<
                global::AI.Sandbox.Engine.Core.Runtime
                    .RuntimeCommitFact> calls)
        {
            this.calls = calls;
        }

        public global::System.Threading.Tasks.ValueTask HandleAsync(
            global::AI.Sandbox.Engine.Core.Events.EventEnvelope<
                global::AI.Sandbox.Engine.Core.Runtime
                    .RuntimeCommitFact> envelope,
            global::System.Threading.CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            calls.Add(envelope.Payload);

            return global::System.Threading.Tasks.ValueTask.CompletedTask;
        }
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019d0000-0000-7800-8800-000000008801");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Commands.CommandIdKind> CommandId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Commands.CommandIdKind>.Parse(
                "019d0000-0000-7900-8900-000000008802");
}
