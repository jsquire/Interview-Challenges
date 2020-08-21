using System;
using System.Linq;
using Squire.MinimumCoinChallenge.Strategies;

namespace Squire.MinimumCoinChallenge
{
    /// <summary>
    ///   Hosts the entry point for the application along with any supporting infrastructure.
    /// </summary>
    ///
    public static class EntryPoint
    {
        /// <summary>
        ///   This application demonstrates different approaches to solving the Minimum Coin Change challenge.        ///
        /// </summary>
        ///
        /// <param name="value">The desired monetary value to calculate change for, specified as a whole number. (example: --value 50)</param>
        /// <param name="denominations">A space-delimited list of coin denominations available to make change with, each specified as a whole number. (example: --denominations 1 5 10 25)</param>
        /// <param name="strategy">The <see cref="Strategy"/> to use for calculating change. (example: --strategy Dynamic)</param>
        ///
        /// <example>
        ///   <c>MinimumCoinChallenge --value 50 --denominations 1 5 10 25 --strategy Greedy</c>
        /// </example>
        ///
        /// <example>
        ///   <c>dotnet run MinimumCoinChallenge --value 50 --denominations 1 5 10 25</c>
        /// </example>
        ///
        /// <seealso href="https://github.com/jsquire/Interview-Challenges/blob/master/challenges/minimum-coin-change/ReadMe.md" />
        ///
        public static void Main(int value,
                                int[] denominations,
                                Strategy strategy = Strategy.Dynamic)
        {
            if (denominations == null)
            {
                throw new ArgumentNullException(nameof(denominations));
            }

            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "The value cannot be negative.");
            }

            Console.WriteLine($"Run starting for the value: { value } using coins [ { String.Join(", ", denominations) } ]...");
            Console.WriteLine($"Applying the { strategy } strategy to solve the challenge.");
            Console.WriteLine();

            // Solve the challenge using the requested strategy and report the answer.  If the
            // answer set is empty, consider the challenge unsolvable for the parameters specified.

            var answer = CreateStrategy(strategy).Solve(value, denominations);

            if (answer.Count > 0)
            {
                Console.WriteLine("Answer:");

                foreach (var denomination in answer.Keys.OrderBy(item => item))
                {
                    Console.WriteLine($"\t Coin: { denomination } x { answer[denomination] }");
                }
            }
            else
            {
                Console.WriteLine("The challenge is not solvable for the desired value with the available denominations.");
            }

            Console.WriteLine();
            Console.WriteLine("Run Complete.");
            Console.WriteLine();
            Console.Write("Press a key...");
            Console.Read();
        }

        /// <summary>
        ///   Creates an <see cref="IStrategy" /> which applies the given <paramref name="strategy"/>
        ///   to solve the coin change problem.
        /// </summary>
        ///
        /// <param name="strategy">The strategy to locate create a solution instance for.</param>
        ///
        /// <returns>The <see cref="IStrategy" /> for the requested <paramref name="strategy"/>.</returns>
        ///
        internal static IStrategy CreateStrategy(Strategy strategy) => strategy switch
        {
            Strategy.Dynamic => new DynamicStrategy(),
            Strategy.Greedy => new GreedyStrategy(),
            _ => throw new ArgumentException($"Unknown strategy: `{ strategy }`.", nameof(strategy))
        };

    }
}
