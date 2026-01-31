namespace Squire.LongestPalindromeChallenge;

/// <summary>
///   Represents the strategy to be used for solving the minimum change problem.
/// </summary>
///
public enum Strategy
{
    /// <summary>
    ///   An approach that pre-computes candidate pairs for potential palindromes should be applied.  This is a version
    ///   of the original inspiration, optimized for greater performance and efficiency.
    /// </summary>
    ///
    CandidatePairsOptimized,

    /// <summary>
    ///   An approach that pre-computes candidate pairs for potential palindromes should be applied.  This version is not
    ///   highly optimized, staying close to the original inspiration for solving the challenge.
    /// </summary>
    ///
    CandidatePairs,

    /// <summary>
    ///   An approach that expands around the center point of all potential palindromes should be applied.  This is a well-known
    ///   algorithm which balances performance and complexity.
    /// </summary>
    ///
    ExpandAroundCenter
}
