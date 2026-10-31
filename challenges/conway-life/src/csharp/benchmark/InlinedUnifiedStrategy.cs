using System.Runtime.CompilerServices;
using Squire.ConwayLifeChallenge.Infrastructure;

namespace Squire.ConwayLifeChallenge.Benchmark;

/// <summary>
///   A benchmark-only variant of <see cref="Squire.ConwayLifeChallenge.Strategies.ActiveRegionOptimizedStrategy" />
///   that runs the single-pass form of the generation methods with aggressive inlining.
/// </summary>
///
/// <remarks>
///   This exists only to measure whether the cost that the split form of the optimized strategy wins
///   is the cost of separate methods for each path.  It is not part of the application and is never
///   selected by <see cref="Squire.ConwayLifeChallenge.StrategyBase.CreateStrategy" />.
///
///   The algorithm is identical to the optimized strategy, and identical to <see cref="UnifiedPathStrategy" />,
///   which is the same single-pass form without inlining.  The generation methods here are marked
///   <see cref="MethodImplAttribute" /> with <c>AggressiveInlining</c>, so the only difference
///   between the two single-pass forms is the inlining directive on those methods.
///
///   If the single-pass cost is call overhead, this form should close the gap toward the split form.
///   If it is the cost of the null check in the guard, it lands near <see cref="UnifiedPathStrategy" />,
///   because the null check can't be inlined away.
/// </remarks>
///
internal sealed class InlinedUnifiedStrategy : StrategyBase
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
