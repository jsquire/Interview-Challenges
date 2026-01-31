using System.Runtime.CompilerServices;
using Squire.LongestPalindromeChallenge.Infrastructure;

namespace Squire.LongestPalindromeChallenge.Strategies;

/// <summary>
///   An approach that expands around the center point of all potential palindromes should be applied.  This is a well-known
///   algorithm which balances performance and complexity.
/// </summary>
///
/// <seealso cref="Squire.LongestPalindromeChallenge.StrategyBase" />
///

internal class ExpandAroundCenterStrategy : StrategyBase
{
    /// <inheritdoc />
    public override Strategy Strategy => Strategy.ExpandAroundCenter;

    /// <inheritdoc />
    public override Palindrome? Solve(string value)
    {
        var valueSpan = value.AsSpan();

        // If the value is a valid palindrome, no further processing is required.  A
        // single-character string is is considered a valid palindrome and should be treated
        // as such.

        if ((valueSpan is { Length: 1 }) || (IsPalindrome(valueSpan)))
        {
            return new Palindrome(value, 0, value.Length);
        }

        // Track the best palindrome found so far.  At minimum, a palindrome that satisfies the
        // challenge will have at least two characters to be considered a valid result.

        var bestStart = 0;
        var bestLength = 1;

        // The expand-around-center approach leverages the fact that palindromes are symmetric around their
        // center point, so iterating through all center points and expanding out to identify palindromes avoids
        // the need to evaluate all possible substrings.
        //
        // In any string of length n, there are (2n -1) potential center points for palindromes:
        //   - n centers for odd-length palindromes (each character position)
        //   - (n - 1) centers for even-length palindromes (each gap between adjacent characters)
        //
        // For each center, expansion costs O(n) in the worst case, yielding O(n^2) total time.
        // However, in practice most centers produce short palindromes, so the average case is
        // is generally faster than the worst case.

        for (var index = 0; index < valueSpan.Length; ++index)
        {
            // Expand for odd-length palindromes.  The center is a single character at the
            // index position.

            var (oddStart, oddLength) = ExpandFromCenter(valueSpan, index, index);

            if (oddLength > bestLength)
            {
                bestStart = oddStart;
                bestLength = oddLength;
            }

            // Expand for even-length palindromes.  The center is the gap between the index position and
            // the next character.  Because there may not be a character after the index, there may not be
            // an even-length palindrome centered here.

            var centerEnd = index + 1;

            if (centerEnd < valueSpan.Length)
            {
                var (evenStart, evenLength) = ExpandFromCenter(valueSpan, index, centerEnd);

                if (evenLength > bestLength)
                {
                    bestStart = evenStart;
                    bestLength = evenLength;
                }
            }
        }

        // A palindrome must have at least two characters to be considered a valid result.
        // If no such palindrome was found, return null to indicate no solution exists.

        if (bestLength <= 1)
        {
            return default;
        }

        // Create the result by slicing the span and converting to a string.  This is the only
        // allocation in the hot path; all expansion work operates purely on the span.

        return new Palindrome(
            valueSpan.Slice(bestStart, bestLength).ToString(),
            bestStart,
            bestLength);
    }

    /// <summary>
    ///   Expands outward from the given center position to find the longest palindrome centered
    ///   at that location.
    /// </summary>
    ///
    /// <param name="value">The source character span to search within.</param>
    /// <param name="left">The left starting position of the center.</param>
    /// <param name="right">The right starting position of the center.</param>
    ///
    /// <returns>A tuple identifying the longest palindrome found centered at the specified position.</returns>
    ///
    /// <remarks>
    ///   For odd-length palindromes, <paramref name="left"/> and <paramref name="right"/>
    ///   are equal, both pointing to the single center character.  For even-length palindromes,
    ///   <paramref name="right"/> equals <paramref name="left"/> + 1, representing the gap
    ///   between two adjacent characters.
    ///
    ///   The expansion continues outward as long as:
    ///   <list type="bullet">
    ///     <item><description>The left pointer remains within bounds (>= 0).</description></item>
    ///     <item><description>The right pointer remains within bounds (less than span length).</description></item>
    ///     <item><description>The characters at the left and right positions match.</description></item>
    ///   </list>
    ///
    ///   When the loop terminates, the pointers have moved one position beyond the palindrome boundaries.
    ///   The actual palindrome spans from (left + 1) to (right - 1) inclusive, giving a length of (right - left - 1).
    ///
    ///   For example, given "racecar" and expanding from center index 3 ('e'):
    ///   <code>
    ///     Initial:  left=3, right=3  -> 'e' matches 'e' (trivially)
    ///     Step 1:   left=2, right=4  -> 'c' matches 'c'
    ///     Step 2:   left=1, right=5  -> 'a' matches 'a'
    ///     Step 3:   left=0, right=6  -> 'r' matches 'r'
    ///     Step 4:   left=-1, right=7 -> out of bounds, loop exits
    ///     Result:   start = (-1 + 1) = 0, length = (7 - (-1) - 1) = 7
    ///   </code>
    /// </remarks>
    ///
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int Start, int Length) ExpandFromCenter(ReadOnlySpan<char> value,
                                                            int left,
                                                            int right)
    {
        // Expand outward while the characters at the left and right positions match.
        // Each successful iteration extends the palindrome by two characters, one on each side.

        while ((left >= 0) && (right < value.Length) && (value[left] == value[right]))
        {
            --left;
            ++right;
        }

        // When the loop exits, left and right have moved one position beyond the palindrome
        // boundaries.  The palindrome spans from (left + 1) to (right - 1) inclusive.  Since
        // the indexes are expanded, the length calculation normalizes to (right - left - 1).

        return
        (
            Start: (left + 1),
            Length: (right - left - 1)
        );
    }
}
