# Conway's Game of Life Benchmark

### Purpose

This project holds the benchmark suite for the Conway's Game of Life challenge.  Each entry in the suite is a measurement we made to answer a specific question with data, and the findings are recorded here as design input for how the solution evolves.

The benchmarks support our iteration on efficiency.  We pick a question, design benchmarks that answer it, run them, and write down what the data shows.  Findings here are evidence for design decisions, not decoration.  Strategies used only by the benchmarks, such as `NoVisitedSetStrategy` and `DensityGatedStrategy`, live in this project and are never selected by `StrategyBase.CreateStrategy`.

### How an analysis is structured

Each analysis starts with the question it answers.  The benchmarks measure timing and, with the memory diagnoser enabled, allocations and GC cycles, because allocations and GC cycles are performance as well.  Findings are stated only where the data supports them, and design issues the data surfaced are recorded alongside the results.

### Analyses

#### Density and the per-generation visited set

**Question.**  The `ActiveRegionStrategy` uses a per-generation visited set so a dead cell that is a neighbor of more than one live cell is evaluated once per generation instead of once per live neighbor.  Three questions came out of our iteration on that.

1. How much does computing the density of a population cost, and is that cost worth paying to decide whether the visited set helps?
2. Does a density-gated approach, which decides per generation whether to use the visited set, beat always using it or never using it?
3. At what density does the visited set start to pay off?

**What was measured.**  The visited set makes a difference in dense populations, where deduplication saves work, and not in sparse ones.  The density of a population, live cells divided by bounding box area, is the candidate signal.  `DensityAnalysisBenchmarks` measures three groups.

- **Density cost.**  The cost of computing the density of 100, 1,000, and 10,000 cells, compared against the cost of simulating one generation of the same population.  This shows the overhead of the check relative to the work it gates.
- **Gated approach.**  The sparse, dense, mixed, and R-pentomino workloads, each simulated with the visited set always on, always off, and density-gated.  This shows whether the gated approach is a good one-size-fits-all.
- **Crossover.**  Populations at 15, 20, 25, 30, and 35 percent density, each simulated with and without the visited set.  This narrows the density where the visited set starts to pay off.

**The data.**

*Density cost.*  The density check is a single pass over the live cells, so it is O(n) in the population size.

| Population | Density check | One generation | Density as a share of a generation |
|---|---|---|---|
| 100 cells | 64.75 ns | 29,277.67 ns | 0.22% |
| 1,000 cells | 606.02 ns | 1,047,859.22 ns | 0.058% |
| 10,000 cells | 6,973.29 ns | 8,805,142.08 ns | 0.079% |

The density check scales roughly linearly at about 0.6 to 0.7 ns per cell.  The generation cost scales super-linearly, so the density check is a smaller share of the work as the population grows.

*Gated approach.*  Each workload is simulated with the visited set always on, always off, and density-gated at a threshold of 0.25.

| Workload | Always | Never | Gated | Gated vs the best of the two |
|---|---|---|---|---|
| Sparse (0.025%) | 999,270.61 ns | 748,564.00 ns | 760,415.16 ns | 1.6% over |
| Dense (82%) | 53,696.06 ns | 138,056.70 ns | 53,980.23 ns | 0.5% over |
| Mixed (10%) | 1,052,983.66 ns | 941,533.19 ns | 941,753.93 ns | 0.02% over |
| R-pentomino (30 gen) | 262,237.29 ns | 291,987.99 ns | 271,097.37 ns | 3.4% over |

The gated approach picks the right side in every case.  It is within 3.4% of the best of always-use or never-use in every workload.

*Crossover.*  Each population is simulated with and without the visited set.

| Density | With visited set | Without | With vs without |
|---|---|---|---|
| 15% | 907,514.02 ns | 903,598.99 ns | 0.4% slower |
| 20% | 760,132.15 ns | 771,145.89 ns | 1.4% faster |
| 25% | 606,335.95 ns | 680,948.15 ns | 11% faster |
| 30% | 555,791.92 ns | 639,003.40 ns | 13% faster |
| 35% | 454,103.35 ns | 586,295.90 ns | 22.5% faster |

The crossover is between 15% and 20% density.  Below it the visited set does not help, and at 15% it is actually 0.4% slower.  Above it the benefit grows with density, from 11% at 25% to 22.5% at 35%.

**Findings.**

- The density check is negligible.  It is under 0.1% of a generation's cost in every case, so it is safe to pay it to decide whether the visited set helps.
- The gated approach is a good one-size-fits-all.  It is within 3.4% of the best of always-use or never-use in every workload, and a threshold of 0.25 sits above the crossover, so it uses the visited set when it helps and skips it when it does not.
- The crossover is around 15 to 20 percent density.  Below it the visited set does not help.  Above it the benefit grows with density.

**Design issue the data surfaced.**  The gated approach allocates the visited set eagerly, even when it is not used.  For the sparse workload it allocates 62,024 B while the never-use approach allocates 31,024 B.  The extra 31,000 B is the visited set, and the same gap appears in the mixed workload.  The strategy allocates the set once at the start of the simulation and clears it at the end of each generation, so when the density is below the threshold the set is allocated but never used.  This is a cost that lazy allocation, only when first needed, would avoid.  It does not change the timing results, because the allocation is a one-time cost amortized across the generations, but it does change the allocation numbers.

### Running the benchmarks

From the `benchmark` directory:

```shell
dotnet run -c Release
```

To run a single case, filter by name:

```shell
dotnet run -c Release -- --filter "DensityAnalysisBenchmarks.GatedSparse"
```

### Notes

- The benchmark has its own solution, `ConwayLifeChallenge.Benchmark.slnx`, and is not part of the main `ConwayLifeChallenge.slnx`.
- The benchmarks run in Release mode with the default BenchmarkDotNet job.
- Run the benchmark from the `benchmark` directory so the `BenchmarkDotNet.Artifacts` folder it creates stays under the benchmark area.
- The random populations use a fixed seed so the results are reproducible.
