using NUnit.Framework;
using Squire.LongestPalindromeChallenge.Infrastructure;

namespace Squire.LongestPalindromeChallenge.Tests.Infrastructure;

/// <summary>
///   The suite of tests for the <see cref="StrategyBase" /> interface.
/// </summary>
///
[TestFixture]
public class StrategyBaseTests
{
    /// <summary>The strategy implementation to use for testing default implementations.</summary>
    private readonly TestableStrategy TestStrategy = new TestableStrategy();

    /// <summary>
    ///   Verifies behavior of the <see cref="StrategyBase.IsPalindrome" /> method.
    /// </summary>
    ///
    [Test]
    public void EmptyStringIsPalindrome()
    {
        Assert.That(TestStrategy.IsPalindrome(""), Is.True, "An empty string should be considered a palindrome.");
    }

    /// <summary>
    ///   Verifies behavior of the <see cref="StrategyBase.IsPalindrome" /> method.
    /// </summary>
    ///
    [Test]
    public void SingleCharacterIsPalindrome()
    {
        Assert.That(TestStrategy.IsPalindrome("a"), Is.True, "A single character should be considered a palindrome.");
    }

    /// <summary>
    ///   Verifies behavior of the <see cref="StrategyBase.IsPalindrome" /> method.
    /// </summary>
    ///
    [Test]
    public void TwoIdenticalCharactersIsPalindrome()
    {
        Assert.That(TestStrategy.IsPalindrome("aa"), Is.True, "Two identical characters should be considered a palindrome.");
    }

    /// <summary>
    ///   Verifies behavior of the <see cref="StrategyBase.IsPalindrome" /> method.
    /// </summary>
    ///
    [Test]
    public void TwoDifferentCharactersIsNotPalindrome()
    {
        Assert.That(TestStrategy.IsPalindrome("ab"), Is.False, "Two different characters should not be considered a palindrome.");
    }

    /// <summary>
    ///   Verifies behavior of the <see cref="StrategyBase.IsPalindrome" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("aba")]
    [TestCase("abcba")]
    [TestCase("racecar")]
    public void OddLengthPalindromeIsRecognized(string value)
    {
        Assert.That(TestStrategy.IsPalindrome(value), Is.True, $"The odd-length palindrome `{ value }` should be recognized.");
    }

    /// <summary>
    ///   Verifies behavior of the <see cref="StrategyBase.IsPalindrome" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("abba")]
    [TestCase("abccba")]
    public void EvenLengthPalindromeIsRecognized(string value)
    {
        Assert.That(TestStrategy.IsPalindrome(value), Is.True, $"The even-length palindrome `{ value }` should be recognized.");
    }

    /// <summary>
    ///   Verifies behavior of the <see cref="StrategyBase.IsPalindrome" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("abc")]
    [TestCase("abca")]
    public void NonPalindromeIsNotRecognized(string value)
    {
        Assert.That(TestStrategy.IsPalindrome(value), Is.False, $"The non-palindrome `{ value }` should not be recognized as a palindrome.");
    }

    /// <summary>
    ///   Verifies behavior of the <see cref="StrategyBase.IsPalindrome" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("abcda")]
    [TestCase("axyza")]
    public void MiddleDoesNotMatchIsNotPalindrome(string value)
    {
        Assert.That(TestStrategy.IsPalindrome(value), Is.False, $"The value `{ value }` should not be a palindrome when only endpoints match.");
    }

    /// <summary>
    ///   Verifies behavior of the <see cref="StrategyBase.IsPalindrome" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("Aa")]
    [TestCase("Aba")]
    public void MismatchedCaseIsNotRecognized(string value)
    {
        Assert.That(TestStrategy.IsPalindrome(value), Is.False, $"The value `{ value }` should not be a palindrome due to case sensitivity.");
    }

    /// <summary>
    ///   Verifies behavior of the <see cref="StrategyBase.IsPalindrome" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("a!a")]
    [TestCase("a a")]
    public void SpecialCharactersAreRecognized(string value)
    {
        Assert.That(TestStrategy.IsPalindrome(value), Is.True, $"The value `{ value }` should be recognized as a palindrome including special characters.");
    }

    /// <summary>
    ///   Verifies behavior of the <see cref="StrategyBase.IsPalindrome" /> method.
    /// </summary>
    ///
    [Test]
    public void AllIdenticalCharactersAreRecognized()
    {
        Assert.That(TestStrategy.IsPalindrome("aaaa"), Is.True, "A string of all identical characters should be considered a palindrome.");
    }

    /// <summary>
    ///   Verifies behavior of the <see cref="StrategyBase.IsPalindrome" /> method.
    /// </summary>
    ///
    [Test]
    public void WhitespaceOnlyIsRecognized()
    {
        Assert.That(TestStrategy.IsPalindrome("   "), Is.True, "A string of whitespace characters should be considered a palindrome.");
    }

    /// <summary>
    ///   Verifies behavior of the <see cref="StrategyBase.IsPalindrome" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("121")]
    [TestCase("12321")]
    public void NumericPalindromeIsRecognized(string value)
    {
        Assert.That(TestStrategy.IsPalindrome(value), Is.True, $"The numeric palindrome `{ value }` should be recognized.");
    }

    /// <summary>
    ///   Verifies behavior of the <see cref="StrategyBase.IsPalindrome" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("a1a")]
    [TestCase("1a1")]
    [TestCase("a1b1a")]
    public void MixedAlphanumericPalindromeIsRecognized(string value)
    {
        Assert.That(TestStrategy.IsPalindrome(value), Is.True, $"The mixed alphanumeric palindrome `{ value }` should be recognized.");
    }

    /// <summary>
    ///   Verifies behavior of the <see cref="StrategyBase.IsPalindrome" /> method.
    /// </summary>
    ///
    [Test]
    public void LongPalindromeIsRecognized()
    {
        var half = new string('a', 50) + "b" + new string('c', 49);
        var palindrome = half + "X" + new string(half.Reverse().ToArray());

        Assert.That(TestStrategy.IsPalindrome(palindrome), Is.True, "A long palindrome should be recognized.");
    }

    /// <summary>
    ///   A minimal implementation of <see cref="StrategyBase" /> used to expose
    ///   the default interface methods for testing.
    /// </summary>
    ///
    private class TestableStrategy : StrategyBase
    {
        /// <summary>
        ///   The <see cref="Strategy" /> represented by this implementation.
        /// </summary>
        ///
        public override Strategy Strategy => throw new NotImplementedException();

        /// <summary>
        ///   Solves the challenge for the desired value.
        /// </summary>
        ///
        /// <param name="value">The <c>string</c> to calculate the longest palindrome substring for.</param>
        ///
        /// <returns>The longest palindrome substring found in the <paramref name="value" />.</returns>
        ///
        public override Palindrome? Solve(string value) => throw new NotImplementedException();

        /// <summary>
        ///   Determines whether the specified sequence is a palindrome.
        /// </summary>
        ///
        /// <param name="value">The value to consider.</param>
        ///
        /// <returns><c>true</c> if the specified value is palindrome; otherwise, <c>false</c>.</returns>
        ///
        public new bool IsPalindrome(ReadOnlySpan<char> value) => base.IsPalindrome(value);
    }
}
