using BenchmarkDotNet.Attributes;
using Squire.ConwayLifeChallenge.Infrastructure;
using Squire.ConwayLifeChallenge.Strategies;

namespace Squire.ConwayLifeChallenge.Benchmark;

/// <summary>
///   Answers three questions about the per-generation visited set.
/// </summary>
///
/// <remarks>
///   The first group measures the cost of computing the density of a population and compares it
///   against the cost of a single generation, so the overhead of the density check can be seen
///   relative to the work it gates.  The second group compares the always-use, never-use, and
///   density-gated approaches across sparse, dense, mixed, and R-pentomino workloads.  The third
///   group narrows the crossover density where the visited set starts to pay off.
/// </remarks>
///
[MemoryDiagnoser]
public class DensityAnalysisBenchmarks
{
    /// <summary>The density threshold at or above which the gated approach uses the visited set.</summary>
    private const double Threshold = 0.25;

    /// <summary>100 cells in a 100 by 100 box.</summary>
    private readonly HashSet<Coordinate> Cells100 = CreateCells(100, 100);

    /// <summary>1,000 cells in a 100 by 100 box.</summary>
    private readonly HashSet<Coordinate> Cells1000 = CreateCells(1_000, 100);

    /// <summary>10,000 cells in a 200 by 200 box.</summary>
    private readonly HashSet<Coordinate> Cells10000 = CreateCells(10_000, 200);

    /// <summary>1,000 cells in a 2,000 by 2,000 box, a density of 0.025 percent.</summary>
    private readonly HashSet<Coordinate> Sparse = CreateCells(1_000, 2_000);

    /// <summary>1,000 cells in a 35 by 35 box, a density of 82 percent.</summary>
    private readonly HashSet<Coordinate> Dense = CreateCells(1_000, 35);

    /// <summary>1,000 cells in a 100 by 100 box, a density of 10 percent.</summary>
    private readonly HashSet<Coordinate> Mixed = CreateCells(1_000, 100);

    /// <summary>The R-pentomino, a methuselah with five live cells.</summary>
    private readonly HashSet<Coordinate> RPentomino =
    [
        new(1, 0), new(2, 0), new(0, 1), new(1, 1), new(1, 2)
    ];

    /// <summary>1,000 cells at a density of about 15 percent.</summary>
    private readonly HashSet<Coordinate> Density15 = CreateCells(1_000, 82);

    /// <summary>1,000 cells at a density of about 20 percent.</summary>
    private readonly HashSet<Coordinate> Density20 = CreateCells(1_000, 71);

    /// <summary>1,000 cells at a density of about 25 percent.</summary>
    private readonly HashSet<Coordinate> Density25 = CreateCells(1_000, 63);

    /// <summary>1,000 cells at a density of about 30 percent.</summary>
    private readonly HashSet<Coordinate> Density30 = CreateCells(1_000, 58);

    /// <summary>1,000 cells at a density of about 35 percent.</summary>
    private readonly HashSet<Coordinate> Density35 = CreateCells(1_000, 53);

    /// <summary>Computes the density of 100 cells.</summary>
    [Benchmark]
    public double DensityCost100() => DensityCalculator.GetDensity(Cells100);

    /// <summary>Computes the density of 1,000 cells.</summary>
    [Benchmark]
    public double DensityCost1000() => DensityCalculator.GetDensity(Cells1000);

    /// <summary>Computes the density of 10,000 cells.</summary>
    [Benchmark]
    public double DensityCost10000() => DensityCalculator.GetDensity(Cells10000);

    /// <summary>Simulates 100 cells for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> Generation100() => new ActiveRegionStrategy().Simulate(Cells100, 1);

    /// <summary>Simulates 1,000 cells for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> Generation1000() => new ActiveRegionStrategy().Simulate(Cells1000, 1);

    /// <summary>Simulates 10,000 cells for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> Generation10000() => new ActiveRegionStrategy().Simulate(Cells10000, 1);

    /// <summary>Simulates the sparse population for one generation, always using the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> AlwaysWithSparse() => new ActiveRegionStrategy().Simulate(Sparse, 1);

    /// <summary>Simulates the sparse population for one generation, never using the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> NeverWithSparse() => new NoVisitedSetStrategy().Simulate(Sparse, 1);

    /// <summary>Simulates the sparse population for one generation, using the density-gated approach.</summary>
    [Benchmark]
    public HashSet<Coordinate> GatedSparse() => new DensityGatedStrategy(Threshold).Simulate(Sparse, 1);

    /// <summary>Simulates the dense population for one generation, always using the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> AlwaysWithDense() => new ActiveRegionStrategy().Simulate(Dense, 1);

    /// <summary>Simulates the dense population for one generation, never using the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> NeverWithDense() => new NoVisitedSetStrategy().Simulate(Dense, 1);

    /// <summary>Simulates the dense population for one generation, using the density-gated approach.</summary>
    [Benchmark]
    public HashSet<Coordinate> GatedDense() => new DensityGatedStrategy(Threshold).Simulate(Dense, 1);

    /// <summary>Simulates the mixed population for one generation, always using the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> AlwaysWithMixed() => new ActiveRegionStrategy().Simulate(Mixed, 1);

    /// <summary>Simulates the mixed population for one generation, never using the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> NeverWithMixed() => new NoVisitedSetStrategy().Simulate(Mixed, 1);

    /// <summary>Simulates the mixed population for one generation, using the density-gated approach.</summary>
    [Benchmark]
    public HashSet<Coordinate> GatedMixed() => new DensityGatedStrategy(Threshold).Simulate(Mixed, 1);

    /// <summary>Simulates the R-pentomino for thirty generations, always using the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> AlwaysWithRPentomino() => new ActiveRegionStrategy().Simulate(RPentomino, 30);

    /// <summary>Simulates the R-pentomino for thirty generations, never using the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> NeverWithRPentomino() => new NoVisitedSetStrategy().Simulate(RPentomino, 30);

    /// <summary>Simulates the R-pentomino for thirty generations, using the density-gated approach.</summary>
    [Benchmark]
    public HashSet<Coordinate> GatedRPentomino() => new DensityGatedStrategy(Threshold).Simulate(RPentomino, 30);

    /// <summary>Simulates the 15 percent population for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> With15() => new ActiveRegionStrategy().Simulate(Density15, 1);

    /// <summary>Simulates the 15 percent population for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> Without15() => new NoVisitedSetStrategy().Simulate(Density15, 1);

    /// <summary>Simulates the 20 percent population for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> With20() => new ActiveRegionStrategy().Simulate(Density20, 1);

    /// <summary>Simulates the 20 percent population for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> Without20() => new NoVisitedSetStrategy().Simulate(Density20, 1);

    /// <summary>Simulates the 25 percent population for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> With25() => new ActiveRegionStrategy().Simulate(Density25, 1);

    /// <summary>Simulates the 25 percent population for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> Without25() => new NoVisitedSetStrategy().Simulate(Density25, 1);

    /// <summary>Simulates the 30 percent population for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> With30() => new ActiveRegionStrategy().Simulate(Density30, 1);

    /// <summary>Simulates the 30 percent population for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> Without30() => new NoVisitedSetStrategy().Simulate(Density30, 1);

    /// <summary>Simulates the 35 percent population for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> With35() => new ActiveRegionStrategy().Simulate(Density35, 1);

    /// <summary>Simulates the 35 percent population for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> Without35() => new NoVisitedSetStrategy().Simulate(Density35, 1);

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
