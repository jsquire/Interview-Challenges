# The Longest Palindromic Substring Challenge

### Solutions overview

The first strategy discussed here was a natural instinct when approaching the problem as if it were an interview.  It is based on candidate pairs, the observation that a palindrome must start and end with the same character.  The second is a refinement of that approach, introducing pruning, priority ordering, and inline run detection to improve average-case performance.  Both work well in practice but carry worst-case costs that scale poorly with the number of candidates.

The third strategy, expand around center, is a well-known algorithm explored after the initial implementations.  It takes a fundamentally different angle by building palindromes outward from each possible center point rather than filtering candidates.  This trades the overhead of candidate management for a simpler, space-efficient approach with the same quadratic time complexity.

Other well-known approaches, including dynamic programming and Manacher's algorithm, are discussed briefly but were not implemented.

Each strategy is discussed in more detail below.

### The candidate pairs approach

**_Concept_**

This approach is based on the observation that any palindrome must begin and end with the same character.  By scanning the string and tracking where each character appears, we can identify all candidate substrings that could potentially be palindromes.  These candidates are then sorted by length and evaluated in order, with the first one confirmed as a palindrome being the answer.

**_Steps_**

1. Scan the string from left to right, tracking the positions where each character appears.

2. When a character is encountered that has been seen before, create candidate pairs by combining each previous position of that character with the current position.  Each pair represents a potential palindrome substring.

3. Once all candidates are collected, sort them by length in descending order.  For candidates of equal length, prefer the one that appears earlier in the string.

4. Evaluate each candidate in sorted order by checking if the substring is actually a palindrome.  The first confirmed palindrome is the answer.

**_Example_**

Consider the string `"babad"`:

1. Scan and track character positions:
   - `b` at position 0
   - `a` at position 1
   - `b` at position 2 (previously seen at 0, creates candidate pair [0, 2])
   - `a` at position 3 (previously seen at 1, creates candidate pair [1, 3])
   - `d` at position 4

2. Candidate pairs sorted by length: `[0, 2]` and `[1, 3]` both have length 3.

3. Evaluate `[0, 2]`: substring is `"bab"`, which is a palindrome.

4. Return `"bab"` as the result.

**_Complexity_**

Worst Case Time Complexity: `O(n^3)`, broken down by phase:
- Candidate generation: `O(n^2)` when most characters repeat
- Sorting candidates: `O(n^2 log n)`
- Palindrome verification: `O(n^3)` when `O(n^2)` candidates each require `O(n)` to check

Average Case Time Complexity: `O(n^2)`, dominated by candidate generation.  For a fixed alphabet, the number of pairs remains `O(n^2)` even for diverse inputs.  The longest-first sort order allows early termination during verification, reducing the constant factor.

Space Complexity: `O(n^2)` for storing candidate pairs in the worst case.

**_Practical Performance_**

The `O(n^3)` worst case requires a pathological input where nearly all characters are the same, producing a quadratic number of candidates that each need linear verification.  For typical inputs with a diverse character mix, the longest candidate is usually found early in the sorted order, and most verification checks are short.  The quadratic space cost for storing all candidate pairs is the more notable practical concern.

### The optimized candidate pairs approach

**_Concept_**

This approach builds on the candidate pairs strategy but introduces several optimizations to improve average-case performance.  The key insight is that runs of repeating characters are guaranteed palindromes and can be tracked inline without generating individual pairs.  Additionally, candidates are stored in a priority queue rather than sorted afterward, allowing early termination once a long enough palindrome is found.

**_Optimizations_**

- **Inline run detection**: When consecutive identical characters are encountered, they form a guaranteed palindrome.  Rather than expanding these into individual candidate pairs immediately, they are tracked as a single unit.

- **Priority queue ordering**: Candidates are stored in a max-heap by length, allowing the longest candidates to be evaluated first.  This enables early termination when a palindrome is found that is longer than any remaining candidate.

- **Pruning at enqueue time**: Candidates shorter than the longest known palindrome are not added to the queue, reducing unnecessary work.

- **Early exit for full-string palindromes**: If the entire string is itself a palindrome, the answer is returned immediately without further processing.

**_Complexity_**

Worst Case Time Complexity: `O(n^3)`, broken down by phase:
- Candidate generation: `O(n^2)` when most characters repeat
- Priority queue operations: `O(n^2 log n)` for insertion and extraction
- Palindrome verification: `O(n^3)` when `O(n^2)` candidates each require `O(n)` to check

Average Case Time Complexity: `O(n^2 log n)`, broken down by phase:
- Candidate generation: `O(n^2)` for a fixed alphabet, same as the unoptimized approach
- Priority queue operations: `O(n^2 log n)` for insertion and extraction
- Palindrome verification: significantly reduced by pruning and early termination, as candidates shorter than the current best are never enqueued

Space Complexity: `O(n^2)` for the candidate queue and position tracking in the worst case.

**_Practical Performance_**

The optimizations meaningfully reduce the work done on typical inputs.  Run detection collapses repeating sequences that would otherwise generate many individual pairs, and pruning avoids enqueueing candidates that cannot beat the current best.  The priority queue ordering ensures the most promising candidates are evaluated first, allowing early termination before most of the queue is touched.  The worst case still requires an adversarial input that defeats all of these heuristics simultaneously.

### The expand around center approach

**_Concept_**

This approach leverages the fundamental property that every palindrome is symmetric around its center.  Rather than generating and testing candidate substrings, we iterate through all possible center points and expand outward while characters match.  This directly constructs palindromes rather than filtering candidates.

**_Key Insight_**

A palindrome can have either:
- An **odd length**, with a single character at its center (e.g., `"aba"` centered on `b`)
- An **even length**, with the center being the gap between two characters (e.g., `"abba"` centered between the two `b` characters)

For a string of length `n`, there are `n` possible centers for odd-length palindromes and `n - 1` possible centers for even-length palindromes, giving `2n - 1` centers total.

**_Steps_**

1. If the string has a single character or is itself a palindrome, return it immediately.

2. For each position in the string:
   - Expand outward from that position as the center of an odd-length palindrome.
   - Expand outward from the gap between that position and the next as the center of an even-length palindrome.

3. For each expansion, continue while the left and right characters match and remain within bounds.

4. Track the longest palindrome found across all expansions.

**_Example_**

Consider the string `"babad"`:

1. Center at position 0 (`b`):
   - Odd expansion: `"b"` (length 1)
   - Even expansion between positions 0 and 1: `b` ≠ `a`, no palindrome

2. Center at position 1 (`a`):
   - Odd expansion: `a` matches itself, expand to positions 0 and 2: `b` = `b`, palindrome `"bab"` (length 3)
   - Even expansion between positions 1 and 2: `a` ≠ `b`, no palindrome

3. Center at position 2 (`b`):
   - Odd expansion: `b` matches itself, expand to positions 1 and 3: `a` = `a`, palindrome `"aba"` (length 3)
   - Even expansion between positions 2 and 3: `b` ≠ `a`, no palindrome

4. Centers at positions 3 and 4 yield shorter palindromes.

5. Return `"bab"` (found first among the longest).

**_Complexity_**

Worst Case Time Complexity: `O(n^2)`, broken down by phase:
- Center iteration: `O(n)` for `2n - 1` centers
- Expansion per center: up to `O(n)` character comparisons

Average Case Time Complexity: closer to `O(n)` for diverse inputs.  Most expansions terminate after a small number of comparisons, as the probability of continued matching drops exponentially with alphabet size.  The early exit check for full-string palindromes handles the pathological all-same-character case in `O(n)`.

Space Complexity: `O(1)` as no additional data structures are needed beyond a few tracking variables.  This is a significant advantage over the candidate pairs approaches.

**_Practical Performance_**

The early exit check for full-string palindromes handles the pathological all-same-character case in `O(n)`, so the quadratic worst case requires a more nuanced adversarial input.  Combined with constant space overhead, this makes expand around center the most practical choice among the implemented strategies.

### Other approaches

**_Dynamic Programming_**

A dynamic programming approach builds a table where `dp[i][j]` indicates whether the substring from index `i` to `j` is a palindrome.  The recurrence relation is:

- Base case: All single characters are palindromes (`dp[i][i] = true`)
- Base case: Two adjacent identical characters form a palindrome (`dp[i][i+1] = true` if `s[i] == s[i+1]`)
- Recurrence: `dp[i][j] = (s[i] == s[j]) && dp[i+1][j-1]`

This approach has `O(n^2)` time complexity and `O(n^2)` space complexity.  While the time complexity matches expand around center, the quadratic space requirement offers no benefit for this problem.  The expand around center approach achieves the same time complexity with constant space, making dynamic programming a less attractive choice for this particular challenge.

**_Manacher's Algorithm_**

Manacher's algorithm solves this problem in `O(n)` time and `O(n)` space by exploiting the symmetry of palindromes to avoid redundant comparisons.  When a palindrome is found within a larger palindrome, information about its radius can often be inferred from the mirror position, avoiding the need to re-expand.

While theoretically optimal, Manacher's algorithm has significant implementation complexity.  It requires transforming the input string to handle both odd and even length palindromes uniformly, and the core logic involves tracking multiple state variables and handling several special cases.  For interview scenarios where the input constraint is typically small (2,000 characters or less), the practical performance difference between `O(n)` and `O(n^2)` is negligible, while the implementation complexity increase is substantial.  Unless specifically asked for a linear-time solution, the expand around center approach is generally preferred.

### Additional resources

- [Longest palindromic substring (Wikipedia)](https://en.wikipedia.org/wiki/Longest_palindromic_substring)
- [Manacher's Algorithm (Wikipedia)](https://en.wikipedia.org/wiki/Longest_palindromic_substring#Manacher's_algorithm)
- [Longest Palindromic Substring (LeetCode)](https://leetcode.com/problems/longest-palindromic-substring/)
- [Dynamic Programming (Wikipedia)](https://en.wikipedia.org/wiki/Dynamic_programming)
