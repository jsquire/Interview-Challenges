using System.Collections;

namespace Squire.MinimumCoinChallenge.Tests
{
    /// <summary>
    ///   The set of extension methods for the <see cref="IReadOnlyList{T}" /> class.
    /// </summary>
    ///
    internal static class IReadOnlyListExtensions
    {
        /// <summary>
        ///   Determines whether two sets of <see cref="CoinUse" /> instances are equivalent.
        /// </summary>
        ///
        /// <param name="instance">The instance that this method was invoked on.</param>
        /// <param name="compareTo">The set to compare the instance against..</param>
        ///
        /// <returns><c>true</c> if the sets are equivalent; otherwise, <c>false</c>.</returns>
        ///
        public static bool IsEquivalentTo(this IReadOnlyList<CoinUse> instance,
                                          IReadOnlyList<CoinUse> compareTo)
        {
            if (object.ReferenceEquals(instance, compareTo))
            {
                return true;
            }

            if ((object.ReferenceEquals(instance, null)) || (object.ReferenceEquals(compareTo, null)))
            {
                return false;
            }

            if (instance.Count != compareTo.Count)
            {
                return false;
            }

            instance = instance.OrderBy(coinUse => coinUse.Denomination).ToArray();
            compareTo = compareTo.OrderBy(coinUse => coinUse.Denomination).ToArray();

            for (var index = 0; index < instance.Count; ++index)
            {
                if (!StructuralComparisons.StructuralEqualityComparer.Equals(instance[index], compareTo[index]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
