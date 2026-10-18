namespace Squire.ConwayLifeChallenge.Infrastructure;

/// <summary>
///   Enumerates neighboring coordinates without allocating a collection.
/// </summary>
///
/// <param name="coordinate">The cell whose neighbors are enumerated.</param>
///
internal readonly struct CoordinateNeighborEnumerable(Coordinate coordinate)
{
    /// <summary>
    ///   Creates independent traversal state for the neighboring cells.
    /// </summary>
    ///
    /// <returns>An enumerator positioned before the first neighbor.</returns>
    ///
    public Enumerator GetEnumerator() => new(coordinate);

    /// <summary>
    ///   Visits the representable cells surrounding a coordinate.
    /// </summary>
    ///
    /// <param name="coordinate">The cell whose neighbors are enumerated.</param>
    ///
    public struct Enumerator(Coordinate coordinate)
    {
        /// <summary>The center coordinate to compute the neighbors of.</summary>
        private readonly Coordinate Center = coordinate;

        /// <summary>The index of the current neighbor at a point of time while enumerating.</summary>
        private int _index;

        /// <summary>
        ///   The coordinate selected by the last successful call to <see cref="MoveNext" />.
        /// </summary>
        ///
        public Coordinate Current { readonly get; private set; }

        /// <summary>
        ///   Advances to the next neighbor within the signed 64-bit range.
        /// </summary>
        ///
        /// <returns><c>true</c> if another neighbor is available; otherwise, <c>false</c>.</returns>
        ///
        public bool MoveNext()
        {
            while (_index < 9)
            {
                var index = _index++;

                // The nine positions form a 3-by-3 square; its center is not a neighbor.

                if (index == 4)
                {
                    continue;
                }

                // Remainder selects the column and integer division selects the row.
                // Subtracting one centers both offsets on zero, avoiding an offset lookup array.

                var offsetX = (index % 3) - 1;
                var offsetY = (index / 3) - 1;

                // Check before adding so opposite boundaries never become adjacent through overflow.

                if (((offsetX < 0) && (Center.X == long.MinValue))
                    || ((offsetX > 0) && (Center.X == long.MaxValue))
                    || ((offsetY < 0) && (Center.Y == long.MinValue))
                    || ((offsetY > 0) && (Center.Y == long.MaxValue)))
                {
                    continue;
                }

                Current = new(Center.X + offsetX, Center.Y + offsetY);
                return true;
            }

            return false;
        }
    }
}
