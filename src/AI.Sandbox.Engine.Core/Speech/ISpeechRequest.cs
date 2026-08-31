namespace AI.Sandbox.Engine.Core.Speech;

/// <summary>
/// Marks one immutable exact request payload accepted by a speech adapter.
/// Recognition requests normally carry recorded audio; synthesis requests
/// normally carry text or structured utterance data.
/// </summary>
public interface ISpeechRequest
{
}
