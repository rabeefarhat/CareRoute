<!-- docs/adr/ADR-001-clean-architecture.md -->
# ADR-001 — Clean Architecture with one project per layer

**Status:** Accepted · **Date:** __ · **Lesson:** L03

## Context
CareRoute has real business rules (referral workflow, no double booking) and
will swap infrastructure (RabbitMQ → Azure Service Bus, hosted timer → Durable Functions).

## Decision
Four projects: Domain ← Application ← Infrastructure, and Api referencing Application
and Infrastructure. Ports live in Application, adapters in Infrastructure, wiring in
Program.cs. Bounded contexts are folders inside each layer.
Tests: xUnit, FluentAssertions pinned to [7.2.2,8.0.0) (v8+ is commercial), NSubstitute
for ports only, Bogus with a fixed seed and fixed reference date.

## Consequences
+ Domain and Application are testable without infrastructure; adapters are swappable.
+ The compiler blocks outward project references.
− More projects and files; some ceremony for simple reads.
− Package references are NOT blocked by the compiler → architecture test in L22.