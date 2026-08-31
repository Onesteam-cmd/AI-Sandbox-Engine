namespace AI.Sandbox.Engine.Core.Tests;

public sealed class HostRuntimeDeadlineRetryDecisionTests
{
    private readonly record struct RequestPayload(string Text) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRequest;

    private readonly record struct RetryReason(string Code) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRetryReason;

    private record OpenRetryReason(string Code) :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRetryReason;

    private sealed class CountingRetryReason :
        global::AI.Sandbox.Engine.Core.HostRuntime.IHostRuntimeRetryReason
    {
        public int InvocationCount { get; private set; }

        public void Invoke() => InvocationCount++;
    }

    [Xunit.Fact]
    public void DeadlinePolicyIdsAndBoundsAreValidated()
    {
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDeadline.Create(default, 10));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDeadline.Create(ClockId(), -1));
        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryPolicy.Create(default, 2, 1));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryPolicy.Create(PolicyId(), 0, 1));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryPolicy.Create(
                    PolicyId(),
                    global::AI.Sandbox.Engine.Core.HostRuntime
                        .HostRuntimeRetryPolicy.MaximumAttemptCount + 1,
                    1));
        Xunit.Assert.Throws<ArgumentOutOfRangeException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryPolicy.Create(PolicyId(), 2, -1));
    }

    [Xunit.Fact]
    public void DeadlineAndPolicyPreserveExternalAuthority()
    {
        var deadline = global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDeadline.Create(ClockId(), 100);
        var policy = Policy(maximumAttempts: 4, retryDelayTicks: 7);

        Xunit.Assert.Equal(ClockId(), deadline.ClockId);
        Xunit.Assert.Equal(100, deadline.DueTick);
        Xunit.Assert.Equal(PolicyId(), policy.PolicyId);
        Xunit.Assert.Equal(4, policy.MaximumAttempts);
        Xunit.Assert.Equal(7, policy.RetryDelayTicks);
    }

    [Xunit.Fact]
    public void FailedRequestProducesDeterministicRetryDecision()
    {
        var failed = Terminal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Failed);
        var reason = new RetryReason("transient");

        var decision = Decide(
            failed,
            expectedRevision: failed.Revision,
            observedTick: 20,
            completedAttemptNumber: 1,
            policy: Policy(3, 5),
            deadline: Deadline(100),
            reason);

        Xunit.Assert.True(decision.ShouldRetry);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryDecisionStatus.RetryAllowed,
            decision.Status);
        Xunit.Assert.Same(failed, decision.Request);
        Xunit.Assert.Equal(1, decision.CompletedAttemptNumber);
        Xunit.Assert.Equal(2, decision.NextAttemptNumber);
        Xunit.Assert.Equal<long?>(25L, decision.RetryAtTick);
        Xunit.Assert.Equal(reason, decision.Reason);
    }

    [Xunit.Fact]
    public void RejectedRequestMayRetryWithoutDeadline()
    {
        var rejected = Terminal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Rejected);

        var decision = Decide(
            rejected,
            rejected.Revision,
            observedTick: 40,
            completedAttemptNumber: 2,
            policy: Policy(3, 0),
            deadline: null,
            new RetryReason("provider_rejected"));

        Xunit.Assert.True(decision.ShouldRetry);
        Xunit.Assert.Null(decision.Deadline);
        Xunit.Assert.Equal<long?>(40L, decision.RetryAtTick);
        Xunit.Assert.Equal(3, decision.NextAttemptNumber);
    }

    [Xunit.Fact]
    public void DeadlineExceededIsExplicit()
    {
        var failed = Terminal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Failed);

        var expired = Decide(
            failed,
            failed.Revision,
            observedTick: 100,
            completedAttemptNumber: 1,
            policy: Policy(3, 1),
            deadline: Deadline(100),
            new RetryReason("expired"));
        var delayCrossesDeadline = Decide(
            failed,
            failed.Revision,
            observedTick: 95,
            completedAttemptNumber: 1,
            policy: Policy(3, 5),
            deadline: Deadline(100),
            new RetryReason("crosses"));

        foreach (var decision in new[] { expired, delayCrossesDeadline })
        {
            Xunit.Assert.Equal(
                global::AI.Sandbox.Engine.Core.HostRuntime
                    .HostRuntimeRetryDecisionStatus.DeadlineExceeded,
                decision.Status);
            Xunit.Assert.False(decision.ShouldRetry);
            Xunit.Assert.Equal(0, decision.NextAttemptNumber);
            Xunit.Assert.Null(decision.RetryAtTick);
        }
    }

    [Xunit.Fact]
    public void AttemptLimitAndInvalidStateAreExplicit()
    {
        var failed = Terminal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Failed);
        var pending = Pending();

        var limited = Decide(
            failed,
            failed.Revision,
            observedTick: 1,
            completedAttemptNumber: 3,
            policy: Policy(3, 0),
            deadline: null,
            new RetryReason("limit"));
        var invalid = Decide(
            pending,
            pending.Revision,
            observedTick: 1,
            completedAttemptNumber: 1,
            policy: Policy(3, 0),
            deadline: null,
            new RetryReason("invalid"));

        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryDecisionStatus.AttemptLimitReached,
            limited.Status);
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryDecisionStatus.InvalidRequestState,
            invalid.Status);
    }

    [Xunit.Fact]
    public void StaleRevisionAndClockMismatchAreExplicit()
    {
        var failed = Terminal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Failed);

        var stale = Decide(
            failed,
            failed.Revision + 1,
            observedTick: 1,
            completedAttemptNumber: 1,
            policy: Policy(3, 0),
            deadline: null,
            new RetryReason("stale"));
        Xunit.Assert.Equal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryDecisionStatus.StaleRevision,
            stale.Status);
        Xunit.Assert.Same(failed, stale.Request);

        Xunit.Assert.Throws<ArgumentException>(
            () => global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryDecisionFlow.Decide(
                    failed,
                    failed.Revision,
                    OtherClockId(),
                    observedTick: 1,
                    completedAttemptNumber: 1,
                    Policy(3, 0),
                    Deadline(100),
                    new RetryReason("clock")));
    }

    [Xunit.Fact]
    public void ContractsDoNotWaitScheduleRetryOrExecuteReason()
    {
        var failed = Terminal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState.Failed);
        var reason = new CountingRetryReason();

        var decision = Decide(
            failed,
            failed.Revision,
            observedTick: 5,
            completedAttemptNumber: 1,
            policy: Policy(3, 10),
            deadline: Deadline(100),
            reason);

        Xunit.Assert.True(decision.ShouldRetry);
        Xunit.Assert.Equal(0, reason.InvocationCount);
        Xunit.Assert.Same(failed, decision.Request);

        Xunit.Assert.Throws<ArgumentException>(
            () => Decide(
                failed,
                failed.Revision,
                observedTick: 5,
                completedAttemptNumber: 1,
                policy: Policy(3, 10),
                deadline: Deadline(100),
                new OpenRetryReason("invalid")));
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRetryDecision<RequestPayload> Decide(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestEnvelope<RequestPayload> request,
            long expectedRevision,
            long observedTick,
            int completedAttemptNumber,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryPolicy policy,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeDeadline? deadline,
            global::AI.Sandbox.Engine.Core.HostRuntime
                .IHostRuntimeRetryReason reason) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRetryDecisionFlow.Decide(
                request,
                expectedRevision,
                ClockId(),
                observedTick,
                completedAttemptNumber,
                policy,
                deadline,
                reason);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRetryPolicy Policy(
            int maximumAttempts,
            long retryDelayTicks) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRetryPolicy.Create(
                PolicyId(),
                maximumAttempts,
                retryDelayTicks);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeDeadline Deadline(long dueTick) =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeDeadline.Create(ClockId(), dueTick);

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRequestEnvelope<RequestPayload> Terminal(
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestState state)
    {
        var pending = Pending();
        return global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.Finalize(
                pending,
                pending.Revision,
                state).Envelope;
    }

    private static global::AI.Sandbox.Engine.Core.HostRuntime
        .HostRuntimeRequestEnvelope<RequestPayload> Pending() =>
        global::AI.Sandbox.Engine.Core.HostRuntime
            .HostRuntimeRequestFlow.Create(
                RequestId(),
                RuntimeId(),
                OperationId(),
                CorrelationId(),
                default,
                new RequestPayload("payload"));

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>
        Id<TKind>(int suffix) =>
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind>.Parse(
            $"019d0000-0000-7000-8000-{suffix:D12}");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRequestIdKind>
        RequestId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRequestIdKind>(3201);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeInstanceIdKind>
        RuntimeId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeInstanceIdKind>(3202);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeOperationIdKind>
        OperationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeOperationIdKind>(3203);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeCorrelationIdKind>
        CorrelationId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeCorrelationIdKind>(3204);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        ClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(3205);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeClockIdKind>
        OtherClockId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeClockIdKind>(3206);

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.HostRuntime.HostRuntimeRetryPolicyIdKind>
        PolicyId() => Id<
            global::AI.Sandbox.Engine.Core.HostRuntime
                .HostRuntimeRetryPolicyIdKind>(3207);
}
