namespace AI.Sandbox.Engine.Core.Tests;

public sealed class CommandSystemTests
{
    private sealed record CounterState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    private sealed record AddValue(int Amount) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    private readonly record struct MultiplyValue(int Factor) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    private sealed record UnhandledCommand(string Value) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    private abstract record AbstractCommand :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    private record OpenReferenceCommand(int Value) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    [Xunit.Fact]
    public void Envelope_PreservesObservedWorldMetadata()
    {
        var commandId = CreateCommandId(1);
        var worldId = CreateWorldId();
        var version = global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateVersion.From(4);
        var payload = new AddValue(3);

        var envelope = global::AI.Sandbox.Engine.Core.Commands
            .CommandEnvelope<AddValue>.Create(
                commandId,
                worldId,
                version,
                expectedSimulationTick: 9,
                payload);

        Xunit.Assert.Equal(commandId, envelope.CommandId);
        Xunit.Assert.Equal(worldId, envelope.WorldId);
        Xunit.Assert.Equal(version, envelope.ExpectedWorldStateVersion);
        Xunit.Assert.Equal(9UL, envelope.ExpectedSimulationTick);
        Xunit.Assert.Same(payload, envelope.Payload);
    }

    [Xunit.Fact]
    public void Envelope_RejectsEmptyIdsAndNullPayload()
    {
        var worldId = CreateWorldId();
        var commandId = CreateCommandId(1);
        var version = global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateVersion.Initial;

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Commands
                .CommandEnvelope<AddValue>.Create(
                    default,
                    worldId,
                    version,
                    0,
                    new AddValue(1)));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Commands
                .CommandEnvelope<AddValue>.Create(
                    commandId,
                    default,
                    version,
                    0,
                    new AddValue(1)));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.Commands
                .CommandEnvelope<AddValue>.Create(
                    commandId,
                    worldId,
                    version,
                    0,
                    null!));
    }

    [Xunit.Fact]
    public void Builder_RejectsInvalidTypesDuplicateHandlersAndNulls()
    {
        var builder = new global::AI.Sandbox.Engine.Core.Commands
            .CommandProcessorBuilder<CounterState>();

        Xunit.Assert.Throws<ArgumentException>(
            () => builder.Add<AbstractCommand>(
                new AbstractCommandHandler()));
        Xunit.Assert.Throws<ArgumentException>(
            () => builder.Add(
                new OpenReferenceCommandHandler()));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => builder.Add<AddValue>(null!));

        _ = builder.Add(new AddHandler());
        Xunit.Assert.Throws<ArgumentException>(
            () => builder.Add(new AddHandler()));
    }

    [Xunit.Fact]
    public void Builder_CannotBeReusedAfterBuild()
    {
        var manager = CreateManager(new CounterState(0));
        var builder = new global::AI.Sandbox.Engine.Core.Commands
            .CommandProcessorBuilder<CounterState>();
        _ = builder.Add(new AddHandler());

        var processor = builder.Build(manager);

        Xunit.Assert.Equal(1, processor.HandlerCount);
        Xunit.Assert.Throws<InvalidOperationException>(
            () => builder.Add(new MultiplyHandler()));
        Xunit.Assert.Throws<InvalidOperationException>(
            () => builder.Build(manager));
    }

    [Xunit.Fact]
    public void UnregisteredCommand_DoesNotEvaluateOrChangeWorld()
    {
        var manager = CreateManager(new CounterState(4));
        var processor = new global::AI.Sandbox.Engine.Core.Commands
            .CommandProcessorBuilder<CounterState>()
            .Add(new AddHandler())
            .Build(manager);
        var before = manager.Read();

        var result = processor.Execute(
            CreateEnvelope(
                before,
                new UnhandledCommand("test"),
                commandSuffix: 2));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Commands
                .CommandExecutionStatus.HandlerNotRegistered,
            result.Status);
        Xunit.Assert.False(result.HandlerWasEvaluated);
        Xunit.Assert.False(result.WasApplied);
        Xunit.Assert.Same(before, result.Snapshot);
        Xunit.Assert.Same(before, manager.Read());
    }

    [Xunit.Fact]
    public void WorldMismatch_IsRejectedBeforeHandler()
    {
        var handler = new CountingAddHandler();
        var manager = CreateManager(new CounterState(4));
        var processor = BuildProcessor(manager, handler);
        var before = manager.Read();
        var envelope = global::AI.Sandbox.Engine.Core.Commands
            .CommandEnvelope<AddValue>.Create(
                CreateCommandId(1),
                CreateOtherWorldId(),
                before.Version,
                before.SimulationTick,
                new AddValue(2));

        var result = processor.Execute(envelope);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Commands
                .CommandExecutionStatus.WorldMismatch,
            result.Status);
        Xunit.Assert.False(result.HandlerWasEvaluated);
        Xunit.Assert.Equal(0, handler.EvaluationCount);
        Xunit.Assert.Same(before, manager.Read());
    }

    [Xunit.Fact]
    public void StaleVersion_IsRejectedBeforeHandler()
    {
        var handler = new CountingAddHandler();
        var manager = CreateManager(new CounterState(4));
        var processor = BuildProcessor(manager, handler);
        var stale = manager.Read();

        _ = manager.TryApply(
            stale.Version,
            stale.SimulationTick,
            new ExternalSetTransition(8));

        var result = processor.Execute(
            CreateEnvelope(
                stale,
                new AddValue(2),
                commandSuffix: 3));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Commands
                .CommandExecutionStatus.VersionConflict,
            result.Status);
        Xunit.Assert.False(result.HandlerWasEvaluated);
        Xunit.Assert.Equal(0, handler.EvaluationCount);
        Xunit.Assert.Equal(8, manager.Read().State.Value);
    }

    [Xunit.Fact]
    public void TickMismatch_IsRejectedBeforeHandler()
    {
        var handler = new CountingAddHandler();
        var manager = CreateManager(
            new CounterState(4),
            initialTick: 5);
        var processor = BuildProcessor(manager, handler);
        var before = manager.Read();
        var envelope = global::AI.Sandbox.Engine.Core.Commands
            .CommandEnvelope<AddValue>.Create(
                CreateCommandId(4),
                before.WorldId,
                before.Version,
                expectedSimulationTick: 4,
                new AddValue(2));

        var result = processor.Execute(envelope);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Commands
                .CommandExecutionStatus.SimulationTickMismatch,
            result.Status);
        Xunit.Assert.False(result.HandlerWasEvaluated);
        Xunit.Assert.Equal(0, handler.EvaluationCount);
        Xunit.Assert.Same(before, manager.Read());
    }

    [Xunit.Fact]
    public void AcceptedCommand_CommitsOnceAndPreservesLogicalTick()
    {
        var manager = CreateManager(
            new CounterState(4),
            initialTick: 7);
        var handler = new CountingAddHandler();
        var processor = BuildProcessor(manager, handler);
        var before = manager.Read();

        var result = processor.Execute(
            CreateEnvelope(
                before,
                new AddValue(3),
                commandSuffix: 5));

        Xunit.Assert.True(result.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Commands
                .CommandExecutionStatus.Applied,
            result.Status);
        Xunit.Assert.True(result.HandlerWasEvaluated);
        Xunit.Assert.Equal(1, handler.EvaluationCount);
        Xunit.Assert.Equal(7, result.Snapshot.State.Value);
        Xunit.Assert.Equal(1UL, result.Snapshot.Version.Value);
        Xunit.Assert.Equal(7UL, result.Snapshot.SimulationTick);
    }

    [Xunit.Fact]
    public void RejectedCommand_PreservesAuthoritativeSnapshot()
    {
        var manager = CreateManager(new CounterState(4));
        var handler = new RejectingHandler();
        var processor = new global::AI.Sandbox.Engine.Core.Commands
            .CommandProcessorBuilder<CounterState>()
            .Add<AddValue>(handler)
            .Build(manager);
        var before = manager.Read();

        var result = processor.Execute(
            CreateEnvelope(
                before,
                new AddValue(-1),
                commandSuffix: 6));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Commands
                .CommandExecutionStatus.Rejected,
            result.Status);
        Xunit.Assert.True(result.HandlerWasEvaluated);
        Xunit.Assert.Equal("negative amount", result.FailureReason);
        Xunit.Assert.Equal(1, handler.EvaluationCount);
        Xunit.Assert.Same(before, result.Snapshot);
        Xunit.Assert.Same(before, manager.Read());
    }

    [Xunit.Fact]
    public void HandlerException_PreservesAuthoritativeState()
    {
        var manager = CreateManager(new CounterState(4));
        var processor = new global::AI.Sandbox.Engine.Core.Commands
            .CommandProcessorBuilder<CounterState>()
            .Add<AddValue>(new ThrowingHandler())
            .Build(manager);
        var before = manager.Read();

        var exception = Xunit.Assert.Throws<InvalidOperationException>(
            () => processor.Execute(
                CreateEnvelope(
                    before,
                    new AddValue(1),
                    commandSuffix: 7)));

        Xunit.Assert.Equal("handler failure", exception.Message);
        Xunit.Assert.Same(before, manager.Read());
    }

    [Xunit.Fact]
    public void NullOrSameStateDecision_IsAHandlerContractFailure()
    {
        var nullManager = CreateManager(new CounterState(4));
        var nullProcessor =
            new global::AI.Sandbox.Engine.Core.Commands
                .CommandProcessorBuilder<CounterState>()
                .Add<AddValue>(new NullDecisionHandler())
                .Build(nullManager);
        var nullBefore = nullManager.Read();

        Xunit.Assert.Throws<InvalidOperationException>(
            () => nullProcessor.Execute(
                CreateEnvelope(
                    nullBefore,
                    new AddValue(1),
                    commandSuffix: 8)));
        Xunit.Assert.Same(nullBefore, nullManager.Read());

        var sameManager = CreateManager(new CounterState(4));
        var sameProcessor =
            new global::AI.Sandbox.Engine.Core.Commands
                .CommandProcessorBuilder<CounterState>()
                .Add<AddValue>(new SameStateHandler())
                .Build(sameManager);
        var sameBefore = sameManager.Read();

        Xunit.Assert.Throws<InvalidOperationException>(
            () => sameProcessor.Execute(
                CreateEnvelope(
                    sameBefore,
                    new AddValue(1),
                    commandSuffix: 9)));
        Xunit.Assert.Same(sameBefore, sameManager.Read());
    }

    [Xunit.Fact]
    public void ExactCommandTypesUseIndependentHandlers()
    {
        var manager = CreateManager(new CounterState(3));
        var processor =
            new global::AI.Sandbox.Engine.Core.Commands
                .CommandProcessorBuilder<CounterState>()
                .Add(new AddHandler())
                .Add(new MultiplyHandler())
                .Build(manager);

        var add = processor.Execute(
            CreateEnvelope(
                manager.Read(),
                new AddValue(2),
                commandSuffix: 10));
        var multiply = processor.Execute(
            CreateEnvelope(
                manager.Read(),
                new MultiplyValue(4),
                commandSuffix: 11));

        Xunit.Assert.True(add.WasApplied);
        Xunit.Assert.True(multiply.WasApplied);
        Xunit.Assert.Equal(20, manager.Read().State.Value);
        Xunit.Assert.Equal(2UL, manager.Read().Version.Value);
        Xunit.Assert.Equal(0UL, manager.Read().SimulationTick);
    }

    [Xunit.Fact]
    public async Task VersionConflictDuringEvaluation_DoesNotRetryHandler()
    {
        using var entered = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();
        var handler = new BlockingAddHandler(entered, release);
        var manager = CreateManager(new CounterState(0));
        var processor = BuildProcessor(manager, handler);
        var observed = manager.Read();
        var commandTask = Task.Run(
            () => processor.Execute(
                CreateEnvelope(
                    observed,
                    new AddValue(1),
                    commandSuffix: 12)));

        Xunit.Assert.True(entered.Wait(TimeSpan.FromSeconds(5)));

        var external = manager.TryApply(
            observed.Version,
            observed.SimulationTick,
            new ExternalSetTransition(50));

        release.Set();
        var result = await commandTask;

        Xunit.Assert.True(external.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Commands
                .CommandExecutionStatus.VersionConflict,
            result.Status);
        Xunit.Assert.True(result.HandlerWasEvaluated);
        Xunit.Assert.Equal(1, handler.EvaluationCount);
        Xunit.Assert.Equal(50, manager.Read().State.Value);
        Xunit.Assert.Equal(1UL, manager.Read().Version.Value);
    }

    [Xunit.Fact]
    public void SchedulerAndCommands_PreserveSeparateVersionAndTickSemantics()
    {
        var manager = CreateManager(new CounterState(0));
        var scheduler =
            new global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSchedulerBuilder<CounterState>()
                .Add(CreateSystemId(), new TickIncrementSystem())
                .Build(manager);
        var processor = BuildProcessor(manager, new AddHandler());

        var firstTick = scheduler.RunNextTick();
        var command = processor.Execute(
            CreateEnvelope(
                manager.Read(),
                new AddValue(10),
                commandSuffix: 13));
        var secondTick = scheduler.RunNextTick();

        Xunit.Assert.True(firstTick.WasApplied);
        Xunit.Assert.True(command.WasApplied);
        Xunit.Assert.True(secondTick.WasApplied);
        Xunit.Assert.Equal(12, manager.Read().State.Value);
        Xunit.Assert.Equal(3UL, manager.Read().Version.Value);
        Xunit.Assert.Equal(2UL, manager.Read().SimulationTick);
        Xunit.Assert.Equal(1UL, command.Snapshot.SimulationTick);
    }

    private static global::AI.Sandbox.Engine.Core.Commands
        .CommandProcessor<CounterState> BuildProcessor(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<CounterState> manager,
            global::AI.Sandbox.Engine.Core.Commands
                .ICommandHandler<CounterState, AddValue> handler)
    {
        return new global::AI.Sandbox.Engine.Core.Commands
            .CommandProcessorBuilder<CounterState>()
            .Add(handler)
            .Build(manager);
    }

    private static global::AI.Sandbox.Engine.Core.Commands
        .CommandEnvelope<TCommand> CreateEnvelope<TCommand>(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateSnapshot<CounterState> snapshot,
            TCommand command,
            int commandSuffix)
        where TCommand : notnull,
            global::AI.Sandbox.Engine.Core.Commands.IEngineCommand
    {
        return global::AI.Sandbox.Engine.Core.Commands
            .CommandEnvelope<TCommand>.Create(
                CreateCommandId(commandSuffix),
                snapshot.WorldId,
                snapshot.Version,
                snapshot.SimulationTick,
                command);
    }

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<CounterState> CreateManager(
            CounterState state,
            ulong initialTick = 0)
    {
        return global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<CounterState>.Create(
                CreateWorldId(),
                state,
                initialTick);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> CreateWorldId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000000800");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>
        CreateOtherWorldId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000000801");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Commands.CommandIdKind> CreateCommandId(
            int suffix)
    {
        var text = $"019b0000-0000-7000-8a00-{suffix:D12}";
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
                    "019b0000-0000-7000-8b00-000000000001");
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

    private sealed class CountingAddHandler :
        global::AI.Sandbox.Engine.Core.Commands
            .ICommandHandler<CounterState, AddValue>
    {
        public int EvaluationCount { get; private set; }

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

    private sealed class MultiplyHandler :
        global::AI.Sandbox.Engine.Core.Commands
            .ICommandHandler<CounterState, MultiplyValue>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<CounterState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<CounterState, MultiplyValue> context)
        {
            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<CounterState>.Accept(
                    context.Snapshot.State with
                    {
                        Value = context.Snapshot.State.Value *
                            context.Envelope.Payload.Factor,
                    });
        }
    }

    private sealed class RejectingHandler :
        global::AI.Sandbox.Engine.Core.Commands
            .ICommandHandler<CounterState, AddValue>
    {
        public int EvaluationCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<CounterState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<CounterState, AddValue> context)
        {
            _ = context;
            EvaluationCount++;

            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<CounterState>.Reject(
                    "negative amount");
        }
    }

    private sealed class ThrowingHandler :
        global::AI.Sandbox.Engine.Core.Commands
            .ICommandHandler<CounterState, AddValue>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<CounterState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<CounterState, AddValue> context)
        {
            _ = context;
            throw new InvalidOperationException("handler failure");
        }
    }

    private sealed class NullDecisionHandler :
        global::AI.Sandbox.Engine.Core.Commands
            .ICommandHandler<CounterState, AddValue>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<CounterState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<CounterState, AddValue> context)
        {
            _ = context;
            return null!;
        }
    }

    private sealed class SameStateHandler :
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
                    context.Snapshot.State);
        }
    }

    private sealed class BlockingAddHandler :
        global::AI.Sandbox.Engine.Core.Commands
            .ICommandHandler<CounterState, AddValue>
    {
        private readonly ManualResetEventSlim entered;
        private readonly ManualResetEventSlim release;

        public BlockingAddHandler(
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
                    "The command conflict test did not release the handler.");
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

    private sealed class AbstractCommandHandler :
        global::AI.Sandbox.Engine.Core.Commands
            .ICommandHandler<CounterState, AbstractCommand>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<CounterState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<CounterState, AbstractCommand> context)
        {
            _ = context;
            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<CounterState>.Reject("invalid type");
        }
    }

    private sealed class OpenReferenceCommandHandler :
        global::AI.Sandbox.Engine.Core.Commands
            .ICommandHandler<CounterState, OpenReferenceCommand>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<CounterState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<CounterState, OpenReferenceCommand> context)
        {
            _ = context;
            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<CounterState>.Reject("invalid type");
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

    private sealed class TickIncrementSystem :
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
                        Value = context.State.Value + 1,
                    });
        }
    }
}
