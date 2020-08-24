using System;
using System.Collections.Generic;
using NUnit.Framework;
using Squire.MinimumCoinChallenge.Strategies;

namespace Squire.MinimumCoinChallenge.Tests
{
    /// <summary>
    ///   The suite of tests for the <see cref="GreedyStrategy" /> class.
    /// </summary>
    ///
    [TestFixture]
    internal class GreedyStrategyTests
    {
        /// <summary>
        ///   The set of test cases for combinations without a solution.
        /// </summary>
        ///
        private static IEnumerable<object[]> UnsolvableCases
        {
            get
            {
                yield return new object[] { 30, new int[] { 50 }};
                yield return new object[] { 30, new int[] { 100, 31, 50 }};
                yield return new object[] {  9, new int[] { 2, 4, 6 }};
            }
        }

        /// <summary>
        ///   The set of test cases for combinations with a valid solution.
        /// </summary>
        ///
        private static IEnumerable<object[]> SolvableCases
        {
            get
            {
                yield return new object[]
                {
                    1,
                    new int[] { 1, 5, 10, 25, 50 },
                    new[] { new CoinUse(1, 1) }
                };

                yield return new object[]
                {
                    5,
                    new int[] { 50, 25, 10, 5, 1 },
                    new[] { new CoinUse(5, 1) }
                };

                yield return new object[]
                {
                    10,
                    new int[] { 1, 10, 5, 25, 50 },
                    new[] { new CoinUse(10, 1) }
                };

                yield return new object[]
                {
                    25,
                    new int[] { 1, 5, 10, 25, 50 },
                    new[] { new CoinUse(25, 1) }
                };

                yield return new object[]
                {
                    6,
                    new int[] { 1, 5, 10, 25, 50 },
                    new[] { new CoinUse(1, 1), new CoinUse(5, 1) }
                };

                yield return new object[]
                {
                    138,
                    new int[] { 25, 5, 10, 50, 1 },
                    new[] { new CoinUse(1, 3), new CoinUse(10, 1), new CoinUse(25, 1), new CoinUse(50, 2) }
                };

                yield return new object[]
                {
                    50,
                    new int[] { 1, 51 },
                    new[] { new CoinUse(1, 50) }
                };
            }
        }

        /// <summary>
        ///   The set of test cases for combinations with a solution that is incorrect
        ///   due to having local maximums.
        /// </summary>
        ///
        private static IEnumerable<object[]> LocalMaximumCases
        {
            get
            {
                yield return new object[]
                {
                    40,
                    new int[] { 1, 20, 10, 5, 25 },
                    new[] { new CoinUse(5, 1), new CoinUse(10, 1), new CoinUse(25, 1) }
                };

                yield return new object[]
                {
                    41,
                    new int[] { 22, 25, 1, 9, 12 },
                    new[] { new CoinUse(1, 4), new CoinUse(12, 1), new CoinUse(25, 1) }
                };
            }
        }

        /// <summary>
        ///   Verifies behavior of the <see cref="GreedyStrategy.Solve" /> method.
        /// </summary>
        ///
        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public void NoValueProducesNoSolution(int value)
        {
            var strategy = new GreedyStrategy();
            var result = strategy.Solve(value, new[] { 1, 5, 10, 25 });

            Assert.That(result, Is.Not.Null, "The result should have been returned.");
            Assert.That(result, Is.Empty, "The result should not contain any denominations.");
        }

        /// <summary>
        ///   Verifies behavior of the <see cref="GreedyStrategy.Solve" /> method.
        /// </summary>
        ///
        [Test]
        public void NoDenominationsProduceNoSolution()
        {
            var strategy = new GreedyStrategy();
            var result = strategy.Solve(1, Array.Empty<int>());

            Assert.That(result, Is.Not.Null, "The result should have been returned.");
            Assert.That(result, Is.Empty, "The result should not contain any denominations.");
        }

        /// <summary>
        ///   Verifies behavior of the <see cref="GreedyStrategy.Solve" /> method.
        /// </summary>
        ///
        [Test]
        [TestCaseSource(nameof(UnsolvableCases))]
        public void UnsolvableCombinationsProduceNoSolution(int value,
                                                            int[] denominations)
        {
            var strategy = new GreedyStrategy();
            var result = strategy.Solve(value, denominations);

            Assert.That(result, Is.Not.Null, "The result should have been returned.");
            Assert.That(result, Is.Empty, "The result should not contain any denominations.");
        }

        /// <summary>
        ///   Verifies behavior of the <see cref="GreedyStrategy.Solve" /> method.
        /// </summary>
        ///
        [Test]
        [TestCaseSource(nameof(SolvableCases))]
        public void SolvableCasesProduceCorrectSolutions(int value,
                                                         int[] denominations,
                                                         IReadOnlyList<CoinUse> expectedResult)
        {
            var strategy = new GreedyStrategy();
            var result = strategy.Solve(value, denominations);

            Assert.That(result, Is.Not.Null, "The result should have been returned.");
            Assert.That(result.IsEquivalentTo(expectedResult), Is.True, "The result should have matched the expectation.");
        }

        /// <summary>
        ///   Verifies behavior of the <see cref="GreedyStrategy.Solve" /> method.
        /// </summary>
        ///
        [Test]
        [TestCaseSource(nameof(LocalMaximumCases))]
        public void LocalMaximumCasesProduceIncorrectSolutions(int value,
                                                               int[] denominations,
                                                               IReadOnlyList<CoinUse> expectedResult)
        {
            var strategy = new GreedyStrategy();
            var result = strategy.Solve(value, denominations);

            Assert.That(result, Is.Not.Null, "The result should have been returned.");
            Assert.That(result.IsEquivalentTo(expectedResult), Is.True, "The result should have matched the expectation.");
        }
    }
}
