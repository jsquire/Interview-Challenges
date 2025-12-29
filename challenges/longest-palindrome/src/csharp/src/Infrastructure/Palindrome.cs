namespace Squire.LongestPalindromeChallenge.Infrastructure;

/// <summary>
///   Represents a palindrome substring found within a given string.
/// </summary>
///
/// <param name="Value">The palindrome substring that was found.</param>
/// <param name="StartIndex">The starting index of the palindrome within the original string.</param>
/// <param name="Length">The length of the palindrome substring.</param>
///
/// <seealso cref="IEquatable{Palindrome}" />
///
public record Palindrome(string Value, int StartIndex, int Length)
{
}
