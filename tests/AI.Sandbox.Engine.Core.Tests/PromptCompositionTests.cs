namespace AI.Sandbox.Engine.Core.Tests;

public sealed class PromptCompositionTests
{
    private readonly record struct ComposeRequest(string Topic) :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptRequest;

    private abstract record AbstractRequest :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptRequest;

    private sealed record ConcreteAbstractRequest(string Topic) :
        AbstractRequest;

    private record OpenRequest(string Topic) :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptRequest;

    private readonly record struct PromptText(string Text) :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptContent;

    private abstract record AbstractContent :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptContent;

    private sealed record ConcreteAbstractContent(string Text) :
        AbstractContent;

    private record OpenContent(string Text) :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptContent;

    private readonly record struct RenderedPrompt(string Text) :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptDocument;

    private abstract record AbstractDocument :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptDocument;

    private sealed record ConcreteAbstractDocument(string Text) :
        AbstractDocument;

    private record OpenDocument(string Text) :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptDocument;

    private readonly record struct AdvanceValue(int Delta) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    private sealed record PromptWorldState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    [Xunit.Fact]
    public void PayloadTypesAndBudgetValuesMustBeExactAndBounded()
    {
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Prompting
                .PromptRequestEnvelope<AbstractRequest>.Create(
                    RequestId(1),
                    OwnerId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    Budget(10),
                    new ConcreteAbstractRequest("topic")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Prompting
                .PromptRequestEnvelope<OpenRequest>.Create(
                    RequestId(2),
                    OwnerId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    Budget(10),
                    new OpenRequest("topic")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Prompting
                .PromptCandidateEnvelope<AbstractContent>.Create(
                    CandidateId(1),
                    OwnerId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptInclusionMode.Optional,
                    Priority(5000),
                    Cost(2),
                    new ConcreteAbstractContent("content")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Prompting
                .PromptCandidateEnvelope<OpenContent>.Create(
                    CandidateId(2),
                    OwnerId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptInclusionMode.Optional,
                    Priority(5000),
                    Cost(2),
                    new OpenContent("content")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Prompting
                .PromptDocumentEnvelope<AbstractDocument>.Create(
                    DocumentId(1),
                    ComposerId(1),
                    OwnerId(),
                    WorldId(),
                    Cost(2),
                    new ConcreteAbstractDocument("document")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Prompting
                .PromptDocumentEnvelope<OpenDocument>.Create(
                    DocumentId(2),
                    ComposerId(1),
                    OwnerId(),
                    WorldId(),
                    Cost(2),
                    new OpenDocument("document")));

        Xunit.Assert.False(
            global::AI.Sandbox.Engine.Core.Prompting.PromptBudget
                .TryFromUnits(0, out _));
        Xunit.Assert.False(
            global::AI.Sandbox.Engine.Core.Prompting.PromptCost
                .TryFromUnits(1000001, out _));
        Xunit.Assert.False(
            global::AI.Sandbox.Engine.Core.Prompting.PromptPriority
                .TryFromBasisPoints(0, out _));
        Xunit.Assert.Throws<FormatException>(
            () => global::AI.Sandbox.Engine.Core.Prompting
                .PromptRejectionCode.Parse("Provider Blocked"));
    }

    [Xunit.Fact]
    public void RequestCandidateAndDocumentEnvelopesPreserveScope()
    {
        var request = global::AI.Sandbox.Engine.Core.Prompting
            .PromptRequestEnvelope<ComposeRequest>.Create(
                RequestId(10),
                OwnerId(),
                WorldId(),
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateVersion.From(3),
                8,
                Budget(100),
                new ComposeRequest("door"));
        var candidate = CreateCandidate(
            CandidateId(10),
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptInclusionMode.Required,
            9000,
            20,
            "knowledge");
        var document = CreateDocument(
            DocumentId(10),
            ComposerId(10),
            OwnerId(),
            WorldId(),
            70,
            "rendered");

        Xunit.Assert.Equal(RequestId(10), request.RequestId);
        Xunit.Assert.Equal(OwnerId(), request.OwnerEntityId);
        Xunit.Assert.Equal(WorldId(), request.WorldId);
        Xunit.Assert.Equal((ulong)8, request.SimulationTick);
        Xunit.Assert.Equal(100, request.Budget.Units);
        Xunit.Assert.Equal("door", request.Payload.Topic);

        Xunit.Assert.Equal(CandidateId(10), candidate.CandidateId);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptInclusionMode.Required,
            candidate.InclusionMode);
        Xunit.Assert.Equal(9000, candidate.Priority.BasisPoints);
        Xunit.Assert.Equal(20, candidate.Cost.Units);

        Xunit.Assert.Equal(DocumentId(10), document.DocumentId);
        Xunit.Assert.Equal(ComposerId(10), document.ComposerId);
        Xunit.Assert.Equal(70, document.Cost.Units);
        Xunit.Assert.Equal("rendered", document.Payload.Text);
    }

    [Xunit.Fact]
    public void BudgetManagerSelectsCandidatesDeterministically()
    {
        var manager = new global::AI.Sandbox.Engine.Core.Prompting
            .PromptBudgetManager<PromptText>();
        var required = CreateCandidate(
            CandidateId(30),
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptInclusionMode.Required,
            1000,
            4,
            "required");
        var tooLargeHighPriority = CreateCandidate(
            CandidateId(31),
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptInclusionMode.Optional,
            9000,
            9,
            "too-large");
        var fitting = CreateCandidate(
            CandidateId(32),
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptInclusionMode.Optional,
            8000,
            6,
            "fitting");
        var tieLater = CreateCandidate(
            CandidateId(34),
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptInclusionMode.Optional,
            7000,
            1,
            "tie-later");
        var tieEarlier = CreateCandidate(
            CandidateId(33),
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptInclusionMode.Optional,
            7000,
            1,
            "tie-earlier");

        var selected = manager.Allocate(
            Budget(12),
            new[]
            {
                tieLater,
                fitting,
                required,
                tooLargeHighPriority,
                tieEarlier,
            });

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptBudgetStatus.Selected,
            selected.Status);
        Xunit.Assert.Equal(4, selected.RequiredUnits);
        Xunit.Assert.Equal(12, selected.UsedUnits);
        Xunit.Assert.Equal(0, selected.RemainingUnits);
        Xunit.Assert.Equal(
            new[]
            {
                CandidateId(30),
                CandidateId(32),
                CandidateId(33),
                CandidateId(34),
            },
            selected.SelectedCandidates
                .Select(candidate => candidate.CandidateId)
                .ToArray());

        Xunit.Assert.Throws<ArgumentException>(
            () => manager.Allocate(Budget(20), new[] { required, required }));
    }

    [Xunit.Fact]
    public void RequiredBudgetExceededSkipsComposer()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var composer = new FixedComposer(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionDecision<RenderedPrompt>.Compose(
                    CreateDocument(
                        DocumentId(40),
                        ComposerId(40),
                        OwnerId(),
                        WorldId(),
                        1,
                        "unused")));
        var processor = CreateProcessor(
            manager,
            ComposerId(40),
            composer);

        var result = processor.Compose(
            CreateRequest(snapshot, 40, 10),
            new[]
            {
                CreateCandidate(
                    CandidateId(40),
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptInclusionMode.Required,
                    9000,
                    6,
                    "first"),
                CreateCandidate(
                    CandidateId(41),
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptInclusionMode.Required,
                    8000,
                    5,
                    "second"),
            });

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionStatus.RequiredBudgetExceeded,
            result.Status);
        Xunit.Assert.False(result.ComposerWasInvoked);
        Xunit.Assert.Equal(0, composer.CallCount);
        Xunit.Assert.Equal(11, result.BudgetResult!.RequiredUnits);
        Xunit.Assert.Throws<InvalidOperationException>(
            () => _ = result.BudgetResult.SelectedCandidates);
    }

    [Xunit.Fact]
    public void ProcessorComposesOnceWithBudgetedCandidates()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var composerId = ComposerId(50);
        var composer = new FixedComposer(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionDecision<RenderedPrompt>.Compose(
                    CreateDocument(
                        DocumentId(50),
                        composerId,
                        OwnerId(),
                        WorldId(),
                        10,
                        "final")));
        var processor = CreateProcessor(manager, composerId, composer);

        var result = processor.Compose(
            CreateRequest(snapshot, 50, 10),
            new[]
            {
                CreateCandidate(
                    CandidateId(50),
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptInclusionMode.Required,
                    1000,
                    4,
                    "required"),
                CreateCandidate(
                    CandidateId(51),
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptInclusionMode.Optional,
                    9000,
                    7,
                    "skipped"),
                CreateCandidate(
                    CandidateId(52),
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptInclusionMode.Optional,
                    8000,
                    6,
                    "selected"),
            });

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionStatus.Composed,
            result.Status);
        Xunit.Assert.True(result.WasComposed);
        Xunit.Assert.True(result.ComposerWasInvoked);
        Xunit.Assert.Equal(1, composer.CallCount);
        Xunit.Assert.Equal(2, composer.LastSelectedCount);
        Xunit.Assert.Equal(
            "final",
            result.Decision!.Document.Payload.Text);
    }

    [Xunit.Fact]
    public void StaleAndInvalidCandidateScopeSkipsComposer()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var composer = new FixedComposer(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionDecision<RenderedPrompt>.Reject(
                    RejectionCode("unused")));
        var processor = CreateProcessor(
            manager,
            ComposerId(60),
            composer);

        var wrongWorldRequest = global::AI.Sandbox.Engine.Core.Prompting
            .PromptRequestEnvelope<ComposeRequest>.Create(
                RequestId(60),
                OwnerId(),
                OtherWorldId(),
                snapshot.Version,
                snapshot.SimulationTick,
                Budget(10),
                new ComposeRequest("topic"));
        var wrongVersionRequest = global::AI.Sandbox.Engine.Core.Prompting
            .PromptRequestEnvelope<ComposeRequest>.Create(
                RequestId(61),
                OwnerId(),
                snapshot.WorldId,
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateVersion.From(
                        checked(snapshot.Version.Value + 1)),
                snapshot.SimulationTick,
                Budget(10),
                new ComposeRequest("topic"));
        var validRequest = CreateRequest(snapshot, 62, 10);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionStatus.WorldMismatch,
            processor.Compose(
                wrongWorldRequest,
                Array.Empty<
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptCandidateEnvelope<PromptText>>()).Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionStatus.VersionConflict,
            processor.Compose(
                wrongVersionRequest,
                Array.Empty<
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptCandidateEnvelope<PromptText>>()).Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionStatus.CandidateWorldMismatch,
            processor.Compose(
                validRequest,
                new[]
                {
                    CreateCandidate(
                        CandidateId(60),
                        global::AI.Sandbox.Engine.Core.Prompting
                            .PromptInclusionMode.Optional,
                        5000,
                        1,
                        "wrong-world",
                        worldId: OtherWorldId()),
                }).Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionStatus.CandidateOwnerMismatch,
            processor.Compose(
                validRequest,
                new[]
                {
                    CreateCandidate(
                        CandidateId(61),
                        global::AI.Sandbox.Engine.Core.Prompting
                            .PromptInclusionMode.Optional,
                        5000,
                        1,
                        "wrong-owner",
                        ownerId: OtherOwnerId()),
                }).Status);
        var duplicate = CreateCandidate(
            CandidateId(62),
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptInclusionMode.Optional,
            5000,
            1,
            "duplicate");
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionStatus.DuplicateCandidate,
            processor.Compose(
                validRequest,
                new[] { duplicate, duplicate }).Status);
        Xunit.Assert.Equal(0, composer.CallCount);
    }

    [Xunit.Fact]
    public void DocumentScopeAndBudgetAreValidated()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var request = CreateRequest(snapshot, 70, 10);
        var candidates = Array.Empty<
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCandidateEnvelope<PromptText>>();

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionStatus.ResultWorldMismatch,
            ComposeFixed(
                manager,
                request,
                candidates,
                ComposerId(70),
                CreateDocument(
                    DocumentId(70),
                    ComposerId(70),
                    OwnerId(),
                    OtherWorldId(),
                    1,
                    "wrong-world")).Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionStatus.ResultOwnerMismatch,
            ComposeFixed(
                manager,
                request,
                candidates,
                ComposerId(71),
                CreateDocument(
                    DocumentId(71),
                    ComposerId(71),
                    OtherOwnerId(),
                    WorldId(),
                    1,
                    "wrong-owner")).Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionStatus.ResultComposerMismatch,
            ComposeFixed(
                manager,
                request,
                candidates,
                ComposerId(72),
                CreateDocument(
                    DocumentId(72),
                    ComposerId(99),
                    OwnerId(),
                    WorldId(),
                    1,
                    "wrong-composer")).Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionStatus.ResultBudgetExceeded,
            ComposeFixed(
                manager,
                request,
                candidates,
                ComposerId(73),
                CreateDocument(
                    DocumentId(73),
                    ComposerId(73),
                    OwnerId(),
                    WorldId(),
                    11,
                    "over-budget")).Status);
    }

    [Xunit.Fact]
    public void AuthorityChangeDuringCompositionDiscardsWithoutRetry()
    {
        var manager = CreateManager();
        var runtime = CreateRuntime(manager);
        var snapshot = manager.Read();
        var composerId = ComposerId(80);
        var composer = new MutatingComposer(runtime, composerId);
        var processor = CreateProcessor(manager, composerId, composer);

        var result = processor.Compose(
            CreateRequest(snapshot, 80, 10),
            Array.Empty<
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptCandidateEnvelope<PromptText>>());

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionStatus.VersionConflict,
            result.Status);
        Xunit.Assert.True(result.ComposerWasInvoked);
        Xunit.Assert.Null(result.Decision);
        Xunit.Assert.Equal(1, composer.CallCount);
        Xunit.Assert.Equal(1, manager.Read().State.Value);
    }

    [Xunit.Fact]
    public void RejectionAndExceptionRemainExplicitWithoutRetry()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var rejecting = new FixedComposer(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionDecision<RenderedPrompt>.Reject(
                    RejectionCode("prompt.blocked")));
        var rejected = CreateProcessor(
            manager,
            ComposerId(90),
            rejecting).Compose(
                CreateRequest(snapshot, 90, 10),
                Array.Empty<
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptCandidateEnvelope<PromptText>>());

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionStatus.Rejected,
            rejected.Status);
        Xunit.Assert.Equal(
            "prompt.blocked",
            rejected.Decision!.RejectionCode.Value);
        Xunit.Assert.Equal(1, rejecting.CallCount);

        var throwing = new ThrowingComposer();
        var throwingProcessor = CreateProcessor(
            manager,
            ComposerId(91),
            throwing);

        var exception = Xunit.Assert.Throws<InvalidOperationException>(
            () => throwingProcessor.Compose(
                CreateRequest(manager.Read(), 91, 10),
                Array.Empty<
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptCandidateEnvelope<PromptText>>()));

        Xunit.Assert.Equal("composer failure", exception.Message);
        Xunit.Assert.Equal(1, throwing.CallCount);
    }

    private static global::AI.Sandbox.Engine.Core.Prompting
        .PromptCompositionResult<ComposeRequest, PromptText, RenderedPrompt>
        ComposeFixed(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<PromptWorldState> manager,
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptRequestEnvelope<ComposeRequest> request,
            IEnumerable<
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptCandidateEnvelope<PromptText>> candidates,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptComposerIdKind> composerId,
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptDocumentEnvelope<RenderedPrompt> document)
    {
        var composer = new FixedComposer(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionDecision<RenderedPrompt>.Compose(document));

        return CreateProcessor(
            manager,
            composerId,
            composer).Compose(request, candidates);
    }

    private static global::AI.Sandbox.Engine.Core.Prompting
        .PromptCompositionProcessor<
            PromptWorldState,
            ComposeRequest,
            PromptText,
            RenderedPrompt> CreateProcessor(
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateManager<PromptWorldState> manager,
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptComposerIdKind> composerId,
                global::AI.Sandbox.Engine.Core.Prompting.IPromptComposer<
                    PromptWorldState,
                    ComposeRequest,
                    PromptText,
                    RenderedPrompt> composer) =>
        global::AI.Sandbox.Engine.Core.Prompting.PromptCompositionProcessor<
            PromptWorldState,
            ComposeRequest,
            PromptText,
            RenderedPrompt>.Create(
                manager,
                composerId,
                composer);

    private static global::AI.Sandbox.Engine.Core.Prompting
        .PromptRequestEnvelope<ComposeRequest> CreateRequest(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateSnapshot<PromptWorldState> snapshot,
            int suffix,
            int budget) =>
        global::AI.Sandbox.Engine.Core.Prompting
            .PromptRequestEnvelope<ComposeRequest>.Create(
                RequestId(suffix),
                OwnerId(),
                snapshot.WorldId,
                snapshot.Version,
                snapshot.SimulationTick,
                Budget(budget),
                new ComposeRequest("topic"));

    private static global::AI.Sandbox.Engine.Core.Prompting
        .PromptCandidateEnvelope<PromptText> CreateCandidate(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptCandidateIdKind> candidateId,
            global::AI.Sandbox.Engine.Core.Prompting.PromptInclusionMode mode,
            int priority,
            int cost,
            string text,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>?
                    ownerId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>?
                    worldId = null) =>
        global::AI.Sandbox.Engine.Core.Prompting
            .PromptCandidateEnvelope<PromptText>.Create(
                candidateId,
                ownerId ?? OwnerId(),
                worldId ?? WorldId(),
                mode,
                Priority(priority),
                Cost(cost),
                new PromptText(text));

    private static global::AI.Sandbox.Engine.Core.Prompting
        .PromptDocumentEnvelope<RenderedPrompt> CreateDocument(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptDocumentIdKind> documentId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptComposerIdKind> composerId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
            int cost,
            string text) =>
        global::AI.Sandbox.Engine.Core.Prompting
            .PromptDocumentEnvelope<RenderedPrompt>.Create(
                documentId,
                composerId,
                ownerId,
                worldId,
                Cost(cost),
                new RenderedPrompt(text));

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<PromptWorldState> CreateManager() =>
        global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<PromptWorldState>.Create(
                WorldId(),
                new PromptWorldState(0));

    private static global::AI.Sandbox.Engine.Core.Runtime
        .RuntimeOrchestrator<PromptWorldState> CreateRuntime(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<PromptWorldState> manager) =>
        new global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestratorBuilder<PromptWorldState>()
            .AddCommandHandler(new AdvanceValueHandler())
            .Build(manager);

    private sealed class AdvanceValueHandler :
        global::AI.Sandbox.Engine.Core.Commands.ICommandHandler<
            PromptWorldState,
            AdvanceValue>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<PromptWorldState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<PromptWorldState, AdvanceValue> context)
        {
            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<PromptWorldState>.Accept(
                    context.Snapshot.State with
                    {
                        Value = checked(
                            context.Snapshot.State.Value +
                            context.Envelope.Payload.Delta),
                    });
        }
    }

    private sealed class FixedComposer :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptComposer<
            PromptWorldState,
            ComposeRequest,
            PromptText,
            RenderedPrompt>
    {
        private readonly global::AI.Sandbox.Engine.Core.Prompting
            .PromptCompositionDecision<RenderedPrompt> decision;

        public FixedComposer(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionDecision<RenderedPrompt> decision)
        {
            this.decision = decision;
        }

        public int CallCount { get; private set; }

        public int LastSelectedCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Prompting
            .PromptCompositionDecision<RenderedPrompt> Compose(
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptCompositionContext<
                        PromptWorldState,
                        ComposeRequest,
                        PromptText> context)
        {
            CallCount = checked(CallCount + 1);
            LastSelectedCount =
                context.BudgetResult.SelectedCandidates.Count;
            return decision;
        }
    }

    private sealed class MutatingComposer :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptComposer<
            PromptWorldState,
            ComposeRequest,
            PromptText,
            RenderedPrompt>
    {
        private readonly global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestrator<PromptWorldState> runtime;
        private readonly global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Prompting.PromptComposerIdKind>
                composerId;

        public MutatingComposer(
            global::AI.Sandbox.Engine.Core.Runtime
                .RuntimeOrchestrator<PromptWorldState> runtime,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Prompting.PromptComposerIdKind>
                    composerId)
        {
            this.runtime = runtime;
            this.composerId = composerId;
        }

        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Prompting
            .PromptCompositionDecision<RenderedPrompt> Compose(
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptCompositionContext<
                        PromptWorldState,
                        ComposeRequest,
                        PromptText> context)
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

            return global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionDecision<RenderedPrompt>.Compose(
                    CreateDocument(
                        DocumentId(900 + CallCount),
                        composerId,
                        context.Request.OwnerEntityId,
                        context.Request.WorldId,
                        1,
                        "discarded"));
        }
    }

    private sealed class ThrowingComposer :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptComposer<
            PromptWorldState,
            ComposeRequest,
            PromptText,
            RenderedPrompt>
    {
        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Prompting
            .PromptCompositionDecision<RenderedPrompt> Compose(
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptCompositionContext<
                        PromptWorldState,
                        ComposeRequest,
                        PromptText> context)
        {
            _ = context;
            CallCount = checked(CallCount + 1);
            throw new InvalidOperationException("composer failure");
        }
    }

    private static global::AI.Sandbox.Engine.Core.Prompting.PromptBudget
        Budget(int units) =>
        global::AI.Sandbox.Engine.Core.Prompting.PromptBudget.FromUnits(units);

    private static global::AI.Sandbox.Engine.Core.Prompting.PromptCost
        Cost(int units) =>
        global::AI.Sandbox.Engine.Core.Prompting.PromptCost.FromUnits(units);

    private static global::AI.Sandbox.Engine.Core.Prompting.PromptPriority
        Priority(int basisPoints) =>
        global::AI.Sandbox.Engine.Core.Prompting.PromptPriority
            .FromBasisPoints(basisPoints);

    private static global::AI.Sandbox.Engine.Core.Prompting.PromptRejectionCode
        RejectionCode(string text) =>
        global::AI.Sandbox.Engine.Core.Prompting.PromptRejectionCode
            .Parse(text);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7600-8600-000000000001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> OtherWorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7600-8600-000000000002");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OwnerId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(
                "019b0000-0000-7700-8700-000000000001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OtherOwnerId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(
                "019b0000-0000-7700-8700-000000000002");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Prompting.PromptRequestIdKind>
            RequestId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptRequestIdKind>.Parse(
                    $"019b0000-0000-7800-8800-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Prompting.PromptComposerIdKind>
            ComposerId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptComposerIdKind>.Parse(
                    $"019b0000-0000-7900-8900-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Prompting.PromptCandidateIdKind>
            CandidateId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCandidateIdKind>.Parse(
                    $"019b0000-0000-7a00-8a00-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Prompting.PromptDocumentIdKind>
            DocumentId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptDocumentIdKind>.Parse(
                    $"019b0000-0000-7b00-8b00-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Commands.CommandIdKind>
            CommandId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Commands.CommandIdKind>.Parse(
                $"019b0000-0000-7c00-8c00-{suffix:D12}");
}
