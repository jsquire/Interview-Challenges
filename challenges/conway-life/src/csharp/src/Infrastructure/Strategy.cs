namespace Squire.ConwayLifeChallenge;

/// <summary>
///   Represents the strategy to be used for simulating Conway's Game of Life.
/// </summary>
///
public enum Strategy
{
    /// <summary>
    ///   An intuitive approach should be applied, staying close to the original inspiration for solving the challenge.
    /// </summary>
    ///
    ActiveRegion,

    /// <summary>
    ///   An intuitive approach should be applied, optimized for greater performance and efficiency.  This is a
    ///   version of the original inspiration, optimized based on the data from the challenge's benchmarking.
    /// </summary>
    ///
    ActiveRegionOptimized
}
