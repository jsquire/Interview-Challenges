using Squire.ConwayLifeChallenge.Infrastructure;

namespace Squire.ConwayLifeChallenge.Benchmark;

/// <summary>
///   A benchmark-only variant of <see cref="Squire.ConwayLifeChallenge.Strategies.ActiveRegionStrategy" />
///   that computes the density at the start of each generation and chooses whether to use the
///   per-generation visited set based on that.
/// </summary>
///
/// <remarks>
///   This exists only to measure the cost and benefit of the density-gated approach.  It is not
///   part of the application and is never selected by <see cref="Squire.ConwayLifeChallenge.StrategyBase.CreateStrategy" />.
///
///   The algorithm is identical to <see cref="Squire.ConwayLifeChallenge.Strategies.ActiveRegionStrategy" />
///   except that the visited set is only used when the density is at or above a threshold.  When the
///   density is below the threshold, the visited set is skipped, which saves the allocation and GC
///   cost of tracking dead cells.
/// </remarks>
///
internal sealed class DensityGatedStrategy : StrategyBase
{
    private readonly double Threshold;

    /// <summary>
    ///   Initializes a new instance of the <see cref="DensityGatedStrategy" /> class.
    /// </summary>
    ///
    /// <param name="threshold">The density threshold at or above which the visited set is used.</param>
    ///
    public DensityGatedStrategy(double threshold)
    {
        Threshold = threshold;
    }

    /// <inheritdoc />
    public override Strategy Strategy => Strategy.ActiveRegion;

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
        var visitedCells = new HashSet<Coordinate>(cells.Count);

        while (startingAliveCells.Count > 0)
        {
            if (DensityCalculator.GetDensity(startingAliveCells) >= Threshold)
            {
                SimulateGenerationWithVisitedSet(startingAliveCells, endingAliveCells, visitedCells);
            }
            else
            {
                SimulateGenerationWithoutVisitedSet(startingAliveCells, endingAliveCells);
            }

            if (--generations <= 0)
            {
                return endingAliveCells;
            }

            (startingAliveCells, endingAliveCells) = (endingAliveCells, startingAliveCells);
            endingAliveCells.Clear();
            visitedCells.Clear();
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
    private static void SimulateGenerationWithVisitedSet(HashSet<Coordinate> startingAliveCells,
                                                          HashSet<Coordinate> endingAliveCells,
                                                          HashSet<Coordinate> generationVisitedCells)
    {
        foreach (var cell in startingAliveCells)
        {
            ProcessCellWithVisitedSet(cell, startingAliveCells, endingAliveCells, generationVisitedCells);
        }
    }

    /// <summary>
    ///   Executes a run for a single generation of the simulation without the visited set.
    /// </summary>
    ///
    /// <param name="startingAliveCells">The initial set of live cells for the generation.</param>
    /// <param name="endingAliveCells">The set of live cells after the generation has been simulated.</param>
    ///
    private static void SimulateGenerationWithoutVisitedSet(HashSet<Coordinate> startingAliveCells,
                                                             HashSet<Coordinate> endingAliveCells)
    {
        foreach (var cell in startingAliveCells)
        {
            ProcessCellWithoutVisitedSet(cell, startingAliveCells, endingAliveCells);
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
    private static void ProcessCellWithVisitedSet(Coordinate cell,
                                                   HashSet<Coordinate> startingAliveCells,
                                                   HashSet<Coordinate> endingAliveCells,
                                                   HashSet<Coordinate> generationVisitedCells,
                                                   byte depth = 0)
    {
        if ((depth > 1)
            || (generationVisitedCells.Contains(cell))
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
                ProcessCellWithVisitedSet(neighborCell, startingAliveCells, endingAliveCells, generationVisitedCells, depth);
            }
        }

        _ = startingAliveCells.Contains(cell) switch
        {
            true when liveNeighbors is 2 or 3 => endingAliveCells.Add(cell),
            false when liveNeighbors is 3 => endingAliveCells.Add(cell),
            false => generationVisitedCells.Add(cell),
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
    private static void ProcessCellWithoutVisitedSet(Coordinate cell,
                                                      HashSet<Coordinate> startingAliveCells,
                                                      HashSet<Coordinate> endingAliveCells,
                                                      byte depth = 0)
    {
        if ((depth > 1)
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
                ProcessCellWithoutVisitedSet(neighborCell, startingAliveCells, endingAliveCells, depth);
            }
        }

        _ = startingAliveCells.Contains(cell) switch
        {
            true when liveNeighbors is 2 or 3 => endingAliveCells.Add(cell),
            false when liveNeighbors is 3 => endingAliveCells.Add(cell),
            _ => false
        };
    }
}
