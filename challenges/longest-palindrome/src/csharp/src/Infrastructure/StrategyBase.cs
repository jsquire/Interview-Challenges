using System.Runtime.CompilerServices;
using Squire.LongestPalindromeChallenge.Infrastructure;

namespace Squire.LongestPalindromeChallenge;

/// <summary>
///   The contract to be satisfied by a strategy implementation for
///   solving the challenge.
/// </summary>
///
internal abstract class StrategyBase
{
    /// <summary>
    ///   The <see cref="Strategy" /> represented by this implementation.
    /// </summary>
    ///
    public abstract Strategy Strategy { get; }

    /// <summary>
    ///   Solves the challenge for the desired value.
    /// </summary>
    ///
    /// <param name="value">The <c>string</c> to calculate the longest palindrome substring for.</param>
    ///
    /// <returns>
    ///   The longest palindrome substring found in the <paramref name="value" />; <c>null</c> if no palindrome was found.
    ///   In the case where multiple palindromes of equal length are found, the one which occurs first in the <paramref name="value" />
    ///   is returned.
    /// </returns>
    ///
    /// <remarks>
    ///   As an internal construct, it is expected that the parameters have already been
    ///   validated and, thus, are legal values.
    /// </remarks>
    ///
    public abstract Palindrome? Solve(string value);

    /// <summary>
    ///   Calculates the length of the printable ASCII character range, which runs from
    ///   32 (space) to 126 (~).
    /// </summary>
    ///
    /// <returns>The count of printable ASCII characters.</returns>
    ///
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int CalculatePrintableAsciiCount() => ('~' - ' ' + 1);

    /// <summary>
    ///   Determines whether the specified sequence is a palindrome.
    /// </summary>
    ///
    /// <param name="value">The value to consider.</param>
    ///
    /// <returns><c>true</c> if the specified value is palindrome; otherwise, <c>false</c>.</returns>
    ///
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static bool IsPalindrome(ReadOnlySpan<char> value)
    {
        // If there are no characters or just one character, it's a palindrome.

        if (value is { Length: <= 1 })
        {
            return true;
        }

        // For the sequence to be a be a palindrome, the characters must match their inflection
        // from each side of the sequence.  To test this, start at each end of the sequence and
        // move towards the center, comparing characters.
        //
        // If no character mismatches are found before the indexes collapse, the sequence is a
        // palindrome.  The approach of assuming a palindrome as the default case is used to avoid
        // having to do extra work for the case when the sequence has an odd length, as the exact center
        // character can be ignored.

        var startIndex = 0;
        var endIndex = value.Length - 1;

        while (startIndex < endIndex)
        {
            if (value[startIndex] != value[endIndex])
            {
                return false;
            }

            startIndex++;
            endIndex--;
        }

        return true;
    }
}
