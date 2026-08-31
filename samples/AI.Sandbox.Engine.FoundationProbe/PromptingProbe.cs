internal static class PromptingProbe
{
    private readonly record struct PromptText(string Text) :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptContent;

    internal sealed record Result(
        string Status,
        int AllocationCallCount,
        int InputCandidateCount,
        int SelectedCandidateCount,
        int RequiredUnits,
        int UsedUnits,
        int RemainingUnits,
        bool RequiredSelected,
        bool HighestPriorityOptionalSelected,
        bool LowerPriorityOptionalSkipped,
        bool DeterministicOrderValid,
        bool SelectedIdentityPreserved,
        bool InputCandidateIdentityPreserved,
        bool InputPayloadValuesPreserved,
        bool OwnerScopePreserved,
        bool WorldScopePreserved);

    internal static Result Run()
    {
        var manager =
            new global::AI.Sandbox.Engine.Core.Prompting
                .PromptBudgetManager<PromptText>();

        var ownerId = OwnerId();
        var worldId = WorldId();

        var required =
            CreateCandidate(
                CandidateId(92),
                ownerId,
                worldId,
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptInclusionMode.Required,
                priorityBasisPoints: 1_000,
                costUnits: 4,
                text: "required");

        var highPriorityOptional =
            CreateCandidate(
                CandidateId(93),
                ownerId,
                worldId,
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptInclusionMode.Optional,
                priorityBasisPoints: 9_000,
                costUnits: 6,
                text: "high");

        var lowerPriorityOptional =
            CreateCandidate(
                CandidateId(94),
                ownerId,
                worldId,
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptInclusionMode.Optional,
                priorityBasisPoints: 8_000,
                costUnits: 5,
                text: "low");

        var input =
            new[]
            {
                lowerPriorityOptional,
                required,
                highPriorityOptional,
            };

        var requiredTextBefore = required.Payload.Text;
        var highTextBefore = highPriorityOptional.Payload.Text;
        var lowTextBefore = lowerPriorityOptional.Payload.Text;

        var allocationCallCount = 0;
        allocationCallCount = checked(allocationCallCount + 1);

        var allocation =
            manager.Allocate(
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptBudget.FromUnits(10),
                input);

        if (allocationCallCount != 1)
        {
            throw new global::System.InvalidOperationException(
                $"Prompt budget allocation call count was {allocationCallCount}.");
        }

        if (allocation.Status !=
            global::AI.Sandbox.Engine.Core.Prompting.PromptBudgetStatus.Selected)
        {
            throw new global::System.InvalidOperationException(
                $"Prompt budget allocation failed: {allocation.Status}.");
        }

        if (allocation.RequiredUnits != 4 ||
            allocation.UsedUnits != 10 ||
            allocation.RemainingUnits != 0)
        {
            throw new global::System.InvalidOperationException(
                "Prompt budget accounting did not match the exact allocation.");
        }

        var selected = allocation.SelectedCandidates;

        var requiredSelected =
            selected.Any(candidate =>
                candidate.CandidateId == required.CandidateId);

        var highestPriorityOptionalSelected =
            selected.Any(candidate =>
                candidate.CandidateId ==
                    highPriorityOptional.CandidateId);

        var lowerPriorityOptionalSkipped =
            selected.All(candidate =>
                candidate.CandidateId !=
                    lowerPriorityOptional.CandidateId);

        var deterministicOrderValid =
            selected.Count == 2 &&
            selected[0].CandidateId == required.CandidateId &&
            selected[1].CandidateId ==
                highPriorityOptional.CandidateId;

        var selectedIdentityPreserved =
            selected.Count == 2 &&
            global::System.Object.ReferenceEquals(
                required,
                selected[0]) &&
            global::System.Object.ReferenceEquals(
                highPriorityOptional,
                selected[1]);

        var inputCandidateIdentityPreserved =
            global::System.Object.ReferenceEquals(
                lowerPriorityOptional,
                input[0]) &&
            global::System.Object.ReferenceEquals(
                required,
                input[1]) &&
            global::System.Object.ReferenceEquals(
                highPriorityOptional,
                input[2]);

        var inputPayloadValuesPreserved =
            required.Payload.Text == requiredTextBefore &&
            highPriorityOptional.Payload.Text == highTextBefore &&
            lowerPriorityOptional.Payload.Text == lowTextBefore &&
            required.Payload.Text == "required" &&
            highPriorityOptional.Payload.Text == "high" &&
            lowerPriorityOptional.Payload.Text == "low";

        var ownerScopePreserved =
            input.All(candidate =>
                candidate.OwnerEntityId == ownerId) &&
            selected.All(candidate =>
                candidate.OwnerEntityId == ownerId);

        var worldScopePreserved =
            input.All(candidate =>
                candidate.WorldId == worldId) &&
            selected.All(candidate =>
                candidate.WorldId == worldId);

        if (!requiredSelected ||
            !highestPriorityOptionalSelected ||
            !lowerPriorityOptionalSkipped ||
            !deterministicOrderValid ||
            !selectedIdentityPreserved ||
            !inputCandidateIdentityPreserved ||
            !inputPayloadValuesPreserved ||
            !ownerScopePreserved ||
            !worldScopePreserved)
        {
            throw new global::System.InvalidOperationException(
                "Prompt budget deterministic selection invariants failed.");
        }

        return new Result(
            allocation.Status.ToString(),
            allocationCallCount,
            input.Length,
            selected.Count,
            allocation.RequiredUnits,
            allocation.UsedUnits,
            allocation.RemainingUnits,
            requiredSelected,
            highestPriorityOptionalSelected,
            lowerPriorityOptionalSkipped,
            deterministicOrderValid,
            selectedIdentityPreserved,
            inputCandidateIdentityPreserved,
            inputPayloadValuesPreserved,
            ownerScopePreserved,
            worldScopePreserved);
    }

    private readonly record struct ComposeRequest(string Topic) :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptRequest;

    private readonly record struct RenderedPrompt(string Text) :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptDocument;

    private sealed record PromptWorldState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    internal sealed record CompositionResult(
        string Status,
        int ProcessorCallCount,
        bool WasComposed,
        bool ComposerWasInvoked,
        int ComposerCallCount,
        int ComposerSelectedCount,
        bool ComposerContextValid,
        string DecisionStatus,
        string BudgetStatus,
        int BudgetSelectedCandidateCount,
        int RequiredUnits,
        int UsedUnits,
        int RemainingUnits,
        bool RequestIdentityPreserved,
        bool DocumentIdentityPreserved,
        bool DocumentScopeValid,
        bool DocumentPayloadValid,
        bool WorldAuthorityUnchanged,
        int BeforeValue,
        int AfterValue,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion BeforeVersion,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion AfterVersion,
        ulong BeforeSimulationTick,
        ulong AfterSimulationTick);

    internal static CompositionResult RunComposition()
    {
        var manager =
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<PromptWorldState>.Create(
                    WorldId(),
                    new PromptWorldState(0));

        var before = manager.Read();

        var ownerId = OwnerId();
        var requestId = RequestId(93);
        var composerId = ComposerId(93);
        var documentId = DocumentId(93);

        var request =
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptRequestEnvelope<ComposeRequest>.Create(
                    requestId,
                    ownerId,
                    before.WorldId,
                    before.Version,
                    before.SimulationTick,
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptBudget.FromUnits(10),
                    new ComposeRequest("foundation"));

        var required =
            CreateCandidate(
                CandidateId(95),
                ownerId,
                before.WorldId,
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptInclusionMode.Required,
                priorityBasisPoints: 1_000,
                costUnits: 4,
                text: "required");

        var skippedOptional =
            CreateCandidate(
                CandidateId(96),
                ownerId,
                before.WorldId,
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptInclusionMode.Optional,
                priorityBasisPoints: 9_000,
                costUnits: 7,
                text: "skip");

        var selectedOptional =
            CreateCandidate(
                CandidateId(97),
                ownerId,
                before.WorldId,
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptInclusionMode.Optional,
                priorityBasisPoints: 8_000,
                costUnits: 6,
                text: "selected");

        var document =
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptDocumentEnvelope<RenderedPrompt>.Create(
                    documentId,
                    composerId,
                    ownerId,
                    before.WorldId,
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptCost.FromUnits(10),
                    new RenderedPrompt("final"));

        var composer =
            new FixedComposer(
                before,
                request,
                required,
                selectedOptional,
                composerId,
                document);

        var processor =
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionProcessor<
                    PromptWorldState,
                    ComposeRequest,
                    PromptText,
                    RenderedPrompt>.Create(
                        manager,
                        composerId,
                        composer);

        var processorCallCount = 0;
        processorCallCount = checked(processorCallCount + 1);

        var result =
            processor.Compose(
                request,
                new[]
                {
                    skippedOptional,
                    required,
                    selectedOptional,
                });

        if (processorCallCount != 1)
        {
            throw new global::System.InvalidOperationException(
                $"Prompt composition processor call count was {processorCallCount}.");
        }

        if (result.Status !=
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionStatus.Composed)
        {
            throw new global::System.InvalidOperationException(
                $"Prompt composition failed: {result.Status}.");
        }

        if (!result.WasComposed ||
            !result.ComposerWasInvoked ||
            composer.CallCount != 1 ||
            composer.LastSelectedCount != 2 ||
            !composer.ContextValid)
        {
            throw new global::System.InvalidOperationException(
                "Prompt composition invocation invariants failed.");
        }

        var budgetResult = result.BudgetResult;

        if (budgetResult is null ||
            budgetResult.Status !=
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptBudgetStatus.Selected ||
            budgetResult.SelectedCandidates.Count != 2 ||
            budgetResult.RequiredUnits != 4 ||
            budgetResult.UsedUnits != 10 ||
            budgetResult.RemainingUnits != 0)
        {
            throw new global::System.InvalidOperationException(
                "Prompt composition budget invariants failed.");
        }

        var decision = result.Decision;

        if (decision is null ||
            decision.Status !=
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptCompositionDecisionStatus.Composed ||
            decision.Document is null)
        {
            throw new global::System.InvalidOperationException(
                "Prompt composition decision invariants failed.");
        }

        var requestIdentityPreserved =
            global::System.Object.ReferenceEquals(
                request,
                result.Request);

        var documentIdentityPreserved =
            global::System.Object.ReferenceEquals(
                document,
                decision.Document) &&
            global::System.Object.ReferenceEquals(
                document,
                decision.Document);

        var documentScopeValid =
            decision.Document is not null &&
            decision.Document.DocumentId == documentId &&
            decision.Document.ComposerId == composerId &&
            decision.Document.OwnerEntityId == ownerId &&
            decision.Document.WorldId == before.WorldId &&
            decision.Document.Cost.Units == 10;

        var documentPayloadValid =
            decision.Document is not null &&
            decision.Document.Payload.Text == "final";

        var after = manager.Read();

        var worldAuthorityUnchanged =
            after.WorldId == before.WorldId &&
            global::System.Object.ReferenceEquals(
                before.State,
                after.State) &&
            before.State.Value == after.State.Value &&
            before.Version == after.Version &&
            before.SimulationTick == after.SimulationTick;

        if (!requestIdentityPreserved ||
            !documentIdentityPreserved ||
            !documentScopeValid ||
            !documentPayloadValid ||
            !worldAuthorityUnchanged)
        {
            throw new global::System.InvalidOperationException(
                "Prompt composition identity or authority invariants failed.");
        }

        return new CompositionResult(
            result.Status.ToString(),
            processorCallCount,
            result.WasComposed,
            result.ComposerWasInvoked,
            composer.CallCount,
            composer.LastSelectedCount,
            composer.ContextValid,
            decision.Status.ToString(),
            budgetResult.Status.ToString(),
            budgetResult.SelectedCandidates.Count,
            budgetResult.RequiredUnits,
            budgetResult.UsedUnits,
            budgetResult.RemainingUnits,
            requestIdentityPreserved,
            documentIdentityPreserved,
            documentScopeValid,
            documentPayloadValid,
            worldAuthorityUnchanged,
            before.State.Value,
            after.State.Value,
            before.Version,
            after.Version,
            before.SimulationTick,
            after.SimulationTick);
    }

    private sealed class FixedComposer :
        global::AI.Sandbox.Engine.Core.Prompting.IPromptComposer<
            PromptWorldState,
            ComposeRequest,
            PromptText,
            RenderedPrompt>
    {
        private readonly global::AI.Sandbox.Engine.Core.WorldState
            .WorldStateSnapshot<PromptWorldState> _expectedSnapshot;

        private readonly global::AI.Sandbox.Engine.Core.Prompting
            .PromptRequestEnvelope<ComposeRequest> _expectedRequest;

        private readonly global::AI.Sandbox.Engine.Core.Prompting
            .PromptCandidateEnvelope<PromptText> _expectedRequired;

        private readonly global::AI.Sandbox.Engine.Core.Prompting
            .PromptCandidateEnvelope<PromptText> _expectedOptional;

        private readonly global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptComposerIdKind> _expectedComposerId;

        private readonly global::AI.Sandbox.Engine.Core.Prompting
            .PromptDocumentEnvelope<RenderedPrompt> _document;

        internal FixedComposer(
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateSnapshot<PromptWorldState> expectedSnapshot,
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptRequestEnvelope<ComposeRequest> expectedRequest,
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCandidateEnvelope<PromptText> expectedRequired,
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptCandidateEnvelope<PromptText> expectedOptional,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptComposerIdKind> expectedComposerId,
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptDocumentEnvelope<RenderedPrompt> document)
        {
            _expectedSnapshot = expectedSnapshot;
            _expectedRequest = expectedRequest;
            _expectedRequired = expectedRequired;
            _expectedOptional = expectedOptional;
            _expectedComposerId = expectedComposerId;
            _document = document;
        }

        internal int CallCount { get; private set; }

        internal int LastSelectedCount { get; private set; }

        internal bool ContextValid { get; private set; }

        public global::AI.Sandbox.Engine.Core.Prompting
            .PromptCompositionDecision<RenderedPrompt> Compose(
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptCompositionContext<
                        PromptWorldState,
                        ComposeRequest,
                        PromptText> context)
        {
            CallCount = checked(CallCount + 1);
            LastSelectedCount = context.BudgetResult.SelectedCandidates.Count;

            ContextValid =
                global::System.Object.ReferenceEquals(
                    _expectedSnapshot.State,
                    context.Snapshot.State) &&
                _expectedSnapshot.WorldId == context.Snapshot.WorldId &&
                _expectedSnapshot.Version == context.Snapshot.Version &&
                _expectedSnapshot.SimulationTick ==
                    context.Snapshot.SimulationTick &&
                global::System.Object.ReferenceEquals(
                    _expectedRequest,
                    context.Request) &&
                context.ComposerId == _expectedComposerId &&
                context.BudgetResult.Status ==
                    global::AI.Sandbox.Engine.Core.Prompting
                        .PromptBudgetStatus.Selected &&
                context.BudgetResult.RequiredUnits == 4 &&
                context.BudgetResult.UsedUnits == 10 &&
                context.BudgetResult.RemainingUnits == 0 &&
                context.BudgetResult.SelectedCandidates.Count == 2 &&
                global::System.Object.ReferenceEquals(
                    _expectedRequired,
                    context.BudgetResult.SelectedCandidates[0]) &&
                global::System.Object.ReferenceEquals(
                    _expectedOptional,
                    context.BudgetResult.SelectedCandidates[1]);

            return global::AI.Sandbox.Engine.Core.Prompting
                .PromptCompositionDecision<RenderedPrompt>.Compose(
                    _document);
        }
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Prompting.PromptRequestIdKind>
        RequestId(int suffix) =>
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptRequestIdKind>.Parse(
                        $"019d0000-0000-7900-8900-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Prompting.PromptComposerIdKind>
        ComposerId(int suffix) =>
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptComposerIdKind>.Parse(
                        $"019d0000-0000-7a00-8a00-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Prompting.PromptDocumentIdKind>
        DocumentId(int suffix) =>
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptDocumentIdKind>.Parse(
                        $"019d0000-0000-7b00-8b00-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Prompting
        .PromptCandidateEnvelope<PromptText> CreateCandidate(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptCandidateIdKind> candidateId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerId,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
            global::AI.Sandbox.Engine.Core.Prompting
                .PromptInclusionMode mode,
            int priorityBasisPoints,
            int costUnits,
            string text) =>
        global::AI.Sandbox.Engine.Core.Prompting
            .PromptCandidateEnvelope<PromptText>.Create(
                candidateId,
                ownerId,
                worldId,
                mode,
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptPriority.FromBasisPoints(priorityBasisPoints),
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptCost.FromUnits(costUnits),
                new PromptText(text));

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
        OwnerId() =>
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(
                    "019d0000-0000-7600-8600-000000000092");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>
        WorldId() =>
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                    "019d0000-0000-7700-8700-000000000092");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Prompting.PromptCandidateIdKind>
        CandidateId(int suffix) =>
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Prompting
                    .PromptCandidateIdKind>.Parse(
                        $"019d0000-0000-7800-8800-{suffix:D12}");
}
