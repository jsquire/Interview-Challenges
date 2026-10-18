---
name: testing
description: Applies NUnit and NSubstitute test standards for behavior, organization, assertions, and test doubles. Use when writing, modifying, debugging, or reviewing tests.
---

# Testing

- Test observable behavior and public contracts rather than implementation details.
- Name tests for the behavior and condition they verify.
- Keep each test independent and understandable without shared mutable fixture state.
- Use NUnit constraint assertions such as `Assert.That`.
- Add assertion messages when they provide diagnostic context that the assertion does not.
- Use `Assert.ThatAsync` for asynchronous exception assertions.
- Avoid tests whose only meaningful assertion is that ordinary valid code does not throw.
- Do not add ceremonial Arrange, Act, and Assert comments.
- Keep one fixture per production type when that mapping is natural.
- Use NSubstitute for interaction-based test doubles.
- Prefer a simple fake when it communicates behavior more clearly than a mock.
- Verify user-observable interactions rather than incidental internal calls.
- Add categories only when the repository has a concrete filtering need.
- Run the narrowest relevant test selection, then run the project or solution tests before completion.
