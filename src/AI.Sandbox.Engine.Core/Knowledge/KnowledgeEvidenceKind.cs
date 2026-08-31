namespace AI.Sandbox.Engine.Core.Knowledge;

/// <summary>
/// Identifies the broad provenance category of one current claim revision.
/// </summary>
public enum KnowledgeEvidenceKind
{
    /// <summary>
    /// The claim was derived from one subjective perception observation.
    /// </summary>
    Perception = 0,

    /// <summary>
    /// The claim was communicated by another entity.
    /// </summary>
    Communication = 1,

    /// <summary>
    /// The claim was inferred from already available information.
    /// </summary>
    Inference = 2,

    /// <summary>
    /// The claim entered through an explicit external or authored source.
    /// </summary>
    External = 3,
}
