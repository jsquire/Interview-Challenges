namespace Squire.ConwayLifeChallenge.Infrastructure;

/// <summary>
///   Represents a coordinate in the 64-bit signed integer space.
/// </summary>
///
/// <param name="X">The x coordinate.</param>
/// <param name="Y">The y coordinate.</param>
///
public readonly record struct Coordinate(long X, long Y)
{
    /// <summary>
    ///   Enumerates the surrounding cells whose coordinates are within the signed 64-bit range.
    /// </summary>
    ///
    /// <returns>The neighboring coordinates, excluding this cell and without wrapping at the range boundaries.</returns>
    ///
    /// <remarks>
    ///   The Moore neighborhood of range one is the 3-by-3 square containing a center cell and
    ///   the eight positions touching it horizontally, vertically, or diagonally.  In this diagram,
    ///   C is this coordinate and each N is a surrounding position, regardless of whether it is alive.
    ///   <code>
    ///   N N N
    ///   N C N
    ///   N N N
    ///   </code>
    ///   This method enumerates the N positions only.  Life counts live cells at those positions,
    ///   never the center itself; the center's alive/dead state is a separate input to the rule.
    ///
    ///   Coordinates outside the signed 64-bit range are omitted rather than wrapped to the opposite
    ///   side.  An interior cell has eight neighbors, an edge cell has five, and a corner cell has three.
    /// </remarks>
    ///
    /// <seealso href="https://mathworld.wolfram.com/MooreNeighborhood.html" />
    ///
    internal CoordinateNeighborEnumerable GetNeighbors() => new(this);
}
