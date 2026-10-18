---
name: csharp-conventions
description: Applies this repository's C# naming, formatting, organization, async, validation, and performance conventions. Use when writing, modifying, or reviewing C# code.
---

# C# Conventions

## Language and formatting

- Use the target framework, nullable context, language version, and analyzer settings from repository configuration.
- Prefer `var` for local variables.
- Use four spaces, Allman braces, and file-scoped namespaces.
- Use language keywords such as `string` rather than framework aliases such as `String`.
- Use braces for control flow.
- Prefer `nameof`, pattern matching, null patterns, and switch expressions when they improve clarity.

## Naming

- Use PascalCase for types, members, constants, and readonly fields.
- Use `_camelCase` only for mutable fields.
- Prefix interfaces with `I`.
- Use camelCase for parameters and local variables.
- Add the `Async` suffix to methods that return `Task`, `Task<T>`, `ValueTask`, or `ValueTask<T>`.

## Member organization

Order members by constants, fields, events, properties, constructors, methods, and nested types. Within a group, place more visible members first. Place static fields and properties before instance members. Place instance methods before static methods.

## Design

- Validate public arguments with the most specific standard exception or guard API.
- Use asynchronous APIs for asynchronous I/O.
- Accept `CancellationToken` on cancellable public asynchronous operations and long-running work.
- Prefer constructor injection when a type has external dependencies.
- Use standard exception types.
- Avoid `async void` except for event handlers.
- Use `ConfigureAwait(false)` only in reusable library code when the context decision is intentional.
- Measure before adding allocation-oriented complexity.

## Using directives

Place using directives outside the namespace. Order System namespaces, third-party namespaces, then project namespaces. Alphabetize within each group. Keep ordinary using directives contiguous. Put aliases and `using static` directives in a separate block.
