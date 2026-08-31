# Contribution workflow

## Commit discipline

Every foundation step follows this sequence:

1. Implement one bounded architectural increment.
2. Run repository verification.
3. Run build and tests applicable to the current state.
4. Inspect the staged diff.
5. Create one intentional commit.

## Commit naming

Foundation commits use the format:

```text
NNNN Short imperative description
```

Example:

```text
0001 Repository Bootstrap
```

## Architectural changes

A change to a frozen foundation requires:

1. A written alternative analysis.
2. An attempt to disprove the proposed replacement.
3. Compatibility analysis for both the detective game and Living World.
4. A new ADR documenting the decision and migration consequences.

## Prohibited shortcuts

- LLM output directly mutating world state.
- Gameplay references inside reusable engine projects.
- Keyword or hotword logic used as the semantic-understanding architecture.
- Unversioned package dependencies.
- Skipping tests or verification to preserve a preferred design.
