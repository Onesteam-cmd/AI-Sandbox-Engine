namespace AI.Sandbox.Engine.Core.Tests;

public sealed class EventSystemTests
{
    private readonly record struct PositionChanged(int X, int Y) :
        global::AI.Sandbox.Engine.Core.Events.IEngineEvent;

    private sealed record MessageObserved(string Text) :
        global::AI.Sandbox.Engine.Core.Events.IEngineEvent;

    private sealed record OtherEvent :
        global::AI.Sandbox.Engine.Core.Events.IEngineEvent;

    [Xunit.Fact]
    public void Envelope_PreservesAuthoritativeMetadataAndPayload()
    {
        var eventId = CreateEventId(1);
        var payload = new PositionChanged(10, 20);

        var envelope =
            global::AI.Sandbox.Engine.Core.Events.EventEnvelope<PositionChanged>.Create(
                eventId,
                42,
                900,
                payload);

        Xunit.Assert.True(envelope.IsValid);
        Xunit.Assert.Equal(eventId, envelope.EventId);
        Xunit.Assert.Equal(42UL, envelope.Sequence);
        Xunit.Assert.Equal(900UL, envelope.SimulationTick);
        Xunit.Assert.Equal(payload, envelope.Payload);
    }

    [Xunit.Fact]
    public void Envelope_RejectsEmptyEventIdentifier()
    {
        var exception = Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Events.EventEnvelope<PositionChanged>.Create(
                default,
                0,
                0,
                new PositionChanged(1, 2)));

        Xunit.Assert.Equal("eventId", exception.ParamName);
    }

    [Xunit.Fact]
    public void Envelope_RejectsNullReferencePayload()
    {
        var exception = Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.Events.EventEnvelope<MessageObserved>.Create(
                CreateEventId(2),
                0,
                0,
                null!));

        Xunit.Assert.Equal("payload", exception.ParamName);
    }

    [Xunit.Fact]
    public void DefaultEnvelope_IsInvalid()
    {
        global::AI.Sandbox.Engine.Core.Events.EventEnvelope<PositionChanged>
            envelope = default;

        Xunit.Assert.False(envelope.IsValid);
    }

    [Xunit.Fact]
    public async Task Dispatcher_WithNoHandlers_CompletesWithoutSideEffects()
    {
        var dispatcher =
            new global::AI.Sandbox.Engine.Core.Events.EventDispatcherBuilder()
                .Build();
        var envelope = CreateEnvelope(new PositionChanged(1, 1), 3);

        await dispatcher.DispatchAsync(envelope);
    }

    [Xunit.Fact]
    public async Task Dispatcher_InvokesHandlersSequentiallyInRegistrationOrder()
    {
        var calls = new List<string>();
        var first = new RecordingHandler<PositionChanged>("first", calls);
        var second = new RecordingHandler<PositionChanged>("second", calls);
        var dispatcher =
            new global::AI.Sandbox.Engine.Core.Events.EventDispatcherBuilder()
                .Add(first)
                .Add(second)
                .Build();

        await dispatcher.DispatchAsync(
            CreateEnvelope(new PositionChanged(2, 3), 4));

        Xunit.Assert.Equal(new[] { "first", "second" }, calls);
    }

    [Xunit.Fact]
    public async Task Dispatcher_UsesExactPayloadTypeRegistrations()
    {
        var calls = new List<string>();
        var unrelated = new RecordingHandler<OtherEvent>("other", calls);
        var dispatcher =
            new global::AI.Sandbox.Engine.Core.Events.EventDispatcherBuilder()
                .Add(unrelated)
                .Build();

        await dispatcher.DispatchAsync(
            CreateEnvelope(new PositionChanged(4, 5), 5));

        Xunit.Assert.Empty(calls);
    }

    [Xunit.Fact]
    public async Task Dispatcher_SupportsReferenceTypeEvents()
    {
        var calls = new List<string>();
        var handler = new RecordingHandler<MessageObserved>("message", calls);
        var dispatcher =
            new global::AI.Sandbox.Engine.Core.Events.EventDispatcherBuilder()
                .Add(handler)
                .Build();
        var envelope =
            global::AI.Sandbox.Engine.Core.Events.EventEnvelope<MessageObserved>.Create(
                CreateEventId(6),
                6,
                60,
                new MessageObserved("heard"));

        await dispatcher.DispatchAsync(envelope);

        Xunit.Assert.Equal(new[] { "message" }, calls);
    }

    [Xunit.Fact]
    public async Task Dispatcher_RejectsDefaultEnvelope()
    {
        var dispatcher =
            new global::AI.Sandbox.Engine.Core.Events.EventDispatcherBuilder()
                .Build();

        var exception = await Xunit.Assert.ThrowsAsync<ArgumentException>(
            async () => await dispatcher.DispatchAsync(
                default(
                    global::AI.Sandbox.Engine.Core.Events.EventEnvelope<
                        PositionChanged>)));

        Xunit.Assert.Equal("envelope", exception.ParamName);
    }

    [Xunit.Fact]
    public async Task Dispatcher_ObservesCancellationBeforeFirstHandler()
    {
        var calls = new List<string>();
        var handler = new RecordingHandler<PositionChanged>("called", calls);
        var dispatcher =
            new global::AI.Sandbox.Engine.Core.Events.EventDispatcherBuilder()
                .Add(handler)
                .Build();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Xunit.Assert.ThrowsAsync<OperationCanceledException>(
            async () => await dispatcher.DispatchAsync(
                CreateEnvelope(new PositionChanged(6, 7), 7),
                cancellation.Token));

        Xunit.Assert.Empty(calls);
    }

    [Xunit.Fact]
    public async Task Dispatcher_StopsAfterHandlerFailure()
    {
        var calls = new List<string>();
        var first = new ThrowingHandler<PositionChanged>("first", calls);
        var second = new RecordingHandler<PositionChanged>("second", calls);
        var dispatcher =
            new global::AI.Sandbox.Engine.Core.Events.EventDispatcherBuilder()
                .Add(first)
                .Add(second)
                .Build();

        var exception = await Xunit.Assert.ThrowsAsync<InvalidOperationException>(
            async () => await dispatcher.DispatchAsync(
                CreateEnvelope(new PositionChanged(8, 9), 8)));

        Xunit.Assert.Equal("handler failure", exception.Message);
        Xunit.Assert.Equal(new[] { "first" }, calls);
    }

    [Xunit.Fact]
    public void Builder_CannotRegisterAfterBuild()
    {
        var builder =
            new global::AI.Sandbox.Engine.Core.Events.EventDispatcherBuilder();
        _ = builder.Build();

        Xunit.Assert.Throws<InvalidOperationException>(
            () => builder.Add(
                new RecordingHandler<PositionChanged>(
                    "late",
                    new List<string>())));
    }

    [Xunit.Fact]
    public void Builder_CannotBuildTwice()
    {
        var builder =
            new global::AI.Sandbox.Engine.Core.Events.EventDispatcherBuilder();
        _ = builder.Build();

        Xunit.Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Xunit.Fact]
    public void Builder_RejectsNullHandler()
    {
        var builder =
            new global::AI.Sandbox.Engine.Core.Events.EventDispatcherBuilder();

        Xunit.Assert.Throws<ArgumentNullException>(
            () => builder.Add<PositionChanged>(null!));
    }

    [Xunit.Fact]
    public void EventIdentifierKind_IsDistinctFromArbitraryKinds()
    {
        var eventIdType = typeof(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Events.EventIdKind>);
        var otherIdType = typeof(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<OtherIdKind>);

        Xunit.Assert.NotEqual(eventIdType, otherIdType);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Events.EventIdKind> CreateEventId(
            int suffix)
    {
        var text = $"019b0000-0000-7000-8000-{suffix:D12}";
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Events.EventIdKind>.Parse(text);
    }

    private static global::AI.Sandbox.Engine.Core.Events.EventEnvelope<TEvent>
        CreateEnvelope<TEvent>(TEvent payload, int suffix)
        where TEvent : global::AI.Sandbox.Engine.Core.Events.IEngineEvent
    {
        return global::AI.Sandbox.Engine.Core.Events.EventEnvelope<TEvent>.Create(
            CreateEventId(suffix),
            (ulong)suffix,
            (ulong)(suffix * 10),
            payload);
    }

    private readonly record struct OtherIdKind
    {
    }

    private sealed class RecordingHandler<TEvent> :
        global::AI.Sandbox.Engine.Core.Events.IEventHandler<TEvent>
        where TEvent : global::AI.Sandbox.Engine.Core.Events.IEngineEvent
    {
        private readonly string name;
        private readonly List<string> calls;

        public RecordingHandler(string name, List<string> calls)
        {
            this.name = name;
            this.calls = calls;
        }

        public ValueTask HandleAsync(
            global::AI.Sandbox.Engine.Core.Events.EventEnvelope<TEvent> envelope,
            CancellationToken cancellationToken)
        {
            _ = envelope;
            cancellationToken.ThrowIfCancellationRequested();
            calls.Add(name);
            return ValueTask.CompletedTask;
        }
    }

    private sealed class ThrowingHandler<TEvent> :
        global::AI.Sandbox.Engine.Core.Events.IEventHandler<TEvent>
        where TEvent : global::AI.Sandbox.Engine.Core.Events.IEngineEvent
    {
        private readonly string name;
        private readonly List<string> calls;

        public ThrowingHandler(string name, List<string> calls)
        {
            this.name = name;
            this.calls = calls;
        }

        public ValueTask HandleAsync(
            global::AI.Sandbox.Engine.Core.Events.EventEnvelope<TEvent> envelope,
            CancellationToken cancellationToken)
        {
            _ = envelope;
            cancellationToken.ThrowIfCancellationRequested();
            calls.Add(name);
            throw new InvalidOperationException("handler failure");
        }
    }
}
