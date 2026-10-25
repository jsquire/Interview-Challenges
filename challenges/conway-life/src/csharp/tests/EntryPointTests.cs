using NUnit.Framework;
using Squire.ConwayLifeChallenge.Infrastructure;

namespace Squire.ConwayLifeChallenge.Tests;

/// <summary>
///   The suite of tests for the <see cref="EntryPoint" /> class.
/// </summary>
///
/// <remarks>
///   Because the <see cref="EntryPoint" /> interacts with the <see cref="Console" />
///   and must mock the static instance, these tests cannot be run in parallel.
/// </remarks>
///
[TestFixture]
[NonParallelizable]
public class EntryPointTests
{
    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.TryParseCoordinate" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("1 2", 1, 2)]
    [TestCase("-100 20", -100, 20)]
    [TestCase("0 0", 0, 0)]
    [TestCase("1  2", 1, 2)]
    [TestCase("9223372036854775807 -9223372036854775808", long.MaxValue, long.MinValue)]
    public void TryParseCoordinateAcceptsValidCoordinates(string value,
                                                          long expectedX,
                                                          long expectedY)
    {
        var result = EntryPoint.TryParseCoordinate(value.AsSpan(), out var coordinate);

        Assert.That(result, Is.True, $"The coordinate pair `{ value }` should be valid.");
        Assert.That(coordinate, Is.EqualTo(new Coordinate(expectedX, expectedY)), "The parsed coordinate was incorrect.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.TryParseCoordinate" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("")]
    [TestCase("1")]
    [TestCase(" 1 2")]
    [TestCase("1 ")]
    [TestCase("1 2 3")]
    [TestCase("a b")]
    [TestCase("1.5 2")]
    public void TryParseCoordinateRejectsInvalidCoordinates(string value)
    {
        var result = EntryPoint.TryParseCoordinate(value.AsSpan(), out _);
        Assert.That(result, Is.False, $"The coordinate pair `{ value }` should be invalid.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ParseFileContents" /> method.
    /// </summary>
    ///
    [Test]
    public void ParseFileContentsReturnsCoordinates()
    {
        var expected = new Coordinate[]
        {
            new(1, 2),
            new(3, 4)
        };

        var input = expected.Select(item => $"{item.X} {item.Y}");

        var result = EntryPoint.ParseFileContents(input);
        Assert.That(result, Is.EquivalentTo(expected), "All coordinates in the file should be returned.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ParseFileContents" /> method.
    /// </summary>
    ///
    [Test]
    public void ParseFileContentsIgnoresComments()
    {
        var result = EntryPoint.ParseFileContents([ "#Life 1.06", "# A comment", "1 2" ]);
        Assert.That(result, Is.EquivalentTo([ new Coordinate(1, 2) ]), "Comments should not produce coordinates.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ParseFileContents" /> method.
    /// </summary>
    ///
    [Test]
    public void ParseFileContentsIgnoresBlankLines()
    {
        var expected = new Coordinate[]
        {
            new(1, 2),
            new(3, 4)
        };

        var result = EntryPoint.ParseFileContents([ "", "1 2", "   ", "3 4" ]);
        Assert.That(result, Is.EquivalentTo(expected), "Blank lines should not produce coordinates.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ParseFileContents" /> method.
    /// </summary>
    ///
    [Test]
    public void ParseFileContentsCollapsesDuplicateCoordinates()
    {
        var result = EntryPoint.ParseFileContents([ "1 2", "1 2" ]);
        Assert.That(result, Is.EquivalentTo([ new Coordinate(1, 2) ]), "Duplicate coordinates should be collapsed.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ParseFileContents" /> method.
    /// </summary>
    ///
    [Test]
    public void ParseFileContentsReturnsEmptyPopulationForEmptyInput()
    {
        var result = EntryPoint.ParseFileContents([]);
        Assert.That(result, Is.Empty, "An empty file should produce an empty population.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ParseFileContents" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("invalid")]
    [TestCase("1 2 3")]
    [TestCase("9223372036854775808 0")]
    public void ParseFileContentsRejectsNonParsableLines(string line)
    {
        Assert.That(() => EntryPoint.ParseFileContents([ "#Life 1.06", "", "  ", "1 2", line, "3 4" ]),
            Throws.TypeOf<FormatException>().With.Message.EqualTo($"Invalid coordinate on line 5: { line }"),
            "Invalid input should fail with the physical line number, including comments and blank lines.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ReadFromConsole" /> method.
    /// </summary>
    ///
    [Test]
    public void ReadFromConsoleReturnsCoordinatesEnteredBeforeBlankLine()
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            var input = new StringReader($"1 2{Environment.NewLine}3 4{Environment.NewLine}{Environment.NewLine}5 6");
            var output = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);

            var result = EntryPoint.ReadFromConsole();
            Assert.That(result, Is.EquivalentTo([ new Coordinate(1, 2), new Coordinate(3, 4) ]), "Input after the blank line should not be read.");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ReadFromConsole" /> method.
    /// </summary>
    ///
    [Test]
    public void ReadFromConsoleIgnoresComments()
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            var input = new StringReader($"#Life 1.06{Environment.NewLine}1 2{Environment.NewLine}# A comment{Environment.NewLine}3 4{Environment.NewLine}{Environment.NewLine}");
            var output = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);

            var result = EntryPoint.ReadFromConsole();
            Assert.That(result, Is.EquivalentTo([ new Coordinate(1, 2), new Coordinate(3, 4) ]), "Comments should not produce coordinates.");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ReadFromConsole" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("invalid")]
    [TestCase("1 2 3")]
    [TestCase("9223372036854775808 0")]
    public void ReadFromConsoleRejectsInvalidCoordinates(string line)
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            using var input = new StringReader($"1 2{Environment.NewLine}{ line }{Environment.NewLine}3 4{Environment.NewLine}{Environment.NewLine}");
            using var output = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);

            Assert.That(() => EntryPoint.ReadFromConsole(),
                Throws.TypeOf<FormatException>().With.Message.EqualTo($"Invalid coordinate: { line }"),
                "Invalid input should fail rather than return a partial population.");
            Assert.That(input.ReadLine(), Is.EqualTo("3 4"), "Input should stop at the first invalid coordinate.");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.Main" /> method.
    /// </summary>
    ///
    [TestCase("-1")]
    [TestCase("-2147483648")]
    [TestCase("31")]
    [TestCase("2147483647")]
    public void MainRejectsOutOfRangeGenerationsBeforeReadingInput(string generations)
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;
        var originalError = Console.Error;

        try
        {
            using var input = new StringReader("1 2");
            using var output = new StringWriter();
            using var error = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);
            Console.SetError(error);

            EntryPoint.Main([ "--generations", generations ]);

            Assert.That(error.ToString(), Does.Contain("The number of generations must be between 0 and 30, inclusive."), "Out-of-range generation counts should report the supported range.");
            Assert.That(output.ToString(), Does.Not.Contain("Enter live cell coordinates"), "Invalid options should not start interactive input.");
            Assert.That(input.ReadLine(), Is.EqualTo("1 2"), "Invalid options should not consume input.");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.Execute" /> method.
    /// </summary>
    ///
    [TestCase(-1)]
    [TestCase(int.MinValue)]
    [TestCase(31)]
    [TestCase(int.MaxValue)]
    public void ExecuteRejectsOutOfRangeGenerationsBeforeReadingInput(int generations)
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            using var input = new StringReader("1 2");
            using var output = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);

            Assert.That(() => EntryPoint.Execute(null, generations, Strategy.ActiveRegion),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("generations"),
                "Direct execution should reject generation counts outside the supported range.");
            Assert.That(output.ToString(), Is.Empty, "Validation should precede prompting.");
            Assert.That(input.ReadLine(), Is.EqualTo("1 2"), "Validation should precede reading input.");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.Main" /> method.
    /// </summary>
    ///
    [TestCase("0")]
    [TestCase("1")]
    [TestCase("29")]
    [TestCase("30")]
    [TestCase(null)]
    public void MainAcceptsSupportedGenerationsAndReadsInput(string? generations)
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;
        var originalError = Console.Error;

        try
        {
            using var input = new StringReader("invalid");
            using var output = new StringWriter();
            using var error = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);
            Console.SetError(error);

            EntryPoint.Main(generations is null ? [] : [ "--generations", generations ]);

            Assert.That(output.ToString(), Does.Contain("Enter live cell coordinates"), "Valid generation counts should reach input processing.");
            Assert.That(error.ToString(), Does.Contain("Invalid coordinate: invalid"), "Malformed input should fail after accepting the generation count.");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.Main" /> method.
    /// </summary>
    ///
    [Test]
    public void MainAcceptsActiveRegionStrategyAndReadsInput()
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;
        var originalError = Console.Error;

        try
        {
            using var input = new StringReader("invalid");
            using var output = new StringWriter();
            using var error = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);
            Console.SetError(error);

            EntryPoint.Main([ "--strategy", "ActiveRegion" ]);

            Assert.That(output.ToString(), Does.Contain("Enter live cell coordinates"), "The active region strategy option should reach input processing.");
            Assert.That(error.ToString(), Does.Contain("Invalid coordinate: invalid"), "Malformed input should fail after accepting the strategy.");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ReadFromConsole" /> method.
    /// </summary>
    ///
    [Test]
    public void ReadFromConsoleReturnsEmptyPopulationForBlankInput()
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            var input = new StringReader(Environment.NewLine);
            var output = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);

            var result = EntryPoint.ReadFromConsole();
            Assert.That(result, Is.Empty, "A blank first line should produce an empty population.");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ReadFromConsole" /> method.
    /// </summary>
    ///
    [Test]
    public void ReadFromConsoleCollapsesDuplicateCoordinates()
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            var input = new StringReader($"1 2{Environment.NewLine}1 2{Environment.NewLine}{Environment.NewLine}");
            var output = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);

            var result = EntryPoint.ReadFromConsole();
            Assert.That(result, Is.EquivalentTo([ new Coordinate(1, 2) ]), "Duplicate coordinates should be collapsed.");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.WriteToConsole" /> method.
    /// </summary>
    ///
    [Test]
    public void WriteToConsoleWritesLifeFileContents()
    {
        var originalOut = Console.Out;

        try
        {
            var output = new StringWriter();
            Console.SetOut(output);

            EntryPoint.WriteToConsole([ new Coordinate(1, 2), new Coordinate(3, 4) ]);

            var expected = $"#Life 1.06{Environment.NewLine}1 2{Environment.NewLine}3 4{Environment.NewLine}";
            Assert.That(output.ToString(), Is.EqualTo(expected), "The population should be written in Life 1.06 format.");
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.WriteToConsole" /> method.
    /// </summary>
    ///
    [Test]
    public void WriteToConsoleWritesHeaderForEmptyPopulation()
    {
        var originalOut = Console.Out;

        try
        {
            var output = new StringWriter();
            Console.SetOut(output);

            EntryPoint.WriteToConsole([]);
            Assert.That(output.ToString(), Is.EqualTo($"#Life 1.06{Environment.NewLine}"), "An empty population should write only the header.");
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.WriteToConsole" /> method.
    /// </summary>
    ///
    [Test]
    public void WriteToConsoleWritesNegativeCoordinates()
    {
        var originalOut = Console.Out;

        try
        {
            var output = new StringWriter();
            Console.SetOut(output);

            EntryPoint.WriteToConsole([ new Coordinate(-5, 10) ]);

            var expected = $"#Life 1.06{Environment.NewLine}-5 10{Environment.NewLine}";
            Assert.That(output.ToString(), Is.EqualTo(expected), "Negative coordinates should be written without modification.");
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ParseFileContents" /> method.
    /// </summary>
    ///
    [TestCase(9_999)]
    [TestCase(10_000)]
    public void ParseFileContentsAcceptsPopulationWithinLimit(int count)
    {
        var expected = Enumerable.Range(0, count).Select(index => new Coordinate(index, -index)).ToArray();
        var lines = expected.Select(cell => $"{ cell.X } { cell.Y }")
            .Concat([ "0 0", "# A comment after the population", "", "  " ]);

        var result = EntryPoint.ParseFileContents(lines);

        Assert.That(result.SetEquals(expected), Is.True, "All distinct coordinates should be returned without duplicates, comments, or blank lines consuming additional population capacity.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ParseFileContents" /> method.
    /// </summary>
    ///
    [Test]
    public void ParseFileContentsRejectsPopulationAboveLimitWithoutReadingFurther()
    {
        var linesRead = 0;
        var lines = Enumerable.Range(0, 10_002).Select(index =>
        {
            linesRead++;
            return $"{ index } 0";
        });

        Assert.That(() => EntryPoint.ParseFileContents(lines),
            Throws.TypeOf<FormatException>().With.Message.EqualTo("The initial population cannot exceed 10000 distinct live cells."),
            "The first distinct cell beyond the limit should invalidate the input.");
        Assert.That(linesRead, Is.EqualTo(10_001), "Input should stop immediately when the population limit is exceeded.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ParseFileContents" /> method.
    /// </summary>
    ///
    [Test]
    public void ParseFileContentsStillRejectsMalformedLinesAtPopulationLimit()
    {
        var lines = Enumerable.Range(0, 10_000).Select(index => $"{ index } 0").Append("invalid");

        Assert.That(() => EntryPoint.ParseFileContents(lines),
            Throws.TypeOf<FormatException>().With.Message.EqualTo("Invalid coordinate on line 10001: invalid"),
            "Reaching the population limit must not bypass validation of subsequent lines.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ReadFromConsole" /> method.
    /// </summary>
    ///
    [TestCase(9_999)]
    [TestCase(10_000)]
    public void ReadFromConsoleAcceptsPopulationWithinLimit(int count)
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            var expected = Enumerable.Range(0, count).Select(index => new Coordinate(index, -index)).ToArray();
            var lines = expected.Select(cell => $"{ cell.X } { cell.Y }")
                .Concat([ "0 0", "# A comment after the population", "", "unread" ]);
            using var input = new StringReader(string.Join(Environment.NewLine, lines));
            using var output = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);

            var result = EntryPoint.ReadFromConsole();

            Assert.That(result.SetEquals(expected), Is.True, "All distinct coordinates should be returned without duplicates or comments consuming additional population capacity.");
            Assert.That(input.ReadLine(), Is.EqualTo("unread"), "The blank line should still terminate input at the population limit.");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ReadFromConsole" /> method.
    /// </summary>
    ///
    [Test]
    public void ReadFromConsoleRejectsPopulationAboveLimitWithoutReadingFurther()
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            var lines = Enumerable.Range(0, 10_001).Select(index => $"{ index } 0").Append("unread");
            using var input = new StringReader(string.Join(Environment.NewLine, lines));
            using var output = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);

            Assert.That(() => EntryPoint.ReadFromConsole(),
                Throws.TypeOf<FormatException>().With.Message.EqualTo("The initial population cannot exceed 10000 distinct live cells."),
                "The first distinct cell beyond the limit should invalidate the input.");
            Assert.That(input.ReadLine(), Is.EqualTo("unread"), "Input should stop immediately when the population limit is exceeded.");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ReadFromConsole" /> method.
    /// </summary>
    ///
    [Test]
    public void ReadFromConsoleStillRejectsMalformedLinesAtPopulationLimit()
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            var lines = Enumerable.Range(0, 10_000).Select(index => $"{ index } 0").Append("invalid");
            using var input = new StringReader(string.Join(Environment.NewLine, lines));
            using var output = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);

            Assert.That(() => EntryPoint.ReadFromConsole(),
                Throws.TypeOf<FormatException>().With.Message.EqualTo("Invalid coordinate: invalid"),
                "Reaching the population limit must not bypass validation of subsequent lines.");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }
}
