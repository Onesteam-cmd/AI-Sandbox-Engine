namespace AI.Sandbox.Engine.Core.Behavior;

/// <summary>
/// Represents the explicit result of one action-validation invocation.
/// </summary>
/// <typeparam name="TAction">The exact action-proposal payload type.</typeparam>
/// <typeparam name="TCommand">
/// The exact command payload type produced when approved.
/// </typeparam>
public sealed class ActionValidationResult<TAction, TCommand>
    where TAction : IActionProposal
    where TCommand : global::AI.Sandbox.Engine.Core.Commands.IEngineCommand
{
    private ActionValidationResult(
        ActionValidationStatus status,
        ActionProposalEnvelope<TAction> proposal,
        bool validatorWasInvoked,
        ActionDecision<TCommand>? decision)
    {
        Status = status;
        Proposal = proposal;
        ValidatorWasInvoked = validatorWasInvoked;
        Decision = decision;
    }

    /// <summary>
    /// Gets the complete invocation status.
    /// </summary>
    public ActionValidationStatus Status { get; }

    /// <summary>
    /// Gets the proposal supplied to validation.
    /// </summary>
    public ActionProposalEnvelope<TAction> Proposal { get; }

    /// <summary>
    /// Gets a value indicating whether the validator was invoked.
    /// </summary>
    public bool ValidatorWasInvoked { get; }

    /// <summary>
    /// Gets the stable validator decision when it was not discarded by a
    /// post-evaluation authority conflict.
    /// </summary>
    public ActionDecision<TCommand>? Decision { get; }

    /// <summary>
    /// Gets a value indicating whether a stable validator decision is present.
    /// </summary>
    public bool HasStableDecision => Decision is not null;

    /// <summary>
    /// Gets a value indicating whether a stable approved decision was returned.
    /// </summary>
    public bool WasApproved => Status == ActionValidationStatus.Approved;

    internal static ActionValidationResult<TAction, TCommand> NotEvaluated(
        ActionValidationStatus status,
        ActionProposalEnvelope<TAction> proposal) =>
        new(status, proposal, false, null);

    internal static ActionValidationResult<TAction, TCommand> Discarded(
        ActionValidationStatus status,
        ActionProposalEnvelope<TAction> proposal) =>
        new(status, proposal, true, null);

    internal static ActionValidationResult<TAction, TCommand> Evaluated(
        ActionValidationStatus status,
        ActionProposalEnvelope<TAction> proposal,
        ActionDecision<TCommand> decision) =>
        new(status, proposal, true, decision);
}
