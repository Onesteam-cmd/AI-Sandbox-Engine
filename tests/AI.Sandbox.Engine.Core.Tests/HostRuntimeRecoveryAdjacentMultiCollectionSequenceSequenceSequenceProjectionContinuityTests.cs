using System.Reflection;
using AI.Sandbox.Engine.Core.HostRuntime;
using Xunit;

namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionContinuityTests
{
    [Fact]
    public void PublicContractsAreAvailable()
    {
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionIdKind).IsClass);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationIdKind).IsClass);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus).IsEnum);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjection<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidation<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionResult<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationResult<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceFlow).IsAbstract);
    }

    [Fact]
    public void ProjectionStatusDefinesExpectedOutcomes()
    {
        var names = Enum.GetNames<HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus>();
        Assert.Equal(16, names.Length);
        Assert.Contains("AdjacentMultiCollectionSequenceSequenceSequenceProjected", names);
        Assert.Contains("MultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidated", names);
        Assert.Contains("StaleSelectionRevision", names);
        Assert.Contains("AdjacentMultiCollectionSequenceSequenceSequenceProjectionTickRegressed", names);
        Assert.Contains("SelectionMultiCollectionSequenceSequenceSequenceSummaryMismatch", names);
        Assert.Contains("SelectionBoundarySupersessionMismatch", names);
        Assert.Contains("SelectionCheckpointMismatch", names);
        Assert.Contains("SelectionSupersessionMismatch", names);
        Assert.Contains("StaleRangeSummaryRevision", names);
        Assert.Contains("StaleAdjacentMultiCollectionSequenceSequenceSequenceRevision", names);
        Assert.Contains("ContinuityValidationTickRegressed", names);
        Assert.Contains("RangeSummaryMismatch", names);
        Assert.Contains("MultiCollectionSequenceSequenceSequenceRangeNotAdjacent", names);
        Assert.Contains("CheckpointRangeNotAdjacent", names);
        Assert.Contains("SupersessionBoundaryMismatch", names);
        Assert.Contains("CheckpointBoundaryMismatch", names);
    }

    [Fact]
    public void AdjacentProjectionPreservesExactMultiCollectionSequenceAuthorityShape()
    {
        var type = typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjection<,,>);
        AssertReadableProperties(type,
            "ProjectionId", "Selection", "Summary", "SourceSequence",
            "SourceCollection", "SourceProjection", "Chain",
            "SelectsPreviousMultiCollectionSequenceSequenceSequence", "SelectsNextMultiCollectionSequenceSequenceSequence",
            "MultiCollectionSequenceSequenceSequenceSummaries", "BoundarySupersessions",
            "Checkpoints", "Supersessions", "AdjacentBoundarySupersession",
            "StartSequenceSequenceSequenceSummaryIndex", "EndSequenceSequenceSequenceSummaryIndex",
            "MultiCollectionSequenceSequenceSequenceCount",
            "StartSequenceSequenceSummaryIndex", "EndSequenceSequenceSummaryIndex",
            "MultiCollectionSequenceSequenceCount",
            "StartSequenceSummaryIndex", "EndSequenceSummaryIndex",
            "MultiCollectionSequenceCount", "MultiCollectionCount",
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
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidation<,,>);
        AssertReadableProperties(type,
            "ValidationId", "Summary", "AdjacentMultiCollectionSequenceSequenceSequence", "Selection",
            "ValidatesPreviousMultiCollectionSequenceSequenceSequence", "ValidatesNextMultiCollectionSequenceSequenceSequence",
            "ConnectingSupersession", "PriorCheckpoint", "SuccessorCheckpoint",
            "ValidatedTick", "Revision");
    }

    [Fact]
    public void FlowExposesProjectionAndContinuityMethods()
    {
        var type = typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceFlow);
        AssertGenericMethod(type, "ProjectMultiCollectionSequenceSequenceSequence", 4);
        AssertGenericMethod(type, "ValidateContinuity", 6);
    }

    [Fact]
    public void ResultContractsPreserveSourceAuthorities()
    {
        AssertReadableProperties(
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionResult<,,>),
            "Status", "Selection", "Projection", "Succeeded");
        AssertReadableProperties(
            typeof(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationResult<,,>),
            "Status", "Summary", "AdjacentMultiCollectionSequenceSequenceSequence", "Validation", "Succeeded");
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
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjection<,,>);
        AssertReadOnlyListProperty(projection, "MultiCollectionSequenceSequenceSequenceSummaries");
        AssertReadOnlyListProperty(projection, "BoundarySupersessions");
        AssertReadOnlyListProperty(projection, "Checkpoints");
        AssertReadOnlyListProperty(projection, "Supersessions");
    }

    [Fact]
    public void ContractsRemainSynchronousAndSideEffectFree()
    {
        var flow = typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceFlow);
        foreach (var method in flow.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            var returnTypeName = method.ReturnType.FullName ?? string.Empty;
            Assert.False(returnTypeName.Contains("Task", StringComparison.Ordinal));
            Assert.False(returnTypeName.Contains("ValueTask", StringComparison.Ordinal));
        }
    }

    private static Type[] ContractTypes() =>
    [
        typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjection<,,>),
        typeof(
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidation<,,>),
        typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionResult<,,>),
        typeof(
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationResult<,,>)
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
