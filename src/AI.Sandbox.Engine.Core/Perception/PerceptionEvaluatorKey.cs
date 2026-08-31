namespace AI.Sandbox.Engine.Core.Perception;

internal readonly record struct PerceptionEvaluatorKey(
    Type StimulusType,
    Type SignalType);
