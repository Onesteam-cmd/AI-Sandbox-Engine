namespace AI.Sandbox.Engine.Core.Tests;

public sealed class DialogueOrchestrationTests
{
    private readonly record struct DialogueInput(string Text) :
        global::AI.Sandbox.Engine.Core.Dialogue.IDialogueInput;

    private readonly record struct EvidenceArtifact(string Value) :
        global::AI.Sandbox.Engine.Core.Dialogue.IDialogueArtifact;

    private readonly record struct NextStep(string Name) :
        global::AI.Sandbox.Engine.Core.Dialogue.IDialogueDirective;

    private readonly record struct CompletedReply(string Text) :
        global::AI.Sandbox.Engine.Core.Dialogue.IDialogueCompletion;

    private abstract record AbstractDirective :
        global::AI.Sandbox.Engine.Core.Dialogue.IDialogueDirective;

    private sealed record ConcreteDirective(string Name) : AbstractDirective;

    private record OpenCompletion(string Text) :
        global::AI.Sandbox.Engine.Core.Dialogue.IDialogueCompletion;

    private readonly record struct Topic(string Value) :
        global::AI.Sandbox.Engine.Core.Conversation.IConversationTopic;

    private readonly record struct AdvanceValue(int Delta) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    private sealed record DialogueWorldState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    [Xunit.Fact]
    public void ExactPayloadsSequencesAndCodesAreBounded()
    {
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueArtifactSequence.From(0));
        Xunit.Assert.Throws<FormatException>(
            () => global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueRejectionCode.Parse("Invalid Code"));

        var code = global::AI.Sandbox.Engine.Core.Dialogue
            .DialogueRejectionCode.Parse("context.missing");
        Xunit.Assert.Equal("context.missing", code.Value);

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationDecision<
                    AbstractDirective,
                    CompletedReply>.Continue(
                        new ConcreteDirective("invalid")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationDecision<
                    NextStep,
                    OpenCompletion>.Complete(
                        new OpenCompletion("invalid")));
    }

    [Xunit.Fact]
    public void RequestPreservesConversationAuthorityAndArtifactCorrelation()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var conversation = CreateConversation();
        var artifacts = new[]
        {
            CreateArtifact(snapshot, conversation, 2, 2),
            CreateArtifact(snapshot, conversation, 1, 1),
        };

        var request = CreateRequest(snapshot, conversation, artifacts);

        Xunit.Assert.Equal(ExchangeId(), request.ExchangeId);
        Xunit.Assert.Equal(conversation.ConversationId, request.ConversationId);
        Xunit.Assert.Equal(conversation.Revision, request.ExpectedConversationRevision);
        Xunit.Assert.Equal(OwnerId(), request.PerspectiveOwnerEntityId);
        Xunit.Assert.Equal(SpeakerId(), request.SourceSpeakerEntityId);
        Xunit.Assert.Equal(snapshot.WorldId, request.WorldId);
        Xunit.Assert.Equal(snapshot.Version, request.ExpectedWorldStateVersion);
        Xunit.Assert.Equal(snapshot.SimulationTick, request.ExpectedSimulationTick);
        Xunit.Assert.Equal("hello", request.Input.Text);
        Xunit.Assert.Equal(2, request.Artifacts.Count);
        Xunit.Assert.Equal(1, request.Artifacts[0].Sequence.Value);
        Xunit.Assert.Equal(2, request.Artifacts[1].Sequence.Value);
    }

    [Xunit.Fact]
    public void ContinueDecisionReturnsOneExactDirectiveExactlyOnce()
    {
        var manager = CreateManager();
        var conversation = CreateConversation();
        var orchestrator = new FixedOrchestrator(
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationDecision<NextStep, CompletedReply>
                .Continue(new NextStep("invoke-model")));
        var processor = CreateProcessor(manager, orchestrator);

        var result = processor.Process(
            CreateRequest(manager.Read(), conversation),
            conversation);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationStatus.Continued,
            result.Status);
        Xunit.Assert.True(result.OrchestratorWasInvoked);
        Xunit.Assert.True(result.HasStableDecision);
        Xunit.Assert.True(result.WasContinued);
        Xunit.Assert.False(result.WasCompleted);
        Xunit.Assert.Equal(1, orchestrator.CallCount);
        Xunit.Assert.Equal("invoke-model", result.Decision!.Directive.Name);
    }

    [Xunit.Fact]
    public void CompleteAndRejectDecisionsRemainExplicit()
    {
        var manager = CreateManager();
        var conversation = CreateConversation();

        var completionOrchestrator = new FixedOrchestrator(
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationDecision<NextStep, CompletedReply>
                .Complete(new CompletedReply("done")));
        var completed = CreateProcessor(manager, completionOrchestrator)
            .Process(CreateRequest(manager.Read(), conversation), conversation);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationStatus.Completed,
            completed.Status);
        Xunit.Assert.Equal("done", completed.Decision!.Completion.Text);

        var rejectionOrchestrator = new FixedOrchestrator(
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationDecision<NextStep, CompletedReply>
                .Reject(global::AI.Sandbox.Engine.Core.Dialogue
                    .DialogueRejectionCode.Parse("policy.denied")));
        var rejected = CreateProcessor(manager, rejectionOrchestrator)
            .Process(CreateRequest(manager.Read(), conversation), conversation);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationStatus.Rejected,
            rejected.Status);
        Xunit.Assert.Equal(
            "policy.denied",
            rejected.Decision!.RejectionCode.Value);
    }

    [Xunit.Fact]
    public void PreflightMismatchesSkipOrchestrator()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var conversation = CreateConversation();
        var orchestrator = new FixedOrchestrator(
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationDecision<NextStep, CompletedReply>
                .Continue(new NextStep("unused")));
        var processor = CreateProcessor(manager, orchestrator);

        var cases = new[]
        {
            (
                global::AI.Sandbox.Engine.Core.Dialogue
                    .DialogueOrchestrationStatus.OrchestratorMismatch,
                CreateRequest(snapshot, conversation, orchestratorId: OrchestratorId(2))),
            (
                global::AI.Sandbox.Engine.Core.Dialogue
                    .DialogueOrchestrationStatus.WorldMismatch,
                CreateRequest(snapshot, conversation, worldId: OtherWorldId())),
            (
                global::AI.Sandbox.Engine.Core.Dialogue
                    .DialogueOrchestrationStatus.VersionConflict,
                CreateRequest(
                    snapshot,
                    conversation,
                    version: global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.From(snapshot.Version.Value + 1))),
            (
                global::AI.Sandbox.Engine.Core.Dialogue
                    .DialogueOrchestrationStatus.SimulationTickMismatch,
                CreateRequest(snapshot, conversation, tick: snapshot.SimulationTick + 1)),
            (
                global::AI.Sandbox.Engine.Core.Dialogue
                    .DialogueOrchestrationStatus.ConversationRevisionConflict,
                CreateRequest(
                    snapshot,
                    conversation,
                    revision: global::AI.Sandbox.Engine.Core.Conversation
                        .ConversationRevision.From(conversation.Revision.Value + 1))),
            (
                global::AI.Sandbox.Engine.Core.Dialogue
                    .DialogueOrchestrationStatus.PerspectiveOwnerNotParticipant,
                CreateRequest(snapshot, conversation, owner: OutsiderId())),
            (
                global::AI.Sandbox.Engine.Core.Dialogue
                    .DialogueOrchestrationStatus.SourceSpeakerNotParticipant,
                CreateRequest(snapshot, conversation, speaker: OutsiderId())),
        };

        foreach (var item in cases)
        {
            var result = processor.Process(item.Item2, conversation);
            Xunit.Assert.Equal(item.Item1, result.Status);
            Xunit.Assert.False(result.OrchestratorWasInvoked);
            Xunit.Assert.False(result.HasStableDecision);
            Xunit.Assert.Null(result.Decision);
        }

        Xunit.Assert.Equal(0, orchestrator.CallCount);
    }

    [Xunit.Fact]
    public void ClosedConversationAndInvalidAudienceSkipOrchestrator()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var conversation = CreateConversation();
        var orchestrator = new FixedOrchestrator(
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationDecision<NextStep, CompletedReply>
                .Continue(new NextStep("unused")));
        var processor = CreateProcessor(manager, orchestrator);

        var invalidAudience = global::AI.Sandbox.Engine.Core.Conversation
            .AddressAudience.SpecificParticipants(new[] { SpeakerId() });
        var invalidResult = processor.Process(
            CreateRequest(snapshot, conversation, audience: invalidAudience),
            conversation);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationStatus.AudienceInvalid,
            invalidResult.Status);

        var incompleteAllAudience = global::AI.Sandbox.Engine.Core.Conversation
            .AddressAudience.AllParticipants(new[] { OwnerId() });
        var incompleteAllResult = processor.Process(
            CreateRequest(
                snapshot,
                conversation,
                audience: incompleteAllAudience),
            conversation);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationStatus.AudienceInvalid,
            incompleteAllResult.Status);

        var closedMutation = conversation.Close(conversation.Revision);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationMutationStatus.Closed,
            closedMutation.Status);
        var closed = closedMutation.State;
        var closedResult = processor.Process(
            CreateRequest(snapshot, closed, revision: closed.Revision),
            closed);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationStatus.ConversationClosed,
            closedResult.Status);
        Xunit.Assert.Equal(0, orchestrator.CallCount);
    }

    [Xunit.Fact]
    public void AuthorityChangeDiscardsDecisionWithoutRetry()
    {
        var manager = CreateManager();
        var conversation = CreateConversation();
        var orchestrator = new ConflictOrchestrator(CreateRuntime(manager));
        var processor = CreateProcessor(manager, orchestrator);

        var result = processor.Process(
            CreateRequest(manager.Read(), conversation),
            conversation);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationStatus.AuthorityChanged,
            result.Status);
        Xunit.Assert.True(result.OrchestratorWasInvoked);
        Xunit.Assert.False(result.HasStableDecision);
        Xunit.Assert.Null(result.Decision);
        Xunit.Assert.Equal(1, orchestrator.CallCount);
    }

    [Xunit.Fact]
    public void OrchestratorExceptionPropagatesWithoutRetry()
    {
        var manager = CreateManager();
        var conversation = CreateConversation();
        var orchestrator = new ThrowingOrchestrator();
        var processor = CreateProcessor(manager, orchestrator);

        Xunit.Assert.Throws<InvalidOperationException>(
            () => processor.Process(
                CreateRequest(manager.Read(), conversation),
                conversation));
        Xunit.Assert.Equal(1, orchestrator.CallCount);
    }

    [Xunit.Fact]
    public void OrchestrationRemainsReadOnlyAndDoesNotExecuteDirective()
    {
        var manager = CreateManager();
        var before = manager.Read();
        var conversation = CreateConversation();
        var orchestrator = new FixedOrchestrator(
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationDecision<NextStep, CompletedReply>
                .Continue(new NextStep("execute-command-later")));

        var result = CreateProcessor(manager, orchestrator).Process(
            CreateRequest(before, conversation),
            conversation);
        var after = manager.Read();

        Xunit.Assert.Equal(before.Version, after.Version);
        Xunit.Assert.Equal(before.SimulationTick, after.SimulationTick);
        Xunit.Assert.Same(before.State, after.State);
        Xunit.Assert.Equal(
            conversation.Revision,
            CreateConversation().Revision);
        Xunit.Assert.Equal(
            "execute-command-later",
            result.Decision!.Directive.Name);
    }

    private static global::AI.Sandbox.Engine.Core.Dialogue
        .DialogueOrchestrationProcessor<
            DialogueWorldState,
            DialogueInput,
            Topic,
            NextStep,
            CompletedReply> CreateProcessor(
        global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<DialogueWorldState> manager,
        global::AI.Sandbox.Engine.Core.Dialogue.IDialogueOrchestrator<
            DialogueWorldState,
            DialogueInput,
            Topic,
            NextStep,
            CompletedReply> orchestrator) =>
        global::AI.Sandbox.Engine.Core.Dialogue
            .DialogueOrchestrationProcessor<
                DialogueWorldState,
                DialogueInput,
                Topic,
                NextStep,
                CompletedReply>.Create(manager, orchestrator);

    private static global::AI.Sandbox.Engine.Core.Dialogue
        .DialogueOrchestrationRequestEnvelope<DialogueInput> CreateRequest(
        global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateSnapshot<DialogueWorldState> snapshot,
        global::AI.Sandbox.Engine.Core.Conversation.ConversationState<Topic>
            conversation,
        IEnumerable<global::AI.Sandbox.Engine.Core.Dialogue
            .DialogueArtifactEnvelope>? artifacts = null,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Dialogue.DialogueOrchestratorIdKind>
            orchestratorId = default,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>
            worldId = default,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion?
            version = null,
        ulong? tick = null,
        global::AI.Sandbox.Engine.Core.Conversation.ConversationRevision
            revision = default,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
            owner = default,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
            speaker = default,
        global::AI.Sandbox.Engine.Core.Conversation.AddressAudience?
            audience = null) =>
        global::AI.Sandbox.Engine.Core.Dialogue
            .DialogueOrchestrationRequestEnvelope<DialogueInput>.Create(
                OrchestrationId(),
                orchestratorId.IsEmpty ? OrchestratorId(1) : orchestratorId,
                ExchangeId(),
                conversation.ConversationId,
                revision.IsInitialized ? revision : conversation.Revision,
                owner.IsEmpty ? OwnerId() : owner,
                speaker.IsEmpty ? SpeakerId() : speaker,
                audience ?? global::AI.Sandbox.Engine.Core.Conversation
                    .AddressAudience.SpecificParticipants(new[] { OwnerId() }),
                worldId.IsEmpty ? snapshot.WorldId : worldId,
                version ?? snapshot.Version,
                tick ?? snapshot.SimulationTick,
                new DialogueInput("hello"),
                artifacts ?? Array.Empty<
                    global::AI.Sandbox.Engine.Core.Dialogue
                        .DialogueArtifactEnvelope>());

    private static global::AI.Sandbox.Engine.Core.Dialogue
        .DialogueArtifactEnvelope CreateArtifact(
        global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateSnapshot<DialogueWorldState> snapshot,
        global::AI.Sandbox.Engine.Core.Conversation.ConversationState<Topic>
            conversation,
        int suffix,
        int sequence) =>
        global::AI.Sandbox.Engine.Core.Dialogue.DialogueArtifactEnvelope.Create(
            ArtifactId(suffix),
            ArtifactSourceId(1),
            ExchangeId(),
            conversation.ConversationId,
            OwnerId(),
            snapshot.WorldId,
            snapshot.Version,
            snapshot.SimulationTick,
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueArtifactSequence.From(sequence),
            new EvidenceArtifact($"artifact-{suffix}"));

    private static global::AI.Sandbox.Engine.Core.Conversation
        .ConversationState<Topic> CreateConversation() =>
        global::AI.Sandbox.Engine.Core.Conversation
            .ConversationState<Topic>.Start(
                WorldId(),
                ConversationId(),
                new[] { SpeakerId(), OwnerId(), ThirdParticipantId() },
                new Topic("topic"));

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<DialogueWorldState> CreateManager() =>
        global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<DialogueWorldState>.Create(
                WorldId(),
                new DialogueWorldState(0));

    private static global::AI.Sandbox.Engine.Core.Runtime
        .RuntimeOrchestrator<DialogueWorldState> CreateRuntime(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<DialogueWorldState> manager) =>
        new global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestratorBuilder<DialogueWorldState>()
            .AddCommandHandler(new AdvanceValueHandler())
            .Build(manager);

    private sealed class AdvanceValueHandler :
        global::AI.Sandbox.Engine.Core.Commands.ICommandHandler<
            DialogueWorldState,
            AdvanceValue>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<DialogueWorldState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<DialogueWorldState, AdvanceValue> context) =>
            global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<DialogueWorldState>.Accept(
                    context.Snapshot.State with
                    {
                        Value = checked(
                            context.Snapshot.State.Value +
                            context.Envelope.Payload.Delta),
                    });
    }

    private sealed class FixedOrchestrator :
        global::AI.Sandbox.Engine.Core.Dialogue.IDialogueOrchestrator<
            DialogueWorldState,
            DialogueInput,
            Topic,
            NextStep,
            CompletedReply>
    {
        private readonly global::AI.Sandbox.Engine.Core.Dialogue
            .DialogueOrchestrationDecision<NextStep, CompletedReply> decision;

        public FixedOrchestrator(
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationDecision<NextStep, CompletedReply>
                decision)
        {
            this.decision = decision;
        }

        public global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Dialogue.DialogueOrchestratorIdKind>
            OrchestratorId => DialogueOrchestrationTests.OrchestratorId(1);

        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Dialogue
            .DialogueOrchestrationDecision<NextStep, CompletedReply> Decide(
                global::AI.Sandbox.Engine.Core.Dialogue
                    .DialogueOrchestrationContext<
                        DialogueWorldState,
                        DialogueInput,
                        Topic> context)
        {
            CallCount = checked(CallCount + 1);
            return decision;
        }
    }

    private sealed class ConflictOrchestrator :
        global::AI.Sandbox.Engine.Core.Dialogue.IDialogueOrchestrator<
            DialogueWorldState,
            DialogueInput,
            Topic,
            NextStep,
            CompletedReply>
    {
        private readonly global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestrator<DialogueWorldState> runtime;

        public ConflictOrchestrator(
            global::AI.Sandbox.Engine.Core.Runtime
                .RuntimeOrchestrator<DialogueWorldState> runtime)
        {
            this.runtime = runtime;
        }

        public global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Dialogue.DialogueOrchestratorIdKind>
            OrchestratorId => DialogueOrchestrationTests.OrchestratorId(1);

        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Dialogue
            .DialogueOrchestrationDecision<NextStep, CompletedReply> Decide(
                global::AI.Sandbox.Engine.Core.Dialogue
                    .DialogueOrchestrationContext<
                        DialogueWorldState,
                        DialogueInput,
                        Topic> context)
        {
            CallCount = checked(CallCount + 1);
            var snapshot = runtime.Read();
            var result = runtime.ExecuteCommand(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandEnvelope<AdvanceValue>.Create(
                        CommandId(900 + CallCount),
                        snapshot.WorldId,
                        snapshot.Version,
                        snapshot.SimulationTick,
                        new AdvanceValue(1)));

            if (!result.WasCommitted)
            {
                throw new InvalidOperationException(
                    "The conflict test could not advance authority.");
            }

            return global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationDecision<NextStep, CompletedReply>
                .Continue(new NextStep("late"));
        }
    }

    private sealed class ThrowingOrchestrator :
        global::AI.Sandbox.Engine.Core.Dialogue.IDialogueOrchestrator<
            DialogueWorldState,
            DialogueInput,
            Topic,
            NextStep,
            CompletedReply>
    {
        public global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Dialogue.DialogueOrchestratorIdKind>
            OrchestratorId => DialogueOrchestrationTests.OrchestratorId(1);

        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Dialogue
            .DialogueOrchestrationDecision<NextStep, CompletedReply> Decide(
                global::AI.Sandbox.Engine.Core.Dialogue
                    .DialogueOrchestrationContext<
                        DialogueWorldState,
                        DialogueInput,
                        Topic> context)
        {
            CallCount = checked(CallCount + 1);
            throw new InvalidOperationException("orchestrator failure");
        }
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId() =>
        Id<global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>(1);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> OtherWorldId() =>
        Id<global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>(2);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> SpeakerId() =>
        Id<global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>(10);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OwnerId() =>
        Id<global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>(11);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
        ThirdParticipantId() =>
        Id<global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>(12);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OutsiderId() =>
        Id<global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>(99);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Conversation.ConversationIdKind>
        ConversationId() =>
        Id<global::AI.Sandbox.Engine.Core.Conversation.ConversationIdKind>(20);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Dialogue.DialogueExchangeIdKind>
        ExchangeId() =>
        Id<global::AI.Sandbox.Engine.Core.Dialogue.DialogueExchangeIdKind>(30);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Dialogue.DialogueOrchestrationIdKind>
        OrchestrationId() =>
        Id<global::AI.Sandbox.Engine.Core.Dialogue.DialogueOrchestrationIdKind>(31);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Dialogue.DialogueOrchestratorIdKind>
        OrchestratorId(int suffix) =>
        Id<global::AI.Sandbox.Engine.Core.Dialogue.DialogueOrchestratorIdKind>(40 + suffix);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Dialogue.DialogueArtifactIdKind>
        ArtifactId(int suffix) =>
        Id<global::AI.Sandbox.Engine.Core.Dialogue.DialogueArtifactIdKind>(50 + suffix);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Dialogue.DialogueArtifactSourceIdKind>
        ArtifactSourceId(int suffix) =>
        Id<global::AI.Sandbox.Engine.Core.Dialogue.DialogueArtifactSourceIdKind>(60 + suffix);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Commands.CommandIdKind> CommandId(int suffix) =>
        Id<global::AI.Sandbox.Engine.Core.Commands.CommandIdKind>(70 + suffix);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind> Id<TKind>(
        int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(
            $"019d0000-0000-7000-8000-{suffix:D12}");
}
