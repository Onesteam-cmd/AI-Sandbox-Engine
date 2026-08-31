namespace AI.Sandbox.Engine.Core.Behavior;

/// <summary>
/// Represents one pure action-validation decision.
/// </summary>
/// <typeparam name="TCommand">
/// The exact command payload produced when approved.
/// </typeparam>
public sealed class ActionDecision<TCommand>
    where TCommand : global::AI.Sandbox.Engine.Core.Commands.IEngineCommand
{
    private readonly TCommand? command;
    private readonly ActionRejectionCode rejectionCode;

    private ActionDecision(
        ActionDecisionStatus status,
        TCommand? command,
        ActionRejectionCode rejectionCode)
    {
        Status = status;
        this.command = command;
        this.rejectionCode = rejectionCode;
    }

    /// <summary>
    /// Gets the validator decision status.
    /// </summary>
    public ActionDecisionStatus Status { get; }

    /// <summary>
    /// Gets a value indicating whether the proposal was approved.
    /// </summary>
    public bool IsApproved => Status == ActionDecisionStatus.Approved;

    /// <summary>
    /// Gets the exact approved command payload.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The decision is not approved.
    /// </exception>
    public TCommand Command =>
        Status == ActionDecisionStatus.Approved
            ? command!
            : throw new InvalidOperationException(
                "A rejected action decision has no command payload.");

    /// <summary>
    /// Gets the stable rejection code.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The decision is not rejected.
    /// </exception>
    public ActionRejectionCode RejectionCode =>
        Status == ActionDecisionStatus.Rejected
            ? rejectionCode
            : throw new InvalidOperationException(
                "An approved action decision has no rejection code.");

    /// <summary>
    /// Creates an approved decision carrying one exact command payload.
    /// </summary>
    /// <param name="command">The command payload to return to the host.</param>
    /// <returns>The approved decision.</returns>
    public static ActionDecision<TCommand> Approve(TCommand command)
    {
        BehaviorTypePolicy.EnsureExactType(
            typeof(TCommand),
            typeof(global::AI.Sandbox.Engine.Core.Commands.IEngineCommand),
            "command output");

        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        return new ActionDecision<TCommand>(
            ActionDecisionStatus.Approved,
            command,
            default);
    }

    /// <summary>
    /// Creates a rejected decision with one stable machine-readable reason.
    /// </summary>
    /// <param name="rejectionCode">The initialized rejection code.</param>
    /// <returns>The rejected decision.</returns>
    public static ActionDecision<TCommand> Reject(
        ActionRejectionCode rejectionCode)
    {
        BehaviorTypePolicy.EnsureExactType(
            typeof(TCommand),
            typeof(global::AI.Sandbox.Engine.Core.Commands.IEngineCommand),
            "command output");

        if (!rejectionCode.IsInitialized)
        {
            throw new ArgumentException(
                "The rejection code must be initialized.",
                nameof(rejectionCode));
        }

        return new ActionDecision<TCommand>(
            ActionDecisionStatus.Rejected,
            default,
            rejectionCode);
    }
}
