namespace AI.Sandbox.Engine.Core.StructuredOutput;

/// <summary>
/// Defines one synchronous pure decoder from an exact provider-neutral model
/// response to one exact structured-output payload.
/// </summary>
/// <typeparam name="TState">The immutable authoritative world-state type.</typeparam>
/// <typeparam name="TResponse">The exact model-response payload type.</typeparam>
/// <typeparam name="TOutput">The exact structured-output payload type.</typeparam>
public interface IStructuredOutputDecoder<TState, TResponse, TOutput>
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TResponse : global::AI.Sandbox.Engine.Core.Modeling.IModelResponse
    where TOutput : IStructuredModelOutput
{
    /// <summary>
    /// Decodes one response exactly once without changing authority.
    /// </summary>
    /// <param name="context">The stable structured-output context.</param>
    /// <returns>One exact payload or explicit rejection.</returns>
    public StructuredOutputDecision<TOutput> Decode(
        StructuredOutputContext<TState, TResponse> context);
}
