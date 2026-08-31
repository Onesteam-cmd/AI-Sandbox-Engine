# ADR-0091: Social Turn Foundation Probe

- Status: Accepted
- Date: 2026-08-07

## Context

After commit 0090, the Perception downstream-consumer value gate found no
executable mutation candidate. A fresh practical-value ranking selected Social
and `SocialTurnCoordinationProcessor`.

The first Social mutation-shape review incorrectly treated existing verifier
checks for the Social Core API as FoundationProbe coverage. The corrected review
distinguished API verification from executable probe coverage.

`ConversationProbe` is already a large Conversation/Dialogue probe and cannot
absorb Social turn coordination without semantic mixing. No generic
low-coupling probe exists.

## Decision

Allow one reviewed exception to the normal prohibition on automatic probe-file
growth and add exactly one `SocialProbe.cs`.

The probe must:

1. create one immutable authoritative World State snapshot;
2. create one immutable ConversationState and preserve its revision;
3. create one exact social turn coordination request with matching world,
   version, simulation tick, conversation, revision, audience, and speaker;
4. include two ordered eligible proposals;
5. invoke one exact coordinator through `SocialTurnCoordinationProcessor`;
6. require coordinator invocation exactly once;
7. require one stable Granted decision selecting an existing proposal;
8. require the returned selected proposal to preserve exact identity;
9. prove ConversationState revision remains unchanged;
10. prove authoritative World State reference, state, version, and simulation
    tick remain unchanged.

The probe must not create a speaking queue, scheduler, retry policy, timer,
background worker, automatic dialogue execution, command, runtime dispatch,
speech prompting, or persistence.

## Consequences

FoundationProbe now covers a practical Social coordination boundary without
changing Core contracts or recovery semantics.

The next increment must value-gate distinct production consumers of Social.
Further mechanical Social contract nesting or automatic probe-file growth is
not permitted.
