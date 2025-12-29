using NUnit.Framework;

namespace Squire.LongestPalindromeChallenge.Tests;

/// <summary>
///   The suite of tests for the <see cref="EntryPoint" /> class.
/// </summary>
///
[TestFixture]
public class EntryPointTests
{
    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.Execute" /> method.
    /// </summary>
    ///
    [Test]
    public void ExecuteValidatesNullValue()
    {
        Assert.That(() => EntryPoint.Execute(null, Strategy.BruteForce), Throws.InstanceOf<ArgumentNullException>(), "A null value should not be allowed.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.Execute" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("")]
    [TestCase(" ")]
    [TestCase("   ")]
    [TestCase("\t")]
    [TestCase("\n")]
    public void ExecuteValidatesEmptyOrWhitespaceValue(string value)
    {
        Assert.That(() => EntryPoint.Execute(value, Strategy.BruteForce), Throws.InstanceOf<ArgumentException>(), "An empty or whitespace value should be rejected.");
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

                Assert.That(implementedStrategy, Is.Not.EqualTo(default(IStrategy)), $"The strategy `{ strategyName }` was not populated.");
                Assert.That(implementedStrategy.Strategy, Is.EqualTo(expectedStrategy), $"The strategy implementation for `{ strategyName }` does not support the correct strategy.");
            }
            catch (NotImplementedException)
            {
                // Take no action; this strategy was not implemented.
            }
        }
    }
}
