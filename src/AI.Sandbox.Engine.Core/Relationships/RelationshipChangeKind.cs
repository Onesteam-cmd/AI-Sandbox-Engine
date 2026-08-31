namespace AI.Sandbox.Engine.Core.Relationships;

/// <summary>
/// Identifies the broad provenance category of one explicit relationship
/// change.
/// </summary>
public enum RelationshipChangeKind
{
    /// <summary>
    /// The relationship changed because of a direct interaction.
    /// </summary>
    Interaction = 0,

    /// <summary>
    /// The relationship changed because of communicated information.
    /// </summary>
    Communication = 1,

    /// <summary>
    /// The relationship changed from a perception observation.
    /// </summary>
    Perception = 2,

    /// <summary>
    /// The relationship changed from current subjective knowledge.
    /// </summary>
    Knowledge = 3,

    /// <summary>
    /// The relationship changed from a retained memory episode.
    /// </summary>
    Memory = 4,

    /// <summary>
    /// The relationship changed from an explicit inference.
    /// </summary>
    Inference = 5,

    /// <summary>
    /// The relationship changed from an explicit authored or imported source.
    /// </summary>
    External = 6,
}
