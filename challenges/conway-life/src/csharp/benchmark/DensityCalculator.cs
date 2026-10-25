using Squire.ConwayLifeChallenge.Infrastructure;

namespace Squire.ConwayLifeChallenge.Benchmark;

/// <summary>
///   Computes the density of a population as the ratio of live cells to bounding box area.
/// </summary>
///
/// <remarks>
///   The density is the number of live cells divided by the area of the bounding box that
///   contains them.  It is a single pass over the live cells, so it is O(n) in the population
///   size.  This is the value the <see cref="DensityGatedStrategy" /> uses to decide whether
///   the per-generation visited set pays off.
/// </remarks>
///
internal static class DensityCalculator
{
    /// <summary>
    ///   Computes the density of a population as the ratio of live cells to bounding box area.
    /// </summary>
    ///
    /// <param name="cells">The population to measure.</param>
    ///
    /// <returns>The density as a value between 0 and 1, or 0 for an empty population.</returns>
    ///
    public static double GetDensity(HashSet<Coordinate> cells)
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
