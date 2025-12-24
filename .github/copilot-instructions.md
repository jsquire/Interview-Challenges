# Interview Challenges Repository - Copilot Instructions

This document provides universal guidelines for GitHub Copilot when working on projects in this repository. These instructions apply across all programming languages and challenge implementations.

## About This Repository

This repository contains solutions to various interview challenges, with each challenge potentially implemented in multiple programming languages. The structure is:

```
challenges/
  {challenge-name}/
    README.md           # Challenge description
    Solution.md         # Solution discussion
    src/
      {language}/       # Language-specific implementation (e.g., csharp/, python/, java/)
        src/            # Source code
        tests/          # Test code
```

**Language-Specific Instructions**: When working with language-specific files, Copilot automatically loads additional instructions from `.github/instructions/{language}.instructions.md`. For example, C# files automatically include guidelines from `.github/instructions/csharp.instructions.md`.

## General Instructions for Copilot Responses

- Provide concise, focused explanations without unnecessary elaboration.
- When analyzing images, first describe exactly what is visually present before conducting any web-based research.
- Cross-check all details with both the image and external sources to ensure accuracy.
- Clearly explain your reasoning by verifying claims step by step.
- Avoid hallucinations at all costs—only respond with grounded, verifiable information.

## Data Validation and Anti-Hallucination Framework

### CRITICAL: Never Make Assumptions - Always Validate

**Fundamental Principle**: Every technical decision must be based on verified, authoritative data. Assumptions and "best guesses" during implementation are strictly forbidden.

### Pre-Implementation Validation Protocol

**BEFORE making any code changes:**

1. **Verify Current State**
   - Read existing files to understand current implementation
   - Use `grep_search` or `semantic_search` to understand usage patterns
   - Check test files to understand expected behavior
   - Validate assumptions about existing APIs and interfaces

2. **Research Authoritative Sources**
   - For .NET APIs: Use official Microsoft documentation
   - For third-party libraries: Check official documentation and GitHub repositories  
   - For VS Code APIs: Verify against official VS Code API documentation
   - For project patterns: Analyze existing codebase patterns, not external assumptions

3. **Surface Uncertainty**
   - When data is insufficient: State "I don't have enough information to make this decision"
   - When multiple approaches exist: Present options with trade-offs for human decision
   - When authoritative sources are unavailable: Request guidance rather than guessing

### Mandatory Analysis Steps

**For every implementation task:**

1. **Current State Analysis**
   ```
   - What exists now? (verified by reading actual files)
   - What are the current patterns? (verified by code analysis)
   - What are the constraints? (verified by project structure/dependencies)
   ```

2. **Requirements Validation**
   ```
   - What exactly needs to be implemented? (clarify ambiguous requirements)
   - What are the acceptance criteria? (verify against existing test patterns)
   - What are the dependencies? (verify by examining current codebase)
   ```

3. **Approach Planning**
   ```
   - What approach aligns with existing patterns? (verified by codebase analysis)
   - What are the risks and unknowns? (identify areas needing validation)
   - What assumptions need verification? (list all assumptions explicitly)
   ```

### Forbidden Practices

**❌ NEVER:**
- Make API calls or use library features without first verifying they exist
- Assume file structures or patterns without reading actual files
- Implement based on "standard practices" without verifying project-specific patterns
- Guess at parameter types, method signatures, or interface contracts
- Use placeholder implementations with TODO comments
- Make breaking changes without analyzing impact on existing code

**✅ ALWAYS:**
- Present plans to chat for review before implementation
- Verify all APIs and patterns against actual codebase
- Read existing files to understand current state
- Ask for clarification when requirements are ambiguous
- Provide specific file/line references when discussing existing code

## Research and Verification Workflow

### Step 1: Gather Authoritative Data

**Use tools systematically to collect facts:**

1. **For existing code understanding:**
   - `read_file`: Get current implementation details
   - `semantic_search`: Understand usage patterns across codebase
   - `grep_search`: Find specific patterns or API usage
   - `list_code_usages`: Understand how interfaces/classes are implemented

2. **For external API verification:**
   - `fetch_webpage`: Get official documentation
   - `get_vscode_api`: For VS Code extension development
   - `github_repo`: For understanding third-party library patterns (when specified)

3. **For project structure validation:**
   - `list_dir`: Understand actual directory structure
   - `file_search`: Find configuration files and dependencies
   - `get_errors`: Verify current compilation state

### Step 2: Validate All Assumptions

**Before proposing any solution:**

1. **List all assumptions explicitly:**
   ```
   "I am assuming that..."
   "Based on the existing pattern in [specific file], I believe..."
   "The requirements suggest X, but I need to verify..."
   ```

2. **Provide evidence for each assumption:**
   ```
   "Verified by reading [file] lines [X-Y]"
   "Confirmed by documentation at [URL]"
   "Pattern established in [specific examples with file references]"
   ```

3. **Identify knowledge gaps:**
   ```
   "I don't have enough information about..."
   "This assumption needs verification because..."
   "I would need to research [specific topic] to be certain..."
   ```

### Step 3: Present Plans for Review

**Always surface implementation plans to chat BEFORE coding:**

1. **Current State Summary:**
   - What exists now (with specific file references)
   - What patterns are established (with examples)
   - What constraints exist (with evidence)

2. **Proposed Approach:**
   - Specific changes planned (with rationale)
   - Files to be modified/created (with justification)
   - Dependencies and integration points (verified)

3. **Risks and Unknowns:**
   - What could go wrong?
   - What needs further research?
   - What assumptions still need validation?

4. **Request for Approval:**
   - "Does this approach align with project goals?"
   - "Are there constraints I haven't considered?"
   - "Should I proceed with this implementation?"

## Quality Assurance Checkpoints

### Before Every Code Change

**Mandatory verification checklist:**

- [ ] Have I read all relevant existing files?
- [ ] Have I verified all API references against authoritative sources?
- [ ] Have I identified and validated all assumptions?
- [ ] Have I presented the plan for human review?
- [ ] Do I have specific evidence for every technical decision?
- [ ] Have I checked for integration impacts on existing code?

### During Implementation

**Continuous validation:**

- Verify each API call/method signature before use
- Check existing patterns before introducing new approaches
- Test assumptions against actual codebase behavior
- Stop and ask for guidance when encountering unknowns

### After Implementation

**Validation requirements:**

- Run tests to verify functionality
- Check for compilation errors
- Validate against original requirements
- Confirm integration with existing patterns

## Communication Standards for Technical Decisions

### When You Don't Know

**Required phrasing patterns:**

- "I don't have sufficient information to determine..."
- "This would require research into..."
- "I can see X in the codebase, but I'm uncertain about Y..."
- "The official documentation would need to be consulted for..."
- "I should verify this assumption before proceeding..."

### When Presenting Analysis

**Required structure:**

1. **Facts** (with sources): "Based on reading [file], I can see..."
2. **Analysis** (with reasoning): "This suggests that..."
3. **Uncertainties** (explicitly stated): "However, I'm unclear about..."
4. **Recommendations** (with rationale): "I recommend... because..."
5. **Next Steps** (specific actions): "To validate this, I should..."

### When Requesting Approval

**Always include:**

- Current state summary (verified)
- Proposed changes (specific)
- Rationale (evidence-based)
- Risks/unknowns (honest assessment)
- Request for guidance (explicit)

## Universal Development Principles

These principles apply across all programming languages and challenge implementations in this repository.

### Code Quality Standards

- Write clean, readable, and maintainable code
- Follow established patterns within each language ecosystem
- Prefer explicit over implicit when clarity matters
- Balance performance with maintainability appropriately

### Testing Philosophy

- Design tests around behavior, not implementation details
- Cover edge cases and boundary conditions
- Test external behavior observable by users of the API
- Never bypass normal instantiation or access internal state for testing convenience
- If testing reveals API design problems, discuss the design rather than working around it

### Documentation

- Document the "why" behind non-obvious decisions
- Keep documentation close to the code it describes
- Update documentation when changing behavior
- Write self-documenting code when possible

### Error Handling

- Validate inputs at public API boundaries
- Use appropriate exception types for the language/framework
- Provide meaningful error messages with context
- Handle resource cleanup properly (disposables, closures, etc.)

### Git Workflow

- Make atomic commits focused on single changes
- Write clear commit messages explaining what and why
- Include test updates with implementation changes
- Ensure all tests pass before committing

## Contributing to This Repository

When adding a new language implementation:

1. Create language-specific instructions in `.github/instructions/{language}.instructions.md`
2. Use frontmatter with glob patterns to automatically apply the instructions
3. Follow the directory structure: `challenges/{name}/src/{language}/`
4. Include comprehensive tests appropriate for the language ecosystem
5. Document build, test, and run commands in the implementation directory

**Example frontmatter for path-specific instructions:**

```yaml
---
applyTo: "**/python/**/*.py"
---
```

This ensures Copilot automatically loads the correct language-specific guidance when working with files matching the pattern.
