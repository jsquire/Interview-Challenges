namespace Squire.LongestPalindromeChallenge;

/// <summary>
///   Represents the strategy to be used for solving the minimum change problem.
/// </summary>
///
public enum Strategy
{
    /// <summary>A brute force approach should be applied.</summary>
    BruteForce,

    /// <summary>An approach that pre-computes candidate pairs for potential palindromes should be applied.</summary>
    CandidatePairs
}
