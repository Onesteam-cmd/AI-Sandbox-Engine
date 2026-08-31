namespace AI.Sandbox.Engine.Core.Prompting;

/// <summary>
/// Defines the explicit outcome of deterministic candidate budgeting.
/// </summary>
public enum PromptBudgetStatus
{
    /// <summary>
    /// Every required candidate fit and optional candidates were selected.
    /// </summary>
    Selected = 0,

    /// <summary>
    /// The required candidate cost exceeded the available budget.
    /// </summary>
    RequiredBudgetExceeded = 1,
}
