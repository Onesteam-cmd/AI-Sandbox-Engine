namespace AI.Sandbox.Engine.Core.Dialogue;

/// <summary>
/// Captures one immutable host request to choose the next dialogue directive or
/// complete an exchange.
/// </summary>
/// <typeparam name="TInput">The exact dialogue-input payload type.</typeparam>
public sealed record DialogueOrchestrationRequestEnvelope<TInput>
    where TInput : IDialogueInput
{
    private const int MaximumArtifactCount = 128;
    private readonly global::System.Collections.ObjectModel.ReadOnlyCollection<
        DialogueArtifactEnvelope> artifacts;

    private DialogueOrchestrationRequestEnvelope(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            DialogueOrchestrationIdKind> orchestrationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            DialogueOrchestratorIdKind> orchestratorId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<DialogueExchangeIdKind>
            exchangeId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Conversation.ConversationIdKind>
            conversationId,
        global::AI.Sandbox.Engine.Core.Conversation.ConversationRevision
            expectedConversationRevision,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
            perspectiveOwnerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
            sourceSpeakerEntityId,
        global::AI.Sandbox.Engine.Core.Conversation.AddressAudience audience,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            expectedWorldStateVersion,
        ulong expectedSimulationTick,
        TInput input,
        DialogueArtifactEnvelope[] artifacts)
    {
        OrchestrationId = orchestrationId;
        OrchestratorId = orchestratorId;
        ExchangeId = exchangeId;
        ConversationId = conversationId;
        ExpectedConversationRevision = expectedConversationRevision;
        PerspectiveOwnerEntityId = perspectiveOwnerEntityId;
        SourceSpeakerEntityId = sourceSpeakerEntityId;
        Audience = audience;
        WorldId = worldId;
        ExpectedWorldStateVersion = expectedWorldStateVersion;
        ExpectedSimulationTick = expectedSimulationTick;
        Input = input;
        this.artifacts = Array.AsReadOnly(artifacts);
    }

    /// <summary>Gets the externally assigned orchestration operation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        DialogueOrchestrationIdKind> OrchestrationId { get; }

    /// <summary>Gets the expected orchestrator ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        DialogueOrchestratorIdKind> OrchestratorId { get; }

    /// <summary>Gets the stable dialogue exchange ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<DialogueExchangeIdKind>
        ExchangeId { get; }

    /// <summary>Gets the stable conversation ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Conversation.ConversationIdKind>
        ConversationId { get; }

    /// <summary>Gets the optimistic expected conversation revision.</summary>
    public global::AI.Sandbox.Engine.Core.Conversation.ConversationRevision
        ExpectedConversationRevision { get; }

    /// <summary>Gets the subjective perspective owner.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
        PerspectiveOwnerEntityId { get; }

    /// <summary>Gets the source speaker of the exchange input.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
        SourceSpeakerEntityId { get; }

    /// <summary>Gets the currently resolved audience.</summary>
    public global::AI.Sandbox.Engine.Core.Conversation.AddressAudience
        Audience { get; }

    /// <summary>Gets the expected authoritative world ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId { get; }

    /// <summary>Gets the expected World State version.</summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
        ExpectedWorldStateVersion { get; }

    /// <summary>Gets the expected simulation tick.</summary>
    public ulong ExpectedSimulationTick { get; }

    /// <summary>Gets the exact immutable exchange input.</summary>
    public TInput Input { get; }

    /// <summary>Gets deterministically ordered prior exchange artifacts.</summary>
    public IReadOnlyList<DialogueArtifactEnvelope> Artifacts => artifacts;

    /// <summary>
    /// Creates one validated immutable orchestration request.
    /// </summary>
    /// <param name="orchestrationId">The operation ID.</param>
    /// <param name="orchestratorId">The expected orchestrator ID.</param>
    /// <param name="exchangeId">The dialogue exchange ID.</param>
    /// <param name="conversationId">The conversation ID.</param>
    /// <param name="expectedConversationRevision">Expected revision.</param>
    /// <param name="perspectiveOwnerEntityId">Perspective owner.</param>
    /// <param name="sourceSpeakerEntityId">Source speaker.</param>
    /// <param name="audience">Resolved audience.</param>
    /// <param name="worldId">Expected world ID.</param>
    /// <param name="expectedWorldStateVersion">Expected state version.</param>
    /// <param name="expectedSimulationTick">Expected simulation tick.</param>
    /// <param name="input">Exact exchange input.</param>
    /// <param name="artifacts">Zero through 128 correlated artifacts.</param>
    /// <returns>The validated request.</returns>
    public static DialogueOrchestrationRequestEnvelope<TInput> Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            DialogueOrchestrationIdKind> orchestrationId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            DialogueOrchestratorIdKind> orchestratorId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<DialogueExchangeIdKind>
            exchangeId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Conversation.ConversationIdKind>
            conversationId,
        global::AI.Sandbox.Engine.Core.Conversation.ConversationRevision
            expectedConversationRevision,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
            perspectiveOwnerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>
            sourceSpeakerEntityId,
        global::AI.Sandbox.Engine.Core.Conversation.AddressAudience audience,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
            expectedWorldStateVersion,
        ulong expectedSimulationTick,
        TInput input,
        IEnumerable<DialogueArtifactEnvelope> artifacts)
    {
        DialogueTypePolicy.EnsureExactType(
            typeof(TInput),
            typeof(IDialogueInput),
            "dialogue input");

        EnsureNonEmpty(orchestrationId, nameof(orchestrationId));
        EnsureNonEmpty(orchestratorId, nameof(orchestratorId));
        EnsureNonEmpty(exchangeId, nameof(exchangeId));
        EnsureNonEmpty(conversationId, nameof(conversationId));
        EnsureNonEmpty(
            perspectiveOwnerEntityId,
            nameof(perspectiveOwnerEntityId));
        EnsureNonEmpty(sourceSpeakerEntityId, nameof(sourceSpeakerEntityId));
        EnsureNonEmpty(worldId, nameof(worldId));

        if (!expectedConversationRevision.IsInitialized)
        {
            throw new ArgumentException(
                "The expected conversation revision must be initialized.",
                nameof(expectedConversationRevision));
        }
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        ArgumentNullException.ThrowIfNull(audience);
        ArgumentNullException.ThrowIfNull(artifacts);

        var artifactArray = artifacts.ToArray();
        if (artifactArray.Length > MaximumArtifactCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(artifacts),
                "Dialogue requests may contain at most 128 artifacts.");
        }

        foreach (var artifact in artifactArray)
        {
            ArgumentNullException.ThrowIfNull(artifact);

            if (artifact.ExchangeId != exchangeId ||
                artifact.ConversationId != conversationId ||
                artifact.PerspectiveOwnerEntityId != perspectiveOwnerEntityId ||
                artifact.WorldId != worldId ||
                artifact.WorldStateVersion != expectedWorldStateVersion ||
                artifact.SimulationTick != expectedSimulationTick)
            {
                throw new ArgumentException(
                    "Every dialogue artifact must match request correlation.",
                    nameof(artifacts));
            }
        }

        var ordered = artifactArray
            .OrderBy(static artifact => artifact.Sequence)
            .ThenBy(static artifact => artifact.ArtifactId)
            .ToArray();

        var artifactIds = new HashSet<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                DialogueArtifactIdKind>>();
        var sequences = new HashSet<DialogueArtifactSequence>();

        foreach (var artifact in ordered)
        {
            if (!artifactIds.Add(artifact.ArtifactId))
            {
                throw new ArgumentException(
                    "Dialogue artifact IDs must be unique.",
                    nameof(artifacts));
            }

            if (!sequences.Add(artifact.Sequence))
            {
                throw new ArgumentException(
                    "Dialogue artifact sequences must be unique.",
                    nameof(artifacts));
            }
        }

        return new DialogueOrchestrationRequestEnvelope<TInput>(
            orchestrationId,
            orchestratorId,
            exchangeId,
            conversationId,
            expectedConversationRevision,
            perspectiveOwnerEntityId,
            sourceSpeakerEntityId,
            audience,
            worldId,
            expectedWorldStateVersion,
            expectedSimulationTick,
            input,
            ordered);
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
