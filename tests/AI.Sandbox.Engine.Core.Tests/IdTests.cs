namespace AI.Sandbox.Engine.Core.Tests;

public sealed class IdTests
{
    private readonly struct EntityKind
    {
    }

    private readonly struct WorldKind
    {
    }

    [Xunit.Fact]
    public void From_PreservesNonEmptyGuid()
    {
        var value = Guid.Parse("019b0000-0000-7000-8000-000000000001");

        var identifier =
            global::AI.Sandbox.Engine.Core.Identifiers.Id<EntityKind>.From(value);

        Xunit.Assert.Equal(value, identifier.Value);
        Xunit.Assert.False(identifier.IsEmpty);
    }

    [Xunit.Fact]
    public void From_RejectsEmptyGuid()
    {
        var exception = Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.Identifiers.Id<EntityKind>.From(
                Guid.Empty));

        Xunit.Assert.Equal("value", exception.ParamName);
    }

    [Xunit.Fact]
    public void TryFrom_RejectsEmptyGuidWithoutThrowing()
    {
        var succeeded =
            global::AI.Sandbox.Engine.Core.Identifiers.Id<EntityKind>.TryFrom(
                Guid.Empty,
                out var identifier);

        Xunit.Assert.False(succeeded);
        Xunit.Assert.True(identifier.IsEmpty);
    }

    [Xunit.Fact]
    public void DefaultValue_IsExplicitlyEmpty()
    {
        global::AI.Sandbox.Engine.Core.Identifiers.Id<EntityKind> identifier =
            default;

        Xunit.Assert.True(identifier.IsEmpty);
        Xunit.Assert.Equal(Guid.Empty, identifier.Value);
    }

    [Xunit.Fact]
    public void DifferentKinds_ProduceDifferentClosedTypes()
    {
        var entityType =
            typeof(global::AI.Sandbox.Engine.Core.Identifiers.Id<EntityKind>);
        var worldType =
            typeof(global::AI.Sandbox.Engine.Core.Identifiers.Id<WorldKind>);

        Xunit.Assert.NotEqual(entityType, worldType);
    }

    [Xunit.Fact]
    public void Equality_UsesTheUnderlyingGuidWithinOneKind()
    {
        var value = Guid.Parse("019b0000-0000-7000-8000-000000000002");
        var first =
            global::AI.Sandbox.Engine.Core.Identifiers.Id<EntityKind>.From(value);
        var second =
            global::AI.Sandbox.Engine.Core.Identifiers.Id<EntityKind>.From(value);

        Xunit.Assert.Equal(first, second);
        Xunit.Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Xunit.Fact]
    public void ToString_UsesCanonicalGuidDFormat()
    {
        var value = Guid.Parse("019b0000-0000-7000-8000-000000000003");
        var identifier =
            global::AI.Sandbox.Engine.Core.Identifiers.Id<EntityKind>.From(value);

        Xunit.Assert.Equal(
            "019b0000-0000-7000-8000-000000000003",
            identifier.ToString());
    }

    [Xunit.Fact]
    public void TryParse_RoundTripsCanonicalText()
    {
        const string text = "019b0000-0000-7000-8000-000000000004";

        var succeeded =
            global::AI.Sandbox.Engine.Core.Identifiers.Id<EntityKind>.TryParse(
                text,
                out var identifier);

        Xunit.Assert.True(succeeded);
        Xunit.Assert.Equal(text, identifier.ToString());
    }

    [Xunit.Fact]
    public void TryParse_RejectsNonCanonicalGuidFormats()
    {
        const string compactText = "019b0000000070008000000000000005";

        var succeeded =
            global::AI.Sandbox.Engine.Core.Identifiers.Id<EntityKind>.TryParse(
                compactText,
                out var identifier);

        Xunit.Assert.False(succeeded);
        Xunit.Assert.True(identifier.IsEmpty);
    }

    [Xunit.Theory]
    [Xunit.InlineData("")]
    [Xunit.InlineData("not-an-identifier")]
    [Xunit.InlineData("00000000-0000-0000-0000-000000000000")]
    public void Parse_RejectsInvalidOrEmptyText(string text)
    {
        Xunit.Assert.Throws<FormatException>(
            () => global::AI.Sandbox.Engine.Core.Identifiers.Id<EntityKind>.Parse(
                text));
    }

    [Xunit.Fact]
    public void Parse_RejectsNullText()
    {
        Xunit.Assert.Throws<ArgumentNullException>(
            () => global::AI.Sandbox.Engine.Core.Identifiers.Id<EntityKind>.Parse(
                null!));
    }

    [Xunit.Fact]
    public void CompareTo_UsesStableGuidOrdering()
    {
        var lower =
            global::AI.Sandbox.Engine.Core.Identifiers.Id<EntityKind>.From(
                Guid.Parse("00000000-0000-0000-0000-000000000001"));
        var higher =
            global::AI.Sandbox.Engine.Core.Identifiers.Id<EntityKind>.From(
                Guid.Parse("00000000-0000-0000-0000-000000000002"));

        Xunit.Assert.True(lower.CompareTo(higher) < 0);
        Xunit.Assert.True(higher.CompareTo(lower) > 0);
        Xunit.Assert.Equal(0, lower.CompareTo(lower));
    }

    [Xunit.Fact]
    public void IdentifierType_HasNoHiddenRandomGenerator()
    {
        var identifierType =
            typeof(global::AI.Sandbox.Engine.Core.Identifiers.Id<EntityKind>);

        Xunit.Assert.Null(identifierType.GetMethod("New"));
        Xunit.Assert.Null(identifierType.GetMethod("Create"));
    }
}
