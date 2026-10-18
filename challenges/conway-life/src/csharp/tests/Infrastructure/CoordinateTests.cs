using NUnit.Framework;
using Squire.ConwayLifeChallenge.Infrastructure;

namespace Squire.ConwayLifeChallenge.Tests;

/// <summary>
///   The suite of tests for the <see cref="Coordinate" /> type.
/// </summary>
///
[TestFixture]
public class CoordinateTests
{
    /// <summary>
    ///   Verifies functionality of the <see cref="Coordinate.GetNeighbors" /> method.
    /// </summary>
    ///
    [TestCase(0L, 0L)]
    [TestCase(100L, -200L)]
    [TestCase(-2000000000000L, -2000000000000L)]
    [TestCase(long.MinValue + 1, long.MaxValue - 1)]
    public void GetNeighborsReturnsEightSurroundingCells(long x,
                                                        long y)
    {
        var coordinate = new Coordinate(x, y);
        Coordinate[] expected =
        [
            new(x - 1, y - 1), new(x, y - 1), new(x + 1, y - 1),
            new(x - 1, y),                   new(x + 1, y),
            new(x - 1, y + 1), new(x, y + 1), new(x + 1, y + 1)
        ];
        var actual = new List<Coordinate>();

        foreach (var neighbor in coordinate.GetNeighbors())
        {
            actual.Add(neighbor);
        }

        Assert.That(actual, Is.EquivalentTo(expected), "Each surrounding cell should occur exactly once, excluding the center.");
        Assert.That(coordinate, Is.EqualTo(new Coordinate(x, y)), "Enumeration should not change the source coordinate.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="Coordinate.GetNeighbors" /> method.
    /// </summary>
    ///
    [TestCase(long.MinValue, 0L, 5)]
    [TestCase(long.MaxValue, 0L, 5)]
    [TestCase(0L, long.MinValue, 5)]
    [TestCase(0L, long.MaxValue, 5)]
    [TestCase(long.MinValue, long.MinValue, 3)]
    [TestCase(long.MinValue, long.MaxValue, 3)]
    [TestCase(long.MaxValue, long.MinValue, 3)]
    [TestCase(long.MaxValue, long.MaxValue, 3)]
    public void GetNeighborsOmitsUnrepresentableCells(long x,
                                                      long y,
                                                      int expectedCount)
    {
        var coordinate = new Coordinate(x, y);
        var actual = new HashSet<Coordinate>();
        var count = 0;

        foreach (var neighbor in coordinate.GetNeighbors())
        {
            var distanceX = Int128.Abs((Int128)neighbor.X - x);
            var distanceY = Int128.Abs((Int128)neighbor.Y - y);

            Assert.That(distanceX, Is.LessThanOrEqualTo((Int128)1), "A neighbor must not wrap across the x boundary.");
            Assert.That(distanceY, Is.LessThanOrEqualTo((Int128)1), "A neighbor must not wrap across the y boundary.");
            Assert.That(neighbor, Is.Not.EqualTo(coordinate), "The center is not a neighbor.");

            actual.Add(neighbor);
            count++;
        }

        Assert.That(count, Is.EqualTo(expectedCount), "Only representable neighbors should be returned.");
        Assert.That(actual, Has.Count.EqualTo(expectedCount), "Neighbors should not be duplicated.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="Coordinate.GetNeighbors" /> method.
    /// </summary>
    ///
    [Test]
    public void GetNeighborsSupportsIndependentRepeatedEnumeration()
    {
        var neighbors = new Coordinate(10, 20).GetNeighbors();
        var first = neighbors.GetEnumerator();
        var second = neighbors.GetEnumerator();

        Assert.That(first.MoveNext(), Is.True, "The first enumerator should advance to its first neighbor.");
        Assert.That(first.Current, Is.EqualTo(new Coordinate(9, 19)), "The first enumerator should start at the first neighboring coordinate.");
        Assert.That(first.MoveNext(), Is.True, "The first enumerator should advance to its second neighbor.");
        Assert.That(second.MoveNext(), Is.True, "The second enumerator should independently advance to its first neighbor.");
        Assert.That(second.Current, Is.EqualTo(new Coordinate(9, 19)), "Advancing one enumerator must not advance another.");

        var pairs = 0;

        foreach (var outer in neighbors)
        {
            foreach (var inner in neighbors)
            {
                pairs++;
            }
        }

        Assert.That(pairs, Is.EqualTo(64), "Each traversal should start with independent state.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="Coordinate.GetNeighbors" /> method.
    /// </summary>
    ///
    [Test]
    public void GetNeighborsRemainsExhaustedAfterCompletion()
    {
        var enumerator = new Coordinate(0, 0).GetNeighbors().GetEnumerator();
        var count = 0;

        while (enumerator.MoveNext())
        {
            count++;
        }

        Assert.That(count, Is.EqualTo(8));
        Assert.That(enumerator.MoveNext(), Is.False, "An exhausted enumerator should not restart.");
        Assert.That(enumerator.MoveNext(), Is.False, "Repeated advances should remain exhausted.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="Coordinate.GetNeighbors" /> method.
    /// </summary>
    ///
    [Test]
    public void GetNeighborsDoesNotAllocateDuringDirectEnumeration()
    {
        var coordinate = new Coordinate(10, 20);
        var sum = 0L;

        foreach (var neighbor in coordinate.GetNeighbors())
        {
            sum += neighbor.X;
        }

        var before = GC.GetAllocatedBytesForCurrentThread();

        for (var iteration = 0; iteration < 1000; iteration++)
        {
            foreach (var neighbor in coordinate.GetNeighbors())
            {
                sum += neighbor.X;
            }
        }

        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.That(sum, Is.EqualTo(80080L), "All enumerated coordinates should be consumed.");
        Assert.That(allocated, Is.Zero, "Direct neighbor enumeration should not allocate managed objects.");
    }
}
