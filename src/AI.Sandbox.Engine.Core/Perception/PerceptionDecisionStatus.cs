namespace AI.Sandbox.Engine.Core.Perception;

/// <summary>
/// Identifies the pure decision made by a perception evaluator.
/// </summary>
public enum PerceptionDecisionStatus
{
    /// <summary>
    /// The observer receives one subjective signal.
    /// </summary>
    Observed = 0,

    /// <summary>
    /// The candidate stimulus does not become a signal for this observer.
    /// </summary>
    Ignored = 1,
}
