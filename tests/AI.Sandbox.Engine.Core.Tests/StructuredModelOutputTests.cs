namespace AI.Sandbox.Engine.Core.Tests;

public sealed class StructuredModelOutputTests
{
    private readonly record struct RawResponse(string Text) :
        global::AI.Sandbox.Engine.Core.Modeling.IModelResponse;

    private readonly record struct DialogueOutput(
        string Reply,
        string ActionProposal) :
        global::AI.Sandbox.Engine.Core.StructuredOutput
            .IStructuredModelOutput;

    private readonly record struct PlanningOutput(string Goal) :
        global::AI.Sandbox.Engine.Core.StructuredOutput
            .IStructuredModelOutput;

    private abstract record AbstractOutput :
        global::AI.Sandbox.Engine.Core.StructuredOutput
            .IStructuredModelOutput;

    private sealed record ConcreteAbstractOutput(string Text) : AbstractOutput;

    private record OpenOutput(string Text) :
        global::AI.Sandbox.Engine.Core.StructuredOutput
            .IStructuredModelOutput;

    private readonly record struct AdvanceValue(int Delta) :
        global::AI.Sandbox.Engine.Core.Commands.IEngineCommand;

    private sealed record StructuredWorldState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    [Xunit.Fact]
    public void PayloadTypesSchemaVersionsAndCodesMustBeExactAndBounded()
    {
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => SchemaVersion(0));
        Xunit.Assert.Throws<FormatException>(
            () => global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputRejectionCode.Parse("Invalid Code"));

        var code = global::AI.Sandbox.Engine.Core.StructuredOutput
            .StructuredOutputRejectionCode.Parse("schema.invalid");
        Xunit.Assert.Equal("schema.invalid", code.Value);

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputDecision<AbstractOutput>.Decode(
                    new ConcreteAbstractOutput("invalid")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputDecision<OpenOutput>.Decode(
                    new OpenOutput("invalid")));

        var decision = global::AI.Sandbox.Engine.Core.StructuredOutput
            .StructuredOutputDecision<DialogueOutput>.Decode(
                new DialogueOutput("hello", "wait"));

        Xunit.Assert.Equal("hello", decision.Payload.Reply);
    }

    [Xunit.Fact]
    public void RequestPreservesSchemaAndSourceResponseCorrelation()
    {
        var snapshot = CreateManager().Read();
        var response = CreateResponse(snapshot, 10);
        var request = CreateRequest(response, 10);

        Xunit.Assert.Equal(OutputId(10), request.OutputId);
        Xunit.Assert.Equal(DecoderId(1), request.DecoderId);
        Xunit.Assert.Equal(SchemaId(1), request.SchemaId);
        Xunit.Assert.Equal(1, request.SchemaVersion.Value);
        Xunit.Assert.Same(response, request.SourceResponse);
        Xunit.Assert.Equal(snapshot.WorldId, response.WorldId);
        Xunit.Assert.Equal(snapshot.Version, response.WorldStateVersion);
        Xunit.Assert.Equal(snapshot.SimulationTick, response.SimulationTick);
    }

    [Xunit.Fact]
    public void DecoderProducesCorrelatedOutputExactlyOnce()
    {
        var manager = CreateManager();
        var decoder = new FixedDecoder(
            global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputDecision<DialogueOutput>.Decode(
                    new DialogueOutput("answer", "open-door")));
        var processor = CreateProcessor(manager, decoder);
        var snapshot = manager.Read();
        var response = CreateResponse(snapshot, 20);
        var request = CreateRequest(response, 20);

        var result = processor.Process(request);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputProcessingStatus.Decoded,
            result.Status);
        Xunit.Assert.True(result.DecoderWasInvoked);
        Xunit.Assert.True(result.HasStableDecision);
        Xunit.Assert.True(result.WasDecoded);
        Xunit.Assert.Equal(1, decoder.CallCount);
        Xunit.Assert.NotNull(result.Decision);
        Xunit.Assert.NotNull(result.Output);

        var output = result.Output!;
        Xunit.Assert.Equal(request.OutputId, output.OutputId);
        Xunit.Assert.Equal(request.DecoderId, output.DecoderId);
        Xunit.Assert.Equal(request.SchemaId, output.SchemaId);
        Xunit.Assert.Equal(request.SchemaVersion, output.SchemaVersion);
        Xunit.Assert.Equal(response.InvocationId, output.SourceInvocationId);
        Xunit.Assert.Equal(response.AdapterId, output.SourceAdapterId);
        Xunit.Assert.Equal(
            response.ModelProfileId,
            output.SourceModelProfileId);
        Xunit.Assert.Equal(
            response.PromptDocumentId,
            output.SourcePromptDocumentId);
        Xunit.Assert.Equal(response.OwnerEntityId, output.OwnerEntityId);
        Xunit.Assert.Equal(response.WorldId, output.WorldId);
        Xunit.Assert.Equal(
            response.WorldStateVersion,
            output.WorldStateVersion);
        Xunit.Assert.Equal(response.SimulationTick, output.SimulationTick);
        Xunit.Assert.Equal(response.Usage, output.SourceUsage);
        Xunit.Assert.Equal("answer", output.Payload.Reply);
        Xunit.Assert.Equal("open-door", output.Payload.ActionProposal);
    }

    [Xunit.Fact]
    public void DecoderMayRejectWithoutProducingOutput()
    {
        var manager = CreateManager();
        var decoder = new FixedDecoder(
            global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputDecision<DialogueOutput>.Reject(
                    global::AI.Sandbox.Engine.Core.StructuredOutput
                        .StructuredOutputRejectionCode.Parse(
                            "invalid.structure")));
        var processor = CreateProcessor(manager, decoder);
        var request = CreateRequest(
            CreateResponse(manager.Read(), 30),
            30);

        var result = processor.Process(request);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputProcessingStatus.Rejected,
            result.Status);
        Xunit.Assert.True(result.DecoderWasInvoked);
        Xunit.Assert.True(result.HasStableDecision);
        Xunit.Assert.False(result.WasDecoded);
        Xunit.Assert.Equal(1, decoder.CallCount);
        Xunit.Assert.NotNull(result.Decision);
        Xunit.Assert.Null(result.Output);
        Xunit.Assert.Equal(
            "invalid.structure",
            result.Decision!.RejectionCode.Value);
    }

    [Xunit.Fact]
    public void PreflightMismatchesSkipDecoder()
    {
        var manager = CreateManager();
        var snapshot = manager.Read();
        var decoder = new FixedDecoder(
            global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputDecision<DialogueOutput>.Decode(
                    new DialogueOutput("unused", "unused")));
        var processor = CreateProcessor(manager, decoder);

        var cases = new[]
        {
            (
                global::AI.Sandbox.Engine.Core.StructuredOutput
                    .StructuredOutputProcessingStatus.DecoderMismatch,
                CreateRequest(
                    CreateResponse(snapshot, 40),
                    40,
                    decoderId: DecoderId(2))),
            (
                global::AI.Sandbox.Engine.Core.StructuredOutput
                    .StructuredOutputProcessingStatus.SchemaMismatch,
                CreateRequest(
                    CreateResponse(snapshot, 41),
                    41,
                    schemaId: SchemaId(2))),
            (
                global::AI.Sandbox.Engine.Core.StructuredOutput
                    .StructuredOutputProcessingStatus.SchemaVersionMismatch,
                CreateRequest(
                    CreateResponse(snapshot, 42),
                    42,
                    schemaVersion: SchemaVersion(2))),
            (
                global::AI.Sandbox.Engine.Core.StructuredOutput
                    .StructuredOutputProcessingStatus.WorldMismatch,
                CreateRequest(
                    CreateResponse(
                        OtherWorldId(),
                        snapshot.Version,
                        snapshot.SimulationTick,
                        43),
                    43)),
            (
                global::AI.Sandbox.Engine.Core.StructuredOutput
                    .StructuredOutputProcessingStatus.VersionConflict,
                CreateRequest(
                    CreateResponse(
                        snapshot.WorldId,
                        global::AI.Sandbox.Engine.Core.WorldState
                            .WorldStateVersion.From(
                                checked(snapshot.Version.Value + 1)),
                        snapshot.SimulationTick,
                        44),
                    44)),
            (
                global::AI.Sandbox.Engine.Core.StructuredOutput
                    .StructuredOutputProcessingStatus
                        .SimulationTickMismatch,
                CreateRequest(
                    CreateResponse(
                        snapshot.WorldId,
                        snapshot.Version,
                        checked(snapshot.SimulationTick + 1),
                        45),
                    45)),
        };

        foreach (var item in cases)
        {
            var result = processor.Process(item.Item2);
            Xunit.Assert.Equal(item.Item1, result.Status);
            Xunit.Assert.False(result.DecoderWasInvoked);
            Xunit.Assert.False(result.HasStableDecision);
            Xunit.Assert.Null(result.Decision);
            Xunit.Assert.Null(result.Output);
        }

        Xunit.Assert.Equal(0, decoder.CallCount);
    }

    [Xunit.Fact]
    public void AuthorityChangeDiscardsDecisionWithoutRetry()
    {
        var manager = CreateManager();
        var runtime = CreateRuntime(manager);
        var decoder = new ConflictDecoder(runtime);
        var processor = CreateProcessor(manager, decoder);
        var request = CreateRequest(
            CreateResponse(manager.Read(), 50),
            50);

        var result = processor.Process(request);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputProcessingStatus.AuthorityChanged,
            result.Status);
        Xunit.Assert.True(result.DecoderWasInvoked);
        Xunit.Assert.False(result.HasStableDecision);
        Xunit.Assert.False(result.WasDecoded);
        Xunit.Assert.Null(result.Decision);
        Xunit.Assert.Null(result.Output);
        Xunit.Assert.Equal(1, decoder.CallCount);
        Xunit.Assert.Equal(1, manager.Read().State.Value);
    }

    [Xunit.Fact]
    public void DecoderExceptionPropagatesWithoutRetry()
    {
        var manager = CreateManager();
        var decoder = new ThrowingDecoder();
        var processor = CreateProcessor(manager, decoder);
        var request = CreateRequest(
            CreateResponse(manager.Read(), 60),
            60);

        var exception = Xunit.Assert.Throws<InvalidOperationException>(
            () => processor.Process(request));

        Xunit.Assert.Equal("decoder failure", exception.Message);
        Xunit.Assert.Equal(1, decoder.CallCount);
        Xunit.Assert.Equal(0, manager.Read().State.Value);
    }

    [Xunit.Fact]
    public void MultipleExactSchemasRemainHostDefinedWithoutClosedDirectiveEnum()
    {
        var dialogue = global::AI.Sandbox.Engine.Core.StructuredOutput
            .StructuredOutputDecision<DialogueOutput>.Decode(
                new DialogueOutput("speak", "move"));
        var planning = global::AI.Sandbox.Engine.Core.StructuredOutput
            .StructuredOutputDecision<PlanningOutput>.Decode(
                new PlanningOutput("find shelter"));

        Xunit.Assert.Equal("speak", dialogue.Payload.Reply);
        Xunit.Assert.Equal("find shelter", planning.Payload.Goal);
        Xunit.Assert.NotEqual(
            typeof(DialogueOutput),
            typeof(PlanningOutput));
    }

    [Xunit.Fact]
    public void DecodingRemainsReadOnlyAndNonAuthoritative()
    {
        var manager = CreateManager();
        var before = manager.Read();
        var decoder = new FixedDecoder(
            global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputDecision<DialogueOutput>.Decode(
                    new DialogueOutput("promise", "change-world")));
        var processor = CreateProcessor(manager, decoder);

        var result = processor.Process(
            CreateRequest(CreateResponse(before, 70), 70));
        var after = manager.Read();

        Xunit.Assert.True(result.WasDecoded);
        Xunit.Assert.Equal(before.Version, after.Version);
        Xunit.Assert.Equal(before.SimulationTick, after.SimulationTick);
        Xunit.Assert.Same(before.State, after.State);
        Xunit.Assert.Equal(0, after.State.Value);
    }

    private static global::AI.Sandbox.Engine.Core.StructuredOutput
        .StructuredOutputProcessor<
            StructuredWorldState,
            RawResponse,
            DialogueOutput> CreateProcessor(
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateManager<StructuredWorldState> manager,
                global::AI.Sandbox.Engine.Core.StructuredOutput
                    .IStructuredOutputDecoder<
                        StructuredWorldState,
                        RawResponse,
                        DialogueOutput> decoder) =>
        global::AI.Sandbox.Engine.Core.StructuredOutput
            .StructuredOutputProcessor<
                StructuredWorldState,
                RawResponse,
                DialogueOutput>.Create(
                    manager,
                    DecoderId(1),
                    SchemaId(1),
                    SchemaVersion(1),
                    decoder);

    private static global::AI.Sandbox.Engine.Core.StructuredOutput
        .StructuredOutputRequestEnvelope<RawResponse> CreateRequest(
            global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationResponseEnvelope<RawResponse> response,
            int outputSuffix,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.StructuredOutput
                    .StructuredOutputDecoderIdKind>? decoderId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.StructuredOutput
                    .StructuredOutputSchemaIdKind>? schemaId = null,
            global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputSchemaVersion? schemaVersion = null) =>
        global::AI.Sandbox.Engine.Core.StructuredOutput
            .StructuredOutputRequestEnvelope<RawResponse>.Create(
                OutputId(outputSuffix),
                decoderId ?? DecoderId(1),
                schemaId ?? SchemaId(1),
                schemaVersion ?? SchemaVersion(1),
                response);

    private static global::AI.Sandbox.Engine.Core.Modeling
        .ModelInvocationResponseEnvelope<RawResponse> CreateResponse(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateSnapshot<StructuredWorldState> snapshot,
            int suffix) =>
        CreateResponse(
            snapshot.WorldId,
            snapshot.Version,
            snapshot.SimulationTick,
            suffix);

    private static global::AI.Sandbox.Engine.Core.Modeling
        .ModelInvocationResponseEnvelope<RawResponse> CreateResponse(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
            global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion version,
            ulong simulationTick,
            int suffix) =>
        global::AI.Sandbox.Engine.Core.Modeling
            .ModelInvocationResponseEnvelope<RawResponse>.Create(
                InvocationId(suffix),
                AdapterId(1),
                ProfileId(1),
                PromptDocumentId(suffix),
                OwnerId(),
                worldId,
                version,
                simulationTick,
                global::AI.Sandbox.Engine.Core.Modeling.ModelUsage.Create(10, 5),
                new RawResponse($"response-{suffix}"));

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<StructuredWorldState> CreateManager() =>
        global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<StructuredWorldState>.Create(
                WorldId(),
                new StructuredWorldState(0));

    private static global::AI.Sandbox.Engine.Core.Runtime
        .RuntimeOrchestrator<StructuredWorldState> CreateRuntime(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<StructuredWorldState> manager) =>
        new global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestratorBuilder<StructuredWorldState>()
            .AddCommandHandler(new AdvanceValueHandler())
            .Build(manager);

    private sealed class AdvanceValueHandler :
        global::AI.Sandbox.Engine.Core.Commands.ICommandHandler<
            StructuredWorldState,
            AdvanceValue>
    {
        public global::AI.Sandbox.Engine.Core.Commands
            .CommandDecision<StructuredWorldState> Evaluate(
                global::AI.Sandbox.Engine.Core.Commands
                    .CommandContext<StructuredWorldState, AdvanceValue>
                        context)
        {
            return global::AI.Sandbox.Engine.Core.Commands
                .CommandDecision<StructuredWorldState>.Accept(
                    context.Snapshot.State with
                    {
                        Value = checked(
                            context.Snapshot.State.Value +
                            context.Envelope.Payload.Delta),
                    });
        }
    }

    private sealed class FixedDecoder :
        global::AI.Sandbox.Engine.Core.StructuredOutput
            .IStructuredOutputDecoder<
                StructuredWorldState,
                RawResponse,
                DialogueOutput>
    {
        private readonly global::AI.Sandbox.Engine.Core.StructuredOutput
            .StructuredOutputDecision<DialogueOutput> decision;

        public FixedDecoder(
            global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputDecision<DialogueOutput> decision)
        {
            this.decision = decision;
        }

        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.StructuredOutput
            .StructuredOutputDecision<DialogueOutput> Decode(
                global::AI.Sandbox.Engine.Core.StructuredOutput
                    .StructuredOutputContext<
                        StructuredWorldState,
                        RawResponse> context)
        {
            CallCount = checked(CallCount + 1);
            return decision;
        }
    }

    private sealed class ConflictDecoder :
        global::AI.Sandbox.Engine.Core.StructuredOutput
            .IStructuredOutputDecoder<
                StructuredWorldState,
                RawResponse,
                DialogueOutput>
    {
        private readonly global::AI.Sandbox.Engine.Core.Runtime
            .RuntimeOrchestrator<StructuredWorldState> runtime;

        public ConflictDecoder(
            global::AI.Sandbox.Engine.Core.Runtime
                .RuntimeOrchestrator<StructuredWorldState> runtime)
        {
            this.runtime = runtime;
        }

        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.StructuredOutput
            .StructuredOutputDecision<DialogueOutput> Decode(
                global::AI.Sandbox.Engine.Core.StructuredOutput
                    .StructuredOutputContext<
                        StructuredWorldState,
                        RawResponse> context)
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

            return global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputDecision<DialogueOutput>.Decode(
                    new DialogueOutput("late", "late"));
        }
    }

    private sealed class ThrowingDecoder :
        global::AI.Sandbox.Engine.Core.StructuredOutput
            .IStructuredOutputDecoder<
                StructuredWorldState,
                RawResponse,
                DialogueOutput>
    {
        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.StructuredOutput
            .StructuredOutputDecision<DialogueOutput> Decode(
                global::AI.Sandbox.Engine.Core.StructuredOutput
                    .StructuredOutputContext<
                        StructuredWorldState,
                        RawResponse> context)
        {
            CallCount = checked(CallCount + 1);
            throw new InvalidOperationException("decoder failure");
        }
    }

    private static global::AI.Sandbox.Engine.Core.StructuredOutput
        .StructuredOutputSchemaVersion SchemaVersion(int value) =>
        global::AI.Sandbox.Engine.Core.StructuredOutput
            .StructuredOutputSchemaVersion.From(value);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019d0000-0000-7000-8000-000000000001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> OtherWorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019d0000-0000-7000-8000-000000000002");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OwnerId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(
                "019d0000-0000-7100-8100-000000000001");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Modeling.ModelInvocationIdKind>
            InvocationId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationIdKind>.Parse(
                    $"019d0000-0000-7200-8200-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Modeling.ModelAdapterIdKind>
            AdapterId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Modeling.ModelAdapterIdKind>.Parse(
                $"019d0000-0000-7300-8300-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Modeling.ModelProfileIdKind>
            ProfileId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Modeling.ModelProfileIdKind>.Parse(
                $"019d0000-0000-7400-8400-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Prompting.PromptDocumentIdKind>
            PromptDocumentId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptDocumentIdKind>.Parse(
                    $"019d0000-0000-7500-8500-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.StructuredOutput.StructuredOutputIdKind>
            OutputId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputIdKind>.Parse(
                    $"019d0000-0000-7600-8600-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.StructuredOutput
            .StructuredOutputDecoderIdKind> DecoderId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputDecoderIdKind>.Parse(
                    $"019d0000-0000-7700-8700-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.StructuredOutput
            .StructuredOutputSchemaIdKind> SchemaId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.StructuredOutput
                .StructuredOutputSchemaIdKind>.Parse(
                    $"019d0000-0000-7800-8800-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Commands.CommandIdKind> CommandId(
            int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Commands.CommandIdKind>.Parse(
                $"019d0000-0000-7900-8900-{suffix:D12}");
}
