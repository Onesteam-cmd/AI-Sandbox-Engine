namespace AI.Sandbox.Engine.Core.StructuredOutput;

/// <summary>
/// Represents one immutable exact decoder decision.
/// </summary>
/// <typeparam name="TOutput">The exact structured-output payload type.</typeparam>
public sealed class StructuredOutputDecision<TOutput>
    where TOutput : IStructuredModelOutput
{
    private readonly TOutput? payload;
    private readonly StructuredOutputRejectionCode rejectionCode;

    private StructuredOutputDecision(
        StructuredOutputDecisionStatus status,
        TOutput? payload,
        StructuredOutputRejectionCode rejectionCode)
    {
        Status = status;
        this.payload = payload;
        this.rejectionCode = rejectionCode;
    }

    /// <summary>
    /// Gets the decoder decision status.
    /// </summary>
    public StructuredOutputDecisionStatus Status { get; }

    /// <summary>
    /// Gets the exact decoded payload.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The decision is not decoded.
    /// </exception>
    public TOutput Payload =>
        Status == StructuredOutputDecisionStatus.Decoded
            ? payload!
            : throw new InvalidOperationException(
                "A rejected structured-output decision has no payload.");

    /// <summary>
    /// Gets the stable rejection code.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The decision is not rejected.
    /// </exception>
    public StructuredOutputRejectionCode RejectionCode =>
        Status == StructuredOutputDecisionStatus.Rejected
            ? rejectionCode
            : throw new InvalidOperationException(
                "A decoded structured-output decision has no rejection code.");

    /// <summary>
    /// Creates one decoded decision carrying one exact payload.
    /// </summary>
    /// <param name="payload">The exact decoded payload.</param>
    /// <returns>The decoded decision.</returns>
    public static StructuredOutputDecision<TOutput> Decode(TOutput payload)
    {
        StructuredOutputTypePolicy.EnsureExactType(
            typeof(TOutput),
            typeof(IStructuredModelOutput),
            "structured model output");

        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return new StructuredOutputDecision<TOutput>(
            StructuredOutputDecisionStatus.Decoded,
            payload,
            default);
    }

    /// <summary>
    /// Creates one explicitly rejected decoder decision.
    /// </summary>
    /// <param name="rejectionCode">The initialized stable rejection code.</param>
    /// <returns>The rejected decision.</returns>
    public static StructuredOutputDecision<TOutput> Reject(
        StructuredOutputRejectionCode rejectionCode)
    {
        StructuredOutputTypePolicy.EnsureExactType(
            typeof(TOutput),
            typeof(IStructuredModelOutput),
            "structured model output");

        if (!rejectionCode.IsInitialized)
        {
            throw new ArgumentException(
                "The structured-output rejection code must be initialized.",
                nameof(rejectionCode));
        }

        return new StructuredOutputDecision<TOutput>(
            StructuredOutputDecisionStatus.Rejected,
            default,
            rejectionCode);
    }
}
