using System.Reflection;
using AI.Sandbox.Engine.Core.HostRuntime;
using Xunit;

namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoveryMultiCollectionSequenceRangeSummaryAdjacentMultiCollectionTests
{
    [Fact]
    public void PublicContractsAreAvailable()
    {
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjectionIdKind).IsValueType);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSelectionIdKind).IsValueType);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus).IsEnum);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSelection<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjectionResult<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSelectionResult<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryFlow).IsAbstract);
    }

    [Fact]
    public void SummaryStatusDefinesExpectedOutcomes()
    {
        var names = Enum.GetNames<HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus>();
        Assert.Equal(13, names.Length);
        Assert.Contains("MultiCollectionSequenceCheckpointRangeSummaryProjected", names);
        Assert.Contains("PreviousAdjacentMultiCollectionSelected", names);
        Assert.Contains("NextAdjacentMultiCollectionSelected", names);
        Assert.Contains("StaleRangeRevision", names);
        Assert.Contains("RangeSummaryProjectionTickRegressed", names);
        Assert.Contains("StaleSummaryRevision", names);
        Assert.Contains("AdjacentMultiCollectionSelectionTickRegressed", names);
        Assert.Contains("TooManyAdjacentMultiCollections", names);
        Assert.Contains("NoPreviousAdjacentMultiCollection", names);
        Assert.Contains("NoNextAdjacentMultiCollection", names);
        Assert.Contains("PreviousAdjacentMultiCollectionTooShort", names);
        Assert.Contains("NextAdjacentMultiCollectionTooShort", names);
        Assert.Contains("AdjacentMultiCollectionBoundaryMismatch", names);
    }

    [Fact]
    public void SummaryProjectionPreservesExactRangeAuthorityShape()
    {
        var type = typeof(HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<,,>);
        AssertReadableProperties(type,
            "SummaryId", "Range", "Sequence", "SourceSequence", "SourceCollection",
            "SourceProjection", "Chain", "StartCheckpoint", "EndCheckpoint",
            "IncomingSupersession", "OutgoingSupersession", "CrossedBoundarySupersessions",
            "StartCheckpointIndex", "EndCheckpointIndex", "StartSummaryIndex",
            "EndSummaryIndex", "CheckpointCount", "SupersessionCount",
            "MultiCollectionCount", "SequenceCount", "CrossedBoundaryCount",
            "StartsAtSequenceStart", "EndsAtSequenceEnd", "CoversEntireSequence",
            "ProjectedTick", "Revision");
    }

    [Fact]
    public void AdjacentSelectionPreservesExactMultiCollectionAuthorities()
    {
        var type = typeof(HostRuntimeRecoveryAdjacentMultiCollectionSelection<,,>);
        AssertReadableProperties(type,
            "SelectionId", "Summary", "SourceSequence",
            "SelectsPreviousMultiCollection", "SelectsNextMultiCollection",
            "MultiCollectionSummaries", "BoundarySupersessions",
            "AdjacentBoundarySupersession", "FirstSummary", "LastSummary",
            "StartSummaryIndex", "EndSummaryIndex", "MultiCollectionCount",
            "CollectionPairCount", "CollectionCount", "SummaryCount",
            "SequenceCount", "PairCount", "WindowCount",
            "StartCollectionPairIndex", "EndCollectionPairIndex",
            "StartCheckpointIndex", "EndCheckpointIndex", "CheckpointCount",
            "SupersessionCount", "StartCheckpoint", "EndCheckpoint",
            "IncomingSupersession", "OutgoingSupersession",
            "StartsAtSourceSequenceStart", "EndsAtSourceSequenceEnd",
            "SelectedTick", "Revision");
    }

    [Fact]
    public void FlowExposesBoundedProjectionAndSelectionMethods()
    {
        var type = typeof(HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryFlow);
        Assert.Equal(
            HostRuntimeRecoveryContinuousMultiCollectionSequenceFlow.MaximumSummaryCount,
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryFlow.MaximumAdjacentMultiCollectionCount);
        AssertGenericMethod(type, "ProjectSummary", 4);
        AssertGenericMethod(type, "SelectPreviousMultiCollection", 5);
        AssertGenericMethod(type, "SelectNextMultiCollection", 5);
    }

    [Fact]
    public void ResultContractsPreserveSourceAuthorities()
    {
        AssertReadableProperties(
            typeof(HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjectionResult<,,>),
            "Status", "Range", "Summary", "Succeeded");
        AssertReadableProperties(
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionSelectionResult<,,>),
            "Status", "Summary", "Selection", "Succeeded");
    }

    [Fact]
    public void SummaryAndSelectionContractsExposeNoPublicSetters()
    {
        foreach (var type in ContractTypes())
        {
            Assert.DoesNotContain(
                type.GetProperties(BindingFlags.Instance | BindingFlags.Public),
                property => property.SetMethod is { IsPublic: true });
        }
    }

    [Fact]
    public void CollectionPropertiesRemainReadOnly()
    {
        AssertReadOnlyListProperty(
            typeof(HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<,,>),
            "CrossedBoundarySupersessions");
        AssertReadOnlyListProperty(
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionSelection<,,>),
            "MultiCollectionSummaries");
        AssertReadOnlyListProperty(
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionSelection<,,>),
            "BoundarySupersessions");
    }

    [Fact]
    public void ContractsRemainBoundedAndSideEffectFree()
    {
        var flow = typeof(HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryFlow);
        Assert.InRange(
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryFlow.MaximumAdjacentMultiCollectionCount,
            1,
            HostRuntimeRecoveryContinuousMultiCollectionSequenceFlow.MaximumSummaryCount);
        foreach (var method in flow.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            var returnTypeName = method.ReturnType.FullName ?? string.Empty;
            Assert.False(returnTypeName.Contains("Task", StringComparison.Ordinal));
            Assert.False(returnTypeName.Contains("ValueTask", StringComparison.Ordinal));
        }
    }

    private static Type[] ContractTypes() =>
    [
        typeof(HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<,,>),
        typeof(HostRuntimeRecoveryAdjacentMultiCollectionSelection<,,>),
        typeof(HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjectionResult<,,>),
        typeof(HostRuntimeRecoveryAdjacentMultiCollectionSelectionResult<,,>)
    ];

    private static void AssertReadableProperties(Type type, params string[] names)
    {
        foreach (var name in names)
        {
            var property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            Assert.NotNull(property);
            Assert.NotNull(property!.GetMethod);
        }
    }

    private static void AssertGenericMethod(Type type, string name, int parameterCount)
    {
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(method => method.Name == name && method.IsGenericMethodDefinition)
            .ToArray();
        var method = Assert.Single(methods);
        Assert.Equal(3, method.GetGenericArguments().Length);
        Assert.Equal(parameterCount, method.GetParameters().Length);
    }

    private static void AssertReadOnlyListProperty(Type type, string name)
    {
        var property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
        Assert.NotNull(property);
        Assert.True(property!.PropertyType.IsGenericType);
        Assert.Equal(typeof(IReadOnlyList<>), property.PropertyType.GetGenericTypeDefinition());
    }
}
