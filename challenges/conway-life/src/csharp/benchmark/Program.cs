using BenchmarkDotNet.Running;

namespace Squire.ConwayLifeChallenge.Benchmark;

/// <summary>
///   Hosts the entry point for the benchmark suite.
/// </summary>
///
public static class Program
{
    /// <summary>
    ///   Runs the benchmark suite.
    /// </summary>
    ///
    public static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--evolution")
        {
            PopulationEvolution.Run();
            return;
        }

        if (args.Length > 0 && args[0] == "--unified-path")
        {
            _ = BenchmarkRunner.Run<UnifiedPathBenchmarks>();
            return;
        }

        _ = BenchmarkRunner.Run<DensityAnalysisBenchmarks>();
    }
}
