namespace AI.Sandbox.Engine.Core.Dialogue;

/// <summary>
/// Validates authority and conversation correlation around exactly one pure
/// dialogue-orchestrator invocation.
/// </summary>
/// <typeparam name="TState">The authoritative world-state type.</typeparam>
/// <typeparam name="TInput">The exact input payload type.</typeparam>
/// <typeparam name="TTopic">The exact conversation-topic payload type.</typeparam>
/// <typeparam name="TDirective">The exact directive payload type.</typeparam>
/// <typeparam name="TCompletion">The exact completion payload type.</typeparam>
public sealed class DialogueOrchestrationProcessor<
    TState,
    TInput,
    TTopic,
    TDirective,
    TCompletion>
    where TState : class,
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TInput : IDialogueInput
    where TTopic : global::AI.Sandbox.Engine.Core.Conversation.IConversationTopic
    where TDirective : IDialogueDirective
    where TCompletion : IDialogueCompletion
{
    private readonly global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<TState> worldStateManager;
    private readonly IDialogueOrchestrator<
        TState,
        TInput,
        TTopic,
        TDirective,
        TCompletion> orchestrator;
    private readonly global::AI.Sandbox.Engine.Core.Identifiers.Id<
        DialogueOrchestratorIdKind> orchestratorId;

    private DialogueOrchestrationProcessor(
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateManager<TState>
            worldStateManager,
        IDialogueOrchestrator<
            TState,
            TInput,
            TTopic,
            TDirective,
            TCompletion> orchestrator)
    {
        this.worldStateManager = worldStateManager;
        this.orchestrator = orchestrator;
        orchestratorId = orchestrator.OrchestratorId;
    }

    /// <summary>
    /// Creates one processor around current authority and one exact orchestrator.
    /// </summary>
    /// <param name="worldStateManager">The authoritative state manager.</param>
    /// <param name="orchestrator">The pure orchestrator invoked at most once.</param>
    /// <returns>The configured processor.</returns>
    public static DialogueOrchestrationProcessor<
        TState,
        TInput,
        TTopic,
        TDirective,
        TCompletion> Create(
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateManager<TState>
            worldStateManager,
        IDialogueOrchestrator<
            TState,
            TInput,
            TTopic,
            TDirective,
            TCompletion> orchestrator)
    {
        ArgumentNullException.ThrowIfNull(worldStateManager);
        ArgumentNullException.ThrowIfNull(orchestrator);

        DialogueTypePolicy.EnsureExactType(
            typeof(TInput),
            typeof(IDialogueInput),
            "dialogue input");
        DialogueTypePolicy.EnsureExactType(
            typeof(TTopic),
            typeof(global::AI.Sandbox.Engine.Core.Conversation
                .IConversationTopic),
            "conversation topic");
        DialogueTypePolicy.EnsureExactType(
            typeof(TDirective),
            typeof(IDialogueDirective),
            "dialogue directive");
        DialogueTypePolicy.EnsureExactType(
            typeof(TCompletion),
            typeof(IDialogueCompletion),
            "dialogue completion");

        if (orchestrator.OrchestratorId.IsEmpty)
        {
            throw new ArgumentException(
                "Dialogue orchestrators must expose a non-empty ID.",
                nameof(orchestrator));
        }

        return new DialogueOrchestrationProcessor<
            TState,
            TInput,
            TTopic,
            TDirective,
            TCompletion>(worldStateManager, orchestrator);
    }

    /// <summary>
    /// Processes one immutable request without executing the returned directive.
    /// </summary>
    /// <param name="request">The immutable exchange request.</param>
    /// <param name="conversation">The immutable current conversation.</param>
    /// <returns>One explicit validated result.</returns>
    public DialogueOrchestrationResult<TDirective, TCompletion> Process(
        DialogueOrchestrationRequestEnvelope<TInput> request,
        global::AI.Sandbox.Engine.Core.Conversation.ConversationState<TTopic>
            conversation)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(conversation);

        var before = worldStateManager.Read();
        var preflightStatus = ValidatePreflight(before, request, conversation);
        if (preflightStatus is not null)
        {
            return Result(preflightStatus.Value, false, false, null);
        }

        var context = new DialogueOrchestrationContext<
            TState,
            TInput,
            TTopic>(before, request, conversation);

        var decision = orchestrator.Decide(context) ??
            throw new InvalidOperationException(
                "Dialogue orchestrators must return a decision.");

        var after = worldStateManager.Read();
        if (after.Version != before.Version ||
            after.SimulationTick != before.SimulationTick)
        {
            return Result(
                DialogueOrchestrationStatus.AuthorityChanged,
                true,
                false,
                null);
        }

        return decision.Status switch
        {
            DialogueOrchestrationDecisionStatus.Continue => Result(
                DialogueOrchestrationStatus.Continued,
                true,
                true,
                decision),
            DialogueOrchestrationDecisionStatus.Complete => Result(
                DialogueOrchestrationStatus.Completed,
                true,
                true,
                decision),
            DialogueOrchestrationDecisionStatus.Rejected => Result(
                DialogueOrchestrationStatus.Rejected,
                true,
                true,
                decision),
            _ => Result(
                DialogueOrchestrationStatus.DecisionInvalid,
                true,
                false,
                null),
        };
    }

    private DialogueOrchestrationStatus? ValidatePreflight(
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateSnapshot<TState>
            snapshot,
        DialogueOrchestrationRequestEnvelope<TInput> request,
        global::AI.Sandbox.Engine.Core.Conversation.ConversationState<TTopic>
            conversation)
    {
        if (request.OrchestratorId != orchestratorId)
        {
            return DialogueOrchestrationStatus.OrchestratorMismatch;
        }

        if (request.WorldId != snapshot.WorldId)
        {
            return DialogueOrchestrationStatus.WorldMismatch;
        }

        if (request.ExpectedWorldStateVersion != snapshot.Version)
        {
            return DialogueOrchestrationStatus.VersionConflict;
        }

        if (request.ExpectedSimulationTick != snapshot.SimulationTick)
        {
            return DialogueOrchestrationStatus.SimulationTickMismatch;
        }

        if (request.ConversationId != conversation.ConversationId ||
            request.WorldId != conversation.WorldId)
        {
            return DialogueOrchestrationStatus.ConversationMismatch;
        }

        if (request.ExpectedConversationRevision != conversation.Revision)
        {
            return DialogueOrchestrationStatus.ConversationRevisionConflict;
        }

        if (conversation.IsClosed)
        {
            return DialogueOrchestrationStatus.ConversationClosed;
        }

        if (!conversation.ParticipantEntityIds.Contains(
                request.PerspectiveOwnerEntityId))
        {
            return DialogueOrchestrationStatus
                .PerspectiveOwnerNotParticipant;
        }

        if (!conversation.ParticipantEntityIds.Contains(
                request.SourceSpeakerEntityId))
        {
            return DialogueOrchestrationStatus.SourceSpeakerNotParticipant;
        }

        if (!IsAudienceValid(
                request.SourceSpeakerEntityId,
                request.Audience,
                conversation.ParticipantEntityIds))
        {
            return DialogueOrchestrationStatus.AudienceInvalid;
        }

        foreach (var artifact in request.Artifacts)
        {
            if (artifact.ExchangeId != request.ExchangeId ||
                artifact.ConversationId != request.ConversationId ||
                artifact.PerspectiveOwnerEntityId !=
                    request.PerspectiveOwnerEntityId ||
                artifact.WorldId != request.WorldId ||
                artifact.WorldStateVersion !=
                    request.ExpectedWorldStateVersion ||
                artifact.SimulationTick != request.ExpectedSimulationTick)
            {
                return DialogueOrchestrationStatus.ArtifactInvalid;
            }
        }

        return null;
    }

    private static bool IsAudienceValid(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> speaker,
        global::AI.Sandbox.Engine.Core.Conversation.AddressAudience audience,
        IReadOnlyList<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>
            participants)
    {
        var targets = audience.TargetEntityIds;
        if (audience.Kind == global::AI.Sandbox.Engine.Core.Conversation
            .AddressAudienceKind.None)
        {
            return targets.Count == 0;
        }

        if (targets.Count == 0)
        {
            return false;
        }

        foreach (var target in targets)
        {
            if (target == speaker || !participants.Contains(target))
            {
                return false;
            }
        }

        if (audience.Kind == global::AI.Sandbox.Engine.Core.Conversation
            .AddressAudienceKind.SpecificParticipants)
        {
            return true;
        }

        if (audience.Kind != global::AI.Sandbox.Engine.Core.Conversation
                .AddressAudienceKind.AllParticipants ||
            targets.Count != participants.Count - 1)
        {
            return false;
        }

        var expected = participants
            .Where(participant => participant != speaker)
            .ToArray();
        return expected.SequenceEqual(targets);
    }

    private static DialogueOrchestrationResult<TDirective, TCompletion>
        Result(
            DialogueOrchestrationStatus status,
            bool invoked,
            bool stable,
            DialogueOrchestrationDecision<TDirective, TCompletion>? decision) =>
        new(status, invoked, stable, decision);
}
