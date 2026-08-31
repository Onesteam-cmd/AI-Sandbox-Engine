# ADR-0053: Host Recovery Adjacent-Window Projection and Checkpoint-Range Continuity Validation Contracts

## Status

Accepted.

## Context

Commit 0052 introduced advisory previous-window and next-window selection over one exact checkpoint-range summary, while deliberately leaving selected-window projection outside Core. Core also lacked an explicit immutable authority proving that the selected projected window and the summarized source range share one exact connecting checkpoint-supersession boundary.

## Decision

Core owns an externally identified revisioned adjacent-window projection authority. Projection accepts one exact immutable selection, validates optimistic selection revision and non-regressing external time, materializes the selected checkpoint and internal supersession authorities from the unchanged validated chain, and verifies exact selected endpoints and boundary supersessions.

Core also owns an externally identified revisioned checkpoint-range continuity-validation authority. Validation accepts one exact range summary and one projected adjacent window, validates both optimistic revisions and non-regressing external time, requires the projection to originate from that exact summary, verifies immediately adjacent chain indexes, and proves that both authorities expose the same connecting supersession with exact prior and successor checkpoints.

Projection and validation do not discover, reorder, load, store, index, archive, delete, retain, compact, paginate, diagnose, restart, supervise, schedule, wait, transport, mutate history, or execute.

## Consequences

- Advisory adjacent-window selection can be materialized into exact immutable checkpoint evidence.
- Previous and next continuity become explicit authorities rather than inferred index arithmetic.
- Range and projected-window boundaries preserve the same connecting supersession object and endpoint checkpoints.
- Cross-window summary projection and bounded cross-window queries remain future contracts.
