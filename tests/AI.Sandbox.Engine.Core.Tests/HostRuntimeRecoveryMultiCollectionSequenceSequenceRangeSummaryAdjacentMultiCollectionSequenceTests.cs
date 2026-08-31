using System.Reflection;
using AI.Sandbox.Engine.Core.HostRuntime;
using Xunit;

namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoveryMultiCollectionSequenceSequenceRangeSummaryAdjacentMultiCollectionSequenceTests
{
    [Fact]
    public void PublicContractsAreAvailable()
    {
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjectionIdKind).IsValueType);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionIdKind).IsValueType);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus).IsEnum);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjectionResult<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionResult<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryFlow).IsAbstract);
    }

    [Fact]
    public void SummaryStatusDefinesExpectedOutcomes()
    {
        var names = Enum.GetNames<HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus>();
        Assert.Equal(13, names.Length);
        Assert.Contains("MultiCollectionSequenceSequenceCheckpointRangeSummaryProjected", names);
        Assert.Contains("PreviousAdjacentMultiCollectionSequenceSelected", names);
        Assert.Contains("NextAdjacentMultiCollectionSequenceSelected", names);
        Assert.Contains("StaleRangeRevision", names);
        Assert.Contains("RangeSummaryProjectionTickRegressed", names);
        Assert.Contains("StaleSummaryRevision", names);
        Assert.Contains("AdjacentMultiCollectionSequenceSelectionTickRegressed", names);
        Assert.Contains("TooManyAdjacentMultiCollectionSequences", names);
        Assert.Contains("NoPreviousAdjacentMultiCollectionSequence", names);
        Assert.Contains("NoNextAdjacentMultiCollectionSequence", names);
        Assert.Contains("PreviousAdjacentMultiCollectionSequenceTooShort", names);
        Assert.Contains("NextAdjacentMultiCollectionSequenceTooShort", names);
        Assert.Contains("AdjacentMultiCollectionSequenceBoundaryMismatch", names);
    }

    [Fact]
    public void SummaryProjectionPreservesExactRangeAuthorityShape()
    {
        var type = typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection<,,>);
        AssertReadableProperties(type,
            "SummaryId", "Range", "Sequence", "SourceSequence", "SourceCollection",
            "SourceProjection", "Chain", "StartCheckpoint", "EndCheckpoint",
            "IncomingSupersession", "OutgoingSupersession", "CrossedBoundarySupersessions",
            "StartCheckpointIndex", "EndCheckpointIndex", "StartSummaryIndex",
            "EndSummaryIndex", "CheckpointCount", "SupersessionCount",
            "MultiCollectionSequenceCount", "SummaryCount", "CrossedBoundaryCount",
            "StartsAtSequenceStart", "EndsAtSequenceEnd", "CoversEntireSequence",
            "ProjectedTick", "Revision");
    }

    [Fact]
    public void AdjacentSelectionPreservesExactMultiCollectionSequenceAuthorities()
    {
        var type = typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection<,,>);
        AssertReadableProperties(type,
            "SelectionId", "Summary", "SourceSequence",
            "SelectsPreviousMultiCollectionSequence", "SelectsNextMultiCollectionSequence",
            "MultiCollectionSequenceSummaries", "BoundarySupersessions",
            "AdjacentBoundarySupersession", "FirstSummary", "LastSummary",
            "StartSequenceSummaryIndex", "EndSequenceSummaryIndex",
            "MultiCollectionSequenceCount", "StartSummaryIndex", "EndSummaryIndex",
            "MultiCollectionCount", "CollectionPairCount", "CollectionCount",
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
        var type = typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryFlow);
        Assert.Equal(
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceFlow.MaximumSummaryCount,
            HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryFlow.MaximumAdjacentMultiCollectionSequenceCount);
        AssertGenericMethod(type, "ProjectSummary", 4);
        AssertGenericMethod(type, "SelectPreviousMultiCollectionSequence", 5);
        AssertGenericMethod(type, "SelectNextMultiCollectionSequence", 5);
    }

    [Fact]
    public void ResultContractsPreserveSourceAuthorities()
    {
        AssertReadableProperties(
            typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjectionResult<,,>),
            "Status", "Range", "Summary", "Succeeded");
        AssertReadableProperties(
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionResult<,,>),
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
            typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection<,,>),
            "CrossedBoundarySupersessions");
        AssertReadOnlyListProperty(
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection<,,>),
            "MultiCollectionSequenceSummaries");
        AssertReadOnlyListProperty(
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection<,,>),
            "BoundarySupersessions");
    }

    [Fact]
    public void ContractsRemainBoundedAndSideEffectFree()
    {
        var flow = typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryFlow);
        Assert.InRange(
            HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryFlow.MaximumAdjacentMultiCollectionSequenceCount,
            1,
            HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceFlow.MaximumSummaryCount);
        foreach (var method in flow.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            var returnTypeName = method.ReturnType.FullName ?? string.Empty;
            Assert.False(returnTypeName.Contains("Task", StringComparison.Ordinal));
            Assert.False(returnTypeName.Contains("ValueTask", StringComparison.Ordinal));
        }
    }

    private static Type[] ContractTypes() =>
    [
        typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection<,,>),
        typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection<,,>),
        typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjectionResult<,,>),
        typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionResult<,,>)
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
