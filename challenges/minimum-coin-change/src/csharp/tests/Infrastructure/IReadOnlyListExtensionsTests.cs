using System.Collections.Generic;
using NUnit.Framework;

namespace Squire.MinimumCoinChallenge.Tests
{
    /// <summary>
    ///   The suite of tests for the <see cref="IReadOnlyListExtensions" /> class.
    /// </summary>
    ///
    [TestFixture]
    public class IReadOnlyListExtensionsTests
    {
        /// <summary>
        ///   Validates behavior of the <see cref="IReadOnlyListExtensions.IsEquivalentTo(IReadOnlyList{CoinUse}, IReadOnlyList{CoinUse})" />
        ///   method.
        /// </summary>
        ///
        [Test]
        public void IsEquivalentToWithNullInstances()
        {
            Assert.That(((IReadOnlyList<CoinUse>)null!).IsEquivalentTo(null!), Is.True);
        }

        /// <summary>
        ///   Validates behavior of the <see cref="IReadOnlyListExtensions.IsEquivalentTo(IReadOnlyList{CoinUse}, IReadOnlyList{CoinUse})" />
        ///   method.
        /// </summary>
        ///
        [Test]
        public void IsEquivalentToWithOneNullInstance()
        {
            var nullSet = (IReadOnlyList<CoinUse>?)null;
            var emptySet = new List<CoinUse>();

            Assert.That(nullSet!.IsEquivalentTo(emptySet), Is.False, "A null instance should not be equivalent.");
            Assert.That(emptySet.IsEquivalentTo(nullSet!), Is.False, "A null operand should not be equivalent.");
        }

        /// <summary>
        ///   Validates behavior of the <see cref="IReadOnlyListExtensions.IsEquivalentTo(IReadOnlyList{CoinUse}, IReadOnlyList{CoinUse})" />
        ///   method.
        /// </summary>
        ///
        [Test]
        public void IsEquivalentWithSetsOfDifferenceSizes()
        {
            var first = new[] { new CoinUse(5, 10) };
            var second = new[] { new CoinUse(5, 10), new CoinUse(1, 1) };

            Assert.That(first.IsEquivalentTo(second), Is.False, "The instances should not be equivalent when the smaller set is the instance.");
            Assert.That(second.IsEquivalentTo(first), Is.False, "The instances should not be equivalent when the larger set is the instance.");
        }

        /// <summary>
        ///   Validates behavior of the <see cref="IReadOnlyListExtensions.IsEquivalentTo(IReadOnlyList{CoinUse}, IReadOnlyList{CoinUse})" />
        ///   method.
        /// </summary>
        ///
        [Test]
        public void IsEquivalentWithSetsOfDifferenceValues()
        {
            var first = new[] { new CoinUse(5, 10) };
            var second = new[] { new CoinUse(1, 1) };

            Assert.That(first.IsEquivalentTo(second), Is.False, "The instances should not be equivalent when the first set is the instance.");
            Assert.That(second.IsEquivalentTo(first), Is.False, "The instances should not be equivalent when the second set is the instance.");
        }

        /// <summary>
        ///   Validates behavior of the <see cref="IReadOnlyListExtensions.IsEquivalentTo(IReadOnlyList{CoinUse}, IReadOnlyList{CoinUse})" />
        ///   method.
        /// </summary>
        ///
        [Test]
        public void IsEquivalentWithEquivalentSingleItemSets()
        {
            var first = new[] { new CoinUse(5, 10) };
            var second = new[] { new CoinUse(5, 10) };

            Assert.That(first.IsEquivalentTo(second), Is.True, "The instances should be equivalent when the first set is the instance.");
            Assert.That(second.IsEquivalentTo(first), Is.True, "The instances should be equivalent when the second set is the instance.");
        }

        /// <summary>
        ///   Validates behavior of the <see cref="IReadOnlyListExtensions.IsEquivalentTo(IReadOnlyList{CoinUse}, IReadOnlyList{CoinUse})" />
        ///   method.
        /// </summary>
        ///
        [Test]
        public void IsEquivalentWithEquivalentMultipleItemSets()
        {
            var first = new[] { new CoinUse(5, 10), new CoinUse(1, 3) };
            var second = new[] { new CoinUse(5, 10), new CoinUse(1, 3) };

            Assert.That(first.IsEquivalentTo(second), Is.True, "The instances should be equivalent when the first set is the instance.");
            Assert.That(second.IsEquivalentTo(first), Is.True, "The instances should be equivalent when the second set is the instance.");
        }

        /// <summary>
        ///   Validates behavior of the <see cref="IReadOnlyListExtensions.IsEquivalentTo(IReadOnlyList{CoinUse}, IReadOnlyList{CoinUse})" />
        ///   method.
        /// </summary>
        ///
        [Test]
        public void IsEquivalentWithEquivalentSetsOrderedDifferently()
        {
            var first = new[] { new CoinUse(5, 10), new CoinUse(1, 3) };
            var second = new[] { new CoinUse(1, 3), new CoinUse(5, 10) };

            Assert.That(first.IsEquivalentTo(second), Is.True, "The instances should be equivalent when the first set is the instance.");
            Assert.That(second.IsEquivalentTo(first), Is.True, "The instances should be equivalent when the second set is the instance.");
        }
    }
}
