namespace AI.Sandbox.Engine.Core.Conversation;

/// <summary>
/// Represents immutable world-scoped current state for one conversation.
/// </summary>
/// <typeparam name="TTopic">The exact conversation-topic payload type.</typeparam>
public sealed class ConversationState<TTopic>
    where TTopic : IConversationTopic
{
    private const int MaximumParticipantCount = 64;
    private readonly global::System.Collections.ObjectModel.ReadOnlyCollection<
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>
        participantEntityIds;

    private ConversationState(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ConversationIdKind>
            conversationId,
        ConversationRevision revision,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>[] participants,
        TTopic topic,
        ConversationTurn? lastTurn,
        bool isClosed)
    {
        WorldId = worldId;
        ConversationId = conversationId;
        Revision = revision;
        participantEntityIds = Array.AsReadOnly(participants);
        Topic = topic;
        LastTurn = lastTurn;
        IsClosed = isClosed;
    }

    /// <summary>
    /// Gets the authoritative world containing this conversation.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId { get; }

    /// <summary>
    /// Gets the stable conversation ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<ConversationIdKind>
        ConversationId { get; }

    /// <summary>
    /// Gets the optimistic current conversation revision.
    /// </summary>
    public ConversationRevision Revision { get; }

    /// <summary>
    /// Gets deterministically ordered conversation participants.
    /// </summary>
    public IReadOnlyList<
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>
        ParticipantEntityIds => participantEntityIds;

    /// <summary>
    /// Gets the exact current topic payload.
    /// </summary>
    public TTopic Topic { get; }

    /// <summary>
    /// Gets the most recently recorded completed turn, when present.
    /// </summary>
    public ConversationTurn? LastTurn { get; }

    /// <summary>
    /// Gets a value indicating whether the conversation was closed.
    /// </summary>
    public bool IsClosed { get; }

    /// <summary>
    /// Starts one immutable conversation state.
    /// </summary>
    /// <param name="worldId">The authoritative world ID.</param>
    /// <param name="conversationId">The externally assigned conversation ID.</param>
    /// <param name="participantEntityIds">
    /// From two through 64 unique participant entity IDs.
    /// </param>
    /// <param name="topic">The exact initial topic payload.</param>
    /// <returns>The validated initial state at revision one.</returns>
    public static ConversationState<TTopic> Start(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<ConversationIdKind>
            conversationId,
        IEnumerable<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>
            participantEntityIds,
        TTopic topic)
    {
        ConversationTypePolicy.EnsureExactType(
            typeof(TTopic),
            typeof(IConversationTopic),
            "conversation topic");

        EnsureNonEmpty(worldId, nameof(worldId));
        EnsureNonEmpty(conversationId, nameof(conversationId));
        ArgumentNullException.ThrowIfNull(participantEntityIds);

        if (topic is null)
        {
            throw new ArgumentNullException(nameof(topic));
        }

        var participants = participantEntityIds.ToArray();
        if (participants.Length is < 2 or > MaximumParticipantCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(participantEntityIds),
                "Conversations must contain from 2 through 64 participants.");
        }

        foreach (var participant in participants)
        {
            EnsureNonEmpty(participant, nameof(participantEntityIds));
        }

        var ordered = participants
            .OrderBy(static participant => participant)
            .ToArray();

        for (var index = 1; index < ordered.Length; index++)
        {
            if (ordered[index] == ordered[index - 1])
            {
                throw new ArgumentException(
                    "Conversation participant IDs must be unique.",
                    nameof(participantEntityIds));
            }
        }

        return new ConversationState<TTopic>(
            worldId,
            conversationId,
            ConversationRevision.Initial,
            ordered,
            topic,
            null,
            false);
    }

    /// <summary>
    /// Returns whether the supplied entity belongs to the current roster.
    /// </summary>
    /// <param name="entityId">The candidate participant ID.</param>
    /// <returns>
    /// <see langword="true"/> when the entity belongs to this conversation.
    /// </returns>
    public bool ContainsParticipant(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> entityId) =>
        !entityId.IsEmpty &&
        participantEntityIds.BinarySearch(entityId) >= 0;

    /// <summary>
    /// Records the next completed turn using optimistic revision semantics.
    /// </summary>
    /// <param name="expectedRevision">The expected current revision.</param>
    /// <param name="turn">The candidate next completed turn.</param>
    /// <returns>An explicit mutation result.</returns>
    public ConversationMutationResult<TTopic> RecordTurn(
        ConversationRevision expectedRevision,
        ConversationTurn turn)
    {
        ArgumentNullException.ThrowIfNull(turn);

        if (expectedRevision != Revision)
        {
            return Result(ConversationMutationStatus.RevisionConflict);
        }

        if (IsClosed)
        {
            return Result(ConversationMutationStatus.ConversationClosed);
        }

        var expectedTurnNumber =
            LastTurn is null
                ? ConversationTurnNumber.First
                : LastTurn.TurnNumber.Next();

        if (turn.TurnNumber != expectedTurnNumber)
        {
            return Result(ConversationMutationStatus.TurnSequenceConflict);
        }

        if (!ContainsParticipant(turn.SpeakerEntityId))
        {
            return Result(ConversationMutationStatus.SpeakerNotParticipant);
        }

        if (!IsAudienceValid(turn.SpeakerEntityId, turn.Audience))
        {
            return Result(ConversationMutationStatus.AudienceInvalid);
        }

        return Changed(
            ConversationMutationStatus.TurnRecorded,
            Topic,
            turn,
            false);
    }

    /// <summary>
    /// Revises the exact current topic using optimistic revision semantics.
    /// </summary>
    /// <param name="expectedRevision">The expected current revision.</param>
    /// <param name="topic">The candidate exact topic payload.</param>
    /// <returns>An explicit mutation result.</returns>
    public ConversationMutationResult<TTopic> ReviseTopic(
        ConversationRevision expectedRevision,
        TTopic topic)
    {
        ConversationTypePolicy.EnsureExactType(
            typeof(TTopic),
            typeof(IConversationTopic),
            "conversation topic");

        if (topic is null)
        {
            throw new ArgumentNullException(nameof(topic));
        }

        if (expectedRevision != Revision)
        {
            return Result(ConversationMutationStatus.RevisionConflict);
        }

        if (IsClosed)
        {
            return Result(ConversationMutationStatus.ConversationClosed);
        }

        if (EqualityComparer<TTopic>.Default.Equals(Topic, topic))
        {
            return Result(ConversationMutationStatus.Unchanged);
        }

        return Changed(
            ConversationMutationStatus.TopicRevised,
            topic,
            LastTurn,
            false);
    }

    /// <summary>
    /// Closes the conversation using optimistic revision semantics.
    /// </summary>
    /// <param name="expectedRevision">The expected current revision.</param>
    /// <returns>An explicit mutation result.</returns>
    public ConversationMutationResult<TTopic> Close(
        ConversationRevision expectedRevision)
    {
        if (expectedRevision != Revision)
        {
            return Result(ConversationMutationStatus.RevisionConflict);
        }

        if (IsClosed)
        {
            return Result(ConversationMutationStatus.Unchanged);
        }

        return Changed(
            ConversationMutationStatus.Closed,
            Topic,
            LastTurn,
            true);
    }

    private bool IsAudienceValid(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> speaker,
        AddressAudience audience)
    {
        ArgumentNullException.ThrowIfNull(audience);

        var targets = audience.TargetEntityIds;
        if (audience.Kind == AddressAudienceKind.None)
        {
            return targets.Count == 0;
        }

        if (targets.Count == 0)
        {
            return false;
        }

        foreach (var target in targets)
        {
            if (target == speaker || !ContainsParticipant(target))
            {
                return false;
            }
        }

        if (audience.Kind == AddressAudienceKind.SpecificParticipants)
        {
            return true;
        }

        if (audience.Kind != AddressAudienceKind.AllParticipants ||
            targets.Count != participantEntityIds.Count - 1)
        {
            return false;
        }

        var expected = participantEntityIds
            .Where(participant => participant != speaker)
            .ToArray();

        return expected.SequenceEqual(targets);
    }

    private ConversationMutationResult<TTopic> Result(
        ConversationMutationStatus status) =>
        new(status, this, false);

    private ConversationMutationResult<TTopic> Changed(
        ConversationMutationStatus status,
        TTopic topic,
        ConversationTurn? lastTurn,
        bool isClosed)
    {
        var state = new ConversationState<TTopic>(
            WorldId,
            ConversationId,
            Revision.Next(),
            participantEntityIds.ToArray(),
            topic,
            lastTurn,
            isClosed);

        return new ConversationMutationResult<TTopic>(
            status,
            state,
            true);
    }

    private static void EnsureNonEmpty<TKind>(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind> id,
        string parameterName)
    {
        if (id.IsEmpty)
        {
            throw new ArgumentException(
                "Identifiers must be non-empty.",
                parameterName);
        }
    }
}

internal static class ConversationParticipantSearchExtensions
{
    internal static int BinarySearch<T>(
        this global::System.Collections.ObjectModel.ReadOnlyCollection<T>
            values,
        T value)
        where T : IComparable<T>
    {
        var lower = 0;
        var upper = values.Count - 1;

        while (lower <= upper)
        {
            var middle = lower + ((upper - lower) / 2);
            var comparison = values[middle].CompareTo(value);
            if (comparison == 0)
            {
                return middle;
            }

            if (comparison < 0)
            {
                lower = middle + 1;
            }
            else
            {
                upper = middle - 1;
            }
        }

        return -1;
    }
}
