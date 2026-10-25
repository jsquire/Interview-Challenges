using BenchmarkDotNet.Attributes;
using Squire.ConwayLifeChallenge.Infrastructure;
using Squire.ConwayLifeChallenge.Strategies;

namespace Squire.ConwayLifeChallenge.Benchmark;

/// <summary>
///   Compares the <see cref="Squire.ConwayLifeChallenge.Strategies.ActiveRegionStrategy" /> (with the
///   per-generation visited set) against <see cref="NoVisitedSetStrategy" /> (without it) across a range
///   of populations and generation counts.
/// </summary>
///
/// <remarks>
///   Both strategies produce identical results.  The only difference is that the visited set avoids
///   re-processing a dead cell that is a neighbor of more than one live cell.  The benchmark measures
///   the real-world cost of that deduplication.
/// </remarks>
///
[MemoryDiagnoser]
public class VisitedSetBenchmarks
{
    /// <summary>The blinker, a period-2 oscillator with three live cells.</summary>
    private readonly HashSet<Coordinate> Blinker =
    [
        new(1, 0), new(2, 0), new(3, 0)
    ];

    /// <summary>The block, a still life with four live cells.</summary>
    private readonly HashSet<Coordinate> Block =
    [
        new(0, 0), new(1, 0), new(0, 1), new(1, 1)
    ];

    /// <summary>The glider, a period-4 spaceship with five live cells.</summary>
    private readonly HashSet<Coordinate> Glider =
    [
        new(1, 0), new(2, 1), new(0, 2), new(1, 2), new(2, 2)
    ];

    /// <summary>The R-pentomino, a methuselah with five live cells.</summary>
    private readonly HashSet<Coordinate> RPentomino =
    [
        new(1, 0), new(2, 0), new(0, 1), new(1, 1), new(1, 2)
    ];

    /// <summary>A random population of 100 live cells.</summary>
    private readonly HashSet<Coordinate> HundredCells = CreateRandomCells(100);

    /// <summary>A random population of 1,000 live cells.</summary>
    private readonly HashSet<Coordinate> ThousandCells = CreateRandomCells(1_000);

    /// <summary>A random population of 10,000 live cells.</summary>
    private readonly HashSet<Coordinate> TenThousandCells = CreateRandomCells(10_000);

    /// <summary>Simulates the blinker for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithVisitedSetBlinker() => new ActiveRegionStrategy().Simulate(Blinker, 1);

    /// <summary>Simulates the blinker for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutVisitedSetBlinker() => new NoVisitedSetStrategy().Simulate(Blinker, 1);

    /// <summary>Simulates the block for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithVisitedSetBlock() => new ActiveRegionStrategy().Simulate(Block, 1);

    /// <summary>Simulates the block for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutVisitedSetBlock() => new NoVisitedSetStrategy().Simulate(Block, 1);

    /// <summary>Simulates the glider for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithVisitedSetGlider() => new ActiveRegionStrategy().Simulate(Glider, 1);

    /// <summary>Simulates the glider for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutVisitedSetGlider() => new NoVisitedSetStrategy().Simulate(Glider, 1);

    /// <summary>Simulates the R-pentomino for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithVisitedSetRPentomino() => new ActiveRegionStrategy().Simulate(RPentomino, 1);

    /// <summary>Simulates the R-pentomino for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutVisitedSetRPentomino() => new NoVisitedSetStrategy().Simulate(RPentomino, 1);

    /// <summary>Simulates the R-pentomino for ten generations with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithVisitedSetRPentominoTenGenerations() => new ActiveRegionStrategy().Simulate(RPentomino, 10);

    /// <summary>Simulates the R-pentomino for ten generations without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutVisitedSetRPentominoTenGenerations() => new NoVisitedSetStrategy().Simulate(RPentomino, 10);

    /// <summary>Simulates the R-pentomino for thirty generations with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithVisitedSetRPentominoThirtyGenerations() => new ActiveRegionStrategy().Simulate(RPentomino, 30);

    /// <summary>Simulates the R-pentomino for thirty generations without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutVisitedSetRPentominoThirtyGenerations() => new NoVisitedSetStrategy().Simulate(RPentomino, 30);

    /// <summary>Simulates 100 random cells for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithVisitedSetHundredCells() => new ActiveRegionStrategy().Simulate(HundredCells, 1);

    /// <summary>Simulates 100 random cells for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutVisitedSetHundredCells() => new NoVisitedSetStrategy().Simulate(HundredCells, 1);

    /// <summary>Simulates 1,000 random cells for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithVisitedSetThousandCells() => new ActiveRegionStrategy().Simulate(ThousandCells, 1);

    /// <summary>Simulates 1,000 random cells for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutVisitedSetThousandCells() => new NoVisitedSetStrategy().Simulate(ThousandCells, 1);

    /// <summary>Simulates 10,000 random cells for one generation with the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithVisitedSetTenThousandCells() => new ActiveRegionStrategy().Simulate(TenThousandCells, 1);

    /// <summary>Simulates 10,000 random cells for one generation without the visited set.</summary>
    [Benchmark]
    public HashSet<Coordinate> WithoutVisitedSetTenThousandCells() => new NoVisitedSetStrategy().Simulate(TenThousandCells, 1);

    /// <summary>
    ///   Creates a deterministic random population of distinct live cells.
    /// </summary>
    ///
    /// <param name="count">The number of distinct live cells to create.</param>
    ///
    /// <returns>A set of <paramref name="count" /> distinct live cells.</returns>
    ///
    private static HashSet<Coordinate> CreateRandomCells(int count)
    {
        var random = new Random(12345);
        var cells = new HashSet<Coordinate>(count);

        while (cells.Count < count)
        {
            var x = random.Next(-1000, 1000);
            var y = random.Next(-1000, 1000);
            _ = cells.Add(new Coordinate(x, y));
        }

        return cells;
    }
}
