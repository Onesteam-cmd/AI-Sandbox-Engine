namespace AI.Sandbox.Engine.Core.Perception;

/// <summary>
/// Identifies the externally visible outcome of one perception evaluation.
/// </summary>
public enum PerceptionEvaluationStatus
{
    /// <summary>
    /// A subjective signal was produced.
    /// </summary>
    Observed = 0,

    /// <summary>
    /// The evaluator deliberately ignored the candidate stimulus.
    /// </summary>
    Ignored = 1,

    /// <summary>
    /// No evaluator is registered for the exact stimulus/signal pair.
    /// </summary>
    EvaluatorNotRegistered = 2,

    /// <summary>
    /// The candidate targets a different world.
    /// </summary>
    WorldMismatch = 3,

    /// <summary>
    /// The expected World State version is stale or changed during evaluation.
    /// </summary>
    VersionConflict = 4,

    /// <summary>
    /// The candidate was formed at a different logical simulation tick.
    /// </summary>
    SimulationTickMismatch = 5,
}
