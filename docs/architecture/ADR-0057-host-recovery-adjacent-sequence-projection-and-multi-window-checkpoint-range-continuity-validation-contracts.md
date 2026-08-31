# ADR-0057: Host Recovery Adjacent-Sequence Projection and Multi-Window Checkpoint-Range Continuity Validation Contracts

## Status

Accepted.

## Context

Commit 0056 introduced advisory previous-sequence and next-sequence selection over one exact multi-window checkpoint-range summary, while deliberately leaving selected-sequence projection outside Core. Core also lacked an explicit immutable authority proving that one projected selected sequence and the summarized source range share immediate pair, checkpoint, and supersession continuity.

## Decision

Core owns an externally identified revisioned adjacent-sequence projection authority. Projection accepts one exact immutable selection, validates optimistic selection revision and non-regressing external time, verifies exact source-sequence pair and internal boundary authorities, materializes the selected checkpoint and supersession interval from the unchanged validated chain, and verifies exact endpoint and immediate boundary evidence.

Core also owns an externally identified revisioned multi-window checkpoint-range continuity-validation authority. Validation accepts one exact multi-window range summary and one projected adjacent sequence, validates both optimistic revisions and non-regressing external time, requires the projection to originate from that exact summary, verifies immediately adjacent pair and checkpoint indexes, and proves that both authorities expose the same connecting supersession with exact prior and successor checkpoints.

Projection and validation do not discover, reorder, load, store, index, archive, delete, retain, compact, paginate, diagnose, restart, supervise, schedule, wait, transport, mutate history, or execute.

## Consequences

- Advisory adjacent-sequence selection can be materialized into exact immutable checkpoint and supersession evidence.
- Previous- and next-sequence continuity become explicit authorities rather than inferred pair arithmetic.
- Selected internal boundaries and the one range-connecting supersession remain exact source-chain authorities.
- Continuous multi-sequence summary and bounded cross-sequence queries remain future contracts.
