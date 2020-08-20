using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public Dictionary<int, int> Solve(int value,
                                          int[] denominations)
        {
            var solution = new Dictionary<int, int>();

            // If the value is zero or set of denominations was empty, the challenge cannot be solved.

            if ((value <= 0) || (denominations.Length == 0))
            {
                return solution;
            }

            // The order that calculations are performed is significant because of the relationship between
            // answers; the denominations should be processed in decreasing order to ensure that the largest
            // denominational is preferred, as it will always utilize fewer coins than lower denominations.
            //
            // Because we do not want to mutate the parameter, use LINQ to make a sorted copy rather than
            // Array.Sort which would sort in-place.

            denominations = denominations
                .OrderByDescending(denomination => denomination)
                .ToArray();

            // Consider each denomination in turn, using as many as possible without exceeding the requested
            // value.

            int usedCount;

            foreach (var denomination in denominations)
            {
                // It is not possible for any denomination of zero or below to be used.  Guard against a
                // potential division error when calculating.

                usedCount = denomination > 0
                    ? (int)Math.Floor(value / (decimal)denomination)
                    : 0;

                // If any of the denomination was used, calculate the amount for the number of coins of that
                // denomination used.  The amount will be included in the solution and the remaining value needed
                // should be decreased by that amount.

                if (usedCount > 0)
                {
                    value = (value - (usedCount * denomination));
                    solution.Add(denomination, usedCount);
                }
            }

            // The solution contains any coins that were used; if the value was able to be satisfied, then
            // the solution is valid; otherwise, the combination was unable to be solved.

            return (value == 0)
                ? solution
                : new Dictionary<int, int>();
        }
    }
}
