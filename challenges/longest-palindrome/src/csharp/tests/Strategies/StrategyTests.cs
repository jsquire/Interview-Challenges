using NUnit.Framework;
using Squire.LongestPalindromeChallenge.Strategies;

namespace Squire.LongestPalindromeChallenge.Tests.Strategies;

/// <summary>
///   The suite of tests for verifying the correctness of <see cref="StrategyBase" />
///   implementations.
/// </summary>
///
[TestFixture]
public class StrategyTests
{
    /// <summary>
    ///   Returns all implemented strategies for data-driven tests.
    /// </summary>
    ///
    private static IEnumerable<StrategyBase> ImplementedStrategies()
    {
        foreach (var strategy in Enum.GetValues<Strategy>())
        {
            StrategyBase? implementation;

            // Some strategies may not be implemented, so they need to
            // be filtered out.

            try
            {
                implementation = EntryPoint.CreateStrategy(strategy);
            }
            catch (NotImplementedException)
            {
                continue;
            }

            yield return implementation;
        }
    }

    // ==========================================
    // Basic Recognition (simple to complex)
    // ==========================================

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void SingleCharacterReturnsThatCharacter()
    {
        var expected = "a";

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(expected);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] A single character should return a palindrome.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should be the single character.");
            Assert.That(result.StartIndex, Is.EqualTo(0), $"[{ strategy.Strategy }] The start index should be 0.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void TwoIdenticalCharactersReturnsBoth()
    {
        var expected = "aa";

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(expected);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] Two identical characters should return a palindrome.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should be both characters.");
            Assert.That(result.StartIndex, Is.EqualTo(0), $"[{ strategy.Strategy }] The start index should be 0.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void OddLengthPalindromeIsRecognized()
    {
        var expected = "aba";

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(expected);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] An odd-length palindrome should be recognized.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should match.");
            Assert.That(result.StartIndex, Is.EqualTo(0), $"[{ strategy.Strategy }] The start index should be 0.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void EvenLengthPalindromeIsRecognized()
    {
        var expected = "abba";

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(expected);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] An even-length palindrome should be recognized.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should match.");
            Assert.That(result.StartIndex, Is.EqualTo(0), $"[{ strategy.Strategy }] The start index should be 0.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void ClassicPalindromeIsRecognized()
    {
        var expected = "racecar";

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(expected);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] A classic palindrome should be recognized.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should match.");
            Assert.That(result.StartIndex, Is.EqualTo(0), $"[{ strategy.Strategy }] The start index should be 0.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void EvenPerfectPalindromeIsRecognized()
    {
        var expected = "abcddcba";

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(expected);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] A perfect even palindrome should be recognized.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should match.");
            Assert.That(result.StartIndex, Is.EqualTo(0), $"[{ strategy.Strategy }] The start index should be 0.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void AllIdenticalCharactersReturnsFullString()
    {
        var expected = "aaaa";

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(expected);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] All identical characters should return the full string.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should be the full string.");
            Assert.That(result.StartIndex, Is.EqualTo(0), $"[{ strategy.Strategy }] The start index should be 0.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    // ==========================================
    // Negative Cases (not palindromes)
    // ==========================================

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void TwoDifferentCharactersAreNotRecognized()
    {
        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve("ab");
            Assert.That(result, Is.Null, $"[{ strategy.Strategy }] Two different characters should return null.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void AllDistinctCharactersReturnsNull()
    {
        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve("abcd");
            Assert.That(result, Is.Null, $"[{ strategy.Strategy }] All distinct characters should return null.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void CaseSensitivityIsRespected()
    {
        foreach (var strategy in ImplementedStrategies())
        {
            // "Aa" is not a palindrome due to case sensitivity.

            var result = strategy.Solve("Aa");
            Assert.That(result, Is.Null, $"[{ strategy.Strategy }] Case-sensitive comparison should not match 'A' with 'a'.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void RepeatingPatternWithNoPalindromeReturnsNull()
    {
        foreach (var strategy in ImplementedStrategies())
        {
            // "abcabc" has no palindrome longer than 1 character.

            var result = strategy.Solve("abcabc");
            Assert.That(result, Is.Null, $"[{ strategy.Strategy }] A repeating pattern with no palindrome should return null.");
        }
    }

    // ==========================================
    // Embedded Palindromes
    // ==========================================

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void EmbeddedPalindromeInMiddleIsFound()
    {
        var input = "xabay";
        var expected = "aba";
        var expectedStart = 1;

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(input);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] An embedded palindrome should be found.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should match.");
            Assert.That(result.StartIndex, Is.EqualTo(expectedStart), $"[{ strategy.Strategy }] The start index should be { expectedStart }.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void EmbeddedPalindromeAtStartIsFound()
    {
        var input = "abcbaxyz";
        var expected = "abcba";
        var expectedStart = 0;

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(input);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] A palindrome at the start should be found.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should match.");
            Assert.That(result.StartIndex, Is.EqualTo(expectedStart), $"[{ strategy.Strategy }] The start index should be { expectedStart }.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void EmbeddedPalindromeAtEndIsFound()
    {
        var input = "xyzabcba";
        var expected = "abcba";
        var expectedStart = 3;

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(input);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] A palindrome at the end should be found.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should match.");
            Assert.That(result.StartIndex, Is.EqualTo(expectedStart), $"[{ strategy.Strategy }] The start index should be { expectedStart }.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    // ==========================================
    // Selection Logic (multiple candidates)
    // ==========================================

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void LongestPalindromeIsSelectedWhenMultipleExist()
    {
        var input = "forgeeksskeegfor";
        var expected = "geeksskeeg";
        var expectedStart = 3;

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(input);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] The longest palindrome should be selected.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should match the longest.");
            Assert.That(result.StartIndex, Is.EqualTo(expectedStart), $"[{ strategy.Strategy }] The start index should be { expectedStart }.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void FirstOccurrenceWinsWhenMultipleSameLengthPalindromesExist()
    {
        // Both "aba" at index 0 and "cdc" at index 4 are length 3.

        var input = "abaXcdc";
        var expected = "aba";
        var expectedStart = 0;

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(input);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] A palindrome should be found.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The first occurring palindrome should be returned.");
            Assert.That(result.StartIndex, Is.EqualTo(expectedStart), $"[{ strategy.Strategy }] The start index should be { expectedStart } for the first occurrence.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void NestedPalindromesSelectLongest()
    {
        var expected = "abacaba";

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(expected);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] The longest nested palindrome should be selected.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The full string is the longest palindrome.");
            Assert.That(result.StartIndex, Is.EqualTo(0), $"[{ strategy.Strategy }] The start index should be 0.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void OverlappingCandidatesSelectsLongest()
    {
        var expected = "aabaa";

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(expected);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] Overlapping candidates should select the longest.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The full string is the longest palindrome.");
            Assert.That(result.StartIndex, Is.EqualTo(0), $"[{ strategy.Strategy }] The start index should be 0.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    // ==========================================
    // Character Types
    // ==========================================

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase(" ")]
    [TestCase("~")]
    public void BoundaryAsciiCharactersAreRecognized(string expected)
    {
        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(expected);
            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] Boundary ASCII character '{ expected }' should return a palindrome.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void SpaceInMiddleOfPalindromeIsRecognized()
    {
        var expected = "a a";

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(expected);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] A palindrome with a space in the middle should be recognized.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should include the space.");
            Assert.That(result.StartIndex, Is.EqualTo(0), $"[{ strategy.Strategy }] The start index should be 0.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("12321")]
    [TestCase("!@#@!")]
    [TestCase("a1b1a")]
    public void NonAlphabeticCharactersFormValidPalindrome(string expected)
    {
        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(expected);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] Non-alphabetic characters should form a valid palindrome.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should match.");
        }
    }

    // ==========================================
    // Scale
    // ==========================================

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void LongConstructedOddPalindromeIsRecognized()
    {
        foreach (var strategy in ImplementedStrategies())
        {
            // Construct: 50 'a's + 'b' + 50 'a's = 101 char palindrome.

            var input = new string('a', 50) + "b" + new string('a', 50);
            var result = strategy.Solve(input);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] A long constructed odd palindrome should be recognized.");
            Assert.That(result.Value, Is.EqualTo(input), $"[{ strategy.Strategy }] The palindrome value should be the full string.");
            Assert.That(result.StartIndex, Is.EqualTo(0), $"[{ strategy.Strategy }] The start index should be 0.");
            Assert.That(result.Length, Is.EqualTo(101), $"[{ strategy.Strategy }] The length should be 101.");
        }
    }
}