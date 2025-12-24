using System;
using NUnit.Framework;

namespace Squire.MinimumCoinChallenge.Tests
{
    /// <summary>
    ///   The suite of tests for the <see cref="EntryPoint" /> class.
    /// </summary>
    ///
    [TestFixture]
    public class EntryPointTests
    {
        /// <summary>
        ///   Verifies functionality of the <see cref="EntryPoint.Main" /> method.
        /// </summary>
        ///
        [Test]
        public void MainValidatesTheDenominations()
        {
            Assert.That(() => EntryPoint.Execute(0, null!, Strategy.Dynamic), Throws.InstanceOf<ArgumentNullException>(), "The set of denominations should not allow null.");
        }

        /// <summary>
        ///   Verifies functionality of the <see cref="EntryPoint.Main" /> method.
        /// </summary>
        ///
        [Test]
        [TestCase(-1)]
        [TestCase(-10)]
        [TestCase(-32767)]
        public void MainValidatesTheValue(int desiredValue)
        {
            Assert.That(() => EntryPoint.Execute(desiredValue, [ 1, 2, 3 ], Strategy.Dynamic), Throws.InstanceOf<ArgumentOutOfRangeException>(), "The value should be at least zero.");
        }

        /// <summary>
        ///   Verifies functionality of the <see cref="EntryPoint.Main" /> method.
        /// </summary>
        ///
        [Test]
        [TestCase(-1)]
        [TestCase(-5)]
        [TestCase(-100)]
        public void MainValidatesNegativeDenominations(int negativeDenomination)
        {
            Assert.That(() => EntryPoint.Execute(10, [ negativeDenomination ], Strategy.Dynamic), Throws.InstanceOf<ArgumentException>(), "Negative denominations should be rejected.");
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
}
