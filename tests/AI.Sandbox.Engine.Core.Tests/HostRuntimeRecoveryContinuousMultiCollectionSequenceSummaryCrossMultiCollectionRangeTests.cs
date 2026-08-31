namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryCrossMultiCollectionRangeTests
{
    [Xunit.Fact]
    public void IdsAndPublicContractNamesAreExact()
    {
        Xunit.Assert.True(typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjectionIdKind).IsPublic);
        Xunit.Assert.True(typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQueryIdKind).IsPublic);

        AssertGenericPublic(
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection<,,>));
        AssertGenericPublic(
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQuery<,,>));
        AssertGenericPublic(
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjectionResult<,,>));
        AssertGenericPublic(
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQueryResult<,,>));
    }

    [Xunit.Fact]
    public void FlowExposesProjectionAndBoundedRangeQueryMethods()
    {
        var flow = typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryFlow);
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
                .HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus>();

        Xunit.Assert.Contains(
            "ContinuousMultiCollectionSequenceSummaryProjected",
            names);
        Xunit.Assert.Contains(
            "CrossMultiCollectionCheckpointRangeQueried",
            names);
        Xunit.Assert.Contains("StaleContinuityRevision", names);
        Xunit.Assert.Contains(
            "RangeDoesNotCrossMultiCollectionBoundary",
            names);
        Xunit.Assert.Contains("RangeTooLarge", names);
        Xunit.Assert.Equal(names.Length, global::System.Linq.Enumerable.Count(global::System.Linq.Enumerable.Distinct(names)));
    }

    [Xunit.Fact]
    public void SummaryProjectionIsImmutableAndRevisioned()
    {
        var type = typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection<,,>);

        AssertImmutable(type);
        AssertReadableProperty(type, "SummaryId");
        AssertReadableProperty(type, "Continuity");
        AssertReadableProperty(type, "RangeSummary");
        AssertReadableProperty(type, "AdjacentMultiCollection");
        AssertReadableProperty(type, "ConnectingSupersession");
        AssertReadableProperty(type, "StartSummaryIndex");
        AssertReadableProperty(type, "EndSummaryIndex");
        AssertReadableProperty(type, "ProjectedTick");
        AssertReadableProperty(type, "Revision");
    }

    [Xunit.Fact]
    public void RangeQueryIsImmutableAndCrossesMultiCollectionBoundary()
    {
        var type = typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQuery<,,>);

        AssertImmutable(type);
        AssertReadableProperty(type, "QueryId");
        AssertReadableProperty(type, "MultiCollectionSequenceSummary");
        AssertReadableProperty(type, "Checkpoints");
        AssertReadableProperty(type, "Supersessions");
        AssertReadableProperty(type, "ConnectingSupersession");
        AssertReadableProperty(type, "CrossesMultiCollectionBoundary");
        AssertReadableProperty(type, "QueriedTick");
        AssertReadableProperty(type, "Revision");
    }

    [Xunit.Fact]
    public void ResultContractsExposeExplicitSuccess()
    {
        var summaryResult = typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjectionResult<,,>);
        var queryResult = typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQueryResult<,,>);

        AssertImmutable(summaryResult);
        AssertImmutable(queryResult);
        AssertReadableProperty(summaryResult, "Status");
        AssertReadableProperty(summaryResult, "Continuity");
        AssertReadableProperty(summaryResult, "Summary");
        AssertReadableProperty(summaryResult, "Succeeded");
        AssertReadableProperty(queryResult, "Status");
        AssertReadableProperty(queryResult, "MultiCollectionSequenceSummary");
        AssertReadableProperty(queryResult, "Query");
        AssertReadableProperty(queryResult, "Succeeded");
    }

    [Xunit.Fact]
    public void ConstructorsRemainNonPublic()
    {
        foreach (var type in new[]
        {
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection<,,>),
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQuery<,,>),
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjectionResult<,,>),
            typeof(global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQueryResult<,,>),
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
                .HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryFlow
                .MaximumCheckpointCount);
        Xunit.Assert.InRange(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryFlow
                .MaximumCheckpointCount,
            1,
            1024);
    }

    [Xunit.Fact]
    public void ContractsRemainSynchronousAndSideEffectFree()
    {
        var flow = typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryFlow);
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
            .HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection<,,>));
        AssertImmutable(typeof(global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQuery<,,>));
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
