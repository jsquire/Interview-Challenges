using Squire.ConwayLifeChallenge.Infrastructure;

namespace Squire.ConwayLifeChallenge.Benchmark;

/// <summary>
///   A benchmark-only variant of <see cref="Squire.ConwayLifeChallenge.Strategies.ActiveRegionStrategy" />
///   that removes the per-generation visited set.
/// </summary>
///
/// <remarks>
///   This exists only to measure the cost of the visited set.  It is not part of the application and
///   is never selected by <see cref="Squire.ConwayLifeChallenge.StrategyBase.CreateStrategy" />.
///
///   The algorithm is identical to <see cref="Squire.ConwayLifeChallenge.Strategies.ActiveRegionStrategy" />
///   except that dead cells are no longer tracked as visited.  A dead cell that is a neighbor of more
///   than one live cell is therefore processed once per live neighbor instead of once per generation.
///   The result is unchanged because a cell's fate depends only on its live neighbors, which are fixed
///   for the generation.
/// </remarks>
///
internal sealed class NoVisitedSetStrategy : StrategyBase
{
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

        while (startingAliveCells.Count > 0)
        {
            SimulateGeneration(startingAliveCells, endingAliveCells);

            if (--generations <= 0)
            {
                return endingAliveCells;
            }

            (startingAliveCells, endingAliveCells) = (endingAliveCells, startingAliveCells);
            endingAliveCells.Clear();
        }

        return endingAliveCells;
    }

    /// <summary>
    ///   Executes a run for a single generation of the simulation.
    /// </summary>
    ///
    /// <param name="startingAliveCells">The initial set of live cells for the generation.</param>
    /// <param name="endingAliveCells">The set of live cells after the generation has been simulated.</param>
    ///
    private static void SimulateGeneration(HashSet<Coordinate> startingAliveCells,
                                           HashSet<Coordinate> endingAliveCells)
    {
        foreach (var cell in startingAliveCells)
        {
            ProcessCell(cell, startingAliveCells, endingAliveCells);
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
    /// <param name="depth">Optional.  The current recursive depth of processing.</param>
    ///
    private static void ProcessCell(Coordinate cell,
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
                ProcessCell(neighborCell, startingAliveCells, endingAliveCells, depth);
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
