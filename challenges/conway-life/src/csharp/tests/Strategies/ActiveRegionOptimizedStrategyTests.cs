using NUnit.Framework;
using Squire.ConwayLifeChallenge.Infrastructure;
using Squire.ConwayLifeChallenge.Strategies;

namespace Squire.ConwayLifeChallenge.Tests.Strategies;

/// <summary>
///   The suite of tests for the <see cref="ActiveRegionOptimizedStrategy" /> class.
/// </summary>
///
[TestFixture]
public class ActiveRegionOptimizedStrategyTests
{
    /// <summary>
    ///   Verifies functionality of the <see cref="ActiveRegionOptimizedStrategy.Simulate" /> method when
    ///   no generations are requested.
    /// </summary>
    ///
    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(int.MinValue)]
    public void SimulateReturnsACopyOfThePopulationWhenNoGenerationsAreRequested(int generations)
    {
        var population = new HashSet<Coordinate>
        {
            new(1, 2),
            new(3, 4)
        };

        var result = new ActiveRegionOptimizedStrategy().Simulate(population, generations);

        Assert.That(result, Is.EquivalentTo(population), "Zero generations should return the starting population.");
        Assert.That(ReferenceEquals(result, population), Is.False, "No-generations results should not alias the caller's set.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="ActiveRegionOptimizedStrategy.Simulate" /> method for the
    ///   blinker, the simplest known oscillator.
    /// </summary>
    ///
    [Test]
    public void SimulateOscillatesTheBlinkerBetweenHorizontalAndVertical()
    {
        var horizontal = new HashSet<Coordinate>
        {
            new(0, 1),
            new(1, 1),
            new(2, 1)
        };

        var vertical = new HashSet<Coordinate>
        {
            new(1, 0),
            new(1, 1),
            new(1, 2)
        };

        var strategy = new ActiveRegionOptimizedStrategy();

        Assert.That(strategy.Simulate(horizontal, 1), Is.EquivalentTo(vertical), "One generation should rotate the blinker.");
        Assert.That(strategy.Simulate(horizontal, 2), Is.EquivalentTo(horizontal), "Two generations should return the blinker to its start.");
        Assert.That(strategy.Simulate(horizontal, 31), Is.EquivalentTo(vertical), "An odd generation count should leave the blinker vertical.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="ActiveRegionOptimizedStrategy.Simulate" /> method for a
    ///   sparse population, which stays below the density threshold and exercises the path without the
    ///   per-generation visited set.
    /// </summary>
    ///
    [Test]
    public void SimulateMatchesActiveRegionOnSparsePopulations()
    {
        // The cells are spread far enough apart that the population density is about three percent,
        // well below the strategy's threshold.  Each cell has one live neighbor, so every cell dies.

        var sparse = new HashSet<Coordinate>
        {
            new(0, 0),
            new(0, 10),
            new(10, 0),
            new(10, 10)
        };

        var expected = new ActiveRegionStrategy().Simulate(new HashSet<Coordinate>(sparse), 5);
        var result = new ActiveRegionOptimizedStrategy().Simulate(sparse, 5);

        Assert.That(result, Is.EquivalentTo(expected), "Sparse simulations should match the active region strategy.");
        Assert.That(result, Is.Empty, "Cells with a single live neighbor should die.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="ActiveRegionOptimizedStrategy.Simulate" /> method for a
    ///   dense population that evolves across many generations, which exercises the path with the
    ///   per-generation visited set.
    /// </summary>
    ///
    [Test]
    public void SimulateMatchesActiveRegionOnDensePopulations()
    {
        // The R-pentomino is small and dense, so the populations it produces stay at or above the
        // strategy's threshold for their early generations.

        var rPentomino = new HashSet<Coordinate>
        {
            new(1, 0),
            new(2, 0),
            new(0, 1),
            new(1, 1),
            new(1, 2)
        };

        foreach (var generations in new int[] { 1, 10, 30 })
        {
            var expected = new ActiveRegionStrategy().Simulate(new HashSet<Coordinate>(rPentomino), generations);
            var result = new ActiveRegionOptimizedStrategy().Simulate(rPentomino, generations);

            Assert.That(result, Is.EquivalentTo(expected), $"The {generations} generation result should match the active region strategy.");
        }
    }

    /// <summary>
    ///   Verifies that the <see cref="ActiveRegionOptimizedStrategy.Simulate" /> method does not mutate
    ///   the population provided by the caller.
    /// </summary>
    ///
    [Test]
    public void SimulateDoesNotMutateTheCallerPopulation()
    {
        var rPentomino = new HashSet<Coordinate>
        {
            new(1, 0),
            new(2, 0),
            new(0, 1),
            new(1, 1),
            new(1, 2)
        };

        var original = new HashSet<Coordinate>(rPentomino);

        _ = new ActiveRegionOptimizedStrategy().Simulate(rPentomino, 10);

        Assert.That(rPentomino, Is.EquivalentTo(original), "Simulation should not mutate the caller's population.");
    }
}
