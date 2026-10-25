using System.Runtime.CompilerServices;
using Squire.ConwayLifeChallenge.Infrastructure;

namespace Squire.ConwayLifeChallenge.Strategies;

/// <summary>
///   An intuitive approach for simulating Conway's Game of Life.
/// </summary>
///
/// <remarks>
///   This was the intuitive solution that Jesse first conceptualized, without doing any research
///   or analysis of common solutions and techniques for solving the problem.
/// </remarks>
///
/// <seealso cref="Squire.ConwayLifeChallenge.StrategyBase" />
///
internal class ActiveRegionStrategy : StrategyBase
{
    /// <inheritdoc />
    public override Strategy Strategy => Strategy.ActiveRegion;

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

        var startingAliveCells = generations switch
        {
            > 1 => new HashSet<Coordinate>(cells, cells.Comparer),
            _ => cells
        };

        var endingAliveCells = new HashSet<Coordinate>(cells.Count);
        var visitedCells = new HashSet<Coordinate>(cells.Count);

        while (startingAliveCells.Count > 0)
        {
            SimulateGeneration(startingAliveCells, endingAliveCells, visitedCells);

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
            visitedCells.Clear();
        }

        return endingAliveCells;
    }

    /// <summary>
    ///   Executes a run for a single generation of the simulation.
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    ///   Processes a cell and, if needed, it's direct neighbors to determine if the cell(s) will be alive
    ///   in the next generation.
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
}
