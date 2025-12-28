using Squire.LongestPalindromeChallenge.Infrastructure;

namespace Squire.LongestPalindromeChallenge;

/// <summary>
///   The contract to be satisfied by a strategy implementation for
///   solving the challenge.
/// </summary>
///
internal interface IStrategy
{
    /// <summary>
    ///   The <see cref="Strategy" /> represented by this implementation.
    /// </summary>
    ///
    public Strategy Strategy { get; }

    /// <summary>
    ///   Solves the challenge for the desired value.
    /// </summary>
    ///
    /// <param name="value">The <c>string</c> to calculate the longest palindrome substring for.</param>
    ///
    /// <returns>The longest palindrome substring found in the <paramref name="value" />; <c>null</c> if no palindrome was found.</returns>
    ///
    /// <remarks>
    ///   As an internal construct, it is expected that the parameters have already been
    ///   validated and, thus, are legal values.
    /// </remarks>
    ///
    public Palindrome? Solve(string value);
}
