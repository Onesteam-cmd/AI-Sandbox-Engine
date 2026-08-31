using System.Reflection;
using AI.Sandbox.Engine.Core.HostRuntime;
using Xunit;

namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoveryAdjacentMultiCollectionProjectionContinuityTests
{
    [Fact]
    public void PublicContractsAreAvailable()
    {
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionProjectionIdKind).IsClass);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidationIdKind).IsClass);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus).IsEnum);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionProjection<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidation<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionProjectionResult<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidationResult<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionFlow).IsAbstract);
    }

    [Fact]
    public void ProjectionStatusDefinesExpectedOutcomes()
    {
        var names = Enum.GetNames<HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus>();
        Assert.Equal(16, names.Length);
        Assert.Contains("AdjacentMultiCollectionProjected", names);
        Assert.Contains("MultiCollectionSequenceCheckpointRangeContinuityValidated", names);
        Assert.Contains("StaleSelectionRevision", names);
        Assert.Contains("AdjacentMultiCollectionProjectionTickRegressed", names);
        Assert.Contains("SelectionMultiCollectionSummaryMismatch", names);
        Assert.Contains("SelectionBoundarySupersessionMismatch", names);
        Assert.Contains("SelectionCheckpointMismatch", names);
        Assert.Contains("SelectionSupersessionMismatch", names);
        Assert.Contains("StaleRangeSummaryRevision", names);
        Assert.Contains("StaleAdjacentMultiCollectionRevision", names);
        Assert.Contains("ContinuityValidationTickRegressed", names);
        Assert.Contains("RangeSummaryMismatch", names);
        Assert.Contains("MultiCollectionRangeNotAdjacent", names);
        Assert.Contains("CheckpointRangeNotAdjacent", names);
        Assert.Contains("SupersessionBoundaryMismatch", names);
        Assert.Contains("CheckpointBoundaryMismatch", names);
    }

    [Fact]
    public void AdjacentProjectionPreservesExactMultiCollectionAuthorityShape()
    {
        var type = typeof(HostRuntimeRecoveryAdjacentMultiCollectionProjection<,,>);
        AssertReadableProperties(type,
            "ProjectionId", "Selection", "Summary", "SourceSequence",
            "SourceCollection", "SourceProjection", "Chain",
            "SelectsPreviousMultiCollection", "SelectsNextMultiCollection",
            "MultiCollectionSummaries", "BoundarySupersessions",
            "Checkpoints", "Supersessions", "AdjacentBoundarySupersession",
            "StartSummaryIndex", "EndSummaryIndex", "MultiCollectionCount",
            "CollectionPairCount", "CollectionCount", "SummaryCount",
            "SequenceCount", "PairCount", "WindowCount",
            "StartCheckpointIndex", "EndCheckpointIndex",
            "CheckpointCount", "SupersessionCount",
            "StartCheckpoint", "EndCheckpoint",
            "IncomingSupersession", "OutgoingSupersession",
            "StartsAtSourceSequenceStart", "EndsAtSourceSequenceEnd",
            "ProjectedTick", "Revision");
    }

    [Fact]
    public void ContinuityValidationPreservesExactBoundaryAuthorityShape()
    {
        var type = typeof(
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidation<,,>);
        AssertReadableProperties(type,
            "ValidationId", "Summary", "AdjacentMultiCollection", "Selection",
            "ValidatesPreviousMultiCollection", "ValidatesNextMultiCollection",
            "ConnectingSupersession", "PriorCheckpoint", "SuccessorCheckpoint",
            "ValidatedTick", "Revision");
    }

    [Fact]
    public void FlowExposesProjectionAndContinuityMethods()
    {
        var type = typeof(HostRuntimeRecoveryAdjacentMultiCollectionFlow);
        AssertGenericMethod(type, "ProjectMultiCollection", 4);
        AssertGenericMethod(type, "ValidateContinuity", 6);
    }

    [Fact]
    public void ResultContractsPreserveSourceAuthorities()
    {
        AssertReadableProperties(
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionProjectionResult<,,>),
            "Status", "Selection", "Projection", "Succeeded");
        AssertReadableProperties(
            typeof(
                HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidationResult<,,>),
            "Status", "Summary", "AdjacentMultiCollection", "Validation", "Succeeded");
    }

    [Fact]
    public void ProjectionAndContinuityContractsExposeNoPublicSetters()
    {
        foreach (var type in ContractTypes())
        {
            Assert.DoesNotContain(
                type.GetProperties(BindingFlags.Instance | BindingFlags.Public),
                property => property.SetMethod is { IsPublic: true });
        }
    }

    [Fact]
    public void MaterializedEvidenceCollectionsRemainReadOnly()
    {
        var projection =
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionProjection<,,>);
        AssertReadOnlyListProperty(projection, "MultiCollectionSummaries");
        AssertReadOnlyListProperty(projection, "BoundarySupersessions");
        AssertReadOnlyListProperty(projection, "Checkpoints");
        AssertReadOnlyListProperty(projection, "Supersessions");
    }

    [Fact]
    public void ContractsRemainSynchronousAndSideEffectFree()
    {
        var flow = typeof(HostRuntimeRecoveryAdjacentMultiCollectionFlow);
        foreach (var method in flow.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            var returnTypeName = method.ReturnType.FullName ?? string.Empty;
            Assert.False(returnTypeName.Contains("Task", StringComparison.Ordinal));
            Assert.False(returnTypeName.Contains("ValueTask", StringComparison.Ordinal));
        }
    }

    private static Type[] ContractTypes() =>
    [
        typeof(HostRuntimeRecoveryAdjacentMultiCollectionProjection<,,>),
        typeof(
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidation<,,>),
        typeof(HostRuntimeRecoveryAdjacentMultiCollectionProjectionResult<,,>),
        typeof(
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidationResult<,,>)
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
