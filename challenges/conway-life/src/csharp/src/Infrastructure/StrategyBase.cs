using Squire.ConwayLifeChallenge.Infrastructure;
using Squire.ConwayLifeChallenge.Strategies;

namespace Squire.ConwayLifeChallenge;

/// <summary>
///   The contract to be satisfied by a strategy implementation for
///   simulating Conway's Game of Life.
/// </summary>
///
internal abstract class StrategyBase
{
    /// <summary>
    ///   The <see cref="Strategy" /> represented by this implementation.
    /// </summary>
    ///
    public abstract Strategy Strategy { get; }

    /// <summary>
    ///   Simulates the specified number of generations.
    /// </summary>
    ///
    /// <param name="cells">The initial set of live cells.</param>
    /// <param name="generations">The number of generations to simulate.</param>
    ///
    /// <returns>The set of live cells after the specified number of generations.</returns>
    ///
    public abstract HashSet<Coordinate> Simulate(HashSet<Coordinate> cells,
                                                 int generations);

    /// <summary>
    ///   Creates a <see cref="StrategyBase" /> which applies the given <paramref name="strategy" />
    ///   to simulate Conway's Game of Life.
    /// </summary>
    ///
    /// <param name="strategy">The strategy to create an implementation for.</param>
    ///
    /// <returns>The <see cref="StrategyBase" /> for the requested <paramref name="strategy" />.</returns>
    ///
    /// <exception cref="ArgumentException">The requested strategy is not recognized.</exception>
    ///
    internal static StrategyBase CreateStrategy(Strategy strategy) => strategy switch
    {
        Strategy.ActiveRegion => new ActiveRegionStrategy(),
        _ => throw new ArgumentException($"Unknown strategy: `{ strategy }`.", nameof(strategy))
    };
}
