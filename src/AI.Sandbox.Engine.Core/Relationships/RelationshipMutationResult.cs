namespace AI.Sandbox.Engine.Core.Relationships;

/// <summary>
/// Describes one explicit immutable relationship-set mutation result.
/// </summary>
/// <typeparam name="TState">The exact relationship-state type.</typeparam>
public sealed class RelationshipMutationResult<TState>
    where TState : notnull, IRelationshipState
{
    internal RelationshipMutationResult(
        RelationshipMutationStatus status,
        RelationshipSet<TState> relationshipSet,
        RelationshipEntry<TState>? entry,
        RelationshipChangeReference change)
    {
        Status = status;
        RelationshipSet = relationshipSet;
        Entry = entry;
        Change = change;
    }

    /// <summary>
    /// Gets the explicit mutation status.
    /// </summary>
    public RelationshipMutationStatus Status { get; }

    /// <summary>
    /// Gets the resulting immutable relationship set.
    /// </summary>
    public RelationshipSet<TState> RelationshipSet { get; }

    /// <summary>
    /// Gets the affected current or removed entry when available.
    /// </summary>
    public RelationshipEntry<TState>? Entry { get; }

    /// <summary>
    /// Gets the attempted explicit change reference.
    /// </summary>
    public RelationshipChangeReference Change { get; }

    /// <summary>
    /// Gets a value indicating whether authoritative component replacement may
    /// proceed.
    /// </summary>
    public bool WasApplied =>
        Status is RelationshipMutationStatus.Added or
            RelationshipMutationStatus.Revised or
            RelationshipMutationStatus.Removed;
}
