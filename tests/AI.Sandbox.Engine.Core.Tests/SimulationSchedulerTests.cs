namespace AI.Sandbox.Engine.Core.Tests;

public sealed class SimulationSchedulerTests
{
    private sealed record CounterState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    [Xunit.Fact]
    public void DecisionFactories_CreateConsistentDecisions()
    {
        var unchanged = global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<CounterState>.Unchanged();
        var updatedState = new CounterState(2);
        var updated = global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<CounterState>.Update(updatedState);
        var rejected = global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<CounterState>.Reject("blocked");

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemStatus.Unchanged,
            unchanged.Status);
        Xunit.Assert.Null(unchanged.NextState);
        Xunit.Assert.Null(unchanged.RejectionReason);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemStatus.Updated,
            updated.Status);
        Xunit.Assert.Same(updatedState, updated.NextState);
        Xunit.Assert.Null(updated.RejectionReason);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemStatus.Rejected,
            rejected.Status);
        Xunit.Assert.Null(rejected.NextState);
        Xunit.Assert.Equal("blocked", rejected.RejectionReason);
    }

    [Xunit.Fact]
    public void DecisionFactories_RejectInvalidArguments()
    {
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemDecision<CounterState>.Update(null!));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemDecision<CounterState>.Reject(" "));
    }

    [Xunit.Fact]
    public void Builder_RejectsEmptyAndDuplicateSystemIds()
    {
        var builder = new global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSchedulerBuilder<CounterState>();
        var system = new AddSystem(1);

        Xunit.Assert.Throws<ArgumentException>(
            () => builder.Add(default, system));

        var systemId = CreateSystemId(1);
        _ = builder.Add(systemId, system);

        Xunit.Assert.Throws<ArgumentException>(
            () => builder.Add(systemId, new AddSystem(2)));
    }

    [Xunit.Fact]
    public void Builder_RejectsNullSystemAndManager()
    {
        var builder = new global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSchedulerBuilder<CounterState>();
        var systemId = CreateSystemId(1);

        Xunit.Assert.Throws<ArgumentNullException>(
            () => builder.Add(systemId, null!));
        Xunit.Assert.Throws<ArgumentNullException>(
            () => builder.Build(null!));
    }

    [Xunit.Fact]
    public void Builder_CannotBeReusedAfterBuild()
    {
        var manager = CreateManager(new CounterState(0));
        var builder = new global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSchedulerBuilder<CounterState>();
        _ = builder.Add(CreateSystemId(1), new AddSystem(1));

        var scheduler = builder.Build(manager);

        Xunit.Assert.Equal(1, scheduler.SystemCount);
        Xunit.Assert.Throws<InvalidOperationException>(
            () => builder.Add(CreateSystemId(2), new AddSystem(2)));
        Xunit.Assert.Throws<InvalidOperationException>(
            () => builder.Build(manager));
    }

    [Xunit.Fact]
    public void EmptyScheduler_AdvancesOneTickAndVersion()
    {
        var manager = CreateManager(new CounterState(5), 10);
        var scheduler = BuildScheduler(manager);

        var result = scheduler.RunNextTick();

        Xunit.Assert.True(result.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Simulation
                .SimulationStepStatus.Applied,
            result.Status);
        Xunit.Assert.Equal(11UL, result.TargetSimulationTick);
        Xunit.Assert.Equal(11UL, result.Snapshot.SimulationTick);
        Xunit.Assert.Equal(1UL, result.Snapshot.Version.Value);
        Xunit.Assert.Equal(0, result.ExecutedSystemCount);
        Xunit.Assert.Equal(5, result.Snapshot.State.Value);
    }

    [Xunit.Fact]
    public void Systems_ExecuteSequentiallyInRegistrationOrder()
    {
        var calls = new List<string>();
        var firstId = CreateSystemId(1);
        var secondId = CreateSystemId(2);
        var first = new RecordingAddSystem(
            "first",
            2,
            calls);
        var second = new RecordingAddSystem(
            "second",
            3,
            calls);
        var manager = CreateManager(new CounterState(1));
        var scheduler = BuildScheduler(
            manager,
            (firstId, first),
            (secondId, second));

        var result = scheduler.RunNextTick();

        Xunit.Assert.Equal(new[] { "first", "second" }, calls);
        Xunit.Assert.Equal(6, result.Snapshot.State.Value);
        Xunit.Assert.Equal(2, result.ExecutedSystemCount);
        Xunit.Assert.Equal(1UL, result.Snapshot.SimulationTick);
        Xunit.Assert.Equal(1UL, result.Snapshot.Version.Value);
    }

    [Xunit.Fact]
    public void Context_ContainsStableTickAndSystemMetadata()
    {
        var systemId = CreateSystemId(3);
        var capture = new ContextCaptureSystem();
        var manager = CreateManager(new CounterState(4), 20);
        var scheduler = BuildScheduler(manager, (systemId, capture));

        var result = scheduler.RunNextTick();

        Xunit.Assert.NotNull(capture.Context);
        var context = capture.Context!;
        Xunit.Assert.Equal(CreateWorldId(), context.WorldId);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateVersion.Initial,
            context.StartingVersion);
        Xunit.Assert.Equal(20UL, context.PreviousSimulationTick);
        Xunit.Assert.Equal(21UL, context.TargetSimulationTick);
        Xunit.Assert.Equal(systemId, context.SystemId);
        Xunit.Assert.Equal(0, context.SystemIndex);
        Xunit.Assert.Equal(4, context.State.Value);
        Xunit.Assert.True(result.WasApplied);
    }

    [Xunit.Fact]
    public void UnchangedSystem_PreservesWorkingStateReference()
    {
        var initial = new CounterState(7);
        var manager = CreateManager(initial);
        var scheduler = BuildScheduler(
            manager,
            (CreateSystemId(1), new UnchangedSystem()));

        var result = scheduler.RunNextTick();

        Xunit.Assert.True(result.WasApplied);
        Xunit.Assert.Same(initial, result.Snapshot.State);
        Xunit.Assert.Equal(1, result.ExecutedSystemCount);
    }

    [Xunit.Fact]
    public void RejectedSystem_AbortsWholeTickAndLaterSystems()
    {
        var calls = new List<string>();
        var firstId = CreateSystemId(1);
        var rejectingId = CreateSystemId(2);
        var laterId = CreateSystemId(3);
        var initial = new CounterState(10);
        var manager = CreateManager(initial);
        var scheduler = BuildScheduler(
            manager,
            (
                firstId,
                new RecordingAddSystem("first", 5, calls)
            ),
            (
                rejectingId,
                new RejectingSystem("cannot advance", calls)
            ),
            (
                laterId,
                new RecordingAddSystem("later", 100, calls)
            ));

        var result = scheduler.RunNextTick();

        Xunit.Assert.False(result.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Simulation
                .SimulationStepStatus.SystemRejected,
            result.Status);
        Xunit.Assert.Equal(new[] { "first", "reject" }, calls);
        Xunit.Assert.Equal(2, result.ExecutedSystemCount);
        Xunit.Assert.Equal(rejectingId, result.RejectedSystemId);
        Xunit.Assert.Equal("cannot advance", result.RejectionReason);
        Xunit.Assert.Same(initial, result.Snapshot.State);
        Xunit.Assert.Equal(0UL, result.Snapshot.Version.Value);
        Xunit.Assert.Equal(0UL, result.Snapshot.SimulationTick);
        Xunit.Assert.Same(result.Snapshot, manager.Read());
    }

    [Xunit.Fact]
    public void SystemException_LeavesAuthoritativeStateUnchanged()
    {
        var initial = new CounterState(3);
        var manager = CreateManager(initial);
        var scheduler = BuildScheduler(
            manager,
            (CreateSystemId(1), new AddSystem(5)),
            (CreateSystemId(2), new ThrowingSystem()));

        var exception = Xunit.Assert.Throws<InvalidOperationException>(
            () => scheduler.RunNextTick());

        Xunit.Assert.Equal("system failure", exception.Message);
        Xunit.Assert.Same(initial, manager.Read().State);
        Xunit.Assert.Equal(0UL, manager.Read().Version.Value);
        Xunit.Assert.Equal(0UL, manager.Read().SimulationTick);
    }

    [Xunit.Fact]
    public void NullSystemDecision_LeavesAuthoritativeStateUnchanged()
    {
        var initial = new CounterState(3);
        var manager = CreateManager(initial);
        var scheduler = BuildScheduler(
            manager,
            (CreateSystemId(1), new NullDecisionSystem()));

        Xunit.Assert.Throws<InvalidOperationException>(
            () => scheduler.RunNextTick());

        Xunit.Assert.Same(initial, manager.Read().State);
        Xunit.Assert.Equal(0UL, manager.Read().Version.Value);
    }

    [Xunit.Fact]
    public void TickOverflowOccursBeforeSystemExecution()
    {
        var system = new CountingUnchangedSystem();
        var manager = CreateManager(
            new CounterState(0),
            ulong.MaxValue);
        var scheduler = BuildScheduler(
            manager,
            (CreateSystemId(1), system));

        Xunit.Assert.Throws<OverflowException>(
            () => scheduler.RunNextTick());

        Xunit.Assert.Equal(0, system.ExecutionCount);
        Xunit.Assert.Equal(ulong.MaxValue, manager.Read().SimulationTick);
        Xunit.Assert.Equal(0UL, manager.Read().Version.Value);
    }

    [Xunit.Fact]
    public async Task ExternalWorldStateCommit_CausesConflictWithoutRetry()
    {
        using var entered = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();
        var blocking = new BlockingAddSystem(entered, release);
        var manager = CreateManager(new CounterState(0));
        var scheduler = BuildScheduler(
            manager,
            (CreateSystemId(1), blocking));

        var schedulerTask = Task.Run(scheduler.RunNextTick);

        Xunit.Assert.True(entered.Wait(TimeSpan.FromSeconds(5)));

        var external = manager.TryApply(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateVersion.Initial,
            1,
            new ExternalSetTransition(50));

        release.Set();
        var schedulerResult = await schedulerTask;

        Xunit.Assert.True(external.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Simulation
                .SimulationStepStatus.VersionConflict,
            schedulerResult.Status);
        Xunit.Assert.False(schedulerResult.WasApplied);
        Xunit.Assert.Equal(1, schedulerResult.ExecutedSystemCount);
        Xunit.Assert.Equal(1, blocking.ExecutionCount);
        Xunit.Assert.Equal(50, manager.Read().State.Value);
        Xunit.Assert.Equal(1UL, manager.Read().Version.Value);
        Xunit.Assert.Equal(1UL, manager.Read().SimulationTick);
    }

    [Xunit.Fact]
    public async Task ConcurrentSchedulerCalls_AreSerializedIntoDistinctTicks()
    {
        using var firstEntered = new ManualResetEventSlim();
        using var releaseFirst = new ManualResetEventSlim();
        var system = new FirstCallBlockingSystem(
            firstEntered,
            releaseFirst);
        var manager = CreateManager(new CounterState(0));
        var scheduler = BuildScheduler(
            manager,
            (CreateSystemId(1), system));

        var firstTask = Task.Run(scheduler.RunNextTick);
        Xunit.Assert.True(firstEntered.Wait(TimeSpan.FromSeconds(5)));

        using var secondStarted = new ManualResetEventSlim();
        var secondTask = Task.Run(
            () =>
            {
                secondStarted.Set();
                return scheduler.RunNextTick();
            });
        Xunit.Assert.True(secondStarted.Wait(TimeSpan.FromSeconds(5)));
        Xunit.Assert.False(secondTask.IsCompleted);

        releaseFirst.Set();

        var first = await firstTask;
        var second = await secondTask;

        Xunit.Assert.True(first.WasApplied);
        Xunit.Assert.True(second.WasApplied);
        Xunit.Assert.Equal(1UL, first.Snapshot.SimulationTick);
        Xunit.Assert.Equal(2UL, second.Snapshot.SimulationTick);
        Xunit.Assert.Equal(2, manager.Read().State.Value);
        Xunit.Assert.Equal(2UL, manager.Read().Version.Value);
        Xunit.Assert.Equal(2, system.ExecutionCount);
    }

    private static global::AI.Sandbox.Engine.Core.Simulation
        .SimulationScheduler<CounterState> BuildScheduler(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<CounterState> manager,
            params (
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.Simulation
                        .SimulationSystemIdKind> Id,
                global::AI.Sandbox.Engine.Core.Simulation
                    .ISimulationSystem<CounterState> System
            )[] systems)
    {
        var builder = new global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSchedulerBuilder<CounterState>();

        foreach (var system in systems)
        {
            _ = builder.Add(system.Id, system.System);
        }

        return builder.Build(manager);
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
                "019b0000-0000-7000-8000-000000000400");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Simulation.SimulationSystemIdKind>
        CreateSystemId(int suffix)
    {
        var text = $"019b0000-0000-7000-8300-{suffix:D12}";
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemIdKind>.Parse(text);
    }

    private sealed class AddSystem :
        global::AI.Sandbox.Engine.Core.Simulation
            .ISimulationSystem<CounterState>
    {
        private readonly int amount;

        public AddSystem(int amount)
        {
            this.amount = amount;
        }

        public global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<CounterState> Execute(
                global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemContext<CounterState> context)
        {
            return global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemDecision<CounterState>.Update(
                    context.State with
                    {
                        Value = context.State.Value + amount,
                    });
        }
    }

    private sealed class RecordingAddSystem :
        global::AI.Sandbox.Engine.Core.Simulation
            .ISimulationSystem<CounterState>
    {
        private readonly string name;
        private readonly int amount;
        private readonly List<string> calls;

        public RecordingAddSystem(
            string name,
            int amount,
            List<string> calls)
        {
            this.name = name;
            this.amount = amount;
            this.calls = calls;
        }

        public global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<CounterState> Execute(
                global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemContext<CounterState> context)
        {
            calls.Add(name);

            return global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemDecision<CounterState>.Update(
                    context.State with
                    {
                        Value = context.State.Value + amount,
                    });
        }
    }

    private sealed class ContextCaptureSystem :
        global::AI.Sandbox.Engine.Core.Simulation
            .ISimulationSystem<CounterState>
    {
        public global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemContext<CounterState>? Context { get; private set; }

        public global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<CounterState> Execute(
                global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemContext<CounterState> context)
        {
            Context = context;

            return global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemDecision<CounterState>.Unchanged();
        }
    }

    private sealed class UnchangedSystem :
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
                .SimulationSystemDecision<CounterState>.Unchanged();
        }
    }

    private sealed class CountingUnchangedSystem :
        global::AI.Sandbox.Engine.Core.Simulation
            .ISimulationSystem<CounterState>
    {
        public int ExecutionCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<CounterState> Execute(
                global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemContext<CounterState> context)
        {
            _ = context;
            ExecutionCount++;

            return global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemDecision<CounterState>.Unchanged();
        }
    }

    private sealed class RejectingSystem :
        global::AI.Sandbox.Engine.Core.Simulation
            .ISimulationSystem<CounterState>
    {
        private readonly string reason;
        private readonly List<string> calls;

        public RejectingSystem(
            string reason,
            List<string> calls)
        {
            this.reason = reason;
            this.calls = calls;
        }

        public global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<CounterState> Execute(
                global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemContext<CounterState> context)
        {
            _ = context;
            calls.Add("reject");

            return global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemDecision<CounterState>.Reject(reason);
        }
    }

    private sealed class ThrowingSystem :
        global::AI.Sandbox.Engine.Core.Simulation
            .ISimulationSystem<CounterState>
    {
        public global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<CounterState> Execute(
                global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemContext<CounterState> context)
        {
            _ = context;
            throw new InvalidOperationException("system failure");
        }
    }

    private sealed class NullDecisionSystem :
        global::AI.Sandbox.Engine.Core.Simulation
            .ISimulationSystem<CounterState>
    {
        public global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<CounterState> Execute(
                global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemContext<CounterState> context)
        {
            _ = context;
            return null!;
        }
    }

    private sealed class BlockingAddSystem :
        global::AI.Sandbox.Engine.Core.Simulation
            .ISimulationSystem<CounterState>
    {
        private readonly ManualResetEventSlim entered;
        private readonly ManualResetEventSlim release;

        public BlockingAddSystem(
            ManualResetEventSlim entered,
            ManualResetEventSlim release)
        {
            this.entered = entered;
            this.release = release;
        }

        public int ExecutionCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<CounterState> Execute(
                global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemContext<CounterState> context)
        {
            ExecutionCount++;
            entered.Set();

            if (!release.Wait(TimeSpan.FromSeconds(5)))
            {
                throw new TimeoutException(
                    "The external-conflict test did not release the system.");
            }

            return global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemDecision<CounterState>.Update(
                    context.State with
                    {
                        Value = context.State.Value + 1,
                    });
        }
    }

    private sealed class FirstCallBlockingSystem :
        global::AI.Sandbox.Engine.Core.Simulation
            .ISimulationSystem<CounterState>
    {
        private readonly ManualResetEventSlim firstEntered;
        private readonly ManualResetEventSlim releaseFirst;

        public FirstCallBlockingSystem(
            ManualResetEventSlim firstEntered,
            ManualResetEventSlim releaseFirst)
        {
            this.firstEntered = firstEntered;
            this.releaseFirst = releaseFirst;
        }

        public int ExecutionCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<CounterState> Execute(
                global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemContext<CounterState> context)
        {
            ExecutionCount++;

            if (ExecutionCount == 1)
            {
                firstEntered.Set();

                if (!releaseFirst.Wait(TimeSpan.FromSeconds(5)))
                {
                    throw new TimeoutException(
                        "The scheduler-serialization test did not release.");
                }
            }

            return global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemDecision<CounterState>.Update(
                    context.State with
                    {
                        Value = context.State.Value + 1,
                    });
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
}
