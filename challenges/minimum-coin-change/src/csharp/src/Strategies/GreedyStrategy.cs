namespace Squire.MinimumCoinChallenge.Strategies
{
    /// <summary>
    ///   Solves the minimum coin challenge using a greedy strategy.
    /// </summary>
    ///
    /// <seealso cref="IStrategy" />
    ///
    internal class GreedyStrategy : IStrategy
    {
        /// <inheritdoc/>
        public Strategy Strategy => Strategy.Greedy;

        /// <inheritdoc/>
        public IReadOnlyList<CoinUse> Solve(int value,
                                            int[] denominations)
        {
            var solution = new List<CoinUse>(denominations.Length);

            // If the value is zero or set of denominations was empty, the challenge cannot be solved.

            if ((value <= 0) || (denominations.Length == 0))
            {
                return solution;
            }

            // The order that calculations are performed is significant because of the relationship between
            // answers; the denominations should be processed in decreasing order to ensure that the largest
            // denominational is preferred, as it will always utilize fewer coins than lower denominations.
            //
            // Because we do not want to mutate the parameter, make a sorted copy rather than a direct use of
            // Array.Sort which would sort in-place.  The assumption being made is that the number of dominations
            // is a small enough set that using a stack allocation for the copy is reasonable.

           var sortedDenominations = (Span<int>)stackalloc int[denominations.Length];

            denominations.CopyTo(sortedDenominations);
            sortedDenominations.Sort(static (left, right) => right.CompareTo(left));

            // Consider each denomination in turn, using as many as possible without exceeding the requested
            // value.

            int usedCount;

            foreach (var denomination in sortedDenominations)
            {
                // It is not possible for any denomination of zero or below to be used.  Guard against a
                // potential division error when calculating.

                usedCount = denomination > 0
                    ? (value / denomination)
                    : 0;

                // If any of the denomination was used, calculate the amount for the number of coins of that
                // denomination used.  The amount will be included in the solution and the remaining value needed
                // should be decreased by that amount.

                if (usedCount > 0)
                {
                    value = (value - (usedCount * denomination));
                    solution.Add(new CoinUse(denomination, usedCount));

                    // No need to consider smaller denominations if value is fully satisfied.

                    if (value == 0)
                    {
                        break;
                    }
                }
            }

            // The solution contains any coins that were used; if the value was able to be satisfied, then
            // the solution is valid; otherwise, the combination was unable to be solved.

            return (value == 0)
                ? solution
                : Array.Empty<CoinUse>();
        }
    }
}
