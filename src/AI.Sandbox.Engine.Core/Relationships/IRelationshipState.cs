namespace AI.Sandbox.Engine.Core.Relationships;

/// <summary>
/// Marks one immutable, exact relationship-state payload defined by a domain
/// module.
/// </summary>
/// <remarks>
/// Core intentionally does not prescribe a closed set of social axes. Concrete
/// modules define the payload required by their own simulation rules.
/// </remarks>
public interface IRelationshipState
{
}
