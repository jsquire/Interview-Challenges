using Squire.ConwayLifeChallenge.Infrastructure;

namespace Squire.ConwayLifeChallenge.Strategies;

/// <summary>
///   An optimized approach for simulating Conway's Game of Life.
/// </summary>
///
/// <remarks>
///   This is a version of the active region strategy, optimized for greater performance and efficiency.
///   The optimization is based on the benchmarking data collected for the challenge.  That data shows the
///   per-generation visited set helps only when the population is dense, so the strategy computes the
///   density of the population at the start of each generation and uses the visited set only when the
///   density is at or above the threshold.
///
///   The generation methods are split into a dedicated path for each option.  A single method with a
///   nullable visited set would remove the duplication, but it would pay a null check at every cell and
///   neighbor in the generations that do not use the set.  A measurement of the three forms, including
///   the single-pass form with aggressive inlining, shows the split form is 0.9% to 4.6% faster with
///   identical allocations and inlining does not recover the difference, so the split form is kept.
///   The full analysis is in the challenge's benchmark area.
/// </remarks>
///
/// <seealso cref="Squire.ConwayLifeChallenge.StrategyBase" />
///
internal sealed class ActiveRegionOptimizedStrategy : StrategyBase
{
    /// <summary>
    ///   The population density at or above which the per-generation visited set is used for the
    ///   generation.
    /// </summary>
    ///
    /// <remarks>
    ///   The value comes from the challenge's benchmark data.  The visited set starts to pay off above a
    ///   crossover between 15% and 20% density, so the threshold sits above that crossover.  At 25%
    ///   density the visiting set measured 11% faster than without it, and the threshold gated the
    ///   approach to within 3.4% of the best of always-use or never-use in every workload measured.  The
    ///   full analysis is in the challenge's benchmark area.
    /// </remarks>
    ///
    private const double DensityThreshold = 0.25;

    /// <inheritdoc />
    public override Strategy Strategy => Strategy.ActiveRegionOptimized;

    /// <inheritdoc />
    public override HashSet<Coordinate> Simulate(HashSet<Coordinate> cells,
                                                 int generations)
    {
        ArgumentNullException.ThrowIfNull(cells, nameof(cells));

        // If there are no generations to simulate, then the result is simply
        // the starting set of live cells.

        if (generations <= 0)
        {
            return [ ..cells ];
        }

        // The simulation requires that each generation be simulated as a pure function,
        // which means that three sets are needed for tracking:
        //
        //   1. The starting set of live cells for the generation.
        //   2. The ending set of live cells for the generation.
        //   3. The set of dead cells that have been visited during the generation.
        //
        // Simulating generations will mutate the sets of cells by design.  To avoid
        // mutating the set of live cells provided by the caller, the initial
        // starting set must be a copy if there are multiple generations to simulate.
        //
        // Unlike the active region strategy, the visited set is allocated lazily on the first
        // generation that uses it.  That avoids paying the allocation and garbage collection
        // cost for populations that stay below the threshold and never use the set.

        var startingAliveCells = generations switch
        {
            > 1 => new HashSet<Coordinate>(cells, cells.Comparer),
            _ => cells
        };

        var endingAliveCells = new HashSet<Coordinate>(cells.Count);
        var visitedCells = default(HashSet<Coordinate>?);

        while (startingAliveCells.Count > 0)
        {
            // The visited set de-duplicates dead cells that are neighbors of more than one live cell,
            // which saves work for dense populations and is wasted overhead for sparse ones.  The
            // density check is a single pass over the live cells and measured under 0.1% of a
            // generation's cost, so it is safe to pay for it to pick the cheaper path.

            if (ComputeDensity(startingAliveCells) >= DensityThreshold)
            {
                visitedCells ??= new HashSet<Coordinate>(startingAliveCells.Count);
                SimulateGeneration(startingAliveCells, endingAliveCells, visitedCells);
            }
            else
            {
                SimulateGenerationNoVistedTracking(startingAliveCells, endingAliveCells);
            }

            // If there are no more generations to simulate, then the result is the
            // current set of live cells after the last generation has been simulated.

            if (--generations <= 0)
            {
                return endingAliveCells;
            }

            // Swap the starting and ending sets for the next generation and clear the working
            // sets for the next generation.

            (startingAliveCells, endingAliveCells) = (endingAliveCells, startingAliveCells);
            endingAliveCells.Clear();
            visitedCells?.Clear();
        }

        return endingAliveCells;
    }

    /// <summary>
    ///   Executes a run for a single generation of the simulation using the visited set.
    /// </summary>
    ///
    /// <param name="startingAliveCells">The initial set of live cells for the generation.</param>
    /// <param name="endingAliveCells">The set of live cells after the generation has been simulated.</param>
    /// <param name="generationVisitedCells">The set of cells that have been visited during the generation.</param>
    ///
    /// <remarks>
    ///   The caller retains ownership of all parameters but must be aware that <paramref name="endingAliveCells" />
    ///   and <paramref name="generationVisitedCells" /> sets will be mutated during simulation.
    ///
    ///   At the end of the simulation, the contents of <paramref name="endingAliveCells"/> will reflect the state of the
    ///   cells for generation.  The content of <paramref name="generationVisitedCells"/> will reflect scratch work for
    ///   tracking used by the simulation and has no meaning to the caller.
    /// </remarks>
    ///
    private static void SimulateGeneration(HashSet<Coordinate> startingAliveCells,
                                           HashSet<Coordinate> endingAliveCells,
                                           HashSet<Coordinate> generationVisitedCells)
    {
        foreach (var cell in startingAliveCells)
        {
            ProcessCell(cell, startingAliveCells, endingAliveCells, generationVisitedCells);
        }
    }

    /// <summary>
    ///   Executes a run for a single generation of the simulation without the visited set.
    /// </summary>
    ///
    /// <param name="startingAliveCells">The initial set of live cells for the generation.</param>
    /// <param name="endingAliveCells">The set of live cells after the generation has been simulated.</param>
    ///
    /// <remarks>
    ///   The caller retains ownership of all parameters but must be aware that <paramref name="endingAliveCells" />
    ///   will be mutated during simulation.  The ending set will reflect the state of the cells for the
    ///   generation at the end of the simulation.
    /// </remarks>
    ///
    private static void SimulateGenerationNoVistedTracking(HashSet<Coordinate> startingAliveCells,
                                                           HashSet<Coordinate> endingAliveCells)
    {
        foreach (var cell in startingAliveCells)
        {
            ProcessCellNoVisitedTracking(cell, startingAliveCells, endingAliveCells);
        }
    }

    /// <summary>
    ///   Processes a cell and, if needed, it's direct neighbors to determine if the cell(s) will be alive
    ///   in the next generation, using the visited set.
    /// </summary>
    ///
    /// <param name="cell">The cell to process.</param>
    /// <param name="startingAliveCells">The initial set of live cells for the generation.</param>
    /// <param name="endingAliveCells">The set of live cells after the generation has been simulated.</param>
    /// <param name="generationVisitedCells">The set of cells that have been visited during the generation.</param>
    /// <param name="depth">Optional.  The current recursive depth of processing.</param>
    ///
    /// <remarks>
    ///   The caller retains ownership of all parameters but must be aware that <paramref name="endingAliveCells" />
    ///   and <paramref name="generationVisitedCells" /> sets will be mutated during simulation.
    ///
    ///   At the end of processing, the cell and any direct neighbors will be added to the contents of <paramref name="endingAliveCells"/>
    ///   if determined to be alive.  Otherwise, they will be added to <paramref name="generationVisitedCells"/>.
    /// </remarks>
    ///
    private static void ProcessCell(Coordinate cell,
                                    HashSet<Coordinate> startingAliveCells,
                                    HashSet<Coordinate> endingAliveCells,
                                    HashSet<Coordinate> generationVisitedCells,
                                    byte depth = 0)
    {
        // Only the direct neighbors of a cell should be processed, so limit the recursive
        // depth to 1.  If the depth is above that or the cell has already been visited or
        // ruled alive for the generation, skip it.

        if ((depth > 1)
            || (generationVisitedCells.Contains(cell))
            || (endingAliveCells.Contains(cell)))
        {
            return;
        }

        // Consider the cell by processing its neighbors.  Because dead neighbors need to
        // be considered for the next generation, direct neighbors are processed recursively with
        // a depth guard to limit considering neighbors-of-neighbors.

        var liveNeighbors = 0;

        ++depth;

        foreach (var neighborCell in cell.GetNeighbors())
        {
            if (startingAliveCells.Contains(neighborCell))
            {
                ++liveNeighbors;
            }
            else
            {
                ProcessCell(neighborCell, startingAliveCells, endingAliveCells, generationVisitedCells, depth);
            }
        }

        // Determine if the cell will be alive in the next generation.  The rules for Conway's Game of Life are:
        //
        //  - If the cell was alive, it survives to the next generation if it has 2 or 3 live neighbors.
        //  - If the cell was dead, it becomes alive in the next generation if it has exactly 3 live neighbors.
        //  - If the cell does not meet the above conditions, it will be dead in the next generation.

        _ = startingAliveCells.Contains(cell) switch
        {
            true when liveNeighbors is 2 or 3 => endingAliveCells.Add(cell),
            false when liveNeighbors is 3 => endingAliveCells.Add(cell),
            false => generationVisitedCells.Add(cell),

            // Cells that were in the starting set of live cells but die don't need to be
            // separately tracked as visited, as they're already tracked in the starting set.

            _ => false
        };
    }

    /// <summary>
    ///   Processes a cell and, if needed, it's direct neighbors to determine if the cell(s) will be alive
    ///   in the next generation, without the visited set.
    /// </summary>
    ///
    /// <param name="cell">The cell to process.</param>
    /// <param name="startingAliveCells">The initial set of live cells for the generation.</param>
    /// <param name="endingAliveCells">The set of live cells after the generation has been simulated.</param>
    /// <param name="depth">Optional.  The current recursive depth of processing.</param>
    ///
    /// <remarks>
    ///   The caller retains ownership of all parameters but must be aware that <paramref name="endingAliveCells" />
    ///   sets will be mutated during simulation.  At the end of processing, the cell and any direct
    ///   neighbors will be added to the contents of <paramref name="endingAliveCells" /> if determined
    ///   to be alive.
    ///
    ///   The split of this method and <see cref="ProcessCell" /> is deliberate.  A single
    ///   method shared by both paths pays a null check at every cell and neighbor, and measured that
    ///   cost is 0.9% to 4.6% of the simulation in every workload.
    /// </remarks>
    ///
    private static void ProcessCellNoVisitedTracking(Coordinate cell,
                                                     HashSet<Coordinate> startingAliveCells,
                                                     HashSet<Coordinate> endingAliveCells,
                                                     byte depth = 0)
    {
        // Only the direct neighbors of a cell should be processed, so limit the recursive
        // depth to 1.  If the depth is above that or the cell has already been ruled alive
        // for the generation, skip it.

        if ((depth > 1)
            || (endingAliveCells.Contains(cell)))
        {
            return;
        }

        // Consider the cell by processing its neighbors.  Because dead neighbors need to
        // be considered for the next generation, direct neighbors are processed recursively with
        // a depth guard to limit considering neighbors-of-neighbors.

        var liveNeighbors = 0;

        ++depth;

        foreach (var neighborCell in cell.GetNeighbors())
        {
            if (startingAliveCells.Contains(neighborCell))
            {
                ++liveNeighbors;
            }
            else
            {
                ProcessCellNoVisitedTracking(neighborCell, startingAliveCells, endingAliveCells, depth);
            }
        }

        // Determine if the cell will be alive in the next generation.  The rules for Conway's Game of Life are:
        //
        //  - If the cell was alive, it survives to the next generation if it has 2 or 3 live neighbors.
        //  - If the cell was dead, it becomes alive in the next generation if it has exactly 3 live neighbors.
        //  - If the cell does not meet the above conditions, it will be dead in the next generation.

        _ = startingAliveCells.Contains(cell) switch
        {
            true when liveNeighbors is 2 or 3 => endingAliveCells.Add(cell),
            false when liveNeighbors is 3 => endingAliveCells.Add(cell),

            // Cells that were in the starting set of live cells but die don't need to be
            // separately tracked as visited, as they're already tracked in the starting set.

            _ => false
        };
    }

    /// <summary>
    ///   Computes the density of a population as the ratio of live cells to bounding box area.
    /// </summary>
    ///
    /// <param name="cells">The population to measure.</param>
    ///
    /// <returns>The density as a value between 0 and 1, or 0 for an empty population.</returns>
    ///
    private static double ComputeDensity(HashSet<Coordinate> cells)
    {
        if (cells.Count == 0)
        {
            return 0;
        }

        var minX = long.MaxValue;
        var maxX = long.MinValue;
        var minY = long.MaxValue;
        var maxY = long.MinValue;

        foreach (var cell in cells)
        {
            if (cell.X < minX)
            {
                minX = cell.X;
            }

            if (cell.X > maxX)
            {
                maxX = cell.X;
            }

            if (cell.Y < minY)
            {
                minY = cell.Y;
            }

            if (cell.Y > maxY)
            {
                maxY = cell.Y;
            }
        }

        var width = maxX - minX + 1;
        var height = maxY - minY + 1;
        var area = width * height;

        return (double)cells.Count / area;
    }
}
