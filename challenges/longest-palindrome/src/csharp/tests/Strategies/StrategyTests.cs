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

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void RepeatingSymbolRunAtEndIsRecognized()
    {
        var input = "baaa";
        var expected = "aaa";
        var expectedStart = 1;

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(input);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] A repeating symbol run at the end should be found.");
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
    public void RepeatingSymbolRunInMiddleIsRecognized()
    {
        var input = "baaac";
        var expected = "aaa";
        var expectedStart = 1;

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(input);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] A repeating symbol run in the middle should be found.");
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

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void RepeatingSymbolRunBeatsEmbeddedPalindrome()
    {
        var input = "aaaaaaXaba";
        var expected = "aaaaaa";
        var expectedStart = 0;

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(input);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] The longer repeating run should be selected.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should match the repeating run.");
            Assert.That(result.StartIndex, Is.EqualTo(expectedStart), $"[{ strategy.Strategy }] The start index should be { expectedStart }.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void EmbeddedPalindromeBeatsRepeatingRun()
    {
        var input = "aaYabcbaZbb";
        var expected = "abcba";
        var expectedStart = 3;

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(input);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] The longer embedded palindrome should be selected.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should match the embedded palindrome.");
            Assert.That(result.StartIndex, Is.EqualTo(expectedStart), $"[{ strategy.Strategy }] The start index should be { expectedStart }.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void MultipleRepeatingRunsSelectsLongest()
    {
        var input = "aaYbbbZcc";
        var expected = "bbb";
        var expectedStart = 3;

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(input);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] The longest repeating run should be selected.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should match the longest run.");
            Assert.That(result.StartIndex, Is.EqualTo(expectedStart), $"[{ strategy.Strategy }] The start index should be { expectedStart }.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    [Test]
    public void RepeatingRunAtStartBeatsLaterCandidate()
    {
        var input = "aaaXcbc";
        var expected = "aaa";
        var expectedStart = 0;

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(input);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] The longer repeating run at start should be selected.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should match the run at start.");
            Assert.That(result.StartIndex, Is.EqualTo(expectedStart), $"[{ strategy.Strategy }] The start index should be { expectedStart }.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    /// <remarks>
    ///   This test exercises a scenario where multiple repeating runs are superseded
    ///   by a longer candidate palindrome that spans across the runs and non-run characters.
    ///   It stresses the interaction between run tracking and candidate pair expansion.
    /// </remarks>
    ///
    [Test]
    public void CandidateSpanningMultipleRunsIsRecognized()
    {
        var input = "XYZaaWbbbWaaX";
        var expected = "aaWbbbWaa";
        var expectedStart = 3;

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(input);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] A candidate spanning multiple runs should be found.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should match the spanning candidate.");
            Assert.That(result.StartIndex, Is.EqualTo(expectedStart), $"[{ strategy.Strategy }] The start index should be { expectedStart }.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

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

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    /// <remarks>
    ///   This test targets a specific scenario where a palindrome spans from an early
    ///   character occurrence to a later run of the same character. The candidate pair
    ///   expansion must correctly calculate the length using the earlier position, not
    ///   the start of the run.
    ///
    ///   Input: "aXXXXXaaa"
    ///   - 'a' at position 0
    ///   - 'X' run at positions 1-5 (length 5)
    ///   - 'a' run at positions 6-8 (length 3)
    ///
    ///   The longest palindrome is "aXXXXXa" (positions 0-6, length 7), formed by
    ///   pairing the 'a' at position 0 with the 'a' at position 6.
    /// </remarks>
    ///
    [Test]
    public void PalindromeSpanningEarlyCharacterToLaterRunIsFound()
    {
        var input = "aXXXXXaaa";
        var expected = "aXXXXXa";
        var expectedStart = 0;

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(input);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] A palindrome spanning an early char to a later run should be found.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should match.");
            Assert.That(result.StartIndex, Is.EqualTo(expectedStart), $"[{ strategy.Strategy }] The start index should be { expectedStart }.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Solve" /> method.
    /// </summary>
    ///
    /// <remarks>
    ///   This test targets a bug in ExpandPairs where the candidate length was calculated
    ///   using the run's start index instead of the earlier position being paired with.
    ///
    ///   Input: "zXXbbbXX"
    ///   - 'z' at 0
    ///   - 'XX' at 1-2 (length 2, becomes longestPalindrome initially)
    ///   - 'bbb' at 3-5 (length 3, becomes new longestPalindrome)
    ///   - 'XX' at 6-7 (length 2, expanded via ExpandPairs at end)
    ///
    ///   The full string "zXXbbbXX" is NOT a palindrome, so early-exit doesn't trigger.
    ///
    ///   The pair (1, 7) forms "XXbbbXX" (length 7), a valid palindrome.
    ///   - At index 6: 'X' seen before at [1, 2], creates (1, 6) len 6, (2, 6) len 5
    ///   - At index 7: extends run, deferred to ExpandPairs
    ///   - ExpandPairs for (X, 6, 2): positions = [1, 2, 6, 7]
    ///     - index=7: (1, 7) should have length 7
    ///     - Bug calculates: 7 - 6 + 1 = 2 (wrong, filtered out)
    ///     - Correct: 7 - 1 + 1 = 7 (should be enqueued)
    ///
    ///   So (1, 7) is only creatable via ExpandPairs, and the bug would miss it.
    /// </remarks>
    ///
    [Test]
    public void ExpandPairsCalculatesLengthFromPositionNotRunStart()
    {
        var input = "zXXbbbXX";
        var expected = "XXbbbXX";
        var expectedStart = 1;

        foreach (var strategy in ImplementedStrategies())
        {
            var result = strategy.Solve(input);

            Assert.That(result, Is.Not.Null, $"[{ strategy.Strategy }] A palindrome with runs at both ends should be found.");
            Assert.That(result.Value, Is.EqualTo(expected), $"[{ strategy.Strategy }] The palindrome value should match.");
            Assert.That(result.StartIndex, Is.EqualTo(expectedStart), $"[{ strategy.Strategy }] The start index should be { expectedStart }.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), $"[{ strategy.Strategy }] The length should be { expected.Length }.");
        }
    }
}