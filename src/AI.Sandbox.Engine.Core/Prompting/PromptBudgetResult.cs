namespace AI.Sandbox.Engine.Core.Prompting;

/// <summary>
/// Represents one immutable deterministic prompt-budget allocation.
/// </summary>
/// <typeparam name="TContent">The exact candidate-content payload type.</typeparam>
public sealed class PromptBudgetResult<TContent>
    where TContent : IPromptContent
{
    private readonly IReadOnlyList<PromptCandidateEnvelope<TContent>>
        selectedCandidates;

    internal PromptBudgetResult(
        PromptBudgetStatus status,
        IReadOnlyList<PromptCandidateEnvelope<TContent>> selectedCandidates,
        int requiredUnits,
        int usedUnits,
        int remainingUnits)
    {
        Status = status;
        this.selectedCandidates = selectedCandidates;
        RequiredUnits = requiredUnits;
        UsedUnits = usedUnits;
        RemainingUnits = remainingUnits;
    }

    /// <summary>
    /// Gets the explicit allocation status.
    /// </summary>
    public PromptBudgetStatus Status { get; }

    /// <summary>
    /// Gets the sum of every required candidate cost.
    /// </summary>
    public int RequiredUnits { get; }

    /// <summary>
    /// Gets the sum of selected candidate costs.
    /// </summary>
    public int UsedUnits { get; }

    /// <summary>
    /// Gets budget units remaining after deterministic selection.
    /// </summary>
    public int RemainingUnits { get; }

    /// <summary>
    /// Gets deterministically ordered selected candidates.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The required candidate set exceeded the budget.
    /// </exception>
    public IReadOnlyList<PromptCandidateEnvelope<TContent>>
        SelectedCandidates =>
        Status == PromptBudgetStatus.Selected
            ? selectedCandidates
            : throw new InvalidOperationException(
                "An exceeded prompt budget has no selected candidate set.");

    /// <summary>
    /// Gets a value indicating whether allocation succeeded.
    /// </summary>
    public bool WasSelected => Status == PromptBudgetStatus.Selected;
}
