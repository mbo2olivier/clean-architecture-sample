---
name: cnss-ddd-modeling
description: Generate or incrementally update CNSS DDD and Clean Architecture models from procedure manuals, legal or regulatory texts, glossary material, architecture decisions, and the current validated module model. Use when Codex must model a CNSS business module, derive bounded contexts and use cases from procedural sources, compare a proposal against an existing model, or produce structured modeling outputs with explicit assumptions, open questions, conflicts, Mermaid diagrams, and Clean Architecture structure.
---

# CNSS DDD Clean Modeler

Model a CNSS business module from documentary evidence first, not from architectural preference. Preserve continuity with the validated model, observed repo conventions, and explicit foundation decisions.

## Start Here

Read these sources before proposing a model:
- [`references/required-resources.md`](./references/required-resources.md) for the mandatory reading order
- [`references/skill-prompt.md`](./references/skill-prompt.md) for the expert operating posture and non-negotiable rules

Treat repo paths as relative to the active workspace root. If the expected CNSS repo structure is missing, say which critical resources are unavailable and lower certainty instead of filling gaps silently.

## Inputs

Expect these inputs when available:
- target CNSS module
- user intent: initial creation or incremental update
- validated current model for the module
- new or changed procedure manual excerpts
- applicable laws, regulations, circulars, or service notes
- glossary, architecture decisions, and cross-module dependencies

If a critical input is missing, continue only with explicit uncertainty markers.

## Required Workflow

1. Read conventions, templates, and agent protocols from the repo in the order defined in [`references/required-resources.md`](./references/required-resources.md).
2. Read the latest validated version of the target module before proposing any change.
3. Read the new business sources and extract actors, tasks, business rules, documents, states, and external dependencies.
4. Map procedural tasks into use cases that fit the observed `Request/Response/Handler/Validator` style.
5. Derive entities, value objects, aggregates, domain services, repositories, and domain events from the documented business reality.
6. Apply the foundation decisions already fixed for the CNSS memory baseline:
   - keep `Aggregats` as the folder naming convention
   - model value objects with the official base `Cnss.Shared.Domain.Abstractions.ValueObject`
   - generate business identifiers through a domain service
   - keep repository commit explicit in the target business project convention
   - use aggregate factories only when the assembly complexity justifies them
7. Build a suggested Clean Architecture structure aligned with the style already observed in the repo.
8. Compare the proposal against the current validated model and make the delta explicit.
9. Produce both human-readable documentation and a JSON payload conforming to `docs/agent/modeling-output-schema.json`.

## Modeling Rules

Always do these things:
- ground every business rule in a source or mark it as an inference
- separate `Conventions observées`, `Hypothèses`, and `Questions ouvertes`
- document coexisting variants instead of silently deleting one
- preserve vocabulary and functional continuity unless a documented conflict forces change
- state conflicts with the current model explicitly before suggesting replacement

Never do these things:
- invent a business rule silently
- treat a draft document as official truth
- refactor the model around a theoretical ideal disconnected from the repo style
- remove an aggregate, use case, or rule without calling out the break
- claim legal certainty when the source is ambiguous

## Initial Creation vs Incremental Update

For initial creation:
- build a first structured model from the available sources
- expose weak zones as assumptions and open questions

For incremental update:
- start from the last validated module version
- compute an explicit delta
- list conflicts with the existing model
- preserve naming and behavioral continuity when possible

## Output Contract

Start the response with a short synthesis of the module and its purpose. Then provide the modeled elements and end with clearly separated sections for:
- `Conventions observées`
- `Hypothèses`
- `Questions ouvertes`
- `Conflits avec le modèle existant`

Include these artifacts when the request is substantive:
- JSON output matching `docs/agent/modeling-output-schema.json`
- Mermaid diagrams when they clarify flows, aggregates, or boundaries
- suggested Clean Architecture structure

If the task is an update, include an explicit delta versus the current validated version.

## Resource Use

Use [`references/required-resources.md`](./references/required-resources.md) to decide what to load and in which order. Use [`references/skill-prompt.md`](./references/skill-prompt.md) when you need the full expert prompt, stricter guardrails, or a reset toward the intended CNSS modeling posture.
