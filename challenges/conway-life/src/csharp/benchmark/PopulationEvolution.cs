using Squire.ConwayLifeChallenge.Infrastructure;
using Squire.ConwayLifeChallenge.Strategies;

namespace Squire.ConwayLifeChallenge.Benchmark;

/// <summary>
///   Simulates sparse, dense, and mixed workloads for thirty generations and records the population
///   size and density at each generation.
/// </summary>
///
/// <remarks>
///   This is a measurement, not a benchmark.  It exists to answer whether the initial seed predicts
///   the regime the simulation runs in.  The visited set's benefit depends on the density at each
///   generation, so if the density is stable across generations the initial seed is a reliable
///   predictor, and if it swings it is not.
/// </remarks>
///
public static class PopulationEvolution
{
    /// <summary>
    ///   Runs the population evolution measurement and prints the results.
    /// </summary>
    ///
    public static void Run()
    {
        var sparse = CreateCells(1_000, 2_000);
        var dense = CreateCells(1_000, 35);
        var mixed = CreateCells(1_000, 100);

        var strategy = new ActiveRegionStrategy();

        Console.WriteLine("Generation | Sparse (count, density) | Dense (count, density) | Mixed (count, density)");
        Console.WriteLine("-----------|-------------------------|------------------------|-------------------------");

        for (var gen = 0; gen <= 30; gen++)
        {
            Console.WriteLine(
                $"{gen,10} | {Format(sparse)} | {Format(dense)} | {Format(mixed)}");

            if (gen < 30)
            {
                sparse = strategy.Simulate(sparse, 1);
                dense = strategy.Simulate(dense, 1);
                mixed = strategy.Simulate(mixed, 1);
            }
        }
    }

    /// <summary>
    ///   Formats a population as its count and density.
    /// </summary>
    ///
    /// <param name="cells">The population to format.</param>
    ///
    /// <returns>A string with the count and density.</returns>
    ///
    private static string Format(HashSet<Coordinate> cells)
    {
        if (cells.Count == 0)
        {
            return "0, 0%";
        }

        var (minX, maxX, minY, maxY) = GetBoundingBox(cells);
        var area = (maxX - minX + 1) * (maxY - minY + 1);
        var density = 100.0 * cells.Count / area;

        return $"{cells.Count}, {density:F1}%";
    }

    /// <summary>
    ///   Gets the bounding box of a population.
    /// </summary>
    ///
    /// <param name="cells">The population to measure.</param>
    ///
    /// <returns>The minimum and maximum X and Y coordinates.</returns>
    ///
    private static (long MinX, long MaxX, long MinY, long MaxY) GetBoundingBox(HashSet<Coordinate> cells)
    {
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

        return (minX, maxX, minY, maxY);
    }

    /// <summary>
    ///   Creates a deterministic random population of distinct live cells within a square box.
    /// </summary>
    ///
    /// <param name="count">The number of distinct live cells to create.</param>
    /// <param name="boxSize">The side length of the square box the cells are placed in.</param>
    ///
    /// <returns>A set of <paramref name="count" /> distinct live cells.</returns>
    ///
    private static HashSet<Coordinate> CreateCells(int count, int boxSize)
    {
        var random = new Random(12345);
        var half = boxSize / 2;
        var cells = new HashSet<Coordinate>(count);

        while (cells.Count < count)
        {
            var x = random.Next(-half, half);
            var y = random.Next(-half, half);
            _ = cells.Add(new Coordinate(x, y));
        }

        return cells;
    }
}
