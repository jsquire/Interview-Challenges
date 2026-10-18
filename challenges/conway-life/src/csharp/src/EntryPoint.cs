using System.CommandLine;
using Squire.ConwayLifeChallenge.Infrastructure;

namespace Squire.ConwayLifeChallenge;

/// <summary>
///   Hosts the entry point for the application along with any supporting infrastructure.
/// </summary>
///
public static class EntryPoint
{
    /// <summary>The maximum number of distinct live cells allowed in the initial population.</summary>
    private const int MaximumInitialPopulation = 10_000;

    /// <summary>The maximum number of generations allowed in a simulation.</summary>
    private const int MaximumGenerations = 30;

    /// <summary>
    ///   Simulates Conway's Game of Life in 64-bit signed integer space.
    /// </summary>
    ///
    /// <param name="args">The command-line arguments.</param>
    ///
    /// <example>
    ///   <c>ConwayLifeChallenge</c>
    /// </example>
    ///
    /// <example>
    ///   <c>ConwayLifeChallenge --file glider.lif --generations 10 --strategy Naive</c>
    /// </example>
    ///
    /// <example>
    ///   <c>ConwayLifeChallenge --generations 5</c>
    /// </example>
    ///
    /// <seealso href="https://github.com/jsquire/Interview-Challenges/blob/main/challenges/conway-life/README.md" />
    ///
    public static void Main(string[] args)
    {
        var rootCommand = new RootCommand("Conway's Game of Life in 64-bit signed integer space.");

        var fileOption = new Option<string?>("--file")
        {
            Description = $"Path to a Life 1.06 file containing at most { MaximumInitialPopulation } distinct live cells."
        };

        var generationsOption = new Option<int>("--generations")
        {
            Description = $"Number of generations to simulate (0 through { MaximumGenerations }).",
            DefaultValueFactory = _ => 10,
            Validators =
            {
                result =>
                {
                    var generations = result.GetValueOrDefault<int>();

                    if ((generations < 0) || (generations > MaximumGenerations))
                    {
                        result.AddError($"The number of generations must be between 0 and { MaximumGenerations }, inclusive.");
                    }
                }
            }
        };

        var strategyOption = new Option<Strategy>("--strategy")
        {
            Description = "The strategy to use for simulating the generations.",
            Required = true,
            DefaultValueFactory = _ => Strategy.Naive
        };

        rootCommand.Options.Add(fileOption);
        rootCommand.Options.Add(generationsOption);
        rootCommand.Options.Add(strategyOption);

        rootCommand.SetAction(parseResult =>
        {
            var file = parseResult.GetValue(fileOption);
            var generations = parseResult.GetValue(generationsOption);
            var strategy = parseResult.GetValue(strategyOption);

            Execute(file, generations, strategy);
        });

        rootCommand
            .Parse(args)
            .Invoke();
    }

    /// <summary>
    ///   Reads the initial population, runs the simulation, and writes the result.
    /// </summary>
    ///
    /// <param name="file">Optional path to a Life 1.06 file.</param>
    /// <param name="generations">Number of generations to simulate.</param>
    /// <param name="strategy">The <see cref="Strategy" /> to use for simulating the generations.</param>
    ///
    /// <exception cref="ArgumentOutOfRangeException">The number of generations is outside the supported range of 0 through 30.</exception>
    ///
    internal static void Execute(string? file,
                                 int generations,
                                 Strategy strategy)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(generations);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(generations, MaximumGenerations);

        if (file is not null)
        {
            if (!Path.Exists(file))
            {
                throw new FileNotFoundException($"The file '{file}' does not exist.");
            }
        }

        var initialPopulation = file is not null
            ? ParseFileContents(File.ReadLines(file))
            : ReadFromConsole();

        var result = StrategyBase.CreateStrategy(strategy).Simulate(initialPopulation, generations);

        WriteToConsole(result);
    }

    /// <summary>
    ///   Reads coordinates from standard input until a blank line is encountered.
    /// </summary>
    ///
    /// <returns>The set of live cell coordinates.</returns>
    ///
    /// <exception cref="FormatException">An input line is not a valid coordinate or comment, or the initial population exceeds 10,000 distinct cells.</exception>
    ///
    internal static HashSet<Coordinate> ReadFromConsole()
    {
        Console.WriteLine("Enter live cell coordinates (x y), one per line.");
        Console.WriteLine($"Enter at most { MaximumInitialPopulation } distinct live cells.");
        Console.WriteLine();
        Console.WriteLine("Example:");
        Console.WriteLine("  1 1");
        Console.WriteLine("  2 1");
        Console.WriteLine("  -3 1");
        Console.WriteLine("  -21003 -1");
        Console.WriteLine();
        Console.WriteLine("Enter a blank line when finished.");
        Console.WriteLine();

        var cells = new HashSet<Coordinate>();

        while (true)
        {
            var line = Console.ReadLine();
            var lineSpan = line.AsSpan();

            // A line containing only whitespace is considered a blank line and terminates input.

            if ((lineSpan.IsEmpty) || (lineSpan.IsWhiteSpace()))
            {
                break;
            }

            // Comments are allowed in Life 1.06 format and should be ignored.

            if (lineSpan.StartsWith('#'))
            {
                continue;
            }

            if (TryParseCoordinate(lineSpan, out var coordinate))
            {
                AddInitialCell(cells, coordinate);
            }
            else
            {
                throw new FormatException($"Invalid coordinate: {line}");
            }
        }

        return cells;
    }

    /// <summary>
    ///   Reads coordinates from a Life 1.06 file.
    /// </summary>
    ///
    /// <param name="fileContents">The contents of the file.</param>
    ///
    /// <returns>The set of live cell coordinates.</returns>
    ///
    /// <exception cref="FormatException">A non-blank input line is not a valid coordinate or comment, or the initial population exceeds 10,000 distinct cells.</exception>
    ///
    internal static HashSet<Coordinate> ParseFileContents(IEnumerable<string> fileContents)
    {
        var cells = new HashSet<Coordinate>();
        var lineNumber = 0;

        foreach (var line in fileContents)
        {
            ++lineNumber;

            var lineSpan = line.AsSpan();

            // Blank lines and comments are allowed in Life 1.06 format and should be ignored.

            if ((lineSpan.IsEmpty)
                || (lineSpan.IsWhiteSpace())
                || (lineSpan.StartsWith('#')))
            {
                continue;
            }

            if (TryParseCoordinate(lineSpan, out var coordinate))
            {
                AddInitialCell(cells, coordinate);
            }
            else
            {
                throw new FormatException($"Invalid coordinate on line { lineNumber }: { line }");
            }
        }

        return cells;
    }

    /// <summary>
    ///   Writes the population to standard output in Life 1.06 format.
    /// </summary>
    ///
    /// <param name="cells">The collection of live cell coordinates.</param>
    ///
    internal static void WriteToConsole(IEnumerable<Coordinate> cells)
    {
        Console.WriteLine("#Life 1.06");

        foreach (var cell in cells)
        {
            Console.WriteLine($"{cell.X} {cell.Y}");
        }
    }

    /// <summary>
    ///   Attempts to parse a coordinate pair from a line of text.
    /// </summary>
    ///
    /// <param name="line">The line to parse.</param>
    /// <param name="coordinate">The parsed coordinate, if successful.</param>
    ///
    /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
    ///
    internal static bool TryParseCoordinate(ReadOnlySpan<char> line,
                                            out Coordinate coordinate)
    {
        coordinate = default;

        var separatorIndex = line.IndexOf(' ');

        // If there is no space after the first character or the only space comes
        // as the last character, then the line is not a valid coordinate pair.

        if ((separatorIndex <= 0) || ((separatorIndex + 1) >= line.Length))
        {
            return false;
        }

        // Slice the span on the embedded string to separate the x and y
        // values.  Since TryParse ignores leading and trailing whitespace, there
        // is no need to trim the slices.

        if ((long.TryParse(line[ ..separatorIndex ], out var x))
            && (long.TryParse(line[ (separatorIndex + 1).. ], out var y)))
        {
            coordinate = new Coordinate(x, y);
            return true;
        }

        return false;
    }

    /// <summary>
    ///   Adds a distinct initial cell while enforcing the population limit shared by both input paths.
    /// </summary>
    ///
    /// <param name="cells">The initial population being collected.</param>
    /// <param name="coordinate">The cell to add.</param>
    ///
    /// <exception cref="FormatException">The initial population exceeds 10,000 distinct cells.</exception>
    ///
    private static void AddInitialCell(HashSet<Coordinate> cells,
                                       Coordinate coordinate)
    {
        if ((cells.Add(coordinate)) && (cells.Count > MaximumInitialPopulation))
        {
            throw new FormatException($"The initial population cannot exceed { MaximumInitialPopulation } distinct live cells.");
        }
    }
}
