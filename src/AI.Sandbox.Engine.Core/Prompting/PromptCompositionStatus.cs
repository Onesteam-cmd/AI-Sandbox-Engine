namespace AI.Sandbox.Engine.Core.Prompting;

/// <summary>
/// Defines the complete outcome of one prompt-composition invocation.
/// </summary>
public enum PromptCompositionStatus
{
    /// <summary>
    /// The request world did not match current authority.
    /// </summary>
    WorldMismatch = 0,

    /// <summary>
    /// The request version was stale or authority changed during composition.
    /// </summary>
    VersionConflict = 1,

    /// <summary>
    /// The request tick was stale or changed during composition.
    /// </summary>
    SimulationTickMismatch = 2,

    /// <summary>
    /// One candidate belonged to another world.
    /// </summary>
    CandidateWorldMismatch = 3,

    /// <summary>
    /// One candidate belonged to another subjective owner.
    /// </summary>
    CandidateOwnerMismatch = 4,

    /// <summary>
    /// Candidate IDs were not unique.
    /// </summary>
    DuplicateCandidate = 5,

    /// <summary>
    /// Required candidate cost exceeded the request budget.
    /// </summary>
    RequiredBudgetExceeded = 6,

    /// <summary>
    /// The document belonged to another world.
    /// </summary>
    ResultWorldMismatch = 7,

    /// <summary>
    /// The document belonged to another subjective owner.
    /// </summary>
    ResultOwnerMismatch = 8,

    /// <summary>
    /// The document identified another composer.
    /// </summary>
    ResultComposerMismatch = 9,

    /// <summary>
    /// The document estimated cost exceeded the request budget.
    /// </summary>
    ResultBudgetExceeded = 10,

    /// <summary>
    /// One exact prompt document was composed and validated.
    /// </summary>
    Composed = 11,

    /// <summary>
    /// The composer explicitly rejected composition.
    /// </summary>
    Rejected = 12,
}
