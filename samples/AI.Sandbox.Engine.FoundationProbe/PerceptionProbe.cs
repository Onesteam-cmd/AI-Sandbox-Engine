internal static class PerceptionProbe
{
    private sealed record CounterState(int Value) :
        global::AI.Sandbox.Engine.Core.WorldState.IWorldState;

    private sealed record TextStimulus(string Text) :
        global::AI.Sandbox.Engine.Core.Perception.IPerceptionStimulus;

    private readonly record struct TextSignal(string Text) :
        global::AI.Sandbox.Engine.Core.Perception.IPerceptionSignal;

    internal sealed record Result(
        string Status,
        bool EvaluatorWasExecuted,
        int EvaluatorEvaluationCount,
        bool ObservationPresent,
        bool ObservationIdentityValid,
        bool ObservationSnapshotMetadataValid,
        ushort ConfidenceBasisPoints,
        string SignalText,
        bool SnapshotReferencePreserved,
        bool WorldAuthorityUnchanged,
        int BeforeValue,
        int AfterValue,
        ulong BeforeVersion,
        ulong AfterVersion,
        ulong BeforeSimulationTick,
        ulong AfterSimulationTick);

    internal static Result Run()
    {
        var manager =
            global::AI.Sandbox.Engine.Core.WorldState
                .WorldStateManager<CounterState>.Create(
                    CreateWorldId(),
                    new CounterState(0));

        var evaluator = new CountingTextEvaluator();

        var processor =
            new global::AI.Sandbox.Engine.Core.Perception
                .PerceptionProcessorBuilder<CounterState>()
                .Add<TextStimulus, TextSignal>(evaluator)
                .Build(manager);

        var before = manager.Read();

        var envelope =
            global::AI.Sandbox.Engine.Core.Perception
                .PerceptionStimulusEnvelope<TextStimulus>.Create(
                    CreateStimulusId(),
                    CreateChannelId(),
                    CreateObserverId(),
                    before.WorldId,
                    before.Version,
                    before.SimulationTick,
                    new TextStimulus("candidate"));

        var result =
            processor.Evaluate<TextStimulus, TextSignal>(envelope);

        if (result.Status !=
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionEvaluationStatus.Observed ||
            !result.WasObserved ||
            !result.EvaluatorWasExecuted)
        {
            throw new global::System.InvalidOperationException(
                $"Perception evaluation failed: {result.Status}.");
        }

        if (evaluator.EvaluationCount != 1)
        {
            throw new global::System.InvalidOperationException(
                $"Perception evaluator execution count was " +
                $"{evaluator.EvaluationCount}.");
        }

        var observation = result.Observation;

        if (observation is null)
        {
            throw new global::System.InvalidOperationException(
                "Observed perception result did not contain an observation.");
        }

        var observationIdentityValid =
            observation.StimulusId == envelope.StimulusId &&
            observation.ChannelId == envelope.ChannelId &&
            observation.ObserverEntityId == envelope.ObserverEntityId;

        if (!observationIdentityValid)
        {
            throw new global::System.InvalidOperationException(
                "Perception observation identity did not match the envelope.");
        }

        var observationSnapshotMetadataValid =
            observation.WorldId == before.WorldId &&
            observation.WorldStateVersion == before.Version &&
            observation.SimulationTick == before.SimulationTick;

        if (!observationSnapshotMetadataValid)
        {
            throw new global::System.InvalidOperationException(
                "Perception observation snapshot metadata did not match.");
        }

        if (observation.Confidence.BasisPoints != 8_000)
        {
            throw new global::System.InvalidOperationException(
                $"Perception confidence was " +
                $"{observation.Confidence.BasisPoints}.");
        }

        if (observation.Signal != new TextSignal("candidate"))
        {
            throw new global::System.InvalidOperationException(
                $"Perception signal was '{observation.Signal.Text}'.");
        }

        var after = manager.Read();

        var snapshotReferencePreserved =
            global::System.Object.ReferenceEquals(before, result.Snapshot);

        var worldAuthorityUnchanged =
            global::System.Object.ReferenceEquals(before, after) &&
            before.State.Value == after.State.Value &&
            before.Version == after.Version &&
            before.SimulationTick == after.SimulationTick;

        if (!snapshotReferencePreserved)
        {
            throw new global::System.InvalidOperationException(
                "Perception result did not retain the authoritative snapshot.");
        }

        if (!worldAuthorityUnchanged)
        {
            throw new global::System.InvalidOperationException(
                "Perception evaluation changed authoritative World State.");
        }

        return new Result(
            result.Status.ToString(),
            result.EvaluatorWasExecuted,
            evaluator.EvaluationCount,
            true,
            observationIdentityValid,
            observationSnapshotMetadataValid,
            observation.Confidence.BasisPoints,
            observation.Signal.Text,
            snapshotReferencePreserved,
            worldAuthorityUnchanged,
            before.State.Value,
            after.State.Value,
            before.Version.Value,
            after.Version.Value,
            before.SimulationTick,
            after.SimulationTick);
    }

    private sealed class CountingTextEvaluator :
        global::AI.Sandbox.Engine.Core.Perception
            .IPerceptionEvaluator<
                CounterState,
                TextStimulus,
                TextSignal>
    {
        public int EvaluationCount { get; private set; }

        public global::AI.Sandbox.Engine.Core.Perception
            .PerceptionDecision<TextSignal> Evaluate(
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionContext<
                        CounterState,
                        TextStimulus> context)
        {
            EvaluationCount++;

            return global::AI.Sandbox.Engine.Core.Perception
                .PerceptionDecision<TextSignal>.Observe(
                    new TextSignal(context.Envelope.Payload.Text),
                    global::AI.Sandbox.Engine.Core.Perception
                        .PerceptionConfidence.FromBasisPoints(8_000));
        }
    }

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>
        CreateWorldId() =>
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind>.Parse(
                    "019b0000-0000-7000-8000-000000009000");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
        CreateObserverId() =>
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>.Parse(
                    "019b0000-0000-7000-9900-000000009000");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Perception.PerceptionStimulusIdKind>
        CreateStimulusId() =>
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionStimulusIdKind>.Parse(
                        "019b0000-0000-7000-9a00-000000009000");

    private static global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Perception.PerceptionChannelIdKind>
        CreateChannelId() =>
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Perception
                    .PerceptionChannelIdKind>.Parse(
                        "019b0000-0000-7000-9b00-000000009000");
}
