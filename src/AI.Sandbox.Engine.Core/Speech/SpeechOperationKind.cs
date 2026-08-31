namespace AI.Sandbox.Engine.Core.Speech;

/// <summary>
/// Identifies the provider-neutral speech operation performed by an adapter.
/// </summary>
public enum SpeechOperationKind
{
    /// <summary>Recognizes speech from supplied audio input.</summary>
    Recognition = 0,

    /// <summary>Synthesizes speech audio from supplied utterance input.</summary>
    Synthesis = 1,
}
