using System.Reflection;
using AI.Sandbox.Engine.Core.HostRuntime;
using Xunit;

namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionContinuityTests
{
    [Fact]
    public void PublicContractsAreAvailable()
    {
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionIdKind).IsClass);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidationIdKind).IsClass);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus).IsEnum);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjection<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidation<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionResult<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidationResult<,,>).IsPublic);
        Assert.True(typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceFlow).IsAbstract);
    }

    [Fact]
    public void ProjectionStatusDefinesExpectedOutcomes()
    {
        var names = Enum.GetNames<HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus>();
        Assert.Equal(16, names.Length);
        Assert.Contains("AdjacentMultiCollectionSequenceSequenceProjected", names);
        Assert.Contains("MultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidated", names);
        Assert.Contains("StaleSelectionRevision", names);
        Assert.Contains("AdjacentMultiCollectionSequenceSequenceProjectionTickRegressed", names);
        Assert.Contains("SelectionMultiCollectionSequenceSequenceSummaryMismatch", names);
        Assert.Contains("SelectionBoundarySupersessionMismatch", names);
        Assert.Contains("SelectionCheckpointMismatch", names);
        Assert.Contains("SelectionSupersessionMismatch", names);
        Assert.Contains("StaleRangeSummaryRevision", names);
        Assert.Contains("StaleAdjacentMultiCollectionSequenceSequenceRevision", names);
        Assert.Contains("ContinuityValidationTickRegressed", names);
        Assert.Contains("RangeSummaryMismatch", names);
        Assert.Contains("MultiCollectionSequenceSequenceRangeNotAdjacent", names);
        Assert.Contains("CheckpointRangeNotAdjacent", names);
        Assert.Contains("SupersessionBoundaryMismatch", names);
        Assert.Contains("CheckpointBoundaryMismatch", names);
    }

    [Fact]
    public void AdjacentProjectionPreservesExactMultiCollectionSequenceAuthorityShape()
    {
        var type = typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjection<,,>);
        AssertReadableProperties(type,
            "ProjectionId", "Selection", "Summary", "SourceSequence",
            "SourceCollection", "SourceProjection", "Chain",
            "SelectsPreviousMultiCollectionSequenceSequence", "SelectsNextMultiCollectionSequenceSequence",
            "MultiCollectionSequenceSequenceSummaries", "BoundarySupersessions",
            "Checkpoints", "Supersessions", "AdjacentBoundarySupersession",
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
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidation<,,>);
        AssertReadableProperties(type,
            "ValidationId", "Summary", "AdjacentMultiCollectionSequenceSequence", "Selection",
            "ValidatesPreviousMultiCollectionSequenceSequence", "ValidatesNextMultiCollectionSequenceSequence",
            "ConnectingSupersession", "PriorCheckpoint", "SuccessorCheckpoint",
            "ValidatedTick", "Revision");
    }

    [Fact]
    public void FlowExposesProjectionAndContinuityMethods()
    {
        var type = typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceFlow);
        AssertGenericMethod(type, "ProjectMultiCollectionSequenceSequence", 4);
        AssertGenericMethod(type, "ValidateContinuity", 6);
    }

    [Fact]
    public void ResultContractsPreserveSourceAuthorities()
    {
        AssertReadableProperties(
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionResult<,,>),
            "Status", "Selection", "Projection", "Succeeded");
        AssertReadableProperties(
            typeof(
                HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidationResult<,,>),
            "Status", "Summary", "AdjacentMultiCollectionSequenceSequence", "Validation", "Succeeded");
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
            typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjection<,,>);
        AssertReadOnlyListProperty(projection, "MultiCollectionSequenceSequenceSummaries");
        AssertReadOnlyListProperty(projection, "BoundarySupersessions");
        AssertReadOnlyListProperty(projection, "Checkpoints");
        AssertReadOnlyListProperty(projection, "Supersessions");
    }

    [Fact]
    public void ContractsRemainSynchronousAndSideEffectFree()
    {
        var flow = typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceFlow);
        foreach (var method in flow.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            var returnTypeName = method.ReturnType.FullName ?? string.Empty;
            Assert.False(returnTypeName.Contains("Task", StringComparison.Ordinal));
            Assert.False(returnTypeName.Contains("ValueTask", StringComparison.Ordinal));
        }
    }

    private static Type[] ContractTypes() =>
    [
        typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjection<,,>),
        typeof(
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidation<,,>),
        typeof(HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionResult<,,>),
        typeof(
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidationResult<,,>)
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
