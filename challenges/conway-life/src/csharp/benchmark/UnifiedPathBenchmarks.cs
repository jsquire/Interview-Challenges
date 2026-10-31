using BenchmarkDotNet.Attributes;
using Squire.ConwayLifeChallenge.Infrastructure;
using Squire.ConwayLifeChallenge.Strategies;

namespace Squire.ConwayLifeChallenge.Benchmark;

/// <summary>
///   Measures the cost of the split form of the optimized strategy against the single-pass form
///   that carries an optional visited set.
/// </summary>
///
/// <remarks>
///   Both strategies are algorithmically identical, including the density gate and the lazy
///   allocation of the visited set.  The only difference is the structure of the generation
///   methods.  The split form runs a dedicated method for each path, and the single-pass form
///   runs one method that carries the visited set as a nullable parameter and checks it for null
///   on every cell and neighbor processed.
///
///   The workloads cover the three regimes the gate produces.  Sparse and mixed stay well below
///   the threshold for their entire run, where the null check is paid on every cell and neighbor.
///   Dense crosses above the threshold partway through the run, where the null check is paid in
///   addition to the set lookup.  The R-pentomino crosses the threshold over its lifetime.  The
///   time difference is the cost of the null check in the single-pass form, and the allocation
///   should be identical if the form is neutral there.
///
///   The single-pass form is also measured with its generation methods marked aggressive inline.
///   If the single-pass cost is the cost of separate methods, the inlined form should close the
///   gap toward the split form.  If it is the cost of the null check, the inlined form lands
///   near the non-inlined form, because a null check can't be inlined away.
/// </remarks>
///
[MemoryDiagnoser]
public class UnifiedPathBenchmarks
{
    /// <summary>1,000 cells in a 2,000 by 2,000 box, a density of 0.025 percent.</summary>
    private readonly HashSet<Coordinate> Sparse = CreateCells(1_000, 2_000);

    /// <summary>1,000 cells in a 100 by 100 box, a density of 10 percent.</summary>
    private readonly HashSet<Coordinate> Mixed = CreateCells(1_000, 100);

    /// <summary>1,000 cells in a 35 by 35 box, a density of 82 percent.</summary>
    private readonly HashSet<Coordinate> Dense = CreateCells(1_000, 35);

    /// <summary>The R-pentomino, a methuselah with five live cells.</summary>
    private readonly HashSet<Coordinate> RPentomino =
    [
        new(1, 0), new(2, 0), new(0, 1), new(1, 1), new(1, 2)
    ];

    /// <summary>Simulates the sparse population for thirty generations with the split form.</summary>
    [Benchmark]
    public HashSet<Coordinate> SplitSparse() => new ActiveRegionOptimizedStrategy().Simulate(Sparse, 30);

    /// <summary>Simulates the sparse population for thirty generations with the single-pass form.</summary>
    [Benchmark]
    public HashSet<Coordinate> UnifiedSparse() => new UnifiedPathStrategy().Simulate(Sparse, 30);

    /// <summary>Simulates the sparse population for thirty generations with the inlined single-pass form.</summary>
    [Benchmark]
    public HashSet<Coordinate> InlinedSparse() => new InlinedUnifiedStrategy().Simulate(Sparse, 30);

    /// <summary>Simulates the mixed population for thirty generations with the split form.</summary>
    [Benchmark]
    public HashSet<Coordinate> SplitMixed() => new ActiveRegionOptimizedStrategy().Simulate(Mixed, 30);

    /// <summary>Simulates the mixed population for thirty generations with the single-pass form.</summary>
    [Benchmark]
    public HashSet<Coordinate> UnifiedMixed() => new UnifiedPathStrategy().Simulate(Mixed, 30);

    /// <summary>Simulates the mixed population for thirty generations with the inlined single-pass form.</summary>
    [Benchmark]
    public HashSet<Coordinate> InlinedMixed() => new InlinedUnifiedStrategy().Simulate(Mixed, 30);

    /// <summary>Simulates the dense population for thirty generations with the split form.</summary>
    [Benchmark]
    public HashSet<Coordinate> SplitDense() => new ActiveRegionOptimizedStrategy().Simulate(Dense, 30);

    /// <summary>Simulates the dense population for thirty generations with the single-pass form.</summary>
    [Benchmark]
    public HashSet<Coordinate> UnifiedDense() => new UnifiedPathStrategy().Simulate(Dense, 30);

    /// <summary>Simulates the dense population for thirty generations with the inlined single-pass form.</summary>
    [Benchmark]
    public HashSet<Coordinate> InlinedDense() => new InlinedUnifiedStrategy().Simulate(Dense, 30);

    /// <summary>Simulates the R-pentomino for thirty generations with the split form.</summary>
    [Benchmark]
    public HashSet<Coordinate> SplitRPentomino() => new ActiveRegionOptimizedStrategy().Simulate(RPentomino, 30);

    /// <summary>Simulates the R-pentomino for thirty generations with the single-pass form.</summary>
    [Benchmark]
    public HashSet<Coordinate> UnifiedRPentomino() => new UnifiedPathStrategy().Simulate(RPentomino, 30);

    /// <summary>Simulates the R-pentomino for thirty generations with the inlined single-pass form.</summary>
    [Benchmark]
    public HashSet<Coordinate> InlinedRPentomino() => new InlinedUnifiedStrategy().Simulate(RPentomino, 30);

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
