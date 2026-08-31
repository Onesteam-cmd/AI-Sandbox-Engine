namespace AI.Sandbox.Engine.Core.Tests;

public sealed class BehaviorActionValidationTests
{
    private readonly record struct SeekSafety(int Priority) :
        global::AI.Sandbox.Engine.Core.Behavior.IBehaviorIntent;

    private abstract record AbstractIntent :
        global::AI.Sandbox.Engine.Core.Behavior.IBehaviorIntent;

    private sealed record ConcreteAbstractIntent(int Value) :
        AbstractIntent;

    private record OpenIntent(int Value) :
        global::AI.Sandbox.Engine.Core.Behavior.IBehaviorIntent;

    private readonly record struct MoveBy(int Delta) :
        global::AI.Sandbox.Engine.Core.Behavior.IActionProposal;

    private abstract record AbstractAction :
        global::AI.Sandbox.Engine.Core.Behavior.IActionProposal;

    private sealed record ConcreteAbstractAction(int Value) :
        AbstractAction;

    private record OpenAction(int Value) :
        global::AI.Sandbox.Engine.Core.Behavior.IActionProposal;

    private readonly record struct ChangeValue(int Delta) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    private sealed record BehaviorWorldState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    [Xunit.Fact]
    public void IntentActionAndApprovedCommandTypesMustBeExact()
    {
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Behavior
                .BehaviorIntentEnvelope<AbstractIntent>.Create(
                    IntentId(1),
                    ActorId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    new ConcreteAbstractIntent(1)));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Behavior
                .BehaviorIntentEnvelope<OpenIntent>.Create(
                    IntentId(2),
                    ActorId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    new OpenIntent(1)));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Behavior
                .ActionProposalEnvelope<AbstractAction>.CreateExternal(
                    ProposalId(1),
                    ActorId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    new ConcreteAbstractAction(1)));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Behavior
                .ActionProposalEnvelope<OpenAction>.CreateExternal(
                    ProposalId(2),
                    ActorId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    new OpenAction(1)));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Behavior
                .ActionDecision<
                    global::AI.Sandbox.Engine.Core.Commands.IEngineCommand>
                .Reject(RejectionCode("blocked.policy")));

        var intent = global::AI.Sandbox.Engine.Core.Behavior
            .BehaviorIntentEnvelope<SeekSafety>.Create(
                IntentId(3),
                ActorId(),
                WorldId(),
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateVersion.Initial,
                0,
                new SeekSafety(9));
        var proposal = global::AI.Sandbox.Engine.Core.Behavior
            .ActionProposalEnvelope<MoveBy>.CreateFromIntent(
                ProposalId(3),
                intent,
                new MoveBy(2));

        Xunit.Assert.Equal(9, intent.Payload.Priority);
        Xunit.Assert.Equal(2, proposal.Payload.Delta);
    }

    [Xunit.Fact]
    public void ActionProposalPreservesOptionalSourceIntentCoordinates()
    {
        var intent = global::AI.Sandbox.Engine.Core.Behavior
            .BehaviorIntentEnvelope<SeekSafety>.Create(
                IntentId(10),
                ActorId(),
                WorldId(),
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateVersion.Initial,
                7,
                new SeekSafety(4));
        var derived = global::AI.Sandbox.Engine.Core.Behavior
            .ActionProposalEnvelope<MoveBy>.CreateFromIntent(
                ProposalId(10),
                intent,
                new MoveBy(3));
        var external = global::AI.Sandbox.Engine.Core.Behavior
            .ActionProposalEnvelope<MoveBy>.CreateExternal(
                ProposalId(11),
                ActorId(),
                WorldId(),
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateVersion.Initial,
                7,
                new MoveBy(5));

        Xunit.Assert.True(derived.SourceIntentId.HasValue);
        Xunit.Assert.Equal(
            intent.IntentId,
            derived.SourceIntentId.Value);
        Xunit.Assert.Equal(intent.ActorEntityId, derived.ActorEntityId);
        Xunit.Assert.Equal(intent.WorldId, derived.WorldId);
        Xunit.Assert.Equal(
            intent.WorldStateVersion,
            derived.WorldStateVersion);
        Xunit.Assert.Equal(intent.SimulationTick, derived.SimulationTick);
        Xunit.Assert.Null(external.SourceIntentId);
        Xunit.Assert.NotEqual(
            typeof(global::AI.Sandbox.Engine.Core.Behavior
                .IBehaviorIntent),
            typeof(global::AI.Sandbox.Engine.Core.Behavior
                .IActionProposal));
    }

    [Xunit.Fact]
    public void ApprovedActionReturnsTypedCommandWithoutCommittingWorldState()
    {
        var manager = CreateManager();
        var validator = new ApprovingValidator();
        var processor = global::AI.Sandbox.Engine.Core.Behavior
            .ActionValidationProcessor<
                BehaviorWorldState,
                MoveBy,
                ChangeValue>.Create(
                    manager,
                    validator);
        var before = manager.Read();

        var result = processor.Validate(
            ExternalProposal(before, 20, 3));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Behavior
                .ActionValidationStatus.Approved,
            result.Status);
        Xunit.Assert.True(result.ValidatorWasInvoked);
        Xunit.Assert.True(result.HasStableDecision);
        Xunit.Assert.True(result.WasApproved);
        Xunit.Assert.Equal(1, validator.CallCount);
        Xunit.Assert.NotNull(result.Decision);
        Xunit.Assert.True(result.Decision!.IsApproved);
        Xunit.Assert.Equal(3, result.Decision.Command.Delta);
        Xunit.Assert.Throws<InvalidOperationException>(
            () => _ = result.Decision.RejectionCode);

        var afterValidation = manager.Read();
        Xunit.Assert.Equal(before.Version, afterValidation.Version);
        Xunit.Assert.Equal(before.SimulationTick, afterValidation.SimulationTick);
        Xunit.Assert.Equal(0, afterValidation.State.Value);

        var runtime = CreateRuntime(manager);
        var commit = runtime.ExecuteCommand(
            global::AI.Sandbox.Engine.Core.Commands
                .CommandEnvelope<ChangeValue>.Create(
                    CommandId(20),
                    afterValidation.WorldId,
                    afterValidation.Version,
                    afterValidation.SimulationTick,
                    result.Decision.Command));

        Xunit.Assert.True(commit.WasCommitted);
        Xunit.Assert.Equal(3, manager.Read().State.Value);
    }

    [Xunit.Fact]
    public void RejectedActionIsExplicitAndDoesNotProduceCommand()
    {
        var manager = CreateManager();
        var validator = new RejectingValidator();
        var processor = global::AI.Sandbox.Engine.Core.Behavior
            .ActionValidationProcessor<
                BehaviorWorldState,
                MoveBy,
                ChangeValue>.Create(
                    manager,
                    validator);
        var before = manager.Read();

        var result = processor.Validate(
            ExternalProposal(before, 30, 4));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Behavior
                .ActionValidationStatus.Rejected,
            result.Status);
        Xunit.Assert.True(result.ValidatorWasInvoked);
        Xunit.Assert.True(result.HasStableDecision);
        Xunit.Assert.False(result.WasApproved);
        Xunit.Assert.Equal(1, validator.CallCount);
        Xunit.Assert.NotNull(result.Decision);
        Xunit.Assert.False(result.Decision!.IsApproved);
        Xunit.Assert.Equal(
            RejectionCode("blocked.policy"),
            result.Decision.RejectionCode);
        Xunit.Assert.Throws<InvalidOperationException>(
            () => _ = result.Decision.Command);
        Xunit.Assert.Equal(before.Version, manager.Read().Version);
        Xunit.Assert.Equal(0, manager.Read().State.Value);
    }

    [Xunit.Fact]
    public void StaleScopeIsRejectedBeforeValidatorRuns()
    {
        var manager = CreateManager();
        var validator = new ApprovingValidator();
        var processor = global::AI.Sandbox.Engine.Core.Behavior
            .ActionValidationProcessor<
                BehaviorWorldState,
                MoveBy,
                ChangeValue>.Create(
                    manager,
                    validator);
        var snapshot = manager.Read();

        var wrongWorld = global::AI.Sandbox.Engine.Core.Behavior
            .ActionProposalEnvelope<MoveBy>.CreateExternal(
                ProposalId(40),
                ActorId(),
                OtherWorldId(),
                snapshot.Version,
                snapshot.SimulationTick,
                new MoveBy(1));
        var wrongVersion = global::AI.Sandbox.Engine.Core.Behavior
            .ActionProposalEnvelope<MoveBy>.CreateExternal(
                ProposalId(41),
                ActorId(),
                snapshot.WorldId,
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateVersion.From(
                        checked(snapshot.Version.Value + 1)),
                snapshot.SimulationTick,
                new MoveBy(1));
        var wrongTick = global::AI.Sandbox.Engine.Core.Behavior
            .ActionProposalEnvelope<MoveBy>.CreateExternal(
                ProposalId(42),
                ActorId(),
                snapshot.WorldId,
                snapshot.Version,
                checked(snapshot.SimulationTick + 1),
                new MoveBy(1));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Behavior
                .ActionValidationStatus.WorldMismatch,
            processor.Validate(wrongWorld).Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Behavior
                .ActionValidationStatus.VersionConflict,
            processor.Validate(wrongVersion).Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Behavior
                .ActionValidationStatus.SimulationTickMismatch,
            processor.Validate(wrongTick).Status);
        Xunit.Assert.Equal(0, validator.CallCount);
        Xunit.Assert.Equal(snapshot.Version, manager.Read().Version);
    }

    [Xunit.Fact]
    public void AuthorityChangeDuringValidationDiscardsDecisionWithoutRetry()
    {
        var manager = CreateManager();
        var runtime = CreateRuntime(manager);
        var validator = new MutatingValidator(runtime);
        var processor = global::AI.Sandbox.Engine.Core.Behavior
            .ActionValidationProcessor<
                BehaviorWorldState,
                MoveBy,
                ChangeValue>.Create(
                    manager,
                    validator);
        var before = manager.Read();

        var result = processor.Validate(
            ExternalProposal(before, 50, 6));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Behavior
                .ActionValidationStatus.VersionConflict,
            result.Status);
        Xunit.Assert.True(result.ValidatorWasInvoked);
        Xunit.Assert.False(result.HasStableDecision);
        Xunit.Assert.False(result.WasApproved);
        Xunit.Assert.Null(result.Decision);
        Xunit.Assert.Equal(1, validator.CallCount);
        Xunit.Assert.Equal(1, manager.Read().State.Value);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateVersion.From(1),
            manager.Read().Version);
    }

    [Xunit.Fact]
    public void ValidatorExceptionPropagatesWithoutRetry()
    {
        var manager = CreateManager();
        var validator = new ThrowingValidator();
        var processor = global::AI.Sandbox.Engine.Core.Behavior
            .ActionValidationProcessor<
                BehaviorWorldState,
                MoveBy,
                ChangeValue>.Create(
                    manager,
                    validator);
        var before = manager.Read();

        var exception = Xunit.Assert.Throws<InvalidOperationException>(
            () => processor.Validate(
                ExternalProposal(before, 60, 1)));

        Xunit.Assert.Equal("validator failure", exception.Message);
        Xunit.Assert.Equal(1, validator.CallCount);
        Xunit.Assert.Equal(before.Version, manager.Read().Version);
        Xunit.Assert.Equal(0, manager.Read().State.Value);
    }

    [Xunit.Fact]
    public void RejectionCodesAreOrdinalStableAndValidated()
    {
        var first = RejectionCode("blocked.access");
        var second = RejectionCode("blocked.range");

        Xunit.Assert.True(first.IsInitialized);
        Xunit.Assert.Equal("blocked.access", first.Value);
        Xunit.Assert.Equal("blocked.access", first.ToString());
        Xunit.Assert.True(first.CompareTo(second) < 0);
        Xunit.Assert.True(
            global::AI.Sandbox.Engine.Core.Behavior.ActionRejectionCode
                .TryParse("state_conflict-2", out var parsed));
        Xunit.Assert.Equal("state_conflict-2", parsed.Value);
        Xunit.Assert.False(
            global::AI.Sandbox.Engine.Core.Behavior.ActionRejectionCode
                .TryParse("Blocked Access", out _));
        Xunit.Assert.Throws<FormatException>(
            () => RejectionCode(string.Empty));
        Xunit.Assert.Throws<InvalidOperationException>(
            () => _ = default(
                global::AI.Sandbox.Engine.Core.Behavior.ActionRejectionCode)
                .Value);
    }

    private static global::AI.Sandbox.Engine.Core.Behavior
        .ActionProposalEnvelope<MoveBy> ExternalProposal(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateSnapshot<BehaviorWorldState> snapshot,
            int idSuffix,
            int delta) =>
        global::AI.Sandbox.Engine.Core.Behavior
            .ActionProposalEnvelope<MoveBy>.CreateExternal(
                ProposalId(idSuffix),
                ActorId(),
                snapshot.WorldId,
                snapshot.Version,
                snapshot.SimulationTick,
                new MoveBy(delta));

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<BehaviorWorldState> CreateManager() =>
        global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<BehaviorWorldState>.Create(
                WorldId(),
                new BehaviorWorldState(0));

    private static global::AI.Sandbox.Engine.Core.Runtime
        .RuntimeOrchestrator<BehaviorWorldState> CreateRuntime(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<BehaviorWorldState> manager) =>
        new global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestratorBuilder<BehaviorWorldState>()
            .AddCommandHandler(new ChangeValueHandler())
            .Build(manager);

    private sealed class ChangeValueHandler :
        global::AI.Sandbox.Engine.Core.Commands.ICommandHandler<
            BehaviorWorldState,
            ChangeValue>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<BehaviorWorldState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<BehaviorWorldState, ChangeValue> context)
        {
            var nextValue = checked(
                context.Snapshot.State.Value +
                context.Envelope.Payload.Delta);

            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<BehaviorWorldState>.Accept(
                    context.Snapshot.State with
                    {
                        Value = nextValue,
                    });
        }
    }

    private sealed class ApprovingValidator :
        global::AI.Sandbox.Engine.Core.Behavior.IActionValidator<
            BehaviorWorldState,
            MoveBy,
            ChangeValue>
    {
        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Behavior
            .ActionDecision<ChangeValue> Evaluate(
                global::AI.Sandbox.Engine.Core.Behavior
                    .ActionValidationContext<BehaviorWorldState, MoveBy>
                        context)
        {
            CallCount = checked(CallCount + 1);

            return global::AI.Sandbox.Engine.Core.Behavior
                .ActionDecision<ChangeValue>.Approve(
                    new ChangeValue(context.Proposal.Payload.Delta));
        }
    }

    private sealed class RejectingValidator :
        global::AI.Sandbox.Engine.Core.Behavior.IActionValidator<
            BehaviorWorldState,
            MoveBy,
            ChangeValue>
    {
        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Behavior
            .ActionDecision<ChangeValue> Evaluate(
                global::AI.Sandbox.Engine.Core.Behavior
                    .ActionValidationContext<BehaviorWorldState, MoveBy>
                        context)
        {
            _ = context;
            CallCount = checked(CallCount + 1);

            return global::AI.Sandbox.Engine.Core.Behavior
                .ActionDecision<ChangeValue>.Reject(
                    RejectionCode("blocked.policy"));
        }
    }

    private sealed class MutatingValidator :
        global::AI.Sandbox.Engine.Core.Behavior.IActionValidator<
            BehaviorWorldState,
            MoveBy,
            ChangeValue>
    {
        private readonly global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestrator<BehaviorWorldState> runtime;

        public MutatingValidator(
            global::AI.Sandbox.Engine.Core.Runtime
                .RuntimeOrchestrator<BehaviorWorldState> runtime)
        {
            this.runtime = runtime;
        }

        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Behavior
            .ActionDecision<ChangeValue> Evaluate(
                global::AI.Sandbox.Engine.Core.Behavior
                    .ActionValidationContext<BehaviorWorldState, MoveBy>
                        context)
        {
            CallCount = checked(CallCount + 1);
            var snapshot = runtime.Read();
            var result = runtime.ExecuteCommand(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandEnvelope<ChangeValue>.Create(
                        CommandId(900 + CallCount),
                        snapshot.WorldId,
                        snapshot.Version,
                        snapshot.SimulationTick,
                        new ChangeValue(1)));

            if (!result.WasCommitted)
            {
                throw new InvalidOperationException(
                    "The conflict test could not advance authority.");
            }

            return global::AI.Sandbox.Engine.Core.Behavior
                .ActionDecision<ChangeValue>.Approve(
                    new ChangeValue(context.Proposal.Payload.Delta));
        }
    }

    private sealed class ThrowingValidator :
        global::AI.Sandbox.Engine.Core.Behavior.IActionValidator<
            BehaviorWorldState,
            MoveBy,
            ChangeValue>
    {
        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Behavior
            .ActionDecision<ChangeValue> Evaluate(
                global::AI.Sandbox.Engine.Core.Behavior
                    .ActionValidationContext<BehaviorWorldState, MoveBy>
                        context)
        {
            _ = context;
            CallCount = checked(CallCount + 1);
            throw new InvalidOperationException("validator failure");
        }
    }

    private static global::AI.Sandbox.Engine.Core.Behavior
        .ActionRejectionCode RejectionCode(string text) =>
        global::AI.Sandbox.Engine.Core.Behavior.ActionRejectionCode.Parse(text);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000002000");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> OtherWorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000002001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ActorId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(
                "019b0000-0000-7100-8100-000000002000");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Behavior.BehaviorIntentIdKind>
            IntentId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Behavior
                .BehaviorIntentIdKind>.Parse(
                    $"019b0000-0000-7200-8200-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Behavior.ActionProposalIdKind>
            ProposalId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Behavior
                .ActionProposalIdKind>.Parse(
                    $"019b0000-0000-7300-8300-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Commands.CommandIdKind>
            CommandId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Commands.CommandIdKind>.Parse(
                $"019b0000-0000-7400-8400-{suffix:D12}");
}
