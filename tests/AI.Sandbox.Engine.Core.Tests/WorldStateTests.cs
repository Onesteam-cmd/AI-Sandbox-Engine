namespace AI.Sandbox.Engine.Core.Tests;

public sealed class WorldStateTests
{
    private sealed record CounterState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    [Xunit.Fact]
    public void Create_ProducesAuthoritativeInitialSnapshot()
    {
        var initialState = new CounterState(10);
        var manager = CreateManager(initialState, 25);

        var snapshot = manager.Read();

        Xunit.Assert.Equal(CreateWorldId(), snapshot.WorldId);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion.Initial,
            snapshot.Version);
        Xunit.Assert.True(snapshot.Version.IsInitial);
        Xunit.Assert.Equal(25UL, snapshot.SimulationTick);
        Xunit.Assert.Same(initialState, snapshot.State);
    }

    [Xunit.Fact]
    public void Create_RejectsEmptyWorldIdentifier()
    {
        var exception = Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<CounterState>.Create(
                    default,
                    new CounterState(0)));

        Xunit.Assert.Equal("worldId", exception.ParamName);
    }

    [Xunit.Fact]
    public void Create_RejectsNullInitialState()
    {
        var exception = Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<CounterState>.Create(
                    CreateWorldId(),
                    null!));

        Xunit.Assert.Equal("initialState", exception.ParamName);
    }

    [Xunit.Fact]
    public void Version_RoundTripsAndOrdersNumerically()
    {
        var lower = global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateVersion.From(4);
        var higher = global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateVersion.From(5);

        Xunit.Assert.True(lower.CompareTo(higher) < 0);
        Xunit.Assert.True(higher.CompareTo(lower) > 0);
        Xunit.Assert.Equal("5", higher.ToString());
    }

    [Xunit.Fact]
    public void Decision_AcceptsNonNullNextState()
    {
        var next = new CounterState(1);

        var decision = global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateTransitionDecision<CounterState>.Accept(next);

        Xunit.Assert.True(decision.IsAccepted);
        Xunit.Assert.Same(next, decision.NextState);
        Xunit.Assert.Null(decision.RejectionReason);
    }

    [Xunit.Fact]
    public void Decision_RejectsNullAcceptedState()
    {
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateTransitionDecision<CounterState>.Accept(null!));
    }

    [Xunit.Theory]
    [Xunit.InlineData("")]
    [Xunit.InlineData(" ")]
    public void Decision_RejectRequiresNonBlankReason(string reason)
    {
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateTransitionDecision<CounterState>.Reject(reason));
    }

    [Xunit.Fact]
    public void TryApply_CommitsOneAcceptedTransitionAtomically()
    {
        var manager = CreateManager(new CounterState(1));
        var transition = new IncrementTransition(2);

        var result = manager.TryApply(
            global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion.Initial,
            10,
            transition);

        Xunit.Assert.True(result.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateApplyStatus.Applied,
            result.Status);
        Xunit.Assert.Equal(1UL, result.Snapshot.Version.Value);
        Xunit.Assert.Equal(10UL, result.Snapshot.SimulationTick);
        Xunit.Assert.Equal(3, result.Snapshot.State.Value);
        Xunit.Assert.Same(result.Snapshot, manager.Read());
        Xunit.Assert.Equal(1, transition.EvaluationCount);
    }

    [Xunit.Fact]
    public void TryApply_VersionConflictDoesNotEvaluateTransition()
    {
        var manager = CreateManager(new CounterState(1));
        var transition = new IncrementTransition(1);
        var wrongVersion = global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateVersion.From(9);

        var result = manager.TryApply(wrongVersion, 1, transition);

        Xunit.Assert.False(result.WasApplied);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateApplyStatus.VersionConflict,
            result.Status);
        Xunit.Assert.Equal(0, transition.EvaluationCount);
        Xunit.Assert.Equal(0UL, manager.Read().Version.Value);
        Xunit.Assert.Equal(1, manager.Read().State.Value);
    }

    [Xunit.Fact]
    public void TryApply_TickRegressionDoesNotEvaluateTransition()
    {
        var manager = CreateManager(new CounterState(1), 10);
        var transition = new IncrementTransition(1);

        var result = manager.TryApply(
            global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion.Initial,
            9,
            transition);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateApplyStatus.SimulationTickRegression,
            result.Status);
        Xunit.Assert.Equal(0, transition.EvaluationCount);
        Xunit.Assert.Equal(10UL, manager.Read().SimulationTick);
    }

    [Xunit.Fact]
    public void TryApply_RejectedTransitionLeavesStateUnchanged()
    {
        var initial = new CounterState(7);
        var manager = CreateManager(initial);
        var transition = new RejectingTransition("insufficient authority");

        var result = manager.TryApply(
            global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion.Initial,
            1,
            transition);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateApplyStatus.Rejected,
            result.Status);
        Xunit.Assert.Equal("insufficient authority", result.RejectionReason);
        Xunit.Assert.Same(initial, result.Snapshot.State);
        Xunit.Assert.Same(result.Snapshot, manager.Read());
        Xunit.Assert.Equal(1, transition.EvaluationCount);
    }

    [Xunit.Fact]
    public void TryApply_TransitionExceptionLeavesStateUnchanged()
    {
        var initial = new CounterState(3);
        var manager = CreateManager(initial);
        var transition = new ThrowingTransition();

        var exception = Xunit.Assert.Throws<InvalidOperationException>(
            () => manager.TryApply(
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateVersion.Initial,
                1,
                transition));

        Xunit.Assert.Equal("transition failure", exception.Message);
        Xunit.Assert.Same(initial, manager.Read().State);
        Xunit.Assert.Equal(0UL, manager.Read().Version.Value);
    }

    [Xunit.Fact]
    public void TryApply_RejectsNullTransition()
    {
        var manager = CreateManager(new CounterState(0));

        var exception = Xunit.Assert.Throws<ArgumentNullException>(
            () => manager.TryApply(
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateVersion.Initial,
                0,
                null!));

        Xunit.Assert.Equal("transition", exception.ParamName);
    }

    [Xunit.Fact]
    public async Task ConcurrentSameVersionTransitions_CommitAtMostOnce()
    {
        var manager = CreateManager(new CounterState(0));
        using var entered = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();
        var blocked = new BlockingIncrementTransition(entered, release);

        var blockedTask = Task.Run(
            () => manager.TryApply(
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateVersion.Initial,
                1,
                blocked));

        Xunit.Assert.True(entered.Wait(TimeSpan.FromSeconds(5)));

        var winning = manager.TryApply(
            global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion.Initial,
            2,
            new IncrementTransition(10));

        release.Set();
        var losing = await blockedTask;

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateApplyStatus.Applied,
            winning.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateApplyStatus.VersionConflict,
            losing.Status);
        Xunit.Assert.Equal(10, manager.Read().State.Value);
        Xunit.Assert.Equal(1UL, manager.Read().Version.Value);
        Xunit.Assert.Equal(1, blocked.EvaluationCount);
    }

    [Xunit.Fact]
    public void TryApply_EvaluatesTransitionExactlyOnce()
    {
        var manager = CreateManager(new CounterState(1));
        var transition = new IncrementTransition(1);

        _ = manager.TryApply(
            global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion.Initial,
            1,
            transition);

        Xunit.Assert.Equal(1, transition.EvaluationCount);
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
                "019b0000-0000-7000-8000-000000000100");
    }

    private sealed class IncrementTransition :
        global::AI.Sandbox.Engine.Core.WorldState
            .IWorldStateTransition<CounterState>
    {
        private readonly int amount;

        public IncrementTransition(int amount)
        {
            this.amount = amount;
        }

        public int EvaluationCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateTransitionDecision<CounterState> Evaluate(
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateSnapshot<CounterState> current)
        {
            EvaluationCount++;
            return global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateTransitionDecision<CounterState>.Accept(
                    current.State with
                    {
                        Value = current.State.Value + amount,
                    });
        }
    }

    private sealed class RejectingTransition :
        global::AI.Sandbox.Engine.Core.WorldState
            .IWorldStateTransition<CounterState>
    {
        private readonly string reason;

        public RejectingTransition(string reason)
        {
            this.reason = reason;
        }

        public int EvaluationCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateTransitionDecision<CounterState> Evaluate(
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateSnapshot<CounterState> current)
        {
            _ = current;
            EvaluationCount++;
            return global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateTransitionDecision<CounterState>.Reject(reason);
        }
    }

    private sealed class ThrowingTransition :
        global::AI.Sandbox.Engine.Core.WorldState
            .IWorldStateTransition<CounterState>
    {
        public global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateTransitionDecision<CounterState> Evaluate(
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateSnapshot<CounterState> current)
        {
            _ = current;
            throw new InvalidOperationException("transition failure");
        }
    }

    private sealed class BlockingIncrementTransition :
        global::AI.Sandbox.Engine.Core.WorldState
            .IWorldStateTransition<CounterState>
    {
        private readonly ManualResetEventSlim entered;
        private readonly ManualResetEventSlim release;

        public BlockingIncrementTransition(
            ManualResetEventSlim entered,
            ManualResetEventSlim release)
        {
            this.entered = entered;
            this.release = release;
        }

        public int EvaluationCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateTransitionDecision<CounterState> Evaluate(
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateSnapshot<CounterState> current)
        {
            EvaluationCount++;
            entered.Set();

            if (!release.Wait(TimeSpan.FromSeconds(5)))
            {
                throw new TimeoutException(
                    "The concurrency test did not release the transition.");
            }

            return global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateTransitionDecision<CounterState>.Accept(
                    current.State with
                    {
                        Value = current.State.Value + 1,
                    });
        }
    }
}
