using BenchmarkDotNet.Attributes;
using Squire.ConwayLifeChallenge.Infrastructure;
using Squire.ConwayLifeChallenge.Strategies;

namespace Squire.ConwayLifeChallenge.Benchmark;

/// <summary>
///   Measures the per-generation cost and allocation of the <see cref="Squire.ConwayLifeChallenge.Strategies.ActiveRegionStrategy" />
///   (with the per-generation visited set) against <see cref="NoVisitedSetStrategy" /> (without it).
/// </summary>
///
/// <remarks>
///   Two workloads are measured.  The density sweep holds the population at 1,000 cells and varies
///   the bounding box to set the density, isolating the variable that determines whether the visited
///   set pays off.  The evolution workload simulates the R-pentomino for an increasing number of
///   generations so the per-generation cost and allocation can be tracked as the pattern grows and
///   settles.  Both report time and allocation so the two can be correlated.
/// </remarks>
///
[MemoryDiagnoser]
public class PerGenerationBenchmarks
{
    /// <summary>1,000 cells in a 2,000 by 2,000 box, a density of 0.025 percent.</summary>
    private readonly HashSet<Coordinate> Sparse = CreateCells(1_000, 2_000);

    /// <summary>1,000 cells in a 500 by 500 box, a density of 0.4 percent.</summary>
    private readonly HashSet<Coordinate> Low = CreateCells(1_000, 500);

    /// <summary>1,000 cells in a 200 by 200 box, a density of 2.5 percent.</summary>
    private readonly HashSet<Coordinate> Medium = CreateCells(1_000, 200);

    /// <summary>1,000 cells in a 100 by 100 box, a density of 10 percent.</summary>
    private readonly HashSet<Coordinate> High = CreateCells(1_000, 100);

    /// <summary>1,000 cells in a 50 by 50 box, a density of 40 percent.</summary>
    private readonly HashSet<Coordinate> VeryHigh = CreateCells(1_000, 50);

    /// <summary>1,000 cells in a 35 by 35 box, a density of 82 percent.</summary>
    private readonly HashSet<Coordinate> Dense = CreateCells(1_000, 35);

    /// <summary>The R-pentomino, a methuselah with five live cells.</summary>
    private readonly HashSet<Coordinate> RPentomino =
    [
        new(1, 0), new(2, 0), new(0, 1), new(1, 1), new(1, 2)
    ];

    /// <summary>Simulates the sparse population for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithSparse() => new ActiveRegionStrategy().Simulate(Sparse, 1);

    /// <summary>Simulates the sparse population for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutSparse() => new NoVisitedSetStrategy().Simulate(Sparse, 1);

    /// <summary>Simulates the low density population for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithLow() => new ActiveRegionStrategy().Simulate(Low, 1);

    /// <summary>Simulates the low density population for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutLow() => new NoVisitedSetStrategy().Simulate(Low, 1);

    /// <summary>Simulates the medium density population for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithMedium() => new ActiveRegionStrategy().Simulate(Medium, 1);

    /// <summary>Simulates the medium density population for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutMedium() => new NoVisitedSetStrategy().Simulate(Medium, 1);

    /// <summary>Simulates the high density population for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithHigh() => new ActiveRegionStrategy().Simulate(High, 1);

    /// <summary>Simulates the high density population for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutHigh() => new NoVisitedSetStrategy().Simulate(High, 1);

    /// <summary>Simulates the very high density population for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithVeryHigh() => new ActiveRegionStrategy().Simulate(VeryHigh, 1);

    /// <summary>Simulates the very high density population for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutVeryHigh() => new NoVisitedSetStrategy().Simulate(VeryHigh, 1);

    /// <summary>Simulates the dense population for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithDense() => new ActiveRegionStrategy().Simulate(Dense, 1);

    /// <summary>Simulates the dense population for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutDense() => new NoVisitedSetStrategy().Simulate(Dense, 1);

    /// <summary>Simulates the R-pentomino for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithRPentominoOne() => new ActiveRegionStrategy().Simulate(RPentomino, 1);

    /// <summary>Simulates the R-pentomino for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutRPentominoOne() => new NoVisitedSetStrategy().Simulate(RPentomino, 1);

    /// <summary>Simulates the R-pentomino for two generations with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithRPentominoTwo() => new ActiveRegionStrategy().Simulate(RPentomino, 2);

    /// <summary>Simulates the R-pentomino for two generations without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutRPentominoTwo() => new NoVisitedSetStrategy().Simulate(RPentomino, 2);

    /// <summary>Simulates the R-pentomino for three generations with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithRPentominoThree() => new ActiveRegionStrategy().Simulate(RPentomino, 3);

    /// <summary>Simulates the R-pentomino for three generations without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutRPentominoThree() => new NoVisitedSetStrategy().Simulate(RPentomino, 3);

    /// <summary>Simulates the R-pentomino for five generations with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithRPentominoFive() => new ActiveRegionStrategy().Simulate(RPentomino, 5);

    /// <summary>Simulates the R-pentomino for five generations without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutRPentominoFive() => new NoVisitedSetStrategy().Simulate(RPentomino, 5);

    /// <summary>Simulates the R-pentomino for ten generations with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithRPentominoTen() => new ActiveRegionStrategy().Simulate(RPentomino, 10);

    /// <summary>Simulates the R-pentomino for ten generations without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutRPentominoTen() => new NoVisitedSetStrategy().Simulate(RPentomino, 10);

    /// <summary>Simulates the R-pentomino for twenty generations with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithRPentominoTwenty() => new ActiveRegionStrategy().Simulate(RPentomino, 20);

    /// <summary>Simulates the R-pentomino for twenty generations without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutRPentominoTwenty() => new NoVisitedSetStrategy().Simulate(RPentomino, 20);

    /// <summary>Simulates the R-pentomino for thirty generations with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithRPentominoThirty() => new ActiveRegionStrategy().Simulate(RPentomino, 30);

    /// <summary>Simulates the R-pentomino for thirty generations without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutRPentominoThirty() => new NoVisitedSetStrategy().Simulate(RPentomino, 30);

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
