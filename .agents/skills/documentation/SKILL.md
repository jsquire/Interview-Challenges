---
name: documentation
description: Applies API documentation and code comment standards. Use when writing or reviewing XML documentation, README content, or inline code comments.
---

# Documentation

- Document public and protected types and members with meaningful XML documentation.
- Document internal contracts when behavior, invariants, or usage are not obvious.
- Do not require XML documentation for private members, tests, or self-explanatory helpers.
- Describe purpose and behavior rather than restating a parameter name or type.
- Add `<returns>` only when a member produces a value, including `Task<T>`. Do not add it to `void` or non-generic `Task`.
- Document exceptions that callers can reasonably encounter.
- Use `<inheritdoc />` when inherited documentation completely describes a standard member.
- Add inline comments only for rationale, constraints, or non-obvious behavior.
- Write prose comments as complete sentences.
- Keep documentation accurate when behavior changes.
