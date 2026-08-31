namespace AI.Sandbox.Engine.Core.Tests;

public sealed class ProviderNeutralModelTests
{
    private readonly record struct ModelInput(string Text) :
        global::AI.Sandbox.Engine.Core.Modeling.IModelRequest;

    private abstract record AbstractInput :
        global::AI.Sandbox.Engine.Core.Modeling.IModelRequest;

    private sealed record ConcreteAbstractInput(string Text) : AbstractInput;

    private record OpenInput(string Text) :
        global::AI.Sandbox.Engine.Core.Modeling.IModelRequest;

    private readonly record struct ModelOutput(string Text) :
        global::AI.Sandbox.Engine.Core.Modeling.IModelResponse;

    private abstract record AbstractOutput :
        global::AI.Sandbox.Engine.Core.Modeling.IModelResponse;

    private sealed record ConcreteAbstractOutput(string Text) : AbstractOutput;

    private record OpenOutput(string Text) :
        global::AI.Sandbox.Engine.Core.Modeling.IModelResponse;

    [Xunit.Fact]
    public void PayloadTypesCodesAndUnitsMustBeExactAndBounded()
    {
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationRequestEnvelope<AbstractInput>.Create(
                    InvocationId(1),
                    AdapterId(1),
                    ProfileId(1),
                    PromptDocumentId(1),
                    OwnerId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    OutputLimit(10),
                    new ConcreteAbstractInput("input")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationRequestEnvelope<OpenInput>.Create(
                    InvocationId(2),
                    AdapterId(1),
                    ProfileId(1),
                    PromptDocumentId(1),
                    OwnerId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    OutputLimit(10),
                    new OpenInput("input")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationResponseEnvelope<AbstractOutput>.Create(
                    InvocationId(3),
                    AdapterId(1),
                    ProfileId(1),
                    PromptDocumentId(1),
                    OwnerId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    Usage(1, 1),
                    new ConcreteAbstractOutput("output")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationResponseEnvelope<OpenOutput>.Create(
                    InvocationId(4),
                    AdapterId(1),
                    ProfileId(1),
                    PromptDocumentId(1),
                    OwnerId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    Usage(1, 1),
                    new OpenOutput("output")));

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationDecision<AbstractOutput>.Reject(
                    global::AI.Sandbox.Engine.Core.Modeling
                        .ModelRejectionCode.Parse("policy.denied")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationDecision<OpenOutput>.Fail(
                    global::AI.Sandbox.Engine.Core.Modeling
                        .ModelFailureCode.Parse("transport.failed")));

        Xunit.Assert.False(
            global::AI.Sandbox.Engine.Core.Modeling.ModelOutputLimit
                .TryFromUnits(0, out _));
        Xunit.Assert.False(
            global::AI.Sandbox.Engine.Core.Modeling.ModelOutputLimit
                .TryFromUnits(1000001, out _));
        Xunit.Assert.False(
            global::AI.Sandbox.Engine.Core.Modeling.ModelUsage
                .TryCreate(-1, 0, out _));
        Xunit.Assert.True(
            global::AI.Sandbox.Engine.Core.Modeling.ModelUsage
                .TryCreate(0, 0, out var zeroUsage));
        Xunit.Assert.True(zeroUsage.IsInitialized);
        Xunit.Assert.Throws<FormatException>(
            () => global::AI.Sandbox.Engine.Core.Modeling
                .ModelRejectionCode.Parse("Provider Blocked"));
        Xunit.Assert.Throws<FormatException>(
            () => global::AI.Sandbox.Engine.Core.Modeling
                .ModelFailureCode.Parse("HTTP_500"));
    }

    [Xunit.Fact]
    public void RequestAndResponseEnvelopesPreserveCorrelation()
    {
        var version = global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateVersion.From(3);
        var request = CreateRequest(
            InvocationId(10),
            AdapterId(10),
            ProfileId(10),
            PromptDocumentId(10),
            OwnerId(),
            WorldId(),
            version,
            8,
            100,
            "request");
        var response = CreateResponse(
            InvocationId(10),
            AdapterId(10),
            ProfileId(10),
            PromptDocumentId(10),
            OwnerId(),
            WorldId(),
            version,
            8,
            40,
            12,
            "response");

        Xunit.Assert.Equal(InvocationId(10), request.InvocationId);
        Xunit.Assert.Equal(AdapterId(10), request.AdapterId);
        Xunit.Assert.Equal(ProfileId(10), request.ModelProfileId);
        Xunit.Assert.Equal(PromptDocumentId(10), request.PromptDocumentId);
        Xunit.Assert.Equal(OwnerId(), request.OwnerEntityId);
        Xunit.Assert.Equal(WorldId(), request.WorldId);
        Xunit.Assert.Equal(version, request.WorldStateVersion);
        Xunit.Assert.Equal((ulong)8, request.SimulationTick);
        Xunit.Assert.Equal(100, request.OutputLimit.Units);
        Xunit.Assert.Equal("request", request.Payload.Text);

        Xunit.Assert.Equal(InvocationId(10), response.InvocationId);
        Xunit.Assert.Equal(40, response.Usage.InputUnits);
        Xunit.Assert.Equal(12, response.Usage.OutputUnits);
        Xunit.Assert.Equal("response", response.Payload.Text);
    }

    [Xunit.Fact]
    public async Task ProcessorCompletesWithExactlyOneAdapterInvocation()
    {
        var request = CreateRequest();
        var response = CreateMatchingResponse(request, 9, "completed");
        var adapter = new FixedAdapter(
            global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationDecision<ModelOutput>.Complete(response));
        var processor = CreateProcessor(request.AdapterId, adapter);

        var result = await processor.InvokeAsync(request);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationStatus.Completed,
            result.Status);
        Xunit.Assert.True(result.WasCompleted);
        Xunit.Assert.True(result.AdapterWasInvoked);
        Xunit.Assert.Equal(1, adapter.CallCount);
        Xunit.Assert.Same(request, adapter.LastContext!.Request);
        Xunit.Assert.Equal(request.AdapterId, adapter.LastContext.AdapterId);
        Xunit.Assert.Equal("completed", result.Decision!.Response!.Payload.Text);
    }

    [Xunit.Fact]
    public async Task RequestAdapterMismatchSkipsAdapter()
    {
        var request = CreateRequest(adapterId: AdapterId(2));
        var adapter = new FixedAdapter(
            global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationDecision<ModelOutput>.Fail(
                    FailureCode("not-called")));
        var processor = CreateProcessor(AdapterId(1), adapter);

        var result = await processor.InvokeAsync(request);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationStatus.RequestAdapterMismatch,
            result.Status);
        Xunit.Assert.False(result.AdapterWasInvoked);
        Xunit.Assert.Null(result.Decision);
        Xunit.Assert.Equal(0, adapter.CallCount);
    }

    [Xunit.Fact]
    public async Task AdapterRejectionAndFailureRemainExplicit()
    {
        var request = CreateRequest();
        var rejectedAdapter = new FixedAdapter(
            global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationDecision<ModelOutput>.Reject(
                    RejectionCode("policy.denied")));
        var failedAdapter = new FixedAdapter(
            global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationDecision<ModelOutput>.Fail(
                    FailureCode("transport.unavailable")));

        var rejected = await CreateProcessor(
            request.AdapterId,
            rejectedAdapter).InvokeAsync(request);
        var failed = await CreateProcessor(
            request.AdapterId,
            failedAdapter).InvokeAsync(request);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationStatus.Rejected,
            rejected.Status);
        Xunit.Assert.Equal(
            "policy.denied",
            rejected.Decision!.RejectionCode!.Value.Value);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationStatus.Failed,
            failed.Status);
        Xunit.Assert.Equal(
            "transport.unavailable",
            failed.Decision!.FailureCode!.Value.Value);
        Xunit.Assert.Equal(1, rejectedAdapter.CallCount);
        Xunit.Assert.Equal(1, failedAdapter.CallCount);
    }

    [Xunit.Fact]
    public async Task CompletedResponseCorrelationIsValidated()
    {
        var request = CreateRequest();
        var version = request.WorldStateVersion;
        var cases = new[]
        {
            Case(
                global::AI.Sandbox.Engine.Core.Modeling
                    .ModelInvocationStatus.ResponseInvocationMismatch,
                CreateResponse(
                    InvocationId(2), request.AdapterId, request.ModelProfileId,
                    request.PromptDocumentId, request.OwnerEntityId,
                    request.WorldId, version, request.SimulationTick,
                    5, 5, "invocation")),
            Case(
                global::AI.Sandbox.Engine.Core.Modeling
                    .ModelInvocationStatus.ResponseAdapterMismatch,
                CreateResponse(
                    request.InvocationId, AdapterId(2), request.ModelProfileId,
                    request.PromptDocumentId, request.OwnerEntityId,
                    request.WorldId, version, request.SimulationTick,
                    5, 5, "adapter")),
            Case(
                global::AI.Sandbox.Engine.Core.Modeling
                    .ModelInvocationStatus.ResponseProfileMismatch,
                CreateResponse(
                    request.InvocationId, request.AdapterId, ProfileId(2),
                    request.PromptDocumentId, request.OwnerEntityId,
                    request.WorldId, version, request.SimulationTick,
                    5, 5, "profile")),
            Case(
                global::AI.Sandbox.Engine.Core.Modeling
                    .ModelInvocationStatus.ResponsePromptDocumentMismatch,
                CreateResponse(
                    request.InvocationId, request.AdapterId,
                    request.ModelProfileId, PromptDocumentId(2),
                    request.OwnerEntityId, request.WorldId, version,
                    request.SimulationTick, 5, 5, "prompt")),
            Case(
                global::AI.Sandbox.Engine.Core.Modeling
                    .ModelInvocationStatus.ResponseOwnerMismatch,
                CreateResponse(
                    request.InvocationId, request.AdapterId,
                    request.ModelProfileId, request.PromptDocumentId,
                    OtherOwnerId(), request.WorldId, version,
                    request.SimulationTick, 5, 5, "owner")),
            Case(
                global::AI.Sandbox.Engine.Core.Modeling
                    .ModelInvocationStatus.ResponseWorldMismatch,
                CreateResponse(
                    request.InvocationId, request.AdapterId,
                    request.ModelProfileId, request.PromptDocumentId,
                    request.OwnerEntityId, OtherWorldId(), version,
                    request.SimulationTick, 5, 5, "world")),
            Case(
                global::AI.Sandbox.Engine.Core.Modeling
                    .ModelInvocationStatus.ResponseVersionMismatch,
                CreateResponse(
                    request.InvocationId, request.AdapterId,
                    request.ModelProfileId, request.PromptDocumentId,
                    request.OwnerEntityId, request.WorldId,
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.From(2),
                    request.SimulationTick, 5, 5, "version")),
            Case(
                global::AI.Sandbox.Engine.Core.Modeling
                    .ModelInvocationStatus.ResponseSimulationTickMismatch,
                CreateResponse(
                    request.InvocationId, request.AdapterId,
                    request.ModelProfileId, request.PromptDocumentId,
                    request.OwnerEntityId, request.WorldId, version,
                    1, 5, 5, "tick")),
        };

        foreach (var testCase in cases)
        {
            var adapter = new FixedAdapter(
                global::AI.Sandbox.Engine.Core.Modeling
                    .ModelInvocationDecision<ModelOutput>.Complete(
                        testCase.Response));
            var result = await CreateProcessor(
                request.AdapterId,
                adapter).InvokeAsync(request);

            Xunit.Assert.Equal(testCase.Status, result.Status);
            Xunit.Assert.True(result.AdapterWasInvoked);
            Xunit.Assert.Equal(1, adapter.CallCount);
        }
    }

    [Xunit.Fact]
    public async Task OutputLimitIsEnforcedWithoutRetry()
    {
        var request = CreateRequest(outputLimit: 10);
        var adapter = new FixedAdapter(
            global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationDecision<ModelOutput>.Complete(
                    CreateMatchingResponse(request, 11, "too-large")));

        var result = await CreateProcessor(
            request.AdapterId,
            adapter).InvokeAsync(request);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationStatus.ResponseOutputLimitExceeded,
            result.Status);
        Xunit.Assert.False(result.WasCompleted);
        Xunit.Assert.Equal(1, adapter.CallCount);
    }

    [Xunit.Fact]
    public async Task CancellationAndExceptionPropagateWithoutRetry()
    {
        var request = CreateRequest();
        var canceledAdapter = new FixedAdapter(
            global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationDecision<ModelOutput>.Fail(
                    FailureCode("not-called")));
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Xunit.Assert.ThrowsAsync<OperationCanceledException>(
            async () => await CreateProcessor(
                request.AdapterId,
                canceledAdapter).InvokeAsync(
                    request,
                    cancellation.Token));
        Xunit.Assert.Equal(0, canceledAdapter.CallCount);

        var throwingAdapter = new ThrowingAdapter();
        await Xunit.Assert.ThrowsAsync<InvalidOperationException>(
            async () => await CreateProcessor(
                request.AdapterId,
                throwingAdapter).InvokeAsync(request));
        Xunit.Assert.Equal(1, throwingAdapter.CallCount);
    }

    private static (
        global::AI.Sandbox.Engine.Core.Modeling.ModelInvocationStatus Status,
        global::AI.Sandbox.Engine.Core.Modeling
            .ModelInvocationResponseEnvelope<ModelOutput> Response) Case(
                global::AI.Sandbox.Engine.Core.Modeling.ModelInvocationStatus
                    status,
                global::AI.Sandbox.Engine.Core.Modeling
                    .ModelInvocationResponseEnvelope<ModelOutput> response) =>
        (status, response);

    private static global::AI.Sandbox.Engine.Core.Modeling
        .ModelInvocationProcessor<ModelInput, ModelOutput> CreateProcessor(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Modeling.ModelAdapterIdKind>
                    adapterId,
            global::AI.Sandbox.Engine.Core.Modeling
                .IModelAdapter<ModelInput, ModelOutput> adapter) =>
        global::AI.Sandbox.Engine.Core.Modeling
            .ModelInvocationProcessor<ModelInput, ModelOutput>.Create(
                adapterId,
                adapter);

    private static global::AI.Sandbox.Engine.Core.Modeling
        .ModelInvocationRequestEnvelope<ModelInput> CreateRequest(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Modeling.ModelInvocationIdKind>?
                    invocationId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Modeling.ModelAdapterIdKind>?
                    adapterId = null,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Modeling.ModelProfileIdKind>?
                    profileId = null,
            int outputLimit = 20) =>
        CreateRequest(
            invocationId ?? InvocationId(1),
            adapterId ?? AdapterId(1),
            profileId ?? ProfileId(1),
            PromptDocumentId(1),
            OwnerId(),
            WorldId(),
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateVersion.Initial,
            0,
            outputLimit,
            "request");

    private static global::AI.Sandbox.Engine.Core.Modeling
        .ModelInvocationRequestEnvelope<ModelInput> CreateRequest(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Modeling.ModelInvocationIdKind>
                    invocationId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Modeling.ModelAdapterIdKind>
                    adapterId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Modeling.ModelProfileIdKind>
                    profileId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Prompting.PromptDocumentIdKind>
                    promptDocumentId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
                    ownerEntityId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
            global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion version,
            ulong simulationTick,
            int outputLimit,
            string text) =>
        global::AI.Sandbox.Engine.Core.Modeling
            .ModelInvocationRequestEnvelope<ModelInput>.Create(
                invocationId,
                adapterId,
                profileId,
                promptDocumentId,
                ownerEntityId,
                worldId,
                version,
                simulationTick,
                OutputLimit(outputLimit),
                new ModelInput(text));

    private static global::AI.Sandbox.Engine.Core.Modeling
        .ModelInvocationResponseEnvelope<ModelOutput> CreateMatchingResponse(
            global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationRequestEnvelope<ModelInput> request,
            int outputUnits,
            string text) =>
        CreateResponse(
            request.InvocationId,
            request.AdapterId,
            request.ModelProfileId,
            request.PromptDocumentId,
            request.OwnerEntityId,
            request.WorldId,
            request.WorldStateVersion,
            request.SimulationTick,
            5,
            outputUnits,
            text);

    private static global::AI.Sandbox.Engine.Core.Modeling
        .ModelInvocationResponseEnvelope<ModelOutput> CreateResponse(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Modeling.ModelInvocationIdKind>
                    invocationId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Modeling.ModelAdapterIdKind>
                    adapterId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Modeling.ModelProfileIdKind>
                    profileId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Prompting.PromptDocumentIdKind>
                    promptDocumentId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
                    ownerEntityId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
            global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion version,
            ulong simulationTick,
            int inputUnits,
            int outputUnits,
            string text) =>
        global::AI.Sandbox.Engine.Core.Modeling
            .ModelInvocationResponseEnvelope<ModelOutput>.Create(
                invocationId,
                adapterId,
                profileId,
                promptDocumentId,
                ownerEntityId,
                worldId,
                version,
                simulationTick,
                Usage(inputUnits, outputUnits),
                new ModelOutput(text));

    private sealed class FixedAdapter :
        global::AI.Sandbox.Engine.Core.Modeling
            .IModelAdapter<ModelInput, ModelOutput>
    {
        private readonly global::AI.Sandbox.Engine.Core.Modeling
            .ModelInvocationDecision<ModelOutput> decision;

        public FixedAdapter(
            global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationDecision<ModelOutput> decision)
        {
            this.decision = decision;
        }

        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Modeling
            .ModelInvocationContext<ModelInput>? LastContext { get; private set; }

        public ValueTask<global::AI.Sandbox.Engine.Core.Modeling
            .ModelInvocationDecision<ModelOutput>> InvokeAsync(
                global::AI.Sandbox.Engine.Core.Modeling
                    .ModelInvocationContext<ModelInput> context,
                CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount = checked(CallCount + 1);
            LastContext = context;
            return ValueTask.FromResult(decision);
        }
    }

    private sealed class ThrowingAdapter :
        global::AI.Sandbox.Engine.Core.Modeling
            .IModelAdapter<ModelInput, ModelOutput>
    {
        public int CallCount { get; private set; }

        public ValueTask<global::AI.Sandbox.Engine.Core.Modeling
            .ModelInvocationDecision<ModelOutput>> InvokeAsync(
                global::AI.Sandbox.Engine.Core.Modeling
                    .ModelInvocationContext<ModelInput> context,
                CancellationToken cancellationToken)
        {
            _ = context;
            _ = cancellationToken;
            CallCount = checked(CallCount + 1);
            throw new InvalidOperationException("adapter failure");
        }
    }

    private static global::AI.Sandbox.Engine.Core.Modeling.ModelOutputLimit
        OutputLimit(int units) =>
        global::AI.Sandbox.Engine.Core.Modeling.ModelOutputLimit
            .FromUnits(units);

    private static global::AI.Sandbox.Engine.Core.Modeling.ModelUsage Usage(
        int inputUnits,
        int outputUnits) =>
        global::AI.Sandbox.Engine.Core.Modeling.ModelUsage.Create(
            inputUnits,
            outputUnits);

    private static global::AI.Sandbox.Engine.Core.Modeling.ModelRejectionCode
        RejectionCode(string value) =>
        global::AI.Sandbox.Engine.Core.Modeling.ModelRejectionCode.Parse(value);

    private static global::AI.Sandbox.Engine.Core.Modeling.ModelFailureCode
        FailureCode(string value) =>
        global::AI.Sandbox.Engine.Core.Modeling.ModelFailureCode.Parse(value);

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
        global::AI.Sandbox.Engine.Core.Modeling.ModelInvocationIdKind>
            InvocationId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Modeling
                .ModelInvocationIdKind>.Parse(
                    $"019b0000-0000-7d00-8d00-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Modeling.ModelAdapterIdKind>
            AdapterId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Modeling.ModelAdapterIdKind>.Parse(
                $"019b0000-0000-7e00-8e00-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Modeling.ModelProfileIdKind>
            ProfileId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Modeling.ModelProfileIdKind>.Parse(
                $"019b0000-0000-7f00-8f00-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Prompting.PromptDocumentIdKind>
            PromptDocumentId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptDocumentIdKind>.Parse(
                    $"019b0000-0000-7b00-8b00-{suffix:D12}");
}
