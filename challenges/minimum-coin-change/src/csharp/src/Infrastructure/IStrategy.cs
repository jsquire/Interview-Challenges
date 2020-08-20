using System.Collections.Generic;

namespace Squire.MinimumCoinChallenge
{
    /// <summary>
    ///   The contract to be satisfied by a strategy implementation for
    ///   solving the challenge.
    /// </summary>
    ///
    internal interface IStrategy
    {
        /// <summary>
        ///   The <see cref="Strategy" /> represented by this implementation.
        /// </summary>
        ///
        public Strategy Strategy { get; }

        /// <summary>
        ///   Solves the challenge for the desired value and set of available denominations.
        /// </summary>
        ///
        /// <param name="value">The desired monetary value to calculate change for.</param>
        /// <param name="denominations">The set of coin denominations available to make change with.</param>
        ///
        /// <returns>The mapping of denomination-to-amount that represents the solution; if the set is empty, no solution was found.</returns>
        ///
        /// <remarks>
        ///  As an internal construct, it is expected that the parameters have already been
        ///  validated and, thus, are legal values.
        /// </remarks>
        ///
        public Dictionary<int, int> Solve(int value,
                                          int[] denominations);
    }
}
