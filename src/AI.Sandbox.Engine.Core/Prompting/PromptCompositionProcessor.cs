namespace AI.Sandbox.Engine.Core.Prompting;

/// <summary>
/// Applies deterministic context budgeting and invokes one exact pure composer
/// without calling providers or mutating authoritative or subjective state.
/// </summary>
/// <typeparam name="TState">The immutable authoritative world-state type.</typeparam>
/// <typeparam name="TRequest">The exact prompt-request payload type.</typeparam>
/// <typeparam name="TContent">The exact candidate-content payload type.</typeparam>
/// <typeparam name="TDocument">The exact prompt-document payload type.</typeparam>
public sealed class PromptCompositionProcessor<
    TState,
    TRequest,
    TContent,
    TDocument>
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TRequest : IPromptRequest
    where TContent : IPromptContent
    where TDocument : IPromptDocument
{
    private readonly global::AI.Sandbox.Engine.Core.WorldState
        .WorldStateManager<TState> manager;
    private readonly global::AI.Sandbox.Engine.Core.Identifiers
        .Id<PromptComposerIdKind> composerId;
    private readonly PromptBudgetManager<TContent> budgetManager;
    private readonly IPromptComposer<
        TState,
        TRequest,
        TContent,
        TDocument> composer;

    private PromptCompositionProcessor(
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateManager<TState>
            manager,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<PromptComposerIdKind>
            composerId,
        IPromptComposer<TState, TRequest, TContent, TDocument> composer)
    {
        this.manager = manager;
        this.composerId = composerId;
        this.composer = composer;
        budgetManager = new PromptBudgetManager<TContent>();
    }

    /// <summary>
    /// Creates a processor bound to one authority manager and exact composer.
    /// </summary>
    /// <param name="manager">The authoritative World State manager.</param>
    /// <param name="composerId">The stable exact composer ID.</param>
    /// <param name="composer">The synchronous pure composer.</param>
    /// <returns>The configured composition processor.</returns>
    public static PromptCompositionProcessor<
        TState,
        TRequest,
        TContent,
        TDocument> Create(
            global::AI.Sandbox.Engine.Core.WorldState.WorldStateManager<TState>
                manager,
            global::AI.Sandbox.Engine.Core.Identifiers.Id<PromptComposerIdKind>
                composerId,
            IPromptComposer<TState, TRequest, TContent, TDocument> composer)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(composer);

        if (composerId.IsEmpty)
        {
            throw new ArgumentException(
                "The prompt composer ID cannot be empty.",
                nameof(composerId));
        }

        PromptTypePolicy.EnsureExactType(
            typeof(TRequest),
            typeof(IPromptRequest),
            "prompt request");
        PromptTypePolicy.EnsureExactType(
            typeof(TContent),
            typeof(IPromptContent),
            "prompt content");
        PromptTypePolicy.EnsureExactType(
            typeof(TDocument),
            typeof(IPromptDocument),
            "prompt document");

        return new PromptCompositionProcessor<
            TState,
            TRequest,
            TContent,
            TDocument>(
                manager,
                composerId,
                composer);
    }

    /// <summary>
    /// Budgets candidates and composes exactly once when snapshot coordinates
    /// and candidate scope remain valid.
    /// </summary>
    /// <param name="request">The exact owner-scoped prompt request.</param>
    /// <param name="candidates">The exact prompt candidates.</param>
    /// <returns>The explicit composition result.</returns>
    public PromptCompositionResult<TRequest, TContent, TDocument> Compose(
        PromptRequestEnvelope<TRequest> request,
        IEnumerable<PromptCandidateEnvelope<TContent>> candidates)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(candidates);

        var materialized = candidates.ToArray();
        foreach (var candidate in materialized)
        {
            ArgumentNullException.ThrowIfNull(candidate);
        }

        var before = manager.Read();

        if (request.WorldId != before.WorldId)
        {
            return PromptCompositionResult<
                TRequest,
                TContent,
                TDocument>.NotComposed(
                    PromptCompositionStatus.WorldMismatch,
                    request);
        }

        if (request.WorldStateVersion != before.Version)
        {
            return PromptCompositionResult<
                TRequest,
                TContent,
                TDocument>.NotComposed(
                    PromptCompositionStatus.VersionConflict,
                    request);
        }

        if (request.SimulationTick != before.SimulationTick)
        {
            return PromptCompositionResult<
                TRequest,
                TContent,
                TDocument>.NotComposed(
                    PromptCompositionStatus.SimulationTickMismatch,
                    request);
        }

        var candidateIds = new HashSet<
            global::AI.Sandbox.Engine.Core.Identifiers
                .Id<PromptCandidateIdKind>>();
        foreach (var candidate in materialized)
        {
            if (candidate.WorldId != request.WorldId)
            {
                return PromptCompositionResult<
                    TRequest,
                    TContent,
                    TDocument>.NotComposed(
                        PromptCompositionStatus.CandidateWorldMismatch,
                        request);
            }

            if (candidate.OwnerEntityId != request.OwnerEntityId)
            {
                return PromptCompositionResult<
                    TRequest,
                    TContent,
                    TDocument>.NotComposed(
                        PromptCompositionStatus.CandidateOwnerMismatch,
                        request);
            }

            if (!candidateIds.Add(candidate.CandidateId))
            {
                return PromptCompositionResult<
                    TRequest,
                    TContent,
                    TDocument>.NotComposed(
                        PromptCompositionStatus.DuplicateCandidate,
                        request);
            }
        }

        var budgetResult = budgetManager.Allocate(
            request.Budget,
            materialized);

        if (budgetResult.Status ==
            PromptBudgetStatus.RequiredBudgetExceeded)
        {
            return PromptCompositionResult<
                TRequest,
                TContent,
                TDocument>.BudgetRejected(
                    request,
                    budgetResult);
        }

        var context = new PromptCompositionContext<
            TState,
            TRequest,
            TContent>(
                before,
                request,
                budgetResult,
                composerId);
        var decision = composer.Compose(context) ??
            throw new InvalidOperationException(
                "Prompt composers cannot return null decisions.");

        var after = manager.Read();

        if (after.SimulationTick != before.SimulationTick)
        {
            return PromptCompositionResult<
                TRequest,
                TContent,
                TDocument>.Discarded(
                    PromptCompositionStatus.SimulationTickMismatch,
                    request,
                    budgetResult);
        }

        if (after.Version != before.Version)
        {
            return PromptCompositionResult<
                TRequest,
                TContent,
                TDocument>.Discarded(
                    PromptCompositionStatus.VersionConflict,
                    request,
                    budgetResult);
        }

        var status = decision.Status switch
        {
            PromptCompositionDecisionStatus.Rejected =>
                PromptCompositionStatus.Rejected,
            PromptCompositionDecisionStatus.Composed =>
                ValidateDocument(request, decision.Document),
            _ => throw new InvalidOperationException(
                "Unknown prompt composition decision status."),
        };

        return PromptCompositionResult<
            TRequest,
            TContent,
            TDocument>.Evaluated(
                status,
                request,
                budgetResult,
                decision);
    }

    private PromptCompositionStatus ValidateDocument(
        PromptRequestEnvelope<TRequest> request,
        PromptDocumentEnvelope<TDocument> document)
    {
        if (document.WorldId != request.WorldId)
        {
            return PromptCompositionStatus.ResultWorldMismatch;
        }

        if (document.OwnerEntityId != request.OwnerEntityId)
        {
            return PromptCompositionStatus.ResultOwnerMismatch;
        }

        if (document.ComposerId != composerId)
        {
            return PromptCompositionStatus.ResultComposerMismatch;
        }

        return document.Cost.Units > request.Budget.Units
            ? PromptCompositionStatus.ResultBudgetExceeded
            : PromptCompositionStatus.Composed;
    }
}
