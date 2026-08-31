namespace AI.Sandbox.Engine.Core.Knowledge;

/// <summary>
/// Marks one immutable exact subjective claim type.
/// </summary>
/// <remarks>
/// A claim may be wrong, incomplete, outdated, or contradictory. Its presence
/// in a knowledge set does not make it an authoritative World State fact.
/// </remarks>
public interface IKnowledgeClaim
{
}
