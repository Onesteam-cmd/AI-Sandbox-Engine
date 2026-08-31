# ADR-0083: Host Runtime Normal Lifecycle Foundation Probe

- Status: Accepted
- Date: 2026-08-07

## Context

The Host recovery contract hierarchy had reached repeated mechanical
Sequence nesting without a production consumer for the deepest authorities.
Continuing that pattern would increase public API and validation surface
without a distinct Host integration scenario.

The existing FoundationProbe validates deterministic World execution,
persistence continuation, and event dispatch, but it does not execute the
normal Host Runtime lifecycle represented by the contracts from commits
0031 through 0038.

## Decision

Add one self-contained FoundationProbe scenario that composes only existing
Host Runtime authorities:

1. create a pending request;
2. admit it into a bounded queue;
3. acquire an externally identified worker lease;
4. select and create one dispatch;
5. acknowledge one in-flight attempt;
6. create one externally reported completion;
7. settle the attempt, finalize the request, and release the lease.

Add repository validation that requires this scenario and rejects a planned
next foundation increment that mechanically extends recovery Sequence
nesting.

No new Core contract is introduced.

## Consequences

The foundation gains executable cross-contract evidence for the normal Host
Runtime path. Recovery hierarchy growth is closed unless a future increment
demonstrates a distinct consumer and invariant that cannot be composed from
existing authorities.

Core continues to own no queue storage, transport, worker execution,
scheduling, waiting, persistence, process supervision, or wall-clock access.
