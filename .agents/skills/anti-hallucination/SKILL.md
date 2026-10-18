---
name: anti-hallucination
description: Verifies technical claims and implementation decisions against local evidence and authoritative sources. Use for every implementation, investigation, debugging, planning, or review task.
---

# Evidence and Verification

1. Inspect the relevant repository files before reasoning about current behavior.
2. Identify which statements are facts, inferences, unknowns, and recommendations.
3. Verify externally checkable claims against current primary documentation or source repositories.
4. Treat search summaries, generated summaries, issue comments, and third-party articles as discovery aids unless independently verified.
5. State what evidence is missing. Say that you do not know when the available data cannot support a conclusion.
6. Do not invent APIs, versions, configuration keys, file contents, test results, or compatibility claims.
7. Prefer repository tools, compilation, tests, analyzers, and executable checks over visual inspection alone.
8. Stop for human input when a consequential choice cannot be resolved from evidence.
9. Report failed or skipped validation plainly.

When communicating a decision, provide the evidence first, then the conclusion. Include file references for repository facts and direct links for external claims when the host supports them.
