# Conway's Game of Life Challenge

### Solutions overview

The full signed 64-bit coordinate space is too large to store or process as a grid.  Even a rectangle around a small starting population can contain an enormous amount of empty space.  These constraints apply regardless of the approach used to simulate the population.

Any solution must identify the cells that need consideration while keeping each generation based on the completed state of the previous one.  It must also handle coordinates at the signed 64-bit limits without allowing arithmetic to wrap across the board.  How the population is represented and how repeated work is avoided determine much of the cost, with trade-offs between execution time, memory use, and implementation complexity.

### The initial approach _(Active Region strategy)_

In the spirit of the exercise, this approach was conceptualized by exploring the problem space and approaching it intuitively as a candidate would do in a whiteboard setting.  No research into prior art, common algorithms, nor the work of others was done.

The approach used was to implement the intuitive solution in modern C# with attention to performance considerations, specifically focusing on avoiding allocations and efficient reuse of sets.  A deliberate effort was made to optimize for readability over raw performance where techniques such as bit-shifting would have made the code less clear without a demonstrated performance benefit.

The simulation strategy is hand-written with no AI coding assistance.  AI was used in the exploration of the problem space, analysis, synthesizing a comprehensive test matrix, implementing the test suite, and boilerplate infrastructure code.  All AI code was reviewed for safety and correctness and tweaked by hand to ensure consistency with the surrounding codebase.

**_Concept_**

A surviving cell must already be alive.  A birth must be next to live cells.  Together, these observations limit each generation's decisions to the starting live cells and their dead neighbors.

A `HashSet<Coordinate>` stores the population for live-neighbor lookups.  A separate set collects the next population so that births and deaths do not affect the current generation's decisions, as the rules require that the generation is processed as a pure function.  Dead candidates with no state change are tracked separately to avoid reevaluating them during the generation.

Coordinates remain pairs of signed 64-bit integers.  Neighbor offsets are checked before addition, excluding positions outside the coordinate range rather than allowing them to wrap.  Unlike the ideal infinite board, this board has edges and corners.

**_Steps_**

1. If no generations are requested, the state does not change.  The result is a copy of the starting population.

2. For each generation, keep the starting population unchanged while collecting the next population and tracking evaluated dead candidates.

3. Visit each live cell and its initially dead neighbors.  Count each cell's live neighbors using only the starting population.

4. Add surviving cells and new births to the next population.  Record initially dead candidates that remain dead in the visited set.  A candidate already present in either set does not need to be evaluated again during the generation.

5. Return the result when the requested generations are complete.  Otherwise, exchange the population sets and clear the working sets for reuse.  Stop early if the population becomes empty.

**_Example_**

Consider three live cells in a horizontal line, commonly called a blinker.

| Generation | Live coordinates |
| --- | --- |
| Starting population | `(0, 1), (1, 1), (2, 1)` |
| Next population | `(1, 0), (1, 1), (1, 2)` |

1. The two end cells each have one live neighbor and die.

2. The middle cell has two live neighbors and survives.

3. The dead cells above and below the middle each have three live neighbors and become alive.

Both birth locations are reached from multiple starting cells, but their outcomes are calculated only once.  The births are collected separately and do not contribute to neighbor counts until the next generation.

**_Complexity_**

Let `n` be the largest live population reached during the simulation, including the starting population, and `g` the number of generations actually executed.

Expected time complexity is `O(g * n)` for a nonempty simulation, assuming expected constant-time hash lookups and amortized constant-time insertion.  Each starting live cell has at most eight neighbors, and each distinct candidate examines at most eight neighbors of its own.  The nested loops and bounded recursion therefore do not add another population-sized factor.

Using the largest population also accounts for clearing reused sets, whose retained capacity can exceed the current population.  Heavy hash collisions can make lookups linear, giving a conservative worst-case bound of `O(g * n^2)`.

Space complexity is `O(n)` for the population sets and visited candidates.  Recursion depth is bounded independently of population size.  With zero generations, copying the input takes `O(n)` time and space.

**_Practical Performance_**

Coordinates are value types, and neighbor enumeration does not allocate temporary collections.  The population buffers are reused rather than recreated for each generation.  A single-generation run reads the caller's input directly.  Longer runs copy it once to keep buffer reuse from modifying that input.

Population growth increases both the work and storage required.  The working sets retain their capacity when cleared, so a small final population does not imply a small memory footprint throughout the run.

The processing rate and memory allowance in the [problem-space discussion](./README.md#problem-space) are planning estimates.  They are not measured throughput or peak-memory results for this implementation.

### The optimized active region approach

**_Concept_**

This approach builds on the active region strategy.  The benchmarking analysis of the initial approach found that the value of the per-generation visited set depends on the density of the population.  Below a crossover between 15% and 20% density the visited set does not help, and at 15% density it is slightly slower than not using it.  Above the crossover the benefit grows with density, measuring 11% faster at 25% and 22.5% faster at 35%.

The optimized strategy therefore computes the density of the population at the start of each generation and uses the visited set only when the density is at or above a threshold of 25%.  The density is the population size divided by the area of its bounding box, a single pass over the live cells.  That check measured under 0.1% of a generation's cost in every case, so it is cheap enough to pay for on every generation.  The full analysis, including the per-density and per-workload data, is in the [benchmark area](./src/csharp/benchmark/README.md#density-and-the-per-generation-visited-set).

**_Optimizations_**

- **Density-gated visited set**: Each generation picks its own path.  Populations at or above the 25% threshold use the visited set, and those below it skip it.  The gated approach measured within 3.4% of the best of always-use or never-use in every workload, and it is on the winning side in every workload.

- **Lazy allocated visited set**: The visited set is allocated on the first generation that uses it, not up front.  Benchmarking surfaced that eager allocation paid about 31,000 bytes even in workloads that never used the set, so the allocation only happens when the set earns its cost.

- **Split generation paths**: The generation methods keep a dedicated path for the visited set and one for not, instead of a single path with a nullable visited set.  That duplication is deliberate.  Benchmarking the three forms found the single-pass form 0.9% to 4.6% slower with identical allocations, and aggressive inlining of the single-pass methods did not recover the difference.  The full analysis, including the inlining probe, is in the [benchmark area](./src/csharp/benchmark/README.md#split-paths-or-a-single-nullable-path).

**_Complexity_**

Expected time complexity is `O(g * n)` per generation for a nonempty simulation, the same as the initial approach, with an additional `O(n)` density pass per generation.  Space complexity is `O(n)` for the population sets and visited candidates.  With zero generations, copying the input takes `O(n)` time and space.

**_Practical Performance_**

The strategy keeps the active region strengths while dropping the work that does not pay off.  For the 82% density workload it matches the always-use behavior, which measured 53,696.06 ns against 138,056.70 ns for the never-use behavior.  For the sparse workload it matches the never-use behavior, which measured 748,564.00 ns against 999,270.61 ns for the always-use behavior, and it allocates 31,024 B where the always-use behavior allocates 546,533 B.  The cost of that choice is the per-generation density pass and, at most, one generation on the other path when a population crosses the threshold mid-simulation.

### Other approaches

Hashlife is a more advanced algorithm used by modern programs such as Golly.  It represents patterns as a tree structure, which allows it to simulate very large patterns efficiently.  It is a candidate for the efficiency iteration.

### Additional resources

- [Conway's Game of Life (Wikipedia)](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life)
- [Life 1.06 format (LifeWiki)](https://conwaylife.com/wiki/Life_1.06)
- [.NET HashSet implementation](https://github.com/dotnet/runtime/blob/v10.0.0/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/HashSet.cs)
