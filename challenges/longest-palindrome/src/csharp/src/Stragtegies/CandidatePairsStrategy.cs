using Squire.LongestPalindromeChallenge.Infrastructure;

namespace Squire.LongestPalindromeChallenge.Stragtegies;

internal class CandidatePairsStrategy : StrategyBase
{
    /// <inheritdoc />
    public override Strategy Strategy => Strategy.CandidatePairs;

    /// <inheritdoc />
    public override Palindrome? Solve(string value)
    {
        throw new NotImplementedException();
    }
}
