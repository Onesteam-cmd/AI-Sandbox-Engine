namespace AI.Sandbox.Engine.FoundationProbe;

internal static class ProductPipelineProbe
{
    private readonly record struct ContextQuery(string Topic) :
        global::AI.Sandbox.Engine.Core.ContextRetrieval.IContextQuery;
    private readonly record struct ContextItem(string Text) :
        global::AI.Sandbox.Engine.Core.ContextRetrieval.IContextItem;
    private readonly record struct PromptRequest(string UserText) :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptRequest;
    private readonly record struct PromptContent(string Text) :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptContent;
    private readonly record struct PromptDocument(string Text) :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptDocument;
    private readonly record struct ModelRequest(string Text) :
        global::AI.Sandbox.Engine.Core.Modeling.IModelRequest;
    private readonly record struct ModelResponse(string Text) :
        global::AI.Sandbox.Engine.Core.Modeling.IModelResponse;
    private readonly record struct StructuredReply(string Reply, int Delta) :
        global::AI.Sandbox.Engine.Core.StructuredOutput.IStructuredModelOutput;
    private readonly record struct MoveBy(int Delta) :
        global::AI.Sandbox.Engine.Core.Behavior.IActionProposal;
    private readonly record struct ChangeValue(int Delta) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;
    private sealed record PipelineWorldState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    internal sealed record Result(
        string Status,
        string ContextStatus,
        string PromptStatus,
        string ModelStatus,
        string StructuredStatus,
        string ActionStatus,
        string RuntimeStatus,
        int RetrieverCalls,
        int ComposerCalls,
        int AdapterCalls,
        int DecoderCalls,
        int ValidatorCalls,
        bool AuthorityUnchangedBeforeCommand,
        int BeforeValue,
        int AfterValue,
        ulong BeforeVersion,
        ulong AfterVersion,
        string Reply);

    internal static async Task<Result> RunAsync()
    {
        var manager =
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<PipelineWorldState>.Create(
                    WorldId(),
                    new PipelineWorldState(0));

        var before = manager.Read();
        var ownerId = OwnerId();
        var retrieverId = RetrieverId();

        var contextItem =
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextItemEnvelope<ContextItem>.Create(
                    ContextItemId(),
                    retrieverId,
                    ownerId,
                    before.WorldId,
                    global::AI.Sandbox.Engine.Core.ContextRetrieval
                        .ContextRelevance.FromBasisPoints(9_000),
                    new ContextItem(
                        "witness saw the suspect near the station"));

        var retriever = new FixedRetriever(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalDecision<ContextItem>.Retrieve(
                    new[] { contextItem }));

        var contextProcessor =
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalProcessor<
                    PipelineWorldState,
                    ContextQuery,
                    ContextItem>.Create(
                        manager,
                        retrieverId,
                        retriever);

        var contextResult = contextProcessor.Retrieve(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextQueryEnvelope<ContextQuery>.Create(
                    ContextQueryId(),
                    ownerId,
                    before.WorldId,
                    before.Version,
                    before.SimulationTick,
                    global::AI.Sandbox.Engine.Core.ContextRetrieval
                        .ContextItemLimit.From(1),
                    new ContextQuery("suspect-location")));

        if (!contextResult.WasRetrieved ||
            retriever.CallCount != 1 ||
            contextResult.Decision is null ||
            contextResult.Decision.Items.Count != 1 ||
            contextResult.Decision.Items[0].Payload.Text !=
                "witness saw the suspect near the station")
        {
            throw new InvalidOperationException(
                "Product pipeline context retrieval invariants failed.");
        }

        var promptRequest =
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptRequestEnvelope<PromptRequest>.Create(
                    PromptRequestId(),
                    ownerId,
                    before.WorldId,
                    before.Version,
                    before.SimulationTick,
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptBudget.FromUnits(8),
                    new PromptRequest("Where were you last night?"));

        var contextCandidate =
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCandidateEnvelope<PromptContent>.Create(
                    PromptCandidateId(1),
                    ownerId,
                    before.WorldId,
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptInclusionMode.Required,
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptPriority.FromBasisPoints(9_000),
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptCost.FromUnits(4),
                    new PromptContent(
                        contextResult.Decision.Items[0].Payload.Text));

        var userCandidate =
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCandidateEnvelope<PromptContent>.Create(
                    PromptCandidateId(2),
                    ownerId,
                    before.WorldId,
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptInclusionMode.Required,
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptPriority.FromBasisPoints(8_000),
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptCost.FromUnits(4),
                    new PromptContent(promptRequest.Payload.UserText));

        var composerId = PromptComposerId();
        var expectedDocument =
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptDocumentEnvelope<PromptDocument>.Create(
                    PromptDocumentId(),
                    composerId,
                    ownerId,
                    before.WorldId,
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptCost.FromUnits(8),
                    new PromptDocument(
                        "Context: witness saw the suspect near the station\n" +
                        "User: Where were you last night?"));

        var composer = new FixedComposer(expectedDocument);
        var promptProcessor =
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionProcessor<
                    PipelineWorldState,
                    PromptRequest,
                    PromptContent,
                    PromptDocument>.Create(
                        manager,
                        composerId,
                        composer);

        var promptResult = promptProcessor.Compose(
            promptRequest,
            new[] { userCandidate, contextCandidate });

        if (!promptResult.WasComposed ||
            !promptResult.ComposerWasInvoked ||
            composer.CallCount != 1 ||
            promptResult.Decision is null ||
            promptResult.Decision.Document is null ||
            !ReferenceEquals(
                expectedDocument,
                promptResult.Decision.Document))
        {
            throw new InvalidOperationException(
                "Product pipeline prompt composition invariants failed.");
        }

        var composedDocument = promptResult.Decision.Document;
        var modelRequest =
            global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationRequestEnvelope<ModelRequest>.Create(
                    ModelInvocationId(),
                    ModelAdapterId(),
                    ModelProfileId(),
                    composedDocument.DocumentId,
                    ownerId,
                    before.WorldId,
                    before.Version,
                    before.SimulationTick,
                    global::AI.Sandbox.Engine.Core.Modeling
                        .ModelOutputLimit.FromUnits(32),
                    new ModelRequest(composedDocument.Payload.Text));

        var adapter = new FakeAdapter();
        var modelProcessor =
            global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationProcessor<ModelRequest, ModelResponse>.Create(
                    modelRequest.AdapterId,
                    adapter);

        var modelResult = await modelProcessor.InvokeAsync(modelRequest);

        if (!modelResult.WasCompleted ||
            !modelResult.AdapterWasInvoked ||
            adapter.CallCount != 1 ||
            modelResult.Decision?.Response is null)
        {
            throw new InvalidOperationException(
                "Product pipeline model invocation invariants failed.");
        }

        var response = modelResult.Decision.Response;
        var decoder = new FakeDecoder();
        var structuredProcessor =
            global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputProcessor<
                    PipelineWorldState,
                    ModelResponse,
                    StructuredReply>.Create(
                        manager,
                        StructuredDecoderId(),
                        StructuredSchemaId(),
                        global::AI.Sandbox.Engine.Core.StructuredOutput
                            .StructuredOutputSchemaVersion.From(1),
                        decoder);

        var structuredResult = structuredProcessor.Process(
            global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputRequestEnvelope<ModelResponse>.Create(
                    StructuredOutputId(),
                    StructuredDecoderId(),
                    StructuredSchemaId(),
                    global::AI.Sandbox.Engine.Core.StructuredOutput
                        .StructuredOutputSchemaVersion.From(1),
                    response));

        if (!structuredResult.WasDecoded ||
            !structuredResult.DecoderWasInvoked ||
            !structuredResult.HasStableDecision ||
            decoder.CallCount != 1 ||
            structuredResult.Output is null ||
            structuredResult.Output.Payload.Reply !=
                "I was near the station." ||
            structuredResult.Output.Payload.Delta != 2)
        {
            throw new InvalidOperationException(
                "Product pipeline structured output invariants failed.");
        }

        var afterInference = manager.Read();
        var authorityUnchangedBeforeCommand =
            afterInference.WorldId == before.WorldId &&
            ReferenceEquals(afterInference.State, before.State) &&
            afterInference.State.Value == before.State.Value &&
            afterInference.Version == before.Version &&
            afterInference.SimulationTick == before.SimulationTick;

        if (!authorityUnchangedBeforeCommand)
        {
            throw new InvalidOperationException(
                "Inference stages mutated authoritative World State.");
        }

        var validator = new ApprovingValidator();
        var actionProcessor =
            global::AI.Sandbox.Engine.Core.Behavior
                .ActionValidationProcessor<
                    PipelineWorldState,
                    MoveBy,
                    ChangeValue>.Create(
                        manager,
                        validator);

        var actionResult = actionProcessor.Validate(
            global::AI.Sandbox.Engine.Core.Behavior
                .ActionProposalEnvelope<MoveBy>.CreateExternal(
                    ActionProposalId(),
                    ownerId,
                    before.WorldId,
                    before.Version,
                    before.SimulationTick,
                    new MoveBy(structuredResult.Output.Payload.Delta)));

        if (!actionResult.WasApproved ||
            !actionResult.ValidatorWasInvoked ||
            !actionResult.HasStableDecision ||
            validator.CallCount != 1 ||
            actionResult.Decision is null ||
            actionResult.Decision.Command.Delta != 2)
        {
            throw new InvalidOperationException(
                "Product pipeline action validation invariants failed.");
        }

        var runtime =
            new global::AI.Sandbox.Engine.Core.Runtime
                .RuntimeOrchestratorBuilder<PipelineWorldState>()
                .AddCommandHandler(new ChangeValueHandler())
                .Build(manager);

        var runtimeResult = runtime.ExecuteCommand(
            global::AI.Sandbox.Engine.Core.Commands
                .CommandEnvelope<ChangeValue>.Create(
                    CommandId(),
                    before.WorldId,
                    before.Version,
                    before.SimulationTick,
                    actionResult.Decision.Command));

        var after = manager.Read();

        if (!runtimeResult.WasCommitted ||
            after.State.Value != 2 ||
            after.Version.Value != checked(before.Version.Value + 1) ||
            after.SimulationTick != before.SimulationTick)
        {
            throw new InvalidOperationException(
                "Product pipeline runtime command invariants failed.");
        }

        return new Result(
            "Completed",
            contextResult.Status.ToString(),
            promptResult.Status.ToString(),
            modelResult.Status.ToString(),
            structuredResult.Status.ToString(),
            actionResult.Status.ToString(),
            runtimeResult.WasCommitted ? "Committed" : "NotCommitted",
            retriever.CallCount,
            composer.CallCount,
            adapter.CallCount,
            decoder.CallCount,
            validator.CallCount,
            authorityUnchangedBeforeCommand,
            before.State.Value,
            after.State.Value,
            before.Version.Value,
            after.Version.Value,
            structuredResult.Output.Payload.Reply);
    }

    private sealed class FixedRetriever :
        global::AI.Sandbox.Engine.Core.ContextRetrieval.IContextRetriever<
            PipelineWorldState,
            ContextQuery,
            ContextItem>
    {
        private readonly global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextRetrievalDecision<ContextItem> decision;

        internal FixedRetriever(
            global::AI.Sandbox.Engine.Core.ContextRetrieval
                .ContextRetrievalDecision<ContextItem> decision) =>
            this.decision = decision;

        internal int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.ContextRetrieval
            .ContextRetrievalDecision<ContextItem> Retrieve(
                global::AI.Sandbox.Engine.Core.ContextRetrieval
                    .ContextRetrievalContext<PipelineWorldState, ContextQuery>
                        context)
        {
            _ = context;
            CallCount = checked(CallCount + 1);
            return decision;
        }
    }

    private sealed class FixedComposer :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptComposer<
            PipelineWorldState,
            PromptRequest,
            PromptContent,
            PromptDocument>
    {
        private readonly global::AI.Sandbox.Engine.Core.Prompting
            .PromptDocumentEnvelope<PromptDocument> document;

        internal FixedComposer(
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptDocumentEnvelope<PromptDocument> document) =>
            this.document = document;

        internal int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Prompting
            .PromptCompositionDecision<PromptDocument> Compose(
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptCompositionContext<
                        PipelineWorldState,
                        PromptRequest,
                        PromptContent> context)
        {
            CallCount = checked(CallCount + 1);

            if (context.BudgetResult.Status !=
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptBudgetStatus.Selected ||
                context.BudgetResult.SelectedCandidates.Count != 2 ||
                context.BudgetResult.UsedUnits != 8)
            {
                throw new InvalidOperationException(
                    "Product pipeline composer received invalid budget context.");
            }

            return global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionDecision<PromptDocument>.Compose(document);
        }
    }

    private sealed class FakeAdapter :
        global::AI.Sandbox.Engine.Core.Modeling
            .IModelAdapter<ModelRequest, ModelResponse>
    {
        internal int CallCount { get; private set; }

        public ValueTask<global::AI.Sandbox.Engine.Core.Modeling
            .ModelInvocationDecision<ModelResponse>> InvokeAsync(
                global::AI.Sandbox.Engine.Core.Modeling
                    .ModelInvocationContext<ModelRequest> context,
                CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount = checked(CallCount + 1);

            if (!context.Request.Payload.Text.Contains(
                    "witness saw the suspect near the station",
                    StringComparison.Ordinal) ||
                !context.Request.Payload.Text.Contains(
                    "Where were you last night?",
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Fake adapter did not receive the composed product prompt.");
            }

            var request = context.Request;
            var response =
                global::AI.Sandbox.Engine.Core.Modeling
                    .ModelInvocationResponseEnvelope<ModelResponse>.Create(
                        request.InvocationId,
                        request.AdapterId,
                        request.ModelProfileId,
                        request.PromptDocumentId,
                        request.OwnerEntityId,
                        request.WorldId,
                        request.WorldStateVersion,
                        request.SimulationTick,
                        global::AI.Sandbox.Engine.Core.Modeling
                            .ModelUsage.Create(8, 6),
                        new ModelResponse(
                            """{"reply":"I was near the station.","delta":2}"""));

            return ValueTask.FromResult(
                global::AI.Sandbox.Engine.Core.Modeling
                    .ModelInvocationDecision<ModelResponse>.Complete(response));
        }
    }

    private sealed class FakeDecoder :
        global::AI.Sandbox.Engine.Core.StructuredOutput
            .IStructuredOutputDecoder<
                PipelineWorldState,
                ModelResponse,
                StructuredReply>
    {
        internal int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.StructuredOutput
            .StructuredOutputDecision<StructuredReply> Decode(
                global::AI.Sandbox.Engine.Core.StructuredOutput
                    .StructuredOutputContext<
                        PipelineWorldState,
                        ModelResponse> context)
        {
            _ = context;
            CallCount = checked(CallCount + 1);
            return global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputDecision<StructuredReply>.Decode(
                    new StructuredReply("I was near the station.", 2));
        }
    }

    private sealed class ApprovingValidator :
        global::AI.Sandbox.Engine.Core.Behavior.IActionValidator<
            PipelineWorldState,
            MoveBy,
            ChangeValue>
    {
        internal int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Behavior
            .ActionDecision<ChangeValue> Evaluate(
                global::AI.Sandbox.Engine.Core.Behavior
                    .ActionValidationContext<PipelineWorldState, MoveBy>
                        context)
        {
            CallCount = checked(CallCount + 1);
            return global::AI.Sandbox.Engine.Core.Behavior
                .ActionDecision<ChangeValue>.Approve(
                    new ChangeValue(context.Proposal.Payload.Delta));
        }
    }

    private sealed class ChangeValueHandler :
        global::AI.Sandbox.Engine.Core.Commands.ICommandHandler<
            PipelineWorldState,
            ChangeValue>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<PipelineWorldState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<PipelineWorldState, ChangeValue> context) =>
            global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<PipelineWorldState>.Accept(
                    context.Snapshot.State with
                    {
                        Value = checked(
                            context.Snapshot.State.Value +
                            context.Envelope.Payload.Delta),
                    });
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId() =>
        Id<global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>(
            "019e0000-0000-7000-8000-000000000094");
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OwnerId() =>
        Id<global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>(
            "019e0000-0000-7100-8100-000000000094");
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.ContextRetrieval.ContextQueryIdKind>
        ContextQueryId() =>
        Id<global::AI.Sandbox.Engine.Core.ContextRetrieval.ContextQueryIdKind>(
            "019e0000-0000-7200-8200-000000000094");
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.ContextRetrieval.ContextRetrieverIdKind>
        RetrieverId() =>
        Id<global::AI.Sandbox.Engine.Core.ContextRetrieval.ContextRetrieverIdKind>(
            "019e0000-0000-7300-8300-000000000094");
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.ContextRetrieval.ContextItemIdKind>
        ContextItemId() =>
        Id<global::AI.Sandbox.Engine.Core.ContextRetrieval.ContextItemIdKind>(
            "019e0000-0000-7400-8400-000000000094");
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Prompting.PromptRequestIdKind>
        PromptRequestId() =>
        Id<global::AI.Sandbox.Engine.Core.Prompting.PromptRequestIdKind>(
            "019e0000-0000-7500-8500-000000000094");
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Prompting.PromptCandidateIdKind>
        PromptCandidateId(int suffix) =>
        Id<global::AI.Sandbox.Engine.Core.Prompting.PromptCandidateIdKind>(
            $"019e0000-0000-7600-8600-{suffix:D12}");
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Prompting.PromptComposerIdKind>
        PromptComposerId() =>
        Id<global::AI.Sandbox.Engine.Core.Prompting.PromptComposerIdKind>(
            "019e0000-0000-7700-8700-000000000094");
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Prompting.PromptDocumentIdKind>
        PromptDocumentId() =>
        Id<global::AI.Sandbox.Engine.Core.Prompting.PromptDocumentIdKind>(
            "019e0000-0000-7800-8800-000000000094");
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Modeling.ModelInvocationIdKind>
        ModelInvocationId() =>
        Id<global::AI.Sandbox.Engine.Core.Modeling.ModelInvocationIdKind>(
            "019e0000-0000-7900-8900-000000000094");
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Modeling.ModelAdapterIdKind>
        ModelAdapterId() =>
        Id<global::AI.Sandbox.Engine.Core.Modeling.ModelAdapterIdKind>(
            "019e0000-0000-7a00-8a00-000000000094");
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Modeling.ModelProfileIdKind>
        ModelProfileId() =>
        Id<global::AI.Sandbox.Engine.Core.Modeling.ModelProfileIdKind>(
            "019e0000-0000-7b00-8b00-000000000094");
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.StructuredOutput.StructuredOutputIdKind>
        StructuredOutputId() =>
        Id<global::AI.Sandbox.Engine.Core.StructuredOutput.StructuredOutputIdKind>(
            "019e0000-0000-7c00-8c00-000000000094");
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.StructuredOutput.StructuredOutputDecoderIdKind>
        StructuredDecoderId() =>
        Id<global::AI.Sandbox.Engine.Core.StructuredOutput.StructuredOutputDecoderIdKind>(
            "019e0000-0000-7d00-8d00-000000000094");
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.StructuredOutput.StructuredOutputSchemaIdKind>
        StructuredSchemaId() =>
        Id<global::AI.Sandbox.Engine.Core.StructuredOutput.StructuredOutputSchemaIdKind>(
            "019e0000-0000-7e00-8e00-000000000094");
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Behavior.ActionProposalIdKind>
        ActionProposalId() =>
        Id<global::AI.Sandbox.Engine.Core.Behavior.ActionProposalIdKind>(
            "019e0000-0000-7f00-8f00-000000000094");
    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Commands.CommandIdKind> CommandId() =>
        Id<global::AI.Sandbox.Engine.Core.Commands.CommandIdKind>(
            "019e0000-0000-7001-8001-000000000094");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>
        Id<TKind>(string value)
        where TKind : struct =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(value);
}
