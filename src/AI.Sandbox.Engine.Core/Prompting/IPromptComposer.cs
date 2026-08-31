namespace AI.Sandbox.Engine.Core.Prompting;

/// <summary>
/// Defines one synchronous pure composer for exact prompt contracts.
/// </summary>
/// <typeparam name="TState">The immutable authoritative world-state type.</typeparam>
/// <typeparam name="TRequest">The exact prompt-request payload type.</typeparam>
/// <typeparam name="TContent">The exact candidate-content payload type.</typeparam>
/// <typeparam name="TDocument">The exact prompt-document payload type.</typeparam>
public interface IPromptComposer<TState, TRequest, TContent, TDocument>
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TRequest : IPromptRequest
    where TContent : IPromptContent
    where TDocument : IPromptDocument
{
    /// <summary>
    /// Composes exactly once without calling a model provider or changing state.
    /// </summary>
    /// <param name="context">The stable budgeted composition context.</param>
    /// <returns>One exact document or an explicit rejection.</returns>
    public PromptCompositionDecision<TDocument> Compose(
        PromptCompositionContext<TState, TRequest, TContent> context);
}
