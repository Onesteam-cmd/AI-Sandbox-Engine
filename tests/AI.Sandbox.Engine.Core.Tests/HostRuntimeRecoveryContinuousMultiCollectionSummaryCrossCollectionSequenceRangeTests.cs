namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoveryContinuousMultiCollectionSummaryCrossCollectionSequenceRangeTests
{
    [Xunit.Fact]
    public void IdsAndPublicContractNamesAreExact()
    {
        Xunit.Assert.True(typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionIdKind).IsPublic);
        Xunit.Assert.True(typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQueryIdKind).IsPublic);

        AssertGenericPublic(
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<,,>));
        AssertGenericPublic(
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQuery<,,>));
        AssertGenericPublic(
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionResult<,,>));
        AssertGenericPublic(
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQueryResult<,,>));
    }

    [Xunit.Fact]
    public void FlowExposesProjectionAndBoundedRangeQueryMethods()
    {
        var flow = typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiCollectionSummaryFlow);
        var methods = flow.GetMethods(
            global::System.Reflection.BindingFlags.Public |
            global::System.Reflection.BindingFlags.Static |
            global::System.Reflection.BindingFlags.DeclaredOnly);

        var project = Xunit.Assert.Single(
            methods,
            method => method.Name == "ProjectSummary");
        var query = Xunit.Assert.Single(
            methods,
            method => method.Name == "QueryRange");

        Xunit.Assert.True(project.IsGenericMethodDefinition);
        Xunit.Assert.True(query.IsGenericMethodDefinition);
        Xunit.Assert.Equal(3, project.GetGenericArguments().Length);
        Xunit.Assert.Equal(3, query.GetGenericArguments().Length);
        Xunit.Assert.Equal(4, project.GetParameters().Length);
        Xunit.Assert.Equal(6, query.GetParameters().Length);
    }

    [Xunit.Fact]
    public void StatusDefinesExplicitProjectionAndQueryOutcomes()
    {
        var names = global::System.Enum.GetNames<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus>();

        Xunit.Assert.Contains(
            "ContinuousMultiCollectionSummaryProjected",
            names);
        Xunit.Assert.Contains(
            "CrossCollectionSequenceCheckpointRangeQueried",
            names);
        Xunit.Assert.Contains("StaleContinuityRevision", names);
        Xunit.Assert.Contains(
            "RangeDoesNotCrossCollectionSequenceBoundary",
            names);
        Xunit.Assert.Contains("RangeTooLarge", names);
        Xunit.Assert.Equal(names.Length, global::System.Linq.Enumerable.Count(global::System.Linq.Enumerable.Distinct(names)));
    }

    [Xunit.Fact]
    public void SummaryProjectionIsImmutableAndRevisioned()
    {
        var type = typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<,,>);

        AssertImmutable(type);
        AssertReadableProperty(type, "SummaryId");
        AssertReadableProperty(type, "Continuity");
        AssertReadableProperty(type, "RangeSummary");
        AssertReadableProperty(type, "AdjacentCollectionSequence");
        AssertReadableProperty(type, "ConnectingSupersession");
        AssertReadableProperty(type, "StartCollectionPairIndex");
        AssertReadableProperty(type, "EndCollectionPairIndex");
        AssertReadableProperty(type, "ProjectedTick");
        AssertReadableProperty(type, "Revision");
    }

    [Xunit.Fact]
    public void RangeQueryIsImmutableAndCrossesCollectionSequenceBoundary()
    {
        var type = typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQuery<,,>);

        AssertImmutable(type);
        AssertReadableProperty(type, "QueryId");
        AssertReadableProperty(type, "MultiCollectionSummary");
        AssertReadableProperty(type, "Checkpoints");
        AssertReadableProperty(type, "Supersessions");
        AssertReadableProperty(type, "ConnectingSupersession");
        AssertReadableProperty(type, "CrossesSequenceBoundary");
        AssertReadableProperty(type, "QueriedTick");
        AssertReadableProperty(type, "Revision");
    }

    [Xunit.Fact]
    public void ResultContractsExposeExplicitSuccess()
    {
        var summaryResult = typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionResult<,,>);
        var queryResult = typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQueryResult<,,>);

        AssertImmutable(summaryResult);
        AssertImmutable(queryResult);
        AssertReadableProperty(summaryResult, "Status");
        AssertReadableProperty(summaryResult, "Continuity");
        AssertReadableProperty(summaryResult, "Summary");
        AssertReadableProperty(summaryResult, "Succeeded");
        AssertReadableProperty(queryResult, "Status");
        AssertReadableProperty(queryResult, "MultiCollectionSummary");
        AssertReadableProperty(queryResult, "Query");
        AssertReadableProperty(queryResult, "Succeeded");
    }

    [Xunit.Fact]
    public void ConstructorsRemainNonPublic()
    {
        foreach (var type in new[]
        {
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<,,>),
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQuery<,,>),
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionResult<,,>),
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQueryResult<,,>),
        })
        {
            var publicConstructors = type.GetConstructors(
                global::System.Reflection.BindingFlags.Public |
                global::System.Reflection.BindingFlags.Instance);
            Xunit.Assert.Empty(publicConstructors);
        }
    }

    [Xunit.Fact]
    public void MaximumCheckpointCountRemainsBounded()
    {
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryLineageWindowFlow.MaximumCheckpointCount,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSummaryFlow
                .MaximumCheckpointCount);
        Xunit.Assert.InRange(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSummaryFlow
                .MaximumCheckpointCount,
            1,
            1024);
    }

    [Xunit.Fact]
    public void ContractsRemainSynchronousAndSideEffectFree()
    {
        var flow = typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiCollectionSummaryFlow);
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
            .HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<,,>));
        AssertImmutable(typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQuery<,,>));
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
