using System;
using System.Collections.Generic;
using NUnit.Framework;
using Squire.MinimumCoinChallenge.Strategies;

namespace Squire.MinimumCoinChallenge.Tests
{
    /// <summary>
    ///   The suite of tests for the <see cref="DynamicStrategy" /> class.
    /// </summary>
    ///
    [TestFixture]
    public class DynamicStrategyTests
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
        ///   The set of test cases for combinations without a solution.
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
                    new Dictionary<int, int> {{ 1, 1 }}
                };

                yield return new object[]
                {
                    5,
                    new int[] { 50, 25, 10, 5, 1 },
                    new Dictionary<int, int> {{ 5, 1 }}
                };

                yield return new object[]
                {
                    10,
                    new int[] { 1, 10, 5, 25, 50 },
                    new Dictionary<int, int> {{ 10, 1 }}
                };

                yield return new object[]
                {
                    25,
                    new int[] { 1, 5, 10, 25, 50 },
                    new Dictionary<int, int> {{ 25, 1 }}
                };

                yield return new object[]
                {
                    6,
                    new int[] { 1, 5, 10, 25, 50 },
                    new Dictionary<int, int> {{ 1, 1 }, { 5, 1 }}
                };

                yield return new object[]
                {
                    138,
                    new int[] { 25, 5, 10, 50, 1 },
                    new Dictionary<int, int> {{ 1, 3 }, { 10, 1 }, {25, 1}, {50, 2}}
                };

                yield return new object[]
                {
                    50,
                    new int[] { 1, 51 },
                    new Dictionary<int, int> {{ 1, 50 }}
                };

                yield return new object[]
                {
                    40,
                    new int[] { 1, 20, 10, 5, 25 },
                    new Dictionary<int, int> {{ 20, 2 }}
                };

                yield return new object[]
                {
                    41,
                    new int[] { 22, 25, 1, 9, 12 },
                    new Dictionary<int, int> {{ 1, 1 }, { 9, 2 }, { 22, 1 }}
                };
            }
        }

        /// <summary>
        ///   Verifies behavior of the <see cref="DynamicStrategy.Solve" /> method.
        /// </summary>
        ///
        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public void NoValueProducesNoSolution(int value)
        {
            var strategy = new DynamicStrategy();
            var result = strategy.Solve(value, new[] { 1, 5, 10, 25 });

            Assert.That(result, Is.Not.Null, "The result should have been returned.");
            Assert.That(result, Is.Empty, "The result should not contain any denominations.");
        }

        /// <summary>
        ///   Verifies behavior of the <see cref="DynamicStrategy.Solve" /> method.
        /// </summary>
        ///
        [Test]
        public void NoDenominationsProduceNoSolution()
        {
            var strategy = new DynamicStrategy();
            var result = strategy.Solve(1, Array.Empty<int>());

            Assert.That(result, Is.Not.Null, "The result should have been returned.");
            Assert.That(result, Is.Empty, "The result should not contain any denominations.");
        }

        /// <summary>
        ///   Verifies behavior of the <see cref="DynamicStrategy.Solve" /> method.
        /// </summary>
        ///
        [Test]
        [TestCaseSource(nameof(UnsolvableCases))]
        public void UnsolvableCombinationsProduceNoSolution(int value,
                                                            int[] denominations)
        {
            var strategy = new DynamicStrategy();
            var result = strategy.Solve(value, denominations);

            Assert.That(result, Is.Not.Null, "The result should have been returned.");
            Assert.That(result, Is.Empty, "The result should not contain any denominations.");
        }

        /// <summary>
        ///   Verifies behavior of the <see cref="DynamicStrategy.Solve" /> method.
        /// </summary>
        ///
        [Test]
        [TestCaseSource(nameof(SolvableCases))]
        public void SolvableCasesProduceCorrectSolutions(int value,
                                                         int[] denominations,
                                                         Dictionary<int, int> expectedResult)
        {
            var strategy = new DynamicStrategy();
            var result = strategy.Solve(value, denominations);

            Assert.That(result, Is.Not.Null, "The result should have been returned.");
            Assert.That(result, Is.EquivalentTo(expectedResult), "The result should have matched the expectation.");
        }
    }
}
