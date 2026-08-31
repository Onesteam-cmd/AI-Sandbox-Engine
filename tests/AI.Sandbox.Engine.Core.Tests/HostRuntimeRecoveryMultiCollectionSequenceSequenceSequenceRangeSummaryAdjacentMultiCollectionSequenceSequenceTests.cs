using System.Reflection;
using AI.Sandbox.Engine.Core.HostRuntime;
using Xunit;

namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryAdjacentMultiCollectionSequenceSequenceTests
{
    [Fact]
    public void PublicContractsAreAvailable()
    {
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjectionIdKind).IsValueType);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelectionIdKind).IsValueType);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus).IsEnum);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjection<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelection<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelectionResult<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryFlow).IsAbstract);
    }

    [Fact]
    public void SummaryStatusDefinesExpectedOutcomes()
    {
        var names = Enum.GetNames<HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus>();
        Assert.Equal(13, names.Length);
        Assert.Contains("MultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjected", names);
        Assert.Contains("PreviousAdjacentMultiCollectionSequenceSequenceSelected", names);
        Assert.Contains("NextAdjacentMultiCollectionSequenceSequenceSelected", names);
        Assert.Contains("StaleRangeRevision", names);
        Assert.Contains("RangeSummaryProjectionTickRegressed", names);
        Assert.Contains("StaleSummaryRevision", names);
        Assert.Contains("AdjacentMultiCollectionSequenceSequenceSelectionTickRegressed", names);
        Assert.Contains("TooManyAdjacentMultiCollectionSequenceSequences", names);
        Assert.Contains("NoPreviousAdjacentMultiCollectionSequenceSequence", names);
        Assert.Contains("NoNextAdjacentMultiCollectionSequenceSequence", names);
        Assert.Contains("PreviousAdjacentMultiCollectionSequenceSequenceTooShort", names);
        Assert.Contains("NextAdjacentMultiCollectionSequenceSequenceTooShort", names);
        Assert.Contains("AdjacentMultiCollectionSequenceSequenceBoundaryMismatch", names);
    }

    [Fact]
    public void SummaryProjectionPreservesExactRangeAuthorityShape()
    {
        var type = typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjection<,,>);
        AssertReadableProperties(type,
            "SummaryId", "Range", "Sequence", "SourceSequence", "SourceCollection",
            "SourceProjection", "Chain", "StartCheckpoint", "EndCheckpoint",
            "IncomingSupersession", "OutgoingSupersession", "CrossedBoundarySupersessions",
            "StartCheckpointIndex", "EndCheckpointIndex", "StartSummaryIndex",
            "EndSummaryIndex", "CheckpointCount", "SupersessionCount",
            "MultiCollectionSequenceSequenceCount", "SummaryCount", "CrossedBoundaryCount",
            "StartsAtSequenceStart", "EndsAtSequenceEnd", "CoversEntireSequence",
            "ProjectedTick", "Revision");
    }

    [Fact]
    public void AdjacentSelectionPreservesExactMultiCollectionSequenceSequenceAuthorities()
    {
        var type = typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelection<,,>);
        AssertReadableProperties(type,
            "SelectionId", "Summary", "SourceSequence",
            "SelectsPreviousMultiCollectionSequenceSequence", "SelectsNextMultiCollectionSequenceSequence",
            "MultiCollectionSequenceSequenceSummaries", "BoundarySupersessions",
            "AdjacentBoundarySupersession", "FirstSummary", "LastSummary",
            "StartSequenceSequenceSummaryIndex", "EndSequenceSequenceSummaryIndex",
            "MultiCollectionSequenceSequenceCount",
            "StartSequenceSummaryIndex", "EndSequenceSummaryIndex",
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
        var type = typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryFlow);
        Assert.Equal(
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceFlow.MaximumSummaryCount,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryFlow.MaximumAdjacentMultiCollectionSequenceSequenceCount);
        AssertGenericMethod(type, "ProjectSummary", 4);
        AssertGenericMethod(type, "SelectPreviousMultiCollectionSequenceSequence", 5);
        AssertGenericMethod(type, "SelectNextMultiCollectionSequenceSequence", 5);
    }

    [Fact]
    public void ResultContractsPreserveSourceAuthorities()
    {
        AssertReadableProperties(
            typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult<,,>),
            "Status", "Range", "Summary", "Succeeded");
        AssertReadableProperties(
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelectionResult<,,>),
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
            typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjection<,,>),
            "CrossedBoundarySupersessions");
        AssertReadOnlyListProperty(
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelection<,,>),
            "MultiCollectionSequenceSequenceSummaries");
        AssertReadOnlyListProperty(
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelection<,,>),
            "BoundarySupersessions");
    }

    [Fact]
    public void ContractsRemainBoundedAndSideEffectFree()
    {
        var flow = typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryFlow);
        Assert.InRange(
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryFlow.MaximumAdjacentMultiCollectionSequenceSequenceCount,
            1,
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceFlow.MaximumSummaryCount);
        foreach (var method in flow.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            var returnTypeName = method.ReturnType.FullName ?? string.Empty;
            Assert.False(returnTypeName.Contains("Task", StringComparison.Ordinal));
            Assert.False(returnTypeName.Contains("ValueTask", StringComparison.Ordinal));
        }
    }

    private static Type[] ContractTypes() =>
    [
        typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjection<,,>),
        typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelection<,,>),
        typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult<,,>),
        typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelectionResult<,,>)
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
