namespace AI.Sandbox.Engine.Core.Prompting;

/// <summary>
/// Represents the explicit result of one prompt-composition invocation.
/// </summary>
/// <typeparam name="TRequest">The exact prompt-request payload type.</typeparam>
/// <typeparam name="TContent">The exact candidate-content payload type.</typeparam>
/// <typeparam name="TDocument">The exact prompt-document payload type.</typeparam>
public sealed class PromptCompositionResult<TRequest, TContent, TDocument>
    where TRequest : IPromptRequest
    where TContent : IPromptContent
    where TDocument : IPromptDocument
{
    private PromptCompositionResult(
        PromptCompositionStatus status,
        PromptRequestEnvelope<TRequest> request,
        bool composerWasInvoked,
        PromptBudgetResult<TContent>? budgetResult,
        PromptCompositionDecision<TDocument>? decision)
    {
        Status = status;
        Request = request;
        ComposerWasInvoked = composerWasInvoked;
        BudgetResult = budgetResult;
        Decision = decision;
    }

    /// <summary>
    /// Gets the complete invocation status.
    /// </summary>
    public PromptCompositionStatus Status { get; }

    /// <summary>
    /// Gets the exact request supplied to composition.
    /// </summary>
    public PromptRequestEnvelope<TRequest> Request { get; }

    /// <summary>
    /// Gets a value indicating whether the composer was invoked.
    /// </summary>
    public bool ComposerWasInvoked { get; }

    /// <summary>
    /// Gets the deterministic budget result when candidate validation completed.
    /// </summary>
    public PromptBudgetResult<TContent>? BudgetResult { get; }

    /// <summary>
    /// Gets the stable composer decision when authority did not invalidate it.
    /// </summary>
    public PromptCompositionDecision<TDocument>? Decision { get; }

    /// <summary>
    /// Gets a value indicating whether a prompt was composed and validated.
    /// </summary>
    public bool WasComposed => Status == PromptCompositionStatus.Composed;

    internal static PromptCompositionResult<TRequest, TContent, TDocument>
        NotComposed(
            PromptCompositionStatus status,
            PromptRequestEnvelope<TRequest> request) =>
        new(status, request, false, null, null);

    internal static PromptCompositionResult<TRequest, TContent, TDocument>
        BudgetRejected(
            PromptRequestEnvelope<TRequest> request,
            PromptBudgetResult<TContent> budgetResult) =>
        new(
            PromptCompositionStatus.RequiredBudgetExceeded,
            request,
            false,
            budgetResult,
            null);

    internal static PromptCompositionResult<TRequest, TContent, TDocument>
        Discarded(
            PromptCompositionStatus status,
            PromptRequestEnvelope<TRequest> request,
            PromptBudgetResult<TContent> budgetResult) =>
        new(status, request, true, budgetResult, null);

    internal static PromptCompositionResult<TRequest, TContent, TDocument>
        Evaluated(
            PromptCompositionStatus status,
            PromptRequestEnvelope<TRequest> request,
            PromptBudgetResult<TContent> budgetResult,
            PromptCompositionDecision<TDocument> decision) =>
        new(status, request, true, budgetResult, decision);
}
