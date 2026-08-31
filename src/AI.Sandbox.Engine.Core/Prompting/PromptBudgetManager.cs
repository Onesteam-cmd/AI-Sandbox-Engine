namespace AI.Sandbox.Engine.Core.Prompting;

/// <summary>
/// Deterministically selects required and optional exact prompt candidates
/// within one provider-neutral budget.
/// </summary>
/// <typeparam name="TContent">The exact candidate-content payload type.</typeparam>
public sealed class PromptBudgetManager<TContent>
    where TContent : IPromptContent
{
    /// <summary>
    /// Creates a stateless exact prompt-budget manager.
    /// </summary>
    public PromptBudgetManager()
    {
        PromptTypePolicy.EnsureExactType(
            typeof(TContent),
            typeof(IPromptContent),
            "prompt content");
    }

    /// <summary>
    /// Selects candidates without calling providers or mutating any state.
    /// </summary>
    /// <param name="budget">The positive provider-neutral budget.</param>
    /// <param name="candidates">The candidate set to validate and select.</param>
    /// <returns>The deterministic allocation result.</returns>
    public PromptBudgetResult<TContent> Allocate(
        PromptBudget budget,
        IEnumerable<PromptCandidateEnvelope<TContent>> candidates)
    {
        if (!budget.IsInitialized)
        {
            throw new ArgumentException(
                "The prompt budget must be initialized.",
                nameof(budget));
        }

        ArgumentNullException.ThrowIfNull(candidates);

        var materialized = candidates.ToArray();
        var candidateIds = new HashSet<
            global::AI.Sandbox.Engine.Core.Identifiers
                .Id<PromptCandidateIdKind>>();

        foreach (var candidate in materialized)
        {
            ArgumentNullException.ThrowIfNull(candidate);

            if (!candidateIds.Add(candidate.CandidateId))
            {
                throw new ArgumentException(
                    "Prompt candidates cannot repeat a candidate ID.",
                    nameof(candidates));
            }
        }

        Array.Sort(
            materialized,
            static (left, right) =>
            {
                var inclusionOrder =
                    left.InclusionMode.CompareTo(right.InclusionMode);
                if (inclusionOrder != 0)
                {
                    return inclusionOrder;
                }

                var priorityOrder =
                    right.Priority.CompareTo(left.Priority);
                return priorityOrder != 0
                    ? priorityOrder
                    : left.CandidateId.CompareTo(right.CandidateId);
            });

        var requiredUnits = 0;
        foreach (var candidate in materialized)
        {
            if (candidate.InclusionMode == PromptInclusionMode.Required)
            {
                requiredUnits = checked(
                    requiredUnits + candidate.Cost.Units);
            }
        }

        if (requiredUnits > budget.Units)
        {
            return new PromptBudgetResult<TContent>(
                PromptBudgetStatus.RequiredBudgetExceeded,
                Array.Empty<PromptCandidateEnvelope<TContent>>(),
                requiredUnits,
                0,
                budget.Units);
        }

        var selected = new List<PromptCandidateEnvelope<TContent>>(
            materialized.Length);
        var remainingUnits = budget.Units;

        foreach (var candidate in materialized)
        {
            if (candidate.InclusionMode == PromptInclusionMode.Required)
            {
                selected.Add(candidate);
                remainingUnits = checked(
                    remainingUnits - candidate.Cost.Units);
                continue;
            }

            if (candidate.Cost.Units <= remainingUnits)
            {
                selected.Add(candidate);
                remainingUnits = checked(
                    remainingUnits - candidate.Cost.Units);
            }
        }

        return new PromptBudgetResult<TContent>(
            PromptBudgetStatus.Selected,
            Array.AsReadOnly(selected.ToArray()),
            requiredUnits,
            checked(budget.Units - remainingUnits),
            remainingUnits);
    }
}
