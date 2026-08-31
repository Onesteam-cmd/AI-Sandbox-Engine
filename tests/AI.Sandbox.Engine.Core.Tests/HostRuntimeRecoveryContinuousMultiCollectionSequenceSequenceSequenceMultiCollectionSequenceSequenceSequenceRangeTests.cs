namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceMultiCollectionSequenceSequenceSequenceRangeTests
{
    [Xunit.Fact]
    public void IdsAndPublicContractNamesAreExact()
    {
        Xunit.Assert.True(typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidationIdKind).IsPublic);
        Xunit.Assert.True(typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryIdKind).IsPublic);

        AssertGenericPublic(
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidation<,,>));
        AssertGenericPublic(
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQuery<,,>));
        AssertGenericPublic(
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidationResult<,,>));
        AssertGenericPublic(
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryResult<,,>));
    }

    [Xunit.Fact]
    public void FlowExposesSequenceValidationAndBoundedRangeQueryMethods()
    {
        var flow = typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceFlow);
        var methods = flow.GetMethods(
            global::System.Reflection.BindingFlags.Public |
            global::System.Reflection.BindingFlags.Static |
            global::System.Reflection.BindingFlags.DeclaredOnly);

        var validate = Xunit.Assert.Single(
            methods,
            method => method.Name == "ValidateSequence");
        var query = Xunit.Assert.Single(
            methods,
            method => method.Name == "QueryRange");

        Xunit.Assert.True(validate.IsGenericMethodDefinition);
        Xunit.Assert.True(query.IsGenericMethodDefinition);
        Xunit.Assert.Equal(3, validate.GetGenericArguments().Length);
        Xunit.Assert.Equal(3, query.GetGenericArguments().Length);
        Xunit.Assert.Equal(4, validate.GetParameters().Length);
        Xunit.Assert.Equal(6, query.GetParameters().Length);
    }

    [Xunit.Fact]
    public void StatusDefinesExplicitValidationAndQueryOutcomes()
    {
        var names = global::System.Enum.GetNames<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceStatus>();

        Xunit.Assert.Contains(
            "ContinuousMultiCollectionSequenceSequenceSequenceValidated",
            names);
        Xunit.Assert.Contains(
            "MultiCollectionSequenceSequenceSequenceCheckpointRangeQueried",
            names);
        Xunit.Assert.Contains(
            "StaleMultiCollectionSequenceSequenceSummaryRevision",
            names);
        Xunit.Assert.Contains(
            "RangeDoesNotCrossMultiCollectionSequenceSequenceBoundary",
            names);
        Xunit.Assert.Contains("RangeTooLarge", names);
        Xunit.Assert.Equal(
            names.Length,
            global::System.Linq.Enumerable.Count(
                global::System.Linq.Enumerable.Distinct(names)));
    }

    [Xunit.Fact]
    public void SequenceValidationIsImmutableAndRevisioned()
    {
        var type = typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidation<,,>);

        AssertImmutable(type);
        AssertReadableProperty(type, "ValidationId");
        AssertReadableProperty(type, "MultiCollectionSequenceSequenceSummaries");
        AssertReadableProperty(type, "BoundarySupersessions");
        AssertReadableProperty(type, "SourceSequence");
        AssertReadableProperty(type, "SourceCollection");
        AssertReadableProperty(type, "StartSequenceSummaryIndex");
        AssertReadableProperty(type, "EndSequenceSummaryIndex");
        AssertReadableProperty(type, "MultiCollectionSequenceCount");
        AssertReadableProperty(type, "ValidatedTick");
        AssertReadableProperty(type, "Revision");
    }

    [Xunit.Fact]
    public void RangeQueryIsImmutableAndCrossesMultiCollectionSequenceSequenceBoundaries()
    {
        var type = typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQuery<,,>);

        AssertImmutable(type);
        AssertReadableProperty(type, "QueryId");
        AssertReadableProperty(type, "Sequence");
        AssertReadableProperty(type, "Checkpoints");
        AssertReadableProperty(type, "Supersessions");
        AssertReadableProperty(type, "CrossedBoundarySupersessions");
        AssertReadableProperty(type, "StartSummaryIndex");
        AssertReadableProperty(type, "EndSummaryIndex");
        AssertReadableProperty(type, "CrossedBoundaryCount");
        AssertReadableProperty(type, "QueriedTick");
        AssertReadableProperty(type, "Revision");
    }

    [Xunit.Fact]
    public void ResultContractsExposeExplicitSuccess()
    {
        var validationResult = typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidationResult<,,>);
        var queryResult = typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryResult<,,>);

        AssertImmutable(validationResult);
        AssertImmutable(queryResult);
        AssertReadableProperty(validationResult, "Status");
        AssertReadableProperty(validationResult, "MultiCollectionSequenceSequenceSummaries");
        AssertReadableProperty(validationResult, "Validation");
        AssertReadableProperty(validationResult, "Succeeded");
        AssertReadableProperty(queryResult, "Status");
        AssertReadableProperty(queryResult, "Sequence");
        AssertReadableProperty(queryResult, "Query");
        AssertReadableProperty(queryResult, "Succeeded");
    }

    [Xunit.Fact]
    public void ConstructorsRemainNonPublic()
    {
        foreach (var type in new[]
        {
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidation<,,>),
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQuery<,,>),
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidationResult<,,>),
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryResult<,,>),
        })
        {
            var publicConstructors = type.GetConstructors(
                global::System.Reflection.BindingFlags.Public |
                global::System.Reflection.BindingFlags.Instance);
            Xunit.Assert.Empty(publicConstructors);
        }
    }

    [Xunit.Fact]
    public void SequenceAndCheckpointBoundsRemainExplicit()
    {
        Xunit.Assert.Equal(
            8,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceFlow
                .MaximumSummaryCount);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryFlow
                .MaximumCheckpointCount,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceFlow
                .MaximumCheckpointCount);
        Xunit.Assert.InRange(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceFlow
                .MaximumCheckpointCount,
            1,
            1024);
    }

    [Xunit.Fact]
    public void ContractsRemainSynchronousAndSideEffectFree()
    {
        var flow = typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceFlow);
        var methods = flow.GetMethods(
            global::System.Reflection.BindingFlags.Public |
            global::System.Reflection.BindingFlags.Static |
            global::System.Reflection.BindingFlags.DeclaredOnly);

        Xunit.Assert.All(
            methods,
            method =>
            {
                Xunit.Assert.False(
                    typeof(global::System.Threading.Tasks.Task)
                        .IsAssignableFrom(method.ReturnType));
                Xunit.Assert.Null(
                    global::System.Linq.Enumerable.SingleOrDefault(
                        method.GetCustomAttributes(
                            typeof(global::System.Runtime.CompilerServices
                                .AsyncStateMachineAttribute),
                            inherit: false)));
            });

        AssertImmutable(typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidation<,,>));
        AssertImmutable(typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQuery<,,>));
    }

    private static void AssertGenericPublic(global::System.Type type)
    {
        Xunit.Assert.True(type.IsPublic);
        Xunit.Assert.True(type.IsGenericTypeDefinition);
        Xunit.Assert.Equal(3, type.GetGenericArguments().Length);
    }

    private static void AssertImmutable(global::System.Type type)
    {
        Xunit.Assert.All(
            type.GetProperties(
                global::System.Reflection.BindingFlags.Public |
                global::System.Reflection.BindingFlags.Instance),
            property => Xunit.Assert.Null(property.SetMethod));
    }

    private static void AssertReadableProperty(
        global::System.Type type,
        string propertyName)
    {
        var property = type.GetProperty(
            propertyName,
            global::System.Reflection.BindingFlags.Public |
            global::System.Reflection.BindingFlags.Instance);
        Xunit.Assert.NotNull(property);
        Xunit.Assert.NotNull(property.GetMethod);
        Xunit.Assert.Null(property.SetMethod);
    }
}
