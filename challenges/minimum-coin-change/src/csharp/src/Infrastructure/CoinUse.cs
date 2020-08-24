namespace Squire.MinimumCoinChallenge
{
    /// <summary>
    ///   Represents the usage of a single coin as part of a solution.
    /// </summary>
    ///
    internal struct CoinUse
    {
        /// <summary>The denomination of the coin that was used.</summary>
        public readonly int Denomination;

        /// <summary>The count of coins used.</summary>
        public readonly int Count;

        /// <summary>
        ///   Initializes a new <see cref="CoinUse" /> instance.
        /// </summary>
        ///
        /// <param name="denomination">The denomination of the coin that was used.</param>
        /// <param name="count">The count of coins used.</param>
        ///
        public CoinUse(int denomination, int count) => (Denomination, Count) = (denomination, count);
    }
}
