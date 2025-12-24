using System;
using System.Collections.Generic;
using System.Linq;

namespace Squire.MinimumCoinChallenge.Strategies
{
    /// <summary>
    ///   Solves the minimum coin challenge using a dynamic programming strategy.
    /// </summary>
    ///
    /// <seealso cref="IStrategy" />
    ///
    internal class DynamicStrategy : IStrategy
    {
        /// <inheritdoc/>
        public Strategy Strategy => Strategy.Dynamic;

        /// <inheritdoc/>
        public IReadOnlyList<CoinUse> Solve(int value,
                                            int[] denominations)
        {
            // If the value is zero or set of denominations was empty, the challenge cannot be solved.

            if ((value <= 0) || (denominations.Length == 0))
            {
                return Array.Empty<CoinUse>();
            }

            // The order that calculations are performed is significant because of the relationship between
            // answers; the denominations should be processed in increasing order.
            //
            // Because we do not want to mutate the parameter, make a sorted copy rather than a direct use of
            // Array.Sort which would sort in-place.  The assumption being made is that the number of dominations
            // is a small enough set that using a stack allocation for the copy is reasonable.

            var sortedDenominations = (Span<int>)stackalloc int[denominations.Length];

            denominations.CopyTo(sortedDenominations);
            sortedDenominations.Sort();

            // Because solutions for a given value and set of denominations depend on those previously calculated, solutions
            // for each subproblem will need to be memoized to avoid the need to recalculate.  This can be done in several ways,
            // including using a hash table or two-dimensional array.
            //
            // To minimize allocations and improve cache locality, this implementation uses a single-dimensional array
            // of value tuples storing only count and the index of the coin used from the sorted set of dimensions. This
            // avoids per-cell List allocations for the solutions table as each is calculated.
            //
            // To allow for better readability, include a slot for zero values.  This trades a small bit of storage for using
            // the value itself as the index into the table, rather than having to calculate its index by shifting by -1 each time.

            var solutions = new (int CoinCount, int LastCoinIndex)[value + 1];

            // Initialize the base case: zero value requires zero coins. All other values start as unsolvable (int.MaxValue coins, no last coin).

            solutions[0] = (0, -1);

            for (var index = 1; index <= value; ++index)
            {
                solutions[index] = (int.MaxValue, -1);
            }

            // Begin building out the solution table, working through each denomination fully for each potential value.  This
            // supports the relationship between subproblems, allowing each to reference earlier computations to avoid duplication.

            for (var currentValue = 1; currentValue <= value; ++currentValue)
            {
                for (var denominationIndex = 0; denominationIndex < sortedDenominations.Length; ++denominationIndex)
                {
                    var denomination = sortedDenominations[denominationIndex];

                    // It is not possible for any denomination of zero or below to be used, nor can a denomination larger than
                    // the current value contribute to a solution.

                    if ((denomination <= 0) || (denomination > currentValue))
                    {
                        continue;
                    }

                    var remainingValue = currentValue - denomination;
                    var previousSolution = solutions[remainingValue];

                    // If the subproblem for the remaining value is solvable and using this denomination results in fewer total than
                    // the current best solution, adopt it.  This consideration ensures that local maximums do not cause an incorrect solution.

                    if ((previousSolution.CoinCount != int.MaxValue)
                        && (previousSolution.CoinCount + 1 < solutions[currentValue].CoinCount))
                    {
                        solutions[currentValue] = (previousSolution.CoinCount + 1, denominationIndex);
                    }
                }
            }

            // All possible solutions have been calculated; check if the requested value was solvable. If the combination was unsolvable,
            // an empty set is returned.

            if (solutions[value].CoinCount == int.MaxValue)
            {
                return Array.Empty<CoinUse>();
            }

            // Backtrack through the values and construct the solution.  Each step reveals the denomination used, allowing the number
            // of each to be counted to build the final list.
            //
            // This approach requires a small bit of work to finalize, but allows a single list allocation for the final result rather
            // than needing to allocate during each iteration when solving.

            var coinCounts = (Span<int>)stackalloc int[sortedDenominations.Length];
            var remaining = value;
            var usedDenominationCount = 0;

            while (remaining > 0)
            {
                var coinIndex = solutions[remaining].LastCoinIndex;

                if (coinCounts[coinIndex] == 0)
                {
                    usedDenominationCount++;
                }

                coinCounts[coinIndex]++;
                remaining -= sortedDenominations[coinIndex];
            }

            // Build the final result as a single List allocation containing the denomination and count pairs.

            var result = new List<CoinUse>(usedDenominationCount);

            for (var index = 0; index < sortedDenominations.Length; ++index)
            {
                if (coinCounts[index] > 0)
                {
                    result.Add(new CoinUse(sortedDenominations[index], coinCounts[index]));
                }
            }


            return result;
        }
    }
}
