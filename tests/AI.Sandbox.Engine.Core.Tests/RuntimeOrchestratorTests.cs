namespace AI.Sandbox.Engine.Core.Tests;

public sealed class RuntimeOrchestratorTests
{
    private sealed record CounterState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    private readonly record struct AddValue(int Amount) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    [Xunit.Fact]
    public void BuilderCreatesOneRuntimeAndCannotBeReused()
    {
        var manager = CreateManager();
        var builder =
            new global::AI.Sandbox.Engine.Core.Runtime
                .RuntimeOrchestratorBuilder<CounterState>()
                .AddCommandHandler(new AddHandler())
                .AddSimulationSystem(
                    CreateSystemId(),
                    new IncrementSystem());

        var runtime = builder.Build(manager);

        Xunit.Assert.Same(manager.Read(), runtime.Read());
        Xunit.Assert.Throws<InvalidOperationException>(
            () => builder.AddCommandHandler(new AddHandler()));
        Xunit.Assert.Throws<InvalidOperationException>(
            () => builder.AddSimulationSystem(
                CreateOtherSystemId(),
                new IncrementSystem()));
        Xunit.Assert.Throws<InvalidOperationException>(
            () => builder.Build(manager));
    }

    [Xunit.Fact]
    public async Task CommandCommitReturnsFactButDoesNotDispatchAutomatically()
    {
        var manager = CreateManager();
        var runtime = CreateRuntime(manager);
        var observed = runtime.Read();
        var calls = new List<
            global::AI.Sandbox.Engine.Core.Runtime.RuntimeCommitFact>();
        var dispatcher =
            new global::AI.Sandbox.Engine.Core.Events
                .EventDispatcherBuilder()
                .Add<
                    global::AI.Sandbox.Engine.Core.Runtime
                        .RuntimeCommitFact>(
                    new CommitFactHandler(calls))
                .Build();

        var result = runtime.ExecuteCommand(
            CreateEnvelope(
                observed,
                amount: 5,
                commandSuffix: 1));

        Xunit.Assert.True(result.WasInvoked);
        Xunit.Assert.True(result.WasCommitted);
        Xunit.Assert.NotNull(result.CommandResult);
        Xunit.Assert.True(result.CommandResult!.WasApplied);
        Xunit.Assert.Empty(calls);
        Xunit.Assert.NotNull(result.CommitFact);
        var commitFact = result.CommitFact!;
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Runtime
                .RuntimeCommitKind.Command,
            commitFact.Kind);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateVersion.Initial,
            commitFact.PreviousWorldStateVersion);
        Xunit.Assert.Equal(
            1UL,
            commitFact.CurrentWorldStateVersion.Value);
        Xunit.Assert.Equal(0UL, commitFact.PreviousSimulationTick);
        Xunit.Assert.Equal(0UL, commitFact.CurrentSimulationTick);
        Xunit.Assert.True(commitFact.CommandId.HasValue);
        Xunit.Assert.Equal(
            CreateCommandId(1),
            commitFact.CommandId.Value);

        var envelope = global::AI.Sandbox.Engine.Core.Events
            .EventEnvelope<
                global::AI.Sandbox.Engine.Core.Runtime
                    .RuntimeCommitFact>.Create(
                        CreateEventId(),
                        sequence: 1,
                        simulationTick:
                            commitFact.CurrentSimulationTick,
                        commitFact);

        await dispatcher.DispatchAsync(envelope);

        Xunit.Assert.Single(calls);
        Xunit.Assert.Same(commitFact, calls[0]);
    }

    [Xunit.Fact]
    public void TickCommitReturnsExactVersionAndTickFact()
    {
        var manager = CreateManager();
        var runtime = CreateRuntime(manager);

        var result = runtime.RunNextTick();

        Xunit.Assert.True(result.WasInvoked);
        Xunit.Assert.True(result.WasCommitted);
        Xunit.Assert.NotNull(result.SimulationResult);
        Xunit.Assert.True(result.SimulationResult!.WasApplied);
        Xunit.Assert.NotNull(result.CommitFact);
        var commitFact = result.CommitFact!;
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Runtime
                .RuntimeCommitKind.SimulationTick,
            commitFact.Kind);
        Xunit.Assert.Null(commitFact.CommandId);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateVersion.Initial,
            commitFact.PreviousWorldStateVersion);
        Xunit.Assert.Equal(
            1UL,
            commitFact.CurrentWorldStateVersion.Value);
        Xunit.Assert.Equal(0UL, commitFact.PreviousSimulationTick);
        Xunit.Assert.Equal(1UL, commitFact.CurrentSimulationTick);
        Xunit.Assert.Equal(100, manager.Read().State.Value);
    }

    [Xunit.Fact]
    public void RejectedCommandAndTickProduceNoCommitFact()
    {
        var commandManager = CreateManager();
        var commandRuntime =
            new global::AI.Sandbox.Engine.Core.Runtime
                .RuntimeOrchestratorBuilder<CounterState>()
                .AddCommandHandler<AddValue>(
                    new RejectingHandler())
                .Build(commandManager);
        var command = commandRuntime.ExecuteCommand(
            CreateEnvelope(
                commandRuntime.Read(),
                amount: -1,
                commandSuffix: 2));

        Xunit.Assert.True(command.WasInvoked);
        Xunit.Assert.False(command.WasCommitted);
        Xunit.Assert.NotNull(command.CommandResult);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Commands
                .CommandExecutionStatus.Rejected,
            command.CommandResult!.Status);
        Xunit.Assert.Null(command.CommitFact);
        Xunit.Assert.Equal(0, commandManager.Read().State.Value);

        var tickManager = CreateManager();
        var tickRuntime =
            new global::AI.Sandbox.Engine.Core.Runtime
                .RuntimeOrchestratorBuilder<CounterState>()
                .AddSimulationSystem(
                    CreateSystemId(),
                    new RejectingSystem())
                .Build(tickManager);
        var tick = tickRuntime.RunNextTick();

        Xunit.Assert.True(tick.WasInvoked);
        Xunit.Assert.False(tick.WasCommitted);
        Xunit.Assert.NotNull(tick.SimulationResult);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Simulation
                .SimulationStepStatus.SystemRejected,
            tick.SimulationResult!.Status);
        Xunit.Assert.Null(tick.CommitFact);
        Xunit.Assert.Equal(0UL, tickManager.Read().Version.Value);
        Xunit.Assert.Equal(0UL, tickManager.Read().SimulationTick);
    }

    [Xunit.Fact]
    public async Task ConcurrentInvocationReturnsBusyWithoutQueueing()
    {
        using var entered = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();
        var manager = CreateManager();
        var runtime =
            new global::AI.Sandbox.Engine.Core.Runtime
                .RuntimeOrchestratorBuilder<CounterState>()
                .AddCommandHandler<AddValue>(
                    new BlockingHandler(entered, release))
                .AddSimulationSystem(
                    CreateSystemId(),
                    new IncrementSystem())
                .Build(manager);
        var commandTask = Task.Run(
            () => runtime.ExecuteCommand(
                CreateEnvelope(
                    runtime.Read(),
                    amount: 1,
                    commandSuffix: 3)));

        Xunit.Assert.True(entered.Wait(TimeSpan.FromSeconds(5)));

        var busyTick = runtime.RunNextTick();

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Runtime
                .RuntimeInvocationStatus.Busy,
            busyTick.InvocationStatus);
        Xunit.Assert.False(busyTick.WasInvoked);
        Xunit.Assert.False(busyTick.WasCommitted);
        Xunit.Assert.Null(busyTick.SimulationResult);
        Xunit.Assert.Null(busyTick.CommitFact);
        Xunit.Assert.Equal(0, busyTick.Snapshot.State.Value);

        release.Set();
        var command = await commandTask;

        Xunit.Assert.True(command.WasCommitted);
        Xunit.Assert.Equal(1, manager.Read().State.Value);
        Xunit.Assert.Equal(0UL, manager.Read().SimulationTick);
    }

    [Xunit.Fact]
    public async Task ExternalConflictProducesNoFactAndDoesNotRetry()
    {
        using var entered = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();
        var handler = new BlockingHandler(entered, release);
        var manager = CreateManager();
        var runtime =
            new global::AI.Sandbox.Engine.Core.Runtime
                .RuntimeOrchestratorBuilder<CounterState>()
                .AddCommandHandler<AddValue>(handler)
                .Build(manager);
        var observed = runtime.Read();
        var commandTask = Task.Run(
            () => runtime.ExecuteCommand(
                CreateEnvelope(
                    observed,
                    amount: 1,
                    commandSuffix: 4)));

        Xunit.Assert.True(entered.Wait(TimeSpan.FromSeconds(5)));

        var external = manager.TryApply(
            observed.Version,
            observed.SimulationTick,
            new ExternalSetTransition(50));

        release.Set();
        var result = await commandTask;

        Xunit.Assert.True(external.WasApplied);
        Xunit.Assert.True(result.WasInvoked);
        Xunit.Assert.False(result.WasCommitted);
        Xunit.Assert.NotNull(result.CommandResult);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Commands
                .CommandExecutionStatus.VersionConflict,
            result.CommandResult!.Status);
        Xunit.Assert.Null(result.CommitFact);
        Xunit.Assert.Equal(1, handler.EvaluationCount);
        Xunit.Assert.Equal(50, manager.Read().State.Value);
    }

    [Xunit.Fact]
    public void SequentialOperationsFollowExplicitCallerOrder()
    {
        var manager = CreateManager();
        var runtime = CreateRuntime(manager);

        var firstCommand = runtime.ExecuteCommand(
            CreateEnvelope(
                runtime.Read(),
                amount: 5,
                commandSuffix: 5));
        var firstTick = runtime.RunNextTick();
        var secondCommand = runtime.ExecuteCommand(
            CreateEnvelope(
                runtime.Read(),
                amount: 2,
                commandSuffix: 6));
        var secondTick = runtime.RunNextTick();

        Xunit.Assert.True(firstCommand.WasCommitted);
        Xunit.Assert.True(firstTick.WasCommitted);
        Xunit.Assert.True(secondCommand.WasCommitted);
        Xunit.Assert.True(secondTick.WasCommitted);
        Xunit.Assert.Equal(207, manager.Read().State.Value);
        Xunit.Assert.Equal(4UL, manager.Read().Version.Value);
        Xunit.Assert.Equal(2UL, manager.Read().SimulationTick);
        Xunit.Assert.Equal(
            new[]
            {
                global::AI.Sandbox.Engine.Core.Runtime
                    .RuntimeCommitKind.Command,
                global::AI.Sandbox.Engine.Core.Runtime
                    .RuntimeCommitKind.SimulationTick,
                global::AI.Sandbox.Engine.Core.Runtime
                    .RuntimeCommitKind.Command,
                global::AI.Sandbox.Engine.Core.Runtime
                    .RuntimeCommitKind.SimulationTick,
            },
            new[]
            {
                firstCommand.CommitFact!.Kind,
                firstTick.CommitFact!.Kind,
                secondCommand.CommitFact!.Kind,
                secondTick.CommitFact!.Kind,
            });
    }

    [Xunit.Fact]
    public void SaveRestoreContinuationWithRuntimeOperationsIsByteIdentical()
    {
        var uninterrupted = RunScenario(
            cycles: 20,
            checkpointCycle: null);
        var resumed = RunScenario(
            cycles: 20,
            checkpointCycle: 7);

        Xunit.Assert.Equal(
            uninterrupted.WorldId,
            resumed.WorldId);
        Xunit.Assert.Equal(
            uninterrupted.WorldStateVersion,
            resumed.WorldStateVersion);
        Xunit.Assert.Equal(
            uninterrupted.SimulationTick,
            resumed.SimulationTick);
        Xunit.Assert.Equal(
            uninterrupted.Checksum,
            resumed.Checksum);
        Xunit.Assert.True(
            uninterrupted.Payload.ContentEquals(
                resumed.Payload));
    }

    private static global::AI.Sandbox.Engine.Core.Persistence
        .WorldSnapshotDocument RunScenario(
            int cycles,
            int? checkpointCycle)
    {
        var persistence =
            new global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateSnapshotPersistence<CounterState>(
                    new CounterCodec());
        var manager = CreateManager();
        var runtime = CreateRuntime(manager);

        if (checkpointCycle is null)
        {
            RunCycles(runtime, cycles, startCycle: 0);
        }
        else
        {
            RunCycles(
                runtime,
                checkpointCycle.Value,
                startCycle: 0);
            var checkpoint = persistence.Capture(runtime.Read());
            var restored = persistence.Restore(checkpoint);

            Xunit.Assert.True(restored.WasRestored);
            Xunit.Assert.NotNull(restored.Snapshot);

            manager = global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<CounterState>.Restore(
                    restored.Snapshot!);
            runtime = CreateRuntime(manager);
            RunCycles(
                runtime,
                cycles - checkpointCycle.Value,
                startCycle: checkpointCycle.Value);
        }

        return persistence.Capture(runtime.Read());
    }

    private static void RunCycles(
        global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestrator<CounterState> runtime,
        int count,
        int startCycle)
    {
        for (var offset = 0; offset < count; offset++)
        {
            var cycle = startCycle + offset;
            var command = runtime.ExecuteCommand(
                CreateEnvelope(
                    runtime.Read(),
                    amount: 3,
                    commandSuffix: cycle + 10));
            var tick = runtime.RunNextTick();

            Xunit.Assert.True(command.WasCommitted);
            Xunit.Assert.True(tick.WasCommitted);
        }
    }

    private static global::AI.Sandbox.Engine.Core.Runtime
        .RuntimeOrchestrator<CounterState> CreateRuntime(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<CounterState> manager)
    {
        return new global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestratorBuilder<CounterState>()
            .AddCommandHandler(new AddHandler())
            .AddSimulationSystem(
                CreateSystemId(),
                new IncrementSystem())
            .Build(manager);
    }

    private static global::AI.Sandbox.Engine.Core.Commands
        .CommandEnvelope<AddValue> CreateEnvelope(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateSnapshot<CounterState> snapshot,
            int amount,
            int commandSuffix)
    {
        return global::AI.Sandbox.Engine.Core.Commands
            .CommandEnvelope<AddValue>.Create(
                CreateCommandId(commandSuffix),
                snapshot.WorldId,
                snapshot.Version,
                snapshot.SimulationTick,
                new AddValue(amount));
    }

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<CounterState> CreateManager()
    {
        return global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<CounterState>.Create(
                CreateWorldId(),
                new CounterState(0));
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> CreateWorldId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000001100");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Commands.CommandIdKind> CreateCommandId(
            int suffix)
    {
        var text = $"019b0000-0000-7000-9200-{suffix:D12}";
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Commands.CommandIdKind>.Parse(text);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Simulation.SimulationSystemIdKind>
        CreateSystemId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemIdKind>.Parse(
                    "019b0000-0000-7000-9300-000000000001");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Simulation.SimulationSystemIdKind>
        CreateOtherSystemId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemIdKind>.Parse(
                    "019b0000-0000-7000-9300-000000000002");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Events.EventIdKind> CreateEventId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Events.EventIdKind>.Parse(
                "019b0000-0000-7000-9400-000000000001");
    }

    private sealed class AddHandler :
        global::AI.Sandbox.Engine.Core.Commands
            .ICommandHandler<CounterState, AddValue>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<CounterState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<CounterState, AddValue> context)
        {
            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<CounterState>.Accept(
                    context.Snapshot.State with
                    {
                        Value = context.Snapshot.State.Value +
                            context.Envelope.Payload.Amount,
                    });
        }
    }

    private sealed class RejectingHandler :
        global::AI.Sandbox.Engine.Core.Commands
            .ICommandHandler<CounterState, AddValue>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<CounterState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<CounterState, AddValue> context)
        {
            _ = context;

            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<CounterState>.Reject(
                    "rejected by test");
        }
    }

    private sealed class BlockingHandler :
        global::AI.Sandbox.Engine.Core.Commands
            .ICommandHandler<CounterState, AddValue>
    {
        private readonly ManualResetEventSlim entered;
        private readonly ManualResetEventSlim release;

        public BlockingHandler(
            ManualResetEventSlim entered,
            ManualResetEventSlim release)
        {
            this.entered = entered;
            this.release = release;
        }

        public int EvaluationCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<CounterState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<CounterState, AddValue> context)
        {
            EvaluationCount++;
            entered.Set();

            if (!release.Wait(TimeSpan.FromSeconds(5)))
            {
                throw new TimeoutException(
                    "The runtime test did not release the handler.");
            }

            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<CounterState>.Accept(
                    context.Snapshot.State with
                    {
                        Value = context.Snapshot.State.Value +
                            context.Envelope.Payload.Amount,
                    });
        }
    }

    private sealed class IncrementSystem :
        global::AI.Sandbox.Engine.Core.Simulation
            .ISimulationSystem<CounterState>
    {
        public global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<CounterState> Execute(
                global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemContext<CounterState> context)
        {
            return global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemDecision<CounterState>.Update(
                    context.State with
                    {
                        Value = context.State.Value + 100,
                    });
        }
    }

    private sealed class RejectingSystem :
        global::AI.Sandbox.Engine.Core.Simulation
            .ISimulationSystem<CounterState>
    {
        public global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<CounterState> Execute(
                global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemContext<CounterState> context)
        {
            _ = context;

            return global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemDecision<CounterState>.Reject(
                    "rejected by test");
        }
    }

    private sealed class ExternalSetTransition :
        global::AI.Sandbox.Engine.Core.WorldState
            .IWorldStateTransition<CounterState>
    {
        private readonly int value;

        public ExternalSetTransition(int value)
        {
            this.value = value;
        }

        public global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateTransitionDecision<CounterState> Evaluate(
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateSnapshot<CounterState> current)
        {
            return global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateTransitionDecision<CounterState>.Accept(
                    current.State with
                    {
                        Value = value,
                    });
        }
    }

    private sealed class CommitFactHandler :
        global::AI.Sandbox.Engine.Core.Events.IEventHandler<
            global::AI.Sandbox.Engine.Core.Runtime.RuntimeCommitFact>
    {
        private readonly List<
            global::AI.Sandbox.Engine.Core.Runtime.RuntimeCommitFact> calls;

        public CommitFactHandler(
            List<
                global::AI.Sandbox.Engine.Core.Runtime.RuntimeCommitFact> calls)
        {
            this.calls = calls;
        }

        public ValueTask HandleAsync(
            global::AI.Sandbox.Engine.Core.Events.EventEnvelope<
                global::AI.Sandbox.Engine.Core.Runtime
                    .RuntimeCommitFact> envelope,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            calls.Add(envelope.Payload);

            return ValueTask.CompletedTask;
        }
    }

    private sealed class CounterCodec :
        global::AI.Sandbox.Engine.Core.Persistence
            .IWorldStateSnapshotCodec<CounterState>
    {
        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaId SchemaId { get; } =
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaId.Parse("runtime.validation");

        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaVersion CurrentSchemaVersion { get; } =
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion.From(1);

        public bool CanDecode(
            global::AI.Sandbox.Engine.Core.Persistence
                .PersistenceSchemaVersion version)
        {
            return version == CurrentSchemaVersion;
        }

        public global::AI.Sandbox.Engine.Core.Persistence
            .SnapshotPayload Encode(CounterState state)
        {
            var text = state.Value.ToString(
                System.Globalization.CultureInfo.InvariantCulture);

            return global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotPayload.From(
                    System.Text.Encoding.UTF8.GetBytes(text));
        }

        public global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<CounterState> Decode(
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion version,
                global::AI.Sandbox.Engine.Core.Persistence
                    .SnapshotPayload payload)
        {
            if (version != CurrentSchemaVersion)
            {
                return Reject("Unsupported schema version.");
            }

            var text = System.Text.Encoding.UTF8.GetString(
                payload.ToArray());

            if (!int.TryParse(
                text,
                System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture,
                out var value))
            {
                return Reject("Malformed runtime state.");
            }

            return global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateDecodeDecision<CounterState>.Accept(
                    new CounterState(value));
        }

        private static global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<CounterState> Reject(
                string reason)
        {
            return global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateDecodeDecision<CounterState>.Reject(reason);
        }
    }
}
