namespace AI.Sandbox.Engine.Core.Tests;

public sealed class ProviderNeutralSpeechTests
{
    private readonly record struct RecognitionInput(string AudioReference) :
        global::AI.Sandbox.Engine.Core.Speech.ISpeechRequest;

    private readonly record struct SynthesisInput(string Text) :
        global::AI.Sandbox.Engine.Core.Speech.ISpeechRequest;

    private abstract record AbstractRequest :
        global::AI.Sandbox.Engine.Core.Speech.ISpeechRequest;

    private sealed record ConcreteAbstractRequest(string Value) :
        AbstractRequest;

    private record OpenRequest(string Value) :
        global::AI.Sandbox.Engine.Core.Speech.ISpeechRequest;

    private readonly record struct TranscriptOutput(string Text) :
        global::AI.Sandbox.Engine.Core.Speech.ISpeechResponse;

    private readonly record struct AudioOutput(string AudioReference) :
        global::AI.Sandbox.Engine.Core.Speech.ISpeechResponse;

    private abstract record AbstractResponse :
        global::AI.Sandbox.Engine.Core.Speech.ISpeechResponse;

    private sealed record ConcreteAbstractResponse(string Value) :
        AbstractResponse;

    private record OpenResponse(string Value) :
        global::AI.Sandbox.Engine.Core.Speech.ISpeechResponse;

    [Xunit.Fact]
    public void PayloadTypesOperationsCodesAndUnitsMustBeExactAndBounded()
    {
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Speech
                .SpeechInvocationRequestEnvelope<AbstractRequest>.Create(
                    global::AI.Sandbox.Engine.Core.Speech
                        .SpeechOperationKind.Recognition,
                    InvocationId(1),
                    AdapterId(1),
                    ProfileId(1),
                    OwnerId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    InputLimit(10),
                    OutputLimit(10),
                    new ConcreteAbstractRequest("input")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Speech
                .SpeechInvocationRequestEnvelope<OpenRequest>.Create(
                    global::AI.Sandbox.Engine.Core.Speech
                        .SpeechOperationKind.Synthesis,
                    InvocationId(2),
                    AdapterId(1),
                    ProfileId(1),
                    OwnerId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    InputLimit(10),
                    OutputLimit(10),
                    new OpenRequest("input")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Speech
                .SpeechInvocationResponseEnvelope<AbstractResponse>.Create(
                    global::AI.Sandbox.Engine.Core.Speech
                        .SpeechOperationKind.Recognition,
                    InvocationId(3),
                    AdapterId(1),
                    ProfileId(1),
                    OwnerId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    Usage(1, 1),
                    new ConcreteAbstractResponse("output")));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Speech
                .SpeechInvocationResponseEnvelope<OpenResponse>.Create(
                    global::AI.Sandbox.Engine.Core.Speech
                        .SpeechOperationKind.Synthesis,
                    InvocationId(4),
                    AdapterId(1),
                    ProfileId(1),
                    OwnerId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    Usage(1, 1),
                    new OpenResponse("output")));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.Speech
                .SpeechInvocationRequestEnvelope<RecognitionInput>.Create(
                    (global::AI.Sandbox.Engine.Core.Speech
                        .SpeechOperationKind)99,
                    InvocationId(5),
                    AdapterId(1),
                    ProfileId(1),
                    OwnerId(),
                    WorldId(),
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                    0,
                    InputLimit(10),
                    OutputLimit(10),
                    new RecognitionInput("audio")));

        Xunit.Assert.False(
            global::AI.Sandbox.Engine.Core.Speech.SpeechInputLimit
                .TryFromUnits(0, out _));
        Xunit.Assert.False(
            global::AI.Sandbox.Engine.Core.Speech.SpeechOutputLimit
                .TryFromUnits(1000001, out _));
        Xunit.Assert.False(
            global::AI.Sandbox.Engine.Core.Speech.SpeechUsage
                .TryCreate(-1, 0, out _));
        Xunit.Assert.True(
            global::AI.Sandbox.Engine.Core.Speech.SpeechUsage
                .TryCreate(0, 0, out var zeroUsage));
        Xunit.Assert.True(zeroUsage.IsInitialized);
        Xunit.Assert.Throws<FormatException>(
            () => global::AI.Sandbox.Engine.Core.Speech
                .SpeechRejectionCode.Parse("Policy Denied"));
        Xunit.Assert.Throws<FormatException>(
            () => global::AI.Sandbox.Engine.Core.Speech
                .SpeechFailureCode.Parse("HTTP_500"));
    }

    [Xunit.Fact]
    public void RequestAndResponseEnvelopesPreserveSpeechCorrelation()
    {
        var version = global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateVersion.From(3);
        var request = CreateRecognitionRequest(
            invocationId: InvocationId(10),
            adapterId: AdapterId(10),
            profileId: ProfileId(10),
            ownerId: OwnerId(),
            worldId: WorldId(),
            version: version,
            tick: 8,
            inputLimit: 100,
            outputLimit: 50,
            audioReference: "audio");
        var response = CreateTranscriptResponse(
            request,
            usage: Usage(40, 12),
            text: "recognized");

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Speech
                .SpeechOperationKind.Recognition,
            request.OperationKind);
        Xunit.Assert.Equal(InvocationId(10), request.InvocationId);
        Xunit.Assert.Equal(AdapterId(10), request.AdapterId);
        Xunit.Assert.Equal(ProfileId(10), request.SpeechProfileId);
        Xunit.Assert.Equal(OwnerId(), request.OwnerEntityId);
        Xunit.Assert.Equal(WorldId(), request.WorldId);
        Xunit.Assert.Equal(version, request.WorldStateVersion);
        Xunit.Assert.Equal((ulong)8, request.SimulationTick);
        Xunit.Assert.Equal(100, request.InputLimit.Units);
        Xunit.Assert.Equal(50, request.OutputLimit.Units);
        Xunit.Assert.Equal("audio", request.Payload.AudioReference);

        Xunit.Assert.Equal(request.OperationKind, response.OperationKind);
        Xunit.Assert.Equal(request.InvocationId, response.InvocationId);
        Xunit.Assert.Equal(40, response.Usage.InputUnits);
        Xunit.Assert.Equal(12, response.Usage.OutputUnits);
        Xunit.Assert.Equal("recognized", response.Payload.Text);
    }

    [Xunit.Fact]
    public async Task ProcessorCompletesRecognitionAndSynthesisExactlyOnce()
    {
        var recognitionRequest = CreateRecognitionRequest();
        var recognitionResponse = CreateTranscriptResponse(
            recognitionRequest,
            Usage(4, 2),
            "hello");
        var recognitionAdapter =
            new FixedAdapter<RecognitionInput, TranscriptOutput>(
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationDecision<TranscriptOutput>.Complete(
                        recognitionResponse));
        var recognitionProcessor =
            global::AI.Sandbox.Engine.Core.Speech
                .SpeechInvocationProcessor<
                    RecognitionInput,
                    TranscriptOutput>.Create(
                        recognitionRequest.AdapterId,
                        recognitionAdapter);

        var recognitionResult =
            await recognitionProcessor.InvokeAsync(recognitionRequest);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Speech
                .SpeechInvocationStatus.Completed,
            recognitionResult.Status);
        Xunit.Assert.True(recognitionResult.WasCompleted);
        Xunit.Assert.Equal(1, recognitionAdapter.CallCount);
        Xunit.Assert.Same(
            recognitionRequest,
            recognitionAdapter.LastContext!.Request);

        var synthesisRequest = CreateSynthesisRequest();
        var synthesisResponse = CreateAudioResponse(
            synthesisRequest,
            Usage(3, 8),
            "audio-out");
        var synthesisAdapter =
            new FixedAdapter<SynthesisInput, AudioOutput>(
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationDecision<AudioOutput>.Complete(
                        synthesisResponse));
        var synthesisProcessor =
            global::AI.Sandbox.Engine.Core.Speech
                .SpeechInvocationProcessor<
                    SynthesisInput,
                    AudioOutput>.Create(
                        synthesisRequest.AdapterId,
                        synthesisAdapter);

        var synthesisResult =
            await synthesisProcessor.InvokeAsync(synthesisRequest);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Speech
                .SpeechInvocationStatus.Completed,
            synthesisResult.Status);
        Xunit.Assert.Equal(1, synthesisAdapter.CallCount);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Speech
                .SpeechOperationKind.Synthesis,
            synthesisResult.Request.OperationKind);
        Xunit.Assert.Equal(
            "audio-out",
            synthesisResult.Decision!.Response!.Payload.AudioReference);
    }

    [Xunit.Fact]
    public async Task RequestAdapterMismatchSkipsSpeechAdapter()
    {
        var request = CreateRecognitionRequest(adapterId: AdapterId(2));
        var adapter = new FixedAdapter<RecognitionInput, TranscriptOutput>(
            global::AI.Sandbox.Engine.Core.Speech
                .SpeechInvocationDecision<TranscriptOutput>.Reject(
                    global::AI.Sandbox.Engine.Core.Speech
                        .SpeechRejectionCode.Parse("not.used")));
        var processor = global::AI.Sandbox.Engine.Core.Speech
            .SpeechInvocationProcessor<
                RecognitionInput,
                TranscriptOutput>.Create(
                    AdapterId(1),
                    adapter);

        var result = await processor.InvokeAsync(request);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Speech
                .SpeechInvocationStatus.RequestAdapterMismatch,
            result.Status);
        Xunit.Assert.False(result.AdapterWasInvoked);
        Xunit.Assert.Null(result.Decision);
        Xunit.Assert.Equal(0, adapter.CallCount);
    }

    [Xunit.Fact]
    public async Task AdapterRejectionAndFailureRemainExplicit()
    {
        var request = CreateRecognitionRequest();
        var rejection = global::AI.Sandbox.Engine.Core.Speech
            .SpeechRejectionCode.Parse("language.unsupported");
        var rejectedAdapter =
            new FixedAdapter<RecognitionInput, TranscriptOutput>(
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationDecision<TranscriptOutput>.Reject(
                        rejection));
        var rejectedProcessor = global::AI.Sandbox.Engine.Core.Speech
            .SpeechInvocationProcessor<
                RecognitionInput,
                TranscriptOutput>.Create(
                    request.AdapterId,
                    rejectedAdapter);

        var rejected = await rejectedProcessor.InvokeAsync(request);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Speech
                .SpeechInvocationStatus.Rejected,
            rejected.Status);
        Xunit.Assert.Equal(rejection, rejected.Decision!.RejectionCode);
        Xunit.Assert.Null(rejected.Decision.FailureCode);

        var failure = global::AI.Sandbox.Engine.Core.Speech
            .SpeechFailureCode.Parse("provider.unavailable");
        var failedAdapter =
            new FixedAdapter<RecognitionInput, TranscriptOutput>(
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationDecision<TranscriptOutput>.Fail(failure));
        var failedProcessor = global::AI.Sandbox.Engine.Core.Speech
            .SpeechInvocationProcessor<
                RecognitionInput,
                TranscriptOutput>.Create(
                    request.AdapterId,
                    failedAdapter);

        var failed = await failedProcessor.InvokeAsync(request);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Speech
                .SpeechInvocationStatus.Failed,
            failed.Status);
        Xunit.Assert.Equal(failure, failed.Decision!.FailureCode);
        Xunit.Assert.Null(failed.Decision.RejectionCode);
    }

    [Xunit.Fact]
    public async Task CompletedResponseCorrelationIsValidated()
    {
        var request = CreateRecognitionRequest();

        var cases = new[]
        {
            (
                CreateTranscriptResponse(
                    request,
                    Usage(1, 1),
                    "x",
                    operationKind: global::AI.Sandbox.Engine.Core.Speech
                        .SpeechOperationKind.Synthesis),
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationStatus.ResponseOperationMismatch),
            (
                CreateTranscriptResponse(
                    request,
                    Usage(1, 1),
                    "x",
                    invocationId: InvocationId(99)),
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationStatus.ResponseInvocationMismatch),
            (
                CreateTranscriptResponse(
                    request,
                    Usage(1, 1),
                    "x",
                    adapterId: AdapterId(99)),
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationStatus.ResponseAdapterMismatch),
            (
                CreateTranscriptResponse(
                    request,
                    Usage(1, 1),
                    "x",
                    profileId: ProfileId(99)),
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationStatus.ResponseProfileMismatch),
            (
                CreateTranscriptResponse(
                    request,
                    Usage(1, 1),
                    "x",
                    ownerId: OtherOwnerId()),
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationStatus.ResponseOwnerMismatch),
            (
                CreateTranscriptResponse(
                    request,
                    Usage(1, 1),
                    "x",
                    worldId: OtherWorldId()),
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationStatus.ResponseWorldMismatch),
            (
                CreateTranscriptResponse(
                    request,
                    Usage(1, 1),
                    "x",
                    version: global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.From(2)),
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationStatus.ResponseVersionMismatch),
            (
                CreateTranscriptResponse(
                    request,
                    Usage(1, 1),
                    "x",
                    tick: 99),
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationStatus
                        .ResponseSimulationTickMismatch),
        };

        foreach (var item in cases)
        {
            var adapter =
                new FixedAdapter<RecognitionInput, TranscriptOutput>(
                    global::AI.Sandbox.Engine.Core.Speech
                        .SpeechInvocationDecision<TranscriptOutput>.Complete(
                            item.Item1));
            var processor = global::AI.Sandbox.Engine.Core.Speech
                .SpeechInvocationProcessor<
                    RecognitionInput,
                    TranscriptOutput>.Create(
                        request.AdapterId,
                        adapter);

            var result = await processor.InvokeAsync(request);

            Xunit.Assert.Equal(item.Item2, result.Status);
            Xunit.Assert.Equal(1, adapter.CallCount);
        }
    }

    [Xunit.Fact]
    public async Task InputAndOutputLimitsAreEnforcedWithoutRetry()
    {
        var request = CreateRecognitionRequest(
            inputLimit: 10,
            outputLimit: 5);
        var inputExceeded =
            CreateTranscriptResponse(request, Usage(11, 1), "input");
        var inputAdapter =
            new FixedAdapter<RecognitionInput, TranscriptOutput>(
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationDecision<TranscriptOutput>.Complete(
                        inputExceeded));
        var inputProcessor = global::AI.Sandbox.Engine.Core.Speech
            .SpeechInvocationProcessor<
                RecognitionInput,
                TranscriptOutput>.Create(
                    request.AdapterId,
                    inputAdapter);

        var inputResult = await inputProcessor.InvokeAsync(request);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Speech
                .SpeechInvocationStatus.ResponseInputLimitExceeded,
            inputResult.Status);
        Xunit.Assert.Equal(1, inputAdapter.CallCount);

        var outputExceeded =
            CreateTranscriptResponse(request, Usage(1, 6), "output");
        var outputAdapter =
            new FixedAdapter<RecognitionInput, TranscriptOutput>(
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationDecision<TranscriptOutput>.Complete(
                        outputExceeded));
        var outputProcessor = global::AI.Sandbox.Engine.Core.Speech
            .SpeechInvocationProcessor<
                RecognitionInput,
                TranscriptOutput>.Create(
                    request.AdapterId,
                    outputAdapter);

        var outputResult = await outputProcessor.InvokeAsync(request);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.Speech
                .SpeechInvocationStatus.ResponseOutputLimitExceeded,
            outputResult.Status);
        Xunit.Assert.Equal(1, outputAdapter.CallCount);
    }

    [Xunit.Fact]
    public async Task CancellationAndExceptionPropagateWithoutRetry()
    {
        var request = CreateRecognitionRequest();
        var canceledAdapter =
            new FixedAdapter<RecognitionInput, TranscriptOutput>(
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationDecision<TranscriptOutput>.Reject(
                        global::AI.Sandbox.Engine.Core.Speech
                            .SpeechRejectionCode.Parse("unused")));
        var canceledProcessor = global::AI.Sandbox.Engine.Core.Speech
            .SpeechInvocationProcessor<
                RecognitionInput,
                TranscriptOutput>.Create(
                    request.AdapterId,
                    canceledAdapter);
        using var source = new CancellationTokenSource();
        source.Cancel();

        await Xunit.Assert.ThrowsAsync<OperationCanceledException>(
            () => canceledProcessor
                .InvokeAsync(request, source.Token)
                .AsTask());
        Xunit.Assert.Equal(0, canceledAdapter.CallCount);

        var throwingAdapter =
            new ThrowingAdapter<RecognitionInput, TranscriptOutput>();
        var throwingProcessor = global::AI.Sandbox.Engine.Core.Speech
            .SpeechInvocationProcessor<
                RecognitionInput,
                TranscriptOutput>.Create(
                    request.AdapterId,
                    throwingAdapter);

        await Xunit.Assert.ThrowsAsync<InvalidOperationException>(
            () => throwingProcessor.InvokeAsync(request).AsTask());
        Xunit.Assert.Equal(1, throwingAdapter.CallCount);
    }

    private static global::AI.Sandbox.Engine.Core.Speech
        .SpeechInvocationRequestEnvelope<RecognitionInput>
            CreateRecognitionRequest(
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.Speech
                        .SpeechInvocationIdKind>? invocationId = null,
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.Speech
                        .SpeechAdapterIdKind>? adapterId = null,
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.Speech
                        .SpeechProfileIdKind>? profileId = null,
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>?
                        ownerId = null,
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>?
                        worldId = null,
                global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion?
                    version = null,
                ulong tick = 1,
                int inputLimit = 100,
                int outputLimit = 100,
                string audioReference = "audio") =>
        global::AI.Sandbox.Engine.Core.Speech
            .SpeechInvocationRequestEnvelope<RecognitionInput>.Create(
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechOperationKind.Recognition,
                invocationId ?? InvocationId(1),
                adapterId ?? AdapterId(1),
                profileId ?? ProfileId(1),
                ownerId ?? OwnerId(),
                worldId ?? WorldId(),
                version ??
                    global::AI.Sandbox.Engine.Core.WorldState
                        .WorldStateVersion.Initial,
                tick,
                InputLimit(inputLimit),
                OutputLimit(outputLimit),
                new RecognitionInput(audioReference));

    private static global::AI.Sandbox.Engine.Core.Speech
        .SpeechInvocationRequestEnvelope<SynthesisInput>
            CreateSynthesisRequest() =>
        global::AI.Sandbox.Engine.Core.Speech
            .SpeechInvocationRequestEnvelope<SynthesisInput>.Create(
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechOperationKind.Synthesis,
                InvocationId(2),
                AdapterId(2),
                ProfileId(2),
                OwnerId(),
                WorldId(),
                global::AI.Sandbox.Engine.Core.WorldState
                    .WorldStateVersion.Initial,
                2,
                InputLimit(100),
                OutputLimit(100),
                new SynthesisInput("speak"));

    private static global::AI.Sandbox.Engine.Core.Speech
        .SpeechInvocationResponseEnvelope<TranscriptOutput>
            CreateTranscriptResponse(
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationRequestEnvelope<RecognitionInput> request,
                global::AI.Sandbox.Engine.Core.Speech.SpeechUsage usage,
                string text,
                global::AI.Sandbox.Engine.Core.Speech.SpeechOperationKind?
                    operationKind = null,
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.Speech
                        .SpeechInvocationIdKind>? invocationId = null,
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.Speech
                        .SpeechAdapterIdKind>? adapterId = null,
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.Speech
                        .SpeechProfileIdKind>? profileId = null,
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>?
                        ownerId = null,
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>?
                        worldId = null,
                global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion?
                    version = null,
                ulong? tick = null) =>
        global::AI.Sandbox.Engine.Core.Speech
            .SpeechInvocationResponseEnvelope<TranscriptOutput>.Create(
                operationKind ?? request.OperationKind,
                invocationId ?? request.InvocationId,
                adapterId ?? request.AdapterId,
                profileId ?? request.SpeechProfileId,
                ownerId ?? request.OwnerEntityId,
                worldId ?? request.WorldId,
                version ?? request.WorldStateVersion,
                tick ?? request.SimulationTick,
                usage,
                new TranscriptOutput(text));

    private static global::AI.Sandbox.Engine.Core.Speech
        .SpeechInvocationResponseEnvelope<AudioOutput>
            CreateAudioResponse(
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationRequestEnvelope<SynthesisInput> request,
                global::AI.Sandbox.Engine.Core.Speech.SpeechUsage usage,
                string audioReference) =>
        global::AI.Sandbox.Engine.Core.Speech
            .SpeechInvocationResponseEnvelope<AudioOutput>.Create(
                request.OperationKind,
                request.InvocationId,
                request.AdapterId,
                request.SpeechProfileId,
                request.OwnerEntityId,
                request.WorldId,
                request.WorldStateVersion,
                request.SimulationTick,
                usage,
                new AudioOutput(audioReference));

    private sealed class FixedAdapter<TRequest, TResponse> :
        global::AI.Sandbox.Engine.Core.Speech
            .ISpeechAdapter<TRequest, TResponse>
        where TRequest : global::AI.Sandbox.Engine.Core.Speech.ISpeechRequest
        where TResponse : global::AI.Sandbox.Engine.Core.Speech.ISpeechResponse
    {
        private readonly global::AI.Sandbox.Engine.Core.Speech
            .SpeechInvocationDecision<TResponse> decision;

        public FixedAdapter(
            global::AI.Sandbox.Engine.Core.Speech
                .SpeechInvocationDecision<TResponse> decision)
        {
            this.decision = decision;
        }

        public int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Speech
            .SpeechInvocationContext<TRequest>? LastContext { get; private set; }

        public ValueTask<global::AI.Sandbox.Engine.Core.Speech
            .SpeechInvocationDecision<TResponse>> InvokeAsync(
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationContext<TRequest> context,
                CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount = checked(CallCount + 1);
            LastContext = context;
            return ValueTask.FromResult(decision);
        }
    }

    private sealed class ThrowingAdapter<TRequest, TResponse> :
        global::AI.Sandbox.Engine.Core.Speech
            .ISpeechAdapter<TRequest, TResponse>
        where TRequest : global::AI.Sandbox.Engine.Core.Speech.ISpeechRequest
        where TResponse : global::AI.Sandbox.Engine.Core.Speech.ISpeechResponse
    {
        public int CallCount { get; private set; }

        public ValueTask<global::AI.Sandbox.Engine.Core.Speech
            .SpeechInvocationDecision<TResponse>> InvokeAsync(
                global::AI.Sandbox.Engine.Core.Speech
                    .SpeechInvocationContext<TRequest> context,
                CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount = checked(CallCount + 1);
            throw new InvalidOperationException("adapter failure");
        }
    }

    private static global::AI.Sandbox.Engine.Core.Speech.SpeechInputLimit
        InputLimit(int units) =>
        global::AI.Sandbox.Engine.Core.Speech.SpeechInputLimit
            .FromUnits(units);

    private static global::AI.Sandbox.Engine.Core.Speech.SpeechOutputLimit
        OutputLimit(int units) =>
        global::AI.Sandbox.Engine.Core.Speech.SpeechOutputLimit
            .FromUnits(units);

    private static global::AI.Sandbox.Engine.Core.Speech.SpeechUsage
        Usage(int inputUnits, int outputUnits) =>
        global::AI.Sandbox.Engine.Core.Speech.SpeechUsage
            .Create(inputUnits, outputUnits);

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
        global::AI.Sandbox.Engine.Core.Speech.SpeechInvocationIdKind>
            InvocationId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Speech
                .SpeechInvocationIdKind>.Parse(
                    $"019b0000-0000-7100-8100-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Speech.SpeechAdapterIdKind>
            AdapterId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Speech.SpeechAdapterIdKind>.Parse(
                $"019b0000-0000-7200-8200-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Speech.SpeechProfileIdKind>
            ProfileId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Speech.SpeechProfileIdKind>.Parse(
                $"019b0000-0000-7300-8300-{suffix:D12}");
}
