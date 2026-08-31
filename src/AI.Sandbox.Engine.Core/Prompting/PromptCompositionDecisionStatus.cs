namespace AI.Sandbox.Engine.Core.Prompting;

/// <summary>
/// Defines the pure decision returned by one exact prompt composer.
/// </summary>
public enum PromptCompositionDecisionStatus
{
    /// <summary>
    /// The composer produced one exact prompt document.
    /// </summary>
    Composed = 0,

    /// <summary>
    /// The composer explicitly rejected composition.
    /// </summary>
    Rejected = 1,
}
