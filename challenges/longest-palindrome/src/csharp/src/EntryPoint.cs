using System.CommandLine;
using Squire.LongestPalindromeChallenge.Strategies;

namespace Squire.LongestPalindromeChallenge;

/// <summary>
///   Hosts the entry point for the application along with any supporting infrastructure.
/// </summary>
///
public static class EntryPoint
{
    /// <summary>
    ///   This application demonstrates different approaches to solving the Longest Palindrome Substring challenge.
    /// </summary>
    ///
    //// <param name="args">The command-line arguments.</param>
    ///
    /// <example>
    ///   <c>LongestPalindromeChallenge "abbat badab" --strategy Default</c>
    /// </example>
    ///
    /// <example>
    ///   <c>dotnet run LongestPalindromeChallenge "tacocat"</c>
    /// </example>
    ///
    /// <seealso href="https://github.com/jsquire/Interview-Challenges/blob/main/challenges/longest-palindrome/README.md" />
    ///
    public static void Main(string[] args)
    {
        var rootCommand = new RootCommand("Demonstrates different approaches to solving the Longest Palindrome Substring challenge");

        var valueArgument = new Argument<string>("value")
        {
            Description = "The string of ASCII printable characters to calculate the longest palindrome substring for.",
            Arity = ArgumentArity.ExactlyOne,
            Validators =
            {
                result =>
                {
                    var value = result.GetValueOrDefault<string>();

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        result.AddError("The value cannot be null, empty, or whitespace.");
                        return;
                    }

                    foreach (var character in value.AsSpan())
                    {
                        if ((!char.IsAscii(character)) || (char.IsControl(character)))
                        {
                            result.AddError("The value can only contain ASCII printable characters (code points 32 through 126).");
                            break;
                        }
                    }
                }
            }
        };

        var strategyOption = new Option<Strategy>("--strategy")
        {
            Description = "The strategy to use for calculating change",
            Required = true,
            DefaultValueFactory = _ => Strategy.CandidatePairsOptimized
        };

        rootCommand.Arguments.Add(valueArgument);
        rootCommand.Options.Add(strategyOption);

        rootCommand.SetAction(parseResult =>
        {
            var value = parseResult.GetValue(valueArgument);
            var strategy = parseResult.GetValue(strategyOption);

            Execute(value, strategy);
        });

        rootCommand
            .Parse(args)
            .Invoke();
    }

    /// <summary>
    ///   Executes the Longest Palindrome Substring challenge with the specified parameters.
    /// </summary>
    ///
    /// <param name="value">The <c>string</c> to calculate the longest palindrome substring for.</param>
    /// <param name="strategy">The <see cref="Strategy"/> to use for solving the challenge.</param>
    ///
    /// <remarks>
    ///   As an internal construct, it is expected that the parameters have already been
    ///   validated and, thus, are legal values.
    /// </remarks>
    ///
    internal static void Execute(string? value,
                                 Strategy strategy)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

        Console.WriteLine($"Run starting for the value: \"{ value }\" ...");
        Console.WriteLine($"Applying the { strategy } strategy to solve the challenge.");
        Console.WriteLine();

        // Solve the challenge using the requested strategy and report the answer.  If the
        // answer is null, consider the challenge unsolvable for the parameters specified.

        var answer = CreateStrategy(strategy).Solve(value);

        if (answer is not null)
        {
            Console.WriteLine("Answer:");

            Console.WriteLine($"\tPalindrome:  { answer.Value }");
            Console.WriteLine($"\tStart Index: { answer.StartIndex }");
            Console.WriteLine($"\tLength:      { answer.Length }");
        }
        else
        {
            Console.WriteLine("The challenge is not solvable, as there were no valid palindromes in the provided string.");
        }

        Console.WriteLine();
        Console.WriteLine("Run Complete.");
        Console.WriteLine();
        Console.Write("Press a key...");
        Console.Read();
    }

    /// <summary>
    ///   Creates an <see cref="StrategyBase" /> which applies the given <paramref name="strategy"/>
    ///   to solve the longest palindrome substring problem.
    /// </summary>
    ///
    /// <param name="strategy">The strategy to locate create a solution instance for.</param>
    ///
    /// <returns>The <see cref="StrategyBase" /> for the requested <paramref name="strategy"/>.</returns>
    ///
    internal static StrategyBase CreateStrategy(Strategy strategy) => strategy switch
    {
        Strategy.CandidatePairsOptimized => new CandidatePairsOptimizedStrategy(),
        Strategy.CandidatePairs => new CandidatePairsStrategy(),
        Strategy.ExpandAroundCenter => new ExpandAroundCenterStrategy(),
        _ => throw new ArgumentException($"Unknown strategy: `{ strategy }`.", nameof(strategy))
    };
}
