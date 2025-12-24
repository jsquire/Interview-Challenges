using System;
using System.CommandLine;
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
        //// <param name="args">The command-line arguments.</param>
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
        public static void Main(string[] args)
        {
            var rootCommand = new RootCommand("Demonstrates different approaches to solving the Minimum Coin Change challenge");

            var valueOption = new Option<int>("--value")
            {
                Description = "The desired monetary value to calculate change for, specified as a whole number",
                Required = true
            };

            var denominationsOption = new Option<int[]>("--denominations")
            {
                Description = "A space-delimited list of coin denominations available to make change with, each specified as a whole number",
                Required = true,
                AllowMultipleArgumentsPerToken = true
            };

            var strategyOption = new Option<Strategy>("--strategy")
            {
                Description = "The strategy to use for calculating change",
                Required = true,
                DefaultValueFactory = _ => Strategy.Dynamic
            };

            rootCommand.Options.Add(valueOption);
            rootCommand.Options.Add(denominationsOption);
            rootCommand.Options.Add(strategyOption);

            rootCommand.SetAction(parseResult =>
            {
                var value = parseResult.GetValue(valueOption);
                var denominations = parseResult.GetValue(denominationsOption);
                var strategy = parseResult.GetValue(strategyOption);

                Execute(value, denominations!, strategy);
            });

            rootCommand
                .Parse(args)
                .Invoke();
        }

        /// <summary>
        ///   Executes the minimum coin change challenge with the specified parameters.
        /// </summary>
        ///
        /// <param name="value">The desired monetary value to calculate change for.</param>
        /// <param name="denominations">The coin denominations available to make change with.</param>
        /// <param name="strategy">The <see cref="Strategy"/> to use for calculating change.</param>
        ///
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="denominations"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="denominations"/> is empty or contains negative values.</exception>"
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is negative.</exception>
        ///
        internal static void Execute(int value,
                                     int[] denominations,
                                     Strategy strategy)
        {
            ArgumentNullException.ThrowIfNull(denominations);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(value));

            if (denominations.Length == 0)
            {
                throw new ArgumentException("At least one coin denomination must be specified.", nameof(denominations));
            }

            if (denominations.Any(denomination => denomination <= 0))
            {
                throw new ArgumentException("All denominations must be greater than or equal to zero.", nameof(denominations));
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

                foreach (var coinUse in answer.OrderBy(coinUse => coinUse.Denomination))
                {
                    Console.WriteLine($"\t Coin: { coinUse.Denomination } x { coinUse.Count }");
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
