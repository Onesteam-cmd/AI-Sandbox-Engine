namespace AI.Sandbox.Engine.Core.Dialogue;

/// <summary>
/// Represents one immutable exact dialogue-orchestrator decision.
/// </summary>
/// <typeparam name="TDirective">The exact host-directive payload type.</typeparam>
/// <typeparam name="TCompletion">The exact completion payload type.</typeparam>
public sealed class DialogueOrchestrationDecision<TDirective, TCompletion>
    where TDirective : IDialogueDirective
    where TCompletion : IDialogueCompletion
{
    private readonly TDirective? directive;
    private readonly TCompletion? completion;
    private readonly DialogueRejectionCode rejectionCode;

    private DialogueOrchestrationDecision(
        DialogueOrchestrationDecisionStatus status,
        TDirective? directive,
        TCompletion? completion,
        DialogueRejectionCode rejectionCode)
    {
        Status = status;
        this.directive = directive;
        this.completion = completion;
        this.rejectionCode = rejectionCode;
    }

    /// <summary>Gets the semantic decision status.</summary>
    public DialogueOrchestrationDecisionStatus Status { get; }

    /// <summary>Gets the exact next-step directive.</summary>
    /// <exception cref="InvalidOperationException">The decision is not Continue.</exception>
    public TDirective Directive =>
        Status == DialogueOrchestrationDecisionStatus.Continue
            ? directive!
            : throw new InvalidOperationException(
                "Only a continue decision has a directive.");

    /// <summary>Gets the exact completion payload.</summary>
    /// <exception cref="InvalidOperationException">The decision is not Complete.</exception>
    public TCompletion Completion =>
        Status == DialogueOrchestrationDecisionStatus.Complete
            ? completion!
            : throw new InvalidOperationException(
                "Only a complete decision has a completion payload.");

    /// <summary>Gets the stable rejection code.</summary>
    /// <exception cref="InvalidOperationException">The decision is not Rejected.</exception>
    public DialogueRejectionCode RejectionCode =>
        Status == DialogueOrchestrationDecisionStatus.Rejected
            ? rejectionCode
            : throw new InvalidOperationException(
                "Only a rejected decision has a rejection code.");

    /// <summary>Continues host orchestration with one exact directive.</summary>
    /// <param name="directive">The exact next-step directive.</param>
    /// <returns>The validated continue decision.</returns>
    public static DialogueOrchestrationDecision<TDirective, TCompletion>
        Continue(TDirective directive)
    {
        EnsurePayloadTypes();
        if (directive is null)
        {
            throw new ArgumentNullException(nameof(directive));
        }

        return new DialogueOrchestrationDecision<TDirective, TCompletion>(
            DialogueOrchestrationDecisionStatus.Continue,
            directive,
            default,
            default);
    }

    /// <summary>Completes the exchange with one exact payload.</summary>
    /// <param name="completion">The exact completion payload.</param>
    /// <returns>The validated completion decision.</returns>
    public static DialogueOrchestrationDecision<TDirective, TCompletion>
        Complete(TCompletion completion)
    {
        EnsurePayloadTypes();
        if (completion is null)
        {
            throw new ArgumentNullException(nameof(completion));
        }

        return new DialogueOrchestrationDecision<TDirective, TCompletion>(
            DialogueOrchestrationDecisionStatus.Complete,
            default,
            completion,
            default);
    }

    /// <summary>Rejects orchestration with one stable code.</summary>
    /// <param name="rejectionCode">The initialized rejection code.</param>
    /// <returns>The validated rejection decision.</returns>
    public static DialogueOrchestrationDecision<TDirective, TCompletion>
        Reject(DialogueRejectionCode rejectionCode)
    {
        EnsurePayloadTypes();
        if (!rejectionCode.IsInitialized)
        {
            throw new ArgumentException(
                "The dialogue rejection code must be initialized.",
                nameof(rejectionCode));
        }

        return new DialogueOrchestrationDecision<TDirective, TCompletion>(
            DialogueOrchestrationDecisionStatus.Rejected,
            default,
            default,
            rejectionCode);
    }

    private static void EnsurePayloadTypes()
    {
        DialogueTypePolicy.EnsureExactType(
            typeof(TDirective),
            typeof(IDialogueDirective),
            "dialogue directive");
        DialogueTypePolicy.EnsureExactType(
            typeof(TCompletion),
            typeof(IDialogueCompletion),
            "dialogue completion");
    }
}
