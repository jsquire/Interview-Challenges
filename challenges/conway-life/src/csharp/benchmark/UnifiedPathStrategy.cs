using Squire.ConwayLifeChallenge.Infrastructure;

namespace Squire.ConwayLifeChallenge.Benchmark;

/// <summary>
///   A benchmark-only variant of <see cref="Squire.ConwayLifeChallenge.Strategies.ActiveRegionOptimizedStrategy" />
///   that runs the single-pass form of the generation methods.
/// </summary>
///
/// <remarks>
///   This exists only to measure the cost of the proposed refactor of the optimized strategy from its
///   split form of dedicated generation methods for each path to a single pass that carries an
///   optional visited set for each generation.  It is not part of the application and is never
///   selected by <see cref="Squire.ConwayLifeChallenge.StrategyBase.CreateStrategy" />.
///
///   The algorithm is identical to the optimized strategy.  The only difference is that
///   <c>SimulateGeneration</c> and <c>ProcessCell</c> are single methods that take a nullable
///   visited set, which is passed as null for generations where the density gate is off and
///   checked for null on every cell and neighbor processed.
/// </remarks>
///
internal sealed class UnifiedPathStrategy : StrategyBase
{
    private const double DensityThreshold = 0.25;

    /// <inheritdoc />
    public override Strategy Strategy => Strategy.ActiveRegionOptimized;

    /// <inheritdoc />
    public override HashSet<Coordinate> Simulate(HashSet<Coordinate> cells,
                                                 int generations)
    {
        ArgumentNullException.ThrowIfNull(cells, nameof(cells));

        if (generations <= 0)
        {
            return [ ..cells ];
        }

        var startingAliveCells = generations switch
        {
            > 1 => new HashSet<Coordinate>(cells, cells.Comparer),
            _ => cells
        };

        var endingAliveCells = new HashSet<Coordinate>(cells.Count);
        var visitedCells = default(HashSet<Coordinate>?);

        while (startingAliveCells.Count > 0)
        {
            var generationVisitedCells = ComputeDensity(startingAliveCells) >= DensityThreshold
                ? (visitedCells ??= new HashSet<Coordinate>(startingAliveCells.Count))
                : default(HashSet<Coordinate>?);

            SimulateGeneration(startingAliveCells, endingAliveCells, generationVisitedCells);

            if (--generations <= 0)
            {
                return endingAliveCells;
            }

            (startingAliveCells, endingAliveCells) = (endingAliveCells, startingAliveCells);
            endingAliveCells.Clear();
            visitedCells?.Clear();
        }

        return endingAliveCells;
    }

    /// <summary>
    ///   Executes a run for a single generation of the simulation.
    /// </summary>
    ///
    /// <param name="startingAliveCells">The initial set of live cells for the generation.</param>
    /// <param name="endingAliveCells">The set of live cells after the generation has been simulated.</param>
    /// <param name="generationVisitedCells">Optional.  The set of cells that have been visited during the generation.</param>
    ///
    private static void SimulateGeneration(HashSet<Coordinate> startingAliveCells,
                                           HashSet<Coordinate> endingAliveCells,
                                           HashSet<Coordinate>? generationVisitedCells)
    {
        foreach (var cell in startingAliveCells)
        {
            ProcessCell(cell, startingAliveCells, endingAliveCells, generationVisitedCells);
        }
    }

    /// <summary>
    ///   Processes a cell and, if needed, it's direct neighbors to determine if the cell(s) will be alive
    ///   in the next generation.
    /// </summary>
    ///
    /// <param name="cell">The cell to process.</param>
    /// <param name="startingAliveCells">The initial set of live cells for the generation.</param>
    /// <param name="endingAliveCells">The set of live cells after the generation has been simulated.</param>
    /// <param name="generationVisitedCells">Optional.  The set of cells that have been visited during the generation.</param>
    /// <param name="depth">Optional.  The current recursive depth of processing.</param>
    ///
    private static void ProcessCell(Coordinate cell,
                                    HashSet<Coordinate> startingAliveCells,
                                    HashSet<Coordinate> endingAliveCells,
                                    HashSet<Coordinate>? generationVisitedCells,
                                    byte depth = 0)
    {
        if ((depth > 1)
            || (generationVisitedCells is not null && generationVisitedCells.Contains(cell))
            || (endingAliveCells.Contains(cell)))
        {
            return;
        }

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

        _ = startingAliveCells.Contains(cell) switch
        {
            true when liveNeighbors is 2 or 3 => endingAliveCells.Add(cell),
            false when liveNeighbors is 3 => endingAliveCells.Add(cell),
            false => generationVisitedCells?.Add(cell) is true,
            _ => false
        };
    }

    /// <summary>
    ///   Computes the density of a population as the ratio of live cells to bounding box area.
    /// </summary>
    ///
    /// <param name="cells">The population to measure.</param>
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
