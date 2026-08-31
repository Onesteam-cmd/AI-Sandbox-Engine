using System.Reflection;
using AI.Sandbox.Engine.Core.HostRuntime;
using Xunit;

namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionContinuityTests
{
    [Fact]
    public void PublicContractsAreAvailable()
    {
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionIdKind).IsClass);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidationIdKind).IsClass);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus).IsEnum);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjection<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidation<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionResult<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidationResult<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceFlow).IsAbstract);
    }

    [Fact]
    public void ProjectionStatusDefinesExpectedOutcomes()
    {
        var names = Enum.GetNames<HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus>();
        Assert.Equal(16, names.Length);
        Assert.Contains("AdjacentMultiCollectionSequenceProjected", names);
        Assert.Contains("MultiCollectionSequenceSequenceCheckpointRangeContinuityValidated", names);
        Assert.Contains("StaleSelectionRevision", names);
        Assert.Contains("AdjacentMultiCollectionSequenceProjectionTickRegressed", names);
        Assert.Contains("SelectionMultiCollectionSequenceSummaryMismatch", names);
        Assert.Contains("SelectionBoundarySupersessionMismatch", names);
        Assert.Contains("SelectionCheckpointMismatch", names);
        Assert.Contains("SelectionSupersessionMismatch", names);
        Assert.Contains("StaleRangeSummaryRevision", names);
        Assert.Contains("StaleAdjacentMultiCollectionSequenceRevision", names);
        Assert.Contains("ContinuityValidationTickRegressed", names);
        Assert.Contains("RangeSummaryMismatch", names);
        Assert.Contains("MultiCollectionSequenceRangeNotAdjacent", names);
        Assert.Contains("CheckpointRangeNotAdjacent", names);
        Assert.Contains("SupersessionBoundaryMismatch", names);
        Assert.Contains("CheckpointBoundaryMismatch", names);
    }

    [Fact]
    public void AdjacentProjectionPreservesExactMultiCollectionSequenceAuthorityShape()
    {
        var type = typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjection<,,>);
        AssertReadableProperties(type,
            "ProjectionId", "Selection", "Summary", "SourceSequence",
            "SourceCollection", "SourceProjection", "Chain",
            "SelectsPreviousMultiCollectionSequence", "SelectsNextMultiCollectionSequence",
            "MultiCollectionSequenceSummaries", "BoundarySupersessions",
            "Checkpoints", "Supersessions", "AdjacentBoundarySupersession",
            "StartSequenceSummaryIndex", "EndSequenceSummaryIndex",
            "MultiCollectionSequenceCount",
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
            HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidation<,,>);
        AssertReadableProperties(type,
            "ValidationId", "Summary", "AdjacentMultiCollectionSequence", "Selection",
            "ValidatesPreviousMultiCollectionSequence", "ValidatesNextMultiCollectionSequence",
            "ConnectingSupersession", "PriorCheckpoint", "SuccessorCheckpoint",
            "ValidatedTick", "Revision");
    }

    [Fact]
    public void FlowExposesProjectionAndContinuityMethods()
    {
        var type = typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceFlow);
        AssertGenericMethod(type, "ProjectMultiCollectionSequence", 4);
        AssertGenericMethod(type, "ValidateContinuity", 6);
    }

    [Fact]
    public void ResultContractsPreserveSourceAuthorities()
    {
        AssertReadableProperties(
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionResult<,,>),
            "Status", "Selection", "Projection", "Succeeded");
        AssertReadableProperties(
            typeof(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidationResult<,,>),
            "Status", "Summary", "AdjacentMultiCollectionSequence", "Validation", "Succeeded");
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
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjection<,,>);
        AssertReadOnlyListProperty(projection, "MultiCollectionSequenceSummaries");
        AssertReadOnlyListProperty(projection, "BoundarySupersessions");
        AssertReadOnlyListProperty(projection, "Checkpoints");
        AssertReadOnlyListProperty(projection, "Supersessions");
    }

    [Fact]
    public void ContractsRemainSynchronousAndSideEffectFree()
    {
        var flow = typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceFlow);
        foreach (var method in flow.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            var returnTypeName = method.ReturnType.FullName ?? string.Empty;
            Assert.False(returnTypeName.Contains("Task", StringComparison.Ordinal));
            Assert.False(returnTypeName.Contains("ValueTask", StringComparison.Ordinal));
        }
    }

    private static Type[] ContractTypes() =>
    [
        typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjection<,,>),
        typeof(
            HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidation<,,>),
        typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionResult<,,>),
        typeof(
            HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidationResult<,,>)
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
