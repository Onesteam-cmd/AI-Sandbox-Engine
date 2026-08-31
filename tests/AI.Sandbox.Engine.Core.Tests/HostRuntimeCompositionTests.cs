namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeCompositionTests
{
    private readonly record struct ValueCapability(string Name) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCapability;

    private sealed record SealedCapability(string Name) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCapability;

    private record OpenCapability(string Name) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCapability;

    private sealed class CountingCapability :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeCapability
    {
        public int StartCount { get; private set; }

        public void Start() => StartCount++;
    }

    [Xunit.Fact]
    public void IdsDependenciesAndExactPayloadsAreValidated()
    {
        Xunit.Assert.Throws<ArgumentException>(
            () => Descriptor(default, new ValueCapability("invalid")));
        Xunit.Assert.Throws<ArgumentException>(
            () => Descriptor(
                CapabilityId(1),
                new OpenCapability("invalid")));
        Xunit.Assert.Throws<ArgumentException>(
            () => Descriptor(
                CapabilityId(1),
                new ValueCapability("self"),
                CapabilityId(1)));
        Xunit.Assert.Throws<ArgumentException>(
            () => Descriptor(
                CapabilityId(1),
                new ValueCapability("duplicate"),
                CapabilityId(2),
                CapabilityId(2)));

        var descriptor = Descriptor(
            CapabilityId(3),
            new SealedCapability("ok"),
            CapabilityId(2),
            CapabilityId(1));

        Xunit.Assert.Equal(CapabilityId(1), descriptor.Dependencies[0]);
        Xunit.Assert.Equal(CapabilityId(2), descriptor.Dependencies[1]);
    }

    [Xunit.Fact]
    public void CompositionOrdersDependenciesBeforeDependentsDeterministically()
    {
        var database = Descriptor(
            CapabilityId(1),
            new ValueCapability("database"));
        var retrieval = Descriptor(
            CapabilityId(2),
            new ValueCapability("retrieval"),
            database.CapabilityId);
        var dialogue = Descriptor(
            CapabilityId(3),
            new ValueCapability("dialogue"),
            retrieval.CapabilityId);

        var result = Compose(dialogue, retrieval, database);

        Xunit.Assert.True(result.Succeeded);
        Xunit.Assert.Equal(
            new[]
            {
                database.CapabilityId,
                retrieval.CapabilityId,
                dialogue.CapabilityId,
            },
            result.Composition!.Capabilities
                .Select(static capability => capability.CapabilityId)
                .ToArray());
    }

    [Xunit.Fact]
    public void IndependentCapabilitiesUseStableIdTieBreak()
    {
        var third = Descriptor(
            CapabilityId(3),
            new ValueCapability("third"));
        var first = Descriptor(
            CapabilityId(1),
            new ValueCapability("first"));
        var second = Descriptor(
            CapabilityId(2),
            new ValueCapability("second"));

        var result = Compose(third, first, second);

        Xunit.Assert.Equal(
            new[]
            {
                first.CapabilityId,
                second.CapabilityId,
                third.CapabilityId,
            },
            result.Composition!.Capabilities
                .Select(static capability => capability.CapabilityId)
                .ToArray());
    }

    [Xunit.Fact]
    public void DuplicateCapabilityIsRejected()
    {
        var first = Descriptor(
            CapabilityId(1),
            new ValueCapability("first"));
        var duplicate = Descriptor(
            CapabilityId(1),
            new ValueCapability("duplicate"));

        var result = Compose(first, duplicate);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompositionStatus.DuplicateCapability,
            result.Status);
        Xunit.Assert.Equal(CapabilityId(1), result.RelatedCapabilityId);
        Xunit.Assert.Null(result.Composition);
    }

    [Xunit.Fact]
    public void MissingDependencyIsRejected()
    {
        var capability = Descriptor(
            CapabilityId(2),
            new ValueCapability("dependent"),
            CapabilityId(99));

        var result = Compose(capability);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompositionStatus.MissingDependency,
            result.Status);
        Xunit.Assert.Equal(capability.CapabilityId, result.RelatedCapabilityId);
    }

    [Xunit.Fact]
    public void CycleIsRejected()
    {
        var first = Descriptor(
            CapabilityId(1),
            new ValueCapability("first"),
            CapabilityId(2));
        var second = Descriptor(
            CapabilityId(2),
            new ValueCapability("second"),
            CapabilityId(1));

        var result = Compose(first, second);

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompositionStatus.CycleDetected,
            result.Status);
        Xunit.Assert.Equal(CapabilityId(1), result.RelatedCapabilityId);
    }

    [Xunit.Fact]
    public void EmptyAndOversizedCompositionsAreExplicit()
    {
        var empty = Compose();
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompositionStatus.Empty,
            empty.Status);

        var oversized = Enumerable
            .Range(1, 129)
            .Select(index => Descriptor(
                CapabilityId(index),
                new ValueCapability(index.ToString(
                    System.Globalization.CultureInfo.InvariantCulture))))
            .ToArray();

        var tooMany = Compose(oversized);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCompositionStatus.TooManyCapabilities,
            tooMany.Status);
    }

    [Xunit.Fact]
    public void CompositionIsImmutableAndDoesNotStartCapabilities()
    {
        var payload = new CountingCapability();
        var descriptor = Descriptor(CapabilityId(1), payload);
        var result = Compose(descriptor);

        Xunit.Assert.Equal(0, payload.StartCount);
        var list = Xunit.Assert.IsAssignableFrom<
            System.Collections.Generic.IList<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeCapabilityDescriptor>>(
                        result.Composition!.Capabilities);
        Xunit.Assert.Throws<NotSupportedException>(
            () => list.Add(descriptor));
        Xunit.Assert.Equal(0, payload.StartCount);
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeCapabilityDescriptor Descriptor(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeCapabilityIdKind> capabilityId,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .IHostRuntimeCapability payload,
            params global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeCapabilityIdKind>[] dependencies) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCapabilityDescriptor.Create(
                capabilityId,
                payload,
                dependencies);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeCompositionResult Compose(
            params global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCapabilityDescriptor[] capabilities) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompositionBuilder.Compose(
                CompositionId(),
                capabilities);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCapabilityIdKind> CapabilityId(int suffix) =>
        Id<global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCapabilityIdKind>(1000 + suffix);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompositionIdKind> CompositionId() =>
        Id<global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeCompositionIdKind>(2000);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind> Id<TKind>(
        int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(
            $"019d0000-0000-7000-8000-{suffix:D12}");
}
