# ADR-0061: Host Recovery Adjacent-Collection Projection and Multi-Sequence Checkpoint-Range Continuity Validation Contracts

## Status

Accepted.

## Context

Commit 0060 introduced advisory previous-collection and next-collection selection over one exact multi-sequence checkpoint-range summary, while deliberately leaving selected-collection projection outside Core. Core also lacked an explicit immutable authority proving that one projected selected collection and the summarized source range share immediate summary, checkpoint, and supersession continuity.

## Decision

Core owns an externally identified revisioned adjacent-collection projection authority. Projection accepts one exact immutable selection, validates optimistic selection revision and non-regressing external time, verifies exact source-collection summary and internal boundary authorities, materializes the selected checkpoint and supersession interval from the unchanged validated chain, and verifies exact endpoint and immediate boundary evidence.

Core also owns an externally identified revisioned multi-sequence checkpoint-range continuity-validation authority. Validation accepts one exact multi-sequence range summary and one projected adjacent collection, validates both optimistic revisions and non-regressing external time, requires unchanged source-summary identity, proves immediate summary-index and checkpoint-index adjacency, and preserves the one exact supersession connecting both authorities with its prior and successor checkpoints.

Projection and validation remain bounded, immutable, synchronous, deterministic, and side-effect free. Core does not discover adjacent summaries, reorder caller authority, load or store state, index or archive history, compact or paginate evidence, diagnose operations, schedule or supervise work, wait, restart, transport, mutate history, or execute recovered work.

## Consequences

- Hosts can materialize exact adjacent collections selected by commit 0060 without granting Core discovery or storage responsibility.
- Callers receive explicit stale-revision, regressed-time, source-summary mismatch, selected-summary mismatch, boundary mismatch, checkpoint mismatch, supersession mismatch, and non-adjacent interval outcomes.
- Successful continuity authority preserves the exact range summary, projected collection, selection, connecting supersession, boundary checkpoints, external validation tick, and revision.
- The next bounded increment can summarize continuous collection pairs and query exact cross-collection checkpoint ranges without changing Core ownership boundaries.
