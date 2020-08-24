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
            // Because we do not want to mutate the parameter, use LINQ to make a sorted copy rather than
            // Array.Sort which would sort in-place.

            denominations = denominations
                .OrderBy(denomination => denomination)
                .ToArray();

            // Because solutions for a given value and set of denominations depend on those previously calculated, solutions
            // for each subproblem will need to be memoized to avoid the need to recalculate.  This can be done in several ways,
            // including using a hash table or two-dimensional array.
            //
            // Using a two-dimensional array will allow a single upfront allocation to reserve the necessary memory, though would
            // potentially be a poor choice for very large sets due to the need for contiguous memory blocks.  Using a hash table
            // will require potentially allocating each time a key needs to be generated, thus paying a higher cost for lookups
            // despite the O(1) performance of the data structure itself, but would allow for more memory flexibility.
            //
            // This implementation will assume that the value and size of the denominations set are small enough that memory is not
            // a concern, and will use an array for storage.  Likewise, for ease, it will be assumed that the heap allocations
            // needed for use of a dictionary to store each answer set is not a concern within the constraints of performance
            // or memory use.
            //
            // To allow for better readability, include a column for zero values.  This trades a small bit of storage for using
            // the value itself as the index into the table, rather than having to calculate its index by shifting by -1 each time.

            var solutionTable = new Solution[denominations.Length, value + 1];

            // Begin building out the solution table, working through each denomination fully for each potential value.  This
            // supports the relationship between subproblems, allowing each to reference earlier computations to avoid duplication.

            Solution currentSolution;
            int currentDenomination;
            int previousDenominationIndex;
            int usedCount;
            int remainingValue;

            for (var denominationIndex = 0; denominationIndex < denominations.Length; ++denominationIndex)
            {
                for (var currentValue = 0; currentValue <= value; ++currentValue)
                {
                    currentSolution = Solution.Unsolvable;
                    currentDenomination = denominations[denominationIndex];
                    previousDenominationIndex = denominationIndex - 1;

                    // If the value or denomination is not above 0, then there is no solution.

                    if ((value == 0) || (currentDenomination <= 0))
                    {
                        solutionTable[denominationIndex, currentValue] = currentSolution;
                        continue;
                    }

                    // Calculate the solution using the maximum number of the current denomination, if the combination is solvable.

                    usedCount = (int)Math.Floor(currentValue / (decimal)currentDenomination);
                    remainingValue = (currentValue - (usedCount * currentDenomination));

                    // If the current denomination is unable to satisfy the current value in its entirety and there are previously
                    // considered denominations available, then use the already calculated solution for the remaining value.

                    if ((remainingValue > 0) && (denominationIndex != 0))
                    {
                        currentSolution = solutionTable[previousDenominationIndex, remainingValue];

                        if (currentSolution.CoinCount != int.MaxValue)
                        {
                            currentSolution = new Solution(usedCount + currentSolution.CoinCount, new List<CoinUse>(currentSolution.Coins));

                            if (usedCount > 0)
                            {
                                currentSolution.Coins.Add(new CoinUse(currentDenomination, usedCount));
                            }
                        }
                    }
                    else if (remainingValue == 0)
                    {
                        currentSolution = new Solution(usedCount, new List<CoinUse>(denominations.Length) { new CoinUse(currentDenomination, usedCount) });
                    }

                    // If this is the not first denomination calculated, the current solution should be the one that uses the least number of coins,
                    // choosing between making use of the current denomination or not doing so.  This consideration ensures that local maximums do
                    // not cause an incorrect solution.

                    if (denominationIndex != 0)
                    {
                        currentSolution = (currentSolution.CoinCount < solutionTable[previousDenominationIndex, currentValue].CoinCount)
                            ? currentSolution
                            : solutionTable[previousDenominationIndex, currentValue];
                    }

                    solutionTable[denominationIndex, currentValue] = currentSolution;
                }
            }

            // All possible solutions have been calculated; look up the solution for the requested combination.  If the combination was
            // unsolvable, an empty set will be returned.

            return solutionTable[solutionTable.GetLength(0) - 1, solutionTable.GetLength(1) - 1].Coins;
        }

        /// <summary>
        ///   The answer for a specific value and set of denominations, detailing the number of coins and the composition of the set
        ///   of change.
        /// </summary>
        ///
        private struct Solution
        {
            /// <summary>Represents the answer for an unsolvable combination.</summary>
            public static readonly Solution Unsolvable = new Solution(int.MaxValue, new List<CoinUse>(0));

            /// <summary>The number of coins in the answer; <see cref="int.MaxValue" /> if there is no valid solution.</summary>
            public readonly int CoinCount;

            /// <summary>The used coins dominations and count of each in the answer; <c>null</c> if there is no valid solution.</summary>
            public readonly List<CoinUse> Coins;

            /// <summary>
            ///   Initializes a new instance of the <see cref="Solution" />.
            /// </summary>
            ///
            /// <param name="coinCount">The number of coins in the answer; <see cref="int.MaxValue" /> if there is no valid solution.</param>
            /// <param name="coins">The used coins dominations and count of each in the answer; <c>null</c> if there is no valid solution.</param>
            ///
            public Solution(int coinCount, List<CoinUse> coins) => (CoinCount, Coins) = (coinCount, coins);
        }
    }
}
