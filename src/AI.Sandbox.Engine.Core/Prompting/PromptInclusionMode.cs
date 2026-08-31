namespace AI.Sandbox.Engine.Core.Prompting;

/// <summary>
/// Defines whether one candidate must be present or may be budget-selected.
/// </summary>
public enum PromptInclusionMode
{
    /// <summary>
    /// The candidate must fit; otherwise composition cannot start.
    /// </summary>
    Required = 0,

    /// <summary>
    /// The candidate may be omitted when the remaining budget is insufficient.
    /// </summary>
    Optional = 1,
}
