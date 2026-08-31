using AI.Sandbox.Engine.Core.Entities;
using AI.Sandbox.Engine.Core.Identifiers;

namespace AI.Sandbox.Engine.Core.Components;

internal interface IComponentStore
{
    public int Count { get; }

    public IReadOnlyList<Id<EntityIdKind>> EntityIds { get; }

    public IComponentStore Remove(
        Id<EntityIdKind> entityId,
        out bool wasRemoved);
}
