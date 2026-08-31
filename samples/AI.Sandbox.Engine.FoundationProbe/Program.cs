namespace AI.Sandbox.Engine.FoundationProbe;

internal static class Program
{
    private readonly record struct CounterComponent(int Value) :
        global::AI.Sandbox.Engine.Core.Components.IComponent;

    private sealed record ProbeWorldState(
        global::AI.Sandbox.Engine.Core.Entities.EntityRegistry Entities,
        global::AI.Sandbox.Engine.Core.Components.ComponentRegistry Components) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    private readonly record struct ProbeCompleted(
        ulong SimulationTick,
        ulong WorldStateVersion,
        string Checksum) :
        global::AI.Sandbox.Engine.Core.Events.IEngineEvent;

    private sealed record ProbeRun(
        global::AI.Sandbox.Engine.Core.Persistence.WorldSnapshotDocument Document,
        ProbeWorldState State);

    private static async Task<int> Main(string[] args)
    {
        if (!TryReadTickCount(args, out var tickCount))
        {
            Console.Error.WriteLine(
                "Usage: FoundationProbe [positive-tick-count]");
            return 2;
        }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var uninterrupted = RunScenario(
                tickCount,
                checkpointTick: null);
            var resumed = RunScenario(
                tickCount,
                checkpointTick: tickCount / 2);

            EnsureEquivalent(uninterrupted, resumed, tickCount);

            var eventCalls = new List<string>();
            var dispatcher =
                new global::AI.Sandbox.Engine.Core.Events
                    .EventDispatcherBuilder()
                    .Add<ProbeCompleted>(
                        new RecordingHandler("first", eventCalls))
                    .Add<ProbeCompleted>(
                        new RecordingHandler("second", eventCalls))
                    .Build();
            var envelope = global::AI.Sandbox.Engine.Core.Events
                .EventEnvelope<ProbeCompleted>.Create(
                    CreateEventId(),
                    sequence: 1,
                    simulationTick: resumed.Document.SimulationTick,
                    new ProbeCompleted(
                        resumed.Document.SimulationTick,
                        resumed.Document.WorldStateVersion.Value,
                        resumed.Document.Checksum.Value));

            await dispatcher.DispatchAsync(envelope);

            if (!eventCalls.SequenceEqual(
                new[] { "first", "second" },
                StringComparer.Ordinal))
            {
                throw new InvalidOperationException(
                    "Post-commit event handlers did not execute in order.");
            }

            var hostRuntime = HostRuntimeProbe.Run();
            var hostRuntimeRetry = HostRuntimeProbe.RunRetry();
            var hostRuntimeAbandonment =
                HostRuntimeProbe.RunAbandonment();
            var conversation = ConversationProbe.Run();
            var command = CommandProbe.Run();
            var perception = PerceptionProbe.Run();
            var social = SocialProbe.Run();
            var prompting = PromptingProbe.Run();
            var promptComposition =
                PromptingProbe.RunComposition();
            var productPipeline =
                await ProductPipelineProbe.RunAsync();

            stopwatch.Stop();

            Console.WriteLine("FOUNDATION_PROBE_OK");
            Console.WriteLine(
                $"host_runtime_admission={hostRuntime.AdmissionStatus}");
            Console.WriteLine(
                $"host_runtime_selection={hostRuntime.SelectionStatus}");
            Console.WriteLine(
                $"host_runtime_acknowledgement={hostRuntime.AcknowledgementStatus}");
            Console.WriteLine(
                $"host_runtime_settlement={hostRuntime.SettlementStatus}");
            Console.WriteLine(
                $"host_runtime_request_state={hostRuntime.RequestState}");
            Console.WriteLine(
                $"host_runtime_lease_state={hostRuntime.LeaseState}");
            Console.WriteLine(
                $"host_runtime_queued_count={hostRuntime.QueuedCount}");
            Console.WriteLine(
                $"host_runtime_retry_decision={hostRuntimeRetry.RetryDecisionStatus}");
            Console.WriteLine(
                $"host_runtime_requeue={hostRuntimeRetry.RequeueStatus}");
            Console.WriteLine(
                $"host_runtime_requeued_request_state={hostRuntimeRetry.RequeuedRequestState}");
            Console.WriteLine(
                $"host_runtime_requeued_queue_count={hostRuntimeRetry.RequeuedQueueCount}");
            Console.WriteLine(
                $"host_runtime_deadletter_decision={hostRuntimeRetry.DeadLetterDecisionStatus}");
            Console.WriteLine(
                $"host_runtime_deadletter={hostRuntimeRetry.DeadLetterStatus}");
            Console.WriteLine(
                $"host_runtime_deadletter_kind={hostRuntimeRetry.DeadLetterKind}");
            Console.WriteLine(
                $"host_runtime_cancellation_abandonment={hostRuntimeAbandonment.CancellationStatus}");
            Console.WriteLine(
                $"host_runtime_cancellation_kind={hostRuntimeAbandonment.CancellationKind}");
            Console.WriteLine(
                $"host_runtime_cancelled_request_state={hostRuntimeAbandonment.CancelledRequestState}");
            Console.WriteLine(
                $"host_runtime_released_lease_state={hostRuntimeAbandonment.ReleasedLeaseState}");
            Console.WriteLine(
                $"host_runtime_lease_expiry_abandonment={hostRuntimeAbandonment.LeaseExpiryStatus}");
            Console.WriteLine(
                $"host_runtime_lease_expiry_kind={hostRuntimeAbandonment.LeaseExpiryKind}");
            Console.WriteLine(
                $"host_runtime_failed_request_state={hostRuntimeAbandonment.FailedRequestState}");
            Console.WriteLine(
                $"host_runtime_expired_lease_state={hostRuntimeAbandonment.ExpiredLeaseState}");
            Console.WriteLine(
                $"conversation_status={conversation.Status}");
            Console.WriteLine(
                $"conversation_resolver_invoked={conversation.ResolverWasInvoked}");
            Console.WriteLine(
                $"conversation_stable_decision={conversation.HasStableDecision}");
            Console.WriteLine(
                $"conversation_resolver_call_count={conversation.ResolverCallCount}");
            Console.WriteLine(
                $"conversation_initial_revision={conversation.ConversationRevision}");
            Console.WriteLine(
                $"conversation_exact_audience={conversation.ExactAudience}");
            Console.WriteLine(
                $"conversation_world_authority_unchanged={conversation.WorldAuthorityUnchanged}");
            Console.WriteLine(
                $"dialogue_status={conversation.DialogueStatus}");
            Console.WriteLine(
                $"dialogue_orchestrator_invoked={conversation.DialogueOrchestratorWasInvoked}");
            Console.WriteLine(
                $"dialogue_stable_decision={conversation.DialogueHasStableDecision}");
            Console.WriteLine(
                $"dialogue_orchestrator_call_count={conversation.DialogueOrchestratorCallCount}");
            Console.WriteLine(
                $"dialogue_was_continued={conversation.DialogueWasContinued}");
            Console.WriteLine(
                $"dialogue_directive_name={conversation.DialogueDirectiveName}");
            Console.WriteLine(
                $"dialogue_world_authority_unchanged={conversation.DialogueWorldAuthorityUnchanged}");
            Console.WriteLine(
                $"command_status={command.Status}");
            Console.WriteLine(
                $"command_was_applied={command.WasApplied}");
            Console.WriteLine(
                $"command_handler_evaluated={command.HandlerWasEvaluated}");
            Console.WriteLine(
                $"command_handler_evaluation_count={command.HandlerEvaluationCount}");
            Console.WriteLine(
                $"command_before_value={command.BeforeValue}");
            Console.WriteLine(
                $"command_after_value={command.AfterValue}");
            Console.WriteLine(
                $"command_before_version={command.BeforeVersion}");
            Console.WriteLine(
                $"command_after_version={command.AfterVersion}");
            Console.WriteLine(
                $"command_before_tick={command.BeforeSimulationTick}");
            Console.WriteLine(
                $"command_after_tick={command.AfterSimulationTick}");
            Console.WriteLine(
                $"command_version_advanced_once={command.VersionAdvancedExactlyOnce}");
            Console.WriteLine(
                $"command_tick_preserved={command.SimulationTickPreserved}");
            Console.WriteLine(
                $"runtime_invocation_status={command.RuntimeInvocationStatus}");
            Console.WriteLine(
                $"runtime_was_invoked={command.RuntimeWasInvoked}");
            Console.WriteLine(
                $"runtime_was_committed={command.RuntimeWasCommitted}");
            Console.WriteLine(
                $"runtime_command_status={command.RuntimeCommandStatus}");
            Console.WriteLine(
                $"runtime_commit_fact_valid={command.RuntimeCommitFactValid}");
            Console.WriteLine(
                $"runtime_auto_dispatch_absent={command.RuntimeAutoDispatchAbsent}");
            Console.WriteLine(
                $"perception_status={perception.Status}");
            Console.WriteLine(
                $"perception_evaluator_executed={perception.EvaluatorWasExecuted}");
            Console.WriteLine(
                $"perception_evaluator_evaluation_count={perception.EvaluatorEvaluationCount}");
            Console.WriteLine(
                $"perception_observation_present={perception.ObservationPresent}");
            Console.WriteLine(
                $"perception_observation_identity_valid={perception.ObservationIdentityValid}");
            Console.WriteLine(
                $"perception_observation_snapshot_metadata_valid={perception.ObservationSnapshotMetadataValid}");
            Console.WriteLine(
                $"perception_confidence_basis_points={perception.ConfidenceBasisPoints}");
            Console.WriteLine(
                $"perception_signal={perception.SignalText}");
            Console.WriteLine(
                $"perception_snapshot_reference_preserved={perception.SnapshotReferencePreserved}");
            Console.WriteLine(
                $"perception_world_authority_unchanged={perception.WorldAuthorityUnchanged}");
            Console.WriteLine(
                $"perception_before_value={perception.BeforeValue}");
            Console.WriteLine(
                $"perception_after_value={perception.AfterValue}");
            Console.WriteLine(
                $"perception_before_version={perception.BeforeVersion}");
            Console.WriteLine(
                $"perception_after_version={perception.AfterVersion}");
            Console.WriteLine(
                $"perception_before_tick={perception.BeforeSimulationTick}");
            Console.WriteLine(
                $"perception_after_tick={perception.AfterSimulationTick}");
            Console.WriteLine(
                $"social_turn_status={social.Status}");
            Console.WriteLine(
                $"social_turn_coordinator_invoked={social.CoordinatorWasInvoked}");
            Console.WriteLine(
                $"social_turn_coordinator_call_count={social.CoordinatorCallCount}");
            Console.WriteLine(
                $"social_turn_stable_decision={social.StableDecision}");
            Console.WriteLine(
                $"social_turn_decision_granted={social.DecisionGranted}");
            Console.WriteLine(
                $"social_turn_selected_proposal_present={social.SelectedProposalPresent}");
            Console.WriteLine(
                $"social_turn_selected_proposal_identity_valid={social.SelectedProposalIdentityValid}");
            Console.WriteLine(
                $"social_turn_two_proposal_request={social.TwoProposalRequest}");
            Console.WriteLine(
                $"social_turn_deterministic_proposal_order_valid={social.DeterministicProposalOrderValid}");
            Console.WriteLine(
                $"social_turn_request_authority_metadata_valid={social.RequestAuthorityMetadataValid}");
            Console.WriteLine(
                $"social_turn_conversation_revision_preserved={social.ConversationRevisionPreserved}");
            Console.WriteLine(
                $"social_turn_world_authority_unchanged={social.WorldAuthorityUnchanged}");
            Console.WriteLine(
                $"social_turn_before_value={social.BeforeValue}");
            Console.WriteLine(
                $"social_turn_after_value={social.AfterValue}");
            Console.WriteLine(
                $"social_turn_before_version={social.BeforeVersion}");
            Console.WriteLine(
                $"social_turn_after_version={social.AfterVersion}");
            Console.WriteLine(
                $"social_turn_before_tick={social.BeforeSimulationTick}");
            Console.WriteLine(
                $"social_turn_after_tick={social.AfterSimulationTick}");
            Console.WriteLine(
                $"prompt_budget_status={prompting.Status}");
            Console.WriteLine(
                $"prompt_budget_allocation_call_count={prompting.AllocationCallCount}");
            Console.WriteLine(
                $"prompt_budget_input_candidate_count={prompting.InputCandidateCount}");
            Console.WriteLine(
                $"prompt_budget_selected_candidate_count={prompting.SelectedCandidateCount}");
            Console.WriteLine(
                $"prompt_budget_required_units={prompting.RequiredUnits}");
            Console.WriteLine(
                $"prompt_budget_used_units={prompting.UsedUnits}");
            Console.WriteLine(
                $"prompt_budget_remaining_units={prompting.RemainingUnits}");
            Console.WriteLine(
                $"prompt_budget_required_selected={prompting.RequiredSelected}");
            Console.WriteLine(
                $"prompt_budget_highest_priority_optional_selected={prompting.HighestPriorityOptionalSelected}");
            Console.WriteLine(
                $"prompt_budget_lower_priority_optional_skipped={prompting.LowerPriorityOptionalSkipped}");
            Console.WriteLine(
                $"prompt_budget_deterministic_order_valid={prompting.DeterministicOrderValid}");
            Console.WriteLine(
                $"prompt_budget_selected_identity_preserved={prompting.SelectedIdentityPreserved}");
            Console.WriteLine(
                $"prompt_budget_input_candidate_identity_preserved={prompting.InputCandidateIdentityPreserved}");
            Console.WriteLine(
                $"prompt_budget_input_payload_values_preserved={prompting.InputPayloadValuesPreserved}");
            Console.WriteLine(
                $"prompt_budget_owner_scope_preserved={prompting.OwnerScopePreserved}");
            Console.WriteLine(
                $"prompt_budget_world_scope_preserved={prompting.WorldScopePreserved}");
            Console.WriteLine(
                $"prompt_composition_status={promptComposition.Status}");
            Console.WriteLine(
                $"prompt_composition_processor_call_count={promptComposition.ProcessorCallCount}");
            Console.WriteLine(
                $"prompt_composition_was_composed={promptComposition.WasComposed}");
            Console.WriteLine(
                $"prompt_composition_composer_invoked={promptComposition.ComposerWasInvoked}");
            Console.WriteLine(
                $"prompt_composition_composer_call_count={promptComposition.ComposerCallCount}");
            Console.WriteLine(
                $"prompt_composition_composer_selected_count={promptComposition.ComposerSelectedCount}");
            Console.WriteLine(
                $"prompt_composition_composer_context_valid={promptComposition.ComposerContextValid}");
            Console.WriteLine(
                $"prompt_composition_decision_status={promptComposition.DecisionStatus}");
            Console.WriteLine(
                $"prompt_composition_budget_status={promptComposition.BudgetStatus}");
            Console.WriteLine(
                $"prompt_composition_budget_selected_candidate_count={promptComposition.BudgetSelectedCandidateCount}");
            Console.WriteLine(
                $"prompt_composition_required_units={promptComposition.RequiredUnits}");
            Console.WriteLine(
                $"prompt_composition_used_units={promptComposition.UsedUnits}");
            Console.WriteLine(
                $"prompt_composition_remaining_units={promptComposition.RemainingUnits}");
            Console.WriteLine(
                $"prompt_composition_request_identity_preserved={promptComposition.RequestIdentityPreserved}");
            Console.WriteLine(
                $"prompt_composition_document_identity_preserved={promptComposition.DocumentIdentityPreserved}");
            Console.WriteLine(
                $"prompt_composition_document_scope_valid={promptComposition.DocumentScopeValid}");
            Console.WriteLine(
                $"prompt_composition_document_payload_valid={promptComposition.DocumentPayloadValid}");
            Console.WriteLine(
                $"prompt_composition_world_authority_unchanged={promptComposition.WorldAuthorityUnchanged}");
            Console.WriteLine(
                $"core_product_pipeline={productPipeline.Status}");
            Console.WriteLine(
                $"core_product_context={productPipeline.ContextStatus}");
            Console.WriteLine(
                $"core_product_prompt={productPipeline.PromptStatus}");
            Console.WriteLine(
                $"core_product_model={productPipeline.ModelStatus}");
            Console.WriteLine(
                $"core_product_structured={productPipeline.StructuredStatus}");
            Console.WriteLine(
                $"core_product_action={productPipeline.ActionStatus}");
            Console.WriteLine(
                $"core_product_runtime={productPipeline.RuntimeStatus}");
            Console.WriteLine(
                $"core_product_authority_unchanged_before_command={productPipeline.AuthorityUnchangedBeforeCommand}");
            Console.WriteLine(
                $"core_product_value_transition={productPipeline.BeforeValue}->{productPipeline.AfterValue}");
            Console.WriteLine(
                $"core_product_version_transition={productPipeline.BeforeVersion}->{productPipeline.AfterVersion}");
            Console.WriteLine(
                $"core_product_reply={productPipeline.Reply}");
            Console.WriteLine("CORE_PRODUCT_PIPELINE_OK");
            Console.WriteLine(
                $"prompt_composition_before_value={promptComposition.BeforeValue}");
            Console.WriteLine(
                $"prompt_composition_after_value={promptComposition.AfterValue}");
            Console.WriteLine(
                $"prompt_composition_before_version={promptComposition.BeforeVersion}");
            Console.WriteLine(
                $"prompt_composition_after_version={promptComposition.AfterVersion}");
            Console.WriteLine(
                $"prompt_composition_before_tick={promptComposition.BeforeSimulationTick}");
            Console.WriteLine(
                $"prompt_composition_after_tick={promptComposition.AfterSimulationTick}");
            Console.WriteLine($"ticks={tickCount}");
            Console.WriteLine(
                $"version={resumed.Document.WorldStateVersion.Value}");
            Console.WriteLine(
                $"checksum={resumed.Document.Checksum.Value}");
            Console.WriteLine($"elapsed_ms={stopwatch.ElapsedMilliseconds}");
            Console.WriteLine(
                $"known_entities={resumed.State.Entities.KnownCount}");
            Console.WriteLine(
                $"active_entities={resumed.State.Entities.ActiveCount}");
            Console.WriteLine(
                $"components={resumed.State.Components.ComponentCount}");
            Console.WriteLine(
                $"events={string.Join(',', eventCalls)}");

            return 0;
        }
        catch (Exception exception)
        {
            stopwatch.Stop();
            Console.Error.WriteLine("FOUNDATION_PROBE_FAILED");
            Console.Error.WriteLine(exception);
            return 1;
        }
    }

    private static bool TryReadTickCount(
        string[] args,
        out int tickCount)
    {
        tickCount = 5_000;

        if (args.Length == 0)
        {
            return true;
        }

        return args.Length == 1 &&
            int.TryParse(
                args[0],
                System.Globalization.NumberStyles.None,
                System.Globalization.CultureInfo.InvariantCulture,
                out tickCount) &&
            tickCount > 1;
    }

    private static ProbeRun RunScenario(
        int totalTicks,
        int? checkpointTick)
    {
        var codec = new ProbeCodec();
        var persistence =
            new global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateSnapshotPersistence<ProbeWorldState>(codec);
        var manager = CreateManager();
        var scheduler = CreateScheduler(manager);

        if (checkpointTick is null)
        {
            RunTicks(scheduler, totalTicks);
        }
        else
        {
            RunTicks(scheduler, checkpointTick.Value);

            var checkpoint = persistence.Capture(manager.Read());
            var restored = persistence.Restore(checkpoint);
            if (!restored.WasRestored || restored.Snapshot is null)
            {
                throw new InvalidOperationException(
                    $"Probe restore failed: {restored.Status} " +
                    restored.FailureReason);
            }

            manager = global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<ProbeWorldState>.Restore(
                    restored.Snapshot);
            scheduler = CreateScheduler(manager);
            RunTicks(
                scheduler,
                totalTicks - checkpointTick.Value);
        }

        var finalSnapshot = manager.Read();
        var finalDocument = persistence.Capture(finalSnapshot);

        return new ProbeRun(
            finalDocument,
            finalSnapshot.State);
    }

    private static void RunTicks(
        global::AI.Sandbox.Engine.Core.Simulation
            .SimulationScheduler<ProbeWorldState> scheduler,
        int count)
    {
        for (var index = 0; index < count; index++)
        {
            var result = scheduler.RunNextTick();
            if (!result.WasApplied)
            {
                throw new InvalidOperationException(
                    $"Probe tick failed: {result.Status}");
            }
        }
    }

    private static void EnsureEquivalent(
        ProbeRun uninterrupted,
        ProbeRun resumed,
        int tickCount)
    {
        if (uninterrupted.Document.WorldId != resumed.Document.WorldId ||
            uninterrupted.Document.WorldStateVersion !=
                resumed.Document.WorldStateVersion ||
            uninterrupted.Document.SimulationTick !=
                resumed.Document.SimulationTick ||
            uninterrupted.Document.Checksum != resumed.Document.Checksum ||
            !uninterrupted.Document.Payload.ContentEquals(
                resumed.Document.Payload))
        {
            throw new InvalidOperationException(
                "Uninterrupted and save/restore execution diverged.");
        }

        if (resumed.Document.SimulationTick != (ulong)tickCount ||
            resumed.Document.WorldStateVersion.Value != (ulong)tickCount)
        {
            throw new InvalidOperationException(
                "Probe did not advance version and tick exactly once per step.");
        }

        if (!resumed.State.Components.TryGet<CounterComponent>(
            CreateEntityId(),
            out var counter) ||
            counter.Value != tickCount)
        {
            throw new InvalidOperationException(
                "Final component value does not match tick count.");
        }

        if (!resumed.State.Components.IsConsistentWith(
            resumed.State.Entities))
        {
            throw new InvalidOperationException(
                "Final Entity and Component registries are inconsistent.");
        }
    }

    private static global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<ProbeWorldState> CreateManager()
    {
        var entityId = CreateEntityId();
        var entities =
            global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
                .FromActiveEntities(new[] { entityId });
        var components =
            new global::AI.Sandbox.Engine.Core.Components
                .ComponentRegistryBuilder(entities)
                .Add(entityId, new CounterComponent(0))
                .Build();
        var state = new ProbeWorldState(entities, components);

        return global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateManager<ProbeWorldState>.Create(
                CreateWorldId(),
                state);
    }

    private static global::AI.Sandbox.Engine.Core.Simulation
        .SimulationScheduler<ProbeWorldState> CreateScheduler(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<ProbeWorldState> manager)
    {
        return new global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSchedulerBuilder<ProbeWorldState>()
            .Add(CreateSystemId(), new IncrementSystem())
            .Build(manager);
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> CreateWorldId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019b0000-0000-7000-8000-000000000600");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> CreateEntityId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(
                "019b0000-0000-7000-8400-000000000001");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Simulation.SimulationSystemIdKind>
        CreateSystemId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemIdKind>.Parse(
                    "019b0000-0000-7000-8500-000000000001");
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Events.EventIdKind> CreateEventId()
    {
        return global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Events.EventIdKind>.Parse(
                "019b0000-0000-7000-8600-000000000001");
    }

    private sealed class IncrementSystem :
        global::AI.Sandbox.Engine.Core.Simulation
            .ISimulationSystem<ProbeWorldState>
    {
        public global::AI.Sandbox.Engine.Core.Simulation
            .SimulationSystemDecision<ProbeWorldState> Execute(
                global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemContext<ProbeWorldState> context)
        {
            var entityId = CreateEntityId();

            if (!context.State.Components.TryGet<CounterComponent>(
                entityId,
                out var current))
            {
                return global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemDecision<ProbeWorldState>.Reject(
                        "Counter component is missing.");
            }

            var mutation = context.State.Components.Set(
                context.State.Entities,
                entityId,
                new CounterComponent(current.Value + 1));

            if (!mutation.WasApplied)
            {
                return global::AI.Sandbox.Engine.Core.Simulation
                    .SimulationSystemDecision<ProbeWorldState>.Reject(
                        mutation.Status.ToString());
            }

            return global::AI.Sandbox.Engine.Core.Simulation
                .SimulationSystemDecision<ProbeWorldState>.Update(
                    context.State with
                    {
                        Components = mutation.Registry,
                    });
        }
    }

    private sealed class ProbeCodec :
        global::AI.Sandbox.Engine.Core.Persistence
            .IWorldStateSnapshotCodec<ProbeWorldState>
    {
        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaId SchemaId { get; } =
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaId.Parse("foundation.probe");

        public global::AI.Sandbox.Engine.Core.Persistence
            .PersistenceSchemaVersion CurrentSchemaVersion { get; } =
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion.From(1);

        public bool CanDecode(
            global::AI.Sandbox.Engine.Core.Persistence
                .PersistenceSchemaVersion version)
        {
            return version == CurrentSchemaVersion;
        }

        public global::AI.Sandbox.Engine.Core.Persistence
            .SnapshotPayload Encode(ProbeWorldState state)
        {
            var entityId = CreateEntityId();

            if (state.Entities.GetLifecycleStatus(entityId) !=
                    global::AI.Sandbox.Engine.Core.Entities
                        .EntityLifecycleStatus.Active ||
                !state.Components.TryGet<CounterComponent>(
                    entityId,
                    out var counter) ||
                !state.Components.IsConsistentWith(state.Entities))
            {
                throw new InvalidOperationException(
                    "Probe state violates persistence invariants.");
            }

            var text = counter.Value.ToString(
                System.Globalization.CultureInfo.InvariantCulture);

            return global::AI.Sandbox.Engine.Core.Persistence
                .SnapshotPayload.From(
                    System.Text.Encoding.UTF8.GetBytes(text));
        }

        public global::AI.Sandbox.Engine.Core.Persistence
            .WorldStateDecodeDecision<ProbeWorldState> Decode(
                global::AI.Sandbox.Engine.Core.Persistence
                    .PersistenceSchemaVersion version,
                global::AI.Sandbox.Engine.Core.Persistence
                    .SnapshotPayload payload)
        {
            if (version != CurrentSchemaVersion)
            {
                return global::AI.Sandbox.Engine.Core.Persistence
                    .WorldStateDecodeDecision<ProbeWorldState>.Reject(
                        "Unsupported schema version.");
            }

            var text = System.Text.Encoding.UTF8.GetString(
                payload.ToArray());
            if (!int.TryParse(
                text,
                System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture,
                out var value) ||
                value < 0)
            {
                return global::AI.Sandbox.Engine.Core.Persistence
                    .WorldStateDecodeDecision<ProbeWorldState>.Reject(
                        "Invalid counter payload.");
            }

            var entityId = CreateEntityId();
            var entities =
                global::AI.Sandbox.Engine.Core.Entities.EntityRegistry
                    .FromActiveEntities(new[] { entityId });
            var components =
                new global::AI.Sandbox.Engine.Core.Components
                    .ComponentRegistryBuilder(entities)
                    .Add(entityId, new CounterComponent(value))
                    .Build();

            return global::AI.Sandbox.Engine.Core.Persistence
                .WorldStateDecodeDecision<ProbeWorldState>.Accept(
                    new ProbeWorldState(entities, components));
        }
    }

    private sealed class RecordingHandler :
        global::AI.Sandbox.Engine.Core.Events.IEventHandler<ProbeCompleted>
    {
        private readonly string name;
        private readonly List<string> calls;

        public RecordingHandler(
            string name,
            List<string> calls)
        {
            this.name = name;
            this.calls = calls;
        }

        public ValueTask HandleAsync(
            global::AI.Sandbox.Engine.Core.Events
                .EventEnvelope<ProbeCompleted> envelope,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (envelope.Payload.SimulationTick == 0 ||
                envelope.Payload.WorldStateVersion == 0 ||
                string.IsNullOrWhiteSpace(envelope.Payload.Checksum))
            {
                throw new InvalidOperationException(
                    "Probe event metadata is incomplete.");
            }

            calls.Add(name);
            return ValueTask.CompletedTask;
        }
    }
}
