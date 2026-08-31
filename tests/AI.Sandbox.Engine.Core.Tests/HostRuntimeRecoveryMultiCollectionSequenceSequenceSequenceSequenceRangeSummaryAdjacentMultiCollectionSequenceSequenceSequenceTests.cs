using System.Reflection;
using AI.Sandbox.Engine.Core.HostRuntime;
using Xunit;

namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryAdjacentMultiCollectionSequenceSequenceSequenceTests
{
    [Fact]
    public void PublicContractsAreAvailable()
    {
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjectionIdKind).IsValueType);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionIdKind).IsValueType);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus).IsEnum);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelection<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionResult<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryFlow).IsAbstract);
    }

    [Fact]
    public void SummaryStatusDefinesExpectedOutcomes()
    {
        var names = Enum.GetNames<HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus>();
        Assert.Equal(13, names.Length);
        Assert.Contains("MultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjected", names);
        Assert.Contains("PreviousAdjacentMultiCollectionSequenceSequenceSequenceSelected", names);
        Assert.Contains("NextAdjacentMultiCollectionSequenceSequenceSequenceSelected", names);
        Assert.Contains("StaleRangeRevision", names);
        Assert.Contains("RangeSummaryProjectionTickRegressed", names);
        Assert.Contains("StaleSummaryRevision", names);
        Assert.Contains("AdjacentMultiCollectionSequenceSequenceSequenceSelectionTickRegressed", names);
        Assert.Contains("TooManyAdjacentMultiCollectionSequenceSequenceSequences", names);
        Assert.Contains("NoPreviousAdjacentMultiCollectionSequenceSequenceSequence", names);
        Assert.Contains("NoNextAdjacentMultiCollectionSequenceSequenceSequence", names);
        Assert.Contains("PreviousAdjacentMultiCollectionSequenceSequenceSequenceTooShort", names);
        Assert.Contains("NextAdjacentMultiCollectionSequenceSequenceSequenceTooShort", names);
        Assert.Contains("AdjacentMultiCollectionSequenceSequenceSequenceBoundaryMismatch", names);
    }

    [Fact]
    public void SummaryProjectionPreservesExactRangeAuthorityShape()
    {
        var type = typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection<,,>);
        AssertReadableProperties(type,
            "SummaryId", "Range", "Sequence", "SourceSequence", "SourceCollection",
            "SourceProjection", "Chain", "StartCheckpoint", "EndCheckpoint",
            "IncomingSupersession", "OutgoingSupersession", "CrossedBoundarySupersessions",
            "StartCheckpointIndex", "EndCheckpointIndex", "StartSummaryIndex",
            "EndSummaryIndex", "CheckpointCount", "SupersessionCount",
            "MultiCollectionSequenceSequenceSequenceCount", "SummaryCount", "CrossedBoundaryCount",
            "StartsAtSequenceStart", "EndsAtSequenceEnd", "CoversEntireSequence",
            "ProjectedTick", "Revision");
    }

    [Fact]
    public void AdjacentSelectionPreservesExactMultiCollectionSequenceSequenceSequenceAuthorities()
    {
        var type = typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelection<,,>);
        AssertReadableProperties(type,
            "SelectionId", "Summary", "SourceSequence",
            "SelectsPreviousMultiCollectionSequenceSequenceSequence", "SelectsNextMultiCollectionSequenceSequenceSequence",
            "MultiCollectionSequenceSequenceSequenceSummaries", "BoundarySupersessions",
            "AdjacentBoundarySupersession", "FirstSummary", "LastSummary",
            "StartSequenceSequenceSequenceSummaryIndex", "EndSequenceSequenceSequenceSummaryIndex",
            "MultiCollectionSequenceSequenceSequenceCount",
            "StartSequenceSequenceSummaryIndex", "EndSequenceSequenceSummaryIndex",
            "MultiCollectionSequenceSequenceCount",
            "MultiCollectionSequenceCount", "MultiCollectionCount",
            "CollectionPairCount", "CollectionCount",
            "SummaryCount", "SequenceCount", "PairCount", "WindowCount",
            "StartCheckpointIndex", "EndCheckpointIndex", "CheckpointCount",
            "SupersessionCount", "StartCheckpoint", "EndCheckpoint",
            "IncomingSupersession", "OutgoingSupersession",
            "StartsAtSourceSequenceStart", "EndsAtSourceSequenceEnd",
            "SelectedTick", "Revision");
    }

    [Fact]
    public void FlowExposesBoundedProjectionAndSelectionMethods()
    {
        var type = typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryFlow);
        Assert.Equal(
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceFlow.MaximumSummaryCount,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryFlow.MaximumAdjacentMultiCollectionSequenceSequenceSequenceCount);
        AssertGenericMethod(type, "ProjectSummary", 4);
        AssertGenericMethod(type, "SelectPreviousMultiCollectionSequenceSequenceSequence", 5);
        AssertGenericMethod(type, "SelectNextMultiCollectionSequenceSequenceSequence", 5);
    }

    [Fact]
    public void ResultContractsPreserveSourceAuthorities()
    {
        AssertReadableProperties(
            typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult<,,>),
            "Status", "Range", "Summary", "Succeeded");
        AssertReadableProperties(
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionResult<,,>),
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
            typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection<,,>),
            "CrossedBoundarySupersessions");
        AssertReadOnlyListProperty(
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelection<,,>),
            "MultiCollectionSequenceSequenceSequenceSummaries");
        AssertReadOnlyListProperty(
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelection<,,>),
            "BoundarySupersessions");
    }

    [Fact]
    public void ContractsRemainBoundedAndSideEffectFree()
    {
        var flow = typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryFlow);
        Assert.InRange(
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryFlow.MaximumAdjacentMultiCollectionSequenceSequenceSequenceCount,
            1,
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceFlow.MaximumSummaryCount);
        foreach (var method in flow.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            var returnTypeName = method.ReturnType.FullName ?? string.Empty;
            Assert.False(returnTypeName.Contains("Task", StringComparison.Ordinal));
            Assert.False(returnTypeName.Contains("ValueTask", StringComparison.Ordinal));
        }
    }

    private static Type[] ContractTypes() =>
    [
        typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection<,,>),
        typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelection<,,>),
        typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult<,,>),
        typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionResult<,,>)
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
