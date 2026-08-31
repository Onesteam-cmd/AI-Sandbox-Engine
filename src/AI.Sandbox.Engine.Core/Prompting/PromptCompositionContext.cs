namespace AI.Sandbox.Engine.Core.Prompting;

/// <summary>
/// Supplies one composer with a stable snapshot, exact request, deterministic
/// budget allocation, and explicit composer identity.
/// </summary>
/// <typeparam name="TState">The immutable authoritative world-state type.</typeparam>
/// <typeparam name="TRequest">The exact prompt-request payload type.</typeparam>
/// <typeparam name="TContent">The exact candidate-content payload type.</typeparam>
public sealed class PromptCompositionContext<TState, TRequest, TContent>
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TRequest : IPromptRequest
    where TContent : IPromptContent
{
    internal PromptCompositionContext(
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateSnapshot<TState>
            snapshot,
        PromptRequestEnvelope<TRequest> request,
        PromptBudgetResult<TContent> budgetResult,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<PromptComposerIdKind>
            composerId)
    {
        Snapshot = snapshot;
        Request = request;
        BudgetResult = budgetResult;
        ComposerId = composerId;
    }

    /// <summary>
    /// Gets the stable authoritative snapshot used for composition.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateSnapshot<TState>
        Snapshot { get; }

    /// <summary>
    /// Gets the exact owner-scoped prompt request.
    /// </summary>
    public PromptRequestEnvelope<TRequest> Request { get; }

    /// <summary>
    /// Gets the successful deterministic budget allocation.
    /// </summary>
    public PromptBudgetResult<TContent> BudgetResult { get; }

    /// <summary>
    /// Gets the exact composer identity for document provenance.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<PromptComposerIdKind>
        ComposerId { get; }
}
