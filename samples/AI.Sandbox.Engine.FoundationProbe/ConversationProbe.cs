namespace AI.Sandbox.Engine.FoundationProbe;

internal static class ConversationProbe
{
    private sealed record ProbeConversationWorldState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    private sealed record ProbeAddressQuery(string Value) :
        global::AI.Sandbox.Engine.Core.Conversation.IAddressQuery;

    private sealed record ProbeTopic(string Value) :
        global::AI.Sandbox.Engine.Core.Conversation.IConversationTopic;

    private sealed record ProbeDialogueInput(string Text) :
        global::AI.Sandbox.Engine.Core.Dialogue.IDialogueInput;

    private sealed record ProbeDialogueDirective(string Name) :
        global::AI.Sandbox.Engine.Core.Dialogue.IDialogueDirective;

    private sealed record ProbeDialogueCompletion(string Text) :
        global::AI.Sandbox.Engine.Core.Dialogue.IDialogueCompletion;

    internal sealed record Result(
        string Status,
        bool ResolverWasInvoked,
        bool HasStableDecision,
        int ResolverCallCount,
        long ConversationRevision,
        bool ExactAudience,
        bool WorldAuthorityUnchanged,
        string DialogueStatus,
        bool DialogueOrchestratorWasInvoked,
        bool DialogueHasStableDecision,
        int DialogueOrchestratorCallCount,
        bool DialogueWasContinued,
        string DialogueDirectiveName,
        bool DialogueWorldAuthorityUnchanged);

    internal static Result Run()
    {
        var worldId = WorldId();
        var speakerId = EntityId(8602);
        var intendedParticipantId = EntityId(8603);
        var otherParticipantId = EntityId(8604);

        var manager =
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<ProbeConversationWorldState>.Create(
                    worldId,
                    new ProbeConversationWorldState(0));

        var before = manager.Read();

        var conversation =
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationState<ProbeTopic>.Start(
                    worldId,
                    ConversationId(8605),
                    new[]
                    {
                        speakerId,
                        intendedParticipantId,
                        otherParticipantId,
                    },
                    new ProbeTopic("foundation-conversation"));

        if (conversation.Revision.Value != 1)
        {
            throw new global::System.InvalidOperationException(
                "Conversation did not start at revision one.");
        }

        var audience =
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressAudience.SpecificParticipants(
                    new[] { intendedParticipantId });

        var resolver =
            new FixedResolver(
                ResolverId(8606),
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolutionDecision.Resolve(
                        audience,
                        global::AI.Sandbox.Engine.Core.Conversation
                            .AddressResolutionConfidence.FromBasisPoints(9000)));

        var processor =
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionProcessor<
                    ProbeConversationWorldState,
                    ProbeAddressQuery,
                    ProbeTopic>.Create(
                        manager,
                        resolver);

        var request =
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionRequestEnvelope<ProbeAddressQuery>.Create(
                    ResolutionId(8607),
                    resolver.ResolverId,
                    conversation.ConversationId,
                    speakerId,
                    before.WorldId,
                    before.Version,
                    before.SimulationTick,
                    conversation.Revision,
                    new ProbeAddressQuery("resolve intended participant"));

        var resolution =
            processor.Resolve(
                request,
                conversation);

        if (resolution.Status !=
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionStatus.Resolved)
        {
            throw new global::System.InvalidOperationException(
                $"Conversation address resolution failed: {resolution.Status}");
        }

        if (!resolution.ResolverWasInvoked)
        {
            throw new global::System.InvalidOperationException(
                "Conversation resolver was not invoked.");
        }

        if (!resolution.HasStableDecision ||
            resolution.Decision is null)
        {
            throw new global::System.InvalidOperationException(
                "Conversation did not retain a stable resolution decision.");
        }

        if (resolver.CallCount != 1)
        {
            throw new global::System.InvalidOperationException(
                $"Conversation resolver call count was {resolver.CallCount}.");
        }

        var exactAudience =
            global::System.Object.ReferenceEquals(
                resolution.Decision.Audience,
                audience);

        if (!exactAudience)
        {
            throw new global::System.InvalidOperationException(
                "Conversation did not retain the exact resolved audience.");
        }

        var resolvedAudience =
            resolution.Decision.Audience ??
            throw new global::System.InvalidOperationException(
                "Resolved Conversation decision did not retain an audience.");

        var dialogueOrchestrator =
            new FixedDialogueOrchestrator(
                DialogueOrchestratorId(8701),
                global::AI.Sandbox.Engine.Core.Dialogue
                    .DialogueOrchestrationDecision<
                        ProbeDialogueDirective,
                        ProbeDialogueCompletion>.Continue(
                            new ProbeDialogueDirective("invoke-model")));

        var dialogueProcessor =
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationProcessor<
                    ProbeConversationWorldState,
                    ProbeDialogueInput,
                    ProbeTopic,
                    ProbeDialogueDirective,
                    ProbeDialogueCompletion>.Create(
                        manager,
                        dialogueOrchestrator);

        var dialogueRequest =
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationRequestEnvelope<
                    ProbeDialogueInput>.Create(
                        DialogueOrchestrationId(8702),
                        dialogueOrchestrator.OrchestratorId,
                        DialogueExchangeId(8703),
                        conversation.ConversationId,
                        conversation.Revision,
                        intendedParticipantId,
                        speakerId,
                        resolvedAudience,
                        before.WorldId,
                        before.Version,
                        before.SimulationTick,
                        new ProbeDialogueInput("hello"),
                        global::System.Array.Empty<
                            global::AI.Sandbox.Engine.Core.Dialogue
                                .DialogueArtifactEnvelope>());

        var dialogueResult =
            dialogueProcessor.Process(
                dialogueRequest,
                conversation);

        if (dialogueResult.Status !=
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationStatus.Continued)
        {
            throw new global::System.InvalidOperationException(
                $"Dialogue orchestration failed: {dialogueResult.Status}");
        }

        if (!dialogueResult.OrchestratorWasInvoked)
        {
            throw new global::System.InvalidOperationException(
                "Dialogue orchestrator was not invoked.");
        }

        if (!dialogueResult.HasStableDecision ||
            dialogueResult.Decision is null)
        {
            throw new global::System.InvalidOperationException(
                "Dialogue orchestration did not retain a stable decision.");
        }

        if (!dialogueResult.WasContinued ||
            dialogueResult.WasCompleted)
        {
            throw new global::System.InvalidOperationException(
                "Dialogue orchestration did not retain Continue semantics.");
        }

        if (dialogueOrchestrator.CallCount != 1)
        {
            throw new global::System.InvalidOperationException(
                $"Dialogue orchestrator call count was " +
                $"{dialogueOrchestrator.CallCount}.");
        }

        if (!global::System.String.Equals(
            dialogueResult.Decision.Directive.Name,
            "invoke-model",
            global::System.StringComparison.Ordinal))
        {
            throw new global::System.InvalidOperationException(
                "Dialogue orchestration did not retain the exact directive.");
        }

        var dialogueAfter = manager.Read();
        var dialogueWorldAuthorityUnchanged =
            before.WorldId.Equals(dialogueAfter.WorldId) &&
            before.Version.Equals(dialogueAfter.Version) &&
            before.SimulationTick == dialogueAfter.SimulationTick &&
            global::System.Object.Equals(
                before.State,
                dialogueAfter.State);

        if (!dialogueWorldAuthorityUnchanged)
        {
            throw new global::System.InvalidOperationException(
                "Dialogue orchestration mutated world authority.");
        }

        var after = manager.Read();
        var worldAuthorityUnchanged =
            before.WorldId.Equals(after.WorldId) &&
            before.Version.Equals(after.Version) &&
            before.SimulationTick == after.SimulationTick &&
            global::System.Object.Equals(before.State, after.State);

        if (!worldAuthorityUnchanged)
        {
            throw new global::System.InvalidOperationException(
                "Conversation address resolution mutated world authority.");
        }

        return new Result(
            resolution.Status.ToString(),
            resolution.ResolverWasInvoked,
            resolution.HasStableDecision,
            resolver.CallCount,
            conversation.Revision.Value,
            exactAudience,
            worldAuthorityUnchanged,
            dialogueResult.Status.ToString(),
            dialogueResult.OrchestratorWasInvoked,
            dialogueResult.HasStableDecision,
            dialogueOrchestrator.CallCount,
            dialogueResult.WasContinued,
            dialogueResult.Decision.Directive.Name,
            dialogueWorldAuthorityUnchanged);
    }

    private sealed class FixedDialogueOrchestrator :
        global::AI.Sandbox.Engine.Core.Dialogue.IDialogueOrchestrator<
            ProbeConversationWorldState,
            ProbeDialogueInput,
            ProbeTopic,
            ProbeDialogueDirective,
            ProbeDialogueCompletion>
    {
        private readonly global::AI.Sandbox.Engine.Core.Dialogue
            .DialogueOrchestrationDecision<
                ProbeDialogueDirective,
                ProbeDialogueCompletion> decision;

        internal FixedDialogueOrchestrator(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Dialogue
                    .DialogueOrchestratorIdKind> orchestratorId,
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationDecision<
                    ProbeDialogueDirective,
                    ProbeDialogueCompletion> decision)
        {
            OrchestratorId = orchestratorId;
            this.decision = decision;
        }

        public global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Dialogue.DialogueOrchestratorIdKind>
            OrchestratorId { get; }

        internal int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Dialogue
            .DialogueOrchestrationDecision<
                ProbeDialogueDirective,
                ProbeDialogueCompletion> Decide(
                    global::AI.Sandbox.Engine.Core.Dialogue
                        .DialogueOrchestrationContext<
                            ProbeConversationWorldState,
                            ProbeDialogueInput,
                            ProbeTopic> context)
        {
            CallCount = checked(CallCount + 1);

            _ = context;

            return decision;
        }
    }

    private sealed class FixedResolver :
        global::AI.Sandbox.Engine.Core.Conversation.IAddressResolver<
            ProbeConversationWorldState,
            ProbeAddressQuery,
            ProbeTopic>
    {
        private readonly global::AI.Sandbox.Engine.Core.Conversation
            .AddressResolutionDecision decision;

        internal FixedResolver(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolverIdKind> resolverId,
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionDecision decision)
        {
            ResolverId = resolverId;
            this.decision = decision;
        }

        public global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Conversation.AddressResolverIdKind>
            ResolverId { get; }

        internal int CallCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Conversation
            .AddressResolutionDecision Resolve(
                global::AI.Sandbox.Engine.Core.Conversation
                    .AddressResolutionContext<
                        ProbeConversationWorldState,
                        ProbeAddressQuery,
                        ProbeTopic> context)
        {
            CallCount = checked(CallCount + 1);

            if (context.CandidateEntityIds.Count != 2)
            {
                throw new global::System.InvalidOperationException(
                    "Conversation resolver candidate set was not speaker-excluded.");
            }

            return decision;
        }
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId() =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                "019d0000-0000-7000-8000-000000008601");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> EntityId(
            int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(
                $"019d0000-0000-7100-8100-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Conversation.ConversationIdKind>
            ConversationId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Conversation
                .ConversationIdKind>.Parse(
                    $"019d0000-0000-7200-8200-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Conversation.AddressResolverIdKind>
            ResolverId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolverIdKind>.Parse(
                    $"019d0000-0000-7300-8300-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Conversation.AddressResolutionIdKind>
            ResolutionId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Conversation
                .AddressResolutionIdKind>.Parse(
                    $"019d0000-0000-7400-8400-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Dialogue.DialogueOrchestratorIdKind>
            DialogueOrchestratorId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestratorIdKind>.Parse(
                    $"019d0000-0000-7500-8500-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Dialogue.DialogueOrchestrationIdKind>
            DialogueOrchestrationId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueOrchestrationIdKind>.Parse(
                    $"019d0000-0000-7600-8600-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Dialogue.DialogueExchangeIdKind>
            DialogueExchangeId(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Dialogue
                .DialogueExchangeIdKind>.Parse(
                    $"019d0000-0000-7700-8700-{suffix:D12}");
}
