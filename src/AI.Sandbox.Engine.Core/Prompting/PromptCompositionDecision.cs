namespace AI.Sandbox.Engine.Core.Prompting;

/// <summary>
/// Represents one pure exact prompt-composition decision.
/// </summary>
/// <typeparam name="TDocument">The exact prompt-document payload type.</typeparam>
public sealed class PromptCompositionDecision<TDocument>
    where TDocument : IPromptDocument
{
    private readonly PromptDocumentEnvelope<TDocument>? document;
    private readonly PromptRejectionCode rejectionCode;

    private PromptCompositionDecision(
        PromptCompositionDecisionStatus status,
        PromptDocumentEnvelope<TDocument>? document,
        PromptRejectionCode rejectionCode)
    {
        Status = status;
        this.document = document;
        this.rejectionCode = rejectionCode;
    }

    /// <summary>
    /// Gets the pure composer decision status.
    /// </summary>
    public PromptCompositionDecisionStatus Status { get; }

    /// <summary>
    /// Gets the composed exact prompt document.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The decision did not compose a document.
    /// </exception>
    public PromptDocumentEnvelope<TDocument> Document =>
        Status == PromptCompositionDecisionStatus.Composed
            ? document!
            : throw new InvalidOperationException(
                "A rejected prompt decision has no document.");

    /// <summary>
    /// Gets the stable rejection code.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The decision is not rejected.
    /// </exception>
    public PromptRejectionCode RejectionCode =>
        Status == PromptCompositionDecisionStatus.Rejected
            ? rejectionCode
            : throw new InvalidOperationException(
                "A composed prompt decision has no rejection code.");

    /// <summary>
    /// Creates a successful composed decision.
    /// </summary>
    /// <param name="document">The exact composed prompt document.</param>
    /// <returns>The composed decision.</returns>
    public static PromptCompositionDecision<TDocument> Compose(
        PromptDocumentEnvelope<TDocument> document)
    {
        PromptTypePolicy.EnsureExactType(
            typeof(TDocument),
            typeof(IPromptDocument),
            "prompt document");
        ArgumentNullException.ThrowIfNull(document);

        return new PromptCompositionDecision<TDocument>(
            PromptCompositionDecisionStatus.Composed,
            document,
            default);
    }

    /// <summary>
    /// Creates an explicitly rejected composition decision.
    /// </summary>
    /// <param name="rejectionCode">The initialized stable rejection code.</param>
    /// <returns>The rejected decision.</returns>
    public static PromptCompositionDecision<TDocument> Reject(
        PromptRejectionCode rejectionCode)
    {
        PromptTypePolicy.EnsureExactType(
            typeof(TDocument),
            typeof(IPromptDocument),
            "prompt document");

        if (!rejectionCode.IsInitialized)
        {
            throw new ArgumentException(
                "The prompt rejection code must be initialized.",
                nameof(rejectionCode));
        }

        return new PromptCompositionDecision<TDocument>(
            PromptCompositionDecisionStatus.Rejected,
            null,
            rejectionCode);
    }
}
