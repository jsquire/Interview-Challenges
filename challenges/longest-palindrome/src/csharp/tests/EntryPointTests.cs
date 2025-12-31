using NUnit.Framework;

namespace Squire.LongestPalindromeChallenge.Tests;

/// <summary>
///   The suite of tests for the <see cref="EntryPoint" /> class.
/// </summary>
///
[TestFixture]
[NonParallelizable]
public class EntryPointTests
{
    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.Main" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("")]
    [TestCase(" ")]
    [TestCase("   ")]
    [TestCase("\t")]
    [TestCase("\n")]
    public void MainRejectsEmptyOrWhitespaceValue(string value)
    {
        var originalOut = Console.Out;
        var originalError = Console.Error;

        try
        {
            var output = new StringWriter();
            var error = new StringWriter();

            Console.SetOut(output);
            Console.SetError(error);

            EntryPoint.Main([value]);
            Assert.That(error.ToString(), Does.Contain("cannot be null, empty, or whitespace"), "An empty or whitespace value should be rejected.");
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.Main" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("a\x00b")]
    [TestCase("a\x1Fb")]
    [TestCase("a\x7Fb")]
    public void MainRejectsControlCharacters(string value)
    {
        var originalOut = Console.Out;
        var originalError = Console.Error;

        try
        {
            var output = new StringWriter();
            var error = new StringWriter();

            Console.SetOut(output);
            Console.SetError(error);

            EntryPoint.Main([value]);
            Assert.That(error.ToString(), Does.Contain("ASCII printable characters"), "Control characters should be rejected.");
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.Main" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("café")]
    [TestCase("naïve")]
    [TestCase("日本語")]
    public void MainRejectsNonAsciiCharacters(string value)
    {
        var originalOut = Console.Out;
        var originalError = Console.Error;

        try
        {
            var output = new StringWriter();
            var error = new StringWriter();

            Console.SetOut(output);
            Console.SetError(error);

            EntryPoint.Main([value]);
            Assert.That(error.ToString(), Does.Contain("ASCII printable characters"), "Non-ASCII characters should be rejected.");
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.Main" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase(" ~ ")]
    [TestCase("~a~")]
    [TestCase(" ")]
    public void MainAcceptsBoundaryAsciiCharacters(string value)
    {
        var originalOut = Console.Out;
        var originalError = Console.Error;

        try
        {
            var output = new StringWriter();
            var error = new StringWriter();

            Console.SetOut(output);
            Console.SetError(error);

            EntryPoint.Main([value]);
            Assert.That(error.ToString(), Does.Not.Contain("ASCII printable characters"), "Boundary ASCII characters should be accepted.");
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.CreateStrategy" /> method.
    /// </summary>
    ///
    [Test]
    public void StrategiesAreRecognized()
    {
        foreach (var strategy in Enum.GetNames(typeof(Strategy)))
        {
            Exception? capturedException;

            try
            {
                EntryPoint.CreateStrategy(Enum.Parse<Strategy>(strategy));
                capturedException = null;
            }
            catch (Exception ex)
            {
                capturedException = ex;
            }

            Assert.That(capturedException, Is.Null.Or.TypeOf<NotImplementedException>(), $"The strategy `{ strategy }` was not recognized.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.CreateStrategy" /> method.
    /// </summary>
    ///
    [Test]
    public void InvalidStrategiesAreRejected()
    {
        var invalid = (Strategy)Int32.MinValue;
        Assert.That(() => EntryPoint.CreateStrategy(invalid), Throws.InstanceOf<ArgumentException>(), "An invalid strategy should be rejected.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.CreateStrategy" /> method.
    /// </summary>
    ///
    [Test]
    public void ImplementedStrategiesArePopulated()
    {
        foreach (var strategyName in Enum.GetNames(typeof(Strategy)))
        {
            try
            {
                var expectedStrategy = Enum.Parse<Strategy>(strategyName);
                var implementedStrategy = EntryPoint.CreateStrategy(expectedStrategy);

                Assert.That(implementedStrategy, Is.Not.EqualTo(default(StrategyBase)), $"The strategy `{ strategyName }` was not populated.");
                Assert.That(implementedStrategy.Strategy, Is.EqualTo(expectedStrategy), $"The strategy implementation for `{ strategyName }` does not support the correct strategy.");
            }
            catch (NotImplementedException)
            {
                // Take no action; this strategy was not implemented.
            }
        }
    }
}
