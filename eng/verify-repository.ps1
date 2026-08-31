[CmdletBinding()]
param()

. (Join-Path $PSScriptRoot 'common.ps1')

$root = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $root 'AI.Sandbox.Engine.slnx'
$coreProjectPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\AI.Sandbox.Engine.Core.csproj'
$testProjectPath = Join-Path $root 'tests\AI.Sandbox.Engine.Core.Tests\AI.Sandbox.Engine.Core.Tests.csproj'
$packagesPath = Join-Path $root 'Directory.Packages.props'
$idSourcePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Identifiers\Id.cs'
$eventEnvelopePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Events\EventEnvelope.cs'
$eventDispatcherPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Events\EventDispatcher.cs'
$eventBuilderPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Events\EventDispatcherBuilder.cs'
$eventHandlerPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Events\IEventHandler.cs'
$worldStateInterfacePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\WorldState\IWorldState.cs'
$worldStateSnapshotPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\WorldState\WorldStateSnapshot.cs'
$worldStateDecisionPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\WorldState\WorldStateTransitionDecision.cs'
$worldStateManagerPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\WorldState\WorldStateManager.cs'
$entityRegistryPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Entities\EntityRegistry.cs'
$entityMutationResultPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Entities\EntityMutationResult.cs'
$componentInterfacePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Components\IComponent.cs'
$componentRegistryPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Components\ComponentRegistry.cs'
$componentBuilderPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Components\ComponentRegistryBuilder.cs'
$componentStorePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Components\ComponentStore.cs'
$simulationSystemInterfacePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Simulation\ISimulationSystem.cs'
$simulationBuilderPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Simulation\SimulationSchedulerBuilder.cs'
$simulationSchedulerPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Simulation\SimulationScheduler.cs'
$simulationTransitionPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Simulation\SimulationTickTransition.cs'
$coreAssemblyTestsPath = Join-Path $root 'tests\AI.Sandbox.Engine.Core.Tests\CoreAssemblyTests.cs'
$persistenceCodecPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Persistence\IWorldStateSnapshotCodec.cs'
$persistenceServicePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Persistence\WorldStateSnapshotPersistence.cs'
$snapshotDocumentPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Persistence\WorldSnapshotDocument.cs'
$snapshotPayloadPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Persistence\SnapshotPayload.cs'
$snapshotChecksumPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Persistence\SnapshotChecksum.cs'
$foundationValidationScriptPath = Join-Path $root 'eng\validate-foundation.ps1'
$foundationValidationTestsPath = Join-Path $root 'tests\AI.Sandbox.Engine.Core.Tests\FoundationValidationTests.cs'
$foundationProbeProjectPath = Join-Path $root 'samples\AI.Sandbox.Engine.FoundationProbe\AI.Sandbox.Engine.FoundationProbe.csproj'
$foundationProbeProgramPath = Join-Path $root 'samples\AI.Sandbox.Engine.FoundationProbe\Program.cs'
$commandInterfacePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Commands\IEngineCommand.cs'
$commandEnvelopePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Commands\CommandEnvelope.cs'
$commandHandlerPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Commands\ICommandHandler.cs'
$commandProcessorPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Commands\CommandProcessor.cs'
$commandTransitionPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Commands\CommandTransition.cs'
$randomStatePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Randomness\DeterministicRandomState.cs'
$randomSeedPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Randomness\RandomSeed.cs'
$randomVersionPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Randomness\RandomAlgorithmVersion.cs'
$simulationDurationPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Time\SimulationDuration.cs'
$simulationInstantPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Time\SimulationInstant.cs'
$simulationTickDurationPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Time\SimulationTickDuration.cs'
$simulationTimelinePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Time\SimulationTimeline.cs'
$runtimeCommitFactPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Runtime\RuntimeCommitFact.cs'
$runtimeBuilderPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Runtime\RuntimeOrchestratorBuilder.cs'
$runtimeOrchestratorPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Runtime\RuntimeOrchestrator.cs'
$runtimeCommandResultPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Runtime\RuntimeCommandResult.cs'
$runtimeTickResultPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Runtime\RuntimeTickResult.cs'
$spatialDistancePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Spatial\SpatialDistance.cs'
$spatialPointPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Spatial\SpatialPoint.cs'
$spatialPositionPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Spatial\SpatialPosition.cs'
$spatialTopologyPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Spatial\SpatialTopology.cs'
$spatialBuilderPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Spatial\SpatialTopologyBuilder.cs'
$perceptionStimulusEnvelopePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Perception\PerceptionStimulusEnvelope.cs'
$perceptionDecisionPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Perception\PerceptionDecision.cs'
$perceptionEvaluatorPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Perception\IPerceptionEvaluator.cs'
$perceptionObservationPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Perception\PerceptionObservation.cs'
$perceptionProcessorPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Perception\PerceptionProcessor.cs'
$perceptionBuilderPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Perception\PerceptionProcessorBuilder.cs'
$knowledgeEvidencePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Knowledge\KnowledgeEvidenceReference.cs'
$knowledgeEntryPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Knowledge\KnowledgeEntry.cs'
$knowledgeSetPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Knowledge\KnowledgeSet.cs'
$knowledgeMutationResultPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Knowledge\KnowledgeMutationResult.cs'
$memoryOriginPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Memory\MemoryOriginReference.cs'
$memoryEntryPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Memory\MemoryEntry.cs'
$memoryStorePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Memory\MemoryStore.cs'
$memoryRecallQueryPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Memory\MemoryRecallQuery.cs'
$memoryMutationResultPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Memory\MemoryMutationResult.cs'
$relationshipChangePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Relationships\RelationshipChangeReference.cs'
$relationshipEntryPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Relationships\RelationshipEntry.cs'
$relationshipSetPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Relationships\RelationshipSet.cs'
$relationshipMutationResultPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Relationships\RelationshipMutationResult.cs'
$behaviorIntentEnvelopePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Behavior\BehaviorIntentEnvelope.cs'
$actionProposalEnvelopePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Behavior\ActionProposalEnvelope.cs'
$actionDecisionPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Behavior\ActionDecision.cs'
$actionValidationProcessorPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Behavior\ActionValidationProcessor.cs'
$contextQueryEnvelopePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextQueryEnvelope.cs'
$contextItemEnvelopePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextItemEnvelope.cs'
$contextRetrievalDecisionPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextRetrievalDecision.cs'
$contextRetrievalProcessorPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextRetrievalProcessor.cs'
$promptRequestEnvelopePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Prompting\PromptRequestEnvelope.cs'
$promptCandidateEnvelopePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Prompting\PromptCandidateEnvelope.cs'
$promptBudgetManagerPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Prompting\PromptBudgetManager.cs'
$promptCompositionDecisionPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Prompting\PromptCompositionDecision.cs'
$promptCompositionProcessorPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Prompting\PromptCompositionProcessor.cs'
$modelInvocationRequestPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Modeling\ModelInvocationRequestEnvelope.cs'
$modelInvocationResponsePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Modeling\ModelInvocationResponseEnvelope.cs'
$modelInvocationDecisionPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Modeling\ModelInvocationDecision.cs'
$modelAdapterPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Modeling\IModelAdapter.cs'
$modelInvocationProcessorPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Modeling\ModelInvocationProcessor.cs'
$speechInvocationRequestPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Speech\SpeechInvocationRequestEnvelope.cs'
$speechInvocationResponsePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Speech\SpeechInvocationResponseEnvelope.cs'
$speechInvocationDecisionPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Speech\SpeechInvocationDecision.cs'
$speechAdapterPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Speech\ISpeechAdapter.cs'
$speechInvocationProcessorPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Speech\SpeechInvocationProcessor.cs'
$conversationStatePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Conversation\ConversationState.cs'
$addressResolutionRequestPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Conversation\AddressResolutionRequestEnvelope.cs'
$addressResolverPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Conversation\IAddressResolver.cs'
$addressResolutionProcessorPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Conversation\AddressResolutionProcessor.cs'
$socialTurnRequestPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Social\SocialTurnCoordinationRequestEnvelope.cs'
$socialTurnCoordinatorPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Social\ISocialTurnCoordinator.cs'
$socialTurnProcessorPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Social\SocialTurnCoordinationProcessor.cs'
$structuredOutputRequestPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\StructuredOutput\StructuredOutputRequestEnvelope.cs'
$structuredOutputEnvelopePath = Join-Path $root 'src\AI.Sandbox.Engine.Core\StructuredOutput\StructuredModelOutputEnvelope.cs'
$structuredOutputDecoderPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\StructuredOutput\IStructuredOutputDecoder.cs'
$structuredOutputProcessorPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\StructuredOutput\StructuredOutputProcessor.cs'
$dialogueRequestPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Dialogue\DialogueOrchestrationRequestEnvelope.cs'
$dialogueArtifactPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Dialogue\DialogueArtifactEnvelope.cs'
$dialogueOrchestratorPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Dialogue\IDialogueOrchestrator.cs'
$dialogueProcessorPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Dialogue\DialogueOrchestrationProcessor.cs'

$requiredFiles = @(
    '.editorconfig',
    '.gitattributes',
    '.gitignore',
    'AI.Sandbox.Engine.slnx',
    'Directory.Build.props',
    'Directory.Packages.props',
    'global.json',
    'README.md',
    'docs\constitution\PROJECT_CONSTITUTION.md',
    'docs\product\NEUROGAME_VISION_RU.md',
    'docs\architecture\ADR-0001-runtime-platform.md',
    'docs\architecture\ADR-0002-core-library-boundary.md',
    'docs\architecture\ADR-0003-typed-identifiers.md',
    'docs\architecture\ADR-0004-event-system.md',
    'docs\architecture\ADR-0005-world-state.md',
    'docs\architecture\ADR-0006-entity-system.md',
    'docs\architecture\ADR-0007-component-system.md',
    'docs\architecture\ADR-0008-simulation-scheduler.md',
    'docs\architecture\ADR-0009-persistence.md',
    'docs\architecture\ADR-0010-foundation-validation.md',
    'docs\architecture\ADR-0011-command-system.md',
    'docs\architecture\ADR-0012-deterministic-randomness.md',
    'docs\architecture\ADR-0013-simulation-time-model.md',
    'docs\architecture\ADR-0014-runtime-orchestrator.md',
    'docs\architecture\ADR-0015-spatial-model.md',
    'docs\architecture\ADR-0016-perception-model.md',
    'docs\architecture\ADR-0017-knowledge-model.md',
    'docs\architecture\ADR-0018-memory-model.md',
    'docs\architecture\ADR-0019-relationship-model.md',
    'docs\architecture\ADR-0020-behavior-action-validation.md',
    'docs\architecture\ADR-0021-context-retrieval.md',
    'docs\architecture\ADR-0022-prompt-composer-context-budget.md',
    'docs\architecture\ADR-0023-provider-neutral-model-contracts.md',
    'docs\architecture\ADR-0024-provider-neutral-speech-contracts.md',
    'docs\architecture\ADR-0025-conversation-state-address-resolution.md',
    'docs\architecture\ADR-0026-social-turn-taking.md',
    'docs\architecture\ADR-0027-structured-model-output.md',
    'docs\architecture\ADR-0028-dialogue-orchestration.md',
    'docs\roadmap\FOUNDATION_COMMITS.md',
    'docs\roadmap\UNREAL_DEVELOPMENT_PLAN.md',
    'docs\validation\FOUNDATION_VALIDATION_0010.md',
    'docs\status\CURRENT.md',
    'eng\common.ps1',
    'eng\validate-foundation.ps1',
    'samples\AI.Sandbox.Engine.FoundationProbe\AI.Sandbox.Engine.FoundationProbe.csproj',
    'samples\AI.Sandbox.Engine.FoundationProbe\Program.cs',
    'samples\AI.Sandbox.Engine.FoundationProbe\packages.lock.json',
    'src\AI.Sandbox.Engine.Core\AI.Sandbox.Engine.Core.csproj',
    'src\AI.Sandbox.Engine.Core\CoreAssemblyMarker.cs',
    'src\AI.Sandbox.Engine.Core\Events\EventDispatcher.cs',
    'src\AI.Sandbox.Engine.Core\Events\EventDispatcherBuilder.cs',
    'src\AI.Sandbox.Engine.Core\Events\EventEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\Events\EventIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Events\IEngineEvent.cs',
    'src\AI.Sandbox.Engine.Core\Events\IEventHandler.cs',
    'src\AI.Sandbox.Engine.Core\Commands\CommandContext.cs',
    'src\AI.Sandbox.Engine.Core\Commands\CommandDecision.cs',
    'src\AI.Sandbox.Engine.Core\Commands\CommandDecisionStatus.cs',
    'src\AI.Sandbox.Engine.Core\Commands\CommandEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\Commands\CommandExecutionResult.cs',
    'src\AI.Sandbox.Engine.Core\Commands\CommandExecutionStatus.cs',
    'src\AI.Sandbox.Engine.Core\Commands\CommandIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Commands\CommandProcessor.cs',
    'src\AI.Sandbox.Engine.Core\Commands\CommandProcessorBuilder.cs',
    'src\AI.Sandbox.Engine.Core\Commands\CommandTransition.cs',
    'src\AI.Sandbox.Engine.Core\Commands\CommandTypePolicy.cs',
    'src\AI.Sandbox.Engine.Core\Commands\ICommandHandler.cs',
    'src\AI.Sandbox.Engine.Core\Commands\IEngineCommand.cs',
    'src\AI.Sandbox.Engine.Core\Components\ComponentMutationResult.cs',
    'src\AI.Sandbox.Engine.Core\Components\ComponentMutationStatus.cs',
    'src\AI.Sandbox.Engine.Core\Components\ComponentPurgeResult.cs',
    'src\AI.Sandbox.Engine.Core\Components\ComponentRegistry.cs',
    'src\AI.Sandbox.Engine.Core\Components\ComponentRegistryBuilder.cs',
    'src\AI.Sandbox.Engine.Core\Components\ComponentStore.cs',
    'src\AI.Sandbox.Engine.Core\Components\ComponentStoreBuilder.cs',
    'src\AI.Sandbox.Engine.Core\Components\ComponentTypePolicy.cs',
    'src\AI.Sandbox.Engine.Core\Components\IComponent.cs',
    'src\AI.Sandbox.Engine.Core\Components\IComponentStore.cs',
    'src\AI.Sandbox.Engine.Core\Components\IComponentStoreBuilder.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\IDialogueInput.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\IDialogueArtifact.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\IDialogueDirective.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\IDialogueCompletion.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\DialogueExchangeIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\DialogueOrchestrationIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\DialogueOrchestratorIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\DialogueArtifactIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\DialogueArtifactSourceIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\DialogueArtifactSequence.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\DialogueRejectionCode.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\DialogueTypePolicy.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\DialogueArtifactEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\DialogueOrchestrationRequestEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\DialogueOrchestrationContext.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\DialogueOrchestrationDecisionStatus.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\DialogueOrchestrationDecision.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\IDialogueOrchestrator.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\DialogueOrchestrationStatus.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\DialogueOrchestrationResult.cs',
    'src\AI.Sandbox.Engine.Core\Dialogue\DialogueOrchestrationProcessor.cs',
    'src\AI.Sandbox.Engine.Core\Entities\EntityIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Entities\EntityLifecycleStatus.cs',
    'src\AI.Sandbox.Engine.Core\Entities\EntityMutationResult.cs',
    'src\AI.Sandbox.Engine.Core\Entities\EntityMutationStatus.cs',
    'src\AI.Sandbox.Engine.Core\Entities\EntityRegistry.cs',
    'src\AI.Sandbox.Engine.Core\Identifiers\Id.cs',
    'src\AI.Sandbox.Engine.Core\Knowledge\IKnowledgeClaim.cs',
    'src\AI.Sandbox.Engine.Core\Knowledge\KnowledgeClaimIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Knowledge\KnowledgeConfidence.cs',
    'src\AI.Sandbox.Engine.Core\Knowledge\KnowledgeEntry.cs',
    'src\AI.Sandbox.Engine.Core\Knowledge\KnowledgeEvidenceIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Knowledge\KnowledgeEvidenceKind.cs',
    'src\AI.Sandbox.Engine.Core\Knowledge\KnowledgeEvidenceReference.cs',
    'src\AI.Sandbox.Engine.Core\Knowledge\KnowledgeMutationResult.cs',
    'src\AI.Sandbox.Engine.Core\Knowledge\KnowledgeMutationStatus.cs',
    'src\AI.Sandbox.Engine.Core\Knowledge\KnowledgeSet.cs',
    'src\AI.Sandbox.Engine.Core\Knowledge\KnowledgeTypePolicy.cs',
    'src\AI.Sandbox.Engine.Core\Memory\IMemoryContent.cs',
    'src\AI.Sandbox.Engine.Core\Memory\MemoryEntry.cs',
    'src\AI.Sandbox.Engine.Core\Memory\MemoryIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Memory\MemoryMutationResult.cs',
    'src\AI.Sandbox.Engine.Core\Memory\MemoryMutationStatus.cs',
    'src\AI.Sandbox.Engine.Core\Memory\MemoryOriginIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Memory\MemoryOriginKind.cs',
    'src\AI.Sandbox.Engine.Core\Memory\MemoryOriginReference.cs',
    'src\AI.Sandbox.Engine.Core\Memory\MemoryRecallQuery.cs',
    'src\AI.Sandbox.Engine.Core\Memory\MemoryRecallResult.cs',
    'src\AI.Sandbox.Engine.Core\Memory\MemorySalience.cs',
    'src\AI.Sandbox.Engine.Core\Memory\MemoryStore.cs',
    'src\AI.Sandbox.Engine.Core\Memory\MemoryStrength.cs',
    'src\AI.Sandbox.Engine.Core\Memory\MemoryTypePolicy.cs',
    'src\AI.Sandbox.Engine.Core\Behavior\IBehaviorIntent.cs',
    'src\AI.Sandbox.Engine.Core\Behavior\IActionProposal.cs',
    'src\AI.Sandbox.Engine.Core\Behavior\BehaviorIntentIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Behavior\ActionProposalIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Behavior\BehaviorTypePolicy.cs',
    'src\AI.Sandbox.Engine.Core\Behavior\BehaviorIntentEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\Behavior\ActionProposalEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\Behavior\ActionRejectionCode.cs',
    'src\AI.Sandbox.Engine.Core\Behavior\ActionDecisionStatus.cs',
    'src\AI.Sandbox.Engine.Core\Behavior\ActionDecision.cs',
    'src\AI.Sandbox.Engine.Core\Behavior\ActionValidationContext.cs',
    'src\AI.Sandbox.Engine.Core\Behavior\IActionValidator.cs',
    'src\AI.Sandbox.Engine.Core\Behavior\ActionValidationStatus.cs',
    'src\AI.Sandbox.Engine.Core\Behavior\ActionValidationResult.cs',
    'src\AI.Sandbox.Engine.Core\Behavior\ActionValidationProcessor.cs',
    'src\AI.Sandbox.Engine.Core\ContextRetrieval\IContextQuery.cs',
    'src\AI.Sandbox.Engine.Core\ContextRetrieval\IContextItem.cs',
    'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextQueryIdKind.cs',
    'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextRetrieverIdKind.cs',
    'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextItemIdKind.cs',
    'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextItemLimit.cs',
    'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextRelevance.cs',
    'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextRejectionCode.cs',
    'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextTypePolicy.cs',
    'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextQueryEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\IConversationTopic.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\IAddressQuery.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\ConversationIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\AddressResolutionIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\AddressResolverIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\ConversationRevision.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\ConversationTurnNumber.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\AddressAudienceKind.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\AddressResolutionConfidence.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\AddressRejectionCode.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\ConversationTypePolicy.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\AddressAudience.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\ConversationTurn.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\ConversationMutationStatus.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\ConversationMutationResult.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\ConversationState.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\AddressResolutionRequestEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\AddressResolutionContext.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\AddressResolutionDecisionStatus.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\AddressResolutionDecision.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\IAddressResolver.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\AddressResolutionStatus.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\AddressResolutionResult.cs',
    'src\AI.Sandbox.Engine.Core\Conversation\AddressResolutionProcessor.cs',
    'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextItemEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextRetrievalContext.cs',
    'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextRetrievalDecisionStatus.cs',
    'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextRetrievalDecision.cs',
    'src\AI.Sandbox.Engine.Core\ContextRetrieval\IContextRetriever.cs',
    'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextRetrievalStatus.cs',
    'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextRetrievalResult.cs',
    'src\AI.Sandbox.Engine.Core\ContextRetrieval\ContextRetrievalProcessor.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\IModelRequest.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\IModelResponse.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\ModelInvocationIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\ModelAdapterIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\ModelProfileIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\ModelOutputLimit.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\ModelUsage.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\ModelRejectionCode.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\ModelFailureCode.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\ModelTypePolicy.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\ModelInvocationRequestEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\ModelInvocationResponseEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\ModelInvocationContext.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\ModelInvocationDecisionStatus.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\ModelInvocationDecision.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\IModelAdapter.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\ModelInvocationStatus.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\ModelInvocationResult.cs',
    'src\AI.Sandbox.Engine.Core\Modeling\ModelInvocationProcessor.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\IPromptRequest.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\IPromptContent.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\IPromptDocument.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptRequestIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptComposerIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptCandidateIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptDocumentIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptBudget.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptCost.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptPriority.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptInclusionMode.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptRejectionCode.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptTypePolicy.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptRequestEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptCandidateEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptBudgetStatus.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptBudgetResult.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptBudgetManager.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptDocumentEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptCompositionContext.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptCompositionDecisionStatus.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptCompositionDecision.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\IPromptComposer.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptCompositionStatus.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptCompositionResult.cs',
    'src\AI.Sandbox.Engine.Core\Prompting\PromptCompositionProcessor.cs',
    'src\AI.Sandbox.Engine.Core\Relationships\IRelationshipState.cs',
    'src\AI.Sandbox.Engine.Core\Relationships\RelationshipChangeIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Relationships\RelationshipChangeKind.cs',
    'src\AI.Sandbox.Engine.Core\Relationships\RelationshipChangeReference.cs',
    'src\AI.Sandbox.Engine.Core\Relationships\RelationshipEntry.cs',
    'src\AI.Sandbox.Engine.Core\Relationships\RelationshipMutationResult.cs',
    'src\AI.Sandbox.Engine.Core\Relationships\RelationshipMutationStatus.cs',
    'src\AI.Sandbox.Engine.Core\Relationships\RelationshipSet.cs',
    'src\AI.Sandbox.Engine.Core\Relationships\RelationshipTypePolicy.cs',
    'src\AI.Sandbox.Engine.Core\Perception\IPerceptionEvaluator.cs',
    'src\AI.Sandbox.Engine.Core\Perception\IPerceptionSignal.cs',
    'src\AI.Sandbox.Engine.Core\Perception\IPerceptionStimulus.cs',
    'src\AI.Sandbox.Engine.Core\Perception\PerceptionChannelIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Perception\PerceptionConfidence.cs',
    'src\AI.Sandbox.Engine.Core\Perception\PerceptionContext.cs',
    'src\AI.Sandbox.Engine.Core\Perception\PerceptionDecision.cs',
    'src\AI.Sandbox.Engine.Core\Perception\PerceptionDecisionStatus.cs',
    'src\AI.Sandbox.Engine.Core\Perception\PerceptionEvaluationResult.cs',
    'src\AI.Sandbox.Engine.Core\Perception\PerceptionEvaluationStatus.cs',
    'src\AI.Sandbox.Engine.Core\Perception\PerceptionEvaluatorKey.cs',
    'src\AI.Sandbox.Engine.Core\Perception\PerceptionObservation.cs',
    'src\AI.Sandbox.Engine.Core\Perception\PerceptionProcessor.cs',
    'src\AI.Sandbox.Engine.Core\Perception\PerceptionProcessorBuilder.cs',
    'src\AI.Sandbox.Engine.Core\Perception\PerceptionStimulusEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\Perception\PerceptionStimulusIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Perception\PerceptionTypePolicy.cs',
    'src\AI.Sandbox.Engine.Core\Persistence\IWorldStateSnapshotCodec.cs',
    'src\AI.Sandbox.Engine.Core\Persistence\PersistenceSchemaId.cs',
    'src\AI.Sandbox.Engine.Core\Persistence\PersistenceSchemaVersion.cs',
    'src\AI.Sandbox.Engine.Core\Persistence\SnapshotChecksum.cs',
    'src\AI.Sandbox.Engine.Core\Persistence\SnapshotFormatVersion.cs',
    'src\AI.Sandbox.Engine.Core\Persistence\SnapshotPayload.cs',
    'src\AI.Sandbox.Engine.Core\Persistence\SnapshotRestoreResult.cs',
    'src\AI.Sandbox.Engine.Core\Persistence\SnapshotRestoreStatus.cs',
    'src\AI.Sandbox.Engine.Core\Persistence\WorldSnapshotDocument.cs',
    'src\AI.Sandbox.Engine.Core\Persistence\WorldStateDecodeDecision.cs',
    'src\AI.Sandbox.Engine.Core\Persistence\WorldStateSnapshotPersistence.cs',
    'src\AI.Sandbox.Engine.Core\Randomness\DeterministicRandomState.cs',
    'src\AI.Sandbox.Engine.Core\Randomness\RandomAlgorithmVersion.cs',
    'src\AI.Sandbox.Engine.Core\Randomness\RandomDraw.cs',
    'src\AI.Sandbox.Engine.Core\Randomness\RandomSeed.cs',
    'src\AI.Sandbox.Engine.Core\Randomness\RandomStreamIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Runtime\RuntimeCommandResult.cs',
    'src\AI.Sandbox.Engine.Core\Runtime\RuntimeCommitFact.cs',
    'src\AI.Sandbox.Engine.Core\Runtime\RuntimeCommitKind.cs',
    'src\AI.Sandbox.Engine.Core\Runtime\RuntimeInvocationStatus.cs',
    'src\AI.Sandbox.Engine.Core\Runtime\RuntimeOrchestrator.cs',
    'src\AI.Sandbox.Engine.Core\Runtime\RuntimeOrchestratorBuilder.cs',
    'src\AI.Sandbox.Engine.Core\Runtime\RuntimeTickResult.cs',
    'src\AI.Sandbox.Engine.Core\Simulation\ISimulationSystem.cs',
    'src\AI.Sandbox.Engine.Core\Simulation\SimulationScheduler.cs',
    'src\AI.Sandbox.Engine.Core\Simulation\SimulationSchedulerBuilder.cs',
    'src\AI.Sandbox.Engine.Core\Simulation\SimulationStepResult.cs',
    'src\AI.Sandbox.Engine.Core\Simulation\SimulationStepStatus.cs',
    'src\AI.Sandbox.Engine.Core\Simulation\SimulationSystemContext.cs',
    'src\AI.Sandbox.Engine.Core\Simulation\SimulationSystemDecision.cs',
    'src\AI.Sandbox.Engine.Core\Simulation\SimulationSystemIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Simulation\SimulationSystemRegistration.cs',
    'src\AI.Sandbox.Engine.Core\Simulation\SimulationSystemStatus.cs',
    'src\AI.Sandbox.Engine.Core\Simulation\SimulationTickTransition.cs',
    'src\AI.Sandbox.Engine.Core\Properties\AssemblyInfo.cs',
    'src\AI.Sandbox.Engine.Core\Social\ISocialTurnProposal.cs',
    'src\AI.Sandbox.Engine.Core\Social\SocialTurnCoordinationIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Social\SocialTurnCoordinatorIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Social\SocialTurnProposalIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Social\SocialTurnRequestKind.cs',
    'src\AI.Sandbox.Engine.Core\Social\SocialTurnPriority.cs',
    'src\AI.Sandbox.Engine.Core\Social\SocialTurnNoTurnCode.cs',
    'src\AI.Sandbox.Engine.Core\Social\SocialTurnRejectionCode.cs',
    'src\AI.Sandbox.Engine.Core\Social\SocialTurnTypePolicy.cs',
    'src\AI.Sandbox.Engine.Core\Social\SocialTurnProposalEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\Social\SocialTurnCoordinationRequestEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\Social\SocialTurnCoordinationContext.cs',
    'src\AI.Sandbox.Engine.Core\Social\SocialTurnCoordinationDecisionStatus.cs',
    'src\AI.Sandbox.Engine.Core\Social\SocialTurnCoordinationDecision.cs',
    'src\AI.Sandbox.Engine.Core\Social\ISocialTurnCoordinator.cs',
    'src\AI.Sandbox.Engine.Core\Social\SocialTurnCoordinationStatus.cs',
    'src\AI.Sandbox.Engine.Core\Social\SocialTurnCoordinationResult.cs',
    'src\AI.Sandbox.Engine.Core\Social\SocialTurnCoordinationProcessor.cs',
    'src\AI.Sandbox.Engine.Core\Spatial\SpatialConnection.cs',
    'src\AI.Sandbox.Engine.Core\Spatial\SpatialDistance.cs',
    'src\AI.Sandbox.Engine.Core\Spatial\SpatialPlace.cs',
    'src\AI.Sandbox.Engine.Core\Spatial\SpatialPlaceIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Spatial\SpatialPoint.cs',
    'src\AI.Sandbox.Engine.Core\Spatial\SpatialPosition.cs',
    'src\AI.Sandbox.Engine.Core\Spatial\SpatialTopology.cs',
    'src\AI.Sandbox.Engine.Core\Spatial\SpatialTopologyBuilder.cs',
    'src\AI.Sandbox.Engine.Core\Speech\ISpeechRequest.cs',
    'src\AI.Sandbox.Engine.Core\Speech\ISpeechResponse.cs',
    'src\AI.Sandbox.Engine.Core\Speech\SpeechOperationKind.cs',
    'src\AI.Sandbox.Engine.Core\Speech\SpeechInvocationIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Speech\SpeechAdapterIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Speech\SpeechProfileIdKind.cs',
    'src\AI.Sandbox.Engine.Core\Speech\SpeechInputLimit.cs',
    'src\AI.Sandbox.Engine.Core\Speech\SpeechOutputLimit.cs',
    'src\AI.Sandbox.Engine.Core\Speech\SpeechUsage.cs',
    'src\AI.Sandbox.Engine.Core\Speech\SpeechRejectionCode.cs',
    'src\AI.Sandbox.Engine.Core\Speech\SpeechFailureCode.cs',
    'src\AI.Sandbox.Engine.Core\Speech\SpeechTypePolicy.cs',
    'src\AI.Sandbox.Engine.Core\Speech\SpeechInvocationRequestEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\Speech\SpeechInvocationResponseEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\Speech\SpeechInvocationContext.cs',
    'src\AI.Sandbox.Engine.Core\Speech\SpeechInvocationDecisionStatus.cs',
    'src\AI.Sandbox.Engine.Core\Speech\SpeechInvocationDecision.cs',
    'src\AI.Sandbox.Engine.Core\Speech\ISpeechAdapter.cs',
    'src\AI.Sandbox.Engine.Core\Speech\SpeechInvocationStatus.cs',
    'src\AI.Sandbox.Engine.Core\Speech\SpeechInvocationResult.cs',
    'src\AI.Sandbox.Engine.Core\Speech\SpeechInvocationProcessor.cs',
    'src\AI.Sandbox.Engine.Core\StructuredOutput\IStructuredModelOutput.cs',
    'src\AI.Sandbox.Engine.Core\StructuredOutput\StructuredOutputIdKind.cs',
    'src\AI.Sandbox.Engine.Core\StructuredOutput\StructuredOutputDecoderIdKind.cs',
    'src\AI.Sandbox.Engine.Core\StructuredOutput\StructuredOutputSchemaIdKind.cs',
    'src\AI.Sandbox.Engine.Core\StructuredOutput\StructuredOutputSchemaVersion.cs',
    'src\AI.Sandbox.Engine.Core\StructuredOutput\StructuredOutputRejectionCode.cs',
    'src\AI.Sandbox.Engine.Core\StructuredOutput\StructuredOutputTypePolicy.cs',
    'src\AI.Sandbox.Engine.Core\StructuredOutput\StructuredOutputRequestEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\StructuredOutput\StructuredOutputContext.cs',
    'src\AI.Sandbox.Engine.Core\StructuredOutput\StructuredOutputDecisionStatus.cs',
    'src\AI.Sandbox.Engine.Core\StructuredOutput\StructuredOutputDecision.cs',
    'src\AI.Sandbox.Engine.Core\StructuredOutput\IStructuredOutputDecoder.cs',
    'src\AI.Sandbox.Engine.Core\StructuredOutput\StructuredModelOutputEnvelope.cs',
    'src\AI.Sandbox.Engine.Core\StructuredOutput\StructuredOutputProcessingStatus.cs',
    'src\AI.Sandbox.Engine.Core\StructuredOutput\StructuredOutputProcessingResult.cs',
    'src\AI.Sandbox.Engine.Core\StructuredOutput\StructuredOutputProcessor.cs',
    'src\AI.Sandbox.Engine.Core\Time\SimulationDuration.cs',
    'src\AI.Sandbox.Engine.Core\Time\SimulationInstant.cs',
    'src\AI.Sandbox.Engine.Core\Time\SimulationTickDuration.cs',
    'src\AI.Sandbox.Engine.Core\Time\SimulationTimeline.cs',
    'src\AI.Sandbox.Engine.Core\WorldState\IWorldState.cs',
    'src\AI.Sandbox.Engine.Core\WorldState\IWorldStateTransition.cs',
    'src\AI.Sandbox.Engine.Core\WorldState\WorldIdKind.cs',
    'src\AI.Sandbox.Engine.Core\WorldState\WorldStateApplyResult.cs',
    'src\AI.Sandbox.Engine.Core\WorldState\WorldStateApplyStatus.cs',
    'src\AI.Sandbox.Engine.Core\WorldState\WorldStateManager.cs',
    'src\AI.Sandbox.Engine.Core\WorldState\WorldStateSnapshot.cs',
    'src\AI.Sandbox.Engine.Core\WorldState\WorldStateTransitionDecision.cs',
    'src\AI.Sandbox.Engine.Core\WorldState\WorldStateVersion.cs',
    'src\AI.Sandbox.Engine.Core\packages.lock.json',
    'tests\AI.Sandbox.Engine.Core.Tests\AI.Sandbox.Engine.Core.Tests.csproj',
    'tests\AI.Sandbox.Engine.Core.Tests\CommandSystemTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\ComponentSystemTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\CoreAssemblyTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\DeterministicRandomnessTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\DialogueOrchestrationTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\EntitySystemTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\EventSystemTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\FoundationValidationTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\IdTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\KnowledgeModelTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\MemoryModelTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\BehaviorActionValidationTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\ContextRetrievalTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\PromptCompositionTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\ConversationAddressResolutionTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\ProviderNeutralModelTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\ProviderNeutralSpeechTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\RelationshipModelTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\PerceptionModelTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\PersistenceTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\RuntimeOrchestratorTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\SimulationSchedulerTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\SocialTurnTakingTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\StructuredModelOutputTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\SimulationTimeTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\SpatialModelTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\WorldStateTests.cs',
    'tests\AI.Sandbox.Engine.Core.Tests\packages.lock.json'
)

foreach ($relativePath in $requiredFiles) {
    $absolutePath = Join-Path $root $relativePath
    if (-not (Test-Path -LiteralPath $absolutePath -PathType Leaf)) {
        throw "Required file is missing: $relativePath"
    }
}

$globalJson = Get-Content -LiteralPath (Join-Path $root 'global.json') -Raw |
    ConvertFrom-Json
if ($globalJson.sdk.version -ne '10.0.100') {
    throw 'global.json must use 10.0.100 as the minimum SDK baseline.'
}
if ($globalJson.sdk.rollForward -ne 'latestFeature') {
    throw 'global.json must use latestFeature roll-forward within .NET 10.'
}
if ($globalJson.sdk.allowPrerelease -ne $false) {
    throw 'Prerelease .NET SDKs must remain disabled.'
}

[xml]$buildProps =
    Get-Content -LiteralPath (Join-Path $root 'Directory.Build.props') -Raw
[xml]$packagesProps = Get-Content -LiteralPath $packagesPath -Raw
[xml]$coreProject = Get-Content -LiteralPath $coreProjectPath -Raw
[xml]$testProject = Get-Content -LiteralPath $testProjectPath -Raw
[xml]$foundationProbeProject =
    Get-Content -LiteralPath $foundationProbeProjectPath -Raw

function Get-ProjectProperty {
    param(
        [Parameter(Mandatory)][xml]$Project,
        [Parameter(Mandatory)][string]$Name
    )

    $node = $Project.SelectSingleNode("/Project/PropertyGroup/$Name")
    if ($null -eq $node) {
        return $null
    }

    return $node.InnerText
}

if ((Get-ProjectProperty -Project $coreProject -Name 'AssemblyName') -ne
    'AI.Sandbox.Engine.Core') {
    throw 'Core AssemblyName policy is invalid.'
}
if ((Get-ProjectProperty -Project $coreProject -Name 'RootNamespace') -ne
    'AI.Sandbox.Engine.Core') {
    throw 'Core RootNamespace policy is invalid.'
}
if ((Get-ProjectProperty -Project $testProject -Name 'AssemblyName') -ne
    'AI.Sandbox.Engine.Core.Tests') {
    throw 'Test AssemblyName policy is invalid.'
}
if ((Get-ProjectProperty -Project $testProject -Name 'RootNamespace') -ne
    'AI.Sandbox.Engine.Core.Tests') {
    throw 'Test RootNamespace policy is invalid.'
}
if ((Get-ProjectProperty -Project $testProject -Name 'IsTestProject') -ne
    'true') {
    throw 'The test project must explicitly set IsTestProject=true.'
}

if ((Get-ProjectProperty -Project $foundationProbeProject -Name 'OutputType') -ne
    'Exe') {
    throw 'Foundation Probe must remain an executable sample.'
}
if ((Get-ProjectProperty -Project $foundationProbeProject -Name 'AssemblyName') -ne
    'AI.Sandbox.Engine.FoundationProbe') {
    throw 'Foundation Probe AssemblyName policy is invalid.'
}
if ((Get-ProjectProperty -Project $foundationProbeProject -Name 'RootNamespace') -ne
    'AI.Sandbox.Engine.FoundationProbe') {
    throw 'Foundation Probe RootNamespace policy is invalid.'
}

$probePackageReferences =
    @($foundationProbeProject.SelectNodes('//PackageReference'))
$probeProjectReferences =
    @($foundationProbeProject.SelectNodes('//ProjectReference'))
if ($probePackageReferences.Count -ne 0) {
    throw 'Foundation Probe must not add package dependencies.'
}
if ($probeProjectReferences.Count -ne 1) {
    throw 'Foundation Probe must reference exactly the Core project.'
}

$probeProjectDirectory = Split-Path -Parent $foundationProbeProjectPath
$declaredProbeReference = [System.IO.Path]::GetFullPath(
    (Join-Path -Path $probeProjectDirectory -ChildPath (
        $probeProjectReferences[0].GetAttribute('Include'))))
if ($declaredProbeReference -ne
    [System.IO.Path]::GetFullPath($coreProjectPath)) {
    throw 'Foundation Probe may reference only AI.Sandbox.Engine.Core.'
}

$testNoWarn = Get-ProjectProperty -Project $testProject -Name 'NoWarn'
if ([string]::IsNullOrWhiteSpace($testNoWarn) -or
    $testNoWarn -notmatch '(^|;)CS1591(;|$)') {
    throw 'The test project must suppress CS1591 locally.'
}

if ((Get-ProjectProperty -Project $buildProps -Name 'GenerateDocumentationFile') -ne 'true') {
    throw 'Directory.Build.props must generate XML documentation.'
}

$corePackageReferences = @($coreProject.SelectNodes('//PackageReference'))
$coreProjectReferences = @($coreProject.SelectNodes('//ProjectReference'))
if ($corePackageReferences.Count -ne 0 -or
    $coreProjectReferences.Count -ne 0) {
    throw 'AI.Sandbox.Engine.Core must remain dependency-free.'
}

$testProjectReferences = @($testProject.SelectNodes('//ProjectReference'))
if ($testProjectReferences.Count -ne 1) {
    throw 'The core test project must contain exactly one ProjectReference.'
}

$testProjectDirectory = Split-Path -Parent $testProjectPath
$resolvedReference = [System.IO.Path]::GetFullPath($coreProjectPath)
$declaredInclude = $testProjectReferences[0].GetAttribute('Include')
$declaredPath = Join-Path -Path $testProjectDirectory -ChildPath $declaredInclude
$declaredReference = [System.IO.Path]::GetFullPath($declaredPath)
if ($declaredReference -ne $resolvedReference) {
    throw "Unexpected test ProjectReference: $declaredInclude"
}

$centralVersions = @{}
foreach ($packageVersion in @($packagesProps.SelectNodes('//PackageVersion'))) {
    $centralVersions[$packageVersion.GetAttribute('Include')] =
        $packageVersion.GetAttribute('Version')
}

$testPackageReferences = @($testProject.SelectNodes('//PackageReference'))
if ($testPackageReferences.Count -eq 0) {
    throw 'The test project must reference a test framework.'
}

foreach ($packageReference in $testPackageReferences) {
    $name = $packageReference.GetAttribute('Include')

    if ($packageReference.HasAttribute('Version') -or
        $null -ne $packageReference.SelectSingleNode('Version')) {
        throw "Package '$name' declares a local version."
    }

    if (-not $centralVersions.ContainsKey($name) -or
        [string]::IsNullOrWhiteSpace($centralVersions[$name])) {
        throw "Package '$name' has no central PackageVersion."
    }
}

$idSource = Get-Content -LiteralPath $idSourcePath -Raw
if ($idSource -notmatch
    'public\s+readonly\s+record\s+struct\s+Id<TKind>') {
    throw 'The approved generic typed-ID declaration is missing.'
}
if ($idSource -notmatch 'private\s+Id\(Guid\s+value\)') {
    throw 'Typed IDs must not expose a public value constructor.'
}
if ($idSource -notmatch 'Guid\.TryParseExact') {
    throw 'Typed IDs must enforce canonical parsing.'
}
if ($idSource -match 'Guid\.(NewGuid|CreateVersion7)') {
    throw 'Typed IDs must not hide nondeterministic ID generation.'
}
if ($idSource -notmatch 'parsed\s+==\s+Guid\.Empty') {
    throw 'Typed IDs must reject Guid.Empty during parsing.'
}

$eventEnvelopeSource = Get-Content -LiteralPath $eventEnvelopePath -Raw
if ($eventEnvelopeSource -notmatch
    'public\s+readonly\s+record\s+struct\s+EventEnvelope<TEvent>') {
    throw 'The immutable generic event envelope is missing.'
}
if ($eventEnvelopeSource -notmatch
    'where\s+TEvent\s+:\s+IEngineEvent') {
    throw 'Event envelopes must accept only engine event payloads.'
}
if ($eventEnvelopeSource -notmatch 'Id<EventIdKind>\s+EventId') {
    throw 'Event envelopes must use a strongly typed event identifier.'
}
if ($eventEnvelopeSource -notmatch 'ulong\s+Sequence') {
    throw 'Event envelopes must carry an authoritative sequence.'
}
if ($eventEnvelopeSource -notmatch 'ulong\s+SimulationTick') {
    throw 'Event envelopes must carry an authoritative simulation tick.'
}
if ($eventEnvelopeSource -match
    'DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7)') {
    throw 'Event envelopes must not read time or generate identifiers.'
}

$eventHandlerSource = Get-Content -LiteralPath $eventHandlerPath -Raw
if ($eventHandlerSource -notmatch
    'interface\s+IEventHandler<TEvent>') {
    throw 'The event handler contract must remain invariant.'
}
if ($eventHandlerSource -match
    'interface\s+IEventHandler<\s*(in|out)\s+TEvent>') {
    throw 'IEventHandler<TEvent> cannot use variance with EventEnvelope<TEvent>.'
}
if ($eventHandlerSource -notmatch
    'public\s+ValueTask\s+HandleAsync\s*\(') {
    throw 'Interface members must declare explicit public accessibility.'
}

$eventBuilderSource = Get-Content -LiteralPath $eventBuilderPath -Raw
if ($eventBuilderSource -notmatch 'List<IEventHandler<TEvent>>') {
    throw 'Handler registration order must be represented explicitly.'
}
if ($eventBuilderSource -notmatch 'cannot be reused after Build') {
    throw 'The dispatcher builder must freeze after Build.'
}

$eventDispatcherSource =
    Get-Content -LiteralPath $eventDispatcherPath -Raw
if ($eventDispatcherSource -notmatch
    'TryGetValue\(typeof\(TEvent\)') {
    throw 'Dispatch must use exact payload-type lookup.'
}
if ($eventDispatcherSource -notmatch
    'foreach\s+\(var\s+handler\s+in\s+handlers\)') {
    throw 'Dispatch must invoke the ordered handler chain explicitly.'
}
if ($eventDispatcherSource -notmatch 'ConfigureAwait\(false\)') {
    throw 'Asynchronous handler dispatch must avoid context capture.'
}

$forbiddenDispatcherPatterns = @(
    '\bTask\.Run\s*\(',
    '\bTask\.WhenAll\s*\(',
    '\bParallel\.',
    '\bThread\.',
    '\bChannel<',
    '\bEnqueue\s*\(',
    '\bDequeue\s*\(',
    '\bRetryAsync\s*\(',
    '\bSaveAsync\s*\(',
    '\bPersistAsync\s*\(',
    '\bAppendAsync\s*\(',
    '\bEventStore\b'
)

foreach ($pattern in $forbiddenDispatcherPatterns) {
    if ($eventDispatcherSource -match $pattern) {
        throw "The foundation dispatcher contains forbidden behavior matching: $pattern"
    }
}

$worldStateInterfaceSource =
    Get-Content -LiteralPath $worldStateInterfacePath -Raw
if ($worldStateInterfaceSource -notmatch
    'public\s+interface\s+IWorldState') {
    throw 'The immutable world-state marker contract is missing.'
}
if ($worldStateInterfaceSource -notmatch 'deeply immutable') {
    throw 'The world-state immutability contract must remain documented.'
}

$worldStateSnapshotSource =
    Get-Content -LiteralPath $worldStateSnapshotPath -Raw
if ($worldStateSnapshotSource -notmatch
    'public\s+sealed\s+class\s+WorldStateSnapshot<TState>') {
    throw 'World-state snapshots must remain immutable sealed reference objects.'
}
if ($worldStateSnapshotSource -notmatch
    'Id<WorldIdKind>\s+WorldId') {
    throw 'World-state snapshots must carry a typed world ID.'
}
if ($worldStateSnapshotSource -notmatch
    'WorldStateVersion\s+Version') {
    throw 'World-state snapshots must carry a strong version value.'
}
if ($worldStateSnapshotSource -notmatch
    'ulong\s+SimulationTick') {
    throw 'World-state snapshots must carry logical simulation time.'
}
if ($worldStateSnapshotSource -match '\bpublic\s+set\s*;') {
    throw 'World-state snapshot properties must not expose public setters.'
}

$worldStateDecisionSource =
    Get-Content -LiteralPath $worldStateDecisionPath -Raw
if ($worldStateDecisionSource -notmatch
    'Accept\(TState\s+nextState\)') {
    throw 'World-state transitions must explicitly accept a next state.'
}
if ($worldStateDecisionSource -notmatch
    'Reject\(string\s+reason\)') {
    throw 'World-state transitions must support explicit rejection.'
}
if ($worldStateDecisionSource -match '\bpublic\s+set\s*;') {
    throw 'Transition decisions must not expose public setters.'
}

$worldStateManagerSource =
    Get-Content -LiteralPath $worldStateManagerPath -Raw
if ($worldStateManagerSource -notmatch
    'public\s+sealed\s+class\s+WorldStateManager<TState>') {
    throw 'The authoritative generic World State Manager is missing.'
}
if ($worldStateManagerSource -notmatch
    'private\s+readonly\s+System\.Threading\.Lock\s+gate') {
    throw 'World State commits must use one explicit synchronization boundary.'
}
if ($worldStateManagerSource -notmatch
    'WorldStateVersion\s+expectedVersion') {
    throw 'World State commits must require optimistic expected-version checks.'
}
if ($worldStateManagerSource -notmatch
    'transition\.Evaluate\(observed\)') {
    throw 'World State transitions must evaluate against a captured snapshot.'
}
if ($worldStateManagerSource -notmatch
    'current\.Version\s*!=\s*observed\.Version') {
    throw 'World State commits must recheck the observed version atomically.'
}
if ($worldStateManagerSource -notmatch
    'current\.Version\.Next\(\)') {
    throw 'Successful World State commits must advance version exactly once.'
}
if ($worldStateManagerSource -match
    'EventDispatcher|DispatchAsync|IEventHandler') {
    throw 'World State authority must not hide event dispatch.'
}

$forbiddenWorldStatePatterns = @(
    '\bDateTime\b',
    '\bDateTimeOffset\b',
    '\bStopwatch\b',
    '\bGuid\.(NewGuid|CreateVersion7)\s*\(',
    '\bTask\.Run\s*\(',
    '\bTask\.WhenAll\s*\(',
    '\bParallel\.',
    '\bThread\.',
    '\bChannel<',
    '\bRetryAsync\s*\(',
    '\bSaveAsync\s*\(',
    '\bPersistAsync\s*\(',
    '\bAppendAsync\s*\(',
    '\bHttpClient\b'
)

foreach ($pattern in $forbiddenWorldStatePatterns) {
    if ($worldStateManagerSource -match $pattern) {
        throw "World State Manager contains forbidden behavior matching: $pattern"
    }
}

$worldStateProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\WorldState') -Filter '*.cs'
$worldStateProductionText = (
    $worldStateProductionFiles |
        Get-Content -Raw |
        Out-String
)
if ($worldStateProductionText -match
    '\b(NPC|Detective|Crime|Quest|Inventory|Gameplay|LLM)\b') {
    throw 'Generic World State code contains gameplay or AI-provider knowledge.'
}

$entityRegistrySource = Get-Content -LiteralPath $entityRegistryPath -Raw
if ($entityRegistrySource -notmatch 'public\s+sealed\s+class\s+EntityRegistry') {
    throw 'The immutable entity registry is missing.'
}
if ($entityRegistrySource -notmatch 'Id<EntityIdKind>\[\]\s+knownEntityIds') {
    throw 'The entity registry must retain every known identifier.'
}
if ($entityRegistrySource -notmatch 'Id<EntityIdKind>\[\]\s+activeEntityIds') {
    throw 'The entity registry must track active identities separately.'
}
if ($entityRegistrySource -notmatch 'FromActiveEntities\s*\(') {
    throw 'The entity registry must support efficient initial batch creation.'
}
if ($entityRegistrySource -notmatch 'CreateEntity\s*\(') {
    throw 'The entity registry must support immutable entity creation.'
}
if ($entityRegistrySource -notmatch 'DestroyEntity\s*\(') {
    throw 'The entity registry must support immutable entity destruction.'
}
if ($entityRegistrySource -notmatch 'new\s+EntityRegistry\(knownEntityIds,\s*nextActive\)') {
    throw 'Destroyed identities must remain permanently reserved.'
}
if ($entityRegistrySource -notmatch 'EntityMutationStatus\.AlreadyKnown') {
    throw 'Known identifiers must not be created again.'
}
if ($entityRegistrySource -match '\bpublic\s+set\s*;') {
    throw 'Entity registry state must not expose public setters.'
}
if ($entityRegistrySource -match '\b(Guid\.(NewGuid|CreateVersion7)|DateTime|DateTimeOffset|Stopwatch)\b') {
    throw 'The entity registry must not generate identity or read wall-clock time.'
}
if ($entityRegistrySource -match '\b(Component|NPC|Detective|Crime|Quest|Inventory|Gameplay|LLM)\b') {
    throw 'The generic entity registry contains component, gameplay, or AI knowledge.'
}
if ($entityRegistrySource -match 'EventDispatcher|DispatchAsync|IEventHandler') {
    throw 'The entity registry must not hide event dispatch.'
}

$entityMutationResultSource =
    Get-Content -LiteralPath $entityMutationResultPath -Raw
if ($entityMutationResultSource -notmatch 'public\s+sealed\s+class\s+EntityMutationResult') {
    throw 'The explicit entity mutation result contract is missing.'
}
if ($entityMutationResultSource -notmatch 'EntityRegistry\s+Registry') {
    throw 'Every entity mutation result must expose the resulting registry.'
}
if ($entityMutationResultSource -notmatch 'bool\s+WasApplied') {
    throw 'Every entity mutation result must expose whether state changed.'
}
if ($entityMutationResultSource -match '\bpublic\s+set\s*;') {
    throw 'Entity mutation results must not expose public setters.'
}

$componentInterfaceSource =
    Get-Content -LiteralPath $componentInterfacePath -Raw
if ($componentInterfaceSource -notmatch
    'public\s+interface\s+IComponent') {
    throw 'The immutable component marker contract is missing.'
}
if ($componentInterfaceSource -notmatch 'Components contain state only') {
    throw 'The component data-only contract must remain documented.'
}

$componentRegistrySource =
    Get-Content -LiteralPath $componentRegistryPath -Raw
if ($componentRegistrySource -notmatch
    'public\s+sealed\s+class\s+ComponentRegistry') {
    throw 'The immutable component registry is missing.'
}
if ($componentRegistrySource -notmatch
    'IReadOnlyDictionary<Type,\s*IComponentStore>\s+stores') {
    throw 'Component Registry must keep exact component-type stores private.'
}
if ($componentRegistrySource -notmatch
    'Set<TComponent>\s*\(') {
    throw 'Component Registry must support typed immutable add or replace.'
}
if ($componentRegistrySource -notmatch
    'Remove<TComponent>\s*\(') {
    throw 'Component Registry must support typed immutable removal.'
}
if ($componentRegistrySource -notmatch 'PurgeEntity\s*\(') {
    throw 'Component Registry must support complete entity cleanup.'
}
if ($componentRegistrySource -notmatch 'IsConsistentWith\s*\(') {
    throw 'Component Registry must expose entity-lifecycle consistency checks.'
}
if ($componentRegistrySource -notmatch
    'EntityLifecycleStatus\.Active') {
    throw 'Component assignment must require an active entity.'
}
if ($componentRegistrySource -match '\bpublic\s+set\s*;') {
    throw 'Component Registry must not expose public setters.'
}
if ($componentRegistrySource -match
    'EventDispatcher|DispatchAsync|IEventHandler') {
    throw 'Component Registry must not hide event dispatch.'
}
if ($componentRegistrySource -match
    '\b(Guid\.(NewGuid|CreateVersion7)|DateTime|DateTimeOffset|Stopwatch|HttpClient)\b') {
    throw 'Component Registry must not generate identity, read time, or perform I/O.'
}

$componentBuilderSource =
    Get-Content -LiteralPath $componentBuilderPath -Raw
if ($componentBuilderSource -notmatch
    'public\s+sealed\s+class\s+ComponentRegistryBuilder') {
    throw 'The efficient component-registry builder is missing.'
}
if ($componentBuilderSource -notmatch
    'cannot be reused after Build') {
    throw 'Component Registry Builder must be single-use.'
}
if ($componentBuilderSource -notmatch
    'EntityLifecycleStatus\.Active') {
    throw 'Initial component assignment must require active entities.'
}

$componentStoreContractPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Components\IComponentStore.cs'
$componentStoreBuilderContractPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Components\IComponentStoreBuilder.cs'
$componentStoreContractSource =
    Get-Content -LiteralPath $componentStoreContractPath -Raw
$componentStoreBuilderContractSource =
    Get-Content -LiteralPath $componentStoreBuilderContractPath -Raw
if ($componentStoreContractSource -notmatch
    'public\s+int\s+Count\s*\{') {
    throw 'Internal component-store interface members require explicit public accessibility.'
}
if ($componentStoreContractSource -notmatch
    'public\s+IComponentStore\s+Remove\s*\(') {
    throw 'Internal component-store removal must declare explicit public accessibility.'
}
if ($componentStoreBuilderContractSource -notmatch
    'public\s+IComponentStore\s+Build\s*\(') {
    throw 'Internal component-store builder members require explicit public accessibility.'
}

$componentStoreSource =
    Get-Content -LiteralPath $componentStorePath -Raw
if ($componentStoreSource -notmatch
    'internal\s+sealed\s+class\s+ComponentStore<TComponent>') {
    throw 'The exact-type internal component store is missing.'
}
if ($componentStoreSource -notmatch
    'Array\.BinarySearch\(entityIds,\s*entityId\)') {
    throw 'Component lookups must use deterministic sorted entity IDs.'
}
if ($componentStoreSource -notmatch
    'EqualityComparer<TComponent>\.Default\.Equals') {
    throw 'Component updates must detect unchanged values.'
}
if ($componentStoreSource -match '\bpublic\s+set\s*;') {
    throw 'Internal component stores must not expose mutable public setters.'
}

$componentProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Components') -Filter '*.cs'
$componentProductionText = (
    $componentProductionFiles |
        Get-Content -Raw |
        Out-String
)
if ($componentProductionText -match
    '\b(NPC|Detective|Crime|Quest|Inventory|LLM)\b') {
    throw 'Generic component code contains game or AI-domain vocabulary.'
}
if ($componentProductionText -match
    '\b(Task\.Run|Task\.WhenAll|Parallel\.|Thread\.|Channel<)\b') {
    throw 'Component storage must not introduce hidden concurrency or queues.'
}

$simulationSystemInterfaceSource =
    Get-Content -LiteralPath $simulationSystemInterfacePath -Raw
if ($simulationSystemInterfaceSource -notmatch
    'public\s+interface\s+ISimulationSystem<TState>') {
    throw 'The generic simulation-system contract is missing.'
}
if ($simulationSystemInterfaceSource -notmatch
    'public\s+SimulationSystemDecision<TState>\s+Execute\s*\(') {
    throw 'Simulation-system interface members require explicit public accessibility.'
}
if ($simulationSystemInterfaceSource -notmatch
    'side-effect-free') {
    throw 'The side-effect-free simulation-system contract must remain documented.'
}

$simulationBuilderSource =
    Get-Content -LiteralPath $simulationBuilderPath -Raw
if ($simulationBuilderSource -notmatch
    'public\s+sealed\s+class\s+SimulationSchedulerBuilder<TState>') {
    throw 'The deterministic scheduler builder is missing.'
}
if ($simulationBuilderSource -notmatch
    'List<SimulationSystemRegistration<TState>>') {
    throw 'Simulation-system registration order must be stored explicitly.'
}
if ($simulationBuilderSource -notmatch
    'HashSet<Id<SimulationSystemIdKind>>') {
    throw 'Simulation-system IDs must be unique.'
}
if ($simulationBuilderSource -notmatch
    'cannot be reused after Build') {
    throw 'Simulation Scheduler Builder must be single-use.'
}

$simulationSchedulerSource =
    Get-Content -LiteralPath $simulationSchedulerPath -Raw
if ($simulationSchedulerSource -notmatch
    'public\s+sealed\s+class\s+SimulationScheduler<TState>') {
    throw 'The deterministic simulation scheduler is missing.'
}
if ($simulationSchedulerSource -notmatch
    'private\s+readonly\s+System\.Threading\.Lock\s+runGate') {
    throw 'Calls on one scheduler must have one explicit serialization boundary.'
}
if ($simulationSchedulerSource -notmatch
    'checked\(observed\.SimulationTick\s*\+\s*1\)') {
    throw 'Every scheduler step must target exactly the next logical tick.'
}
if ($simulationSchedulerSource -notmatch
    'worldStateManager\.TryApply\s*\(') {
    throw 'The scheduler must commit only through World State Manager.'
}
if ($simulationSchedulerSource -match
    '\b(Task\.Run|Task\.WhenAll|Parallel\.|Thread\.|Timer|PeriodicTimer|Channel<)\b') {
    throw 'The scheduler must not create hidden threads, timers, parallelism, or queues.'
}
if ($simulationSchedulerSource -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7)|HttpClient)\b') {
    throw 'The scheduler must not read wall-clock time, generate IDs, or perform I/O.'
}
if ($simulationSchedulerSource -match
    'EventDispatcher|DispatchAsync|IEventHandler') {
    throw 'The scheduler must not hide event dispatch.'
}
if ($simulationSchedulerSource -match
    '\b(Retry|Save|Persist|Append)\w*\s*\(') {
    throw 'The scheduler must not hide retries or persistence.'
}

$simulationTransitionSource =
    Get-Content -LiteralPath $simulationTransitionPath -Raw
if ($simulationTransitionSource -notmatch
    'for\s*\(var\s+index\s*=\s*0;\s*index\s*<\s*registrations\.Count;\s*index\+\+\)') {
    throw 'Simulation systems must execute in fixed registration order.'
}
if ($simulationTransitionSource -notmatch
    'registration\.System\.Execute\(context\)') {
    throw 'Each simulation system must execute exactly through its typed contract.'
}
if ($simulationTransitionSource -notmatch
    'ExecutedSystemCount\+\+') {
    throw 'Scheduler diagnostics must count every executed system.'
}
if ($simulationTransitionSource -notmatch
    'WorldStateTransitionDecision<TState>\.Accept\(workingState\)') {
    throw 'A complete system sequence must propose one final immutable state.'
}
if ($simulationTransitionSource -match
    'EventDispatcher|DispatchAsync|IEventHandler') {
    throw 'Simulation transition evaluation must not dispatch events.'
}

$coreAssemblyTestsSource =
    Get-Content -LiteralPath $coreAssemblyTestsPath -Raw
if ($coreAssemblyTestsSource -notmatch
    'approvedTypeNames\s*\.OrderBy\(name\s*=>\s*name,\s*StringComparer\.Ordinal\)') {
    throw 'The approved public API list must be sorted with the same ordinal comparer as reflection output.'
}

$simulationProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Simulation') -Filter '*.cs'
$simulationProductionText = (
    $simulationProductionFiles |
        Get-Content -Raw |
        Out-String
)
if ($simulationProductionText -match
    '\b(NPC|Detective|Crime|Quest|Inventory|LLM)\b') {
    throw 'Generic simulation code contains game or AI-domain vocabulary.'
}

$persistenceCodecSource =
    Get-Content -LiteralPath $persistenceCodecPath -Raw
if ($persistenceCodecSource -notmatch
    'public\s+interface\s+IWorldStateSnapshotCodec<TState>') {
    throw 'The generic World State snapshot codec contract is missing.'
}
if ($persistenceCodecSource -notmatch
    'public\s+PersistenceSchemaId\s+SchemaId\s*\{') {
    throw 'Persistence codec schema identity must be explicit.'
}
if ($persistenceCodecSource -notmatch
    'public\s+bool\s+CanDecode\s*\(') {
    throw 'Persistence codecs must declare supported schema versions.'
}
if ($persistenceCodecSource -notmatch
    'public\s+SnapshotPayload\s+Encode\s*\(') {
    throw 'Persistence codecs must expose deterministic encoding.'
}
if ($persistenceCodecSource -notmatch
    'public\s+WorldStateDecodeDecision<TState>\s+Decode\s*\(') {
    throw 'Persistence codecs must expose explicit decode decisions.'
}
if ($persistenceCodecSource -notmatch
    'Stable schema IDs must not depend on CLR type names') {
    throw 'Stable persistence identity policy must remain documented.'
}

$persistenceServiceSource =
    Get-Content -LiteralPath $persistenceServicePath -Raw
if ($persistenceServiceSource -notmatch
    'public\s+sealed\s+class\s+WorldStateSnapshotPersistence<TState>') {
    throw 'The transport-independent snapshot persistence service is missing.'
}
if ($persistenceServiceSource -notmatch
    'codec\.Encode\(snapshot\.State\)') {
    throw 'Snapshot capture must delegate state encoding to one explicit codec.'
}
if ($persistenceServiceSource -notmatch
    'SnapshotChecksum\.Compute\(payload\)') {
    throw 'Snapshot capture must checksum the exact encoded payload.'
}
if ($persistenceServiceSource -notmatch
    'document\.FormatVersion\s*!=\s*SnapshotFormatVersion\.Current') {
    throw 'Restore must validate outer format before decoding.'
}
if ($persistenceServiceSource -notmatch
    'document\.SchemaId\s*!=\s*codec\.SchemaId') {
    throw 'Restore must validate stable schema identity before decoding.'
}
if ($persistenceServiceSource -notmatch
    '!codec\.CanDecode\(document\.SchemaVersion\)') {
    throw 'Restore must validate schema-version support before decoding.'
}
if ($persistenceServiceSource -notmatch
    '!document\.HasValidChecksum') {
    throw 'Restore must validate checksum before decoding.'
}
if ($persistenceServiceSource -notmatch
    'codec\.Decode\s*\(') {
    throw 'Restore must decode through the explicit codec.'
}
if ($persistenceServiceSource -notmatch
    'new\s+WorldStateSnapshot<TState>\s*\(') {
    throw 'Successful restore must reconstruct immutable snapshot metadata.'
}
if ($persistenceServiceSource -match
    '\b(File\.|Directory\.|FileStream|StreamReader|StreamWriter|SqlConnection|HttpClient)\b') {
    throw 'Core snapshot persistence must not perform storage or network I/O.'
}
if ($persistenceServiceSource -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7))\b') {
    throw 'Core snapshot persistence must not read clocks or generate IDs.'
}
if ($persistenceServiceSource -match
    'EventDispatcher|DispatchAsync|SimulationScheduler|RunNextTick') {
    throw 'Snapshot persistence must not dispatch events or run simulation.'
}

$snapshotDocumentSource =
    Get-Content -LiteralPath $snapshotDocumentPath -Raw
foreach ($requiredProperty in @(
    'FormatVersion',
    'SchemaId',
    'SchemaVersion',
    'WorldId',
    'WorldStateVersion',
    'SimulationTick',
    'Payload',
    'Checksum'
)) {
    if ($snapshotDocumentSource -notmatch
        "public\s+[\w<>\.]+\s+$requiredProperty\s*\{\s*get;\s*\}") {
        throw "Snapshot document property '$requiredProperty' is missing or mutable."
    }
}
if ($snapshotDocumentSource -notmatch
    'bool\s+HasValidChecksum') {
    throw 'Snapshot documents must expose integrity verification.'
}
if ($snapshotDocumentSource -match '\bpublic\s+set\s*;') {
    throw 'Snapshot documents must not expose public setters.'
}

$snapshotPayloadSource =
    Get-Content -LiteralPath $snapshotPayloadPath -Raw
if ($snapshotPayloadSource -notmatch 'data\.ToArray\(\)') {
    throw 'Snapshot payload creation must defensively copy input bytes.'
}
if ($snapshotPayloadSource -notmatch 'bytes\.ToArray\(\)') {
    throw 'Snapshot payload output must return caller-owned copies.'
}
if ($snapshotPayloadSource -match
    'ReadOnlyMemory<byte>\s+\w+\s*\{\s*get;') {
    throw 'Snapshot payload must not expose recoverable backing arrays.'
}

$snapshotChecksumSource =
    Get-Content -LiteralPath $snapshotChecksumPath -Raw
if ($snapshotChecksumSource -notmatch
    'SHA256\.HashData\s*\(') {
    throw 'Snapshot payload integrity must use SHA-256.'
}
if ($snapshotChecksumSource -notmatch
    'ToLowerInvariant\(\)') {
    throw 'Snapshot checksums must have canonical lowercase text.'
}

$worldStateManagerSource =
    Get-Content -LiteralPath $worldStateManagerPath -Raw
if ($worldStateManagerSource -notmatch
    'public\s+static\s+WorldStateManager<TState>\s+Restore\s*\(') {
    throw 'World State Manager must adopt validated restored snapshots explicitly.'
}
if ($worldStateManagerSource -notmatch
    'return\s+new\s+WorldStateManager<TState>\(snapshot\)') {
    throw 'Restored managers must preserve the validated snapshot exactly.'
}

$persistenceProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Persistence') -Filter '*.cs'
$persistenceProductionText = (
    $persistenceProductionFiles |
        Get-Content -Raw |
        Out-String
)
if ($persistenceProductionText -match
    '\b(NPC|Detective|Crime|Quest|Inventory|LLM)\b') {
    throw 'Generic persistence code contains game or AI-domain vocabulary.'
}
if ($persistenceProductionText -match
    '\b(Task\.Run|Task\.WhenAll|Parallel\.|Thread\.|Timer|Channel<)\b') {
    throw 'Persistence contracts must not introduce hidden execution or queues.'
}

$validationTokens = $null
$validationParseErrors = $null
[System.Management.Automation.Language.Parser]::ParseFile(
    $foundationValidationScriptPath,
    [ref]$validationTokens,
    [ref]$validationParseErrors) | Out-Null
if ($validationParseErrors.Count -gt 0) {
    throw 'eng\validate-foundation.ps1 contains PowerShell syntax errors.'
}

$foundationValidationScriptSource =
    Get-Content -LiteralPath $foundationValidationScriptPath -Raw
if ($foundationValidationScriptSource -notmatch
    'FullyQualifiedName~FoundationValidationTests') {
    throw 'Foundation validation must execute the dedicated integration tests.'
}
if ($foundationValidationScriptSource -notmatch
    'for\s*\(\$iteration\s*=\s*1;\s*\$iteration\s*-le\s*\$RepeatCount') {
    throw 'Foundation validation must repeat the headless probe.'
}
if ($foundationValidationScriptSource -notmatch
    '\^checksum=\[0-9a-f\]\{64\}\$') {
    throw 'Foundation validation must verify a canonical deterministic checksum.'
}
if ($foundationValidationScriptSource -notmatch
    '\$elapsedMilliseconds\s*-gt\s*\$MaxMilliseconds') {
    throw 'Foundation validation must enforce the broad performance budget.'
}

$foundationValidationTestsSource =
    Get-Content -LiteralPath $foundationValidationTestsPath -Raw
foreach ($requiredTest in @(
    'UninterruptedAndSaveRestoreContinuationAreIdentical',
    'IndependentConstructionOrdersProduceIdenticalSnapshots',
    'EverySuccessfulTickAdvancesVersionAndLogicalTimeExactlyOnce',
    'FinalSnapshotCanBeCapturedRestoredAndContinuedAgain'
)) {
    if ($foundationValidationTestsSource -notmatch [regex]::Escape($requiredTest)) {
        throw "Foundation validation test '$requiredTest' is missing."
    }
}
foreach ($requiredLayer in @(
    'EventDispatcherBuilder',
    'WorldStateManager',
    'EntityRegistry',
    'ComponentRegistryBuilder',
    'SimulationSchedulerBuilder',
    'WorldStateSnapshotPersistence'
)) {
    if ($foundationValidationTestsSource -notmatch [regex]::Escape($requiredLayer)) {
        throw "Foundation integration tests do not cover layer '$requiredLayer'."
    }
}

$foundationProbeProgramSource =
    Get-Content -LiteralPath $foundationProbeProgramPath -Raw
if ($foundationProbeProgramSource -notmatch 'FOUNDATION_PROBE_OK') {
    throw 'Foundation Probe must emit an explicit success marker.'
}
if ($foundationProbeProgramSource -notmatch
    'RunScenario\(\s*tickCount,\s*checkpointTick:\s*null\)') {
    throw 'Foundation Probe must execute an uninterrupted scenario.'
}
if ($foundationProbeProgramSource -notmatch
    'checkpointTick:\s*tickCount\s*/\s*2') {
    throw 'Foundation Probe must execute a save and restore continuation.'
}
if ($foundationProbeProgramSource -notmatch
    'ContentEquals\s*\(') {
    throw 'Foundation Probe must compare deterministic payload bytes.'
}
if ($foundationProbeProgramSource -notmatch
    'EventDispatcherBuilder') {
    throw 'Foundation Probe must validate post-commit event dispatch.'
}
if ($foundationProbeProgramSource -match
    '\b(HttpClient|File\.|Directory\.|SqlConnection)\b') {
    throw 'Foundation Probe must remain self-contained and headless.'
}
$foundationProbeHostRuntimePath = Join-Path $root (
    'samples\AI.Sandbox.Engine.FoundationProbe\HostRuntimeProbe.cs')
if (-not (Test-Path -LiteralPath $foundationProbeHostRuntimePath)) {
    throw 'Host Runtime normal lifecycle probe source is missing.'
}

$foundationProbeHostRuntimeSource =
    Get-Content -LiteralPath $foundationProbeHostRuntimePath -Raw
foreach ($requiredToken in @(
    'HostRuntimeQueueAdmissionFlow.Decide'
    'HostRuntimeWorkLeaseFlow.Acquire'
    'HostRuntimeDispatchSelectionFlow.Select'
    'HostRuntimeDispatchAcknowledgementFlow.Acknowledge'
    'HostRuntimeAttemptSettlementFlow.Settle'
    'HostRuntimeCompletionFlow.Create'
    'HostRuntimeRetryDecisionFlow.Decide'
    'HostRuntimeRetryRequeueFlow.Requeue'
    'HostRuntimeDeadLetterDispositionFlow.Dispose'
    'HostRuntimeRequestFlow.RequestCancellation'
    'HostRuntimeAbandonedAttemptDispositionFlow.Dispose'
)) {
    if ($foundationProbeHostRuntimeSource -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host Runtime normal lifecycle probe is missing: ' +
            $requiredToken)
    }
}

if ($foundationProbeProgramSource -notmatch
    'HostRuntimeProbe\.Run\(\)') {
    throw 'Foundation Probe must execute the Host Runtime lifecycle probe.'
}
if ($foundationProbeProgramSource -notmatch
    'HostRuntimeProbe\.RunRetry\(\)') {
    throw (
        'Foundation Probe must execute Host Runtime retry, requeue, and ' +
        'dead-letter scenarios.')
}
if ($foundationProbeProgramSource -notmatch
    'HostRuntimeProbe\.RunAbandonment\(\)') {
    throw (
        'Foundation Probe must execute Host Runtime cancellation, ' +
        'lease-expiry, and abandonment scenarios.')
}

$foundationProbeConversationPath = Join-Path $root (
    'samples\AI.Sandbox.Engine.FoundationProbe\ConversationProbe.cs')
if (-not (Test-Path -LiteralPath $foundationProbeConversationPath)) {
    throw 'Conversation address-resolution probe source is missing.'
}

$foundationProbeConversationSource =
    Get-Content -LiteralPath $foundationProbeConversationPath -Raw

foreach ($requiredToken in @(
    'ConversationState<ProbeTopic>.Start'
    'AddressAudience.SpecificParticipants'
    'AddressResolutionDecision.Resolve'
    'AddressResolutionConfidence.FromBasisPoints'
    'AddressResolutionProcessor<'
    'AddressResolutionRequestEnvelope<ProbeAddressQuery>.Create'
)) {
    if ($foundationProbeConversationSource -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Conversation address-resolution probe is missing: ' +
            $requiredToken)
    }
}

if ($foundationProbeProgramSource -notmatch
    'ConversationProbe\.Run\(\)') {
    throw (
        'Foundation Probe must execute Conversation address resolution.')
}

foreach ($requiredToken in @(
    'DialogueOrchestrationProcessor<'
    'DialogueOrchestrationRequestEnvelope<'
    'DialogueOrchestrationDecision<'
    'DialogueOrchestrationStatus.Continued'
    'FixedDialogueOrchestrator'
    'dialogueOrchestrator.CallCount != 1'
    'resolution.Decision.Audience'
)) {
    if ($foundationProbeConversationSource -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Conversation-to-Dialogue probe is missing: ' +
            $requiredToken)
    }
}

foreach ($requiredToken in @(
    'dialogue_status='
    'dialogue_orchestrator_call_count='
    'dialogue_was_continued='
    'dialogue_directive_name='
    'dialogue_world_authority_unchanged='
)) {
    if ($foundationProbeProgramSource -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Foundation Probe Dialogue output is missing: ' +
            $requiredToken)
    }
}

$foundationProbeCommandPath = Join-Path (
    Split-Path -Parent $foundationProbeProgramPath) 'CommandProbe.cs'

if (-not (Test-Path -LiteralPath $foundationProbeCommandPath -PathType Leaf)) {
    throw (
        'Command FoundationProbe source is missing: ' +
        $foundationProbeCommandPath)
}

$foundationProbeCommandSource =
    Get-Content -LiteralPath $foundationProbeCommandPath -Raw

foreach ($requiredToken in @(
    'internal static class CommandProbe'
    'CommandProcessorBuilder<CounterState>'
    'CommandEnvelope<AddValue>.Create'
    'processor.Execute(envelope)'
    'CommandExecutionStatus.Applied'
    'handler.EvaluationCount != 1'
    'after.Version.Value == before.Version.Value + 1UL'
    'after.SimulationTick == before.SimulationTick'
)) {
    if ($foundationProbeCommandSource -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Command FoundationProbe source is missing: ' +
            $requiredToken)
    }
}

foreach ($requiredToken in @(
    'CommandProbe.Run()'
    'command_status='
    'command_was_applied='
    'command_handler_evaluation_count='
    'command_before_value='
    'command_after_value='
    'command_before_version='
    'command_after_version='
    'command_version_advanced_once='
    'command_tick_preserved='
)) {
    if ($foundationProbeProgramSource -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Foundation Probe Command output is missing: ' +
            $requiredToken)
    }
}

foreach ($requiredToken in @(
    'RuntimeOrchestratorBuilder<CounterState>'
    'runtime.ExecuteCommand(runtimeEnvelope)'
    'RuntimeInvocationStatus.Completed'
    'runtimeResult.WasInvoked'
    'runtimeResult.WasCommitted'
    'runtimeResult.CommandResult.Status'
    'runtimeResult.CommitFact'
    'runtimeCommitFact.IsCommandCommit'
    'RuntimeCommitKind.Command'
    'runtimeDispatchCalls.Count == 0'
)) {
    if ($foundationProbeCommandSource -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Runtime handoff CommandProbe source is missing: ' +
            $requiredToken)
    }
}

foreach ($requiredToken in @(
    'runtime_invocation_status='
    'runtime_was_invoked='
    'runtime_was_committed='
    'runtime_command_status='
    'runtime_commit_fact_valid='
    'runtime_auto_dispatch_absent='
)) {
    if ($foundationProbeProgramSource -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Foundation Probe Runtime handoff output is missing: ' +
            $requiredToken)
    }
}

$foundationProbePerceptionPath = Join-Path (
    Split-Path -Parent $foundationProbeProgramPath) 'PerceptionProbe.cs'

if (-not (Test-Path -LiteralPath $foundationProbePerceptionPath -PathType Leaf)) {
    throw (
        'Perception FoundationProbe source is missing: ' +
        $foundationProbePerceptionPath)
}

$foundationProbePerceptionSource =
    Get-Content -LiteralPath $foundationProbePerceptionPath -Raw

foreach ($requiredToken in @(
    'internal static class PerceptionProbe'
    'PerceptionProcessorBuilder<CounterState>'
    '.Add<TextStimulus, TextSignal>(evaluator)'
    'processor.Evaluate<TextStimulus, TextSignal>(envelope)'
    'PerceptionEvaluationStatus.Observed'
    'evaluator.EvaluationCount != 1'
    'observation.StimulusId == envelope.StimulusId'
    'observation.WorldStateVersion == before.Version'
    'observation.SimulationTick == before.SimulationTick'
    'Object.ReferenceEquals(before, after)'
    'before.Version == after.Version'
    'before.SimulationTick == after.SimulationTick'
)) {
    if ($foundationProbePerceptionSource -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Perception FoundationProbe source is missing: ' +
            $requiredToken)
    }
}

foreach ($requiredToken in @(
    'PerceptionProbe.Run()'
    'perception_status='
    'perception_evaluator_executed='
    'perception_evaluator_evaluation_count='
    'perception_observation_present='
    'perception_observation_identity_valid='
    'perception_observation_snapshot_metadata_valid='
    'perception_confidence_basis_points='
    'perception_signal='
    'perception_snapshot_reference_preserved='
    'perception_world_authority_unchanged='
    'perception_before_value='
    'perception_after_value='
    'perception_before_version='
    'perception_after_version='
    'perception_before_tick='
    'perception_after_tick='
)) {
    if ($foundationProbeProgramSource -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Foundation Probe Perception output is missing: ' +
            $requiredToken)
    }
}

$foundationProbeSocialPath = Join-Path (
    Split-Path -Parent $foundationProbeProgramPath) 'SocialProbe.cs'

if (-not (Test-Path -LiteralPath $foundationProbeSocialPath -PathType Leaf)) {
    throw (
        'Social FoundationProbe source is missing: ' +
        $foundationProbeSocialPath)
}

$foundationProbeSocialSource =
    Get-Content -LiteralPath $foundationProbeSocialPath -Raw

foreach ($requiredToken in @(
    'internal static class SocialProbe'
    'SocialTurnCoordinationProcessor<'
    'SocialTurnCoordinationRequestEnvelope<Proposal>.Create('
    'SocialTurnCoordinationDecision.Grant('
    'request.Proposals.Count == 2'
    'coordinator.CallCount != 1'
    'SocialTurnCoordinationStatus.Granted'
    'SocialTurnCoordinationDecisionStatus.Granted'
    'result.SelectedProposal'
    'conversation.Revision == conversationRevisionBefore'
    'Object.ReferenceEquals(before, after)'
    'before.Version == after.Version'
    'before.SimulationTick == after.SimulationTick'
)) {
    if ($foundationProbeSocialSource -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Social FoundationProbe source is missing: ' +
            $requiredToken)
    }
}

foreach ($requiredToken in @(
    'SocialProbe.Run()'
    'social_turn_status='
    'social_turn_coordinator_invoked='
    'social_turn_coordinator_call_count='
    'social_turn_stable_decision='
    'social_turn_decision_granted='
    'social_turn_selected_proposal_present='
    'social_turn_selected_proposal_identity_valid='
    'social_turn_two_proposal_request='
    'social_turn_deterministic_proposal_order_valid='
    'social_turn_request_authority_metadata_valid='
    'social_turn_conversation_revision_preserved='
    'social_turn_world_authority_unchanged='
    'social_turn_before_value='
    'social_turn_after_value='
    'social_turn_before_version='
    'social_turn_after_version='
    'social_turn_before_tick='
    'social_turn_after_tick='
)) {
    if ($foundationProbeProgramSource -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Foundation Probe Social output is missing: ' +
            $requiredToken)
    }
}

$foundationProbePromptingPath = Join-Path (
    Split-Path -Parent $foundationProbeProgramPath) 'PromptingProbe.cs'

if (-not (Test-Path -LiteralPath $foundationProbePromptingPath -PathType Leaf)) {
    throw (
        'Prompting FoundationProbe source is missing: ' +
        $foundationProbePromptingPath)
}

$foundationProbePromptingSource =
    Get-Content -LiteralPath $foundationProbePromptingPath -Raw

foreach ($requiredToken in @(
    'internal static class PromptingProbe'
    'PromptBudgetManager<PromptText>'
    'PromptCandidateEnvelope<PromptText>.Create('
    'PromptBudget.FromUnits(10)'
    'PromptPriority.FromBasisPoints'
    'PromptCost.FromUnits'
    'allocationCallCount != 1'
    'PromptBudgetStatus.Selected'
    'selected[0].CandidateId == required.CandidateId'
    'highPriorityOptional.CandidateId'
    'lowerPriorityOptional.CandidateId'
    'Object.ReferenceEquals('
    'inputPayloadValuesPreserved'
    'ownerScopePreserved'
    'worldScopePreserved'
)) {
    if ($foundationProbePromptingSource -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Prompting FoundationProbe source is missing: ' +
            $requiredToken)
    }
}

foreach ($requiredToken in @(
    'PromptingProbe.Run()'
    'prompt_budget_status='
    'prompt_budget_allocation_call_count='
    'prompt_budget_input_candidate_count='
    'prompt_budget_selected_candidate_count='
    'prompt_budget_required_units='
    'prompt_budget_used_units='
    'prompt_budget_remaining_units='
    'prompt_budget_required_selected='
    'prompt_budget_highest_priority_optional_selected='
    'prompt_budget_lower_priority_optional_skipped='
    'prompt_budget_deterministic_order_valid='
    'prompt_budget_selected_identity_preserved='
    'prompt_budget_input_candidate_identity_preserved='
    'prompt_budget_input_payload_values_preserved='
    'prompt_budget_owner_scope_preserved='
    'prompt_budget_world_scope_preserved='
)) {
    if ($foundationProbeProgramSource -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Foundation Probe Prompt Budget output is missing: ' +
            $requiredToken)
    }
}

# CORE-0093 persistent Prompt Composition FoundationProbe verification.
foreach ($requiredToken in @(
    'internal static CompositionResult RunComposition()'
    'private sealed record PromptWorldState'
    'PromptCompositionProcessor<'
    'PromptRequestEnvelope<ComposeRequest>.Create('
    'PromptDocumentEnvelope<RenderedPrompt>.Create('
    'processor.Compose('
    'result.Decision'
    'context.Snapshot'
    'context.BudgetResult.SelectedCandidates'
    'PromptCompositionDecision<RenderedPrompt>.Compose('
    'worldAuthorityUnchanged'
)) {
    if ($foundationProbePromptingSource -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Prompt Composition FoundationProbe source is missing: ' +
            $requiredToken)
    }
}

foreach ($requiredToken in @(
    'PromptingProbe.RunComposition()'
    'prompt_composition_status='
    'prompt_composition_processor_call_count='
    'prompt_composition_was_composed='
    'prompt_composition_composer_invoked='
    'prompt_composition_composer_call_count='
    'prompt_composition_composer_selected_count='
    'prompt_composition_composer_context_valid='
    'prompt_composition_decision_status='
    'prompt_composition_budget_status='
    'prompt_composition_budget_selected_candidate_count='
    'prompt_composition_required_units='
    'prompt_composition_used_units='
    'prompt_composition_remaining_units='
    'prompt_composition_request_identity_preserved='
    'prompt_composition_document_identity_preserved='
    'prompt_composition_document_scope_valid='
    'prompt_composition_document_payload_valid='
    'prompt_composition_world_authority_unchanged='
    'prompt_composition_before_value='
    'prompt_composition_after_value='
    'prompt_composition_before_version='
    'prompt_composition_after_version='
    'prompt_composition_before_tick='
    'prompt_composition_after_tick='
)) {
    if ($foundationProbeProgramSource -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Foundation Probe Prompt Composition output is missing: ' +
            $requiredToken)
    }
}$currentStatusSource = Get-Content -LiteralPath (
    Join-Path $root 'docs\status\CURRENT.md') -Raw
if ($currentStatusSource -match
    'Next bounded foundation increment:\s*`\d{4}\s+Host Recovery[^`]*(?:Sequence-){4,}') {
    throw (
        'Recovery hierarchy growth must not continue through mechanical ' +
        'Sequence nesting.')
}

$commandInterfaceSource =
    Get-Content -LiteralPath $commandInterfacePath -Raw
if ($commandInterfaceSource -notmatch
    'public\s+interface\s+IEngineCommand') {
    throw 'The immutable command marker contract is missing.'
}
if ($commandInterfaceSource -notmatch
    'request,\s+not\s+a\s+completed\s+fact') {
    throw 'Command and event semantics must remain explicitly separated.'
}

$commandEnvelopeSource =
    Get-Content -LiteralPath $commandEnvelopePath -Raw
foreach ($requiredProperty in @(
    'CommandId',
    'WorldId',
    'ExpectedWorldStateVersion',
    'ExpectedSimulationTick',
    'Payload'
)) {
    if ($commandEnvelopeSource -notmatch
        "public\s+[\w<>,\.]+\s+$requiredProperty\s*\{\s*get;\s*\}") {
        throw "Command envelope property '$requiredProperty' is missing or mutable."
    }
}
if ($commandEnvelopeSource -match '\bpublic\s+set\s*;') {
    throw 'Command envelopes must not expose public setters.'
}
if ($commandEnvelopeSource -notmatch
    'CommandTypePolicy\.EnsureConcrete<TCommand>\(\)') {
    throw 'Command envelopes must require exact concrete command types.'
}

$commandHandlerSource =
    Get-Content -LiteralPath $commandHandlerPath -Raw
if ($commandHandlerSource -notmatch
    'public\s+interface\s+ICommandHandler<TState,\s*TCommand>') {
    throw 'The exact-type command handler contract is missing.'
}
if ($commandHandlerSource -notmatch
    'public\s+CommandDecision<TState>\s+Evaluate\s*\(') {
    throw 'Command-handler interface members require explicit public accessibility.'
}
if ($commandHandlerSource -notmatch 'at most once') {
    throw 'Exactly-once command-handler evaluation policy must remain documented.'
}

$commandProcessorSource =
    Get-Content -LiteralPath $commandProcessorPath -Raw
if ($commandProcessorSource -notmatch
    'public\s+sealed\s+class\s+CommandProcessor<TState>') {
    throw 'The exact-type command processor is missing.'
}
if ($commandProcessorSource -notmatch
    'envelope\.WorldId\s*!=\s*observed\.WorldId') {
    throw 'Command processing must reject a mismatched world before evaluation.'
}
if ($commandProcessorSource -notmatch
    'envelope\.ExpectedWorldStateVersion\s*!=\s*observed\.Version') {
    throw 'Command processing must reject stale versions before evaluation.'
}
if ($commandProcessorSource -notmatch
    'envelope\.ExpectedSimulationTick\s*!=\s*observed\.SimulationTick') {
    throw 'Command processing must reject stale logical ticks before evaluation.'
}
if ($commandProcessorSource -notmatch
    'handlers\.TryGetValue\s*\(\s*typeof\(TCommand\)') {
    throw 'Command processing must route by exact concrete command type.'
}
if ($commandProcessorSource -notmatch
    'worldStateManager\.TryApply\s*\(') {
    throw 'Accepted commands must commit only through World State Manager.'
}
if ($commandProcessorSource -match
    '\b(Task\.Run|Task\.WhenAll|Parallel\.|Thread\.|Timer|Channel<)\b') {
    throw 'Command Processor must not introduce hidden execution or queues.'
}
if ($commandProcessorSource -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7)|HttpClient)\b') {
    throw 'Command Processor must not read time, generate IDs, or perform I/O.'
}
if ($commandProcessorSource -match
    'EventDispatcher|DispatchAsync|IEventHandler') {
    throw 'Command Processor must not hide event dispatch.'
}
if ($commandProcessorSource -match
    '\b(Retry|Persist|Save|Append)\w*\s*\(') {
    throw 'Command Processor must not hide retries or persistence.'
}

$commandTransitionSource =
    Get-Content -LiteralPath $commandTransitionPath -Raw
if ($commandTransitionSource -notmatch
    'handler\.Evaluate\(context\)') {
    throw 'Every command must execute exactly through its typed handler.'
}
if ($commandTransitionSource -notmatch
    'WasEvaluated\s*=\s*true') {
    throw 'Command transition diagnostics must record handler evaluation.'
}
if ($commandTransitionSource -notmatch
    'ReferenceEquals\(nextState,\s*current\.State\)') {
    throw 'Accepted commands must not create no-op version increments.'
}
if ($commandTransitionSource -notmatch
    'WorldStateTransitionDecision<TState>\.Accept\(nextState\)') {
    throw 'Accepted commands must propose one immutable World State root.'
}
if ($commandTransitionSource -notmatch
    'WorldStateTransitionDecision<TState>\.Reject\s*\(') {
    throw 'Rejected commands must remain explicit World State rejections.'
}

$commandProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Commands') -Filter '*.cs'
$commandProductionText = (
    $commandProductionFiles |
        Get-Content -Raw |
        Out-String
)
if ($commandProductionText -match
    '\b(NPC|Detective|Crime|Quest|Inventory|LLM)\b') {
    throw 'Generic command code contains game or AI-domain vocabulary.'
}
if ($commandProductionText -match
    '\b(File\.|Directory\.|FileStream|SqlConnection|HttpClient)\b') {
    throw 'Command contracts must not perform storage or network I/O.'
}

$randomStateSource =
    Get-Content -LiteralPath $randomStatePath -Raw
if ($randomStateSource -notmatch
    'public\s+readonly\s+record\s+struct\s+DeterministicRandomState') {
    throw 'The immutable deterministic random state is missing.'
}
foreach ($requiredProperty in @(
    'AlgorithmVersion',
    'StreamId',
    'StateValue',
    'DrawCount'
)) {
    if ($randomStateSource -notmatch
        "public\s+[\w<>,\.]+\s+$requiredProperty\s*\{\s*get;\s*\}") {
        throw "Deterministic random property '$requiredProperty' is missing or mutable."
    }
}
if ($randomStateSource -notmatch
    'RandomAlgorithmVersion\.Current') {
    throw 'Deterministic random state must expose an explicit algorithm version.'
}
if ($randomStateSource -notmatch
    'HashCanonicalStreamId\s*\(') {
    throw 'Independent stream derivation must use stable stream identity.'
}
if ($randomStateSource -notmatch
    'checked\(DrawCount\s*\+\s*1\)') {
    throw 'Primitive random draw count must advance with checked arithmetic.'
}
if ($randomStateSource -notmatch
    'unchecked\(0UL\s*-\s*exclusiveUpperBound\)\s*%\s*exclusiveUpperBound') {
    throw 'Bounded unsigned draws must use unbiased rejection sampling.'
}
if ($randomStateSource -notmatch
    '1\.0\s*/\s*9007199254740992\.0') {
    throw 'Double draws must use a stable 53-bit conversion.'
}
if ($randomStateSource -match '\bSystem\.Random\b|\bRandom\.Shared\b') {
    throw 'Deterministic randomness must not delegate to System.Random.'
}
if ($randomStateSource -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7))\b') {
    throw 'Deterministic randomness must not read clocks or generate identity.'
}
if ($randomStateSource -match '\bpublic\s+set\s*;') {
    throw 'Deterministic random state must not expose public setters.'
}

$randomSeedSource =
    Get-Content -LiteralPath $randomSeedPath -Raw
if ($randomSeedSource -notmatch
    'private\s+readonly\s+bool\s+isInitialized') {
    throw 'Random Seed must distinguish explicit zero from the default value.'
}
if ($randomSeedSource -notmatch
    'public\s+static\s+RandomSeed\s+From\s*\(\s*ulong\s+value\s*\)') {
    throw 'Random Seed must be explicitly initialized.'
}

$randomVersionSource =
    Get-Content -LiteralPath $randomVersionPath -Raw
if ($randomVersionSource -notmatch
    'public\s+static\s+RandomAlgorithmVersion\s+Current') {
    throw 'Random algorithm persistence version is missing.'
}

$randomProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Randomness') -Filter '*.cs'
$randomProductionText = (
    $randomProductionFiles |
        Get-Content -Raw |
        Out-String
)
if ($randomProductionText -match
    '\b(NPC|Detective|Crime|Quest|Inventory|LLM)\b') {
    throw 'Generic randomness code contains game or AI-domain vocabulary.'
}
if ($randomProductionText -match
    '\b(Task\.Run|Task\.WhenAll|Parallel\.|Thread\.|Timer|Channel<)\b') {
    throw 'Randomness primitives must not introduce hidden execution or queues.'
}
if ($randomProductionText -match
    '\b(File\.|Directory\.|FileStream|SqlConnection|HttpClient)\b') {
    throw 'Randomness primitives must not perform storage or network I/O.'
}

$simulationDurationSource =
    Get-Content -LiteralPath $simulationDurationPath -Raw
if ($simulationDurationSource -notmatch
    'public\s+readonly\s+record\s+struct\s+SimulationDuration') {
    throw 'The integer simulation duration primitive is missing.'
}
if ($simulationDurationSource -notmatch
    'public\s+ulong\s+Microseconds\s*\{\s*get;\s*\}') {
    throw 'Simulation Duration must use immutable integer microseconds.'
}
foreach ($factory in @(
    'FromMicroseconds',
    'FromMilliseconds',
    'FromSeconds',
    'FromMinutes',
    'FromHours',
    'FromDays'
)) {
    if ($simulationDurationSource -notmatch
        "public\s+static\s+SimulationDuration\s+$factory\s*\(") {
        throw "Simulation Duration factory '$factory' is missing."
    }
}
if ($simulationDurationSource -notmatch
    'checked\(Microseconds\s*\+\s*other\.Microseconds\)') {
    throw 'Simulation Duration addition must use checked arithmetic.'
}
if ($simulationDurationSource -notmatch
    'checked\(Microseconds\s*\*\s*factor\)') {
    throw 'Simulation Duration multiplication must use checked arithmetic.'
}

$simulationInstantSource =
    Get-Content -LiteralPath $simulationInstantPath -Raw
if ($simulationInstantSource -notmatch
    'public\s+readonly\s+record\s+struct\s+SimulationInstant') {
    throw 'The deterministic simulation instant primitive is missing.'
}
if ($simulationInstantSource -notmatch
    'public\s+ulong\s+MicrosecondsSinceEpoch\s*\{\s*get;\s*\}') {
    throw 'Simulation Instant must use immutable integer epoch microseconds.'
}
if ($simulationInstantSource -notmatch
    'checked\s*\(\s*MicrosecondsSinceEpoch\s*\+\s*duration\.Microseconds') {
    throw 'Simulation Instant addition must use checked arithmetic.'
}

$simulationTickDurationSource =
    Get-Content -LiteralPath $simulationTickDurationPath -Raw
if ($simulationTickDurationSource -notmatch
    'public\s+readonly\s+record\s+struct\s+SimulationTickDuration') {
    throw 'The positive logical tick duration primitive is missing.'
}
if ($simulationTickDurationSource -notmatch
    'if\s*\(value\s*==\s*0\)') {
    throw 'Logical simulation tick duration must reject zero.'
}

$simulationTimelineSource =
    Get-Content -LiteralPath $simulationTimelinePath -Raw
if ($simulationTimelineSource -notmatch
    'public\s+readonly\s+record\s+struct\s+SimulationTimeline') {
    throw 'The deterministic simulation timeline is missing.'
}
if ($simulationTimelineSource -notmatch
    'checked\s*\(\s*simulationTick\s*\*\s*TickDuration\.Microseconds') {
    throw 'Tick-to-instant conversion must use checked integer multiplication.'
}
if ($simulationTimelineSource -notmatch
    'instant\.MicrosecondsSinceEpoch\s*/\s*TickDuration\.Microseconds') {
    throw 'Timeline floor mapping must use integer division.'
}
if ($simulationTimelineSource -notmatch
    'instant\.MicrosecondsSinceEpoch\s*%\s*TickDuration\.Microseconds') {
    throw 'Timeline ceiling mapping must inspect the integer remainder.'
}
if ($simulationTimelineSource -notmatch
    'GetInstant\(currentTick\)\.Add\(delay\)') {
    throw 'Deadline conversion must begin at an exact current tick boundary.'
}

$simulationTimeFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Time') -Filter '*.cs'
$simulationTimeText = (
    $simulationTimeFiles |
        Get-Content -Raw |
        Out-String
)
if ($simulationTimeText -match
    '\b(DateTime|DateTimeOffset|TimeSpan|TimeProvider|Stopwatch|Environment\.TickCount)\b') {
    throw 'Simulation time must not depend on wall-clock or framework time types.'
}
if ($simulationTimeText -match '\b(double|float|decimal)\b') {
    throw 'Authoritative simulation time must use integer arithmetic only.'
}
if ($simulationTimeText -match '\bpublic\s+set\s*;') {
    throw 'Simulation time values must not expose public setters.'
}
if ($simulationTimeText -match
    '\b(Task\.Run|Task\.WhenAll|Parallel\.|Thread\.|Timer|Channel<)\b') {
    throw 'Simulation time primitives must not introduce hidden execution.'
}
if ($simulationTimeText -match
    '\b(File\.|Directory\.|FileStream|SqlConnection|HttpClient)\b') {
    throw 'Simulation time primitives must not perform I/O.'
}
if ($simulationTimeText -match
    '\b(NPC|Detective|Crime|Quest|Inventory|LLM)\b') {
    throw 'Generic simulation time code contains game or AI-domain vocabulary.'
}

$runtimeCommitFactSource =
    Get-Content -LiteralPath $runtimeCommitFactPath -Raw
if ($runtimeCommitFactSource -notmatch
    'public\s+sealed\s+class\s+RuntimeCommitFact\s*:\s*IEngineEvent') {
    throw 'The runtime commit completed-fact event contract is missing.'
}
foreach ($requiredProperty in @(
    'Kind',
    'WorldId',
    'PreviousWorldStateVersion',
    'CurrentWorldStateVersion',
    'PreviousSimulationTick',
    'CurrentSimulationTick',
    'CommandId'
)) {
    if ($runtimeCommitFactSource -notmatch
        "public\s+[\w<>,\.\?]+\s+$requiredProperty\s*\{\s*get;\s*\}") {
        throw "Runtime commit fact property '$requiredProperty' is missing or mutable."
    }
}
if ($runtimeCommitFactSource -notmatch
    'snapshot\.SimulationTick\s*-\s*1') {
    throw 'Simulation commit facts must record the exact previous tick.'
}
if ($runtimeCommitFactSource -notmatch
    'WorldStateVersion\.From\(current\.Value\s*-\s*1\)') {
    throw 'Runtime commit facts must record the exact previous version.'
}
if ($runtimeCommitFactSource -match '\bpublic\s+set\s*;') {
    throw 'Runtime commit facts must not expose public setters.'
}

$runtimeBuilderSource =
    Get-Content -LiteralPath $runtimeBuilderPath -Raw
if ($runtimeBuilderSource -notmatch
    'public\s+sealed\s+class\s+RuntimeOrchestratorBuilder<TState>') {
    throw 'The runtime orchestrator builder is missing.'
}
if ($runtimeBuilderSource -notmatch
    'commandBuilder\.Build\(worldStateManager\)') {
    throw 'Runtime commands must share the supplied World State Manager.'
}
if ($runtimeBuilderSource -notmatch
    'simulationBuilder\.Build\(worldStateManager\)') {
    throw 'Runtime simulation must share the supplied World State Manager.'
}

$runtimeOrchestratorSource =
    Get-Content -LiteralPath $runtimeOrchestratorPath -Raw
if ($runtimeOrchestratorSource -notmatch
    'public\s+sealed\s+class\s+RuntimeOrchestrator<TState>') {
    throw 'The caller-driven runtime orchestrator is missing.'
}
if ($runtimeOrchestratorSource -notmatch
    'Interlocked\.CompareExchange\s*\(') {
    throw 'Runtime operation admission must be immediate and atomic.'
}
if ($runtimeOrchestratorSource -notmatch
    'return\s+RuntimeCommandResult<TState>\.Busy\s*\(') {
    throw 'Concurrent command invocations must return Busy immediately.'
}
if ($runtimeOrchestratorSource -notmatch
    'return\s+RuntimeTickResult<TState>\.Busy\s*\(') {
    throw 'Concurrent tick invocations must return Busy immediately.'
}
if ($runtimeOrchestratorSource -notmatch
    'commandProcessor\.Execute\(envelope\)') {
    throw 'Runtime commands must delegate to Command Processor.'
}
if ($runtimeOrchestratorSource -notmatch
    'simulationScheduler\.RunNextTick\(\)') {
    throw 'Runtime ticks must delegate to Simulation Scheduler.'
}
if ($runtimeOrchestratorSource -match
    '\b(EventDispatcher|DispatchAsync|EventEnvelope)\b') {
    throw 'Runtime Orchestrator must not hide post-commit event dispatch.'
}
if ($runtimeOrchestratorSource -match
    '\b(lock\s*\(|Monitor\.|SemaphoreSlim|WaitHandle|ManualResetEvent|AutoResetEvent)\b') {
    throw 'Runtime Orchestrator must not wait or queue operations.'
}
if ($runtimeOrchestratorSource -match
    '\b(Task\.Run|Task\.WhenAll|Parallel\.|Thread\.|Timer|Channel<)\b') {
    throw 'Runtime Orchestrator must not create hidden execution.'
}
if ($runtimeOrchestratorSource -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7)|HttpClient)\b') {
    throw 'Runtime Orchestrator must not read time, generate IDs, or call providers.'
}
if ($runtimeOrchestratorSource -match
    '\b(File\.|Directory\.|FileStream|SqlConnection)\b') {
    throw 'Runtime Orchestrator must not perform persistence or I/O.'
}

$runtimeCommandResultSource =
    Get-Content -LiteralPath $runtimeCommandResultPath -Raw
if ($runtimeCommandResultSource -notmatch
    'commandResult\.WasApplied\s*\?\s*RuntimeCommitFact\.FromCommand') {
    throw 'Runtime command facts must exist only for applied commands.'
}

$runtimeTickResultSource =
    Get-Content -LiteralPath $runtimeTickResultPath -Raw
if ($runtimeTickResultSource -notmatch
    'simulationResult\.WasApplied\s*\?\s*RuntimeCommitFact\.FromSimulationTick') {
    throw 'Runtime tick facts must exist only for applied ticks.'
}

$runtimeProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Runtime') -Filter '*.cs'
$runtimeProductionText = (
    $runtimeProductionFiles |
        Get-Content -Raw |
        Out-String
)
if ($runtimeProductionText -match
    '\b(NPC|Detective|Crime|Quest|Inventory|LLM)\b') {
    throw 'Generic runtime orchestration contains game or AI-domain vocabulary.'
}
if ($runtimeProductionText -match
    '\b(Retry|Queue|Persist|Save|Load)\w*\s*\(') {
    throw 'Runtime orchestration must not hide retry, queue, or persistence behavior.'
}

$spatialDistanceSource =
    Get-Content -LiteralPath $spatialDistancePath -Raw
if ($spatialDistanceSource -notmatch
    'public\s+readonly\s+record\s+struct\s+SpatialDistance') {
    throw 'The deterministic integer spatial distance primitive is missing.'
}
if ($spatialDistanceSource -notmatch
    'public\s+ulong\s+Millimeters\s*\{\s*get;\s*\}') {
    throw 'Spatial Distance must use immutable integer millimeters.'
}
if ($spatialDistanceSource -notmatch
    'checked\(Millimeters\s*\+\s*other\.Millimeters\)') {
    throw 'Spatial Distance addition must use checked arithmetic.'
}

$spatialPointSource =
    Get-Content -LiteralPath $spatialPointPath -Raw
if ($spatialPointSource -notmatch
    'public\s+readonly\s+record\s+struct\s+SpatialPoint') {
    throw 'The deterministic local spatial point is missing.'
}
foreach ($coordinate in @(
    'XMillimeters',
    'YMillimeters',
    'ZMillimeters'
)) {
    if ($spatialPointSource -notmatch
        "public\s+long\s+$coordinate\s*\{\s*get;\s*\}") {
        throw "Spatial point coordinate '$coordinate' is missing or mutable."
    }
}
if ($spatialPointSource -notmatch
    'System\.UInt128\s+GetSquaredDistanceTo') {
    throw 'Spatial point distance must remain exact and squared.'
}
if ($spatialPointSource -notmatch
    'System\.Int128') {
    throw 'Spatial point differences must avoid signed 64-bit overflow.'
}
if ($spatialPointSource -notmatch
    'MaximumAbsoluteCoordinateMillimeters') {
    throw 'Spatial point coordinates must remain explicitly bounded.'
}
if ($spatialPointSource -match
    '\b(Math\.Sqrt|Vector2|Vector3|Quaternion|Matrix4x4)\b') {
    throw 'Core spatial points must not depend on floating geometry APIs.'
}

$spatialPositionSource =
    Get-Content -LiteralPath $spatialPositionPath -Raw
if ($spatialPositionSource -notmatch
    'public\s+readonly\s+record\s+struct\s+SpatialPosition\s*:\s*IComponent') {
    throw 'Spatial Position must remain immutable authoritative component data.'
}
if ($spatialPositionSource -notmatch
    'PlaceId\s*\{\s*get;\s*\}') {
    throw 'Spatial Position must contain stable place identity.'
}
if ($spatialPositionSource -notmatch
    'TryGetSquaredDistanceTo\s*\(') {
    throw 'Spatial Position must expose same-place exact distance checks.'
}

$spatialBuilderSource =
    Get-Content -LiteralPath $spatialBuilderPath -Raw
if ($spatialBuilderSource -notmatch
    'public\s+sealed\s+class\s+SpatialTopologyBuilder') {
    throw 'The deterministic spatial topology builder is missing.'
}
if ($spatialBuilderSource -notmatch
    'Spatial containment contains a cycle') {
    throw 'Spatial containment cycles must be rejected.'
}
if ($spatialBuilderSource -notmatch
    'ValidateConnectionEndpoints\(\)') {
    throw 'Spatial connection endpoints must be validated at build time.'
}
if ($spatialBuilderSource -notmatch
    'OrderBy\(place\s*=>\s*place\.PlaceId\)') {
    throw 'Spatial places must freeze in stable ID order.'
}
if ($spatialBuilderSource -notmatch
    'ThenBy\(connection\s*=>\s*connection\.ToPlaceId\)') {
    throw 'Spatial connections must freeze in stable endpoint order.'
}

$spatialTopologySource =
    Get-Content -LiteralPath $spatialTopologyPath -Raw
if ($spatialTopologySource -notmatch
    'public\s+sealed\s+class\s+SpatialTopology') {
    throw 'The immutable spatial topology is missing.'
}
if ($spatialTopologySource -notmatch
    'Array\.AsReadOnly') {
    throw 'Spatial topology collections must not expose mutable arrays.'
}
if ($spatialTopologySource -notmatch
    'IsContainedWithin\s*\(') {
    throw 'Spatial topology must expose hierarchy containment queries.'
}
if ($spatialTopologySource -notmatch
    'GetOutgoingConnections\s*\(') {
    throw 'Spatial topology must expose directed connection queries.'
}
if ($spatialTopologySource -match
    '\b(FindPath|ShortestPath|Dijkstra|AStar)\b') {
    throw 'Navigation policy must not be hidden inside the base spatial model.'
}

$spatialProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Spatial') -Filter '*.cs'
$spatialProductionText = (
    $spatialProductionFiles |
        Get-Content -Raw |
        Out-String
)
if ($spatialProductionText -match
    '\b(float|double|decimal)\b') {
    throw 'Authoritative spatial data must use integer arithmetic only.'
}
if ($spatialProductionText -match
    '\b(Unreal|Unity|NavMesh|Physics|Raycast|Transform)\b') {
    throw 'Core spatial data must not depend on presentation or engine geometry.'
}
if ($spatialProductionText -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7)|HttpClient)\b') {
    throw 'Spatial primitives must not read time, generate IDs, or call providers.'
}
if ($spatialProductionText -match
    '\b(Task\.Run|Task\.WhenAll|Parallel\.|Thread\.|Timer|Channel<)\b') {
    throw 'Spatial primitives must not introduce hidden execution.'
}
if ($spatialProductionText -match
    '\b(File\.|Directory\.|FileStream|SqlConnection)\b') {
    throw 'Spatial primitives must not perform persistence or I/O.'
}
if ($spatialProductionText -match
    '\b(NPC|Detective|Crime|Quest|Inventory|LLM)\b') {
    throw 'Generic spatial code contains game or AI-domain vocabulary.'
}
if ($spatialProductionText -match '\bpublic\s+set\s*;') {
    throw 'Spatial values must not expose public setters.'
}

$perceptionStimulusEnvelopeSource =
    Get-Content -LiteralPath $perceptionStimulusEnvelopePath -Raw
if ($perceptionStimulusEnvelopeSource -notmatch
    'public\s+sealed\s+class\s+PerceptionStimulusEnvelope<TStimulus>') {
    throw 'The version-gated perception stimulus envelope is missing.'
}
foreach ($requiredProperty in @(
    'StimulusId',
    'ChannelId',
    'ObserverEntityId',
    'WorldId',
    'ExpectedWorldStateVersion',
    'ExpectedSimulationTick',
    'Payload'
)) {
    if ($perceptionStimulusEnvelopeSource -notmatch
        "public\s+[\w<>,\.\?]+\s+$requiredProperty\s*\{\s*get;\s*\}") {
        throw "Perception stimulus property '$requiredProperty' is missing or mutable."
    }
}
if ($perceptionStimulusEnvelopeSource -notmatch
    'PerceptionTypePolicy\.EnsureConcrete<TStimulus>') {
    throw 'Perception stimuli must use exact concrete types.'
}

$perceptionDecisionSource =
    Get-Content -LiteralPath $perceptionDecisionPath -Raw
if ($perceptionDecisionSource -notmatch
    'public\s+sealed\s+class\s+PerceptionDecision<TSignal>') {
    throw 'The pure observed/ignored perception decision is missing.'
}
if ($perceptionDecisionSource -notmatch
    'public\s+bool\s+HasSignal\s*\{\s*get;\s*\}') {
    throw 'Perception decisions must explicitly distinguish value-type signals.'
}
if ($perceptionDecisionSource -notmatch
    'confidence\.EnsureUsableForObservation\(\)') {
    throw 'Observed signals must require initialized non-zero confidence.'
}
if ($perceptionDecisionSource -notmatch
    'PerceptionDecisionStatus\.Ignored') {
    throw 'Ignored perception decisions must remain explicit.'
}

$perceptionEvaluatorSource =
    Get-Content -LiteralPath $perceptionEvaluatorPath -Raw
if ($perceptionEvaluatorSource -notmatch
    'public\s+interface\s+IPerceptionEvaluator<TState,\s*TStimulus,\s*TSignal>') {
    throw 'The exact pure perception evaluator contract is missing.'
}
if ($perceptionEvaluatorSource -notmatch
    'public\s+PerceptionDecision<TSignal>\s+Evaluate\s*\(') {
    throw 'Perception evaluator members require explicit public accessibility.'
}
if ($perceptionEvaluatorSource -notmatch 'at most once') {
    throw 'Exactly-once evaluator execution policy must remain documented.'
}
$perceptionEvaluatorSemanticText =
    ($perceptionEvaluatorSource -replace '(?m)^\s*///\s?', '') `
        -replace '\s+', ' '
if ($perceptionEvaluatorSemanticText -notmatch
    'not automatically a fact, memory, belief, or item of knowledge') {
    throw 'Perception must remain semantically separate from knowledge and memory.'
}

$perceptionObservationSource =
    Get-Content -LiteralPath $perceptionObservationPath -Raw
if ($perceptionObservationSource -notmatch
    'public\s+sealed\s+class\s+PerceptionObservation<TSignal>') {
    throw 'The subjective perception observation is missing.'
}
foreach ($requiredProperty in @(
    'StimulusId',
    'ChannelId',
    'ObserverEntityId',
    'WorldId',
    'WorldStateVersion',
    'SimulationTick',
    'Confidence',
    'Signal'
)) {
    if ($perceptionObservationSource -notmatch
        "public\s+[\w<>,\.\?]+\s+$requiredProperty\s*\{\s*get;\s*\}") {
        throw "Perception observation property '$requiredProperty' is missing or mutable."
    }
}

$perceptionBuilderSource =
    Get-Content -LiteralPath $perceptionBuilderPath -Raw
if ($perceptionBuilderSource -notmatch
    'public\s+sealed\s+class\s+PerceptionProcessorBuilder<TState>') {
    throw 'The exact-type perception processor builder is missing.'
}
if ($perceptionBuilderSource -notmatch
    'new\s+PerceptionEvaluatorKey\s*\(\s*typeof\(TStimulus\),\s*typeof\(TSignal\)') {
    throw 'Perception evaluator registration must use exact stimulus/signal pairs.'
}

$perceptionProcessorSource =
    Get-Content -LiteralPath $perceptionProcessorPath -Raw
if ($perceptionProcessorSource -notmatch
    'public\s+sealed\s+class\s+PerceptionProcessor<TState>') {
    throw 'The read-only perception processor is missing.'
}
if ($perceptionProcessorSource -notmatch
    'envelope\.WorldId\s*!=\s*observedSnapshot\.WorldId') {
    throw 'Perception must reject a mismatched world before evaluation.'
}
if ($perceptionProcessorSource -notmatch
    'envelope\.ExpectedWorldStateVersion\s*!=\s*observedSnapshot\.Version') {
    throw 'Perception must reject stale versions before evaluation.'
}
if ($perceptionProcessorSource -notmatch
    'envelope\.ExpectedSimulationTick\s*!=\s*observedSnapshot\.SimulationTick') {
    throw 'Perception must reject stale logical ticks before evaluation.'
}
if ($perceptionProcessorSource -notmatch
    'evaluator\.Evaluate\(context\)') {
    throw 'Perception must evaluate exactly through the registered typed evaluator.'
}
if ($perceptionProcessorSource -notmatch
    'currentSnapshot\.Version\s*!=\s*observedSnapshot\.Version') {
    throw 'Perception must discard observations when authority changes during evaluation.'
}
if ($perceptionProcessorSource -match
    'TryApply\s*\(|CommandProcessor|SimulationScheduler') {
    throw 'Perception Processor must not mutate World State.'
}
if ($perceptionProcessorSource -match
    '\b(EventDispatcher|DispatchAsync|EventEnvelope)\b') {
    throw 'Perception Processor must not hide event dispatch.'
}
if ($perceptionProcessorSource -match
    '\b(Task\.Run|Task\.WhenAll|Parallel\.|Thread\.|Timer|Channel<)\b') {
    throw 'Perception Processor must not create hidden execution.'
}
if ($perceptionProcessorSource -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7)|HttpClient)\b') {
    throw 'Perception Processor must not read time, generate IDs, or call providers.'
}
if ($perceptionProcessorSource -match
    '\b(File\.|Directory\.|FileStream|SqlConnection)\b') {
    throw 'Perception Processor must not perform persistence or I/O.'
}

$perceptionProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Perception') -Filter '*.cs'
$perceptionProductionText = (
    $perceptionProductionFiles |
        Get-Content -Raw |
        Out-String
)
if ($perceptionProductionText -match
    '\b(Unreal|Unity|NavMesh|Raycast|Physics)\b') {
    throw 'Generic perception contracts must not depend on presentation geometry.'
}
if ($perceptionProductionText -match
    '\b(NPC|Detective|Crime|Quest|Inventory|LLM)\b') {
    throw 'Generic perception code contains game or AI-domain vocabulary.'
}
if ($perceptionProductionText -match
    '\b(MemoryStore|KnowledgeBase|BeliefStore)\b') {
    throw 'Perception must not hide memory, knowledge, or belief storage.'
}
if ($perceptionProductionText -match '\bpublic\s+set\s*;') {
    throw 'Perception contracts must not expose public setters.'
}

$knowledgeEvidenceSource =
    Get-Content -LiteralPath $knowledgeEvidencePath -Raw
if ($knowledgeEvidenceSource -notmatch
    'public\s+sealed\s+record\s+KnowledgeEvidenceReference') {
    throw 'The immutable knowledge evidence reference is missing.'
}
if ($knowledgeEvidenceSource -notmatch
    'FromPerception<TSignal>\s*\(') {
    throw 'Knowledge must preserve explicit perception provenance.'
}
if ($knowledgeEvidenceSource -notmatch
    'Communication evidence requires a source entity') {
    throw 'Communication evidence must retain its source entity.'
}

$knowledgeEntrySource =
    Get-Content -LiteralPath $knowledgeEntryPath -Raw
if ($knowledgeEntrySource -notmatch
    'public\s+sealed\s+class\s+KnowledgeEntry<TClaim>') {
    throw 'The current subjective knowledge entry is missing.'
}
if ($knowledgeEntrySource -notmatch
    'checked\(Revision\s*\+\s*1\)') {
    throw 'Knowledge revisions must advance with checked arithmetic.'
}
$knowledgeEntrySemanticText =
    ($knowledgeEntrySource -replace '(?m)^\s*///\s?', '') `
        -replace '\s+', ' '
if ($knowledgeEntrySemanticText -notmatch
    'Revision history is intentionally not retained here') {
    throw 'Knowledge must remain separate from episodic memory history.'
}

$knowledgeSetSource =
    Get-Content -LiteralPath $knowledgeSetPath -Raw
if ($knowledgeSetSource -notmatch
    'public\s+sealed\s+class\s+KnowledgeSet<TClaim>\s*:\s*IComponent') {
    throw 'Knowledge Set must remain immutable authoritative component data.'
}
if ($knowledgeSetSource -notmatch
    'OrderBy\(entry\s*=>\s*entry\.ClaimId\)') {
    throw 'Restored knowledge entries must freeze in stable claim-ID order.'
}
foreach ($statusName in @(
    'RevisionConflict',
    'EvidenceRegression',
    'EvidenceWorldMismatch',
    'EvidenceOwnerMismatch'
)) {
    if ($knowledgeSetSource -notmatch
        "KnowledgeMutationStatus\.$statusName") {
        throw "Knowledge Set is missing explicit status '$statusName'."
    }
}
if ($knowledgeSetSource -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7)|HttpClient)\b') {
    throw 'Knowledge Set must not read time, generate IDs, or call providers.'
}
if ($knowledgeSetSource -match
    '\b(Task\.Run|Task\.WhenAll|Parallel\.|Thread\.|Timer|Channel<)\b') {
    throw 'Knowledge Set must not create hidden execution.'
}
if ($knowledgeSetSource -match
    '\b(File\.|Directory\.|FileStream|SqlConnection)\b') {
    throw 'Knowledge Set must not perform persistence or I/O.'
}

$knowledgeMutationResultSource =
    Get-Content -LiteralPath $knowledgeMutationResultPath -Raw
if ($knowledgeMutationResultSource -notmatch
    'public\s+bool\s+WasApplied') {
    throw 'Knowledge mutations must expose explicit application status.'
}

$knowledgeProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Knowledge') -Filter '*.cs'
$knowledgeProductionText = (
    $knowledgeProductionFiles |
        Get-Content -Raw |
        Out-String
)
if ($knowledgeProductionText -match
    '\b(Unreal|Unity|NavMesh|Raycast|Physics)\b') {
    throw 'Generic knowledge contracts must not depend on presentation geometry.'
}
if ($knowledgeProductionText -match
    '\b(NPC|Detective|Crime|Quest|Inventory|LLM)\b') {
    throw 'Generic knowledge code contains game or AI-domain vocabulary.'
}
if ($knowledgeProductionText -match
    '\b(MemoryStore|EpisodicMemory|LongTermMemory)\b') {
    throw 'Knowledge must not hide memory storage.'
}
if ($knowledgeProductionText -match
    '\b(EventDispatcher|DispatchAsync|EventEnvelope)\b') {
    throw 'Knowledge values must not hide event dispatch.'
}
if ($knowledgeProductionText -match '\bpublic\s+set\s*;') {
    throw 'Knowledge contracts must not expose public setters.'
}

$memoryOriginSource =
    Get-Content -LiteralPath $memoryOriginPath -Raw
if ($memoryOriginSource -notmatch
    'public\s+sealed\s+record\s+MemoryOriginReference') {
    throw 'The immutable memory origin reference is missing.'
}
if ($memoryOriginSource -notmatch
    'FromPerception<TSignal>\s*\(') {
    throw 'Memory must preserve perception provenance.'
}
if ($memoryOriginSource -notmatch
    'FromKnowledge<TClaim>\s*\(') {
    throw 'Memory must preserve knowledge provenance.'
}
if ($memoryOriginSource -notmatch
    'Communication origin requires only a source entity') {
    throw 'Communication memories must retain their source entity.'
}

$memoryEntrySource =
    Get-Content -LiteralPath $memoryEntryPath -Raw
if ($memoryEntrySource -notmatch
    'public\s+sealed\s+class\s+MemoryEntry<TContent>') {
    throw 'The immutable memory entry is missing.'
}
if ($memoryEntrySource -notmatch
    'checked\(Revision\s*\+\s*1\)') {
    throw 'Memory revisions must advance with checked arithmetic.'
}
if ($memoryEntrySource -notmatch
    'public\s+uint\s+RecallPriority') {
    throw 'Memory entries must expose deterministic integer recall priority.'
}

$memoryStoreSource =
    Get-Content -LiteralPath $memoryStorePath -Raw
if ($memoryStoreSource -notmatch
    'public\s+sealed\s+class\s+MemoryStore<TContent>\s*:\s*IComponent') {
    throw 'Memory Store must remain immutable authoritative component data.'
}
if ($memoryStoreSource -notmatch
    'OrderBy\(entry\s*=>\s*entry\.MemoryId\)') {
    throw 'Restored memories must freeze in stable memory-ID order.'
}
foreach ($statusName in @(
    'RevisionConflict',
    'TemporalRegression',
    'OriginWorldMismatch',
    'OriginOwnerMismatch',
    'Forgotten'
)) {
    if ($memoryStoreSource -notmatch
        "MemoryMutationStatus\.$statusName") {
        throw "Memory Store is missing explicit status '$statusName'."
    }
}
if ($memoryStoreSource -notmatch
    'OrderByDescending\(entry\s*=>\s*entry\.RecallPriority\)') {
    throw 'Memory recall must use deterministic integer ranking.'
}
if ($memoryStoreSource -notmatch
    'ThenBy\(entry\s*=>\s*entry\.MemoryId\)') {
    throw 'Memory recall ties must resolve by stable memory ID.'
}
if ($memoryStoreSource -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7)|HttpClient)\b') {
    throw 'Memory Store must not read wall-clock time, generate IDs, or call providers.'
}
if ($memoryStoreSource -match
    '\b(Task\.Run|Task\.WhenAll|Parallel\.|Thread\.|Timer|Channel<)\b') {
    throw 'Memory Store must not create hidden execution or automatic decay.'
}
if ($memoryStoreSource -match
    '\b(File\.|Directory\.|FileStream|SqlConnection)\b') {
    throw 'Memory Store must not perform persistence or I/O.'
}

$memoryRecallQuerySource =
    Get-Content -LiteralPath $memoryRecallQueryPath -Raw
if ($memoryRecallQuerySource -notmatch
    'maximumResults\s+is\s+<\s*1\s+or\s+>\s*1_024') {
    throw 'Memory recall result bounds must remain explicit.'
}

$memoryMutationResultSource =
    Get-Content -LiteralPath $memoryMutationResultPath -Raw
if ($memoryMutationResultSource -notmatch
    'public\s+bool\s+WasApplied') {
    throw 'Memory mutations must expose explicit application status.'
}

$memoryProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Memory') -Filter '*.cs'
$memoryProductionText = (
    $memoryProductionFiles |
        Get-Content -Raw |
        Out-String
)
if ($memoryProductionText -match
    '\b(Unreal|Unity|NavMesh|Raycast|Physics)\b') {
    throw 'Generic memory contracts must not depend on presentation geometry.'
}
if ($memoryProductionText -match
    '\b(NPC|Detective|Crime|Quest|Inventory|LLM)\b') {
    throw 'Generic memory code contains game or AI-domain vocabulary.'
}
if ($memoryProductionText -match
    '\b(EventDispatcher|DispatchAsync|EventEnvelope)\b') {
    throw 'Memory values must not hide event dispatch.'
}
if ($memoryProductionText -match
    '\b(TryApply|CommandProcessor|SimulationScheduler)\b') {
    throw 'Memory values must not mutate or orchestrate World State.'
}
if ($memoryProductionText -match '\bpublic\s+set\s*;') {
    throw 'Memory contracts must not expose public setters.'
}

$relationshipChangeSource =
    Get-Content -LiteralPath $relationshipChangePath -Raw
if ($relationshipChangeSource -notmatch
    'public\s+sealed\s+record\s+RelationshipChangeReference') {
    throw 'The immutable relationship change reference is missing.'
}
if ($relationshipChangeSource -notmatch
    'OwnerEntityId\s*==\s*targetEntityId') {
    throw 'Relationship change provenance must reject self-directed targets.'
}
foreach ($factoryName in @(
    'FromPerception',
    'FromKnowledge',
    'FromMemory'
)) {
    if ($relationshipChangeSource -notmatch
        "public\s+static\s+RelationshipChangeReference\s+$factoryName") {
        throw "Relationship provenance factory '$factoryName' is missing."
    }
}
foreach ($propertyName in @(
    'KnowledgeClaimId',
    'KnowledgeEvidenceId',
    'MemoryId',
    'MemoryOriginId',
    'PerceptionStimulusId',
    'PerceptionChannelId'
)) {
    if ($relationshipChangeSource -notmatch
        "public\s+.*\s+$propertyName\s*\{\s*get;\s*\}") {
        throw "Relationship provenance property '$propertyName' is missing."
    }
}

$relationshipEntrySource =
    Get-Content -LiteralPath $relationshipEntryPath -Raw
if ($relationshipEntrySource -notmatch
    'public\s+sealed\s+class\s+RelationshipEntry<TState>') {
    throw 'The exact relationship entry is missing.'
}
if ($relationshipEntrySource -notmatch
    'if\s*\(\s*revision\s*==\s*0\s*\)') {
    throw 'Relationship revisions must remain positive.'
}
if ($relationshipEntrySource -notmatch
    'checked\(Revision\s*\+\s*1\)') {
    throw 'Relationship revision increments must remain checked.'
}
if ($relationshipEntrySource -notmatch
    'lastChange\.WorldStateVersion\s*!=\s*lastUpdatedWorldStateVersion') {
    throw 'Latest relationship change metadata must match the current entry.'
}

$relationshipSetSource =
    Get-Content -LiteralPath $relationshipSetPath -Raw
if ($relationshipSetSource -notmatch
    'public\s+sealed\s+class\s+RelationshipSet<TState>\s*:\s*IComponent') {
    throw 'Relationship Set must remain an immutable component.'
}
if ($relationshipSetSource -notmatch
    'OwnerEntityId') {
    throw 'Relationship Set must remain owner-scoped.'
}
if ($relationshipSetSource -notmatch
    'Array\.Sort\(') {
    throw 'Relationship restoration must sort targets deterministically.'
}
if ($relationshipSetSource -notmatch
    'left\.TargetEntityId\.CompareTo\(right\.TargetEntityId\)') {
    throw 'Relationship ordering must use stable target IDs.'
}
foreach ($operationName in @('Add', 'Revise', 'Remove')) {
    if ($relationshipSetSource -notmatch
        "public\s+RelationshipMutationResult<TState>\s+$operationName\s*\(") {
        throw "Relationship operation '$operationName' is missing."
    }
}
foreach ($statusName in @(
    'RelationshipAlreadyExists',
    'RelationshipNotFound',
    'RevisionConflict',
    'ChangeWorldMismatch',
    'ChangeOwnerMismatch',
    'ChangeTargetMismatch',
    'TemporalRegression',
    'Unchanged'
)) {
    if ($relationshipSetSource -notmatch
        "RelationshipMutationStatus\.$statusName") {
        throw "Relationship Set is missing explicit status '$statusName'."
    }
}

$relationshipMutationResultSource =
    Get-Content -LiteralPath $relationshipMutationResultPath -Raw
if ($relationshipMutationResultSource -notmatch
    'public\s+bool\s+WasApplied') {
    throw 'Relationship mutations must expose explicit application status.'
}

$relationshipProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Relationships') -Filter '*.cs'
$relationshipProductionText = (
    $relationshipProductionFiles |
        Get-Content -Raw |
        Out-String
)
if ($relationshipProductionText -match
    '\b(Trust|Love|Fear|Hate|Loyalty|Romance)\b') {
    throw 'Generic relationship code freezes product-specific social axes.'
}
if ($relationshipProductionText -match
    '\b(float|double|decimal)\b') {
    throw 'Generic relationship contracts must not freeze floating social axes.'
}
if ($relationshipProductionText -match
    '\b(Unreal|Unity|NavMesh|Raycast|Physics)\b') {
    throw 'Generic relationship contracts depend on presentation geometry.'
}
if ($relationshipProductionText -match
    '\b(NPC|Detective|Crime|Quest|Inventory|LLM)\b') {
    throw 'Generic relationship code contains game or AI-domain vocabulary.'
}
if ($relationshipProductionText -match
    '\b(EventDispatcher|DispatchAsync|EventEnvelope)\b') {
    throw 'Relationship values must not hide event dispatch.'
}
if ($relationshipProductionText -match
    '\b(TryApply|CommandProcessor|SimulationScheduler)\b') {
    throw 'Relationship values must not mutate or orchestrate World State.'
}
if ($relationshipProductionText -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7)|HttpClient)\b') {
    throw 'Relationship values must not read wall-clock time, generate IDs, or call providers.'
}
if ($relationshipProductionText -match
    '\b(Task\.Run|Task\.WhenAll|Parallel\.|Thread\.|Timer|Channel<)\b') {
    throw 'Relationship values must not create hidden execution or decay.'
}
if ($relationshipProductionText -match
    '\b(File\.|Directory\.|FileStream|SqlConnection)\b') {
    throw 'Relationship values must not perform persistence or I/O.'
}
if ($relationshipProductionText -match
    '\b(KnowledgeSet|MemoryStore)\b') {
    throw 'Relationship values must not rewrite Knowledge or Memory.'
}
if ($relationshipProductionText -match '\bpublic\s+set\s*;') {
    throw 'Relationship contracts must not expose public setters.'
}

$behaviorIntentEnvelopeSource =
    Get-Content -LiteralPath $behaviorIntentEnvelopePath -Raw
if ($behaviorIntentEnvelopeSource -notmatch
    'public\s+sealed\s+record\s+BehaviorIntentEnvelope<TIntent>') {
    throw 'The immutable behavior intent envelope is missing.'
}
foreach ($propertyName in @(
    'IntentId',
    'ActorEntityId',
    'WorldId',
    'WorldStateVersion',
    'SimulationTick',
    'Payload'
)) {
    if ($behaviorIntentEnvelopeSource -notmatch
        "public\s+[^\{;]+?\s+$propertyName\s*\{\s*get;\s*\}") {
        throw "Behavior intent property '$propertyName' is missing."
    }
}

$actionProposalEnvelopeSource =
    Get-Content -LiteralPath $actionProposalEnvelopePath -Raw
if ($actionProposalEnvelopeSource -notmatch
    'public\s+sealed\s+record\s+ActionProposalEnvelope<TAction>') {
    throw 'The immutable action proposal envelope is missing.'
}
foreach ($factoryName in @('CreateFromIntent', 'CreateExternal')) {
    if ($actionProposalEnvelopeSource -notmatch
        "public\s+static\s+ActionProposalEnvelope<TAction>\s+$factoryName") {
        throw "Action proposal factory '$factoryName' is missing."
    }
}
if ($actionProposalEnvelopeSource -notmatch
    'SourceIntentId\s*\{\s*get;\s*\}') {
    throw 'Action proposal intent provenance is missing.'
}

$actionDecisionSource = Get-Content -LiteralPath $actionDecisionPath -Raw
if ($actionDecisionSource -notmatch
    'public\s+sealed\s+class\s+ActionDecision<TCommand>') {
    throw 'The typed action decision is missing.'
}
foreach ($factoryName in @('Approve', 'Reject')) {
    if ($actionDecisionSource -notmatch
        "public\s+static\s+ActionDecision<TCommand>\s+$factoryName") {
        throw "Action decision factory '$factoryName' is missing."
    }
}
if ($actionDecisionSource -notmatch
    'where\s+TCommand\s*:\s*global::AI\.Sandbox\.Engine\.Core\.Commands\.IEngineCommand') {
    throw 'Approved action output must remain an exact engine command payload.'
}

$actionValidationProcessorSource =
    Get-Content -LiteralPath $actionValidationProcessorPath -Raw
if ($actionValidationProcessorSource -notmatch
    'public\s+sealed\s+class\s+ActionValidationProcessor<TState,\s*TAction,\s*TCommand>') {
    throw 'The action-validation processor is missing.'
}
$validatorCalls = [regex]::Matches(
    $actionValidationProcessorSource,
    'validator\.Evaluate\(context\)').Count
if ($validatorCalls -ne 1) {
    throw "Action validation must invoke the validator exactly once in source; found $validatorCalls."
}
foreach ($statusName in @(
    'WorldMismatch',
    'VersionConflict',
    'SimulationTickMismatch',
    'Approved',
    'Rejected'
)) {
    if ($actionValidationProcessorSource -notmatch
        "ActionValidationStatus\.$statusName") {
        throw "Action validation status '$statusName' is missing."
    }
}
if ($actionValidationProcessorSource -notmatch
    'var\s+before\s*=\s*manager\.Read\(\)') {
    throw 'Action validation preflight authority read is missing.'
}
if ($actionValidationProcessorSource -notmatch
    'var\s+after\s*=\s*manager\.Read\(\)') {
    throw 'Action validation post-evaluation authority read is missing.'
}

$behaviorProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Behavior') -Filter '*.cs'
$behaviorProductionText = (
    $behaviorProductionFiles |
        Get-Content -Raw |
        Out-String
)
foreach ($constraintPattern in @(
    'public\s+sealed\s+class\s+ActionValidationContext<TState,\s*TAction>[\s\S]*?where\s+TState\s*:\s*class,\s*global::AI\.Sandbox\.Engine\.Core\.WorldState\.IWorldState',
    'public\s+interface\s+IActionValidator<TState,\s*TAction,\s*TCommand>[\s\S]*?where\s+TState\s*:\s*class,\s*global::AI\.Sandbox\.Engine\.Core\.WorldState\.IWorldState',
    'public\s+sealed\s+class\s+ActionValidationProcessor<TState,\s*TAction,\s*TCommand>[\s\S]*?where\s+TState\s*:\s*class,\s*global::AI\.Sandbox\.Engine\.Core\.WorldState\.IWorldState'
)) {
    if ($behaviorProductionText -notmatch $constraintPattern) {
        throw 'Behavior validation types must preserve World State reference-type constraints.'
    }
}
if ($behaviorProductionText -match
    '\b(NPC|Detective|Crime|Quest|Inventory|Weapon|Door|NavMesh|LLM)\b') {
    throw 'Generic behavior contracts contain game or provider vocabulary.'
}
if ($behaviorProductionText -match
    '\b(TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor|SimulationScheduler)\b') {
    throw 'Behavior validation must not execute or apply authoritative changes.'
}
if ($behaviorProductionText -match
    '\b(EventDispatcher|DispatchAsync|EventEnvelope)\b') {
    throw 'Behavior validation must not hide event dispatch.'
}
if ($behaviorProductionText -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7)|HttpClient)\b') {
    throw 'Behavior validation must not read wall-clock time, generate IDs, or call providers.'
}
if ($behaviorProductionText -match
    '\b(Task\.Run|Task\.WhenAll|Parallel\.|Thread\.|Timer|Channel<)\b') {
    throw 'Behavior validation must not create hidden execution or retries.'
}
if ($behaviorProductionText -match
    '\b(File\.|Directory\.|FileStream|SqlConnection)\b') {
    throw 'Behavior validation must not perform persistence or I/O.'
}
if ($behaviorProductionText -match
    '\b(KnowledgeSet|MemoryStore|RelationshipSet)\b') {
    throw 'Behavior validation must not rewrite subjective state layers.'
}
if ($behaviorProductionText -match '\bpublic\s+set\s*;') {
    throw 'Behavior contracts must not expose public setters.'
}

$contextQueryEnvelopeSource =
    Get-Content -LiteralPath $contextQueryEnvelopePath -Raw
if ($contextQueryEnvelopeSource -notmatch
    'public\s+sealed\s+record\s+ContextQueryEnvelope<TQuery>') {
    throw 'The immutable context query envelope is missing.'
}
foreach ($propertyName in @(
    'QueryId',
    'OwnerEntityId',
    'WorldId',
    'WorldStateVersion',
    'SimulationTick',
    'ItemLimit',
    'Payload'
)) {
    if ($contextQueryEnvelopeSource -notmatch
        "public\s+[^\{;]+?\s+$propertyName\s*\{\s*get;\s*\}") {
        throw "Context query property '$propertyName' is missing."
    }
}

$contextItemEnvelopeSource =
    Get-Content -LiteralPath $contextItemEnvelopePath -Raw
if ($contextItemEnvelopeSource -notmatch
    'public\s+sealed\s+record\s+ContextItemEnvelope<TItem>') {
    throw 'The immutable context item envelope is missing.'
}
foreach ($propertyName in @(
    'ItemId',
    'RetrieverId',
    'OwnerEntityId',
    'WorldId',
    'Relevance',
    'Payload'
)) {
    if ($contextItemEnvelopeSource -notmatch
        "public\s+[^\{;]+?\s+$propertyName\s*\{\s*get;\s*\}") {
        throw "Context item property '$propertyName' is missing."
    }
}

$contextRetrievalDecisionSource =
    Get-Content -LiteralPath $contextRetrievalDecisionPath -Raw
if ($contextRetrievalDecisionSource -notmatch
    'public\s+sealed\s+class\s+ContextRetrievalDecision<TItem>') {
    throw 'The typed context retrieval decision is missing.'
}
foreach ($factoryName in @('Retrieve', 'Empty', 'Reject')) {
    if ($contextRetrievalDecisionSource -notmatch
        "public\s+static\s+ContextRetrievalDecision<TItem>\s+$factoryName") {
        throw "Context retrieval factory '$factoryName' is missing."
    }
}
if ($contextRetrievalDecisionSource -notmatch
    'right\.Relevance\.CompareTo\(left\.Relevance\)') {
    throw 'Context items must sort by descending relevance.'
}
if ($contextRetrievalDecisionSource -notmatch
    'left\.ItemId\.CompareTo\(right\.ItemId\)') {
    throw 'Context item tie-breaking must use stable item IDs.'
}

$contextRetrievalProcessorSource =
    Get-Content -LiteralPath $contextRetrievalProcessorPath -Raw
if ($contextRetrievalProcessorSource -notmatch
    'public\s+sealed\s+class\s+ContextRetrievalProcessor<TState,\s*TQuery,\s*TItem>') {
    throw 'The context retrieval processor is missing.'
}
$retrieverCalls = [regex]::Matches(
    $contextRetrievalProcessorSource,
    'retriever\.Retrieve\(context\)').Count
if ($retrieverCalls -ne 1) {
    throw "Context retrieval must invoke the retriever exactly once in source; found $retrieverCalls."
}
foreach ($statusName in @(
    'WorldMismatch',
    'VersionConflict',
    'SimulationTickMismatch',
    'ResultWorldMismatch',
    'ResultOwnerMismatch',
    'ResultRetrieverMismatch',
    'ItemLimitExceeded',
    'Retrieved',
    'Empty',
    'Rejected'
)) {
    if ($contextRetrievalProcessorSource -notmatch
        "ContextRetrievalStatus\.$statusName") {
        throw "Context retrieval status '$statusName' is missing."
    }
}
if ($contextRetrievalProcessorSource -notmatch
    'var\s+before\s*=\s*manager\.Read\(\)') {
    throw 'Context retrieval preflight authority read is missing.'
}
if ($contextRetrievalProcessorSource -notmatch
    'var\s+after\s*=\s*manager\.Read\(\)') {
    throw 'Context retrieval post-evaluation authority read is missing.'
}

$contextProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\ContextRetrieval') -Filter '*.cs'
$contextProductionText = (
    $contextProductionFiles |
        Get-Content -Raw |
        Out-String
)
foreach ($constraintPattern in @(
    'public\s+sealed\s+class\s+ContextRetrievalContext<TState,\s*TQuery>[\s\S]*?where\s+TState\s*:\s*class,\s*global::AI\.Sandbox\.Engine\.Core\.WorldState\.IWorldState',
    'public\s+interface\s+IContextRetriever<TState,\s*TQuery,\s*TItem>[\s\S]*?where\s+TState\s*:\s*class,\s*global::AI\.Sandbox\.Engine\.Core\.WorldState\.IWorldState',
    'public\s+sealed\s+class\s+ContextRetrievalProcessor<TState,\s*TQuery,\s*TItem>[\s\S]*?where\s+TState\s*:\s*class,\s*global::AI\.Sandbox\.Engine\.Core\.WorldState\.IWorldState'
)) {
    if ($contextProductionText -notmatch $constraintPattern) {
        throw 'Context retrieval types must preserve World State reference-type constraints.'
    }
}
if ($contextProductionText -match
    '\b(NPC|Detective|Crime|Quest|Inventory|Weapon|Door|NavMesh|LLM)\b') {
    throw 'Generic context retrieval contracts contain game or provider vocabulary.'
}
if ($contextProductionText -match
    '\b(TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor|SimulationScheduler)\b') {
    throw 'Context retrieval must not execute or apply authoritative changes.'
}
if ($contextProductionText -match
    '\b(EventDispatcher|DispatchAsync|EventEnvelope)\b') {
    throw 'Context retrieval must not hide event dispatch.'
}
if ($contextProductionText -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7)|HttpClient)\b') {
    throw 'Context retrieval must not read wall-clock time, generate IDs, or call providers.'
}
if ($contextProductionText -match
    '\b(Task\.Run|Task\.WhenAll|Parallel\.|Thread\.|Timer|Channel<)\b') {
    throw 'Context retrieval must not create hidden execution or retries.'
}
if ($contextProductionText -match
    '\b(File\.|Directory\.|FileStream|SqlConnection)\b') {
    throw 'Context retrieval must not perform persistence or I/O.'
}
if ($contextProductionText -match
    '\b(KnowledgeSet|MemoryStore|RelationshipSet)\b') {
    throw 'Context retrieval must not mutate or own subjective state stores.'
}
if ($contextProductionText -match '\bContextSourceKind\b') {
    throw 'Context retrieval must not freeze a closed source-kind enum.'
}
if ($contextProductionText -match '\bpublic\s+set\s*;') {
    throw 'Context retrieval contracts must not expose public setters.'
}


$promptRequestEnvelopeSource =
    Get-Content -LiteralPath $promptRequestEnvelopePath -Raw
if ($promptRequestEnvelopeSource -notmatch
    'public\s+sealed\s+record\s+PromptRequestEnvelope<TRequest>') {
    throw 'The immutable prompt request envelope is missing.'
}
foreach ($propertyName in @(
    'RequestId',
    'OwnerEntityId',
    'WorldId',
    'WorldStateVersion',
    'SimulationTick',
    'Budget',
    'Payload'
)) {
    if ($promptRequestEnvelopeSource -notmatch
        "public\s+[^\{;]+?\s+$propertyName\s*\{\s*get;\s*\}") {
        throw "Prompt request property '$propertyName' is missing."
    }
}

$promptCandidateEnvelopeSource =
    Get-Content -LiteralPath $promptCandidateEnvelopePath -Raw
if ($promptCandidateEnvelopeSource -notmatch
    'public\s+sealed\s+record\s+PromptCandidateEnvelope<TContent>') {
    throw 'The immutable prompt candidate envelope is missing.'
}
foreach ($propertyName in @(
    'CandidateId',
    'OwnerEntityId',
    'WorldId',
    'InclusionMode',
    'Priority',
    'Cost',
    'Payload'
)) {
    if ($promptCandidateEnvelopeSource -notmatch
        "public\s+[^\{;]+?\s+$propertyName\s*\{\s*get;\s*\}") {
        throw "Prompt candidate property '$propertyName' is missing."
    }
}

$promptBudgetManagerSource =
    Get-Content -LiteralPath $promptBudgetManagerPath -Raw
if ($promptBudgetManagerSource -notmatch
    'public\s+sealed\s+class\s+PromptBudgetManager<TContent>') {
    throw 'The deterministic prompt budget manager is missing.'
}
if ($promptBudgetManagerSource -notmatch
    'left\.InclusionMode\.CompareTo\(right\.InclusionMode\)') {
    throw 'Required prompt candidates must sort before optional candidates.'
}
if ($promptBudgetManagerSource -notmatch
    'right\.Priority\.CompareTo\(left\.Priority\)') {
    throw 'Optional prompt candidates must sort by descending priority.'
}
if ($promptBudgetManagerSource -notmatch
    'left\.CandidateId\.CompareTo\(right\.CandidateId\)') {
    throw 'Prompt candidate tie-breaking must use stable candidate IDs.'
}
if ($promptBudgetManagerSource -notmatch
    'PromptBudgetStatus\.RequiredBudgetExceeded') {
    throw 'Required prompt budget overflow must remain explicit.'
}

$promptCompositionResultPath = Join-Path $root 'src\AI.Sandbox.Engine.Core\Prompting\PromptCompositionResult.cs'
$promptCompositionResultSource =
    Get-Content -LiteralPath $promptCompositionResultPath -Raw
if ($promptCompositionResultSource -notmatch
    'BudgetRejected[\s\S]*?PromptCompositionStatus\.RequiredBudgetExceeded') {
    throw 'Required-budget rejection must map to PromptCompositionStatus.RequiredBudgetExceeded.'
}

$promptCompositionDecisionSource =
    Get-Content -LiteralPath $promptCompositionDecisionPath -Raw
foreach ($factoryName in @('Compose', 'Reject')) {
    if ($promptCompositionDecisionSource -notmatch
        "public\s+static\s+PromptCompositionDecision<TDocument>\s+$factoryName") {
        throw "Prompt composition factory '$factoryName' is missing."
    }
}

$promptCompositionProcessorSource =
    Get-Content -LiteralPath $promptCompositionProcessorPath -Raw
if ($promptCompositionProcessorSource -notmatch
    'public\s+sealed\s+class\s+PromptCompositionProcessor<') {
    throw 'The prompt composition processor is missing.'
}
$composerCalls = [regex]::Matches(
    $promptCompositionProcessorSource,
    'composer\.Compose\(context\)').Count
if ($composerCalls -ne 1) {
    throw "Prompt composition must invoke the composer exactly once in source; found $composerCalls."
}
if ($promptCompositionProcessorSource -notmatch
    'var\s+before\s*=\s*manager\.Read\(\)') {
    throw 'Prompt composition preflight authority read is missing.'
}
if ($promptCompositionProcessorSource -notmatch
    'var\s+after\s*=\s*manager\.Read\(\)') {
    throw 'Prompt composition post-composer authority read is missing.'
}
$promptProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Prompting') -Filter '*.cs'
$promptProductionText = (
    $promptProductionFiles |
        Get-Content -Raw |
        Out-String
)
foreach ($statusName in @(
    'WorldMismatch',
    'VersionConflict',
    'SimulationTickMismatch',
    'CandidateWorldMismatch',
    'CandidateOwnerMismatch',
    'DuplicateCandidate',
    'RequiredBudgetExceeded',
    'ResultWorldMismatch',
    'ResultOwnerMismatch',
    'ResultComposerMismatch',
    'ResultBudgetExceeded',
    'Composed',
    'Rejected'
)) {
    if ($promptProductionText -notmatch
        "PromptCompositionStatus\.$statusName") {
        throw "Prompt composition status '$statusName' is missing from the Prompting production contract."
    }
}
foreach ($constraintPattern in @(
    'public\s+sealed\s+class\s+PromptCompositionContext<TState,\s*TRequest,\s*TContent>[\s\S]*?where\s+TState\s*:\s*class,\s*global::AI\.Sandbox\.Engine\.Core\.WorldState\.IWorldState',
    'public\s+interface\s+IPromptComposer<TState,\s*TRequest,\s*TContent,\s*TDocument>[\s\S]*?where\s+TState\s*:\s*class,\s*global::AI\.Sandbox\.Engine\.Core\.WorldState\.IWorldState',
    'public\s+sealed\s+class\s+PromptCompositionProcessor<[\s\S]*?where\s+TState\s*:\s*class,\s*global::AI\.Sandbox\.Engine\.Core\.WorldState\.IWorldState'
)) {
    if ($promptProductionText -notmatch $constraintPattern) {
        throw 'Prompt composition types must preserve World State reference constraints.'
    }
}
if ($promptProductionText -match
    '\b(NPC|Detective|Crime|Quest|Inventory|Weapon|Door|NavMesh)\b') {
    throw 'Generic prompt composition contracts contain game vocabulary.'
}
if ($promptProductionText -match
    '\b(OpenAI|Anthropic|Gemini|Tokenizer|Tiktoken|HttpClient)\b') {
    throw 'Prompt composition must remain provider and tokenizer neutral.'
}
if ($promptProductionText -match
    '\b(ContextRetrievalProcessor|IContextRetriever|\.Retrieve\()\b') {
    throw 'Prompt composition must consume supplied candidates, not perform retrieval.'
}
if ($promptProductionText -match
    '\b(TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor|SimulationScheduler)\b') {
    throw 'Prompt composition must not execute or apply authoritative changes.'
}
if ($promptProductionText -match
    '\b(EventDispatcher|DispatchAsync|EventEnvelope)\b') {
    throw 'Prompt composition must not hide event dispatch.'
}
if ($promptProductionText -match
    '\b(KnowledgeSet|MemoryStore|RelationshipSet)\b') {
    throw 'Prompt composition must not own or mutate subjective stores.'
}
if ($promptProductionText -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7)|Timer)\b') {
    throw 'Prompt composition must not read clocks, generate IDs, or schedule work.'
}
if ($promptProductionText -match
    '\b(Task\.Run|Task\.WhenAll|Parallel\.|Thread\.|Channel<)\b') {
    throw 'Prompt composition must not create hidden execution or retries.'
}
if ($promptProductionText -match
    '\b(File\.|Directory\.|FileStream|SqlConnection)\b') {
    throw 'Prompt composition must not perform persistence or I/O.'
}
if ($promptProductionText -match '\bpublic\s+set\s*;') {
    throw 'Prompt composition contracts must not expose public setters.'
}

$modelInvocationRequestSource =
    Get-Content -LiteralPath $modelInvocationRequestPath -Raw
if ($modelInvocationRequestSource -notmatch
    'public\s+sealed\s+record\s+ModelInvocationRequestEnvelope<TRequest>') {
    throw 'The immutable model invocation request envelope is missing.'
}
foreach ($propertyName in @(
    'InvocationId',
    'AdapterId',
    'ModelProfileId',
    'PromptDocumentId',
    'OwnerEntityId',
    'WorldId',
    'WorldStateVersion',
    'SimulationTick',
    'OutputLimit',
    'Payload'
)) {
    if ($modelInvocationRequestSource -notmatch
        "public\s+[^\{;]+?\s+$propertyName\s*\{\s*get;\s*\}") {
        throw "Model request property '$propertyName' is missing."
    }
}

$modelInvocationResponseSource =
    Get-Content -LiteralPath $modelInvocationResponsePath -Raw
if ($modelInvocationResponseSource -notmatch
    'public\s+sealed\s+record\s+ModelInvocationResponseEnvelope<TResponse>') {
    throw 'The immutable model invocation response envelope is missing.'
}
foreach ($propertyName in @(
    'InvocationId',
    'AdapterId',
    'ModelProfileId',
    'PromptDocumentId',
    'OwnerEntityId',
    'WorldId',
    'WorldStateVersion',
    'SimulationTick',
    'Usage',
    'Payload'
)) {
    if ($modelInvocationResponseSource -notmatch
        "public\s+[^\{;]+?\s+$propertyName\s*\{\s*get;\s*\}") {
        throw "Model response property '$propertyName' is missing."
    }
}

$modelAdapterSource = Get-Content -LiteralPath $modelAdapterPath -Raw
if ($modelAdapterSource -notmatch
    'public\s+interface\s+IModelAdapter<TRequest,\s*TResponse>') {
    throw 'The exact provider-neutral model adapter interface is missing.'
}
if ($modelAdapterSource -notmatch
    'ValueTask<ModelInvocationDecision<TResponse>>\s+InvokeAsync') {
    throw 'The model adapter boundary must remain explicitly asynchronous.'
}

$modelInvocationDecisionSource =
    Get-Content -LiteralPath $modelInvocationDecisionPath -Raw
foreach ($factoryName in @('Complete', 'Reject', 'Fail')) {
    if ($modelInvocationDecisionSource -notmatch
        "public\s+static\s+ModelInvocationDecision<TResponse>\s+$factoryName") {
        throw "Model invocation decision factory '$factoryName' is missing."
    }
}

$modelInvocationProcessorSource =
    Get-Content -LiteralPath $modelInvocationProcessorPath -Raw
if ($modelInvocationProcessorSource -notmatch
    'public\s+sealed\s+class\s+ModelInvocationProcessor<TRequest,\s*TResponse>') {
    throw 'The model invocation processor is missing.'
}
$modelAdapterCalls = [regex]::Matches(
    $modelInvocationProcessorSource,
    'adapter[\s\S]*?\.InvokeAsync\(context,\s*cancellationToken\)').Count
if ($modelAdapterCalls -ne 1) {
    throw "Model invocation must call the adapter exactly once in source; found $modelAdapterCalls."
}
if ($modelInvocationProcessorSource -notmatch '\.ConfigureAwait\(false\)') {
    throw 'The asynchronous model adapter call must avoid synchronization-context capture.'
}

$modelProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Modeling') -Filter '*.cs'
$modelProductionText = (
    $modelProductionFiles |
        Get-Content -Raw |
        Out-String
)
foreach ($statusName in @(
    'Completed',
    'Rejected',
    'Failed',
    'RequestAdapterMismatch',
    'ResponseInvocationMismatch',
    'ResponseAdapterMismatch',
    'ResponseProfileMismatch',
    'ResponsePromptDocumentMismatch',
    'ResponseOwnerMismatch',
    'ResponseWorldMismatch',
    'ResponseVersionMismatch',
    'ResponseSimulationTickMismatch',
    'ResponseOutputLimitExceeded'
)) {
    if ($modelProductionText -notmatch "ModelInvocationStatus\.$statusName") {
        throw "Model invocation status '$statusName' is missing."
    }
}
if ($modelProductionText -match
    '\b(OpenAI|Anthropic|Gemini|AzureOpenAI|Ollama|Groq|Bedrock)\b') {
    throw 'Provider-neutral model contracts contain a concrete provider name.'
}
if ($modelProductionText -match
    '\b(HttpClient|HttpRequestMessage|HttpResponseMessage|Socket|WebSocket|GrpcChannel)\b') {
    throw 'Generic model contracts must not implement provider transport.'
}
if ($modelProductionText -match
    '\b(Task\.Delay|Task\.Run|Polly|Retry|Backoff|Timer|PeriodicTimer)\b') {
    throw 'Generic model contracts must not hide retries, backoff, or scheduling.'
}
if ($modelProductionText -match
    '\b(WorldStateManager|TryApply|RuntimeOrchestrator|CommandProcessor|SimulationScheduler)\b') {
    throw 'Model invocation must not access or mutate authoritative state.'
}
if ($modelProductionText -match
    '\b(EventDispatcher|DispatchAsync|KnowledgeSet|MemoryStore|RelationshipSet)\b') {
    throw 'Model invocation must not dispatch events or mutate subjective stores.'
}
if ($modelProductionText -match
    '\b(PromptCompositionProcessor|IPromptComposer|ContextRetrievalProcessor)\b') {
    throw 'Model invocation must consume an explicit request, not compose or retrieve context.'
}
if ($modelProductionText -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7))\b') {
    throw 'Model invocation must not read clocks or generate identities.'
}
if ($modelProductionText -match
    '\b(File\.|Directory\.|FileStream|SqlConnection)\b') {
    throw 'Generic model contracts must not perform persistence or filesystem I/O.'
}
if ($modelProductionText -match
    '\b(NPC|Detective|Crime|Quest|Inventory|Weapon|Door|NavMesh)\b') {
    throw 'Generic model contracts contain game vocabulary.'
}
if ($modelProductionText -match '\bpublic\s+set\s*;') {
    throw 'Provider-neutral model contracts must not expose public setters.'
}

$speechInvocationRequestSource =
    Get-Content -LiteralPath $speechInvocationRequestPath -Raw
if ($speechInvocationRequestSource -notmatch
    'public\s+sealed\s+record\s+SpeechInvocationRequestEnvelope<TRequest>') {
    throw 'The immutable speech invocation request envelope is missing.'
}
foreach ($propertyName in @(
    'OperationKind',
    'InvocationId',
    'AdapterId',
    'SpeechProfileId',
    'OwnerEntityId',
    'WorldId',
    'WorldStateVersion',
    'SimulationTick',
    'InputLimit',
    'OutputLimit',
    'Payload'
)) {
    if ($speechInvocationRequestSource -notmatch
        "public\s+[^\{;]+?\s+$propertyName\s*\{\s*get;\s*\}") {
        throw "Speech request property '$propertyName' is missing."
    }
}

$speechInvocationResponseSource =
    Get-Content -LiteralPath $speechInvocationResponsePath -Raw
if ($speechInvocationResponseSource -notmatch
    'public\s+sealed\s+record\s+SpeechInvocationResponseEnvelope<TResponse>') {
    throw 'The immutable speech invocation response envelope is missing.'
}
foreach ($propertyName in @(
    'OperationKind',
    'InvocationId',
    'AdapterId',
    'SpeechProfileId',
    'OwnerEntityId',
    'WorldId',
    'WorldStateVersion',
    'SimulationTick',
    'Usage',
    'Payload'
)) {
    if ($speechInvocationResponseSource -notmatch
        "public\s+[^\{;]+?\s+$propertyName\s*\{\s*get;\s*\}") {
        throw "Speech response property '$propertyName' is missing."
    }
}

$speechAdapterSource = Get-Content -LiteralPath $speechAdapterPath -Raw
if ($speechAdapterSource -notmatch
    'public\s+interface\s+ISpeechAdapter<TRequest,\s*TResponse>') {
    throw 'The exact provider-neutral speech adapter interface is missing.'
}
if ($speechAdapterSource -notmatch
    'ValueTask<SpeechInvocationDecision<TResponse>>\s+InvokeAsync') {
    throw 'The speech adapter boundary must remain explicitly asynchronous.'
}

$speechInvocationDecisionSource =
    Get-Content -LiteralPath $speechInvocationDecisionPath -Raw
foreach ($factoryName in @('Complete', 'Reject', 'Fail')) {
    if ($speechInvocationDecisionSource -notmatch
        "public\s+static\s+SpeechInvocationDecision<TResponse>\s+$factoryName") {
        throw "Speech invocation decision factory '$factoryName' is missing."
    }
}

$speechInvocationProcessorSource =
    Get-Content -LiteralPath $speechInvocationProcessorPath -Raw
if ($speechInvocationProcessorSource -notmatch
    'public\s+sealed\s+class\s+SpeechInvocationProcessor<TRequest,\s*TResponse>') {
    throw 'The speech invocation processor is missing.'
}
$speechAdapterCalls = [regex]::Matches(
    $speechInvocationProcessorSource,
    'adapter[\s\S]*?\.InvokeAsync\(context,\s*cancellationToken\)').Count
if ($speechAdapterCalls -ne 1) {
    throw "Speech invocation must call the adapter exactly once in source; found $speechAdapterCalls."
}
if ($speechInvocationProcessorSource -notmatch '\.ConfigureAwait\(false\)') {
    throw 'The asynchronous speech adapter call must avoid synchronization-context capture.'
}

$speechProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Speech') -Filter '*.cs'
$speechProductionText = (
    $speechProductionFiles |
        Get-Content -Raw |
        Out-String
)
foreach ($operationSpec in @(
    @{ Name = 'Recognition'; Value = 0 }
    @{ Name = 'Synthesis'; Value = 1 }
)) {
    $operationName = [string]$operationSpec.Name
    $operationValue = [int]$operationSpec.Value
    if ($speechProductionText -notmatch
        "(?m)^\s*$operationName\s*=\s*$operationValue\s*,?\s*$") {
        throw "Speech operation '$operationName' is missing or renumbered."
    }
}
foreach ($statusName in @(
    'Completed',
    'Rejected',
    'Failed',
    'RequestAdapterMismatch',
    'ResponseOperationMismatch',
    'ResponseInvocationMismatch',
    'ResponseAdapterMismatch',
    'ResponseProfileMismatch',
    'ResponseOwnerMismatch',
    'ResponseWorldMismatch',
    'ResponseVersionMismatch',
    'ResponseSimulationTickMismatch',
    'ResponseInputLimitExceeded',
    'ResponseOutputLimitExceeded'
)) {
    if ($speechProductionText -notmatch "SpeechInvocationStatus\.$statusName") {
        throw "Speech invocation status '$statusName' is missing."
    }
}
if ($speechProductionText -match
    '\b(OpenAI|Anthropic|Gemini|Azure|Whisper|ElevenLabs|Polly|Deepgram)\b') {
    throw 'Provider-neutral speech contracts contain a concrete provider name.'
}
if ($speechProductionText -match
    '\b(HttpClient|HttpRequestMessage|HttpResponseMessage|Socket|WebSocket|GrpcChannel)\b') {
    throw 'Generic speech contracts must not implement provider transport.'
}
if ($speechProductionText -match
    '\b(Task\.Delay|Task\.Run|Retry|Backoff|Timer|PeriodicTimer)\b') {
    throw 'Generic speech contracts must not hide retries, backoff, or scheduling.'
}
if ($speechProductionText -match
    '\b(WorldStateManager|TryApply|RuntimeOrchestrator|CommandProcessor|SimulationScheduler)\b') {
    throw 'Speech invocation must not access or mutate authoritative state.'
}
if ($speechProductionText -match
    '\b(EventDispatcher|DispatchAsync|KnowledgeSet|MemoryStore|RelationshipSet)\b') {
    throw 'Speech invocation must not dispatch events or mutate subjective stores.'
}
if ($speechProductionText -match
    '\b(ModelInvocationProcessor|IModelAdapter|PromptCompositionProcessor|ContextRetrievalProcessor)\b') {
    throw 'Speech invocation must remain separate from model and prompt orchestration.'
}
if ($speechProductionText -match
    '\b(AudioDevice|Microphone|Speaker|NAudio|FFmpeg|PortAudio)\b') {
    throw 'Generic speech contracts must not capture, play, or transcode audio.'
}
if ($speechProductionText -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7))\b') {
    throw 'Speech invocation must not read clocks or generate identities.'
}
if ($speechProductionText -match
    '\b(File\.|Directory\.|FileStream|SqlConnection)\b') {
    throw 'Generic speech contracts must not perform persistence or filesystem I/O.'
}
if ($speechProductionText -match
    '\b(NPC|Detective|Crime|Quest|Inventory|Weapon|Door|NavMesh)\b') {
    throw 'Generic speech contracts contain game vocabulary.'
}
if ($speechProductionText -match '\bpublic\s+set\s*;') {
    throw 'Provider-neutral speech contracts must not expose public setters.'
}

$conversationStateSource = Get-Content -LiteralPath $conversationStatePath -Raw
if ($conversationStateSource -notmatch
    'public\s+sealed\s+class\s+ConversationState<TTopic>') {
    throw 'The immutable generic conversation state is missing.'
}
foreach ($methodName in @('Start', 'RecordTurn', 'ReviseTopic', 'Close')) {
    if ($conversationStateSource -notmatch
        "public\s+[^\n]+\s+$methodName\s*\(") {
        throw "Conversation state method '$methodName' is missing."
    }
}
if ($conversationStateSource -notmatch
    'OrderBy\(static\s+participant\s*=>\s*participant\)') {
    throw 'Conversation participant ordering must remain deterministic.'
}
if ($conversationStateSource -notmatch
    'expectedRevision\s*!=\s*Revision') {
    throw 'Conversation mutations must preserve optimistic revision checks.'
}

$addressResolutionRequestSource =
    Get-Content -LiteralPath $addressResolutionRequestPath -Raw
if ($addressResolutionRequestSource -notmatch
    'public\s+sealed\s+record\s+AddressResolutionRequestEnvelope<TQuery>') {
    throw 'The immutable address-resolution request envelope is missing.'
}
foreach ($propertyName in @(
    'ResolutionId',
    'ResolverId',
    'ConversationId',
    'SpeakerEntityId',
    'WorldId',
    'WorldStateVersion',
    'SimulationTick',
    'ExpectedConversationRevision',
    'Payload'
)) {
    if ($addressResolutionRequestSource -notmatch
        "public\s+[^\{;]+?\s+$propertyName\s*\{\s*get;\s*\}") {
        throw "Address request property '$propertyName' is missing."
    }
}

$addressResolverSource = Get-Content -LiteralPath $addressResolverPath -Raw
if ($addressResolverSource -notmatch
    'public\s+interface\s+IAddressResolver<TWorldState,\s*TQuery,\s*TTopic>') {
    throw 'The exact address resolver interface is missing.'
}
if ($addressResolverSource -notmatch
    'AddressResolutionDecision\s+Resolve\(') {
    throw 'The address resolver must expose one explicit Resolve method.'
}

$addressResolutionProcessorSource =
    Get-Content -LiteralPath $addressResolutionProcessorPath -Raw
if ($addressResolutionProcessorSource -notmatch
    'public\s+sealed\s+class\s+AddressResolutionProcessor<TWorldState,\s*TQuery,\s*TTopic>') {
    throw 'The address-resolution processor is missing.'
}
$addressResolverCalls = [regex]::Matches(
    $addressResolutionProcessorSource,
    'resolver\.Resolve\(context\)').Count
if ($addressResolverCalls -ne 1) {
    throw "Address resolution must call the resolver exactly once in source; found $addressResolverCalls."
}
$authorityReads = [regex]::Matches(
    $addressResolutionProcessorSource,
    'worldStateManager\.Read\(\)').Count
if ($authorityReads -ne 2) {
    throw "Address resolution must read authority exactly before and after resolution; found $authorityReads."
}

$conversationProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Conversation') -Filter '*.cs'
$conversationProductionText = (
    $conversationProductionFiles |
        Get-Content -Raw |
        Out-String
)
foreach ($audienceSpec in @(
    @{ Name = 'None'; Value = 0 }
    @{ Name = 'SpecificParticipants'; Value = 1 }
    @{ Name = 'AllParticipants'; Value = 2 }
)) {
    $audienceName = [string]$audienceSpec.Name
    $audienceValue = [int]$audienceSpec.Value
    if ($conversationProductionText -notmatch
        "(?m)^\s*$audienceName\s*=\s*$audienceValue\s*,?\s*$") {
        throw "Address audience '$audienceName' is missing or renumbered."
    }
}
foreach ($statusName in @(
    'Resolved',
    'Rejected',
    'ResolverMismatch',
    'WorldMismatch',
    'VersionConflict',
    'SimulationTickMismatch',
    'ConversationMismatch',
    'ConversationWorldMismatch',
    'ConversationRevisionConflict',
    'ConversationClosed',
    'SpeakerNotParticipant',
    'AuthorityChanged',
    'AudienceInvalid'
)) {
    if ($conversationProductionText -notmatch
        "(?m)^\s*$statusName\s*=\s*\d+\s*,?\s*$") {
        throw "Address-resolution status '$statusName' is missing."
    }
}
if ($conversationProductionText -match
    '\b(OpenAI|Anthropic|Gemini|Azure|Whisper|ElevenLabs|Deepgram)\b') {
    throw 'Conversation contracts contain a concrete model or speech provider.'
}
if ($conversationProductionText -match
    '\b(HttpClient|HttpRequestMessage|HttpResponseMessage|Socket|WebSocket|GrpcChannel)\b') {
    throw 'Conversation contracts must not implement provider transport.'
}
if ($conversationProductionText -match
    '\b(Task\.Delay|Task\.Run|Retry|Backoff|Timer|PeriodicTimer)\b') {
    throw 'Conversation contracts must not hide retries or scheduling.'
}
if ($conversationProductionText -match
    '\b(ModelInvocationProcessor|SpeechInvocationProcessor|PromptCompositionProcessor|ContextRetrievalProcessor)\b') {
    throw 'Conversation contracts must not own model, speech, prompt, or retrieval orchestration.'
}
if ($conversationProductionText -match
    '\b(TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor|SimulationScheduler)\b') {
    throw 'Conversation contracts must not mutate or execute authoritative state.'
}
if ($conversationProductionText -match
    '\b(EventDispatcher|DispatchAsync|KnowledgeSet|MemoryStore|RelationshipSet)\b') {
    throw 'Conversation contracts must not dispatch events or mutate subjective stores.'
}
if ($conversationProductionText -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7)|FileStream|SqlConnection)\b') {
    throw 'Conversation contracts contain hidden clocks, generated IDs, or I/O.'
}
if ($conversationProductionText -match '\bpublic\s+set\s*;') {
    throw 'Conversation contracts must not expose public setters.'
}
if ($conversationProductionText -match
    '\b(Crime|Murder|Detective|Quest|Unreal|Unity|Blueprint|Actor|Pawn)\b') {
    throw 'Generic conversation contracts contain game-specific vocabulary.'
}

$socialTurnRequestSource = Get-Content -LiteralPath $socialTurnRequestPath -Raw
if ($socialTurnRequestSource -notmatch
    'public\s+sealed\s+class\s+SocialTurnCoordinationRequestEnvelope<TProposal>') {
    throw 'The immutable social turn request envelope is missing.'
}
foreach ($propertyName in @(
    'CoordinationId',
    'CoordinatorId',
    'ConversationId',
    'CurrentSpeakerEntityId',
    'Audience',
    'WorldId',
    'WorldStateVersion',
    'SimulationTick',
    'ExpectedConversationRevision',
    'Proposals'
)) {
    if ($socialTurnRequestSource -notmatch
        "public\s+[^\{;]+?\s+$propertyName\s*(=>|\{\s*get;\s*\})") {
        throw "Social turn request property '$propertyName' is missing."
    }
}
if ($socialTurnRequestSource -notmatch
    'OrderByDescending\([\s\S]*?proposal\.Priority\.BasisPoints\)[\s\S]*?ThenBy\(static\s+proposal\s*=>\s*proposal\.ProposalId\)') {
    throw 'Social turn proposals must remain deterministically ordered.'
}
if ($socialTurnRequestSource -notmatch
    'Each participant may submit at most one proposal') {
    throw 'Social turn requests must preserve one proposal per participant.'
}

$socialTurnCoordinatorSource =
    Get-Content -LiteralPath $socialTurnCoordinatorPath -Raw
if ($socialTurnCoordinatorSource -notmatch
    'public\s+interface\s+ISocialTurnCoordinator<TWorldState,\s*TProposal,\s*TTopic>') {
    throw 'The exact social turn coordinator interface is missing.'
}
if ($socialTurnCoordinatorSource -notmatch
    'SocialTurnCoordinationDecision\s+Coordinate\(') {
    throw 'The social turn coordinator must expose one Coordinate method.'
}

$socialTurnProcessorSource =
    Get-Content -LiteralPath $socialTurnProcessorPath -Raw
if ($socialTurnProcessorSource -notmatch
    'public\s+sealed\s+class\s+SocialTurnCoordinationProcessor<') {
    throw 'The social turn coordination processor is missing.'
}
$socialCoordinatorCalls = [regex]::Matches(
    $socialTurnProcessorSource,
    'coordinator\.Coordinate\(context\)').Count
if ($socialCoordinatorCalls -ne 1) {
    throw "Social turn coordination must call the coordinator exactly once in source; found $socialCoordinatorCalls."
}
$socialAuthorityReads = [regex]::Matches(
    $socialTurnProcessorSource,
    'worldStateManager\.Read\(\)').Count
if ($socialAuthorityReads -ne 2) {
    throw "Social turn coordination must read authority exactly before and after coordination; found $socialAuthorityReads."
}
if ($socialTurnProcessorSource -notmatch
    'request\.Proposals\.Count\s*==\s*0') {
    throw 'Social turn coordination must explicitly skip empty proposal sets.'
}
if ($socialTurnProcessorSource -notmatch
    'conversation\.LastTurn\s+is\s+null') {
    throw 'Social turn coordination must require a completed current turn.'
}
if ($socialTurnProcessorSource -notmatch
    'AudienceEquals\(conversation\.LastTurn\.Audience,\s*request\.Audience\)') {
    throw 'Social turn coordination must correlate the current audience.'
}

$socialProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Social') -Filter '*.cs'
$socialProductionText = (
    $socialProductionFiles |
        Get-Content -Raw |
        Out-String
)
foreach ($requestKindSpec in @(
    @{ Name = 'Response'; Value = 0 }
    @{ Name = 'Interruption'; Value = 1 }
)) {
    $requestKindName = [string]$requestKindSpec.Name
    $requestKindValue = [int]$requestKindSpec.Value
    if ($socialProductionText -notmatch
        "(?m)^\s*$requestKindName\s*=\s*$requestKindValue\s*,?\s*$") {
        throw "Social turn request kind '$requestKindName' is missing or renumbered."
    }
}
foreach ($statusName in @(
    'Granted',
    'NoTurn',
    'Rejected',
    'NoEligibleProposals',
    'CoordinatorMismatch',
    'WorldMismatch',
    'VersionConflict',
    'SimulationTickMismatch',
    'ConversationMismatch',
    'ConversationWorldMismatch',
    'ConversationRevisionConflict',
    'ConversationClosed',
    'CurrentTurnMissing',
    'CurrentSpeakerMismatch',
    'CurrentAudienceMismatch',
    'ProposalInvalid',
    'AuthorityChanged',
    'SelectionInvalid'
)) {
    if ($socialProductionText -notmatch
        "(?m)^\s*$statusName\s*=\s*\d+\s*,?\s*$") {
        throw "Social turn status '$statusName' is missing."
    }
}
if ($socialProductionText -match
    '\b(OpenAI|Anthropic|Gemini|Azure|Whisper|ElevenLabs|Deepgram)\b') {
    throw 'Social turn contracts contain a concrete provider.'
}
if ($socialProductionText -match
    '\b(HttpClient|HttpRequestMessage|HttpResponseMessage|Socket|WebSocket|GrpcChannel)\b') {
    throw 'Social turn contracts must not implement transport.'
}
if ($socialProductionText -match
    '\b(Task\.Delay|Task\.Run|Retry|Backoff|Timer|PeriodicTimer)\b') {
    throw 'Social turn contracts must not hide retries or scheduling.'
}
if ($socialProductionText -match
    '\b(ModelInvocationProcessor|SpeechInvocationProcessor|PromptCompositionProcessor|ContextRetrievalProcessor|AddressResolutionProcessor)\b') {
    throw 'Social turn contracts must not own external orchestration.'
}
if ($socialProductionText -match
    '\b(TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor|SimulationScheduler)\b') {
    throw 'Social turn contracts must not mutate or execute authoritative state.'
}
if ($socialProductionText -match
    '\b(EventDispatcher|DispatchAsync|KnowledgeSet|MemoryStore|RelationshipSet)\b') {
    throw 'Social turn contracts must not dispatch events or mutate subjective stores.'
}
if ($socialProductionText -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7)|FileStream|SqlConnection)\b') {
    throw 'Social turn contracts contain hidden clocks, generated IDs, or I/O.'
}
if ($socialProductionText -match '\bpublic\s+set\s*;') {
    throw 'Social turn contracts must not expose public setters.'
}
if ($socialProductionText -match
    '\b(Crime|Murder|Detective|Quest|Unreal|Unity|Blueprint|Actor|Pawn)\b') {
    throw 'Generic social turn contracts contain game-specific vocabulary.'
}


$structuredOutputRequestSource =
    Get-Content -LiteralPath $structuredOutputRequestPath -Raw
if ($structuredOutputRequestSource -notmatch
    'public\s+sealed\s+record\s+StructuredOutputRequestEnvelope<TResponse>') {
    throw 'The immutable structured-output request envelope is missing.'
}
foreach ($propertyName in @(
    'OutputId'
    'DecoderId'
    'SchemaId'
    'SchemaVersion'
    'SourceResponse'
)) {
    if ($structuredOutputRequestSource -notmatch
        "public\s+[^\{;]+?\s+$propertyName\s*(=>|\{\s*get;\s*\})") {
        throw "Structured-output request property '$propertyName' is missing."
    }
}

$structuredOutputEnvelopeSource =
    Get-Content -LiteralPath $structuredOutputEnvelopePath -Raw
if ($structuredOutputEnvelopeSource -notmatch
    'public\s+sealed\s+record\s+StructuredModelOutputEnvelope<TOutput>') {
    throw 'The validated structured model-output envelope is missing.'
}
foreach ($propertyName in @(
    'OutputId'
    'DecoderId'
    'SchemaId'
    'SchemaVersion'
    'SourceInvocationId'
    'SourceAdapterId'
    'SourceModelProfileId'
    'SourcePromptDocumentId'
    'OwnerEntityId'
    'WorldId'
    'WorldStateVersion'
    'SimulationTick'
    'SourceUsage'
    'Payload'
)) {
    if ($structuredOutputEnvelopeSource -notmatch
        "public\s+[^\{;]+?\s+$propertyName\s*(=>|\{\s*get;\s*\})") {
        throw "Structured-output envelope property '$propertyName' is missing."
    }
}

$structuredOutputDecoderSource =
    Get-Content -LiteralPath $structuredOutputDecoderPath -Raw
if ($structuredOutputDecoderSource -notmatch
    'public\s+interface\s+IStructuredOutputDecoder<TState,\s*TResponse,\s*TOutput>') {
    throw 'The exact structured-output decoder interface is missing.'
}
if ($structuredOutputDecoderSource -notmatch
    'StructuredOutputDecision<TOutput>\s+Decode\(') {
    throw 'The structured-output decoder must expose one Decode method.'
}

$structuredOutputProcessorSource =
    Get-Content -LiteralPath $structuredOutputProcessorPath -Raw
if ($structuredOutputProcessorSource -notmatch
    'public\s+sealed\s+class\s+StructuredOutputProcessor<TState,\s*TResponse,\s*TOutput>') {
    throw 'The structured-output processor is missing.'
}
$structuredDecoderCalls = [regex]::Matches(
    $structuredOutputProcessorSource,
    'decoder\.Decode\(context\)').Count
if ($structuredDecoderCalls -ne 1) {
    throw "Structured-output processing must call the decoder exactly once in source; found $structuredDecoderCalls."
}
$structuredAuthorityReads = [regex]::Matches(
    $structuredOutputProcessorSource,
    'worldStateManager\.Read\(\)').Count
if ($structuredAuthorityReads -ne 2) {
    throw "Structured-output processing must read authority exactly before and after decoding; found $structuredAuthorityReads."
}
foreach ($requiredPattern in @(
    'request\.DecoderId\s*!=\s*decoderId'
    'request\.SchemaId\s*!=\s*schemaId'
    'request\.SchemaVersion\s*!=\s*schemaVersion'
    'response\.WorldId\s*!=\s*before\.WorldId'
    'response\.WorldStateVersion\s*!=\s*before\.Version'
    'response\.SimulationTick\s*!=\s*before\.SimulationTick'
    'StructuredModelOutputEnvelope<TOutput>\.Create\('
)) {
    if ($structuredOutputProcessorSource -notmatch $requiredPattern) {
        throw "Structured-output processor is missing required correlation pattern '$requiredPattern'."
    }
}

$structuredOutputProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\StructuredOutput') -Filter '*.cs'
$structuredOutputProductionText = (
    $structuredOutputProductionFiles |
        Get-Content -Raw |
        Out-String
)
foreach ($decisionSpec in @(
    @{ Name = 'Decoded'; Value = 0 }
    @{ Name = 'Rejected'; Value = 1 }
)) {
    $decisionName = [string]$decisionSpec.Name
    $decisionValue = [int]$decisionSpec.Value
    if ($structuredOutputProductionText -notmatch
        "(?m)^\s*$decisionName\s*=\s*$decisionValue\s*,?\s*$") {
        throw "Structured-output decision '$decisionName' is missing or renumbered."
    }
}
foreach ($statusName in @(
    'Decoded'
    'Rejected'
    'DecoderMismatch'
    'SchemaMismatch'
    'SchemaVersionMismatch'
    'WorldMismatch'
    'VersionConflict'
    'SimulationTickMismatch'
    'AuthorityChanged'
)) {
    if ($structuredOutputProductionText -notmatch
        "(?m)^\s*$statusName\s*=\s*\d+\s*,?\s*$") {
        throw "Structured-output processing status '$statusName' is missing."
    }
}
if ($structuredOutputProductionText -notmatch
    'StructuredOutputSchemaVersion\s+From\(int\s+value\)') {
    throw 'Structured-output schema versions must remain explicit and positive.'
}
if ($structuredOutputProductionText -match
    '\b(OpenAI|Anthropic|Gemini|Azure|Whisper|ElevenLabs|Deepgram)\b') {
    throw 'Structured-output contracts contain a concrete provider.'
}
if ($structuredOutputProductionText -match
    '\b(HttpClient|HttpRequestMessage|HttpResponseMessage|Socket|WebSocket|GrpcChannel)\b') {
    throw 'Structured-output contracts must not implement transport.'
}
if ($structuredOutputProductionText -match
    '\b(JsonSerializer|JsonDocument|Utf8JsonReader|Newtonsoft)\b') {
    throw 'Structured-output Core contracts must not own a concrete serialization format.'
}
if ($structuredOutputProductionText -match
    '\b(Task\.Delay|Task\.Run|Retry|Backoff|Timer|PeriodicTimer)\b') {
    throw 'Structured-output contracts must not hide retries or scheduling.'
}
if ($structuredOutputProductionText -match
    '\b(ModelInvocationProcessor|SpeechInvocationProcessor|PromptCompositionProcessor|ContextRetrievalProcessor|AddressResolutionProcessor|SocialTurnCoordinationProcessor)\b') {
    throw 'Structured-output contracts must not own adjacent orchestration.'
}
if ($structuredOutputProductionText -match
    '\b(TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor|SimulationScheduler)\b') {
    throw 'Structured-output contracts must not mutate or execute authoritative state.'
}
if ($structuredOutputProductionText -match
    '\b(EventDispatcher|DispatchAsync|KnowledgeSet|MemoryStore|RelationshipSet)\b') {
    throw 'Structured-output contracts must not dispatch events or mutate subjective stores.'
}
if ($structuredOutputProductionText -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7)|FileStream|SqlConnection)\b') {
    throw 'Structured-output contracts contain hidden clocks, generated IDs, or I/O.'
}
if ($structuredOutputProductionText -match '\bpublic\s+set\s*;') {
    throw 'Structured-output contracts must not expose public setters.'
}
if ($structuredOutputProductionText -match
    '\b(Crime|Murder|Detective|Quest|Unreal|Unity|Blueprint|Actor|Pawn)\b') {
    throw 'Generic structured-output contracts contain game-specific vocabulary.'
}


$dialogueRequestSource = Get-Content -LiteralPath $dialogueRequestPath -Raw
if ($dialogueRequestSource -notmatch
    'public\s+sealed\s+record\s+DialogueOrchestrationRequestEnvelope<TInput>') {
    throw 'The immutable dialogue-orchestration request is missing.'
}
foreach ($propertyName in @(
    'OrchestrationId'
    'OrchestratorId'
    'ExchangeId'
    'ConversationId'
    'ExpectedConversationRevision'
    'PerspectiveOwnerEntityId'
    'SourceSpeakerEntityId'
    'Audience'
    'WorldId'
    'ExpectedWorldStateVersion'
    'ExpectedSimulationTick'
    'Input'
    'Artifacts'
)) {
    if ($dialogueRequestSource -notmatch
        "public\s+[^\{;]+?\s+$propertyName\s*(=>|\{\s*get;\s*\})") {
        throw "Dialogue-orchestration request property '$propertyName' is missing."
    }
}

$dialogueArtifactSource = Get-Content -LiteralPath $dialogueArtifactPath -Raw
foreach ($propertyName in @(
    'ArtifactId'
    'SourceId'
    'ExchangeId'
    'ConversationId'
    'PerspectiveOwnerEntityId'
    'WorldId'
    'WorldStateVersion'
    'SimulationTick'
    'Sequence'
    'Payload'
)) {
    if ($dialogueArtifactSource -notmatch
        "public\s+[^\{;]+?\s+$propertyName\s*(=>|\{\s*get;\s*\})") {
        throw "Dialogue artifact property '$propertyName' is missing."
    }
}

$dialogueOrchestratorSource = Get-Content -LiteralPath $dialogueOrchestratorPath -Raw
if ($dialogueOrchestratorSource -notmatch
    'public\s+interface\s+IDialogueOrchestrator<') {
    throw 'The exact pure dialogue orchestrator interface is missing.'
}
if ($dialogueOrchestratorSource -notmatch
    'DialogueOrchestrationDecision<TDirective,\s*TCompletion>\s+Decide\(') {
    throw 'The dialogue orchestrator must expose one Decide method.'
}

$dialogueProcessorSource = Get-Content -LiteralPath $dialogueProcessorPath -Raw
if ($dialogueProcessorSource -notmatch
    'public\s+sealed\s+class\s+DialogueOrchestrationProcessor<') {
    throw 'The dialogue orchestration processor is missing.'
}
$dialogueOrchestratorCalls = [regex]::Matches(
    $dialogueProcessorSource,
    'orchestrator\.Decide\(context\)').Count
if ($dialogueOrchestratorCalls -ne 1) {
    throw "Dialogue orchestration must call the orchestrator exactly once in source; found $dialogueOrchestratorCalls."
}
$dialogueAuthorityReads = [regex]::Matches(
    $dialogueProcessorSource,
    'worldStateManager\.Read\(\)').Count
if ($dialogueAuthorityReads -ne 2) {
    throw "Dialogue orchestration must read authority exactly before and after deciding; found $dialogueAuthorityReads."
}
foreach ($requiredPattern in @(
    'request\.OrchestratorId\s*!=\s*orchestratorId'
    'request\.WorldId\s*!=\s*snapshot\.WorldId'
    'request\.ExpectedWorldStateVersion\s*!=\s*snapshot\.Version'
    'request\.ExpectedSimulationTick\s*!=\s*snapshot\.SimulationTick'
    'request\.ExpectedConversationRevision\s*!=\s*conversation\.Revision'
    'after\.Version\s*!=\s*before\.Version'
    'after\.SimulationTick\s*!=\s*before\.SimulationTick'
)) {
    if ($dialogueProcessorSource -notmatch $requiredPattern) {
        throw "Dialogue processor is missing required correlation pattern '$requiredPattern'."
    }
}

$dialogueProductionFiles = Get-ChildItem -LiteralPath (
    Join-Path $root 'src\AI.Sandbox.Engine.Core\Dialogue') -Filter '*.cs'
$dialogueProductionText = (
    $dialogueProductionFiles | Get-Content -Raw | Out-String
)
foreach ($decisionSpec in @(
    @{ Name = 'Continue'; Value = 0 }
    @{ Name = 'Complete'; Value = 1 }
    @{ Name = 'Rejected'; Value = 2 }
)) {
    $decisionName = [string]$decisionSpec.Name
    $decisionValue = [int]$decisionSpec.Value
    if ($dialogueProductionText -notmatch
        "(?m)^\s*$decisionName\s*=\s*$decisionValue\s*,?\s*$") {
        throw "Dialogue decision '$decisionName' is missing or renumbered."
    }
}
foreach ($statusName in @(
    'Continued'
    'Completed'
    'Rejected'
    'OrchestratorMismatch'
    'WorldMismatch'
    'VersionConflict'
    'SimulationTickMismatch'
    'ConversationMismatch'
    'ConversationRevisionConflict'
    'ConversationClosed'
    'PerspectiveOwnerNotParticipant'
    'SourceSpeakerNotParticipant'
    'AudienceInvalid'
    'ArtifactInvalid'
    'AuthorityChanged'
    'DecisionInvalid'
)) {
    if ($dialogueProductionText -notmatch
        "(?m)^\s*$statusName\s*=\s*\d+\s*,?\s*$") {
        throw "Dialogue orchestration status '$statusName' is missing."
    }
}
if ($dialogueProductionText -match
    '\b(OpenAI|Anthropic|Gemini|Azure|Whisper|ElevenLabs|Deepgram)\b') {
    throw 'Dialogue orchestration contracts contain a concrete provider.'
}
if ($dialogueProductionText -match
    '\b(HttpClient|HttpRequestMessage|Socket|WebSocket|GrpcChannel|NamedPipe)\b') {
    throw 'Dialogue orchestration contracts implement transport.'
}
if ($dialogueProductionText -match
    '\b(JsonSerializer|JsonDocument|Utf8JsonReader|Newtonsoft)\b') {
    throw 'Dialogue orchestration contracts own a concrete serialization format.'
}
if ($dialogueProductionText -match
    '\b(Task\.Delay|Task\.Run|Retry|Backoff|Timer|PeriodicTimer)\b') {
    throw 'Dialogue orchestration contracts hide retries or scheduling.'
}
if ($dialogueProductionText -match
    '\b(ModelInvocationProcessor|SpeechInvocationProcessor|PromptCompositionProcessor|ContextRetrievalProcessor|AddressResolutionProcessor|SocialTurnCoordinationProcessor|ActionValidationProcessor|StructuredOutputProcessor)\b') {
    throw 'Dialogue orchestration contracts must return host directives rather than invoke adjacent processors.'
}
if ($dialogueProductionText -match
    '\b(TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor|SimulationScheduler|EventDispatcher)\b') {
    throw 'Dialogue orchestration contracts mutate or execute authority.'
}
if ($dialogueProductionText -match
    '\b(KnowledgeSet|MemoryStore|RelationshipSet)\b') {
    throw 'Dialogue orchestration contracts mutate subjective stores.'
}
if ($dialogueProductionText -match
    '\b(DateTime|DateTimeOffset|Stopwatch|Guid\.(NewGuid|CreateVersion7)|FileStream|SqlConnection)\b') {
    throw 'Dialogue orchestration contracts contain hidden clocks, IDs, or I/O.'
}
if ($dialogueProductionText -match '\bpublic\s+set\s*;') {
    throw 'Dialogue orchestration contracts expose a public setter.'
}
if ($dialogueProductionText -match
    '\b(Crime|Murder|Detective|Quest|Unreal|Unity|Blueprint|Actor|Pawn)\b') {
    throw 'Generic dialogue orchestration contracts contain game-specific vocabulary.'
}

$hostRuntimeDirectory = Join-Path $root 'src\AI.Sandbox.Engine.Core\HostRuntime'
if (-not (Test-Path -LiteralPath $hostRuntimeDirectory)) {
    throw 'Host runtime composition directory is missing.'
}
$expectedHostRuntimeFileNames = @(
    'HostRuntimeAdmissionIdKind.cs'
    'HostRuntimeAttemptIdKind.cs'
    'HostRuntimeAttemptSettlement.cs'
    'HostRuntimeAttemptSettlementFlow.cs'
    'HostRuntimeAttemptSettlementResult.cs'
    'HostRuntimeAttemptSettlementStatus.cs'
    'HostRuntimeSettlementIdKind.cs'
    'HostRuntimeRetryRequeue.cs'
    'HostRuntimeRetryRequeueFlow.cs'
    'HostRuntimeRetryRequeueIdKind.cs'
    'HostRuntimeRetryRequeueResult.cs'
    'HostRuntimeRetryRequeueStatus.cs'
    'HostRuntimeDeadLetterDisposition.cs'
    'HostRuntimeDeadLetterDispositionFlow.cs'
    'HostRuntimeDeadLetterDispositionIdKind.cs'
    'HostRuntimeDeadLetterDispositionKind.cs'
    'HostRuntimeDeadLetterDispositionResult.cs'
    'HostRuntimeDeadLetterDispositionStatus.cs'
    'HostRuntimeAbandonedAttemptDisposition.cs'
    'HostRuntimeAbandonedAttemptDispositionFlow.cs'
    'HostRuntimeAbandonedAttemptDispositionIdKind.cs'
    'HostRuntimeAbandonedAttemptDispositionKind.cs'
    'HostRuntimeAbandonedAttemptDispositionResult.cs'
    'HostRuntimeAbandonedAttemptDispositionStatus.cs'
    'HostRuntimeActiveWorkSnapshotIdKind.cs'
    'HostRuntimeActiveWorkReconciliationIdKind.cs'
    'HostRuntimeActiveWorkItem.cs'
    'HostRuntimeActiveWorkSnapshot.cs'
    'HostRuntimeActiveWorkReconciliation.cs'
    'HostRuntimeActiveWorkStatus.cs'
    'HostRuntimeActiveWorkSnapshotResult.cs'
    'HostRuntimeActiveWorkReconciliationResult.cs'
    'HostRuntimeActiveWorkFlow.cs'
    'HostRuntimeRecoveryCheckpointIdKind.cs'
    'HostRuntimeRecoveryContinuationIdKind.cs'
    'HostRuntimeRecoveryStatus.cs'
    'HostRuntimeRecoveryCheckpoint.cs'
    'HostRuntimeRecoveryContinuation.cs'
    'HostRuntimeRecoveryCheckpointResult.cs'
    'HostRuntimeRecoveryContinuationResult.cs'
    'HostRuntimeRecoveryFlow.cs'
    'HostRuntimeRecoveryResumptionPlanIdKind.cs'
    'HostRuntimeResumedWorkSelectionIdKind.cs'
    'HostRuntimeRecoveryResumptionStatus.cs'
    'HostRuntimeRecoveryResumptionPlan.cs'
    'HostRuntimeResumedWorkSelection.cs'
    'HostRuntimeRecoveryResumptionPlanResult.cs'
    'HostRuntimeResumedWorkSelectionResult.cs'
    'HostRuntimeRecoveryResumptionFlow.cs'
    'HostRuntimeRecoveryReadmissionIdKind.cs'
    'HostRuntimeRecoveryLeaseReacquisitionIdKind.cs'
    'HostRuntimeRecoveryReadmissionStatus.cs'
    'HostRuntimeRecoveryReadmission.cs'
    'HostRuntimeRecoveryLeaseReacquisition.cs'
    'HostRuntimeRecoveryReadmissionResult.cs'
    'HostRuntimeRecoveryLeaseReacquisitionResult.cs'
    'HostRuntimeRecoveryReadmissionFlow.cs'
    'HostRuntimeRecoveryDispatchReconstructionIdKind.cs'
    'HostRuntimeRecoveryResumedAttemptAcknowledgementIdKind.cs'
    'HostRuntimeRecoveryDispatchStatus.cs'
    'HostRuntimeRecoveryDispatchReconstruction.cs'
    'HostRuntimeRecoveryResumedAttemptAcknowledgement.cs'
    'HostRuntimeRecoveryDispatchReconstructionResult.cs'
    'HostRuntimeRecoveryResumedAttemptAcknowledgementResult.cs'
    'HostRuntimeRecoveryDispatchFlow.cs'
    'HostRuntimeRecoveryResumedAttemptSettlementIdKind.cs'
    'HostRuntimeRecoveryCycleCompletionIdKind.cs'
    'HostRuntimeRecoverySettlementStatus.cs'
    'HostRuntimeRecoveryResumedAttemptSettlement.cs'
    'HostRuntimeRecoveryCycleCompletion.cs'
    'HostRuntimeRecoveryResumedAttemptSettlementResult.cs'
    'HostRuntimeRecoveryCycleCompletionResult.cs'
    'HostRuntimeRecoverySettlementFlow.cs'
    'HostRuntimeRecoveryCheckpointSupersessionIdKind.cs'
    'HostRuntimeRecoveryCompletedCycleSummaryIdKind.cs'
    'HostRuntimeRecoveryCheckpointSupersessionStatus.cs'
    'HostRuntimeRecoveryCheckpointSupersession.cs'
    'HostRuntimeRecoveryCompletedCycleSummary.cs'
    'HostRuntimeRecoveryCheckpointSupersessionResult.cs'
    'HostRuntimeRecoveryCompletedCycleSummaryResult.cs'
    'HostRuntimeRecoveryCheckpointSupersessionFlow.cs'
    'HostRuntimeRecoverySupersessionChainIdKind.cs'
    'HostRuntimeRecoveryLatestCheckpointSelectionIdKind.cs'
    'HostRuntimeRecoverySupersessionChainStatus.cs'
    'HostRuntimeRecoverySupersessionChain.cs'
    'HostRuntimeRecoveryLatestCheckpointSelection.cs'
    'HostRuntimeRecoverySupersessionChainResult.cs'
    'HostRuntimeRecoveryLatestCheckpointSelectionResult.cs'
    'HostRuntimeRecoverySupersessionChainFlow.cs'
    'HostRuntimeRecoveryChainSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryCheckpointLineageQueryIdKind.cs'
    'HostRuntimeRecoveryChainQueryStatus.cs'
    'HostRuntimeRecoveryChainSummaryProjection.cs'
    'HostRuntimeRecoveryCheckpointLineageQuery.cs'
    'HostRuntimeRecoveryChainSummaryProjectionResult.cs'
    'HostRuntimeRecoveryCheckpointLineageQueryResult.cs'
    'HostRuntimeRecoveryChainQueryFlow.cs'
    'HostRuntimeRecoveryLineageWindowProjectionIdKind.cs'
    'HostRuntimeRecoveryCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryLineageWindowStatus.cs'
    'HostRuntimeRecoveryLineageWindowProjection.cs'
    'HostRuntimeRecoveryCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryLineageWindowProjectionResult.cs'
    'HostRuntimeRecoveryCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryLineageWindowFlow.cs'
    'HostRuntimeRecoveryCheckpointRangeSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryAdjacentWindowSelectionIdKind.cs'
    'HostRuntimeRecoveryCheckpointRangeSummaryStatus.cs'
    'HostRuntimeRecoveryCheckpointRangeSummaryProjection.cs'
    'HostRuntimeRecoveryAdjacentWindowSelection.cs'
    'HostRuntimeRecoveryCheckpointRangeSummaryProjectionResult.cs'
    'HostRuntimeRecoveryAdjacentWindowSelectionResult.cs'
    'HostRuntimeRecoveryCheckpointRangeSummaryFlow.cs'
    'HostRuntimeRecoveryAdjacentWindowProjectionIdKind.cs'
    'HostRuntimeRecoveryCheckpointRangeContinuityValidationIdKind.cs'
    'HostRuntimeRecoveryAdjacentWindowProjectionStatus.cs'
    'HostRuntimeRecoveryAdjacentWindowProjection.cs'
    'HostRuntimeRecoveryCheckpointRangeContinuityValidation.cs'
    'HostRuntimeRecoveryAdjacentWindowProjectionResult.cs'
    'HostRuntimeRecoveryCheckpointRangeContinuityValidationResult.cs'
    'HostRuntimeRecoveryAdjacentWindowFlow.cs'
    'HostRuntimeRecoveryContinuousWindowPairSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryCrossWindowCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousWindowPairStatus.cs'
    'HostRuntimeRecoveryContinuousWindowPairSummaryProjection.cs'
    'HostRuntimeRecoveryCrossWindowCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousWindowPairSummaryProjectionResult.cs'
    'HostRuntimeRecoveryCrossWindowCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousWindowPairFlow.cs'
    'HostRuntimeRecoveryContinuousWindowSequenceValidationIdKind.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousWindowSequenceStatus.cs'
    'HostRuntimeRecoveryContinuousWindowSequenceValidation.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousWindowSequenceValidationResult.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousWindowSequenceFlow.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryAdjacentSequenceSelectionIdKind.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection.cs'
    'HostRuntimeRecoveryAdjacentSequenceSelection.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjectionResult.cs'
    'HostRuntimeRecoveryAdjacentSequenceSelectionResult.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryFlow.cs'
    'HostRuntimeRecoveryAdjacentSequenceProjectionIdKind.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidationIdKind.cs'
    'HostRuntimeRecoveryAdjacentSequenceProjectionStatus.cs'
    'HostRuntimeRecoveryAdjacentSequenceProjection.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation.cs'
    'HostRuntimeRecoveryAdjacentSequenceProjectionResult.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidationResult.cs'
    'HostRuntimeRecoveryAdjacentSequenceFlow.cs'
    'HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryCrossSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiSequenceStatus.cs'
    'HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection.cs'
    'HostRuntimeRecoveryCrossSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionResult.cs'
    'HostRuntimeRecoveryCrossSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiSequenceFlow.cs'
    'HostRuntimeRecoveryContinuousMultiSequenceCollectionValidationIdKind.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus.cs'
    'HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiSequenceCollectionValidationResult.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiSequenceCollectionFlow.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryAdjacentCollectionSelectionIdKind.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection.cs'
    'HostRuntimeRecoveryAdjacentCollectionSelection.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjectionResult.cs'
    'HostRuntimeRecoveryAdjacentCollectionSelectionResult.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryFlow.cs'
    'HostRuntimeRecoveryAdjacentCollectionProjectionIdKind.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidationIdKind.cs'
    'HostRuntimeRecoveryAdjacentCollectionProjectionStatus.cs'
    'HostRuntimeRecoveryAdjacentCollectionProjection.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidation.cs'
    'HostRuntimeRecoveryAdjacentCollectionProjectionResult.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidationResult.cs'
    'HostRuntimeRecoveryAdjacentCollectionFlow.cs'
    'HostRuntimeRecoveryContinuousCollectionPairSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryCrossCollectionCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousCollectionPairStatus.cs'
    'HostRuntimeRecoveryContinuousCollectionPairSummaryProjection.cs'
    'HostRuntimeRecoveryCrossCollectionCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousCollectionPairSummaryProjectionResult.cs'
    'HostRuntimeRecoveryCrossCollectionCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousCollectionPairFlow.cs'
    'HostRuntimeRecoveryContinuousCollectionSequenceValidationIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousCollectionSequenceStatus.cs'
    'HostRuntimeRecoveryContinuousCollectionSequenceValidation.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousCollectionSequenceValidationResult.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousCollectionSequenceFlow.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryAdjacentCollectionSequenceSelectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection.cs'
    'HostRuntimeRecoveryAdjacentCollectionSequenceSelection.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjectionResult.cs'
    'HostRuntimeRecoveryAdjacentCollectionSequenceSelectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryFlow.cs'
    'HostRuntimeRecoveryAdjacentCollectionSequenceProjectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidationIdKind.cs'
    'HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus.cs'
    'HostRuntimeRecoveryAdjacentCollectionSequenceProjection.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidation.cs'
    'HostRuntimeRecoveryAdjacentCollectionSequenceProjectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidationResult.cs'
    'HostRuntimeRecoveryAdjacentCollectionSequenceFlow.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection.cs'
    'HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionResult.cs'
    'HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSummaryFlow.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceValidationIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceValidation.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceValidationResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceFlow.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSelectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSelection.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjectionResult.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSelectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryFlow.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionProjectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidationIdKind.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionProjection.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidation.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionProjectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidationResult.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionFlow.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection.cs'
    'HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjectionResult.cs'
    'HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryFlow.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceValidationIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceStatus.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceValidation.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceValidationResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceFlow.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjectionResult.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryFlow.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidationIdKind.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjection.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidation.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidationResult.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceFlow.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjection.cs'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjectionResult.cs'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryFlow.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidationIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceStatus.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidation.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidationResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceFlow.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjection.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelection.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryFlow.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidationIdKind.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjection.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidation.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidationResult.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceFlow.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection.cs'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjectionResult.cs'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryFlow.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidationIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidation.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidationResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceFlow.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelection.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryFlow.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationIdKind.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjection.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidation.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationResult.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceFlow.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryStatus.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryProjection.cs'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryProjectionResult.cs'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryFlow.cs'
    'HostRuntimeCapabilityDescriptor.cs'
    'HostRuntimeCapabilityIdKind.cs'
    'HostRuntimeClockIdKind.cs'
    'HostRuntimeCompletionEnvelope.cs'
    'HostRuntimeCompletionFlow.cs'
    'HostRuntimeCompletionKind.cs'
    'HostRuntimeCompletionRouter.cs'
    'HostRuntimeCompletionRoutingResult.cs'
    'HostRuntimeCompletionRoutingStatus.cs'
    'HostRuntimeComposition.cs'
    'HostRuntimeCompositionBuilder.cs'
    'HostRuntimeCompositionIdKind.cs'
    'HostRuntimeCompositionResult.cs'
    'HostRuntimeCompositionStatus.cs'
    'HostRuntimeCorrelationIdKind.cs'
    'HostRuntimeDeadline.cs'
    'HostRuntimeDispatchAcknowledgementFlow.cs'
    'HostRuntimeDispatchAcknowledgementResult.cs'
    'HostRuntimeDispatchAcknowledgementStatus.cs'
    'HostRuntimeDispatchEnvelope.cs'
    'HostRuntimeDispatchFlow.cs'
    'HostRuntimeDispatchIdKind.cs'
    'HostRuntimeDispatchSelection.cs'
    'HostRuntimeDispatchSelectionFlow.cs'
    'HostRuntimeDispatchSelectionIdKind.cs'
    'HostRuntimeDispatchSelectionResult.cs'
    'HostRuntimeDispatchSelectionStatus.cs'
    'HostRuntimeEndpointIdKind.cs'
    'HostRuntimeHealthProbeIdKind.cs'
    'HostRuntimeHealthStatus.cs'
    'HostRuntimeHealthUpdateResult.cs'
    'HostRuntimeHealthUpdateStatus.cs'
    'HostRuntimeInFlightAttempt.cs'
    'HostRuntimeInstanceIdKind.cs'
    'HostRuntimeLeaseIdKind.cs'
    'HostRuntimeLeaseState.cs'
    'HostRuntimeLeaseTransitionResult.cs'
    'HostRuntimeLeaseTransitionStatus.cs'
    'HostRuntimeLifecycle.cs'
    'HostRuntimeLifecycleSnapshot.cs'
    'HostRuntimeLifecycleState.cs'
    'HostRuntimeLifecycleTransitionResult.cs'
    'HostRuntimeLifecycleTransitionStatus.cs'
    'HostRuntimeOperationIdKind.cs'
    'HostRuntimePriority.cs'
    'HostRuntimePriorityClass.cs'
    'HostRuntimePriorityOrder.cs'
    'HostRuntimeQueueAdmission.cs'
    'HostRuntimeQueueAdmissionFlow.cs'
    'HostRuntimeQueueAdmissionResult.cs'
    'HostRuntimeQueueAdmissionStatus.cs'
    'HostRuntimeQueueCapacity.cs'
    'HostRuntimeQueueIdKind.cs'
    'HostRuntimeQueueSnapshot.cs'
    'HostRuntimeRequestEnvelope.cs'
    'HostRuntimeRequestFlow.cs'
    'HostRuntimeRequestIdKind.cs'
    'HostRuntimeRequestState.cs'
    'HostRuntimeRequestTransitionResult.cs'
    'HostRuntimeRequestTransitionStatus.cs'
    'HostRuntimeRetryDecision.cs'
    'HostRuntimeRetryDecisionFlow.cs'
    'HostRuntimeRetryDecisionStatus.cs'
    'HostRuntimeRetryPolicy.cs'
    'HostRuntimeRetryPolicyIdKind.cs'
    'HostRuntimeRouteIdKind.cs'
    'HostRuntimeTypePolicy.cs'
    'HostRuntimeWorkerIdKind.cs'
    'HostRuntimeWorkLease.cs'
    'HostRuntimeWorkLeaseFlow.cs'
    'IHostRuntimeCancellationReason.cs'
    'IHostRuntimeCapability.cs'
    'IHostRuntimeCompletion.cs'
    'IHostRuntimeHealthDetail.cs'
    'IHostRuntimeRequest.cs'
    'IHostRuntimeRetryReason.cs'
)
$hostRuntimeFiles = @(
    Get-ChildItem -LiteralPath $hostRuntimeDirectory -File -Filter '*.cs')
$actualHostRuntimeFileNames = @(
    $hostRuntimeFiles |
        Select-Object -ExpandProperty Name |
        Sort-Object)
$expectedHostRuntimeFileNames = @(
    $expectedHostRuntimeFileNames |
        Sort-Object)
$hostRuntimeInventoryDifferences = @(
    Compare-Object `
        -ReferenceObject $expectedHostRuntimeFileNames `
        -DifferenceObject $actualHostRuntimeFileNames)
if ($hostRuntimeInventoryDifferences.Count -gt 0) {
    throw (
        "Host-runtime production file set mismatch. Expected: " +
        ($expectedHostRuntimeFileNames -join ', ') +
        ". Actual: " +
        ($actualHostRuntimeFileNames -join ', ') +
        ".")
}
$hostRuntimeText = ($hostRuntimeFiles | Get-Content -Raw | Out-String)
$hostRuntimeBuilder = Get-Content -LiteralPath (
    Join-Path $hostRuntimeDirectory 'HostRuntimeCompositionBuilder.cs') -Raw
foreach ($requiredType in @(
    'IHostRuntimeCapability'
    'HostRuntimeCapabilityDescriptor'
    'HostRuntimeComposition'
    'HostRuntimeCompositionBuilder'
    'HostRuntimeCompositionResult'
    'HostRuntimeCompositionStatus'
)) {
    if ($hostRuntimeText -notmatch "\b$requiredType\b") {
        throw "Host runtime contract '$requiredType' is missing."
    }
}
foreach ($statusName in @(
    'Composed'
    'Empty'
    'TooManyCapabilities'
    'DuplicateCapability'
    'MissingDependency'
    'CycleDetected'
)) {
    if ($hostRuntimeText -notmatch "(?m)^\s*$statusName\s*=\s*\d+\s*,?\s*$") {
        throw "Host runtime composition status '$statusName' is missing."
    }
}
if ($hostRuntimeBuilder -notmatch 'MaximumCapabilityCount\s*=\s*128') {
    throw 'Host runtime composition must retain the 128-capability bound.'
}
if ($hostRuntimeText -notmatch 'MaximumDependencyCount\s*=\s*32') {
    throw 'Host runtime dependencies must retain the 32-item bound.'
}
if ($hostRuntimeBuilder -notmatch 'SortedSet<') {
    throw 'Host runtime composition must use deterministic ready-node ordering.'
}
if ($hostRuntimeText -match '\b(Microsoft\.Extensions\.DependencyInjection|IServiceCollection|ServiceProvider|Autofac|DryIoc)\b') {
    throw 'Generic Core must not own a dependency-injection container.'
}
if ($hostRuntimeText -match '\b(StartAsync|StopAsync|DisposeAsync|BackgroundService|IHostedService|Task\.Run|Task\.Delay|Timer|PeriodicTimer)\b') {
    throw 'Host runtime composition must not execute lifecycle or background work.'
}
if ($hostRuntimeText -match '\b(HttpClient|Socket|WebSocket|NamedPipe|GrpcChannel|FileStream|SqlConnection)\b') {
    throw 'Host runtime composition must not implement transport or I/O.'
}
if ($hostRuntimeText -match '\b(OpenAI|Anthropic|Gemini|Azure|Whisper|ElevenLabs|Unreal|Unity|Blueprint|Actor|Pawn)\b') {
    throw 'Host runtime composition contains provider- or game-specific coupling.'
}
if ($hostRuntimeText -match '\bpublic\s+set\s*;') {
    throw 'Host runtime composition exposes a public setter.'
}
foreach ($requiredDocumentationFragment in @(
    '/// <summary>Composition completed successfully.</summary>'
    '/// <summary>No capability descriptors were supplied.</summary>'
    '/// <summary>The bounded capability count was exceeded.</summary>'
    '/// <summary>More than one descriptor used the same capability ID.</summary>'
    '/// <summary>A declared dependency was absent from the composition input.</summary>'
    '/// <summary>The capability dependency graph contained a cycle.</summary>'
    '/// <summary>Gets the externally assigned stable capability ID.</summary>'
    '/// <summary>Gets the exact immutable capability payload.</summary>'
    '/// <summary>Gets dependency IDs in stable ordinal order.</summary>'
    '/// Creates one validated immutable capability descriptor.'
    '/// <summary>Gets the externally assigned stable composition ID.</summary>'
    '/// <summary>Gets capabilities in deterministic dependency-safe order.</summary>'
    '/// <summary>Gets the explicit composition outcome.</summary>'
    '/// <summary>Gets the immutable composition when composition succeeded.</summary>'
    '/// Gets the capability related to a duplicate, missing dependency, or cycle.'
    '/// <summary>Gets whether a usable immutable composition was produced.</summary>'
    '/// Validates and deterministically orders a bounded capability graph.'
)) {
    if (-not $hostRuntimeText.Contains($requiredDocumentationFragment)) {
        throw "Host-runtime XML documentation is missing: $requiredDocumentationFragment"
    }
}
$hostLifecycleDirectory = Join-Path $root 'src\AI.Sandbox.Engine.Core\HostRuntime'
$hostLifecycleFiles = @(
    'HostRuntimeHealthProbeIdKind.cs'
    'HostRuntimeHealthStatus.cs'
    'HostRuntimeHealthUpdateResult.cs'
    'HostRuntimeHealthUpdateStatus.cs'
    'HostRuntimeInstanceIdKind.cs'
    'HostRuntimeLifecycle.cs'
    'HostRuntimeLifecycleSnapshot.cs'
    'HostRuntimeLifecycleState.cs'
    'HostRuntimeLifecycleTransitionResult.cs'
    'HostRuntimeLifecycleTransitionStatus.cs'
    'IHostRuntimeHealthDetail.cs'
)
foreach ($hostLifecycleFile in $hostLifecycleFiles) {
    $hostLifecyclePath = Join-Path $hostLifecycleDirectory $hostLifecycleFile
    if (-not (Test-Path -LiteralPath $hostLifecyclePath)) {
        throw "Host lifecycle contract is missing: $hostLifecycleFile"
    }
}
$hostLifecycleText = (
    $hostLifecycleFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostLifecycleDirectory $_) -Raw
        } |
        Out-String)
if ($hostLifecycleText -notmatch 'HostRuntimeLifecycleState\.Created') {
    throw 'Host lifecycle transitions are missing the Created state.'
}
if ($hostLifecycleText -notmatch 'HostRuntimeLifecycleState\.Stopped') {
    throw 'Host lifecycle transitions are missing the Stopped state.'
}
if ($hostLifecycleText -notmatch 'HostRuntimeHealthStatus\.Unknown') {
    throw 'Host lifecycle contracts are missing explicit unknown health.'
}
if ($hostLifecycleText -notmatch 'HostRuntimeLifecycleTransitionStatus\.StaleRevision') {
    throw 'Host lifecycle contracts are missing optimistic stale-revision handling.'
}
if ($hostLifecycleText -notmatch 'HostRuntimeHealthUpdateStatus\.InvalidLifecycleState') {
    throw 'Host health contracts are missing lifecycle-state rejection.'
}
if ($hostLifecycleText -match '\b(Task\.Run|Task\.Delay|Timer|Thread|HttpClient|Socket|NamedPipe|StartAsync|StopAsync|Process\.Start)\b') {
    throw 'Host lifecycle contracts contain execution, scheduling, or transport coupling.'
}
if ($hostLifecycleText -match '\bpublic\s+set\s*;') {
    throw 'Host lifecycle contracts expose a public setter.'
}
$hostLifecycleTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\HostRuntimeLifecycleHealthTests.cs')
if (-not (Test-Path -LiteralPath $hostLifecycleTestPath)) {
    throw 'Host lifecycle and health tests are missing.'
}
$hostLifecycleTestText = Get-Content -LiteralPath $hostLifecycleTestPath -Raw
foreach ($testName in @(
    'SnapshotIdsAndInitialStateAreValidated'
    'LifecycleTransitionsAreExplicitAndImmutable'
    'InvalidAndStaleTransitionsAreRejected'
    'HealthObservationRequiresActiveLifecycleAndExactPayload'
    'HealthObservationIsImmutableAndRevisioned'
    'HealthStatusProbeAndRevisionValidationAreExplicit'
    'FaultAndShutdownPathIsExplicit'
    'ContractsDoNotExecuteLifecycleOrHealthSideEffects'
)) {
    if ($hostLifecycleTestText -notmatch $testName) {
        throw "Host lifecycle test is missing: $testName"
    }
}
$hostRequestDirectory = Join-Path $root 'src\AI.Sandbox.Engine.Core\HostRuntime'
$hostRequestFiles = @(
    'HostRuntimeCorrelationIdKind.cs'
    'HostRuntimeOperationIdKind.cs'
    'HostRuntimeRequestEnvelope.cs'
    'HostRuntimeRequestFlow.cs'
    'HostRuntimeRequestIdKind.cs'
    'HostRuntimeRequestState.cs'
    'HostRuntimeRequestTransitionResult.cs'
    'HostRuntimeRequestTransitionStatus.cs'
    'IHostRuntimeCancellationReason.cs'
    'IHostRuntimeRequest.cs'
)
foreach ($hostRequestFile in $hostRequestFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRequestDirectory $hostRequestFile))) {
        throw "Host request contract is missing: $hostRequestFile"
    }
}
$hostRequestText = (
    $hostRequestFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRequestDirectory $_) -Raw
        } |
        Out-String)
foreach ($requiredHostRequestToken in @(
    'HostRuntimeRequestState.Pending'
    'HostRuntimeRequestState.CancellationRequested'
    'HostRuntimeRequestState.Completed'
    'HostRuntimeRequestTransitionStatus.StaleRevision'
    'EnsureExactCancellationReason'
    'checked(envelope.Revision + 1)'
)) {
    if ($hostRequestText -notmatch [regex]::Escape(
        $requiredHostRequestToken)) {
        throw "Host request token is missing: $requiredHostRequestToken"
    }
}
if ($hostRequestText -match '\b(CancellationTokenSource|Cancel\(|Task\.Run|Task\.Delay|Timer|Thread|HttpClient|Socket|NamedPipe|Process\.Start)\b') {
    throw 'Host request contracts contain execution, transport, or active cancellation.'
}
if ($hostRequestText -match '\bpublic\s+set\s*;') {
    throw 'Host request contracts expose a public setter.'
}
$hostRequestTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\HostRuntimeRequestCorrelationCancellationTests.cs')
if (-not (Test-Path -LiteralPath $hostRequestTestPath)) {
    throw 'Host request correlation and cancellation tests are missing.'
}
$hostRequestTestText = Get-Content -LiteralPath $hostRequestTestPath -Raw
foreach ($testName in @(
    'IdsPayloadAndParentAreValidated'
    'CreationPreservesCorrelationAndOptionalParent'
    'CancellationRequestIsExplicitImmutableAndRevisioned'
    'StaleAndRepeatedCancellationAreRejected'
    'TerminalizationIsExplicitAndPreservesCancellationReason'
    'InvalidTerminalTargetsAndTerminalRetransitionAreRejected'
    'CancellationReasonMustBeExact'
    'ContractsDoNotExecuteRequestsOrCancellation'
)) {
    if ($hostRequestTestText -notmatch $testName) {
        throw "Host request test is missing: $testName"
    }
}
$hostRetryDirectory = Join-Path $root 'src\AI.Sandbox.Engine.Core\HostRuntime'
$hostRetryFiles = @(
    'HostRuntimeClockIdKind.cs'
    'HostRuntimeDeadline.cs'
    'HostRuntimeRetryDecision.cs'
    'HostRuntimeRetryDecisionFlow.cs'
    'HostRuntimeRetryDecisionStatus.cs'
    'HostRuntimeRetryPolicy.cs'
    'HostRuntimeRetryPolicyIdKind.cs'
    'IHostRuntimeRetryReason.cs'
)
foreach ($hostRetryFile in $hostRetryFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRetryDirectory $hostRetryFile))) {
        throw "Host retry contract is missing: $hostRetryFile"
    }
}
$hostRetryText = (
    $hostRetryFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRetryDirectory $_) -Raw
        } |
        Out-String)
foreach ($requiredHostRetryToken in @(
    'HostRuntimeRetryDecisionStatus.RetryAllowed'
    'HostRuntimeRetryDecisionStatus.DeadlineExceeded'
    'HostRuntimeRetryDecisionStatus.AttemptLimitReached'
    'HostRuntimeRequestState.Failed'
    'HostRuntimeRequestState.Rejected'
    'EnsureExactRetryReason'
    'checked(observedTick + policy.RetryDelayTicks)'
)) {
    if ($hostRetryText -notmatch [regex]::Escape(
        $requiredHostRetryToken)) {
        throw "Host retry token is missing: $requiredHostRetryToken"
    }
}
if ($hostRetryText -match '\b(DateTime|DateTimeOffset|Stopwatch|Task\.Run|Task\.Delay|Timer|Thread|CancellationTokenSource|HttpClient|Socket|NamedPipe|Process\.Start)\b') {
    throw 'Host retry contracts contain wall-clock, execution, transport, or active cancellation coupling.'
}
if ($hostRetryText -match '\bpublic\s+set\s*;') {
    throw 'Host retry contracts expose a public setter.'
}
$hostRetryTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\HostRuntimeDeadlineRetryDecisionTests.cs')
if (-not (Test-Path -LiteralPath $hostRetryTestPath)) {
    throw 'Host deadline and retry-decision tests are missing.'
}
$hostRetryTestText = Get-Content -LiteralPath $hostRetryTestPath -Raw
foreach ($testName in @(
    'DeadlinePolicyIdsAndBoundsAreValidated'
    'DeadlineAndPolicyPreserveExternalAuthority'
    'FailedRequestProducesDeterministicRetryDecision'
    'RejectedRequestMayRetryWithoutDeadline'
    'DeadlineExceededIsExplicit'
    'AttemptLimitAndInvalidStateAreExplicit'
    'StaleRevisionAndClockMismatchAreExplicit'
    'ContractsDoNotWaitScheduleRetryOrExecuteReason'
)) {
    if ($hostRetryTestText -notmatch $testName) {
        throw "Host retry test is missing: $testName"
    }
}
$hostDispatchDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostDispatchFiles = @(
    'HostRuntimeCompletionEnvelope.cs'
    'HostRuntimeCompletionFlow.cs'
    'HostRuntimeCompletionKind.cs'
    'HostRuntimeCompletionRouter.cs'
    'HostRuntimeCompletionRoutingResult.cs'
    'HostRuntimeCompletionRoutingStatus.cs'
    'HostRuntimeDispatchEnvelope.cs'
    'HostRuntimeDispatchFlow.cs'
    'HostRuntimeDispatchIdKind.cs'
    'HostRuntimeEndpointIdKind.cs'
    'HostRuntimeRouteIdKind.cs'
    'IHostRuntimeCompletion.cs'
)
foreach ($hostDispatchFile in $hostDispatchFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostDispatchDirectory $hostDispatchFile))) {
        throw "Host dispatch contract is missing: $hostDispatchFile"
    }
}
$hostDispatchText = (
    $hostDispatchFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostDispatchDirectory $_) -Raw
        } |
        Out-String)
foreach ($requiredHostDispatchToken in @(
    'HostRuntimeCompletionRoutingStatus.Routed'
    'HostRuntimeCompletionRoutingStatus.DispatchMismatch'
    'HostRuntimeCompletionRoutingStatus.CompletionMismatch'
    'HostRuntimeRequestState.CancellationRequested'
    'HostRuntimeRequestFlow.Finalize'
    'EnsureExactCompletion'
)) {
    if ($hostDispatchText -notmatch [regex]::Escape(
        $requiredHostDispatchToken)) {
        throw "Host dispatch token is missing: $requiredHostDispatchToken"
    }
}
if ($hostDispatchText -match '\b(Task\.Run|Task\.Delay|Timer|Thread|CancellationTokenSource|HttpClient|Socket|NamedPipe|Process\.Start|SendAsync|ReceiveAsync)\b') {
    throw 'Host dispatch contracts contain transport, execution, waiting, or active cancellation.'
}
if ($hostDispatchText -match '\bpublic\s+set\s*;') {
    throw 'Host dispatch contracts expose a public setter.'
}
$hostDispatchTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\HostRuntimeDispatchCompletionRoutingTests.cs')
if (-not (Test-Path -LiteralPath $hostDispatchTestPath)) {
    throw 'Host dispatch and completion-routing tests are missing.'
}
$hostDispatchTestText = Get-Content -LiteralPath (
    $hostDispatchTestPath) -Raw
foreach ($testName in @(
    'DispatchIdsAttemptAndStateAreValidated'
    'DispatchPreservesRoutingAndRequestAuthority'
    'CompletionIdsKindPayloadAndAttemptAreValidated'
    'SuccessfulCompletionRoutesAndFinalizesRequest'
    'CancellationRequestedRequestAcceptsCancelledCompletion'
    'CompletionIdentityMismatchIsExplicitAndUnchanged'
    'StaleTerminalAndDispatchMismatchAreExplicit'
    'ContractsDoNotDispatchTransportOrExecuteCompletion'
)) {
    if ($hostDispatchTestText -notmatch $testName) {
        throw "Host dispatch test is missing: $testName"
    }
}
if ($hostDispatchTestText -match
    'Xunit\.Assert\.Same\(pending\.Payload, dispatch\.Request\.Payload\)') {
    throw (
        'Host dispatch tests use identity comparison for a value-type ' +
        'request payload.')
}
if ($hostDispatchTestText -notmatch
    'Xunit\.Assert\.Equal\(pending\.Payload, dispatch\.Request\.Payload\)') {
    throw 'Host dispatch value-type payload equality assertion is missing.'
}
$hostQueueDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostQueueFiles = @(
    'HostRuntimeAdmissionIdKind.cs'
    'HostRuntimePriority.cs'
    'HostRuntimePriorityClass.cs'
    'HostRuntimePriorityOrder.cs'
    'HostRuntimeQueueAdmission.cs'
    'HostRuntimeQueueAdmissionFlow.cs'
    'HostRuntimeQueueAdmissionResult.cs'
    'HostRuntimeQueueAdmissionStatus.cs'
    'HostRuntimeQueueCapacity.cs'
    'HostRuntimeQueueIdKind.cs'
    'HostRuntimeQueueSnapshot.cs'
)
foreach ($hostQueueFile in $hostQueueFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostQueueDirectory $hostQueueFile))) {
        throw "Host queue contract is missing: $hostQueueFile"
    }
}
$hostQueueText = (
    $hostQueueFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostQueueDirectory $_) -Raw
        } |
        Out-String)
foreach ($requiredHostQueueToken in @(
    'HostRuntimeQueueAdmissionStatus.Admitted'
    'HostRuntimeQueueAdmissionStatus.StaleQueueRevision'
    'HostRuntimeQueueAdmissionStatus.InvalidRequestState'
    'HostRuntimeQueueAdmissionStatus.QueueFull'
    'HostRuntimeRequestState.Pending'
    'Unchanged<TRequest>'
    'checked(snapshot.QueuedCount + 1)'
    'checked(snapshot.Revision + 1)'
    'right.Class.CompareTo(left.Class)'
    'left.Sequence.CompareTo(right.Sequence)'
)) {
    if ($hostQueueText -notmatch [regex]::Escape(
        $requiredHostQueueToken)) {
        throw "Host queue token is missing: $requiredHostQueueToken"
    }
}
if ($hostQueueText -match '\b(ConcurrentQueue|PriorityQueue|Channel|BlockingCollection|Task\.Run|Task\.Delay|Timer|Thread|Semaphore|Monitor|HttpClient|Socket|NamedPipe|Process\.Start)\b') {
    throw 'Host queue contracts contain concrete storage, scheduling, waiting, transport, or execution.'
}
if ($hostQueueText -match '\bpublic\s+set\s*;') {
    throw 'Host queue contracts expose a public setter.'
}
if ($hostQueueText -match
    'return\s+Unchanged\s*\(') {
    throw (
        'Host queue admission flow must specify Unchanged<TRequest> ' +
        'explicitly.')
}
$hostQueueTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\HostRuntimeQueueAdmissionPriorityTests.cs')
if (-not (Test-Path -LiteralPath $hostQueueTestPath)) {
    throw 'Host queue admission and priority tests are missing.'
}
$hostQueueTestText = Get-Content -LiteralPath (
    $hostQueueTestPath) -Raw
foreach ($testName in @(
    'IdsCapacityAndBoundsAreValidated'
    'PriorityOrderingIsDeterministic'
    'QueueSnapshotPreservesExternalAuthority'
    'PendingRequestIsAdmittedImmutably'
    'StaleRevisionIsExplicitAndUnchanged'
    'FullQueueAndInvalidRequestStateAreExplicit'
    'AdmissionPreservesRequestAndPriorityIdentity'
    'ContractsDoNotStoreScheduleOrExecuteRequest'
)) {
    if ($hostQueueTestText -notmatch $testName) {
        throw "Host queue test is missing: $testName"
    }
}
$hostLeaseDirectory = Join-Path $root 'src\AI.Sandbox.Engine.Core\HostRuntime'
$hostLeaseFiles = @(
    'HostRuntimeLeaseIdKind.cs'
    'HostRuntimeLeaseState.cs'
    'HostRuntimeLeaseTransitionResult.cs'
    'HostRuntimeLeaseTransitionStatus.cs'
    'HostRuntimeWorkerIdKind.cs'
    'HostRuntimeWorkLease.cs'
    'HostRuntimeWorkLeaseFlow.cs'
)
foreach ($fileName in $hostLeaseFiles) {
    if (-not (Test-Path -LiteralPath (Join-Path $hostLeaseDirectory $fileName))) {
        throw "Host lease contract is missing: $fileName"
    }
}
$hostLeaseText = ($hostLeaseFiles | ForEach-Object {
    Get-Content -LiteralPath (Join-Path $hostLeaseDirectory $_) -Raw
} | Out-String)
foreach ($token in @(
    'HostRuntimeLeaseState.Active'
    'HostRuntimeLeaseState.Released'
    'HostRuntimeLeaseState.Expired'
    'HostRuntimeLeaseTransitionStatus.WorkerMismatch'
    'HostRuntimeLeaseTransitionStatus.ClockMismatch'
    'checked(acquiredTick + durationTicks)'
    'checked(observedTick + durationTicks)'
)) {
    if ($hostLeaseText -notmatch [regex]::Escape($token)) {
        throw "Host lease token is missing: $token"
    }
}
if ($hostLeaseText -match '\b(Task\.Run|Task\.Delay|Timer|Thread|Semaphore|Monitor|ConcurrentQueue|PriorityQueue|HttpClient|Socket|NamedPipe|Process\.Start|DateTime|DateTimeOffset|Stopwatch)\b') {
    throw 'Host lease contracts contain execution, waiting, concrete scheduling, transport, or wall-clock coupling.'
}
if ($hostLeaseText -match '\bpublic\s+set\s*;') {
    throw 'Host lease contracts expose a public setter.'
}
foreach ($documentedLeaseToken in @(
    '/// <summary>Gets the explicit lease transition outcome.</summary>'
    '/// <summary>Gets the externally assigned lease ID.</summary>'
    '/// <summary>Gets the externally assigned worker ID.</summary>'
    '/// <summary>Acquires immutable worker ownership of one admitted request.</summary>'
    '/// <summary>Renews one active lease using external monotonic time.</summary>'
    '/// <summary>Releases one active lease for its exact named worker.</summary>'
    '/// <summary>Expires one active lease when external time reaches its boundary.</summary>'
)) {
    if ($hostLeaseText -notmatch [regex]::Escape(
        $documentedLeaseToken)) {
        throw "Host lease XML documentation is missing: $documentedLeaseToken"
    }
}
$hostLeaseTestPath = Join-Path $root 'tests\AI.Sandbox.Engine.Core.Tests\HostRuntimeLeaseWorkerOwnershipTests.cs'
if (-not (Test-Path -LiteralPath $hostLeaseTestPath)) {
    throw 'Host lease and worker-ownership tests are missing.'
}
$hostLeaseTestText = Get-Content -LiteralPath $hostLeaseTestPath -Raw
foreach ($testName in @(
    'IdsTicksAndDurationsAreValidated'
    'AcquisitionPreservesAdmissionAndOwner'
    'RenewalExtendsExclusiveExpiryImmutably'
    'ReleaseRequiresExactWorker'
    'ExpiryRequiresDueTickAndMatchingClock'
    'StaleRevisionAndInvalidStateAreExplicit'
    'LeasePreservesRequestQueueAndPriorityAuthority'
    'ContractsDoNotWaitScheduleOrExecuteWorker'
)) {
    if ($hostLeaseTestText -notmatch $testName) {
        throw "Host lease test is missing: $testName"
    }
}
$hostSelectionDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostSelectionFiles = @(
    'HostRuntimeDispatchSelection.cs'
    'HostRuntimeDispatchSelectionFlow.cs'
    'HostRuntimeDispatchSelectionIdKind.cs'
    'HostRuntimeDispatchSelectionResult.cs'
    'HostRuntimeDispatchSelectionStatus.cs'
)
foreach ($fileName in $hostSelectionFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostSelectionDirectory $fileName))) {
        throw "Host dispatch-selection contract is missing: $fileName"
    }
}

$hostSelectionText = (
    $hostSelectionFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostSelectionDirectory $_) -Raw
        } |
        Out-String)

$normalizedHostSelectionText =
    $hostSelectionText -replace '\s*\.\s*', '.'
foreach ($token in @(
    'HostRuntimeDispatchSelectionStatus.Selected'
    'HostRuntimeDispatchSelectionStatus.StaleQueueRevision'
    'HostRuntimeDispatchSelectionStatus.EmptyQueue'
    'HostRuntimeDispatchSelectionStatus.QueueMismatch'
    'HostRuntimeDispatchSelectionStatus.InvalidLeaseState'
    'HostRuntimeDispatchSelectionStatus.ClockMismatch'
    'HostRuntimeDispatchSelectionStatus.LeaseExpired'
    'checked(snapshot.QueuedCount - 1)'
    'checked(snapshot.Revision + 1)'
    'HostRuntimeDispatchFlow.Create'
    'lease.Admission.Request'
)) {
    if (-not $normalizedHostSelectionText.Contains($token)) {
        throw "Host dispatch-selection token is missing: $token"
    }
}

foreach ($documentationToken in @(
    '/// <summary>'
    '/// <typeparam name="TRequest">'
    '/// <param name="selectionId">'
    '/// <returns>'
)) {
    if ($hostSelectionText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host dispatch-selection XML documentation is missing: ' +
            $documentationToken)
    }
}

if ($hostSelectionText -match
    '\b(ConcurrentQueue|PriorityQueue|BlockingCollection|Channel|Task\.Run|Task\.Delay|Timer|Thread|Semaphore|Monitor|lock\s*\(|HttpClient|Socket|NamedPipe|Process\.Start|DateTime|DateTimeOffset|Stopwatch)\b') {
    throw (
        'Host dispatch-selection contracts contain concrete storage, ' +
        'scheduling, waiting, transport, execution, or wall-clock coupling.')
}
if ($hostSelectionText -match '\bpublic\s+set\s*;') {
    throw 'Host dispatch-selection contracts expose a public setter.'
}

$hostSelectionTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeDequeueDispatchSelectionTests.cs')
if (-not (Test-Path -LiteralPath $hostSelectionTestPath)) {
    throw 'Host dequeue and dispatch-selection tests are missing.'
}

$hostSelectionTestText = Get-Content -LiteralPath (
    $hostSelectionTestPath) -Raw
foreach ($testName in @(
    'IdsTicksAndQueueBoundsAreValidated'
    'SelectionDequeuesAndCreatesDispatchImmutably'
    'StaleRevisionEmptyQueueAndQueueMismatchAreExplicit'
    'LeaseStateClockAndExpiryAreValidated'
    'SelectionPreservesLeasePriorityAndRoutingAuthority'
    'DequeueRevisionAndCountAdvanceExactlyOnce'
    'DispatchValidationRemainsAuthoritative'
    'ContractsDoNotStoreQueueDispatchOrExecuteRequest'
)) {
    if ($hostSelectionTestText -notmatch $testName) {
        throw "Host dispatch-selection test is missing: $testName"
    }
}
$hostAcknowledgementFiles = @(
    'HostRuntimeAttemptIdKind.cs'
    'HostRuntimeDispatchAcknowledgementFlow.cs'
    'HostRuntimeDispatchAcknowledgementResult.cs'
    'HostRuntimeDispatchAcknowledgementStatus.cs'
    'HostRuntimeInFlightAttempt.cs'
)
foreach ($fileName in $hostAcknowledgementFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRuntimeDirectory $fileName))) {
        throw "Host dispatch-acknowledgement contract is missing: $fileName"
    }
}

$hostAcknowledgementText = (
    $hostAcknowledgementFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRuntimeDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostAcknowledgementText =
    $hostAcknowledgementText -replace '\s*\.\s*', '.'

foreach ($token in @(
    'HostRuntimeDispatchAcknowledgementStatus.Acknowledged'
    'HostRuntimeDispatchAcknowledgementStatus.StaleRequestRevision'
    'HostRuntimeDispatchAcknowledgementStatus.StaleLeaseRevision'
    'HostRuntimeDispatchAcknowledgementStatus.SelectionRequestMismatch'
    'HostRuntimeDispatchAcknowledgementStatus.SelectionLeaseMismatch'
    'HostRuntimeDispatchAcknowledgementStatus.InvalidRequestState'
    'HostRuntimeDispatchAcknowledgementStatus.InvalidLeaseState'
    'HostRuntimeDispatchAcknowledgementStatus.LeaseMismatch'
    'HostRuntimeDispatchAcknowledgementStatus.WorkerMismatch'
    'HostRuntimeDispatchAcknowledgementStatus.DispatchMismatch'
    'HostRuntimeDispatchAcknowledgementStatus.RequestMismatch'
    'HostRuntimeDispatchAcknowledgementStatus.AttemptNumberMismatch'
    'HostRuntimeDispatchAcknowledgementStatus.ClockMismatch'
    'HostRuntimeDispatchAcknowledgementStatus.BeforeLeaseAcquisition'
    'HostRuntimeDispatchAcknowledgementStatus.LeaseExpired'
    'public static class HostRuntimeDispatchAcknowledgementFlow'
    'Acknowledge<TRequest>('
    'new HostRuntimeInFlightAttempt<TRequest>'
    'HostRuntimeRequestState.Pending'
    'request.Revision'
    'lease.Revision'
    'attempt: null'
)) {
    if (-not $normalizedHostAcknowledgementText.Contains($token)) {
        throw "Host dispatch-acknowledgement token is missing: $token"
    }
}

foreach ($documentationToken in @(
    '/// <summary>'
    '/// <typeparam name="TRequest">'
    '/// <param name="attemptId">'
    '/// <param name="request">'
    '/// <param name="lease">'
    '/// <returns>'
)) {
    if ($hostAcknowledgementText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host dispatch-acknowledgement XML documentation is missing: ' +
            $documentationToken)
    }
}

if ($hostAcknowledgementText -match
    '\b(ConcurrentQueue|PriorityQueue|BlockingCollection|Channel|Task\.Run|Task\.Delay|Timer|Thread|Semaphore|Monitor|lock\s*\(|HttpClient|Socket|NamedPipe|Process\.Start|DateTime|DateTimeOffset|Stopwatch)\b') {
    throw (
        'Host dispatch-acknowledgement contracts contain concrete storage, ' +
        'scheduling, waiting, transport, execution, or wall-clock coupling.')
}
if ($hostAcknowledgementText -match '\bpublic\s+set\s*;') {
    throw 'Host dispatch-acknowledgement contracts expose a public setter.'
}

$hostAcknowledgementTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeInFlightDispatchAcknowledgementTests.cs')
if (-not (Test-Path -LiteralPath $hostAcknowledgementTestPath)) {
    throw 'Host in-flight dispatch-acknowledgement tests are missing.'
}

$hostAcknowledgementTestText = Get-Content -LiteralPath (
    $hostAcknowledgementTestPath) -Raw
foreach ($testName in @(
    'IdsRevisionsAttemptAndTickAreValidated'
    'AcknowledgementCreatesImmutableInFlightAuthority'
    'StaleRequestAndLeaseRevisionsAreExplicit'
    'SelectionAndAcknowledgementMismatchesAreExplicit'
    'RequestLeaseClockAndTimeBoundariesAreExplicit'
    'RenewedActiveLeaseCanBeAcknowledged'
    'ContractsDoNotExecuteOrMutateAuthority'
)) {
    if ($hostAcknowledgementTestText -notmatch $testName) {
        throw "Host dispatch-acknowledgement test is missing: $testName"
    }
}
$hostSettlementFiles = @(
    'HostRuntimeAttemptSettlement.cs'
    'HostRuntimeAttemptSettlementFlow.cs'
    'HostRuntimeAttemptSettlementResult.cs'
    'HostRuntimeAttemptSettlementStatus.cs'
    'HostRuntimeSettlementIdKind.cs'
)
foreach ($fileName in $hostSettlementFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRuntimeDirectory $fileName))) {
        throw "Host attempt-settlement contract is missing: $fileName"
    }
}

$hostSettlementText = (
    $hostSettlementFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRuntimeDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostSettlementText =
    $hostSettlementText -replace '\s*\.\s*', '.'

foreach ($token in @(
    'HostRuntimeAttemptSettlementStatus.Settled'
    'HostRuntimeAttemptSettlementStatus.StaleRequestRevision'
    'HostRuntimeAttemptSettlementStatus.StaleLeaseRevision'
    'HostRuntimeAttemptSettlementStatus.AttemptRequestMismatch'
    'HostRuntimeAttemptSettlementStatus.AttemptLeaseMismatch'
    'HostRuntimeAttemptSettlementStatus.InvalidRequestState'
    'HostRuntimeAttemptSettlementStatus.InvalidLeaseState'
    'HostRuntimeAttemptSettlementStatus.WorkerMismatch'
    'HostRuntimeAttemptSettlementStatus.ClockMismatch'
    'HostRuntimeAttemptSettlementStatus.BeforeAcknowledgement'
    'HostRuntimeAttemptSettlementStatus.LeaseExpired'
    'HostRuntimeAttemptSettlementStatus.CompletionMismatch'
    'HostRuntimeAttemptSettlementStatus.RequestTransitionRejected'
    'HostRuntimeAttemptSettlementStatus.LeaseTransitionRejected'
    'public static class HostRuntimeAttemptSettlementFlow'
    'Settle<TRequest, TCompletion>('
    'HostRuntimeCompletionRouter.Route'
    'HostRuntimeWorkLeaseFlow.Release'
    'new HostRuntimeAttemptSettlement<TRequest, TCompletion>'
    'HostRuntimeRequestState.CancellationRequested'
    'routing.Request'
    'release.Lease'
)) {
    if (-not $normalizedHostSettlementText.Contains($token)) {
        throw "Host attempt-settlement token is missing: $token"
    }
}

foreach ($documentationToken in @(
    '/// <summary>'
    '/// <typeparam name="TRequest">'
    '/// <typeparam name="TCompletion">'
    '/// <param name="settlementId">'
    '/// <param name="attempt">'
    '/// <param name="completion">'
    '/// <returns>'
)) {
    if ($hostSettlementText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host attempt-settlement XML documentation is missing: ' +
            $documentationToken)
    }
}

if ($hostSettlementText -match
    '\b(ConcurrentQueue|PriorityQueue|BlockingCollection|Channel|Task\.Run|Task\.Delay|Timer|Thread|Semaphore|Monitor|lock\s*\(|HttpClient|Socket|NamedPipe|Process\.Start|DateTime|DateTimeOffset|Stopwatch)\b') {
    throw (
        'Host attempt-settlement contracts contain concrete storage, ' +
        'scheduling, waiting, transport, execution, or wall-clock coupling.')
}
if ($hostSettlementText -match '\bpublic\s+set\s*;') {
    throw 'Host attempt-settlement contracts expose a public setter.'
}

$hostSettlementTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeAttemptSettlementTests.cs')
if (-not (Test-Path -LiteralPath $hostSettlementTestPath)) {
    throw 'Host attempt-settlement tests are missing.'
}

$hostSettlementTestText = Get-Content -LiteralPath (
    $hostSettlementTestPath) -Raw
foreach ($testName in @(
    'IdsRevisionsTickAndArgumentsAreValidated'
    'SettlementProducesTerminalRequestAndReleasedLease'
    'CompletionKindsProduceExactTerminalStates'
    'StaleRevisionsAndAttemptAuthorityMismatchesAreExplicit'
    'RequestLeaseWorkerClockAndTimeBoundariesAreExplicit'
    'CompletionIdentityMismatchIsExplicit'
    'RenewedActiveLeaseCanBeSettled'
    'ContractsDoNotExecuteOrMutateAuthority'
)) {
    if ($hostSettlementTestText -notmatch $testName) {
        throw "Host attempt-settlement test is missing: $testName"
    }
}
$hostRetryRequeueDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRetryRequeueFiles = @(
    'HostRuntimeRetryRequeue.cs'
    'HostRuntimeRetryRequeueFlow.cs'
    'HostRuntimeRetryRequeueIdKind.cs'
    'HostRuntimeRetryRequeueResult.cs'
    'HostRuntimeRetryRequeueStatus.cs'
)
foreach ($fileName in $hostRetryRequeueFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRetryRequeueDirectory $fileName))) {
        throw "Host retry-requeue contract is missing: $fileName"
    }
}

$hostRetryRequeueText = (
    $hostRetryRequeueFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRetryRequeueDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRetryRequeueText =
    $hostRetryRequeueText -replace '\s*\.\s*', '.'
foreach ($token in @(
    'public static class HostRuntimeRetryRequeueFlow'
    'Requeue<TRequest, TCompletion>('
    'HostRuntimeRetryRequeueStatus.Requeued'
    'HostRuntimeRetryRequeueStatus.InvalidSettlementOutcome'
    'HostRuntimeRetryRequeueStatus.RetryDenied'
    'HostRuntimeRetryRequeueStatus.SettlementRequestMismatch'
    'HostRuntimeRetryRequeueStatus.AttemptNumberMismatch'
    'HostRuntimeRetryRequeueStatus.ClockMismatch'
    'HostRuntimeRetryRequeueStatus.RetryDecisionBeforeSettlement'
    'HostRuntimeRetryRequeueStatus.BeforeSettlement'
    'HostRuntimeRetryRequeueStatus.BeforeRetryTick'
    'HostRuntimeRetryRequeueStatus.StaleQueueRevision'
    'HostRuntimeRetryRequeueStatus.QueueFull'
    'HostRuntimeRetryRequeueStatus.AdmissionRejected'
    'HostRuntimeQueueAdmissionFlow.Decide'
    'HostRuntimeRequestState.Pending'
    'checked(settlement.Request.Revision + 1)'
    'retryDecision.RetryAtTick'
    'retryDecision.Policy.RetryDelayTicks'
    'settlement.SettledTick'
    'settlement.Request'
    'admission.Admission!'
)) {
    if (-not $normalizedHostRetryRequeueText.Contains($token)) {
        throw "Host retry-requeue token is missing: $token"
    }
}

foreach ($documentationToken in @(
    '/// <summary>'
    '/// <typeparam name="TRequest">'
    '/// <typeparam name="TCompletion">'
    '/// <param name="requeueId">'
    '/// <param name="admissionId">'
    '/// <param name="settlement">'
    '/// <param name="retryDecision">'
    '/// <param name="snapshot">'
    '/// <param name="expectedQueueRevision">'
    '/// <param name="priority">'
    '/// <param name="clockId">'
    '/// <param name="observedTick">'
    '/// <returns>'
)) {
    if ($hostRetryRequeueText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host retry-requeue XML documentation is missing: ' +
            $documentationToken)
    }
}

if ($hostRetryRequeueText -match
    '\b(ConcurrentQueue|PriorityQueue|BlockingCollection|Channel|Task\.Run|Task\.Delay|Timer|Thread|Semaphore|Monitor|lock\s*\(|HttpClient|Socket|NamedPipe|Process\.Start|DateTime|DateTimeOffset|Stopwatch)\b') {
    throw (
        'Host retry-requeue contracts contain concrete storage, scheduling, ' +
        'waiting, transport, execution, or wall-clock coupling.')
}
if ($hostRetryRequeueText -match '\bpublic\s+set\s*;') {
    throw 'Host retry-requeue contracts expose a public setter.'
}

$hostRetryRequeueTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRetryRequeueTests.cs')
if (-not (Test-Path -LiteralPath $hostRetryRequeueTestPath)) {
    throw 'Host retry-requeue tests are missing.'
}

$hostRetryRequeueTestText = Get-Content -LiteralPath (
    $hostRetryRequeueTestPath) -Raw
foreach ($testName in @(
    'IdsRevisionTickAndArgumentsAreValidated'
    'FailedSettlementIsRequeuedImmutably'
    'RejectedSettlementIsRequeuedImmutably'
    'CompletedAndCancelledSettlementsAreNotRetryable'
    'DeniedRetryDecisionIsExplicitAndUnchanged'
    'SettlementRequestAttemptAndClockMismatchesAreExplicit'
    'SettlementAndRetryTickBoundariesAreExplicit'
    'StaleAndFullQueueRemainUnchanged'
    'RequeuePreservesLineagePriorityAndAttemptNumbers'
    'ContractsDoNotScheduleStoreOrExecutePayloads'
)) {
    if ($hostRetryRequeueTestText -notmatch $testName) {
        throw "Host retry-requeue test is missing: $testName"
    }
}

$hostDeadLetterDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostDeadLetterFiles = @(
    'HostRuntimeDeadLetterDisposition.cs'
    'HostRuntimeDeadLetterDispositionFlow.cs'
    'HostRuntimeDeadLetterDispositionIdKind.cs'
    'HostRuntimeDeadLetterDispositionKind.cs'
    'HostRuntimeDeadLetterDispositionResult.cs'
    'HostRuntimeDeadLetterDispositionStatus.cs'
)
foreach ($fileName in $hostDeadLetterFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostDeadLetterDirectory $fileName))) {
        throw "Host dead-letter disposition contract is missing: $fileName"
    }
}

$hostDeadLetterText = (
    $hostDeadLetterFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostDeadLetterDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostDeadLetterText =
    $hostDeadLetterText -replace '\s*\.\s*', '.'
foreach ($token in @(
    'public static class HostRuntimeDeadLetterDispositionFlow'
    'Dispose<TRequest, TCompletion>('
    'HostRuntimeDeadLetterDispositionStatus.Disposed'
    'HostRuntimeDeadLetterDispositionStatus.InvalidSettlementOutcome'
    'HostRuntimeDeadLetterDispositionStatus.SettlementRequestMismatch'
    'HostRuntimeDeadLetterDispositionStatus.AttemptNumberMismatch'
    'HostRuntimeDeadLetterDispositionStatus.ClockMismatch'
    'HostRuntimeDeadLetterDispositionStatus.BeforeSettlement'
    'HostRuntimeDeadLetterDispositionStatus.RetryStillAllowed'
    'HostRuntimeDeadLetterDispositionStatus.UnsupportedRetryDenial'
    'HostRuntimeRetryDecisionStatus.AttemptLimitReached'
    'HostRuntimeRetryDecisionStatus.DeadlineExceeded'
    'HostRuntimeDeadLetterDispositionKind.AttemptLimitReached'
    'HostRuntimeDeadLetterDispositionKind.DeadlineExceeded'
    'retryDecision.Request'
    'retryDecision.CompletedAttemptNumber'
    'retryDecision.ClockId'
    'retryDecision.ShouldRetry'
    'settlement.SettledTick'
    'new HostRuntimeDeadLetterDisposition<TRequest, TCompletion>'
    'ObservedTerminalRequestRevision'
    'RetryDecisionStatus'
)) {
    if (-not $normalizedHostDeadLetterText.Contains($token)) {
        throw "Host dead-letter disposition token is missing: $token"
    }
}

foreach ($documentationToken in @(
    '/// <summary>'
    '/// <typeparam name="TRequest">'
    '/// <typeparam name="TCompletion">'
    '/// <param name="dispositionId">'
    '/// <param name="settlement">'
    '/// <param name="retryDecision">'
    '/// <param name="clockId">'
    '/// <param name="disposedTick">'
    '/// <returns>'
)) {
    if ($hostDeadLetterText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host dead-letter disposition XML documentation is missing: ' +
            $documentationToken)
    }
}

if ($hostDeadLetterText -match
    '\b(ConcurrentQueue|PriorityQueue|BlockingCollection|Channel|Task\.Run|Task\.Delay|Timer|Thread|Semaphore|Monitor|lock\s*\(|HttpClient|Socket|NamedPipe|Process\.Start|DateTime|DateTimeOffset|Stopwatch)\b') {
    throw (
        'Host dead-letter disposition contracts contain concrete storage, ' +
        'scheduling, waiting, transport, execution, or wall-clock coupling.')
}
if ($hostDeadLetterText -match '\bpublic\s+set\s*;') {
    throw 'Host dead-letter disposition contracts expose a public setter.'
}

$hostDeadLetterTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeDeadLetterDispositionTests.cs')
if (-not (Test-Path -LiteralPath $hostDeadLetterTestPath)) {
    throw 'Host dead-letter disposition tests are missing.'
}

$hostDeadLetterTestText = Get-Content -LiteralPath (
    $hostDeadLetterTestPath) -Raw
foreach ($testName in @(
    'IdsTickAndArgumentsAreValidated'
    'AttemptLimitCreatesDeadLetterDisposition'
    'DeadlineExceededCreatesDeadLetterDisposition'
    'CompletedAndCancelledSettlementsAreRejected'
    'AllowedRetryCannotBeDeadLettered'
    'SettlementRequestAttemptAndClockMismatchesAreExplicit'
    'DispositionTickBeforeSettlementIsExplicit'
    'UnsupportedRetryDenialsRemainExplicit'
    'DispositionPreservesTerminalAuthorityAndLineage'
    'ContractsDoNotStoreScheduleOrExecutePayloads'
)) {
    if ($hostDeadLetterTestText -notmatch $testName) {
        throw "Host dead-letter disposition test is missing: $testName"
    }
}

$hostAbandonedAttemptDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostAbandonedAttemptFiles = @(
    'HostRuntimeAbandonedAttemptDisposition.cs'
    'HostRuntimeAbandonedAttemptDispositionFlow.cs'
    'HostRuntimeAbandonedAttemptDispositionIdKind.cs'
    'HostRuntimeAbandonedAttemptDispositionKind.cs'
    'HostRuntimeAbandonedAttemptDispositionResult.cs'
    'HostRuntimeAbandonedAttemptDispositionStatus.cs'
)
foreach ($fileName in $hostAbandonedAttemptFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostAbandonedAttemptDirectory $fileName))) {
        throw "Host abandoned-attempt disposition contract is missing: $fileName"
    }
}

$hostAbandonedAttemptText = (
    $hostAbandonedAttemptFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostAbandonedAttemptDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostAbandonedAttemptText =
    $hostAbandonedAttemptText -replace '\s*\.\s*', '.'
foreach ($token in @(
    'public static class HostRuntimeAbandonedAttemptDispositionFlow'
    'Dispose<TRequest>('
    'HostRuntimeAbandonedAttemptDispositionStatus.Disposed'
    'HostRuntimeAbandonedAttemptDispositionStatus.StaleRequestRevision'
    'HostRuntimeAbandonedAttemptDispositionStatus.StaleLeaseRevision'
    'HostRuntimeAbandonedAttemptDispositionStatus.AttemptRequestMismatch'
    'HostRuntimeAbandonedAttemptDispositionStatus.AttemptLeaseMismatch'
    'HostRuntimeAbandonedAttemptDispositionStatus.AttemptWorkerMismatch'
    'HostRuntimeAbandonedAttemptDispositionStatus.InvalidRequestState'
    'HostRuntimeAbandonedAttemptDispositionStatus.InvalidLeaseState'
    'HostRuntimeAbandonedAttemptDispositionStatus.ClockMismatch'
    'HostRuntimeAbandonedAttemptDispositionStatus.BeforeAcknowledgement'
    'HostRuntimeAbandonedAttemptDispositionStatus.CancellationNotRequested'
    'HostRuntimeAbandonedAttemptDispositionStatus.LeaseAlreadyExpired'
    'HostRuntimeAbandonedAttemptDispositionStatus.LeaseNotExpired'
    'HostRuntimeAbandonedAttemptDispositionStatus.RequestTransitionRejected'
    'HostRuntimeAbandonedAttemptDispositionStatus.LeaseTransitionRejected'
    'HostRuntimeAbandonedAttemptDispositionKind.CancellationRequested'
    'HostRuntimeAbandonedAttemptDispositionKind.LeaseExpired'
    'HostRuntimeRequestFlow.Finalize'
    'HostRuntimeWorkLeaseFlow.Release'
    'HostRuntimeWorkLeaseFlow.Expire'
    'new HostRuntimeAbandonedAttemptDisposition<TRequest>'
    'HostRuntimeRequestState.Cancelled'
    'HostRuntimeRequestState.Failed'
)) {
    if (-not $normalizedHostAbandonedAttemptText.Contains($token)) {
        throw "Host abandoned-attempt disposition token is missing: $token"
    }
}

foreach ($documentationToken in @(
    '/// <summary>'
    '/// <typeparam name="TRequest">'
    '/// <param name="dispositionId">'
    '/// <param name="kind">'
    '/// <param name="attempt">'
    '/// <param name="request">'
    '/// <param name="lease">'
    '/// <param name="expectedRequestRevision">'
    '/// <param name="expectedLeaseRevision">'
    '/// <param name="clockId">'
    '/// <param name="observedTick">'
    '/// <returns>'
)) {
    if ($hostAbandonedAttemptText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host abandoned-attempt disposition XML documentation is missing: ' +
            $documentationToken)
    }
}

if ($hostAbandonedAttemptText -match
    '\b(ConcurrentQueue|PriorityQueue|BlockingCollection|Channel|Task\.Run|Task\.Delay|Timer|Thread|Semaphore|Monitor|lock\s*\(|HttpClient|Socket|NamedPipe|Process\.Start|DateTime|DateTimeOffset|Stopwatch)\b') {
    throw (
        'Host abandoned-attempt disposition contracts contain concrete storage, ' +
        'scheduling, waiting, transport, execution, or wall-clock coupling.')
}
if ($hostAbandonedAttemptText -match '\bpublic\s+set\s*;') {
    throw 'Host abandoned-attempt disposition contracts expose a public setter.'
}

$hostAbandonedAttemptTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeAbandonedAttemptDispositionTests.cs')
if (-not (Test-Path -LiteralPath $hostAbandonedAttemptTestPath)) {
    throw 'Host abandoned-attempt disposition tests are missing.'
}

$hostAbandonedAttemptTestText = Get-Content -LiteralPath (
    $hostAbandonedAttemptTestPath) -Raw
foreach ($testName in @(
    'IdsRevisionsAndTickAreValidated'
    'CancellationDispositionCancelsRequestAndReleasesLease'
    'LeaseExpiryDispositionFailsPendingRequestAndExpiresLease'
    'LeaseExpiryPreservesCancellationAsCancelledTerminalState'
    'StaleRevisionsAreExplicit'
    'AttemptLineageWorkerAndClockMismatchesAreExplicit'
    'InvalidRequestAndLeaseStatesAreExplicit'
    'CancellationAndExpiryTimeBoundariesAreExplicit'
    'DispositionPreservesAuthorityWithoutExecutingPayloads'
)) {
    if ($hostAbandonedAttemptTestText -notmatch $testName) {
        throw "Host abandoned-attempt disposition test is missing: $testName"
    }
}

$hostActiveWorkDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostActiveWorkFiles = @(
    'HostRuntimeActiveWorkSnapshotIdKind.cs'
    'HostRuntimeActiveWorkReconciliationIdKind.cs'
    'HostRuntimeActiveWorkItem.cs'
    'HostRuntimeActiveWorkSnapshot.cs'
    'HostRuntimeActiveWorkReconciliation.cs'
    'HostRuntimeActiveWorkStatus.cs'
    'HostRuntimeActiveWorkSnapshotResult.cs'
    'HostRuntimeActiveWorkReconciliationResult.cs'
    'HostRuntimeActiveWorkFlow.cs'
)
foreach ($fileName in $hostActiveWorkFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostActiveWorkDirectory $fileName))) {
        throw "Host active-work contract is missing: $fileName"
    }
}

$hostActiveWorkText = (
    $hostActiveWorkFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostActiveWorkDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostActiveWorkText =
    $hostActiveWorkText -replace '\s*\.\s*', '.'
foreach ($token in @(
    'public static class HostRuntimeActiveWorkFlow'
    'Capture<TRequest>('
    'Reconcile<TRequest>('
    'HostRuntimeActiveWorkStatus.Captured'
    'HostRuntimeActiveWorkStatus.Reconciled'
    'HostRuntimeActiveWorkStatus.TooManyItems'
    'HostRuntimeActiveWorkStatus.DuplicateAttempt'
    'HostRuntimeActiveWorkStatus.RuntimeMismatch'
    'HostRuntimeActiveWorkStatus.ClockMismatch'
    'HostRuntimeActiveWorkStatus.AttemptRequestMismatch'
    'HostRuntimeActiveWorkStatus.AttemptLeaseMismatch'
    'HostRuntimeActiveWorkStatus.AttemptWorkerMismatch'
    'HostRuntimeActiveWorkStatus.RequestRevisionRegressed'
    'HostRuntimeActiveWorkStatus.LeaseRevisionRegressed'
    'HostRuntimeActiveWorkStatus.InvalidRequestState'
    'HostRuntimeActiveWorkStatus.InvalidLeaseState'
    'HostRuntimeActiveWorkStatus.BeforeAcknowledgement'
    'HostRuntimeActiveWorkStatus.LeaseExpired'
    'HostRuntimeActiveWorkStatus.StaleSnapshotRevision'
    'HostRuntimeActiveWorkStatus.NonSequentialSnapshotRevision'
    'HostRuntimeActiveWorkStatus.ObservationTickRegressed'
    'HostRuntimeActiveWorkStatus.RetainedAttemptLineageMismatch'
    'HostRuntimeActiveWorkStatus.RetainedRequestRevisionRegressed'
    'HostRuntimeActiveWorkStatus.RetainedLeaseRevisionRegressed'
    'new HostRuntimeActiveWorkSnapshot<TRequest>'
    'new HostRuntimeActiveWorkReconciliation<TRequest>'
    'Array.AsReadOnly'
    'MaximumActiveWorkItemCount = 256'
)) {
    if (-not $normalizedHostActiveWorkText.Contains($token)) {
        throw "Host active-work token is missing: $token"
    }
}

foreach ($documentationToken in @(
    '/// <summary>'
    '/// <typeparam name="TRequest">'
    '/// <param name="snapshotId">'
    '/// <param name="runtimeInstanceId">'
    '/// <param name="clockId">'
    '/// <param name="observedTick">'
    '/// <param name="revision">'
    '/// <param name="items">'
    '/// <param name="reconciliationId">'
    '/// <param name="previousSnapshot">'
    '/// <param name="expectedPreviousRevision">'
    '/// <param name="currentSnapshot">'
    '/// <returns>'
)) {
    if ($hostActiveWorkText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host active-work XML documentation is missing: ' +
            $documentationToken)
    }
}

if ($hostActiveWorkText -match
    '\b(ConcurrentQueue|PriorityQueue|BlockingCollection|Channel|Task\.Run|Task\.Delay|Timer|Thread|Semaphore|Monitor|lock\s*\(|HttpClient|Socket|NamedPipe|Process\.Start|DateTime|DateTimeOffset|Stopwatch)\b') {
    throw (
        'Host active-work contracts contain concrete scheduling, waiting, ' +
        'transport, execution, concurrency, or wall-clock coupling.')
}
if ($hostActiveWorkText -match '\bpublic\s+set\s*;') {
    throw 'Host active-work contracts expose a public setter.'
}

$hostActiveWorkTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeActiveWorkReconciliationTests.cs')
if (-not (Test-Path -LiteralPath $hostActiveWorkTestPath)) {
    throw 'Host active-work reconciliation tests are missing.'
}

$hostActiveWorkTestText = Get-Content -LiteralPath (
    $hostActiveWorkTestPath) -Raw
foreach ($testName in @(
    'IdsRevisionsTicksAndCollectionsAreValidated'
    'SnapshotCapturesEmptyAndDeterministicallyOrdersActiveWork'
    'SnapshotRejectsTooManyAndDuplicateAttempts'
    'SnapshotLineageRuntimeWorkerAndClockMismatchesAreExplicit'
    'SnapshotStateRevisionAndTimeBoundariesAreExplicit'
    'ReconciliationComputesAddedRetainedAndRemovedAttempts'
    'ReconciliationSnapshotMismatchesAreExplicit'
    'ReconciliationRejectsChangedOrRegressedRetainedAuthority'
    'ContractsDefensivelyCopyCollectionsWithoutInferringOutcomes'
)) {
    if ($hostActiveWorkTestText -notmatch $testName) {
        throw "Host active-work reconciliation test is missing: $testName"
    }
}

$hostRecoveryDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryFiles = @(
    'HostRuntimeRecoveryCheckpointIdKind.cs'
    'HostRuntimeRecoveryContinuationIdKind.cs'
    'HostRuntimeRecoveryStatus.cs'
    'HostRuntimeRecoveryCheckpoint.cs'
    'HostRuntimeRecoveryContinuation.cs'
    'HostRuntimeRecoveryCheckpointResult.cs'
    'HostRuntimeRecoveryContinuationResult.cs'
    'HostRuntimeRecoveryFlow.cs'
)
foreach ($fileName in $hostRecoveryFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoveryDirectory $fileName))) {
        throw "Host recovery contract is missing: $fileName"
    }
}

$hostRecoveryText = (
    $hostRecoveryFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryText =
    $hostRecoveryText -replace '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryCheckpointIdKind'
    'HostRuntimeRecoveryContinuationIdKind'
    'HostRuntimeRecoveryStatus'
    'HostRuntimeRecoveryCheckpoint'
    'HostRuntimeRecoveryContinuation'
    'HostRuntimeRecoveryCheckpointResult'
    'HostRuntimeRecoveryContinuationResult'
    'HostRuntimeRecoveryFlow'
)) {
    if ($hostRecoveryText -notmatch "\b$requiredType\b") {
        throw "Host recovery contract '$requiredType' is missing."
    }
}

foreach ($statusName in @(
    'CheckpointCreated'
    'ContinuationCreated'
    'RuntimeMismatch'
    'CompositionMismatch'
    'BeforeActiveWorkObservation'
    'UnsupportedWorldSnapshotFormat'
    'WorldSnapshotChecksumMismatch'
    'StaleCheckpointRevision'
    'RestoreFailed'
    'RestoredWorldMismatch'
    'RestoredWorldVersionMismatch'
    'RestoredSimulationTickMismatch'
    'ContinuationTickRegressed'
)) {
    if ($hostRecoveryText -notmatch (
        "(?m)^\s*$statusName\s*=\s*\d+\s*,?\s*$")) {
        throw "Host recovery status '$statusName' is missing."
    }
}

foreach ($requiredToken in @(
    'public static class HostRuntimeRecoveryFlow'
    'CaptureCheckpoint<TRequest>('
    'Continue<TRequest, TState>('
    'SnapshotFormatVersion.Current'
    'worldSnapshotDocument.HasValidChecksum'
    'restoreResult.WasRestored'
    'checked(checkpoint.Revision + 1)'
    'new HostRuntimeRecoveryCheckpoint<TRequest>('
    'new HostRuntimeRecoveryContinuation<TRequest, TState>('
)) {
    if ($normalizedHostRecoveryText -notmatch [regex]::Escape(
        $requiredToken)) {
        throw "Host recovery token is missing: $requiredToken"
    }
}

foreach ($documentationToken in @(
    '<typeparam name="TRequest">'
    '<typeparam name="TState">'
    '<param name="checkpointId">'
    '<param name="lifecycleSnapshot">'
    '<param name="composition">'
    '<param name="queueSnapshot">'
    '<param name="activeWorkSnapshot">'
    '<param name="worldSnapshotDocument">'
    '<param name="capturedTick">'
    '<param name="revision">'
    '<param name="continuationId">'
    '<param name="checkpoint">'
    '<param name="expectedCheckpointRevision">'
    '<param name="restoreResult">'
    '<param name="continuedTick">'
    '<returns>'
)) {
    if ($hostRecoveryText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw "Host recovery XML documentation is missing: $documentationToken"
    }
}

if ($hostRecoveryText -match '\bpublic\s+set\s*;') {
    throw 'Host recovery contracts expose a public setter.'
}
if ($hostRecoveryText -match (
    '\b(Task\.Run|Task\.Delay|Timer|Thread|HttpClient|Socket|NamedPipe|' +
    'Process\.Start|DateTime(?:Offset)?\.(?:Now|UtcNow)|Stopwatch|' +
    'System\.IO\.File|System\.IO\.Directory|FileStream)\b')) {
    throw (
        'Host recovery contracts contain storage, execution, scheduling, ' +
        'transport, process-control, or hidden-clock coupling.')
}

$hostRecoveryTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryCheckpointContinuationTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryTestPath)) {
    throw 'Host recovery checkpoint and continuation tests are missing.'
}
$hostRecoveryTestText =
    Get-Content -LiteralPath $hostRecoveryTestPath -Raw
foreach ($testName in @(
    'IdsTicksRevisionsAndArgumentsAreValidated'
    'CheckpointCapturesExactImmutableAuthorities'
    'CheckpointRuntimeAndCompositionMismatchesAreExplicit'
    'CheckpointTimeAndWorldDocumentBoundariesAreExplicit'
    'ContinuationRestoresMatchingWorldAndAdvancesRevision'
    'ContinuationStaleRevisionAndTickRegressionAreExplicit'
    'ContinuationRestoreFailureIsExplicit'
    'ContinuationWorldIdentityVersionAndTickMismatchesAreExplicit'
    'ContractsPreserveAuthorityWithoutStorageOrAutomaticRestart'
)) {
    if ($hostRecoveryTestText -notmatch $testName) {
        throw "Host recovery test is missing: $testName"
    }
}

$hostRecoveryResumptionDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryResumptionFiles = @(
    'HostRuntimeRecoveryResumptionPlanIdKind.cs'
    'HostRuntimeResumedWorkSelectionIdKind.cs'
    'HostRuntimeRecoveryResumptionStatus.cs'
    'HostRuntimeRecoveryResumptionPlan.cs'
    'HostRuntimeResumedWorkSelection.cs'
    'HostRuntimeRecoveryResumptionPlanResult.cs'
    'HostRuntimeResumedWorkSelectionResult.cs'
    'HostRuntimeRecoveryResumptionFlow.cs'
)
foreach ($fileName in $hostRecoveryResumptionFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoveryResumptionDirectory $fileName))) {
        throw "Host recovery resumption contract is missing: $fileName"
    }
}

$hostRecoveryResumptionText = (
    $hostRecoveryResumptionFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryResumptionDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryResumptionText =
    $hostRecoveryResumptionText -replace '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryResumptionPlanIdKind'
    'HostRuntimeResumedWorkSelectionIdKind'
    'HostRuntimeRecoveryResumptionStatus'
    'HostRuntimeRecoveryResumptionPlan'
    'HostRuntimeResumedWorkSelection'
    'HostRuntimeRecoveryResumptionPlanResult'
    'HostRuntimeResumedWorkSelectionResult'
    'HostRuntimeRecoveryResumptionFlow'
)) {
    if ($hostRecoveryResumptionText -notmatch "\b$requiredType\b") {
        throw "Host recovery resumption contract '$requiredType' is missing."
    }
}

foreach ($statusName in @(
    'PlanCreated'
    'SelectionCreated'
    'StaleContinuationRevision'
    'PlanningTickRegressed'
    'NoResumableWork'
    'StalePlanRevision'
    'SelectionTickRegressed'
    'AttemptNotPlanned'
)) {
    if ($hostRecoveryResumptionText -notmatch (
        "(?m)^\s*$statusName\s*=\s*\d+\s*,?\s*$")) {
        throw "Host recovery resumption status '$statusName' is missing."
    }
}

foreach ($requiredToken in @(
    'public static class HostRuntimeRecoveryResumptionFlow'
    'Plan<TRequest, TState>('
    'Select<TRequest, TState>('
    'HostRuntimeRequestState.Pending'
    'HostRuntimeRequestState.CancellationRequested'
    'candidates.ToArray()'
    'suppressedAttemptIds.ToArray()'
    'checked(plan.Revision + 1)'
    'new HostRuntimeRecoveryResumptionPlan<TRequest, TState>('
    'new HostRuntimeResumedWorkSelection<TRequest, TState>('
)) {
    if ($normalizedHostRecoveryResumptionText -notmatch [regex]::Escape(
        $requiredToken)) {
        throw "Host recovery resumption token is missing: $requiredToken"
    }
}

foreach ($documentationToken in @(
    '<typeparam name="TRequest">'
    '<typeparam name="TState">'
    '<param name="planId">'
    '<param name="continuation">'
    '<param name="expectedContinuationRevision">'
    '<param name="plannedTick">'
    '<param name="revision">'
    '<param name="selectionId">'
    '<param name="plan">'
    '<param name="expectedPlanRevision">'
    '<param name="attemptId">'
    '<param name="selectedTick">'
    '<returns>'
)) {
    if ($hostRecoveryResumptionText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host recovery resumption XML documentation is missing: ' +
            $documentationToken)
    }
}

if ($hostRecoveryResumptionText -match '\bpublic\s+set\s*;') {
    throw 'Host recovery resumption contracts expose a public setter.'
}
if ($hostRecoveryResumptionText -match (
    '\b(Task\.Run|Task\.Delay|Timer|Thread|HttpClient|Socket|NamedPipe|' +
    'Process\.Start|DateTime(?:Offset)?\.(?:Now|UtcNow)|Stopwatch|' +
    'System\.IO\.File|System\.IO\.Directory|FileStream|' +
    'HostRuntimeQueueAdmissionFlow|HostRuntimeWorkLeaseFlow|' +
    'HostRuntimeDispatchSelectionFlow|HostRuntimeDispatchFlow|' +
    'HostRuntimeDispatchAcknowledgementFlow)\b')) {
    throw (
        'Host recovery resumption contracts contain restart, storage, ' +
        'ownership, dispatch, execution, scheduling, transport, process, ' +
        'or hidden-clock coupling.')
}

$hostRecoveryResumptionTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryResumptionSelectionTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryResumptionTestPath)) {
    throw 'Host recovery resumption and selection tests are missing.'
}
$hostRecoveryResumptionTestText =
    Get-Content -LiteralPath $hostRecoveryResumptionTestPath -Raw
foreach ($testName in @(
    'IdsTicksRevisionsAndArgumentsAreValidated'
    'PlanBuildsDeterministicPendingCandidatesAndSuppressesCancellation'
    'PlanStaleContinuationRevisionAndTickRegressionAreExplicit'
    'PlanWithOnlyCancellationRequestedWorkIsExplicit'
    'SelectionPreservesExactCandidateAuthorityAndAdvancesRevision'
    'SelectionStaleRevisionAndTickRegressionAreExplicit'
    'SelectionMissingCandidateIsExplicit'
    'PlanCollectionsAreDefensivelyOwnedAndDeterministicallyBounded'
    'ContractsRemainAdvisoryWithoutRestartSchedulingOrExecution'
)) {
    if ($hostRecoveryResumptionTestText -notmatch $testName) {
        throw "Host recovery resumption test is missing: $testName"
    }
}


$hostRecoveryReadmissionDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryReadmissionFiles = @(
    'HostRuntimeRecoveryReadmissionIdKind.cs'
    'HostRuntimeRecoveryLeaseReacquisitionIdKind.cs'
    'HostRuntimeRecoveryReadmissionStatus.cs'
    'HostRuntimeRecoveryReadmission.cs'
    'HostRuntimeRecoveryLeaseReacquisition.cs'
    'HostRuntimeRecoveryReadmissionResult.cs'
    'HostRuntimeRecoveryLeaseReacquisitionResult.cs'
    'HostRuntimeRecoveryReadmissionFlow.cs'
)
foreach ($fileName in $hostRecoveryReadmissionFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoveryReadmissionDirectory $fileName))) {
        throw "Host recovery readmission contract is missing: $fileName"
    }
}

$hostRecoveryReadmissionText = (
    $hostRecoveryReadmissionFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryReadmissionDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryReadmissionText =
    $hostRecoveryReadmissionText -replace '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryReadmissionIdKind'
    'HostRuntimeRecoveryLeaseReacquisitionIdKind'
    'HostRuntimeRecoveryReadmissionStatus'
    'HostRuntimeRecoveryReadmission'
    'HostRuntimeRecoveryLeaseReacquisition'
    'HostRuntimeRecoveryReadmissionResult'
    'HostRuntimeRecoveryLeaseReacquisitionResult'
    'HostRuntimeRecoveryReadmissionFlow'
)) {
    if ($hostRecoveryReadmissionText -notmatch "\b$requiredType\b") {
        throw "Host recovery readmission contract '$requiredType' is missing."
    }
}

foreach ($statusName in @(
    'Readmitted'
    'LeaseReacquired'
    'StaleSelectionRevision'
    'ReadmissionTickRegressed'
    'QueueMismatch'
    'PriorAdmissionIdReused'
    'StaleQueueRevision'
    'QueueFull'
    'AdmissionRejected'
    'StaleReadmissionRevision'
    'ReacquisitionTickRegressed'
    'ClockMismatch'
    'PriorLeaseIdReused'
)) {
    if ($hostRecoveryReadmissionText -notmatch (
        "(?m)^\s*$statusName\s*=\s*\d+\s*,?\s*$")) {
        throw "Host recovery readmission status '$statusName' is missing."
    }
}

foreach ($requiredToken in @(
    'public static class HostRuntimeRecoveryReadmissionFlow'
    'Readmit<TRequest, TState>('
    'Reacquire<TRequest, TState>('
    'HostRuntimeQueueAdmissionFlow.Decide'
    'HostRuntimeWorkLeaseFlow.Acquire'
    'priorAdmission.QueueId != checkpointQueue.QueueId'
    'admissionId == priorAdmission.AdmissionId'
    'leaseId == readmission.Selection.Candidate.LeaseId'
    'checked(selection.Revision + 1)'
    'checked(readmission.Revision + 1)'
    'new HostRuntimeRecoveryReadmission<TRequest, TState>('
    'new HostRuntimeRecoveryLeaseReacquisition<TRequest, TState>('
)) {
    if ($normalizedHostRecoveryReadmissionText -notmatch [regex]::Escape(
        $requiredToken)) {
        throw "Host recovery readmission token is missing: $requiredToken"
    }
}

foreach ($documentationToken in @(
    '<typeparam name="TRequest">'
    '<typeparam name="TState">'
    '<param name="readmissionId">'
    '<param name="admissionId">'
    '<param name="selection">'
    '<param name="expectedSelectionRevision">'
    '<param name="snapshot">'
    '<param name="expectedQueueRevision">'
    '<param name="readmittedTick">'
    '<param name="reacquisitionId">'
    '<param name="leaseId">'
    '<param name="workerId">'
    '<param name="readmission">'
    '<param name="expectedReadmissionRevision">'
    '<param name="clockId">'
    '<param name="reacquiredTick">'
    '<param name="durationTicks">'
    '<returns>'
)) {
    if ($hostRecoveryReadmissionText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host recovery readmission XML documentation is missing: ' +
            $documentationToken)
    }
}

if ($hostRecoveryReadmissionText -match '\bpublic\s+set\s*;') {
    throw 'Host recovery readmission contracts expose a public setter.'
}
if ($hostRecoveryReadmissionText -match (
    '\b(Task\.Run|Task\.Delay|Timer|Thread|HttpClient|Socket|NamedPipe|' +
    'Process\.Start|DateTime(?:Offset)?\.(?:Now|UtcNow)|Stopwatch|' +
    'System\.IO\.File|System\.IO\.Directory|FileStream|' +
    'HostRuntimeDispatchSelectionFlow|HostRuntimeDispatchFlow|' +
    'HostRuntimeDispatchAcknowledgementFlow|HostRuntimeRetryRequeueFlow)\b')) {
    throw (
        'Host recovery readmission contracts contain restart, storage, ' +
        'dispatch, attempt, execution, scheduling, transport, process, ' +
        'or hidden-clock coupling.')
}

$hostRecoveryReadmissionTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryReadmissionLeaseReacquisitionTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryReadmissionTestPath)) {
    throw 'Host recovery readmission and lease-reacquisition tests are missing.'
}
$hostRecoveryReadmissionTestText =
    Get-Content -LiteralPath $hostRecoveryReadmissionTestPath -Raw
foreach ($testName in @(
    'IdsTicksRevisionsDurationsAndArgumentsAreValidated'
    'ReadmissionPreservesSelectionAndCreatesAdmission'
    'ReadmissionStaleSelectionRevisionAndTickRegressionAreExplicit'
    'ReadmissionQueueLineageAndPriorAdmissionReuseAreExplicit'
    'ReadmissionAdmissionFailuresPreserveQueueAuthority'
    'ReacquisitionCreatesNewLeaseAndAdvancesRevision'
    'ReacquisitionStaleRevisionTickClockAndPriorLeaseReuseAreExplicit'
    'ReacquisitionPreservesRequestQueueAttemptAndWorkerLineage'
    'ContractsRemainBoundedWithoutDispatchSchedulingOrExecution'
)) {
    if ($hostRecoveryReadmissionTestText -notmatch $testName) {
        throw "Host recovery readmission test is missing: $testName"
    }
}

$hostRecoveryDispatchDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryDispatchFiles = @(
    'HostRuntimeRecoveryDispatchReconstructionIdKind.cs'
    'HostRuntimeRecoveryResumedAttemptAcknowledgementIdKind.cs'
    'HostRuntimeRecoveryDispatchStatus.cs'
    'HostRuntimeRecoveryDispatchReconstruction.cs'
    'HostRuntimeRecoveryResumedAttemptAcknowledgement.cs'
    'HostRuntimeRecoveryDispatchReconstructionResult.cs'
    'HostRuntimeRecoveryResumedAttemptAcknowledgementResult.cs'
    'HostRuntimeRecoveryDispatchFlow.cs'
)
foreach ($fileName in $hostRecoveryDispatchFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoveryDispatchDirectory $fileName))) {
        throw "Host recovery dispatch contract is missing: $fileName"
    }
}

$hostRecoveryDispatchText = (
    $hostRecoveryDispatchFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryDispatchDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryDispatchText =
    $hostRecoveryDispatchText -replace '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryDispatchReconstructionIdKind'
    'HostRuntimeRecoveryResumedAttemptAcknowledgementIdKind'
    'HostRuntimeRecoveryDispatchStatus'
    'HostRuntimeRecoveryDispatchReconstruction'
    'HostRuntimeRecoveryResumedAttemptAcknowledgement'
    'HostRuntimeRecoveryDispatchReconstructionResult'
    'HostRuntimeRecoveryResumedAttemptAcknowledgementResult'
    'HostRuntimeRecoveryDispatchFlow'
)) {
    if ($hostRecoveryDispatchText -notmatch "\b$requiredType\b") {
        throw "Host recovery dispatch contract '$requiredType' is missing."
    }
}

foreach ($statusName in @(
    'DispatchReconstructed'
    'AttemptAcknowledged'
    'StaleReacquisitionRevision'
    'ReconstructionTickRegressed'
    'QueueMismatch'
    'PriorSelectionIdReused'
    'PriorDispatchIdReused'
    'AttemptNumberMismatch'
    'DispatchSelectionRejected'
    'StaleReconstructionRevision'
    'AcknowledgementTickRegressed'
    'PriorAttemptIdReused'
    'DispatchAcknowledgementRejected'
)) {
    if ($hostRecoveryDispatchText -notmatch (
        "(?m)^\s*$statusName\s*=\s*\d+\s*,?\s*$")) {
        throw "Host recovery dispatch status '$statusName' is missing."
    }
}

foreach ($requiredToken in @(
    'public static class HostRuntimeRecoveryDispatchFlow'
    'Reconstruct<TRequest, TState>('
    'Acknowledge<TRequest, TState>('
    'HostRuntimeDispatchSelectionFlow.Select'
    'HostRuntimeDispatchAcknowledgementFlow.Acknowledge'
    'selectionId == reacquisition.Selection.Candidate.Attempt.Selection.SelectionId'
    'dispatchId == reacquisition.Selection.Candidate.Attempt.DispatchId'
    'attemptId == reconstruction.PriorAttemptId'
    'checked(reacquisition.Revision + 1)'
    'checked(reconstruction.Revision + 1)'
    'new HostRuntimeRecoveryDispatchReconstruction<'
    'new HostRuntimeRecoveryResumedAttemptAcknowledgement<'
)) {
    if (-not $normalizedHostRecoveryDispatchText.Contains($requiredToken)) {
        throw "Host recovery dispatch token is missing: $requiredToken"
    }
}

foreach ($documentationToken in @(
    '<typeparam name="TRequest">'
    '<typeparam name="TState">'
    '<param name="reconstructionId">'
    '<param name="selectionId">'
    '<param name="reacquisition">'
    '<param name="expectedReacquisitionRevision">'
    '<param name="snapshot">'
    '<param name="expectedQueueRevision">'
    '<param name="clockId">'
    '<param name="reconstructedTick">'
    '<param name="dispatchId">'
    '<param name="routeId">'
    '<param name="endpointId">'
    '<param name="attemptNumber">'
    '<param name="acknowledgementId">'
    '<param name="attemptId">'
    '<param name="reconstruction">'
    '<param name="expectedReconstructionRevision">'
    '<param name="request">'
    '<param name="lease">'
    '<param name="expectedRequestRevision">'
    '<param name="expectedLeaseRevision">'
    '<param name="acknowledgedLeaseId">'
    '<param name="acknowledgedWorkerId">'
    '<param name="acknowledgedDispatchId">'
    '<param name="acknowledgedRequestId">'
    '<param name="acknowledgedAttemptNumber">'
    '<param name="acknowledgedTick">'
    '<returns>'
)) {
    if ($hostRecoveryDispatchText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host recovery dispatch XML documentation is missing: ' +
            $documentationToken)
    }
}

if ($hostRecoveryDispatchText -match '\bpublic\s+set\s*;') {
    throw 'Host recovery dispatch contracts expose a public setter.'
}
if ($hostRecoveryDispatchText -match (
    '\b(Task\.Run|Task\.Delay|Timer|Thread|HttpClient|Socket|NamedPipe|' +
    'Process\.Start|DateTime(?:Offset)?\.(?:Now|UtcNow)|Stopwatch|' +
    'System\.IO\.File|System\.IO\.Directory|FileStream)\b')) {
    throw (
        'Host recovery dispatch contracts contain restart, storage, ' +
        'execution, scheduling, transport, process, or hidden-clock coupling.')
}

$hostRecoveryDispatchTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryDispatchAcknowledgementTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryDispatchTestPath)) {
    throw 'Host recovery dispatch and acknowledgement tests are missing.'
}
$hostRecoveryDispatchTestText =
    Get-Content -LiteralPath $hostRecoveryDispatchTestPath -Raw
foreach ($testName in @(
    'IdsTicksRevisionsAttemptsAndArgumentsAreValidated'
    'ReconstructionCreatesNewDispatchAndAdvancesAuthority'
    'ReconstructionRevisionTimeQueueIdentityAndAttemptAreExplicit'
    'ReconstructionPreservesUnderlyingDispatchSelectionOutcomes'
    'AcknowledgementCreatesNewResumedAttemptAndAdvancesAuthority'
    'AcknowledgementRevisionTimeAndPriorAttemptReuseAreExplicit'
    'AcknowledgementPreservesUnderlyingValidationOutcomes'
    'RecoveryLineageAndRevisionsAdvanceExactlyOnce'
    'ContractsRemainBoundedWithoutTransportSchedulingOrExecution'
)) {
    if ($hostRecoveryDispatchTestText -notmatch $testName) {
        throw "Host recovery dispatch test is missing: $testName"
    }
}

$hostRecoverySettlementDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoverySettlementFiles = @(
    'HostRuntimeRecoveryResumedAttemptSettlementIdKind.cs'
    'HostRuntimeRecoveryCycleCompletionIdKind.cs'
    'HostRuntimeRecoverySettlementStatus.cs'
    'HostRuntimeRecoveryResumedAttemptSettlement.cs'
    'HostRuntimeRecoveryCycleCompletion.cs'
    'HostRuntimeRecoveryResumedAttemptSettlementResult.cs'
    'HostRuntimeRecoveryCycleCompletionResult.cs'
    'HostRuntimeRecoverySettlementFlow.cs'
)
foreach ($fileName in $hostRecoverySettlementFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoverySettlementDirectory $fileName))) {
        throw "Host recovery settlement contract is missing: $fileName"
    }
}

$hostRecoverySettlementText = (
    $hostRecoverySettlementFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoverySettlementDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoverySettlementText =
    $hostRecoverySettlementText -replace '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryResumedAttemptSettlementIdKind'
    'HostRuntimeRecoveryCycleCompletionIdKind'
    'HostRuntimeRecoverySettlementStatus'
    'HostRuntimeRecoveryResumedAttemptSettlement'
    'HostRuntimeRecoveryCycleCompletion'
    'HostRuntimeRecoveryResumedAttemptSettlementResult'
    'HostRuntimeRecoveryCycleCompletionResult'
    'HostRuntimeRecoverySettlementFlow'
)) {
    if ($hostRecoverySettlementText -notmatch "\b$requiredType\b") {
        throw "Host recovery settlement contract '$requiredType' is missing."
    }
}

foreach ($statusName in @(
    'AttemptSettled'
    'CycleCompleted'
    'StaleAcknowledgementRevision'
    'SettlementTickRegressed'
    'AttemptSettlementRejected'
    'StaleSettlementRevision'
    'CompletionTickRegressed'
)) {
    if ($hostRecoverySettlementText -notmatch (
        "(?m)^\s*$statusName\s*=\s*\d+\s*,?\s*$")) {
        throw "Host recovery settlement status '$statusName' is missing."
    }
}

foreach ($requiredToken in @(
    'public static class HostRuntimeRecoverySettlementFlow'
    'Settle<TRequest, TState, TCompletion>('
    'Complete<TRequest, TState, TCompletion>('
    'HostRuntimeAttemptSettlementFlow.Settle'
    'checked(acknowledgement.Revision + 1)'
    'checked(settlement.Revision + 1)'
    'new HostRuntimeRecoveryResumedAttemptSettlement<'
    'new HostRuntimeRecoveryCycleCompletion<'
)) {
    if (-not $normalizedHostRecoverySettlementText.Contains($requiredToken)) {
        throw "Host recovery settlement token is missing: $requiredToken"
    }
}

foreach ($documentationToken in @(
    '<typeparam name="TRequest">'
    '<typeparam name="TState">'
    '<typeparam name="TCompletion">'
    '<param name="recoverySettlementId">'
    '<param name="settlementId">'
    '<param name="acknowledgement">'
    '<param name="expectedAcknowledgementRevision">'
    '<param name="request">'
    '<param name="lease">'
    '<param name="expectedRequestRevision">'
    '<param name="expectedLeaseRevision">'
    '<param name="settlingWorkerId">'
    '<param name="clockId">'
    '<param name="settledTick">'
    '<param name="completion">'
    '<param name="cycleCompletionId">'
    '<param name="settlement">'
    '<param name="expectedSettlementRevision">'
    '<param name="completedTick">'
    '<returns>'
)) {
    if ($hostRecoverySettlementText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host recovery settlement XML documentation is missing: ' +
            $documentationToken)
    }
}

if ($hostRecoverySettlementText -match '\bpublic\s+set\s*;') {
    throw 'Host recovery settlement contracts expose a public setter.'
}
if ($hostRecoverySettlementText -match (
    '\b(Task\.Run|Task\.Delay|Timer|Thread|HttpClient|Socket|NamedPipe|' +
    'Process\.Start|DateTime(?:Offset)?\.(?:Now|UtcNow)|Stopwatch|' +
    'System\.IO\.File|System\.IO\.Directory|FileStream)\b')) {
    throw (
        'Host recovery settlement contracts contain retry, storage, ' +
        'execution, scheduling, transport, process, or hidden-clock coupling.')
}

$hostRecoverySettlementTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoverySettlementCycleCompletionTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoverySettlementTestPath)) {
    throw 'Host recovery settlement and cycle-completion tests are missing.'
}
$hostRecoverySettlementTestText =
    Get-Content -LiteralPath $hostRecoverySettlementTestPath -Raw
foreach ($testName in @(
    'IdsTicksRevisionsAndArgumentsAreValidated'
    'SettlementCreatesTerminalAuthorityAndAdvancesRevision'
    'SettlementRevisionAndTimeAreExplicit'
    'SettlementPreservesUnderlyingAttemptSettlementOutcomes'
    'EveryTerminalCompletionKindCanCloseTheResumedAttempt'
    'CycleCompletionClosesExactSettlementAuthority'
    'CycleCompletionRevisionAndTimeAreExplicit'
    'RecoveryLineageAndRevisionsAdvanceExactlyOnce'
    'ContractsRemainBoundedWithoutRetryTransportOrExecution'
)) {
    if ($hostRecoverySettlementTestText -notmatch $testName) {
        throw "Host recovery settlement test is missing: $testName"
    }
}

$hostRecoveryCheckpointSupersessionDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryCheckpointSupersessionFiles = @(
    'HostRuntimeRecoveryCheckpointSupersessionIdKind.cs'
    'HostRuntimeRecoveryCompletedCycleSummaryIdKind.cs'
    'HostRuntimeRecoveryCheckpointSupersessionStatus.cs'
    'HostRuntimeRecoveryCheckpointSupersession.cs'
    'HostRuntimeRecoveryCompletedCycleSummary.cs'
    'HostRuntimeRecoveryCheckpointSupersessionResult.cs'
    'HostRuntimeRecoveryCompletedCycleSummaryResult.cs'
    'HostRuntimeRecoveryCheckpointSupersessionFlow.cs'
)
foreach ($fileName in $hostRecoveryCheckpointSupersessionFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoveryCheckpointSupersessionDirectory $fileName))) {
        throw "Host recovery checkpoint-supersession contract is missing: $fileName"
    }
}

$hostRecoveryCheckpointSupersessionText = (
    $hostRecoveryCheckpointSupersessionFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryCheckpointSupersessionDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryCheckpointSupersessionText =
    $hostRecoveryCheckpointSupersessionText -replace '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryCheckpointSupersessionIdKind'
    'HostRuntimeRecoveryCompletedCycleSummaryIdKind'
    'HostRuntimeRecoveryCheckpointSupersessionStatus'
    'HostRuntimeRecoveryCheckpointSupersession'
    'HostRuntimeRecoveryCompletedCycleSummary'
    'HostRuntimeRecoveryCheckpointSupersessionResult'
    'HostRuntimeRecoveryCompletedCycleSummaryResult'
    'HostRuntimeRecoveryCheckpointSupersessionFlow'
)) {
    if ($hostRecoveryCheckpointSupersessionText -notmatch "\b$requiredType\b") {
        throw (
            "Host recovery checkpoint-supersession contract " +
            "'$requiredType' is missing.")
    }
}

foreach ($statusName in @(
    'CheckpointSuperseded'
    'SummaryCreated'
    'StaleCycleCompletionRevision'
    'StaleSuccessorCheckpointRevision'
    'CheckpointIdReused'
    'RuntimeMismatch'
    'CompositionMismatch'
    'QueueMismatch'
    'ClockMismatch'
    'WorldMismatch'
    'SuccessorCheckpointTickRegressed'
    'SuccessorCheckpointRevisionNotAdvanced'
    'WorldStateVersionRegressed'
    'SimulationTickRegressed'
    'SupersessionTickRegressed'
    'StaleSupersessionRevision'
    'SummaryTickRegressed'
)) {
    if ($hostRecoveryCheckpointSupersessionText -notmatch (
        "(?m)^\s*$statusName\s*=\s*\d+\s*,?\s*$")) {
        throw (
            "Host recovery checkpoint-supersession status " +
            "'$statusName' is missing.")
    }
}

foreach ($requiredToken in @(
    'public static class HostRuntimeRecoveryCheckpointSupersessionFlow'
    'Supersede<TRequest, TState, TCompletion>('
    'Summarize<TRequest, TState, TCompletion>('
    'successorCheckpoint.CheckpointId == priorCheckpoint.CheckpointId'
    'successorCheckpoint.CapturedTick < cycleCompletion.CompletedTick'
    'successorCheckpoint.Revision <= priorCheckpoint.Revision'
    'successorCheckpoint.WorldSnapshotDocument.WorldStateVersion.CompareTo('
    'successorCheckpoint.WorldSnapshotDocument.SimulationTick <'
    'global::System.Math.Max('
    'checked(supersession.Revision + 1)'
    'new HostRuntimeRecoveryCheckpointSupersession<'
    'new HostRuntimeRecoveryCompletedCycleSummary<'
)) {
    if (-not $normalizedHostRecoveryCheckpointSupersessionText.Contains(
        $requiredToken)) {
        throw (
            'Host recovery checkpoint-supersession token is missing: ' +
            $requiredToken)
    }
}

foreach ($documentationToken in @(
    '<typeparam name="TRequest">'
    '<typeparam name="TState">'
    '<typeparam name="TCompletion">'
    '<param name="supersessionId">'
    '<param name="cycleCompletion">'
    '<param name="expectedCycleCompletionRevision">'
    '<param name="successorCheckpoint">'
    '<param name="expectedSuccessorCheckpointRevision">'
    '<param name="supersededTick">'
    '<param name="summaryId">'
    '<param name="supersession">'
    '<param name="expectedSupersessionRevision">'
    '<param name="summarizedTick">'
    '<returns>'
)) {
    if ($hostRecoveryCheckpointSupersessionText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host recovery checkpoint-supersession XML documentation is ' +
            'missing: ' + $documentationToken)
    }
}

if ($hostRecoveryCheckpointSupersessionText -match '\bpublic\s+set\s*;') {
    throw (
        'Host recovery checkpoint-supersession contracts expose a public setter.')
}
if ($hostRecoveryCheckpointSupersessionText -match (
    '\b(Task\.Run|Task\.Delay|Timer|Thread|HttpClient|Socket|NamedPipe|' +
    'Process\.Start|DateTime(?:Offset)?\.(?:Now|UtcNow)|Stopwatch|' +
    'System\.IO\.File|System\.IO\.Directory|FileStream)\b')) {
    throw (
        'Host recovery checkpoint-supersession contracts contain storage, ' +
        'archival, execution, scheduling, transport, process, or hidden-clock ' +
        'coupling.')
}

$hostRecoveryCheckpointSupersessionTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryCheckpointSupersessionSummaryTests.cs')
if (-not (Test-Path -LiteralPath (
    $hostRecoveryCheckpointSupersessionTestPath))) {
    throw (
        'Host recovery checkpoint-supersession and cycle-summary tests are ' +
        'missing.')
}
$hostRecoveryCheckpointSupersessionTestText =
    Get-Content -LiteralPath (
        $hostRecoveryCheckpointSupersessionTestPath) -Raw
foreach ($testName in @(
    'IdsTicksRevisionsAndArgumentsAreValidated'
    'OptimisticRevisionsAreExplicit'
    'CheckpointIdentityRevisionAndTimeMustAdvance'
    'RuntimeCompositionQueueClockAndWorldLineageMustMatch'
    'WorldSnapshotAuthorityCannotRegress'
    'SupersessionPreservesBothCheckpointAuthorities'
    'SummaryRevisionAndTimeAreExplicit'
    'SummaryIsACompactImmutableProjection'
    'EveryTerminalOutcomeCanBeSummarized'
)) {
    if ($hostRecoveryCheckpointSupersessionTestText -notmatch $testName) {
        throw (
            'Host recovery checkpoint-supersession test is missing: ' +
            $testName)
    }
}

foreach ($documentationToken in @(
    'checkpoint supersession'
    'completed-cycle summary'
    'without deleting'
    'without storage'
)) {
    if ($hostRecoveryCheckpointSupersessionText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host recovery checkpoint-supersession boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoverySupersessionChainDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoverySupersessionChainFiles = @(
    'HostRuntimeRecoverySupersessionChainIdKind.cs'
    'HostRuntimeRecoveryLatestCheckpointSelectionIdKind.cs'
    'HostRuntimeRecoverySupersessionChainStatus.cs'
    'HostRuntimeRecoverySupersessionChain.cs'
    'HostRuntimeRecoveryLatestCheckpointSelection.cs'
    'HostRuntimeRecoverySupersessionChainResult.cs'
    'HostRuntimeRecoveryLatestCheckpointSelectionResult.cs'
    'HostRuntimeRecoverySupersessionChainFlow.cs'
)
foreach ($fileName in $hostRecoverySupersessionChainFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoverySupersessionChainDirectory $fileName))) {
        throw "Host recovery supersession-chain contract is missing: $fileName"
    }
}

$hostRecoverySupersessionChainText = (
    $hostRecoverySupersessionChainFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoverySupersessionChainDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoverySupersessionChainText =
    $hostRecoverySupersessionChainText -replace '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoverySupersessionChainIdKind'
    'HostRuntimeRecoveryLatestCheckpointSelectionIdKind'
    'HostRuntimeRecoverySupersessionChainStatus'
    'HostRuntimeRecoverySupersessionChain'
    'HostRuntimeRecoveryLatestCheckpointSelection'
    'HostRuntimeRecoverySupersessionChainResult'
    'HostRuntimeRecoveryLatestCheckpointSelectionResult'
    'HostRuntimeRecoverySupersessionChainFlow'
)) {
    if ($hostRecoverySupersessionChainText -notmatch "\b$requiredType\b") {
        throw (
            "Host recovery supersession-chain contract " +
            "'$requiredType' is missing.")
    }
}

foreach ($statusName in @(
    'ChainValidated'
    'LatestCheckpointSelected'
    'EmptyChain'
    'TooManySupersessions'
    'SupersessionRevisionCountMismatch'
    'StaleSupersessionRevision'
    'DuplicateSupersessionId'
    'DuplicatePriorCheckpointId'
    'DuplicateSuccessorCheckpointId'
    'DisconnectedChain'
    'CycleDetected'
    'CheckpointAuthorityMismatch'
    'RuntimeMismatch'
    'CompositionMismatch'
    'QueueMismatch'
    'ClockMismatch'
    'WorldMismatch'
    'CheckpointRevisionRegressed'
    'CheckpointCaptureTickRegressed'
    'WorldStateVersionRegressed'
    'SimulationTickRegressed'
    'SupersessionTickRegressed'
    'ValidationTickRegressed'
    'StaleChainRevision'
    'SelectionTickRegressed'
)) {
    if ($hostRecoverySupersessionChainText -notmatch (
        "(?m)^\s*$statusName\s*=\s*\d+\s*,?\s*$")) {
        throw (
            "Host recovery supersession-chain status '$statusName' is missing.")
    }
}

foreach ($requiredToken in @(
    'MaximumSupersessionCount = 256'
    'Validate<TRequest, TState, TCompletion>'
    'SelectLatest<TRequest, TState, TCompletion>'
    'Array.AsReadOnly'
    'IReadOnlyList'
    'CheckpointAuthorityMatches'
)) {
    if ($normalizedHostRecoverySupersessionChainText -notmatch (
        [regex]::Escape($requiredToken))) {
        throw (
            'Host recovery supersession-chain implementation token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoverySupersessionChainText -match '\bpublic\s+set\s*;') {
    throw (
        'Host recovery supersession-chain contracts expose a public setter.')
}
if ($hostRecoverySupersessionChainText -match (
    '\b(Task\.Run|Task\.Delay|Timer|Thread|HttpClient|Socket|NamedPipe|' +
    'Process\.Start|DateTime(?:Offset)?\.(?:Now|UtcNow)|Stopwatch|' +
    'System\.IO\.File|System\.IO\.Directory|FileStream)\b')) {
    throw (
        'Host recovery supersession-chain contracts contain storage, ' +
        'archival, execution, scheduling, transport, process, or hidden-clock ' +
        'coupling.')
}

$hostRecoverySupersessionChainTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoverySupersessionChainSelectionTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoverySupersessionChainTestPath)) {
    throw (
        'Host recovery supersession-chain and latest-checkpoint tests are missing.')
}
$hostRecoverySupersessionChainTestText =
    Get-Content -LiteralPath $hostRecoverySupersessionChainTestPath -Raw
foreach ($testName in @(
    'IdsBoundsRevisionsAndArgumentsAreValidated'
    'EmptyAndOversizedChainsAreRejected'
    'OptimisticSupersessionRevisionsAreExplicit'
    'DuplicateAndDisconnectedAuthoritiesAreRejected'
    'ValidationTickAndRevisionAdvanceExactlyOnce'
    'ChainValidationPreservesOrderedAuthority'
    'LatestSelectionRevisionAndTimeAreExplicit'
    'LatestSelectionReturnsExactSuccessorCheckpoint'
    'ContractsRemainBoundedWithoutStorageOrHistoryMutation'
)) {
    if ($hostRecoverySupersessionChainTestText -notmatch $testName) {
        throw (
            'Host recovery supersession-chain test is missing: ' +
            $testName)
    }
}

foreach ($documentationToken in @(
    'supersession chain'
    'latest checkpoint'
    'without storage'
    'history mutation'
)) {
    if ($hostRecoverySupersessionChainText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host recovery supersession-chain boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryChainQueryDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryChainQueryFiles = @(
    'HostRuntimeRecoveryChainSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryCheckpointLineageQueryIdKind.cs'
    'HostRuntimeRecoveryChainQueryStatus.cs'
    'HostRuntimeRecoveryChainSummaryProjection.cs'
    'HostRuntimeRecoveryCheckpointLineageQuery.cs'
    'HostRuntimeRecoveryChainSummaryProjectionResult.cs'
    'HostRuntimeRecoveryCheckpointLineageQueryResult.cs'
    'HostRuntimeRecoveryChainQueryFlow.cs'
)
foreach ($fileName in $hostRecoveryChainQueryFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoveryChainQueryDirectory $fileName))) {
        throw "Host recovery chain-query contract is missing: $fileName"
    }
}

$hostRecoveryChainQueryText = (
    $hostRecoveryChainQueryFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryChainQueryDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryChainQueryText =
    $hostRecoveryChainQueryText -replace '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryChainSummaryProjectionIdKind'
    'HostRuntimeRecoveryCheckpointLineageQueryIdKind'
    'HostRuntimeRecoveryChainQueryStatus'
    'HostRuntimeRecoveryChainSummaryProjection'
    'HostRuntimeRecoveryCheckpointLineageQuery'
    'HostRuntimeRecoveryChainSummaryProjectionResult'
    'HostRuntimeRecoveryCheckpointLineageQueryResult'
    'HostRuntimeRecoveryChainQueryFlow'
)) {
    if ($hostRecoveryChainQueryText -notmatch "\b$requiredType\b") {
        throw (
            "Host recovery chain-query contract '$requiredType' is missing.")
    }
}

foreach ($statusName in @(
    'ChainSummaryProjected'
    'CheckpointLineageResolved'
    'StaleChainRevision'
    'ProjectionTickRegressed'
    'StaleProjectionRevision'
    'QueryTickRegressed'
    'CheckpointNotFound'
)) {
    if ($hostRecoveryChainQueryText -notmatch (
        "(?m)^\s*$statusName\s*=\s*\d+\s*,?\s*$")) {
        throw "Host recovery chain-query status '$statusName' is missing."
    }
}

foreach ($requiredToken in @(
    'ProjectSummary<TRequest, TState, TCompletion>'
    'QueryLineage<TRequest, TState, TCompletion>'
    'SupersessionCount'
    'CheckpointCount'
    'IncomingSupersession'
    'OutgoingSupersession'
    'CheckpointIndex'
    'IsRootCheckpoint'
    'IsLatestCheckpoint'
)) {
    if ($normalizedHostRecoveryChainQueryText -notmatch (
        [regex]::Escape($requiredToken))) {
        throw (
            'Host recovery chain-query implementation token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryChainQueryText -match '\bpublic\s+set\s*;') {
    throw 'Host recovery chain-query contracts expose a public setter.'
}
if ($hostRecoveryChainQueryText -match (
    '\b(Task\.Run|Task\.Delay|Timer|Thread|HttpClient|Socket|NamedPipe|' +
    'Process\.Start|DateTime(?:Offset)?\.(?:Now|UtcNow)|Stopwatch|' +
    'System\.IO\.File|System\.IO\.Directory|FileStream)\b')) {
    throw (
        'Host recovery chain-query contracts contain storage, archival, ' +
        'execution, scheduling, transport, process, or hidden-clock coupling.')
}

$hostRecoveryChainQueryTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryChainSummaryLineageQueryTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryChainQueryTestPath)) {
    throw 'Host recovery chain-summary and lineage-query tests are missing.'
}
$hostRecoveryChainQueryTestText =
    Get-Content -LiteralPath $hostRecoveryChainQueryTestPath -Raw
foreach ($testName in @(
    'IdsRevisionsTicksAndArgumentsAreValidated'
    'ProjectionRejectsStaleRevisionAndRegressedTick'
    'ProjectionPreservesExactValidatedChainAndCompactBounds'
    'RootCheckpointQueryResolvesOutgoingLineage'
    'LatestCheckpointQueryResolvesIncomingLineage'
    'QueryRejectsStaleProjectionRevision'
    'QueryRejectsRegressedTick'
    'QueryRejectsUnknownCheckpoint'
    'ContractsRemainImmutableBoundedAndSideEffectFree'
)) {
    if ($hostRecoveryChainQueryTestText -notmatch $testName) {
        throw 'Host recovery chain-query test is missing: ' + $testName
    }
}

foreach ($documentationToken in @(
    'compact immutable projection'
    'checkpoint-lineage query'
    'without discovery'
    'history mutation'
)) {
    if ($hostRecoveryChainQueryText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host recovery chain-query boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryLineageWindowDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryLineageWindowFiles = @(
    'HostRuntimeRecoveryLineageWindowProjectionIdKind.cs'
    'HostRuntimeRecoveryCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryLineageWindowStatus.cs'
    'HostRuntimeRecoveryLineageWindowProjection.cs'
    'HostRuntimeRecoveryCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryLineageWindowProjectionResult.cs'
    'HostRuntimeRecoveryCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryLineageWindowFlow.cs'
)
foreach ($fileName in $hostRecoveryLineageWindowFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoveryLineageWindowDirectory $fileName))) {
        throw "Host recovery lineage-window contract is missing: $fileName"
    }
}

$hostRecoveryLineageWindowText = (
    $hostRecoveryLineageWindowFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryLineageWindowDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryLineageWindowText =
    $hostRecoveryLineageWindowText -replace '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryLineageWindowProjectionIdKind'
    'HostRuntimeRecoveryCheckpointRangeQueryIdKind'
    'HostRuntimeRecoveryLineageWindowStatus'
    'HostRuntimeRecoveryLineageWindowProjection'
    'HostRuntimeRecoveryCheckpointRangeQuery'
    'HostRuntimeRecoveryLineageWindowProjectionResult'
    'HostRuntimeRecoveryCheckpointRangeQueryResult'
    'HostRuntimeRecoveryLineageWindowFlow'
)) {
    if ($hostRecoveryLineageWindowText -notmatch "\b$requiredType\b") {
        throw (
            "Host recovery lineage-window contract '$requiredType' is missing.")
    }
}

foreach ($statusName in @(
    'LineageWindowProjected'
    'CheckpointRangeResolved'
    'StaleProjectionRevision'
    'WindowProjectionTickRegressed'
    'WindowStartOutOfRange'
    'WindowEndOutOfRange'
    'TooManyWindowCheckpoints'
    'StaleWindowRevision'
    'RangeQueryTickRegressed'
    'RangeStartNotFound'
    'RangeEndNotFound'
    'RangeOrderInvalid'
)) {
    if ($hostRecoveryLineageWindowText -notmatch (
        "(?m)^\s*$statusName\s*=\s*\d+\s*,?\s*$")) {
        throw "Host recovery lineage-window status '$statusName' is missing."
    }
}

foreach ($requiredToken in @(
    'ProjectWindow<TRequest, TState, TCompletion>'
    'QueryRange<TRequest, TState, TCompletion>'
    'MaximumCheckpointCount'
    'StartCheckpointIndex'
    'EndCheckpointIndex'
    'StartWindowIndex'
    'EndWindowIndex'
    'StartChainIndex'
    'EndChainIndex'
    'IncomingSupersession'
    'OutgoingSupersession'
    'Checkpoints'
    'Supersessions'
)) {
    if ($normalizedHostRecoveryLineageWindowText -notmatch (
        [regex]::Escape($requiredToken))) {
        throw (
            'Host recovery lineage-window implementation token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryLineageWindowText -match '\bpublic\s+set\s*;') {
    throw 'Host recovery lineage-window contracts expose a public setter.'
}
if ($hostRecoveryLineageWindowText -match (
    '\b(Task\.Run|Task\.Delay|Timer|Thread|HttpClient|Socket|NamedPipe|' +
    'Process\.Start|DateTime(?:Offset)?\.(?:Now|UtcNow)|Stopwatch|' +
    'System\.IO\.File|System\.IO\.Directory|FileStream)\b')) {
    throw (
        'Host recovery lineage-window contracts contain storage, archival, ' +
        'execution, scheduling, transport, process, or hidden-clock coupling.')
}

$hostRecoveryLineageWindowTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryLineageWindowCheckpointRangeTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryLineageWindowTestPath)) {
    throw 'Host recovery lineage-window and checkpoint-range tests are missing.'
}
$hostRecoveryLineageWindowTestText =
    Get-Content -LiteralPath $hostRecoveryLineageWindowTestPath -Raw
foreach ($testName in @(
    'IdsRevisionsTicksAndArgumentsAreValidated'
    'WindowRejectsStaleRevisionAndRegressedTick'
    'WindowRejectsInvalidBoundsAndOversizedCount'
    'RootOnlyWindowPreservesBoundaryLineage'
    'FullWindowPreservesExactChainLineage'
    'SingleCheckpointRangeResolvesInclusiveBounds'
    'FullCheckpointRangeResolvesExactWindow'
    'RangeRejectsStaleRevisionRegressedTickAndInvalidBoundaries'
    'ContractsRemainImmutableBoundedAndSideEffectFree'
)) {
    if ($hostRecoveryLineageWindowTestText -notmatch $testName) {
        throw 'Host recovery lineage-window test is missing: ' + $testName
    }
}

foreach ($documentationToken in @(
    'bounded contiguous recovery lineage windows'
    'inclusive checkpoint ranges'
    'without discovery'
    'history mutation'
)) {
    if ($hostRecoveryLineageWindowText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host recovery lineage-window boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryRangeSummaryDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryRangeSummaryFiles = @(
    'HostRuntimeRecoveryCheckpointRangeSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryAdjacentWindowSelectionIdKind.cs'
    'HostRuntimeRecoveryCheckpointRangeSummaryStatus.cs'
    'HostRuntimeRecoveryCheckpointRangeSummaryProjection.cs'
    'HostRuntimeRecoveryAdjacentWindowSelection.cs'
    'HostRuntimeRecoveryCheckpointRangeSummaryProjectionResult.cs'
    'HostRuntimeRecoveryAdjacentWindowSelectionResult.cs'
    'HostRuntimeRecoveryCheckpointRangeSummaryFlow.cs'
)
foreach ($fileName in $hostRecoveryRangeSummaryFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoveryRangeSummaryDirectory $fileName))) {
        throw "Host recovery range-summary contract is missing: $fileName"
    }
}

$hostRecoveryRangeSummaryText = (
    $hostRecoveryRangeSummaryFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryRangeSummaryDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryRangeSummaryText =
    $hostRecoveryRangeSummaryText -replace '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryCheckpointRangeSummaryProjectionIdKind'
    'HostRuntimeRecoveryAdjacentWindowSelectionIdKind'
    'HostRuntimeRecoveryCheckpointRangeSummaryStatus'
    'HostRuntimeRecoveryCheckpointRangeSummaryProjection'
    'HostRuntimeRecoveryAdjacentWindowSelection'
    'HostRuntimeRecoveryCheckpointRangeSummaryProjectionResult'
    'HostRuntimeRecoveryAdjacentWindowSelectionResult'
    'HostRuntimeRecoveryCheckpointRangeSummaryFlow'
)) {
    if ($hostRecoveryRangeSummaryText -notmatch "\b$requiredType\b") {
        throw "Host recovery range-summary contract '$requiredType' is missing."
    }
}

foreach ($statusName in @(
    'CheckpointRangeSummaryProjected'
    'PreviousAdjacentWindowSelected'
    'NextAdjacentWindowSelected'
    'StaleRangeRevision'
    'RangeSummaryProjectionTickRegressed'
    'StaleSummaryRevision'
    'AdjacentWindowSelectionTickRegressed'
    'TooManyAdjacentWindowCheckpoints'
    'NoPreviousAdjacentWindow'
    'NoNextAdjacentWindow'
    'PreviousAdjacentWindowTooShort'
    'NextAdjacentWindowTooShort'
)) {
    if ($hostRecoveryRangeSummaryText -notmatch (
        "(?m)^\s*$statusName\s*=\s*\d+\s*,?\s*$")) {
        throw "Host recovery range-summary status '$statusName' is missing."
    }
}

foreach ($requiredToken in @(
    'ProjectSummary<TRequest, TState, TCompletion>'
    'SelectPreviousWindow<TRequest, TState, TCompletion>'
    'SelectNextWindow<TRequest, TState, TCompletion>'
    'MaximumAdjacentWindowCheckpointCount'
    'StartWindowIndex'
    'EndWindowIndex'
    'StartChainIndex'
    'EndChainIndex'
    'CheckpointCount'
    'IncomingSupersession'
    'OutgoingSupersession'
    'SelectsPreviousWindow'
    'SelectsNextWindow'
)) {
    if ($normalizedHostRecoveryRangeSummaryText -notmatch (
        [regex]::Escape($requiredToken))) {
        throw (
            'Host recovery range-summary implementation token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryRangeSummaryText -match '\bpublic\s+set\s*;') {
    throw 'Host recovery range-summary contracts expose a public setter.'
}
if ($hostRecoveryRangeSummaryText -match (
    '\b(Task\.Run|Task\.Delay|Timer|Thread|HttpClient|Socket|NamedPipe|' +
    'Process\.Start|DateTime(?:Offset)?\.(?:Now|UtcNow)|Stopwatch|' +
    'System\.IO\.File|System\.IO\.Directory|FileStream)\b')) {
    throw (
        'Host recovery range-summary contracts contain storage, archival, ' +
        'execution, scheduling, transport, process, or hidden-clock coupling.')
}

$hostRecoveryRangeSummaryTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryCheckpointRangeSummaryAdjacentWindowTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryRangeSummaryTestPath)) {
    throw 'Host recovery range-summary and adjacent-window tests are missing.'
}
$hostRecoveryRangeSummaryTestText =
    Get-Content -LiteralPath $hostRecoveryRangeSummaryTestPath -Raw
foreach ($testName in @(
    'IdsRevisionsTicksCountsAndArgumentsAreValidated'
    'RangeSummaryRejectsStaleRevisionAndRegressedTick'
    'RangeSummaryPreservesExactCompactEvidence'
    'AdjacentSelectionRejectsStaleRevisionRegressedTickAndOversizedCount'
    'PreviousSelectionRejectsMissingAndInsufficientLineage'
    'PreviousSelectionPreservesExactAdjacentBoundary'
    'NextSelectionRejectsMissingAndInsufficientLineage'
    'NextSelectionPreservesExactAdjacentBoundary'
    'ContractsRemainImmutableBoundedAndSideEffectFree'
)) {
    if ($hostRecoveryRangeSummaryTestText -notmatch $testName) {
        throw 'Host recovery range-summary test is missing: ' + $testName
    }
}

foreach ($documentationToken in @(
    'compact recovery checkpoint-range summaries'
    'bounded adjacent windows'
    'without discovery'
    'pagination'
)) {
    if ($hostRecoveryRangeSummaryText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host recovery range-summary boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryAdjacentProjectionDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryAdjacentProjectionFiles = @(
    'HostRuntimeRecoveryAdjacentWindowProjectionIdKind.cs'
    'HostRuntimeRecoveryCheckpointRangeContinuityValidationIdKind.cs'
    'HostRuntimeRecoveryAdjacentWindowProjectionStatus.cs'
    'HostRuntimeRecoveryAdjacentWindowProjection.cs'
    'HostRuntimeRecoveryCheckpointRangeContinuityValidation.cs'
    'HostRuntimeRecoveryAdjacentWindowProjectionResult.cs'
    'HostRuntimeRecoveryCheckpointRangeContinuityValidationResult.cs'
    'HostRuntimeRecoveryAdjacentWindowFlow.cs'
)
foreach ($fileName in $hostRecoveryAdjacentProjectionFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoveryAdjacentProjectionDirectory $fileName))) {
        throw "Host recovery adjacent-window contract is missing: $fileName"
    }
}

$hostRecoveryAdjacentProjectionText = (
    $hostRecoveryAdjacentProjectionFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryAdjacentProjectionDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryAdjacentProjectionText =
    $hostRecoveryAdjacentProjectionText -replace '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryAdjacentWindowProjectionIdKind'
    'HostRuntimeRecoveryCheckpointRangeContinuityValidationIdKind'
    'HostRuntimeRecoveryAdjacentWindowProjectionStatus'
    'HostRuntimeRecoveryAdjacentWindowProjection'
    'HostRuntimeRecoveryCheckpointRangeContinuityValidation'
    'HostRuntimeRecoveryAdjacentWindowProjectionResult'
    'HostRuntimeRecoveryCheckpointRangeContinuityValidationResult'
    'HostRuntimeRecoveryAdjacentWindowFlow'
)) {
    if ($hostRecoveryAdjacentProjectionText -notmatch "\b$requiredType\b") {
        throw "Host recovery adjacent-window contract '$requiredType' is missing."
    }
}

foreach ($statusName in @(
    'AdjacentWindowProjected'
    'CheckpointRangeContinuityValidated'
    'StaleSelectionRevision'
    'AdjacentWindowProjectionTickRegressed'
    'SelectionCheckpointMismatch'
    'SelectionSupersessionMismatch'
    'StaleRangeSummaryRevision'
    'StaleAdjacentWindowRevision'
    'ContinuityValidationTickRegressed'
    'RangeSummaryMismatch'
    'CheckpointRangeNotAdjacent'
    'CheckpointBoundaryMismatch'
    'SupersessionBoundaryMismatch'
)) {
    if ($hostRecoveryAdjacentProjectionText -notmatch (
        "(?m)^\s*$statusName\s*=\s*\d+\s*,?\s*$")) {
        throw "Host recovery adjacent-window status '$statusName' is missing."
    }
}

foreach ($requiredToken in @(
    'ProjectWindow<TRequest, TState, TCompletion>'
    'ValidateContinuity<TRequest, TState, TCompletion>'
    'MaximumCheckpointCount'
    'Selection'
    'Summary'
    'SourceProjection'
    'Chain'
    'Checkpoints'
    'Supersessions'
    'IncomingSupersession'
    'OutgoingSupersession'
    'ConnectingSupersession'
    'PriorCheckpoint'
    'SuccessorCheckpoint'
    'ProjectedTick'
    'ValidatedTick'
    'Revision'
)) {
    if ($hostRecoveryAdjacentProjectionText -notmatch [regex]::Escape(
        $requiredToken)) {
        throw (
            'Host recovery adjacent-window token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryAdjacentProjectionText -notmatch
    'public\s+const\s+int\s+MaximumCheckpointCount') {
    throw 'Host recovery adjacent-window bound is missing.'
}
if ($normalizedHostRecoveryAdjacentProjectionText -notmatch
    'HostRuntimeRecoveryLineageWindowFlow.MaximumCheckpointCount') {
    throw 'Host recovery adjacent-window bound must reuse the lineage-window bound.'
}
if ($hostRecoveryAdjacentProjectionText -match '\bpublic\s+set\s*;') {
    throw 'Host recovery adjacent-window contracts must remain immutable.'
}
if ($hostRecoveryAdjacentProjectionText -match
    '\b(File\.|Directory\.|FileStream|SqlConnection|HttpClient)\b') {
    throw 'Host recovery adjacent-window contracts must not perform I/O.'
}
if ($hostRecoveryAdjacentProjectionText -match
    '\b(Task\.Delay|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw 'Host recovery adjacent-window contracts must not wait or read clocks.'
}
if ($hostRecoveryAdjacentProjectionText -match
    '\b(Guid\.(NewGuid|CreateVersion7))\b') {
    throw 'Host recovery adjacent-window contracts must not generate identities.'
}

$hostRecoveryAdjacentProjectionTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryAdjacentWindowProjectionContinuityTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryAdjacentProjectionTestPath)) {
    throw 'Host recovery adjacent-window projection test file is missing.'
}
$hostRecoveryAdjacentProjectionTestText =
    Get-Content -LiteralPath $hostRecoveryAdjacentProjectionTestPath -Raw
foreach ($testName in @(
    'IdsRevisionsTicksAndArgumentsAreValidated'
    'ProjectionRejectsStaleRevisionAndRegressedTick'
    'PreviousProjectionPreservesExactSelectedEvidence'
    'NextProjectionPreservesExactSelectedEvidence'
    'ContinuityRejectsStaleRevisionsAndRegressedTick'
    'ContinuityRejectsMismatchedRangeSummary'
    'PreviousContinuityPreservesExactConnectingSupersession'
    'NextContinuityPreservesExactConnectingSupersession'
    'ContractsRemainImmutableBoundedAndSideEffectFree'
)) {
    if ($hostRecoveryAdjacentProjectionTestText -notmatch $testName) {
        throw 'Host recovery adjacent-window test is missing: ' + $testName
    }
}

foreach ($documentationToken in @(
    'Projects exact selected recovery windows'
    'validates their checkpoint-range'
    'without discovery'
    'pagination'
)) {
    if ($hostRecoveryAdjacentProjectionText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host recovery adjacent-window boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryContinuousWindowPairDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryContinuousWindowPairFiles = @(
    'HostRuntimeRecoveryContinuousWindowPairSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryCrossWindowCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousWindowPairStatus.cs'
    'HostRuntimeRecoveryContinuousWindowPairSummaryProjection.cs'
    'HostRuntimeRecoveryCrossWindowCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousWindowPairSummaryProjectionResult.cs'
    'HostRuntimeRecoveryCrossWindowCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousWindowPairFlow.cs'
)
foreach ($fileName in $hostRecoveryContinuousWindowPairFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoveryContinuousWindowPairDirectory $fileName))) {
        throw "Host recovery continuous-window pair contract is missing: $fileName"
    }
}

$hostRecoveryContinuousWindowPairText = (
    $hostRecoveryContinuousWindowPairFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryContinuousWindowPairDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryContinuousWindowPairText =
    $hostRecoveryContinuousWindowPairText -replace '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryContinuousWindowPairSummaryProjectionIdKind'
    'HostRuntimeRecoveryCrossWindowCheckpointRangeQueryIdKind'
    'HostRuntimeRecoveryContinuousWindowPairStatus'
    'HostRuntimeRecoveryContinuousWindowPairSummaryProjection'
    'HostRuntimeRecoveryCrossWindowCheckpointRangeQuery'
    'HostRuntimeRecoveryContinuousWindowPairSummaryProjectionResult'
    'HostRuntimeRecoveryCrossWindowCheckpointRangeQueryResult'
    'HostRuntimeRecoveryContinuousWindowPairFlow'
)) {
    if ($hostRecoveryContinuousWindowPairText -notmatch "\b$requiredType\b") {
        throw "Host recovery continuous-window pair contract '$requiredType' is missing."
    }
}

foreach ($statusName in @(
    'ContinuousWindowPairSummaryProjected'
    'CrossWindowCheckpointRangeQueried'
    'StaleContinuityRevision'
    'PairSummaryProjectionTickRegressed'
    'ContinuitySourceMismatch'
    'WindowPairNotContinuous'
    'WindowPairSupersessionMismatch'
    'WindowPairCheckpointMismatch'
    'StalePairSummaryRevision'
    'CrossWindowRangeQueryTickRegressed'
    'RangeStartNotFound'
    'RangeEndNotFound'
    'RangeOrderInvalid'
    'RangeDoesNotCrossWindowBoundary'
    'RangeTooLarge'
    'RangeSupersessionMismatch'
)) {
    if ($hostRecoveryContinuousWindowPairText -notmatch (
        "(?m)^\s*$statusName\s*=\s*\d+\s*,?\s*$")) {
        throw "Host recovery continuous-window pair status '$statusName' is missing."
    }
}

foreach ($requiredToken in @(
    'ProjectPair<TRequest, TState, TCompletion>'
    'QueryRange<TRequest, TState, TCompletion>'
    'MaximumCheckpointCount'
    'Continuity'
    'RangeSummary'
    'AdjacentWindow'
    'SourceProjection'
    'Chain'
    'ConnectingSupersession'
    'ConnectingPriorCheckpoint'
    'ConnectingSuccessorCheckpoint'
    'StartCheckpointIndex'
    'EndCheckpointIndex'
    'Checkpoints'
    'Supersessions'
    'IncomingSupersession'
    'OutgoingSupersession'
    'ProjectedTick'
    'QueriedTick'
    'Revision'
)) {
    if ($hostRecoveryContinuousWindowPairText -notmatch [regex]::Escape(
        $requiredToken)) {
        throw (
            'Host recovery continuous-window pair token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryContinuousWindowPairText -notmatch
    'public\s+const\s+int\s+MaximumCheckpointCount') {
    throw 'Host recovery cross-window query bound is missing.'
}
if ($normalizedHostRecoveryContinuousWindowPairText -notmatch
    'HostRuntimeRecoveryLineageWindowFlow.MaximumCheckpointCount') {
    throw 'Host recovery cross-window query must reuse the lineage-window bound.'
}
if ($hostRecoveryContinuousWindowPairText -match '\bpublic\s+set\s*;') {
    throw 'Host recovery continuous-window pair contracts must remain immutable.'
}
if ($hostRecoveryContinuousWindowPairText -match
    '\b(File\.|Directory\.|FileStream|SqlConnection|HttpClient)\b') {
    throw 'Host recovery continuous-window pair contracts must not perform I/O.'
}
if ($hostRecoveryContinuousWindowPairText -match
    '\b(Task\.Delay|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw 'Host recovery continuous-window pair contracts must not wait or read clocks.'
}
if ($hostRecoveryContinuousWindowPairText -match
    '\b(Guid\.(NewGuid|CreateVersion7))\b') {
    throw 'Host recovery continuous-window pair contracts must not generate identities.'
}

$hostRecoveryContinuousWindowPairTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryContinuousWindowPairCrossWindowRangeTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryContinuousWindowPairTestPath)) {
    throw 'Host recovery continuous-window pair test file is missing.'
}
$hostRecoveryContinuousWindowPairTestText =
    Get-Content -LiteralPath $hostRecoveryContinuousWindowPairTestPath -Raw
foreach ($testName in @(
    'IdsRevisionsTicksAndArgumentsAreValidated'
    'PairProjectionRejectsStaleRevisionAndRegressedTick'
    'PreviousPairSummaryPreservesExactContinuousEvidence'
    'NextPairSummaryPreservesExactContinuousEvidence'
    'CrossWindowQueryRejectsStaleRevisionAndRegressedTick'
    'QueryRejectsMissingReversedAndSingleWindowRanges'
    'PreviousPairQueryPreservesExactCrossBoundaryEvidence'
    'NextPairQueryPreservesExactCrossBoundaryEvidence'
    'ContractsRemainImmutableBoundedAndSideEffectFree'
)) {
    if ($hostRecoveryContinuousWindowPairTestText -notmatch $testName) {
        throw 'Host recovery continuous-window pair test is missing: ' + $testName
    }
}

foreach ($documentationToken in @(
    'Projects compact summaries over exact continuous recovery-window pairs'
    'resolves bounded inclusive ranges crossing their shared boundary'
    'without'
    'pagination'
)) {
    if ($hostRecoveryContinuousWindowPairText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host recovery continuous-window pair boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryContinuousWindowSequenceDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryContinuousWindowSequenceFiles = @(
    'HostRuntimeRecoveryContinuousWindowSequenceValidationIdKind.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousWindowSequenceStatus.cs'
    'HostRuntimeRecoveryContinuousWindowSequenceValidation.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousWindowSequenceValidationResult.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousWindowSequenceFlow.cs'
)
foreach ($fileName in $hostRecoveryContinuousWindowSequenceFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoveryContinuousWindowSequenceDirectory $fileName))) {
        throw "Host recovery continuous-window sequence contract is missing: $fileName"
    }
}

$hostRecoveryContinuousWindowSequenceText = (
    $hostRecoveryContinuousWindowSequenceFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryContinuousWindowSequenceDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryContinuousWindowSequenceText =
    $hostRecoveryContinuousWindowSequenceText -replace '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryContinuousWindowSequenceValidationIdKind'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeQueryIdKind'
    'HostRuntimeRecoveryContinuousWindowSequenceStatus'
    'HostRuntimeRecoveryContinuousWindowSequenceValidation'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeQuery'
    'HostRuntimeRecoveryContinuousWindowSequenceValidationResult'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeQueryResult'
    'HostRuntimeRecoveryContinuousWindowSequenceFlow'
)) {
    if ($hostRecoveryContinuousWindowSequenceText -notmatch $requiredType) {
        throw 'Host recovery continuous-window sequence type is missing: ' + $requiredType
    }
}

foreach ($requiredToken in @(
    'MaximumPairCount = 8'
    'MaximumCheckpointCount ='
    'ValidateSequence<TRequest, TState, TCompletion>'
    'QueryRange<TRequest, TState, TCompletion>'
    'PairCollectionEmpty'
    'PairCollectionTooLarge'
    'PairRevisionCountMismatch'
    'StalePairSummaryRevision'
    'SequenceValidationTickRegressed'
    'DuplicatePairSummaryId'
    'PairSourceMismatch'
    'PairSupersessionMismatch'
    'SequenceNotContinuous'
    'SequenceBoundarySupersessionMismatch'
    'StaleSequenceRevision'
    'MultiWindowRangeQueryTickRegressed'
    'RangeStartNotFound'
    'RangeEndNotFound'
    'RangeOrderInvalid'
    'RangeDoesNotCrossWindowBoundary'
    'RangeTooLarge'
    'RangeSupersessionMismatch'
    'ContinuousWindowSequenceValidated'
    'MultiWindowCheckpointRangeQueried'
    'Array.AsReadOnly(pairSummaries)'
    'Array.AsReadOnly(boundarySupersessions)'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'Array.AsReadOnly(crossedBoundarySupersessions)'
)) {
    if ($normalizedHostRecoveryContinuousWindowSequenceText -notmatch
        [regex]::Escape($requiredToken)) {
        throw 'Host recovery continuous-window sequence token is missing: ' + $requiredToken
    }
}

if ($hostRecoveryContinuousWindowSequenceText -match
    '\b(Task\.Delay|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw 'Host recovery continuous-window sequence contracts must not wait or read clocks.'
}
if ($hostRecoveryContinuousWindowSequenceText -match
    '\b(Guid\.(NewGuid|CreateVersion7))\b') {
    throw 'Host recovery continuous-window sequence contracts must not generate identities.'
}

$hostRecoveryContinuousWindowSequenceTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryContinuousWindowSequenceMultiWindowRangeTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryContinuousWindowSequenceTestPath)) {
    throw 'Host recovery continuous-window sequence test file is missing.'
}
$hostRecoveryContinuousWindowSequenceTestText =
    Get-Content -LiteralPath $hostRecoveryContinuousWindowSequenceTestPath -Raw
foreach ($testName in @(
    'IdsRevisionsTicksAndArgumentsAreValidated'
    'EmptyOversizedAndRevisionCountSequencesAreRejected'
    'SequenceValidationRejectsStaleRevisionAndRegressedTick'
    'SequenceValidationRejectsDuplicateSourceAndContinuityFailures'
    'SinglePairSequencePreservesExactBoundedAuthority'
    'MultiWindowQueryRejectsStaleRegressedMissingAndInvalidRanges'
    'MultiWindowQueryPreservesExactCrossBoundaryEvidence'
    'SequenceAndQueryCollectionsRemainImmutable'
    'ContractsRemainBoundedAndSideEffectFree'
)) {
    if ($hostRecoveryContinuousWindowSequenceTestText -notmatch $testName) {
        throw 'Host recovery continuous-window sequence test is missing: ' + $testName
    }
}

foreach ($documentationToken in @(
    'Validates bounded ordered sequences of exact continuous-window pairs'
    'resolves bounded inclusive ranges across their validated window boundaries'
    'without'
    'pagination'
)) {
    if ($hostRecoveryContinuousWindowSequenceText -notmatch [regex]::Escape(
        $documentationToken)) {
        throw (
            'Host recovery continuous-window sequence boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryMultiWindowRangeSummaryDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryMultiWindowRangeSummaryFiles = @(
    'HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryAdjacentSequenceSelectionIdKind.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection.cs'
    'HostRuntimeRecoveryAdjacentSequenceSelection.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjectionResult.cs'
    'HostRuntimeRecoveryAdjacentSequenceSelectionResult.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryFlow.cs'
)
foreach ($fileName in $hostRecoveryMultiWindowRangeSummaryFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoveryMultiWindowRangeSummaryDirectory $fileName))) {
        throw "Host recovery multi-window range-summary contract is missing: $fileName"
    }
}

$hostRecoveryMultiWindowRangeSummaryText = (
    $hostRecoveryMultiWindowRangeSummaryFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryMultiWindowRangeSummaryDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryMultiWindowRangeSummaryText =
    (($hostRecoveryMultiWindowRangeSummaryText -replace
        '(?m)^\s*///\s*', ' ') -replace '\s+', ' ') -replace
        '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjectionIdKind'
    'HostRuntimeRecoveryAdjacentSequenceSelectionIdKind'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection'
    'HostRuntimeRecoveryAdjacentSequenceSelection'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjectionResult'
    'HostRuntimeRecoveryAdjacentSequenceSelectionResult'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryFlow'
)) {
    if ($hostRecoveryMultiWindowRangeSummaryText -notmatch $requiredType) {
        throw 'Host recovery multi-window range-summary type is missing: ' + $requiredType
    }
}

foreach ($requiredToken in @(
    'MaximumAdjacentSequencePairCount ='
    'ProjectSummary<TRequest, TState, TCompletion>'
    'SelectPreviousSequence<TRequest, TState, TCompletion>'
    'SelectNextSequence<TRequest, TState, TCompletion>'
    'MultiWindowCheckpointRangeSummaryProjected'
    'PreviousAdjacentSequenceSelected'
    'NextAdjacentSequenceSelected'
    'StaleRangeRevision'
    'RangeSummaryProjectionTickRegressed'
    'StaleSummaryRevision'
    'AdjacentSequenceSelectionTickRegressed'
    'TooManyAdjacentSequencePairs'
    'NoPreviousAdjacentSequence'
    'NoNextAdjacentSequence'
    'PreviousAdjacentSequenceTooShort'
    'NextAdjacentSequenceTooShort'
    'AdjacentSequenceBoundaryMismatch'
    'Array.AsReadOnly(pairSummaries)'
    'Array.AsReadOnly(boundarySupersessions)'
    'CrossedBoundarySupersessions'
    'AdjacentBoundarySupersession'
)) {
    if ($normalizedHostRecoveryMultiWindowRangeSummaryText -notmatch
        [regex]::Escape($requiredToken)) {
        throw 'Host recovery multi-window range-summary token is missing: ' + $requiredToken
    }
}

if ($hostRecoveryMultiWindowRangeSummaryText -match
    '\b(Task\.Delay|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw 'Host recovery multi-window range-summary contracts must not wait or read clocks.'
}
if ($hostRecoveryMultiWindowRangeSummaryText -match
    '\b(Guid\.(NewGuid|CreateVersion7))\b') {
    throw 'Host recovery multi-window range-summary contracts must not generate identities.'
}

$hostRecoveryMultiWindowRangeSummaryTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryMultiWindowRangeSummaryAdjacentSequenceTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryMultiWindowRangeSummaryTestPath)) {
    throw 'Host recovery multi-window range-summary test file is missing.'
}
$hostRecoveryMultiWindowRangeSummaryTestText =
    Get-Content -LiteralPath $hostRecoveryMultiWindowRangeSummaryTestPath -Raw
foreach ($testName in @(
    'IdsRevisionsTicksCountsAndArgumentsAreValidated'
    'MultiWindowSummaryRejectsStaleRevisionAndRegressedTick'
    'MultiWindowSummaryPreservesExactCompactEvidence'
    'AdjacentSelectionRejectsStaleRevisionRegressedTickAndOversizedCount'
    'PreviousSelectionRejectsMissingAdjacentSequence'
    'NextSelectionRejectsMissingAdjacentSequence'
    'FailedSelectionsPreserveSourceAuthorityWithoutMaterialization'
    'SummaryAndSelectionContractsExposeNoPublicSetters'
    'ContractsRemainBoundedAndSideEffectFree'
)) {
    if ($hostRecoveryMultiWindowRangeSummaryTestText -notmatch $testName) {
        throw 'Host recovery multi-window range-summary test is missing: ' + $testName
    }
}

foreach ($documentationToken in @(
    'Projects compact multi-window checkpoint-range summaries'
    'selects exact bounded adjacent pair-summary sequences'
    'without discovery'
    'pagination'
)) {
    if ($normalizedHostRecoveryMultiWindowRangeSummaryText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery multi-window range-summary boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryAdjacentSequenceDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryAdjacentSequenceFiles = @(
    'HostRuntimeRecoveryAdjacentSequenceProjectionIdKind.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidationIdKind.cs'
    'HostRuntimeRecoveryAdjacentSequenceProjectionStatus.cs'
    'HostRuntimeRecoveryAdjacentSequenceProjection.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation.cs'
    'HostRuntimeRecoveryAdjacentSequenceProjectionResult.cs'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidationResult.cs'
    'HostRuntimeRecoveryAdjacentSequenceFlow.cs'
)
foreach ($fileName in $hostRecoveryAdjacentSequenceFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoveryAdjacentSequenceDirectory $fileName))) {
        throw "Host recovery adjacent-sequence contract is missing: $fileName"
    }
}

$hostRecoveryAdjacentSequenceText = (
    $hostRecoveryAdjacentSequenceFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryAdjacentSequenceDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryAdjacentSequenceText =
    (($hostRecoveryAdjacentSequenceText -replace
        '(?m)^\s*///\s*', ' ') -replace '\s+', ' ') -replace
        '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryAdjacentSequenceProjectionIdKind'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidationIdKind'
    'HostRuntimeRecoveryAdjacentSequenceProjectionStatus'
    'HostRuntimeRecoveryAdjacentSequenceProjection'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation'
    'HostRuntimeRecoveryAdjacentSequenceProjectionResult'
    'HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidationResult'
    'HostRuntimeRecoveryAdjacentSequenceFlow'
)) {
    if ($hostRecoveryAdjacentSequenceText -notmatch $requiredType) {
        throw 'Host recovery adjacent-sequence type is missing: ' + $requiredType
    }
}

foreach ($requiredToken in @(
    'ProjectSequence<TRequest, TState, TCompletion>'
    'ValidateContinuity<TRequest, TState, TCompletion>'
    'AdjacentSequenceProjected'
    'MultiWindowCheckpointRangeContinuityValidated'
    'StaleSelectionRevision'
    'AdjacentSequenceProjectionTickRegressed'
    'SelectionPairSummaryMismatch'
    'SelectionBoundarySupersessionMismatch'
    'SelectionCheckpointMismatch'
    'SelectionSupersessionMismatch'
    'StaleRangeSummaryRevision'
    'StaleAdjacentSequenceRevision'
    'ContinuityValidationTickRegressed'
    'RangeSummaryMismatch'
    'PairRangeNotAdjacent'
    'CheckpointRangeNotAdjacent'
    'SupersessionBoundaryMismatch'
    'CheckpointBoundaryMismatch'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'AdjacentBoundarySupersession'
    'BoundarySupersessions'
)) {
    if ($normalizedHostRecoveryAdjacentSequenceText -notmatch
        [regex]::Escape($requiredToken)) {
        throw 'Host recovery adjacent-sequence token is missing: ' + $requiredToken
    }
}

if ($hostRecoveryAdjacentSequenceText -match
    '\b(Task\.Delay|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw 'Host recovery adjacent-sequence contracts must not wait or read clocks.'
}
if ($hostRecoveryAdjacentSequenceText -match
    '\b(Guid\.(NewGuid|CreateVersion7))\b') {
    throw 'Host recovery adjacent-sequence contracts must not generate identities.'
}

$hostRecoveryAdjacentSequenceTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryAdjacentSequenceProjectionContinuityTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryAdjacentSequenceTestPath)) {
    throw 'Host recovery adjacent-sequence test file is missing.'
}
$hostRecoveryAdjacentSequenceTestText =
    Get-Content -LiteralPath $hostRecoveryAdjacentSequenceTestPath -Raw
foreach ($testName in @(
    'IdsRevisionsTicksAndArgumentsAreValidated'
    'ProjectionRejectsStaleRevisionAndRegressedTick'
    'ProjectionRejectsPairBoundaryAndSupersessionEvidenceMismatch'
    'ProjectionMaterializesExactSelectedSequenceEvidence'
    'ContinuityRejectsStaleRevisionsAndRegressedTick'
    'ContinuityRejectsSourcePairCheckpointAndBoundaryMismatch'
    'ContinuityValidatesExactPreviousSequenceBoundary'
    'ProjectionAndValidationCollectionsRemainImmutable'
    'ContractsRemainBoundedAndSideEffectFree'
)) {
    if ($hostRecoveryAdjacentSequenceTestText -notmatch $testName) {
        throw 'Host recovery adjacent-sequence test is missing: ' + $testName
    }
}

foreach ($documentationToken in @(
    'Projects exact selected adjacent sequences'
    'immediate continuity with summarized multi-window checkpoint ranges'
    'without discovery'
    'pagination'
)) {
    if ($normalizedHostRecoveryAdjacentSequenceText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery adjacent-sequence boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryContinuousMultiSequenceDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryContinuousMultiSequenceFiles = @(
    'HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryCrossSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiSequenceStatus.cs'
    'HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection.cs'
    'HostRuntimeRecoveryCrossSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionResult.cs'
    'HostRuntimeRecoveryCrossSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiSequenceFlow.cs'
)
foreach ($fileName in $hostRecoveryContinuousMultiSequenceFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoveryContinuousMultiSequenceDirectory $fileName))) {
        throw "Host recovery continuous multi-sequence contract is missing: $fileName"
    }
}

$hostRecoveryContinuousMultiSequenceText = (
    $hostRecoveryContinuousMultiSequenceFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryContinuousMultiSequenceDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryContinuousMultiSequenceText =
    (($hostRecoveryContinuousMultiSequenceText -replace
        '(?m)^\s*///\s*', ' ') -replace '\s+', ' ') -replace
        '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionIdKind'
    'HostRuntimeRecoveryCrossSequenceCheckpointRangeQueryIdKind'
    'HostRuntimeRecoveryContinuousMultiSequenceStatus'
    'HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection'
    'HostRuntimeRecoveryCrossSequenceCheckpointRangeQuery'
    'HostRuntimeRecoveryContinuousMultiSequenceSummaryProjectionResult'
    'HostRuntimeRecoveryCrossSequenceCheckpointRangeQueryResult'
    'HostRuntimeRecoveryContinuousMultiSequenceFlow'
)) {
    if ($hostRecoveryContinuousMultiSequenceText -notmatch $requiredType) {
        throw 'Host recovery continuous multi-sequence type is missing: ' + $requiredType
    }
}

foreach ($requiredToken in @(
    'MaximumCheckpointCount'
    'ProjectSummary<TRequest, TState, TCompletion>'
    'QueryRange<TRequest, TState, TCompletion>'
    'ContinuousMultiSequenceSummaryProjected'
    'CrossSequenceCheckpointRangeQueried'
    'StaleContinuityRevision'
    'MultiSequenceSummaryProjectionTickRegressed'
    'ContinuitySourceMismatch'
    'SequencePairNotContinuous'
    'SequenceCheckpointRangeNotContinuous'
    'SequencePairSupersessionMismatch'
    'SequencePairCheckpointMismatch'
    'StaleMultiSequenceSummaryRevision'
    'CrossSequenceRangeQueryTickRegressed'
    'RangeStartNotFound'
    'RangeEndNotFound'
    'RangeOrderInvalid'
    'RangeDoesNotCrossSequenceBoundary'
    'RangeTooLarge'
    'RangeSupersessionMismatch'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'ConnectingSupersession'
    'ConnectingPriorPairIndex'
    'ConnectingSuccessorPairIndex'
    'CrossesSequenceBoundary'
)) {
    if ($normalizedHostRecoveryContinuousMultiSequenceText -notmatch
        [regex]::Escape($requiredToken)) {
        throw 'Host recovery continuous multi-sequence token is missing: ' + $requiredToken
    }
}

if ($hostRecoveryContinuousMultiSequenceText -match
    '\b(Task\.Delay|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw 'Host recovery continuous multi-sequence contracts must not wait or read clocks.'
}
if ($hostRecoveryContinuousMultiSequenceText -match
    '\b(Guid\.(NewGuid|CreateVersion7))\b') {
    throw 'Host recovery continuous multi-sequence contracts must not generate identities.'
}

$hostRecoveryContinuousMultiSequenceTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryContinuousMultiSequenceCrossSequenceRangeTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryContinuousMultiSequenceTestPath)) {
    throw 'Host recovery continuous multi-sequence test file is missing.'
}
$hostRecoveryContinuousMultiSequenceTestText =
    Get-Content -LiteralPath $hostRecoveryContinuousMultiSequenceTestPath -Raw
foreach ($testName in @(
    'IdsRevisionsTicksAndArgumentsAreValidated'
    'SummaryProjectionRejectsStaleRevisionAndRegressedTick'
    'SummaryProjectionRejectsSourcePairAndSupersessionMismatch'
    'SummaryProjectionPreservesExactContinuousEvidence'
    'CrossSequenceQueryRejectsStaleRevisionAndRegressedTick'
    'QueryRejectsMissingReversedAndSingleSequenceRanges'
    'QueryPreservesExactCrossSequenceEvidence'
    'QueryCollectionsRemainImmutable'
    'ContractsRemainBoundedAndSideEffectFree'
)) {
    if ($hostRecoveryContinuousMultiSequenceTestText -notmatch $testName) {
        throw 'Host recovery continuous multi-sequence test is missing: ' + $testName
    }
}

foreach ($documentationToken in @(
    'Projects compact summaries over one exact continuous recovery multi-sequence'
    'bounded inclusive ranges crossing their shared boundary'
    'without discovery'
    'pagination'
)) {
    if ($normalizedHostRecoveryContinuousMultiSequenceText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery continuous multi-sequence boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryContinuousMultiSequenceCollectionDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryContinuousMultiSequenceCollectionFiles = @(
    'HostRuntimeRecoveryContinuousMultiSequenceCollectionValidationIdKind.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus.cs'
    'HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiSequenceCollectionValidationResult.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiSequenceCollectionFlow.cs'
)
foreach ($fileName in $hostRecoveryContinuousMultiSequenceCollectionFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoveryContinuousMultiSequenceCollectionDirectory $fileName))) {
        throw (
            'Host recovery continuous multi-sequence collection contract is missing: ' +
            $fileName)
    }
}

$hostRecoveryContinuousMultiSequenceCollectionText = (
    $hostRecoveryContinuousMultiSequenceCollectionFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryContinuousMultiSequenceCollectionDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryContinuousMultiSequenceCollectionText =
    (($hostRecoveryContinuousMultiSequenceCollectionText -replace
        '(?m)^\s*///\s*', ' ') -replace '\s+', ' ') -replace
        '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryContinuousMultiSequenceCollectionValidationIdKind'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeQueryIdKind'
    'HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus'
    'HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeQuery'
    'HostRuntimeRecoveryContinuousMultiSequenceCollectionValidationResult'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeQueryResult'
    'HostRuntimeRecoveryContinuousMultiSequenceCollectionFlow'
)) {
    if ($hostRecoveryContinuousMultiSequenceCollectionText -notmatch
        $requiredType) {
        throw (
            'Host recovery continuous multi-sequence collection type is missing: ' +
            $requiredType)
    }
}

foreach ($requiredToken in @(
    'MaximumSummaryCount = 8'
    'MaximumCheckpointCount ='
    'ValidateCollection<TRequest, TState, TCompletion>'
    'QueryRange<TRequest, TState, TCompletion>'
    'SummaryCollectionEmpty'
    'SummaryCollectionTooLarge'
    'SummaryRevisionCountMismatch'
    'StaleMultiSequenceSummaryRevision'
    'CollectionValidationTickRegressed'
    'DuplicateMultiSequenceSummaryId'
    'SummarySourceMismatch'
    'SummarySupersessionMismatch'
    'SummaryPairRangeNotContinuous'
    'SummaryCheckpointRangeNotContinuous'
    'CollectionBoundarySupersessionMismatch'
    'StaleCollectionRevision'
    'MultiSequenceRangeQueryTickRegressed'
    'RangeStartNotFound'
    'RangeEndNotFound'
    'RangeOrderInvalid'
    'RangeDoesNotCrossSequenceBoundary'
    'RangeTooLarge'
    'RangeSupersessionMismatch'
    'ContinuousMultiSequenceCollectionValidated'
    'MultiSequenceCheckpointRangeQueried'
    'Array.AsReadOnly(multiSequenceSummaries)'
    'Array.AsReadOnly(boundarySupersessions)'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'Array.AsReadOnly(crossedBoundarySupersessions)'
)) {
    if ($normalizedHostRecoveryContinuousMultiSequenceCollectionText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery continuous multi-sequence collection token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryContinuousMultiSequenceCollectionText -match
    '\b(Task\.Delay|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery continuous multi-sequence collection contracts must not ' +
        'wait or read clocks.')
}
if ($hostRecoveryContinuousMultiSequenceCollectionText -match
    '\b(Guid\.(NewGuid|CreateVersion7))\b') {
    throw (
        'Host recovery continuous multi-sequence collection contracts must not ' +
        'generate identities.')
}

$hostRecoveryContinuousMultiSequenceCollectionTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryContinuousMultiSequenceCollectionMultiSequenceRangeTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryContinuousMultiSequenceCollectionTestPath)) {
    throw (
        'Host recovery continuous multi-sequence collection contract tests ' +
        'are missing.')
}

$hostRecoveryContinuousMultiSequenceCollectionTestText = Get-Content -LiteralPath (
    $hostRecoveryContinuousMultiSequenceCollectionTestPath) -Raw
$hostRecoveryContinuousMultiSequenceCollectionFactCount = (
    [regex]::Matches(
        $hostRecoveryContinuousMultiSequenceCollectionTestText,
        '\[Xunit\.Fact\]').Count)
if ($hostRecoveryContinuousMultiSequenceCollectionFactCount -ne 9) {
    throw (
        'Host recovery continuous multi-sequence collection tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryContinuousMultiSequenceCollectionTestText -match
    'Skip\s*=') {
    throw (
        'Host recovery continuous multi-sequence collection tests must not ' +
        'be skipped.')
}

foreach ($requiredTest in @(
    'IdsRevisionsTicksAndArgumentsAreValidated'
    'EmptyOversizedAndRevisionCountCollectionsAreRejected'
    'CollectionValidationRejectsStaleRevisionAndRegressedTick'
    'CollectionValidationRejectsDuplicateSourceAndContinuityFailures'
    'SingleSummaryCollectionPreservesExactBoundedAuthority'
    'MultiSequenceQueryRejectsStaleRevisionAndRegressedTick'
    'QueryRejectsMissingReversedAndSingleCheckpointRanges'
    'QueryPreservesExactMultiSequenceEvidence'
    'CollectionsRemainImmutableAndContractsStayBounded'
)) {
    if ($hostRecoveryContinuousMultiSequenceCollectionTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery continuous multi-sequence collection test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Validates bounded ordered collections of exact continuous multi-sequence summaries'
    'resolves bounded inclusive ranges across their validated sequence boundaries'
    'without discovery'
    'reordering'
    'storage'
    'indexing'
    'history mutation'
    'archival'
    'compaction'
    'pagination'
    'diagnostics'
    'scheduling'
    'supervision'
    'waiting'
    'restart'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryContinuousMultiSequenceCollectionText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery continuous multi-sequence collection boundary text ' +
            'is missing: ' + $documentationToken)
    }
}

$hostRecoveryMultiSequenceRangeSummaryDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryMultiSequenceRangeSummaryFiles = @(
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryAdjacentCollectionSelectionIdKind.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection.cs'
    'HostRuntimeRecoveryAdjacentCollectionSelection.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjectionResult.cs'
    'HostRuntimeRecoveryAdjacentCollectionSelectionResult.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryFlow.cs'
)
foreach ($fileName in $hostRecoveryMultiSequenceRangeSummaryFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoveryMultiSequenceRangeSummaryDirectory $fileName))) {
        throw (
            'Host recovery multi-sequence range-summary contract is missing: ' +
            $fileName)
    }
}

$hostRecoveryMultiSequenceRangeSummaryText = (
    $hostRecoveryMultiSequenceRangeSummaryFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryMultiSequenceRangeSummaryDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryMultiSequenceRangeSummaryText =
    (($hostRecoveryMultiSequenceRangeSummaryText -replace
        '(?m)^\s*///\s*', ' ') -replace '\s+', ' ') -replace
        '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjectionIdKind'
    'HostRuntimeRecoveryAdjacentCollectionSelectionIdKind'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection'
    'HostRuntimeRecoveryAdjacentCollectionSelection'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjectionResult'
    'HostRuntimeRecoveryAdjacentCollectionSelectionResult'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryFlow'
)) {
    if ($hostRecoveryMultiSequenceRangeSummaryText -notmatch $requiredType) {
        throw (
            'Host recovery multi-sequence range-summary type is missing: ' +
            $requiredType)
    }
}

foreach ($requiredToken in @(
    'MaximumAdjacentCollectionSummaryCount ='
    'ProjectSummary<TRequest, TState, TCompletion>'
    'SelectPreviousCollection<TRequest, TState, TCompletion>'
    'SelectNextCollection<TRequest, TState, TCompletion>'
    'MultiSequenceCheckpointRangeSummaryProjected'
    'PreviousAdjacentCollectionSelected'
    'NextAdjacentCollectionSelected'
    'StaleRangeRevision'
    'RangeSummaryProjectionTickRegressed'
    'StaleSummaryRevision'
    'AdjacentCollectionSelectionTickRegressed'
    'TooManyAdjacentCollectionSummaries'
    'NoPreviousAdjacentCollection'
    'NoNextAdjacentCollection'
    'PreviousAdjacentCollectionTooShort'
    'NextAdjacentCollectionTooShort'
    'AdjacentCollectionBoundaryMismatch'
    'Array.AsReadOnly(multiSequenceSummaries)'
    'Array.AsReadOnly(boundarySupersessions)'
)) {
    if ($normalizedHostRecoveryMultiSequenceRangeSummaryText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery multi-sequence range-summary token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryMultiSequenceRangeSummaryText -match
    '\b(Task\.Delay|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery multi-sequence range-summary contracts must not wait ' +
        'or read clocks.')
}
if ($hostRecoveryMultiSequenceRangeSummaryText -match
    '\b(Guid\.(NewGuid|CreateVersion7))\b') {
    throw (
        'Host recovery multi-sequence range-summary contracts must not ' +
        'generate identities.')
}

$hostRecoveryMultiSequenceRangeSummaryTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryMultiSequenceRangeSummaryAdjacentCollectionTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryMultiSequenceRangeSummaryTestPath)) {
    throw (
        'Host recovery multi-sequence range-summary contract tests are missing.')
}

$hostRecoveryMultiSequenceRangeSummaryTestText = Get-Content -LiteralPath (
    $hostRecoveryMultiSequenceRangeSummaryTestPath) -Raw
$hostRecoveryMultiSequenceRangeSummaryFactCount = (
    [regex]::Matches(
        $hostRecoveryMultiSequenceRangeSummaryTestText,
        '\[Xunit\.Fact\]').Count)
if ($hostRecoveryMultiSequenceRangeSummaryFactCount -ne 9) {
    throw (
        'Host recovery multi-sequence range-summary tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryMultiSequenceRangeSummaryTestText -match 'Skip\s*=') {
    throw (
        'Host recovery multi-sequence range-summary tests must not be skipped.')
}

foreach ($requiredTest in @(
    'IdsRevisionsTicksCountsAndArgumentsAreValidated'
    'MultiSequenceRangeSummaryRejectsStaleRevisionAndRegressedTick'
    'MultiSequenceRangeSummaryPreservesExactCompactEvidence'
    'AdjacentCollectionSelectionRejectsStaleRevisionRegressedTickAndOversizedCount'
    'PreviousSelectionRejectsMissingAdjacentCollection'
    'NextSelectionRejectsMissingAdjacentCollection'
    'FailedSelectionsPreserveSourceAuthorityWithoutMaterialization'
    'SummaryAndSelectionContractsExposeNoPublicSetters'
    'FlowExposesOnlySynchronousBoundedAdvisoryOperations'
)) {
    if ($hostRecoveryMultiSequenceRangeSummaryTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery multi-sequence range-summary test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Projects compact multi-sequence checkpoint-range summaries'
    'selects exact bounded adjacent summary collections'
    'without discovery'
    'reordering'
    'storage'
    'indexing'
    'history mutation'
    'archival'
    'compaction'
    'pagination'
    'diagnostics'
    'scheduling'
    'supervision'
    'waiting'
    'restart'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryMultiSequenceRangeSummaryText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery multi-sequence range-summary boundary text is ' +
            'missing: ' + $documentationToken)
    }
}


$hostRecoveryAdjacentCollectionProjectionDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryAdjacentCollectionProjectionFiles = @(
    'HostRuntimeRecoveryAdjacentCollectionProjectionIdKind.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidationIdKind.cs'
    'HostRuntimeRecoveryAdjacentCollectionProjectionStatus.cs'
    'HostRuntimeRecoveryAdjacentCollectionProjection.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidation.cs'
    'HostRuntimeRecoveryAdjacentCollectionProjectionResult.cs'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidationResult.cs'
    'HostRuntimeRecoveryAdjacentCollectionFlow.cs'
)
foreach ($fileName in $hostRecoveryAdjacentCollectionProjectionFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoveryAdjacentCollectionProjectionDirectory $fileName))) {
        throw (
            'Host recovery adjacent-collection projection contract is missing: ' +
            $fileName)
    }
}

$hostRecoveryAdjacentCollectionProjectionText = (
    $hostRecoveryAdjacentCollectionProjectionFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryAdjacentCollectionProjectionDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryAdjacentCollectionProjectionText =
    (($hostRecoveryAdjacentCollectionProjectionText -replace
        '(?m)^\s*///\s*', ' ') -replace '\s+', ' ') -replace
        '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryAdjacentCollectionProjectionIdKind'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidationIdKind'
    'HostRuntimeRecoveryAdjacentCollectionProjectionStatus'
    'HostRuntimeRecoveryAdjacentCollectionProjection'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidation'
    'HostRuntimeRecoveryAdjacentCollectionProjectionResult'
    'HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidationResult'
    'HostRuntimeRecoveryAdjacentCollectionFlow'
)) {
    if ($hostRecoveryAdjacentCollectionProjectionText -notmatch $requiredType) {
        throw (
            'Host recovery adjacent-collection projection type is missing: ' +
            $requiredType)
    }
}

foreach ($requiredToken in @(
    'ProjectCollection<TRequest, TState, TCompletion>'
    'ValidateContinuity<TRequest, TState, TCompletion>'
    'AdjacentCollectionProjected'
    'MultiSequenceCheckpointRangeContinuityValidated'
    'StaleSelectionRevision'
    'AdjacentCollectionProjectionTickRegressed'
    'SelectionSummaryMismatch'
    'SelectionBoundarySupersessionMismatch'
    'SelectionCheckpointMismatch'
    'SelectionSupersessionMismatch'
    'StaleRangeSummaryRevision'
    'StaleAdjacentCollectionRevision'
    'ContinuityValidationTickRegressed'
    'RangeSummaryMismatch'
    'SummaryRangeNotAdjacent'
    'CheckpointRangeNotAdjacent'
    'SupersessionBoundaryMismatch'
    'CheckpointBoundaryMismatch'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
)) {
    if ($normalizedHostRecoveryAdjacentCollectionProjectionText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery adjacent-collection projection token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryAdjacentCollectionProjectionText -match
    '\b(Task\.Delay|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery adjacent-collection projection contracts must not wait ' +
        'or read clocks.')
}
if ($hostRecoveryAdjacentCollectionProjectionText -match
    '\b(Guid\.(NewGuid|CreateVersion7))\b') {
    throw (
        'Host recovery adjacent-collection projection contracts must not ' +
        'generate identities.')
}

$hostRecoveryAdjacentCollectionProjectionTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryAdjacentCollectionProjectionContinuityTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryAdjacentCollectionProjectionTestPath)) {
    throw (
        'Host recovery adjacent-collection projection contract tests are missing.')
}

$hostRecoveryAdjacentCollectionProjectionTestText = Get-Content -LiteralPath (
    $hostRecoveryAdjacentCollectionProjectionTestPath) -Raw
$hostRecoveryAdjacentCollectionProjectionFactCount = (
    [regex]::Matches(
        $hostRecoveryAdjacentCollectionProjectionTestText,
        '\[Xunit\.Fact\]').Count)
if ($hostRecoveryAdjacentCollectionProjectionFactCount -ne 9) {
    throw (
        'Host recovery adjacent-collection projection tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryAdjacentCollectionProjectionTestText -match 'Skip\s*=') {
    throw (
        'Host recovery adjacent-collection projection tests must not be skipped.')
}

foreach ($requiredTest in @(
    'IdsRevisionsTicksAndArgumentsAreValidated'
    'ProjectionRejectsStaleRevisionAndRegressedTick'
    'ProjectionRejectsSummaryBoundaryAndSupersessionEvidenceMismatch'
    'ProjectionMaterializesExactSelectedCollectionEvidence'
    'ContinuityRejectsStaleRevisionsAndRegressedTick'
    'ContinuityRejectsSourceSummaryCheckpointAndBoundaryMismatch'
    'ContinuityValidatesExactPreviousCollectionBoundary'
    'ProjectionAndValidationCollectionsRemainImmutable'
    'ContractsRemainBoundedAndSideEffectFree'
)) {
    if ($hostRecoveryAdjacentCollectionProjectionTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery adjacent-collection projection test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Projects exact selected adjacent collections'
    'validates their immediate continuity with summarized multi-sequence checkpoint ranges'
    'without discovery'
    'reordering'
    'storage'
    'indexing'
    'history mutation'
    'archival'
    'compaction'
    'pagination'
    'diagnostics'
    'scheduling'
    'supervision'
    'waiting'
    'restart'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryAdjacentCollectionProjectionText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery adjacent-collection projection boundary text is ' +
            'missing: ' + $documentationToken)
    }
}


$hostRecoveryContinuousCollectionPairDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryContinuousCollectionPairFiles = @(
    'HostRuntimeRecoveryContinuousCollectionPairSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryCrossCollectionCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousCollectionPairStatus.cs'
    'HostRuntimeRecoveryContinuousCollectionPairSummaryProjection.cs'
    'HostRuntimeRecoveryCrossCollectionCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousCollectionPairSummaryProjectionResult.cs'
    'HostRuntimeRecoveryCrossCollectionCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousCollectionPairFlow.cs'
)
foreach ($fileName in $hostRecoveryContinuousCollectionPairFiles) {
    if (-not (Test-Path -LiteralPath (
        Join-Path $hostRecoveryContinuousCollectionPairDirectory $fileName))) {
        throw (
            'Host recovery continuous collection-pair contract is missing: ' +
            $fileName)
    }
}

$hostRecoveryContinuousCollectionPairText = (
    $hostRecoveryContinuousCollectionPairFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryContinuousCollectionPairDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryContinuousCollectionPairText =
    (($hostRecoveryContinuousCollectionPairText -replace
        '(?m)^\s*///\s*', ' ') -replace '\s+', ' ') -replace
        '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryContinuousCollectionPairSummaryProjectionIdKind'
    'HostRuntimeRecoveryCrossCollectionCheckpointRangeQueryIdKind'
    'HostRuntimeRecoveryContinuousCollectionPairStatus'
    'HostRuntimeRecoveryContinuousCollectionPairSummaryProjection'
    'HostRuntimeRecoveryCrossCollectionCheckpointRangeQuery'
    'HostRuntimeRecoveryContinuousCollectionPairSummaryProjectionResult'
    'HostRuntimeRecoveryCrossCollectionCheckpointRangeQueryResult'
    'HostRuntimeRecoveryContinuousCollectionPairFlow'
)) {
    if ($hostRecoveryContinuousCollectionPairText -notmatch $requiredType) {
        throw (
            'Host recovery continuous collection-pair type is missing: ' +
            $requiredType)
    }
}

foreach ($requiredToken in @(
    'MaximumCheckpointCount'
    'ProjectSummary<TRequest, TState, TCompletion>'
    'QueryRange<TRequest, TState, TCompletion>'
    'ContinuousCollectionPairSummaryProjected'
    'CrossCollectionCheckpointRangeQueried'
    'StaleContinuityRevision'
    'CollectionPairSummaryProjectionTickRegressed'
    'ContinuitySourceMismatch'
    'CollectionPairNotContinuous'
    'CollectionCheckpointRangeNotContinuous'
    'CollectionPairSupersessionMismatch'
    'CollectionPairCheckpointMismatch'
    'StaleCollectionPairSummaryRevision'
    'CrossCollectionRangeQueryTickRegressed'
    'RangeStartNotFound'
    'RangeEndNotFound'
    'RangeOrderInvalid'
    'RangeDoesNotCrossCollectionBoundary'
    'RangeTooLarge'
    'RangeSupersessionMismatch'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
)) {
    if ($normalizedHostRecoveryContinuousCollectionPairText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery continuous collection-pair token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryContinuousCollectionPairText -match
    '\b(Task\.Delay|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery continuous collection-pair contracts must not wait ' +
        'or read clocks.')
}
if ($hostRecoveryContinuousCollectionPairText -match
    '\b(Guid\.(NewGuid|CreateVersion7))\b') {
    throw (
        'Host recovery continuous collection-pair contracts must not ' +
        'generate identities.')
}

$hostRecoveryContinuousCollectionPairTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryContinuousCollectionPairCrossCollectionRangeTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryContinuousCollectionPairTestPath)) {
    throw (
        'Host recovery continuous collection-pair contract tests are missing.')
}

$hostRecoveryContinuousCollectionPairTestText = Get-Content -LiteralPath (
    $hostRecoveryContinuousCollectionPairTestPath) -Raw
$hostRecoveryContinuousCollectionPairFactCount = (
    [regex]::Matches(
        $hostRecoveryContinuousCollectionPairTestText,
        '\[Xunit\.Fact\]').Count)
if ($hostRecoveryContinuousCollectionPairFactCount -ne 9) {
    throw (
        'Host recovery continuous collection-pair tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryContinuousCollectionPairTestText -match 'Skip\s*=') {
    throw (
        'Host recovery continuous collection-pair tests must not be skipped.')
}

foreach ($requiredTest in @(
    'IdsRevisionsTicksAndArgumentsAreValidated'
    'SummaryProjectionRejectsStaleRevisionAndRegressedTick'
    'SummaryProjectionRejectsSourceSummaryAndSupersessionMismatch'
    'SummaryProjectionPreservesExactContinuousEvidence'
    'CrossCollectionQueryRejectsStaleRevisionAndRegressedTick'
    'QueryRejectsMissingReversedAndSingleCollectionRanges'
    'QueryPreservesExactCrossCollectionEvidence'
    'QueryCollectionsRemainImmutable'
    'ContractsRemainBoundedAndSideEffectFree'
)) {
    if ($hostRecoveryContinuousCollectionPairTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery continuous collection-pair test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Projects compact summaries over one exact continuous recovery collection-pair'
    'bounded inclusive ranges crossing their shared boundary'
    'without discovery'
    'reordering'
    'storage'
    'indexing'
    'history mutation'
    'archival'
    'compaction'
    'pagination'
    'diagnostics'
    'scheduling'
    'supervision'
    'waiting'
    'restart'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryContinuousCollectionPairText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery continuous collection-pair boundary text is ' +
            'missing: ' + $documentationToken)
    }
}

Push-Location $root
try {

    $sdkVersion = (& dotnet --version).Trim()
    if ($LASTEXITCODE -ne 0 -or $sdkVersion -notmatch '^10\.0\.') {
        throw "The repository must resolve to .NET 10, but resolved SDK is '$sdkVersion'."
    }

    $solutionOutput = @(
        & dotnet sln '.\AI.Sandbox.Engine.slnx' list 2>&1
    )
    if ($LASTEXITCODE -ne 0) {
        throw 'The SLNX solution could not be read by the .NET SDK.'
    }

    $solutionText = ($solutionOutput | Out-String)
    $coreSolutionPath =
        'src\AI.Sandbox.Engine.Core\AI.Sandbox.Engine.Core.csproj'
    $testSolutionPath =
        'tests\AI.Sandbox.Engine.Core.Tests\AI.Sandbox.Engine.Core.Tests.csproj'

    if ($solutionText -notmatch [regex]::Escape($coreSolutionPath)) {
        throw 'The core project is not present in the solution.'
    }
    if ($solutionText -notmatch [regex]::Escape($testSolutionPath)) {
        throw 'The test project is not present in the solution.'
    }

    $probeSolutionPath =
        'samples\AI.Sandbox.Engine.FoundationProbe\AI.Sandbox.Engine.FoundationProbe.csproj'
    if ($solutionText -notmatch [regex]::Escape($probeSolutionPath)) {
        throw 'The Foundation Probe project is not present in the solution.'
    }

    & git diff --check
    if ($LASTEXITCODE -ne 0) {
        throw 'git diff --check found whitespace errors.'
    }
}
finally {
    Pop-Location
}

$hostRecoveryContinuousCollectionSequenceDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryContinuousCollectionSequenceFiles = @(
    'HostRuntimeRecoveryContinuousCollectionSequenceValidationIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousCollectionSequenceStatus.cs'
    'HostRuntimeRecoveryContinuousCollectionSequenceValidation.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousCollectionSequenceValidationResult.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousCollectionSequenceFlow.cs'
)
foreach ($fileName in $hostRecoveryContinuousCollectionSequenceFiles) {
    $contractPath = Join-Path (
        $hostRecoveryContinuousCollectionSequenceDirectory) $fileName
    if (-not (Test-Path -LiteralPath $contractPath)) {
        throw (
            'Host recovery continuous collection-sequence contract is missing: ' +
            $fileName)
    }
}

$hostRecoveryContinuousCollectionSequenceText = (
    $hostRecoveryContinuousCollectionSequenceFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryContinuousCollectionSequenceDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryContinuousCollectionSequenceText =
    (($hostRecoveryContinuousCollectionSequenceText -replace
        '(?m)^\s*///\s*', ' ') -replace '\s+', ' ') -replace
        '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryContinuousCollectionSequenceValidationIdKind'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeQueryIdKind'
    'HostRuntimeRecoveryContinuousCollectionSequenceStatus'
    'HostRuntimeRecoveryContinuousCollectionSequenceValidation'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeQuery'
    'HostRuntimeRecoveryContinuousCollectionSequenceValidationResult'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeQueryResult'
    'HostRuntimeRecoveryContinuousCollectionSequenceFlow'
)) {
    if ($hostRecoveryContinuousCollectionSequenceText -notmatch $requiredType) {
        throw (
            'Host recovery continuous collection-sequence type is missing: ' +
            $requiredType)
    }
}

foreach ($requiredToken in @(
    'MaximumCollectionPairCount = 8'
    'MaximumCheckpointCount'
    'ValidateSequence'
    'QueryRange'
    'ContinuousCollectionSequenceValidated'
    'MultiCollectionCheckpointRangeQueried'
    'CollectionPairSequenceEmpty'
    'CollectionPairSequenceTooLarge'
    'CollectionPairRevisionCountMismatch'
    'StaleCollectionPairSummaryRevision'
    'CollectionSequenceValidationTickRegressed'
    'DuplicateCollectionPairSummaryId'
    'CollectionPairSourceMismatch'
    'CollectionPairSupersessionMismatch'
    'CollectionSummaryRangeNotContinuous'
    'CollectionCheckpointRangeNotContinuous'
    'CollectionSequenceBoundarySupersessionMismatch'
    'StaleCollectionSequenceRevision'
    'MultiCollectionRangeQueryTickRegressed'
    'RangeStartNotFound'
    'RangeEndNotFound'
    'RangeOrderInvalid'
    'RangeDoesNotCrossCollectionBoundary'
    'RangeTooLarge'
    'RangeSupersessionMismatch'
    'Array.AsReadOnly(collectionPairSummaries)'
    'Array.AsReadOnly(boundarySupersessions)'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'Array.AsReadOnly(crossedBoundarySupersessions)'
    'ReferenceEquals'
    'BoundarySupersessions'
    'CrossedBoundarySupersessions'
)) {
    if ($normalizedHostRecoveryContinuousCollectionSequenceText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery continuous collection-sequence token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryContinuousCollectionSequenceText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery continuous collection-sequence contracts must not wait, ' +
        'schedule work, or read clocks.')
}
if ($hostRecoveryContinuousCollectionSequenceText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery continuous collection-sequence contracts must not generate ' +
        'identities, perform transport, or persist data.')
}
if ($hostRecoveryContinuousCollectionSequenceText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery continuous collection-sequence contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryContinuousCollectionSequenceTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryContinuousCollectionSequenceMultiCollectionRangeTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryContinuousCollectionSequenceTestPath)) {
    throw (
        'Host recovery continuous collection-sequence contract tests are missing.')
}

$hostRecoveryContinuousCollectionSequenceTestText = Get-Content -LiteralPath (
    $hostRecoveryContinuousCollectionSequenceTestPath) -Raw
$hostRecoveryContinuousCollectionSequenceFactCount = (
    [regex]::Matches(
        $hostRecoveryContinuousCollectionSequenceTestText,
        '\[Xunit\.Fact\]').Count)
if ($hostRecoveryContinuousCollectionSequenceFactCount -ne 9) {
    throw (
        'Host recovery continuous collection-sequence tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryContinuousCollectionSequenceTestText -match 'Skip\s*=') {
    throw (
        'Host recovery continuous collection-sequence tests must not be skipped.')
}

foreach ($requiredTest in @(
    'IdsRevisionsTicksAndArgumentsAreValidated'
    'EmptyOversizedAndRevisionCountSequencesAreRejected'
    'SequenceValidationRejectsStaleRevisionAndRegressedTick'
    'SequenceValidationRejectsDuplicateAndSourceMismatch'
    'SingleCollectionPairSequencePreservesExactBoundedAuthority'
    'MultiCollectionQueryRejectsMissingOrderAndNonCrossingRanges'
    'MultiCollectionQueryPreservesExactCrossBoundaryEvidence'
    'SequenceAndQueryCollectionsRemainImmutable'
    'ContractsRemainBoundedAndSideEffectFree'
)) {
    if ($hostRecoveryContinuousCollectionSequenceTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery continuous collection-sequence test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Validates bounded ordered sequences of exact continuous recovery'
    'bounded inclusive ranges across'
    'without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryContinuousCollectionSequenceText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery continuous collection-sequence boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryMultiCollectionRangeSummaryDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryMultiCollectionRangeSummaryFiles = @(
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryAdjacentCollectionSequenceSelectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection.cs'
    'HostRuntimeRecoveryAdjacentCollectionSequenceSelection.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjectionResult.cs'
    'HostRuntimeRecoveryAdjacentCollectionSequenceSelectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryFlow.cs'
)
foreach ($fileName in $hostRecoveryMultiCollectionRangeSummaryFiles) {
    $contractPath = Join-Path (
        $hostRecoveryMultiCollectionRangeSummaryDirectory) $fileName
    if (-not (Test-Path -LiteralPath $contractPath)) {
        throw (
            'Host recovery multi-collection range-summary contract is missing: ' +
            $fileName)
    }
}

$hostRecoveryMultiCollectionRangeSummaryText = (
    $hostRecoveryMultiCollectionRangeSummaryFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryMultiCollectionRangeSummaryDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryMultiCollectionRangeSummaryText =
    (($hostRecoveryMultiCollectionRangeSummaryText -replace
        '(?m)^\s*///\s*', ' ') -replace '\s+', ' ') -replace
        '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjectionIdKind'
    'HostRuntimeRecoveryAdjacentCollectionSequenceSelectionIdKind'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection'
    'HostRuntimeRecoveryAdjacentCollectionSequenceSelection'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjectionResult'
    'HostRuntimeRecoveryAdjacentCollectionSequenceSelectionResult'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryFlow'
)) {
    if ($hostRecoveryMultiCollectionRangeSummaryText -notmatch $requiredType) {
        throw (
            'Host recovery multi-collection range-summary type is missing: ' +
            $requiredType)
    }
}

foreach ($requiredToken in @(
    'MaximumAdjacentCollectionSequencePairCount ='
    'ProjectSummary<TRequest, TState, TCompletion>'
    'SelectPreviousCollectionSequence<TRequest, TState, TCompletion>'
    'SelectNextCollectionSequence<TRequest, TState, TCompletion>'
    'MultiCollectionCheckpointRangeSummaryProjected'
    'PreviousAdjacentCollectionSequenceSelected'
    'NextAdjacentCollectionSequenceSelected'
    'StaleRangeRevision'
    'RangeSummaryProjectionTickRegressed'
    'StaleSummaryRevision'
    'AdjacentCollectionSequenceSelectionTickRegressed'
    'TooManyAdjacentCollectionSequencePairs'
    'NoPreviousAdjacentCollectionSequence'
    'NoNextAdjacentCollectionSequence'
    'PreviousAdjacentCollectionSequenceTooShort'
    'NextAdjacentCollectionSequenceTooShort'
    'AdjacentCollectionSequenceBoundaryMismatch'
    'Array.AsReadOnly(collectionPairSummaries)'
    'Array.AsReadOnly(boundarySupersessions)'
    'CollectionPairSummaries'
    'BoundarySupersessions'
    'AdjacentBoundarySupersession'
    'CrossedBoundarySupersessions'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryMultiCollectionRangeSummaryText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery multi-collection range-summary token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryMultiCollectionRangeSummaryText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery multi-collection range-summary contracts must not wait, ' +
        'schedule work, or read clocks.')
}
if ($hostRecoveryMultiCollectionRangeSummaryText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery multi-collection range-summary contracts must not generate ' +
        'identities, perform transport, or persist data.')
}
if ($hostRecoveryMultiCollectionRangeSummaryText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery multi-collection range-summary contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryMultiCollectionRangeSummaryTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryMultiCollectionRangeSummaryAdjacentCollectionSequenceTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryMultiCollectionRangeSummaryTestPath)) {
    throw (
        'Host recovery multi-collection range-summary contract tests are missing.')
}

$hostRecoveryMultiCollectionRangeSummaryTestText = Get-Content -LiteralPath (
    $hostRecoveryMultiCollectionRangeSummaryTestPath) -Raw
$hostRecoveryMultiCollectionRangeSummaryFactCount = (
    [regex]::Matches(
        $hostRecoveryMultiCollectionRangeSummaryTestText,
        '\[Xunit\.Fact\]').Count)
if ($hostRecoveryMultiCollectionRangeSummaryFactCount -ne 9) {
    throw (
        'Host recovery multi-collection range-summary tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryMultiCollectionRangeSummaryTestText -match 'Skip\s*=') {
    throw (
        'Host recovery multi-collection range-summary tests must not be skipped.')
}

foreach ($requiredTest in @(
    'IdsRevisionsTicksCountsAndArgumentsAreValidated'
    'MultiCollectionSummaryRejectsStaleRevisionAndRegressedTick'
    'MultiCollectionSummaryPreservesExactCompactEvidence'
    'AdjacentSelectionRejectsStaleRevisionRegressedTickAndOversizedCount'
    'PreviousSelectionRejectsMissingAdjacentCollectionSequence'
    'NextSelectionRejectsMissingAdjacentCollectionSequence'
    'FailedSelectionsPreserveSourceAuthorityWithoutMaterialization'
    'SummaryAndSelectionContractsExposeNoPublicSetters'
    'ContractsRemainBoundedAndSideEffectFree'
)) {
    if ($hostRecoveryMultiCollectionRangeSummaryTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery multi-collection range-summary test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Projects compact multi-collection checkpoint-range summaries'
    'bounded adjacent collection-sequences'
    'without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryMultiCollectionRangeSummaryText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery multi-collection range-summary boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryAdjacentCollectionSequenceProjectionDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryAdjacentCollectionSequenceProjectionFiles = @(
    'HostRuntimeRecoveryAdjacentCollectionSequenceProjectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidationIdKind.cs'
    'HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus.cs'
    'HostRuntimeRecoveryAdjacentCollectionSequenceProjection.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidation.cs'
    'HostRuntimeRecoveryAdjacentCollectionSequenceProjectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidationResult.cs'
    'HostRuntimeRecoveryAdjacentCollectionSequenceFlow.cs'
)
foreach ($fileName in $hostRecoveryAdjacentCollectionSequenceProjectionFiles) {
    $contractPath = Join-Path (
        $hostRecoveryAdjacentCollectionSequenceProjectionDirectory) $fileName
    if (-not (Test-Path -LiteralPath $contractPath)) {
        throw (
            'Host recovery adjacent collection-sequence projection contract is missing: ' +
            $fileName)
    }
}

$hostRecoveryAdjacentCollectionSequenceProjectionText = (
    $hostRecoveryAdjacentCollectionSequenceProjectionFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryAdjacentCollectionSequenceProjectionDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryAdjacentCollectionSequenceProjectionText =
    (($hostRecoveryAdjacentCollectionSequenceProjectionText -replace
        '(?m)^\s*///\s*', ' ') -replace '\s+', ' ') -replace
        '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryAdjacentCollectionSequenceProjectionIdKind'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidationIdKind'
    'HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus'
    'HostRuntimeRecoveryAdjacentCollectionSequenceProjection'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidation'
    'HostRuntimeRecoveryAdjacentCollectionSequenceProjectionResult'
    'HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidationResult'
    'HostRuntimeRecoveryAdjacentCollectionSequenceFlow'
)) {
    if ($hostRecoveryAdjacentCollectionSequenceProjectionText -notmatch
        [regex]::Escape($requiredType)) {
        throw (
            'Host recovery adjacent collection-sequence projection type is missing: ' +
            $requiredType)
    }
}

foreach ($requiredToken in @(
    'ProjectCollectionSequence<TRequest, TState, TCompletion>'
    'ValidateContinuity<TRequest, TState, TCompletion>'
    'AdjacentCollectionSequenceProjected'
    'MultiCollectionCheckpointRangeContinuityValidated'
    'StaleSelectionRevision'
    'AdjacentCollectionSequenceProjectionTickRegressed'
    'SelectionCollectionPairSummaryMismatch'
    'SelectionBoundarySupersessionMismatch'
    'SelectionCheckpointMismatch'
    'SelectionSupersessionMismatch'
    'StaleRangeSummaryRevision'
    'StaleAdjacentCollectionSequenceRevision'
    'ContinuityValidationTickRegressed'
    'RangeSummaryMismatch'
    'CollectionPairRangeNotAdjacent'
    'CheckpointRangeNotAdjacent'
    'SupersessionBoundaryMismatch'
    'CheckpointBoundaryMismatch'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'CollectionPairSummaries'
    'BoundarySupersessions'
    'AdjacentBoundarySupersession'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryAdjacentCollectionSequenceProjectionText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery adjacent collection-sequence projection token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryAdjacentCollectionSequenceProjectionText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery adjacent collection-sequence projection contracts must not ' +
        'wait, schedule work, or read clocks.')
}
if ($hostRecoveryAdjacentCollectionSequenceProjectionText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery adjacent collection-sequence projection contracts must not ' +
        'generate identities, perform transport, or persist data.')
}
if ($hostRecoveryAdjacentCollectionSequenceProjectionText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery adjacent collection-sequence projection contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryAdjacentCollectionSequenceProjectionTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryAdjacentCollectionSequenceProjectionContinuityTests.cs')
if (-not (Test-Path -LiteralPath (
        $hostRecoveryAdjacentCollectionSequenceProjectionTestPath))) {
    throw (
        'Host recovery adjacent collection-sequence projection contract tests are missing.')
}

$hostRecoveryAdjacentCollectionSequenceProjectionTestText =
    Get-Content -LiteralPath (
        $hostRecoveryAdjacentCollectionSequenceProjectionTestPath) -Raw
$hostRecoveryAdjacentCollectionSequenceProjectionFactCount = (
    [regex]::Matches(
        $hostRecoveryAdjacentCollectionSequenceProjectionTestText,
        '\[Xunit\.Fact\]').Count)
if ($hostRecoveryAdjacentCollectionSequenceProjectionFactCount -ne 9) {
    throw (
        'Host recovery adjacent collection-sequence projection tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryAdjacentCollectionSequenceProjectionTestText -match 'Skip\s*=') {
    throw (
        'Host recovery adjacent collection-sequence projection tests must not be skipped.')
}

foreach ($requiredTest in @(
    'IdsRevisionsTicksAndArgumentsAreValidated'
    'ProjectionRejectsStaleRevisionAndRegressedTick'
    'ProjectionRejectsCollectionPairBoundaryAndSupersessionEvidenceMismatch'
    'ProjectionMaterializesExactSelectedCollectionSequenceEvidence'
    'ContinuityRejectsStaleRevisionsAndRegressedTick'
    'ContinuityRejectsSourceSummaryAndAdjacencyMismatch'
    'ContinuityRejectsSupersessionAndCheckpointBoundaryMismatch'
    'ProjectionAndValidationCollectionsRemainImmutable'
    'ContractsRemainBoundedAndSideEffectFree'
)) {
    if ($hostRecoveryAdjacentCollectionSequenceProjectionTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery adjacent collection-sequence projection test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Projects exact selected adjacent collection sequences'
    'summarized multi-collection checkpoint ranges'
    'without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryAdjacentCollectionSequenceProjectionText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery adjacent collection-sequence projection boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryContinuousMultiCollectionSummaryDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryContinuousMultiCollectionSummaryFiles = @(
    'HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection.cs'
    'HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionResult.cs'
    'HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSummaryFlow.cs'
)

$hostRecoveryContinuousMultiCollectionSummaryTexts = @()
foreach ($fileName in $hostRecoveryContinuousMultiCollectionSummaryFiles) {
    $filePath = Join-Path (
        $hostRecoveryContinuousMultiCollectionSummaryDirectory) $fileName
    if (-not (Test-Path -LiteralPath $filePath)) {
        throw (
            'Host recovery continuous multi-collection summary contract file is missing: ' +
            $fileName)
    }

    $hostRecoveryContinuousMultiCollectionSummaryTexts +=
        Get-Content -LiteralPath $filePath -Raw
}
$hostRecoveryContinuousMultiCollectionSummaryText =
    $hostRecoveryContinuousMultiCollectionSummaryTexts -join "`n"
$normalizedHostRecoveryContinuousMultiCollectionSummaryText =
    $hostRecoveryContinuousMultiCollectionSummaryText -replace '\s+', ' '

foreach ($requiredToken in @(
    'HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionIdKind'
    'HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQueryIdKind'
    'HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus'
    'HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection'
    'HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQuery'
    'HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionResult'
    'HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQueryResult'
    'HostRuntimeRecoveryContinuousMultiCollectionSummaryFlow'
    'ProjectSummary'
    'QueryRange'
    'MaximumCheckpointCount'
    'ContinuousMultiCollectionSummaryProjected'
    'CrossCollectionSequenceCheckpointRangeQueried'
    'StaleContinuityRevision'
    'MultiCollectionSummaryProjectionTickRegressed'
    'ContinuitySourceMismatch'
    'CollectionPairRangeNotContinuous'
    'CollectionSequenceCheckpointRangeNotContinuous'
    'CollectionSequenceSupersessionMismatch'
    'CollectionSequenceCheckpointMismatch'
    'StaleMultiCollectionSummaryRevision'
    'CrossCollectionSequenceRangeQueryTickRegressed'
    'RangeStartNotFound'
    'RangeEndNotFound'
    'RangeOrderInvalid'
    'RangeDoesNotCrossCollectionSequenceBoundary'
    'RangeTooLarge'
    'RangeSupersessionMismatch'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'ConnectingSupersession'
    'ConnectingPriorCheckpointIndex'
    'ConnectingSuccessorCheckpointIndex'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryContinuousMultiCollectionSummaryText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery continuous multi-collection summary token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryContinuousMultiCollectionSummaryText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery continuous multi-collection summary contracts must not ' +
        'wait, schedule work, or read clocks.')
}
if ($hostRecoveryContinuousMultiCollectionSummaryText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery continuous multi-collection summary contracts must not ' +
        'generate identities, perform transport, or persist data.')
}
if ($hostRecoveryContinuousMultiCollectionSummaryText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery continuous multi-collection summary contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryContinuousMultiCollectionSummaryTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryContinuousMultiCollectionSummaryCrossCollectionSequenceRangeTests.cs')
if (-not (Test-Path -LiteralPath (
        $hostRecoveryContinuousMultiCollectionSummaryTestPath))) {
    throw (
        'Host recovery continuous multi-collection summary contract tests are missing.')
}

$hostRecoveryContinuousMultiCollectionSummaryTestText =
    Get-Content -LiteralPath (
        $hostRecoveryContinuousMultiCollectionSummaryTestPath) -Raw
$hostRecoveryContinuousMultiCollectionSummaryFactCount = (
    [regex]::Matches(
        $hostRecoveryContinuousMultiCollectionSummaryTestText,
        '\[Xunit\.Fact\]').Count)
if ($hostRecoveryContinuousMultiCollectionSummaryFactCount -ne 9) {
    throw (
        'Host recovery continuous multi-collection summary tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryContinuousMultiCollectionSummaryTestText -match 'Skip\s*=') {
    throw (
        'Host recovery continuous multi-collection summary tests must not be skipped.')
}

foreach ($requiredTest in @(
    'IdsAndPublicContractNamesAreExact'
    'FlowExposesProjectionAndBoundedRangeQueryMethods'
    'StatusDefinesExplicitProjectionAndQueryOutcomes'
    'SummaryProjectionIsImmutableAndRevisioned'
    'RangeQueryIsImmutableAndCrossesCollectionSequenceBoundary'
    'ResultContractsExposeExplicitSuccess'
    'ConstructorsRemainNonPublic'
    'MaximumCheckpointCountRemainsBounded'
    'ContractsRemainSynchronousAndSideEffectFree'
)) {
    if ($hostRecoveryContinuousMultiCollectionSummaryTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery continuous multi-collection summary test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Projects compact summaries over one exact continuous recovery multi-collection'
    'bounded inclusive ranges crossing their shared boundary'
    'without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryContinuousMultiCollectionSummaryText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery continuous multi-collection summary boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryContinuousMultiCollectionSequenceDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryContinuousMultiCollectionSequenceFiles = @(
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceValidationIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceValidation.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceValidationResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceFlow.cs'
)

$hostRecoveryContinuousMultiCollectionSequenceTexts = @()
foreach ($fileName in $hostRecoveryContinuousMultiCollectionSequenceFiles) {
    $filePath = Join-Path (
        $hostRecoveryContinuousMultiCollectionSequenceDirectory) $fileName
    if (-not (Test-Path -LiteralPath $filePath)) {
        throw (
            'Host recovery continuous multi-collection-sequence contract file is missing: ' +
            $fileName)
    }

    $hostRecoveryContinuousMultiCollectionSequenceTexts +=
        Get-Content -LiteralPath $filePath -Raw
}
$hostRecoveryContinuousMultiCollectionSequenceText =
    $hostRecoveryContinuousMultiCollectionSequenceTexts -join "`n"
$normalizedHostRecoveryContinuousMultiCollectionSequenceText =
    $hostRecoveryContinuousMultiCollectionSequenceText -replace '\s+', ' '

foreach ($requiredToken in @(
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceValidationIdKind'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQueryIdKind'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceValidation'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQuery'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceValidationResult'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQueryResult'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceFlow'
    'ValidateSequence'
    'QueryRange'
    'MaximumSummaryCount'
    'MaximumCheckpointCount'
    'ContinuousMultiCollectionSequenceValidated'
    'MultiCollectionSequenceCheckpointRangeQueried'
    'MultiCollectionSequenceEmpty'
    'MultiCollectionSequenceTooLarge'
    'MultiCollectionSummaryRevisionCountMismatch'
    'StaleMultiCollectionSummaryRevision'
    'MultiCollectionSequenceValidationTickRegressed'
    'DuplicateMultiCollectionSummaryId'
    'MultiCollectionSummarySourceMismatch'
    'MultiCollectionSummarySupersessionMismatch'
    'MultiCollectionSummaryPairRangeNotContinuous'
    'MultiCollectionSummaryCheckpointRangeNotContinuous'
    'MultiCollectionSequenceBoundarySupersessionMismatch'
    'StaleMultiCollectionSequenceRevision'
    'MultiCollectionSequenceRangeQueryTickRegressed'
    'RangeStartNotFound'
    'RangeEndNotFound'
    'RangeOrderInvalid'
    'RangeDoesNotCrossCollectionSequenceBoundary'
    'RangeTooLarge'
    'RangeSupersessionMismatch'
    'Array.AsReadOnly(snapshot)'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'BoundarySupersessions'
    'CrossedBoundarySupersessions'
    'SourceSequence'
    'SourceCollection'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryContinuousMultiCollectionSequenceText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery continuous multi-collection-sequence token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryContinuousMultiCollectionSequenceText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence contracts must not ' +
        'wait, schedule work, or read clocks.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence contracts must not ' +
        'generate identities, perform transport, or persist data.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryContinuousMultiCollectionSequenceTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceMultiCollectionSequenceRangeTests.cs')
if (-not (Test-Path -LiteralPath (
        $hostRecoveryContinuousMultiCollectionSequenceTestPath))) {
    throw (
        'Host recovery continuous multi-collection-sequence contract tests are missing.')
}

$hostRecoveryContinuousMultiCollectionSequenceTestText =
    Get-Content -LiteralPath (
        $hostRecoveryContinuousMultiCollectionSequenceTestPath) -Raw
$hostRecoveryContinuousMultiCollectionSequenceFactCount = (
    [regex]::Matches(
        $hostRecoveryContinuousMultiCollectionSequenceTestText,
        '\[Xunit\.Fact\]').Count)
if ($hostRecoveryContinuousMultiCollectionSequenceFactCount -ne 9) {
    throw (
        'Host recovery continuous multi-collection-sequence tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceTestText -match 'Skip\s*=') {
    throw (
        'Host recovery continuous multi-collection-sequence tests must not be skipped.')
}

foreach ($requiredTest in @(
    'IdsAndPublicContractNamesAreExact'
    'FlowExposesSequenceValidationAndBoundedRangeQueryMethods'
    'StatusDefinesExplicitValidationAndQueryOutcomes'
    'SequenceValidationIsImmutableAndRevisioned'
    'RangeQueryIsImmutableAndCrossesCollectionSequenceBoundaries'
    'ResultContractsExposeExplicitSuccess'
    'ConstructorsRemainNonPublic'
    'SequenceAndCheckpointBoundsRemainExplicit'
    'ContractsRemainSynchronousAndSideEffectFree'
)) {
    if ($hostRecoveryContinuousMultiCollectionSequenceTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery continuous multi-collection-sequence test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Validates bounded ordered sequences of exact continuous multi-collection summaries'
    'bounded inclusive ranges across their validated collection-sequence boundaries'
    'without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryContinuousMultiCollectionSequenceText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery continuous multi-collection-sequence boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryMultiCollectionSequenceRangeSummaryDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryMultiCollectionSequenceRangeSummaryFiles = @(
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSelectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSelection.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjectionResult.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSelectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryFlow.cs'
)
foreach ($fileName in $hostRecoveryMultiCollectionSequenceRangeSummaryFiles) {
    $contractPath = Join-Path (
        $hostRecoveryMultiCollectionSequenceRangeSummaryDirectory) $fileName
    if (-not (Test-Path -LiteralPath $contractPath)) {
        throw (
            'Host recovery multi-collection-sequence range-summary contract is missing: ' +
            $fileName)
    }
}

$hostRecoveryMultiCollectionSequenceRangeSummaryText = (
    $hostRecoveryMultiCollectionSequenceRangeSummaryFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryMultiCollectionSequenceRangeSummaryDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryMultiCollectionSequenceRangeSummaryText =
    (($hostRecoveryMultiCollectionSequenceRangeSummaryText -replace
        '(?m)^\s*///\s*', ' ') -replace '\s+', ' ') -replace
        '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjectionIdKind'
    'HostRuntimeRecoveryAdjacentMultiCollectionSelectionIdKind'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection'
    'HostRuntimeRecoveryAdjacentMultiCollectionSelection'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjectionResult'
    'HostRuntimeRecoveryAdjacentMultiCollectionSelectionResult'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryFlow'
)) {
    if ($hostRecoveryMultiCollectionSequenceRangeSummaryText -notmatch $requiredType) {
        throw (
            'Host recovery multi-collection-sequence range-summary type is missing: ' +
            $requiredType)
    }
}

foreach ($requiredToken in @(
    'MaximumAdjacentMultiCollectionCount ='
    'ProjectSummary<TRequest, TState, TCompletion>'
    'SelectPreviousMultiCollection<TRequest, TState, TCompletion>'
    'SelectNextMultiCollection<TRequest, TState, TCompletion>'
    'MultiCollectionSequenceCheckpointRangeSummaryProjected'
    'PreviousAdjacentMultiCollectionSelected'
    'NextAdjacentMultiCollectionSelected'
    'StaleRangeRevision'
    'RangeSummaryProjectionTickRegressed'
    'StaleSummaryRevision'
    'AdjacentMultiCollectionSelectionTickRegressed'
    'TooManyAdjacentMultiCollections'
    'NoPreviousAdjacentMultiCollection'
    'NoNextAdjacentMultiCollection'
    'PreviousAdjacentMultiCollectionTooShort'
    'NextAdjacentMultiCollectionTooShort'
    'AdjacentMultiCollectionBoundaryMismatch'
    'Array.AsReadOnly(multiCollectionSummaries)'
    'Array.AsReadOnly(boundarySupersessions)'
    'MultiCollectionSummaries'
    'BoundarySupersessions'
    'AdjacentBoundarySupersession'
    'CrossedBoundarySupersessions'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryMultiCollectionSequenceRangeSummaryText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery multi-collection-sequence range-summary token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryMultiCollectionSequenceRangeSummaryText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery multi-collection-sequence range-summary contracts must not wait, ' +
        'schedule work, or read clocks.')
}
if ($hostRecoveryMultiCollectionSequenceRangeSummaryText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery multi-collection-sequence range-summary contracts must not generate ' +
        'identities, perform transport, or persist data.')
}
if ($hostRecoveryMultiCollectionSequenceRangeSummaryText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery multi-collection-sequence range-summary contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryMultiCollectionSequenceRangeSummaryTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryMultiCollectionSequenceRangeSummaryAdjacentMultiCollectionTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryMultiCollectionSequenceRangeSummaryTestPath)) {
    throw (
        'Host recovery multi-collection-sequence range-summary contract tests are missing.')
}

$hostRecoveryMultiCollectionSequenceRangeSummaryTestText = Get-Content -LiteralPath (
    $hostRecoveryMultiCollectionSequenceRangeSummaryTestPath) -Raw
$hostRecoveryMultiCollectionSequenceRangeSummaryFactCount = (
    [regex]::Matches(
        $hostRecoveryMultiCollectionSequenceRangeSummaryTestText,
        '\[Fact\]').Count)
if ($hostRecoveryMultiCollectionSequenceRangeSummaryFactCount -ne 9) {
    throw (
        'Host recovery multi-collection-sequence range-summary tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryMultiCollectionSequenceRangeSummaryTestText -match 'Skip\s*=') {
    throw (
        'Host recovery multi-collection-sequence range-summary tests must not be skipped.')
}

foreach ($requiredTest in @(
    'PublicContractsAreAvailable'
    'SummaryStatusDefinesExpectedOutcomes'
    'SummaryProjectionPreservesExactRangeAuthorityShape'
    'AdjacentSelectionPreservesExactMultiCollectionAuthorities'
    'FlowExposesBoundedProjectionAndSelectionMethods'
    'ResultContractsPreserveSourceAuthorities'
    'SummaryAndSelectionContractsExposeNoPublicSetters'
    'CollectionPropertiesRemainReadOnly'
    'ContractsRemainBoundedAndSideEffectFree'
)) {
    if ($hostRecoveryMultiCollectionSequenceRangeSummaryTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery multi-collection-sequence range-summary test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Projects compact multi-collection-sequence checkpoint-range summaries'
    'bounded adjacent multi-collections'
    'without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryMultiCollectionSequenceRangeSummaryText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery multi-collection-sequence range-summary boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryAdjacentMultiCollectionProjectionDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryAdjacentMultiCollectionProjectionFiles = @(
    'HostRuntimeRecoveryAdjacentMultiCollectionProjectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidationIdKind.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionProjection.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidation.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionProjectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidationResult.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionFlow.cs'
)
foreach ($fileName in $hostRecoveryAdjacentMultiCollectionProjectionFiles) {
    $contractPath = Join-Path (
        $hostRecoveryAdjacentMultiCollectionProjectionDirectory) $fileName
    if (-not (Test-Path -LiteralPath $contractPath)) {
        throw (
            'Host recovery adjacent multi-collection projection contract is missing: ' +
            $fileName)
    }
}

$hostRecoveryAdjacentMultiCollectionProjectionText = (
    $hostRecoveryAdjacentMultiCollectionProjectionFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryAdjacentMultiCollectionProjectionDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryAdjacentMultiCollectionProjectionText =
    (($hostRecoveryAdjacentMultiCollectionProjectionText -replace
        '(?m)^\s*///\s*', ' ') -replace '\s+', ' ') -replace
        '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryAdjacentMultiCollectionProjectionIdKind'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidationIdKind'
    'HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus'
    'HostRuntimeRecoveryAdjacentMultiCollectionProjection'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidation'
    'HostRuntimeRecoveryAdjacentMultiCollectionProjectionResult'
    'HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidationResult'
    'HostRuntimeRecoveryAdjacentMultiCollectionFlow'
)) {
    if ($hostRecoveryAdjacentMultiCollectionProjectionText -notmatch
        [regex]::Escape($requiredType)) {
        throw (
            'Host recovery adjacent multi-collection projection type is missing: ' +
            $requiredType)
    }
}

foreach ($requiredToken in @(
    'ProjectMultiCollection<TRequest, TState, TCompletion>'
    'ValidateContinuity<TRequest, TState, TCompletion>'
    'AdjacentMultiCollectionProjected'
    'MultiCollectionSequenceCheckpointRangeContinuityValidated'
    'StaleSelectionRevision'
    'AdjacentMultiCollectionProjectionTickRegressed'
    'SelectionMultiCollectionSummaryMismatch'
    'SelectionBoundarySupersessionMismatch'
    'SelectionCheckpointMismatch'
    'SelectionSupersessionMismatch'
    'StaleRangeSummaryRevision'
    'StaleAdjacentMultiCollectionRevision'
    'ContinuityValidationTickRegressed'
    'RangeSummaryMismatch'
    'MultiCollectionRangeNotAdjacent'
    'CheckpointRangeNotAdjacent'
    'SupersessionBoundaryMismatch'
    'CheckpointBoundaryMismatch'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'MultiCollectionSummaries'
    'BoundarySupersessions'
    'AdjacentBoundarySupersession'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryAdjacentMultiCollectionProjectionText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery adjacent multi-collection projection token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryAdjacentMultiCollectionProjectionText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery adjacent multi-collection projection contracts must not ' +
        'wait, schedule work, or read clocks.')
}
if ($hostRecoveryAdjacentMultiCollectionProjectionText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery adjacent multi-collection projection contracts must not ' +
        'generate identities, perform transport, or persist data.')
}
if ($hostRecoveryAdjacentMultiCollectionProjectionText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery adjacent multi-collection projection contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryAdjacentMultiCollectionProjectionTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryAdjacentMultiCollectionProjectionContinuityTests.cs')
if (-not (Test-Path -LiteralPath (
        $hostRecoveryAdjacentMultiCollectionProjectionTestPath))) {
    throw (
        'Host recovery adjacent multi-collection projection contract tests are missing.')
}

$hostRecoveryAdjacentMultiCollectionProjectionTestText =
    Get-Content -LiteralPath (
        $hostRecoveryAdjacentMultiCollectionProjectionTestPath) -Raw
$hostRecoveryAdjacentMultiCollectionProjectionFactCount = (
    [regex]::Matches(
        $hostRecoveryAdjacentMultiCollectionProjectionTestText,
        '\[Fact\]').Count)
if ($hostRecoveryAdjacentMultiCollectionProjectionFactCount -ne 9) {
    throw (
        'Host recovery adjacent multi-collection projection tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryAdjacentMultiCollectionProjectionTestText -match 'Skip\s*=') {
    throw (
        'Host recovery adjacent multi-collection projection tests must not be skipped.')
}

foreach ($requiredTest in @(
    'PublicContractsAreAvailable'
    'ProjectionStatusDefinesExpectedOutcomes'
    'AdjacentProjectionPreservesExactMultiCollectionAuthorityShape'
    'ContinuityValidationPreservesExactBoundaryAuthorityShape'
    'FlowExposesProjectionAndContinuityMethods'
    'ResultContractsPreserveSourceAuthorities'
    'ProjectionAndContinuityContractsExposeNoPublicSetters'
    'MaterializedEvidenceCollectionsRemainReadOnly'
    'ContractsRemainSynchronousAndSideEffectFree'
)) {
    if ($hostRecoveryAdjacentMultiCollectionProjectionTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery adjacent multi-collection projection test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Projects exact selected adjacent multi-collections'
    'summarized multi-collection-sequence checkpoint ranges'
    'without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryAdjacentMultiCollectionProjectionText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery adjacent multi-collection projection boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryContinuousMultiCollectionSequenceSummaryDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryContinuousMultiCollectionSequenceSummaryFiles = @(
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection.cs'
    'HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjectionResult.cs'
    'HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryFlow.cs'
)

$hostRecoveryContinuousMultiCollectionSequenceSummaryTexts = @()
foreach ($fileName in $hostRecoveryContinuousMultiCollectionSequenceSummaryFiles) {
    $filePath = Join-Path (
        $hostRecoveryContinuousMultiCollectionSequenceSummaryDirectory) $fileName
    if (-not (Test-Path -LiteralPath $filePath)) {
        throw (
            'Host recovery continuous multi-collection-sequence summary contract file is missing: ' +
            $fileName)
    }

    $hostRecoveryContinuousMultiCollectionSequenceSummaryTexts +=
        Get-Content -LiteralPath $filePath -Raw
}
$hostRecoveryContinuousMultiCollectionSequenceSummaryText =
    $hostRecoveryContinuousMultiCollectionSequenceSummaryTexts -join "`n"
$normalizedHostRecoveryContinuousMultiCollectionSequenceSummaryText =
    $hostRecoveryContinuousMultiCollectionSequenceSummaryText -replace '\s+', ' '

foreach ($requiredToken in @(
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjectionIdKind'
    'HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQueryIdKind'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection'
    'HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQuery'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjectionResult'
    'HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQueryResult'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryFlow'
    'ProjectSummary'
    'QueryRange'
    'MaximumCheckpointCount'
    'ContinuousMultiCollectionSequenceSummaryProjected'
    'CrossMultiCollectionCheckpointRangeQueried'
    'StaleContinuityRevision'
    'MultiCollectionSequenceSummaryProjectionTickRegressed'
    'ContinuitySourceMismatch'
    'MultiCollectionRangeNotContinuous'
    'MultiCollectionCheckpointRangeNotContinuous'
    'MultiCollectionSupersessionMismatch'
    'MultiCollectionCheckpointMismatch'
    'StaleMultiCollectionSequenceSummaryRevision'
    'CrossMultiCollectionRangeQueryTickRegressed'
    'RangeStartNotFound'
    'RangeEndNotFound'
    'RangeOrderInvalid'
    'RangeDoesNotCrossMultiCollectionBoundary'
    'RangeTooLarge'
    'RangeSupersessionMismatch'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'ConnectingSupersession'
    'ConnectingPriorCheckpointIndex'
    'ConnectingSuccessorCheckpointIndex'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryContinuousMultiCollectionSequenceSummaryText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery continuous multi-collection-sequence summary token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryContinuousMultiCollectionSequenceSummaryText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence summary contracts must not ' +
        'wait, schedule work, or read clocks.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSummaryText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence summary contracts must not ' +
        'generate identities, perform transport, or persist data.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSummaryText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence summary contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryContinuousMultiCollectionSequenceSummaryTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryCrossMultiCollectionRangeTests.cs')
if (-not (Test-Path -LiteralPath (
        $hostRecoveryContinuousMultiCollectionSequenceSummaryTestPath))) {
    throw (
        'Host recovery continuous multi-collection-sequence summary contract tests are missing.')
}

$hostRecoveryContinuousMultiCollectionSequenceSummaryTestText =
    Get-Content -LiteralPath (
        $hostRecoveryContinuousMultiCollectionSequenceSummaryTestPath) -Raw
$hostRecoveryContinuousMultiCollectionSequenceSummaryFactCount = (
    [regex]::Matches(
        $hostRecoveryContinuousMultiCollectionSequenceSummaryTestText,
        '\[Xunit\.Fact\]').Count)
if ($hostRecoveryContinuousMultiCollectionSequenceSummaryFactCount -ne 9) {
    throw (
        'Host recovery continuous multi-collection-sequence summary tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSummaryTestText -match 'Skip\s*=') {
    throw (
        'Host recovery continuous multi-collection-sequence summary tests must not be skipped.')
}

foreach ($requiredTest in @(
    'IdsAndPublicContractNamesAreExact'
    'FlowExposesProjectionAndBoundedRangeQueryMethods'
    'StatusDefinesExplicitProjectionAndQueryOutcomes'
    'SummaryProjectionIsImmutableAndRevisioned'
    'RangeQueryIsImmutableAndCrossesMultiCollectionBoundary'
    'ResultContractsExposeExplicitSuccess'
    'ConstructorsRemainNonPublic'
    'MaximumCheckpointCountRemainsBounded'
    'ContractsRemainSynchronousAndSideEffectFree'
)) {
    if ($hostRecoveryContinuousMultiCollectionSequenceSummaryTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery continuous multi-collection-sequence summary test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Projects compact summaries over one exact continuous recovery multi-collection sequence'
    'bounded inclusive ranges crossing their shared boundary'
    'without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryContinuousMultiCollectionSequenceSummaryText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery continuous multi-collection-sequence summary boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryContinuousMultiCollectionSequenceSequenceDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryContinuousMultiCollectionSequenceSequenceFiles = @(
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceValidationIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceStatus.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceValidation.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceValidationResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceFlow.cs'
)

$hostRecoveryContinuousMultiCollectionSequenceSequenceTexts = @()
foreach ($fileName in $hostRecoveryContinuousMultiCollectionSequenceSequenceFiles) {
    $filePath = Join-Path (
        $hostRecoveryContinuousMultiCollectionSequenceSequenceDirectory) $fileName
    if (-not (Test-Path -LiteralPath $filePath)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence contract file is missing: ' +
            $fileName)
    }

    $hostRecoveryContinuousMultiCollectionSequenceSequenceTexts +=
        Get-Content -LiteralPath $filePath -Raw
}
$hostRecoveryContinuousMultiCollectionSequenceSequenceText =
    $hostRecoveryContinuousMultiCollectionSequenceSequenceTexts -join "`n"
$normalizedHostRecoveryContinuousMultiCollectionSequenceSequenceText =
    $hostRecoveryContinuousMultiCollectionSequenceSequenceText -replace '\s+', ' '

foreach ($requiredToken in @(
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceValidationIdKind'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeQueryIdKind'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceStatus'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceValidation'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeQuery'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceValidationResult'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeQueryResult'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceFlow'
    'ValidateSequence'
    'QueryRange'
    'MaximumSummaryCount'
    'MaximumCheckpointCount'
    'ContinuousMultiCollectionSequenceSequenceValidated'
    'MultiCollectionSequenceSequenceCheckpointRangeQueried'
    'MultiCollectionSequenceSequenceEmpty'
    'MultiCollectionSequenceSequenceTooLarge'
    'MultiCollectionSequenceSummaryRevisionCountMismatch'
    'StaleMultiCollectionSequenceSummaryRevision'
    'MultiCollectionSequenceSequenceValidationTickRegressed'
    'DuplicateMultiCollectionSequenceSummaryId'
    'MultiCollectionSequenceSummarySourceMismatch'
    'MultiCollectionSequenceSummarySupersessionMismatch'
    'MultiCollectionSequenceSummaryRangeNotContinuous'
    'MultiCollectionSequenceSummaryCheckpointRangeNotContinuous'
    'MultiCollectionSequenceSequenceBoundarySupersessionMismatch'
    'StaleMultiCollectionSequenceSequenceRevision'
    'MultiCollectionSequenceSequenceRangeQueryTickRegressed'
    'RangeStartNotFound'
    'RangeEndNotFound'
    'RangeOrderInvalid'
    'RangeDoesNotCrossMultiCollectionSequenceBoundary'
    'RangeTooLarge'
    'RangeSupersessionMismatch'
    'Array.AsReadOnly(snapshot)'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'BoundarySupersessions'
    'CrossedBoundarySupersessions'
    'MultiCollectionSequenceSummaries'
    'SourceSequence'
    'SourceCollection'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryContinuousMultiCollectionSequenceSequenceText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryContinuousMultiCollectionSequenceSequenceText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence contracts must not ' +
        'wait, schedule work, or read clocks.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence contracts must not ' +
        'generate identities, perform transport, or persist data.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryContinuousMultiCollectionSequenceSequenceTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceMultiCollectionSequenceSequenceRangeTests.cs')
if (-not (Test-Path -LiteralPath (
        $hostRecoveryContinuousMultiCollectionSequenceSequenceTestPath))) {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence contract tests are missing.')
}

$hostRecoveryContinuousMultiCollectionSequenceSequenceTestText =
    Get-Content -LiteralPath (
        $hostRecoveryContinuousMultiCollectionSequenceSequenceTestPath) -Raw
$hostRecoveryContinuousMultiCollectionSequenceSequenceFactCount = (
    [regex]::Matches(
        $hostRecoveryContinuousMultiCollectionSequenceSequenceTestText,
        '\[Xunit\.Fact\]').Count)
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceFactCount -ne 9) {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceTestText -match 'Skip\s*=') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence tests must not be skipped.')
}

foreach ($requiredTest in @(
    'IdsAndPublicContractNamesAreExact'
    'FlowExposesSequenceValidationAndBoundedRangeQueryMethods'
    'StatusDefinesExplicitValidationAndQueryOutcomes'
    'SequenceValidationIsImmutableAndRevisioned'
    'RangeQueryIsImmutableAndCrossesMultiCollectionSequenceBoundaries'
    'ResultContractsExposeExplicitSuccess'
    'ConstructorsRemainNonPublic'
    'SequenceAndCheckpointBoundsRemainExplicit'
    'ContractsRemainSynchronousAndSideEffectFree'
)) {
    if ($hostRecoveryContinuousMultiCollectionSequenceSequenceTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Validates bounded ordered sequences of exact continuous multi-collection-sequence summaries'
    'bounded inclusive ranges across their validated multi-collection-sequence boundaries'
    'without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryContinuousMultiCollectionSequenceSequenceText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryMultiCollectionSequenceSequenceRangeSummaryDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryMultiCollectionSequenceSequenceRangeSummaryFiles = @(
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjectionResult.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryFlow.cs'
)

$hostRecoveryMultiCollectionSequenceSequenceRangeSummaryTexts = @()
foreach ($fileName in $hostRecoveryMultiCollectionSequenceSequenceRangeSummaryFiles) {
    $filePath = Join-Path (
        $hostRecoveryMultiCollectionSequenceSequenceRangeSummaryDirectory) $fileName
    if (-not (Test-Path -LiteralPath $filePath)) {
        throw (
            'Host recovery multi-collection-sequence-sequence range-summary file is missing: ' +
            $fileName)
    }

    $hostRecoveryMultiCollectionSequenceSequenceRangeSummaryTexts +=
        Get-Content -LiteralPath $filePath -Raw
}
$hostRecoveryMultiCollectionSequenceSequenceRangeSummaryText =
    $hostRecoveryMultiCollectionSequenceSequenceRangeSummaryTexts -join "`n"
$normalizedHostRecoveryMultiCollectionSequenceSequenceRangeSummaryText =
    $hostRecoveryMultiCollectionSequenceSequenceRangeSummaryText -replace '\s+', ' '

foreach ($requiredToken in @(
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjectionIdKind'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionIdKind'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjectionResult'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionResult'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryFlow'
    'ProjectSummary'
    'SelectPreviousMultiCollectionSequence'
    'SelectNextMultiCollectionSequence'
    'MaximumAdjacentMultiCollectionSequenceCount'
    'MultiCollectionSequenceSequenceCheckpointRangeSummaryProjected'
    'PreviousAdjacentMultiCollectionSequenceSelected'
    'NextAdjacentMultiCollectionSequenceSelected'
    'StaleRangeRevision'
    'RangeSummaryProjectionTickRegressed'
    'StaleSummaryRevision'
    'AdjacentMultiCollectionSequenceSelectionTickRegressed'
    'TooManyAdjacentMultiCollectionSequences'
    'NoPreviousAdjacentMultiCollectionSequence'
    'NoNextAdjacentMultiCollectionSequence'
    'PreviousAdjacentMultiCollectionSequenceTooShort'
    'NextAdjacentMultiCollectionSequenceTooShort'
    'AdjacentMultiCollectionSequenceBoundaryMismatch'
    'Array.AsReadOnly(multiCollectionSequenceSummaries)'
    'Array.AsReadOnly(boundarySupersessions)'
    'MultiCollectionSequenceSummaries'
    'AdjacentBoundarySupersession'
    'SourceSequence'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryMultiCollectionSequenceSequenceRangeSummaryText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery multi-collection-sequence-sequence range-summary token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryMultiCollectionSequenceSequenceRangeSummaryText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery multi-collection-sequence-sequence range-summary contracts must not ' +
        'wait, schedule work, or read clocks.')
}
if ($hostRecoveryMultiCollectionSequenceSequenceRangeSummaryText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery multi-collection-sequence-sequence range-summary contracts must not ' +
        'generate identities, perform transport, or persist data.')
}
if ($hostRecoveryMultiCollectionSequenceSequenceRangeSummaryText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery multi-collection-sequence-sequence range-summary contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryMultiCollectionSequenceSequenceRangeSummaryTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceRangeSummaryAdjacentMultiCollectionSequenceTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryMultiCollectionSequenceSequenceRangeSummaryTestPath)) {
    throw (
        'Host recovery multi-collection-sequence-sequence range-summary tests are missing.')
}

$hostRecoveryMultiCollectionSequenceSequenceRangeSummaryTestText =
    Get-Content -LiteralPath (
        $hostRecoveryMultiCollectionSequenceSequenceRangeSummaryTestPath) -Raw
$hostRecoveryMultiCollectionSequenceSequenceRangeSummaryFactCount = (
    [regex]::Matches(
        $hostRecoveryMultiCollectionSequenceSequenceRangeSummaryTestText,
        '\[Fact\]').Count)
if ($hostRecoveryMultiCollectionSequenceSequenceRangeSummaryFactCount -ne 9) {
    throw (
        'Host recovery multi-collection-sequence-sequence range-summary tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryMultiCollectionSequenceSequenceRangeSummaryTestText -match 'Skip\s*=') {
    throw (
        'Host recovery multi-collection-sequence-sequence range-summary tests must not be skipped.')
}

foreach ($requiredTest in @(
    'PublicContractsAreAvailable'
    'SummaryStatusDefinesExpectedOutcomes'
    'SummaryProjectionPreservesExactRangeAuthorityShape'
    'AdjacentSelectionPreservesExactMultiCollectionSequenceAuthorities'
    'FlowExposesBoundedProjectionAndSelectionMethods'
    'ResultContractsPreserveSourceAuthorities'
    'SummaryAndSelectionContractsExposeNoPublicSetters'
    'CollectionPropertiesRemainReadOnly'
    'ContractsRemainBoundedAndSideEffectFree'
)) {
    if ($hostRecoveryMultiCollectionSequenceSequenceRangeSummaryTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery multi-collection-sequence-sequence range-summary test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Projects compact multi-collection-sequence-sequence checkpoint-range summaries'
    'selects exact bounded adjacent multi-collection-sequences without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryMultiCollectionSequenceSequenceRangeSummaryText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery multi-collection-sequence-sequence range-summary boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryAdjacentMultiCollectionSequenceProjectionDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryAdjacentMultiCollectionSequenceProjectionFiles = @(
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidationIdKind.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjection.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidation.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidationResult.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceFlow.cs'
)
foreach ($fileName in $hostRecoveryAdjacentMultiCollectionSequenceProjectionFiles) {
    $contractPath = Join-Path (
        $hostRecoveryAdjacentMultiCollectionSequenceProjectionDirectory) $fileName
    if (-not (Test-Path -LiteralPath $contractPath)) {
        throw (
            'Host recovery adjacent multi-collection-sequence projection contract is missing: ' +
            $fileName)
    }
}

$hostRecoveryAdjacentMultiCollectionSequenceProjectionText = (
    $hostRecoveryAdjacentMultiCollectionSequenceProjectionFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryAdjacentMultiCollectionSequenceProjectionDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryAdjacentMultiCollectionSequenceProjectionText =
    (($hostRecoveryAdjacentMultiCollectionSequenceProjectionText -replace
        '(?m)^\s*///\s*', ' ') -replace '\s+', ' ') -replace
        '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionIdKind'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidationIdKind'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjection'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidation'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionResult'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidationResult'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceFlow'
)) {
    if ($hostRecoveryAdjacentMultiCollectionSequenceProjectionText -notmatch
        [regex]::Escape($requiredType)) {
        throw (
            'Host recovery adjacent multi-collection-sequence projection type is missing: ' +
            $requiredType)
    }
}

foreach ($requiredToken in @(
    'ProjectMultiCollectionSequence<TRequest, TState, TCompletion>'
    'ValidateContinuity<TRequest, TState, TCompletion>'
    'AdjacentMultiCollectionSequenceProjected'
    'MultiCollectionSequenceSequenceCheckpointRangeContinuityValidated'
    'StaleSelectionRevision'
    'AdjacentMultiCollectionSequenceProjectionTickRegressed'
    'SelectionMultiCollectionSequenceSummaryMismatch'
    'SelectionBoundarySupersessionMismatch'
    'SelectionCheckpointMismatch'
    'SelectionSupersessionMismatch'
    'StaleRangeSummaryRevision'
    'StaleAdjacentMultiCollectionSequenceRevision'
    'ContinuityValidationTickRegressed'
    'RangeSummaryMismatch'
    'MultiCollectionSequenceRangeNotAdjacent'
    'CheckpointRangeNotAdjacent'
    'SupersessionBoundaryMismatch'
    'CheckpointBoundaryMismatch'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'MultiCollectionSequenceSummaries'
    'BoundarySupersessions'
    'AdjacentBoundarySupersession'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryAdjacentMultiCollectionSequenceProjectionText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery adjacent multi-collection-sequence projection token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryAdjacentMultiCollectionSequenceProjectionText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery adjacent multi-collection-sequence projection contracts must not ' +
        'wait, schedule work, or read clocks.')
}
if ($hostRecoveryAdjacentMultiCollectionSequenceProjectionText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery adjacent multi-collection-sequence projection contracts must not ' +
        'generate identities, perform transport, or persist data.')
}
if ($hostRecoveryAdjacentMultiCollectionSequenceProjectionText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery adjacent multi-collection-sequence projection contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryAdjacentMultiCollectionSequenceProjectionTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionContinuityTests.cs')
if (-not (Test-Path -LiteralPath (
        $hostRecoveryAdjacentMultiCollectionSequenceProjectionTestPath))) {
    throw (
        'Host recovery adjacent multi-collection-sequence projection contract tests are missing.')
}

$hostRecoveryAdjacentMultiCollectionSequenceProjectionTestText =
    Get-Content -LiteralPath (
        $hostRecoveryAdjacentMultiCollectionSequenceProjectionTestPath) -Raw
$hostRecoveryAdjacentMultiCollectionSequenceProjectionFactCount = (
    [regex]::Matches(
        $hostRecoveryAdjacentMultiCollectionSequenceProjectionTestText,
        '\[Fact\]').Count)
if ($hostRecoveryAdjacentMultiCollectionSequenceProjectionFactCount -ne 9) {
    throw (
        'Host recovery adjacent multi-collection-sequence projection tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryAdjacentMultiCollectionSequenceProjectionTestText -match 'Skip\s*=') {
    throw (
        'Host recovery adjacent multi-collection-sequence projection tests must not be skipped.')
}

foreach ($requiredTest in @(
    'PublicContractsAreAvailable'
    'ProjectionStatusDefinesExpectedOutcomes'
    'AdjacentProjectionPreservesExactMultiCollectionSequenceAuthorityShape'
    'ContinuityValidationPreservesExactBoundaryAuthorityShape'
    'FlowExposesProjectionAndContinuityMethods'
    'ResultContractsPreserveSourceAuthorities'
    'ProjectionAndContinuityContractsExposeNoPublicSetters'
    'MaterializedEvidenceCollectionsRemainReadOnly'
    'ContractsRemainSynchronousAndSideEffectFree'
)) {
    if ($hostRecoveryAdjacentMultiCollectionSequenceProjectionTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery adjacent multi-collection-sequence projection test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Projects exact selected adjacent multi-collection-sequences'
    'summarized multi-collection-sequence-sequence checkpoint ranges'
    'without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryAdjacentMultiCollectionSequenceProjectionText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery adjacent multi-collection-sequence projection boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryFiles = @(
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjection.cs'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjectionResult.cs'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryFlow.cs'
)

$hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryTexts = @()
foreach ($fileName in $hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryFiles) {
    $filePath = Join-Path (
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryDirectory) $fileName
    if (-not (Test-Path -LiteralPath $filePath)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence summary contract file is missing: ' +
            $fileName)
    }

    $hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryTexts +=
        Get-Content -LiteralPath $filePath -Raw
}
$hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryText =
    $hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryTexts -join "`n"
$normalizedHostRecoveryContinuousMultiCollectionSequenceSequenceSummaryText =
    $hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryText -replace '\s+', ' '

foreach ($requiredToken in @(
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjectionIdKind'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceCheckpointRangeQueryIdKind'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjection'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceCheckpointRangeQuery'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjectionResult'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceCheckpointRangeQueryResult'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryFlow'
    'ProjectSummary'
    'QueryRange'
    'MaximumCheckpointCount'
    'ContinuousMultiCollectionSequenceSequenceSummaryProjected'
    'CrossMultiCollectionSequenceCheckpointRangeQueried'
    'StaleContinuityRevision'
    'MultiCollectionSequenceSequenceSummaryProjectionTickRegressed'
    'ContinuitySourceMismatch'
    'MultiCollectionSequenceRangeNotContinuous'
    'MultiCollectionSequenceCheckpointRangeNotContinuous'
    'MultiCollectionSequenceSupersessionMismatch'
    'MultiCollectionSequenceCheckpointMismatch'
    'StaleMultiCollectionSequenceSequenceSummaryRevision'
    'CrossMultiCollectionSequenceRangeQueryTickRegressed'
    'RangeStartNotFound'
    'RangeEndNotFound'
    'RangeOrderInvalid'
    'RangeDoesNotCrossMultiCollectionSequenceBoundary'
    'RangeTooLarge'
    'RangeSupersessionMismatch'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'ConnectingSupersession'
    'ConnectingPriorCheckpointIndex'
    'ConnectingSuccessorCheckpointIndex'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryContinuousMultiCollectionSequenceSequenceSummaryText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence summary token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence summary contracts must not ' +
        'wait, schedule work, or read clocks.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence summary contracts must not ' +
        'generate identities, perform transport, or persist data.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence summary contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryCrossMultiCollectionSequenceRangeTests.cs')
if (-not (Test-Path -LiteralPath (
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryTestPath))) {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence summary contract tests are missing.')
}

$hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryTestText =
    Get-Content -LiteralPath (
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryTestPath) -Raw
$hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryFactCount = (
    [regex]::Matches(
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryTestText,
        '\[Xunit\.Fact\]').Count)
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryFactCount -ne 9) {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence summary tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryTestText -match 'Skip\s*=') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence summary tests must not be skipped.')
}

foreach ($requiredTest in @(
    'IdsAndPublicContractNamesAreExact'
    'FlowExposesProjectionAndBoundedRangeQueryMethods'
    'StatusDefinesExplicitProjectionAndQueryOutcomes'
    'SummaryProjectionIsImmutableAndRevisioned'
    'RangeQueryIsImmutableAndCrossesMultiCollectionSequenceBoundary'
    'ResultContractsExposeExplicitSuccess'
    'ConstructorsRemainNonPublic'
    'MaximumCheckpointCountRemainsBounded'
    'ContractsRemainSynchronousAndSideEffectFree'
)) {
    if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSummaryTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence summary test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Projects compact summaries over one exact continuous recovery multi-collection-sequence-sequence'
    'bounded inclusive ranges crossing their shared boundary'
    'without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryContinuousMultiCollectionSequenceSequenceSummaryText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence summary boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceFiles = @(
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidationIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceStatus.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidation.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidationResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceFlow.cs'
)

$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceTexts = @()
foreach ($fileName in $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceFiles) {
    $filePath = Join-Path (
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceDirectory) $fileName
    if (-not (Test-Path -LiteralPath $filePath)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence-sequence contract file is missing: ' +
            $fileName)
    }

    $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceTexts +=
        Get-Content -LiteralPath $filePath -Raw
}
$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceText =
    $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceTexts -join "`n"
$normalizedHostRecoveryContinuousMultiCollectionSequenceSequenceText =
    $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceText -replace '\s+', ' '

foreach ($requiredToken in @(
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidationIdKind'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryIdKind'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceStatus'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidation'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQuery'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidationResult'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryResult'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceFlow'
    'ValidateSequence'
    'QueryRange'
    'MaximumSummaryCount'
    'MaximumCheckpointCount'
    'ContinuousMultiCollectionSequenceSequenceSequenceValidated'
    'MultiCollectionSequenceSequenceSequenceCheckpointRangeQueried'
    'MultiCollectionSequenceSequenceSequenceEmpty'
    'MultiCollectionSequenceSequenceSequenceTooLarge'
    'MultiCollectionSequenceSequenceSummaryRevisionCountMismatch'
    'StaleMultiCollectionSequenceSequenceSummaryRevision'
    'MultiCollectionSequenceSequenceSequenceValidationTickRegressed'
    'DuplicateMultiCollectionSequenceSequenceSummaryId'
    'MultiCollectionSequenceSequenceSummarySourceMismatch'
    'MultiCollectionSequenceSequenceSummarySupersessionMismatch'
    'MultiCollectionSequenceSequenceSummaryRangeNotContinuous'
    'MultiCollectionSequenceSequenceSummaryCheckpointRangeNotContinuous'
    'MultiCollectionSequenceSequenceSequenceBoundarySupersessionMismatch'
    'StaleMultiCollectionSequenceSequenceSequenceRevision'
    'MultiCollectionSequenceSequenceSequenceRangeQueryTickRegressed'
    'RangeStartNotFound'
    'RangeEndNotFound'
    'RangeOrderInvalid'
    'RangeDoesNotCrossMultiCollectionSequenceSequenceBoundary'
    'RangeTooLarge'
    'RangeSupersessionMismatch'
    'Array.AsReadOnly(snapshot)'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'BoundarySupersessions'
    'CrossedBoundarySupersessions'
    'MultiCollectionSequenceSequenceSummaries'
    'SourceSequence'
    'SourceCollection'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryContinuousMultiCollectionSequenceSequenceText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence-sequence token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence contracts must not ' +
        'wait, schedule work, or read clocks.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence contracts must not ' +
        'generate identities, perform transport, or persist data.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceMultiCollectionSequenceSequenceSequenceRangeTests.cs')
if (-not (Test-Path -LiteralPath (
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceTestPath))) {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence contract tests are missing.')
}

$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceTestText =
    Get-Content -LiteralPath (
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceTestPath) -Raw
$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceFactCount = (
    [regex]::Matches(
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceTestText,
        '\[Xunit\.Fact\]').Count)
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceFactCount -ne 9) {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceTestText -match 'Skip\s*=') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence tests must not be skipped.')
}

foreach ($requiredTest in @(
    'IdsAndPublicContractNamesAreExact'
    'FlowExposesSequenceValidationAndBoundedRangeQueryMethods'
    'StatusDefinesExplicitValidationAndQueryOutcomes'
    'SequenceValidationIsImmutableAndRevisioned'
    'RangeQueryIsImmutableAndCrossesMultiCollectionSequenceSequenceBoundaries'
    'ResultContractsExposeExplicitSuccess'
    'ConstructorsRemainNonPublic'
    'SequenceAndCheckpointBoundsRemainExplicit'
    'ContractsRemainSynchronousAndSideEffectFree'
)) {
    if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence-sequence test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Validates bounded ordered sequences of exact continuous multi-collection-sequence-sequence summaries'
    'bounded inclusive ranges across their validated multi-collection-sequence-sequence boundaries'
    'without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryContinuousMultiCollectionSequenceSequenceText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence-sequence boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryFiles = @(
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjection.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelection.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryFlow.cs'
)

$hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryTexts = @()
foreach ($fileName in $hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryFiles) {
    $filePath = Join-Path (
        $hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryDirectory) $fileName
    if (-not (Test-Path -LiteralPath $filePath)) {
        throw (
            'Host recovery multi-collection-sequence-sequence-sequence range-summary file is missing: ' +
            $fileName)
    }

    $hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryTexts +=
        Get-Content -LiteralPath $filePath -Raw
}
$hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryText =
    $hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryTexts -join "`n"
$normalizedHostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryText =
    $hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryText -replace '\s+', ' '

foreach ($requiredToken in @(
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjectionIdKind'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelectionIdKind'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjection'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelection'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelectionResult'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryFlow'
    'ProjectSummary'
    'SelectPreviousMultiCollectionSequenceSequence'
    'SelectNextMultiCollectionSequenceSequence'
    'MaximumAdjacentMultiCollectionSequenceSequenceCount'
    'MultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjected'
    'PreviousAdjacentMultiCollectionSequenceSequenceSelected'
    'NextAdjacentMultiCollectionSequenceSequenceSelected'
    'StaleRangeRevision'
    'RangeSummaryProjectionTickRegressed'
    'StaleSummaryRevision'
    'AdjacentMultiCollectionSequenceSequenceSelectionTickRegressed'
    'TooManyAdjacentMultiCollectionSequenceSequences'
    'NoPreviousAdjacentMultiCollectionSequenceSequence'
    'NoNextAdjacentMultiCollectionSequenceSequence'
    'PreviousAdjacentMultiCollectionSequenceSequenceTooShort'
    'NextAdjacentMultiCollectionSequenceSequenceTooShort'
    'AdjacentMultiCollectionSequenceSequenceBoundaryMismatch'
    'Array.AsReadOnly(multiCollectionSequenceSequenceSummaries)'
    'Array.AsReadOnly(boundarySupersessions)'
    'MultiCollectionSequenceSequenceSummaries'
    'AdjacentBoundarySupersession'
    'SourceSequence'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery multi-collection-sequence-sequence-sequence range-summary token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery multi-collection-sequence-sequence-sequence range-summary contracts must not ' +
        'wait, schedule work, or read clocks.')
}
if ($hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery multi-collection-sequence-sequence-sequence range-summary contracts must not ' +
        'generate identities, perform transport, or persist data.')
}
if ($hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery multi-collection-sequence-sequence-sequence range-summary contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryAdjacentMultiCollectionSequenceSequenceTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryTestPath)) {
    throw (
        'Host recovery multi-collection-sequence-sequence-sequence range-summary tests are missing.')
}

$hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryTestText =
    Get-Content -LiteralPath (
        $hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryTestPath) -Raw
$hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryFactCount = (
    [regex]::Matches(
        $hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryTestText,
        '\[Fact\]').Count)
if ($hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryFactCount -ne 9) {
    throw (
        'Host recovery multi-collection-sequence-sequence-sequence range-summary tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryTestText -match 'Skip\s*=') {
    throw (
        'Host recovery multi-collection-sequence-sequence-sequence range-summary tests must not be skipped.')
}

foreach ($requiredTest in @(
    'PublicContractsAreAvailable'
    'SummaryStatusDefinesExpectedOutcomes'
    'SummaryProjectionPreservesExactRangeAuthorityShape'
    'AdjacentSelectionPreservesExactMultiCollectionSequenceSequenceAuthorities'
    'FlowExposesBoundedProjectionAndSelectionMethods'
    'ResultContractsPreserveSourceAuthorities'
    'SummaryAndSelectionContractsExposeNoPublicSetters'
    'CollectionPropertiesRemainReadOnly'
    'ContractsRemainBoundedAndSideEffectFree'
)) {
    if ($hostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery multi-collection-sequence-sequence-sequence range-summary test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Projects compact multi-collection-sequence-sequence-sequence checkpoint-range summaries'
    'selects exact bounded adjacent multi-collection-sequence-sequences without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryMultiCollectionSequenceSequenceSequenceRangeSummaryText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery multi-collection-sequence-sequence-sequence range-summary boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionFiles = @(
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidationIdKind.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjection.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidation.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidationResult.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceFlow.cs'
)
foreach ($fileName in $hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionFiles) {
    $contractPath = Join-Path (
        $hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionDirectory) $fileName
    if (-not (Test-Path -LiteralPath $contractPath)) {
        throw (
            'Host recovery adjacent multi-collection-sequence projection contract is missing: ' +
            $fileName)
    }
}

$hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionText = (
    $hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionText =
    (($hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionText -replace
        '(?m)^\s*///\s*', ' ') -replace '\s+', ' ') -replace
        '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionIdKind'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidationIdKind'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjection'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidation'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionResult'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidationResult'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceFlow'
)) {
    if ($hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionText -notmatch
        [regex]::Escape($requiredType)) {
        throw (
            'Host recovery adjacent multi-collection-sequence projection type is missing: ' +
            $requiredType)
    }
}

foreach ($requiredToken in @(
    'ProjectMultiCollectionSequenceSequence<TRequest, TState, TCompletion>'
    'ValidateContinuity<TRequest, TState, TCompletion>'
    'AdjacentMultiCollectionSequenceSequenceProjected'
    'MultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidated'
    'StaleSelectionRevision'
    'AdjacentMultiCollectionSequenceSequenceProjectionTickRegressed'
    'SelectionMultiCollectionSequenceSequenceSummaryMismatch'
    'SelectionBoundarySupersessionMismatch'
    'SelectionCheckpointMismatch'
    'SelectionSupersessionMismatch'
    'StaleRangeSummaryRevision'
    'StaleAdjacentMultiCollectionSequenceSequenceRevision'
    'ContinuityValidationTickRegressed'
    'RangeSummaryMismatch'
    'MultiCollectionSequenceSequenceRangeNotAdjacent'
    'CheckpointRangeNotAdjacent'
    'SupersessionBoundaryMismatch'
    'CheckpointBoundaryMismatch'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'MultiCollectionSequenceSequenceSummaries'
    'BoundarySupersessions'
    'AdjacentBoundarySupersession'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery adjacent multi-collection-sequence projection token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery adjacent multi-collection-sequence projection contracts must not ' +
        'wait, schedule work, or read clocks.')
}
if ($hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery adjacent multi-collection-sequence projection contracts must not ' +
        'generate identities, perform transport, or persist data.')
}
if ($hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery adjacent multi-collection-sequence projection contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionContinuityTests.cs')
if (-not (Test-Path -LiteralPath (
        $hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionTestPath))) {
    throw (
        'Host recovery adjacent multi-collection-sequence projection contract tests are missing.')
}

$hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionTestText =
    Get-Content -LiteralPath (
        $hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionTestPath) -Raw
$hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionFactCount = (
    [regex]::Matches(
        $hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionTestText,
        '\[Fact\]').Count)
if ($hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionFactCount -ne 9) {
    throw (
        'Host recovery adjacent multi-collection-sequence projection tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionTestText -match 'Skip\s*=') {
    throw (
        'Host recovery adjacent multi-collection-sequence projection tests must not be skipped.')
}

foreach ($requiredTest in @(
    'PublicContractsAreAvailable'
    'ProjectionStatusDefinesExpectedOutcomes'
    'AdjacentProjectionPreservesExactMultiCollectionSequenceAuthorityShape'
    'ContinuityValidationPreservesExactBoundaryAuthorityShape'
    'FlowExposesProjectionAndContinuityMethods'
    'ResultContractsPreserveSourceAuthorities'
    'ProjectionAndContinuityContractsExposeNoPublicSetters'
    'MaterializedEvidenceCollectionsRemainReadOnly'
    'ContractsRemainSynchronousAndSideEffectFree'
)) {
    if ($hostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery adjacent multi-collection-sequence projection test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Projects exact selected adjacent multi-collection-sequence-sequences'
    'summarized multi-collection-sequence-sequence-sequence checkpoint ranges'
    'without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryAdjacentMultiCollectionSequenceSequenceProjectionText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery adjacent multi-collection-sequence projection boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryFiles = @(
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection.cs'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjectionResult.cs'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryFlow.cs'
)

$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryTexts = @()
foreach ($fileName in $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryFiles) {
    $filePath = Join-Path (
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryDirectory) $fileName
    if (-not (Test-Path -LiteralPath $filePath)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence-sequence summary contract file is missing: ' +
            $fileName)
    }

    $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryTexts +=
        Get-Content -LiteralPath $filePath -Raw
}
$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryText =
    $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryTexts -join "`n"
$normalizedHostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryText =
    $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryText -replace '\s+', ' '

foreach ($requiredToken in @(
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjectionIdKind'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceCheckpointRangeQueryIdKind'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceCheckpointRangeQuery'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjectionResult'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceCheckpointRangeQueryResult'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryFlow'
    'ProjectSummary'
    'QueryRange'
    'MaximumCheckpointCount'
    'ContinuousMultiCollectionSequenceSequenceSequenceSummaryProjected'
    'CrossMultiCollectionSequenceSequenceCheckpointRangeQueried'
    'StaleContinuityRevision'
    'MultiCollectionSequenceSequenceSequenceSummaryProjectionTickRegressed'
    'ContinuitySourceMismatch'
    'MultiCollectionSequenceSequenceRangeNotContinuous'
    'MultiCollectionSequenceSequenceCheckpointRangeNotContinuous'
    'MultiCollectionSequenceSequenceSupersessionMismatch'
    'MultiCollectionSequenceSequenceCheckpointMismatch'
    'StaleMultiCollectionSequenceSequenceSequenceSummaryRevision'
    'CrossMultiCollectionSequenceSequenceRangeQueryTickRegressed'
    'RangeStartNotFound'
    'RangeEndNotFound'
    'RangeOrderInvalid'
    'RangeDoesNotCrossMultiCollectionSequenceSequenceBoundary'
    'RangeTooLarge'
    'RangeSupersessionMismatch'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'ConnectingSupersession'
    'ConnectingPriorCheckpointIndex'
    'ConnectingSuccessorCheckpointIndex'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence-sequence summary token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence summary contracts must not ' +
        'wait, schedule work, or read clocks.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence summary contracts must not ' +
        'generate identities, perform transport, or persist data.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence summary contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryCrossMultiCollectionSequenceSequenceRangeTests.cs')
if (-not (Test-Path -LiteralPath (
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryTestPath))) {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence summary contract tests are missing.')
}

$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryTestText =
    Get-Content -LiteralPath (
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryTestPath) -Raw
$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryFactCount = (
    [regex]::Matches(
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryTestText,
        '\[Xunit\.Fact\]').Count)
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryFactCount -ne 9) {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence summary tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryTestText -match 'Skip\s*=') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence summary tests must not be skipped.')
}

foreach ($requiredTest in @(
    'IdsAndPublicContractNamesAreExact'
    'FlowExposesProjectionAndBoundedRangeQueryMethods'
    'StatusDefinesExplicitProjectionAndQueryOutcomes'
    'SummaryProjectionIsImmutableAndRevisioned'
    'RangeQueryIsImmutableAndCrossesMultiCollectionSequenceSequenceBoundary'
    'ResultContractsExposeExplicitSuccess'
    'ConstructorsRemainNonPublic'
    'MaximumCheckpointCountRemainsBounded'
    'ContractsRemainSynchronousAndSideEffectFree'
)) {
    if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence-sequence summary test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Projects compact summaries over one exact continuous recovery multi-collection-sequence-sequence-sequence'
    'bounded inclusive ranges crossing their shared boundary'
    'without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence-sequence summary boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceFiles = @(
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidationIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidation.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidationResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceFlow.cs'
)

$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceTexts = @()
foreach ($fileName in $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceFiles) {
    $filePath = Join-Path (
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceDirectory) $fileName
    if (-not (Test-Path -LiteralPath $filePath)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence contract file is missing: ' +
            $fileName)
    }

    $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceTexts +=
        Get-Content -LiteralPath $filePath -Raw
}
$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceText =
    $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceTexts -join "`n"
$normalizedHostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceText =
    $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceText -replace '\s+', ' '

foreach ($requiredToken in @(
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidationIdKind'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQueryIdKind'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidation'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQuery'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidationResult'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQueryResult'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceFlow'
    'ValidateSequence'
    'QueryRange'
    'MaximumSummaryCount'
    'MaximumCheckpointCount'
    'ContinuousMultiCollectionSequenceSequenceSequenceSequenceValidated'
    'MultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQueried'
    'MultiCollectionSequenceSequenceSequenceSequenceEmpty'
    'MultiCollectionSequenceSequenceSequenceSequenceTooLarge'
    'MultiCollectionSequenceSequenceSequenceSummaryRevisionCountMismatch'
    'StaleMultiCollectionSequenceSequenceSequenceSummaryRevision'
    'MultiCollectionSequenceSequenceSequenceSequenceValidationTickRegressed'
    'DuplicateMultiCollectionSequenceSequenceSequenceSummaryId'
    'MultiCollectionSequenceSequenceSequenceSummarySourceMismatch'
    'MultiCollectionSequenceSequenceSequenceSummarySupersessionMismatch'
    'MultiCollectionSequenceSequenceSequenceSummaryRangeNotContinuous'
    'MultiCollectionSequenceSequenceSequenceSummaryCheckpointRangeNotContinuous'
    'MultiCollectionSequenceSequenceSequenceSequenceBoundarySupersessionMismatch'
    'StaleMultiCollectionSequenceSequenceSequenceSequenceRevision'
    'MultiCollectionSequenceSequenceSequenceSequenceRangeQueryTickRegressed'
    'RangeStartNotFound'
    'RangeEndNotFound'
    'RangeOrderInvalid'
    'RangeDoesNotCrossMultiCollectionSequenceSequenceSequenceBoundary'
    'RangeTooLarge'
    'RangeSupersessionMismatch'
    'Array.AsReadOnly(snapshot)'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'BoundarySupersessions'
    'CrossedBoundarySupersessions'
    'MultiCollectionSequenceSequenceSequenceSummaries'
    'SourceSequence'
    'SourceCollection'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence contracts must not ' +
        'wait, schedule work, or read clocks.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence contracts must not ' +
        'generate identities, perform transport, or persist data.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceMultiCollectionSequenceSequenceSequenceSequenceRangeTests.cs')
if (-not (Test-Path -LiteralPath (
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceTestPath))) {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence contract tests are missing.')
}

$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceTestText =
    Get-Content -LiteralPath (
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceTestPath) -Raw
$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceFactCount = (
    [regex]::Matches(
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceTestText,
        '\[Xunit\.Fact\]').Count)
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceFactCount -ne 9) {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceTestText -match 'Skip\s*=') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence tests must not be skipped.')
}

foreach ($requiredTest in @(
    'IdsAndPublicContractNamesAreExact'
    'FlowExposesSequenceValidationAndBoundedRangeQueryMethods'
    'StatusDefinesExplicitValidationAndQueryOutcomes'
    'SequenceValidationIsImmutableAndRevisioned'
    'RangeQueryIsImmutableAndCrossesMultiCollectionSequenceSequenceSequenceBoundaries'
    'ResultContractsExposeExplicitSuccess'
    'ConstructorsRemainNonPublic'
    'SequenceAndCheckpointBoundsRemainExplicit'
    'ContractsRemainSynchronousAndSideEffectFree'
)) {
    if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Validates bounded ordered sequences of exact continuous multi-collection-sequence-sequence-sequence summaries'
    'bounded inclusive ranges across their validated multi-collection-sequence-sequence-sequence boundaries'
    'without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryFiles = @(
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelection.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryFlow.cs'
)

$hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryTexts = @()
foreach ($fileName in $hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryFiles) {
    $filePath = Join-Path (
        $hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryDirectory) $fileName
    if (-not (Test-Path -LiteralPath $filePath)) {
        throw (
            'Host recovery multi-collection-sequence-sequence-sequence-sequence range-summary file is missing: ' +
            $fileName)
    }

    $hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryTexts +=
        Get-Content -LiteralPath $filePath -Raw
}
$hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryText =
    $hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryTexts -join "`n"
$normalizedHostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryText =
    $hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryText -replace '\s+', ' '

foreach ($requiredToken in @(
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjectionIdKind'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionIdKind'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelection'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionResult'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryFlow'
    'ProjectSummary'
    'SelectPreviousMultiCollectionSequenceSequenceSequence'
    'SelectNextMultiCollectionSequenceSequenceSequence'
    'MaximumAdjacentMultiCollectionSequenceSequenceSequenceCount'
    'MultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjected'
    'PreviousAdjacentMultiCollectionSequenceSequenceSequenceSelected'
    'NextAdjacentMultiCollectionSequenceSequenceSequenceSelected'
    'StaleRangeRevision'
    'RangeSummaryProjectionTickRegressed'
    'StaleSummaryRevision'
    'AdjacentMultiCollectionSequenceSequenceSequenceSelectionTickRegressed'
    'TooManyAdjacentMultiCollectionSequenceSequenceSequences'
    'NoPreviousAdjacentMultiCollectionSequenceSequenceSequence'
    'NoNextAdjacentMultiCollectionSequenceSequenceSequence'
    'PreviousAdjacentMultiCollectionSequenceSequenceSequenceTooShort'
    'NextAdjacentMultiCollectionSequenceSequenceSequenceTooShort'
    'AdjacentMultiCollectionSequenceSequenceSequenceBoundaryMismatch'
    'Array.AsReadOnly(multiCollectionSequenceSequenceSequenceSummaries)'
    'Array.AsReadOnly(boundarySupersessions)'
    'MultiCollectionSequenceSequenceSequenceSummaries'
    'AdjacentBoundarySupersession'
    'SourceSequence'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery multi-collection-sequence-sequence-sequence-sequence range-summary token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery multi-collection-sequence-sequence-sequence-sequence range-summary contracts must not ' +
        'wait, schedule work, or read clocks.')
}
if ($hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery multi-collection-sequence-sequence-sequence-sequence range-summary contracts must not ' +
        'generate identities, perform transport, or persist data.')
}
if ($hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery multi-collection-sequence-sequence-sequence-sequence range-summary contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryAdjacentMultiCollectionSequenceSequenceSequenceTests.cs')
if (-not (Test-Path -LiteralPath $hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryTestPath)) {
    throw (
        'Host recovery multi-collection-sequence-sequence-sequence-sequence range-summary tests are missing.')
}

$hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryTestText =
    Get-Content -LiteralPath (
        $hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryTestPath) -Raw
$hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryFactCount = (
    [regex]::Matches(
        $hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryTestText,
        '\[Fact\]').Count)
if ($hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryFactCount -ne 9) {
    throw (
        'Host recovery multi-collection-sequence-sequence-sequence-sequence range-summary tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryTestText -match 'Skip\s*=') {
    throw (
        'Host recovery multi-collection-sequence-sequence-sequence-sequence range-summary tests must not be skipped.')
}

foreach ($requiredTest in @(
    'PublicContractsAreAvailable'
    'SummaryStatusDefinesExpectedOutcomes'
    'SummaryProjectionPreservesExactRangeAuthorityShape'
    'AdjacentSelectionPreservesExactMultiCollectionSequenceSequenceSequenceAuthorities'
    'FlowExposesBoundedProjectionAndSelectionMethods'
    'ResultContractsPreserveSourceAuthorities'
    'SummaryAndSelectionContractsExposeNoPublicSetters'
    'CollectionPropertiesRemainReadOnly'
    'ContractsRemainBoundedAndSideEffectFree'
)) {
    if ($hostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery multi-collection-sequence-sequence-sequence-sequence range-summary test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Projects compact multi-collection-sequence-sequence-sequence-sequence checkpoint-range summaries'
    'selects exact bounded adjacent multi-collection-sequence-sequence-sequences without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryMultiCollectionSequenceSequenceSequenceSequenceRangeSummaryText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery multi-collection-sequence-sequence-sequence-sequence range-summary boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionFiles = @(
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionIdKind.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationIdKind.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjection.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidation.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionResult.cs'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationResult.cs'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceFlow.cs'
)
foreach ($fileName in $hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionFiles) {
    $contractPath = Join-Path (
        $hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionDirectory) $fileName
    if (-not (Test-Path -LiteralPath $contractPath)) {
        throw (
            'Host recovery adjacent multi-collection-sequence projection contract is missing: ' +
            $fileName)
    }
}

$hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionText = (
    $hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionFiles |
        ForEach-Object {
            Get-Content -LiteralPath (
                Join-Path $hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionDirectory $_) -Raw
        } |
        Out-String)
$normalizedHostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionText =
    (($hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionText -replace
        '(?m)^\s*///\s*', ' ') -replace '\s+', ' ') -replace
        '\s*\.\s*', '.'

foreach ($requiredType in @(
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionIdKind'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationIdKind'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjection'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidation'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionResult'
    'HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationResult'
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceFlow'
)) {
    if ($hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionText -notmatch
        [regex]::Escape($requiredType)) {
        throw (
            'Host recovery adjacent multi-collection-sequence projection type is missing: ' +
            $requiredType)
    }
}

foreach ($requiredToken in @(
    'ProjectMultiCollectionSequenceSequenceSequence<TRequest, TState, TCompletion>'
    'ValidateContinuity<TRequest, TState, TCompletion>'
    'AdjacentMultiCollectionSequenceSequenceSequenceProjected'
    'MultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidated'
    'StaleSelectionRevision'
    'AdjacentMultiCollectionSequenceSequenceSequenceProjectionTickRegressed'
    'SelectionMultiCollectionSequenceSequenceSequenceSummaryMismatch'
    'SelectionBoundarySupersessionMismatch'
    'SelectionCheckpointMismatch'
    'SelectionSupersessionMismatch'
    'StaleRangeSummaryRevision'
    'StaleAdjacentMultiCollectionSequenceSequenceSequenceRevision'
    'ContinuityValidationTickRegressed'
    'RangeSummaryMismatch'
    'MultiCollectionSequenceSequenceSequenceRangeNotAdjacent'
    'CheckpointRangeNotAdjacent'
    'SupersessionBoundaryMismatch'
    'CheckpointBoundaryMismatch'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'MultiCollectionSequenceSequenceSequenceSummaries'
    'BoundarySupersessions'
    'AdjacentBoundarySupersession'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery adjacent multi-collection-sequence projection token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery adjacent multi-collection-sequence projection contracts must not ' +
        'wait, schedule work, or read clocks.')
}
if ($hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery adjacent multi-collection-sequence projection contracts must not ' +
        'generate identities, perform transport, or persist data.')
}
if ($hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery adjacent multi-collection-sequence projection contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionContinuityTests.cs')
if (-not (Test-Path -LiteralPath (
        $hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionTestPath))) {
    throw (
        'Host recovery adjacent multi-collection-sequence projection contract tests are missing.')
}

$hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionTestText =
    Get-Content -LiteralPath (
        $hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionTestPath) -Raw
$hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionFactCount = (
    [regex]::Matches(
        $hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionTestText,
        '\[Fact\]').Count)
if ($hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionFactCount -ne 9) {
    throw (
        'Host recovery adjacent multi-collection-sequence projection tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionTestText -match 'Skip\s*=') {
    throw (
        'Host recovery adjacent multi-collection-sequence projection tests must not be skipped.')
}

foreach ($requiredTest in @(
    'PublicContractsAreAvailable'
    'ProjectionStatusDefinesExpectedOutcomes'
    'AdjacentProjectionPreservesExactMultiCollectionSequenceAuthorityShape'
    'ContinuityValidationPreservesExactBoundaryAuthorityShape'
    'FlowExposesProjectionAndContinuityMethods'
    'ResultContractsPreserveSourceAuthorities'
    'ProjectionAndContinuityContractsExposeNoPublicSetters'
    'MaterializedEvidenceCollectionsRemainReadOnly'
    'ContractsRemainSynchronousAndSideEffectFree'
)) {
    if ($hostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery adjacent multi-collection-sequence projection test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Projects exact selected adjacent multi-collection-sequence-sequence-sequences'
    'summarized multi-collection-sequence-sequence-sequence-sequence checkpoint ranges'
    'without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery adjacent multi-collection-sequence projection boundary text is missing: ' +
            $documentationToken)
    }
}

$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryDirectory = Join-Path $root (
    'src\AI.Sandbox.Engine.Core\HostRuntime')
$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryFiles = @(
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryProjectionIdKind.cs'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryIdKind.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryStatus.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryProjection.cs'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceSequenceCheckpointRangeQuery.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryProjectionResult.cs'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryResult.cs'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryFlow.cs'
)

$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryTexts = @()
foreach ($fileName in $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryFiles) {
    $filePath = Join-Path (
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryDirectory) $fileName
    if (-not (Test-Path -LiteralPath $filePath)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence summary contract file is missing: ' +
            $fileName)
    }

    $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryTexts +=
        Get-Content -LiteralPath $filePath -Raw
}
$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryText =
    $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryTexts -join "`n"
$normalizedHostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryText =
    $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryText -replace '\s+', ' '

foreach ($requiredToken in @(
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryProjectionIdKind'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryIdKind'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryStatus'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryProjection'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceSequenceCheckpointRangeQuery'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryProjectionResult'
    'HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryResult'
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryFlow'
    'ProjectSummary'
    'QueryRange'
    'MaximumCheckpointCount'
    'ContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryProjected'
    'CrossMultiCollectionSequenceSequenceSequenceCheckpointRangeQueried'
    'StaleContinuityRevision'
    'MultiCollectionSequenceSequenceSequenceSequenceSummaryProjectionTickRegressed'
    'ContinuitySourceMismatch'
    'MultiCollectionSequenceSequenceSequenceRangeNotContinuous'
    'MultiCollectionSequenceSequenceSequenceCheckpointRangeNotContinuous'
    'MultiCollectionSequenceSequenceSequenceSupersessionMismatch'
    'MultiCollectionSequenceSequenceSequenceCheckpointMismatch'
    'StaleMultiCollectionSequenceSequenceSequenceSequenceSummaryRevision'
    'CrossMultiCollectionSequenceSequenceSequenceRangeQueryTickRegressed'
    'RangeStartNotFound'
    'RangeEndNotFound'
    'RangeOrderInvalid'
    'RangeDoesNotCrossMultiCollectionSequenceSequenceSequenceBoundary'
    'RangeTooLarge'
    'RangeSupersessionMismatch'
    'Array.AsReadOnly(checkpoints)'
    'Array.AsReadOnly(supersessions)'
    'ConnectingSupersession'
    'ConnectingPriorCheckpointIndex'
    'ConnectingSuccessorCheckpointIndex'
    'ReferenceEquals'
)) {
    if ($normalizedHostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryText -notmatch
        [regex]::Escape($requiredToken)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence summary token is missing: ' +
            $requiredToken)
    }
}

if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryText -match
    '\b(Task\.Delay|Task\.Run|Thread\.Sleep|Timer|Stopwatch|DateTime|DateTimeOffset)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence summary contracts must not ' +
        'wait, schedule work, or read clocks.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryText -match
    '\b(Guid\.(NewGuid|CreateVersion7)|HttpClient|FileStream|SqlConnection)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence summary contracts must not ' +
        'generate identities, perform transport, or persist data.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryText -match
    '\b(public\s+set\s*;|TryApply|ExecuteCommand|RuntimeOrchestrator|CommandProcessor)\b') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence summary contracts must remain ' +
        'immutable and advisory-only.')
}

$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryTestPath = Join-Path $root (
    'tests\AI.Sandbox.Engine.Core.Tests\' +
    'HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryCrossMultiCollectionSequenceSequenceSequenceRangeTests.cs')
if (-not (Test-Path -LiteralPath (
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryTestPath))) {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence summary contract tests are missing.')
}

$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryTestText =
    Get-Content -LiteralPath (
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryTestPath) -Raw
$hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryFactCount = (
    [regex]::Matches(
        $hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryTestText,
        '\[Xunit\.Fact\]').Count)
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryFactCount -ne 9) {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence summary tests must contain ' +
        'exactly 9 facts.')
}
if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryTestText -match 'Skip\s*=') {
    throw (
        'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence summary tests must not be skipped.')
}

foreach ($requiredTest in @(
    'IdsAndPublicContractNamesAreExact'
    'FlowExposesProjectionAndBoundedRangeQueryMethods'
    'StatusDefinesExplicitProjectionAndQueryOutcomes'
    'SummaryProjectionIsImmutableAndRevisioned'
    'RangeQueryIsImmutableAndCrossesMultiCollectionSequenceSequenceSequenceBoundary'
    'ResultContractsExposeExplicitSuccess'
    'ConstructorsRemainNonPublic'
    'MaximumCheckpointCountRemainsBounded'
    'ContractsRemainSynchronousAndSideEffectFree'
)) {
    if ($hostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryTestText -notmatch
        [regex]::Escape($requiredTest)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence summary test is missing: ' +
            $requiredTest)
    }
}

foreach ($documentationToken in @(
    'Projects compact summaries over one exact continuous recovery multi-collection-sequence-sequence-sequence-sequence'
    'bounded inclusive ranges crossing their shared boundary'
    'without discovery, reordering'
    'history mutation'
    'transport'
    'execution'
)) {
    if ($normalizedHostRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryText -notmatch
        [regex]::Escape($documentationToken)) {
        throw (
            'Host recovery continuous multi-collection-sequence-sequence-sequence-sequence summary boundary text is missing: ' +
            $documentationToken)
    }
}

Write-Host 'Repository verification passed.' -ForegroundColor Green
